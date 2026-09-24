// ***********************************************************************
// Copyright (c) Charlie Poole and TestCentric contributors.
// Licensed under the MIT License. See LICENSE file in root directory.
// ***********************************************************************

using System;
using System.Windows.Forms;

namespace TestCentric.Gui
{
    public partial class MessageTesterForm : Form
    {
        public MessageTesterForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.Text = "Very short message";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox1.Text = $"Failed to enable logging. Might be due to missing access rights in folder {Environment.CurrentDirectory}. Please consider to start with admin rights.";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            textBox1.Text =
                "Either the GUI was installed without any agents or the installed agents have been deleted. " +
                "Some agents may have encountered errors in loading.\r\n\r\n" +
                "You must install at least one agent in order to be able to load or run tests. " +
                "Install agents using the same source (i.e. nuget or chocolatey) from which you installed the GUI itself." +
                "You should select agents which match the target platforms you are using for development.\r\n\r\n" +
                "Click 'OK' to continue with extremely limited functionality, 'Cancel' to exit.";
        }

        private void button4_Click(object sender, EventArgs e)
        {
            textBox1.Text = new GuiOptions().GetHelpText();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (radioButton6.Checked)
            {
                IMessageDisplay messageDisplayForm = new MessageDisplayForm("Message Tester")
                {
                    StartPosition = FormStartPosition.CenterScreen
                };
                if (radioButton1.Checked)
                    messageDisplayForm.Error(textBox1.Text);
                else if (radioButton2.Checked)
                    messageDisplayForm.Info(textBox1.Text);
                else if (radioButton3.Checked)
                    messageDisplayForm.YesNo(textBox1.Text);
                else if (radioButton4.Checked)
                    messageDisplayForm.OkCancel(textBox1.Text);
                else if (radioButton5.Checked)
                    messageDisplayForm.YesNoCancel(textBox1.Text);
            }
            else if (radioButton7.Checked)
            {
                IMessageDisplay messageDisplayForm = new MessageDisplayForm("Message Tester")
                {
                    StartPosition = FormStartPosition.CenterParent
                };
                if (radioButton1.Checked)
                    messageDisplayForm.Error(textBox1.Text);
                else if (radioButton2.Checked)
                    messageDisplayForm.Info(textBox1.Text);
                else if (radioButton3.Checked)
                    messageDisplayForm.YesNo(textBox1.Text);
                else if (radioButton4.Checked)
                    messageDisplayForm.OkCancel(textBox1.Text);
                else if (radioButton5.Checked)
                    messageDisplayForm.YesNoCancel(textBox1.Text);
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
