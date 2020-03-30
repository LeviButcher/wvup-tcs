#pragma warning disable 1591

using System;
using System.Text;

public static class StringExtensions {
    public static string ToBase64 (this string text) {
        return ToBase64 (text, Encoding.UTF8);
    }

    public static string ToBase64 (this string text, Encoding encoding) {
        byte[] textAsBytes = encoding.GetBytes (text);
        return Convert.ToBase64String (textAsBytes);
    }
}