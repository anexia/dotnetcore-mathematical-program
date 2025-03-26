using System.Diagnostics.CodeAnalysis;
using Anexia.MathematicalProgram.Model;
using Anexia.MathematicalProgram.Model.Interval;
using Anexia.MathematicalProgram.Model.Scalar;
using Anexia.MathematicalProgram.Model.Variable;
using Anexia.MathematicalProgram.Solve;
using Anexia.MathematicalProgram.SolverConfiguration;

namespace Anexia.MathematicalProgram.Examples;

// Example: Feasible model: min 2x + y, s.t. x >= y, integer variables x in [1,3], y binary
[ExcludeFromCodeCoverage]
public static class IlpExample
{
    public static void Main()
    {
        // Create model containing integer variables with real interval boundaries and real coefficients.
        var model =
            new OptimizationModel<IIntegerVariable<IRealScalar>, RealScalar, IRealScalar>();

        // Create integer variable x in [1, 3]
        var x = model.NewVariable<IntegerVariable<IRealScalar>>(
            new IntegralInterval(new IntegerScalar(1), new IntegerScalar(3)), "x");

        // Create integer variable y in [0, 1]
        var y = model.NewVariable<IntegerVariable<IRealScalar>>(
            new IntegralInterval(0, 1), "y");

        // Create weighted sum 1*x - 1*y from collections.
        var xMinusY = model.CreateWeightedSumBuilder()
            .AddWeightedSum([x, y], [1, -1]).Build();

        // Create constraint: 0 <= x + y <= Inf
        var constraint = model.CreateConstraintBuilder()
            .AddWeightedSum(xMinusY)
            .Build(new RealInterval(0, double.PositiveInfinity));

        model.AddConstraint(constraint);

        // Create objective function: min 2x + 2y
        var objFunction = model.CreateObjectiveFunctionBuilder().AddTermToSum(2, x)
            .AddTermToSum(2, y).Build(false);

        // Set objective function and retrieve a completed model that can be passed to the solver.
        var optimizationModel = model.SetObjective(objFunction);

        // Create SCIP Solver and solve model. Different settings can be set via out parameters.
        var result = SolverFactory.SolverFor(IlpSolverType.HiGhs).Solve(optimizationModel,
            new SolverParameter(new EnableSolverOutput(false), RelativeGap.EMinus7,
                new TimeLimitInMilliseconds(10000), new NumberOfThreads(2), ExportModelFilePath: "model.txt"));

        Console.WriteLine(result);
        // Output: ObjectiveValue: 2, IsFeasible: True, IsOptimal: True, OptimalityGap: 0 
        //         Variable Values: x=1, y=-0


        // Pass MPS model generated above to solver.
        var resultFromModel = new IlpSolver(IlpSolverType.Scip).Solve(
            new ModelAsMpsFormat(File.ReadAllText("model.txt")), new SolverParameter());

        // Prints same result as above.
        Console.WriteLine(resultFromModel);

        // Print optimization model
        Console.WriteLine(optimizationModel.ToString());
    }
}