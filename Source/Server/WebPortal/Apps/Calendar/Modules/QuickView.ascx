<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="QuickView.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.Calendar.Modules.QuickView" %>
<div style="min-width: 23em;padding:10px;" class="ibn-propertysheet">
	<span style="font-size: 120%; color: rgb(163, 41, 41);"><b>
		<div style="width: 23em;" id="mtb">
			<asp:Label ID="lblTitle" runat="server"></asp:Label></div>
	</b>
		<br />
	</span><font size="-1"><asp:Label ID="lblTime" runat="server"></asp:Label><br />
		<br style="line-height: 6px;" />
	</font>
	<div>
		<font size="-1"><asp:LinkButton ID="lbDelete" runat="server"></asp:LinkButton>
			<br style="line-height: 6px;" />
			<div id="spanDelete2" runat="server">
			<asp:LinkButton ID="lbDeleteSeries" runat="server"></asp:LinkButton>
			</div></font></div>
	<div style="border-top: 1px solid rgb(163, 41, 41); margin-top: 5px;
		height: 3px; width: 100%;" />
	<font size="-1">
		<nobr><asp:LinkButton ID="lbGoToEdit" runat="server"></asp:LinkButton></nobr>
		<br style="line-height: 6px;" />
	</font>
	<div id="divSeries" runat="server">
		<font size="-1"><asp:LinkButton ID="lbEditSeries" runat="server"></asp:LinkButton>
		<br style="line-height: 2em;" /></font></div>
</div>