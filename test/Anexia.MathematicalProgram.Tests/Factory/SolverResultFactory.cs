// ------------------------------------------------------------------------------------------
//  <copyright file = "SolverResultFactory.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH. All rights reserved.
//  </copyright>
// ------------------------------------------------------------------------------------------

using Anexia.MathematicalProgram.Model.Scalar;
using Anexia.MathematicalProgram.Model.Variable;
using Anexia.MathematicalProgram.Result;

namespace Anexia.MathematicalProgram.Tests.Factory;

public static class SolverResultFactory
{
    public static SolverResult<TVariable, TScalar, TVariableInterval>
        SolverResult<TVariable, TScalar, TVariableInterval>(
            ISolutionValues<TVariable, TScalar, TVariableInterval> solutionValues,
            ObjectiveValue? objectiveValue,
            IsFeasible isFeasible,
            IsOptimal isOptimal,
            OptimalityGap? optimalityGap,
            SolverResultStatus solverResultStatus,
            bool switchedToDefaultSolver) where TVariable : IVariable<TVariableInterval>
        where TVariableInterval : IAddableScalar<TVariableInterval, TVariableInterval> =>
        new(solutionValues, objectiveValue, isFeasible, isOptimal,
            optimalityGap, solverResultStatus, switchedToDefaultSolver);
}