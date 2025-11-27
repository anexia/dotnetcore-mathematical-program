# dotnet-mathematical-program

[![](https://img.shields.io/nuget/v/Anexia.MathematicalProgram "NuGet version badge")](https://www.nuget.org/packages/Anexia.MathematicalProgram)
[![](https://github.com/anexia/dotnetcore-mathematical-program/actions/workflows/test.yml/badge.svg?branch=main "Test status")](https://github.com/anexia/dotnetcore-mathematical-program/actions/workflows/test.yml)
[![codecov.io](https://codecov.io/github/Anexia/dotnetcore-mathematical-program/coverage.svg?branch=main "Code coverage")](https://codecov.io/github/anexia/dotnetcore-mathematical-program/coverage.svg?branch=main)
This library allows you to build and solve linear programs and integer linear programs in a very handy way.
For linear programs, either [SCIP](https://www.scipopt.org/) or Google's [GLOP](https://developers.google.com/optimization/lp/lp_example) solver can be used.
For integer linear programs, SCIP, Gurobi and the Coin-OR CBC branch and cut
solver can be chosen. When the desired solver is not available, i.e., no licence for Gurobi could be found, SCIP is used
as fallback. CBC Solver is marked deprecated, SCIP should be used instead.

## Installation

- Install the latest version of `Anexia.MathematicalProgram` package via nuget

## Description

This library works for any linear program (LP), integer linear program (ILP) or constraint program (CP).

### Anexia.MathematicalProgram.Result

After solving the LP/ILP you get a `SolverResult` according to the `Google.OrTools.LinearSolver.Solver.ResultStatus`.
The `SolverResult` contains the following information:

- **SolutionValues:** You can read out the actual values of the variables to transform the result correctly.
- **ObjectiveValue:** Actual objective value. This value can be either the optimum, a deviation of the optimum
  if the LP/ILP was not entirely solved, or `null` if the LP/ILP is infeasible.
- **IsFeasible:** Information whether the LP/ILP is generally feasible.
- **IsOptimal:** Information whether the LP/ILP was solved to optimality.
- **OptimalityGap:** The deviation to the optimum calculated by
  `Math.Abs(bestObjectiveBound - objectiveValue) / objectiveValue)`. If the objective value and the best bound are zero, 
   the gap is also set to zero. If the objective value is zero but the best bound is not, the gap is defined to be +/- infinity.

***

## Examples for using this library

### Example (Build and solve ILP)

- Feasible model: min 2x + y, s.t. x >= y, integer variables x in [1,3], y binary
- Result: x = 1, objective value = 2

```
var model = new OptimizationModel<IIntegerVariable<IRealScalar>, RealScalar, IRealScalar>();

var x = model.NewVariable<IntegerVariable<IRealScalar>>(new IntegralInterval(1, 3), "x");

var y = model.NewVariable<IntegerVariable<IRealScalar>>(new IntegralInterval(0, 1), "y");

var constraint = model.ConstraintBuilder()
                      .AddWeightedSum([x, y], [1, -1])
                      .Build(new RealInterval(0, double.PositiveInfinity));

model.AddConstraint(constraint);

var objFunction = model.TermsBuilder()
                       .AddTerm(2, x)
                       .AddTerm(2, y).Build()
                       .ToObjectiveFunction(false);
var optimizationModel = model.SetObjective(objFunction);

var result = SolverFactory.SolverFor(IlpSolverType.Scip).Solve(optimizationModel,
            new SolverParameter(new EnableSolverOutput(true)),
            out _);
```

Further detailed examples can be found in the [examples folder](examples).

## Solver parameters (SolverParameter)

You can control solver behavior using the SolverParameter record in Anexia.MathematicalProgram.SolverConfiguration. Common fields:

- EnableSolverOutput: toggles solver console logs.
- TimeLimitInMilliseconds: overall time limit.
- NumberOfThreads: caps thread usage when supported by the solver.
- RelativeGap: early stopping gap (when supported by the solver).
- AdditionalSolverSpecificParameters: extra key/value pairs passed straight to the underlying solver.
- ExportModelFilePath: path to export the model (MPS or solver-specific format depending on backend).

Examples:

Use with native Gurobi API (GurobiNativeSolver):
```
var native = new GurobiNativeSolver();
var result = native.Solve(optimizationModel,
    new SolverParameter(
        new EnableSolverOutput(true),
        NumberOfThreads: new NumberOfThreads(8),
        RelativeGap: RelativeGap.EMinus7,
        AdditionalSolverSpecificParameters: new[]
        {
            ("MIPFocus", "1"),
            ("Heuristics", "0.05")
        },
        ExportModelFilePath: "model.mps"
    )
);
```

Notes:
- For Gurobi parameters, see https://docs.gurobi.com/projects/optimizer/en/current/reference/parameters.html#secparameterreference
- The AdditionalSolverSpecificParameters are forwarded as-is.
- NumberOfThreads, TimeLimitInMilliseconds, and RelativeGap is mapped to the solver’s native time limit.

## Contributing

Contributions are welcomed! Read the [Contributing Guide](CONTRIBUTING.md) for more information.

## Licensing

This project is licensed under MIT License. See [LICENSE](LICENSE) for more information.
