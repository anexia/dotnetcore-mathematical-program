// ------------------------------------------------------------------------------------------
//  <copyright file = "WarningMessage.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH.All rights reserved.
//  </copyright>
//  ------------------------------------------------------------------------------------------

namespace Anexia.MathematicalProgram.Model;

/// <summary>
/// Represents a warning message.
/// </summary>
/// <param name="message">A message.</param>
public sealed class WarningMessage(string message) : MemberwiseEquatable<WarningMessage>
{
    public string Message { get; } = message;

    /// <inheritdoc />
    public override string ToString() => $"{nameof(Message)}: {Message}";
}