
namespace More_tools
{
    partial class Home
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
            this.tw = new More_tools.ToggleSwitch();
            this.SuspendLayout();
            // 
            // tw
            // 
            this.tw.AutoSize = true;
            this.tw.Location = new System.Drawing.Point(384, 131);
            this.tw.MinimumSize = new System.Drawing.Size(45, 22);
            this.tw.Name = "tw";
            this.tw.OffBackColor = System.Drawing.Color.Gray;
            this.tw.OffToggleColor = System.Drawing.Color.Gainsboro;
            this.tw.OnBackColor = System.Drawing.Color.MediumSlateBlue;
            this.tw.OnToggleColor = System.Drawing.Color.WhiteSmoke;
            this.tw.Padding = new System.Windows.Forms.Padding(6);
            this.tw.Size = new System.Drawing.Size(45, 26);
            this.tw.TabIndex = 0;
            this.tw.UseVisualStyleBackColor = true;
            // 
            // Home
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.tw);
            this.Name = "Home";
            this.Text = "Home";
            this.Load += new System.EventHandler(this.Home_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ToggleSwitch tw;
    }
}

