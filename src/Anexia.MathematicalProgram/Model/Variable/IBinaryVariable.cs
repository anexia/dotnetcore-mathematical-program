// ------------------------------------------------------------------------------------------
//  <copyright file = "IBinaryVariable.cs" company = "Anexia Digital Engineering GmbH">
//  Copyright (c) Anexia Digital Engineering GmbH. All rights reserved.
//  </copyright>
// ------------------------------------------------------------------------------------------

using Anexia.MathematicalProgram.Model.Scalar;

namespace Anexia.MathematicalProgram.Model.Variable;

/// <summary>
/// Represents a binary variable (domain {0,1}).
/// </summary>
/// <typeparam name="TIntervalValue">The variable's interval scalar type.</typeparam>
public interface IBinaryVariable<out TIntervalValue> : IIntegerVariable<TIntervalValue>
    where TIntervalValue : IAddableScalar<TIntervalValue, TIntervalValue>;