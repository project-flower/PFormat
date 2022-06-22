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
            AddNew(false);
        }

        public void Add(string title, string format, bool show)
        {
            Insert(tabControl.TabCount, title, format, show);
        }

        public (string, string)[] GetApplicationData()
        {
            TabPageCollection tabPages = GetTabPages();

            var result = new List<(string, string)>(tabPages.Count);

            foreach (TabPage tabPage in tabPages)
            {
                var editor = GetFormatEditor(tabPage);
                result.Add((editor.Title, editor.Format));
            }

            return result.ToArray();
        }

        public string GetCurrentValue()
        {
            FormatEditor currentEditor = GetCurrentEditor();

            if (currentEditor == null) return string.Empty;

            return currentEditor.Value;
        }

        public void Initialize()
        {
            SetApplicationData(new (string, string)[0]);
        }

        public void RemoveAt(int index, bool force = false)
        {
            int tabCount = tabControl.TabCount;

            if ((index < 0) || (index >= tabCount) || (!force && (tabCount < 2))) return;

            TabPageCollection tabPages = GetTabPages();
            var tabPage = tabPages[index];
            RemoveEventHandler(GetFormatEditor(tabPage));
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
                Add(format.Item1, format.Item2, false);
            }

            if (tabPages.Count < 1) AddNew(false);
        }

        public void SetEditable(bool editable)
        {
            foreach (TabPage tabPage in GetTabPages())
            {
                GetFormatEditor(tabPage).SetEditable(editable);
            }

            buttonAdd.Enabled = editable;
            buttonCopy.Enabled = editable;
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
                GetFormatEditor(tabPage).SetResult();
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

        private void AddNew(bool show)
        {
            Add(GenerateUniqueFormatName(Global.DefaultFormatName), string.Empty, show);
        }

        private void formatEditor_ButtonCloseClick(object sender, EventArgs e)
        {
            if (tabControl.TabCount < 2) return;

            var eventArgs = new DialogRequiredEventArgs("タブ を削除します。よろしいですか？", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            DialogRequired(this, eventArgs);

            if (eventArgs.Result != DialogResult.Yes) return;

            RemoveAt(tabControl.SelectedIndex);
        }

        private void CopyFormat(FormatEditor formatEditor, bool show)
        {
            InsertNext(GenerateUniqueFormatName($"{formatEditor.Title} {Global.CopiedSuffix}"), formatEditor.Format, show);
        }

        private void formatEditor_FieldsRequired(object sender, FieldsRequiredEventArgs e)
        {
            e.Fields = fields;
        }

        private void formatEditor_TitleChanged(object sender, TextChangedEventArgs e)
        {
            ((sender as Control).Parent as TabPage).Text = e.Text;
        }

        private string GenerateUniqueFormatName(string formatName)
        {
            for (uint i = 1; i <= uint.MaxValue; ++i)
            {
                string result = $"{formatName} {i}";
                bool found = false;

                foreach (TabPage tabPage in tabControl.TabPages)
                {
                    if (GetFormatEditor(tabPage).Title == result)
                    {
                        found = true;
                        break;
                    }
                }

                if (!found) return result;
            }

            throw new Exception("新しいフォーマット名が作成できません。");
        }

        private FormatEditor GetCurrentEditor()
        {
            TabPage selectedTab = tabControl.SelectedTab;

            if (selectedTab == null) return null;

            return GetFormatEditor(selectedTab);
        }

        private FormatEditor GetFormatEditor(TabPage tabPage)
        {
            return (tabPage.Controls[0] as FormatEditor);
        }

        private TabPageCollection GetTabPages()
        {
            return tabControl.TabPages;
        }

        private void Insert(int index, string title, string format, bool show)
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

            if (show) tabControl.SelectTab(index);
        }

        private void InsertNew(bool show)
        {
            InsertNext(GenerateUniqueFormatName(Global.DefaultFormatName), string.Empty, show);
        }

        private void InsertNext(string title, string format, bool show)
        {
            Insert((tabControl.SelectedIndex + 1), title, format, show);
        }

        private void RemoveEventHandler(FormatEditor formatEditor)
        {
            formatEditor.ButtonCloseClick -= formatEditor_ButtonCloseClick;
            formatEditor.FieldsRequired += formatEditor_FieldsRequired;
            formatEditor.TitleChanged -= formatEditor_TitleChanged;
        }

        #endregion

        // Designer's Methods

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            InsertNew(true);
        }

        private void buttonCopy_Click(object sender, EventArgs e)
        {
            FormatEditor formatEditor = GetCurrentEditor();

            if (formatEditor == null) return;

            try
            {
                CopyFormat(formatEditor, true);
            }
            catch (Exception exception)
            {
                DialogRequired(this, new DialogRequiredEventArgs(exception.Message, MessageBoxButtons.OK, MessageBoxIcon.Error));
            }
        }

        private void buttonUpdate_Click(object sender, EventArgs e)
        {
            UpdateFieldValues();
        }
    }
}
