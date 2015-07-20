<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="IssuePrintPreview.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.HelpDeskManagement.Modules.IssuePrintPreview" %>
<%@ Register TagPrefix="ibn" TagName="report" Src="~/Modules/ReportHeader.ascx" %>
<style type="text/css">
	.pad5 tr td {
		padding:5px;
	}
</style>
<ibn:report ID="reportHeader" runat="server" BtnPrintVisible="true" ForPrintOnly="false" />
<table width="100%" id="mainTable" runat="server" class="ibn-propertysheet pad5">
</table>
<br />
<asp:datagrid id="dgForum" runat="server" allowpaging="False" allowsorting="False" cellpadding="0" 
	gridlines="None" CellSpacing="0" borderwidth="0px" autogeneratecolumns="False" width="100%" 
	enableviewstate="true" ShowHeader="False">
	<columns>
		<asp:TemplateColumn>
			<ItemTemplate>
				<table cellpadding="4" cellspacing="0" border="0" width="100%" class="ibn-propertysheet" style='table-layout:fixed;'>
					<tr class="ForumNode">
						<td style="padding:0px; width:24px;" class="ibn-navline-top" align="center"><%# DataBinder.Eval(Container.DataItem, "Index")%></td>
						<td style="width:200px;padding:3px;" class="ibn-navline-top ibn-value" nowrap="nowrap"><%# DataBinder.Eval(Container.DataItem, "Sender")%></td>
						<td style="width:170px;padding:3px;" class="ibn-navline-top ibn-value"><%# DataBinder.Eval(Container.DataItem, "Created")%></td>
						<td class="ibn-navline-top ibn-value">&nbsp;</td>
					</tr>
					<tr>
						<td class='ibn-navline-top'>&nbsp;</td>
						<td class='ibn-navline-top' colspan='3' style='padding:5px;'>
							 <%#DataBinder.Eval(Container.DataItem, "Message").ToString()%>
						</td>
					</tr>
				</table>
			</ItemTemplate>
		</asp:TemplateColumn>
	</columns>
</asp:datagrid>
<script type="text/javascript" defer="defer">
		window.print();
</script>
<asp:PlaceHolder ID="phPrintScripts" Runat="server">
	<script type="text/javascript">
		function BeforePrint() {
			var coll = document.all;
			if (coll != null) {
				for (var i = 0; i < coll.length; i++) {
					if (coll[i].Printable == "0")
						coll[i].style.display = "none";
					if (coll[i].Printable == "1")
						coll[i].style.display = "block";
				}
			}
		}

		function AfterPrint() {
			var coll = document.all;
			if (coll != null) {
				for (var i = 0; i < coll.length; i++) {
					if (coll[i].Printable == "0")
						coll[i].style.display = "block";
					if (coll[i].Printable == "1")
						coll[i].style.display = "none";
				}
			}
		}

		if (browseris.ie5up) {
			window.onbeforeprint = BeforePrint;
			window.onafterprint = AfterPrint;
		}
	</script>
</asp:PlaceHolder>