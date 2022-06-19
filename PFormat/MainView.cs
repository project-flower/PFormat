using PFormat.Events;
using PFormat.Exceptions;
using System.Windows.Forms;

namespace PFormat
{
    public partial class MainView : UserControl
    {
        #region Private Fields

        private bool editable = true;

        #endregion

        #region Public Properties

        public event DialogRequiredEventHandler DialogRequired = delegate { };
        public bool Editable { get => editable; }

        public string GroupName { get; set; }

        #endregion

        #region Public Methods

        public MainView()
        {
            InitializeComponent();
        }

        public ApplicationData.Group GetApplicationData()
        {
            return new ApplicationData.Group()
            {
                Fields = fieldsPane.GetApplicationData(),
                Formats = formatsPane.GetApplicationData(),
                Name = GroupName
            };
        }

        public string GetCurrentValue()
        {
            return formatsPane.GetCurrentValue();
        }

        public void Initialze()
        {
            fieldsPane.Initialize();
            formatsPane.Initialize();
        }

        public void SetApplicationData(ApplicationData.Group applicationData)
        {
            fieldsPane.SetApplicationData(applicationData.Fields);
            formatsPane.SetApplicationData(applicationData.Formats);
            GroupName = applicationData.Name;
        }

        public void SetEditable(bool editable)
        {
            fieldsPane.SetEditable(editable);
            formatsPane.SetEditable(editable);

            if (!editable)
            {
                UpdateFields();
            }

            this.editable = editable;
        }

        public override string ToString()
        {
            return GroupName;
        }

        public void UpdateFields()
        {
            formatsPane.UpdateFieldValues();
        }

        #endregion

        // Designer's Methods

        private void dialogRequired(object sender, DialogRequiredEventArgs e)
        {
            DialogRequired(this, e);
        }

        private void formatsPane_FieldsRequired(object sender, FieldsRequiredEventArgs e)
        {
            try
            {
                e.Fields = fieldsPane.GetFields();
            }
            catch (FieldDuplicatedException exception)
            {
                DialogRequired(this, new DialogRequiredEventArgs($"フィールド \"{exception.Name}\"\r\nが重複しています。", MessageBoxButtons.OK, MessageBoxIcon.Error));
            }
        }
    }
}
