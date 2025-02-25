// ------------------------------------------------------------------------------------------
//  <copyright file = "ContinuousVariable.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH. All rights reserved.
//  </copyright>
// ------------------------------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Anexia.MathematicalProgram.Model.Interval;
using Anexia.MathematicalProgram.Model.Scalar;

namespace Anexia.MathematicalProgram.Model.Variable;

/// <summary>
/// Represents a continuous variable.
/// </summary>
/// <typeparam name="T">The type of the interval's scalar.</typeparam>
public sealed class ContinuousVariable<T> : MemberwiseEquatable<ContinuousVariable<T>>, IContinuousVariable<T>,
    ICreatableVariable<ContinuousVariable<T>, T> where T : IAddableScalar<T, T>
{
    internal ContinuousVariable(IInterval<T> interval, string name)
    {
        Interval = interval;
        Name = name;
    }

    /// <summary>
    /// The variable's interval.
    /// </summary>
    public IInterval<T> Interval { get; }

    /// <summary>
    /// The variable's name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Creates a new variable with given interval and name.
    /// </summary>
    /// <param name="interval">The interval.</param>
    /// <param name="name">The name.</param>
    /// <returns>New created variable.</returns>
    public static ContinuousVariable<T> Create(IInterval<T> interval, string name) => new(interval, name);

    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public override string ToString() => $"{Interval.LowerBound} <= {Name} <= {Interval.UpperBound}";
}