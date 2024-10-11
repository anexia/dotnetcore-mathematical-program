// ------------------------------------------------------------------------------------------
//  <copyright file = "SolverResult.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH.All rights reserved.
//  </copyright>
//  ------------------------------------------------------------------------------------------

#region

using Google.OrTools.LinearSolver;

#endregion

namespace Anexia.MathematicalProgram.Result;

/// <summary>
/// Represents the solver's result.
/// </summary>
/// <param name="solvedSolver">The original solver used.</param>
/// <param name="objectiveValue">The objective value.</param>
/// <param name="isFeasible">Information, whether the model was feasible, or not.</param>
/// <param name="isOptimal">Information, whether the solution is optimal, or not.</param>
/// <param name="optimalityGap">The optimality gap.</param>
public sealed class SolverResult(
    Solver solvedSolver,
    ObjectiveValue objectiveValue,
    IsFeasible isFeasible,
    IsOptimal isOptimal,
    OptimalityGap optimalityGap) : MemberwiseEquatable<SolverResult>
{
    /// <summary>
    /// The original solver used.
    /// </summary>
    public Solver SolvedSolver { get; } = solvedSolver;

    /// <summary>
    /// The objective value.
    /// </summary>
    public ObjectiveValue ObjectiveValue { get; } = objectiveValue;

    /// <summary>
    /// Information, whether the model was feasible, or not.
    /// </summary>
    public IsFeasible IsFeasible { get; } = isFeasible;

    /// <summary>
    /// Information, whether the solution is optimal, or not.
    /// </summary>
    public IsOptimal IsOptimal { get; } = isOptimal;

    /// <summary>
    /// The optimality gap.
    /// </summary>
    public OptimalityGap OptimalityGap { get; } = optimalityGap;

    /// <inheritdoc />
    public override string ToString() =>
        $"{nameof(ObjectiveValue)}: {ObjectiveValue}, {nameof(IsFeasible)}: {IsFeasible}, {nameof(IsOptimal)}: {IsOptimal}, {nameof(OptimalityGap)}: {OptimalityGap}";
}