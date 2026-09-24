// ------------------------------------------------------------------------------------------
//  <copyright file = "IVariableAttributes.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH. All rights reserved.
//  </copyright>
// ------------------------------------------------------------------------------------------

namespace Anexia.MathematicalProgram.Model.Hint;

/// <summary>
/// Represents a collection of solver specific variable attributes, see <see cref="VariableAttributeType"/>.
/// Every combination of variable and attribute type occurs at most once.
/// </summary>
/// <typeparam name="TVariable">The type of the variable.</typeparam>
public interface IVariableAttributes<out TVariable> : IReadOnlyCollection<IVariableAttribute<TVariable>>
{
}
