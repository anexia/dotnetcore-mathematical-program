using Anexia.MathematicalProgram.Extensions;
using Anexia.MathematicalProgram.Model;
using Anexia.MathematicalProgram.Model.Interval;
using Anexia.MathematicalProgram.Model.Scalar;
using Anexia.MathematicalProgram.Model.Variable;
using Anexia.MathematicalProgram.Result;
using Anexia.MathematicalProgram.SolverConfiguration;
using Google.OrTools.ModelBuilder;
using Gurobi;
using Microsoft.Extensions.Logging;

namespace Anexia.MathematicalProgram.Solve;

/// <summary>
/// Represents a solver for solving ILP problems.
/// </summary>
public sealed class IlpSolver(
    IlpSolverType solverType,
    IlpSolverType? fallbackSolver = null,
    ILogger<IlpSolver>? logger = null)
    : MemberwiseEquatable<IlpSolver>,
        IOptimizationSolver<IIntegerVariable<IRealScalar>, IRealScalar, IRealScalar, RealScalar>
{
    private IlpSolverType SolverType { get; } = solverType;
    private IlpSolverType FallbackSolver { get; } = fallbackSolver ?? IlpSolverType.HiGhs;
    private ILogger<IlpSolver>? Logger { get; } = logger;

    /// <summary>
    /// Solves the given optimization model. Switches solver to SCIP, when the given type is not available.
    /// </summary>
    /// <param name="completedOptimizationModel">The model to be solved.</param>
    /// <param name="solverParameter">Parameters to be passed to the underlying solver.</param>
    /// <returns>Solver result containing solution information.</returns>
    public ISolverResult<IIntegerVariable<IRealScalar>, RealScalar, IRealScalar> Solve(
        ICompletedOptimizationModel<IIntegerVariable<IRealScalar>, IRealScalar, IRealScalar>
            completedOptimizationModel,
        SolverParameter solverParameter)
    {
        if (SolverType == IlpSolverType.GurobiNativeIntegerProgramming)
        {
            try
            {
                return new GurobiNativeSolver(Logger).Solve(completedOptimizationModel,
                    solverParameter);
            }
            catch (MathematicalProgramException exception) when (exception.InnerException is GRBException)
            {
                if (exception.Message.Contains("No Gurobi license found"))
                {
                    Logger.LogInformation("No Gurobi licence found. Original Exception {Exception}", exception);
                    return new IlpSolver(FallbackSolver, FallbackSolver, Logger).Solve(completedOptimizationModel,
                        solverParameter);
                }

                throw;
            }
        }

        var (configuredSolver, solverWasSwitched) = InitializeSolver(solverParameter);

        if (configuredSolver is null)
        {
            return FallbackSolver == IlpSolverType.CbcIntegerProgramming
                ? new IlpCbcSolver().Solve(completedOptimizationModel, solverParameter)
                : new SolverResult<IIntegerVariable<IRealScalar>, RealScalar, IRealScalar>();
        }

        var model = new Google.OrTools.ModelBuilder.Model();

        var variables = completedOptimizationModel.Variables.ToDictionary(
            item => item, item => item switch
            {
                IntegerVariable<IRealScalar> or IntegerVariable<IIntegerScalar> or
                    IntegerVariable<RealScalar> or IntegerVariable<IntegerScalar> => model.NewIntVar(
                        item.Interval.LowerBound.Value,
                        item.Interval.UpperBound.Value, item.Name),
                BinaryVariable or IntegerVariable<IBinaryScalar> => model.NewBoolVar(item.Name),
                _ => throw new ArgumentOutOfRangeException(nameof(item), item, "Variable type not supported.")
            });

        foreach (var constraint in completedOptimizationModel.Constraints)
        {
            model.AddLinearConstraint(
                LinearExpr.Sum(constraint.WeightedSum.Select(term =>
                    LinearExpr.Term(variables[term.Variable], term.Coefficient.Value))),
                constraint.Interval.LowerBound.Value, constraint.Interval.UpperBound.Value);
        }

        model.Optimize(LinearExpr.Sum(completedOptimizationModel.ObjectiveFunction.WeightedSum.Select(term =>
                           LinearExpr.Term(variables[term.Variable], term.Coefficient.Value))) +
                       LinearExpr.Constant(completedOptimizationModel.ObjectiveFunction.Offset?.Value ?? 0),
            completedOptimizationModel.ObjectiveFunction.Maximize);

        ApplyWarmStart(completedOptimizationModel, model, variables, solverWasSwitched ? FallbackSolver : SolverType);

        ExportModelIfRequested(solverParameter, model);

        var result = configuredSolver.Solve(model);
        if (!configuredSolver.HasSolution())
            return ResultHandling.Handle<IIntegerVariable<IRealScalar>, RealScalar, IRealScalar>(result,
                solverWasSwitched);

        var solutionValues = new SolutionValues<IIntegerVariable<IRealScalar>, RealScalar, IRealScalar>(
            variables.ToDictionary(
                variable => variable.Key,
                variable => new RealScalar(configuredSolver.Value(variable.Value))).AsReadOnly());

        return ResultHandling.Handle(result, solverWasSwitched,
            solutionValues, configuredSolver.ObjectiveValue,
            configuredSolver.BestObjectiveBound);
    }

    /// <summary>
    /// Solves the given model by minimizing the objective function.
    /// </summary>
    /// <param name="modelInMpsFormat">The model to be solved in MPS format.</param>
    /// <param name="solverParameter">Parameters to be passed to the underlying solver.</param>
    /// <returns>Solver result containing solution information.</returns>
    public ISolverResult<IIntegerVariable<IRealScalar>, RealScalar, IRealScalar> Solve(
        ModelAsMpsFormat modelInMpsFormat,
        SolverParameter solverParameter)
    {
        var (configuredSolver, solverWasSwitched) = InitializeSolver(solverParameter);

        if (configuredSolver is null)
            throw new SolverNotSupportedException(SolverType, FallbackSolver);

        var model = new Google.OrTools.ModelBuilder.Model();

        model.ImportFromMpsString(modelInMpsFormat.Model);

        ExportModelIfRequested(solverParameter, model);

        var result = configuredSolver.Solve(model);
        if (!configuredSolver.HasSolution())
            return ResultHandling.Handle<IIntegerVariable<IRealScalar>, RealScalar, IRealScalar>(result,
                solverWasSwitched);

        var variables = new Dictionary<IIntegerVariable<IRealScalar>, RealScalar>();
        for (var i = 0; i < model.VariablesCount(); i++)
        {
            var variable = model.VarFromIndex(i);
            if (variable.LowerBound is 0 && variable.UpperBound is 1)
                variables.Add(new BinaryVariable(variable.Name), new RealScalar(configuredSolver.Value(variable)));
            else
                variables.Add(new IntegerVariable<IRealScalar>(
                    new RealInterval(variable.LowerBound, variable.UpperBound),
                    variable.Name), new RealScalar(configuredSolver.Value(variable)));
        }

        var solutionValues =
            new SolutionValues<IIntegerVariable<IRealScalar>, RealScalar, IRealScalar>(variables.AsReadOnly());

        return ResultHandling.Handle(result, solverWasSwitched,
            solutionValues, configuredSolver.ObjectiveValue,
            configuredSolver.BestObjectiveBound);
    }

    /// <summary>
    /// Solves the given model with default parameter.
    /// </summary>
    /// <param name="modelInMpsFormat">The model to be solved in MPS format.</param>
    /// <returns>Solver result containing solution information.</returns>
    public ISolverResult<IIntegerVariable<IRealScalar>, RealScalar, IRealScalar> Solve(
        ModelAsMpsFormat modelInMpsFormat) => Solve(modelInMpsFormat, new SolverParameter());

    private (Solver? configuredSolver, bool solverWasSwitched) InitializeSolver(SolverParameter solverParameter)
    {
        var configuredSolver = new Solver(SolverType.ToEnumString());
        var solverWasSwitched = false;

        if (!configuredSolver.SolverIsSupported())
        {
            Logger?.LogInformation(
                "Desired Solver {SolverType} is not supported, switching to fallback solver {FallbackSolver}",
                SolverType, FallbackSolver);

            solverWasSwitched = true;

            if (FallbackSolver == IlpSolverType.CbcIntegerProgramming)
                return (null, true);

            configuredSolver = new Solver(FallbackSolver.ToEnumString());
        }

        if (!configuredSolver.SolverIsSupported()) throw new SolverNotSupportedException(SolverType, FallbackSolver);

        if (solverParameter.TimeLimitInMilliseconds is not null)
            configuredSolver.SetTimeLimitInSeconds(solverParameter.TimeLimitInMilliseconds.AsSeconds);

        if (solverParameter.EnableSolverOutput.Value)
            configuredSolver.EnableOutput(true);

        var solverTypeToUse = solverWasSwitched ? FallbackSolver : SolverType;
        var solverSpecificParameters = solverParameter.ToSolverSpecificParameters(solverTypeToUse);
        configuredSolver.SetSolverSpecificParameters(solverSpecificParameters);

        Logger?.LogInformation(
            "Initialized Solver {SolverType} with TimeLimit: {TimeLimit} and solver specific parameters {SolverSpecificParameters}",
            SolverType,
            solverParameter.TimeLimitInMilliseconds is null
                ? "unbounded"
                : solverParameter.TimeLimitInMilliseconds.Value + " ms",
            solverSpecificParameters);

        return (configuredSolver, solverWasSwitched);
    }

    /// <summary>
    /// Passes the warm start as solution hint to OR-Tools, which forwards it to the underlying solver
    /// (MIP start for Gurobi, partial solution for SCIP). Variable attributes are only supported by the
    /// native Gurobi solver and are therefore ignored.
    /// </summary>
    /// <remarks>
    /// Hints are not passed to HiGHS: OR-Tools (up to and including 9.14) allocates the hint arrays for HiGHS with
    /// the wrong size (<c>std::vector(0, num_hints)</c>) and writes out of bounds, which corrupts the heap and
    /// crashes the process, see <c>ortools/linear_solver/proto_solver/highs_proto_solver.cc</c>.
    /// </remarks>
    private void ApplyWarmStart(
        ICompletedOptimizationModel<IIntegerVariable<IRealScalar>, IRealScalar, IRealScalar> completedOptimizationModel,
        Google.OrTools.ModelBuilder.Model model,
        IReadOnlyDictionary<IIntegerVariable<IRealScalar>, Variable> variables,
        IlpSolverType effectiveSolverType)
    {
        if (completedOptimizationModel.VariableAttributes is not null)
        {
            Logger?.LogWarning(
                "Variable attributes are only supported by the native Gurobi solver, ignoring {Count} attributes",
                completedOptimizationModel.VariableAttributes.Count);
        }

        if (completedOptimizationModel.WarmStart is null) return;

        if (effectiveSolverType == IlpSolverType.HiGhs)
        {
            Logger?.LogWarning(
                "Solution hints are not supported for HiGHS via OR-Tools, ignoring warm start for {Count} variables",
                completedOptimizationModel.WarmStart.Count);
            return;
        }

        Logger?.LogInformation("Setting solution hint for {Count} variables", completedOptimizationModel.WarmStart.Count);

        foreach (var startValue in completedOptimizationModel.WarmStart)
            model.AddHint(variables[startValue.Variable], startValue.Value.Value);
    }

    private void ExportModelIfRequested(SolverParameter solverParameter, Google.OrTools.ModelBuilder.Model model)
    {
        if (!solverParameter.ExportModelFilePaths.Any()) return;
        Logger?.LogInformation("Exporting model to {ExportModelFilePath}",
            string.Join(", ", solverParameter.ExportModelFilePaths));

        if (solverParameter.ExportModelFilePaths.SingleOrDefault(item => item.EndsWith(".mps")) is not null)
        {
            model.WriteToMpsFile(solverParameter.ExportModelFilePaths.SingleOrDefault(item => item.EndsWith(".mps")),
                false);
        }

        foreach (var modelFilePath in solverParameter.ExportModelFilePaths.Where(item => !item.EndsWith(".mps")))
        {
            model.ExportToFile(modelFilePath);
        }
    }
}