// ------------------------------------------------------------------------------------------
//  <copyright file = "BinaryInterval.cs" company = "Anexia Digital Engineering GmbH">
//  Copyright (c) Anexia Digital Engineering GmbH. All rights reserved.
//  </copyright>
// ------------------------------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Anexia.MathematicalProgram.Model.Scalar;

namespace Anexia.MathematicalProgram.Model.Interval;

/// <summary>
/// Represents a binary interval [0, 1] with integer bounds.
/// </summary>
public record BinaryInterval : IInterval<IBinaryScalar>
{
    /// <summary>
    /// Creates a binary interval with bounds [0,1].
    /// </summary>
    public BinaryInterval()
    {
        LowerBound = BinaryScalar.Zero;
        UpperBound = BinaryScalar.One;
    }

    /// <summary>
    /// The lower bound (always 0).
    /// </summary>
    public IBinaryScalar LowerBound { get; }

    /// <summary>
    /// The upper bound (always 1).
    /// </summary>
    public IBinaryScalar UpperBound { get; }

    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public override string ToString() => $"[{LowerBound.Value},{UpperBound.Value}] (binary int)";
}