<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Directory.Modules.AddGroup" Codebehind="AddGroup.ascx.cs" %>
<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Reference Control="~/Apps/MetaUIEntity/Modules/EntityDropDown.ascx" %>
<%@ Register TagPrefix="btn" namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="~/Modules/BlockHeader.ascx" %>
<%@ Register TagPrefix="ibn" TagName="EntityDD" src="~/Apps/MetaUIEntity/Modules/EntityDropDown.ascx" %>
<script type="text/javascript" src='<%=ResolveClientUrl("~/scripts/List2List.js") %>'></script>
<script type="text/javascript">
<!--
	function GroupExistence (sender,args)
	{
		if((document.forms[0].<%=lbSelectedGroups.ClientID%> != null)&& (document.forms[0].<%=lbSelectedGroups.ClientID%>.options.length>0))
		{
			args.IsValid = true;	
			return;
		}
		args.IsValid = false;	
	}
	function SaveGroups()	
	{
		var sControl=document.forms[0].<%=lbSelectedGroups.ClientID%>;
		
		var str="";
		if(sControl != null)
		{
			for(var i=0;i<sControl.options.length;i++)
			{
				str += sControl.options[i].value + ",";
			}
		}
		document.getElementById('<%=iGroups.ClientID%>').value = str;
	}
//-->
</script>
<table class="ibn-stylebox text" style="MARGIN-TOP: 0px; margin-left:2px" cellspacing="0" cellpadding="0" width="100%" border="0">
	<tr>
		<td><ibn:blockheader id="secHeader" runat="server"></ibn:blockheader></td>
	</tr>
	<tr>
		<td style="padding: 15px;">
			<table class="ibn-propertysheet" cellspacing="3" cellpadding="3" width="100%" border="0">
				<tr>
					<td width="120"><asp:label id="lblGroupTitle" CssClass="boldtext" Runat="server"></asp:label>:&nbsp;
					</td>
					<td><asp:textbox id="tbGroupTitle" CssClass="text" Runat="server" Width="200"></asp:textbox><asp:requiredfieldvalidator id="RequiredFieldValidator1" runat="server" CssClass="Text" ControlToValidate="tbGroupTitle" ErrorMessage=" * "></asp:requiredfieldvalidator>
					</td>
				</tr>
				<tr id="trGroups" runat="server">
					<td valign="top">
						<asp:label id="lblVisible" CssClass="boldtext" Runat="server"></asp:label>:</td>
					<td>
						<table class="text" id="tblGroups" style="height: 150px;" cellpadding="0" border="0">
							<tr>
								<td valign="top" nowrap width="45%" style="PADDING-RIGHT: 6px; PADDING-BOTTOM: 6px;">
									<asp:label id="lblAvailable" CssClass="text" Runat="server"></asp:label><br />
									<asp:listbox id="lbAvailableGroups" runat="server" height="130px" cssclass="text" width="230px"></asp:listbox>
								</td>
								<td style="PADDING-RIGHT: 6px; PADDING-LEFT: 6px; PADDING-BOTTOM: 6px">
									<p align="center">
										<asp:button id="btnAddOneGr" style="MARGIN: 1px" runat="server" CausesValidation="False" cssclass="text" width="30px" text=">"></asp:button><br />
										<asp:button id="btnAddAllGr" style="MARGIN: 1px" runat="server" CausesValidation="False" cssclass="text" width="30px" text=">>"></asp:button><br /><br />
										<asp:button id="btnRemoveOneGr" style="MARGIN: 1px" runat="server" CausesValidation="False" cssclass="text" width="30px" text="<"></asp:button><br />
										<asp:button id="btnRemoveAllGr" style="MARGIN: 1px" runat="server" CausesValidation="False" cssclass="text" width="30px" text="<<"></asp:button>
									</p>
								</td>
								<td valign="top" width="45%" style="PADDING-RIGHT: 20px; PADDING-LEFT: 6px; PADDING-BOTTOM: 6px">
									<asp:label id="lblSelected" CssClass="text" Runat="server"></asp:label><br />
									<asp:listbox id="lbSelectedGroups" runat="server" height="130px" cssclass="text" width="230px"></asp:listbox>
								</td>
							</tr>
						</table>
					</td>
				</tr>
				<tr id="trClient" runat="server">
					<td width="120"><asp:label id="lblClient" CssClass="boldtext" Runat="server"></asp:label>:</td>
					<td><ibn:EntityDD ObjectTypes="Contact,Organization" id="ClientControl" runat="server" Width="200px"/></td>
				</tr>
				<tr>
					<td valign="bottom" align="right" height="40"></td>
					<td valign="bottom" align="right" height="40">
						<p align="left">
							<btn:imbutton class="text" id="btnSave" style="width:110px;" Runat="server" onserverclick="btnSave_ServerClick"></btn:imbutton>&nbsp;&nbsp;
							<btn:imbutton class="text" id="btnCancel" style="width:110px;" Runat="server" Text="" IsDecline="true" CausesValidation="false" onserverclick="btnCancel_ServerClick"></btn:imbutton>
						</p>
					</td>
				</tr>
			</table>
		</td>
	</tr>
</table>
<input id="iGroups" style="VISIBILITY: hidden" runat="server" />