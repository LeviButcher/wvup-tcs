#pragma warning disable 1591

using System;

namespace tcs_service.Helpers {
    [Serializable]
    public class TCSException : Exception {
        public TCSException () {

        }

        public TCSException (string name) : base (name) { }
    }
}