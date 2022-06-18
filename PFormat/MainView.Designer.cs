namespace PFormat
{
    partial class MainView
    {
        /// <summary> 
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージド リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region コンポーネント デザイナーで生成されたコード

        /// <summary> 
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を 
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.fieldsPane = new PFormat.FieldsPane();
            this.formatsPane = new PFormat.FormatsPane();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer
            // 
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.Location = new System.Drawing.Point(0, 0);
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.fieldsPane);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.formatsPane);
            this.splitContainer.Size = new System.Drawing.Size(600, 400);
            this.splitContainer.SplitterDistance = 200;
            this.splitContainer.TabIndex = 0;
            // 
            // fieldsPane
            // 
            this.fieldsPane.AutoScroll = true;
            this.fieldsPane.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fieldsPane.Location = new System.Drawing.Point(0, 0);
            this.fieldsPane.Name = "fieldsPane";
            this.fieldsPane.Size = new System.Drawing.Size(200, 400);
            this.fieldsPane.TabIndex = 0;
            this.fieldsPane.DialogRequired += new PFormat.Events.DialogRequiredEventHandler(this.dialogRequired);
            // 
            // formatsPane
            // 
            this.formatsPane.Dock = System.Windows.Forms.DockStyle.Fill;
            this.formatsPane.Location = new System.Drawing.Point(0, 0);
            this.formatsPane.Name = "formatsPane";
            this.formatsPane.Size = new System.Drawing.Size(396, 400);
            this.formatsPane.TabIndex = 0;
            this.formatsPane.DialogRequired += new PFormat.Events.DialogRequiredEventHandler(this.dialogRequired);
            this.formatsPane.FieldsRequired += new PFormat.Events.FieldsRequiredEventHandler(this.formatsPane_FieldsRequired);
            // 
            // MainView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer);
            this.Name = "MainView";
            this.Size = new System.Drawing.Size(600, 400);
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer;
        private FieldsPane fieldsPane;
        private FormatsPane formatsPane;
    }
}
