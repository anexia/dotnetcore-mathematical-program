// ------------------------------------------------------------------------------------------
//  <copyright file = "IAddableScalar.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH. All rights reserved.
//  </copyright>
// ------------------------------------------------------------------------------------------

namespace Anexia.MathematicalProgram.Model.Scalar;

/// <summary>
/// Represents a scalar where a scalar can be added to.
/// </summary>
/// <typeparam name="TOut">Type of the result.</typeparam>
/// <typeparam name="TIn">The scalar to be added.</typeparam>
public interface IAddableScalar<out TOut, in TIn> where TOut : IAddableScalar<TOut, TIn>
    where TIn : IAddableScalar<TOut, TIn>
{
    /// <summary>
    /// Adds a scalar.
    /// </summary>
    /// <param name="scalar">The scalar to add.</param>
    /// <returns>The result of the addition.</returns>
    public TOut Add(TIn scalar);

    /// <summary>
    /// Subtracts a scalar.
    /// </summary>
    /// <param name="scalar">The scalar to subtract.</param>
    /// <returns>The result of the subtraction.</returns>
    public TOut Subtract(TIn scalar);
}