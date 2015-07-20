<%@ Reference Control="~/Modules/BlockHeaderLightWithMenu.ascx" %>
<%@ Reference Control="~/Modules/PageViewMenu.ascx" %>
<%@ Reference Control="~/Apps/MetaUIEntity/Modules/EntityDropDown.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Documents.Modules.DocumentList" Codebehind="DocumentList.ascx.cs" %>
<%@ register TagPrefix="dg" namespace="Mediachase.UI.Web.Modules.DGExtension" Assembly="Mediachase.UI.Web" %>
<%@ Register TagPrefix="ibn" TagName="EntityDD" src="~/Apps/MetaUIEntity/Modules/EntityDropDown.ascx" %>
<%@ Import Namespace="Mediachase.Ibn.Data" %>
<style type="text/css">
	.cellstyle {font-family: verdana;vertical-align: center;height:23px;border-bottom: 1px solid #e0e0e0}
	.headstyle {padding-top:5px;padding-bottom:5px}
</style>
<script type="text/javascript">
<!--
function DeleteDocument(documentId)
{
	document.forms[0].<%=hdnDocumentId.ClientID %>.value = documentId;
	if(confirm('<%=LocRM.GetString("Warning") %>'))
		<%=Page.ClientScript.GetPostBackEventReference(lblDeleteDocumentAll,"") %>
}

function CollapseExpand(CEType, ProjectId, evt)
{
	evt = (evt) ? evt : ((window.event) ? event : null);
	if (evt)
	{
		var s = (evt.target) ? evt.target : ((evt.srcElement) ? evt.srcElement : null);
		if (s.toString().toLowerCase().indexOf("http://")==0) return;
	}
	document.forms[0].<%=hdnDocumentId.ClientID %>.value = ProjectId;
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
		document.forms[0].<%=hdnDocumentId.ClientID %>.value = ContactUid;
		document.forms[0].<%=hdnColType.ClientID %>.value = "contact";
	}
	else if(OrgUid != empty)
	{
		document.forms[0].<%=hdnDocumentId.ClientID %>.value = OrgUid;
		document.forms[0].<%=hdnColType.ClientID %>.value = "org";
	}
	else
	{
		document.forms[0].<%=hdnDocumentId.ClientID %>.value = empty;
		document.forms[0].<%=hdnColType.ClientID %>.value = "noclient";
	}
	document.forms[0].<%=hdnCollapseExpand.ClientID %>.value = CEType;
	<%=Page.ClientScript.GetPostBackEventReference(lbCollapseExpandPrj,"") %>
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
		<td valign="top">
			<table cellspacing="3" cellpadding="0" border="0" runat="server" id="tblFilterInfoSet" class="text">
			</table>
		</td>
		<td valign="bottom" align="right" height="100%">
			<table height="100%" cellspacing="0" cellpadding="0" style="margin-top:-5px">
				<tr>
					<td valign="top" align="right">
						<asp:Label Runat="server" ID="lblFilterNotSet" style="color:#666666" CssClass="text"></asp:Label>
						<asp:LinkButton ID="lbShowFilter" Runat=server CssClass=text onclick="lbShowFilter_Click"></asp:LinkButton>
					</td>
				</tr>
				<tr>
					<td valign="bottom" style="padding-top:10px">
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
			<table cellpadding="0" cellspacing="0" border="0" width="100%">
				<tr height="30px">
					<td id="tdProject" width="50%" runat="server">
						<table cellspacing="0" width="100%" border="0">
							<tr>
								<td class="text" width="85px"><%=LocRM.GetString("Project") %>:</td>
								<td align="left">
									<asp:dropdownlist id="ddlProject" runat="server" CssClass="Text" Width="95%"></asp:dropdownlist>
								</td>
							</tr>
						</table>
					</td>
					<td width="50%">
						<table cellspacing="0" width="100%" border="0">
							<tr>
								<td class="text" width="85px"><%=LocRM.GetString("Status") %>:</td>
								<td align="left"><asp:dropdownlist id="ddStatus" runat="server" width="95%" cssclass="text"></asp:dropdownlist></td>
							</tr>
						</table>
					</td>
				</tr>
				<tr height="30px">
					<td width="50%">
						<table cellspacing="0" width="100%" border="0">
							<tr>
								<td class="text" align="left" width="85px"><%=LocRM.GetString("Manager") %>:
								</td>
								<td align="left"><asp:dropdownlist id="ddManager" runat="server" width="95%" cssclass="text"></asp:dropdownlist></td>
							</tr>
						</table>
					</td>
					<td width="50%">
						<table cellspacing="0" width="100%" border="0" id="tblPriority" runat="server">
							<tr>
								<td class="text" width="85px"><%=LocRM.GetString("Priority") %>:</td>
								<td align="left"><asp:dropdownlist id="ddPriority" runat="server" width="95%" cssclass="text"></asp:dropdownlist></td>
							</tr>
						</table>
					</td>
				</tr>
				<tr height="30px">
					<td align="left" width="50%">
						<table cellspacing="0" width="100%" border="0">
							<tr>
								<td class="text" align="left" width="85px"><%=LocRM.GetString("Keyword") %>:
								</td>
								<td align="left"><asp:textbox id="tbKeyword" runat="server" style="width:95%;" cssclass="text"></asp:textbox></td>
							</tr>
						</table>
					</td>
					<td width="50%">
						<table cellspacing="0" width="100%" border="0" id="tblClient" runat="server">
							<tr>
								<td class="text" width="85px"><%=LocRM2.GetString("Client")%>:</td>
								<td align="left"><ibn:EntityDD id="ClientControl" ObjectTypes="Contact,Organization" runat="server" Width="95%"/></td>
							</tr>
						</table>
					</td>
				</tr>
			</table>
		</td>
		<td valign="top" style="width:30%;" runat="server" id="tdCategories">
			<%=LocRM.GetString("Category") %>:<br />
			<asp:DropDownList ID="ddGenCatType" Runat="server" CssClass="Text" onchange="ShowListBox(this)"></asp:DropDownList><br />
			<asp:ListBox ID="lbGenCats" Runat="server" CssClass="text" Rows="4" Width="95%" SelectionMode="Multiple"></asp:ListBox>
		</td>
		<td valign="top" style="margin-top:-5px; width:20px;" align="right">
			<asp:LinkButton ID="lbHideFilter" Runat="server" CssClass="text" onclick="lbHideFilter_Click"></asp:LinkButton>
		</td>
	</tr>
	<tr>
		<td colspan="3" align="right">
			<nobr><input class="text" id="btnApply" type="submit" value="Apply" name="Submit" runat="server" onserverclick="btnApply_ServerClick" />&nbsp;&nbsp;
			<input class="text" id="btnReset" type="submit" value="Reset" name="Submit2" runat="server" onserverclick="btnReset_ServerClick" /></nobr>
		</td>
	</tr>
</table>
<dg:datagridextended id="dgDocuments" runat="server" width="100%" autogeneratecolumns="False" borderwidth="1px" CellSpacing="0" gridlines="Horizontal" cellpadding="0" allowsorting="True" pagesize="10" allowpaging="True" EnableViewState=false bordercolor="#afc9ef" HeaderStyle-BackColor="#F0F8FF">
	<Columns>
		<asp:BoundColumn Visible="False" DataField="DocumentId"></asp:BoundColumn>
		<asp:TemplateColumn ItemStyle-Width="18" HeaderStyle-Width=18 ItemStyle-cssclass="ibn-vb2" >
			<ItemTemplate><%# GetPriority (
					(int)DataBinder.Eval(Container.DataItem, "PriorityId"),
					(string)DataBinder.Eval(Container.DataItem, "PriorityName")
					)
					%>
			</ItemTemplate>
		</asp:TemplateColumn>
		<asp:TemplateColumn SortExpression="Title" HeaderText="Title" ItemStyle-cssclass="ibn-vb3" >
			<HeaderTemplate>
				<table border="0" cellpadding="0" cellspacing="0" width="100%" class="ibn-propertysheet" style="BORDER-left: #afc9ef 1px solid;BORDER-right: #afc9ef 1px solid">
					<tr>
						<td colspan="6" >
							<asp:LinkButton Runat="server" CommandName="Sort" CommandArgument="Title" Text = <%# LocRM.GetString("Title") %> ID="Linkbutton1" NAME="Linkbutton1"></asp:LinkButton>
						</td>
					</tr>
					<tr>
						<td>
							<table width="100%" border="0" cellpadding="2" cellspacing="0" class="ibn-propertysheet">
								<tr>
									<td><asp:LinkButton Runat="server" CommandName="Sort" CommandArgument="PriorityId" Text = <%#  LocRM.GetString("Priority") %> ID="Linkbutton2" NAME="Linkbutton2"></asp:LinkButton></td>
									<td width="90"><asp:LinkButton Runat="server" CommandName="Sort" CommandArgument="StatusId" Text =<%# LocRM.GetString("Status") %> ID="Linkbutton3" NAME="Linkbutton3"></asp:LinkButton></td>
									<td width="100"><asp:LinkButton Runat="server" CommandName="Sort" CommandArgument="CreationDate" Text =<%# LocRM.GetString("CreatedDate") %> ID="Linkbutton4" NAME="Linkbutton4"></asp:LinkButton></td>
									<td width="160"><asp:LinkButton Runat="server" CommandName="Sort" CommandArgument="ManagerName" Text =<%# LocRM.GetString("Manager")%> ID="Linkbutton7" NAME="Linkbutton7"></asp:LinkButton></td>
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
							<a href='../Documents/DocumentView.aspx?DocumentId=<%# DataBinder.Eval(Container.DataItem, "DocumentId")%>'><img alt="" width="16" height="16" src="../layouts/images/icons/document.gif" align="absmiddle" border="0" />&nbsp;<%# DataBinder.Eval(Container.DataItem, "Title")%></a>
						</td>
					</tr>
					<tr>
						<td>
							<table width="100%" border="0" cellpadding="2" cellspacing="0" class="ibn-styleheader">
								<tr>
									<td><%# DataBinder.Eval(Container.DataItem, "PriorityName")%></td>
									<td width="90"><%# DataBinder.Eval(Container.DataItem, "StatusName")%></td>
									<td width="100"><%# ((DateTime)DataBinder.Eval(Container.DataItem, "CreationDate")).ToString("d") %></td>
									<td width="160"><%# Mediachase.UI.Web.Util.CommonHelper.GetUserStatus((int)DataBinder.Eval(Container.DataItem, "ManagerId"))%></td>
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
				<asp:HyperLink ImageUrl = "../../layouts/images/Edit.GIF" NavigateUrl='<%# "~/Documents/DocumentEdit.aspx?DocumentID=" + DataBinder.Eval(Container.DataItem, "DocumentId").ToString() + "&ProjectId=" + ProjectID %>' Runat="server" ID="Hyperlink1" Visible='<%# GetBool((int)DataBinder.Eval(Container.DataItem, "CanEdit")) %>' ToolTip='<%#LocRM2.GetString("Edit")%>'>
				</asp:HyperLink>&nbsp;
				<asp:HyperLink id="ibDelete" runat="server" imageurl="../../layouts/images/DELETE.GIF" Visible='<%# GetBool((int)DataBinder.Eval(Container.DataItem, "CanDelete")) %>' NavigateUrl='<%# "javascript:DeleteDocument(" + DataBinder.Eval(Container.DataItem, "DocumentId").ToString() + ")" %>' ToolTip='<%#LocRM2.GetString("Delete")%>' >
				</asp:HyperLink>
						&nbsp;
			</ItemTemplate>
		</asp:TemplateColumn>
	</Columns>
