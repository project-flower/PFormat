using PFormat.Events;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using static System.Windows.Forms.TabControl;

namespace PFormat
{
    public partial class FormatsPane : UserControl
    {
        #region Private Fields

        private bool editable = true;
        private Dictionary<string, string> fields = new Dictionary<string, string>();

        #endregion

        #region Public Properties

        public event DialogRequiredEventHandler DialogRequired = delegate { };
        public event FieldsRequiredEventHandler FieldsRequired = delegate { };

        #endregion

        #region Public Methods

        public FormatsPane()
        {
            InitializeComponent();
            AddNew();
        }

        public void AddNew(string title = "", string format = "")
        {
            Insert(tabControl.TabCount, title, format);
        }

        public string GetCurrentValue()
        {
            TabPage selectedTab = tabControl.SelectedTab;

            if (selectedTab == null) return string.Empty;

            return (selectedTab.Controls[0] as FormatEditor).Value;
        }

        public (string, string)[] GetApplicationData()
        {
            TabPageCollection tabPages = GetTabPages();

            var result = new List<(string, string)>(tabPages.Count);

            foreach (TabPage tabPage in tabPages)
            {
                var editor = tabPage.Controls[0] as FormatEditor;
                result.Add((editor.Title, editor.Value));
            }

            return result.ToArray();
        }

        public void Initialize()
        {
            SetApplicationData(new (string, string)[0]);
        }

        public void Insert(int index, string title = "", string format = "")
        {
            int tabCount = tabControl.TabCount;

            if (index > tabCount) return;

            var tabPage = new TabPage(title);

            var formatEditor = new FormatEditor()
            {
                Dock = DockStyle.Fill,
                Format = format,
                Title = title
            };

            if (editable != formatEditor.Editable)
            {
                formatEditor.SetEditable(editable);
            }

            AddEventHandlers(formatEditor);
            tabPage.Controls.Add(formatEditor);
            TabPageCollection tabPages = GetTabPages();

            if (tabCount <= index)
            {
                tabPages.Add(tabPage);
            }
            else
            {
                tabPages.Insert(index, tabPage);
            }
        }

        public void RemoveAt(int index, bool force = false)
        {
            int tabCount = tabControl.TabCount;

            if ((index < 0) || (index >= tabCount) || (!force && (tabCount < 2))) return;

            TabPageCollection tabPages = GetTabPages();
            var tabPage = tabPages[index];
            RemoveEventHandler(tabPage.Controls[0] as FormatEditor);
            tabPages.RemoveAt(index);
        }

        public void SetApplicationData((string, string)[] formats)
        {
            TabPageCollection tabPages = GetTabPages();

            while (tabPages.Count > 0)
            {
                RemoveAt(0, true);
            }

            foreach ((string, string) format in formats)
            {
                AddNew(format.Item1, format.Item2);
            }

            if (tabPages.Count < 1) AddNew();
        }

        public void SetEditable(bool editable)
        {
            foreach (TabPage tabPage in GetTabPages())
            {
                (tabPage.Controls[0] as FormatEditor).SetEditable(editable);
            }

            buttonAdd.Enabled = editable;
            buttonUpdate.Enabled = !editable;
            this.editable = editable;
        }

        public void UpdateFieldValues()
        {
            var eventArgs = new FieldsRequiredEventArgs();
            FieldsRequired(this, eventArgs);
            fields = eventArgs.Fields;

            foreach (TabPage tabPage in GetTabPages())
            {
                (tabPage.Controls[0] as FormatEditor).SetResult();
            }
        }

        #endregion

        #region Private Methods

        private void AddEventHandlers(FormatEditor formatEditor)
        {
            formatEditor.ButtonCloseClick += formatEditor_ButtonCloseClick;
            formatEditor.FieldsRequired += formatEditor_FieldsRequired;
            formatEditor.TitleChanged += formatEditor_TitleChanged;
        }

        private void formatEditor_ButtonCloseClick(object sender, EventArgs e)
        {
            if (tabControl.TabCount < 2) return;

            var eventArgs = new DialogRequiredEventArgs("タブ を削除します。よろしいですか？", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            DialogRequired(this, eventArgs);

            if (eventArgs.Result != DialogResult.Yes) return;

            RemoveAt(tabControl.SelectedIndex);
        }

        private void formatEditor_FieldsRequired(object sender, FieldsRequiredEventArgs e)
        {
            e.Fields = fields;
        }

        private void formatEditor_TitleChanged(object sender, TextChangedEventArgs e)
        {
            ((sender as Control).Parent as TabPage).Text = e.Text;
        }

        private TabPageCollection GetTabPages()
        {
            return tabControl.TabPages;
        }

        private void RemoveEventHandler(FormatEditor formatEditor)
        {
            formatEditor.ButtonCloseClick -= formatEditor_ButtonCloseClick;
            formatEditor.TitleChanged -= formatEditor_TitleChanged;
        }

        #endregion

        // Designer's Methods

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            Insert(tabControl.SelectedIndex + 1);
        }

        private void buttonUpdate_Click(object sender, EventArgs e)
        {
            UpdateFieldValues();
        }
    }
}
