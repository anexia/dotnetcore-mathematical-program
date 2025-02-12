// ------------------------------------------------------------------------------------------
//  <copyright file = "ICreatableVariable.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH. All rights reserved.
//  </copyright>
// ------------------------------------------------------------------------------------------

using Anexia.MathematicalProgram.Model.Interval;
using Anexia.MathematicalProgram.Model.Scalar;

namespace Anexia.MathematicalProgram.Model.Variable;

/// <summary>
/// Represents a creatable variable.
/// </summary>
/// <typeparam name="TVariable">The variable's type to be created.</typeparam>
/// <typeparam name="TIntervalValue">The variable's interval.</typeparam>
public interface ICreatableVariable<out TVariable, in TIntervalValue>
    where TIntervalValue : IAddableScalar<TIntervalValue, TIntervalValue>
{
    /// <summary>
    /// Creates a new instance of type <see cref="TVariable"/>.
    /// </summary>
    /// <param name="interval">The variable's interval</param>
    /// <param name="name">The variable's name.</param>
    /// <returns>New instance of <see cref="TVariable"/></returns>
    public static abstract TVariable Create(IInterval<TIntervalValue> interval, string name);
}