// ------------------------------------------------------------------------------------------
//  <copyright file = "IIntegerScalar.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH. All rights reserved.
//  </copyright>
// ------------------------------------------------------------------------------------------

namespace Anexia.MathematicalProgram.Model.Scalar;

/// <summary>
/// Represents an integer scalar.
/// </summary>
public interface IIntegerScalar : IRealScalar, IAddableScalar<IIntegerScalar, IIntegerScalar>
{
    /// <summary>
    /// The value.
    /// </summary>
    public new long Value { get; }
}