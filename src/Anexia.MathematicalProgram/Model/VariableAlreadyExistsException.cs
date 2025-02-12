// ------------------------------------------------------------------------------------------
//  <copyright file = "VariableAlreadyExistsException.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH. All rights reserved.
//  </copyright>
// ------------------------------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Anexia.MathematicalProgram.Model.Scalar;
using Anexia.MathematicalProgram.Model.Variable;

namespace Anexia.MathematicalProgram.Model;

[ExcludeFromCodeCoverage]
public sealed class VariableAlreadyExistsException<T>(IVariable<T> variable)
    : Exception($"Variable with same name already exists: {variable}")
    where T : IAddableScalar<T, T>;