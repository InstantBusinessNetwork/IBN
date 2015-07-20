<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Modules.EditControls.DictionarySingleValue" Codebehind="DictionarySingleValue.ascx.cs" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<link type="text/css" rel="stylesheet" href="<%= ResolveClientUrl("~/Styles/IbnFramework/ajax.css") %>" />

<table cellpadding="0" cellspacing="0">
	<tr>
		<td class="text">
			<asp:UpdatePanel runat="server" ID="UpdatePanel1" ChildrenAsTriggers="true" UpdateMode="Conditional">
				<ContentTemplate>
					<asp:Label runat="server" ID="TextLabel" Text="" Width="300" CssClass="SelectedItem" />
					<asp:Panel runat="Server" ID="ItemPanel" style="display:none;visibility:hidden; width:300px;" CssClass="ContextMenuPanel"></asp:Panel>
					<ajaxToolkit:DropDownExtender runat="server" ID="DropDownExtender1" TargetControlID="TextLabel" DropDownControlID="ItemPanel"  />
					<asp:HiddenField runat="server" ID="SavedValue" />
				</ContentTemplate>
			</asp:UpdatePanel>
		</td>
		<td>
			&nbsp;<button id="btnEditItems" runat="server" style="border:0px;padding:0px;position:relative;top:0px;left:0px;height:20px;width:22px;background-color:transparent" type="button"><img 
				height="20" title='<%=LocRM.GetString("EditDictionary")%>' src='<%=ResolveUrl("~/layouts/images/icons/dictionary_edit.gif")%>' width="22" border="0" /></button>
		</td>
	</tr>
</table>

