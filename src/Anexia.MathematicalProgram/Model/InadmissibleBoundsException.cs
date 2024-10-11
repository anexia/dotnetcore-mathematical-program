// ------------------------------------------------------------------------------------------
//  <copyright file = "InadmissibleBoundsException.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH.All rights reserved.
//  </copyright>
//  ------------------------------------------------------------------------------------------

#region

using System.Runtime.Serialization;

#endregion

namespace Anexia.MathematicalProgram.Model;

[Serializable]
public sealed class InadmissibleBoundsException : Exception
{
    private InadmissibleBoundsException(SerializationInfo info, StreamingContext context)
       : base(info, context)
    { }

    public InadmissibleBoundsException(string message)
        : base(message)
    { }

    public InadmissibleBoundsException(LowerBound lowerBound, UpperBound upperBound)
        : base($"Lower bound {lowerBound} is larger than upper bound {upperBound}")
    { }
}