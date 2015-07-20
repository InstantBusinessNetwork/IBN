<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AddResources.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.ProjectManagement.Modules.AddResources" %>
<%@ Reference Control="~/Apps/MetaUI/Toolbar/MetaToolbar.ascx" %>
<%@ Register TagPrefix="mc" TagName="MCGrid" Src="~/Apps/HelpDeskManagement/Modules/MCGrid.ascx" %>
<%@ Register TagPrefix="mc" TagName="MCGridAction" Src="~/Apps/MetaUI/Grid/MetaGridServerEventAction.ascx" %>
<%@ Register TagPrefix="mc" TagName="MetaToolbar" Src="~/Apps/MetaUI/Toolbar/MetaToolbar.ascx" %>
<%@ Register TagPrefix="mc2" Assembly="Mediachase.Ibn.Web.UI.WebControls" Namespace="Mediachase.Ibn.Web.UI.WebControls" %>
<link rel="stylesheet" type="text/css" href='<%= Mediachase.Ibn.Web.UI.WebControls.McScriptLoader.Current.GetScriptUrl("~/Styles/IbnFramework/grid.css", this.Page) %>' />
<script type="text/javascript">
	var resizeFlag = false;
	function LayoutResizeHandler(sender, eventArgs)
	{
	}
	
	function CheckSelected()
	{
		var obj = $find('<%= grdMain.GridClientContainerId %>');
		var fl = true;
		if(obj)
		{
			var hdn = document.getElementById('<%=hdnValue.ClientID %>');
			if(obj.isCheckboxes())
			{
				if(!obj.isChecked())
				{
					if(obj.getSelectedElement() == "")
						fl = false;
					else
						hdn.value = obj.getSelectedElement();
				}
				else
					hdn.value = obj.getCheckedCollection();
			}
			else
			{
				if(obj.getSelectedElement() == "")
					fl = false;
				else
					hdn.value = obj.getSelectedElement();
			}
		}
		else
			fl = false;
		return fl;
	}
	
	function onAddSelected(item)
	{
		if(CheckSelected())
			<%=Page.ClientScript.GetPostBackClientHyperlink(lbAdd, "") %>;
	}
	
	function onAddWithConfirmSelected(item)
	{
		if(CheckSelected())
			<%=Page.ClientScript.GetPostBackClientHyperlink(lbAddWithConfirm, "") %>;
	}
	
	function onAddGroup()
	{
		<%=Page.ClientScript.GetPostBackClientHyperlink(lbAddGroup, "") %>;
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
		border-left: 1px solid #6B92CE;
	}
	.innerContentArea
	{
		background-color: White;
		border:1px solid #BBD4F6;
	}
	
	.pad5{
		padding: 5px;
	}
