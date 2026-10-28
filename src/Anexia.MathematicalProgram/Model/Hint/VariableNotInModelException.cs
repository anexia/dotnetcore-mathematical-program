// ------------------------------------------------------------------------------------------
//  <copyright file = "VariableNotInModelException.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH. All rights reserved.
//  </copyright>
// ------------------------------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Anexia.MathematicalProgram.Model.Scalar;
using Anexia.MathematicalProgram.Model.Variable;

namespace Anexia.MathematicalProgram.Model.Hint;

/// <summary>
/// Thrown when a warm start or variable attribute references a variable that is not part of the model.
/// </summary>
[ExcludeFromCodeCoverage]
public sealed class VariableNotInModelException<T>(IVariable<T> variable)
    : Exception($"Variable is not part of the model: {variable}")
    where T : IAddableScalar<T, T>;
