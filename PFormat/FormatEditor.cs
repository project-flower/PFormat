using PFormat.Events;
using System;
using System.Windows.Forms;

namespace PFormat
{
    public partial class FormatEditor : UserControl
    {
        #region Private Fields

        private bool editable = true;
        private string format = string.Empty;

        #endregion

        #region Public Properties

        public bool Editable { get => editable; }
        public event EventHandler ButtonCloseClick = delegate { };
        public event FieldsRequiredEventHandler FieldsRequired = delegate { };

        public string Format
        {
            get => format;

            set
            {
                format = value;

                if (editable)
                {
                    textBoxFormat.Text = format;
                }
                else
                {
                    SetResult();
                }
            }
        }

        public string Title
        {
            get => textBoxTitle.Text;
            set { textBoxTitle.Text = value; }
        }

        public event TextChangedEventHandler TitleChanged = delegate { };
        public string Value { get => textBoxFormat.Text; }

        #endregion

        #region Public Methods

        public FormatEditor()
        {
            InitializeComponent();
        }

        public void SetEditable(bool editable)
        {
            if (editable == this.editable) return;

            this.editable = editable;
            buttonClose.Visible = editable;
            textBoxTitle.Visible = editable;
            textBoxFormat.ReadOnly = !editable;

            if (editable)
            {
                textBoxFormat.Text = format;
                textBoxFormat.TextChanged += textBoxFormat_TextChanged;
                return;
            }

            textBoxFormat.TextChanged -= textBoxFormat_TextChanged;
            SetResult();
        }

        public void SetResult()
        {
            var eventArgs = new FieldsRequiredEventArgs();
            FieldsRequired(this, eventArgs);
            textBoxFormat.TextChanged -= textBoxFormat_TextChanged;
            textBoxFormat.Text = ReplaceEngine.Replace(format, eventArgs.Fields);
            textBoxFormat.TextChanged += textBoxFormat_TextChanged;
        }

        #endregion

        // Designer's Methods

        private void buttonClose_Click(object sender, EventArgs e)
        {
            ButtonCloseClick(this, e);
        }

        private void buttonClose_MouseEnter(object sender, EventArgs e)
        {
            buttonClose.Image = Properties.Resources.CloseButtonActive;
        }

        private void buttonClose_MouseLeave(object sender, EventArgs e)
        {
            buttonClose.Image = Properties.Resources.CloseButtonInactive;
        }

        private void textBoxFormat_TextChanged(object sender, EventArgs e)
        {
            format = textBoxFormat.Text;
        }

        private void textBoxTitle_TextChanged(object sender, EventArgs e)
        {
            TitleChanged(this, new TextChangedEventArgs(textBoxTitle.Text));
        }
    }
}
