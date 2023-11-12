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
    private DataTable datatable = new DataTable();
    protected async void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            GridView1.DataSource = await Listar();
            GridView1.DataBind();
        }

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

                if (jsonArray.Count > 0)
                {
                    // Assuming the first object in the array represents the structure
                    JObject firstObject = jsonArray.First() as JObject;
                    HashSet<string> columnNames = new HashSet<string>();

                    foreach (JProperty prop in firstObject.Properties())
                    {
                        columnNames.Add(prop.Name.ToUpper());
                    }

                    foreach (string columnName in columnNames)
                    {
                        dataTable.Columns.Add(columnName, typeof(string));
                    }

                    foreach (JObject jsonObj in jsonArray)
                    {
                        DataRow fila = dataTable.NewRow();
                        foreach (JProperty prop in jsonObj.Properties())
                        {
                            fila[prop.Name.ToUpper()] = prop.Value.ToString();
                        }
                        dataTable.Rows.Add(fila);
                    }

                    // Assuming datatable is a class-level variable
                    datatable = dataTable;
                    return dataTable;
                }
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
    protected async void Post(object sender, EventArgs e)
    {
        JObject jsonData = new JObject();

        // Recorre las filas de la tabla
        foreach (GridViewRow row in GridView1.Rows)
        {
            var properties = row.Cells.Cast<DataControlFieldCell>()
                .Select(cell =>
                {
                    var textBox = cell.Controls.OfType<TextBox>().FirstOrDefault();
                    var dropDownList = cell.Controls.OfType<DropDownList>().FirstOrDefault();

                    if (textBox != null)
                    {
                        return new JProperty(textBox.ID.ToLower(), textBox.Text);
                    }
                    else if (dropDownList != null)
                    {
                        return new JProperty(dropDownList.ID.ToLower(), dropDownList.SelectedValue);
                    }
                    else
                    {
                        return null;
                    }
                })
                .Where(property => property != null)
                .ToArray();

            jsonData.Add(properties);

        }

        var jsonContent = Newtonsoft.Json.JsonConvert.SerializeObject(jsonData);

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
    protected void Eliminar()
    {
        if (GridView1.SelectedIndex >= 0)
        {
            int rowIndex = GridView1.SelectedIndex;
            string idToDelete = GridView1.DataKeys[rowIndex]["id"].ToString();

            ViewState["IDToDelete"] = idToDelete;
            confirmationPanel.Style["display"] = "block";
        }
    }

    protected void btnConfirmarEliminar_Click(object sender, EventArgs e)
    {
       
        Delete(ViewState["IDToDelete"].ToString());

        confirmationPanel.Style["display"] = "none";
    }
    protected void btnCancelarEliminar_Click(object sender, EventArgs e)
{
    ScriptManager.RegisterStartupScript(this, GetType(), "HideConfirmationPanel", "$('#confirmationPanel').hide();", true);
}


    protected async void Delete(string idToDelete)
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
    protected async void Put(object sender, EventArgs e)
    {
        JObject jsonData = new JObject();

        JProperty[] properties = datatable.Columns.Cast<DataColumn>()
            .Select((column, index) =>
            {
                var textBox = FindControl("TextBox" + (index + 1)) as TextBox;
                if (column.DataType == typeof(int))
                {
                    return new JProperty(column.ColumnName.ToLower(), int.Parse(textBox.Text));
                }
                else if (column.DataType == typeof(bool))
                {
                    return new JProperty(column.ColumnName.ToLower(), bool.Parse(textBox.Text));
                }
                else
                {
                    return new JProperty(column.ColumnName.ToLower(), textBox.Text);
                }
            })
            .ToArray();

        jsonData.Add(properties);

        var jsonContent = Newtonsoft.Json.JsonConvert.SerializeObject(jsonData);

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



    protected void Editar()
    {
        nuevaFacturaForm.Style["display"] = "block";
    }

    protected void Command(object sender, GridViewCommandEventArgs e,DataGridItem d)
    {
        if (e.CommandName.Equals("Editar"))
        {
            Editar();
        }
        else if (e.CommandName.Equals("Eliminar"))
        {
            Eliminar();
        }
    }

}
