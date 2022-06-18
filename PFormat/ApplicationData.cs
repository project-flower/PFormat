using System;

namespace PFormat
{
    [Serializable]
    public class ApplicationData
    {
        #region Public Classes

        public class Group
        {
            #region Public Fields

            public (string, string[])[] Fields;
            public (string, string)[] Formats;
            public string Name;

            #endregion
        }

        #endregion

        #region Public Fields

        public Group[] Groups;

        #endregion
    }
}
