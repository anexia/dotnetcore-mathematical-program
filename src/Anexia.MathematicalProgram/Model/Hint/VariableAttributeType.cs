// ------------------------------------------------------------------------------------------
//  <copyright file = "VariableAttributeType.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH. All rights reserved.
//  </copyright>
// ------------------------------------------------------------------------------------------

namespace Anexia.MathematicalProgram.Model.Hint;

/// <summary>
/// Solver specific, per-variable attributes that guide the search. Currently only the native Gurobi solver
/// applies these attributes, other solvers ignore them. For a start solution, use
/// <see cref="IWarmStart{TVariable,TVariableInterval}"/> instead, which is supported by all solvers.
/// </summary>
public enum VariableAttributeType
{
    /// <summary>
    /// Hint for the value the variable is likely to take in an optimal solution. Unlike a warm start, hints guide
    /// the whole search and do not need to form a feasible solution.
    /// Gurobi: <see href="https://docs.gurobi.com/projects/optimizer/en/current/reference/attributes/variable.html#varhintval">VarHintVal</see>.
    /// </summary>
    HintValue,

    /// <summary>
    /// Priority of the corresponding <see cref="HintValue"/>. Larger values indicate more confidence. Integral value.
    /// Gurobi: <see href="https://docs.gurobi.com/projects/optimizer/en/current/reference/attributes/variable.html#varhintpri">VarHintPri</see>.
    /// </summary>
    HintPriority,

    /// <summary>
    /// Branching priority of the variable. Variables with higher priority are branched on first. Integral value.
    /// Gurobi: <see href="https://docs.gurobi.com/projects/optimizer/en/current/reference/attributes/variable.html#branchpriority">BranchPriority</see>.
    /// </summary>
    BranchPriority
}
