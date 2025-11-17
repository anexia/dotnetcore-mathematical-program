// ------------------------------------------------------------------------------------------
//  <copyright file = "IntegerVariable.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH. All rights reserved.
//  </copyright>
// ------------------------------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Anexia.MathematicalProgram.Model.Interval;
using Anexia.MathematicalProgram.Model.Scalar;

namespace Anexia.MathematicalProgram.Model.Variable;

/// <summary>
/// Represents an integer variable.
/// </summary>
/// <typeparam name="TInterval">The variable's interval type.</typeparam>
public record IntegerVariable<TInterval> :
    ICreatableVariable<IntegerVariable<TInterval>, TInterval>,
    IIntegerVariable<TInterval> where TInterval : IAddableScalar<TInterval, TInterval>

{
    internal IntegerVariable(IInterval<TInterval> interval, string name)
    {
        Interval = interval;
        Name = name;
    }

    /// <summary>
    /// The interval.
    /// </summary>
    public IInterval<TInterval> Interval { get; }

    /// <summary>
    /// The name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Creates a new variable with given interval and name.
    /// </summary>
    /// <param name="interval">The interval.</param>
    /// <param name="name">The name.</param>
    /// <returns>New created variable.</returns>
    public static IntegerVariable<TInterval> Create(IInterval<TInterval> interval, string name) => new(interval, name);

    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public override string ToString() => $"{Interval.LowerBound} <= {Name} <= {Interval.UpperBound}";
}