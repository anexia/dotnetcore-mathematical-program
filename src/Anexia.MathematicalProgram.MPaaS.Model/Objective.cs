// ------------------------------------------------------------------------------------------
//  <copyright file = "Objective.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH. All rights reserved.
//  </copyright>
// ------------------------------------------------------------------------------------------

using System.Text.Json.Serialization;

namespace Anexia.MathematicalProgram.MPaaS.Model;

[method: JsonConstructor]
public record Objective(IReadOnlyCollection<LinearExpression> Expression, bool Maximize, double? Offset = null);