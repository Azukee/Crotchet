using System;

namespace Crotchet.Quaver.Exceptions
{
    [Serializable]
    public class UnknownHeaderTagException : Exception
    {
        public UnknownHeaderTagException() : base("An unknown HeaderTag has been encountered. It's uncertain if this Tag is important for the map playing and therefore Crotchet has aborted parsing of the chart.")
        { }
    }
}