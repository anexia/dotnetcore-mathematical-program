// ------------------------------------------------------------------------------------------
//  <copyright file = "ResultHandling.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH.All rights reserved.
//  </copyright>
//  ------------------------------------------------------------------------------------------

#region

using Anexia.MathematicalProgram.Solve;
using Google.OrTools.LinearSolver;

#endregion

namespace Anexia.MathematicalProgram.Result;

internal sealed class ResultHandling(Solver solver) : MemberwiseEquatable<ResultHandling>
{
    private Solver Solver { get; } = solver;

    internal SolverResult Handle(Solver.ResultStatus resultStatus)
    {
        switch (resultStatus)
        {
            case Solver.ResultStatus.OPTIMAL:
                return new SolverResult(Solver,
                    new ObjectiveValue(Solver.Objective().Value()),
                    new IsFeasible(true),
                    new IsOptimal(true),
                    new OptimalityGap(0));
            case Solver.ResultStatus.FEASIBLE:
                var objective = Solver.Objective();
                var objectiveValue = objective.Value();

                return new SolverResult(Solver,
                    new ObjectiveValue(objectiveValue),
                    new IsFeasible(true),
                    new IsOptimal(false),
                    new OptimalityGap(Math.Abs(objective.BestBound() - objectiveValue) / objectiveValue));
            case Solver.ResultStatus.INFEASIBLE:
                return new SolverResult(Solver,
                    new ObjectiveValue(double.NaN),
                    new IsFeasible(false),
                    new IsOptimal(false),
                    new OptimalityGap(double.NaN));
            case Solver.ResultStatus.UNBOUNDED:
                throw new MathematicalProgramException("Mathematical program is unbounded.");
            case Solver.ResultStatus.ABNORMAL:
                throw new MathematicalProgramException("Mathematical program is abnormal (probably numerical error).");
            case Solver.ResultStatus.NOT_SOLVED:
                throw new MathematicalProgramException("Mathematical program could not be diagnosed and solved.");
            case Solver.ResultStatus.MODEL_INVALID:
                throw new MathematicalProgramException("Mathematical model is not valid.");
            default:
                throw new MathematicalProgramException("Unknown result status in linear solver.");
        }
    }

    /// <inheritdoc />
    public override string ToString() => $"{nameof(Solver)}: {Solver}";
}