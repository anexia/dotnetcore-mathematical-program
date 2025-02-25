// ------------------------------------------------------------------------------------------
//  <copyright file = "NumberOfWeightsNotEqualToNumberOfVariablesException.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH. All rights reserved.
//  </copyright>
// ------------------------------------------------------------------------------------------


namespace Anexia.MathematicalProgram.Model.Expression;

public sealed class NumberOfWeightsNotEqualToNumberOfVariablesException(string message) : Exception(message)
{
    public NumberOfWeightsNotEqualToNumberOfVariablesException(int varsArrayLength, int weightsArrayLength) : this(
        $"Number of weights must match number of variables. Weights: {weightsArrayLength}, Variables: {varsArrayLength}")
    {
    }
}