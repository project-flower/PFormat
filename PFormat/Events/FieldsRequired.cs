using System;
using System.Collections.Generic;

namespace PFormat.Events
{
    public class FieldsRequiredEventArgs : EventArgs
    {
        #region Public Fields

        public Dictionary<string, string> Fields = new Dictionary<string, string>();

        #endregion
    }

    public delegate void FieldsRequiredEventHandler(object sender, FieldsRequiredEventArgs e);
}
