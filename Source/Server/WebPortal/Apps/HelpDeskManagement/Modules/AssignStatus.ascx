<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AssignStatus.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.HelpDeskManagement.Modules.AssignStatus" %>
<%@ Register TagPrefix="mc" Namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>
<script type="text/javascript">
	function GetIds()
	{
		var s = window.parent.GetSelectedIds();
		var obj = document.getElementById('<%=hfValues.ClientID %>');
		obj.value = s;
		if(s=="")
		{
			var divObj = document.getElementById('noItemsSelected');
			divObj.style.display = "";
		}
	}
</script>
<table width="100%" class="ibn-propertysheet" id="tblMain" runat="server">
	<tr>
		<td style="padding:7px;">
			<b><%=GetGlobalResourceObject("IbnFramework.Incident", "Status").ToString()%>:</b>
		</td>
		<td style="padding:5px;" align="right"><asp:DropDownList ID="ddStatus" runat="server" Width="300px" CssClass="text"></asp:DropDownList></td>
	</tr>
	<tr>
		<td colspan="2" style="padding:5px;" align="center">
			<div style="text-align:left;padding:5px;"><b><%=GetGlobalResourceObject("IbnFramework.Incident", "Comment").ToString()%>:</b></div>
			<asp:TextBox ID="txtComment" runat="server" Width="98%" Rows="6" TextMode="MultiLine" CssClass="text"></asp:TextBox>
		</td>
	</tr>
	<tr>
		<td colspan="2" align="center" valign="middle" style="padding:10px;">
			<mc:ImButton ID="btnSave" runat="server"></mc:ImButton>&nbsp;
			<mc:ImButton ID="btnCancel" runat="server"></mc:ImButton>
		</td>
	</tr>
</table>
<div id="divErrors" runat="server" style="text-align:center;">
	<table style="padding:10px;" class="ibn-propertysheet">
		<tr>
			<td><img alt="" border="0" src='<%=this.Page.ResolveClientUrl("~/layouts/images/check.gif") %>' /></td>
			<td valign="top" style="padding-left:10px;">
				<asp:Label ID="lblResult" CssClass="text" runat="server"></asp:Label>
			</td>
		</tr>
	</table>
	<br />
	<mc:ImButton ID="btnClose" runat="server"></mc:ImButton>
</div>
<div style="height:190px;width:400px;background-color:White; position:absolute;color:Red;left:0px;top:0px;padding-top:30px;text-align:center;display:none;z-index:1000;" id="noItemsSelected">
	<%=GetGlobalResourceObject("IbnFramework.Incident", "NoItemsSelected").ToString()%>
</div>
<asp:HiddenField ID="hfValues" runat="server" />