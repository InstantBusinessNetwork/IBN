<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MetaClassView.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.Apps.MetaDataBase.Modules.ManageControls.MetaClassView" %>
<%@ Reference Control="~/Modules/PageViewMenu.ascx" %>
<%@ Register TagPrefix="ibn3" TagName="BlockHeader" src="~/Modules/PageViewMenu.ascx" %>
<%@ Register TagPrefix="ibn2" Namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>
<asp:Panel ID="Panel1" ScrollBars="Auto" runat="server">
	<table cellspacing="0" cellpadding="0" border="0" width="100%" class="ibn-stylebox2">
		<tr>
		<td>
			<ibn3:BlockHeader id="BlockHeaderMain" runat="server" />
		</td>
	  </tr>
	  <tr>
		<td>
			<asp:UpdatePanel runat="server" ID="upMain" ChildrenAsTriggers="true" UpdateMode="Conditional">
				<ContentTemplate>
					<ibn2:XMLFormBuilder ID="xmlStruct" runat="server" />
				</ContentTemplate>
			</asp:UpdatePanel>
			<asp:UpdateProgress ID="uProgress" runat="server" AssociatedUpdatePanelID="upMain" DisplayAfter="1000">
				<ProgressTemplate>
					<div class="upProgressMain">
						<div class="upProgressCenter">
							<img alt="" style="vertical-align:middle; border: 0;" src='<%= ResolveClientUrl("~/Images/IbnFramework/loading_rss.gif") %>' />&nbsp;
							<asp:Literal ID="Literal2" runat="server" Text="<%$Resources : IbnFramework.GlobalMetaInfo, Loading%>" />...
						</div>
					</div>
				</ProgressTemplate>
			</asp:UpdateProgress>
		</td>
	  </tr>
	</table>
</asp:Panel>