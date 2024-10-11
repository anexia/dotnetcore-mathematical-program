// ------------------------------------------------------------------------------------------
//  <copyright file = "IInterval.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH.All rights reserved.
//  </copyright>
//  ------------------------------------------------------------------------------------------

namespace Anexia.MathematicalProgram.Model;

/// <summary>
/// Represents an Interval.
/// </summary>
public interface IInterval
{
    /// <summary>
    /// The interval's lower bound.
    /// </summary>
    public LowerBound LowerBound { get; }

    /// <summary>
    /// The interval's upper bound.
    /// </summary>
    public UpperBound UpperBound { get; }
}