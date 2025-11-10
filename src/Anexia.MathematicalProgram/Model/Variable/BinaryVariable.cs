// ------------------------------------------------------------------------------------------
//  <copyright file = "BinaryVariable.cs" company = "Anexia Digital Engineering GmbH">
//  Copyright (c) Anexia Digital Engineering GmbH. All rights reserved.
//  </copyright>
// ------------------------------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Anexia.MathematicalProgram.Model.Interval;
using Anexia.MathematicalProgram.Model.Scalar;

namespace Anexia.MathematicalProgram.Model.Variable;

/// <summary>
/// Represents a binary variable in the domain {0,1}.
/// </summary>
public sealed class BinaryVariable : MemberwiseEquatable<BinaryVariable>,
    ICreatableVariable<BinaryVariable, IBinaryScalar>,
    IBinaryVariable<IBinaryScalar>
{
    public BinaryVariable(string name) : this(new BinaryInterval(), name)
    {
    }

    internal BinaryVariable(IInterval<IBinaryScalar> interval, string name)
    {
        Name = name;
        Interval = interval;
    }

    public IInterval<IBinaryScalar> Interval { get; }

    /// <summary>
    /// The name.
    /// </summary>
    public string Name { get; }

    /// <inheritdoc />
    public static BinaryVariable Create(IInterval<IBinaryScalar> interval, string name)
        => new(interval, name);

    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public override string ToString() => $"{Interval.LowerBound} <= {Name} <= {Interval.UpperBound} (binary)";
}