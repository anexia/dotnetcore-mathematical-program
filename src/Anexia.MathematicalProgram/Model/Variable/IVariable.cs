// ------------------------------------------------------------------------------------------
//  <copyright file = "IVariable.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH. All rights reserved.
//  </copyright>
// ------------------------------------------------------------------------------------------

using Anexia.MathematicalProgram.Model.Interval;
using Anexia.MathematicalProgram.Model.Scalar;

namespace Anexia.MathematicalProgram.Model.Variable;

/// <summary>
/// Represents a variable.
/// </summary>
/// <typeparam name="TInterval">The variable's interval type.</typeparam>
public interface IVariable<out TInterval> where TInterval : IAddableScalar<TInterval, TInterval>
{
    /// <summary>
    /// The variable's interval.
    /// </summary>
    public IInterval<TInterval> Interval { get; }

    /// <summary>
    /// The variable's name.
    /// </summary>
    public string Name { get; }
}