<%@ Control Language="c#" Inherits="Mediachase.UI.Web.ToDo.Modules.ToDoList" Codebehind="ToDoList.ascx.cs" %>
<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Reference Control="~/Apps/MetaUIEntity/Modules/EntityDropDown.ascx" %>
<%@ register TagPrefix="dg" namespace="Mediachase.UI.Web.Modules.DGExtension" Assembly="Mediachase.UI.Web" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="~/Modules/BlockHeader.ascx"%>
<%@ Register TagPrefix="ibn" TagName="EntityDD" src="~/Apps/MetaUIEntity/Modules/EntityDropDown.ascx" %>
<%@ Import Namespace="Mediachase.Ibn.Data" %>
<style type="text/css">
	.cellstyle {font-family: verdana;font-size: .68em;vertical-align: center;height:23px;border-bottom: 1px solid #e0e0e0}
	.headstyle {padding-top:5px;padding-bottom:5px}
</style>
<script type="text/javascript">
<!--
function DeleteToDo(toDoId)
{
	document.forms[0].<%=hdnToDoId.ClientID %>.value = toDoId;
	if(confirm('<%=LocRM.GetString("Warning") %>'))
		<%=Page.ClientScript.GetPostBackEventReference(lbDeleteToDoAll,"") %>
}

function CollapseExpand(CEType, ContactUid, OrgUid, evt)
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
		document.forms[0].<%=hdnToDoId.ClientID %>.value = ContactUid;
		document.forms[0].<%=hdnColType.ClientID %>.value = "contact";
	}
	else if(OrgUid != empty)
	{
		document.forms[0].<%=hdnToDoId.ClientID %>.value = OrgUid;
		document.forms[0].<%=hdnColType.ClientID %>.value = "org";
	}
	else
	{
		document.forms[0].<%=hdnToDoId.ClientID %>.value = empty;
		document.forms[0].<%=hdnColType.ClientID %>.value = "noclient";
	}
	document.forms[0].<%=hdnCollapseExpand.ClientID %>.value = CEType;
	<%=Page.ClientScript.GetPostBackEventReference(lbCollapseExpand,"") %>
}

