using System;
using System.Collections.Generic;
using System.Data;
using System.Net.Http;
using System.Threading.Tasks;
using System.Linq;
using Newtonsoft.Json.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

public partial class Facturas : System.Web.UI.Page
{
    protected async void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            await Cargar();
        }
    }

    protected async void Cargar(object sender, EventArgs e)
    {
        await Cargar();
    }


    protected async Task Cargar()
    {
        GridView1.DataSource = await Listar();
        GridView1.DataBind();
    }

    /* Pre: Carga datos tabla
	 * Pro: Lo hace
	 * 
	 * ORG 25/10/2023
	 */

    public async Task<DataTable> Listar()
    {
        using (HttpClient client = new HttpClient())
        {
            HttpResponseMessage response = await client.GetAsync("https://facturass.azurewebsites.net/factura");

            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();

                JArray jsonArray = JArray.Parse(data);

                DataTable dataTable = new DataTable();

                dataTable.Columns.Add("id", typeof(int));
                dataTable.Columns.Add("fecha", typeof(DateTime));
                dataTable.Columns.Add("cif", typeof(string));
                dataTable.Columns.Add("nombre", typeof(string));
                dataTable.Columns.Add("importe", typeof(decimal));
                dataTable.Columns.Add("importe_iva", typeof(decimal));
                dataTable.Columns.Add("moneda", typeof(string));
                dataTable.Columns.Add("fecha_cobro", typeof(DateTime));
                dataTable.Columns.Add("estado", typeof(string));

                foreach(JObject a in jsonArray)
                {
                    DataRow newRow = dataTable.NewRow();

                    newRow["id"] = a["id"].ToObject<int>();
                    newRow["fecha"] = a["fecha"].ToObject<DateTime>();
                    newRow["cif"] = a["cif"].ToObject<string>();
                    newRow["nombre"] = a["nombre"].ToObject<string>();
                    newRow["importe"] = a["importe"].ToObject<decimal>();
                    newRow["importe_iva"] = a["importe_iva"].ToObject<decimal>();
                    newRow["moneda"] = a["moneda"].ToObject<string>();
                    newRow["fecha_cobro"] = a["fecha_cobro"].ToObject<DateTime>();
                    newRow["estado"] = a["estado"].ToObject<bool>() ? "Pagada" : "No Pagada" ;

                    dataTable.Rows.Add(newRow);
                }

                //Aplicamos los filtros y mierdas
                DataTable dataTableFiltrada = dataTable.Clone();
                foreach (DataRow fila in dataTable.Rows)
                {
                    DateTime fecha = DateTime.Parse(fila["fecha"].ToString()); // Ajusta "Fecha" al nombre real de la columna
                    string moneda = fila["moneda"].ToString(); // Ajusta "TipoMoneda" al nombre real de la columna
                    string estadoFila = fila["Estado"].ToString(); // Ajusta "Estado" al nombre real de la columna
                    decimal importe = decimal.Parse(fila["Importe"].ToString()); // Ajusta "Importe" al nombre real de la columna

                    // Aplicar filtros
                    if ((string.IsNullOrEmpty(f_desde.Text) || fecha >= DateTime.Parse(f_desde.Text))
                        && (string.IsNullOrEmpty(f_hasta.Text) || fecha <= DateTime.Parse(f_hasta.Text))
                        && (string.IsNullOrEmpty(f_moneda.SelectedValue) || moneda == f_moneda.SelectedValue)
                        && (string.IsNullOrEmpty(f_estado.SelectedValue) || estadoFila == f_estado.SelectedValue)
                        && (string.IsNullOrEmpty(f_importeMin.Text) || importe >= decimal.Parse(f_importeMin.Text))
                        && (string.IsNullOrEmpty(f_importeMax.Text) || importe <= decimal.Parse(f_importeMax.Text)))
                    {
                        // Agregar la fila a la DataTable filtrada
                        dataTableFiltrada.ImportRow(fila);
                    }
                }

                return dataTableFiltrada;
            }
        }

        return new DataTable();
    }

    /* Pre: Comprueba si hay algun textbox vacio y en funcion de si hay alguno vacio te lo bloque los botones correspondientes
	 * Pro: Lo hace
	 * 
	 * ORG 25/10/2023
	 */
    protected void Vacio(object sender, EventArgs e)
    {
        bool enableButton = true;
        B_Agregar.Enabled = enableButton;


        if (string.IsNullOrEmpty(txtFecha.Text) ||
            string.IsNullOrEmpty(ddlMoneda.SelectedValue) ||
            string.IsNullOrEmpty(txtCIF.Text) ||
            string.IsNullOrEmpty(txtNombre.Text) ||
            string.IsNullOrEmpty(txtImporte.Text) ||
            string.IsNullOrEmpty(txtImporteIVA.Text) ||
            string.IsNullOrEmpty(txtFechaCobro.Text))
        {
            B_Agregar.Enabled = false;
            throw new Exception("Rellena todos los datos");
        }
        else
        {
            B_Agregar.Enabled = true;
        }


    }
    /* Pre: Bloquea boton agregar si hay datos
    * Pro: Lo hace
    * 
    * ORG 25/10/2023
    
   
    protected void AplicarFiltros(object sender, EventArgs e)
    {
        DataView dataView = new DataView(datatable);

        string filtroMoneda = FiltroMoneda.SelectedValue;
        string filtroMetodoEnvio = FiltroEnvio.SelectedValue;

        if (filtroMoneda != "Todos" && filtroMetodoEnvio != "Todos")
        {
            dataView.RowFilter = $"Moneda = '{filtroMoneda}' AND MetodoEnvio = '{filtroMetodoEnvio}'";
        }
        else if (filtroMoneda != "Todos")
        {
            dataView.RowFilter = $"Moneda = '{filtroMoneda}'";
        }
        else if (filtroMetodoEnvio != "Todos")
        {
            dataView.RowFilter = $"MetodoEnvio = '{filtroMetodoEnvio}'";
        }
        Session["Filtro"] = dataView;

        GridView1.DataSource = dataView;
        GridView1.DataBind();
    }


    /* Pre: Hace una peticion a la api de tipo post con los datos introducidos en los textbox
	 * Pro: Lo hace
	 * 
	 * ORG 25/10/2023
	 */
    protected async void Post(JObject factura)
    {
        

        var jsonContent = Newtonsoft.Json.JsonConvert.SerializeObject(factura);

        using (HttpClient client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("accept", "text/plain");
            // client.DefaultRequestHeaders.Add("Content-Type", "application/json");


            var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync($"https://facturass.azurewebsites.net/factura", content);

            if (response.IsSuccessStatusCode)
            {
                GridView1.DataSource = await Listar();
                GridView1.DataBind();
            }
            else
            {
                string script = $"alert('Algun valor proporcionado no es valido; por favor revisa los datos y vuelve a intentarlo');";
                ClientScript.RegisterStartupScript(this.GetType(), "AlertScript", script, true);
            }
        }
    }
    /* Pre: Hace una peticion a la api de tipo get con el dato introducido en el textbox
    * Pro: Lo hace
    * 
    * ORG 25/10/2023
 

    /* Pre: Hace una peticion a la api de tipo delete con el dato introducido en el textbox
    * Pro: Lo hace
    * 
    * ORG 25/10/2023
    */
    protected void Eliminar(object sender, EventArgs e)
    {
        if (GridView1.SelectedIndex >= 0)
        {
            int rowIndex = GridView1.SelectedIndex;
            string idToDelete = GridView1.DataKeys[rowIndex]["id"].ToString();

            ViewState["IDToDelete"] = idToDelete;
            confirmationPanel.Style["display"] = "block";
        }
    }


 


    protected async Task Delete(string idToDelete)
    {
        using (HttpClient client = new HttpClient())
        {
            try
            {
                HttpResponseMessage response = await client.DeleteAsync($"https://facturass.azurewebsites.net/factura/{idToDelete}");

                if (response.IsSuccessStatusCode)
                {
                    GridView1.DataSource = await Listar();
                    GridView1.DataBind();
                }
                else
                {
                    string script = $"alert('Error al eliminar la factura con ID {idToDelete}');";
                    ClientScript.RegisterStartupScript(this.GetType(), "AlertScript", script, true);
                }
            }
            catch (Exception ex)
            {
                string script = $"alert('Error: {ex.Message}');";
                ClientScript.RegisterStartupScript(this.GetType(), "AlertScript", script, true);
            }
        }
    }


    /* Pre: Hace una peticion a la api de tipo put con los datos introducidos en los textbox
    * Pro: Lo hace
    * 
    * ORG 25/10/2023
    */
    protected async void Put(JObject factura)
    {

        var jsonContent = Newtonsoft.Json.JsonConvert.SerializeObject(factura);

        using (HttpClient client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("accept", "text/plain");
            // client.DefaultRequestHeaders.Add("Content-Type", "application/json");


            var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PutAsync($"https://facturass.azurewebsites.net/factura", content);

            if (response.IsSuccessStatusCode)
            {
                GridView1.DataSource = await Listar();
                GridView1.DataBind();
            }
            else
            {
                string script = $"alert('Algun valor proporcionado no es valido; por favor revisa los datos y vuelve a intentarlo');";
                ClientScript.RegisterStartupScript(this.GetType(), "AlertScript", script, true);
            }
        }
    }



    protected void Modificar(object sender, EventArgs e)
    {
        string script = $"alert('${"d"}');";
        ClientScript.RegisterStartupScript(this.GetType(), "AlertScript", script, true);

        nuevaFacturaForm.Style["display"] = "block";
    }

    protected void Agregar(object sender, EventArgs e)
    {
        txtTitulo.InnerText = "Nueva Factura";
        nuevaFacturaForm.Style["display"] = "block";
        B_Modificar.Style["display"] = "none";
    }



    protected void B_Añadir_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(txtFecha.Text) ||
            string.IsNullOrEmpty(ddlMoneda.SelectedValue) ||
            string.IsNullOrEmpty(txtCIF.Text) ||
            string.IsNullOrEmpty(txtNombre.Text) ||
            string.IsNullOrEmpty(txtImporte.Text) ||
            string.IsNullOrEmpty(txtImporteIVA.Text) ||
            string.IsNullOrEmpty(txtFechaCobro.Text))
        {
            {
                string script = $"alert('${txtFecha.Text }{ ddlMoneda.SelectedValue}{ (txtCIF.Text) }{  txtNombre.Text}{ decimal.Parse(txtImporte.Text) }{ txtImporteIVA.Text}{ txtFechaCobro.Text} ');";
                ClientScript.RegisterStartupScript(this.GetType(), "AlertScript", script, true);
            }
        }
        else { 
            var factura = new JObject
            {
                { "fecha", DateTime.Parse(txtFecha.Text) },
                { "cif", txtCIF.Text },
                { "nombre", txtNombre.Text },
                { "importe", decimal.Parse(txtImporte.Text) },
                { "importe_iva", decimal.Parse(txtImporteIVA.Text) },
                { "moneda", ddlMoneda.SelectedItem.Text },
                { "fecha_cobro", DateTime.Parse(txtFecha.Text) },
                { "estado", chkEstado.Checked }
            };
            Post(factura); 
            nuevaFacturaForm.Style["display"] = "none";
        }
    }
    protected void B_Modificar_Click(object sender, EventArgs e)
    {
        string script = $"alert('Error al eliminar la factura con ID {txtFecha.Text}');";
        ClientScript.RegisterStartupScript(this.GetType(), "AlertScript", script, true);
        GridView1.EditIndex = -1;

    }
    protected async void GridView1_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        await Delete(GridView1.Rows[e.RowIndex].Cells[0].Text);
    }

    protected void B_Cancelar_Click(object sender, EventArgs e)
    {
        nuevaFacturaForm.Style["display"] = "none";
        txtFecha.Text = string.Empty;
        txtCIF.Text = string.Empty;
        txtNombre.Text = string.Empty;
        txtImporte.Text = string.Empty;
        txtImporteIVA.Text = string.Empty;
        ddlMoneda.SelectedIndex = 0; 
        txtFechaCobro.Text = string.Empty;
        chkEstado.Checked = false;
    }

    protected  void Edit(int e)
    {
        txtTitulo.InnerText = "Modificar Factura";
        nuevaFacturaForm.Style["display"] = "block";
        B_Añadir.Style["display"] = "none";
        B_Modificar.Style["display"] = "block";

        //string script = $"alert('Error al eliminar la factura con ID {e.Row.RowIndex}');";
        //ClientScript.RegisterStartupScript(this.GetType(), "AlertScript", script, true);

        txtFecha.Text = DateTime.Parse(GridView1.Rows[e].Cells[1].Text).ToString("yyyy-MM-ddTHH:mm");
        txtCIF.Text = GridView1.Rows[e].Cells[2].Text;
        txtNombre.Text = GridView1.Rows[e].Cells[3].Text;
        txtImporte.Text = GridView1.Rows[e].Cells[4].Text;
        txtImporteIVA.Text = GridView1.Rows[e].Cells[5].Text;
        ddlMoneda.SelectedValue = GridView1.Rows[e].Cells[6].Text;
        txtFechaCobro.Text = DateTime.Parse(GridView1.Rows[e].Cells[7].Text).ToString("yyyy-MM-ddTHH:mm");
        chkEstado.Text = GridView1.Rows[e].Cells[8].Text;

        GridView1.EditIndex = -1;
        GridView1.SelectedIndex = -1;
        
    }

    protected void GridView1_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "Editar")
        {
            Edit(Convert.ToInt32(e.CommandArgument));
        }else if (e.CommandName == "Eliminar")
        {

        }
       
    }


}





