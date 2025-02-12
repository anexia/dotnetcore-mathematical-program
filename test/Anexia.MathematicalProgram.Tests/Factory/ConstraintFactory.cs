// ------------------------------------------------------------------------------------------
//  <copyright file = "ConstraintFactory.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH. All rights reserved.
//  </copyright>
// ------------------------------------------------------------------------------------------

using Anexia.MathematicalProgram.Model.Expression;
using Anexia.MathematicalProgram.Model.Interval;
using Anexia.MathematicalProgram.Model.Scalar;
using Anexia.MathematicalProgram.Model.Variable;

namespace Anexia.MathematicalProgram.Tests.Factory;

public static class ConstraintFactory
{
    internal static Constraints<IIntegerVariable<IRealScalar>, RealScalar, IRealScalar> Constraints(
        params IConstraint<IIntegerVariable<IRealScalar>, RealScalar, IRealScalar>[] constraints) =>
        new(constraints);

    internal static Constraint<IIntegerVariable<IRealScalar>, RealScalar, IRealScalar> Constraint(
        IWeightedSum<IIntegerVariable<IRealScalar>, RealScalar, IRealScalar> weightedSum,
        IInterval<IRealScalar> interval) => new(weightedSum, interval);
}