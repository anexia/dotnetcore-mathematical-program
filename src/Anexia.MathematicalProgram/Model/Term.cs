// ------------------------------------------------------------------------------------------
//  <copyright file = "Term.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH.All rights reserved.
//  </copyright>
//  ------------------------------------------------------------------------------------------

#region

using Google.OrTools.LinearSolver;

#endregion

namespace Anexia.MathematicalProgram.Model;

public sealed class Term(Coefficient coefficient, Variable variable) : MemberwiseEquatable<Term>
{
    public Coefficient Coefficient { get; } = coefficient;
    public Variable Variable { get; } = variable;

    public override string ToString() =>
        $"{nameof(Coefficient)} * ({nameof(Variable)} Name, {nameof(Variable)} HashCode): {Coefficient} * ({Variable.Name()}, {Variable.GetHashCode()})";
}