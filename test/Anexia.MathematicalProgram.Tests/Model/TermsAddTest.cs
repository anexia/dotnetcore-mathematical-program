// ------------------------------------------------------------------------------------------
//  <copyright file = "TermsAddTest.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH.All rights reserved.
//  </copyright>
//  ------------------------------------------------------------------------------------------

#region

using Anexia.MathematicalProgram.Model;
using Anexia.MathematicalProgram.Solve;
using Anexia.MathematicalProgram.SolverConfiguration;
using Anexia.MathematicalProgram.Tests.Factory;
using static Anexia.MathematicalProgram.Tests.Factory.TermFactory;

#endregion

namespace Anexia.MathematicalProgram.Tests.Model;

public sealed class TermsAddTest
{
    [Fact]
    public void ConstraintsWithDifferentTermsOrderMatch()
    {
        var solver = IntegerLinearProgramSolver.Create(IlpSolverType.CbcMixedIntegerProgramming, out _);

        _ = solver.AddIntegerVariable(IntervalFactory.Interval(0, 1), "1", out var variable1);
        _ = solver.AddIntegerVariable(IntervalFactory.Interval(0, 1), "2", out var variable2);
        _ = solver.AddIntegerVariable(IntervalFactory.Interval(0, 1), "3", out var variable3);

        var terms = new Terms();
        var term2V2 = Term(2, variable2);
        terms = terms.Add(Term(1, variable1));
        terms = terms.Add(term2V2);
        terms = terms.Add(Term(3, variable3));
        terms = terms.Add(Term(2, variable1));
        terms = terms.Add(term2V2);
        terms = terms.Add(Term(2, variable3));

        Assert.Equal(new Terms(Term(3, variable1), Term(4, variable2), Term(5, variable3)), terms);
    }

    [Fact]
    public void TermsEqualsTest()
    {
        var solver = IntegerLinearProgramSolver.Create(IlpSolverType.CbcMixedIntegerProgramming, out _);

        _ = solver.AddIntegerVariable(IntervalFactory.Interval(0, 1), "1", out var variable1);
        _ = solver.AddIntegerVariable(IntervalFactory.Interval(0, 1), "2", out var variable2);
        _ = solver.AddIntegerVariable(IntervalFactory.Interval(0, 1), "3", out var variable3);

        var terms2V2 = new Terms(Term(2, variable2), Term(3, variable3));
        var expectedResults = new object[][]
        {
            [true, false, true, false, false, false],
            [
                terms2V2.Equals(new Terms(Term(2, variable2), Term(3, variable3))),
                terms2V2.Equals(new Terms(Term(2, variable2))),
                terms2V2.Equals(new Terms(Term(3, variable3), Term(2, variable2))),
                terms2V2.Equals(new Terms(Term(3, variable3), Term(3, variable2))),
                terms2V2.Equals(new Terms(Term(3, variable3), Term(2, variable1))),
                terms2V2.Equals(new Terms(Term(3, variable3), Term(3, variable1))),
            ]
        };


        Assert.Equal(expectedResults[0], expectedResults[1]);
    }
}