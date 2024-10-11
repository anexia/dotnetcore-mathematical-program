// ------------------------------------------------------------------------------------------
//  <copyright file = "IInterval.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH.All rights reserved.
//  </copyright>
//  ------------------------------------------------------------------------------------------

namespace Anexia.MathematicalProgram.Model;

public interface IInterval
{
    public LowerBound LowerBound { get; }
    public UpperBound UpperBound { get; }
}