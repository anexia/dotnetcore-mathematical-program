// ------------------------------------------------------------------------------------------
//  <copyright file = "LPSolver.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH. All rights reserved.
//  </copyright>
// ------------------------------------------------------------------------------------------

using Anexia.MathematicalProgram.Extensions;
using Anexia.MathematicalProgram.Model;
using Anexia.MathematicalProgram.Model.Scalar;
using Anexia.MathematicalProgram.Model.Variable;
using Anexia.MathematicalProgram.Result;
using Anexia.MathematicalProgram.SolverConfiguration;
using Google.OrTools.ModelBuilder;

namespace Anexia.MathematicalProgram.Solve;

/// <summary>
/// Represents a solver to solve linear programming models.
/// </summary>
public sealed class LpSolver : MemberwiseEquatable<LpSolver>,
    IOptimizationSolver<ContinuousVariable<IRealScalar>, IRealScalar, IRealScalar, RealScalar>
{
    private LpSolverType SolverType { get; }

    public LpSolver(LpSolverType solverType)
    {
        SolverType = solverType;
    }

    /// <summary>
    /// Solves the given optimization model. Switches solver to SCIP, then the given type is not available.
    /// </summary>
    /// <param name="completedOptimizationModel">The model to solve.</param>
    /// <param name="solverParameter">Additional solver parameter.</param>
    /// <returns>Solver result containing solution information.</returns>
    public ISolverResult<ContinuousVariable<IRealScalar>, RealScalar, IRealScalar> Solve(
        ICompletedOptimizationModel<ContinuousVariable<IRealScalar>, IRealScalar,
            IRealScalar> completedOptimizationModel,
        SolverParameter solverParameter)
    {
        var solver = new Solver(SolverType.ToEnumString());
        var switchedSolver = false;

        if (!solver.SolverIsSupported())
        {
            switchedSolver = true;
            solver = new Solver(IlpSolverType.Scip.ToEnumString());
        }

        if (solverParameter.TimeLimitInMilliseconds is not null)
            solver.SetTimeLimitInSeconds(solverParameter.TimeLimitInMilliseconds.AsSeconds);

        if (solverParameter.EnableSolverOutput.Value) solver.EnableOutput(true);
        solver.SetSolverSpecificParameters(solverParameter.ToSolverSpecificParameters(SolverType));

        var model = new Google.OrTools.ModelBuilder.Model();

        var variables = completedOptimizationModel.Variables.ToDictionary(item => item, item =>
            model.NewNumVar(item.Interval.LowerBound.Value, item.Interval.UpperBound.Value, item.Name));

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

        var result = solver.Solve(model);
        if (!solver.HasSolution())
            return ResultHandling.Handle<ContinuousVariable<IRealScalar>, RealScalar, IRealScalar>(result,
                switchedSolver);

        var solutionValues = new SolutionValues<ContinuousVariable<IRealScalar>, RealScalar, IRealScalar>(
            variables.ToDictionary(
                variable => variable.Key,
                variable => new RealScalar(solver.Value(variable.Value))).AsReadOnly());

        return ResultHandling.Handle(result, switchedSolver, solutionValues, solver.ObjectiveValue);
    }
}