using System.Diagnostics.CodeAnalysis;
using Anexia.MathematicalProgram.Model;
using Anexia.MathematicalProgram.Model.Interval;
using Anexia.MathematicalProgram.Model.Scalar;
using Anexia.MathematicalProgram.Model.Variable;
using Anexia.MathematicalProgram.Result;
using Anexia.MathematicalProgram.Solve;
using Anexia.MathematicalProgram.SolverConfiguration;

namespace Anexia.MathematicalProgram.Examples;

// Example: Feasible model: min 2x + y, s.t. x >= y, integer variables x in [1,3], y binary
// Cp model can also be passed to ILP solver. ILP model can only be passed to CP solver, when all typed are integer.
[ExcludeFromCodeCoverage]
public static class CpExample
{
    public static void Main()
    {
        BasicCpExample();
        CallbackCpExample();
    }

    public static void BasicCpExample()
    {
        // Create model containing integer variables with real interval boundaries and real coefficients.
        var model =
            new OptimizationModel<IIntegerVariable<IIntegerScalar>, IntegerScalar, IIntegerScalar>();

        // Create integer variable x in [1, 3]
        var x = model.NewVariable<IntegerVariable<IIntegerScalar>>(
            new IntegralInterval(new IntegerScalar(1), new IntegerScalar(3)), "x");

        // Create binary variable y in [0, 1]
        var y = model.NewBinaryVariable<BinaryVariable>("y");

        // Create constraint: 0 <= x + y <= 10
        var constraint = model.CreateConstraintBuilder().AddTermToSum(1, x)
            .AddTermToSum(-1, y).Build(new IntegralInterval(0, 10));

        model.AddConstraint(constraint);

        // Create objective function: max 2x + 2y
        var objFunction = model.CreateObjectiveFunctionBuilder().AddTermToSum(2, x)
            .AddTermToSum(2, y).Build();

        // Set objective function and retrieve a completed model that can be passed to the solver.
        var optimizationModel = model.SetObjective(objFunction);

        // Create CP Solver and solve model. Output is enabled using solver parameter.
        var resultCp = SolverFactory.NewCpSolver().Solve(optimizationModel,
            new SolverParameter(new EnableSolverOutput(true)));

        // Passing CP model to ILP solver yields same result

        // Create SCIP Solver and solve model. Output is enabled using solver parameter.
        var resultScip = SolverFactory.SolverFor(IlpSolverType.Scip).Solve(optimizationModel,
            new SolverParameter(new EnableSolverOutput(true)));


        Console.WriteLine("------ CP -------");
        Console.WriteLine(resultCp);
        // Output: ObjectiveValue: 8, IsFeasible: True, IsOptimal: True, OptimalityGap: 0 
        //         Variable Values: x=1, y=3

        Console.WriteLine("------ SCIP -------");
        Console.WriteLine(resultScip);
        // Output: ObjectiveValue: 2, IsFeasible: True, IsOptimal: True, OptimalityGap: 0 
        //         Variable Values: x=1, y=-0
    }


    public static void CallbackCpExample()
    {
        // Create model containing integer variables with real interval boundaries and real coefficients.
        var model =
            new OptimizationModel<IIntegerVariable<IIntegerScalar>, IntegerScalar, IIntegerScalar>();

        // Create integer variable x in [0, 2]
        var x = model.NewVariable<IntegerVariable<IIntegerScalar>>(
            new IntegralInterval(0, 2), "x");

        // Create integer variable y in [0, 2]
        var y = model.NewVariable<IntegerVariable<IIntegerScalar>>(
            new IntegralInterval(0, 2), "y");

        // Create constraint: 0 <= x + y <= 3
        var constraint = model.CreateConstraintBuilder().AddTermToSum(1, x)
            .AddTermToSum(1, y).Build(new IntegralInterval(0, 3));

        model.AddConstraint(constraint);

        // Create objective function: max 2x
        var objFunction = model.CreateObjectiveFunctionBuilder().AddTermToSum(2, x)
            .AddTermToSum(2, y).Build();

        // Set objective function and retrieve a completed model that can be passed to the solver.
        var optimizationModel = model.SetObjective(objFunction);

        // Create CP Solver and solve model. Output is enabled using solver parameter.
        var resultCp = new ConstraintProgrammingSolver().Solve(optimizationModel,
            new SolverParameter(new EnableSolverOutput(true)),
            new SolutionCallback(), false);

        Console.WriteLine(resultCp);
        // Output: ObjectiveValue: 2, IsFeasible: True, IsOptimal: True, OptimalityGap: 0 
        //         Variable Values: x=1, y=-0
    }
}

internal sealed class SolutionCallback : ICpSolutionCallback
{
    public void OnSolutionCallback(
        SolutionValues<IIntegerVariable<IIntegerScalar>, IntegerScalar, IIntegerScalar> solutionValues)
    {
        Console.WriteLine($"New solution found: {solutionValues}");
    }
}