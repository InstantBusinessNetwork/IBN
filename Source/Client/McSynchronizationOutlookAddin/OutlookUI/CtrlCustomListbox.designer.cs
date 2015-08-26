namespace OutlookAddin.OutlookUI
{
    partial class CustomListBox
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.itemsPanel = new System.Windows.Forms.Panel();
            this.animationTimer = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // itemsPanel
            // 
            this.itemsPanel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.itemsPanel.Location = new System.Drawing.Point(3, 3);
            this.itemsPanel.Name = "itemsPanel";
            this.itemsPanel.Size = new System.Drawing.Size(166, 104);
            this.itemsPanel.TabIndex = 0;
            // 
            // animationTimer
            // 
            this.animationTimer.Enabled = true;
            this.animationTimer.Interval = 50;
            this.animationTimer.Tick += new System.EventHandler(this.AnimationTick);
            // 
            // CustomListBox
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.itemsPanel);
            this.DoubleBuffered = true;
            this.Name = "CustomListBox";
            this.Size = new System.Drawing.Size(172, 110);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel itemsPanel;
        private System.Windows.Forms.Timer animationTimer;
    }
}
