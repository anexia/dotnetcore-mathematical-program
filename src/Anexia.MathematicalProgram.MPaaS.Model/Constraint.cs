// ------------------------------------------------------------------------------------------
//  <copyright file = "Constraint.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH. All rights reserved.
//  </copyright>
// ------------------------------------------------------------------------------------------

using System.Text.Json.Serialization;

namespace Anexia.MathematicalProgram.MPaaS.Model;

[method: JsonConstructor]
public record Constraint(IReadOnlyList<LinearExpression> Terms, double? LowerBound, double? UpperBound);