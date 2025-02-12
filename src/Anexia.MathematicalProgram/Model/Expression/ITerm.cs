using Anexia.MathematicalProgram.Model.Scalar;
using Anexia.MathematicalProgram.Model.Variable;

namespace Anexia.MathematicalProgram.Model.Expression;

/// <summary>
/// Represents a term consisting of a coefficient and a variable. The value of the Term is given by
/// <see cref="Coefficient"/> * <see cref="Variable"/>.
/// </summary>
/// <typeparam name="TVariable">The type of the Variable.</typeparam>
/// <typeparam name="TCoefficient">The scalar type of the variable's coefficient.</typeparam>
/// <typeparam name="TVariableInterval">The type of the variable interval's scalar.</typeparam>
public interface ITerm<out TVariable, out TCoefficient, out TVariableInterval>
    where TVariable : IVariable<TVariableInterval>
    where TCoefficient : IAddableScalar<TCoefficient, TCoefficient>
    where TVariableInterval : IAddableScalar<TVariableInterval, TVariableInterval>
{
    /// <summary>
    /// The coefficient.
    /// </summary>
    public TCoefficient Coefficient { get; }

    /// <summary>
    /// The variable.
    /// </summary>
    public TVariable Variable { get; }
}