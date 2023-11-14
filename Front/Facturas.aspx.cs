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

    private int index;
    private DataTable tabla_C;
    protected async void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
               await Cargar();

            HashSet<string> lista_M = new HashSet<string>();
            HashSet<string> lista_E= new HashSet<string>();

            foreach (DataRow row in tabla_C.Rows)
            {
                string valor = row["moneda"].ToString();
                string estado = row["estado"].ToString();
                lista_M.Add(valor);
                lista_E.Add(estado);
            }

            f_moneda.Items.Add("Todos");
            f_estado.Items.Add("Todos");

            foreach (string valor in lista_M)
            {
                f_moneda.Items.Add(valor);
                ddlMoneda.Items.Add(valor);
            }

            foreach (string valor in lista_E)
            {
                f_estado.Items.Add(valor);
            }
        }
    }

    /* Pre: Llama a la carga de datos
	 * Pro: Lo hace
	 * 
	 * ORG 13/11/2023
	 */
    protected async void Cargar(object sender, EventArgs e)
    {
        await Cargar();
    }

    /* Pre: Llama al metodo listar y rellena el gridview con los datos de la tabla
	 * Pro: Lo hace
	 * 
	 * ORG 13/11/2023
	 */
    protected async Task Cargar()
    {
        GridView1.DataSource = await Listar();
        GridView1.DataBind();
    }

    /* Pre: Hace una llamada a la api y te devuelve una tabla que luego es filtrada
	 * Pro: Lo hace
	 * 
	 * ORG 13/11/2023
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
                tabla_C = dataTable;
                DataTable dataTableFiltrada = dataTable.Clone();
                foreach (DataRow fila in dataTable.Rows)
                {
                    DateTime fecha = DateTime.Parse(fila["fecha"].ToString()); 
                    string moneda = fila["moneda"].ToString(); 
                    string estadoFila = fila["Estado"].ToString(); 
                    decimal importe = decimal.Parse(fila["Importe"].ToString()); 

                    // Aplicar filtros
                    if ((string.IsNullOrEmpty(f_desde.Text) || fecha >= DateTime.Parse(f_desde.Text))
                        && (string.IsNullOrEmpty(f_hasta.Text) || fecha <= DateTime.Parse(f_hasta.Text))
                        && (string.IsNullOrEmpty(f_moneda.SelectedValue) || f_moneda.SelectedValue == "Todos" || moneda == f_moneda.SelectedValue)
                        && (string.IsNullOrEmpty(f_estado.SelectedValue) || f_estado.SelectedValue == "Todos" || estadoFila == f_estado.SelectedValue)
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
 

    /* Pre: Hace una peticion a la api de tipo post con los datos introducidos en los textbox
	 * Pro: Lo hace
	 * 
	 * ORG 13/11/2023
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
    /* Pre: Oculta la tarjeta
    * Pro: Lo hace
    * 
    * ORG 13/11/2023*/

    protected void Cancelar(object sender, EventArgs e)
    {
        confirmationPanel.Style["display"] = "none";
    }




    /* Pre: Llama a delete y oculta la tarjeta
    * Pro: Lo hace
    * 
    * ORG 13/11/2023
    */
    protected async void Eliminar(object sender, EventArgs e)
    {
        if (index >= 0)
        {
            string idToDelete = GridView1.Rows[index].Cells[0].Text;

            await Delete(idToDelete);
            confirmationPanel.Style["display"] = "none";
        }
    }


       /* Pre:Hace una peticion a la api de tipo delete
        * Pro: Lo hace
        * 
        * ORG 13/11/2023
        */
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
    * ORG 13/11/2023
    */
    protected async Task Put(JObject factura)
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

    /* Pre: Limpia los campos de la tarjeta y muestra los botones necesarios
	 * Pro: Lo hace
	 * 
	 * ORG 13/11/2023
	 */
    protected void Agregar(object sender, EventArgs e)
    {
        txtFecha.Text = string.Empty;
        txtCIF.Text = string.Empty;
        txtNombre.Text = string.Empty;
        txtImporte.Text = string.Empty;
        txtImporteIVA.Text = string.Empty;
        ddlMoneda.SelectedIndex = 0;
        txtFechaCobro.Text = string.Empty;
        chkEstado.Checked = false;
        txtTitulo.InnerText = "Nueva Factura";
        B_Añadir.Style["display"] = "block";
        nuevaFacturaForm.Style["display"] = "block";
        B_Modificar.Style["display"] = "none";
    }
    /* Pre: Comprueba si todos los campos estan rellenos y en caso de que si, llama al metodo post
	 * Pro: Lo hace
	 * 
	 * ORG 13/11/2023
	 */
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
                string script = $"alert('Debes rellenar todos los campos antes de añadir una factura');";
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

    /* Pre: Comprueba que todos los datos estan rellenos y llama al metodo Put
	 * Pro: Lo hace
	 * 
	 * ORG 13/11/2023
	 */
    protected async void B_Modificar_Click(object sender, EventArgs e)
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
                string script = $"alert('Debes rellenar todos los campos antes de modificar la factura');";
                ClientScript.RegisterStartupScript(this.GetType(), "AlertScript", script, true);
            }
        }
        else
        {
            var factura = new JObject
            {
                {"id", Int32.Parse(GridView1.Rows[index].Cells[0].Text)},
                { "fecha", DateTime.Parse(txtFecha.Text) },
                { "cif", txtCIF.Text },
                { "nombre", txtNombre.Text },
                { "importe", decimal.Parse(txtImporte.Text) },
                { "importe_iva", decimal.Parse(txtImporteIVA.Text) },
                { "moneda", ddlMoneda.SelectedItem.Text },
                { "fecha_cobro", DateTime.Parse(txtFecha.Text) },
                { "estado", chkEstado.Checked }
            };
            await Put(factura);
            nuevaFacturaForm.Style["display"] = "none";
        }

    }

    /* Pre: Limpia los campos y oculta la tarjeta
	 * Pro: Lo hace
	 * 
	 * ORG 13/11/2023
	 */
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

    /* Pre: Asigna a los textbox los valores de la fila seleccionada
	 * Pro: Lo hace
	 * 
	 * ORG 13/11/2023
	 */
    protected void Edit(int e)
    {
        txtTitulo.InnerText = "Modificar Factura";
        nuevaFacturaForm.Style["display"] = "block";
        B_Añadir.Style["display"] = "none";
        B_Modificar.Style["display"] = "block";

        txtFecha.Text = DateTime.Parse(GridView1.Rows[e].Cells[1].Text).ToString("yyyy-MM-ddTHH:mm");
        txtCIF.Text = GridView1.Rows[e].Cells[2].Text;
        txtNombre.Text = GridView1.Rows[e].Cells[3].Text;
        txtImporte.Text = GridView1.Rows[e].Cells[4].Text;
        txtImporteIVA.Text = GridView1.Rows[e].Cells[5].Text;
        ddlMoneda.SelectedValue = GridView1.Rows[e].Cells[6].Text;
        txtFechaCobro.Text = DateTime.Parse(GridView1.Rows[e].Cells[7].Text).ToString("yyyy-MM-ddTHH:mm");
        chkEstado.Checked = false;

        GridView1.EditIndex = -1;
        GridView1.SelectedIndex = -1;
        
    }

    /* Pre: En funcion de si eliges un boton u otro te hace un metodo u otro
	 * Pro: Lo hace
	 * 
	 * ORG 13/11/2023
	 */
    protected void GridView1_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (GridView1.Rows[Convert.ToInt32(e.CommandArgument)].Cells[8].Text.Equals("Pagada"))
        {
            string script = $"alert('No puedes modificar una factura ya pagada');";
            ClientScript.RegisterStartupScript(this.GetType(), "AlertScript", script, true);
        }
        else
        {
            index = Convert.ToInt32(e.CommandArgument);

            if (e.CommandName == "Editar")
            {
                Edit(Convert.ToInt32(e.CommandArgument));
            }
            else if (e.CommandName == "Eliminar")
            {
                p_text.InnerText = "¿Estás seguro de que deseas eliminar la factura con id " + GridView1.Rows[index].Cells[0].Text + "?";
                confirmationPanel.Style["display"] = "block";
            }
        }
    }


}





