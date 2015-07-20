<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="IncidentListNew.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.HelpDeskManagement.Modules.IncidentListNew" %>
<%@ Reference Control="~/Apps/Common/Design/BlockHeader2.ascx" %>
<%@ Reference Control="~/Apps/MetaUI/Toolbar/MetaToolbar.ascx" %>
<%@ Reference Control="~/Apps/HelpDeskManagement/Modules/MCGrid.ascx" %>
<%@ Register Assembly="Mediachase.Ibn.Web.UI.WebControls" Namespace="Mediachase.Ibn.Web.UI.WebControls" TagPrefix="mc2" %>
<%@ Register TagPrefix="mc" TagName="BlockHeader2" Src="~/Apps/Common/Design/BlockHeader2.ascx" %>
<%@ Register TagPrefix="mc" TagName="MCGrid" Src="~/Apps/HelpDeskManagement/Modules/MCGrid.ascx" %>
<%@ Register TagPrefix="mc" TagName="MCGridAction" Src="~/Apps/MetaUI/Grid/MetaGridServerEventAction.ascx" %>
<%@ Register TagPrefix="mc" TagName="MetaToolbar" Src="~/Apps/MetaUI/Toolbar/MetaToolbar.ascx" %>
<link rel="stylesheet" type="text/css" href='<%= Mediachase.Ibn.Web.UI.WebControls.McScriptLoader.Current.GetScriptUrl("~/Styles/IbnFramework/grid.css", this.Page) %>' />
<script type="text/javascript">
var resizeFlag = false;

