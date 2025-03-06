using System.Collections.Immutable;
using Anexia.MathematicalProgram.Extensions;
using Anexia.MathematicalProgram.Model;
using Anexia.MathematicalProgram.Model.Scalar;
using Anexia.MathematicalProgram.Model.Variable;
using Anexia.MathematicalProgram.Result;
using Anexia.MathematicalProgram.SolverConfiguration;
using Google.OrTools.LinearSolver;
using Google.OrTools.ModelBuilder;
using Solver = Google.OrTools.LinearSolver.Solver;

namespace Anexia.MathematicalProgram.Solve;

/// <summary>
/// Represents a solver using CBC which should not be used anymore.
/// </summary>
[Obsolete("CBC Solver is not supported anymore, please switch to other Solvers.")]
public sealed class IlpCbcSolver : MemberwiseEquatable<IlpCbcSolver>,
    IOptimizationSolver<IIntegerVariable<IRealScalar>, IRealScalar, IRealScalar, RealScalar>, IDisposable
{
    private Solver Solver { get; } = Solver.CreateSolver(IlpSolverType.CbcIntegerProgramming.ToEnumString());

    /// <summary>
    /// Solves the given model.
    /// </summary>
    /// <param name="completedOptimizationModel">The model to be solved.</param>
    /// <param name="solverParameter">Parameters to be passed to the solver.</param>
    /// <returns>The result</returns>
    /// <exception cref="MathematicalProgramException">Thrown when any error occurs while solving.</exception>
    public ISolverResult<IIntegerVariable<IRealScalar>, RealScalar, IRealScalar> Solve(
        ICompletedOptimizationModel<IIntegerVariable<IRealScalar>, IRealScalar, IRealScalar>
            completedOptimizationModel,
        SolverParameter solverParameter)
    {
        var variables = completedOptimizationModel.Variables.ToDictionary(item => item, item =>
            Solver.MakeIntVar(item.Interval.LowerBound.Value, item.Interval.UpperBound.Value, item.Name));

        foreach (var constraint in completedOptimizationModel.Constraints)
        {
            var interval = constraint.Interval;
            var solverConstraint =
                Solver.MakeConstraint(interval.LowerBound.Value, interval.UpperBound.Value);

            foreach (var term in constraint.WeightedSum)
                solverConstraint.SetCoefficient(variables[term.Variable], term.Coefficient.Value);
        }

        var objective = Solver.Objective();
        objective.Clear();

        foreach (var term in completedOptimizationModel.ObjectiveFunction.WeightedSum)
            objective.SetCoefficient(variables[term.Variable], term.Coefficient.Value);

        if (completedOptimizationModel.ObjectiveFunction.Offset is not null)
            objective.SetOffset(completedOptimizationModel.ObjectiveFunction.Offset.Value);

        if (completedOptimizationModel.ObjectiveFunction.Maximize)
            objective.SetMaximization();
        else
            objective.SetMinimization();
        try
        {
            _ = Solver.SetNumThreads((int)(solverParameter.NumberOfThreads?.Value ?? 0));

            if (solverParameter.TimeLimitInMilliseconds is not null)
                Solver.SetTimeLimit(solverParameter.TimeLimitInMilliseconds.Value);
            if (solverParameter.ExportModelFilePath is not null)
                File.WriteAllText(solverParameter.ExportModelFilePath, Solver.ExportModelAsMpsFormat(true, false));

            if (solverParameter.EnableSolverOutput.Value) Solver.EnableOutput();
            using var parameter = new MPSolverParameters();

            if (solverParameter.RelativeGap is not null)
                parameter.SetDoubleParam(MPSolverParameters.DoubleParam.RELATIVE_MIP_GAP,
                    solverParameter.RelativeGap.Value);

            var resultStatus = Solver.Solve(parameter);

            return ResultHandling.Handle(Enum.Parse<SolveStatus>(resultStatus.ToString()),
                false,
                new SolutionValues<IIntegerVariable<IRealScalar>, RealScalar, IRealScalar>(
                    variables.ToDictionary(variable => variable.Key,
                        variable => new RealScalar(variable.Value.SolutionValue())).AsReadOnly()),
                Solver.Objective().Value(),
                Solver.Objective().BestBound());
        }
        catch (Exception exception)
        {
            throw new MathematicalProgramException(exception);
        }
        finally
        {
            Dispose();
        }
    }

    public void Dispose()
    {
        Solver.Clear();
        Solver.Dispose();
    }
}