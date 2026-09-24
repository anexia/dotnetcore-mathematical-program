// ------------------------------------------------------------------------------------------
//  <copyright file = "VariableAttribute.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH. All rights reserved.
//  </copyright>
// ------------------------------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;

namespace Anexia.MathematicalProgram.Model.Hint;

/// <summary>
/// Represents a single solver specific attribute of a variable.
/// </summary>
/// <param name="Variable">The variable.</param>
/// <param name="Type">The type of the attribute.</param>
/// <param name="Value">The value of the attribute.</param>
/// <typeparam name="TVariable">The type of the variable.</typeparam>
public sealed record VariableAttribute<TVariable>(TVariable Variable, VariableAttributeType Type, double Value)
    : IVariableAttribute<TVariable>
{
    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public override string ToString() => $"{Variable}.{Type}={Value}";
}
