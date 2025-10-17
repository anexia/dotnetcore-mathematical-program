// ------------------------------------------------------------------------------------------
//  <copyright file = "ResultHandlingTest.cs" company = "ANEXIA Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA Internetdienstleistungs GmbH. All rights reserved.
//  </copyright>
// ------------------------------------------------------------------------------------------

using Anexia.MathematicalProgram.Model.Scalar;
using Anexia.MathematicalProgram.Model.Variable;
using Anexia.MathematicalProgram.Result;
using Anexia.MathematicalProgram.Solve;
using Google.OrTools.ModelBuilder;
using Google.OrTools.Sat;
using Gurobi;

namespace Anexia.MathematicalProgram.Tests.Result;

public sealed class ResultHandlingTest
{
    public static IEnumerable<object[]> OrToolsModelBuilderStatusMappings()
    {
        yield return [SolveStatus.INFEASIBLE, SolverResultStatus.Infeasible];
        yield return [SolveStatus.UNBOUNDED, SolverResultStatus.Unbounded];
        yield return [SolveStatus.ABNORMAL, SolverResultStatus.Abnormal];
        yield return [SolveStatus.NOT_SOLVED, SolverResultStatus.NotSolved];
        yield return [SolveStatus.MODEL_INVALID, SolverResultStatus.ModelInvalid];
        yield return [SolveStatus.MODEL_IS_VALID, SolverResultStatus.ModelIsValid];
        yield return [SolveStatus.CANCELLED_BY_USER, SolverResultStatus.CancelledByUser];
        yield return [SolveStatus.UNKNOWN_STATUS, SolverResultStatus.UnknownStatus];
        yield return [SolveStatus.INVALID_SOLVER_PARAMETERS, SolverResultStatus.InvalidSolverParameters];
        yield return [SolveStatus.SOLVER_TYPE_UNAVAILABLE, SolverResultStatus.SolverTypeUnavailable];
        yield return [SolveStatus.INCOMPATIBLE_OPTIONS, SolverResultStatus.IncompatibleOptions];
    }

    public static IEnumerable<object[]> GurobiNoSolutionStatusMappings()
    {
        yield return [GRB.Status.INFEASIBLE, SolverResultStatus.Infeasible];
        yield return [GRB.Status.UNBOUNDED, SolverResultStatus.Unbounded];
        yield return [GRB.Status.INTERRUPTED, SolverResultStatus.CancelledByUser];
        yield return [GRB.Status.INF_OR_UNBD, SolverResultStatus.InfOrUnbound];
        yield return [GRB.Status.TIME_LIMIT, SolverResultStatus.Timelimit];
    }

    public static IEnumerable<object[]> GurobiSolutionStatusMappings()
    {
        yield return [GRB.Status.OPTIMAL, SolverResultStatus.Optimal, true];
        yield return [GRB.Status.SUBOPTIMAL, SolverResultStatus.Feasible, false];
        yield return [GRB.Status.TIME_LIMIT, SolverResultStatus.Timelimit, false];
        yield return [GRB.Status.INTERRUPTED, SolverResultStatus.CancelledByUser, false];
        yield return [GRB.Status.MEM_LIMIT, SolverResultStatus.UnknownStatus, false];
    }

    [Theory]
    [MemberData(nameof(OrToolsModelBuilderStatusMappings))]
    public void HandleOrToolsModelBuilderStatusMapsNonSolutionStatuses(
        SolveStatus solveStatus,
        SolverResultStatus expectedStatus)
    {
        var result = ResultHandling.Handle<IIntegerVariable<IRealScalar>, RealScalar, IRealScalar>(
            solveStatus,
            switchedToDefaultSolver: true);

        Assert.Equal(expectedStatus, result.SolverResultStatus);
        Assert.True(result.SolutionValues.Empty);
        Assert.Null(result.ObjectiveValue);
        Assert.False(result.IsFeasible.Value);
        Assert.False(result.IsOptimal.Value);
        Assert.Null(result.OptimalityGap);
        Assert.True(result.SwitchedToDefaultSolver);
    }

    [Theory]
    [MemberData(nameof(GurobiNoSolutionStatusMappings))]
    public void HandleGurobiStatusMapsNonSolutionStatuses(int gurobiStatus, SolverResultStatus expectedStatus)
    {
        var result = ResultHandling.Handle<IIntegerVariable<IRealScalar>, RealScalar, IRealScalar>(
            gurobiStatus,
            switchedToDefaultSolver: true);

        Assert.Equal(expectedStatus, result.SolverResultStatus);
        Assert.True(result.SolutionValues.Empty);
        Assert.Null(result.ObjectiveValue);
        Assert.False(result.IsFeasible.Value);
        Assert.False(result.IsOptimal.Value);
        Assert.Null(result.OptimalityGap);
        Assert.True(result.SwitchedToDefaultSolver);
    }

