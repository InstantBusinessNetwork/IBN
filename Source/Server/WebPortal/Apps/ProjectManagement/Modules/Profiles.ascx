<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Profiles.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.ProjectManagement.Modules.Profiles" %>
<%@ Register TagPrefix="ibn" Assembly="Mediachase.Ibn.Web.UI.WebControls" Namespace="Mediachase.Ibn.Web.UI.WebControls" %>
<%@ Reference Control="~/Apps/MetaUIEntity/Modules/EntityDropDown.ascx" %>
<%@ Reference Control="~/Apps/Common/DateTimePickerAjax/PickerControl.ascx" %>
<%@ Register TagPrefix="ibn" TagName="EntityDD" src="~/Apps/MetaUIEntity/Modules/EntityDropDown.ascx" %>
<%@ Register TagPrefix="mc" TagName="Picker" Src="~/Apps/Common/DateTimePickerAjax/PickerControl.ascx" %>
<style type='text/css'>
	.tab { text-align: center; padding: 2px 5px 2px 5px; border:1px solid #95b7f3; margin-top:10px;}
	.tabSelected {border:1px solid #95b7f3; margin-top:10px; border-bottom:none;padding: 2px 5px 2px 5px; text-align: center; font-family: verdana; font-size: 12px; font-weight:bold; color: #CE3431; }
	.tabGap { border-left: none;border-top: none;border-right: none;}	
	A.tabTLink { color: #003399; font-family: verdana; font-size: 12px; text-decoration: none; font-weight:normal; }
	A.tabTLinkSelected { color: #CE3431; font-family: verdana; font-size: 12px; text-decoration: none; font-weight:bold; }
	A:hover.tabTLink { color: #CE3431; font-family: verdana; font-size: 12px; text-decoration: none; font-weight:normal; }
	
	table.padTable5 tbody tr td
	{
		padding: 5px ! important;
	}
	table.padTable3 tbody tr td
	{
		padding: 3px ! important;
	}
	table.padTable2 tbody tr td
	{
		padding: 2px ! important;
	}
</style>
<script type="text/javascript">
function ChangeTab(id)
{
	var hfValue = document.getElementById('<%= hfValue.ClientID %>');
	
	var panel1 = document.getElementById('<%= panel1.ClientID %>');
	var panel2 = document.getElementById('<%= panel2.ClientID %>');
	var panel3 = document.getElementById('<%= panel3.ClientID %>');
	var panel4 = document.getElementById('<%= panel4.ClientID %>');
	
	var td1 = document.getElementById('tdTab1');
	var td2 = document.getElementById('tdTab2');
	var td3 = document.getElementById('tdTab3');
	var td4 = document.getElementById('tdTab4');
	
	var a1 = document.getElementById('aTab1');
	var a2 = document.getElementById('aTab2');
	var a3 = document.getElementById('aTab3');
	var a4 = document.getElementById('aTab4');
	
	panel1.style.display = "none";
	panel2.style.display = "none";
	panel3.style.display = "none";
	panel4.style.display = "none";
		
	td1.className = "ibn-navframe tab";
	td2.className = "ibn-navframe tab";
	td3.className = "ibn-navframe tab";
	td4.className = "ibn-navframe tab";
	
	a1.className = "tabTLink";
	a2.className = "tabTLink";
	a3.className = "tabTLink";
	a4.className = "tabTLink";
		
	if(id == "Panel1")
	{
		td1.className = "tabSelected";
		a1.className = "tabTLinkSelected";
		panel1.style.display = "";
		hfValue.value = "Panel1";
	}
	else if(id == "Panel2")
	{
		td2.className = "tabSelected";
		a2.className = "tabTLinkSelected";
		panel2.style.display = "";
		hfValue.value = "Panel2";
	}
	else if(id == "Panel3")
	{
		td3.className = "tabSelected";
		a3.className = "tabTLinkSelected";
		panel3.style.display = "";
		hfValue.value = "Panel3";
	}
	else if(id == "Panel4")
	{
		td4.className = "tabSelected";
		a4.className = "tabTLinkSelected";
		panel4.style.display = "";
		hfValue.value = "Panel4";
	}
	resizeTable();
}

function ChangeDD(obj)
{
	id=obj.value;
	if(obj.id.indexOf("ddSDType")>=0)
	{
		objDiv = document.getElementById('<%=tdStartDate.ClientID %>');
		if(id!="0")
			objDiv.style.display = 'block';
		else
			objDiv.style.display = 'none';
	}
	if(obj.id.indexOf("ddFDType")>=0)
	{
		objDiv = document.getElementById('<%=tdFinishDate.ClientID %>');
		if(id!="0")
			objDiv.style.display = 'block';
		else
			objDiv.style.display = 'none';
	}
}

function ShowListBox(obj)
{
	var objPrjGrpType = document.forms[0].<%=ddPrjGrpType.ClientID%>;
	var objLBPrjGrp = document.forms[0].<%=lbPrjGrps.ClientID%>;
	var objGenCatType = document.forms[0].<%=ddGenCatType.ClientID%>;
	var objLBGen = document.forms[0].<%=lbGenCats.ClientID%>;
	var objPrjCatType = document.forms[0].<%=ddPrjCatType.ClientID%>;
	var objLBPrj = document.forms[0].<%=lbPrjCats.ClientID%>;
	
	if(obj == objGenCatType)
	{
		var selValue = obj.value;
		if (selValue == "0")
			objLBGen.style.display = "none";
		else if (selValue == "1")
		{
			objLBGen.style.display = "block";
		}
		else if (selValue == "2")
		{
			objLBGen.style.display = "block";
		}
	}
	else if(obj == objPrjCatType)
	{
		var selValue = obj.value;
		if (selValue == "0")
			objLBPrj.style.display = "none";
		else if (selValue == "1")
		{
			objLBPrj.style.display = "block";
		}
		else if (selValue == "2")
		{
			objLBPrj.style.display = "block";
		}
	}
	else if(obj == objPrjGrpType)
	{
		var selValue = obj.value;
		if (selValue == "0")
			objLBPrjGrp.style.display = "none";
		else if (selValue == "1")
		{
			objLBPrjGrp.style.display = "block";
		}
		else if (selValue == "2")
		{
			objLBPrjGrp.style.display = "block";
		}
	}
}

function resizeTable()
{
	var panel1 = document.getElementById('<%= panel1.ClientID %>');
	var panel2 = document.getElementById('<%= panel2.ClientID %>');
	var panel3 = document.getElementById('<%= panel3.ClientID %>');
	var panel4 = document.getElementById('<%= panel4.ClientID %>');
	
	var hfValue = document.getElementById('<%= hfValue.ClientID %>');
	
	var obj;
	if(hfValue.value == "Panel1")
		obj = panel1;
	else if(hfValue.value == "Panel2")
		obj = panel2;
	else if(hfValue.value == "Panel3")
		obj = panel3;
	else if(hfValue.value == "Panel4")
		obj = panel4;
		
	var toolbarRow = document.getElementById('tableHeader');

	var intHeight = 0;
	if (typeof(window.innerWidth) == "number")
	{
	  intHeight = window.innerHeight;
	}
	else if (document.documentElement && (document.documentElement.clientWidth || document.documentElement.clientHeight))
	{
	  intHeight = document.documentElement.clientHeight;
	}
	else if (document.body && (document.body.clientWidth || document.body.clientHeight))
	{
	  intHeight = document.body.clientHeight;
	}

	if(obj && toolbarRow && (intHeight - toolbarRow.offsetHeight - 40) > 0)
		obj.style.height = (intHeight - toolbarRow.offsetHeight - 40) + "px";
} 
window.onresize=resizeTable; 
window.onload=resizeTable; 
</script>
<table id="tableHeader" cellspacing="0" cellpadding="0" border="0" style="border-width:0px;width:100%;border-collapse:collapse;">
	<tr>
		<td>
			<table cellspacing="0" cellpadding="0" border="0" style="border-width:0px;width:100%;border-collapse:collapse;margin-top:5px;">
				<tr>
					<td class="ibn-stylebox tabGap">&nbsp;</td>
					<td id="tdTab4" class="ibn-navframe tab" style="width:120px;white-space:nowrap;width:120px;">
						<nobr>&nbsp;<a id="aTab4" class='tabTLink' href="javascript:ChangeTab('Panel4');"><%=GetGlobalResourceObject("IbnFramework.Incident", "GeneralTab").ToString()%></a>&nbsp;</nobr>
					</td>
					<td class="ibn-stylebox tabGap">&nbsp;</td>
					<td id="tdTab1" class="ibn-navframe tab" style="width:120px;white-space:nowrap;width:120px;">
						<nobr>&nbsp;<a id="aTab1" class='tabTLink' href="javascript:ChangeTab('Panel1');"><%=GetGlobalResourceObject("IbnFramework.Incident", "FieldsTab").ToString()%></a>&nbsp;</nobr>
					</td>
					<td class="ibn-stylebox tabGap">&nbsp;</td>
					<td id="tdTab2" class="ibn-navframe tab" style="width:120px;white-space:nowrap;width:120px;">
						<nobr>&nbsp;<a id="aTab2" class='tabTLink' href="javascript:ChangeTab('Panel2');"><%=GetGlobalResourceObject("IbnFramework.Incident", "GroupsTab").ToString()%></a>&nbsp;</nobr>
					</td>
					<td class="ibn-stylebox tabGap">&nbsp;</td>
					<td id="tdTab3" class="ibn-navframe tab" style="width:120px;white-space:nowrap;width:120px;">
						<nobr>&nbsp;<a id="aTab3" class='tabTLink' href="javascript:ChangeTab('Panel3');"><%=GetGlobalResourceObject("IbnFramework.Incident", "FiltersTab").ToString()%></a>&nbsp;</nobr>
					</td>
					<td class="ibn-stylebox tabGap">&nbsp;</td>
					<td class="ibn-stylebox tabGap" width="100%">&nbsp;</td>
				</tr>
			</table>
		</td>
	</tr>
</table>
<asp:Panel ID="panel1" runat="server">
	<table width="100%" class="text">
		<tr>
			<td valign="top" align="center">
				<div style="width:70%;padding-top:15px;">
					<ibn:BlockHeaderLight ID="lgdFieldSet" runat="server" HeaderCssClass="ibn-toolbar-light" />
					<table width="100%" class="ibn-stylebox-light padTable5">
						<tr>
							<td width="50%" valign="top">
								<asp:DropDownList ID="ddFieldSets" runat="server" Width="200px" CssClass="text"></asp:DropDownList>
							</td>
							<td valign="top" align="center" width="60">
								<img alt="" src='<% =ResolveClientUrl("~/layouts/images/quicktip.gif") %>' border="0" />
							</td>
							<td class="text" style="PADDING-RIGHT: 15px" valign="top"><asp:Literal ID="Literal1" runat="server" Text="<%$Resources: IbnFramework.Project, NewViewTab1Comment%>" /></td>
						</tr>
					</table>
				</div>
			</td>
		</tr>
	</table>
</asp:Panel>
<asp:Panel ID="panel2" runat="server">
	<table width="100%" class="text">
		<tr>
			<td valign="top" align="center">
				<div style="width:70%;padding-top:15px;">
					<ibn:BlockHeaderLight ID="lgdGroupSet" runat="server" HeaderCssClass="ibn-toolbar-light" />
					<table width="100%" class="ibn-stylebox-light padTable5">
						<tr>
							<td width="50%" valign="top">
								<asp:DropDownList ID="ddGroupField" runat="server" Width="200px" CssClass="text"></asp:DropDownList>
							</td>
							<td valign="top" align="center" width="60">
								<img alt="" src='<% =ResolveClientUrl("~/layouts/images/quicktip.gif") %>' border="0" />
							</td>
							<td class="text" style="PADDING-RIGHT: 15px" valign="top"><asp:Literal ID="Literal2" runat="server" Text="<%$Resources: IbnFramework.Project, NewViewTab2Comment%>" /></td>
						</tr>
					</table>
				</div>
			</td>
		</tr>
	</table>
</asp:Panel>
<asp:Panel ID="panel3" runat="server">
	<table cellpadding="0" cellspacing="0" border="0" style="margin:5px;">
		<tr height="30px">
			<td width="360px">
				<table cellspacing="0" width="100%" border="0">
					<tr>
						<td class="text" width="100px"><%= LocRM.GetString("StartDate")%>:</td>
						<td><asp:DropDownList onchange="ChangeDD(this);" id="ddSDType" runat="server" Width="160px"></asp:DropDownList></td>
						<td runat="server" id="tdStartDate" style="padding-left:5px">
							<mc:Picker ID="dtcStartDate" runat="server" DateCssClass="text" DateWidth="85px" ShowImageButton="false" DateIsRequired="true" />
						</td>
					</tr>
				</table>
			</td>
			<td>
				<table cellspacing="0" width="100%" border="0">
					<tr>
						<td class="text" valign="top">
							<asp:CheckBox Runat="server" ID="chkOnlyForUser"></asp:CheckBox>
						</td>
					</tr>
				</table>
			</td>
		</tr>
		<tr height="30px">
			<td width="360px">
				<table cellspacing="0" width="100%" border="0">
					<tr>
						<td class="text" align="left" width="100px"><%=LocRM.GetString("FinishDate")%>:
						</td>
						<td align="left"><asp:DropDownList onchange="ChangeDD(this);" id="ddFDType" runat="server" Width="160px"></asp:DropDownList></td>
						<td runat="server" id="tdFinishDate" style="padding-left:10px">
							<mc:Picker ID="dtcEndDate" runat="server" DateCssClass="text" DateWidth="85px" ShowImageButton="false" DateIsRequired="true" />
						</td>
					</tr>
				</table>
			</td>
			<td rowspan="8" valign="top">
				<table cellpadding="0" cellspacing="4" border="0">
					<tr id="trPrGroups" runat="server">
						<td width="125px" valign="top"><%=LocRM.GetString("Portfolios")%>:</td>
						<td valign="top">
							<asp:DropDownList ID="ddPrjGrpType" Width="160px" Runat="server" onchange="ShowListBox(this)"></asp:DropDownList><br />
							<asp:ListBox ID="lbPrjGrps" Runat="server" CssClass="text" Rows="4" Width="160px" SelectionMode="Multiple"></asp:ListBox>
						</td>
					</tr>
					<tr id="trGenCats" runat="server">
						<td valign="top"><%=LocRM.GetString("GeneralCats")%>:</td>
						<td valign="top">
							<asp:DropDownList ID="ddGenCatType" Width="160px" Runat="server" onchange="ShowListBox(this)"></asp:DropDownList><br />
							<asp:ListBox ID="lbGenCats" Runat="server" CssClass="text" Rows="4" Width="160px" SelectionMode="Multiple"></asp:ListBox>
						</td>
					</tr>
					<tr id="trPrCats" runat="server">
						<td valign="top"><%=LocRM.GetString("ProjectCats")%>:</td>
						<td valign="top">
							<asp:DropDownList ID="ddPrjCatType" Width="160px" Runat="server" onchange="ShowListBox(this)"></asp:DropDownList><br />
							<asp:ListBox ID="lbPrjCats" Runat="server" CssClass="text" Rows="4" Width="160px" SelectionMode="Multiple"></asp:ListBox>
						</td>
					</tr>
				</table>
			</td>
		</tr>
		<tr height="30px" id="trClient" runat="server">
			<td width="360px">
				<table cellspacing="0" width="100%" border="0">
					<tr>
						<td class="text" align="left" width="100px"><%=LocRM.GetString("Client")%>:
						</td>
						<td align="left"><ibn:EntityDD id="ClientControl" ObjectTypes="Contact,Organization" runat="server" Width="160px" ItemCount="6"  ClassName="Project" ViewName="" PlaceName="ProjectList" CommandName="MC_Client_EntityDD"/></td>
					</tr>
				</table>
			</td>
		</tr>
		<tr height="30px">
			<td width="360px">
				<table cellspacing="0" width="100%" border="0">
					<tr>
						<td class="text" width="100px"><%=LocRM.GetString("Status")%>:</td>
						<td align="left"><asp:DropDownList id="ddStatus" runat="server" Width="160px"></asp:DropDownList></td>
					</tr>
				</table>
			</td>
		</tr>
		<tr height="30px">
			<td width="360px">
				<table cellspacing="0" width="100%" border="0">
					<tr>
						<td class="text" width="100px"><%=LocRM.GetString("tPrjPhase")%>:</td>
						<td align="left"><asp:DropDownList id="ddPrjPhases" runat="server" Width="160px"></asp:DropDownList></td>
					</tr>
				</table>
			</td>
		</tr>
		<tr height="30px">
			<td width="360px">
				<table cellspacing="0" width="100%" border="0">
					<tr>
						<td class="text" width="100px"><%=LocRM.GetString("Type")%>:</td>
						<td align="left"><asp:DropDownList id="ddType" Width="160px" runat="server"></asp:DropDownList></td>
					</tr>
				</table>
			</td>
		</tr>
		<tr height="30px">
			<td width="360px">
				<table cellspacing="0" width="100%" border="0">
					<tr>
						<td class="text" width="100px"><%=LocRM.GetString("Manager")%>:</td>
						<td align="left"><asp:DropDownList id="ddManager" Width="160px" runat="server"></asp:DropDownList></td>
					</tr>
				</table>
			</td>
		</tr>
		<tr height="30px">
			<td width="360px">
				<table cellspacing="0" width="100%" border="0">
					<tr>
						<td class="text" align="left" width="100px"><%=LocRM.GetString("tExeManager")%>:
						</td>
						<td align="left"><asp:DropDownList id="ddExeManager" Width="160px" runat="server"></asp:DropDownList></td>
					</tr>
				</table>
			</td>
		</tr>
		<tr height="30px" id="trPriority" runat="server">
			<td width="360px">
				<table cellspacing="0" width="100%" border="0">
					<tr>
						<td class="text" align="left" width="100px"><%=LocRM.GetString("Priority")%>:
						</td>
						<td align="left"><asp:DropDownList id="ddPriority" Width="160px" runat="server"></asp:DropDownList></td>
					</tr>
				</table>
			</td>
		</tr>
	</table>
</asp:Panel>
<asp:Panel ID="panel4" runat="server">
	<table width="100%" class="text">
		<tr>
			<td valign="top" align="center">
				<div style="width:70%;padding-top:15px;">
					<ibn:BlockHeaderLight ID="lgdFinish" runat="server" HeaderCssClass="ibn-toolbar-light" />
					<table width="100%" class="ibn-stylebox-light padTable5">
						<tr>
							<td width="50%" valign="top">
								<asp:TextBox ID="txtTitle" runat="server" Width="200px" CssClass="text"></asp:TextBox><asp:RequiredFieldValidator ID="rfTitle" runat="server" ControlToValidate="txtTitle" CssClass="text" Display="Dynamic"></asp:RequiredFieldValidator>
								<br />
								<br />
								<asp:CheckBox ID="cbIsPublic" runat="server" />
							</td>
							<td valign="top" align="center" width="60">
								<img alt="" src='<% =ResolveClientUrl("~/layouts/images/quicktip.gif") %>' border="0" />
							</td>
							<td class="text" style="PADDING-RIGHT: 15px" valign="top"><asp:Literal ID="Literal3" runat="server" Text="<%$Resources: IbnFramework.Project, NewViewStep4Comment%>" /></td>
						</tr>
					</table>
				</div>
			</td>
		</tr>
	</table>
</asp:Panel>
<asp:Panel Width="100%" runat="server" ID="divButtons" style="border-top:1px solid #cccccc;padding-top: 7px;">
	<table width="100%">
		<tr>
			<td style="width:200px;">
				<asp:ValidationSummary ID="valSum" runat="server" CssClass="text" />
			</td>
			<td align="left">
				<asp:Button ID="btnSave" runat="server" Text="Save" Width="90px" OnClick="btnSave_Click" />&nbsp;&nbsp;&nbsp;
				<asp:Button ID="btnClose" runat="server" Text="Close" Width="90px" />
			</td>
		</tr>
	</table>
</asp:Panel>
<asp:HiddenField ID="hfValue" runat="server" />