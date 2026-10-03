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
            this.components = new System.ComponentModel.Container();
            this.principalPolicyCheckBox = new System.Windows.Forms.CheckBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.principalPolicyListBox = new System.Windows.Forms.ListBox();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.numberOfAgentsCheckBox = new System.Windows.Forms.CheckBox();
            this.numberOfAgentsUpDown = new System.Windows.Forms.NumericUpDown();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.randomSeedCheckBox = new System.Windows.Forms.CheckBox();
            this.randomSeedTextBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.currentRandomSeedTextBox = new System.Windows.Forms.TextBox();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.numberOfAgentsUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
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
            this.label7.Location = new System.Drawing.Point(80, 123);
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
            this.principalPolicyListBox.Size = new System.Drawing.Size(241, 43);
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
            this.numberOfAgentsCheckBox.Size = new System.Drawing.Size(204, 17);
            this.numberOfAgentsCheckBox.TabIndex = 44;
            this.numberOfAgentsCheckBox.Text = "Limit simultaneous agent processes to";
            this.numberOfAgentsCheckBox.UseVisualStyleBackColor = true;
            this.numberOfAgentsCheckBox.CheckedChanged += new System.EventHandler(this.numberOfAgentsCheckBox_CheckedChanged);
            // 
            // numberOfAgentsUpDown
            // 
            this.numberOfAgentsUpDown.Enabled = false;
            this.numberOfAgentsUpDown.Location = new System.Drawing.Point(314, 31);
            this.numberOfAgentsUpDown.Name = "numberOfAgentsUpDown";
            this.numberOfAgentsUpDown.Size = new System.Drawing.Size(66, 20);
            this.numberOfAgentsUpDown.TabIndex = 43;
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Location = new System.Drawing.Point(139, 189);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(309, 8);
            this.groupBox3.TabIndex = 45;
            this.groupBox3.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 189);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(75, 13);
            this.label1.TabIndex = 46;
            this.label1.Text = "Random Seed";
            // 
            // randomSeedCheckBox
            // 
            this.randomSeedCheckBox.AutoSize = true;
            this.randomSeedCheckBox.Location = new System.Drawing.Point(44, 241);
            this.randomSeedCheckBox.Name = "randomSeedCheckBox";
            this.randomSeedCheckBox.Size = new System.Drawing.Size(157, 17);
            this.randomSeedCheckBox.TabIndex = 47;
            this.randomSeedCheckBox.Text = "Set RandomSeed manually:";
            this.randomSeedCheckBox.UseVisualStyleBackColor = true;
            this.randomSeedCheckBox.CheckedChanged += new System.EventHandler(this.randomSeedCheckBox_CheckedChanged);
            // 
            // randomSeedTextBox
            // 
            this.randomSeedTextBox.Enabled = false;
            this.randomSeedTextBox.Location = new System.Drawing.Point(314, 241);
            this.randomSeedTextBox.Name = "randomSeedTextBox";
            this.randomSeedTextBox.Size = new System.Drawing.Size(66, 20);
            this.randomSeedTextBox.TabIndex = 48;
            this.randomSeedTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.randomSeedTextBox_KeyPress);
            this.randomSeedTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.randomSeedTextBox_Validating);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(24, 215);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(153, 13);
            this.label3.TabIndex = 50;
            this.label3.Text = "Current value of RandomSeed:";
            // 
            // currentRandomSeedTextBox
            // 
            this.currentRandomSeedTextBox.Enabled = false;
            this.currentRandomSeedTextBox.Location = new System.Drawing.Point(314, 215);
            this.currentRandomSeedTextBox.Name = "currentRandomSeedTextBox";
            this.currentRandomSeedTextBox.Size = new System.Drawing.Size(66, 20);
            this.currentRandomSeedTextBox.TabIndex = 51;
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // AdvancedLoaderSettingsPage
            // 
            this.Controls.Add(this.currentRandomSeedTextBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.randomSeedTextBox);
            this.Controls.Add(this.randomSeedCheckBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.groupBox3);
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
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        private GroupBox groupBox3;
        private Label label1;
        private CheckBox randomSeedCheckBox;
        private TextBox randomSeedTextBox;
        private Label label3;
        private TextBox currentRandomSeedTextBox;
        private ErrorProvider errorProvider1;
    }
}