</dg:datagridextended>
<dg:datagridextended id="dgGroupDocs" runat="server" width="100%" autogeneratecolumns="False" borderwidth="0px" CellSpacing="0" gridlines="None" cellpadding="0" allowsorting="True" pagesize="10" allowpaging="True" EnableViewState=false>
	<Columns>
		<asp:BoundColumn Visible="False" DataField="DocumentId"></asp:BoundColumn>
		<asp:BoundColumn Visible="False" DataField="ProjectId"></asp:BoundColumn>
		<asp:TemplateColumn>
			<itemstyle cssclass="ibn-vb2"></itemstyle>
			<headerstyle cssclass="ibn-vh2 headstyle"></headerstyle>
			<ItemTemplate>
				<%# 
					GetTitle((bool)DataBinder.Eval(Container.DataItem, "IsProject"),
					(bool)DataBinder.Eval(Container.DataItem, "IsCollapsed"),
					(int)DataBinder.Eval(Container.DataItem, "ProjectId"),
					(int)DataBinder.Eval(Container.DataItem, "DocumentId"),
					DataBinder.Eval(Container.DataItem, "Title").ToString())
				%>
			</ItemTemplate>
		</asp:TemplateColumn>
		<asp:TemplateColumn>
			<itemstyle cssclass="ibn-vb2" width="100px"></itemstyle>
			<headerstyle cssclass="ibn-vh2 headstyle" width="100px"></headerstyle>
			<ItemTemplate>
				<%# 
					(bool)DataBinder.Eval(Container.DataItem, "IsProject") ? "" :
					DataBinder.Eval(Container.DataItem, "PriorityName")
				%>
			</ItemTemplate>
		</asp:TemplateColumn>
		<asp:TemplateColumn>
			<itemstyle cssclass="ibn-vb2"></itemstyle>
			<headerstyle cssclass="ibn-vh2 headstyle" width="100px"></headerstyle>
			<ItemTemplate>
				<%# 
					(bool)DataBinder.Eval(Container.DataItem, "IsProject") ? "" :
					DataBinder.Eval(Container.DataItem, "StatusName")
				%>
			</ItemTemplate>
		</asp:TemplateColumn>
		<asp:TemplateColumn>
			<itemstyle cssclass="ibn-vb2" width="120px"></itemstyle>
			<headerstyle cssclass="ibn-vh2 headstyle" width="120px"></headerstyle>
			<ItemTemplate>
				<%# (bool)DataBinder.Eval(Container.DataItem, "IsProject") ?"" :
					((DateTime)DataBinder.Eval(Container.DataItem, "CreationDate")).ToShortDateString()
				%>
			</ItemTemplate>
		</asp:TemplateColumn>
		<asp:TemplateColumn>
			<headerstyle cssclass="ibn-vh2 headstyle" width="170px"></headerstyle>
			<itemstyle cssclass="ibn-vb2"></itemstyle>
			<ItemTemplate>
				<%# 
					(bool)DataBinder.Eval(Container.DataItem, "IsProject") ? "" :
					Mediachase.UI.Web.Util.CommonHelper.GetUserStatus((int)DataBinder.Eval(Container.DataItem, "ManagerId"))
				%>
			</ItemTemplate>
		</asp:TemplateColumn>
		<asp:TemplateColumn itemstyle-width="60">
				<headerstyle horizontalalign="Right" cssclass="ibn-vh-right headstyle" width="60px"></headerstyle>
				<itemstyle horizontalalign="Right" cssclass="ibn-vb2" width="60px"></itemstyle>
			<ItemTemplate>
				<%# 
					GetOptionsString((bool)DataBinder.Eval(Container.DataItem, "IsProject"),
									(int)DataBinder.Eval(Container.DataItem, "DocumentId"),
									(int)DataBinder.Eval(Container.DataItem, "CanEdit"),
									(int)DataBinder.Eval(Container.DataItem, "CanDelete"))
				%>
			</ItemTemplate>
		</asp:TemplateColumn>
	</Columns>
