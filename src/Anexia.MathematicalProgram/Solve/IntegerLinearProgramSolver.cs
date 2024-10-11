// ------------------------------------------------------------------------------------------
//  <copyright file = "IntegerLinearProgramSolver.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH.All rights reserved.
//  </copyright>
//  ------------------------------------------------------------------------------------------

#region

using Anexia.MathematicalProgram.Extensions;
using Anexia.MathematicalProgram.Model;
using Anexia.MathematicalProgram.Result;
using Anexia.MathematicalProgram.SolverConfiguration;
using Google.OrTools.LinearSolver;

#endregion

namespace Anexia.MathematicalProgram.Solve;

/// <summary>
/// Represents a solver for solving Integer Linear Programming models.
/// </summary>
public sealed class IntegerLinearProgramSolver : MemberwiseEquatable<IntegerLinearProgramSolver>, IDisposable
{
    private IntegerLinearProgramSolver(Solver solver, IlpSolverType solverType)
    {
        Solver = solver;
        SolverType = solverType;
    }

    /// <summary>
    /// Initializes a new instance of <see cref="IntegerLinearProgramSolver"/> with an optional
    /// <see cref="IlpSolverType"/>. When the <see cref="IlpSolverType"/> is not specified, the default solver is
    /// <see href="https://github.com/coin-or/Cbc">CBC</see>.
    /// If the given solver type is either not supported, or no licence is available, it is switched to
    /// <see href="https://github.com/coin-or/Cbc">CBC</see>.
    /// </summary>
    /// <param name="solverType">The desired solver type.</param>
    public IntegerLinearProgramSolver(IlpSolverType solverType = IlpSolverType.CbcMixedIntegerProgramming)
        : this(Solver.CreateSolver(CheckTypeSupportedOrSwitchToCbc(solverType).ToEnumString()),
            CheckTypeSupportedOrSwitchToCbc(solverType))
    {
    }

    /// <summary>
    /// Creates an instance of <see cref="IntegerLinearProgramSolver"/> with a given solver type.
    /// If the given solver type is either not supported, or no licence is available, it is switched to
    /// <see href="https://github.com/coin-or/Cbc">CBC</see>. Furthermore, a warning message is set via
    /// <paramref name="message"/>.
    /// </summary>
    /// <param name="solverType">The desired solver type.</param>
    /// <param name="message">A warning message when the solver tye gets switched automatically.</param>
    /// <returns>A new instance of <see cref="IntegerLinearProgramSolver"/>.</returns>
    public static IntegerLinearProgramSolver Create(IlpSolverType solverType, out WarningMessage? message)
    {
        message = null;

        if (Solver.SupportsProblemType(Enum.Parse<Solver.OptimizationProblemType>(solverType.ToEnumString())))
            return new IntegerLinearProgramSolver(solverType);

        message = new(
            $"Solver type {solverType} is not supported -> Switched to CBC. There might be no valid licence or an unsupported Gurobi version.");

        return new IntegerLinearProgramSolver();
    }

    private static IlpSolverType CheckTypeSupportedOrSwitchToCbc(IlpSolverType solverType) =>
        Solver.SupportsProblemType(Enum.Parse<Solver.OptimizationProblemType>(solverType.ToEnumString()))
            ? solverType
            : IlpSolverType.CbcMixedIntegerProgramming;

    private Solver Solver { get; }
    private IlpSolverType SolverType { get; }

    /// <summary>
    /// Returns the model in LP format.
    /// </summary>
    /// <param name="obfuscated">Specifies whether the model should be obfuscated, or not. True by default.</param>
    /// <returns>The model.</returns>
    public string ModelAsLpFormat(bool obfuscated = true) => Solver.ExportModelAsLpFormat(obfuscated);

    /// <summary>
    /// Returns the number of constraints.
    /// </summary>
    /// <returns>The number of constraints.</returns>
    public int NumberOfConstraints() => Solver.NumConstraints();

    /// <summary>
    /// Returns the number of variables.
    /// </summary>
    /// <returns>The number of variables.</returns>
    public int NumberOfVariables() => Solver.NumVariables();

