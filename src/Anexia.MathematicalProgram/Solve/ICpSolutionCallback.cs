// ------------------------------------------------------------------------------------------
//  <copyright file = "ICpSolutionCallback.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH. All rights reserved.
//  </copyright>
// ------------------------------------------------------------------------------------------

using Anexia.MathematicalProgram.Model.Scalar;
using Anexia.MathematicalProgram.Model.Variable;
using Anexia.MathematicalProgram.Result;

namespace Anexia.MathematicalProgram.Solve;

/// <summary>
/// Solution Callback for CP Solver. Called when the solver finds a new solution.
/// </summary>
public interface ICpSolutionCallback
{
    /// <summary>
    /// Callback method dispatched when the solver finds a new solution.
    /// </summary>
    /// <param name="solutionValues">The solution values of the found solution.</param>
    public void OnSolutionCallback(
        SolutionValues<IIntegerVariable<IIntegerScalar>, IntegerScalar, IIntegerScalar> solutionValues);
}