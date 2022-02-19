namespace VieweD
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
            this.label1 = new System.Windows.Forms.Label();
            this.btnIN = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.btnOUT = new System.Windows.Forms.Button();
            this.btnSkip = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.lHeaderData = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(266, 54);
            this.label1.TabIndex = 0;
            this.label1.Text = "Unable to indentify the packet direction.\r\nDo you want to assign a default direct" +
    "ion for the remaining data ?";
            // 
            // btnIN
            // 
            this.btnIN.Location = new System.Drawing.Point(12, 79);
            this.btnIN.Name = "btnIN";
            this.btnIN.Size = new System.Drawing.Size(80, 80);
            this.btnIN.TabIndex = 1;
            this.btnIN.Text = "Incoming";
            this.btnIN.UseVisualStyleBackColor = true;
            this.btnIN.Click += new System.EventHandler(this.btnIN_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 63);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(150, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Treat all unknown packets as:";
            // 
            // btnOUT
            // 
            this.btnOUT.Location = new System.Drawing.Point(198, 79);
            this.btnOUT.Name = "btnOUT";
            this.btnOUT.Size = new System.Drawing.Size(80, 80);
            this.btnOUT.TabIndex = 3;
            this.btnOUT.Text = "Outgoing";
            this.btnOUT.UseVisualStyleBackColor = true;
            this.btnOUT.Click += new System.EventHandler(this.btnOUT_Click);
            // 
            // btnSkip
            // 
            this.btnSkip.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnSkip.Location = new System.Drawing.Point(12, 165);
            this.btnSkip.Name = "btnSkip";
            this.btnSkip.Size = new System.Drawing.Size(266, 23);
            this.btnSkip.TabIndex = 4;
            this.btnSkip.Text = "Keep as unknown";
            this.btnSkip.UseVisualStyleBackColor = true;
            this.btnSkip.Click += new System.EventHandler(this.btnSkip_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 202);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(102, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "This packet header:";
            // 
            // lHeaderData
            // 
            this.lHeaderData.AutoSize = true;
            this.lHeaderData.Location = new System.Drawing.Point(12, 225);
            this.lHeaderData.Name = "lHeaderData";
            this.lHeaderData.Size = new System.Drawing.Size(36, 13);
            this.lHeaderData.TabIndex = 6;
            this.lHeaderData.Text = "DATA";
            // 
            // PacketTypeSelectForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnSkip;
            this.ClientSize = new System.Drawing.Size(294, 272);
            this.Controls.Add(this.lHeaderData);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnSkip);
            this.Controls.Add(this.btnOUT);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnIN);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PacketTypeSelectForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Packet Direction";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnIN;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnOUT;
        private System.Windows.Forms.Button btnSkip;
        private System.Windows.Forms.Label label3;
        public System.Windows.Forms.Label lHeaderData;
    }
}