    /// <summary>
    /// Adds a new integer variable to the solver and returns it as an out parameter.
    /// </summary>
    /// <param name="interval">The desired variable's interval.</param>
    /// <param name="variableName">The desired variable's name.</param>
    /// <param name="variable">The newly created variable.</param>
    /// <returns>The updated solver.</returns>
    public IntegerLinearProgramSolver AddIntegerVariable(IInterval interval, string variableName, out Variable variable)
    {
        variable = Solver.MakeIntVar(interval.LowerBound.Value, interval.UpperBound.Value, variableName);

        return this;
    }

    /// <summary>
    /// Adds the given constraints to the solver.
    /// </summary>
    /// <param name="constraints">The constraints to be added.</param>
    /// <returns>The updated solver.</returns>
    public IntegerLinearProgramSolver AddConstraints(Constraints constraints)
    {
        foreach (var constraint in constraints)
        {
            var interval = constraint.Interval;
            var solverConstraint = Solver.MakeConstraint(interval.LowerBound.Value, interval.UpperBound.Value);

            foreach (var term in constraint.Terms)
                solverConstraint.SetCoefficient(term.Variable, term.Coefficient.Value);
        }

        return this;
    }

    /// <summary>
    /// Sets the objective function of the solver.
    /// </summary>
    /// <param name="terms">The terms of the objective function.</param>
    /// <param name="minimize">Boolean whether to minimize or maximize.</param>
    /// <returns>The updated solver.</returns>
    public IntegerLinearProgramSolver SetObjective(Terms terms, bool minimize) =>
        SetObjective(terms, Constant.Zero, minimize);

    /// <summary>
    /// Sets the objective function of the solver.
    /// </summary>
    /// <param name="terms">The terms of the objective function.</param>
    /// <param name="constant">An additional constant offset.</param>
    /// <param name="minimize">Boolean whether to minimize or maximize.</param>
    /// <returns>The updated solver.</returns>
    public IntegerLinearProgramSolver SetObjective(Terms terms, Constant constant, bool minimize)
    {
        var objective = Solver.Objective();
        objective.Clear();

        foreach (var term in terms) objective.SetCoefficient(term.Variable, term.Coefficient.Value);

        objective.SetOffset(constant.Value);

        if (minimize)
            objective.SetMinimization();
        else
            objective.SetMaximization();

        return this;
    }

    /// <summary>
    /// Starts the solving process.
    /// </summary>
    /// <returns>The result after solving.</returns>
    /// <exception cref="MathematicalProgramException">Throws a <see cref="MathematicalProgramException"/> if an error occured while solving.
    /// Furthermore, when the result status is anything other than feasible, infeasible or optimal.
    /// </exception>
    public SolverResult Solve() => Solve(new SolverParameter());

    /// <summary>
    /// Starts the solving process with additional parameters.
    /// </summary>
    /// <param name="solverParameter">The parameters to be used by the solver.</param>
    /// <returns>The result.</returns>
    /// <exception cref="MathematicalProgramException">Throws a <see cref="MathematicalProgramException"/> if an error occured while solving.
    /// Furthermore, when the result status is anything other than feasible, infeasible or optimal.
    /// </exception>
    public SolverResult Solve(SolverParameter solverParameter)
    {
        try
        {
            _ = Solver.SetNumThreads((int)(solverParameter.NumberOfThreads?.Value ?? 0));

            if (solverParameter.TimeLimitInMilliseconds is not null)
                Solver.SetTimeLimit(solverParameter.TimeLimitInMilliseconds.Value);

            if (solverParameter.EnableSolverOutput.Value) Solver.EnableOutput();
            using var parameter = new MPSolverParameters();

            parameter.SetDoubleParam(MPSolverParameters.DoubleParam.RELATIVE_MIP_GAP,
                solverParameter.RelativeGap.Value);

            var resultStatus = Solver.Solve(parameter);

            return new ResultHandling(Solver).Handle(resultStatus);
        }
        catch (Exception exception)
        {
            throw new MathematicalProgramException(exception);
        }
    }


    /// <inheritdoc />
    public void Dispose()
    {
        Solver.Clear();
        Solver.Dispose();
    }

    /// <inheritdoc />
    public override string ToString() => $"IntegerLinearProgrammingSolver {SolverType.ToEnumString()}";
}