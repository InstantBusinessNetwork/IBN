<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Projects.Modules.FinanceAccountsList" Codebehind="FinanceAccountsList.ascx.cs" %>
<%@ register TagPrefix="dg" namespace="Mediachase.UI.Web.Modules.DGExtension" Assembly="Mediachase.UI.Web" %>
<STYLE type=text/css>
	.pp { PADDING-RIGHT: 3px; PADDING-LEFT: 3px;Height:21px; BORDER-BOTTOM: #e4e4e4 1px solid }
	.pp A { COLOR: #003399; TEXT-DECORATION: none }
	.pp A:visited { COLOR: #003399 }
	.pp A:hover { COLOR: red }
	.headstyle {padding-top:5px;padding-bottom:5px; border-bottom:1px solid #e4e4e4}
	.headstyle1 {padding-left:12px; padding-top:5px;padding-bottom:5px; border-bottom:1px solid #e4e4e4}
</STYLE>
<script language=javascript>
function ShowHideList(curObjID,Cur,Sub,Total,ParentVal)
{
	obj = document.getElementById('divEditValue');
	if (obj.style.display == "none")
	{
		openMenu(curObjID,Cur,Sub,Total,ParentVal);
	}
	else
		closeMenu();
}
function closeMenu()
{
	getMenu('divEditValue').style.display = "none";
	getMenu('divEditItem').style.display = "none";
	//document.forms[0].<%=txtAccountTitle.ClientID %>.value="1";
}

function openEditMenu(curObjID, sCurString)
{
	var curObj = document.getElementById(curObjID);
	document.forms[0].<%=txtAccountTitle.ClientID %>.value = sCurString;
	document.forms[0].<%=hdnAccountId.ClientID %>.value = curObjID.substring(3);
	document.forms[0].<%=hdnCollapseExpand.ClientID %>.value = "1"; //edit
	var lgdObj = document.getElementById('<%=lgdEditItem.ClientID %>');
	if(lgdObj!=null)
		lgdObj.innerHTML = '<%=LocRM.GetString("tEditAcc")%>';
	menu = getMenu('divEditItem');
	off = GetTotalOffset(curObj);
	menu.style.left = (off.Left-150).toString() + "px";
	menu.style.top = (off.Top-45).toString() + "px";
	menu.style.display = "";
	document.forms[0].<%=txtAccountTitle.ClientID %>.focus();
}

function openNewMenu(curObjID)
{
	var curObj = document.getElementById(curObjID);
	document.forms[0].<%=hdnAccountId.ClientID %>.value = curObjID.substring(3);
	var lgdObj = document.getElementById('<%=lgdEditItem.ClientID %>');
	if(lgdObj!=null)
		lgdObj.innerHTML = '<%=LocRM.GetString("tNewAcc")%>';
	document.forms[0].<%=hdnCollapseExpand.ClientID %>.value = "0"; //new
	document.forms[0].<%=txtAccountTitle.ClientID %>.value="";
	menu = getMenu('divEditItem');
	off = GetTotalOffset(curObj);
	menu.style.left = (off.Left-150).toString() + "px";
	menu.style.top = (off.Top-45).toString() + "px";
	menu.style.display = "";
	document.forms[0].<%=txtAccountTitle.ClientID %>.focus();
}

function openMenu(curObjID,Cur,Sub,Total,ParentVal)
{
	document.forms[0].<%=txtSum.ClientID %>.value = Total;
	document.forms[0].<%=hdnAccountId.ClientID %>.value = curObjID.substring(3);
	document.forms[0].<%=lowBorder.ClientID %>.value = Sub;
	document.forms[0].<%=highBorder.ClientID %>.value = ParentVal;
	if(curObjID.indexOf("TWL")>=0)
		document.forms[0].<%=hdnCollapseExpand.ClientID %>.value = "0"; //target
	else
		document.forms[0].<%=hdnCollapseExpand.ClientID %>.value = "1"; //estimate
	
	document.forms[0].<%=cbSaveParent.ClientID %>.checked = true;
	
	var objSpan = document.getElementById('<%=cvSum2.ClientID %>');
	if(objSpan!=null)
	{
		objSpan.enabled = true;
		objSpan.style.display = 'none';
	}
	
	var sSpan = document.getElementById('<%=lblSub.ClientID %>');
	sSpan.innerHTML = Sub;
	sSpan = document.getElementById('<%=lblCur.ClientID %>');
	sSpan.innerHTML = Cur;
	sSpan = document.getElementById('<%=lblHelp.ClientID %>');
	var str = sSpan.innerHTML
	if(str.indexOf("=")>=0)
		str = str.substring(str.indexOf("=")+1);
	if(str.indexOf("&lt;")>=0)
		str = str.substring(0,str.indexOf("&lt;"));
	if(ParentVal>=1e+25)
		str = Sub+"&lt;="+str;
	else
		str = Sub+"&lt;="+str+"&lt;="+ParentVal;
	sSpan.innerHTML = str;
	
	menu = getMenu('divEditValue');
	var curObj = document.getElementById(curObjID);
	off = GetTotalOffset(curObj);
	menu.style.left = (off.Left-70).toString() + "px";
	menu.style.top = (off.Top-65).toString() + "px";
	menu.style.display = "";
	document.forms[0].<%=txtSum.ClientID %>.focus();
}
function EnableDisableVal(checkObj)
{
	var objSpan = document.getElementById('<%=cvSum2.ClientID %>');
	if(objSpan!=null)
	{
		if(checkObj.checked)
			objSpan.enabled = true;
		else
			objSpan.enabled = false;
	}
	objSpan = document.getElementById('<%=lblHelp.ClientID %>');
	var lowVal = document.forms[0].<%=lowBorder.ClientID %>.value;
	var highVal = document.forms[0].<%=highBorder.ClientID %>.value;
	var str = objSpan.innerHTML
	if(str.indexOf("=")>=0)
		str = str.substring(str.indexOf("=")+1);
	if(str.indexOf("&lt;")>=0)
		str = str.substring(0,str.indexOf("&lt;"));
	if(checkObj.checked && highVal.indexOf("e+")<0)
		str = lowVal+"&lt;="+str+"&lt;="+highVal;
	else
		str = lowVal+"&lt;="+str;
	objSpan.innerHTML = str;
}
function getMenu(s)
{
	return document.getElementById(s)
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
</script>
<script language=javascript>
function CollapseExpand(CEType, AccountId)
{
	document.forms[0].<%=hdnAccountId.ClientID %>.value = AccountId;
	document.forms[0].<%=hdnCollapseExpand.ClientID %>.value = CEType;
	<%=Page.ClientScript.GetPostBackEventReference(lbCollapseExpandAcc,"") %>
}
</script>
<table cellpadding="0" cellspacing="0" width="100%" border="0" style="OVERFLOW:hidden" align="left" >
	<tr>
		<td valign="top">
			<dg:DataGridExtended Runat="server" ID="dgAccounts" allowpaging="True" allowsorting="False" cellpadding="0" gridlines="None" CellSpacing="0" borderwidth="0px" autogeneratecolumns="False" width="100%" pagesize="10" LayoutFixed="false">
				<columns>
					<asp:boundcolumn datafield="AccountId" Visible="False"></asp:boundcolumn>
					<asp:boundcolumn datafield="ProjectId" Visible="False"></asp:boundcolumn>
					<asp:boundcolumn datafield="OutlineNumber" Visible="False"></asp:boundcolumn>
					<asp:boundcolumn datafield="OutlineLevel" Visible="False"></asp:boundcolumn>
					<asp:templatecolumn>
						<ItemStyle cssclass="ibn-vb2 pp"></ItemStyle>
						<HeaderStyle CssClass="ibn-vh2 headstyle"></HeaderStyle>
						<itemtemplate>
							<%# GetTitle((int)DataBinder.Eval(Container.DataItem, "AccountId"),(bool)DataBinder.Eval(Container.DataItem, "IsSummary"), (bool)DataBinder.Eval(Container.DataItem, "IsCollapsed"), (int)DataBinder.Eval(Container.DataItem, "OutlineLevel"), DataBinder.Eval(Container.DataItem, "Title").ToString())%>
						</itemtemplate>
					</asp:templatecolumn>
					<asp:templatecolumn>
						<HeaderStyle cssclass="ibn-vh2 headstyle1" width="70px"></HeaderStyle>
						<ItemStyle horizontalalign="center" width="70px" cssclass="ibn-vb2 pp"></ItemStyle>
						<itemtemplate>
							<%# Mediachase.IBN.Business.Project.CanEditFinances((int)DataBinder.Eval(Container.DataItem, "ProjectId")) ?
								"<a id=\"TWL"+DataBinder.Eval(Container.DataItem, "AccountId").ToString()+"\" href=\"javascript:ShowHideList('TWL"+DataBinder.Eval(Container.DataItem, "AccountId").ToString()+"',"+((decimal)DataBinder.Eval(Container.DataItem, "TCur")).ToString("f",culture)+","+((decimal)DataBinder.Eval(Container.DataItem, "TSub")).ToString("f",culture)+","+((decimal)DataBinder.Eval(Container.DataItem, "TTotal")).ToString("f",culture)+","+((decimal)DataBinder.Eval(Container.DataItem, "TParent")).ToString("f",culture)+")\">"+((decimal)DataBinder.Eval(Container.DataItem, "TTotal")).ToString("f",culture)+"</a>"
								: ((decimal)DataBinder.Eval(Container.DataItem, "TTotal")).ToString("f",culture)
							%>
						</itemtemplate>
					</asp:templatecolumn>
					<asp:templatecolumn>
						<HeaderStyle cssclass="ibn-vh2 headstyle1" width="70px"></HeaderStyle>
						<ItemStyle horizontalalign="center" width="70px" cssclass="ibn-vb2 pp"></ItemStyle>
						<itemtemplate>
							<%# Mediachase.IBN.Business.Project.CanEditFinances((int)DataBinder.Eval(Container.DataItem, "ProjectId")) ?
								"<a id=\"EWL"+DataBinder.Eval(Container.DataItem, "AccountId").ToString()+"\" href=\"javascript:ShowHideList('EWL"+DataBinder.Eval(Container.DataItem, "AccountId").ToString()+"',"+((decimal)DataBinder.Eval(Container.DataItem, "ECur")).ToString("f",culture)+","+((decimal)DataBinder.Eval(Container.DataItem, "ESub")).ToString("f",culture)+","+((decimal)DataBinder.Eval(Container.DataItem, "ETotal")).ToString("f",culture)+","+((decimal)DataBinder.Eval(Container.DataItem, "EParent")).ToString("f",culture)+")\">"+((decimal)DataBinder.Eval(Container.DataItem, "ETotal")).ToString("f",culture)+"</a>"
								: ((decimal)DataBinder.Eval(Container.DataItem, "ETotal")).ToString("f",culture)
							%>
						</itemtemplate>
					</asp:templatecolumn>
					<asp:templatecolumn>
						<HeaderStyle cssclass="ibn-vh2 headstyle1" width="70px"></HeaderStyle>
						<ItemStyle horizontalalign="center" width="70px" cssclass="ibn-vb2 pp"></ItemStyle>
						<itemtemplate>
							<%# ((decimal)DataBinder.Eval(Container.DataItem, "ATotal")).ToString("f",culture)%>
						</itemtemplate>
					</asp:templatecolumn>
					<asp:templatecolumn>
						<HeaderStyle cssclass="ibn-vh2 headstyle" width="100px"></HeaderStyle>
						<ItemStyle horizontalalign="left" width="100px" cssclass="ibn-vb2 pp"></ItemStyle>
						<itemtemplate>
							<%# "<a title=\""+LocRM.GetString("tNewAcc")+"\" id=\"NWL"+DataBinder.Eval(Container.DataItem, "AccountId").ToString()+"\" href=\"javascript:openNewMenu('NWL"+DataBinder.Eval(Container.DataItem, "AccountId").ToString()+"')\"><img src='../layouts/images/newitem.gif' align='absmiddle' width='16' height='16' border='0' /></a>"%>
							&nbsp;
							<%# (int)DataBinder.Eval(Container.DataItem, "OutlineLevel")>1 ? 
								"<a title=\""+LocRM.GetString("tMoveAcc")+"\" href=\"../Projects/MoveAccount.aspx?AccountId="+DataBinder.Eval(Container.DataItem, "AccountId").ToString()+"&ProjectId="+ProjectId.ToString()+"\"><img src='../layouts/images/moveto.gif' align='absmiddle' width='13' height='13' border='0' /></a>" 
								: ""
							%>
							&nbsp;
							<%# (int)DataBinder.Eval(Container.DataItem, "OutlineLevel")>1 ? 
								"<a title=\""+LocRM.GetString("tEditAcc")+"\" id=\"OWL"+DataBinder.Eval(Container.DataItem, "AccountId").ToString()+"\" href=\"javascript:openEditMenu('OWL"+DataBinder.Eval(Container.DataItem, "AccountId").ToString()+"','"+DataBinder.Eval(Container.DataItem, "Title").ToString()+"')\"><img src='../layouts/images/edit.gif' align='absmiddle' width='16' height='16' border='0' /></a>"
								: ""
							%>
							&nbsp;
							<asp:imagebutton Visible='<%# ((int)DataBinder.Eval(Container.DataItem, "OutlineLevel")>1) %>' id="ibDelete" runat="server" borderwidth="0" 
								width="16" height="16" imageurl="../../layouts/images/delete.gif" 
								commandname="Delete" causesvalidation="False" ImageAlign="AbsMiddle" 
								CommandArgument='<%# DataBinder.Eval(Container.DataItem, "AccountId")%>'/>
						</itemtemplate>
					</asp:templatecolumn>
				</columns>
			</dg:DataGridExtended>
		</td>
	</tr>
</table>
<div id="divEditValue" style="position:absolute; top:30px;left:100px; width:190px; z-index:255;padding:5px;display:none; border: 1px solid #95b7f3;" class="ibn-rtetoolbarmenu ibn-propertysheet ibn-selectedtitle">
	<FIELDSET style="HEIGHT: 150px;margin:0;padding:2px">
		<LEGEND class="text ibn-legend-default" id="lgdBlock" runat="server"></LEGEND>
		<table cellpadding="3" cellspacing="0" border="0" width="100%">
			<tr class="text">
				<td width="70px"><%= LocRM.GetString("tCurrent")%>:</td>
				<td style="padding-left:6px">
					<asp:Label ID="lblCur" Runat=server Width="75px" CssClass="text"></asp:Label>
				</td>
			</tr>
			<tr class="text">
				<td ><%= LocRM.GetString("tSub")%>:</td>
				<td style="border-bottom: #cecece 1px solid; padding-left:6px">
					<asp:Label ID="lblSub" Runat=server Width="75px" CssClass="text"></asp:Label>
				</td>
			</tr>
			<tr class="text">
				<td><b><%= LocRM.GetString("tSum")%>:</b></td>
				<td>
					<asp:TextBox ID="txtSum" Runat=server Width="60px" CssClass="text"></asp:TextBox>
					<asp:CompareValidator ControlToValidate="txtSum" ControlToCompare="lowBorder" CssClass="text" ErrorMessage="*" Display=Dynamic ID="cvSum1" Runat=server Type=Currency Operator="GreaterThanEqual"></asp:CompareValidator>
					<asp:CompareValidator ControlToValidate="txtSum" ControlToCompare="highBorder" CssClass="text" ErrorMessage="*" Display=Dynamic ID="cvSum2" Runat=server Type=Currency Operator="LessThanEqual"></asp:CompareValidator>
				</td>
			</tr>
			<tr>
				<td colspan="2">
					<asp:CheckBox onclick="EnableDisableVal(this)" ID="cbSaveParent" Runat=server CssClass="text"></asp:CheckBox>
					<br>
					<center>
						<asp:Label ID="lblHelp" Runat=server Font-Size="7" CssClass="ibn-alerttext"></asp:Label>
					</center>
				</td>
			</tr>
			<tr height="35px" valign=bottom>
				<td colspan=2 style="padding-left:15px">
					<asp:Button ID="btnUpdateAcc" Width="70px" Runat="server" CssClass="text"></asp:Button>
					<input type=button value='<%=LocRM.GetString("tClose")%>' class="text" style="width:70px" onclick='javascript:closeMenu()' />
				</td>
			</tr>
		</table>
	</fieldset>
</div>
<div id="divEditItem" style="position:absolute; top:30px;left:100px; width:180px; z-index:255;padding:5px;display:none; border: 1px solid #95b7f3;" class="ibn-rtetoolbarmenu ibn-propertysheet ibn-selectedtitle">
	<FIELDSET style="HEIGHT: 85px;margin:0;padding:2px">
		<LEGEND class="text ibn-legend-default" id="lgdEditItem" runat="server"></LEGEND>
		<table cellpadding="3" cellspacing="0" border="0" width="100%">
			<tr>
				<td class="text"><b><%= LocRM.GetString("tTitle")%>:</b></td>
			</tr>
			<tr>
				<td>
					<asp:TextBox ID="txtAccountTitle" Runat=server Width="140px" CssClass="text"></asp:TextBox>
					<asp:RequiredFieldValidator ID="rfAcTitle" ControlToValidate="txtAccountTitle" Runat=server CssClass="text" ErrorMessage="*" Display=Dynamic></asp:RequiredFieldValidator>
				</td>
			</tr>
			<tr valign=bottom>
				<td style="padding-left:10px">
					<asp:Button ID="btnUpdAccName" Runat=server Width="70px" CssClass="text"></asp:Button>
					<input type=button class="text" value='<%=LocRM.GetString("tClose")%>' style="width:70px" onclick='javascript:closeMenu()' />
				</td>
			</tr>
		</table>
	</FIELDSET>
</div>
<input id="hdnCollapseExpand" type="hidden" runat="server" />
<input id="hdnAccountId" type="hidden" runat="server" />
<asp:TextBox id="lowBorder" style="display:none" runat="server"></asp:TextBox>
<asp:TextBox id="highBorder" style="display:none" runat="server"></asp:TextBox>
<asp:LinkButton id="lbCollapseExpandAcc" runat="server" Visible="False" onclick="Collapse_Expand_Click"></asp:LinkButton>