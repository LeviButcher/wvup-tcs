using System;

//Exception that is ment to be returned back to the client as a message
namespace tcs_service.Helpers
{
    [Serializable]
    public class TCSException : Exception
    {
        public TCSException()
        {

        }

        public TCSException(string name) : base(name) { }
    }
}