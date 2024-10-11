// ------------------------------------------------------------------------------------------
//  <copyright file = "ILPSolverType.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH.All rights reserved.
//  </copyright>
//  ------------------------------------------------------------------------------------------

#region

using System.Runtime.Serialization;

#endregion

namespace Anexia.MathematicalProgram.SolverConfiguration;

public enum ILPSolverType
{
    [EnumMember(Value = "CBC_MIXED_INTEGER_PROGRAMMING")]
    CbcMixedIntegerProgramming,

    [EnumMember(Value = "GUROBI_MIXED_INTEGER_PROGRAMMING")]
    GurobiMixedIntegerProgramming
}