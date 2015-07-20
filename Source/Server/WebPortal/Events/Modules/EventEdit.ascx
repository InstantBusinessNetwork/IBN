<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Events.Modules.EventEdit" CodeBehind="EventEdit.ascx.cs" %>
<%@ Reference Control="~/Modules/ObjectDropDownNew.ascx" %>
<%@ Reference Control="~/Apps/MetaUIEntity/Modules/EntityDropDown.ascx" %>
<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Reference Control="~/Modules/BlockHeaderLightWithMenu.ascx" %>
<%@ Reference Control="~/Apps/Common/DateTimePickerAjax/PickerControl.ascx" %>
<%@ Reference Control="~/Modules/MetaDataInternalEditControl.ascx" %>
<%@ Register TagPrefix="mc" TagName="Picker" Src="~/Apps/Common/DateTimePickerAjax/PickerControl.ascx" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" Src="~/Modules/BlockHeader.ascx" %>
<%@ Register TagPrefix="btn" namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>
<%@ Register TagPrefix="cc1" Namespace="Mediachase.FileUploader.Web.UI" Assembly="Mediachase.FileUploader" %>
<%@ Register TagPrefix="ibn" TagName="ObjectDD" Src="~/Modules/ObjectDropDownNew.ascx" %>
<%@ Register TagPrefix="ibn" TagName="EntityDD" Src="~/Apps/MetaUIEntity/Modules/EntityDropDown.ascx" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeaderLight" Src="~/Modules/BlockHeaderLightWithMenu.ascx" %>
<%@ Register TagPrefix="ibn" TagName="MetaDataInternalEditControl" src="~/Modules/MetaDataInternalEditControl.ascx" %>

<script language="javascript" type="text/javascript">
<!--
	function AddCategory(CategoryType) {
		var w = 640;
		var h = 350;
		var l = (screen.width - w) / 2;
		var t = (screen.height - h) / 2;
		winprops = 'resizable=0, height=' + h + ',width=' + w + ',top=' + t + ',left=' + l;
		var f = window.open('../Common/AddCategory.aspx?BtnID=<%=btnRefresh.ClientID %>&DictType=' + CategoryType, "AddCategory", winprops);
	}			

//-->
</script>

<table align="center" width="700" cellpadding="3" cellspacing="0" class="ibn-alerttext"
	id="tblWarning" style="display: none">
	<tr>
		<td width="20" align="center" valign="center">
			<asp:Image ID="imgCaution" runat="server" ImageUrl="~/Layouts/Images/warning.gif"
				Width="16" Height="16" ImageAlign="AbsMiddle"></asp:Image>
		</td>
		<td>
			<%=LocRM.GetString("AssignWarning") %>&nbsp;<a href="#" id="aAssignLink"></a>
		</td>
	</tr>
