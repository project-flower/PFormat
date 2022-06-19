using PFormat.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace PFormat
{
    public partial class FieldEditor : UserControl
    {
        #region Private Fields

        private bool editable = true;

        #endregion

        #region Public Properties

        public event EventHandler ButtonAddNextClick = delegate { };
        public event EventHandler ButtonCloseClick = delegate { };
        public event EventHandler ButtonDownClick = delegate { };
        public event EventHandler ButtonUpClick = delegate { };
        public event TextChangedEventHandler ValueChanged = delegate { };

        public bool Editable
        {
            get => editable;

            set
            {
                editable = value;
                buttonClose.Visible = editable;
                buttonUp.Visible = editable;
                buttonDown.Visible = editable;
                buttonAddNext.Visible = editable;
                textBoxName.ReadOnly = !editable;
            }
        }

        public string FieldName
        {
            get => textBoxName.Text;
            set => textBoxName.Text = value;
        }

        public string Value
        {
            get => comboBoxValue.Text;
            set { comboBoxValue.Text = value; }
        }

        public IEnumerable<string> Values
        {
            get => comboBoxValue.Items.OfType<string>();

            set
            {
                comboBoxValue.BeginUpdate();
                comboBoxValue.Items.Clear();

                foreach (string element in value)
                {
                    comboBoxValue.Items.Add(element);
                }

                comboBoxValue.EndUpdate();
            }
        }

        #endregion

        #region Public Methods

        public FieldEditor()
        {
            InitializeComponent();
        }

        public void Clear()
        {
            FieldName = string.Empty;
            Values = new string[0];
        }

        public void CopyTo(FieldEditor destination)
        {
            destination.FieldName = FieldName;
            destination.Values = Values;
            destination.Value = Value;
        }

        public void SwapTo(FieldEditor target)
        {
            string fieldName = target.FieldName;
            IEnumerable<string> values = target.Values.ToArray();
            string value = target.Value;
            CopyTo(target);
            FieldName = fieldName;
            Values = values;
            Value = value;
        }

        #endregion

        // Designer's Methods

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            comboBoxValue.Items.Add(comboBoxValue.Text);
            comboBoxValue.SelectedIndex = (comboBoxValue.Items.Count - 1);
        }

        private void buttonAddNext_Click(object sender, EventArgs e)
        {
            ButtonAddNextClick(this, e);
        }

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

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            int index = comboBoxValue.SelectedIndex;

            if (index < 0) return;

            comboBoxValue.Items.RemoveAt(index);
            comboBoxValue.Text = string.Empty;
        }

        private void buttonDown_Click(object sender, EventArgs e)
        {
            ButtonDownClick(this, e);
        }

        private void buttonUp_Click(object sender, EventArgs e)
        {
            ButtonUpClick(this, e);
        }
    }
}
