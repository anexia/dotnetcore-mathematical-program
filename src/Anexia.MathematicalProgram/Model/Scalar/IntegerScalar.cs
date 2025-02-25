// ------------------------------------------------------------------------------------------
//  <copyright file = "IntegerScalar.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH. All rights reserved.
//  </copyright>
// ------------------------------------------------------------------------------------------


using System.Diagnostics.CodeAnalysis;

namespace Anexia.MathematicalProgram.Model.Scalar;

/// <summary>
/// Represents an integer scalar.
/// </summary>
/// <param name="Value">The value.</param>
public record IntegerScalar(long Value) : IIntegerScalar,
    IAddableScalar<IntegerScalar, IntegerScalar>
{
    /// <summary>
    /// The value represented as real scalar.
    /// </summary>
    double IRealScalar.Value => Value;

    /// <summary>
    /// Adds another integer.
    /// </summary>
    /// <param name="other">The other integer to add.</param>
    /// <returns>This + other.</returns>
    public IntegerScalar Add(IntegerScalar other) => Value + other.Value;

    /// <inheritdoc />
    public IRealScalar Add(IRealScalar other) => new RealScalar(Value + other.Value);

    /// <inheritdoc />
    public IIntegerScalar Add(IIntegerScalar other) => new IntegerScalar(Value + other.Value);

    /// <inheritdoc />
    public IRealScalar Subtract(IRealScalar scalar) => new RealScalar(Value - scalar.Value);

    /// <inheritdoc />
    public IIntegerScalar Subtract(IIntegerScalar scalar) => new IntegerScalar(Value - scalar.Value);

    /// <summary>
    /// Subtracts another integer.
    /// </summary>
    /// <param name="scalar">The other integer to subtract.</param>
    /// <returns>This - other.</returns>
    public IntegerScalar Subtract(IntegerScalar scalar) => Value - scalar.Value;

    /// <summary>
    /// Adds two integer scalars
    /// </summary>
    /// <param name="left">The lhs.</param>
    /// <param name="right">The rhs.</param>
    /// <returns>The sum left + right.</returns>
    public static IntegerScalar operator +(IntegerScalar left, IntegerScalar right) =>
        new(left.Value + right.Value);

    /// <summary>
    /// Subtracts two integer scalars
    /// </summary>
    /// <param name="left">The lhs.</param>
    /// <param name="right">The rhs.</param>
    /// <returns>The sum left - right.</returns>
    public static IntegerScalar operator -(IntegerScalar left, IntegerScalar right) =>
        new(left.Value - right.Value);

    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public override string ToString() => $"{Value:+#;-#;+0}";

    public static implicit operator IntegerScalar(long scalar) => new(scalar);
}