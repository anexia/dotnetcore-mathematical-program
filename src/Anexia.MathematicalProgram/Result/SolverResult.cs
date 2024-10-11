// ------------------------------------------------------------------------------------------
//  <copyright file = "SolverResult.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH.All rights reserved.
//  </copyright>
//  ------------------------------------------------------------------------------------------

#region

using Google.OrTools.LinearSolver;

#endregion

namespace Anexia.MathematicalProgram.Result;

public sealed class SolverResult(
    Solver solvedSolver,
    ObjectiveValue objectiveValue,
    IsFeasible isFeasible,
    IsOptimal isOptimal,
    OptimalityGap optimalityGap) : MemberwiseEquatable<SolverResult>
{
    public Solver SolvedSolver { get; } = solvedSolver;
    public ObjectiveValue ObjectiveValue { get; } = objectiveValue;
    private IsFeasible IsFeasible { get; } = isFeasible;
    public IsOptimal IsOptimal { get; } = isOptimal;
    public OptimalityGap OptimalityGap { get; } = optimalityGap;

    public bool IlpIsNotFeasible() => !IsFeasible.Value;

    public override string ToString() =>
        $"{nameof(ObjectiveValue)}: {ObjectiveValue}, {nameof(IsFeasible)}: {IsFeasible}, {nameof(IsOptimal)}: {IsOptimal}, {nameof(OptimalityGap)}: {OptimalityGap}";
}