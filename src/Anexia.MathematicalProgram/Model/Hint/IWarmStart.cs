// ------------------------------------------------------------------------------------------
//  <copyright file = "IWarmStart.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH. All rights reserved.
//  </copyright>
// ------------------------------------------------------------------------------------------

using Anexia.MathematicalProgram.Model.Scalar;
using Anexia.MathematicalProgram.Model.Variable;

namespace Anexia.MathematicalProgram.Model.Hint;

/// <summary>
/// Represents a (possibly partial) start solution that is passed to the solver as warm start.
/// Every variable occurs at most once.
/// <list type="bullet">
/// <item><description>Native Gurobi: set as MIP start (<c>Start</c> attribute).</description></item>
/// <item><description>Gurobi and SCIP via OR-Tools: passed as solution hint, which Gurobi treats as MIP start.</description></item>
/// <item><description>CP-SAT: passed as solution hint.</description></item>
/// <item><description>HiGHS: ignored, passing hints to HiGHS crashes OR-Tools (up to and including 9.14).</description></item>
/// <item><description>CBC: ignored.</description></item>
/// </list>
/// </summary>
/// <typeparam name="TVariable">The type of the variable.</typeparam>
/// <typeparam name="TVariableInterval">The type of the variable interval's scalar, which is also the type of the start values.</typeparam>
public interface IWarmStart<out TVariable, out TVariableInterval>
    : IReadOnlyCollection<IStartValue<TVariable, TVariableInterval>>
    where TVariable : IVariable<TVariableInterval>
    where TVariableInterval : IAddableScalar<TVariableInterval, TVariableInterval>
{
}
