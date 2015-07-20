<%@ Reference Control="~/Modules/EditControls/DictionaryMultivalue.ascx" %>
<%@ Reference Control="~/Modules/EditControls/DictionarySingleValue.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Modules.MetaDataEditControl" Codebehind="MetaDataEditControl.ascx.cs" %>
<%@ Register TagPrefix="btn" namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>
<script type="text/javascript">
function CheckValidate(sender)
{
	if (typeof(Page_Validators) != "undefined")
	{
		if(Page_ValidationActive)
		{
			Page_ClientValidate();
			if(!Page_IsValid)
			{
				var obj = document.getElementById('<%=divError.ClientID %>');
				if(obj)
				{
					obj.style.display = "";
					return false;
				}
			}	
		}
	}
	DisableButtons(sender);
}
function ForCancel(sender)
{
	DisableButtons(sender);
	<%=Page.ClientScript.GetPostBackEventReference(btnCancel, "") %>
}
</script>
<table cellpadding="0" cellspacing="0" width="550">
	<tr>
		<td>
			<div id="divError" runat="server" class="text" style="height:50px;vertical-align:middle; margin:5px; padding-top:5px; padding-bottom:5px; background-color:#ffffe1;border:1px solid #bbb;">
			  <blockquote style="padding-left:10px; margin:5px; margin-left:15px; padding-top:3px; padding-bottom:3px">
				<div style="float:left;padding-right:10px;"><img src="../layouts/images/deleteuser.gif" align="absmiddle" /></div>
				<div style="float:left;padding-top:5px;">
				  <asp:Label ID="lblErrorText" CssClass="ibn-propertysheet" runat="server"></asp:Label>
				</div>
			  </blockquote>
			</div>
		</td>
	</tr>
	<tr>
		<td>
			<table class="ibn-propertysheet" style="PADDING-LEFT: 15px" cellspacing="0" cellpadding="5" border="0" runat="server" id="tblCustomFields">
			</table>
		</td>
	</tr>
	<tr runat="server" id="trButtons">
		<td vAlign="bottom" align="right" height="40" colspan="2">
			<btn:imbutton class="text" id="btnSave" Runat="server" style="width:110px;" onserverclick="btnSave_ServerClick"></btn:imbutton>&nbsp;&nbsp;
			<btn:imbutton class="text" CausesValidation="false" id="btnCancel" Runat="server" style="width:110px;" IsDecline="true" onserverclick="btnCancel_ServerClick"></btn:imbutton>
		</td>
	</tr>
</table>
			