// ------------------------------------------------------------------------------------------
//  <copyright file = "MathematicalProgramException.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH.All rights reserved.
//  </copyright>
//  ------------------------------------------------------------------------------------------

#region

using System.Runtime.Serialization;

#endregion

namespace Anexia.MathematicalProgram.Solve;

[Serializable]
public sealed class MathematicalProgramException : Exception
{
    private MathematicalProgramException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    { }

    public MathematicalProgramException(string message)
        : base(message)
    { }
}