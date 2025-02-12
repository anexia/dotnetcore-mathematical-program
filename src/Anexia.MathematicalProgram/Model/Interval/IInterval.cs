// ------------------------------------------------------------------------------------------
//  <copyright file = "IInterval.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH.All rights reserved.
//  </copyright>
//  ------------------------------------------------------------------------------------------

using Anexia.MathematicalProgram.Model.Scalar;

namespace Anexia.MathematicalProgram.Model.Interval;

/// <summary>
/// Represents an interval.
/// </summary>
/// <typeparam name="TScalar">The interval's type.</typeparam>
public interface IInterval<out TScalar> where TScalar : IAddableScalar<TScalar, TScalar>
{
    /// <summary>
    /// The interval's lower bound.
    /// </summary>
    public TScalar LowerBound { get; }

    /// <summary>
    /// The interval's upper bound.
    /// </summary>
    public TScalar UpperBound { get; }
}