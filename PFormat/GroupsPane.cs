using PFormat.Events;
using PFormat.Exceptions;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using static System.Windows.Forms.ComboBox;

namespace PFormat
{
    public partial class GroupsPane : UserControl
    {
        #region Private Classes

        private delegate void DoForMainView(MainView mainView);
        private delegate void DoSomething();

        #endregion

        #region Private Fields

        private bool editable = true;

        #endregion

        #region Public Properties

        public string CopiedGroupSuffix { get; set; }
        public string DefaultGroupName { get; set; }
        public event DialogRequiredEventHandler DialogRequired = delegate { };
        public event EditableChangedEventHandler EditableChanged = delegate { };

        #endregion

        #region Public Methods

        public GroupsPane()
        {
            InitializeComponent();
        }

        public ApplicationData GetApplicationData()
        {
            ObjectCollection items = comboBoxNames.Items;
            ApplicationData.Group[] groups;
            int count = items.Count;

            if (count > 0)
            {
                var list = new List<ApplicationData.Group>(count);

                foreach (ObjectHolder<MainView> item in items)
                {
                    list.Add(item.Instance.GetApplicationData());
                }

                groups = list.ToArray();
            }
            else
            {
                groups = new ApplicationData.Group[0];
            }

            return new ApplicationData()
            {
                Groups = groups
            };
        }

        public string GetCurrentValue()
        {
            string result = string.Empty;

            DoIfCurrentlyDisplayed(delegate (MainView mainView)
            {
                result = mainView.GetCurrentValue();
            });

            return result;
        }

        public void Initialze(bool createBlank)
        {
            ObjectCollection items = comboBoxNames.Items;

            while (items.Count > 0)
            {
                RemoveGroup(0);
            }

            if (createBlank) AddNewGroup(true);
        }

        public void SetApplicationData(ApplicationData data, bool show)
        {
            Initialze(false);

            foreach (ApplicationData.Group group in data.Groups)
            {
                MainView mainView = AddGroup(group.Name, false);
                mainView.SetApplicationData(group);
            }

            if (show && (comboBoxNames.Items.Count > 0))
            {
                ChangeGroup(0);
            }
        }

        public void SetEditable(bool editable)
        {
            DoIfCurrentlyDisplayed(delegate (MainView mainView)
            {
                mainView.SetEditable(editable);
            });

            buttonAdd.Enabled = editable;
            buttonDelete.Enabled = editable;
            buttonRename.Enabled = editable;
            buttonCopy.Enabled = editable;
            this.editable = editable;
        }

        #endregion

        #region Private Methods

        private void AddEventHandlers(MainView mainView)
        {
            mainView.DialogRequired += mainView_DialogRequired;
        }

        private MainView AddGroup(string groupName, bool show)
        {
            if (string.IsNullOrEmpty(groupName))
            {
                groupName = GenerateDefaultGroupName();
            }

            ObjectCollection items = comboBoxNames.Items;

            var result = new MainView()
            {
                Dock = DockStyle.Fill,
                GroupName = groupName
            };

            result.SetEditable(editable);
            items.Add(new ObjectHolder<MainView>() { Instance = result });

            if (show) ChangeGroup(items.Count - 1);

            return result;
        }

        private MainView AddNewGroup(bool show)
        {
            return AddGroup(string.Empty, show);
        }

        private void ChangeGroup(int index)
        {
            DoIfCurrentlyDisplayed(delegate (MainView mainView)
            {
                RemoveEventHandlers(mainView);
            });

            ControlCollection controls = panelMainView.Controls;
            controls.Clear();

            if (index < 0) return;

            if (comboBoxNames.SelectedIndex != index)
            {
                DoWithSuppressEvent(delegate
                {
                    comboBoxNames.SelectedIndex = index;
                });
            }

            MainView newView = ((comboBoxNames.SelectedItem as ObjectHolder<MainView>).Instance);
            controls.Add(newView);
            AddEventHandlers(newView);

            if (newView.Editable == editable) return;

            try
            {
                newView.SetEditable(editable);
            }
            catch (FieldDuplicatedException exception)
            {
                DialogRequired(this, new DialogRequiredEventArgs($"フィールド \"{exception.Name}\"\r\nが重複しています。", MessageBoxButtons.OK, MessageBoxIcon.Error));
                EditableChanged(this, new EditableChangedEventArgs(!editable));
            }
        }

