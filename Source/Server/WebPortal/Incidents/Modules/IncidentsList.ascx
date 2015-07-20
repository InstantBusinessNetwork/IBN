<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Incidents.Modules.IncidentsList" Codebehind="IncidentsList.ascx.cs" %>
<%@ Reference Control="~/Apps/MetaUIEntity/Modules/EntityDropDown.ascx" %>
<%@ register TagPrefix="dg" namespace="Mediachase.UI.Web.Modules.DGExtension" Assembly="Mediachase.UI.Web" %>
<%@ Register TagPrefix="ibn" TagName="EntityDD" src="~/Apps/MetaUIEntity/Modules/EntityDropDown.ascx" %>
<%@ Import Namespace="Mediachase.Ibn.Data" %>
<style type="text/css">
	.cellstyle {font-family: verdana;font-size: .68em;vertical-align: middle;border-bottom: 1px solid #e0e0e0; padding-top:3px;padding-bottom:3px}
	.headstyle {padding-top:5px;padding-bottom:5px;border-bottom: 1px solid #e0e0e0; }
	.mousepointer{cursor: pointer;}
</style>
<script type="text/javascript">
<!--
function DeleteIncident(incidentId)
{
	document.forms[0].<%=hdnIncidentId.ClientID %>.value = incidentId;
	if(confirm('<%=LocRM.GetString("Warning") %>'))
		<%=Page.ClientScript.GetPostBackEventReference(lblDeleteIncidentAll,"") %>
}

function CollapseExpand(CEType, ProjectId, evt)
{
	evt = (evt) ? evt : ((window.event) ? event : null);
	if (evt)
	{
		var s = (evt.target) ? evt.target : ((evt.srcElement) ? evt.srcElement : null);
		if (s.toString().toLowerCase().indexOf("http://")==0) return;
	}
	document.forms[0].<%=hdnIncidentId.ClientID %>.value = ProjectId;
	document.forms[0].<%=hdnColType.ClientID %>.value = "prj";
	document.forms[0].<%=hdnCollapseExpand.ClientID %>.value = CEType;
	<%=Page.ClientScript.GetPostBackEventReference(lbCollapseExpandPrj,"") %>
}

function CollapseExpand2(CEType, ContactUid, OrgUid, evt)
{
	evt = (evt) ? evt : ((window.event) ? event : null);
	if (evt)
	{
		var s = (evt.target) ? evt.target : ((evt.srcElement) ? evt.srcElement : null);
		if (s.toString().toLowerCase().indexOf("http://")==0 ||
				s.toString().toLowerCase().indexOf("javascript:")==0) 
			return;
	}
	
	var empty = '<%=PrimaryKeyId.Empty.ToString()%>';
	if(ContactUid != empty)
	{
		document.forms[0].<%=hdnIncidentId.ClientID %>.value = ContactUid;
		document.forms[0].<%=hdnColType.ClientID %>.value = "contact";
	}
	else if(OrgUid != empty)
	{
		document.forms[0].<%=hdnIncidentId.ClientID %>.value = OrgUid;
		document.forms[0].<%=hdnColType.ClientID %>.value = "org";
	}
	else
	{
		document.forms[0].<%=hdnIncidentId.ClientID %>.value = empty;
		document.forms[0].<%=hdnColType.ClientID %>.value = "noclient";
	}
	document.forms[0].<%=hdnCollapseExpand.ClientID %>.value = CEType;
	<%=Page.ClientScript.GetPostBackEventReference(lbCollapseExpandPrj,"") %>
}

function ShowListBox(obj)
{
	var objGenCatType = document.forms[0].<%=ddGenCatType.ClientID%>;
	var objLBGen = document.forms[0].<%=lbGenCats.ClientID%>;
	var objIssCatType = document.forms[0].<%=ddIssCatType.ClientID%>;
	var objLBIss = document.forms[0].<%=lbIssCats.ClientID%>;
	
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
	else if(obj == objIssCatType)
	{
		var selValue = obj.value;
		if (selValue == "0")
			objLBIss.style.display = "none";
		else if (selValue == "1")
		{
			objLBIss.style.display = "block";
		}
		else if (selValue == "2")
		{
			objLBIss.style.display = "block";
		}
	}
}

function OpenWindow(query,w,h,scroll)
{
	var l = (screen.width - w) / 2;
	var t = (screen.height - h) / 2;
	
	winprops = 'resizable=0, height='+h+',width='+w+',top='+t+',left='+l;
	if (scroll) winprops+=',scrollbars=1';
	var f = window.open(query, "_blank", winprops);
}
//-->
</script>

<table id="tblGroupInfo" runat="server" class="ibn-navline text" cellspacing="0" cellpadding="4" width="100%" border="0">
	<tr>
		<td valign="top">
			<asp:Label Runat="server" ID="lblGroupInfo"></asp:Label>
		</td>
		<td valign="bottom" align="right">
			<input class="text" id="btnReset3" type="submit" value="Reset" runat="server" style="width:120px" onserverclick="btnResetGroup_ServerClick" />
		</td>
	</tr>
</table>
<table id="tblFilterInfo" runat="server" class="ibn-navline text" cellspacing="0" cellpadding="4" width="100%" border="0">
	<tr>
		<td valign="top" style="padding-bottom:5px">
			<table cellspacing="3" cellpadding="0" border="0" runat="server" id="tblFilterInfoSet" class="text">
			</table>
		</td>
		<td valign="top" align="right">
			<table cellspacing="0" cellpadding="0" style="margin-top:5px">
				<tr>
					<td valign="top" align="right">
						<asp:Label Runat="server" ID="lblFilterNotSet" style="color:#666666" CssClass="text"></asp:Label>
						<asp:LinkButton ID="lbShowFilter" Runat="server" CssClass="text" onclick="lbShowFilter_Click"></asp:LinkButton>
					</td>
				</tr>
				<tr>
					<td style="padding-top:7px">
						<input class="text" id="btnReset2" type="submit" value="Reset" runat="server" onserverclick="btnReset_ServerClick" />		
					</td>
				</tr>
			</table>
		</td>
	</tr>
</table>

<table runat="server" id="FilterTable" class="ibn-alternating ibn-navline" cellspacing="0" cellpadding="7" width="100%" border="0">
	<tr>
		<td valign="top">
			<table cellpadding="0" cellspacing="0" border="0">
				<tr height="30px" id="tdProject" runat="server">
					<td colspan="2">
						<table cellspacing="0" width="100%" border="0">
							<tr>
								<td class="text" width="95px"><asp:label id="lblProject" runat="server" CssClass="text"></asp:label>:</td>
								<td><asp:dropdownlist id="ddlProject" runat="server" CssClass="Text" Width="375px"></asp:dropdownlist></td>
							</tr>
						</table>
					</td>
				</tr>
				<tr height="30px">
					<td width="240px" id="tdCreatedBy" runat="server">
						<table cellspacing="0" width="100%" border="0">
							<tr>
								<td class="text" width="95px"><%=LocRM.GetString("CreatedBy") %>:</td>
								<td align="left"><asp:dropdownlist id="ddCreatedBy" runat="server" width="135" cssclass="text"></asp:dropdownlist></td>
							</tr>
						</table>
					</td>
					<td width="240px">
						<table cellspacing="0" width="100%" border="0">
							<tr>
								<td class="text" align="left" width="95px"><%=LocRM.GetString("Manager") %>:
								</td>
								<td align="left"><asp:dropdownlist id="ddManager" runat="server" width="135px" cssclass="text"></asp:dropdownlist></td>
							</tr>
						</table>
					</td>
				</tr>
				<tr height="30px">
					<td id="tdResponsible" width="240px" runat="server">
						<table cellspacing="0" width="100%" border="0">
							<tr>
								<td class="text" width="95px"><%=LocRM.GetString("tResponsible") %>:</td>
								<td align="left"><asp:dropdownlist id="ddResponsible" runat="server" width="135" cssclass="text"></asp:dropdownlist></td>
							</tr>
						</table>
					</td>
					<td width="240px">
						<table cellspacing="0" width="100%" border="0">
							<tr>
								<td class="text" align="left" width="95px"><%=LocRM.GetString("Type") %>:
								</td>
								<td align="left"><asp:dropdownlist id="ddType" runat="server" width="135" cssclass="text"></asp:dropdownlist></td>
							</tr>
						</table>
					</td>
				</tr>
				<tr height="30px">
					<td align="left" width="240px">
						<table cellspacing="0" width="100%" border="0">
							<tr>
								<td class="text" width="95px"><%=LocRM.GetString("Status") %>:</td>
								<td align="left"><asp:dropdownlist id="ddState" runat="server" width="135" cssclass="text"></asp:dropdownlist></td>
							</tr>
						</table>
					</td>
					<td width="240px" id="tdIssBox" runat="server">
						<table cellspacing="0" width="100%" border="0">
							<tr>
								<td class="text" align="left" width="95px"><%=LocRM.GetString("tIssBox") %>:
								</td>
								<td align="left"><asp:dropdownlist id="ddIssBox" runat="server" width="135" cssclass="text"></asp:dropdownlist></td>
							</tr>
						</table>
					</td>
				</tr>
				<tr height="30px">
					<td align="left" width="240px">
						<table cellspacing="0" width="100%" border="0">
							<tr>
								<td class="text" align="left" width="95px"><%=LocRM.GetString("tSeverity") %>:
								</td>
								<td align="left">
									<asp:DropDownList ID="ddSeverity" Runat="server" Width="135px" CssClass="text"></asp:DropDownList>
								</td>
							</tr>
						</table>
					</td>
					<td width="240px">
						<table cellspacing="0" width="100%" border="0" id="tblPriority" runat="server">
							<tr>
								<td class="text" width="95px"><%=LocRM.GetString("Priority") %>:</td>
								<td align="left"><asp:dropdownlist id="ddPriority" runat="server" width="135" cssclass="text"></asp:dropdownlist></td>
							</tr>
						</table>
					</td>
				</tr>
				<tr height="30px">
					<td align="left" width="240px">
						<table cellspacing="0" width="100%" border="0">
							<tr>
								<td class="text" align="left" width="95px"><%=LocRM.GetString("Keyword") %>:
								</td>
								<td align="left"><asp:textbox id="tbKeyword" runat="server" width="135" cssclass="text"></asp:textbox></td>
							</tr>
						</table>
					</td>
					<td width="240px">
						<table cellspacing="0" width="100%" border="0" id="tblClient" runat="server">
							<tr>
								<td class="text" align="left" width="95px"><%=LocRM.GetString("tClient") %>:
								</td>
								<td align="left"><ibn:EntityDD id="ClientControl" ObjectTypes="Contact,Organization" runat="server" Width="135px"/></td>
							</tr>
						</table>
					</td>
				</tr>
			</table>
		</td>
		<td valign="top">
			<table cellpadding="4" cellspacing="0" border="0">
				<tr>
					<td width="130px" class="text"><asp:Label ID="lblOnlyNew" Runat="server" CssClass="text"></asp:Label></td>
					<td valign="top">
						<asp:CheckBox ID="cbOnlyNewMess" Runat="server" CssClass="text"></asp:CheckBox>
					</td>
				</tr>
				<tr id="trCategories" runat="server">
					<td width="130px" valign="top">
						<asp:label id="lblGenCats" runat="server" CssClass=text></asp:label>
					</td>
					<td valign="top">
						<asp:DropDownList ID="ddGenCatType" Width="160px" Runat="server" CssClass="Text" onchange="ShowListBox(this)"></asp:DropDownList><br />
						<asp:ListBox ID="lbGenCats" Runat="server" CssClass="text" Rows="4" Width="160px" SelectionMode="Multiple"></asp:ListBox>
					</td>
				</tr>
				<tr>
					<td valign="top">
						<asp:label id="lblIssCats" runat="server" CssClass="text"></asp:label>
					</td>
					<td valign="top">
						<asp:DropDownList ID="ddIssCatType" Width="160px" Runat="server" CssClass="Text" onchange="ShowListBox(this)"></asp:DropDownList><br />
						<asp:ListBox ID="lbIssCats" Runat="server" CssClass="text" Rows="4" Width="160px" SelectionMode="Multiple"></asp:ListBox>
					</td>
				</tr>
			</table>
		</td>
		<td style="vertical-align:top;">
			<div style="text-align:right;vertical-align:top;">
				<asp:LinkButton ID="lbHideFilter" Runat="server" CssClass="text" onclick="lbHideFilter_Click"></asp:LinkButton>
			</div>
		</td>
	</tr>
	<tr>
		<td colspan="3" align="right">
			<nobr><input class="text" id="btnApply" type="submit" value="Apply" runat="server" onserverclick="btnApply_ServerClick" />&nbsp;&nbsp;
			<input class="text" id="btnReset" type="submit" value="Reset" runat="server" onserverclick="btnReset_ServerClick" /></nobr>
		</td>
	</tr>
</table>
<dg:datagridextended id="dgIncidents" runat="server" width="100%" autogeneratecolumns="False" 
	borderwidth="1px" CellSpacing="0" gridlines="Horizontal" cellpadding="0" allowsorting="True" 
	pagesize="10" allowpaging="True" EnableViewState="false" bordercolor="#afc9ef" 
	HeaderStyle-BackColor="#F0F8FF">
	<Columns>
		<asp:BoundColumn Visible="False" DataField="IncidentId"></asp:BoundColumn>
		<asp:TemplateColumn ItemStyle-Width="18" HeaderStyle-Width=18>
			<HeaderStyle cssclass="ibn-propertysheet"></HeaderStyle>
			<HeaderTemplate>
				<asp:LinkButton Runat=server CommandName="Sort" CommandArgument="PriorityId">
					<img alt="" title='<%# LocRM.GetString("Priority") %>' align="absmiddle" border="0" src='<%# ResolveClientUrl("~/layouts/images/flagorange.gif") %>' />
				</asp:LinkButton>
			</HeaderTemplate>
			<ItemTemplate>
				<%# GetTaskToDoStatus (
					(int)DataBinder.Eval(Container.DataItem, "PriorityId"),
					(string)DataBinder.Eval(Container.DataItem, "PriorityName")
					)
					%>
			</ItemTemplate>
		</asp:TemplateColumn>
		<asp:TemplateColumn SortExpression="Title" HeaderText="Title">
			<HeaderStyle></HeaderStyle>
			<HeaderTemplate>
				<table border="0" cellpadding="0" cellspacing="0" width="100%" class="ibn-propertysheet" style="BORDER-left: #afc9ef 1px solid;BORDER-right: #afc9ef 1px solid">
					<tr>
						<td colspan="6">
							<asp:LinkButton Runat=server CommandName="Sort" CommandArgument="Title" Text = <%# LocRM.GetString("Title") %>></asp:LinkButton>
						</td>
					</tr>
					<tr>
						<td>
							<table width="100%" border="0" cellpadding="2" cellspacing="0" class="ibn-propertysheet">
								<tr>
									<td><asp:LinkButton Runat="server" CommandName="Sort" CommandArgument="IncidentId" Text = <%# LocRM.GetString("tIssueNum") %>></asp:LinkButton></td>
									<td width="110px"><asp:LinkButton Runat="server" CommandName="Sort" CommandArgument="StateName" Text = <%# LocRM.GetString("Status") %>></asp:LinkButton></td>
									<td width="100px"><asp:LinkButton Runat="server" CommandName="Sort" CommandArgument="ClientName" Text = <%# LocRM.GetString("tClient") %>></asp:LinkButton></td>
									<td width="160px"><asp:LinkButton Runat="server" CommandName="Sort" CommandArgument="ResponsibleName" Text =<%# LocRM.GetString("tResponsible") %>></asp:LinkButton></td>
									<td width="100px"><asp:LinkButton Runat="server" CommandName="Sort" CommandArgument="ModifiedDate" Text =<%# LocRM.GetString("ModifiedDate") %>></asp:LinkButton></td>
									<td width="160px"><asp:LinkButton Runat="server" CommandName="Sort" CommandArgument="IncidentBoxName" Text =<%# LocRM.GetString("tIssBox")%>></asp:LinkButton></td>
								</tr>
							</table>
						</td>
					</tr>
				</table>
			</HeaderTemplate>
			<ItemTemplate>
				<table border="0" cellpadding="0" cellspacing="0" width="100%" class="ibn-propertysheet" style="BORDER-LEFT: #afc9ef 1px solid; BORDER-RIGHT: #afc9ef 1px solid">
					<tr>
						<td  style="FONT-SIZE: 11px;padding:2px">
							<%# GetTitleLink
								(
									(string)DataBinder.Eval(Container.DataItem, "Title"),
									(int)DataBinder.Eval(Container.DataItem, "IncidentId"),
									(int)DataBinder.Eval(Container.DataItem, "StateId"),
									(string)DataBinder.Eval(Container.DataItem, "StateName"),
									(bool)DataBinder.Eval(Container.DataItem, "IsNewMessage"),
									(bool)DataBinder.Eval(Container.DataItem, "IsOverdue")
								)
							%>
						</td>
					</tr>
					<tr>
						<td>
							<table width="100%" border="0" cellpadding="2" cellspacing="0" class="ibn-styleheader">
								<tr>
									<td>#<%# Eval("IncidentId")%>,&nbsp;&nbsp;<%#GetTicket((int)Eval("IncidentId"), (int)Eval("IncidentBoxId"), Eval("Identifier"))%></td>
									<td width="110px"><%# Eval("StateName")%><%# (bool)Eval("IsOverdue") ? ", " + GetGlobalResourceObject("IbnFramework.Incident", "Overdue").ToString() : ""%></td>
									<td width="100px"><%# GetClientLink(Eval("OrgUid"), Eval("ContactUid"), Eval("ClientName"))%></td>
									<td width="160px"><%# GetResponsibleLink(Eval("IncidentId"), Eval("StateId"),
															Eval("ResponsibleId"), Eval("ResponsibleName"),
															Eval("IsResponsibleGroup"), Eval("ResponsibleGroupState"),
															Eval("ManagerId"), Eval("ControllerId"))%></td>
									<td width="100px"><%# ((DateTime)Eval("ModifiedDate")).ToString("d")%></td>
									<td width="160px"><%# GetIssBoxLink(Eval("IncidentId"), Eval("IncidentBoxId"), Eval("IncidentBoxName"))%></td>
								</tr>
							</table>
						</td>
					</tr>
				</table>
			</ItemTemplate>
		</asp:TemplateColumn>
		<asp:TemplateColumn SortExpression="Title" HeaderText="Title">
			<HeaderStyle></HeaderStyle>
			<HeaderTemplate>
				<table border="0" cellpadding="0" cellspacing="0" width="100%" class="ibn-propertysheet" style="BORDER-left: #afc9ef 1px solid;BORDER-right: #afc9ef 1px solid">
					<tr>
						<td colspan="6">
							<asp:LinkButton Runat="server" CommandName="Sort" CommandArgument="Title" Text = <%# LocRM.GetString("Title") %>></asp:LinkButton>
						</td>
					</tr>
					<tr>
						<td>
							<table width="100%" border="0" cellpadding="2" cellspacing="0" class="ibn-propertysheet">
								<tr>
									<td><asp:LinkButton Runat="server" CommandName="Sort" CommandArgument="IncidentId" Text = <%# LocRM.GetString("tIssueNum") %>></asp:LinkButton></td>
									<td width="100px"><asp:LinkButton Runat="server" CommandName="Sort" CommandArgument="ActualOpenDate" Text = <%# LocRM.GetString("ActualOpenDate") %>></asp:LinkButton></td>
									<td width="100px"><asp:LinkButton Runat=server CommandName="Sort" CommandArgument="ActualFinishDate" Text =<%# LocRM.GetString("ActualFinishDate") %>></asp:LinkButton></td>
									<td width="100px"><asp:LinkButton Runat="server" CommandName="Sort" CommandArgument="ExpectedResponseDate" Text = <%# LocRM.GetString("ExpectedResponseDate") %>></asp:LinkButton></td>
									<td width="100px"><asp:LinkButton Runat=server CommandName="Sort" CommandArgument="ExpectedResolveDate" Text =<%# LocRM.GetString("ExpectedResolveDate") %>></asp:LinkButton></td>
								</tr>
							</table>
						</td>
					</tr>
				</table>
			</HeaderTemplate>
			<ItemTemplate>
				<table border="0" cellpadding="0" cellspacing="0" width="100%" class="ibn-propertysheet" style="BORDER-LEFT: #afc9ef 1px solid; BORDER-RIGHT: #afc9ef 1px solid">
					<tr>
						<td  style="FONT-SIZE: 11px;padding:2px">
							<%# GetTitleLink
								(
									(string)DataBinder.Eval(Container.DataItem, "Title"),
									(int)DataBinder.Eval(Container.DataItem, "IncidentId"),
									(int)DataBinder.Eval(Container.DataItem, "StateId"),
									(string)DataBinder.Eval(Container.DataItem, "StateName"),
									(bool)DataBinder.Eval(Container.DataItem, "IsNewMessage"),
									(bool)DataBinder.Eval(Container.DataItem, "IsOverdue")
								)
							%>
						</td>
					</tr>
					<tr>
						<td>
							<table width="100%" border="0" cellpadding="2" cellspacing="0" class="ibn-styleheader">
								<tr>
									<td>#<%# DataBinder.Eval(Container.DataItem, "IncidentId")%>,&nbsp;&nbsp;<%#GetTicket((int)DataBinder.Eval(Container.DataItem, "IncidentId"), (int)DataBinder.Eval(Container.DataItem, "IncidentBoxId"), DataBinder.Eval(Container.DataItem, "Identifier"))%></td>
									<td width="100px"><%# (DataBinder.Eval(Container.DataItem, "ActualOpenDate") == DBNull.Value)? "" : ((DateTime)DataBinder.Eval(Container.DataItem, "ActualOpenDate")).ToString("d")%></td>
									<td width="100px"><%# (DataBinder.Eval(Container.DataItem, "ActualFinishDate") == DBNull.Value) ? "" : ((DateTime)DataBinder.Eval(Container.DataItem, "ActualFinishDate")).ToString("d")%></td>
									<td width="100px"><%# (DataBinder.Eval(Container.DataItem, "ExpectedResponseDate") == DBNull.Value) ? "" : ((DateTime)DataBinder.Eval(Container.DataItem, "ExpectedResponseDate")).ToString("d")%></td>
									<td width="100px"><%# (DataBinder.Eval(Container.DataItem, "ExpectedResolveDate") == DBNull.Value) ? "" : ((DateTime)DataBinder.Eval(Container.DataItem, "ExpectedResolveDate")).ToString("d")%></td>
								</tr>
							</table>
						</td>
					</tr>
				</table>
			</ItemTemplate>
		</asp:TemplateColumn>
		<asp:TemplateColumn itemstyle-width="60">
			<headerstyle horizontalalign="Right" cssclass="ibn-vh-right" width="60px"></headerstyle>
			<itemstyle horizontalalign="Right" cssclass="ibn-vb2" width="60px"></itemstyle>
			<ItemTemplate>
				<asp:HyperLink ImageUrl = "../../layouts/images/Edit.GIF" NavigateUrl='<%# "~/Incidents/IncidentEdit.aspx?IncidentID=" + DataBinder.Eval(Container.DataItem, "IncidentId").ToString() + "&ProjectId=" + ProjectID.ToString() %>' Runat="server" ID="Hyperlink1" Visible='<%# CanIncidentEdit((int)DataBinder.Eval(Container.DataItem, "IncidentId")) %>' ToolTip='<%#LocRM.GetString("tEdit")%>' >
				</asp:HyperLink>&nbsp;
				<asp:HyperLink id="ibDelete" runat="server" imageurl="../../layouts/images/DELETE.GIF" Visible='<%# CanIncidentDelete((int)DataBinder.Eval(Container.DataItem, "IncidentId")) %>' NavigateUrl='<%# "javascript:DeleteIncident(" + DataBinder.Eval(Container.DataItem, "IncidentId").ToString() + ")" %>' ToolTip='<%#LocRM.GetString("tDelete")%>' >
				</asp:HyperLink>&nbsp;
			</ItemTemplate>
		</asp:TemplateColumn>
	</Columns>
