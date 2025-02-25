// ------------------------------------------------------------------------------------------
//  <copyright file = "IContinuousVariable.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH. All rights reserved.
//  </copyright>
// ------------------------------------------------------------------------------------------

using Anexia.MathematicalProgram.Model.Scalar;

namespace Anexia.MathematicalProgram.Model.Variable;

/// <summary>
/// Represents a continuous variable.
/// </summary>
/// <typeparam name="TIntervalValue">The type of the variable's interval.</typeparam>
public interface IContinuousVariable<out TIntervalValue> : IVariable<TIntervalValue>
    where TIntervalValue : IAddableScalar<TIntervalValue, TIntervalValue>
{
}