    [Fact]
    public void HandleOrToolsModelBuilderOptimalStatusBuildsResultWithGap()
    {
        var result = ResultHandling.Handle<IIntegerVariable<IRealScalar>, RealScalar, IRealScalar>(
            SolveStatus.OPTIMAL,
            switchedToDefaultSolver: true,
            objectiveValue: 12,
            bestBound: 9);

        Assert.Equal(SolverResultStatus.Optimal, result.SolverResultStatus);
        Assert.Equal(new ObjectiveValue(12), result.ObjectiveValue);
        Assert.Equal(new OptimalityGap(0.25), result.OptimalityGap);
        Assert.True(result.IsFeasible.Value);
        Assert.True(result.IsOptimal.Value);
        Assert.True(result.SwitchedToDefaultSolver);
    }

    [Fact]
    public void HandleCpOptimalStatusReturnsZeroGapWhenObjectiveAndBestBoundAreZero()
    {
        var result = ResultHandling.Handle<IIntegerVariable<IRealScalar>, RealScalar, IRealScalar>(
            CpSolverStatus.Optimal,
            objectiveValue: 0,
            bestBound: 0);

        Assert.Equal(SolverResultStatus.Optimal, result.SolverResultStatus);
        Assert.Equal(new ObjectiveValue(0), result.ObjectiveValue);
        Assert.Equal(new OptimalityGap(0), result.OptimalityGap);
        Assert.True(result.IsFeasible.Value);
        Assert.True(result.IsOptimal.Value);
        Assert.False(result.SwitchedToDefaultSolver);
    }

    [Theory]
    [MemberData(nameof(GurobiSolutionStatusMappings))]
    public void HandleGurobiSolutionStatusBuildsExpectedResult(
        int gurobiStatus,
        SolverResultStatus expectedStatus,
        bool expectedOptimal)
    {
        var result = ResultHandling.HandleGurobi<IIntegerVariable<IRealScalar>, RealScalar, IRealScalar>(
            gurobiStatus,
            objectiveValue: 20,
            bestBound: 15);

        Assert.Equal(expectedStatus, result.SolverResultStatus);
        Assert.Equal(new ObjectiveValue(20), result.ObjectiveValue);
        Assert.Equal(new OptimalityGap(0.25), result.OptimalityGap);
        Assert.True(result.IsFeasible.Value);
        Assert.Equal(expectedOptimal, result.IsOptimal.Value);
        Assert.False(result.SwitchedToDefaultSolver);
    }

    [Fact]
    public void HandleGurobiUnboundedStatusBuildsNonFeasibleResultWhenBoundsArePresent()
    {
        var result = ResultHandling.HandleGurobi<IIntegerVariable<IRealScalar>, RealScalar, IRealScalar>(
            GRB.Status.UNBOUNDED,
            objectiveValue: 20,
            bestBound: 15);

        Assert.Equal(SolverResultStatus.Unbounded, result.SolverResultStatus);
        Assert.True(result.SolutionValues.Empty);
        Assert.Null(result.ObjectiveValue);
        Assert.False(result.IsFeasible.Value);
        Assert.False(result.IsOptimal.Value);
        Assert.Null(result.OptimalityGap);
        Assert.False(result.SwitchedToDefaultSolver);
    }

    [Fact]
    public void HandleOptimalStatusWithoutObjectiveValueThrows()
    {
        var exception = Assert.Throws<MathematicalProgramException>(() =>
            ResultHandling.Handle<IIntegerVariable<IRealScalar>, RealScalar, IRealScalar>(
                SolveStatus.OPTIMAL,
                switchedToDefaultSolver: false));

        Assert.Equal("Mathematical program could not be solved.", exception.Message);
    }

    [Fact]
    public void HandleGurobiSolutionStatusWithoutBestBoundThrows()
    {
        var exception = Assert.Throws<MathematicalProgramException>(() =>
            ResultHandling.HandleGurobi<IIntegerVariable<IRealScalar>, RealScalar, IRealScalar>(
                GRB.Status.OPTIMAL,
                objectiveValue: 1));

        Assert.Equal("Mathematical program could not be solved.", exception.Message);
    }

    [Fact]
    public void HandleUnknownStatusThrows()
    {
        var exception = Assert.Throws<MathematicalProgramException>(() =>
            ResultHandling.Handle<IIntegerVariable<IRealScalar>, RealScalar, IRealScalar>(
                (SolveStatus)int.MaxValue,
                switchedToDefaultSolver: false));

        Assert.Equal("Unknown result status in solver. 2147483647", exception.Message);
    }

    [Fact]
    public void HandleGurobiUnknownStatusThrows()
    {
        var exception = Assert.Throws<MathematicalProgramException>(() =>
            ResultHandling.HandleGurobi<IIntegerVariable<IRealScalar>, RealScalar, IRealScalar>(
                int.MaxValue,
                objectiveValue: 1,
                bestBound: 1));

        Assert.Equal("Unknown result status in solver. 2147483647", exception.Message);
    }
}
