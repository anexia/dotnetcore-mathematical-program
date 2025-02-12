using Anexia.MathematicalProgram.Extensions;
using Anexia.MathematicalProgram.Model;
using Anexia.MathematicalProgram.Model.Scalar;
using Anexia.MathematicalProgram.Model.Variable;
using Anexia.MathematicalProgram.Result;
using Anexia.MathematicalProgram.SolverConfiguration;
using Google.OrTools.ModelBuilder;

namespace Anexia.MathematicalProgram.Solve;

/// <summary>
/// Represents a solver for solving ILP problems.
/// </summary>
public sealed class IlpSolver(IlpSolverType solverType) : MemberwiseEquatable<IlpSolver>,
    IOptimizationSolver<IIntegerVariable<IRealScalar>, IRealScalar, IRealScalar, RealScalar>
{
    private IlpSolverType SolverType { get; } = solverType;

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
        var switchedSolver = false;
        var solver = new Solver(SolverType.ToEnumString());

        if (!solver.SolverIsSupported())
        {
            solver = new Solver(IlpSolverType.Scip.ToEnumString());
            switchedSolver = true;
        }

        if (solverParameter.TimeLimitInMilliseconds is not null)
            solver.SetTimeLimitInSeconds(solverParameter.TimeLimitInMilliseconds.AsSeconds);

        if (solverParameter.EnableSolverOutput.Value) solver.EnableOutput(true);
        solver.SetSolverSpecificParameters(
            solverParameter.ToSolverSpecificParameters(switchedSolver ? IlpSolverType.Scip : SolverType));


        var model = new Google.OrTools.ModelBuilder.Model();

        var variables = completedOptimizationModel.Variables.ToDictionary(item => item, item =>
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
        if (solverParameter.ExportModelFilePath is not null) model.ExportToFile(solverParameter.ExportModelFilePath);

        var result = solver.Solve(model);
        if (!solver.HasSolution())
            return ResultHandling.Handle<IIntegerVariable<IRealScalar>, RealScalar, IRealScalar>(result,
                switchedSolver);

        var solutionValues = new SolutionValues<IIntegerVariable<IRealScalar>, RealScalar, IRealScalar>(
            variables.ToDictionary(
                variable => variable.Key,
                variable => new RealScalar(solver.Value(variable.Value))).AsReadOnly());

        return ResultHandling.Handle(result, switchedSolver, solutionValues, solver.ObjectiveValue,
            solver.BestObjectiveBound);
    }
}