// ------------------------------------------------------------------------------------------
//  <copyright file = "InadmissibleBoundsException.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH.All rights reserved.
//  </copyright>
//  ------------------------------------------------------------------------------------------

namespace Anexia.MathematicalProgram.Model;

/// <summary>
/// An exception where the lower bound is greater than the upper bound.
/// </summary>
public sealed class InadmissibleBoundsException : Exception
{
    /// <summary>
    /// Initializes a new instance of <see cref="InadmissibleBoundsException"/> with given bounds.
    /// </summary>
    /// <param name="lowerBound">The lower bound.</param>
    /// <param name="upperBound">The upper bound.</param>
    internal InadmissibleBoundsException(LowerBound lowerBound, UpperBound upperBound)
        : base($"Lower bound {lowerBound} is larger than upper bound {upperBound}")
    {
    }
}