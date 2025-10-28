// ------------------------------------------------------------------------------------------
//  <copyright file = "GurobiNativeSolver.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH. All rights reserved.
//  </copyright>
// ------------------------------------------------------------------------------------------

using Anexia.MathematicalProgram.Model;
using Anexia.MathematicalProgram.Model.Scalar;
using Anexia.MathematicalProgram.Model.Variable;
using Anexia.MathematicalProgram.Result;
using Anexia.MathematicalProgram.SolverConfiguration;
using Gurobi;
using Microsoft.Extensions.Logging;

namespace Anexia.MathematicalProgram.Solve;

public sealed class GurobiNativeSolver(
    ILogger<IlpSolver>? logger = null)
    : MemberwiseEquatable<IlpSolver>,
        IOptimizationSolver<IIntegerVariable<IRealScalar>, IRealScalar, IRealScalar, RealScalar>
{
    private ILogger<IlpSolver>? Logger { get; } = logger;

    public ISolverResult<IIntegerVariable<IRealScalar>, RealScalar, IRealScalar> Solve(
        ICompletedOptimizationModel<IIntegerVariable<IRealScalar>, IRealScalar, IRealScalar> model,
        SolverParameter solverParameter)
    {
        try
        {
            var env = new GRBEnv(true);
            foreach (var (key, value) in solverParameter.ToSolverSpecificParametersList(IlpSolverType
                         .GurobiIntegerProgramming))
            {
                env.Set(key, value);
            }

            if (solverParameter.TimeLimitInMilliseconds is not null)
                env.TimeLimit = solverParameter.TimeLimitInMilliseconds.AsSeconds;

            env.LogToConsole = solverParameter.EnableSolverOutput.Value ? 1 : 0;

            env.Start();
            var gurobiModel = new GRBModel(env);


            var variables = model.Variables.ToDictionary(
                item => item, item =>
                    gurobiModel.AddVar(item.Interval.LowerBound.Value, item.Interval.UpperBound.Value, 0, GRB.INTEGER,
                        item.Name));

            var cnt = 1;
            foreach (var constraint in model.Constraints)
            {
                var termsExpression = constraint.WeightedSum
                    .Aggregate(
                        new GRBLinExpr(), (expression, term) =>
                        {
                            expression.AddTerm(term.Coefficient.Value, variables[term.Variable]);
                            return expression;
                        });

                gurobiModel.AddConstr(constraint.Interval.LowerBound.Value, GRB.LESS_EQUAL, termsExpression,
                    constraint.Name ?? $"{cnt++}");
                gurobiModel.AddConstr(termsExpression, GRB.LESS_EQUAL, constraint.Interval.UpperBound.Value,
                    constraint.Name ?? $"{cnt++}");
            }

            gurobiModel.SetObjective(model.ObjectiveFunction.WeightedSum
                .Aggregate(
                    new GRBLinExpr(model.ObjectiveFunction.Offset?.Value ?? 0), (expression, term) =>
                    {
                        expression.AddTerm(term.Coefficient.Value, variables[term.Variable]);
                        return expression;
                    }), model.ObjectiveFunction.Maximize ? GRB.MAXIMIZE : GRB.MINIMIZE);

            gurobiModel.Optimize();


            if (solverParameter.ExportModelFilePath is not null)
            {
                Logger?.LogInformation("Exporting model to {ExportModelFilePath}", solverParameter.ExportModelFilePath);

                gurobiModel.Write(solverParameter.ExportModelFilePath);
            }

            if (gurobiModel.SolCount == 0)
                return ResultHandling.Handle<IIntegerVariable<IRealScalar>, RealScalar, IRealScalar>(gurobiModel.Status,
                    false);

            var solutionValues = new SolutionValues<IIntegerVariable<IRealScalar>, RealScalar, IRealScalar>(
                variables.ToDictionary(
                    variable => variable.Key,
                    variable => new RealScalar(gurobiModel.GetVarByName(variable.Key.Name).X)).AsReadOnly());

            return ResultHandling.HandleGurobi(gurobiModel.Status,
                solutionValues, gurobiModel.ObjVal,
                gurobiModel.ObjBound);
        }
        catch (Exception e)
        {
            Logger?.LogError(e, "An error occurred during solving the model: {EMessage}", e.Message);
            throw;
        }
    }
}