</dg:datagridextended>
<dg:datagridextended id="dgGroupIncs" runat="server" width="100%" autogeneratecolumns="False" borderwidth="0px" CellSpacing="0" gridlines="None" cellpadding="0" allowsorting="True" pagesize="10" allowpaging="True" EnableViewState=false>
	<Columns>
		<asp:BoundColumn Visible="False" DataField="IncidentId"></asp:BoundColumn>
		<asp:BoundColumn Visible="False" DataField="ProjectId"></asp:BoundColumn>
		<asp:TemplateColumn>
			<itemstyle cssclass="cellstyle"></itemstyle>
			<headerstyle cssclass="ibn-vh2 headstyle"></headerstyle>
			<ItemTemplate>
				<%# 
					GetTitle
					(
						(bool)DataBinder.Eval(Container.DataItem, "IsProject"),
						(bool)DataBinder.Eval(Container.DataItem, "IsCollapsed"),
						(int)DataBinder.Eval(Container.DataItem, "ProjectId"),
						(int)DataBinder.Eval(Container.DataItem, "IncidentId"),
						DataBinder.Eval(Container.DataItem, "Title").ToString(),
						(int)DataBinder.Eval(Container.DataItem, "StateId"),
						DataBinder.Eval(Container.DataItem, "StateName").ToString(),
						(bool)DataBinder.Eval(Container.DataItem, "IsOverdue")
					)
				%>
			</ItemTemplate>
		</asp:TemplateColumn>
		<asp:TemplateColumn>
			<itemstyle cssclass="cellstyle" width="120px"></itemstyle>
			<headerstyle cssclass="ibn-vh2 headstyle" width="120px"></headerstyle>
			<ItemTemplate>
				<%# 
					(bool)Eval("IsProject") ? "" :
					GetClientLink(Eval("OrgUid"),
						Eval("ContactUid"),
						Eval("ClientName"))
				%>
			</ItemTemplate>
		</asp:TemplateColumn>
		<asp:TemplateColumn>
			<itemstyle cssclass="cellstyle"></itemstyle>
			<headerstyle cssclass="ibn-vh2 headstyle"></headerstyle>
			<ItemTemplate>
				<%# 
					(bool)DataBinder.Eval(Container.DataItem, "IsProject") ? "" :
					GetResponsibleLink(DataBinder.Eval(Container.DataItem, "IncidentId"),DataBinder.Eval(Container.DataItem, "StateId"),
										DataBinder.Eval(Container.DataItem, "ResponsibleId"),DataBinder.Eval(Container.DataItem, "ResponsibleName"),
										DataBinder.Eval(Container.DataItem, "IsResponsibleGroup"),DataBinder.Eval(Container.DataItem, "ResponsibleGroupState"),
										DataBinder.Eval(Container.DataItem, "ManagerId"),DataBinder.Eval(Container.DataItem, "ControllerId"))
				%>
			</ItemTemplate>
		</asp:TemplateColumn>
		<asp:TemplateColumn>
			<headerstyle cssclass="ibn-vh2 headstyle"></headerstyle>
			<itemstyle cssclass="cellstyle"></itemstyle>
			<ItemTemplate>
				<%# 
					(bool)DataBinder.Eval(Container.DataItem, "IsProject") ? "" :
					GetIssBoxLink(DataBinder.Eval(Container.DataItem, "IncidentId"),DataBinder.Eval(Container.DataItem, "IncidentBoxId"), DataBinder.Eval(Container.DataItem, "IncidentBoxName"))
				%>
			</ItemTemplate>
		</asp:TemplateColumn>
		<asp:TemplateColumn itemstyle-width="60px">
				<headerstyle horizontalalign="Right" cssclass="ibn-vh-right headstyle" width="60px"></headerstyle>
				<itemstyle horizontalalign="Right" cssclass="cellstyle" width="60px"></itemstyle>
			<ItemTemplate>
				<%# 
					GetOptionsString((bool)DataBinder.Eval(Container.DataItem, "IsProject"),
									(int)DataBinder.Eval(Container.DataItem, "IncidentId"))
				%>
			</ItemTemplate>
		</asp:TemplateColumn>
	</Columns>
