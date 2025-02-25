// ------------------------------------------------------------------------------------------
//  <copyright file = "IIntegerVariable.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH. All rights reserved.
//  </copyright>
// ------------------------------------------------------------------------------------------


using Anexia.MathematicalProgram.Model.Scalar;

namespace Anexia.MathematicalProgram.Model.Variable;

/// <summary>
/// Represents an integer variable.
/// </summary>
/// <typeparam name="TIntervalValue">The variable's interval type.</typeparam>
public interface IIntegerVariable<out TIntervalValue> : IVariable<TIntervalValue>
    where TIntervalValue : IAddableScalar<TIntervalValue, TIntervalValue>
{
}