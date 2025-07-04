using System.Diagnostics;
using Anexia.MathematicalProgram.Extensions;
using Anexia.MathematicalProgram.Model;
using Anexia.MathematicalProgram.Model.Interval;
using Anexia.MathematicalProgram.Model.Scalar;
using Anexia.MathematicalProgram.Model.Variable;
using Anexia.MathematicalProgram.Result;
using Anexia.MathematicalProgram.SolverConfiguration;
using Google.OrTools.ModelBuilder;
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
    /// Solves the given optimization model. Switches solver to SCIP, then the given type is not available.
    /// </summary>
    /// <param name="completedOptimizationModel">The model to be solved.</param>
    /// <param name="solverParameter">Parameters to be passed to the underlying solver.</param>
    /// <returns>Solver result containing solution information.</returns>
    public ISolverResult<IIntegerVariable<IRealScalar>, RealScalar, IRealScalar> Solve(
        ICompletedOptimizationModel<IIntegerVariable<IRealScalar>, IRealScalar, IRealScalar>
            completedOptimizationModel,
        SolverParameter solverParameter)
    {
        var (configuredSolver, solverWasSwitched) = InitializeSolver(solverParameter);

        var model = new Google.OrTools.ModelBuilder.Model();

        var variables = completedOptimizationModel.Variables.ToDictionary(
            item => item, item =>
                model.NewIntVar(item.Interval.LowerBound.Value, item.Interval.UpperBound.Value, item.Name));

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
            variables.Add(new IntegerVariable<IRealScalar>(new RealInterval(variable.LowerBound, variable.UpperBound),
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

    private (Solver configuredSolver, bool solverWasSwitched) InitializeSolver(SolverParameter solverParameter)
    {
        var configuredSolver = new Solver(SolverType.ToEnumString());
        var solverWasSwitched = false;

        if (!configuredSolver.SolverIsSupported())
        {
            Logger?.LogInformation(
                "Desired Solver {SolverType} is not supported, switching to fallback solver {FallbackSolver}",
                SolverType, FallbackSolver);
            configuredSolver = new Solver(FallbackSolver.ToEnumString());
            solverWasSwitched = true;
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

    private void ExportModelIfRequested(SolverParameter solverParameter, Google.OrTools.ModelBuilder.Model model)
    {
        if (solverParameter.ExportModelFilePath is null) return;
        Logger?.LogInformation("Exporting model to {ExportModelFilePath}", solverParameter.ExportModelFilePath);
        model.WriteToMpsFile(solverParameter.ExportModelFilePath, false);
    }
}