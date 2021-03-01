using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ObjectToJsonVisualizer
{
    public partial class JsonView : Form
    {
        private string m_json = string.Empty;
        public JsonView(string json)
        {
            InitializeComponent();
            this.Text = "JSON视图";
            m_json = json;
        }

        private void OnJsonViewLoad(object sender, EventArgs e)
        {
            this.m_richTextBox.Text = m_json;
        }
    }
}
