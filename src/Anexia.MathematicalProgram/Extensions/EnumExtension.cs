// ------------------------------------------------------------------------------------------
//  <copyright file = "EnumExtension.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH.All rights reserved.
//  </copyright>
//  ------------------------------------------------------------------------------------------

#region

using System.Runtime.Serialization;

#endregion

namespace Anexia.MathematicalProgram.Extensions;

internal static class EnumExtension
{
    internal static string ToEnumString<T>(this T type) where T : Enum
    {
        var enumType = typeof(T);
        var name = Enum.GetName(enumType, type);

        if (name is null) return string.Empty;

        var fieldInfo = enumType.GetField(name);

        if (fieldInfo is null) return string.Empty;

        var enumMemberAttribute =
            ((EnumMemberAttribute[])fieldInfo.GetCustomAttributes(typeof(EnumMemberAttribute), true)).FirstOrDefault();

        return enumMemberAttribute?.Value ?? string.Empty;
    }
}