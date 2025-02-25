// ------------------------------------------------------------------------------------------
//  <copyright file = "IRealScalar.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH. All rights reserved.
//  </copyright>
// ------------------------------------------------------------------------------------------

namespace Anexia.MathematicalProgram.Model.Scalar;

/// <summary>
/// Represents a real scalar.
/// </summary>
public interface IRealScalar : IAddableScalar<IRealScalar, IRealScalar>
{
    /// <summary>
    /// The value.
    /// </summary>
    public double Value { get; }
}