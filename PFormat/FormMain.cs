using PFormat.Events;
using PFormat.Exceptions;
using PFormat.Properties;
using System;
using System.IO;
using System.Windows.Forms;

namespace PFormat
{
    public partial class FormMain : Form
    {
        #region Private Fields

        private readonly string buttonConvertDefaultText;
        private bool dialogShowing = false;
        private bool editable = true;

        #endregion

        #region Public Methods

        public FormMain()
        {
            InitializeComponent();
            MinimumSize = Size;
            buttonConvertDefaultText = buttonConvert.Text;
        }

        #endregion

        #region Private Methods

        private void DoCopy()
        {
            try
            {
                Clipboard.SetText(groupsPane.GetCurrentValue());
                ShowInformation("クリップボードにコピーしました。");
            }
            catch (Exception exception)
            {
                ShowErrorMessage(exception.Message);
            }
        }

        private void DoInitialze()
        {
            groupsPane.Initialze(true);
        }

        private bool LoadSettings()
        {
            groupsPane.Initialze(false);
            Settings settings = Settings.Default;
            settings.Reload();
            Global.CopiedSuffix = settings.CopiedSuffix;
            Global.DefaultFormatName = settings.DefaultFormatName;
            string saveFileName = settings.SaveFileName;

            if (string.IsNullOrEmpty(saveFileName))
            {
                return false;
            }

            try
            {
                if (!File.Exists(saveFileName))
                {
                    ShowErrorMessage($"ファイル{saveFileName}\r\nがありません。");
                    return false;
                }

                groupsPane.SetApplicationData(SaveFileNamager.LoadFromFile(saveFileName), true);
            }
            catch (Exception exception)
            {
                ShowErrorMessage(exception.Message);
                return false;
            }

            return true;
        }

        private void SaveSettings(out string fileName)
        {
            Settings settings = Settings.Default;
            fileName = settings.SaveFileName;

            try
            {
                if (string.IsNullOrEmpty(fileName))
                {
                    fileName = Path.ChangeExtension(Application.ExecutablePath, ".xml");
                    settings.SaveFileName = fileName;
                    settings.Save();
                }

                SaveFileNamager.SaveToFile(fileName, groupsPane.GetApplicationData());
            }
            catch (Exception exception)
            {
                ShowErrorMessage(exception.Message);
                return;
            }
        }

        private void SetEditable(bool editable)
        {
            this.editable = editable;
            buttonConvert.Text = (this.editable ? buttonConvertDefaultText : "編集(&E)");
            buttonInitialize.Enabled = this.editable;
        }

        private void ShowErrorMessage(string message)
        {
            ShowMessage(message, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void ShowInformation(string information)
        {
            ShowMessage(information, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private DialogResult ShowMessage(string text, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            if (dialogShowing) return DialogResult.None;

            dialogShowing = true;
            DialogResult result = MessageBox.Show(this, text, Text, buttons, icon);
            dialogShowing = false;
            return result;
        }

        private void TryChangeEditable()
        {
            try
            {
                groupsPane.SetEditable(!editable);
            }
            catch (FieldDuplicatedException exception)
            {
                ShowErrorMessage($"フィールド \"{exception.Name}\"\r\nが重複しています。");
                return;
            }

            SetEditable(!editable);
        }

        #endregion

        // Designer's Methods

        private void buttonConvert_Click(object sender, EventArgs e)
        {
            TryChangeEditable();
        }

        private void buttonCopy_Click(object sender, EventArgs e)
        {
            DoCopy();
        }

        private void buttonInitialize_Click(object sender, EventArgs e)
        {
            if (ShowMessage("新規作成します。\r\nよろしいですか？", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
                != DialogResult.Yes) return;

            DoInitialze();
        }

        private void buttonLoad_Click(object sender, EventArgs e)
        {
            if (ShowMessage("設定を読み込みます。\r\nよろしいですか？", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
                != DialogResult.Yes) return;
            bool loaded = false;

            try
            {
                loaded = LoadSettings();
            }
            catch (Exception exception)
            {
                ShowErrorMessage(exception.Message);
            }

            if (loaded)
            {
                ShowInformation("設定を読み込みました。");
                return;
            }

            DoInitialze();
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            if (ShowMessage("設定を保存します。\r\nよろしいですか？", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
                != DialogResult.Yes) return;

            try
            {
                SaveSettings(out string fileName);
                ShowInformation($"設定を保存しました。\r\n\r\n{fileName}");
            }
            catch (Exception exception)
            {
                ShowErrorMessage(exception.Message);
            }
        }

        private void groupsPane_DialogRequired(object sender, DialogRequiredEventArgs e)
        {
            e.Result = ShowMessage(e.Text, e.Buttons, e.Icon);
        }

        private void groupsPane_EditableChanged(object sender, EditableChangedEventArgs e)
        {
            SetEditable(e.Editable);
        }

        private void shown(object sender, EventArgs e)
        {
            bool loaded = false;

            try
            {
                loaded = LoadSettings();
            }
            catch (Exception exception)
            {
                ShowErrorMessage(exception.Message);
            }

            if (!loaded) DoInitialze();
        }
    }
}
