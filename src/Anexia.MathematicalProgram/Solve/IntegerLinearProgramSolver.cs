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

public sealed class IntegerLinearProgramSolver : MemberwiseEquatable<IntegerLinearProgramSolver>
{
    private IntegerLinearProgramSolver(Solver solver, SolverParameter solverParameter, ILPSolverType solverType)
    {
        Solver = solver;
        SolverParameter = solverParameter;
        SolverType = solverType;
    }


    public IntegerLinearProgramSolver(ILPSolverType solverType = ILPSolverType.CbcMixedIntegerProgramming)
        : this(Solver.CreateSolver(CheckTypeSupportedOrSwitchToCbc(solverType).ToEnumString()),
            new(),
            CheckTypeSupportedOrSwitchToCbc(solverType))
    {
    }

    public static IntegerLinearProgramSolver Create(ILPSolverType solverType, out WarningMessage? message)
    {
        message = null;

        if (Solver.SupportsProblemType(Enum.Parse<Solver.OptimizationProblemType>(solverType.ToEnumString())))
            return new IntegerLinearProgramSolver(solverType);

        message = new(
            $"Solver type {solverType} is not supported -> Switched to CBC. There might be no valid licence or an unsupported Gurobi version.");

        return new IntegerLinearProgramSolver();
    }

    private static ILPSolverType CheckTypeSupportedOrSwitchToCbc(ILPSolverType solverType) =>
        Solver.SupportsProblemType(Enum.Parse<Solver.OptimizationProblemType>(solverType.ToEnumString()))
            ? solverType
            : ILPSolverType.CbcMixedIntegerProgramming;

    private Solver Solver { get; }
    private SolverParameter SolverParameter { get; }
    private ILPSolverType SolverType { get; }

    public string ModelAsLpFormat() => Solver.ExportModelAsLpFormat(true);

    public int NumberOfConstraints() => Solver.NumConstraints();

    public int NumberOfVariables() => Solver.NumVariables();


    public IntegerLinearProgramSolver SetSolverConfigurations(SolverParameter solverParameter)
    {
        _ = Solver.SetNumThreads((int)solverParameter.NumberOfThreads.Value);

        if (!solverParameter.TimeLimitInMilliseconds.Equals(TimeLimitInMilliseconds.Unbounded))
            Solver.SetTimeLimit(solverParameter.TimeLimitInMilliseconds.Value);

        if (solverParameter.EnableSolverOutput.Value) Solver.EnableOutput();

        return new IntegerLinearProgramSolver(Solver, solverParameter, SolverType);
    }

    public IntegerLinearProgramSolver AddIntegerVariable(IInterval interval, string variableName, out Variable variable)
    {
        variable = Solver.MakeIntVar(interval.LowerBound.Value, interval.UpperBound.Value, variableName);

        return new IntegerLinearProgramSolver(Solver, SolverParameter, SolverType);
    }

    public IntegerLinearProgramSolver AddConstraints(Constraints constraints)
    {
        foreach (var constraint in constraints)
        {
            var interval = constraint.Interval;
            var solverConstraint = Solver.MakeConstraint(interval.LowerBound.Value, interval.UpperBound.Value);

            foreach (var term in constraint.Terms)
                solverConstraint.SetCoefficient(term.Variable, term.Coefficient.Value);
        }

        return new IntegerLinearProgramSolver(Solver, SolverParameter, SolverType);
    }

    public IntegerLinearProgramSolver AddObjective(Terms terms, bool minimize) =>
        AddObjective(terms, Constant.Zero, minimize);

    public IntegerLinearProgramSolver AddObjective(Terms terms, Constant constant, bool minimize)
    {
        var objective = Solver.Objective();

        foreach (var term in terms) objective.SetCoefficient(term.Variable, term.Coefficient.Value);

        objective.SetOffset(constant.Value);

        if (minimize)
            objective.SetMinimization();
        else
            objective.SetMaximization();

        return new IntegerLinearProgramSolver(Solver, SolverParameter, SolverType);
    }

    public SolverResult Solve()
    {
        try
        {
            using var parameter = new MPSolverParameters();

            parameter.SetDoubleParam(MPSolverParameters.DoubleParam.RELATIVE_MIP_GAP,
                SolverParameter.RelativeGap.Value);

            var resultStatus = Solver.Solve(parameter);

            return new ResultHandling(Solver).Handle(resultStatus);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);

            throw new MathematicalProgramException($"Error in ILP solver: {e.Message}, {e.InnerException}");
        }
    }

    public void ClearAndDisposeSolver()
    {
        Solver.Clear();
        Solver.Dispose();
    }

    public override string ToString() => $"Google.OrTools.IntegerLinearSolver {SolverType.ToEnumString()}";
}