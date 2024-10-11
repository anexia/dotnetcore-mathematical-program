// ------------------------------------------------------------------------------------------
//  <copyright file = "TermFactory.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH.All rights reserved.
//  </copyright>
//  ------------------------------------------------------------------------------------------

#region

using Anexia.MathematicalProgram.Model;
using Google.OrTools.LinearSolver;

#endregion

namespace Anexia.MathematicalProgram.Tests.Factory;

public static class TermFactory
{
    public static Term Term(double coefficient, Variable variable) => new(new Coefficient(coefficient), variable);
}