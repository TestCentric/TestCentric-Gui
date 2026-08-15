using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestCentric.Gui.SettingsPages
{
    public partial class AdvancedLoaderSettingsPage
    {
        private CheckBox principalPolicyCheckBox;
        private Label label7;
        private Label label6;
        private GroupBox groupBox1;
        private ListBox principalPolicyListBox;
        private Label label4;
        private GroupBox groupBox2;
        private CheckBox numberOfAgentsCheckBox;
        private NumericUpDown numberOfAgentsUpDown;
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #region Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AdvancedLoaderSettingsPage));
            this.principalPolicyCheckBox = new System.Windows.Forms.CheckBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.principalPolicyListBox = new System.Windows.Forms.ListBox();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.numberOfAgentsCheckBox = new System.Windows.Forms.CheckBox();
            this.numberOfAgentsUpDown = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.numberOfAgentsUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // principalPolicyCheckBox
            // 
            this.principalPolicyCheckBox.AutoSize = true;
            this.principalPolicyCheckBox.Location = new System.Drawing.Point(24, 94);
            this.principalPolicyCheckBox.Name = "principalPolicyCheckBox";
            this.principalPolicyCheckBox.Size = new System.Drawing.Size(214, 17);
            this.principalPolicyCheckBox.TabIndex = 9;
            this.principalPolicyCheckBox.Text = "Set Principal Policy for test AppDomains";
            this.principalPolicyCheckBox.UseVisualStyleBackColor = true;
            this.principalPolicyCheckBox.CheckedChanged += new System.EventHandler(this.principalPolicyCheckBox_CheckedChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(42, 123);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(38, 13);
            this.label7.TabIndex = 10;
            this.label7.Text = "Policy:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(8, 70);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(78, 13);
            this.label6.TabIndex = 7;
            this.label6.Text = "Principal Policy";
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Location = new System.Drawing.Point(139, 70);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(309, 8);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            // 
            // principalPolicyListBox
            // 
            this.principalPolicyListBox.FormattingEnabled = true;
            this.principalPolicyListBox.Items.AddRange(new object[] {
            "UnauthenticatedPrincipal",
            "NoPrincipal",
            "WindowsPrincipal"});
            this.principalPolicyListBox.Location = new System.Drawing.Point(139, 123);
            this.principalPolicyListBox.Name = "principalPolicyListBox";
            this.principalPolicyListBox.Size = new System.Drawing.Size(241, 69);
            this.principalPolicyListBox.TabIndex = 11;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(8, 3);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(59, 13);
            this.label4.TabIndex = 13;
            this.label4.Text = "Agent Limit";
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Location = new System.Drawing.Point(139, 3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(309, 8);
            this.groupBox2.TabIndex = 14;
            this.groupBox2.TabStop = false;
            // 
            // numberOfAgentsCheckBox
            // 
            this.numberOfAgentsCheckBox.AutoSize = true;
            this.numberOfAgentsCheckBox.Location = new System.Drawing.Point(24, 31);
            this.numberOfAgentsCheckBox.Name = "numberOfAgentsCheckBox";
            this.numberOfAgentsCheckBox.Size = new System.Drawing.Size(205, 17);
            this.numberOfAgentsCheckBox.TabIndex = 44;
            this.numberOfAgentsCheckBox.Text = "Limit simultaneous agent processes to";
            this.numberOfAgentsCheckBox.UseVisualStyleBackColor = true;
            this.numberOfAgentsCheckBox.CheckedChanged += new System.EventHandler(this.numberOfAgentsCheckBox_CheckedChanged);
            // 
            // numberOfAgentsUpDown
            // 
            this.numberOfAgentsUpDown.Enabled = false;
            this.numberOfAgentsUpDown.Location = new System.Drawing.Point(324, 28);
            this.numberOfAgentsUpDown.Name = "numberOfAgentsUpDown";
            this.numberOfAgentsUpDown.Size = new System.Drawing.Size(66, 20);
            this.numberOfAgentsUpDown.TabIndex = 43;
            // 
            // AdvancedLoaderSettingsPage
            // 
            this.Controls.Add(this.numberOfAgentsCheckBox);
            this.Controls.Add(this.numberOfAgentsUpDown);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.principalPolicyListBox);
            this.Controls.Add(this.principalPolicyCheckBox);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.groupBox1);
            this.Name = "AdvancedLoaderSettingsPage";
            ((System.ComponentModel.ISupportInitialize)(this.numberOfAgentsUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion
    }
}
