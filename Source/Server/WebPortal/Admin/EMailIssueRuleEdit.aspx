<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Page Language="c#" Inherits="Mediachase.UI.Web.Admin.EMailIssueRuleEdit" CodeBehind="EMailIssueRuleEdit.aspx.cs" %>
<%@ Register TagPrefix="btn" Namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" Src="~/Modules/BlockHeader.ascx" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	
	<title><%=sTitle%></title>

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
<body>
	<form id="frmMain" method="post" runat="server" onkeypress="disableEnterKey()" enctype="multipart/form-data">
	<table class="ibn-stylebox2" cellspacing="0" cellpadding="0" width="100%" border="0">
		<tr>
			<td>
				<ibn:BlockHeader ID="secHeader" runat="server" />
			</td>
		</tr>
		<tr>
			<td>
				<table border="0" cellpadding="7" cellspacing="0" width="100%" class="text ibn-alternating ibn-navline">
					<tr>
						<td width="75px">
							<b>
								<%=LocRM.GetString("tType")%>:</b>
						</td>
						<td>
							<asp:DropDownList ID="ddRuleType" runat="server" AutoPostBack="True" Width="250px">
							</asp:DropDownList>
						</td>
					</tr>
				</table>
				<table border="0" cellpadding="7" cellspacing="0" width="95%" class="text" id="tblMain" runat="server">
					<tr>
						<td width="80px" valign="top" style="padding-top: 10">
							<b>
								<%=LocRM.GetString("tKey")%>:</b>
						</td>
						<td width="260px" valign="top">
							<asp:DropDownList ID="ddKey" runat="server" Width="250px">
							</asp:DropDownList>
							<asp:DataGrid runat="server" ID="dgParams" AutoGenerateColumns="False" AllowPaging="False" AllowSorting="False" CellPadding="5" GridLines="None" CellSpacing="0" BorderWidth="0px" Width="250px" ShowHeader="True">
								<Columns>
									<asp:BoundColumn DataField="Position" Visible="False"></asp:BoundColumn>
									<asp:TemplateColumn>
										<ItemStyle CssClass="ibn-vb2" Width="110px"></ItemStyle>
										<ItemTemplate>
											<%# DataBinder.Eval(Container.DataItem, "Name")%>
										</ItemTemplate>
									</asp:TemplateColumn>
									<asp:TemplateColumn>
										<ItemStyle CssClass="ibn-vb2"></ItemStyle>
										<ItemTemplate>
											<asp:TextBox ID="tbValue" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "DefaultValue")%>' Width="110px"></asp:TextBox>
											<asp:RequiredFieldValidator ID="rfFunc" runat="server" ControlToValidate="tbValue" Display="Dynamic" ErrorMessage="*"></asp:RequiredFieldValidator>
										</ItemTemplate>
									</asp:TemplateColumn>
								</Columns>
							</asp:DataGrid>
						</td>
						<td>
							&nbsp;
						</td>
					</tr>
					<tr id="trOperator" runat="server">
						<td>
							&nbsp;
						</td>
						<td>
							<asp:DropDownList ID="ddOperator" runat="server" Width="250px">
							</asp:DropDownList>
						</td>
					</tr>
					<tr id="trValue" runat="server">
						<td valign="top" style="padding-top: 10px">
							<b>
								<%=LocRM.GetString("tValue")%>:</b>
						</td>
						<td valign="top">
							<asp:TextBox ID="txtValue" runat="server" Width="250px"></asp:TextBox>
							<asp:DropDownList ID="ddEmailBoxes" runat="server" Width="250px">
							</asp:DropDownList>
							<asp:DropDownList ID="ddCreator" runat="server" Width="250px">
							</asp:DropDownList>
							<asp:DropDownList ID="ddProject" runat="server" Width="250px">
							</asp:DropDownList>
							<asp:DropDownList ID="ddType" runat="server" Width="250px">
							</asp:DropDownList>
							<asp:DropDownList ID="ddPriority" runat="server" Width="250px">
							</asp:DropDownList>
							<asp:DropDownList ID="ddSeverity" runat="server" Width="250px">
							</asp:DropDownList>
							<asp:ListBox ID="lbGenCats" runat="server" Width="250px" Rows="4" SelectionMode="Multiple"></asp:ListBox>
							<asp:ListBox ID="lbIssCats" runat="server" Width="250px" Rows="4" SelectionMode="Multiple"></asp:ListBox>
						</td>
						<td>
							<asp:Label ID="lblValueError" runat="server" CssClass="text" ForeColor="#ff0000"></asp:Label>&nbsp;
						</td>
					</tr>
					<tr>
						<td>
							<b>
								<%=LocRM.GetString("tIndex")%>:</b>
						</td>
						<td>
							<asp:DropDownList ID="ddIndex" runat="server" Width="50px">
							</asp:DropDownList>
						</td>
						<td>
							&nbsp;
						</td>
					</tr>
					<tr valign="bottom">
						<td align="right" style="padding-right: 10px" colspan="3">
							<btn:IMButton runat="server" class="text" ID="imbSave" style="width: 110px">
							</btn:IMButton>
							&nbsp;
							<btn:IMButton runat="server" class="text" ID="imbCancel" onclick="javascript:window.close();" style="width: 110px" CausesValidation="false">
							</btn:IMButton>
						</td>
					</tr>
				</table>
				<div style="padding: 10 10 0 95;">
					<asp:Label ID="lblNoFunction" runat="server" CssClass="text" ForeColor="red"></asp:Label>
				</div>
			</td>
		</tr>
	</table>

	<script type="text/javascript">
		//<![CDATA[
		function ChangeKey(obj) {
			var _lbl = document.getElementById('<%=lblValueError.ClientID%>');
			var _val = document.getElementById('<%=txtValue.ClientID%>');
			var _boxes = document.getElementById('<%=ddEmailBoxes.ClientID%>');
			var _creat = document.getElementById('<%=ddCreator.ClientID%>');
			var _prj = document.getElementById('<%=ddProject.ClientID%>');
			var _type = document.getElementById('<%=ddType.ClientID%>');
			var _prior = document.getElementById('<%=ddPriority.ClientID%>');
			var _sever = document.getElementById('<%=ddSeverity.ClientID%>');
			var _gencats = document.getElementById('<%=lbGenCats.ClientID%>');
			var _isscats = document.getElementById('<%=lbIssCats.ClientID%>');

			_lbl.style.display = "none";
			_val.style.display = "";
			_boxes.style.display = "none";
			_creat.style.display = "none";
			_prj.style.display = "none";
			_type.style.display = "none";
			_prior.style.display = "none";
			_sever.style.display = "none";
			_gencats.style.display = "none";
			_isscats.style.display = "none";

			var _oper = document.getElementById('<%=ddOperator.ClientID%>');
			var old_value = _oper.value;
			DeleteAll(_oper);

			if (obj.value == "EMailBox") {
				_val.style.display = "none";
				_boxes.style.display = "";
				AddOption2(_oper, '<%=GetRuleType(Mediachase.IBN.Business.EMail.IncidentBoxRuleType.IsEqual)%>', '2');
				AddOption2(_oper, '<%=GetRuleType(Mediachase.IBN.Business.EMail.IncidentBoxRuleType.NotIsEqual)%>', '8');
			}
			else if (obj.value == "CreatorId") {
				_val.style.display = "none";
				_creat.style.display = "";
				AddOption2(_oper, '<%=GetRuleType(Mediachase.IBN.Business.EMail.IncidentBoxRuleType.IsEqual)%>', '2');
				AddOption2(_oper, '<%=GetRuleType(Mediachase.IBN.Business.EMail.IncidentBoxRuleType.NotIsEqual)%>', '8');
			}
			else if (obj.value == "ProjectId") {
				_val.style.display = "none";
				_prj.style.display = "";
				AddOption2(_oper, '<%=GetRuleType(Mediachase.IBN.Business.EMail.IncidentBoxRuleType.IsEqual)%>', '2');
				AddOption2(_oper, '<%=GetRuleType(Mediachase.IBN.Business.EMail.IncidentBoxRuleType.NotIsEqual)%>', '8');
			}
			else if (obj.value == "TypeId") {
				_val.style.display = "none";
				_type.style.display = "";
				AddOption2(_oper, '<%=GetRuleType(Mediachase.IBN.Business.EMail.IncidentBoxRuleType.IsEqual)%>', '2');
				AddOption2(_oper, '<%=GetRuleType(Mediachase.IBN.Business.EMail.IncidentBoxRuleType.NotIsEqual)%>', '8');
			}
			else if (obj.value == "PriorityId") {
				_val.style.display = "none";
				_prior.style.display = "";
				AddOption2(_oper, '<%=GetRuleType(Mediachase.IBN.Business.EMail.IncidentBoxRuleType.IsEqual)%>', '2');
				AddOption2(_oper, '<%=GetRuleType(Mediachase.IBN.Business.EMail.IncidentBoxRuleType.NotIsEqual)%>', '8');
			}
			else if (obj.value == "SeverityId") {
				_val.style.display = "none";
				_sever.style.display = "";
				AddOption2(_oper, '<%=GetRuleType(Mediachase.IBN.Business.EMail.IncidentBoxRuleType.IsEqual)%>', '2');
				AddOption2(_oper, '<%=GetRuleType(Mediachase.IBN.Business.EMail.IncidentBoxRuleType.NotIsEqual)%>', '8');
			}
			else if (obj.value == "GeneralCategories") {
				_val.style.display = "none";
				_gencats.style.display = "";
				AddOption2(_oper, '<%=GetRuleType(Mediachase.IBN.Business.EMail.IncidentBoxRuleType.Contains)%>', '0');
				AddOption2(_oper, '<%=GetRuleType(Mediachase.IBN.Business.EMail.IncidentBoxRuleType.NotContains)%>', '7');
				AddOption2(_oper, '<%=GetRuleType(Mediachase.IBN.Business.EMail.IncidentBoxRuleType.IsEqual)%>', '2');
				AddOption2(_oper, '<%=GetRuleType(Mediachase.IBN.Business.EMail.IncidentBoxRuleType.NotIsEqual)%>', '8');
			}
			else if (obj.value == "IncidentCategories") {
				_val.style.display = "none";
				_isscats.style.display = "";
				AddOption2(_oper, '<%=GetRuleType(Mediachase.IBN.Business.EMail.IncidentBoxRuleType.Contains)%>', '0');
				AddOption2(_oper, '<%=GetRuleType(Mediachase.IBN.Business.EMail.IncidentBoxRuleType.NotContains)%>', '7');
				AddOption2(_oper, '<%=GetRuleType(Mediachase.IBN.Business.EMail.IncidentBoxRuleType.IsEqual)%>', '2');
				AddOption2(_oper, '<%=GetRuleType(Mediachase.IBN.Business.EMail.IncidentBoxRuleType.NotIsEqual)%>', '8');
			}
			else {
				AddOption2(_oper, '<%=GetRuleType(Mediachase.IBN.Business.EMail.IncidentBoxRuleType.Contains)%>', '0');
				AddOption2(_oper, '<%=GetRuleType(Mediachase.IBN.Business.EMail.IncidentBoxRuleType.NotContains)%>', '7');
				AddOption2(_oper, '<%=GetRuleType(Mediachase.IBN.Business.EMail.IncidentBoxRuleType.RegexMatch)%>', '1');
				AddOption2(_oper, '<%=GetRuleType(Mediachase.IBN.Business.EMail.IncidentBoxRuleType.IsEqual)%>', '2');
				AddOption2(_oper, '<%=GetRuleType(Mediachase.IBN.Business.EMail.IncidentBoxRuleType.NotIsEqual)%>', '8');
			}
			if (CheckExistenceValue(_oper, old_value))
				_oper.value = old_value;
		}
		function ChangeKey2(sId) {
			var obj = document.getElementById(sId);
			if (obj)
				ChangeKey(obj);
		}
		//]]>
	</script>

	</form>
</body>
</html>
