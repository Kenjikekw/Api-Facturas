<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Facturas.aspx.cs" Inherits="Facturas" Async="true" EnableEventValidation="true" %>


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

        .titulo {
        }

        .filtros {
            display: flex;
            justify-content: space-around;
        }

            .filtros div {
            }

        #nuevaFacturaForm {
            display: none;
        }

        #B_Agregar {
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
                    <asp:TextBox ID="f_desde" runat="server" placeholder="Fecha" TextMode="Date" OnTextChanged="Cargar" AutoPostBack="true"></asp:TextBox>
                </div>
                <div>
                    <h4>Hasta</h4>
                    <asp:TextBox ID="f_hasta" runat="server" placeholder="Fecha" TextMode="Date" OnTextChanged="Cargar" AutoPostBack="true"></asp:TextBox>
                </div>
                <div>
                    <h4>Tipo Moneda</h4>
                    <asp:DropDownList ID="f_moneda" runat="server" OnTextChanged="Cargar" AutoPostBack="true"></asp:DropDownList>
                </div>
                <div>
                    <h4>Estado</h4>
                    <asp:DropDownList ID="f_estado" runat="server" OnTextChanged="Cargar" AutoPostBack="true"></asp:DropDownList>
                </div>
                <div>
                    <h4>Importe maximo</h4>
                    <asp:TextBox ID="f_importeMax" runat="server" TextMode="Number" OnTextChanged="Cargar" AutoPostBack="true"></asp:TextBox>
                </div>
                <div>
                    <h4>Importe minimo</h4>
                    <asp:TextBox ID="f_importeMin" runat="server" TextMode="Number" OnTextChanged="Cargar" AutoPostBack="true"></asp:TextBox>
                </div>
            </div>

            <asp:Button ID="B_Agregar" runat="server" Text="Agregar" OnClick="Agregar" />


            <asp:GridView ID="GridView1" runat="server" AlternatingRowStyle-BackColor="WhiteSmoke" AutoGenerateColumns="false" DataKeyNames="id" AutoGenerateEditButton="False" OnRowCommand="GridView1_RowCommand" ClientIDMode="Inherit">
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
                    <asp:ButtonField CommandName="Editar" ButtonType="Image" ImageUrl="data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAEAAAABACAYAAACqaXHeAAAACXBIWXMAAAsTAAALEwEAmpwYAAAEK0lEQVR4nO2aW4hVZRiGnxlnNBU8QQcC7UZR0bSMsmhAqZts8gDdimB0E6FdpLC9MBrqIjJNrcigotQwgzxlmh28sEwpvPJKS71JPFaTnRTEHR+8Cz4Wa81ae8/Y/tea9cJmmPX/a/M/739Y3/etDZUqVSqo7gOeA2rAo0Abg0RDgLeBeuzzDTCaQQC/WcD/AhuAl4ELuna0zCa0Ax8K9C9grmsbD/ystmPAOEoI/0EKfGlNWAzsBV5wM38F6OrjngnAqTJshxUJh1zazJduJdQ0+OtAD/Cr/v8a6Mz5HYVdCTU34yt1bZYz4dMym1CLLfllrq30JtRi8F8Bw2N9BsKEg3qiBKVlbs9vB96LwQ8Fhg2ACed13wICUjvwJ3ADWJIy8J90qrf104QXdc+rBKazGti62PU7gNNqOxRra8aE9er/EoFpm9v73oSFGYdXIyZ0KXewvg8RkNZoUNeAqzETbMnPAUa4/rcqNrirARO6tM2szyYChV8IdCeY4HUbcFzttp/JYUKXwue6skjLJoPQazH4SH2ZcEDXj2slkGFC4eCzTLCk6AuthCTNioXNwcMv6KNf1nZAB+O3CnKS4oTg4NfmhM9jwkg9HeqKE/y+9yaspqDwWSbM1bXTCpbi6lG71ROCgp/fxP3dKY9IM/J218/C5+WqJVxXfyuqtFTrMuAfAL4EHsz4nqwzYbgSKJ9QrSJw+NlAr/pYfZ9+mPB+aPCvO/gnMuA/Bjpyfm+aCdtDgW9z8FdT4Cc0CZ9mwhK354OHN01UkLKlCXjTNOBSQtG05fDrM+BHqbhBP6oy091boO+AX2SmL5+1HL47Zc//rpIUAwC/z0V/7UWA71WfdxLaO1SliarAWfCfO3haDb+hAfikA69D16OorpDw11LCW3t//5v6fJIAb0nKVrX3yqy4pgDn1Gc/cAsBws+/SfBTK3iqmd8f0rLfOJiX/cb/AX6Gi/DsOV/Bt1pt1cxz05f9zBCXvekVDcpeJz1G3xHetowILw/8HpcotVwT9ba2rr8nBWm1tkf0KS08KiTWlb39k5B31x38kLLBm57X4NYKyB5PS4E3gR/UdqFJ+HuAy+qzO0R4v//tZytxGfTf2hrj+nngBZHVJeldDfLpxFY4ovZngDFlmvlIuzTQRSQrygSjQ/JiA/C7Qoc3HdZgHyZZd6oK9H3skLSnQ1z3Fg3edEIDnky2bPn/mGJAIeHR488GPZZ8ekv9V6Ys+2DKWHnU6YIge/zl0VOu5hfNfPR6emeRZt40ye1pO9zuJlsz3Xt6P/N7izTzkebFor08JnQqZ7jhkqOdDfyAMSgtTwh5IxPGKk+YLaMW661uj5t1++woKjyu8tPsZ3OR4VFykgb3B3BG+YD9WusjGWa/2XsWuJ8S6EngM+ANbYfHdTAWelYrVapEmv4DRW0+DWDWQvwAAAAASUVORK5CYII="/>
                    <asp:ButtonField CommandName="Eliminar" ButtonType="Image" ImageUrl="data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAADIAAAAyCAYAAAAeP4ixAAAACXBIWXMAAAsTAAALEwEAmpwYAAADdklEQVR4nO2ZTYhOURjHf0hhZBYagyRFCcksKB8hygLLWSCysDIslB2TaVImliL52tjNSIYVFhRTjI/QO2NmkFkoUxoy0dQ03pmrU/9bp9O9133f+zFD779O9z3n/u95nuec55zzPOeFCv5fLAVeA2+BZfyj2A58AzyV78COv3yzGWgDLgJzmAQ4AvyWAe3Abf0uAscd7gzgEPBGnDE9PwEbJkh/pgOXpcg4cAaYCkwBmtRm3t2Qq7UAg9aMnQOWAPuBIQ1GEzAtTyPmAg+l1AhwMICzSwp6VukDjgFVDncx8FiczrzW2GqgX0K/AOsiuCuBj+L+AjZFcM1MnNLMDGmmUsMjZ0QnsnQkMcSbZKVsJO4gJXgVQ4T/xrU6JoHynsoTEuKeOhrVsxeYr9LrtBHSXi53VE+jQ2KcVWcXHKFBihGiYLnc8/ptdEiMfersFjAP6HZO6oUh36XBbVV9TxqGrLACuxpH4PsI5WoTchdYs7M8DUNM6DCsILBHHRdU4rpLT0xuj8XtVrgynGYg+dwarR4Jr7GMsUfbHl3fRYzrdJXALVjynpEirqjTLhngwzbGjGpdxMK2lXa53XoftGZMqpBq8mQ6vR7wzjZmPMKFcGbG53ap3cU1vW9I05CN6vRVRLhetBQ0o00MblH1ILwQJ9XMcbbS0hFlhmEzMmatI9tVorgFx13tDWZMslPFBwmuC1GsoNH16+8cY6K4rjGrrI0hddx0/N9WzN/JXIWTcluzMKTR2hLtcyTINdzRL4frASezMGR3QERqjy4RCpbK9VR2ZmHIogBBvSUoVwrXUzFhSiYYzNG1vpIh7ktIfYACvoJB7aVw61V/kKUhfm7SHKBIr+68gnanUrjNaeYgYdhr3fGWesjF5bar3eRBmcHPTcxNYzlhRxxuv94ZWZnBzk2qM5iRavWdag4Shk4psCXgxLb93t1u43C3ZpGDhMH/K+F0BiFKi35fysOQBiefSPMcGdfzcB6GrLdO3qiF7Z7WYevG3QA8ycgcVdZCjQo94oYoLq8IzCIn9EnoUMhox3Utm+f3ZQzODW0S2hgzHInTdiLLHCQMByT0pa5wSh19l2ei3KeqH83TkJnAZwm+q/3fvimMcyD66fA24I7qA9ZBmxvWAD8Ccohyy09gLROEWl3eDSQwwHx7NeJeuIIKKiA5/gB+YLZXHntzCwAAAABJRU5ErkJggg==" />


                </Columns>
            </asp:GridView>

            <div id="nuevaFacturaForm" runat="server">
                <h2 id="txtTitulo" runat="server"></h2>

                <div>
                    <div>
                        <h4 for="txtFecha">Fecha:</h4>
                        <asp:TextBox ID="txtFecha" runat="server" placeholder="Fecha" TextMode="DateTimeLocal"></asp:TextBox>
                    </div>
                    <div>
                        <h4 for="txtCIF">CIF:</h4>
                        <asp:TextBox ID="txtCIF" runat="server" placeholder="CIF"></asp:TextBox>
                    </div>
                    <div>
                        <h4 for="txtNombre">Nombre:</h4>
                        <asp:TextBox ID="txtNombre" runat="server" placeholder="Nombre"></asp:TextBox>
                    </div>
                    <div>
                        <h4 for="txtImporte">Importe:</h4>
                        <asp:TextBox ID="txtImporte" runat="server" placeholder="Importe" TextMode="Number" step="0.01"></asp:TextBox>
                    </div>
                    <div>
                        <h4 for="txtImporteIVA">Importe IVA:</h4>
                        <asp:TextBox ID="txtImporteIVA" runat="server" placeholder="Importe IVA" TextMode="Number" step="0.01"></asp:TextBox>
                    </div>
                    <div>
                        <h4 for="ddlMoneda">Moneda:</h4>
                        <asp:DropDownList ID="ddlMoneda" runat="server">
                            <asp:ListItem Text="EUR"></asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div>
                        <h4 for="txtFechaCobro">Fecha de Cobro:</h4>
                        <asp:TextBox ID="txtFechaCobro" runat="server" placeholder="Fecha de Cobro" TextMode="DateTimeLocal"></asp:TextBox>
                    </div>
                    <div>
                        <h4 for="chkEstado">Estado:</h4>
                        <asp:CheckBox ID="chkEstado" runat="server" />
                    </div>
                </div>
                <asp:Button runat="server" ID="B_Añadir" Text="Añadir" OnClick="B_Añadir_Click" />
                <asp:Button runat="server" ID="B_Modificar" Text="Modificar" OnClick="B_Modificar_Click" />
                <asp:Button runat="server" ID="Button1" Text="Cancelar" OnClick="B_Cancelar_Click" />


            </div>


            <div id="confirmationPanel" runat="server" class="confirmation-panel">
                <p runat="server" id="p_text"></p>
                <div>
                    <asp:Button ID="btnConfirmarEliminar" runat="server" Text="Aceptar" OnClick="Eliminar" />
                    <asp:Button ID="btnCancelarEliminar" runat="server" Text="Cancelar" OnClick="Cancelar" />
                </div>

            </div>
        </div>
    </form>
</body>
</html>
