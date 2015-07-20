<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Tasks.Modules.TaskEdit" Codebehind="TaskEdit.ascx.cs" %>
<%@ Reference Control="~/Modules/TimeControl.ascx" %>
<%@ Reference Control="~/Apps/Common/DateTimePickerAjax/PickerControl.ascx" %>
<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Reference Control="~/Modules/BlockHeaderLightWithMenu.ascx" %>
<%@ Reference Control="~/Modules/MetaDataInternalEditControl.ascx" %>
<%@ Register TagPrefix="cc1" Namespace="Mediachase.FileUploader.Web.UI" Assembly="Mediachase.FileUploader" %>
<%@ Register TagPrefix="btn" namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="~/Modules/BlockHeader.ascx" %>
<%@ Register TagPrefix="mc" TagName="Picker" Src="~/Apps/Common/DateTimePickerAjax/PickerControl.ascx" %>
<%@ Register TagPrefix="ibn" TagName="Time" src="~/Modules/TimeControl.ascx" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeaderLight" src="~/Modules/BlockHeaderLightWithMenu.ascx" %>
<%@ Register TagPrefix="ibn" TagName="MetaDataInternalEditControl" src="~/Modules/MetaDataInternalEditControl.ascx" %>
<script type="text/javascript">
<!--
	function AddCategory(CategoryType)
	{
		var w = 640;
		var h = 350;
		var l = (screen.width - w) / 2;
		var t = (screen.height - h) / 2;
		winprops = 'resizable=0, height='+h+',width='+w+',top='+t+',left='+l;
		var f = window.open('../Common/AddCategory.aspx?BtnID=<%=btnRefresh.ClientID %>&DictType=' + CategoryType, "AddCategory", winprops);
	}			
	
	function ShowHidePhase(chkObj, trId, ddlId)
	{
		var trObj = document.getElementById(trId);
		var ddlObj = document.getElementById(ddlId);
		if (chkObj.checked)
		{
			if (trObj != null)
				trObj.style.visibility = "visible";
			if (ddlObj != null)
				ddlObj.style.visibility = "visible";
		}
		else
		{
			if (trObj != null)
				trObj.style.visibility = "hidden";
			if (ddlObj != null)
				ddlObj.style.visibility = "hidden";
		}
	}
//-->
</script>
<table align=center width=700 cellpadding=3 cellspacing=0 class="ibn-alerttext" id="tblWarning" style="display:none">
	<tr>
		<td width=20 align=center valign=center>
			<asp:Image ID="imgCaution" Runat=server ImageUrl="~/Layouts/Images/warning.gif" Width=16 Height=16 ImageAlign="AbsMiddle"></asp:Image>
		</td>
		<td><%=LocRM.GetString("AssignWarning") %>&nbsp;<a href="#" id="aAssignLink"></a>
		</td>
	</tr>
