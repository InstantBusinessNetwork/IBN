<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TransitionEdit.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.Apps.StateMachine.Modules.ManageControls.TransitionEdit" %>
<script type="text/javascript" language="javascript">
	function ShowHide(chkId, txtId)
	{
		var chk = document.getElementById(chkId);
		var txt = document.getElementById(txtId);
		
		if(chk == null || txt == null )
			return;
			
		if (chk.checked)
		{
			txt.style.display = "block";
		}
		else
		{
			txt.style.display = "none";
		}
	}
</script>
<table style="width:100%;"><tr><td style="padding:10px;">
	<asp:Literal ID="Literal2" Text="<%$Resources : IbnFramework.GlobalMetaInfo, State%>" runat="server"></asp:Literal>:
	<asp:Label runat="server" ID="lblFrom" Font-Bold="true"></asp:Label>
	<br /><br />
	<div style="border:2px inset;overflow-y:auto;height:200px;">
	<table cellpadding="3" cellspacing="0" border="0" width="100%" class="ibn-propertysheet" style="table-layout:fixed" runat="server" id="tblMain">
		<tr>
			<th style="width: 50%;"><asp:Literal ID="Literal4" Text="<%$Resources : IbnFramework.GlobalMetaInfo, AllowedTransitions%>" runat="server"></asp:Literal></th>
			<th><asp:Literal ID="Literal1" Text="<%$Resources : IbnFramework.GlobalMetaInfo, TransitionName%>" runat="server"></asp:Literal></th>
		</tr>
	</table>
	</div>
	<br />
	<div style="padding-top:10px; text-align:center;">
	<asp:Button runat="server" ID="btnSave" Text="<%$Resources : IbnFramework.Global, _mc_Save%>" OnClick="btnSave_Click" Width="80" />
	<asp:Button runat="server" ID="btnCancel" Text="<%$Resources : IbnFramework.Global, _mc_Cancel%>" OnClientClick="window.close();" Width="80" style="margin-left:15px;" CausesValidation="false" />
	</div>
</td></tr></table>