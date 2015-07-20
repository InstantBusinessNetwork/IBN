<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Admin.Modules.AddEditWebStubs" CodeBehind="AddEditWebStubs.ascx.cs" %>
<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" Src="~/Modules/BlockHeader.ascx" %>
<%@ Register TagPrefix="btn" Namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>
<%@ Register TagPrefix="cc1" Namespace="Mediachase.FileUploader.Web.UI" Assembly="Mediachase.FileUploader" %>

<script type="text/javascript" src='<%= Page.ResolveUrl("~/Scripts/List2List.js") %>'></script>
<script type="text/javascript">
	//<![CDATA[
	function TextChange() {
		document.getElementById('<%=lblShort.ClientID %>').innerHTML = document.getElementById('<%=txtShort.ClientID %>').value;
	}

	function IconChange() {
		var txtIcon = document.getElementById('<%=txtIcon.ClientID %>');
		var imgIcon = document.getElementById('<%=imgIcon.ClientID %>');
		var lblShort = document.getElementById('<%=lblShort.ClientID %>');

		if (txtIcon.value != "")
			imgIcon.src = txtIcon.value;

		if (imgIcon.src != "") {
			lblShort.style.display = "none";
			imgIcon.style.display = "inline";
		}
		else {
			lblShort.style.display = "";
			imgIcon.style.display = "none";
		}
	}

	function OpenWindowChanged(obj) {
		if (obj.value == "2") {
			document.getElementById('<%=txtWidth.ClientID %>').disabled = false;
			document.getElementById('<%=txtHeight.ClientID %>').disabled = false;
		}
		else {
			document.getElementById('<%=txtWidth.ClientID %>').disabled = true;
			document.getElementById('<%=txtHeight.ClientID %>').disabled = true;
		}
	}

	function GroupExistence(sender, args) {
		if ((document.getElementById('<%=lbSelectedGroups.ClientID%>') != null) && (document.getElementById('<%=lbSelectedGroups.ClientID%>').options.length > 0)) {
			args.IsValid = true;
			return;
		}
		args.IsValid = false;
	}

	function SaveGroups() {
		var sControl = document.getElementById('<%=lbSelectedGroups.ClientID%>');

		var str = "";
		if (sControl != null) {
			for (var i = 0; i < sControl.options.length; i++) {
				str += sControl.options[i].value + ",";
			}
		}
		document.getElementById('<%=iGroups.ClientID%>').value = str;
	}
	//]]>
</script>

