// ------------------------------------------------------------------------------------------
//  <copyright file = "RealScalar.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH.All rights reserved.
//  </copyright>
//  ------------------------------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;

namespace Anexia.MathematicalProgram.Model.Scalar;

/// <summary>
/// Represents a real scalar.
/// </summary>
/// <param name="Value">The scalar's value.</param>
public record RealScalar(double Value) : IRealScalar,
    IAddableScalar<RealScalar, RealScalar>
{
    /// <summary>
    /// Adds a real scalar.
    /// </summary>
    /// <param name="other">The scalar to add.</param>
    /// <returns>The result of the addition.</returns>
    public RealScalar Add(RealScalar other) => Value + other.Value;

    /// <inheritdoc />
    public IRealScalar Add(IRealScalar other) => new RealScalar(Value + other.Value);

    /// <inheritdoc />
    public IRealScalar Subtract(IRealScalar scalar) => new RealScalar(Value - scalar.Value);

    /// <inheritdoc />
    public RealScalar Subtract(RealScalar scalar) => Value - scalar.Value;

    /// <summary>
    /// Adds two real scalars.
    /// </summary>
    /// <param name="left">The lhs.</param>
    /// <param name="right">The rhs.</param>
    /// <returns>The sum left + right.</returns>
    public static RealScalar operator +(RealScalar left, RealScalar right) => new(left.Value + right.Value);

    /// <summary>
    /// Subtracts two real scalars.
    /// </summary>
    /// <param name="left">The lhs.</param>
    /// <param name="right">The rhs.</param>
    /// <returns>The sum left - right.</returns>
    public static RealScalar operator -(RealScalar left, RealScalar right) => new(left.Value - right.Value);

    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public override string ToString() => $"{Value:+#;-#;+0}";

    public static implicit operator RealScalar(double scalar) => new(scalar);
}