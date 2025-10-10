// ------------------------------------------------------------------------------------------
//  <copyright file = "GetSolutionResponse.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH. All rights reserved.
//  </copyright>
// ------------------------------------------------------------------------------------------

using System.Text.Json.Serialization;

namespace Anexia.MathematicalProgram.MPaaS.Model;

[method: JsonConstructor]
public record GetSolutionResponse(
    bool IsFeasible,
    double? Gap,
    double? ObjectiveValue,
    bool IsOptimal,
    string? SolverResultStatus,
    IReadOnlyDictionary<string, double?> VariableValues);