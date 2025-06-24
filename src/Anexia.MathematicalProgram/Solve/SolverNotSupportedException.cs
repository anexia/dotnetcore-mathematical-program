// ------------------------------------------------------------------------------------------
//  <copyright file = "SolverNotSupportedException.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH.All rights reserved.
//  </copyright>
//  ------------------------------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Anexia.MathematicalProgram.SolverConfiguration;

namespace Anexia.MathematicalProgram.Solve;

/// <summary>
/// An exception occured while initializing the solver.
/// </summary>
[ExcludeFromCodeCoverage]
public sealed class SolverNotSupportedException : MathematicalProgramException
{
    internal SolverNotSupportedException(IlpSolverType expected, IlpSolverType fallback)
        : base($"Neither the expected solver {expected} nor fallback solver {fallback} could be initialized.")
    {
    }
}