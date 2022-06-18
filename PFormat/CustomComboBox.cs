using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace PFormat
{
    /// <summary>
    /// SelectedIndex の前回値を記憶する ComboBox
    /// </summary>
    [DesignerCategory("Code")]
    public class CustomComboBox : ComboBox
    {
        #region Private Fields

        private int previousIndex = -1;

        #endregion

        #region Public Properties

        public int PreviousIndex { get => previousIndex; }

        #endregion

        #region Protected Methods

        protected override void OnSelectedIndexChanged(EventArgs e)
        {
            previousIndex = SelectedIndex;
            base.OnSelectedIndexChanged(e);
        }

        #endregion
    }
}
