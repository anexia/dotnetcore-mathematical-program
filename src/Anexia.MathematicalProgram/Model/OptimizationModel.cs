// ------------------------------------------------------------------------------------------
//  <copyright file = "OptimizationModel.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH. All rights reserved.
//  </copyright>
// ------------------------------------------------------------------------------------------

using System.Collections.Immutable;
using Anexia.MathematicalProgram.Model.Expression;
using Anexia.MathematicalProgram.Model.Interval;
using Anexia.MathematicalProgram.Model.Scalar;
using Anexia.MathematicalProgram.Model.Variable;

namespace Anexia.MathematicalProgram.Model;

/// <summary>
/// Represents an optimization model.
/// </summary>
/// <typeparam name="TVariable">The type of the Variable.</typeparam>
/// <typeparam name="TCoefficient">The scalar type of the variable's coefficient.</typeparam>
/// <typeparam name="TInterval">The type of the variable and constraint interval's scalar.</typeparam>
public readonly record struct
    OptimizationModel<TVariable, TCoefficient, TInterval>
    where TVariable : IVariable<TInterval>
    where TCoefficient : IAddableScalar<TCoefficient, TCoefficient>
    where TInterval : IAddableScalar<TInterval, TInterval>
{
    private readonly HashSet<TVariable> _variables;
    private readonly List<IConstraint<TVariable, TCoefficient, TInterval>> _constraints;

    internal OptimizationModel(HashSet<TVariable> variables,
        List<IConstraint<TVariable, TCoefficient, TInterval>> constraints)
    {
        _variables = variables;
        _constraints = constraints;
    }

    public OptimizationModel() : this(new HashSet<TVariable>(),
        new List<IConstraint<TVariable, TCoefficient, TInterval>>())
    {
    }

    /// <summary>
    /// Creates a new variable of type <see cref="TCreatedVariable"/> with given interval and name.
    /// </summary>
    /// <param name="interval">The interval.</param>
    /// <param name="name">The name.</param>
    /// <typeparam name="TCreatedVariable">The desired type of the variable.</typeparam>
    /// <returns>The newly created variable.</returns>
    /// <exception cref="VariableAlreadyExistsException{TInterval}">Thrown when a variable with the same name already exists.</exception>
    public TCreatedVariable NewVariable<TCreatedVariable>(IInterval<TInterval> interval, string name)
        where TCreatedVariable : ICreatableVariable<TCreatedVariable, TInterval>, TVariable
    {
        var variable = TCreatedVariable.Create(interval, name);
        if (!_variables.Add(variable))
            throw new VariableAlreadyExistsException<TInterval>(variable);

        return variable;
    }

    /// <summary>
    /// Adds a new constraint to the model.
    /// </summary>
    /// <param name="constraint">The constraint to be added.</param>
    public void AddConstraint(IConstraint<TVariable, TCoefficient, TInterval> constraint)
    {
        _constraints.Add(constraint);
    }

    /// <summary>
    /// Adds constraints to the model.
    /// </summary>
    /// <param name="constraints">The constraints to be added.</param>
    public void AddConstraints(Constraints<TVariable, TCoefficient, TInterval> constraints)
    {
        _constraints.AddRange(constraints);
    }

    /// <summary>
    /// Sets the objective function of the model.
    /// </summary>
    /// <param name="objectiveFunction">The objective function to be added.</param>
    /// <returns>Returns an instance of <see cref="CompletedOptimizationModel{TVariable,TCoefficient,TInterval}"/> that
    /// can be passed to a solver.</returns>
    public ICompletedOptimizationModel<TVariable, TCoefficient, TInterval> SetObjective(
        IObjectiveFunction<TVariable, TCoefficient, TInterval> objectiveFunction) =>
        new CompletedOptimizationModel<TVariable, TCoefficient, TInterval>(
            new Variables<TVariable, TInterval>([.. _variables]),
            new Constraints<TVariable, TCoefficient, TInterval>(_constraints),
            objectiveFunction);

    /// <summary>
    /// Creates a new instance of <see cref="CreateWeightedSumBuilder"/>
    /// </summary>
    public WeightedSumBuilder<TVariable, TCoefficient, TInterval> CreateWeightedSumBuilder() => new();

    /// <summary>
    /// Creates a new instance of <see cref="CreateConstraintBuilder"/>
    /// </summary>
    public ConstraintBuilder<TVariable, TCoefficient, TInterval> CreateConstraintBuilder() => new();

    /// <summary>
    /// Creates a new instance of <see cref="CreateObjectiveFunctionBuilder"/>
    /// </summary>
    public ObjectiveFunctionBuilder<TVariable, TCoefficient, TInterval> CreateObjectiveFunctionBuilder() => new();
}