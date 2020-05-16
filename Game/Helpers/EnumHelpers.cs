using System;

namespace Game.Helpers
{
    public static class EnumHelpers
    {
        public static T? ParseEnumOrNull<T>(this string text, bool ignoringCase) where T : struct, System.Enum
        {
            return (T?) (Enum.TryParse(typeof(T), text, ignoringCase, out var result) ? result : null);
        }
    }
}