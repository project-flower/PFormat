using System;

namespace PFormat.Events
{
    public class EditableChangedEventArgs : EventArgs
    {
        #region Public Fields

        public readonly bool Editable;

        #endregion

        #region Public Methods

        public EditableChangedEventArgs(bool editable)
        {
            Editable = editable;
        }

        #endregion
    }

    public delegate void EditableChangedEventHandler(object sender, EditableChangedEventArgs e);
}
