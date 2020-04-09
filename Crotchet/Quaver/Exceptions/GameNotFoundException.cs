using System;

namespace Crotchet.Quaver.Exceptions
{
    [Serializable]
    public class GameNotFoundException : Exception
    {
        public GameNotFoundException() : base("The game could not be found and execution has therefore stopped!")
        { }
    }
}