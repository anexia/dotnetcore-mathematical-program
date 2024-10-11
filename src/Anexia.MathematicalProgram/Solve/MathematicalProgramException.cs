// ------------------------------------------------------------------------------------------
//  <copyright file = "MathematicalProgramException.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH.All rights reserved.
//  </copyright>
//  ------------------------------------------------------------------------------------------

namespace Anexia.MathematicalProgram.Solve;

/// <summary>
/// An exception occured while solving a model.
/// </summary>
public sealed class MathematicalProgramException : Exception
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