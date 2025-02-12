// ------------------------------------------------------------------------------------------
//  <copyright file = "Constraints.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH.All rights reserved.
//  </copyright>
//  ------------------------------------------------------------------------------------------


using System.Collections;
using System.Diagnostics.CodeAnalysis;
using Anexia.MathematicalProgram.Model.Scalar;
using Anexia.MathematicalProgram.Model.Variable;

namespace Anexia.MathematicalProgram.Model.Expression;

/// <summary>
/// Represents an immutable list of constraints.
/// </summary>
/// <param name="elements">The constraints.</param>
/// <typeparam name="TVariable">The type of the Variable.</typeparam>
/// <typeparam name="TCoefficient">The scalar type of the variable's coefficient.</typeparam>
/// <typeparam name="TInterval">The type of the interval's scalar.</typeparam>
public sealed class Constraints<TVariable, TCoefficient, TInterval>(
    IReadOnlyCollection<IConstraint<TVariable, TCoefficient, TInterval>> elements)
    : IReadOnlyCollection<IConstraint<TVariable, TCoefficient, TInterval>> where TVariable : IVariable<TInterval>
    where TCoefficient :  IAddableScalar<TCoefficient, TCoefficient>
    where TInterval : IAddableScalar<TInterval, TInterval>
{
    /// <summary>
    /// Creates a new instance of <see cref="Constraints{TVariable,TCoefficient,TInterval}"/> including the
    /// given list of constraints.
    /// </summary>
    /// <param name="elements">A list of constraints.</param>
    public Constraints(IEnumerable<IConstraint<TVariable, TCoefficient, TInterval>> elements)
        : this(elements.ToArray())
    {
    }

    /// <summary>
    /// Creates a new instance of <see cref="Constraints{TVariable,TCoefficient,TInterval}"/> including the
    /// given list of constraints.
    /// </summary>
    /// <param name="elements">A list of constraints.</param>
    public Constraints(params IConstraint<TVariable, TCoefficient, TInterval>[] elements)
        : this(elements.AsReadOnly())
    {
    }

    /// <summary>
    /// Creates a new instance of <see cref="Constraints{TVariable,TCoefficient,TInterval}"/>.
    /// </summary>
    public Constraints()
        : this([])
    {
    }

    /// <summary>
    /// Gets the number of constraints in the collection.
    /// </summary>
    public int Count => elements.Count;


    /// <summary>
    /// Adds the given constraint.
    /// </summary>
    /// <param name="constraint">The constraint to be added.</param>
    /// <returns>A new object with the constraint added.</returns>
    public Constraints<TVariable, TCoefficient, TInterval> Add(
        IConstraint<TVariable, TCoefficient, TInterval> constraint) => new(elements.Append(constraint));

    /// <inheritdoc/>
    public IEnumerator<IConstraint<TVariable, TCoefficient, TInterval>> GetEnumerator() => elements.GetEnumerator();

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc/>
    [ExcludeFromCodeCoverage]
    public override string ToString() => string.Join(",", elements);
}