// ------------------------------------------------------------------------------------------
//  <copyright file = "ConstraintsComparisonTest.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH.All rights reserved.
//  </copyright>
//  ------------------------------------------------------------------------------------------

#region

using Anexia.MathematicalProgram.Model;
using Anexia.MathematicalProgram.Solve;
using Anexia.MathematicalProgram.SolverConfiguration;
using Anexia.MathematicalProgram.Tests.Factory;
using static Anexia.MathematicalProgram.Tests.Factory.ConstraintFactory;
using static Anexia.MathematicalProgram.Tests.Factory.TermFactory;
#endregion

namespace Anexia.MathematicalProgram.Tests.Model;

public sealed class ConstraintsComparisonTest
{
    [Fact]
    public void ConstraintsWithDifferentTermsOrderMatch()
    {
        var solver = IntegerLinearProgramSolver.Create(IlpSolverType.CbcMixedIntegerProgramming, out _);

        _ = solver.AddIntegerVariable(IntervalFactory.Interval(0, 1), "1", out var variable1);
        _ = solver.AddIntegerVariable(IntervalFactory.Interval(0, 1), "2", out var variable2);
        _ = solver.AddIntegerVariable(IntervalFactory.Interval(0, 1), "3", out var variable3);

        var constraints = Constraints(Constraint([Term(1, variable1)], Point.One),
            Constraint([Term(1, variable2), Term(1, variable3)], Point.One),
            Constraint([Term(1, variable1), Term(1, variable3)], Point.One));

        var constraintDifferentTermsOrder = Constraints(Constraint([Term(1, variable1)], Point.One),
            Constraint([Term(1, variable3), Term(1, variable2)], Point.One),
            Constraint([Term(1, variable3), Term(1, variable1)], Point.One));

        Assert.Equal(constraintDifferentTermsOrder, constraints);
    }

    [Fact]
    public void ConstraintsWithDifferentConstraintOrderDoNotOrderMatch()
    {
        var solver = IntegerLinearProgramSolver.Create(IlpSolverType.CbcMixedIntegerProgramming, out _);

        _ = solver.AddIntegerVariable(IntervalFactory.Interval(0, 1), "1", out var variable1);
        _ = solver.AddIntegerVariable(IntervalFactory.Interval(0, 1), "2", out var variable2);
        _ = solver.AddIntegerVariable(IntervalFactory.Interval(0, 1), "3", out var variable3);

        var constraints = Constraints(Constraint([Term(1, variable1)], Point.One),
            Constraint([Term(1, variable2), Term(1, variable3)], Point.One),
            Constraint([Term(1, variable1), Term(1, variable3)], Point.One));

        var constraintDifferentTermsOrder = Constraints(Constraint([Term(1, variable1)], Point.One),
            Constraint([Term(1, variable3), Term(1, variable1)], Point.One),
            Constraint([Term(1, variable3), Term(1, variable2)], Point.One));

        Assert.NotEqual(constraintDifferentTermsOrder, constraints);
    }
}