</dg:datagridextended>
<dg:datagridextended id="dgGroupIncsByClient" runat="server" 
	width="100%" autogeneratecolumns="False" borderwidth="0px" CellSpacing="0" 
	gridlines="None" cellpadding="0" allowsorting="True" pagesize="10" 
	allowpaging="True" EnableViewState="false">
	<Columns>
		<asp:BoundColumn Visible="False" DataField="IncidentId"></asp:BoundColumn>
		<asp:BoundColumn Visible="False" DataField="ContactUid"></asp:BoundColumn>
		<asp:BoundColumn Visible="False" DataField="OrgUid"></asp:BoundColumn>
		<asp:TemplateColumn>
			<itemstyle cssclass="cellstyle"></itemstyle>
			<headerstyle cssclass="ibn-vh2 headstyle"></headerstyle>
			<ItemTemplate>
				<%# 
					GetTitleClient
					(
						(bool)Eval("IsClient"),
						(bool)Eval("IsCollapsed"),
						PrimaryKeyId.Parse(Eval("ContactUid").ToString()),
						PrimaryKeyId.Parse(Eval("OrgUid").ToString()),
						Eval("ClientName").ToString(),
						(int)Eval("IncidentId"),
						Eval("Title").ToString(),
						(int)Eval("StateId"),
						Eval("StateName").ToString(),
						(bool)Eval("IsOverdue")
					)
				%>
			</ItemTemplate>
		</asp:TemplateColumn>
		<asp:TemplateColumn>
			<itemstyle cssclass="cellstyle" width="120px"></itemstyle>
			<headerstyle cssclass="ibn-vh2 headstyle" width="120px"></headerstyle>
			<ItemTemplate>
				<%# 
					(bool)Eval("IsClient") ? "" :
					GetClientLink(Eval("OrgUid"),
						Eval("ContactUid"),
						Eval("ClientName"))
				%>
			</ItemTemplate>
		</asp:TemplateColumn>
		<asp:TemplateColumn>
			<itemstyle cssclass="cellstyle"></itemstyle>
			<headerstyle cssclass="ibn-vh2 headstyle"></headerstyle>
			<ItemTemplate>
				<%# 
					(bool)DataBinder.Eval(Container.DataItem, "IsClient") ? "" :
					GetResponsibleLink(DataBinder.Eval(Container.DataItem, "IncidentId"),DataBinder.Eval(Container.DataItem, "StateId"),
										DataBinder.Eval(Container.DataItem, "ResponsibleId"),DataBinder.Eval(Container.DataItem, "ResponsibleName"),
										DataBinder.Eval(Container.DataItem, "IsResponsibleGroup"),DataBinder.Eval(Container.DataItem, "ResponsibleGroupState"),
										DataBinder.Eval(Container.DataItem, "ManagerId"),DataBinder.Eval(Container.DataItem, "ControllerId"))
				%>
			</ItemTemplate>
		</asp:TemplateColumn>
		<asp:TemplateColumn>
			<headerstyle cssclass="ibn-vh2 headstyle"></headerstyle>
			<itemstyle cssclass="cellstyle"></itemstyle>
			<ItemTemplate>
				<%# 
					(bool)DataBinder.Eval(Container.DataItem, "IsClient") ? "" :
					GetIssBoxLink(DataBinder.Eval(Container.DataItem, "IncidentId"),DataBinder.Eval(Container.DataItem, "IncidentBoxId"), DataBinder.Eval(Container.DataItem, "IncidentBoxName"))
				%>
			</ItemTemplate>
		</asp:TemplateColumn>
		<asp:TemplateColumn itemstyle-width="60">
				<headerstyle horizontalalign="Right" cssclass="ibn-vh-right headstyle" width="60px"></headerstyle>
				<itemstyle horizontalalign="Right" cssclass="cellstyle" width="60px"></itemstyle>
			<ItemTemplate>
				<%# 
					GetOptionsString((bool)DataBinder.Eval(Container.DataItem, "IsClient"),
									(int)DataBinder.Eval(Container.DataItem, "IncidentId"))
				%>
			</ItemTemplate>
		</asp:TemplateColumn>
	</Columns>
