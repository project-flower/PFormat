using System;
using System.Windows.Forms;

namespace PFormat.Events
{
    public class DialogRequiredEventArgs : EventArgs
    {
        #region Public Fields

        public readonly MessageBoxButtons Buttons;
        public readonly MessageBoxIcon Icon;
        public DialogResult Result = DialogResult.None;
        public readonly string Text;

        #endregion

        #region Public Methods

        public DialogRequiredEventArgs(string text, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            Buttons = buttons;
            Icon = icon;
            Text = text;
        }

        #endregion
    }

    public delegate void DialogRequiredEventHandler(object sender, DialogRequiredEventArgs e);
}
