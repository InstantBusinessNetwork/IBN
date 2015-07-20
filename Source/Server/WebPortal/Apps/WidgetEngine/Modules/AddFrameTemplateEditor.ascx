<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AddFrameTemplateEditor.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.Apps.WidgetEngine.Modules.AddFrameTemplateEditor" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeaderLight" src="~/Modules/BlockHeaderLightWithMenu.ascx" %>
<%@ Register TagPrefix="mc" namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>

<div style="margin: 10px 20px 0px 20px;" runat="server" id="divPageTemplate">
	<ibn:BlockHeaderLight ID="secTemplate" runat="server" />
	
	<div style="margin: 0px; height: 260px; overflow-y: none; overflow-x: scroll; margin-bottom: 15px;" class="ibn-WPBorder">
		<asp:Repeater runat="server" ID="repTemplates">
			<ItemTemplate>
				<div class="customizeWSTemplateItem" runat="server" id="mainItemDiv" 
				onmouseover="if (this.className != 'customizeWSTemplateItemSelected') this.className='customizeWSTemplateItemHover';" 
				onmouseout="if (this.className != 'customizeWSTemplateItemSelected') this.className='customizeWSTemplateItem';">
					<table cellpadding="0" cellspacing="0" border="0" class="text" style="width: 95%;">
						<tr>
							<td align="left" style="width: 150px;"><asp:Image runat="server" ID="imgTemplate" AlternateText="TemplateImage" ImageUrl='<%# Eval("ImageUrl") %>' /></td>
							<%--<td style="width: 150px;"><%# Eval("Title") %></td>--%>
							<td><%# Eval("Description") %></td>
						</tr>
					</table>
					<asp:Button runat="server" ID="btnCommand" CommandName='Click' CommandArgument='<%# Eval("Id") %>' Visible="false" />
				</div>
			</ItemTemplate>
		</asp:Repeater>
	</div>
		<div style="float: left; width: 170px;">
			<asp:LinkButton runat="server" Visible="false" ID="lblAddControl" Text="<%$ Resources : IbnFramework.WidgetEngine,_mc_AddControl %>" />
		</div>
		<mc:ImButton runat="server" ID="btnAddAndClose"></mc:ImButton>
		<mc:ImButton runat="server" ID="btnDefault" style="float: right;"></mc:ImButton>&nbsp;
		
</div>
