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

namespace ReporteComedorUY
{
    public partial class ReporteComedorUY : Form
    {

        private string desde = "";
        private string hasta = "";


        public ReporteComedorUY()
        {
            InitializeComponent();
            InitializeDate();
            BuscarConsumos(desde, hasta);
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

        private void BuscarConsumos(string desde, string hasta)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["ApplicationDatabase"].ConnectionString;

            using (SqlConnection sql = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SP_CONSUMOS_TEST", sql))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@Fecha_Desde", desde));
                    cmd.Parameters.Add(new SqlParameter("@Fecha_Hasta", hasta));
                    cmd.Parameters.Add(new SqlParameter("@CentroDeCosto", "Todos los centros"));

                    DataSet ds = new DataSet();
                    DataTable dt = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    sql.Open();
                    da.Fill(dt);

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

            BuscarConsumos(_desde, _hasta);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OfficeExcel._Application app = new OfficeExcel.Application();
            OfficeExcel._Workbook workbook = app.Workbooks.Add(Type.Missing);
            OfficeExcel._Worksheet worksheet = null;

            worksheet = workbook.Sheets["Hoja1"];
            worksheet = workbook.ActiveSheet;
            worksheet.Name = "CustomerDetails";

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
            }

            var saveFileDialoge = new SaveFileDialog();
            saveFileDialoge.FileName = "ReporteConsumosComedor UY";
            saveFileDialoge.DefaultExt = ".xlsx";

            if(saveFileDialoge.ShowDialog() == DialogResult.OK)
            {
                workbook.SaveAs(saveFileDialoge.FileName, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, OfficeExcel.XlSaveAsAccessMode.xlExclusive, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);

            }

            app.Quit();
        }
    }
}
