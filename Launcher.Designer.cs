using System;
using System.Drawing;
using System.Windows.Forms;

namespace CreCraftStudios.HunterReborn.Launcher
{
    partial class Launcher
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            browser = new Microsoft.Web.WebView2.WinForms.WebView2();
            ((System.ComponentModel.ISupportInitialize)browser).BeginInit();
            SuspendLayout();
            // 
            // browser
            // 
            browser.AllowExternalDrop = true;
            browser.CreationProperties = null;
            browser.DefaultBackgroundColor = Color.White;
            browser.Dock = DockStyle.Fill;
            browser.Location = new Point(0, 0);
            browser.Name = "browser";
            browser.Size = new Size(1284, 861);
            browser.TabIndex = 0;
            browser.ZoomFactor = 1D;
            // 
            // Launcher
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1284, 861);
            Controls.Add(browser);
            MinimumSize = new Size(1300, 200);
            Name = "Launcher";
            Text = "Hunter Reborn";
            ((System.ComponentModel.ISupportInitialize)browser).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Microsoft.Web.WebView2.WinForms.WebView2 browser;
    }
}
