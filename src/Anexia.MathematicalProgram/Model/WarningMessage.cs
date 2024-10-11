// ------------------------------------------------------------------------------------------
//  <copyright file = "WarningMessage.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH.All rights reserved.
//  </copyright>
//  ------------------------------------------------------------------------------------------

namespace Anexia.MathematicalProgram.Model;

public sealed class WarningMessage(string message) : MemberwiseEquatable<WarningMessage>
{
    public string Message { get; } = message;

    public override string ToString() => $"{nameof(Message)}: {Message}";
}