</style>
<mc2:McDock ID="DockRight" InnerCssClass="hundred" runat="server" Anchor="Right" EnableSplitter="False" DefaultSize="370">
	<DockItems>
		<div style="height: 100%; width: 100%; position: relative;" class="filter ibn-propertysheet">
			<div style="position: absolute; left:5px; bottom: 2px; top: 0px; right:0px;" class="innerArea">
				<br />
				<span style="padding:9px;"><b><%=LocRM.GetString("Selected") %>:</b></span>
				<br />
				<div style="position:absolute;left:9px;right:9px;top:30px;bottom:40px;OVERFLOW-Y:auto;" class="innerContentArea" id="divLeftContainer">
				
					<asp:UpdatePanel ID="upRight" runat="server" UpdateMode="Conditional">
						<ContentTemplate>
							<asp:DataGrid Runat="server" ID="dgMembers" AutoGenerateColumns="False" 
								AllowPaging="False" AllowSorting="False" gridlines="None" borderwidth="0px" >
								<Columns>
									<asp:BoundColumn DataField="UserId" Visible="False"></asp:BoundColumn>
									<asp:TemplateColumn>
										<HeaderStyle CssClass="text pad5" Width="220px" />
										<ItemStyle CssClass="ibn-propertysheet pad5" Width="220px" />
										<ItemTemplate>
											<%# GetLink( (int)DataBinder.Eval(Container.DataItem, "UserId"),false )%>
										</ItemTemplate>
									</asp:TemplateColumn>
									<asp:TemplateColumn>
									<HeaderStyle CssClass="text pad5" Width="50px" />
										<ItemStyle CssClass="ibn-propertysheet pad5" Width="50px" />
										<ItemTemplate>
											<%# GetStatus
											(
											DataBinder.Eval(Container.DataItem, "MustBeConfirmed"),
											DataBinder.Eval(Container.DataItem, "ResponsePending"),
											DataBinder.Eval(Container.DataItem, "IsConfirmed")
											)%>
										</ItemTemplate>
									</asp:TemplateColumn>
									<asp:templatecolumn>
										<HeaderStyle CssClass="text pad5" Width="20px" />
										<ItemStyle CssClass="ibn-propertysheet pad5" Width="20px" HorizontalAlign="Center" />
										<itemtemplate>
											<asp:imagebutton id="ibDelete" runat="server" borderwidth="0" width="16" height="16" imageurl="~/layouts/images/DELETE.GIF" ImageAlign="AbsMiddle" commandname="Delete" causesvalidation="False"></asp:imagebutton>
										</itemtemplate>
									</asp:templatecolumn>
								</Columns>
							</asp:DataGrid>
						</ContentTemplate>
					</asp:UpdatePanel>
				</div>
				<div style="position:absolute;left:9px;right:9px;top:360px;bottom:5px;text-align:right;" >
					<mc2:IMButton id="btnSave" runat="server" style="width:105px;"></mc2:IMButton>&nbsp;
					<mc2:IMButton id="btnCancel" runat="server" style="width:105px;"></mc2:IMButton>
				</div>
			</div>
		</div>
			
	</DockItems>
</mc2:McDock>	
<mc2:McDock ID="DockTop" runat="server" Anchor="Top" EnableSplitter="False" DefaultSize="89">
	<DockItems>
		<asp:UpdatePanel ID="upTop" runat="server" UpdateMode="Conditional">
			<ContentTemplate>
				<table cellspacing="0" cellpadding="0" border="0" width="100%" class="filter">
					<tr>
						<td style="padding:7px;"><%=LocRM.GetString("Group") %>:</td>
						<td style="padding:7px;">
							<mc2:indenteddropdownlist id="ddGroups" runat="server" CssClass="text" Width="200px" AutoPostBack="True" onselectedindexchanged="ddGroups_ChangeGroup"></mc2:indenteddropdownlist>
						</td>
					</tr>
					<tr>
						<td style="padding:7px;"><%=LocRM.GetString("Search") %>:</td>
						<td style="padding:7px;">
							<asp:TextBox runat="server" ID="tbSearchString" Width="200px"></asp:TextBox>
							<asp:ImageButton Runat="server" id="btnSearch" Width="16" Height="16" ImageUrl="~/layouts/images/search.gif" ImageAlign="AbsMiddle" OnClick="btnSearch_Click" />
							<asp:ImageButton runat="server" ID="btnClear" Width="19" Height="17" ImageUrl="~/Layouts/Images/reset17.gif" ImageAlign="AbsMiddle" OnClick="btnClear_Click" />	
						</td>
					</tr>
					<tr>
						<td style="padding-left: 5px; padding-right: 5px;" colspan="2">
							<div class="noBottomBorder">
								<mc:MetaToolbar runat="server" ID="MainMetaToolbar" GridId="grdMain" />
							</div>		
						</td>
					</tr>	
				</table>
				<asp:HiddenField ID="hdnValue" runat="server" />
				<asp:Button ID="lbAdd" runat="server" style="display:none;" OnClick="lbAdd_Click"></asp:Button>
				<asp:Button ID="lbAddGroup" runat="server" style="display:none;" OnClick="lbAddGroup_Click"></asp:Button>
				<asp:Button ID="lbAddWithConfirm" runat="server" style="display:none;" OnClick="lbAddWithConfirm_Click"></asp:Button>
			</ContentTemplate>
		</asp:UpdatePanel>
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