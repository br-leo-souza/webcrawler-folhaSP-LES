<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CrawlerFolhaSP.aspx.cs" Inherits="roboCrawlerFolhaSP.CrawlerFolhaSP" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head >
   
</head>

<body>
    <form runat="server">
		<div>
			<h2>Webcrawler - Folha de São Paulo</h2>
		</div>

        <div style="width: 300px;text-align: center;">
            <asp:Button ID="btnGerarArquivos" runat="server" Text="Restaurantes" OnClick="btnGerarArquivos_Click" />
        </div>
    </form>
</body>
</html>
