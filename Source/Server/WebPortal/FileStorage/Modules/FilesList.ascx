<%@ Control Language="c#" Inherits="Mediachase.UI.Web.FileStorage.Modules.FilesList" Codebehind="FilesList.ascx.cs" %>
<%@ Reference Control="~/Apps/Common/DateTimePickerAjax/PickerControl.ascx" %>
<%@ Register TagPrefix="mc" TagName="Picker" Src="~/Apps/Common/DateTimePickerAjax/PickerControl.ascx" %>
<%@ Register TagPrefix="dg" namespace="Mediachase.UI.Web.Modules.DGExtension" Assembly="Mediachase.UI.Web" %>
<script type="text/javascript">
	function ViewFile(CName, CKey, FileId)
	{
		ShowWizard('<%= ResolveUrl("~/FileStorage/FileInfoView.aspx")%>' + '?FileId=' + FileId + '&ContainerKey=' + CKey + '&ContainerName=' + CName, 650, 310);
	}
	
	function ChangeModify(obj)
	{
		objTbl = document.getElementById('<%=tableDate.ClientID %>');
		id=obj.value;
		if(id=="9")
		{
			objTbl.style.display = 'block';
		}
		else
		{
			objTbl.style.display = 'none';
		}
	}
</script>
<table cellspacing="0" cellpadding="0" border="0" width="100%">
	<tr id="trFilter" runat="server">
		<td>
			<table runat="server" id="FilterTable" class="ibn-alternating ibn-navline text" cellspacing="0" cellpadding="4" width="100%" border="0">
				<tr height="30px">
					<td style="PADDING-LEFT: 15px;" width="115px" class="text"><b><%=LocRM.GetString("tContentType")%>:</b></td>
					<td width="210px"><asp:DropDownList ID="ddContType" Runat="server" CssClass="text" Width="200px"></asp:DropDownList>
					</td>
					<td align="left"></td>
					<td align="right" valign="top">
						<asp:LinkButton ID="lbHideFilter" Runat="server" CssClass="text" onclick="lbHideFilter_Click"></asp:LinkButton>
					</td>
				</tr>
				<tr height="55px">
					<td width="115px" class="text" style="PADDING-LEFT: 15px"><b><%=LocRM.GetString("tPeriod")%>:</b>&nbsp;</td>
					<td valign="middle" width="210px"><select class="text" id="ddPeriod" style="WIDTH: 200px" onchange="ChangeModify(this);" runat="server" NAME="ddPeriod">
						</select></td>
					<td align="left">
						<table id="tableDate" cellspacing="2" cellpadding="0" runat="server">
							<tr>
								<td class="text"><b><%=LocRM.GetString("tFrom")%>:</b>&nbsp;</td>
								<td><mc:Picker ID="dtcStartDate" runat="server" DateCssClass="text" DateWidth="85px" ShowImageButton="false" DateIsRequired="true" /></td>
								<td class="text">&nbsp;<b><%=LocRM.GetString("tTo")%>:</b>&nbsp;</td>
								<td><mc:Picker ID="dtcEndDate" runat="server" DateCssClass="text" DateWidth="85px" ShowImageButton="false" DateIsRequired="true" /></td>
							</tr>
						</table>
					</td>
					<td valign="bottom" align="right">
						<nobr><input class="text" id="btnApply" type="submit" value="Apply" name="Submit" runat="server" style="WIDTH: 80px" onserverclick="Apply_ServerClick" />&nbsp;&nbsp;
						<input class="text" id="btnReset" type="submit" value="Reset" name="Submit2" runat="server" style="WIDTH: 80px" onserverclick="Reset_ServerClick" /></nobr>
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
									<asp:Label Runat="server" ID="lblFilterNotSet" style="color: #666666;" CssClass="text"></asp:Label>
									<asp:LinkButton ID="lbShowFilter" Runat="server" CssClass="text" onclick="lbShowFilter_Click"></asp:LinkButton>
								</td>
							</tr>
							<tr>
								<td valign="bottom" style="padding-top:10px">
									<input class="text" id="btnReset2" type="submit" value="Reset" runat="server" NAME="btnReset2" onserverclick="Reset_ServerClick" />		
								</td>
							</tr>
						</table>
					</td>
				</tr>
			</table>
		</td>
	</tr>
	<tr>
		<td style="padding: 10px 5px 5px 10px;">
			<dg:datagridextended id="grdMain" runat="server" allowsorting="True" allowpaging="True" width="100%"
					autogeneratecolumns="False" borderwidth="0" gridlines="None" cellpadding="1"	PageSize="10"
					LayoutFixed="True" EnableViewState="false">
				<columns>
					<asp:boundcolumn visible="false" datafield="Id"></asp:boundcolumn>
					<asp:boundcolumn visible="false" datafield="ContainerKey"></asp:boundcolumn>
					<asp:boundcolumn visible="false" datafield="ParentFolderId"></asp:boundcolumn>
					<asp:templatecolumn headerstyle-cssclass="ibn-vh2" itemstyle-cssclass="ibn-vb2" headerstyle-width="21" itemstyle-width="21">
						<itemtemplate>
							<img alt="" src='<%# Eval("Icon")%>' width="16" height="16" />
						</itemtemplate>
					</asp:templatecolumn>
					<asp:templatecolumn headerstyle-cssclass="ibn-vh2" itemstyle-cssclass="ibn-vb2" sortexpression="sortName">
						<itemtemplate>
							<%# GetLink(Eval("ContainerKey").ToString(), (int)Eval("Id"), Eval("Name").ToString(), Eval("ContentType").ToString(), (int)Eval("ParentFolderId"))%>
						</itemtemplate>
					</asp:templatecolumn>
					<asp:templatecolumn headerstyle-cssclass="ibn-vh2" itemstyle-cssclass="ibn-vb2" sortexpression="ContainerKey">
						<itemtemplate></itemtemplate>
					</asp:templatecolumn>
					<asp:boundcolumn headerstyle-cssclass="ibn-vh2" itemstyle-cssclass="ibn-vb2" datafield="Modifier"
						headerstyle-width="150" itemstyle-width="150" sortexpression="sortModifier"></asp:boundcolumn>
					<asp:boundcolumn headerstyle-cssclass="ibn-vh2" itemstyle-cssclass="ibn-vb2" datafield="ModifiedDate"
						headerstyle-width="95" itemstyle-width="95" sortexpression="sortModified"></asp:boundcolumn>
					<asp:boundcolumn headerstyle-cssclass="ibn-vh2" itemstyle-cssclass="ibn-vb2" datafield="Size"
						sortexpression="sortSize" headerstyle-width="70" itemstyle-width="70"></asp:boundcolumn>
					<asp:boundcolumn headerstyle-cssclass="ibn-vh2" itemstyle-cssclass="ibn-vb2" datafield="ActionVS"
						headerstyle-width="25" itemstyle-width="25"></asp:boundcolumn>
					<asp:templatecolumn headerstyle-cssclass="ibn-vh2" itemstyle-cssclass="ibn-vb2" headerstyle-width="25"
						itemstyle-width="25">
						<itemtemplate>
							<%# CanUserEdit((int)Eval("Id"), Eval("ContainerKey").ToString(), (int)Eval("ParentFolderId")) %>
						</itemtemplate>
					</asp:templatecolumn>
					<asp:templatecolumn headerstyle-cssclass="ibn-vh2" itemstyle-cssclass="ibn-vb2" headerstyle-width="25"
						itemstyle-width="25">
						<itemtemplate>
							<asp:ImageButton Visible='<%# CanDelete(Eval("ContainerKey").ToString(),(int)Eval("ParentFolderId"))%>' ID="ibDelete" Runat=server CommandName="Delete" ImageUrl="~/layouts/Images/Delete.gif"></asp:ImageButton>
						</itemtemplate>
					</asp:templatecolumn>
				</columns>
			</dg:datagridextended>
			
			<dg:datagridextended id="grdDetails" runat="server" allowsorting="True" allowpaging="True" width="100%"
					autogeneratecolumns="False" borderwidth="0" gridlines="None" cellpadding="1"	PageSize="10"
					LayoutFixed="True" EnableViewState="false">
				<columns>
					<asp:boundcolumn visible="false" datafield="Id"></asp:boundcolumn>
					<asp:boundcolumn visible="false" datafield="ContainerKey"></asp:boundcolumn>
					<asp:boundcolumn visible="false" datafield="ParentFolderId"></asp:boundcolumn>
					<asp:templatecolumn headerstyle-cssclass="ibn-vh2" itemstyle-cssclass="ibn-vb2" headerstyle-width="21"
						itemstyle-width="21">
						<itemtemplate>
							<img alt="" src='<%# Eval("Icon")%>' width="16" height="16" />
						</itemtemplate>
					</asp:templatecolumn>
					<asp:TemplateColumn>
						<HeaderStyle CssClass="ibn-vh2" />
						<ItemStyle CssClass="ibn-vb2" />
						<HeaderTemplate>
							<table border="0" cellpadding="2" cellspacing="0" width="100%" class="ibn-propertysheet" style="table-layout:fixed;">
								<tr>
									<td><asp:LinkButton Runat="server" CommandName="Sort" CommandArgument="sortName" Text = <%# LocRM.GetString("tName") %>></asp:LinkButton></td>
									<td style="width:200px;"><asp:LinkButton Runat="server" CommandName="Sort" CommandArgument="ContainerKey" Text = <%# LocRM.GetString("tLocation") %>></asp:LinkButton></td>
									<td style="width:150px;"><asp:LinkButton Runat="server" CommandName="Sort" CommandArgument="sortModifier" Text = <%# LocRM.GetString("UpdatedBy") %>></asp:LinkButton></td>
									<td style="width:95px;"><asp:LinkButton Runat="server" CommandName="Sort" CommandArgument="sortModified" Text = <%# LocRM.GetString("UpdatedDate") %>></asp:LinkButton></td>
									<td style="width:70px;"><asp:LinkButton Runat="server" CommandName="Sort" CommandArgument="sortSize" Text =<%# LocRM.GetString("Size") %>></asp:LinkButton></td>
								</tr>
								<tr>
									<td colspan="5" style="color:#808080;"><%# LocRM2.GetString("tDescription")%></td>
								</tr>
							</table>
						</HeaderTemplate>
						<ItemTemplate>
							<table border="0" cellpadding="2" cellspacing="0" width="100%" class="ibn-propertysheet" style="table-layout:fixed;">
								<tr>
									<td valign="top"><%# GetLink((string)Eval("ContainerKey"), (int)Eval("Id"), Eval("Name").ToString(), Eval("ContentType").ToString(), (int)Eval("ParentFolderId"))%></td>
									<td style="width:200px;" valign="top"><asp:Label id="lblLocation" runat="server"></asp:Label></td>
									<td style="width:150px;" valign="top"><%# Eval("Modifier")%></td>
									<td style="width:95px;" valign="top"><%# Eval("ModifiedDate")%></td>
									<td style="width:70px;" valign="top"><%# Eval("Size")%></td>
								</tr>
								<tr>
									<td colspan="5" style="color: #555555;"><%# Eval("Description")%></td>
								</tr>
							</table>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:boundcolumn headerstyle-cssclass="ibn-vh2" itemstyle-cssclass="ibn-vb2" datafield="ActionVS"
						headerstyle-width="25" itemstyle-width="25"></asp:boundcolumn>
					<asp:templatecolumn headerstyle-cssclass="ibn-vh2" itemstyle-cssclass="ibn-vb2" headerstyle-width="25"
						itemstyle-width="25">
						<itemtemplate>
							<%# CanUserEdit((int)Eval("Id"), Eval("ContainerKey").ToString(), (int)Eval("ParentFolderId")) %>
						</itemtemplate>
					</asp:templatecolumn>
					<asp:templatecolumn headerstyle-cssclass="ibn-vh2" itemstyle-cssclass="ibn-vb2" headerstyle-width="25"
						itemstyle-width="25">
						<itemtemplate>
							<asp:ImageButton Visible='<%# CanDelete(Eval("ContainerKey").ToString(), (int)Eval("ParentFolderId"))%>' ID="ibDelete2" Runat=server CommandName="Delete" ImageUrl="~/layouts/Images/Delete.gif"></asp:ImageButton>
						</itemtemplate>
					</asp:templatecolumn>
				</columns>
			</dg:datagridextended>
		</td>
	</tr>
</table>
<asp:LinkButton ID="lbChangeViewTable" Runat="server" Visible="False"></asp:LinkButton>
<asp:LinkButton ID="lbChangeViewDet" Runat="server" Visible="False"></asp:LinkButton>