﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Windows.Forms;
using Microsoft.ApplicationBlocks.Data;
using System.Data.SqlClient;

namespace Concesionaria.Clases
{
    public class cMensajesVentas
    {
        public void InsertarMensaje(string Mensaje, DateTime Fecha, Int32 CodVenta)
        {
            string sql = "Insert into MensajesVenta";
            sql = sql + "(Mensaje,Fecha,CodVenta)";
            sql = sql + "values(" + "'" + Mensaje + "'";
            sql = sql + "," + "'" + Fecha.ToShortDateString() + "'";
            sql = sql + "," + CodVenta.ToString();
            sql = sql + ")";
            cDb.ExecutarNonQuery(sql);
        }

        public DataTable GetMensajesxCodVenta(Int32 CodVenta)
        {
            string sql = "select Fecha,Mensaje from MensajesVenta";
            sql = sql + " where CodVenta =" + CodVenta.ToString();
            sql = sql + " order by Fecha Desc";
            return cDb.ExecuteDataTable(sql);
        }
    } 
}
