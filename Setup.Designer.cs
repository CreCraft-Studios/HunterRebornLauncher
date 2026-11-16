namespace CreCraftStudios.HunterReborn.Launcher
{
    partial class Setup
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
            txtExeLocation = new System.Windows.Forms.TextBox();
            label1 = new System.Windows.Forms.Label();
            btnBrowse = new System.Windows.Forms.Button();
            btnFinish = new System.Windows.Forms.Button();
            SuspendLayout();
            // 
            // txtExeLocation
            // 
            txtExeLocation.Location = new System.Drawing.Point(224, 25);
            txtExeLocation.Name = "txtExeLocation";
            txtExeLocation.Size = new System.Drawing.Size(467, 23);
            txtExeLocation.TabIndex = 0;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(12, 28);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(206, 15);
            label1.TabIndex = 1;
            label1.Text = "theHunter Classic executable location";
            // 
            // btnBrowse
            // 
            btnBrowse.Location = new System.Drawing.Point(697, 25);
            btnBrowse.Name = "btnBrowse";
            btnBrowse.Size = new System.Drawing.Size(75, 23);
            btnBrowse.TabIndex = 2;
            btnBrowse.Text = "Browse";
            btnBrowse.UseVisualStyleBackColor = true;
            btnBrowse.Click += btnBrowse_Click;
            // 
            // btnFinish
            // 
            btnFinish.Location = new System.Drawing.Point(697, 54);
            btnFinish.Name = "btnFinish";
            btnFinish.Size = new System.Drawing.Size(75, 23);
            btnFinish.TabIndex = 3;
            btnFinish.Text = "Finish";
            btnFinish.UseVisualStyleBackColor = true;
            btnFinish.Click += btnFinish_Click;
            // 
            // Setup
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(800, 96);
            Controls.Add(btnFinish);
            Controls.Add(btnBrowse);
            Controls.Add(label1);
            Controls.Add(txtExeLocation);
            Name = "Setup";
            Text = "Hunter Reborn Setup";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.TextBox txtExeLocation;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.Button btnFinish;
    }
}