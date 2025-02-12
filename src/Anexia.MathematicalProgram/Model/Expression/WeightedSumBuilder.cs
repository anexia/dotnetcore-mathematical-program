// ------------------------------------------------------------------------------------------
//  <copyright file = "CreateWeightedSumBuilder.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH. All rights reserved.
//  </copyright>
// ------------------------------------------------------------------------------------------

using Anexia.MathematicalProgram.Model.Scalar;
using Anexia.MathematicalProgram.Model.Variable;

namespace Anexia.MathematicalProgram.Model.Expression;

/// <summary>
/// Represents a builder for creating a weighted sum.
/// </summary>
/// <typeparam name="TVariable">The type of the Variable.</typeparam>
/// <typeparam name="TCoefficient">The scalar type of the variable's coefficient.</typeparam>
/// <typeparam name="TInterval">The type of the variable interval's scalar.</typeparam>
public class WeightedSumBuilder<TVariable, TCoefficient, TInterval> where TVariable : IVariable<TInterval>
    where TCoefficient : IAddableScalar<TCoefficient, TCoefficient>
    where TInterval : IAddableScalar<TInterval, TInterval>
{
    private readonly List<(TCoefficient coefficient, TVariable variable)> _variableCoefficientPairs = [];
    private readonly List<WeightedSum<TVariable, TCoefficient, TInterval>> _weightedSums = [];

    /// <summary>
    /// Adds a new term (<paramref name="coefficient"/> * <paramref name="variable"/>) to the sum.
    /// </summary>
    /// <param name="coefficient">The variable's coefficient.</param>
    /// <param name="variable">The variable.</param>
    /// <returns>The builder including the added term.</returns>
    public WeightedSumBuilder<TVariable, TCoefficient, TInterval> AddTermToSum(TCoefficient coefficient,
        TVariable variable)
    {
        _variableCoefficientPairs.Add((coefficient, variable));
        return this;
    }

    /// <summary>
    /// Adds a weighted sum of variables ( Sum(variables[i] * weights[i]) ). 
    /// </summary>
    /// <param name="variables">The variables to include in the weighted sum.</param>
    /// <param name="weights">The collection of weights corresponding to the variables.</param>
    /// <returns>The CreateWeightedSumBuilder including the added term.</returns>
    /// <exception cref="NumberOfWeightsNotEqualToNumberOfVariablesException">
    /// Thrown when the number of variables does not match the number of weights.
    /// </exception>
    public WeightedSumBuilder<TVariable, TCoefficient, TInterval> AddWeightedSum(IEnumerable<TVariable> variables,
        IEnumerable<TCoefficient> weights)
    {
        var varsArray = variables.ToArray();
        var weightsArray = weights.ToArray();
        if (!varsArray.Length.Equals(weightsArray.Length))
            throw new NumberOfWeightsNotEqualToNumberOfVariablesException(varsArray.Length, weightsArray.Length);
        for (var i = 0; i < varsArray.Length; i++)
        {
            AddTermToSum(weightsArray[i], varsArray[i]);
        }

        return this;
    }

    /// <summary>
    /// Adds another weighted sum to the sum.
    /// </summary>
    /// <param name="weightedSum">The sum to be added.</param>
    /// <returns>The builder including the added sum.</returns>
    public WeightedSumBuilder<TVariable, TCoefficient, TInterval> AddWeightedSum(
        WeightedSum<TVariable, TCoefficient, TInterval> weightedSum)
    {
        _weightedSums.Add(weightedSum);
        return this;
    }

    /// <summary>
    /// Constructs a WeightedSum object.
    /// </summary>
    /// <returns>A <see cref="WeightedSum{TVariable,TCoefficient,TInterval}"/> object.</returns>
    public WeightedSum<TVariable, TCoefficient, TInterval> Build() => _weightedSums.Aggregate(
        _variableCoefficientPairs.Aggregate(new WeightedSum<TVariable, TCoefficient, TInterval>(),
            (sum, pair) => sum.Add(pair.variable, pair.coefficient)),
        (newSum, sum) => newSum.Add(sum));
}