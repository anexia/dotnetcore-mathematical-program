// ------------------------------------------------------------------------------------------
//  <copyright file = "ModelConverter.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH. All rights reserved.
//  </copyright>
// ------------------------------------------------------------------------------------------

using Anexia.MathematicalProgram.Model;
using Anexia.MathematicalProgram.Model.Expression;
using Anexia.MathematicalProgram.Model.Interval;
using Anexia.MathematicalProgram.Model.Scalar;
using Anexia.MathematicalProgram.Model.Variable;
using Anexia.MathematicalProgram.MPaaS.Model;
using Anexia.MathematicalProgram.Result;

namespace Anexia.MathematicalProgram.MPaaS.Client;

public static class ModelConverter
{
    public static PostJobsRequest ToMPaasModel(
        this ICompletedOptimizationModel<IIntegerVariable<IRealScalar>, RealScalar, IRealScalar> model)
    {
        return new PostJobsRequest(model.Variables.Select(ToMPaaSVariable).ToArray(),
            model.Constraints.Select(ToMPaasConstraint).ToArray(), model.ObjectiveFunction.ToMPaasObjective());
    }

    public static Variable ToMPaaSVariable(this IIntegerVariable<IRealScalar> variable) => new(variable.Name,
        variable.Interval.LowerBound.Value, variable.Interval.UpperBound.Value);

    public static LinearExpression ToMPaaSLinearExpression(
        this ITerm<IIntegerVariable<IRealScalar>, RealScalar, IRealScalar> term) => new(
        term.Coefficient.Value, term.Variable.Name);

    public static Objective ToMPaasObjective(
        this IObjectiveFunction<IIntegerVariable<IRealScalar>, RealScalar, IRealScalar> objective) => new(
        objective.WeightedSum.Select(ToMPaaSLinearExpression).ToArray(), objective.Maximize,
        objective.Offset?.Value);

    public static Constraint ToMPaasConstraint(
        IConstraint<IIntegerVariable<IRealScalar>, RealScalar, IRealScalar> constraint) => new(
        constraint.WeightedSum.Select(ToMPaaSLinearExpression).ToArray(),
        constraint.Interval.LowerBound.Value,
        constraint.Interval.UpperBound.Value);

    public static GetSolutionResponse ToMPaaSSolution(
        this ISolverResult<IIntegerVariable<IRealScalar>, RealScalar, IRealScalar> result,
        IEnumerable<IIntegerVariable<IRealScalar>> variables) => new(
        result.IsFeasible.Value,
        result.OptimalityGap?.Value,
        result.ObjectiveValue?.Value,
        result.IsOptimal.Value,
        Enum.GetName(result.SolverResultStatus),
        variables.ToDictionary(variable => variable.Name,
            variable => result.SolutionValues.GetSolutionValueOrDefault(variable)?.Value));

    public static ISolverResult<IIntegerVariable<IRealScalar>, RealScalar, IRealScalar> ToSolverResult(
        GetSolutionResponse solution,
        ICompletedOptimizationModel<IIntegerVariable<IRealScalar>, RealScalar, IRealScalar> model)
    {
        var variablesByName = model.Variables.ToDictionary(variable => variable.Name);

        var solutionValues = solution.VariableValues.ToDictionary(nameAndValue => variablesByName[nameAndValue.Key],
            pair => new RealScalar(pair.Value ?? double.NaN));

        var solverResultStatus = solution.SolverResultStatus != null
            ? Enum.Parse<SolverResultStatus>(solution.SolverResultStatus)
            : SolverResultStatus.UnknownStatus;

        var objectiveValue = solution.ObjectiveValue is null ? null : new ObjectiveValue(solution.ObjectiveValue.Value);

        var result = new SolverResult<IIntegerVariable<IRealScalar>, RealScalar, IRealScalar>(
            new SolutionValues<IIntegerVariable<IRealScalar>, RealScalar, IRealScalar>(solutionValues), objectiveValue,
            new IsFeasible(solution.IsFeasible),
            new IsOptimal(solution.IsOptimal), new OptimalityGap(solution.Gap ?? double.MaxValue), solverResultStatus,
            false);

        return result;
    }

    public static ICompletedOptimizationModel<IIntegerVariable<IRealScalar>, RealScalar, IRealScalar>
        ToOptimizationModel(this PostJobsRequest request)
    {
        var model =
            new OptimizationModel<IIntegerVariable<IRealScalar>, RealScalar, IRealScalar>();

        var variables = request.Variables.ToDictionary(variable => variable.Name, variable =>
            model.NewVariable<IntegerVariable<IRealScalar>>(
                new IntegralInterval(new IntegerScalar((long?)variable.LowerBound ?? long.MinValue),
                    new IntegerScalar((long?)variable.UpperBound ?? long.MaxValue)), variable.Name));

        foreach (var constraint in request.Constraints)
        {
            var weightedSum = constraint.Terms.Aggregate(
                new WeightedSum<IIntegerVariable<IRealScalar>, RealScalar, IRealScalar>(),
                (sum, linearExpression) => sum.Add(variables[linearExpression.Variable],
                    new RealScalar(linearExpression.Coefficient)));

            model.AddConstraint(model.CreateConstraintBuilder().AddWeightedSum(weightedSum)
                .Build(new RealInterval(constraint.LowerBound ?? double.NegativeInfinity,
                    constraint.UpperBound ?? double.PositiveInfinity)));
        }

        return model.SetObjective(request.Objective.Expression.Aggregate(model.CreateObjectiveFunctionBuilder(),
                (builder, expression) =>
                    builder.AddTermToSum(new RealScalar(expression.Coefficient), variables[expression.Variable]))
            .Build(request.Objective.Maximize));
    }
}