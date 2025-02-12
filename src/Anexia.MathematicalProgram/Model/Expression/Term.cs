// ------------------------------------------------------------------------------------------
//  <copyright file = "Term.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH.All rights reserved.
//  </copyright>
//  ------------------------------------------------------------------------------------------


using System.Diagnostics.CodeAnalysis;
using Anexia.MathematicalProgram.Model.Scalar;
using Anexia.MathematicalProgram.Model.Variable;

namespace Anexia.MathematicalProgram.Model.Expression;

/// <summary>
/// Represents a term consisting of a coefficient and a variable. The value of the term is given by
/// <see cref="Coefficient"/> * <see cref="Variable"/>.
/// </summary>
/// <typeparam name="TVariable">The type of the Variable.</typeparam>
/// <typeparam name="TCoefficient">The scalar type of the variable's coefficient.</typeparam>
/// <typeparam name="TInterval">The type of the variable interval's scalar.</typeparam>
public readonly record struct Term<TVariable, TCoefficient, TInterval> :
    ITerm<TVariable, TCoefficient, TInterval> where TVariable : IVariable<TInterval>
    where TCoefficient : IAddableScalar<TCoefficient, TCoefficient>
    where TInterval : IAddableScalar<TInterval, TInterval>
{
    /// <summary>
    /// Represents a term consisting of a coefficient and a variable. The value of the Term is given by
    /// <see cref="Coefficient"/> * <see cref="Variable"/>.
    /// </summary>
    /// <param name="coefficient">The coefficient.</param>
    /// <param name="variable">The variable.</param>
    internal Term(TCoefficient coefficient, TVariable variable)
    {
        Coefficient = coefficient;
        Variable = variable;
    }

    /// <summary>
    /// The coefficient.
    /// </summary>
    public TCoefficient Coefficient { get; }

    /// <summary>
    /// The variable.
    /// </summary>
    public TVariable Variable { get; }

    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public override string ToString() =>
        $"{Coefficient}*{Variable.Name}";
}