// ------------------------------------------------------------------------------------------
//  <copyright file = "ResultHandling.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH.All rights reserved.
//  </copyright>
//  ------------------------------------------------------------------------------------------


using System.Collections.ObjectModel;
using Anexia.MathematicalProgram.Model.Scalar;
using Anexia.MathematicalProgram.Model.Variable;
using Anexia.MathematicalProgram.Solve;
using Google.OrTools.ModelBuilder;
using Google.OrTools.Sat;
using Gurobi;

namespace Anexia.MathematicalProgram.Result;

internal static class ResultHandling
{
    internal static ISolverResult<TVariable, TCoefficient, TVariableInterval>
        Handle<TVariable, TCoefficient, TVariableInterval>(SolveStatus resultStatus,
            bool switchedToDefaultSolver,
            ISolutionValues<TVariable, TCoefficient, TVariableInterval>? solutionValues = null,
            double? objectiveValue = null,
            double? bestBound = null) where TVariable : IVariable<TVariableInterval>
        where TVariableInterval : IAddableScalar<TVariableInterval, TVariableInterval>
    {
        return resultStatus switch
        {
            SolveStatus.OPTIMAL => objectiveValue is null
                ? throw new MathematicalProgramException("Mathematical program could not be solved.")
                : SolverResult(SolverResultStatus.Optimal, switchedToDefaultSolver, solutionValues, objectiveValue,
                    bestBound, true, true),
            SolveStatus.FEASIBLE => objectiveValue is null
                ? throw new MathematicalProgramException("Mathematical program could not be solved.")
                : SolverResult(SolverResultStatus.Feasible, switchedToDefaultSolver, solutionValues, objectiveValue,
                    bestBound, true),
            SolveStatus.INFEASIBLE => SolverResult<TVariable, TCoefficient, TVariableInterval>(
                SolverResultStatus.Infeasible, switchedToDefaultSolver),
            SolveStatus.UNBOUNDED => SolverResult<TVariable, TCoefficient, TVariableInterval>(
                SolverResultStatus.Unbounded, switchedToDefaultSolver),
            SolveStatus.ABNORMAL => SolverResult<TVariable, TCoefficient, TVariableInterval>(
                SolverResultStatus.Abnormal, switchedToDefaultSolver),
            SolveStatus.NOT_SOLVED => SolverResult<TVariable, TCoefficient, TVariableInterval>(
                SolverResultStatus.NotSolved, switchedToDefaultSolver),
            SolveStatus.MODEL_INVALID => SolverResult<TVariable, TCoefficient, TVariableInterval>(
                SolverResultStatus.ModelInvalid, switchedToDefaultSolver),
            SolveStatus.MODEL_IS_VALID => SolverResult<TVariable, TCoefficient, TVariableInterval>(
                SolverResultStatus.ModelIsValid, switchedToDefaultSolver),
            SolveStatus.CANCELLED_BY_USER => SolverResult<TVariable, TCoefficient, TVariableInterval>(
                SolverResultStatus.CancelledByUser, switchedToDefaultSolver),
            SolveStatus.UNKNOWN_STATUS => SolverResult<TVariable, TCoefficient, TVariableInterval>(
                SolverResultStatus.UnknownStatus, switchedToDefaultSolver),
            SolveStatus.INVALID_SOLVER_PARAMETERS => SolverResult<TVariable, TCoefficient, TVariableInterval>(
                SolverResultStatus.InvalidSolverParameters, switchedToDefaultSolver),
            SolveStatus.SOLVER_TYPE_UNAVAILABLE => SolverResult<TVariable, TCoefficient, TVariableInterval>(
                SolverResultStatus.SolverTypeUnavailable, switchedToDefaultSolver),
            SolveStatus.INCOMPATIBLE_OPTIONS => SolverResult<TVariable, TCoefficient, TVariableInterval>(
                SolverResultStatus.IncompatibleOptions, switchedToDefaultSolver),
            _ => throw new MathematicalProgramException($"Unknown result status in solver. {resultStatus}")
        };
    }

    internal static ISolverResult<TVariable, TCoefficient, TVariableInterval>
        Handle<TVariable, TCoefficient, TVariableInterval>(int resultStatus,
            bool switchedToDefaultSolver,
            ISolutionValues<TVariable, TCoefficient, TVariableInterval>? solutionValues = null,
            double? objectiveValue = null,
            double? bestBound = null) where TVariable : IVariable<TVariableInterval>
        where TVariableInterval : IAddableScalar<TVariableInterval, TVariableInterval>
    {
        return resultStatus switch
        {
            GRB.Status.OPTIMAL => objectiveValue is null
                ? throw new MathematicalProgramException("Mathematical program could not be solved.")
                : SolverResult(SolverResultStatus.Optimal, switchedToDefaultSolver, solutionValues, objectiveValue,
                    bestBound, true, true),
            GRB.Status.INFEASIBLE => SolverResult<TVariable, TCoefficient, TVariableInterval>(
                SolverResultStatus.Infeasible, switchedToDefaultSolver),
            GRB.Status.UNBOUNDED => SolverResult<TVariable, TCoefficient, TVariableInterval>(
                SolverResultStatus.Unbounded, switchedToDefaultSolver),
            GRB.Status.INTERRUPTED => SolverResult<TVariable, TCoefficient, TVariableInterval>(
                SolverResultStatus.CancelledByUser, switchedToDefaultSolver),
            GRB.Status.INF_OR_UNBD => SolverResult<TVariable, TCoefficient, TVariableInterval>(
                SolverResultStatus.InfOrUnbound, switchedToDefaultSolver),
            GRB.Status.TIME_LIMIT => SolverResult<TVariable, TCoefficient, TVariableInterval>(
                SolverResultStatus.Timelimit, switchedToDefaultSolver),
            _ => throw new MathematicalProgramException($"Unknown result status in solver. {resultStatus}")
        };
    }