</table>
<table class="ibn-stylebox" cellspacing="0" cellpadding="0" width="100%" border="0" style="margin-top:1px">
	<tr>
		<td colspan=2><ibn:blockheader id="tbSave" runat="server"></ibn:blockheader></td>
	</tr>
	<tr>
		<td style="padding:5px" width="50%" valign=top>
			<ibn:BlockHeaderLight id="hdrBasicInfo" runat="server" CollapsibleControlId="BasicInfoTable" />
				<table class="ibn-stylebox-light text" cellspacing="0" cellpadding="7" width="100%" border="0" runat="server" id="BasicInfoTable">
					<tr>
						<td width="120px"><asp:label id="lblProjectTitle" Runat="server" CssClass="boldtext"></asp:label>:</td>
						<td><asp:label id="lblProject" runat="server" CssClass="text" Font-Bold="True"></asp:label></td>
					</tr>
					<tr>
						<td><asp:label id="lblManagerTitle" Runat="server" CssClass="boldtext"></asp:label>:</td>
						<td><asp:label id="lblManager" runat="server" CssClass="text"></asp:label></td>
					</tr>
					<tr>
						<td><asp:label id="lblTitleTitle" CssClass="boldtext" Runat="server"></asp:label>:</td>
						<td><asp:textbox id="txtTitle" runat="server" CssClass="text" Width="99%" MaxLength="255"></asp:textbox><asp:requiredfieldvalidator id="RequiredFieldValidator1" runat="server" ErrorMessage="*" ControlToValidate="txtTitle" Display="Dynamic"></asp:requiredfieldvalidator></td>
					</tr>
					<tr>
						<td vAlign="top"><asp:label id="lblDescriptionTitle" Runat="server" CssClass="boldtext"></asp:label>:
						</td>
						<td><asp:textbox id="txtDescription" runat="server" CssClass="text" Width="99%" TextMode="MultiLine" Height="127px"></asp:textbox></td>
					</tr>
					<tr id="trLoad" runat="server">
						<td><asp:label id="lblFileLoad" Runat="server" CssClass="boldtext"></asp:label>:</td>
						<td>
							<cc1:mchtmlinputfile id="fAssetFile" runat="server" CssClass="text" Size="40" Width="99%"></cc1:mchtmlinputfile>
							<span id="vFile" runat="server" class="text" style="COLOR:red">*</span>
						</td>
					</tr>
				</table>
		</td>
		<td style="padding:5px" width="50%" valign=top>
			<ibn:BlockHeaderLight id="hdrStatusInfo" runat="server" CollapsibleControlId="StatusInfoTable" />
				<table class="ibn-stylebox-light text" cellspacing="0" cellpadding="7" width="100%" border="0" runat="server" id="StatusInfoTable">
					<tr id="trPriority" runat="server">
						<td width="120px"><asp:label id="lblPriorityTitle" Runat="server" CssClass="boldtext"></asp:label>:</td>
						<td><asp:dropdownlist id="ddlPriority" runat="server" Width="280px"></asp:dropdownlist></td>
					</tr>
					<tr>
						<td><asp:label id="lblStartDateTitle" Runat="server" CssClass="boldtext"></asp:label>:</td>
						<td><mc:Picker ID="dtcStartDate" runat="server" DateCssClass="text" TimeCssClass="text" DateWidth="85px" TimeWidth="60px" ShowImageButton="false" ShowTime="true" DateIsRequired="true" /></td>
					</tr>
					<tr>
						<td><asp:label id="lblEndDateTitle" Runat="server" CssClass="boldtext"></asp:label>:</td>
						<td><mc:Picker ID="dtcEndDate" runat="server" DateCssClass="text" TimeCssClass="text" DateWidth="85px" TimeWidth="60px" ShowImageButton="false" ShowTime="true" DateIsRequired="true" /><asp:CustomValidator id="CustomValidator1" runat="server" Display="Dynamic" ErrorMessage="Wrong Date"></asp:CustomValidator></td>
					</tr>
					<tr id="trTaskTime" runat="server">
						<td><asp:label id="lblTaskTimeTitle" Runat="server" CssClass="boldtext"></asp:label>:</td>
						<td>
							<ibn:Time id="ucTaskTime" ShowTime="HM" HourSpinMaxValue="9999" ViewStartDate="True" runat="server" />
						</td>
					</tr>
					<tr id="trActivation" runat="server">
						<td><asp:label id="lblActivationTitle" Runat="server" CssClass="boldtext"></asp:label>:</td>
						<td>
							<asp:dropdownlist id="ddlActivationType" runat="server" Width="280px"></asp:dropdownlist>
						</td>
					</tr>
					<tr id="trCompletion" runat="server">
						<td><asp:label id="lblCompletionTitle" Runat="server" CssClass="boldtext"></asp:label>:</td>
						<td>
							<asp:dropdownlist id="ddlCompletionType" runat="server" Width="280px"></asp:dropdownlist>
						</td>
					</tr>
					<tr id="trMustConfirm" runat="server">
						<td></td>
						<td>
							<asp:checkbox id="chbMustBeConfirmed" runat="server" Text="Must be confirmed by Manager"></asp:checkbox>
						</td>
					</tr>
					<tr>
						<td></td>
						<td><asp:checkbox id="chbMilestone" runat="server" Text="Is Milestone"></asp:checkbox></td>
					</tr>
					<tr runat="server" id="trPhase">
						<td><asp:label id="lblPhaseTitle" Runat="server" CssClass="boldtext"></asp:label>:</td>
						<td>
							<asp:dropdownlist id="ddlPhase" runat="server" Width="280px"></asp:dropdownlist>
						</td>
					</tr>
				</table>
		</td>
	</tr>
	<tr>
		<td style="padding:5px" width="100%" valign="top" colspan="2">
			<ibn:BlockHeaderLight id="hdrCategoryInfo" runat="server" CollapsibleControlId="CategoryInfoTable" />
				<table class="ibn-stylebox-light text" cellspacing="0" cellpadding="5" width="100%" border="0" runat="server" id="CategoryInfoTable">
					<tr>
						<td width="50%" valign=top>
							<table id="tblCategory" runat="server" class="text" cellspacing="3" cellpadding="3" width="100%" border="0">
								<tr>
									<td width="120" vAlign="top"><asp:label id="lblCategoryTitle" Runat="server" CssClass="text ibn-label"></asp:label>:</td>
									<td><asp:listbox id="lbCategory" runat="server" Width="200px" Rows="5" SelectionMode="Multiple"></asp:listbox><br>
									<div align="right" style="width:200px">
									<button id="btnAddGeneralCategory" runat="server" style="border:0px;padding:0px;position:relative;top:0px;left:0px;height:20px;width:22px;background-color:transparent" type="button"><IMG height="20" src="../layouts/images/icons/dictionary_edit.gif" width="22" border="0"></button>
									</div></td>
								</tr>
							</table>
						</td>
						<td width="50%" valign=top>
							<table class="text" cellspacing="3" cellpadding="3" width="100%" border="0">
								<tr>
									<td valign="top" style="width:110px"><asp:label id="lblTeam" Runat="server" CssClass="text ibn-label"></asp:label>:</td>
									<td>
										<div style="height: 75px; overflow-y: auto; border: inset 2px white; margin-top: 5px; width:250px;">
											<asp:DataGrid ID="ResourcesGrid" runat="server" AllowSorting="False" AllowPaging="False" Width="100%" AutoGenerateColumns="False" BorderWidth="0" GridLines="None" CellPadding="0" CellSpacing="0" ShowFooter="False" ShowHeader="False">
												<Columns>
													<asp:BoundColumn Visible="false" DataField="UserId"></asp:BoundColumn>
													<asp:TemplateColumn ItemStyle-CssClass="text">
														<ItemTemplate>
															<asp:CheckBox runat="server" ID="chkItem" Text='<%# Eval("LastName").ToString() + " " + Eval("FirstName").ToString() %>' Checked='<%# TaskResources.Contains((int)Eval("UserId"))%>'></asp:CheckBox>
														</ItemTemplate>
													</asp:TemplateColumn>
												</Columns>
											</asp:DataGrid>
										</div>
										<asp:CheckBox id="cbConfirmed" runat="server"></asp:CheckBox></td>
								</tr>
							</table>
						</td>
					</tr>
				</table>
		</td>
	</tr>
	<tr>
		<td colspan="2">
			<ibn:MetaDataInternalEditControl runat="server" id="EditControl" MetaClassName="TaskEx"></ibn:MetaDataInternalEditControl>
		</td>
	</tr>
	<tr>
		<td colspan="2" align="right" style="PADDING-RIGHT:7px" height="50" valign="center">
			<table><tr><td>
			<btn:ImButton ID="btnSaveAssign" Runat="server" Class="text" onserverclick="btnSave_ServerClick"></btn:ImButton>&nbsp;&nbsp;
			<btn:ImButton ID="btnSave" Runat="server" Class="text" style="width:115px;" onserverclick="btnSave_ServerClick"></btn:ImButton>&nbsp;&nbsp;
			<btn:ImButton ID="btnCancel" CausesValidation="false" Runat="server" Class="text" style="width:115px;" IsDecline="true" onserverclick="btnCancel_ServerClick"></btn:ImButton>
			</td></tr><tr><td>
			<asp:CheckBox Checked=False CssClass="text" Runat=server ID="cbOneMore"></asp:CheckBox>
			</td></tr></table>
		</td>
	</tr>
</table>
<asp:Button id="btnRefresh" CausesValidation="False" runat="server" Text="Button" style="visibility:hidden;"></asp:Button>

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

