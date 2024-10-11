// ------------------------------------------------------------------------------------------
//  <copyright file = "LinearProgramSolver.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH.All rights reserved.
//  </copyright>
//  ------------------------------------------------------------------------------------------

#region

using Anexia.MathematicalProgram.Model;
using Anexia.MathematicalProgram.Result;
using Anexia.MathematicalProgram.SolverConfiguration;
using Google.OrTools.LinearSolver;

#endregion

namespace Anexia.MathematicalProgram.Solve;

/// <summary>
/// Represents a solver for solving Linear Programming models.
/// </summary>
public sealed class LinearProgramSolver : MemberwiseEquatable<LinearProgramSolver>, IDisposable
{
    private LinearProgramSolver(Solver solver)
    {
        Solver = solver;
    }

    /// <summary>
    /// Initializes a new instance of <see cref="LinearProgramSolver"/> using
    /// <see href="https://developers.google.com/optimization/lp/lp_example">GLOP</see>.
    /// </summary>
    public LinearProgramSolver()
        : this(Solver.CreateSolver(LpSolverType.Glop.ToString()))
    {
    }

    private Solver Solver { get; }

    /// <summary>
    /// Adds a new continuous variable to the solver and returns it as an out parameter.
    /// </summary>
    /// <param name="interval">The desired variable's interval.</param>
    /// <param name="variableName">The desired variable's name.</param>
    /// <param name="variable">The newly created variable.</param>
    /// <returns>The updated solver.</returns>
    public LinearProgramSolver AddContinuousVariable(IInterval interval, string variableName, out Variable variable)
    {
        variable = Solver.MakeNumVar(interval.LowerBound.Value, interval.UpperBound.Value, variableName);

        return this;
    }

    /// <summary>
    /// Adds the given constraints to the solver.
    /// </summary>
    /// <param name="constraints">The constraints to be added.</param>
    /// <returns>The updated solver.</returns>
    public LinearProgramSolver AddConstraints(Constraints constraints)
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
    public LinearProgramSolver SetObjective(Terms terms, bool minimize) => SetObjective(terms, Constant.Zero, minimize);

    /// <summary>
    /// Sets the objective function of the solver.
    /// </summary>
    /// <param name="terms">The terms of the objective function.</param>
    /// <param name="constant">An additional constant offset.</param>
    /// <param name="minimize">Boolean whether to minimize or maximize.</param>
    /// <returns>The updated solver.</returns>
    public LinearProgramSolver SetObjective(Terms terms, Constant constant, bool minimize)
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
    /// <returns>The result after solving.</returns>
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

            var parameter = new MPSolverParameters();

            parameter.SetDoubleParam(MPSolverParameters.DoubleParam.RELATIVE_MIP_GAP,
                solverParameter.RelativeGap.Value);

            var resultStatus = Solver.Solve(parameter);

            return new ResultHandling(Solver).Handle(resultStatus);
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);

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
    public override string ToString() => "Google.OrTools.LinearSolver Glop";
}