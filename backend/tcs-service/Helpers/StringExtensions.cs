using System;
using System.Text;

namespace tcs_service.Helpers
{
    ///<summary>StringExtensions</summary>
    public static class StringExtensions
    {
        ///<summary>Return the Base64 representation of the string</summary>
        public static string ToBase64(this string text)
        {
            return ToBase64(text, Encoding.UTF8);
        }

        ///<summary>Return the Base64 representation of the string</summary>
        public static string ToBase64(this string text, Encoding encoding)
        {
            byte[] textAsBytes = encoding.GetBytes(text);
            return Convert.ToBase64String(textAsBytes);
        }
    }
}
