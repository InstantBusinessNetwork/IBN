<%@ Reference Control="~/Modules/EditControls/DictionaryMultivalue.ascx" %>
<%@ Reference Control="~/Modules/EditControls/DictionarySingleValue.ascx" %>
<%@ Reference Control="~/Modules/EditControls/DateValue.ascx" %>
<%@ Reference Control="~/Modules/EditControls/DateTimeValue.ascx" %>
<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Register TagPrefix="btn" namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="BlockHeader.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Modules.MetaDataBlockEditControl" Codebehind="MetaDataBlockEditControl.ascx.cs" %>
<table class="ibn-stylebox" cellspacing="0" cellpadding="0" width="100%" border="0" style="margin-top:0px">
	<tr>
		<td><ibn:blockheader id="tbMetaInfo" title="Custom Fields" runat="server"></ibn:blockheader></td>
	</tr>
	<tr>
		<td style="padding-left: 7px;padding-top:10px;">
			<table cellpadding="0" cellspacing="0" width="500">
				<tr>
					<td>
						<table class="ibn-propertysheet" style="padding-left:15px;padding-top:10px;" cellspacing="0" cellpadding="5" border="0" runat="server" id="tblCustomFields">
						</table>
					</td>
				</tr>
				<tr>
					<td vAlign="bottom" align="right" height="40" colspan="2">
						<btn:imbutton class="text" id="btnSave" Runat="server" style="width:110px;" onserverclick="btnSave_ServerClick"></btn:imbutton>&nbsp;&nbsp;
						<btn:imbutton class="text" CausesValidation="false" id="btnCancel" Runat="server" style="width:110px;" IsDecline="true"></btn:imbutton>
					</td>
				</tr>
			</table>
			<br>
		</td>
	</tr>
</table>
