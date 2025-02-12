using System.Diagnostics.CodeAnalysis;
using Anexia.MathematicalProgram.Model;
using Anexia.MathematicalProgram.Model.Interval;
using Anexia.MathematicalProgram.Model.Scalar;
using Anexia.MathematicalProgram.Model.Variable;
using Anexia.MathematicalProgram.Solve;
using Anexia.MathematicalProgram.SolverConfiguration;

namespace Anexia.MathematicalProgram.Examples;

// Example: Feasible model: max x, s.t. x <= 2.5, x >= 1, continuous variable x <= 5
[ExcludeFromCodeCoverage]
public static class LpExample
{
    public static void Main()
    {
        // Create model containing continuous variables with real interval boundaries and real coefficients.
        var model =
            new OptimizationModel<ContinuousVariable<IRealScalar>, RealScalar, IRealScalar>();

        // Create integer variable x in [-Inf, 5]
        var x = model.NewVariable<ContinuousVariable<IRealScalar>>(
            new RealInterval(double.NegativeInfinity, 5), "x");

        // Create constraint: -Inf <= x <= 5
        var constraint1 = model.CreateConstraintBuilder().AddTermToSum(1, x)
            .Build(new RealInterval(double.NegativeInfinity, 2.5));

        // Create constraint: 1 <= x <= Inf
        var constraint2 = model.CreateConstraintBuilder()
            .AddTermToSum(1, x).Build(new RealInterval(1, double.PositiveInfinity));

        model.AddConstraint(constraint1);
        model.AddConstraint(constraint2);

        // Create objective function: max x
        var objFunction = model.CreateObjectiveFunctionBuilder().AddTermToSum(1, x).Build();

        // Set objective function and retrieve a completed model that can be passed to the solver.
        var optimizationModel = model.SetObjective(objFunction);

        // Create SCIP Solver and solve model. Output is enabled using solver parameter.
        var result = SolverFactory.SolverFor(LpSolverType.Glop).Solve(optimizationModel,
            new SolverParameter(new EnableSolverOutput(true)));

        Console.WriteLine(result);
        // Output: ObjectiveValue: 2, IsFeasible: True, IsOptimal: True, OptimalityGap: 0 
        //         Variable Values: x=1, y=-0
    }
}