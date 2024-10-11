// ------------------------------------------------------------------------------------------
//  <copyright file = "Point.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH.All rights reserved.
//  </copyright>
//  ------------------------------------------------------------------------------------------

namespace Anexia.MathematicalProgram.Model;

public sealed class Point : MemberwiseEquatable<Point>, IInterval
{
    public static readonly Point One = new(1);

    public Point(double value)
        : this(new LowerBound(value), new UpperBound(value))
    { }

    private Point(LowerBound lowerBound, UpperBound upperBound)
    {
        LowerBound = lowerBound;
        UpperBound = upperBound;
    }

    public LowerBound LowerBound { get; }
    public UpperBound UpperBound { get; }

    public override string ToString() => $"Point [{LowerBound},{UpperBound}]";
}