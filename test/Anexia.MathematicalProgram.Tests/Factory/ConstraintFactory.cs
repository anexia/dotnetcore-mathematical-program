// ------------------------------------------------------------------------------------------
//  <copyright file = "ConstraintFactory.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH.All rights reserved.
//  </copyright>
//  ------------------------------------------------------------------------------------------

#region

using Anexia.MathematicalProgram.Model;

#endregion

namespace Anexia.MathematicalProgram.Tests.Factory;

internal static class ConstraintFactory
{
    public static Constraint Constraint(Term[] terms, IInterval interval) => new(new Terms(terms), interval);

    public static Constraints Constraints(params Constraint[] constraints) => new(constraints);
}