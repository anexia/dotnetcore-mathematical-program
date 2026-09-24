// ------------------------------------------------------------------------------------------
//  <copyright file = "IVariableAttribute.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH. All rights reserved.
//  </copyright>
// ------------------------------------------------------------------------------------------

namespace Anexia.MathematicalProgram.Model.Hint;

/// <summary>
/// Represents a single solver specific attribute of a variable.
/// </summary>
/// <typeparam name="TVariable">The type of the variable.</typeparam>
public interface IVariableAttribute<out TVariable>
{
    /// <summary>
    /// The variable.
    /// </summary>
    public TVariable Variable { get; }

    /// <summary>
    /// The type of the attribute.
    /// </summary>
    public VariableAttributeType Type { get; }

    /// <summary>
    /// The value of the attribute. For integral attributes, the value is truncated to an integer.
    /// </summary>
    public double Value { get; }
}