        private void CopyGroup(MainView mainView)
        {
            DoWithSuppressEvent(delegate
            {
                try
                {
                    string groupName = GenerateCopiedGroupName(mainView.GroupName);
                    var newView = AddGroup(groupName, false);
                    ApplicationData.Group group = mainView.GetApplicationData();
                    group.Name = groupName;
                    newView.SetApplicationData(group);
                    ChangeGroup(comboBoxNames.Items.Count - 1);
                }
                catch (Exception exception)
                {
                    DialogRequired(this, new DialogRequiredEventArgs(exception.Message, MessageBoxButtons.OK, MessageBoxIcon.Error));
                }
            });
        }

        private bool DoIfCurrentlyDisplayed(DoForMainView function)
        {
            MainView mainView = GetCurrentView();

            if (mainView == null) return false;

            function(mainView);
            return true;
        }

        private void DoWithSuppressEvent(DoSomething function)
        {
            comboBoxNames.SelectedIndexChanged -= comboBoxNames_SelectedIndexChanged;
            function();
            comboBoxNames.SelectedIndexChanged += comboBoxNames_SelectedIndexChanged;
        }

        private string GenerateCopiedGroupName(string groupName)
        {
            return GenerateUniqueGroupName($"{groupName} {CopiedGroupSuffix}");
        }

        private string GenerateDefaultGroupName()
        {
            return GenerateUniqueGroupName(DefaultGroupName);
        }

        private string GenerateUniqueGroupName(string groupName)
        {
            for (uint i = 1; i <= uint.MaxValue; ++i)
            {
                string result = $"{groupName} {i}";
                bool found = false;

                foreach (ObjectHolder<MainView> items in comboBoxNames.Items)
                {
                    if (items.Instance.GroupName == result)
                    {
                        found = true;
                        break;
                    }
                }

                if (!found) return result;
            }

            throw new Exception("新しいグループ名が作成できません。");
        }

        private bool ExistsInGroup(string groupName)
        {
            ObjectCollection items = comboBoxNames.Items;

            foreach (ObjectHolder<MainView> item in items)
            {
                if (item.Instance.GroupName == groupName)
                {
                    return true;
                }
            }

            return false;
        }

        private MainView GetCurrentView()
        {
            ControlCollection controls = panelMainView.Controls;

            if (controls.Count > 0)
            {
                return (controls[0] as MainView);
            }

            return null;
        }

        private void mainView_DialogRequired(object sender, DialogRequiredEventArgs e)
        {
            DialogRequired(this, e);
        }

        private void RemoveGroup(int index)
        {
            ObjectCollection items = comboBoxNames.Items;

            if ((index < 0) || (index >= items.Count)) return;

            items.RemoveAt(index);
            ChangeGroup(-1);
        }

        private void RemoveEventHandlers(MainView mainView)
        {
            mainView.DialogRequired -= mainView_DialogRequired;
        }

        #endregion

        // Designer's Methods

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            AddNewGroup(true);
        }

        private void buttonCopy_Click(object sender, EventArgs e)
        {
            MainView mainView = GetCurrentView();

            if (mainView == null) return;

            CopyGroup(mainView);
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            int index = comboBoxNames.SelectedIndex;

            if (index < 0) return;

            MainView mainView = GetCurrentView();
            var eventArgs = new DialogRequiredEventArgs($"{mainView.GroupName}\r\nを削除します。よろしいですか？", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            DialogRequired(this, eventArgs);

            if (eventArgs.Result != DialogResult.Yes) return;

            RemoveGroup(index);
        }

        private void buttonRename_Click(object sender, EventArgs e)
        {
            int index = comboBoxNames.SelectedIndex;

            if (index < 0)
            {
                index = comboBoxNames.PreviousIndex;
            }

            ObjectCollection items = comboBoxNames.Items;

            if ((index < 0) || (index >= items.Count)) return;

            var objectHolder = items[index] as ObjectHolder<MainView>;
            MainView mainView = objectHolder.Instance;
            string newName = comboBoxNames.Text;

            if (ExistsInGroup(newName))
            {
                DialogRequired(this, new DialogRequiredEventArgs($"{newName}\r\nは既に存在します。", MessageBoxButtons.OK, MessageBoxIcon.Error));
                return;
            }

#if false // 警告は必要なさそう。
            string oldName = mainView.GroupName;
            var eventArgs = new DialogRequiredEventArgs($"グループ\r\n{oldName}\r\nを{newName}\r\nに変更してもよろしいですか？", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            DialogRequired(this, eventArgs);

            if (eventArgs.Result != DialogResult.Yes) return;
#endif
            mainView.GroupName = newName;

            DoWithSuppressEvent(delegate
            {
                items.RemoveAt(index);
                items.Insert(index, objectHolder);
            });
        }

        private void comboBoxNames_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChangeGroup(comboBoxNames.SelectedIndex);
        }
    }
}
