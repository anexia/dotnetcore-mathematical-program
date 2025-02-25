// ------------------------------------------------------------------------------------------
//  <copyright file = "Constraint.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH.All rights reserved.
//  </copyright>
//  ------------------------------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Anexia.MathematicalProgram.Model.Interval;
using Anexia.MathematicalProgram.Model.Scalar;
using Anexia.MathematicalProgram.Model.Variable;

namespace Anexia.MathematicalProgram.Model.Expression;

/// <summary>
/// Represents a constraint.
/// </summary>
/// <typeparam name="TVariable">The type of the Variable.</typeparam>
/// <typeparam name="TVariableCoefficient">The scalar type of the variable's coefficient.</typeparam>
/// <typeparam name="TInterval">The type of the interval's scalar.</typeparam>
public readonly record struct
    Constraint<TVariable, TVariableCoefficient, TInterval> :
    IConstraint<TVariable, TVariableCoefficient, TInterval>
    where TVariable : IVariable<TInterval>
    where TInterval : IAddableScalar<TInterval, TInterval>
    where TVariableCoefficient : IAddableScalar<TVariableCoefficient,TVariableCoefficient>
{
    internal Constraint(IWeightedSum<TVariable, TVariableCoefficient, TInterval> weightedSum,
        IInterval<TInterval> interval)
    {
        WeightedSum = weightedSum;
        Interval = interval;
    }

    /// <summary>
    /// The weighted sum included in the constraint.
    /// </summary>
    public IWeightedSum<TVariable, TVariableCoefficient, TInterval> WeightedSum { get; }

    /// <summary>
    /// The weighted sum's interval.
    /// </summary>
    public IInterval<TInterval> Interval { get; }

    /// <inheritdoc/>
    [ExcludeFromCodeCoverage]
    public override string ToString() => $"{Interval.LowerBound} <= {WeightedSum} <= {Interval.UpperBound}";
}