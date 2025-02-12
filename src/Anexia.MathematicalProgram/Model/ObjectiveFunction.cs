// ------------------------------------------------------------------------------------------
//  <copyright file = "ObjectiveFunction.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH. All rights reserved.
//  </copyright>
// ------------------------------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Anexia.MathematicalProgram.Model.Expression;
using Anexia.MathematicalProgram.Model.Scalar;
using Anexia.MathematicalProgram.Model.Variable;

namespace Anexia.MathematicalProgram.Model;

/// <summary>
/// Represents an objective function (min/max offset + Sum(terms)).
/// </summary>
/// <param name="offset">An offset of the function.</param>
/// <param name="weightedSum">The terms.</param>
/// <param name="maximize">Boolean whether to maximize or minimize.</param>
/// <typeparam name="TVariable">The type of the Variable.</typeparam>
/// <typeparam name="TScalar">The scalar type of the variable's coefficient.</typeparam>
/// <typeparam name="TInterval">The type of the variable interval's scalar.</typeparam>
public sealed class ObjectiveFunction<TVariable, TScalar, TInterval>(
    TScalar? offset,
    IWeightedSum<TVariable, TScalar, TInterval> weightedSum,
    bool maximize) : MemberwiseEquatable<ObjectiveFunction<TVariable, TScalar, TInterval>>,
    IObjectiveFunction<TVariable, TScalar, TInterval>
    where TVariable : IVariable<TInterval>
    where TInterval : IAddableScalar<TInterval, TInterval>
    where TScalar : IAddableScalar<TScalar, TScalar>
{
    /// <summary>The terms.</summary>
    public IWeightedSum<TVariable, TScalar, TInterval> WeightedSum { get; } = weightedSum;

    /// <summary>Boolean whether to maximize or minimize.</summary>
    public bool Maximize { get; } = maximize;

    /// <summary>An offset of the function.</summary>
    public TScalar? Offset { get; } = offset;

    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public override string ToString() =>
        $"{(Maximize ? "max" : "min")} {(Offset is not null ? Offset : "")} {string.Join("", WeightedSum)}";
}