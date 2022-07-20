using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Concesionaria.Clases;

namespace Concesionaria
{
    public partial class FrmListadoMovimientos : Form
    {
        public FrmListadoMovimientos()
        {
            InitializeComponent();
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            Clases.cFunciones fun = new Clases.cFunciones();
            if (fun.ValidarFecha(txtFechaDesde.Text) == false)
            {
                MessageBox.Show("Fecha desde incorrecta", Clases.cMensaje.Mensaje());
                return;
            }

            if (fun.ValidarFecha(txtFechaHasta.Text) == false)
            {
                MessageBox.Show("Fecha hasta incorrecta", Clases.cMensaje.Mensaje());
                return;
            }

            if (Convert.ToDateTime(txtFechaDesde.Text) > Convert.ToDateTime(txtFechaHasta.Text))
            {
                MessageBox.Show("La fecha desde debe ser inferior a la fecha hasta", Clases.cMensaje.Mensaje());
                return;
            }

            if (cmbMoneda.SelectedIndex <1)
            {
                MessageBox.Show("Debe seleccionar una moneda", Clases.cMensaje.Mensaje());
                return;

            }

            int CodMoneda = Convert.ToInt32(cmbMoneda.SelectedValue);
            DateTime FechaDesde = Convert.ToDateTime(txtFechaDesde.Text);
            DateTime FechaHasta = Convert.ToDateTime(txtFechaHasta.Text);
            string Concepto = txtConcepto.Text;
            Clases.cMovimiento mov = new Clases.cMovimiento();
            DataTable trdo = mov.GetMovimientoxFecha(FechaDesde, FechaHasta, Concepto, CodMoneda);

            DataTable tResul = new DataTable();
            tResul.Columns.Add("Fecha");
            tResul.Columns.Add("Descripcion");
            tResul.Columns.Add("Ingreso");
            tResul.Columns.Add("Egreso");
            double TotalIngresos = 0;
            double TotalEgresos = 0;
            for (int i = 0; i < trdo.Rows.Count; i++)
            {
                string sFecha = trdo.Rows[i]["Fecha"].ToString();
                string sDescripcion = trdo.Rows[i]["Descripcion"].ToString();
                double Importe = Convert.ToDouble(trdo.Rows[i]["ImporteEfectivo"].ToString());
                DataRow r = tResul.NewRow();
                r["Fecha"] = sFecha;
                r["Descripcion"] = sDescripcion;
                if (Importe > 0)
                {
                    TotalIngresos = TotalIngresos + Importe;
                    r["Ingreso"] = Importe.ToString();
                    r["Egreso"] = "";
                }
                else
                {
                    TotalEgresos = TotalEgresos + Importe;
                    Importe = (-1) * Importe;
                    r["Ingreso"] = "";
                    r["Egreso"] = Importe.ToString();
                }
                tResul.Rows.Add(r);
            }
            tResul = fun.TablaaMiles(tResul, "Ingreso");
            tResul = fun.TablaaMiles(tResul, "Egreso");
            trdo = fun.TablaaMiles(trdo, "ImporteEfectivo");
            //Grilla.DataSource = trdo;
            Grilla.DataSource = tResul;
            Grilla.Columns[1].Width = 400;
            Grilla.Columns[2].Width = 150;
            Grilla.Columns[3].Width = 155;
            Grilla.Columns[1].HeaderText = "Descripción";
            double Total = TotalIngresos + TotalEgresos;
            txtTotal.Text = Total.ToString();
            txtTotal.Text = fun.FormatoEnteroMiles(txtTotal.Text);
        }

        private void FrmListadoMovimientos_Load(object sender, EventArgs e)
        {
            DateTime fechahoy = DateTime.Now;
            txtFechaHasta.Text = fechahoy.ToShortDateString();
            fechahoy = fechahoy.AddMonths(-1);
            txtFechaDesde.Text = fechahoy.ToShortDateString();
            DataTable tbMoneda = cDb.ExecuteDataTable("select * from moneda order by codmoneda");
            cFunciones fun = new Clases.cFunciones();
            fun.LlenarComboDatatable(cmbMoneda, tbMoneda, "Nombre", "CodMoneda");
            if (cmbMoneda.Items.Count > 0)
                cmbMoneda.SelectedIndex = 1;
        }
    }
}
