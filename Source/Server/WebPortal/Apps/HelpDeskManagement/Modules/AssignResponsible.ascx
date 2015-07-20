<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AssignResponsible.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.HelpDeskManagement.Modules.AssignResponsible" %>
<%@ Register TagPrefix="mc" Namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>

<script type="text/javascript">
	//<![CDATA[
	var isNewHandlerSet_issResp = false;
	var savedHandler_issResp = null;

	function SelectThis(obj, _value) {
		var obj1 = document.getElementById('<%=lblClient.ClientID%>');
		var _hid = document.getElementById('<%=hidResp.ClientID%>');
		if (obj1 && _hid && _value != 0) {
			obj1.innerHTML = obj.innerHTML;
			_hid.value = _value;
		}
		closeMenu();
	}
	function TdOver(obj) {
		if (obj) {
			if (obj.className == "cellclass")
				obj.className = "hovercellclass";
		}
	}
	function TdOut(obj) {
		if (obj) {
			if (obj.className == "hovercellclass")
				obj.className = "cellclass";
		}
	}
	function getMenu(s) {
		return document.getElementById(s)
	}
	function closeMenu() {
		CancelBubble_issResp();

		getMenu('divDropDown').style.display = "none";
		if (isNewHandlerSet_issResp) {
			document.onclick = savedHandler_issResp;
			savedHandler_issResp = null;
			isNewHandlerSet_issResp = false;
		}
	}
	function openDD(e, curObjID) {
		CancelBubble_issResp(e);

		var curObj = document.getElementById(curObjID);
		menu = getMenu('divDropDown');
		off = GetTotalOffset(curObj);
		menu.style.left = (off.Left - 233).toString() + "px";
		menu.style.top = (off.Top + 22).toString() + "px";
		menu.style.display = "";

		if (!isNewHandlerSet_issResp) {
			savedHandler_issResp = document.onclick;
			document.onclick = closeMenu;
			isNewHandlerSet_issResp = true;
		}
	}
	function ShowHideList(e, curObjID) {
		obj = getMenu('divDropDown');
		if (obj.style.display == "none") {
			openDD(e, curObjID);
		}
		else
			closeMenu();
	}
	function CancelBubble_issResp(e) {
		var e1 = (e) ? e : ((window.event) ? window.event : null);
		if (e1) {
			e1.cancelBubble = true;
			if (e1.stopPropagation)
				e1.stopPropagation();
		}
	}
	function GetTotalOffset(eSrc) {
		this.Top = 0;
		this.Left = 0;
		while (eSrc) {
			this.Top += eSrc.offsetTop;
			this.Left += eSrc.offsetLeft;
			eSrc = eSrc.offsetParent;
		}
		return this;
	}
	function RefreshFromMore(params) {
		var obj = Sys.Serialization.JavaScriptSerializer.deserialize(params);
		if (obj && obj.CommandArguments && obj.CommandArguments.Key)
			__doPostBack('<%=btnRefreshMore.UniqueID %>', obj.CommandArguments.Key);
		else if (obj && obj.CommandArguments && obj.CommandArguments.SelectedValue && obj.CommandArguments.SelectedHtml) {
			var hidResp = document.getElementById('<%=hidResp.ClientID %>');
			if (hidResp)
				hidResp.value = obj.CommandArguments.SelectedValue;
			var lblClient = document.getElementById('<%=lblClient.ClientID %>');
			if (lblClient)
				lblClient.innerHTML = obj.CommandArguments.SelectedHtml;
		}
	}
	function RefreshFromGroup(params) {
		var obj = Sys.Serialization.JavaScriptSerializer.deserialize(params);
		if (obj && obj.CommandArguments && obj.CommandArguments.Key)
			__doPostBack('<%=btnRefresh.UniqueID %>', obj.CommandArguments.Key);
	}
	function GetIds() {
		var s = window.parent.GetSelectedIds();
		var obj = document.getElementById('<%=hfValues.ClientID %>');
		obj.value = s;
		if (s == "") {
			var divObj = document.getElementById('noItemsSelected');
			divObj.style.display = "";
		}
	}
	//]]>
</script>

<table width="100%" class="ibn-propertysheet">
	<colgroup>
		<col width="60px" />
		<col />
	</colgroup>
	<tr>
		<td style="padding: 7px; width: 60px;">
			<b>
				<%=GetGlobalResourceObject("IbnFramework.Incident", "IssResponsible").ToString()%>:</b>
		</td>
		<td style="padding: 5px; width: 300px;" valign="top">
			<table width="250px" cellpadding="0" cellspacing="0" border="0" class="ibn-propertysheet" style="table-layout: fixed;" runat="server" id="tblMain">
				<tr>
					<td style="padding: 0px;">
						<div class="dropstyle">
							<asp:Label runat="server" ID="lblClient"></asp:Label>
						</div>
					</td>
					<td id="tdChange" runat="server" class="btndown">
						<asp:Label ID="lblChange" runat="server"></asp:Label>
					</td>
				</tr>
			</table>
		</td>
	</tr>
	<tr>
		<td colspan="2" style="padding: 5px;" align="center">
			<div style="text-align: left; padding: 5px;">
				<b>
					<%=GetGlobalResourceObject("IbnFramework.Incident", "Comment").ToString()%>:</b></div>
			<asp:TextBox ID="txtComment" runat="server" Width="98%" Rows="9" TextMode="MultiLine" CssClass="text"></asp:TextBox>
		</td>
	</tr>
	<tr>
		<td colspan="2" align="center" valign="middle" style="padding: 10px;">
			<mc:IMButton ID="btnSave" runat="server">
			</mc:IMButton>
			&nbsp;
			<mc:IMButton ID="btnCancel" runat="server">
			</mc:IMButton>
		</td>
	</tr>
</table>
<div id="divDropDown" style="position: absolute; top: 30px; left: 100px; z-index: 255; padding: 0px; display: none; border: 1px solid #95b7f3;" class="ibn-rtetoolbarmenu ibn-propertysheet ibn-selectedtitle">
	<table cellpadding="0" cellspacing="0" border="0" width="250px" id="tableDD" runat="server" class="text">
	</table>
</div>
<input type="hidden" runat="server" id="hidResp" />
<asp:Button ID="btnRefresh" runat="server" CausesValidation="False" Style="display: none;"></asp:Button>
<asp:Button ID="btnRefreshMore" runat="server" CausesValidation="False" Style="display: none;"></asp:Button>
<div id="divErrors" runat="server" style="text-align: center;">
	<table style="padding: 10px;" class="ibn-propertysheet">
		<tr>
			<td>
				<img alt="" border="0" src='<%=this.Page.ResolveUrl("~/Layouts/Images/check.gif") %>' />
			</td>
			<td valign="top" style="padding-left: 10px;">
				<asp:Label ID="lblResult" CssClass="text" runat="server"></asp:Label>
			</td>
		</tr>
	</table>
	<br />
	<mc:IMButton ID="btnClose" runat="server">
	</mc:IMButton>
</div>
<div style="height: 290px; width: 630px; background-color: White; position: absolute; color: Red; left: 0px; top: 0px; padding-top: 30px; text-align: center; display: none; z-index: 1000;" id="noItemsSelected">
	<%=GetGlobalResourceObject("IbnFramework.Incident", "NoItemsSelectedForResponsible").ToString()%>
</div>
<asp:HiddenField ID="hfValues" runat="server" />