</dg:datagridextended>
<asp:DataGrid ID="dgExport" Runat="server" AutoGenerateColumns="False" AllowPaging="False" 
	AllowSorting="False" EnableViewState="False" Visible="False" 
	ItemStyle-HorizontalAlign="Left" HeaderStyle-Font-Bold="True">
	<Columns>
		<asp:BoundColumn DataField="Title"></asp:BoundColumn>
		<asp:BoundColumn DataField="PriorityName"></asp:BoundColumn>
		<asp:TemplateColumn>
			<ItemTemplate>
				<%# Eval("StateName")%><%# (bool)Eval("IsOverdue") ? ", " + GetGlobalResourceObject("IbnFramework.Incident", "Overdue").ToString() : ""%>
			</ItemTemplate>
		</asp:TemplateColumn>
		<asp:BoundColumn DataField="CreationDate" DataFormatString="{0:yyyy-MM-dd}"></asp:BoundColumn>
		<asp:BoundColumn DataField="CreatorName"></asp:BoundColumn>
		<asp:BoundColumn DataField="ManagerName"></asp:BoundColumn>
	</Columns>
</asp:DataGrid>
<input id="hdnIncidentId" type="hidden" runat="server" />
<input id="hdnColType" type="hidden" runat="server" />
<input id="hdnCollapseExpand" type="hidden" runat="server" />
<asp:linkbutton id="lblDeleteIncidentAll" runat="server" Visible="False" onclick="lblDeleteIncidentAll_Click"></asp:linkbutton>
<asp:LinkButton id="lbCollapseExpandPrj" runat="server" Visible="False" onclick="Collapse_Expand_Click"></asp:LinkButton>
<asp:LinkButton id="lbShowGroup" runat="server" Visible="False" onclick="lbShowGroup_Click"></asp:LinkButton>
<asp:LinkButton ID="lbChangeViewDef" runat="server" Visible="false"></asp:LinkButton>
<asp:LinkButton ID="lbChangeViewDates" runat="server" Visible="false"></asp:LinkButton>