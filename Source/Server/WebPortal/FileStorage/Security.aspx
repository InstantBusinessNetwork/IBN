<%@ Page Language="c#" Inherits="Mediachase.UI.Web.FileStorage.Security" CodeBehind="Security.aspx.cs" %>
<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" Src="~/Modules/BlockHeader.ascx" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	
	<title><%=LocRM.GetString("Security")%></title>

	<script type="text/javascript">
		//<![CDATA[
		function disableEnterKey() {
			try {
				if (window.event.keyCode == 13 && window.event.srcElement.type != "textarea")
					window.event.keyCode = 0;
			}
			catch (e) { }
		}
		//]]>
	</script>

</head>
<body class="UserBackground" id="pT_body">
	<form id="frmMain" method="post" runat="server" enctype="multipart/form-data">
	<ibn:BlockHeader ID="secHeader" runat="server" />
	<table id="mainTable" class="ibn-stylebox-light text" style="height: 100%;" height="100%" cellspacing="0" cellpadding="2" width="100%" border="0">
		<tr height="22">
			<td class="boldtext" width="290" height="22">
				<%=LocRM.GetString("Available") %>
				:
			</td>
			<td class="ibn-navframe boldtext">
				<%=LocRM.GetString("Selected") %>
				:
			</td>
		</tr>
		<tr style="height: 100%">
			<td valign="top" width="290">
				<!-- Groups & Users -->
				<table class="text" style="margin-top: 5px" cellspacing="0" cellpadding="2" width="100%">
					<tr>
						<td width="9%">
							<%=LocRM.GetString("Group") %>:
						</td>
						<td width="91%">
							<asp:DropDownList ID="ddGroups" runat="server" Width="190px" CssClass="text" AutoPostBack="True">
							</asp:DropDownList>
						</td>
						<tr>
							<td valign="top">
								<%=LocRM.GetString("User") %>:
							</td>
							<td valign="top">
								<asp:DropDownList ID="ddUsers" runat="server" Width="190px" CssClass="text">
								</asp:DropDownList>
							</td>
						</tr>
						<tr>
							<td valign="top">
								<%=LocRM.GetString("Rights") %>:
							</td>
							<td valign="top">
								<asp:DropDownList ID="ddRights" runat="server" Width="190px" CssClass="text">
								</asp:DropDownList>
							</td>
						</tr>
						<tr>
							<td valign="top">
							</td>
							<td valign="top">
								<asp:RadioButtonList ID="rbList" runat="server" Width="190px" CssClass="text" RepeatColumns="2">
								</asp:RadioButtonList>
							</td>
						</tr>
						<tr>
							<td valign="top" height="28">
								&nbsp;
							</td>
							<td>
								<button id="btnAdd" runat="server" onclick="DisableButtons(this);DisableCheck();" class="text" style="width: 90px;" type="button" onserverclick="btnAdd_Click">
								</button>
							</td>
						</tr>
				</table>
				<table id="tblInhereted" runat="server" class="text" cellspacing="0" cellpadding="3" width="100%" border="0">
					<tr>
						<td width="99%">
							<label for="cbInherit">
								<input id="cbInherit" onclick="CheckClear()" type="checkbox" value="Has Inherited" name="cbInherit" runat="server">
								<%=LocRM.GetString("tInherit")%>
							</label>
						</td>
						<td width="1%">
							<div align="right">
							</div>
						</td>
					</tr>
				</table>
				<!-- End Groups & Users -->
			</td>
			<td valign="top" height="100%" class="ibn-navframe">
				<!-- Data GRID -->
				<div style="overflow-y: auto; height: 262px">
					<asp:DataGrid ID="dgMembers" runat="server" AutoGenerateColumns="False" AllowPaging="False" AllowSorting="False" CellPadding="3" GridLines="None" CellSpacing="3" BorderWidth="0px">
						<ItemStyle CssClass="ibn-propertysheet"></ItemStyle>
						<HeaderStyle CssClass="text"></HeaderStyle>
						<Columns>
							<asp:BoundColumn DataField="ID" Visible="False"></asp:BoundColumn>
							<asp:TemplateColumn HeaderText='Name'>
								<ItemTemplate>
									<%# GetLink((int)Eval("Weight"), Eval("Name").ToString())%>
								</ItemTemplate>
							</asp:TemplateColumn>
							<asp:TemplateColumn HeaderText='Rights'>
								<ItemTemplate>
									<%# Eval("Rights")%>
								</ItemTemplate>
							</asp:TemplateColumn>
							<asp:TemplateColumn HeaderText="Allow" ItemStyle-Width="45" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
								<ItemTemplate>
									<%# (bool)Eval("Allow") ?
													"<img alt='' src='../layouts/Images/accept.gif'/>":
													"<img alt='' src='../layouts/Images/deny.gif'/>"
									%>
								</ItemTemplate>
							</asp:TemplateColumn>
							<asp:TemplateColumn ItemStyle-Width="30" Visible="True">
								<ItemTemplate>
									<asp:ImageButton Visible='<%# !(bool)Eval("IsInherited") && !(bool)Eval("HasOwnerKey")%>' ID="ibDelete" runat="server" BorderWidth="0" Width="16" Height="16" CommandName="Delete" CausesValidation="False" ImageUrl="../layouts/images/DELETE.GIF"></asp:ImageButton>
								</ItemTemplate>
							</asp:TemplateColumn>
						</Columns>
					</asp:DataGrid>
					<!-- End Data GRID -->
				</div>
			</td>
		</tr>
	</table>
	<asp:LinkButton ID="btnSave" runat="server" Visible="False"></asp:LinkButton>
	<asp:LinkButton ID="btnRestoreInherited" runat="server" Visible="False"></asp:LinkButton>
	<asp:LinkButton ID="btnSetInherited" runat="server" Visible="False"></asp:LinkButton>
	<asp:LinkButton ID="btnClearInherited" runat="server" Visible="False"></asp:LinkButton>
	<asp:LinkButton ID="btnSaveLostChanges" runat="server" Visible="False"></asp:LinkButton>

	<script type="text/javascript">
		//<![CDATA[
		function DisableCheck()
		{
			document.forms[0].<%=cbInherit.ClientID %>.disabled = true;
		}
		function FuncSave()
		{
			<%=Page.ClientScript.GetPostBackEventReference(btnSave,"") %>
		}
		function getMenu(s)
		{
			return document.getElementById(s)
		}
		function openEditItem()
		{
			var menu = getMenu('divEditItem');
			var selectColl = document.getElementsByTagName('select');
			for(var i=0;i<selectColl.length;i++)
				selectColl[i].style.visibility = "hidden";
			menu.style.display = "";
		}
		function cancel()
		{
			getMenu('divEditItem').style.display = "none";
			document.forms[0].<%=cbInherit.ClientID %>.checked = true;
			var selectColl = document.getElementsByTagName('select');
			for(var i=0;i<selectColl.length;i++)
				selectColl[i].style.visibility = "";
		}
		function Leave()
		{
			getMenu('divEditItem').style.display = "none";
			<%=Page.ClientScript.GetPostBackEventReference(btnSetInherited,"") %>
		}
		function Drop()
		{
			getMenu('divEditItem').style.display = "none";
			<%=Page.ClientScript.GetPostBackEventReference(btnClearInherited,"") %>
		}
		function CheckClear()
		{
			if (document.forms[0].<%=cbInherit.ClientID %>.checked == false)
			{
				openEditItem();
			}
			else
				<%=Page.ClientScript.GetPostBackEventReference(btnRestoreInherited,"") %>
		}
		function openMenuLostrights()
		{
			var mTab = document.getElementById('mainTable');
			if(mTab != null)
				mTab.style.display = "none";
				
			var menu = getMenu('divLostRights');
			
			
			var l = (document.body.clientWidth - 200) / 2;
			var t = (document.body.clientHeight - 200) / 2;
			
			if(! browseris.ie5up)
			{
				l = 50;
				t = 50;
			}

			menu.style.left = l.toString() + "px";
			menu.style.top = t.toString() + "px";
			menu.style.display = "";
		}
		function savewithlost()
		{
			<%=Page.ClientScript.GetPostBackEventReference(btnSaveLostChanges,"") %>
		}
		function cancellost()
		{
			getMenu('divLostRights').style.display = "none";
			var mTab = document.getElementById('mainTable');
			if(mTab != null)
				mTab.style.display = "";
		}
		//]]>
	</script>

	<div id="divEditItem" style="position: absolute; top: 30px; left: 10px; width: 340px; z-index: 255; padding: 5px; display: none; border: 1px solid #95b7f3;" class="ibn-rtetoolbarmenu ibn-propertysheet ibn-selectedtitle">
		<fieldset style="height: 220px; margin: 0; padding: 2px">
			<legend class="text ibn-legend-default" id="lgdPModal" runat="server"></legend>
			<table cellpadding="0" cellspacing="0" border="0" width="100%">
				<tr>
					<td class="text">
						<p align="left" class="text">
							<%=LocRM.GetString("tText")%><br/>
							<br/>
							<%=LocRM.GetString("tTextCopy")%><br/>
							<br/>
							<%=LocRM.GetString("tTextRemove")%><br/>
							<br/>
							<%=LocRM.GetString("tTextCancel")%><br/>
						</p>
					</td>
				</tr>
				<tr>
					<td style="padding-left: 10px; padding-top: 10px">
						<input type="button" class="text" value='<%=LocRM.GetString("tCopy")%>' style="width: 97;" onclick="Leave();" />&nbsp;&nbsp;
						<input type="button" class="text" value='<%=LocRM.GetString("tRemove")%>' style="width: 80;" onclick="Drop();" />&nbsp;&nbsp;
						<input type="button" class="text" value='<%=LocRM.GetString("tCancel")%>' style="width: 80;" onclick="cancel();" />
					</td>
				</tr>
			</table>
		</fieldset>
	</div>
	<div id="divLostRights" style="position: absolute; top: 30px; left: 300px; width: 200px; z-index: 255; padding: 5px; display: none; border: 1px solid #95b7f3;" class="ibn-rtetoolbarmenu ibn-propertysheet ibn-selectedtitle">
		<fieldset style="height: 130px; margin: 0; padding: 2px">
			<legend class="text ibn-legend-default" id="lgdLostRights" runat="server"></legend>
			<table cellpadding="0" cellspacing="0" border="0" width="100%" height="100%">
				<tr>
					<td class="text">
						<p align="center" class="text">
							<%=LocRM.GetString("tAttentionText")%>
						</p>
					</td>
				</tr>
				<tr valign="bottom">
					<td style="padding-left: 10px; padding-top: 10px; padding-bottom: 15px" valign="bottom">
						<input type="button" class="text" value='<%=LocRM.GetString("Save")%>' style="width: 80px;" onclick="savewithlost();" />&nbsp;&nbsp;
						<input type="button" class="text" value='<%=LocRM.GetString("Cancel")%>' style="width: 80px;" onclick="cancellost();" />
					</td>
				</tr>
			</table>
		</fieldset>
	</div>
	</form>
</body>
</html>