function ShowListBox(obj)
{
	var objGenCatType = document.forms[0].<%=ddGenCatType.ClientID%>;
	var objLBGen = document.forms[0].<%=lbGenCats.ClientID%>;
	
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
}
//-->
</script>
<table class="ibn-stylebox2" cellspacing="0" cellpadding="0" width="100%" border="0">
	<tr>
		<td>
			<ibn:blockheader id="secHeader" runat="server"></ibn:blockheader>
		</td>
	</tr>
	<tr>
		<td>
			<table id="tblFilterInfo" runat="server" class="ibn-navline text" cellspacing="0" cellpadding="4" width="100%" border="0">
				<tr>
					<td valign="top">
						<table cellspacing="3" cellpadding="0" border="0" runat="server" id="tblFilterInfoSet" class="text">
						</table>
					</td>
					<td valign="bottom" align="right" height="100%">
						<table height="100%" cellspacing="0" cellpadding="0" style="margin-top:-5">
							<tr>
								<td valign="top" align="right">
									<asp:Label Runat="server" ID="lblFilterNotSet" style="color:#666666" CssClass="text"></asp:Label>
									<asp:LinkButton ID="lbShowFilter" Runat=server CssClass=text onclick="lbShowFilter_Click"></asp:LinkButton>
								</td>
							</tr>
							<tr>
								<td valign=bottom style="padding-top:10px">
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
							<tr height="30px">
								<td width="240px">
									<table cellspacing="0" width="100%" border="0">
										<tr>
											<td class="text" width="85px"><%=LocRM.GetString("Project") %>:</td>
											<td align="left">
												<asp:dropdownlist id="ddlProject" runat="server" CssClass="Text" Width="135px"></asp:dropdownlist>
											</td>
										</tr>
									</table>
								</td>
								<td width="230px">
									<table cellspacing="0" width="100%" border="0">
										<tr>
											<td class="text" width="85px"><%=LocRM.GetString("Priority") %>:</td>
											<td align="left"><asp:dropdownlist id="ddPriority" runat="server" width="135" cssclass="text"></asp:dropdownlist></td>
										</tr>
									</table>
								</td>
							</tr>
							<tr height="30px">
								<td width="240px">
									<table cellspacing="0" width="100%" border="0">
										<tr>
											<td class="text" align="left" width="85px"><%=LocRM.GetString("Manager") %>:
											</td>
											<td align="left"><asp:dropdownlist id="ddManager" runat="server" width="135px" cssclass="text"></asp:dropdownlist></td>
										</tr>
									</table>
								</td>
								<td width="230px">
									<table cellspacing="0" width="100%" border="0">
										<tr>
											<td class="text" align="left" width="85px"><%=LocRM.GetString("Keyword") %>:
											</td>
											<td align="left"><asp:textbox id="tbKeyword" runat="server" width="135" cssclass="text"></asp:textbox></td>
										</tr>
									</table>
								</td>
							</tr>
						</table>
					</td>
					<td valign="top" align="right">
						<table cellpadding="4" cellspacing="0" border="0" class="text">
							<tr valign="top">
								<td width="125" valign="top" style="padding-top:6"><%=LocRM.GetString("Category") %>:</td>
								<td valign="top">
									<asp:DropDownList ID="ddGenCatType" Width="160" Runat="server" CssClass="Text" onchange="ShowListBox(this)"></asp:DropDownList><br />
									<asp:ListBox ID="lbGenCats" Runat="server" CssClass="text" Rows="4" Width="160" SelectionMode="Multiple"></asp:ListBox>
								</td>
							</tr>
							<tr>
								<td class="text"><%=LocRM2.GetString("Client")%>:</td>
								<td><ibn:EntityDD id="ClientControl" ObjectTypes="Contact,Organization" runat="server" Width="160px"/></td>
							</tr>
						</table>
					</td>
					<td height="100%">
						<table width="100%" height="100%" cellspacing="0" cellpadding="0" style="margin-top:-5">
							<tr>
								<td align="right" valign="top">
									<asp:LinkButton ID="lbHideFilter" Runat="server" CssClass="text" onclick="lbHideFilter_Click"></asp:LinkButton>
								</td>
							</tr>
							<tr>
								<td valign="bottom" align="right">
									<nobr><input class="text" id="btnApply" type="submit" value="Apply" runat="server" onserverclick="btnApply_ServerClick" />&nbsp;&nbsp;
									<input class="text" id="btnReset" type="submit" value="Reset" runat="server" onserverclick="btnReset_ServerClick" /></nobr>
								</td>
							</tr>
						</table>
					</td>
				</tr>
			</table>
			<dg:datagridextended id="dgGroupToDoByClient" runat="server" width="100%" autogeneratecolumns="False" 
				borderwidth="0px" CellSpacing="0" gridlines="None" cellpadding="0" allowsorting="True" 
				pagesize="10" allowpaging="True" EnableViewState="false">
				<Columns>
					<asp:BoundColumn Visible="False" DataField="ToDoId"></asp:BoundColumn>
					<asp:BoundColumn Visible="False" DataField="ContactUid"></asp:BoundColumn>
					<asp:BoundColumn Visible="False" DataField="OrgUid"></asp:BoundColumn>
					<asp:TemplateColumn>
						<itemstyle cssclass="cellstyle"></itemstyle>
						<headerstyle cssclass="ibn-vh2 headstyle"></headerstyle>
						<ItemTemplate>
							<%# 
								GetTitleClient((bool)Eval("IsClient"),
									(bool)Eval("IsCollapsed"),
									PrimaryKeyId.Parse(Eval("ContactUid").ToString()),
									PrimaryKeyId.Parse(Eval("OrgUid").ToString()),
									Eval("ClientName").ToString(),
									(int)Eval("ToDoId"),
									Eval("Title").ToString())
							%>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn>
						<itemstyle cssclass="cellstyle" width="100px"></itemstyle>
						<headerstyle cssclass="ibn-vh2 headstyle" width="100px"></headerstyle>
						<ItemTemplate>
							<%# 
								(bool)DataBinder.Eval(Container.DataItem, "IsClient") ? "" :
								DataBinder.Eval(Container.DataItem, "PriorityName")
							%>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn>
						<itemstyle cssclass="cellstyle" width="120px"></itemstyle>
						<headerstyle cssclass="ibn-vh2 headstyle" width="120px"></headerstyle>
						<ItemTemplate>
							<%# (bool)DataBinder.Eval(Container.DataItem, "IsClient") ?"" :
								((DateTime)DataBinder.Eval(Container.DataItem, "CreationDate")).ToShortDateString()
							%>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn>
						<headerstyle cssclass="ibn-vh2 headstyle" width="170px"></headerstyle>
						<itemstyle cssclass="cellstyle"></itemstyle>
						<ItemTemplate>
							<%# 
								(bool)DataBinder.Eval(Container.DataItem, "IsClient") ? "" :
								Mediachase.UI.Web.Util.CommonHelper.GetUserStatus((int)DataBinder.Eval(Container.DataItem, "ManagerId"))
							%>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn itemstyle-width="60">
							<headerstyle horizontalalign="Right" cssclass="ibn-vh-right headstyle" width="60px"></headerstyle>
							<itemstyle horizontalalign="Right" cssclass="cellstyle" width="60px"></itemstyle>
						<ItemTemplate>
							<%# 
								GetOptionsString((bool)DataBinder.Eval(Container.DataItem, "IsClient"),
												(int)DataBinder.Eval(Container.DataItem, "TodoId"),
												(int)DataBinder.Eval(Container.DataItem, "CanEdit"),
												(int)DataBinder.Eval(Container.DataItem, "CanDelete"))
							%>
						</ItemTemplate>
					</asp:TemplateColumn>
				</Columns>
			</dg:datagridextended>
		</td>
	</tr>
</table>
<input id="hdnToDoId" type="hidden" runat="server" />
<input id="hdnColType" type="hidden" runat="server" />
<input id="hdnCollapseExpand" type="hidden" runat="server" />
<asp:linkbutton id="lbDeleteToDoAll" runat="server" Visible="False"></asp:linkbutton>
<asp:LinkButton id="lbCollapseExpand" runat="server" Visible="False" onclick="Collapse_Expand_Click"></asp:LinkButton>