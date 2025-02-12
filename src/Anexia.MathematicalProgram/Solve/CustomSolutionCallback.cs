// ------------------------------------------------------------------------------------------
//  <copyright file = "CustomSolutionCallback.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH. All rights reserved.
//  </copyright>
// ------------------------------------------------------------------------------------------

using System.Collections.Immutable;
using Anexia.MathematicalProgram.Model.Scalar;
using Anexia.MathematicalProgram.Model.Variable;
using Anexia.MathematicalProgram.Result;
using Google.OrTools.Sat;

namespace Anexia.MathematicalProgram.Solve;

internal sealed class CustomSolutionCallback(
    ICpSolutionCallback cpSolutionCallback,
    Dictionary<IIntegerVariable<IIntegerScalar>, IntVar> variables) : CpSolverSolutionCallback
{
    private ICpSolutionCallback CpSolutionCallback { get; } =
        cpSolutionCallback;

    private Dictionary<IIntegerVariable<IIntegerScalar>, IntVar> Variables { get; } = variables;

    public override void OnSolutionCallback()
    {
        CpSolutionCallback.OnSolutionCallback(
            new SolutionValues<IIntegerVariable<IIntegerScalar>, IntegerScalar, IIntegerScalar>(
                Variables.ToDictionary(
                    variable => variable.Key,
                    variable => new IntegerScalar((int)Value(variable.Value))).AsReadOnly()));
    }
}