namespace TransparentWindow.Forms
{
    partial class ConfigureScreen
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.PathSelection = new System.Windows.Forms.ComboBox();
            this.edit = new System.Windows.Forms.Button();
            this.UrlInput = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // PathSelection
            // 
            this.PathSelection.FormattingEnabled = true;
            this.PathSelection.Location = new System.Drawing.Point(12, 12);
            this.PathSelection.Name = "PathSelection";
            this.PathSelection.Size = new System.Drawing.Size(165, 21);
            this.PathSelection.TabIndex = 0;
            // 
            // edit
            // 
            this.edit.Location = new System.Drawing.Point(183, 12);
            this.edit.Name = "edit";
            this.edit.Size = new System.Drawing.Size(39, 21);
            this.edit.TabIndex = 1;
            this.edit.Text = "edit";
            this.edit.UseVisualStyleBackColor = true;
            this.edit.Click += new System.EventHandler(this.edit_Click);
            // 
            // UrlInput
            // 
            this.UrlInput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.UrlInput.Location = new System.Drawing.Point(12, 12);
            this.UrlInput.Name = "UrlInput";
            this.UrlInput.Size = new System.Drawing.Size(210, 20);
            this.UrlInput.TabIndex = 2;
            this.UrlInput.Visible = false;
            // 
            // ConfigureScreen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(234, 47);
            this.Controls.Add(this.UrlInput);
            this.Controls.Add(this.edit);
            this.Controls.Add(this.PathSelection);
            this.Name = "ConfigureScreen";
            this.Text = "ConfigureScreen";
            this.Load += new System.EventHandler(this.ConfigureScreen_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox PathSelection;
        private System.Windows.Forms.Button edit;
        private System.Windows.Forms.TextBox UrlInput;
    }
}