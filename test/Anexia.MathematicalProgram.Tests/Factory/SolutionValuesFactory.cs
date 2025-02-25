// ------------------------------------------------------------------------------------------
//  <copyright file = "SolutionValuesFactory.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH. All rights reserved.
//  </copyright>
// ------------------------------------------------------------------------------------------

using Anexia.MathematicalProgram.Model.Scalar;
using Anexia.MathematicalProgram.Model.Variable;
using Anexia.MathematicalProgram.Result;

namespace Anexia.MathematicalProgram.Tests.Factory;

public static class SolutionValuesFactory
{
    public static SolutionValues<TVariable, TScalar, TVariableInterval> SolutionValues<TVariable, TScalar,
        TVariableInterval>(
        params (TVariable Variable, TScalar Coefficient)[] values)
        where TVariableInterval : IAddableScalar<TVariableInterval, TVariableInterval>
        where TVariable : IVariable<TVariableInterval> =>
        new(
            values.ToDictionary(tuple => tuple.Variable, tuple => tuple.Coefficient).AsReadOnly());
}