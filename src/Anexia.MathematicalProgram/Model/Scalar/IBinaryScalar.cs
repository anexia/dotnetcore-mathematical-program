// ------------------------------------------------------------------------------------------
//  <copyright file = "IBinaryScalar.cs" company = "Anexia Digital Engineering GmbH">
//  Copyright (c) Anexia Digital Engineering GmbH. All rights reserved.
//  </copyright>
// ------------------------------------------------------------------------------------------

namespace Anexia.MathematicalProgram.Model.Scalar;

/// <summary>
/// Represents a binary scalar (value restricted to 0 or 1).
/// </summary>
public interface IBinaryScalar : IIntegerScalar, IAddableScalar<IBinaryScalar, IBinaryScalar>
{
}