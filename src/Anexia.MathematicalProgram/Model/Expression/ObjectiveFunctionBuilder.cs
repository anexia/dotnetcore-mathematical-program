// ------------------------------------------------------------------------------------------
//  <copyright file = "CreateObjectiveFunctionBuilder.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH. All rights reserved.
//  </copyright>
// ------------------------------------------------------------------------------------------

using Anexia.MathematicalProgram.Model.Scalar;
using Anexia.MathematicalProgram.Model.Variable;

namespace Anexia.MathematicalProgram.Model.Expression;

/// <summary>
/// A builder to create an objective function.
/// </summary>
/// <typeparam name="TVariable">The type of the Variable.</typeparam>
/// <typeparam name="TCoefficient">The scalar type of the variable's coefficient.</typeparam>
/// <typeparam name="TInterval">The type of the interval's scalar.</typeparam>
public class ObjectiveFunctionBuilder<TVariable, TCoefficient, TInterval> where TVariable : IVariable<TInterval>
    where TCoefficient :  IAddableScalar<TCoefficient, TCoefficient>
    where TInterval : IAddableScalar<TInterval, TInterval>
{
    private readonly WeightedSumBuilder<TVariable, TCoefficient, TInterval> _weightedSumBuilder = new();

    /// <summary> 
    /// Adds a new term (<paramref name="coefficient"/> * <paramref name="variable" />) to the objective function's sum of terms.
    /// </summary>
    /// <param name="coefficient">The variable's coefficient.</param>
    /// <param name="variable">The variable.</param>
    /// <returns>The CreateObjectiveFunctionBuilder including the added term.</returns>
    public ObjectiveFunctionBuilder<TVariable, TCoefficient, TInterval> AddTermToSum(TCoefficient coefficient,
        TVariable variable)
    {
        _weightedSumBuilder.AddTermToSum(coefficient, variable);
        return this;
    }

    /// <summary>
    /// Adds a weighted sum of variables to the constraint Sum(variables[i] * weights[i]). 
    /// </summary>
    /// <param name="weightedSum">The sum to be added.</param>
    /// <returns>The CreateConstraintBuilder including the added term.</returns>
    public ObjectiveFunctionBuilder<TVariable, TCoefficient, TInterval> AddWeightedSum(
        WeightedSum<TVariable, TCoefficient, TInterval> weightedSum)
    {
        _weightedSumBuilder.AddWeightedSum(weightedSum);
        return this;
    }

    /// <summary>
    /// Constructs an <see cref="ObjectiveFunction{TVariable, TVariableCoefficient, TInterval}"/> with given properties.
    /// </summary>
    /// <param name="maximize">Whether to maximize or not, <c>true</c> by deafult.</param>
    /// <param name="offset">Optional offset to add to the sum.</param>
    /// <returns>The newly created <see cref="ObjectiveFunction{TVariable, TVariableCoefficient, TInterval}"/></returns>
    public ObjectiveFunction<TVariable, TCoefficient, TInterval> Build(bool maximize = true,
        TCoefficient? offset = default) =>
        new(offset, _weightedSumBuilder.Build(), maximize);
}