</table>
<table class="ibn-stylebox" style="margin-top: 0px" cellspacing="0" cellpadding="0"
	width="100%" border="0">
	<tr>
		<td colspan="2">
			<ibn:BlockHeader ID="tbSave" runat="server"></ibn:BlockHeader>
		</td>
	</tr>
	<tr>
		<td style="padding: 5px" width="50%" valign="top">
			<asp:Panel ID="apShared" Style="padding-right: 5px; padding-left: 5px; padding-bottom: 5px;
				padding-top: 5px;" CssClass="ibn-propertysheet ibn-navline ibn-alternating" runat="server">
				<img height="16" src="../Layouts/images/caution.gif" width="16" align="absMiddle"
					border="0">&nbsp;<%=LocRM.GetString("SharedEntry") %>
				<asp:Label ID="lblEntryOwner" runat="server"></asp:Label>
			</asp:Panel>
			<ibn:BlockHeaderLight ID="hdrBasicInfo" runat="server" CollapsibleControlId="BasicInfoTable" />
			<table class="ibn-stylebox-light text" cellspacing="0" cellpadding="7" width="100%" border="0" runat="server" id="BasicInfoTable">
				<tr>
					<td class="ibn-label">
						<asp:Label ID="lblTitleTitle" runat="server" CssClass="text"></asp:Label>:
					</td>
					<td class="ibn-value">
						<asp:TextBox ID="txtTitle" runat="server" CssClass="text" Width="260"></asp:TextBox>
						<asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ErrorMessage="*"
							ControlToValidate="txtTitle" Display="Static"></asp:RequiredFieldValidator>
					</td>
				</tr>
				<tr>
					<td valign="top" class="ibn-label">
						<asp:Label ID="lblDescriptionTitle" runat="server" CssClass="text"></asp:Label>:
					</td>
					<td valign="top" class="ibn-value">
						<asp:TextBox ID="txtDescription" runat="server" CssClass="text" Width="260" TextMode="MultiLine"
							Height="150px"></asp:TextBox>
					</td>
				</tr>
				<tr>
					<td class="ibn-label">
						<asp:Label ID="lblLocationTitle" runat="server" CssClass="text"></asp:Label>:
					</td>
					<td class="ibn-value">
						<asp:TextBox ID="txtLocation" runat="server" CssClass="text" Width="260"></asp:TextBox>
					</td>
				</tr>
				<tr>
					<td class="ibn-label">
						<asp:Label ID="lblTypeTitle" runat="server" CssClass="text"></asp:Label>:
					</td>
					<td class="ibn-value">
						<asp:DropDownList ID="ddlType" runat="server" CssClass="text" Width="260"></asp:DropDownList>
					</td>
				</tr>
				<tr>
					<td class="ibn-label">
						<asp:Label ID="ManagerLabel" runat="server" CssClass="text"></asp:Label>:
					</td>
					<td>
						<asp:DropDownList ID="ManagerList" runat="server" CssClass="text" Width="260"></asp:DropDownList>
					</td>
				</tr>
				<tr id="trFileLoader" runat="server">
					<td class="ibn-label">
						<asp:Label ID="lblFileLoad" CssClass="text" runat="server"></asp:Label>:
					</td>
					<td class="ibn-value">
						<cc1:McHtmlInputFile ID="fAssetFile" runat="server" CssClass="text" Width="260">
						</cc1:McHtmlInputFile>
						<span id="vFile" runat="server" class="text" style="color: red">*</span>
					</td>
				</tr>
			</table>
		</td>
		<td style="padding: 5px" width="50%" valign="top">
			<ibn:BlockHeaderLight ID="hdrCategoryInfo" runat="server" CollapsibleControlId="CategoryInfoTable" />
			<table class="ibn-stylebox-light text" cellspacing="0" cellpadding="7" width="100%" border="0" runat="server" id="CategoryInfoTable">
				<tr id="trProject" runat="server">
					<td class="ibn-label">
						<asp:Label ID="lblProjectTitle" runat="server" CssClass="text"></asp:Label>:
					</td>
					<td class="ibn-value">
						<ibn:ObjectDD ID="ucProject" ObjectTypes="3" runat="server" Width="230px" />
					</td>
				</tr>
				<tr>
					<td class="ibn-label">
						<asp:Label ID="lblStartDateTitle" runat="server" CssClass="text"></asp:Label>:
					</td>
					<td class="ibn-value">
						<mc:Picker ID="dtcStartDate" runat="server" DateCssClass="text" TimeCssClass="text"
							DateWidth="85px" TimeWidth="60px" ShowImageButton="false" ShowTime="true" DateIsRequired="true" />
					</td>
				</tr>
				<tr>
					<td class="ibn-label">
						<asp:Label ID="lblEndDateTitle" runat="server" CssClass="text"></asp:Label>:
					</td>
					<td class="ibn-value">
						<mc:Picker ID="dtcEndDate" runat="server" DateCssClass="text" TimeCssClass="text"
							DateWidth="85px" TimeWidth="60px" ShowImageButton="false" ShowTime="true" DateIsRequired="true" />
						<asp:CustomValidator ID="CustomValidator1" runat="server" ErrorMessage="Wrong Date"
							Display="Dynamic"></asp:CustomValidator>
					</td>
				</tr>
				<tr id="trPriority" runat="server">
					<td class="ibn-label">
						<asp:Label ID="lblPriorityTitle" runat="server" CssClass="text"></asp:Label>:
					</td>
					<td class="ibn-value">
						<asp:DropDownList ID="ddlPriority" runat="server" CssClass="text" Width="230">
						</asp:DropDownList>
					</td>
				</tr>
				<tr id="trClient" runat="server">
					<td class="ibn-label">
						<asp:Label ID="lblClient" CssClass="text" runat="server"></asp:Label>:
					</td>
					<td class="ibn-value">
						<ibn:EntityDD ID="ClientControl" ObjectTypes="Contact,Organization" runat="server"
							Width="230px" />
					</td>
				</tr>
				<tr id="trCategories" runat="server">
					<td valign="top" class="ibn-label">
						<asp:Label ID="lblCategoriesTitle" runat="server" CssClass="text"></asp:Label>:
					</td>
					<td valign="top" class="ibn-value">
						<asp:ListBox ID="lbCategory" runat="server" CssClass="text" Width="230" Height="100px"
							SelectionMode="Multiple"></asp:ListBox>
						<div align="right" style="margin-right: 0px; margin-top: 0px; width: 230px;">
							<button id="btnAddGeneralCategory" runat="server" style="border: 0px; padding: 0px;
								position: relative; top: 0px; left: 0px; height: 20px; width: 22px; background-color: transparent"
								type="button">
								<img height="20" width="22" border="0" src="../layouts/images/icons/dictionary_edit.gif"></button>
						</div>
					</td>
				</tr>
			</table>
		</td>
	</tr>
	<tr>
		<td colspan="2">
			<ibn:MetaDataInternalEditControl runat="server" id="EditControl" MetaClassName="EventsEx"></ibn:MetaDataInternalEditControl>
		</td>
	</tr>
	<tr>
		<td valign="center" align="right" colspan="2" height="60">
			<table>
				<tr>
					<td>
						<btn:IMButton class="text" ID="btnSaveAssign" runat="server" OnServerClick="btnSave_ServerClick"></btn:IMButton>
						&nbsp;&nbsp;
						<btn:IMButton class="text" ID="btnSave" style="width: 110px;" runat="server" OnServerClick="btnSave_ServerClick"></btn:IMButton>
						&nbsp;
						<btn:IMButton class="text" ID="btnCancel" style="width: 110px;" runat="server" IsDecline="true" CausesValidation="false" OnServerClick="btnCancel_ServerClick"></btn:IMButton>
					</td>
				</tr>
				<tr>
					<td>
						<asp:CheckBox Checked="False" CssClass="text" runat="server" ID="cbOneMore"></asp:CheckBox>
					</td>
				</tr>
			</table>
		</td>
	</tr>
</table>
<asp:Button ID="btnRefresh" CausesValidation="False" runat="server" Text="Button"
	Style="visibility: hidden;"></asp:Button>
<div align="center">
	<input id="txtManagerId" style="visibility: hidden" name="iGroups" runat="server">
</div>

<script language="javascript">
	function ShowProgress()
	{
	 	if (document.forms[0].<%=fAssetFile.ClientID %> && document.forms[0].<%=fAssetFile.ClientID %>.value!="" && document.forms[0].<%=txtTitle.ClientID %>.value!="")
	 	{
	 		if(!browseris.nav)
	 		{
	 			document.forms[0].<%=btnSaveAssign.ClientID%>.disabled=true
	 			document.forms[0].<%=btnSave.ClientID%>.disabled=true
	 			document.forms[0].<%=btnCancel.ClientID%>.disabled=true
	 		}
			var w = 300;
			var h = 140;
			var l = (screen.width - w) / 2;
			var t = (screen.height - h) / 2;
			winprops = 'resizable=0, height='+h+',width='+w+',top='+t+',left='+l;
			var f = window.open('../External/Progress.aspx?ID='+document.forms[0].__MEDIACHASE_FORM_UNIQUEID.value, "_blank", winprops);
	 	}
	}
	
</script>

