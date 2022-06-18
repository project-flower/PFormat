using System;

namespace PFormat.Events
{
    public class TextChangedEventArgs : EventArgs
    {
        #region Public Fields

        public readonly string Text;

        #endregion

        #region Public Methods

        public TextChangedEventArgs(string text)
        {
            Text = text;
        }

        #endregion
    }

    public delegate void TextChangedEventHandler(object sender, TextChangedEventArgs e);
}