<table cellspacing="0" cellpadding="0" border="0" width="100%" class="ibn-stylebox" style="margin-top: 0px">
	<tr>
		<td>
			<ibn:BlockHeader ID="secHeader" runat="server" Title="" />
		</td>
	</tr>
	<tr>
		<td class="ibn-propertysheet">
			<table cellspacing="7" cellpadding="0" width="100%" align="center" border="0">
				<tr>
					<td style="padding-right: 5px" align="left" width="120" class="boldtext">
						<%=LocRM.GetString("Title") %>:
					</td>
					<td>
						<asp:TextBox runat="server" ID="txtTitle" MaxLength="100" Width="200" CssClass="text"></asp:TextBox>
						<asp:RequiredFieldValidator CssClass="text" runat="server" ID="txtTitleRFValidator" ControlToValidate="txtTitle">*</asp:RequiredFieldValidator>
					</td>
				</tr>
				<tr>
					<td style="padding-right: 5px" align="left" width="120" class="boldtext">
						<%=LocRM.GetString("Url") %>:
					</td>
					<td>
						<asp:TextBox runat="server" ID="txtUrl" MaxLength="1000" Width="200" CssClass="text"></asp:TextBox>
						<asp:RequiredFieldValidator CssClass="text" ID="txtUrlRFValidator" runat="server" ControlToValidate="txtUrl">*</asp:RequiredFieldValidator>
					</td>
				</tr>
				<tr>
					<td style="padding-right: 5px" align="left" width="120" class="boldtext">
						<%=LocRM.GetString("ShortName")%>:
					</td>
					<td>
						<table cellpadding="0" cellspacing="0">
							<tr>
								<td>
									<asp:TextBox runat="server" ID="txtShort" MaxLength="2" Width="30" CssClass="text"></asp:TextBox>
								</td>
								<td style="padding-left: 35px">
									<div style='background-image: url(<%=Page.ResolveUrl("~/Layouts/Images/area.gif")%>); overflow: hidden; width: 40px; height: 40px'>
										<table border="0" cellpadding="0" cellspacing="0" style="table-layout: fixed; background-color: transparent; width:40px;height:40px;">
											<tr>
												<td valign="middle" align="center">
													<asp:Label CssClass="ibn-titlearea" runat="server" ID="lblShort"></asp:Label>
													<img runat="server" id="imgIcon" alt="Icon" src="" style="display: none" />
												</td>
											</tr>
										</table>
									</div>
								</td>
							</tr>
						</table>
					</td>
				</tr>
				<tr>
					<td style="padding-right: 5px" align="left" width="120" class="boldtext" valign="top">
						<%=LocRM.GetString("Icon32")%>:
					</td>
					<td class="text">
						<cc1:McHtmlInputFile ID="txtIcon" runat="server" Width="200" class="text"></cc1:McHtmlInputFile>
						<br />
						<asp:CheckBox runat="server" ID="DeleteIconCheckBox" />
					</td>
				</tr>
				<tr>
					<td style="padding-right: 5px" align="left" width="120" class="boldtext">
						<%=LocRM.GetString("WindowType")%>:
					</td>
					<td>
						<asp:DropDownList runat="server" ID="lstOpenWindow" Width="132" onchange="OpenWindowChanged(this)">
						</asp:DropDownList>
					</td>
				</tr>
				<tr>
					<td style="padding-right: 5px" align="left" width="120" class="boldtext">
						<%=LocRM.GetString("Size")%>:
					</td>
					<td class="text">
						<asp:TextBox runat="server" ID="txtWidth" Width="40" MaxLength="4" Enabled="False" CssClass="text">300</asp:TextBox>
						X
						<asp:TextBox runat="server" ID="txtHeight" Width="40" MaxLength="4" Enabled="False" CssClass="text">250</asp:TextBox>
					</td>
				</tr>
				<tr>
					<td width="100">
					</td>
					<td>
						<asp:CheckBox CssClass="text" runat="server" ID="chkInternal" Text="Internal Link" Visible="False"></asp:CheckBox>
					</td>
				</tr>
				<tr id="trGroups" runat="server">
					<td valign="top" style="padding-right: 5px" width="120" align="left" class="boldtext">
						<%=LocRM.GetString("Groups")%>:
					</td>
					<td>
						<table class="text" id="tblGroups" style="height: 150px" cellpadding="0" border="0">
								<tr>
									<td valign="top" width="45%" style="padding-right: 6px; padding-bottom: 6px;white-space:nowrap;">
										<%=LocRM.GetString("Available")%>
										<br />
										<asp:ListBox ID="lbAvailableGroups" runat="server" CssClass="text" Height="90%" Width="100%"></asp:ListBox>
									</td>
									<td style="padding-right: 6px; padding-left: 6px; padding-bottom: 6px">
										<asp:Button ID="btnAddOneGr" Style="margin: 1px" runat="server" CssClass="text" Width="30px" CausesValidation="False" Text=">"></asp:Button><br />
										<asp:Button ID="btnAddAllGr" Style="margin: 1px" runat="server" CssClass="text" Width="30px" CausesValidation="False" Text=">>"></asp:Button><br />
										<br />
										<asp:Button ID="btnRemoveOneGr" Style="margin: 1px" runat="server" CssClass="text" Width="30px" CausesValidation="False" Text="<"></asp:Button><br />
										<asp:Button ID="btnRemoveAllGr" Style="margin: 1px" runat="server" CssClass="text" Width="30px" CausesValidation="False" Text="<<"></asp:Button>
										<p>
										</p>
									</td>
									<td valign="top" width="45%" style="padding-right: 20px; padding-left: 6px; padding-bottom: 6px">
										<%=LocRM.GetString("Selected")%>
										<br />
										<asp:ListBox ID="lbSelectedGroups" runat="server" CssClass="text" Height="90%" BorderWidth="1" Width="97%"></asp:ListBox>
										<asp:CustomValidator ID="GroupValidator" Style="vertical-align: top" runat="server" ClientValidationFunction="GroupExistence" ErrorMessage="*"></asp:CustomValidator>
									</td>
								</tr>
							</table>
					</td>
				</tr>
				<tr>
					<td width="120">
					</td>
					<td>
						<btn:IMButton class="text" ID="btnSubmit" runat="server" style="width: 110px;" Text="Save" OnServerClick="btnSubmit_Click">
						</btn:IMButton>
						&nbsp;&nbsp;
						<btn:IMButton class="text" CausesValidation="false" style="width: 110px; margin-left: 10px" ID="btnCancel" runat="server" Text="" IsDecline="true" OnServerClick="btnCancel_Click">
						</btn:IMButton>
					</td>
				</tr>
			</table>
		</td>
	</tr>
</table>
<input id="iGroups" style="visibility: hidden" runat="server" name="iGroups" />
<script type="text/javascript">
	//<![CDATA[
	var txtShort = document.getElementById('<%=txtShort.ClientID%>');
	var txtIcon = document.getElementById('<%=txtIcon.ClientID%>');

	if (txtShort.addEventListener) {
		txtShort.addEventListener("input", TextChange, false);
		txtIcon.addEventListener("input", IconChange, false);
		IconChange();
	}
	else {
		// For IE:
		txtShort.onpropertychange = TextChange;
		txtIcon.onpropertychange = IconChange;
	}
	//]]>
</script>