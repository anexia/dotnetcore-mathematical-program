// ------------------------------------------------------------------------------------------
//  <copyright file = "MPaaSExamples.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH. All rights reserved.
//  </copyright>
// ------------------------------------------------------------------------------------------

using Anexia.MathematicalProgram.Model;
using Anexia.MathematicalProgram.Model.Interval;
using Anexia.MathematicalProgram.Model.Scalar;
using Anexia.MathematicalProgram.Model.Variable;
using Anexia.MathematicalProgram.MPaaS.Client;

namespace Anexia.MathematicalProgram.Examples;

public static class MPaaSExamples
{
    public static async Task Main()
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
            .Build(new RealInterval(0, double.MaxValue));

        model.AddConstraint(constraint);

        // Create objective function: min 2x + 2y
        var objFunction = model.CreateObjectiveFunctionBuilder().AddTermToSum(2, x)
            .AddTermToSum(2, y).Build(false);

        // Set objective function and retrieve a completed model that can be passed to the solver.
        var optimizationModel = model.SetObjective(objFunction);
        
        // This code assumes you have a running MPaaS instance listing on localhost:5116.
        var mpaasConfig = new MPaaSClientConfig(TimeSpan.FromMinutes(2));
        var mpaasClient = new MPaaSClient(new Uri("http://localhost:5116"), mpaasConfig, new HttpClient());
        var response = await mpaasClient.SolveAsync(optimizationModel);
        
        Console.WriteLine(response);
    }
}