<%@ Page Language="c#" Inherits="Mediachase.UI.Web.FileStorage.PasteHandler" CodeBehind="PasteHandler.aspx.cs" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	
	<title>IBN</title>

	<script type="text/javascript">
		//<![CDATA[
		function ResizeForm()
		{
			var obj = document.getElementById('mainDiv');
			if(obj)
			{
				var iheight = document.body.clientHeight;
				var iwidth = document.body.clientWidth;
				obj.style.height = iheight;
				obj.style.width = iwidth;
			}
		}
		window.onresize = ResizeForm;
		//]]>
	</script>

</head>
<body class="UserBackground" id="pT_body">

	<script type="text/javascript">
		//<![CDATA[
			function CopyFile(FileId)
			{
				document.forms[0].<%=hdnFileId.ClientID %>.value = FileId;
				<%=Page.ClientScript.GetPostBackEventReference(lbCopyFile,"") %>
			}
		//]]>
	</script>

	<form id="pasteForm" method="post" runat="server" enctype="multipart/form-data">
	<div id="mainDiv" style="height: 195px; width: 195px; overflow-y: auto; overflow-x: auto;">
		<asp:DataGrid runat="server" ID="dgFiles" ShowHeader="False" AutoGenerateColumns="False" AllowPaging="False" AllowSorting="False" CellPadding="3" GridLines="None" CellSpacing="0" BorderWidth="0px" Width="100%">
			<Columns>
				<asp:BoundColumn DataField="FileId" Visible="False"></asp:BoundColumn>
				<asp:TemplateColumn>
					<ItemStyle CssClass="ibn-vb2"></ItemStyle>
					<ItemTemplate>
						<a title='<%=LocRM.GetString("tPaste")%>' href='<%# "javascript:CopyFile(" + DataBinder.Eval(Container.DataItem, "FileId").ToString() + ")" %>'>
							<%# DataBinder.Eval(Container.DataItem, "Name")%>
						</a>
					</ItemTemplate>
				</asp:TemplateColumn>
			</Columns>
		</asp:DataGrid>
		<asp:Label ID="lblAlert" CssClass="ibn-alerttext" runat="server"></asp:Label>
	</div>
	<input id="hdnFileId" type="hidden" runat="server" />
	<asp:LinkButton ID="lbCopyFile" runat="server" Visible="False"></asp:LinkButton>
	</form>
</body>
</html>
