using System;

namespace Crotchet.Quaver.Exceptions
{
    [Serializable]
    public class SignatureScanReturnedNullException : Exception
    {
        public SignatureScanReturnedNullException(): base("The signature scan returned a null pointer and therefore execution has been aborted!")
        { }
    }
}