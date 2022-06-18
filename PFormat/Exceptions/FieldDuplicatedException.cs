using System;

namespace PFormat.Exceptions
{
    public class FieldDuplicatedException : Exception
    {
        #region Public Fields

        public readonly string Name;

        #endregion

        #region Public Methods

        public FieldDuplicatedException(string name)
        {
            Name = name;
        }

        #endregion
    }
}
