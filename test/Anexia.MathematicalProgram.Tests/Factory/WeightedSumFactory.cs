// ------------------------------------------------------------------------------------------
//  <copyright file = "WeightedSumFactory.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH. All rights reserved.
//  </copyright>
// ------------------------------------------------------------------------------------------

using System.Collections.Immutable;
using Anexia.MathematicalProgram.Model.Expression;
using Anexia.MathematicalProgram.Model.Scalar;
using Anexia.MathematicalProgram.Model.Variable;

namespace Anexia.MathematicalProgram.Tests.Factory;

public static class WeightedSumFactory
{
    public static IWeightedSum<IIntegerVariable<IRealScalar>, RealScalar, IRealScalar> WeightedSum(
        params (IIntegerVariable<IRealScalar> Variable, RealScalar Coefficient)[] terms) =>
        new WeightedSum<IIntegerVariable<IRealScalar>, RealScalar, IRealScalar>(
            terms.ToImmutableDictionary(term => term.Variable, term => term.Coefficient));
}