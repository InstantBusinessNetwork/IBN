<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Incidents.Modules.Responsibility" Codebehind="Responsibility.ascx.cs" %>
<%@ Reference Control="~/Modules/BlockHeaderLightWithMenu.ascx" %>
<%@ Reference Control="~/Modules/ObjectDropDownNew.ascx" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="~/Modules/BlockHeaderLightWithMenu.ascx" %>
<%@ Register TagPrefix="ibn" TagName="ObjectDD" Src="~/Modules/ObjectDropDownNew.ascx" %>
<%@ Register TagPrefix="mc" namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>
<script type="text/javascript">
//<![CDATA[
function ChangeVisible(str, str1, obj)
{
	var trMain = document.getElementById('<%=trMain.ClientID%>');
	var tbl = document.getElementById('<%=tblMain.ClientID%>');
	var tbl2 = document.getElementById('tblMain2');
	var tbl3 = document.getElementById('tblMain3');
	if(str.indexOf('_'+obj.value+'_')>=0)
	{
		if (trMain)
			trMain.style.display = "";
		if (tbl)
			tbl.style.display = "";
		if (tbl2)
			tbl2.style.display = "";
		if (tbl3)
			tbl3.style.display = "none";
	}
	else
	{
		if (trMain)
			trMain.style.display = "none";
		if (tbl2)
			tbl2.style.display = "none";
		if (tbl3)
			tbl3.style.display = "none";
	}
	if(str1.indexOf('_'+obj.value+'_')>=0)
	{
		if (trMain)
			trMain.style.display = "";
		if (tbl3)
			tbl3.style.display = "";
		if (tbl)
			tbl.style.display = "none";
	}
}

var isNewHandlerSet_issResp = false;
var savedHandler_issResp = null;

function SelectThis(obj, _value)
{
	var obj1 = document.getElementById('<%=lblClient.ClientID%>');
	var _hid = document.getElementById('<%=hidResp.ClientID%>');
	var _hid2 = document.getElementById('<%=hidDecline.ClientID%>');
	if(obj1 && _hid && _value!=0)
	{
		obj1.innerHTML = obj.innerHTML;
		_hid.value = _value;
		if(_hid2)
			_hid2.value = "0";
	}
	closeMenu();
}
var old_value;
var old_intvalue;
function SelectMe(_value, _intvalue, _decline)
{
	var obj1 = document.getElementById('<%=lblClient.ClientID%>');
	var _hid = document.getElementById('<%=hidResp.ClientID%>');
	if(obj1 && _hid)
	{
		var _hid2 = document.getElementById('<%=hidDecline.ClientID%>');
		var cbAcc = document.getElementById('cbAccept');
		var cbDec = document.getElementById('cbDecline');
		var fsAcc = document.getElementById('fsAccept');
		var fsDec = document.getElementById('fsDecline');
		if(cbDec && cbAcc)
		{
			if(_decline=='0')
			{
				if(cbAcc.checked)
					cbDec.checked = false;
			}
			else
			{
				if(cbDec.checked)
					cbAcc.checked = false;
			}
		}
		if(cbDec && _hid2)
		{
			_hid2.value = (cbDec.checked)? '1' : '0';
		}
		if(cbDec && fsDec)
		{
			if(cbDec.checked)
				fsDec.className = "rest-black";
			else
				fsDec.className = "rest-gray";
		}
		if(cbAcc && fsAcc)
		{
			if(cbAcc.checked)
				fsAcc.className = "rest-black";
			else
				fsAcc.className = "rest-gray";
		}
		
		if(cbAcc && cbAcc.checked)
		{
			old_value = obj1.innerHTML;
			old_intvalue = _hid.value;
			obj1.innerHTML = _value;
			_hid.value = _intvalue;
		}
		else if(cbAcc && old_value && old_intvalue)
		{
			obj1.innerHTML = old_value;
			_hid.value = old_intvalue;
		}
	}
}
function TdOver(obj)
{
  if(obj)
  {
    if(obj.className == "cellclass")
      obj.className = "hovercellclass";
  }
}
function TdOut(obj)
{
 if(obj)
  {
    if(obj.className == "hovercellclass")
      obj.className = "cellclass";
  }
}
function getMenu(s)
{
	return document.getElementById(s)
}
function closeMenu()
{
	CancelBubble_issResp();
	
	getMenu('divDropDown').style.display = "none";
	if (isNewHandlerSet_issResp)
	{
		document.onclick = savedHandler_issResp;
		savedHandler_issResp = null;
		isNewHandlerSet_issResp = false;
	}
}
function openDD(e, curObjID)
{
	CancelBubble_issResp(e);
	 
	var curObj = document.getElementById(curObjID);
	menu = getMenu('divDropDown');
	off = GetTotalOffset(curObj);
	menu.style.left = (off.Left-233).toString() + "px";
	menu.style.top = (off.Top+22).toString() + "px";
	menu.style.display = "";
	
	if (!isNewHandlerSet_issResp)
	{
		savedHandler_issResp = document.onclick;
		document.onclick = closeMenu;
		isNewHandlerSet_issResp = true;
	}
}
function ShowHideList(e, curObjID)
{
	obj = getMenu('divDropDown');
	if (obj.style.display == "none")
	{
		openDD(e, curObjID);
	}
	else
		closeMenu();
}

