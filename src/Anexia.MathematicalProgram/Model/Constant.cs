// ------------------------------------------------------------------------------------------
//  <copyright file = "Constant.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH.All rights reserved.
//  </copyright>
//  ------------------------------------------------------------------------------------------


namespace Anexia.MathematicalProgram.Model;

/// <summary>
/// Represents a constant.
/// </summary>
/// <param name="value">The constant's value.</param>
public sealed class Constant(double value) : MemberwiseEquatable<Constant>
{
    public static readonly Constant Zero = new(0);
    public double Value { get; } = value;
    public static Constant operator +(Constant left, double right) => new(left.Value + right);
    /// <inheritdoc />
    public override string ToString() => $"{Value:F1}";
}