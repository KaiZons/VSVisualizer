using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ListViewVisualizer.View
{
    public partial class ShowData : Form
    {
        private DataTable m_datatable;
        public ShowData(DataTable dataTable)
        {
            InitializeComponent();
            m_datatable = dataTable;
        }

        private void OnShowDataLoad(object sender, EventArgs e)
        {
            this.m_dataGridView.DataSource = m_datatable;
        }
    }
}
