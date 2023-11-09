<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" Async="true" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <!DOCTYPE html>

    <html xmlns="http://www.w3.org/1999/xhtml">
<head>
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
<title></title>
<style>
    /* Estilo para el cuerpo de la página */
    body {
        font-family: Arial, sans-serif;
        background-color: #f0f0f0;
    }

    /* Estilo para el formulario */
    form {
        margin: 20px;
        padding: 20px;
        background-color: #fff;
        border: 1px solid #ccc;
        border-radius: 5px;
    }

    /* Estilo para el DropDownList */
    #DropDownList1 {
        padding: 5px;
        font-size: 16px;
        border: 1px solid #ccc;
        border-radius: 3px;
    }

    /* Estilo para el GridView */
    #GridView1 {
        width: 100%;
        margin-top: 20px;
        border-collapse: collapse;
    }

    /* Estilo para las celdas del GridView */
    #GridView1 td, #GridView1 th {
        padding: 8px;
        text-align: left;
        border: 1px solid #ccc;
    }

    /* Estilo para los TextBox */
    input[type="text"] {
        width: 100%;
        padding: 5px;
        font-size: 14px;
        border: 1px solid #ccc;
        border-radius: 3px;
    }

    /* Estilo para los botones */
    .button-container {
        display: flex;
        justify-content: space-evenly;
        margin-top: 20px;
    }

</style>
</head>
<body>
<form id="form1" runat="server">
    <div>
        <h2>Multiapi</h2>
        <asp:DropDownList ID="DropDownList1" runat="server" AutoPostBack="true" OnSelectedIndexChanged="DropDownList1_SelectedIndexChanged">
        </asp:DropDownList>
        <asp:GridView ID="GridView1" runat="server" AlternatingRowStyle-BackColor="WhiteSmoke">
        </asp:GridView>
        <div id="divs" runat="server">
            
        </div>
        <div class="button-container" id="botones" runat="server" visible="false" >
            <asp:Button runat="server" Text="Get" OnClick="Get" Enabled="false" ID="B_Get"/>
            <asp:Button runat="server" Text="Post" OnClick="Post" Enabled="false" ID="B_Post"/>
            <asp:Button runat="server" Text="Put" OnClick="Put" Enabled="false" ID="B_Put"/>
            <asp:Button runat="server" Text="Delete" OnClick="Delete" Enabled="false" ID="B_Delete"/>
        </div>
    </div>
</form>
</body>
</html>

</asp:Content>
