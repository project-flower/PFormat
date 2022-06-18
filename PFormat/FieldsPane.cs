using PFormat.Events;
using PFormat.Exceptions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace PFormat
{
    public partial class FieldsPane : UserControl
    {
        #region Private Fields

        private bool editable = true;

        #endregion

        #region Public Properties

        public event DialogRequiredEventHandler DialogRequired = delegate { };

        #endregion

        #region Public Methods

        public FieldsPane()
        {
            InitializeComponent();
            AddNew();
        }

        public FieldEditor AddNew()
        {
            var result = new FieldEditor()
            {
                Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right
            };

            if (editable != result.Editable)
            {
                result.Editable = editable;
            }

            Point location;
            int controlsCount = Controls.Count;
            int marginLeft = Math.Max(Padding.Left, result.Margin.Left);

            if (controlsCount > 0)
            {
                Control control = Controls[controlsCount - 1];
                location = new Point(marginLeft, control.Bottom + Math.Max(control.Margin.Bottom, result.Margin.Top));
            }
            else
            {
                location = new Point(marginLeft, Math.Max(Padding.Top, result.Margin.Top));
            }

            result.Location = location;
            result.Width = (Width - marginLeft - Math.Max(Padding.Right, result.Margin.Right));
            AddEventHandlers(result);
            Controls.Add(result);
            return result;
        }

        public Dictionary<string, string> GetFields()
        {
            var result = new Dictionary<string, string>();

            foreach (FieldEditor editor in Controls)
            {
                string name = editor.FieldName;

                if (result.ContainsKey(name))
                {
                    throw new FieldDuplicatedException(name);
                }

                result[name] = editor.Value;
            }

            return result;
        }

        public (string, string[])[] GetApplicationData()
        {
            var result = new List<(string, string[])>(Controls.Count);

            foreach (FieldEditor fieldEditor in Controls)
            {
                result.Add((fieldEditor.FieldName, fieldEditor.Values.ToArray()));
            }

            return result.ToArray();
        }

        public void Initialize()
        {
            SetApplicationData(new (string, string[])[0]);
        }

        public void Insert(int index)
        {
            if (index > Controls.Count) return;

            AddNew();
            int i = (Controls.Count - 1);
            var fieldEditor = Controls[i] as FieldEditor;

            for (; i >= index; --i)
            {
                if (i == index)
                {
                    fieldEditor.Clear();
                    break;
                }

                if (!(GetNextControl(fieldEditor, false) is FieldEditor prevControl)) break;

                prevControl.CopyTo(fieldEditor);

                fieldEditor = prevControl;
            }
        }

        public void RemoveAt(int index, bool force = false)
        {
            int count = Controls.Count;

            if ((index < 0) || (index >= count) || (!force && (count < 2))) return;

            var fieldEditor = Controls[index] as FieldEditor;

            while (true)
            {
                if (!(GetNextControl(fieldEditor, true) is FieldEditor next))
                {
                    RemoveEventHandler(fieldEditor);
                    Controls.RemoveAt(count - 1);
                    break;
                }

                next.CopyTo(fieldEditor);
                fieldEditor = next;
            }
        }

        public void SetApplicationData((string, string[])[] fields)
        {
            while (Controls.Count > 0)
            {
                RemoveAt(0, true);
            }

            foreach ((string, string[]) element in fields)
            {
                FieldEditor fieldEditor = AddNew();
                fieldEditor.FieldName = element.Item1;
                fieldEditor.Values = element.Item2;
            }

            if (Controls.Count < 1) AddNew();
        }

        public void SetEditable(bool editable)
        {
            if (editable == this.editable) return;

            foreach (FieldEditor fieldEditor in Controls)
            {
                fieldEditor.Editable = editable;
            }

            this.editable = editable;
        }

        #endregion

        #region Private Methods

        private void AddEventHandlers(FieldEditor fieldEditor)
        {
            fieldEditor.ButtonAddNextClick += fieldEditor_ButtonAddNextClick;
            fieldEditor.ButtonCloseClick += fieldEditor_ButtonCloseClick;
            fieldEditor.ButtonDownClick += fieldEditor_ButtonDownClick;
            fieldEditor.ButtonUpClick += fieldEditor_ButtonUpClick;
        }

        private void fieldEditor_ButtonAddNextClick(object sender, EventArgs e)
        {
            var fieldEditor = sender as FieldEditor;
            Insert(Controls.IndexOf(fieldEditor) + 1);
        }

        private void fieldEditor_ButtonCloseClick(object sender, EventArgs e)
        {
            if (Controls.Count < 2) return;

            var eventArgs = new DialogRequiredEventArgs("フィールド を削除します。\r\nよろしいですか？", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            DialogRequired(this, eventArgs);

            if (eventArgs.Result != DialogResult.Yes) return;

            RemoveAt(Controls.IndexOf(sender as Control));
        }

        private void fieldEditor_ButtonDownClick(object sender, EventArgs e)
        {
            Swap(sender as FieldEditor, true);
        }

        private void fieldEditor_ButtonUpClick(object sender, EventArgs e)
        {
            Swap(sender as FieldEditor, false);
        }

        private void RemoveEventHandler(FieldEditor fieldEditor)
        {
            fieldEditor.ButtonAddNextClick -= fieldEditor_ButtonAddNextClick;
            fieldEditor.ButtonCloseClick -= fieldEditor_ButtonCloseClick;
            fieldEditor.ButtonDownClick -= fieldEditor_ButtonDownClick;
            fieldEditor.ButtonUpClick -= fieldEditor_ButtonUpClick;
        }

        private void Swap(FieldEditor fieldEditor, bool forward)
        {
            if (GetNextControl(fieldEditor, forward) is FieldEditor destination)
            {
                fieldEditor.SwapTo(destination);
            }
        }

        #endregion
    }
}
