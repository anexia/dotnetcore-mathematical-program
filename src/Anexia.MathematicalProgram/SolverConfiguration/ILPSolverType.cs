// ------------------------------------------------------------------------------------------
//  <copyright file = "ILPSolverType.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH.All rights reserved.
//  </copyright>
//  ------------------------------------------------------------------------------------------


using System.Runtime.Serialization;


namespace Anexia.MathematicalProgram.SolverConfiguration;

/// <summary>
/// The supported ILP solver types.
/// </summary>
public enum IlpSolverType
{
    /// <summary>
    /// <see href="https://github.com/coin-or/Cbc">CBC</see> solver.
    /// </summary>
    [Obsolete("CBC Solver should not be used anymore, switch to SCIP instead.")]
    [EnumMember(Value = "CBC_MIXED_INTEGER_PROGRAMMING")]
    CbcIntegerProgramming,

    /// <summary>
    /// <see href="https://gurobi.com">Gurobi</see> solver. A licence is needed for usage.
    /// </summary>
    [EnumMember(Value = "GUROBI_MIXED_INTEGER_PROGRAMMING")]
    GurobiIntegerProgramming,

    /// <summary>
    /// <see href="https://www.scipopt.org/">SCIP</see> solver.
    /// </summary>
    [EnumMember(Value = "SCIP")] Scip,
}