</dg:datagridextended>

<dg:datagridextended id="dgGroupDocsByClient" runat="server" width="100%" autogeneratecolumns="False" borderwidth="0px" CellSpacing="0" gridlines="None" cellpadding="0" allowsorting="True" pagesize="10" allowpaging="True" EnableViewState=false>
	<Columns>
		<asp:BoundColumn Visible="False" DataField="DocumentId"></asp:BoundColumn>
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
						(int)Eval("DocumentId"),
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
			<itemstyle cssclass="cellstyle" width="100"></itemstyle>
			<headerstyle cssclass="ibn-vh2 headstyle" width="100"></headerstyle>
			<ItemTemplate>
				<%# 
					(bool)DataBinder.Eval(Container.DataItem, "IsClient") ? "" :
					DataBinder.Eval(Container.DataItem, "StatusName")
				%>
			</ItemTemplate>
		</asp:TemplateColumn>
		<asp:TemplateColumn>
			<itemstyle cssclass="ibn-vb2" width="120px"></itemstyle>
			<headerstyle cssclass="ibn-vh2 headstyle" width="120px"></headerstyle>
			<ItemTemplate>
				<%# (bool)DataBinder.Eval(Container.DataItem, "IsClient") ?"" :
					((DateTime)DataBinder.Eval(Container.DataItem, "CreationDate")).ToShortDateString()
				%>
			</ItemTemplate>
		</asp:TemplateColumn>
		<asp:TemplateColumn>
			<headerstyle cssclass="ibn-vh2 headstyle" width="170px"></headerstyle>
			<itemstyle cssclass="ibn-vb2"></itemstyle>
			<ItemTemplate>
				<%# 
					(bool)DataBinder.Eval(Container.DataItem, "IsClient") ? "" :
					Mediachase.UI.Web.Util.CommonHelper.GetUserStatus((int)DataBinder.Eval(Container.DataItem, "ManagerId"))
				%>
			</ItemTemplate>
		</asp:TemplateColumn>
		<asp:TemplateColumn itemstyle-width="60">
				<headerstyle horizontalalign="Right" cssclass="ibn-vh-right headstyle" width="60px"></headerstyle>
				<itemstyle horizontalalign="Right" cssclass="ibn-vb2" width="60px"></itemstyle>
			<ItemTemplate>
				<%# 
					GetOptionsString((bool)DataBinder.Eval(Container.DataItem, "IsClient"),
									(int)DataBinder.Eval(Container.DataItem, "DocumentId"),
									(int)DataBinder.Eval(Container.DataItem, "CanEdit"),
									(int)DataBinder.Eval(Container.DataItem, "CanDelete"))
				%>
			</ItemTemplate>
		</asp:TemplateColumn>
	</Columns>
