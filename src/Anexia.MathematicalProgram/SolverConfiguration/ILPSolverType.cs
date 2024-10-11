// ------------------------------------------------------------------------------------------
//  <copyright file = "ILPSolverType.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH.All rights reserved.
//  </copyright>
//  ------------------------------------------------------------------------------------------

#region

using System.Runtime.Serialization;

#endregion

namespace Anexia.MathematicalProgram.SolverConfiguration;

/// <summary>
/// The supported ILP solver types.
/// </summary>
public enum IlpSolverType
{
    /// <summary>
    /// <see href="https://github.com/coin-or/Cbc">CBC</see> solver.
    /// </summary>
    [EnumMember(Value = "CBC_MIXED_INTEGER_PROGRAMMING")]
    CbcMixedIntegerProgramming,

    /// <summary>
    /// <see href="https://gurobi.com">Gurobi</see> solver. A licence is needed for usage.
    /// </summary>
    [EnumMember(Value = "GUROBI_MIXED_INTEGER_PROGRAMMING")]
    GurobiMixedIntegerProgramming
}