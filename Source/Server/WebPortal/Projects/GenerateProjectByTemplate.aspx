<%@ Page Language="c#" Inherits="Mediachase.UI.Web.Projects.GenerateProjectByTemplate" CodeBehind="GenerateProjectByTemplate.aspx.cs" %>
<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" Src="~/Modules/BlockHeader.ascx" %>
<%@ Register TagPrefix="lst" Namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	
	<title><%=LocRM.GetString("tCreateFromTemplate")%></title>
</head>
<body class="UserBackground" id="pT_body">
	<form id="frmMain" runat="server">
	<ibn:BlockHeader ID="secHeader" Title="ToolBar" runat="server"></ibn:BlockHeader>
	<table class="text" style="height: 100%" height="100%" cellspacing="0" cellpadding="3" width="100%" border="0">
		<tr height="22">
			<td class="boldtext" width="280" height="22">
				<%=LocRM.GetString("tAvailable") %>:
			</td>
			<td width="4">
				&nbsp;
			</td>
			<td class="ibn-navframe boldtext">
				<%=LocRM.GetString("tAssigned") %>:
			</td>
		</tr>
		<tr style="height: 100%">
			<td valign="top" width="280">
				<!-- Groups & Users -->
				<table class="text" style="margin-top: 5px" cellspacing="0" cellpadding="3" width="100%">
					<tr>
						<td width="9%" style="height: 18px">
							<%=LocRM.GetString("tGroup") %>:
						</td>
						<td width="91%" style="height: 18px">
							<lst:IndentedDropDownList ID="ddGroups" runat="server" CssClass="text" Width="190px" AutoPostBack="True" OnSelectedIndexChanged="ddGroups_ChangeGroup">
							</lst:IndentedDropDownList>
						</td>
					</tr>
					<tr>
						<td width="9%">
							<%=LocRM.GetString("tSearch") %>:
						</td>
						<td width="91%">
							<asp:TextBox ID="tbSearch" runat="server" CssClass="text" Width="125px"></asp:TextBox>
							<asp:Button ID="btnSearch" runat="server" Width="60px" CssClass="text" CausesValidation="False" OnClick="btnSearch_Click"></asp:Button>
						</td>
					</tr>
					<tr>
						<td valign="top" height="96">
							<%=LocRM.GetString("tUser") %>:
						</td>
						<td valign="top">
							<asp:ListBox ID="lbUsers" runat="server" CssClass="text" Width="190px" SelectionMode="Multiple" Rows="6"></asp:ListBox>
						</td>
					</tr>
					<tr>
						<td width="9%" style="height: 18px">
							<%=LocRM.GetString("tRole") %>:
						</td>
						<td width="91%" style="height: 18px">
							<lst:IndentedDropDownList ID="ddRole" runat="server" CssClass="text" Width="190px" AutoPostBack="True" DataTextField="RoleName" DataValueField="RoleId" OnSelectedIndexChanged="ddRole_ChangeGroup">
							</lst:IndentedDropDownList>
						</td>
					</tr>
					<tr>
						<td valign="top" height="28">
							&nbsp;
						</td>
						<td>
							<button id="btnAdd" runat="server" class="text" style="width: 90px" type="button" onserverclick="btnAdd_Click">
							</button>
							<button id="btnSave" runat="server" text="Button" style="display: none" type="button" onserverclick="btnSave_Click">
							</button>
							<button id="btnDelete" type="button" text="Button1" runat="server" style="display: none" onserverclick="btnDelete_Click">
							</button>
						</td>
					</tr>
				</table>
				<!-- End Groups & Users -->
			</td>
			<td width="4">
				&nbsp;
			</td>
			<td valign="top" class="ibn-navframe" height="100%">
				<div style="overflow: auto; height: 240px">
					<!-- Repeater -->
					<table border="0" cellpadding="3" cellspacing="0" class="text" width="100%">
						<asp:Repeater ID="repRoles" runat="server">
							<ItemTemplate>
								<tr>
									<td class="ibn-vb2" style="color: #606060">
										<b>
											<%# DataBinder.Eval(Container.DataItem,"RoleName")%></b>
									</td>
								</tr>
								<tr>
									<td class="ibn-vb2">
										<table border="0" cellpadding="0" cellspacing="1" class="text" width="100%">
											<asp:Repeater ID="repUsers" runat="server">
												<ItemTemplate>
													<tr>
														<td width="30px">
															&nbsp;
														</td>
														<td>
															<%# Mediachase.UI.Web.Util.CommonHelper.GetUserStatusUL(int.Parse(DataBinder.Eval(Container.DataItem,"PrincipalId").ToString()))%>
														</td>
														<td width="25px">
															<%# DataBinder.Eval(Container.DataItem,"Delete")%>
														</td>
													</tr>
												</ItemTemplate>
											</asp:Repeater>
										</table>
										<asp:Label Style="padding-left: 60px" ID="lblNone" runat="server" Text="tutu"></asp:Label>
									</td>
								</tr>
							</ItemTemplate>
						</asp:Repeater>
					</table>
					<!-- End Repeater -->
				</div>
			</td>
		</tr>
	</table>
	<input type="hidden" runat="server" id="hidPrId" name="hidPrId" />
	<input type="hidden" runat="server" id="hidRoleId" name="hidRoleId" />

	<script type="text/javascript">
		//<![CDATA[
		function FuncSave()
		{
			DisableButtons(document.forms[0].<%=btnSave.ClientID%>);
			<%=Page.ClientScript.GetPostBackEventReference(btnSave,"") %>;
		}
	
		function DeleteItem(PrincipalId, RoleId)
		{
			if(confirm('<%=LocRM.GetString("tWarning")%>'))
			{
				document.forms[0].<%=hidPrId.ClientID%>.value = PrincipalId;
				document.forms[0].<%=hidRoleId.ClientID%>.value = RoleId;
				DisableButtons(document.forms[0].<%=btnDelete.ClientID%>);
				<%=Page.ClientScript.GetPostBackEventReference(btnDelete,"") %>;
			}
		}
		//]]>
	</script>

	</form>
</body>
</html>