</dg:datagridextended>

<asp:DataGrid ID="dgExport" Runat="server" AutoGenerateColumns="False" AllowPaging="False" 
	AllowSorting="False" EnableViewState="False" Visible="False" ItemStyle-HorizontalAlign="Left" 
	HeaderStyle-Font-Bold="True">
	<Columns>
		<asp:BoundColumn DataField="Title"></asp:BoundColumn>
		<asp:BoundColumn DataField="PriorityName"></asp:BoundColumn>
		<asp:BoundColumn DataField="StatusName"></asp:BoundColumn>
		<asp:BoundColumn DataField="CreationDate" DataFormatString="{0:yyyy-MM-dd}"></asp:BoundColumn>
		<asp:BoundColumn DataField="ManagerName"></asp:BoundColumn>
	</Columns>
</asp:DataGrid>
<input id="hdnDocumentId" type="hidden" runat="server" />
<input id="hdnColType" type="hidden" runat="server" />
<input id="hdnCollapseExpand" type="hidden" runat="server" />
<asp:linkbutton id="lbDeleteDocument" runat="server" Visible="False"></asp:linkbutton><asp:linkbutton id="lblDeleteDocumentAll" runat="server" Visible="False"></asp:linkbutton>
<asp:LinkButton id="lbCollapseExpandPrj" runat="server" Visible="False" onclick="Collapse_Expand_Click"></asp:LinkButton>
<asp:LinkButton id="lbShowGroup" runat="server" Visible="False" onclick="lbShowGroup_Click"></asp:LinkButton>