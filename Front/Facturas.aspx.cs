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
        CreacionComponentes(await Listar());
    }

    /* Pre: Por cada columna de la tabla te crea un textbox y un h4
	 * Pro: Lo hace
	 * 
	 * ORG 25/10/2023
	 */
    protected void CreacionComponentes(DataTable datos)
    {
        if (datos.Rows.Count > 0)
        {
            var divContainer = new HtmlGenericControl("div");
            divContainer.ID = "divs2";
            int id = 1;
            foreach (DataColumn column in datos.Columns)
            {
                var h4 = new HtmlGenericControl("h4");
                h4.InnerText = column.ColumnName;

                var textBox = new TextBox();

                textBox.ID = "Textbox" + id;
                textBox.TextChanged += Vacio;
                textBox.AutoPostBack = true;
                id++;

                divContainer.Controls.Add(h4);
                divContainer.Controls.Add(textBox);
            }
            divs.Controls.Add(divContainer);
            botones.Visible = true;
        }
        else
        {
            botones.Visible = false;
        }
    }

    /* Pre: Hace una petición a la api y te transforma los datos en una tabla
	 * Pro: Lo hace
	 * 
	 * ORG 25/10/2023
	 */
    public async Task<DataTable> Listar()
    {

        using (HttpClient client = new HttpClient())
        {
            HttpResponseMessage response = await client.GetAsync($"https://facturass.azurewebsites.net/factura");

            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();

                JArray jsonArray = JArray.Parse(data);

                DataTable dataTable = new DataTable();

                if (jsonArray.Count > 0)
                {
                    JObject firstObject = jsonArray.First() as JObject;
                    for (int i = 0; i < firstObject.Properties().Count(); i++)
                    {
                        dataTable.Columns.Add(firstObject.Properties().ElementAt(i).Name.ToUpper(), typeof(string));
                    }

                    foreach (JObject jsonObj in jsonArray)
                    {
                        DataRow fila = dataTable.NewRow();
                        foreach (JProperty prop in jsonObj.Properties())
                        {
                            fila[prop.Name] = prop.Value.ToString();
                        }
                        dataTable.Rows.Add(fila);
                    }
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
        TextBox TextBox1 = (TextBox)divs.FindControl("TextBox1");
        if (!string.IsNullOrWhiteSpace(TextBox1.Text) && int.TryParse(TextBox1.Text, out int numericValue))
        {
            B_Get.Enabled = true;
            B_Delete.Enabled = true;
        }
        else
        {
            B_Get.Enabled = false;
            B_Delete.Enabled = false;
        }
        bool enableButton = true;
        int id = 1;
        foreach (DataColumn a in datatable.Columns)
        {
            TextBox textBox = (TextBox)divs.FindControl("TextBox" + id);
            id++;
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                enableButton = false;
                break;
            }
        }
        B_Post.Enabled = enableButton;
        B_Put.Enabled = enableButton;
    }*/
    /* Pre: Aplica los filtros al selecioanr dato de lso gridview
    * Pro: Lo hace
    * 
    * ORG 25/10/2023
    */
    protected void AplicarFiltros(object sender, EventArgs e)
    {
        // Lee el archivo XML
        DataSet ds = new DataSet();
        ds.ReadXml(Server.MapPath("~/App_Data/Facturas_inicial.xml"));

        // Crear un DataView desde el DataSet
        DataView dataView = new DataView(ds.Tables[0]);

        // Obtener los valores de los DropDown
        string filtroMoneda = FiltroMoneda.SelectedValue;
        string filtroMetodoEnvio = FiltroEnvio.SelectedValue;

        // Aplicar filtros
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

        // Guardar el DataView en la sesión
        Session["Filtro"] = dataView;

        // Enlazar el DataView al GridView
        TablaFacturas.DataSource = dataView;
        TablaFacturas.DataBind();
    }











    /* Pre: Hace una peticion a la api de tipo post con los datos introducidos en los textbox
	 * Pro: Lo hace
	 * 
	 * ORG 25/10/2023
	 */
    protected async void Post(object sender, EventArgs e)
    {
        JObject jsonData = new JObject();

        JProperty[] properties = datatable.Columns.Cast<DataColumn>()
            .Skip(1)
            .Select((column, index) =>
            {
                var textBox = FindControl("TextBox" + (index + 2)) as TextBox;
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
    */
    protected async void Get(object sender, EventArgs e)
    {
        TextBox TextBox1 = (TextBox)divs.FindControl("TextBox1");
        using (HttpClient client = new HttpClient())
        {
            HttpResponseMessage response = await client.GetAsync($"https://facturass.azurewebsites.net/factura");

            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                JObject jsonObject = JObject.Parse(data);

                DataTable dataTable = new DataTable();

                if (jsonObject != null)
                {
                    foreach (JProperty prop in jsonObject.Properties())
                    {
                        dataTable.Columns.Add(prop.Name.ToUpper());
                    }

                    DataRow fila = dataTable.NewRow();

                    foreach (JProperty prop in jsonObject.Properties())
                    {
                        fila[prop.Name] = prop.Value.ToString();
                    }
                    dataTable.Rows.Add(fila);

                    GridView1.DataSource = dataTable;
                    GridView1.DataBind();
                }
            }
            else
            {
                string script = $"alert('No existe el id {TextBox1.Text} en la base de datos');";
                ClientScript.RegisterStartupScript(this.GetType(), "AlertScript", script, true);
            }
        }
    }
    /* Pre: Hace una peticion a la api de tipo delete con el dato introducido en el textbox
    * Pro: Lo hace
    * 
    * ORG 25/10/2023
    */
    protected async void Delete(object sender, EventArgs e)
    {
        TextBox TextBox1 = (TextBox)divs.FindControl("TextBox1");

        using (HttpClient client = new HttpClient())
        {

            HttpResponseMessage response = await client.DeleteAsync($"https://facturass.azurewebsites.net/factura");

            if (response.IsSuccessStatusCode)
            {
                GridView1.DataSource = await Listar();
                GridView1.DataBind();
            }
            else
            {
                string script = $"alert('No existe el id {TextBox1.Text} en la base de datos ');";
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
}