function CancelBubble_issResp(e)
{
 var e1 = (e) ? e : ((window.event) ? window.event : null);
 if (e1)
 {
  e1.cancelBubble = true;
  if(e1.stopPropagation)
   e1.stopPropagation();
 }
}

function GetTotalOffset(eSrc)
{
	this.Top = 0;
	this.Left = 0;
	while (eSrc)
	{
		this.Top += eSrc.offsetTop;
		this.Left += eSrc.offsetLeft;
		eSrc = eSrc.offsetParent;
	}
	return this;
}
function OpenWindow(query,w,h,scroll)
{
	var l = (screen.width - w) / 2;
	var t = (screen.height - h) / 2;
	
	winprops = 'resizable=0, height='+h+',width='+w+',top='+t+',left='+l;
	if (scroll) winprops+=',scrollbars=1';
	var f = window.open(query, "_blank", winprops);
}

function RefreshFromMore(params)
{
	var obj = Sys.Serialization.JavaScriptSerializer.deserialize(params);
	if(obj && obj.CommandArguments && obj.CommandArguments.Key)
		__doPostBack('<%=btnRefreshMore.UniqueID %>', obj.CommandArguments.Key);
	else if(obj && obj.CommandArguments && obj.CommandArguments.SelectedValue && obj.CommandArguments.SelectedHtml)
	{
		var hidResp = document.getElementById('<%=hidResp.ClientID %>');
		if(hidResp)
			hidResp.value = obj.CommandArguments.SelectedValue;
		var lblClient = document.getElementById('<%=lblClient.ClientID %>');
		if (lblClient)
			lblClient.innerHTML = obj.CommandArguments.SelectedHtml;
	}
}

function RefreshFromGroup(params)
{
	var obj = Sys.Serialization.JavaScriptSerializer.deserialize(params);
	if(obj && obj.CommandArguments && obj.CommandArguments.Key)
		__doPostBack('<%=btnRefresh.UniqueID %>', obj.CommandArguments.Key);
}
//]]>
</script>
<table cellspacing="0" cellpadding="0" border="0" width="100%" style="margin-top:5px">
	<tr>
		<td>
			<ibn:blockheader id="secHeader" runat="server" />
		</td>
	</tr>