    internal static ISolverResult<TVariable, TCoefficient, TVariableInterval> Handle<TVariable, TCoefficient,
        TVariableInterval>(CpSolverStatus resultStatus,
        ISolutionValues<TVariable, TCoefficient, TVariableInterval>? solutionValues = null,
        double? objectiveValue = null,
        double? bestBound = null) where TVariableInterval : IAddableScalar<TVariableInterval, TVariableInterval>
        where TVariable : IVariable<TVariableInterval>
    {
        return resultStatus switch
        {
            CpSolverStatus.Optimal => objectiveValue is null || bestBound is null
                ? throw new MathematicalProgramException("Mathematical program could not be solved.")
                : SolverResult(SolverResultStatus.Optimal, false, solutionValues, objectiveValue,
                    bestBound, true, true),
            CpSolverStatus.Feasible => objectiveValue is null || bestBound is null
                ? throw new MathematicalProgramException("Mathematical program could not be solved.")
                : SolverResult(SolverResultStatus.Feasible, false, solutionValues, objectiveValue,
                    bestBound, true),
            CpSolverStatus.Infeasible =>
                SolverResult<TVariable, TCoefficient, TVariableInterval>(SolverResultStatus.Infeasible,
                    false),
            CpSolverStatus.Unknown => SolverResult<TVariable, TCoefficient, TVariableInterval>(
                SolverResultStatus.UnknownStatus,
                false),
            CpSolverStatus.ModelInvalid => SolverResult<TVariable, TCoefficient, TVariableInterval>(
                SolverResultStatus.ModelInvalid,
                false),
            _ => throw new MathematicalProgramException($"Unknown result status in solver. {resultStatus}")
        };
    }

    internal static ISolverResult<TVariable, TCoefficient, TVariableInterval> HandleGurobi<TVariable, TCoefficient,
        TVariableInterval>(int resultStatus,
        ISolutionValues<TVariable, TCoefficient, TVariableInterval>? solutionValues = null,
        double? objectiveValue = null,
        double? bestBound = null) where TVariableInterval : IAddableScalar<TVariableInterval, TVariableInterval>
        where TVariable : IVariable<TVariableInterval>
    {
        return resultStatus switch
        {
            GRB.Status.OPTIMAL => objectiveValue is null || bestBound is null
                ? throw new MathematicalProgramException("Mathematical program could not be solved.")
                : SolverResult(SolverResultStatus.Optimal, false, solutionValues, objectiveValue,
                    bestBound, true, true),
            GRB.Status.SUBOPTIMAL => objectiveValue is null || bestBound is null
                ? throw new MathematicalProgramException("Mathematical program could not be solved.")
                : SolverResult(SolverResultStatus.Feasible, false, solutionValues, objectiveValue,
                    bestBound, true),
            GRB.Status.TIME_LIMIT => objectiveValue is null || bestBound is null
                ? throw new MathematicalProgramException("Mathematical program could not be solved.")
                : SolverResult(SolverResultStatus.Timelimit, false, solutionValues, objectiveValue,
                    bestBound, true),
            GRB.Status.INTERRUPTED => objectiveValue is null || bestBound is null
                ? throw new MathematicalProgramException("Mathematical program could not be solved.")
                : SolverResult(SolverResultStatus.CancelledByUser, false, solutionValues, objectiveValue,
                    bestBound, true),
            GRB.Status.MEM_LIMIT => objectiveValue is null || bestBound is null
                ? throw new MathematicalProgramException("Mathematical program could not be solved.")
                : SolverResult(SolverResultStatus.UnknownStatus, false, solutionValues, objectiveValue,
                    bestBound, true),
            GRB.Status.UNBOUNDED => objectiveValue is null || bestBound is null
                ? throw new MathematicalProgramException("Mathematical program could not be solved.")
                : SolverResult<TVariable, TCoefficient, TVariableInterval>(SolverResultStatus.Unbounded, false),

            _ => throw new MathematicalProgramException($"Unknown result status in solver. {resultStatus}")
        };
    }

    private static ISolverResult<TVariable, TCoefficient, TVariableInterval>
        SolverResult<TVariable, TCoefficient, TVariableInterval>(SolverResultStatus resultStatus,
            bool switchedToDefaultSolver,
            ISolutionValues<TVariable, TCoefficient, TVariableInterval>? solutionValues = null,
            double? objectiveValue = null, double? bestBound = null, bool isFeasible = false, bool isOptimal = false)
        where TVariable : IVariable<TVariableInterval>
        where TVariableInterval : IAddableScalar<TVariableInterval, TVariableInterval>
    {
        return new SolverResult<TVariable, TCoefficient, TVariableInterval>(
            solutionValues ??
            new SolutionValues<TVariable, TCoefficient, TVariableInterval>(ReadOnlyDictionary<TVariable, TCoefficient>
                .Empty),
            objectiveValue is null ? null : new ObjectiveValue(objectiveValue.Value),
            new IsFeasible(isFeasible),
            new IsOptimal(isOptimal),
            objectiveValue is null || bestBound is null ? null : CalculateGap(objectiveValue.Value, bestBound.Value),
            resultStatus,
            switchedToDefaultSolver);
    }

    private static OptimalityGap CalculateGap(double objectiveValue, double bestBound) =>
        bestBound is 0 && objectiveValue is 0
            ? new OptimalityGap(0)
            : new OptimalityGap(Math.Abs(bestBound - objectiveValue) / objectiveValue);
}