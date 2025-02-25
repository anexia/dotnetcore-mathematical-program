// ------------------------------------------------------------------------------------------
//  <copyright file = "RelativeGap.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH.All rights reserved.
//  </copyright>
//  ------------------------------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;

namespace Anexia.MathematicalProgram.SolverConfiguration;

/// <summary>
/// Represents the relative gap when the solver terminates. It is an upper bound on the actual MIP gap given by (|ObjBound - ObjValue|) / |ObjValue|.
/// </summary>
/// <param name="relativeGap">The gap.</param>
public sealed class RelativeGap(double relativeGap) : MemberwiseEquatable<RelativeGap>
{
    public static readonly RelativeGap EMinus7 = new(0.0000001);

    public double Value { get; } = relativeGap;

    /// <summary>
    /// Calculates the given negative power of 10, i.e., 10^-negativeExponent
    /// </summary>
    /// <param name="negativeExponent">The exponent of 10^-1</param>
    /// <returns>The calculated value.</returns>
    public static RelativeGap FromEMinus(uint negativeExponent) => new(Math.Pow(10, -negativeExponent));

    /// <inderitdoc />
    [ExcludeFromCodeCoverage]
    public override string ToString() => $"{nameof(Value)}: {Value}";
}