</table>
<asp:UpdatePanel runat="server" ID="MainPanel" RenderMode="Inline">
	<ContentTemplate>
		<table class="ibn-stylebox-light ibn-propertysheet" cellspacing="0" cellpadding="0" width="100%" border="0" runat="server" id="MainTable">
			<tr>
				<td width="420px" valign="top">
					<table cellspacing="0" cellpadding="0" width="100%" border="0" class="ibn-propertysheet" style="table-layout:fixed;">
						<tr>
							<td style="width:110px;" class="padding5"><b><%=LocRM.GetString("Status")%>:</b></td>
							<td style="width:300px;" class="padding5"><asp:DropDownList ID="ddStatus" Runat="server" Width="260px"></asp:DropDownList></td>
						</tr>
						<tr id="trPriority" runat="server">
							<td class="padding5"><b><%=LocRM.GetString("Priority")%>:</b></td>
							<td class="padding5"><asp:DropDownList ID="ddPriority" Runat="server" Width="260px"></asp:DropDownList></td>
						</tr>
						<tr id="trProject" runat="server">
							<td class="padding5"><b><%=LocRM.GetString("Project")%>:</b></td>
							<td class="padding5">
								<ibn:ObjectDD ID="ddProject" ObjectTypes="3" runat="server" Width="260px" />
							</td>
						</tr>
						<tr id="trIssueBox" runat="server">
							<td class="padding5"><b><%=LocRM.GetString("IssueBox")%>:</b></td>
							<td class="padding5">
								<asp:DropDownList ID="ddIssueBox" runat="server" Width="260px" CssClass="text" 
									AutoPostBack="true" onselectedindexchanged="ddIssueBox_SelectedIndexChanged"></asp:DropDownList>
							</td>
						</tr>
						<tr>
							<td></td>
							<td class="padding5">
								<asp:CheckBox runat="server" ID="UseNewResponsible" Checked="false" Visible="false" 
									AutoPostBack="true" oncheckedchanged="UseNewResponsible_CheckedChanged" />
							</td>
						</tr>
						<tr id="trMain" runat="server">
							<td class="padding5"><b><%=LocRM.GetString("tResponsible")%>:</b></td>
							<td class="padding5">
								<table width="260px" cellpadding="0" cellspacing="0" border="0" class="ibn-propertysheet" style="table-layout:fixed;" runat="server" id="tblMain">
									<tr>
										<td style="padding:0px">
											<div class="dropstyle">
												<asp:Label runat="server" ID="lblClient"></asp:Label>
											</div>
										</td>
										<td id="tdChange" runat="server" class="btndown">
											<asp:Label ID="lblChange" Runat="server"></asp:Label>
										</td>
									</tr>
								</table>
								<table cellspacing="0" cellpadding="0" width="260px" border="0" class="ibn-propertysheet" style="table-layout:fixed" id="tblMain3">
									<tr>
										<td valign="top"><asp:Label ID="lblController" Runat="server" CssClass="text"></asp:Label></td>
									</tr>
								</table>
							</td>
						</tr>
						<tr id="trAcceptDecline" runat="server" >
							<td colspan="2" class="padding5">
								<table cellspacing="0" cellpadding="2" width="410px" class="text" id="tblMain2">
									<tr>
										<td><asp:Label ID="lblAccept" Runat="server" CssClass="text"></asp:Label></td>
										<td><asp:Label ID="lblDecline" Runat="server" CssClass="text"></asp:Label></td>
									</tr>
									<tr>
										<td valign="top"><div id="lblAcceptText" runat="server" class="text"></div></td>
										<td valign="top"><div id="lblDeclineText" runat="server" class="text"></div></td>
									</tr>
								</table>
							</td>
						</tr>
					</table>
				</td>
				<td valign="top" align="right" style="padding: 10px;">
					<asp:TextBox ID="txtDescription" Runat="server" Width="98%" CssClass="text" TextMode="MultiLine" Rows="6"></asp:TextBox>
					<div style="padding:10px 7px 7px 7px;">
						<mc:ImButton runat="server" class="text" ID="imbSend" style="width:110px"></mc:ImButton>
					</div>
				</td>
			</tr>
		</table>
		<div id="divDropDown" style="position:absolute; top:30px;left:100px; z-index:255;padding:0px;display:none; border: 1px solid #95b7f3;" class="ibn-rtetoolbarmenu ibn-propertysheet ibn-selectedtitle">
			<table cellpadding="0" cellspacing="0" border="0" width="250px" id="tableDD" runat="server" class="text">
			</table>
		</div>
		<input type="hidden" runat="server" id="hidResp" />
		<input type="hidden" runat="server" id="hidDecline" />
		<asp:Button id="btnRefresh" runat="server" CausesValidation="False" style="display:none;"></asp:Button>
		<asp:Button id="btnRefreshMore" runat="server" CausesValidation="False" style="display:none;"></asp:Button>
	</ContentTemplate>
</asp:UpdatePanel>