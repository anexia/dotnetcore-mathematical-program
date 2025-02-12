// ------------------------------------------------------------------------------------------
//  <copyright file = "InadmissibleBoundsException.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH.All rights reserved.
//  </copyright>
//  ------------------------------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Anexia.MathematicalProgram.Model.Scalar;

namespace Anexia.MathematicalProgram.Model.Variable;

/// <summary>
/// An exception where the lower bound is greater than the upper bound.
/// </summary>
[ExcludeFromCodeCoverage]
public sealed class InadmissibleBoundsException<T> : InvalidOperationException
    where T : IAddableScalar<T, T>
{
    internal InadmissibleBoundsException(IAddableScalar<T, T> lowerBound, IAddableScalar<T, T> upperBound)
        : base($"Lower bound {lowerBound} is larger than upper bound {upperBound}")
    {
    }
}