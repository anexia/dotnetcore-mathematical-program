// ------------------------------------------------------------------------------------------
//  <copyright file = "CreateConstraintBuilder.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH. All rights reserved.
//  </copyright>
// ------------------------------------------------------------------------------------------

using Anexia.MathematicalProgram.Model.Interval;
using Anexia.MathematicalProgram.Model.Scalar;
using Anexia.MathematicalProgram.Model.Variable;

namespace Anexia.MathematicalProgram.Model.Expression;

/// <summary>
/// A builder to create constraints simply.
/// </summary>
/// <typeparam name="TVariable">The type of the Variable.</typeparam>
/// <typeparam name="TCoefficient">The scalar type of the variable's coefficient.</typeparam>
/// <typeparam name="TInterval">The type of the interval's scalar.</typeparam>
public class ConstraintBuilder<TVariable, TCoefficient, TInterval> where TVariable : IVariable<TInterval>
    where TCoefficient : IAddableScalar<TCoefficient, TCoefficient>
    where TInterval : IAddableScalar<TInterval, TInterval>
{
    private readonly WeightedSumBuilder<TVariable, TCoefficient, TInterval> _weightedSumBuilder = new();

    /// <summary>
    /// Adds a new term (<paramref name="coefficient"/> * <paramref name="variable"/>)
    /// to the constraint's weighted sum.
    /// </summary>
    /// <param name="coefficient">The variable's coefficient.</param>
    /// <param name="variable">The variable.</param>
    /// <returns>The CreateConstraintBuilder including the added term.</returns>
    public ConstraintBuilder<TVariable, TCoefficient, TInterval> AddTermToSum(TCoefficient coefficient,
        TVariable variable)
    {
        _weightedSumBuilder.AddTermToSum(coefficient, variable);
        return this;
    }

    /// <summary>
    /// Adds a weighted sum of variables to the constraint.
    /// </summary>
    /// <param name="weightedSum">The sum to be added.</param>
    /// <returns>The CreateConstraintBuilder including the added sum.</returns>
    public ConstraintBuilder<TVariable, TCoefficient, TInterval> AddWeightedSum(
        WeightedSum<TVariable, TCoefficient, TInterval> weightedSum)
    {
        _weightedSumBuilder.AddWeightedSum(weightedSum);
        return this;
    }

    /// <summary>
    /// Adds a weighted sum of variables to the constraint Sum(variables[i] * weights[i]).
    /// </summary>
    /// <param name="variables">The variables to be added.</param>
    /// <param name="weights">The corresponding weights.</param>
    /// <exception cref="NumberOfWeightsNotEqualToNumberOfVariablesException">
    /// Thrown when the number of variables does not match the number of weights.
    /// </exception>
    /// <returns>The CreateConstraintBuilder including the added sum.</returns>
    public ConstraintBuilder<TVariable, TCoefficient, TInterval> AddWeightedSum(IEnumerable<TVariable> variables,
        IEnumerable<TCoefficient> weights)
    {
        _weightedSumBuilder.AddWeightedSum(variables, weights);
        return this;
    }

    /// <summary>
    /// Constructs a <see cref="Constraint{TVariable, TVariableCoefficient, TInterval}"/>
    /// using the provided interval and the sum of terms (lb ≤ terms ≤ ub).
    /// </summary>
    /// <param name="interval">The interval that defines the constraint's boundaries.</param>
    /// <returns>A newly created <see cref="Constraint{TVariable, TVariableCoefficient, TInterval}"/>
    /// containing the specified interval and the sum of terms.</returns>
    public IConstraint<TVariable, TCoefficient, TInterval> Build(IInterval<TInterval> interval) =>
        new Constraint<TVariable, TCoefficient, TInterval>(_weightedSumBuilder.Build(), interval);
}