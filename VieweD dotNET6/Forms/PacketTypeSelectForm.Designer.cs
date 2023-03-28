using System.Windows.Forms;
using System.Drawing;

namespace VieweD.Forms
{
    partial class PacketTypeSelectForm
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
            label1 = new Label();
            BtnIn = new Button();
            label2 = new Label();
            BtnOut = new Button();
            BtnSkip = new Button();
            label3 = new Label();
            HeaderDataLabel = new Label();
            SuspendLayout();
            // 
            // label1
            // 
            label1.Location = new Point(14, 10);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(310, 62);
            label1.TabIndex = 0;
            label1.Text = "Unable to indentify the packet direction.\r\nDo you want to assign a default direction for the remaining data ?";
            // 
            // BtnIn
            // 
            BtnIn.Location = new Point(14, 91);
            BtnIn.Margin = new Padding(4, 3, 4, 3);
            BtnIn.Name = "BtnIn";
            BtnIn.Size = new Size(93, 92);
            BtnIn.TabIndex = 1;
            BtnIn.Text = "Incoming";
            BtnIn.UseVisualStyleBackColor = true;
            BtnIn.Click += BtnIn_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(14, 73);
            label2.Margin = new Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new Size(160, 15);
            label2.TabIndex = 2;
            label2.Text = "Treat all unknown packets as:";
            // 
            // BtnOut
            // 
            BtnOut.Location = new Point(231, 91);
            BtnOut.Margin = new Padding(4, 3, 4, 3);
            BtnOut.Name = "BtnOut";
            BtnOut.Size = new Size(93, 92);
            BtnOut.TabIndex = 3;
            BtnOut.Text = "Outgoing";
            BtnOut.UseVisualStyleBackColor = true;
            BtnOut.Click += BtnOut_Click;
            // 
            // BtnSkip
            // 
            BtnSkip.DialogResult = DialogResult.Cancel;
            BtnSkip.Location = new Point(14, 190);
            BtnSkip.Margin = new Padding(4, 3, 4, 3);
            BtnSkip.Name = "BtnSkip";
            BtnSkip.Size = new Size(310, 27);
            BtnSkip.TabIndex = 4;
            BtnSkip.Text = "Keep as unknown";
            BtnSkip.UseVisualStyleBackColor = true;
            BtnSkip.Click += BtnSkip_Click;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(14, 233);
            label3.Margin = new Padding(4, 0, 4, 0);
            label3.Name = "label3";
            label3.Size = new Size(108, 15);
            label3.TabIndex = 5;
            label3.Text = "This packet header:";
            // 
            // HeaderDataLabel
            // 
            HeaderDataLabel.AutoSize = true;
            HeaderDataLabel.Location = new Point(14, 260);
            HeaderDataLabel.Margin = new Padding(4, 0, 4, 0);
            HeaderDataLabel.Name = "HeaderDataLabel";
            HeaderDataLabel.Size = new Size(35, 15);
            HeaderDataLabel.TabIndex = 6;
            HeaderDataLabel.Text = "DATA";
            // 
            // PacketTypeSelectForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = BtnSkip;
            ClientSize = new Size(343, 314);
            Controls.Add(HeaderDataLabel);
            Controls.Add(label3);
            Controls.Add(BtnSkip);
            Controls.Add(BtnOut);
            Controls.Add(label2);
            Controls.Add(BtnIn);
            Controls.Add(label1);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Margin = new Padding(4, 3, 4, 3);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "PacketTypeSelectForm";
            SizeGripStyle = SizeGripStyle.Hide;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Packet Direction";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button BtnIn;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button BtnOut;
        private System.Windows.Forms.Button BtnSkip;
        private System.Windows.Forms.Label label3;
        public System.Windows.Forms.Label HeaderDataLabel;
    }
}