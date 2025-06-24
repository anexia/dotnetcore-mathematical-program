// ------------------------------------------------------------------------------------------
//  <copyright file = "MathematicalProgramException.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH.All rights reserved.
//  </copyright>
//  ------------------------------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;

namespace Anexia.MathematicalProgram.Solve;

/// <summary>
/// An exception occured while solving a model.
/// </summary>
[ExcludeFromCodeCoverage]
public class MathematicalProgramException : Exception
{
    internal MathematicalProgramException(Exception exception)
        : base($"Error in solver: {exception.Message}, {exception.InnerException}", exception)
    {
    }

    internal MathematicalProgramException(string message)
        : base(message)
    {
    }
}