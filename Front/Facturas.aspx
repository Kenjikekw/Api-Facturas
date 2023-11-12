<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Facturas.aspx.cs" Inherits="Facturas" Async="true" %>


<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>Facturas</title>
<style>
    body {
    font-family: 'Arial', sans-serif;
    background-color: #f0f0f0;
    margin: 0;
    padding: 0;
}

form {
    max-width: 800px;
    margin: 20px auto;
    padding: 20px;
    background-color: #fff;
    border: 1px solid #ccc;
    border-radius: 5px;
    box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
}

h2 {
    text-align: center;
    color: #333;
    margin-bottom: 20px;
}

label {
    display: block;
    margin-bottom: 5px;
    color: #555;
}

table {
    width: 100%;
    border-collapse: collapse;
    margin-top: 20px;
}

table, th, td {
    border: 1px solid #ccc;
}

th, td {
    padding: 10px;
    text-align: left;
}

th {
    background-color: #4CAF50;
    color: #fff;
}

tr:nth-child(even) {
    background-color: #f9f9f9;
}

input[type="text"],
input[type="date"],
input[type="number"],
select,
button {
    width: calc(100% - 20px);
    padding: 10px;
    font-size: 16px;
    margin-bottom: 10px;
    border: 1px solid #ccc;
    border-radius: 3px;
}

button {
    background-color: #2196F3;
    color: #fff;
    border: none;
    border-radius: 3px;
    cursor: pointer;
}

    button:hover {
        background-color: #0b7dda;
    }

#overlay {
    display: none;
    position: fixed;
    top: 0;
    left: 0;
    width: 100%;
    height: 100%;
    background-color: rgba(0, 0, 0, 0.7);
    align-items: center;
    justify-content: center;
    z-index: 1;
}

#popup {
    background-color: #fff;
    padding: 20px;
    border-radius: 8px;
    text-align: center;
    box-shadow: 0 0 10px rgba(9, 9, 9, 0.524);
    position: relative;
}

#confirmationPanel {
    display: none;
    position: fixed;
    top: 50%;
    left: 50%;
    transform: translate(-50%, -50%);
    background: rgba(255, 255, 255, 0.8); 
    padding: 20px;
    z-index: 1001;
}

    .confirmation-panel button {
        margin: 0 10px;
        padding: 10px;
        cursor: pointer;
    }

    .confirmation-panel #btnConfirmarEliminar {
        background-color: #2196F3;
        color: #fff;
    }

    .confirmation-panel #btnCancelarEliminar {
        background-color: #ccc;
        color: #333;
    }

    .titulo{

    }

    .filtros {
        display: flex;
        justify-content: space-around;
    }

    .filtros div{

    }

    #nuevaFacturaForm{
        display: none;    }

    #B_Agregar{
        display: flex;
    margin-left: auto;
    }
</style>


</head>
<body>
    <form id="form1" runat="server">
        <div>
            <h2 class="titulo">Facturas</h2>
            

            <div class="filtros">
                <div>
                    <h4>Desde</h4>
                    <asp:TextBox ID="f_desde" runat="server" placeholder="Fecha" TextMode="Date"></asp:TextBox>
                </div>
                <div>
                    <h4>Hasta</h4>
                    <asp:TextBox ID="f_hasta" runat="server" placeholder="Fecha" TextMode="Date"></asp:TextBox>
                </div>
                <div>
                    <h4>Tipo Moneda</h4>
                    <asp:DropDownList ID="f_moneda" runat="server"></asp:DropDownList>
                </div>
                <div>
                    <h4>Estado</h4>
                    <asp:DropDownList ID="f_estado" runat="server"></asp:DropDownList>
                </div>
                <div>
                    <h4>Importe maximo</h4>
                    <asp:TextBox ID="f_importeMax" runat="server" TextMode="Number"></asp:TextBox>
                </div>
                <div>
                    <h4>Importe minimo</h4>
                    <asp:TextBox ID="f_importeMin" runat="server" TextMode="Number"></asp:TextBox>
                </div>
            </div>

            <asp:Button ID="B_Agregar" runat="server" Text="Agregar" />


            <asp:GridView ID="GridView1" runat="server" AlternatingRowStyle-BackColor="WhiteSmoke" AutoGenerateColumns="false" DataKeyNames="id">
                <Columns>
                    <asp:BoundField DataField="id" HeaderText="ID" SortExpression="id" />
                    <asp:BoundField DataField="fecha" HeaderText="Fecha" SortExpression="fecha" />
                    <asp:BoundField DataField="cif" HeaderText="CIF" SortExpression="cif" />
                    <asp:BoundField DataField="nombre" HeaderText="Nombre" SortExpression="nombre" />
                    <asp:BoundField DataField="importe" HeaderText="Importe" SortExpression="importe" />
                    <asp:BoundField DataField="importe_iva" HeaderText="Importe IVA" SortExpression="importe_iva" />
                    <asp:BoundField DataField="moneda" HeaderText="Moneda" SortExpression="moneda" />
                    <asp:BoundField DataField="fecha_cobro" HeaderText="Fecha Cobro" SortExpression="fecha_cobro" />
                    <asp:BoundField DataField="estado" HeaderText="Estado" SortExpression="estado" />

                    <asp:ButtonField runat="server" Text="Editar" CommandName="Editar" />
                    <asp:ButtonField runat="server" Text="Eliminar" CommandName="Eliminar"/>



                </Columns>
            </asp:GridView>

            <div id="nuevaFacturaForm" runat="server">
                <h2></h2>

                <table>
                    <tr>
                        <td>
                            <label for="txtFecha">Fecha:</label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtFecha" runat="server" placeholder="Fecha" TextMode="Date"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <label for="txtCIF">CIF:</label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtCIF" runat="server" placeholder="CIF"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <label for="txtNombre">Nombre:</label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNombre" runat="server" placeholder="Nombre"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <label for="txtImporte">Importe:</label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtImporte" runat="server" placeholder="Importe" TextMode="Number"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <label for="txtImporteIVA">Importe IVA:</label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtImporteIVA" runat="server" placeholder="Importe IVA" TextMode="Number"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <label for="ddlMoneda">Moneda:</label>
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlMoneda" runat="server">
                          
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <label for="txtFechaCobro">Fecha de Cobro:</label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtFechaCobro" runat="server" placeholder="Fecha de Cobro" TextMode="Date"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <label for="chkEstado">Estado:</label>
                        </td>
                        <td>
                            <asp:CheckBox ID="chkEstado" runat="server" />
                        </td>
                    </tr>
                </table>


            </div>

            <div id="confirmationPanel" runat="server" class="confirmation-panel">
            <p>¿Estás seguro de que deseas eliminar este elemento?</p>
            <asp:Button ID="btnConfirmarEliminar" runat="server" Text="Aceptar" OnClick="btnConfirmarEliminar_Click" />
            <asp:Button ID="btnCancelarEliminar" runat="server" Text="Cancelar" OnClientClick="" />
            </div>
        </div>
    </form>
</body>
</html>
