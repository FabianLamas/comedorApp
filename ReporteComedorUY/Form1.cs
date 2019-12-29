using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ReporteComedorUY
{
    public partial class ReporteComedorUY : Form
    {
        public ReporteComedorUY()
        {
            InitializeComponent();
        }

        private void GetConnection()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["defaultConnection"].ConnectionString;

            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string _desde = fechaDesdePicker.Value.ToString("yyyyMMdd");
            string _hasta = fechaHastaPicker.Value.ToString("yyyyMMdd");

            label3.Text = _desde + " - " + _hasta;
        }

        
    }
}
