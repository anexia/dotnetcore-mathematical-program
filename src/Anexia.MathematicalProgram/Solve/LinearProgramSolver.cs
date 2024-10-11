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

public sealed class LinearProgramSolver : MemberwiseEquatable<LinearProgramSolver>
{
    private LinearProgramSolver(Solver solver, SolverParameter solverParameter)
    {
        Solver = solver;
        SolverParameter = solverParameter;
    }

    public LinearProgramSolver()
        : this(Solver.CreateSolver(LPSolverType.GLOP.ToString()), new())
    { }

    private Solver Solver { get; }
    private SolverParameter SolverParameter { get; }

    public LinearProgramSolver SetSolverConfigurations(SolverParameter solverParameter)
    {
        _ = Solver.SetNumThreads((int)solverParameter.NumberOfThreads.Value);

        if(!solverParameter.TimeLimitInMilliseconds.Equals(TimeLimitInMilliseconds.Unbounded))
            Solver.SetTimeLimit(solverParameter.TimeLimitInMilliseconds.Value);

        if(solverParameter.EnableSolverOutput.Value) Solver.EnableOutput();

        return new LinearProgramSolver(Solver, SolverParameter);
    }

    public LinearProgramSolver AddContinuousVariable(IInterval interval, string variableName, out Variable variable)
    {
        variable = Solver.MakeNumVar(interval.LowerBound.Value, interval.UpperBound.Value, variableName);

        return new LinearProgramSolver(Solver, SolverParameter);
    }

    public LinearProgramSolver AddConstraints(Constraints constraints)
    {
        foreach(var constraint in constraints)
        {
            var interval = constraint.Interval;
            var solverConstraint = Solver.MakeConstraint(interval.LowerBound.Value, interval.UpperBound.Value);

            foreach(var term in constraint.Terms)
                solverConstraint.SetCoefficient(term.Variable, term.Coefficient.Value);
        }

        return new LinearProgramSolver(Solver, SolverParameter);
    }

    public LinearProgramSolver AddObjective(Terms terms, bool minimize) => AddObjective(terms, Constant.Zero, minimize);

    public LinearProgramSolver AddObjective(Terms terms, Constant constant, bool minimize)
    {
        var objective = Solver.Objective();

        foreach(var term in terms) objective.SetCoefficient(term.Variable, term.Coefficient.Value);

        objective.SetOffset(constant.Value);

        if(minimize)
            objective.SetMinimization();
        else
            objective.SetMaximization();

        return new LinearProgramSolver(Solver, SolverParameter);
    }

    public SolverResult Solve()
    {
        try
        {
            var parameter = new MPSolverParameters();

            parameter.SetDoubleParam(MPSolverParameters.DoubleParam.RELATIVE_MIP_GAP,
                SolverParameter.RelativeGap.Value);

            var resultStatus = Solver.Solve(parameter);

            return new ResultHandling(Solver).Handle(resultStatus);
        }
        catch(Exception e)
        {
            Console.WriteLine(e);

            throw new MathematicalProgramException($"Error in LP solver: {e.Message}, {e.InnerException}");
        }
    }
    public void ClearAndDisposeSolver()
    {
        Solver.Clear();
        Solver.Dispose();
    }

    public override string ToString() => "Google.OrTools.LinearSolver GLOP";
}