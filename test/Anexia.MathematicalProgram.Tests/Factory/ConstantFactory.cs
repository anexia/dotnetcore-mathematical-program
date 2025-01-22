// ------------------------------------------------------------------------------------------
//  <copyright file = "ConstantFactory.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH.All rights reserved.
//  </copyright>
//  ------------------------------------------------------------------------------------------

#region

using Anexia.MathematicalProgram.Model;

#endregion

namespace Anexia.MathematicalProgram.Tests.Factory;

public static class ConstantFactory
{
    public static Constant Constant(double constant) => new(constant);
}