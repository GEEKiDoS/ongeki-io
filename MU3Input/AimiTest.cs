using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MU3Input
{
    public partial class AimiTest : Form
    {
        public AimiTest()
        {
            InitializeComponent();
        }

        public void AddOutput(string str)
        {
            textBox1.Text += str;
        }

        private bool btnDown = false;

        public bool GetIsButtonDown()
        {
            if(btnDown)
            {
                btnDown = false;
                return true;
            }

            return false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            btnDown = true;
        }
    }
}
