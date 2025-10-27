// ------------------------------------------------------------------------------------------
//  <copyright file = "Variable.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH. All rights reserved.
//  </copyright>
// ------------------------------------------------------------------------------------------

using System.Text.Json.Serialization;

namespace Anexia.MathematicalProgram.MPaaS.Model;

[method: JsonConstructor]
public record Variable(string Name, double? LowerBound, double? UpperBound);