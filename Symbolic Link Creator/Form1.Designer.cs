

namespace SymbolicLinkCreator
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Label labelSource;
        private System.Windows.Forms.TextBox txtSource;
        private System.Windows.Forms.Button btnBrowseSource;
        private System.Windows.Forms.Label labelDestination;
        private System.Windows.Forms.TextBox txtDestination;
        private System.Windows.Forms.Button btnBrowseDestination;
        private System.Windows.Forms.Button btnCreateSymlink;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.labelSource = new System.Windows.Forms.Label();
            this.txtSource = new System.Windows.Forms.TextBox();
            this.btnBrowseSource = new System.Windows.Forms.Button();
            this.labelDestination = new System.Windows.Forms.Label();
            this.txtDestination = new System.Windows.Forms.TextBox();
            this.btnBrowseDestination = new System.Windows.Forms.Button();
            this.btnCreateSymlink = new System.Windows.Forms.Button();
            this.chk_Directory_SymbolicLink = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // labelSource
            // 
            this.labelSource.AutoSize = true;
            this.labelSource.Location = new System.Drawing.Point(24, 19);
            this.labelSource.Name = "labelSource";
            this.labelSource.Size = new System.Drawing.Size(91, 13);
            this.labelSource.TabIndex = 0;
            this.labelSource.Text = "Source Directory:";
            // 
            // txtSource
            // 
            this.txtSource.Location = new System.Drawing.Point(118, 16);
            this.txtSource.Name = "txtSource";
            this.txtSource.Size = new System.Drawing.Size(343, 20);
            this.txtSource.TabIndex = 1;
            // 
            // btnBrowseSource
            // 
            this.btnBrowseSource.BackColor = System.Drawing.Color.Transparent;
            this.btnBrowseSource.BackgroundImage = global::Symbolic_Link_Creator.Properties.Resources._4673908;
            this.btnBrowseSource.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnBrowseSource.Location = new System.Drawing.Point(470, 12);
            this.btnBrowseSource.Name = "btnBrowseSource";
            this.btnBrowseSource.Size = new System.Drawing.Size(64, 27);
            this.btnBrowseSource.TabIndex = 2;
            this.btnBrowseSource.UseVisualStyleBackColor = false;
            this.btnBrowseSource.Click += new System.EventHandler(this.btnBrowseSource_Click);
            // 
            // labelDestination
            // 
            this.labelDestination.AutoSize = true;
            this.labelDestination.Location = new System.Drawing.Point(3, 63);
            this.labelDestination.Name = "labelDestination";
            this.labelDestination.Size = new System.Drawing.Size(112, 13);
            this.labelDestination.TabIndex = 3;
            this.labelDestination.Text = "Destination Directory:";
            // 
            // txtDestination
            // 
            this.txtDestination.Location = new System.Drawing.Point(118, 60);
            this.txtDestination.Name = "txtDestination";
            this.txtDestination.Size = new System.Drawing.Size(343, 20);
            this.txtDestination.TabIndex = 4;
            // 
            // btnBrowseDestination
            // 
            this.btnBrowseDestination.BackColor = System.Drawing.Color.Transparent;
            this.btnBrowseDestination.BackgroundImage = global::Symbolic_Link_Creator.Properties.Resources._4673908;
            this.btnBrowseDestination.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnBrowseDestination.Location = new System.Drawing.Point(470, 56);
            this.btnBrowseDestination.Name = "btnBrowseDestination";
            this.btnBrowseDestination.Size = new System.Drawing.Size(64, 27);
            this.btnBrowseDestination.TabIndex = 5;
            this.btnBrowseDestination.UseVisualStyleBackColor = false;
            this.btnBrowseDestination.Click += new System.EventHandler(this.btnBrowseDestination_Click);
            // 
            // btnCreateSymlink
            // 
            this.btnCreateSymlink.Location = new System.Drawing.Point(195, 95);
            this.btnCreateSymlink.Name = "btnCreateSymlink";
            this.btnCreateSymlink.Size = new System.Drawing.Size(129, 32);
            this.btnCreateSymlink.TabIndex = 6;
            this.btnCreateSymlink.Text = "Create Junction";
            this.btnCreateSymlink.UseVisualStyleBackColor = true;
            this.btnCreateSymlink.Click += new System.EventHandler(this.btnCreateSymlink_Click);
            // 
            // chk_Directory_SymbolicLink
            // 
            this.chk_Directory_SymbolicLink.AutoSize = true;
            this.chk_Directory_SymbolicLink.Location = new System.Drawing.Point(345, 104);
            this.chk_Directory_SymbolicLink.Name = "chk_Directory_SymbolicLink";
            this.chk_Directory_SymbolicLink.Size = new System.Drawing.Size(133, 17);
            this.chk_Directory_SymbolicLink.TabIndex = 7;
            this.chk_Directory_SymbolicLink.Text = " directory symbolic link";
            this.chk_Directory_SymbolicLink.UseVisualStyleBackColor = true;
            this.chk_Directory_SymbolicLink.CheckedChanged += new System.EventHandler(this.chk_Directory_SymbolicLink_CheckedChanged);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(555, 139);
            this.Controls.Add(this.chk_Directory_SymbolicLink);
            this.Controls.Add(this.btnCreateSymlink);
            this.Controls.Add(this.btnBrowseDestination);
            this.Controls.Add(this.txtDestination);
            this.Controls.Add(this.labelDestination);
            this.Controls.Add(this.btnBrowseSource);
            this.Controls.Add(this.txtSource);
            this.Controls.Add(this.labelSource);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "symlink and junction wizard";
            this.Shown += new System.EventHandler(this.MainForm_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.CheckBox chk_Directory_SymbolicLink;
    }
}
