using Anexia.MathematicalProgram.Model.Interval;
using Anexia.MathematicalProgram.Model.Scalar;
using Anexia.MathematicalProgram.Model.Variable;

namespace Anexia.MathematicalProgram.Model.Expression;

/// <summary>
/// Represents a constraint.
/// </summary>
/// <typeparam name="TVariable">The type of the Variable.</typeparam>
/// <typeparam name="TCoefficient">The scalar type of the variable's coefficient.</typeparam>
/// <typeparam name="TInterval">The type of the interval's scalar.</typeparam>
public interface IConstraint<out TVariable, out TCoefficient, out TInterval> where TVariable : IVariable<TInterval>
    where TCoefficient : IAddableScalar<TCoefficient, TCoefficient>
    where TInterval : IAddableScalar<TInterval, TInterval>
{
    /// <summary>
    /// The constraint's weighted sum.
    /// </summary>
    public IWeightedSum<TVariable, TCoefficient, TInterval> WeightedSum { get; }

    /// <summary>
    /// The constraint's interval.
    /// </summary>
    public IInterval<TInterval> Interval { get; }

    /// <summary>
    /// The constraint's name.
    /// </summary>
    public string? Name { get; }
}