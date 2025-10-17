// ------------------------------------------------------------------------------------------
//  <copyright file = "SolverResultStatus.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH. All rights reserved.
//  </copyright>
// ------------------------------------------------------------------------------------------

namespace Anexia.MathematicalProgram.Result;

/// <summary>
/// Represents the various result statuses that can be returned by a solver
/// during the process of solving mathematical programs.
/// </summary>
public enum SolverResultStatus
{
    Optimal,
    Feasible,
    Infeasible,
    Unbounded,
    Abnormal,
    NotSolved,
    ModelIsValid,
    CancelledByUser,
    UnknownStatus,
    ModelInvalid,
    InvalidSolverParameters,
    SolverTypeUnavailable,
    IncompatibleOptions,
    InfOrUnbound,
    Timelimit
}