using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OfficeExcel = Microsoft.Office.Interop.Excel;
using System.Xml;

namespace ReporteComedorUY
{
    public partial class ReporteComedorUY : Form
    {

        private string desde = "";
        private string hasta = "";
        int cant;
        int porciento = 0;
        int countPro = 0;

        public ReporteComedorUY()
        {
            InitializeComponent();
            InitializeDate();
            BuscarConsumos(desde, hasta, "Todos los centros");
        }

        private void InitializeDate()
        {
            var currentMonth = DateTime.Now.Month;
            var lastDay = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);

            fechaDesdePicker.Value = new DateTime(DateTime.Now.Year, currentMonth, 01);
            fechaHastaPicker.Value = new DateTime(DateTime.Now.Year, currentMonth, lastDay);

            desde = fechaDesdePicker.Value.ToString("yyyyMMdd");
            hasta = fechaHastaPicker.Value.ToString("yyyyMMdd");
        }

        private void BuscarConsumos(string desde, string hasta, string centro)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["ApplicationDatabase"].ConnectionString;

            using (SqlConnection sql = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SP_CONSUMOS_TEST", sql))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@Fecha_Desde", desde));
                    cmd.Parameters.Add(new SqlParameter("@Fecha_Hasta", hasta));
                    cmd.Parameters.Add(new SqlParameter("@CentroDeCosto", centro));

                    DataSet ds = new DataSet();
                    DataTable dt = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    sql.Open();
                    da.Fill(dt);
                    cant = dt.Rows.Count;
                    lblCantidad.Text = cant.ToString();
                    
                    dataGridView1.DataSource = dt;
                    dataGridView1.Show();
                }
            }

            

            // BACKUP
            //using (SqlConnection sql = new SqlConnection(connectionString))
            //{
            //    using (SqlCommand cmd = new SqlCommand("SP_CONSUMOS_TEST", sql))
            //    {
            //        cmd.CommandType = CommandType.StoredProcedure;
            //        cmd.Parameters.Add(new SqlParameter("@Fecha_Desde", desde));
            //        cmd.Parameters.Add(new SqlParameter("@Fecha_Hasta", hasta));
            //        cmd.Parameters.Add(new SqlParameter("@CentroDeCosto", "Todos los centros"));

            //        DataSet ds = new DataSet();
            //        DataTable dt = new DataTable();
            //        SqlDataAdapter da = new SqlDataAdapter(cmd);
            //        sql.Open();
            //        da.Fill(dt);

            //        dataGridView1.DataSource = dt;
            //        dataGridView1.Show();
            //    }
            //}
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string _desde = fechaDesdePicker.Value.ToString("yyyyMMdd");
            string _hasta = fechaHastaPicker.Value.ToString("yyyyMMdd");
            string _centro = cmbCentros.Text;

            BuscarConsumos(_desde, _hasta, _centro);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OfficeExcel._Application app = new OfficeExcel.Application();
            OfficeExcel._Workbook workbook = app.Workbooks.Add(Type.Missing);
            OfficeExcel._Worksheet worksheet = null;

            worksheet = workbook.Sheets["Hoja1"];
            worksheet = workbook.ActiveSheet;
            worksheet.Name = "Consumos";

            for (int i = 1; i < dataGridView1.Columns.Count+1; i++)
            {
                worksheet.Cells[1, i] = dataGridView1.Columns[i - 1].HeaderText;
            }

            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                for (int j = 0; j < dataGridView1.Columns.Count; j++)
                {
                    if(dataGridView1.Rows[i].Cells[j].Value != null)
                    {
                        worksheet.Cells[i + 2, j + 1] = dataGridView1.Rows[i].Cells[j].Value.ToString();
                    }                 
                }


                countPro++;
                porciento = Convert.ToInt32((((double)countPro / (int)cant) * 100.00));
                if (countPro >= cant)
                    pbarExcel.Value = pbarExcel.Maximum;
                else
                    pbarExcel.Value = porciento;
                    lblPorcentaje.Text = porciento.ToString() + "%";
            }
            worksheet.Columns.AutoFit();

            var saveFileDialoge = new SaveFileDialog();
            saveFileDialoge.FileName = "ReporteConsumosComedor-UY" + "_" + fechaDesdePicker.Value.ToString("yyyyMMdd") + "_" + fechaHastaPicker.Value.ToString("yyyyMMdd");
            saveFileDialoge.DefaultExt = ".xlsx";

            if(saveFileDialoge.ShowDialog() == DialogResult.OK)
            {
                workbook.SaveAs(saveFileDialoge.FileName, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, OfficeExcel.XlSaveAsAccessMode.xlExclusive, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);

            }
            porciento = 0;
            countPro = 0;
            lblPorcentaje.Text = "";
            pbarExcel.Value = 0;
            
            app.Quit();
        }

        private void ReporteComedorUY_Load(object sender, EventArgs e)
        {
            pbarExcel.Maximum = 100;
            pbarExcel.Minimum = 0;
            pbarExcel.Step = 1;
            cmbCentros.Items.Clear();

            var connectionString = ConfigurationManager.ConnectionStrings["ApplicationDatabase"].ConnectionString;

            using (SqlConnection sql = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SP_GET_CENTROS", sql))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    DataSet ds = new DataSet();
                    DataTable dt = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    sql.Open();
                    da.Fill(dt);

                    dt.Rows.Add("Todos los centros");

                    cmbCentros.DataSource = dt;
                    cmbCentros.DisplayMember = "Centros";
                    cmbCentros.ValueMember = "Centro_Costo";
                }
            }

            cmbCentros.SelectedValue = "Todos los centros";

        }

    }
}
