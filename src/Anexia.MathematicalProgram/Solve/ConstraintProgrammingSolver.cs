using Anexia.MathematicalProgram.Model;
using Anexia.MathematicalProgram.Model.Scalar;
using Anexia.MathematicalProgram.Model.Variable;
using Anexia.MathematicalProgram.Result;
using Anexia.MathematicalProgram.SolverConfiguration;
using Google.OrTools.Sat;

namespace Anexia.MathematicalProgram.Solve;

/// <summary>
/// Represents a CP solver for solving models with integer variables and coefficients by using the CP-SAT solver from
/// <see href="https://developers.google.com/optimization/cp/cp_solver"/>.
/// </summary>
public sealed class ConstraintProgrammingSolver
    : MemberwiseEquatable<ConstraintProgrammingSolver>,
        IOptimizationSolver<IIntegerVariable<IIntegerScalar>, IIntegerScalar, IIntegerScalar, IntegerScalar>
{
    /// <summary>
    /// Solves the given model.
    /// </summary>
    /// <param name="completedOptimizationModel">The model to be solved.</param>
    /// <param name="solverParameter">Parameters to be passed to the original solver.</param>
    /// <returns>The result.</returns>
    public ISolverResult<IIntegerVariable<IIntegerScalar>, IntegerScalar, IIntegerScalar> Solve(
        ICompletedOptimizationModel<IIntegerVariable<IIntegerScalar>, IIntegerScalar, IIntegerScalar>
            completedOptimizationModel,
        SolverParameter solverParameter) => Solve(completedOptimizationModel, solverParameter, null);

    /// <summary>
    /// Solves the given model.
    /// </summary>
    /// <param name="completedOptimizationModel">The model to be solved.</param>
    /// <param name="solverParameter">Parameters to be passed to the original solver.</param>
    /// <param name="solutionCallback">Callback that is called when a solution is found.</param>
    /// <param name="optimize">When false, no objective function is added and feasible solutions are enumerated.
    /// When true, maximizes or minimizes as defined in the objective function</param>
    /// <returns>The result.</returns>
    public ISolverResult<IIntegerVariable<IIntegerScalar>, IntegerScalar, IIntegerScalar> Solve(
        ICompletedOptimizationModel<IIntegerVariable<IIntegerScalar>, IIntegerScalar, IIntegerScalar>
            completedOptimizationModel,
        SolverParameter solverParameter, ICpSolutionCallback? solutionCallback, bool optimize = true)
    {
        var model = new CpModel();
        var variables = completedOptimizationModel.Variables.ToDictionary(item => item,
            item => item switch
            {
                IntegerVariable<IIntegerScalar> or IntegerVariable<IntegerScalar> => model.NewIntVar(
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

        var expr = LinearExpr.Sum(completedOptimizationModel.ObjectiveFunction.WeightedSum.Select(term =>
                       LinearExpr.Term(variables[term.Variable], term.Coefficient.Value))) +
                   LinearExpr.Constant(completedOptimizationModel.ObjectiveFunction.Offset?.Value ?? 0);
        if (optimize)
        {
            if (completedOptimizationModel.ObjectiveFunction.Maximize) model.Maximize(expr);
            else model.Minimize(expr);
        }

        if (solverParameter.ExportModelFilePath is not null) model.ExportToFile(solverParameter.ExportModelFilePath);

        var solver = new CpSolver();
        if (solverParameter.TimeLimitInMilliseconds is not null)
            solver.StringParameters += $"max_time_in_seconds:{solverParameter.TimeLimitInMilliseconds.AsSeconds}";

        if (solutionCallback is not null)
        {
            solver.StringParameters += "enumerate_all_solutions:true";
        }

        using var sc = new ObjectiveSolutionPrinter();

        var result = solver.Solve(model,
            solutionCallback is null ? null : new CustomSolutionCallback(solutionCallback, variables));

        if (result is not (CpSolverStatus.Feasible or CpSolverStatus.Optimal))
            return ResultHandling.Handle<IIntegerVariable<IIntegerScalar>, IntegerScalar, IIntegerScalar>(result);

        var solutionValues = new SolutionValues<IIntegerVariable<IIntegerScalar>, IntegerScalar, IIntegerScalar>(
            variables.ToDictionary(
                variable => variable.Key,
                variable => new IntegerScalar(solver.Value(variable.Value))).AsReadOnly());

        return ResultHandling.Handle(result, solutionValues, solver.ObjectiveValue, solver.BestObjectiveBound);
    }
}