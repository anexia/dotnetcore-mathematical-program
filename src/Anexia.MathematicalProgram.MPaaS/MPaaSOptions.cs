// ------------------------------------------------------------------------------------------
//  <copyright file = "MPaaSOptions.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH. All rights reserved.
//  </copyright>
// ------------------------------------------------------------------------------------------

using Anexia.MathematicalProgram.SolverConfiguration;

namespace Anexia.MathematicalProgram.MPaaS;

public class MPaaSOptions
{
    public required string Workspace { get; init; }
    public IlpSolverType SolverType { get; init; }
}