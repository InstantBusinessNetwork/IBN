<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SelectObject.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.IbnCommon.Modules.SelectObject" %>
<%@ Register TagPrefix="mc" TagName="MCGrid" Src="~/Apps/HelpDeskManagement/Modules/MCGrid.ascx" %>
<%@ Register TagPrefix="mc" TagName="MCGridAction" Src="~/Apps/MetaUI/Grid/MetaGridServerEventAction.ascx" %>
<%@ Register TagPrefix="mc2" Assembly="Mediachase.Ibn.Web.UI.WebControls" Namespace="Mediachase.Ibn.Web.UI.WebControls" %>
<link rel="stylesheet" type="text/css" href='<%= Mediachase.Ibn.Web.UI.WebControls.McScriptLoader.Current.GetScriptUrl("~/Styles/IbnFramework/grid.css", this.Page) %>' />
<script type="text/javascript">
	var resizeFlag = false;
	function LayoutResizeHandler(sender, eventArgs)
	{
	}

	function StartFocusElement(elId)
	{
		var elem=document.getElementById(elId);
		if(!elem)
			return;
		try
		{
			elem.focus();
		}
		catch(ex)
		{
		}
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
		if(fl)
			<%=Page.ClientScript.GetPostBackClientHyperlink(lbSave, "") %>;
		else
			return false;
	}
</script>
<mc2:McDock ID="DockTop" runat="server" Anchor="top" EnableSplitter="False" DefaultSize="33">
	<DockItems>
		<table cellspacing="0" cellpadding="0" border="0" width="100%" class="filter">
			<tr>
				<td style="padding:7px;">
					<asp:UpdatePanel ID="upTop" runat="server" UpdateMode="Conditional">
						<ContentTemplate>
							<table class="ibn-propertysheet" width="100%" border="0" cellpadding="5" cellspacing="0">
								<tr>
									<td class="ibn-value"><asp:CheckBox ID="cbShowActive" Runat="server" AutoPostBack="True" OnCheckedChanged="cbShowActive_CheckedChanged"></asp:CheckBox> </td>
									<td class="ibn-value"><asp:DropDownList runat="server" ID="ddFilter" AutoPostBack="true" /> </td>
									<td class="ibn-value" align="right" >
										<asp:TextBox runat="server" ID="tbSearchString" Width="200px"></asp:TextBox>
										<asp:ImageButton Runat="server" id="btnSearch" Width="16" Height="16" ImageUrl="~/layouts/images/search.gif" ImageAlign="AbsMiddle" OnClick="btnSearch_Click" />
										<asp:ImageButton runat="server" ID="btnClear" Width="19" Height="17" ImageUrl="~/Layouts/Images/reset17.gif" ImageAlign="AbsMiddle" OnClick="btnClear_Click" />
									</td>
								</tr>
							</table>
						</ContentTemplate>
					</asp:UpdatePanel>
				</td>
			</tr>
		</table>
	</DockItems>
</mc2:McDock>	
<table style="margin-top:0px; padding-top: 0px; table-layout: fixed;" cellspacing="0" cellpadding="0" border="0" width="100%" class="ibn-propertysheet">
	<tr>
		<td style="padding-left: 5px;" class="filter">
			<asp:UpdatePanel ID="grdMainPanel" runat="server" UpdateMode="Conditional">
				<ContentTemplate>
					<mc:MCGrid ID="grdMain" runat="server" />	
					<mc:MCGridAction runat="server" ID="ctrlGridEventUpdater" PlaceName="" ViewName="" ClassName=""  />
				</ContentTemplate>
			</asp:UpdatePanel>
		</td>
	</tr>
</table>
<mc2:McDock ID="DockBottom" runat="server" Anchor="Bottom" EnableSplitter="false" DefaultSize="50">
	<DockItems>
		<table cellspacing="0" cellpadding="0" border="0" width="100%" class="filter">
			<tr>
				<td style="padding:10px; text-align:right;">
					<mc2:IMButton id="btnSave" onclick="CheckSelected();" runat="server" style="width:105px;"></mc2:IMButton>&nbsp;
					<mc2:IMButton id="btnCancel" runat="server" style="width:105px;"></mc2:IMButton>
				</td>
			</tr>
		</table>
	</DockItems>
</mc2:McDock>
<asp:HiddenField ID="hdnValue" runat="server" />
<asp:LinkButton ID="lbSave" runat="server" Visible="false" OnClick="lbSave_Click"></asp:LinkButton>