function LayoutResizeHandler(sender, eventArgs)
{
	var obj = document.getElementById('divLeftContainer');
	var txtGroupDiv = document.getElementById('<%= divLeftSearchContainer.ClientID%>');
	var objPanel = document.getElementById('divLeftPanel');

	if(obj && objPanel)
	{
		var iheight = obj.offsetHeight - 10;
		if(txtGroupDiv && txtGroupDiv.offsetHeight)
			iheight = iheight - txtGroupDiv.offsetHeight;
		objPanel.style.height = iheight + "px";
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

function checkKey(e) 
{ 
	var _key = e.keyCode ? e.keyCode : e.which ? e.which : e.charCode;
	try {
		if (_key == 13)
			<%= Page.ClientScript.GetPostBackEventReference(btnSearch, "") %>
		else
			return true;
	}
	catch (e) {return true;}
}
function OnChangeView(obj)
{
	var hField = document.getElementById('<%=hfFilterValue.ClientID %>');
	if(obj.value=="-1" || obj.value=="-2")
	{
		obj.value = hField.value;
	}
	else if(obj.value == "0")
	{
		obj.value = hField.value;
		eval("<%=AddNewViewScript %>");
	}
	else
	{
		<%=Page.ClientScript.GetPostBackClientHyperlink(lbViewChange, "") %>
	}
}

function GetSelectedIds()
{
	var obj = $find('<%= grdMain.GridClientContainerId %>');
	if(obj && obj.isCheckboxes() && obj.isChecked())
		return obj.getCheckedCollection();
	return "";
}
</script>
<style type="text/css">
	.hundred
	{
		height: 100%;
		width: 100%;
		position: absolute;
	}
	.innerArea
	{
		background-color: #D6E8FF;
		border-top:1px solid #A6A6A6;
		border-left: 1px solid #6B92CE;
		border-right: 1px solid #6B92CE;
		border-bottom: 1px solid #6B92CE;
	}
	.innerContentArea
	{
		background-color: White;
		border:1px solid #BBD4F6;
	}
</style>

<mc2:McDock ID="DockTop" runat="server" Anchor="top" EnableSplitter="False" DefaultSize="101">
	<DockItems>
		<table cellspacing="0" cellpadding="0" border="0" width="100%" class="filter">
			<tr>
				<td class="bottomBorder">
					<mc:BlockHeader2 runat="server" ID="BlockHeaderMain" />
				</td>
			</tr>
			<tr>
				<td style="padding:0px;">
					<asp:UpdatePanel ID="upFilters" runat="server" UpdateMode="Conditional">
						<ContentTemplate>
							<table cellpadding="5" cellspacing="0" width="100%" border="0" class="ibn-propertysheet" style="table-layout:fixed;">
								<colgroup>
									<col width="320px" />
									<col width="21px" />
									<col width="24px" />
									<col align="right" />
								</colgroup>
								<tr>
									<td width="320px" style="padding:5px 5px 0px 12px;">
										<asp:TextBox ID="txtSearch" runat="server" Width="300px" CssClass="text" onkeypress="return checkKey(event);"></asp:TextBox>			
									</td>
									<td width="21px" style="padding:5px 5px 0px 0px;"><asp:ImageButton Runat="server" id="btnSearch" Width="16" Height="16" ImageUrl="~/layouts/images/search.gif" ImageAlign="AbsMiddle" OnClick="btnSearch_Click" /></td>
									<td width="24px"style="padding:5px 5px 0px 0px;"><asp:ImageButton runat="server" ID="btnClear" Width="19" Height="17" ImageUrl="~/Layouts/Images/reset17.gif" ImageAlign="AbsMiddle" OnClick="btnClear_Click" /></td>
									<td align="right" style="padding:5px 5px 0px 0px;">
										<nobr><%=GetGlobalResourceObject("IbnFramework.Incident", "ViewFieldSet").ToString()%>: <mc2:IndentedDropDownList ID="ddFilters" runat="server" Width="260px" onchange="OnChangeView(this)"></mc2:IndentedDropDownList>&nbsp;
										<asp:ImageButton ID="ibEditInfo" runat="server" ImageUrl="~/Layouts/Images/edit.gif" ImageAlign="Top" Width="16px" Height="16px" />
										<asp:ImageButton ID="ibDeleteInfo" runat="server" OnClick="ibDeleteInfo_Click" ImageUrl="~/Layouts/Images/delete.gif" ImageAlign="Top" Width="16px" Height="16px" />&nbsp;</nobr>
									</td>
								</tr>
								<tr>
									<td style="padding:6px 0px 6px 12px;" colspan="4">
										<div id="spanFilters" runat="server" style="height:12px;padding:3px;overflow:hidden;font-size:smaller;">
											<nobr><asp:Label ID="FilterIsSet" runat="server"></asp:Label></nobr>
										</div>
									</td>
								</tr>
							</table>
						</ContentTemplate>
					</asp:UpdatePanel>
				</td>
			</tr>
			<tr>
				<td style="padding-left: 5px; padding-right: 5px;">
					<div class="noBottomBorder">
						<mc:MetaToolbar runat="server" ID="MainMetaToolbar" GridId="grdMain" />
					</div>		
				</td>
			</tr>	
		</table>
	</DockItems>
</mc2:McDock>
	
<mc2:McDock ID="DockLeft" InnerCssClass="hundred" runat="server" Anchor="Left" EnableSplitter="false" DefaultSize="220">
	<DockItems>
		<div style="height: 100%; width: 100%; position: relative;" class="filter ibn-propertysheet">
			<div style="position: absolute; left:5px; bottom: 3px; top: 0px; right:0px;" class="innerArea">
				<div style="padding-top:12px; text-align:center;">
					<asp:DropDownList ID="ddGrouping" runat="server" Width="190px" AutoPostBack="true" OnSelectedIndexChanged="ddGrouping_SelectedIndexChanged"></asp:DropDownList>
				</div>
				<div style="position:absolute;left:9px;right:9px;top:40px;bottom:9px;" class="innerContentArea" id="divLeftContainer">
					<div style="padding:5px 1px 5px 5px;">
<%--						<asp:UpdateProgress runat="server" ID="upLeftMenu" AssociatedUpdatePanelID="upLeftArea" DynamicLayout="false" DisplayAfter="1">
							<ProgressTemplate>
								<div style="z-index: 10000; position: absolute; height: 100%; width: 190px; background: Transparent url('../../../Images/IbnFramework/gray_trans.png') repeat;"></div>
							</ProgressTemplate>
						</asp:UpdateProgress>					
--%>						<asp:UpdatePanel ID="upLeftArea" runat="server" UpdateMode="Conditional">
							<ContentTemplate>
								<div style="padding:3px 0px 5px 3px;" id="divLeftSearchContainer" runat="server">
									<asp:TextBox ID="txtGroupSearch" runat="server" Width="160px" CssClass="text"></asp:TextBox>
									<asp:ImageButton Runat="server" id="ibGroupSearch" Width="16" Height="16" ImageUrl="~/layouts/images/search.gif" ImageAlign="AbsMiddle" OnClick="ibGroupSearch_Click" />
									<asp:ImageButton runat="server" ID="ibGroupClear" Width="19" Height="17" ImageUrl="~/Layouts/Images/reset17.gif" ImageAlign="AbsMiddle" OnClick="ibGroupClear_Click" />
								</div>
								<div style="overflow-y:auto;height:500px;" id="divLeftPanel">
									<asp:DataList ID="groupList" runat="server" CellPadding="0" CellSpacing="2" ShowHeader="False" ShowFooter="False" OnItemCommand="groupList_ItemCommand">
										<ItemStyle CssClass="ibn-propertysheet UserCellNonSelected"></ItemStyle>
										<SelectedItemStyle CssClass="ibn-propertysheet UserCellSelected"></SelectedItemStyle>
										<ItemTemplate>
											<div style="padding:1px;">
											<asp:linkbutton id="lbGroupItem" CommandName='Group' CommandArgument='<%# Eval("ObjectId") %>' runat="server" causesvalidation="False">
												<%# Eval("Title") %>
											</asp:linkbutton>
											</div>
										</ItemTemplate>
									</asp:DataList>
								</div>
							</ContentTemplate>
							<Triggers>
								<asp:AsyncPostBackTrigger ControlID="ddGrouping" />
							</Triggers>
						</asp:UpdatePanel>

						
					</div>
				</div>
			</div>
		</div>
	</DockItems>
</mc2:McDock>
<table style="margin-top:0px; padding-top: 0px; table-layout: fixed;" cellspacing="0" cellpadding="0" border="0" width="100%" class="ibn-propertysheet">
	<tr>
		<td style="padding-left: 5px;" class="filter">
			<asp:UpdatePanel ID="grdMainPanel" runat="server" UpdateMode="Conditional">
				<ContentTemplate>
					<mc:MCGrid ID="grdMain" runat="server" />	
					<mc:MCGridAction runat="server" ID="ctrlGridEventUpdater"  />
				</ContentTemplate>
			</asp:UpdatePanel>
		</td>
	</tr>
</table>

<asp:DataGrid ID="dgExport" Runat="server" AutoGenerateColumns="False" AllowPaging="False" 
	AllowSorting="False" EnableViewState="False" Visible="False" 
	ItemStyle-HorizontalAlign="Left" HeaderStyle-Font-Bold="True">
	<Columns>
		<asp:BoundColumn DataField="IncidentId"></asp:BoundColumn>
		<asp:BoundColumn DataField="Identifier"></asp:BoundColumn>
		<asp:BoundColumn DataField="Title"></asp:BoundColumn>
		<asp:BoundColumn DataField="PriorityName"></asp:BoundColumn>
		<asp:BoundColumn DataField="StateName"></asp:BoundColumn>
		<asp:TemplateColumn>
			<ItemTemplate><%# GetResponsible(Eval("CurrentResponsibleId"))%></ItemTemplate>
		</asp:TemplateColumn>
		<asp:BoundColumn DataField="CreationDate" DataFormatString="{0:yyyy-MM-dd}"></asp:BoundColumn>
		<asp:BoundColumn DataField="ModifiedDate" DataFormatString="{0:yyyy-MM-dd}"></asp:BoundColumn>
		<asp:BoundColumn DataField="ActualOpenDate" DataFormatString="{0:yyyy-MM-dd}"></asp:BoundColumn>
		<asp:BoundColumn DataField="ActualFinishDate" DataFormatString="{0:yyyy-MM-dd}"></asp:BoundColumn>
		<asp:BoundColumn DataField="CreatorName"></asp:BoundColumn>
		<asp:BoundColumn DataField="ManagerName"></asp:BoundColumn>
		<asp:BoundColumn DataField="ClientName"></asp:BoundColumn>
		<asp:BoundColumn DataField="IncidentBoxName"></asp:BoundColumn>
	</Columns>
</asp:DataGrid>
<asp:HiddenField ID="hfFilterValue" runat="server" />
<asp:LinkButton ID="lbViewChange" runat="server" OnClick="lbViewChange_Click"></asp:LinkButton> 

