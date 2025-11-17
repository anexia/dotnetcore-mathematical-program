// ------------------------------------------------------------------------------------------
//  <copyright file = "BinaryScalar.cs" company = "Anexia Digital Engineering GmbH">
//  Copyright (c) Anexia Digital Engineering GmbH. All rights reserved.
//  </copyright>
// ------------------------------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;

namespace Anexia.MathematicalProgram.Model.Scalar;

/// <summary>
/// Represents a binary scalar with a value of 0 or 1.
/// </summary>
public readonly record struct BinaryScalar : IBinaryScalar, IAddableScalar<BinaryScalar, BinaryScalar>
{
    /// <summary>
    /// The value 0.
    /// </summary>
    public static readonly BinaryScalar Zero = new(false);

    /// <summary>
    /// The value 1.
    /// </summary>
    public static readonly BinaryScalar One = new(true);

    /// <summary>
    /// The value (0 (false) or 1 (true).
    /// </summary>
    public bool IsOne { get; }

    public long Value => Convert.ToInt64(IsOne);

    /// <summary>
    /// Initializes a new instance of <see cref="BinaryScalar"/>.
    /// </summary>
    /// <param name="isOne">True, sets the binary to 1, false to 0.</param>
    private BinaryScalar(bool isOne)
    {
        IsOne = isOne;
    }


    /// <summary>
    /// The value represented as real scalar.
    /// </summary>
    double IRealScalar.Value => Value;

    /// <summary>
    /// Adds two binary scalars (Note: 1+1=0).
    /// </summary>
    public BinaryScalar Add(BinaryScalar other) => new(IsOne ^ other.IsOne);

    /// <summary>
    /// Adds two binary scalars (Note: 1+1=0)
    /// </summary>
    public IBinaryScalar Add(IBinaryScalar scalar) => new BinaryScalar(IsOne ^ scalar.IsOne);

    /// <inheritdoc />
    public IRealScalar Add(IRealScalar other) => new RealScalar((Value + other.Value) % 2);

    /// <inheritdoc />
    public IIntegerScalar Add(IIntegerScalar other) => new IntegerScalar((Value + other.Value) % 2);

    /// <summary>
    /// Subtracts two binary scalars (Note: 0-1=1)
    /// </summary>
    public BinaryScalar Subtract(BinaryScalar scalar) => new(IsOne ^ scalar.IsOne);

    /// <inheritdoc />
    public IRealScalar Subtract(IRealScalar scalar) => new RealScalar((Value + scalar.Value) % 2);

    /// <inheritdoc />
    public IIntegerScalar Subtract(IIntegerScalar scalar) => new IntegerScalar((Value + scalar.Value) % 2);

    /// <summary>
    /// Subtracts two binary scalars (Note: 0-1=1).
    /// </summary>
    public IBinaryScalar Subtract(IBinaryScalar scalar) => new BinaryScalar(IsOne ^ scalar.IsOne);

    /// <summary>
    /// Adds two binary scalars.
    /// </summary>
    public static BinaryScalar operator +(BinaryScalar left, BinaryScalar right) => left.Add(right);

    /// <summary>
    /// Subtracts two binary scalars.
    /// </summary>
    public static BinaryScalar operator -(BinaryScalar left, BinaryScalar right) => left.Subtract(right);

    [ExcludeFromCodeCoverage]
    public override string ToString() => $"{Value:+#;-#;+0}";
}