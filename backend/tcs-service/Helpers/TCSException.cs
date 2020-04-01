using System;

namespace tcs_service.Helpers
{
    /// <summary>The Exception for the TCS Application</summary>
    /// This exception class should be used for any exceptions thrown within the TCS app.
    [Serializable]
    public class TCSException : Exception
    {
        /// <summary>TCSException Constructor</summary>
        public TCSException()
        {

        }

        /// <summary>TCSException Constructor</summary>
        public TCSException(string name) : base(name) { }
    }
}