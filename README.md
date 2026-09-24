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

## Warm start (MIP start) and variable hints

A feasible (or partial) start solution can be attached to a completed model with `WithWarmStart`. The values are typed
like the variable intervals (`IRealScalar` for ILP models), so integer, binary and real scalars are accepted.

| Solver                                | Handling of the warm start                                        |
|---------------------------------------|-------------------------------------------------------------------|
| Gurobi (native, `GurobiNativeSolver`) | MIP start via the `Start` attribute                               |
| Gurobi via OR-Tools                   | OR-Tools solution hint, forwarded to Gurobi as MIP start          |
| SCIP                                  | OR-Tools solution hint, added as (partial) solution               |
| CP-SAT                                | Solution hint                                                     |
| HiGHS                                 | Ignored with a warning, see note below                            |
| CBC                                   | Ignored                                                           |

```
var warmStart = new WarmStart<IIntegerVariable<IRealScalar>, IRealScalar>()
    .Add(x, new IntegerScalar(1))
    .Add(y, BinaryScalar.Zero);

var result = SolverFactory.SolverFor(IlpSolverType.GurobiNativeIntegerProgramming)
    .Solve(optimizationModel.WithWarmStart(warmStart), new SolverParameter());

// Re-solve with the previous solution as start solution:
var warmStartFromResult = ((SolutionValues<IIntegerVariable<IRealScalar>, RealScalar, IRealScalar>)result.SolutionValues)
    .ToWarmStart();
```

Note on HiGHS: OR-Tools (up to and including 9.14) sizes the hint arrays for HiGHS incorrectly and writes out of
bounds, which crashes the process. Warm starts are therefore not forwarded to HiGHS, also when HiGHS is used as fallback.

Solver specific per-variable attributes (Gurobi's `VarHintVal`, `VarHintPri` and `BranchPriority`) can be attached with
`WithVariableAttributes`. They are applied by the native Gurobi solver only; all other solvers log and ignore them.

```
var attributes = new VariableAttributes<IIntegerVariable<IRealScalar>>()
    .Add(x, VariableAttributeType.HintValue, 1)
    .Add(x, VariableAttributeType.HintPriority, 5)
    .Add(y, VariableAttributeType.BranchPriority, 10);

var result = new GurobiNativeSolver().Solve(optimizationModel.WithVariableAttributes(attributes), new SolverParameter());
```

Both `WithWarmStart` and `WithVariableAttributes` throw a `VariableNotInModelException` when a referenced variable is not
part of the model.

## Contributing

Contributions are welcomed! Read the [Contributing Guide](CONTRIBUTING.md) for more information.

## Licensing

This project is licensed under MIT License. See [LICENSE](LICENSE) for more information.
