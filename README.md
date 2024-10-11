# dotnet-mathematical-program
==========

[![](https://img.shields.io/nuget/v/Anexia.MathematicalProgram "NuGet version badge")](https://www.nuget.org/packages/Anexia.MathematicalProgram)
[![](https://github.com/anexia/dotnetcore-mathematical-program/actions/workflows/test.yml/badge.svg?branch=main "Test status")](https://github.com/anexia/dotnetcore-mathematical-program/actions/workflows/test.yml)
[![codecov.io](https://codecov.io/github/Anexia/dotnetcore-mathematical-program/coverage.svg?branch=main "Code coverage")](https://codecov.io/github/Anexia/dotnetcore-mathematical-program/coverage.svg?branch=main)
This library allows you to build and solve linear programs and integer linear programs in a very handy way.
The implementation uses Google´s GLOP linear solver for linear programs and optionally, the Coin-OR CBC branch and cut
solver
or the Gurobi solver for integer linear programs via the [Google OR-Tools](https://developers.google.com/optimization)
API.

## Installation

- Install the latest version of `Anexia.MathematicalProgram` package via nuget

## Description

This library works for any linear program (LP) or integer linear program (ILP).

### Anexia.MathematicalProgram.Model

- To build the objective function of your LP/ILP you can use the class `Terms`.
  Each `Term` is defined by a `Coefficient` and a variable of type `Google.OrTools.LinearSolver.Variable`.
  Moreover, there is the possibility to have an additional `Constant`.

- To build your constraints you can use the class `Constraints`.
  Each `Constraint` is defined by `Terms` and an `Interval` that has a lower and an upper bound of
  type `double`.
  For binary intervals simply use `Interval.BinaryInterval`.
  Another implementation is the class `Point` for the case of lower bound equals upper bound.

### Anexia.MathematicalProgram.Solve

#### Linear Programming

For solving an LP you may initialize the `LinearProgramSolver` which uses the GLOP solver in the background.

- **Configuration:** Via `LinearProgramSolver.SetSolverConfigurations()` you can set `SolverParameter` containing a `TimeLimitInMilliseconds`,
  the `NumberOfThreads` that should be maximally used, a `EnableSolverOutput` flag to determine if the solver output should be
  printed on the console and the `RelativeGap` to specify the gap where the solver terminates.
- **Variables:** Via `LinearProgramSolver.AddContinuousVariable()` your continuous LP variables can be added
  using an `IInterval` and a variable name of type `string`.
  The `out`parameter of this method is of type `Google.OrTools.LinearSolver.Variable`.
- **Constraints:** Via `LinearProgramSolver.AddConstraints()` you can simply add your beforehand initialized
  contraints to the solver.
- **Objective:** Via `LinearProgramSolver.AddObjective()` you can add your objective in form of the `Terms`
  and a `Constant` to the solver.
  With a `bool` you can choose if the LP should be `minimized` or `maximized`.
- **Solve:** Via `LinearProgramSolver.Solve()` you either
    - obtain a `SolverResult` (explained below) or
    - a `MathematicalProgramException` with a detailed message on the occured problem is thrown.

#### Integer Linear Programming

For solving an ILP you may initialize the `IntegerLinearProgramSolver` which uses per default the CBC solver in the
background.
Using the highly performant Gurobi solver requires a valid license.
Creating a solver using Gurobi can be done in two ways. Either by passing the argument
`IntegerLinearProgramSolver(ILPSolverType.GurobiMixedIntegerProgramming)`,
or using the static
method `IntegerLinearProgramSolver.Create(ILPSolverType.GurobiMixedIntegerProgramming, our var message)`.
In both ways, the solver checks if the given type is supported, e.g., a valid licence is present, or otherwise, creates
the solver of type CBC. The `Create` method additionally returns a warning message via out parameter.
If the solver has been created as expected, this message is null, otherwise, it contains information that the
solver type switched to CBC.

The main difference to linear program solving is that in this case just integer variables can be added.
The rest of the methods work similar to the LinearProgramSolver.

- **Variables:** Via `IntegerLinearProgramSolver.AddIntegerVariable()` your integer ILP variables can be added using
  an `IInterval` and a variable name of type `string`. The `out`parameter of this method is of type
  `Google.OrTools.LinearSolver.Variable` which are strictly integer.
- **Configuration**, **Constraints**, **Objective**, and **Solve** as above.

### Anexia.MathematicalProgram.Result

After solving the LP/ILP you get a `SolverResult` according to the `Google.OrTools.LinearSolver.Solver.ResultStatus`.
The `SolverResult` containts following information:

- **Solver:** This is the already solved `Google.OrTools.LinearSolver.Solver`.
    - You have the opportunity to log the LP/ILP model in a human readable format by `Solver.ExportModelAsLpFormat()`.
    - You can read out the actual values of the variables via `Google.OrTools.LinearSolver.Variable.SolutionValue()`
      to transform the result correctly.
    - As soon as the solved solver is not needed any more, it should be removed via `Solver.Dispose()`.
- **ObjectiveValue:** Actual objective value. This value can be either the optimum, a deviation of the optimum
  if the LP/ILP was not entirely solved, or `double.NaN` if the LP/ILP is infeasible.
- **IsFeasible:** Information whether the LP/ILP is generally feasible.
- **IsOptimal:** Information wheter the LP/ILP was solved to optimality.
- **OptimalityGap:** The deviation to the optimum calculated by
  `Math.Abs(objective.BestBound() - objectiveValue) / objectiveValue)`. This value is `0` if the optimum was reached,
  and `double.NaN` if the model is infeasible.

***

## Examples for using this library

### Example 1 (Build and solve LP)

- Feasible model: max x, s.t. x <= 2, x >= 1, continuous variable x <= 5
- Result: x = 2, objective value = 2

```
var solver = new LinearProgramSolver().SetSolverConfigurations(TimeLimitInMilliseconds.Unbounded);

solver = solver.AddContinuousVariable(new Interval(double.NegativeInfinity, 5), "TestVariable", out var testVariable);

solver = solver.AddObjective(new Terms(new Term(new Coefficient(1), testVariable)), false);

var constraints = new Constraints(
            new Constraint(new Terms(new Term(new Coefficient(1), testVariable)),
                new Interval(double.NegativeInfinity, 2)),
            new Constraint(new Terms(new Term(new Coefficient(1), testVariable)),
                new Interval(1, double.PositiveInfinity)));

solver = solver.AddConstraints(constraints);

var result = solver.Solve();

Logger.Information(result.SolvedSolver.ExportModelAsLpFormat(false));
```

### Example 2 (Build and solve ILP)

- Feasible model: min 2x + y, s.t. x >= y, integer variables x in [1,3], y binary
- Result: x = 1, y = 0, objective value = 2

```
var solver = new IntegerLinearProgramSolver()
            .SetSolverConfigurations(new TimeLimitInMilliseconds(10), 2, true)
            .AddIntegerVariable(new Interval(1, 3), "VariableX", out var variableX)
            .AddIntegerVariable(new Interval(0, 1), "VariableY", out var variableY)
            .AddObjective(
                new Terms(new Term(new Coefficient(2), variableX), new Term(new Coefficient(1), variableY)), true)
            .AddConstraints(new Constraints(new Constraint(
                    new Terms(new Term(new Coefficient(1), variableX), new Term(new Coefficient(-1), variableY)),
                    new Interval(0, double.PositiveInfinity))));

 var result = solver.Solve();
```

### Example 3 (Build and solve ILP)

- Infeasible model: max 2x, s.t. x = 3, variable x binary

```
var solver = new IntegerLinearProgramSolver()
            .SetSolverConfigurations(TimeLimitInMilliseconds.Unbounded, 2, true)
            .AddIntegerVariable(Interval.BinaryInterval, "TestVariable", out var testVariable)
            .AddObjective(new Terms(new Term(new Coefficient(2), testVariable)), false)
            .AddConstraints(
                new Constraints(new Constraint(new Terms(new Term(new Coefficient(1), testVariable)), new Point(3))));

 var result = solver.Solve();

 Logger.Information(result.SolvedSolver.ExportModelAsLpFormat(false));
```


## Contributing

Contributions are welcomed! Read the [Contributing Guide](CONTRIBUTING.md) for more information.

## Licensing

This project is licensed under MIT License. See [LICENSE](LICENSE) for more information.



