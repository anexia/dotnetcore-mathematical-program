// ------------------------------------------------------------------------------------------
//  <copyright file = "LPSolverType.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH.All rights reserved.
//  </copyright>
//  ------------------------------------------------------------------------------------------

using System.Runtime.Serialization;

namespace Anexia.MathematicalProgram.SolverConfiguration;

/// <summary>
/// The supported LP solvers.
/// </summary>
public enum LpSolverType
{
    /// <summary>
    /// <see href="https://developers.google.com/optimization/lp/lp_example">GLOP</see> solver.
    /// </summary>
    [EnumMember(Value = "GLOP")] Glop,

    /// <summary>
    /// <see href="https://www.scipopt.org/">SCIP</see> solver.
    /// </summary>
    [EnumMember(Value = "SCIP")] Scip,

    /// <summary>
    /// <see href="https://gurobi.com">Gurobi</see> solver. A licence is needed for usage.
    /// </summary>
    [EnumMember(Value = "GUROBI_MIXED_INTEGER_PROGRAMMING")]
    GurobiMixedIntegerProgramming,
}