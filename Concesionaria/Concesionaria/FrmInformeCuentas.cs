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
    public partial class FrmInformeCuentas : Form
    {
        public FrmInformeCuentas()
        {
            InitializeComponent();
        }

        private void FrmInformeCuentas_Load(object sender, EventArgs e)
        {
            Clases.cFunciones fun = new Clases.cFunciones();
            DataTable tbMoneda = cDb.ExecuteDataTable("select * from moneda order by codmoneda");
            fun.LlenarComboDatatable(cmbMoneda, tbMoneda, "Nombre", "CodMoneda");
            if (cmbMoneda.Items.Count > 0)
                cmbMoneda.SelectedIndex = 1;
            Actualizar();
        }

        private void Actualizar()
        {
            if (cmbMoneda.SelectedIndex <1)
            {
                MessageBox.Show("Debe selecionar una moneda", "Sistema");
                return;
            }
            int CodMoneda = Convert.ToInt32(cmbMoneda.SelectedValue);
            Clases.cFunciones fun = new Clases.cFunciones();
            //obtengo el importe de documentos a cobrar
            GetDeudasxPrestamo();
            GetEfectivosaPagar();
            GetCobranzaGeneral();
            GetChequesPagar(CodMoneda);
          
            Clases.cCuota cuota = new Clases.cCuota();
            Clases.cCuotasAnteriores cuotasAnt = new Clases.cCuotasAnteriores();
            Clases.cCheque cheque = new Clases.cCheque();
            Double ImporteCheque = cheque.GetTotalChequesaCobrar();
            Clases.cChequeCobrar chequeCob = new Clases.cChequeCobrar();
            ImporteCheque = ImporteCheque + chequeCob.GetTotalChequesaCobrar();
            Double Importe = cuota.GetMontoCuotasImpagas(CodMoneda);
            Double ImporteCuotasAnteriores = cuotasAnt.GetMontoCuotasImpagas();
            Importe = Importe + ImporteCuotasAnteriores;
            Double ImporteSinInteres = cuota.GetMontoCuotasImpagasSinInteres(CodMoneda);
            double ImporteSinInteresCuotasAnt = cuotasAnt.GetMontoCuotasImpagasSinInteres();
            ImporteSinInteres = ImporteSinInteres + ImporteSinInteresCuotasAnt;
            txtDocumentos.Text = fun.TransformarEntero(Importe.ToString().Replace(",", "."));
            txtDocumentos.Text = fun.FormatoEnteroMiles(txtDocumentos.Text);
            txtDocumentosSinInteres.Text = ImporteSinInteres.ToString();
            txtDocumentosSinInteres.Text = fun.TransformarEntero(ImporteSinInteres.ToString ());
            txtDocumentosSinInteres.Text = fun.FormatoEnteroMiles(txtDocumentosSinInteres.Text);
            txtTotalCheque.Text = fun.FormatoEnteroMiles(ImporteCheque.ToString());
            Clases.cResumenCuentas res = new Clases.cResumenCuentas();
            DataTable trdo = res.GetResumenCuentas(CodMoneda);
            if (trdo.Rows.Count > 0)
            {
                if (trdo.Rows[0]["ImporteEfectivo"].ToString() != "")
                {
                    double ImporteEfectivo = Convert.ToDouble(trdo.Rows[0]["ImporteEfectivo"]);
                    txtEfectivo.Text = fun.TransformarEntero(ImporteEfectivo.ToString().Replace(",", "."));
                    txtEfectivo.Text = fun.FormatoEnteroMiles(txtEfectivo.Text);

                    double ImporteAuto = Convert.ToDouble(trdo.Rows[0]["ImporteAuto"]);
                    txtVehículo.Text = fun.TransformarEntero(ImporteAuto.ToString().Replace(",", "."));
                    txtVehículo.Text = fun.FormatoEnteroMiles(txtVehículo.Text);

                    double ImportePrenda = Convert.ToDouble(trdo.Rows[0]["ImportePrenda"]);
                    txtPrenda.Text = fun.TransformarEntero(ImportePrenda.ToString().Replace(",", "."));
                    txtPrenda.Text = fun.FormatoEnteroMiles(txtPrenda.Text);

                    double ImporteCobranza = Convert.ToDouble(trdo.Rows[0]["ImporteCobranza"]);
                    txtCobranzas.Text = fun.TransformarEntero(ImporteCobranza.ToString().Replace(",", "."));
                    txtCobranzas.Text = fun.FormatoEnteroMiles(txtCobranzas.Text);

                    double ImporteBanco = Convert.ToDouble(trdo.Rows[0]["ImporteBanco"]);
                    txtBanco.Text = fun.TransformarEntero(ImporteBanco.ToString().Replace(",", "."));
                    txtBanco.Text = fun.FormatoEnteroMiles(txtBanco.Text);
                }
                else
                {
                    txtEfectivo.Text = "0";
                    txtPrenda.Text = "0";
                    txtCobranzas.Text = "0";
                    txtBanco.Text = "0";
                }
            }
                

                Clases.cComisionVendedor com = new Clases.cComisionVendedor();
                txtComisiones.Text = com.GetComisionesPendientes().ToString();
                if (txtComisiones.Text != "")
                {
                    txtComisiones.Text = fun.FormatoEnteroMiles(txtComisiones.Text);
                }

                Clases.cGastosPagar gasto = new Clases.cGastosPagar();
                txtGastosPendientes.Text = gasto.GetGastosaPagar().ToString();

                if (txtGastosPendientes.Text != "")
                {
                    txtGastosPendientes.Text = fun.FormatoEnteroMiles(txtGastosPendientes.Text);
                }

                Clases.cCobranza cob = new Clases.cCobranza();
                txtCobranzas.Text = cob.GetTotalDeudaCobranzas(CodMoneda).ToString ();
                txtCobranzas.Text = fun.FormatoEnteroMiles(txtCobranzas.Text);
                GetPrendas(CodMoneda);
                GetTotalVehiculo(CodMoneda);
                GetTarjeta();
            
            btnActualizar.Focus();
        }

        private void GetTarjeta()
        {
            Clases.cFunciones fun = new Clases.cFunciones();
            Clases.cTarjeta tarjeta = new Clases.cTarjeta();
            Double Importe = tarjeta.GetSaldoTarjeta();
            txtTarjeta.Text = Importe.ToString();
            if (Importe > 0)
            {
                txtTarjeta.Text = fun.SepararDecimales(txtTarjeta.Text);
                txtTarjeta.Text = fun.FormatoEnteroMiles(txtTarjeta.Text);
            }
        }


        private void btnActualizar_Click(object sender, EventArgs e)
        {
            Actualizar();
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void GetDeudasxPrestamo()
        {
            Clases.cFunciones fun = new Clases.cFunciones();
            Clases.cPrestamo prestamo = new Clases.cPrestamo();
            double Importe = prestamo.GetTotalDeudaPrestamo();
            txtDeudaxPrestamo.Text = Importe.ToString();
            if (Importe > 0)
            {
                txtDeudaxPrestamo.Text = fun.SepararDecimales(txtDeudaxPrestamo.Text);
                txtDeudaxPrestamo.Text = fun.FormatoEnteroMiles(txtDeudaxPrestamo.Text);
            }
        }

        private void GetChequesPagar(int CodMoneda)
        {
            Clases.cChequesaPagar che = new Clases.cChequesaPagar();
            txtTotalDeudaCheque.Text = che.GetTotalChequesaPagar(CodMoneda).ToString ();
            Clases.cFunciones fun = new Clases.cFunciones ();
            txtTotalDeudaCheque.Text = fun.FormatoEnteroMiles (txtTotalDeudaCheque.Text);
        }

        private void GetCobranzaGeneral()
        {
            Clases.cCobranzaGeneral cob = new Clases.cCobranzaGeneral();
            double Importe = cob.GetTotalCobranza();
            txtCobranzaGeneral.Text = Importe.ToString(); 
            Clases.cFunciones fun = new Clases.cFunciones();
            if (Importe > 0)
            {
                txtCobranzaGeneral.Text = fun.SepararDecimales(txtCobranzaGeneral.Text);
                txtCobranzaGeneral.Text = fun.FormatoEnteroMiles(txtCobranzaGeneral.Text); 
            }
        }

        private void GetEfectivosaPagar()
        {
            Int32 CodMoneda = Convert.ToInt32(cmbMoneda.SelectedValue);
            Clases.cEfectivoaPagar eft = new Clases.cEfectivoaPagar();
            txtEfectivosaPagar.Text = eft.TotalSaldo(CodMoneda).ToString();
            Clases.cFunciones fun = new Clases.cFunciones();
            txtEfectivosaPagar.Text = fun.SepararDecimales(txtEfectivosaPagar.Text);
            txtEfectivosaPagar.Text = fun.FormatoEnteroMiles(txtEfectivosaPagar.Text); 
        }

        private void GetPrendas(int CodMoneda)
        {
            Clases.cPrenda prenda = new Clases.cPrenda();
            double Importe = prenda.GetTotalPrenda(CodMoneda);
            txtPrenda.Text = Importe.ToString();
            Clases.cFunciones fun = new Clases.cFunciones();
            txtPrenda.Text = fun.SepararDecimales(txtPrenda.Text);
            txtPrenda.Text = fun.FormatoEnteroMiles(txtPrenda.Text);
        }

        private void GetTotalVehiculo(int CodMoneda)
        { 
            Clases.cFunciones fun = new Clases.cFunciones();
            Clases.cStockAuto stock = new Clases.cStockAuto();
            DataTable trdo = stock.GetStockDetalladosVigente("", null, CodMoneda);
            double Total = fun.TotalizarColumnaCondicion(trdo, "Costo", "Concesion", "0");
            txtVehículo.Text = Total.ToString();
            if (txtVehículo.Text != "")
            {
                txtVehículo.Text = fun.SepararDecimales(txtVehículo.Text);
                txtVehículo.Text = fun.FormatoEnteroMiles(txtVehículo.Text);
            }
        }
    }
}
