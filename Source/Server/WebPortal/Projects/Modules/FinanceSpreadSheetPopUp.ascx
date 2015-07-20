<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Projects.Modules.FinanceSpreadSheetPopUp" Codebehind="FinanceSpreadSheetPopUp.ascx.cs" %>
<%@ Register TagPrefix="btn" namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>
<table cellpadding="5" cellspacing="0" width="100%">
	<tr class="text">
			<th width="30%" align="right">
				<div runat="server" id="rowUsual">
					<%= LocRM.GetString("title") %>:
				</div>
			</th>
			<td width="70%" align="left">
				<div runat="server" id="rowUsual2">
					<asp:TextBox Runat="server" ID="tbName"></asp:TextBox>
				</div>
			</td>		
	</tr>
	<tr class="text" >
		<td colspan="2">
			<div runat="server" id="rowError" class="text">
				<%= LocRM2.GetString("CantAddRow")%>
			</div>
		</td>
	</tr>
	<tr>
		<td colspan=2 align="center">
			<btn:imbutton class=text runat="server" id="btnOk" style="width:110px;"></btn:imbutton>
			<btn:imbutton class=text runat="server" id="btnCancel" style="width:110px;"></btn:imbutton>
		</td>
	</tr>
</table>