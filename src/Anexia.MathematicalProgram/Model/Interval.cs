// ------------------------------------------------------------------------------------------
//  <copyright file = "Interval.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH.All rights reserved.
//  </copyright>
//  ------------------------------------------------------------------------------------------

namespace Anexia.MathematicalProgram.Model;

public sealed class Interval : MemberwiseEquatable<Interval>, IInterval
{
    public static readonly Interval BinaryInterval = new(0, 1);

    public Interval(LowerBound lowerBound, UpperBound upperBound)
    {
        if (!lowerBound.IsLessThanOrEqualTo(upperBound)) throw new InadmissibleBoundsException(lowerBound, upperBound);
        LowerBound = lowerBound;
        UpperBound = upperBound;
    }

    public Interval(double lowerBound, double upperBound)
        : this(new LowerBound(lowerBound), new UpperBound(upperBound))
    {
    }

    public LowerBound LowerBound { get; }
    public UpperBound UpperBound { get; }

    public override string ToString() => $"Interval [{LowerBound},{UpperBound}]";
}