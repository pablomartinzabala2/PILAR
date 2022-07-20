using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Concesionaria.Clases;
using System.Data.SqlClient;
namespace Concesionaria
{
    public partial class FrmListadoCompra : Form
    {
        public FrmListadoCompra()
        {
            InitializeComponent();
        }

        private void FrmListadoCompra_Load(object sender, EventArgs e)
        {
            DateTime Fecha = DateTime.Now;
            txtFechaHasta.Text = Fecha.ToShortDateString();
            Fecha = Fecha.AddMonths(-1);
            txtFechaDesde.Text  = Fecha.ToShortDateString();
            Buscar();
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            cFunciones fun = new cFunciones();
            if (fun.ValidarFecha (txtFechaDesde.Text)==false)
            {
                Mensaje("La fecha desde es incorrecta");
                return;
            }
             
            if (fun.ValidarFecha(txtFechaHasta.Text) == false)
            {
                Mensaje("La fecha Hasta es incorrecta");
                return;
            }
            DateTime FechaDesde = Convert.ToDateTime(txtFechaDesde.Text);
            DateTime FechaHasta = Convert.ToDateTime(txtFechaHasta.Text);
            string Patente = txtPatente.Text.Trim();
            cCompra compra = new cCompra();
            DataTable trdo = compra.getComprasxFecha(FechaDesde, FechaHasta, Patente);
            Grilla.DataSource = trdo;
        }

        private void Mensaje(string msj)
        {
            MessageBox.Show(msj, "Sistema");
        }

        private void btnBuscarCompra_Click(object sender, EventArgs e)
        {
            cFunciones fun = new cFunciones();
            if (fun.ValidarFecha(txtFechaDesde.Text) == false)
            {
                Mensaje("La fecha desde es incorrecta");
                return;
            }

            if (fun.ValidarFecha(txtFechaHasta.Text) == false)
            {
                Mensaje("La fecha Hasta es incorrecta");
                return;
            }
            Buscar();
        }

        private void Buscar()
        {
            cFunciones fun = new cFunciones();
            DateTime FechaDesde = Convert.ToDateTime(txtFechaDesde.Text);
            DateTime FechaHasta = Convert.ToDateTime(txtFechaHasta.Text);
            string Patente = txtPatente.Text.Trim();
            cCompra compra = new cCompra();
            DataTable trdo = compra.getComprasxFecha(FechaDesde, FechaHasta, Patente);
            trdo = fun.TablaaMiles(trdo, "ImporteCompra");
            Grilla.DataSource = trdo;
            string Col = "0;20;20;20;20;20";
            fun.AnchoColumnas(Grilla, Col);
            Grilla.Columns[5].HeaderText = "Importe Compra";
        }

        private void btnAbrirCompra_Click(object sender, EventArgs e)
        {
            if (Grilla.CurrentRow ==null)
            {
                Mensaje("Debe seleccionar un registro");
                return;
            }
            string CodCompra = Grilla.CurrentRow.Cells[0].Value.ToString();
            Principal.CodCompra = CodCompra;
            FrmAutos frm = new Concesionaria.FrmAutos();
            frm.ShowDialog();
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            string msj = "Confirma anular la Compra ";
            var result = MessageBox.Show(msj, "Información",
                                 MessageBoxButtons.YesNo,
                                 MessageBoxIcon.Question);

            // If the no button was pressed ...
            if (result == DialogResult.No)
            {
                return;
            }

            if (Grilla.CurrentRow ==null)
            {
                MessageBox.Show("Debe seleccionar un registro para continuar");
                return;
            }
            Int32 CodCompra =Convert.ToInt32 (Grilla.CurrentRow.Cells[0].Value.ToString ()); 
            Double ImporteEfectivo = 0;
            Int32 CodStockEntrada = 0;
            Int32? CodStockSalida = null;
            string Patente = "";
            int CodMoneda = 0;
            cCompra compra = new cCompra();
            DataTable trdo = compra.GetCompraxCodigo(Convert.ToInt32 (CodCompra));
            DateTime Fecha = DateTime.Now;
            string Descripcion = "";
            if (trdo.Rows.Count >0)
            {
                if (trdo.Rows[0]["ImporteEfectivo"].ToString ()!="")
                {
                    ImporteEfectivo = Convert.ToDouble(trdo.Rows[0]["ImporteEfectivo"].ToString());
                }

                if (trdo.Rows[0]["CodStockEntrada"].ToString() != "")
                {
                    CodStockEntrada = Convert.ToInt32(trdo.Rows[0]["CodStockEntrada"].ToString());
                }

                if (trdo.Rows[0]["CodStockSalida"].ToString() != "")
                {
                    CodStockSalida = Convert.ToInt32(trdo.Rows[0]["CodStockSalida"].ToString());
                }

                Patente = trdo.Rows[0]["Patente"].ToString();
                Descripcion = "Anulación Compra , Patente " + Patente;
            }
            cEfectivoaPagar eft = new cEfectivoaPagar();
            cStockAuto stock = new cStockAuto();
            cChequesaPagar cheque = new cChequesaPagar();
            SqlConnection con = new SqlConnection();
            cMovimiento mov = new cMovimiento();
            con.ConnectionString = Clases.cConexion.Cadenacon();
            con.Open();
            SqlTransaction Transaccion;
            Transaccion = con.BeginTransaction();
            try
            {
                CodMoneda = compra.GetCodMoneda(con, Transaccion, CodCompra);
                compra.AnularCompra(con, Transaccion,Convert.ToInt32 (CodCompra));
                stock.InsertarBajaStockTran(con, Transaccion, CodStockEntrada, DateTime.Now);
                if (CodStockSalida !=null)
                {
                    stock.InsertarAltaStockTran(con, Transaccion, Convert.ToInt32 (CodStockSalida));
                }
                if (ImporteEfectivo > 0)
                    mov.RegistrarMovimientoDescripcionTransaccion(con, Transaccion, 0, Principal.CodUsuarioLogueado, ImporteEfectivo, 0, 0, 0, 0, Fecha, Descripcion, CodCompra, CodMoneda);
                cheque.BorrarChequesaPagar(con, Transaccion, Convert.ToInt32(CodCompra));
                eft.BorrarEfectivoaPagar(con, Transaccion, CodCompra);
                Transaccion.Commit();
                con.Close();
                MessageBox.Show("Datos anulados correctamente", Clases.cMensaje.Mensaje());
                Buscar();    
            }
            catch (Exception ex)
            {
                msj = "Hubo un error en el proceso " + ex.Message.ToString();
                MessageBox.Show(msj, Clases.cMensaje.Mensaje());
                Transaccion.Rollback();
                con.Close();
            }
        }
    }
}
