// ------------------------------------------------------------------------------------------
//  <copyright file = "Term.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH.All rights reserved.
//  </copyright>
//  ------------------------------------------------------------------------------------------

#region

using Google.OrTools.LinearSolver;

#endregion

namespace Anexia.MathematicalProgram.Model;

/// <summary>
/// Represents a term consisting of a coefficient and a variable. The value of the Term is given by
/// <see cref="Coefficient"/> * <see cref="Variable"/>.
/// </summary>
/// <param name="coefficient">The coefficient.</param>
/// <param name="variable">The variable.</param>
public sealed class Term(Coefficient coefficient, Variable variable) : MemberwiseEquatable<Term>
{
    /// <summary>
    /// The coefficient.
    /// </summary>
    public Coefficient Coefficient { get; } = coefficient;

    /// <summary>
    /// The variable.
    /// </summary>
    public Variable Variable { get; } = variable;

    /// <inheritdoc />
    public override string ToString() =>
        $"{nameof(Coefficient)} * ({nameof(Variable)} Name, {nameof(Variable)} HashCode): {Coefficient} * ({Variable.Name()}, {Variable.GetHashCode()})";
}