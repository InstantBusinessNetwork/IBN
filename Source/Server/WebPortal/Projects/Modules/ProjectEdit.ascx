<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Projects.Modules.ProjectEdit" Codebehind="ProjectEdit.ascx.cs" %>
<%@ Reference Control="~/Apps/MetaUIEntity/Modules/EntityDropDown.ascx" %>
<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Reference Control="~/Modules/BlockHeaderLightWithMenu.ascx" %>
<%@ Reference Control="~/Apps/Common/DateTimePickerAjax/PickerControl.ascx" %>
<%@ Reference Control="~/Modules/MetaDataInternalEditControl.ascx" %>
<%@ Register TagPrefix="mc" TagName="Picker" Src="~/Apps/Common/DateTimePickerAjax/PickerControl.ascx" %>
<%@ Register TagPrefix="btn" namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="~/Modules/BlockHeader.ascx" %>
<%@ Register TagPrefix="ibn" TagName="EntityDD" src="~/Apps/MetaUIEntity/Modules/EntityDropDown.ascx" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeaderLight" src="~/Modules/BlockHeaderLightWithMenu.ascx" %>
<%@ Register TagPrefix="ibn" TagName="MetaDataInternalEditControl" src="~/Modules/MetaDataInternalEditControl.ascx" %>
<script type="text/javascript">
	function AddCategory(CategoryType,ButtonId)
	{
			var w = 640;
			var h = 350;
			var l = (screen.width - w) / 2;
			var t = (screen.height - h) / 2;
			winprops = 'resizable=0, height='+h+',width='+w+',top='+t+',left='+l;
			var f = window.open('../Common/AddCategory.aspx?BtnID='+ButtonId+'&DictType=' + CategoryType, "AddCategory", winprops);
	}
</script>
<table align="center" width="700px" cellpadding="3" cellspacing="0" class="ibn-alerttext" id="tblWarning" style="display:none">
	<tr>
		<td width=20 align="center" valign="center">
			<asp:Image ID="imgCaution" Runat="server" ImageUrl="~/Layouts/Images/warning.gif" Width="16" Height="16" ImageAlign="AbsMiddle"></asp:Image>
		</td>
		<td>
			<%=LocRM.GetString("AssignWarning") %>&nbsp;<a href="#" id="aAssignLink"></a>
		</td>
	</tr>
</table>
<table class="ibn-stylebox" cellspacing="0" cellpadding="0" width="100%" style="margin-top:0px" border="0">
	<tr>
		<td colspan="2"><ibn:blockheader id="tbSave" runat="server"></ibn:blockheader></td>
	</tr>
	<tr>
		<td style="padding:5px" width="50%" valign="top">
			<ibn:BlockHeaderLight id="hdrBasicInfo" runat="server" CollapsibleControlId="BasicInfoTable" />
				<table class="ibn-stylebox-light text" cellspacing="0" cellpadding="7" width="100%" border="0" runat="server" id="BasicInfoTable" style="width:100%;">
					<tr>
						<td style="width:220px"></td>
						<td></td>
					</tr>
					<tr>
						<td class="ibn-value" colspan="2">
						  <asp:label id="lblTitleTitle" CssClass="boldtext" EnableViewState="False" Runat="server"></asp:label>:
						  <asp:requiredfieldvalidator id="TitleValidator2" runat="server" CssClass="text" Display="Dynamic" ErrorMessage="*" ControlToValidate="txtTitle"></asp:requiredfieldvalidator>
						  <br />
						  <asp:textbox id="txtTitle" runat="server" CssClass="text" Width="100%"></asp:textbox>
						</td>
					</tr>
					<tr runat="server" id="trTemplate">
						<td style="width:220px"><asp:label id="lblTemplateLabel" EnableViewState="False" CssClass="boldtext" Runat="server"></asp:label>:</td>
						<td><asp:DropDownList ID="ddlTemplate" Runat="server" Width="99%" AutoPostBack="True"></asp:DropDownList></td>
					</tr>
					<tr>
						<td style="width:220px"><asp:label id="lblManagerTitle" EnableViewState="False" CssClass="boldtext" Runat="server"></asp:label>:</td>
						<td><asp:dropdownlist id="ddlManager" runat="server" Width="99%"></asp:dropdownlist><asp:label id="lblManager" runat="server" Visible="False"></asp:label></td>
					</tr>
					<tr>
						<td style="width:220px"><asp:label id="lblExecManagerTitle" EnableViewState="False" CssClass="boldtext" Runat="server"></asp:label>:</td>
						<td><asp:dropdownlist id="ddlExecManager" runat="server" Width="99%"></asp:dropdownlist></td>
					</tr>
					<tr>
						<td style="width:220px"><asp:label id="lblCalendarTitle" EnableViewState="False" CssClass="boldtext" Runat="server"></asp:label>:</td>
						<td><asp:dropdownlist id="ddlCalendar" runat="server" Width="99%"></asp:dropdownlist></td>
					</tr>
					<tr>
						<td style="width:220px"><asp:label id="lblCurrencySymbol" Runat="server" CssClass="boldtext" EnableViewState="False"></asp:label>:</td>
						<td><asp:dropdownlist id="ddCurrency" runat="server" Width="99%"></asp:dropdownlist></td>
					</tr>
					<tr>
						<td style="width:220px"><asp:label id="lblTypeTitle" EnableViewState="False" CssClass="boldtext" Runat="server"></asp:label>:</td>
						<td><asp:dropdownlist id="ddlType" runat="server" Width="99%" AutoPostBack="true" 
								onselectedindexchanged="ddlType_SelectedIndexChanged"></asp:dropdownlist></td>
					</tr>
					<tr>
						<td style="width:220px"><asp:label id="lblBlockType" EnableViewState="False" CssClass="boldtext" Runat="server"></asp:label>:</td>
						<td><asp:dropdownlist id="ddlBlockType" runat="server" Width="99%"></asp:dropdownlist></td>
					</tr>
				</table>
		</td>
		<td style="padding:5px" width="50%" valign=top>
			<ibn:BlockHeaderLight id="hdrTimeline" runat="server" CollapsibleControlId="TimelineTable" />
			<table class="ibn-stylebox-light text" cellspacing="0" cellpadding="7" width="100%" border="0" runat="server" id="TimelineTable">
				<tr>
					<td style="width:220px"></td>
					<td></td>
				</tr>
				<tr>
					<td><asp:label id="lblTargetStartDateTitle" EnableViewState="False" CssClass="boldtext" Runat="server"></asp:label>:</td>
					<td><mc:Picker ID="dtcTargetStartDate" runat="server" DateCssClass="text" DateWidth="85px" ShowImageButton="false" DateIsRequired="true" /></td>
				</tr>
				<tr>
					<td><asp:label id="lblTargetEndDateTitle" EnableViewState="False" CssClass="boldtext" Runat="server"></asp:label>:</td>
					<td><mc:Picker ID="dtcTargetEndDate" runat="server" DateCssClass="text" DateWidth="85px" ShowImageButton="false" DateIsRequired="true" /><asp:customvalidator id="CustomValidator1" runat="server" Display="Dynamic" ErrorMessage="*"></asp:customvalidator></td>
				</tr>
				<tr>
					<td><asp:label id="lblActualStartDateTitle" EnableViewState="False" CssClass="boldtext" Runat="server"></asp:label>:</td>
					<td><mc:Picker ID="dtcActualStartDate" runat="server" DateCssClass="text" DateWidth="85px" ShowImageButton="false" /></td>
				</tr>
				<tr>
					<td><asp:label id="lblActualFinishDateTitle" EnableViewState="False" CssClass="boldtext" Runat="server"></asp:label>:</td>
					<td><mc:Picker ID="dtcActualFinishDate" runat="server" DateCssClass="text" DateWidth="85px" ShowImageButton="false" /><asp:customvalidator id="CustomValidator2" runat="server" Display="Dynamic" ErrorMessage="*"></asp:customvalidator></td>
				</tr>
			</table>
			<ibn:BlockHeaderLight id="hdrStatusInfo" runat="server" CollapsibleControlId="StatusInfoTable"/>
			<table class="ibn-stylebox-light text" cellspacing="0" cellpadding="7" width="100%" border="0" runat="server" id="StatusInfoTable">
				<tr>
					<td style="width:220px"></td>
					<td></td>
				</tr>
				<tr>
					<td><asp:label id="lblStatusTitle" EnableViewState="False" CssClass="boldtext" Runat="server"></asp:label>:</td>
					<td><asp:dropdownlist id="ddlStatus" runat="server" Width="200px"></asp:dropdownlist></td>
				</tr>
				<tr>
					<td><asp:label id="lblInitialPhase" EnableViewState="False" CssClass="boldtext" Runat="server"></asp:label>:</td>
					<td class="ibn-value">
						<asp:dropdownlist id="ddInitialPhase" runat="server" Width="174px"></asp:dropdownlist>&nbsp;
						<button id="btnAddPhase" runat="server" style="border:0px;padding:0px;position:relative;top:0px;left:0px;height:20px;width:22px;background-color:transparent" type="button"><IMG 
							height="20" src="../layouts/images/icons/dictionary_edit.gif" width="22" border="0"></button>
					</td>
				</tr>
				<tr>
					<td><asp:label id="lblPrjPhaseTitle" EnableViewState="False" CssClass="boldtext" Runat="server"></asp:label>:</td>
					<td class="ibn-value">
						<asp:dropdownlist id="ddPrjPhases" runat="server" Width="174px"></asp:dropdownlist>&nbsp;
						<button id="btnAddPhase2" runat="server" style="border:0px;padding:0px;position:relative;top:0px;left:0px;height:20px;width:22px;background-color:transparent" type="button"><IMG 
							height="20" src="../layouts/images/icons/dictionary_edit.gif" width="22" border="0"></button>
					</td>
				</tr>
				<tr>
					<td><asp:label id="lblRiskLevel" Runat="server" CssClass="boldtext"></asp:label>:</td>
					<td><asp:dropdownlist id="ddlRiskLevel" runat="server" Width="200px"></asp:dropdownlist></td>
				</tr>
				<tr>
					<td><asp:label id="lblOverallStatus" EnableViewState=False CssClass="boldtext" Runat=server></asp:label>:</td>
					<td><asp:DropDownList ID="ddlOverallStatus" Runat=server Width="104px"></asp:DropDownList></td>
				</tr>
				<tr id="trPriority" runat="server">
					<td><asp:label id="lblPriority" Runat="server" CssClass="boldtext"></asp:label>:</td>
					<td><asp:dropdownlist id="ddlPriority" runat="server" Width="200px"></asp:dropdownlist></td>
				</tr>
			</table>
		</td>
	</tr>
	<tr>
		<td style="padding:0px 5px 5px 5px;" width="100%" valign="top" colspan="2">
			<ibn:BlockHeaderLight id="hdrCategoryInfo" runat="server" CollapsibleControlId="CategoryTable" />
			<table class="ibn-stylebox-light text" cellspacing="0" cellpadding="5" width="100%" border="0" runat="server" id="CategoryTable">
				<tr>
					<td width="50%" valign=top>
						<table class="text" cellspacing="0" cellpadding="7" width="100%" border="0">
							<tr>
								<td style="width:220px"></td>
								<td></td>
							</tr>
							<tr id="trClient" runat="server">
								<td><asp:label id="lblClientCustomer" EnableViewState="False" CssClass="boldtext" Runat="server"></asp:label>:</td>
								<td><ibn:EntityDD id="ClientControl" ObjectTypes="Contact,Organization" runat="server" Width="220px"/></td>
							</tr>
							<tr>
								<td vAlign="top" style="padding-top:7px">
									<asp:label id="lblPortfolios" EnableViewState="False" CssClass="boldtext" Runat="server"></asp:label>:
								</td>
								<td style="padding-top:7px">
									<asp:listbox id="lbPortfolios" runat="server" CssClass="text" Width="220px" Rows="5" SelectionMode="Multiple"></asp:listbox>
								</td>
							</tr>
						</table>
					</td>
					<td width="50%" valign=top>
						<table class="text" cellspacing="0" cellpadding="7" width="100%" border="0">
							<tr>
								<td style="width:220px"></td>
								<td></td>
							</tr>
							<tr id="trCategories" runat="server">
								<td vAlign="top"><asp:label id="lblCategoryTitle" EnableViewState="False" CssClass="boldtext" Runat="server"></asp:label>:</td>
								<td><asp:listbox id="lbCategory" runat="server" CssClass="text" Width="200px" Rows="5" SelectionMode="Multiple"></asp:listbox>
									<div align="right" style="width:200px">
									<button id="btnAddGeneralCategory" runat="server" style="border:0px;padding:0px;position:relative;top:0px;left:0px;height:20px;width:22px;background-color:transparent" type="button"><IMG height="20" src="../layouts/images/icons/dictionary_edit.gif" width="22" border="0"></button></div>
								</td>
							</tr>
							<tr id="trProjectCategories" runat="server">
								<td vAlign="top"><asp:label id="lblProjectCategory" EnableViewState="False" CssClass="boldtext" Runat="server"></asp:label>:</td>
								<td><asp:listbox id="lbProjectCategory" runat="server" CssClass="text" Width="200px" Rows="5" SelectionMode="Multiple"></asp:listbox>
									<div align="right" style="width:200px">
									<button id="btnAddProjectCategory" runat="server" style="border:0px;padding:0px;position:relative;top:0px;left:0px;height:20px;width:22px;background-color:transparent" type="button">
										<IMG height="20" src="../layouts/images/icons/dictionary_edit.gif" width="22" border="0"></button></td>
							</tr>
						</table>
					</td>
				</tr>
			</table>
		</td>
	</tr>
	<tr>
		<td style="padding:5px;" width="100%" valign="top" colspan="2">
			<ibn:BlockHeaderLight id="hdrAdditionalInfo" runat="server" CollapsibleControlId="AdditionalInfoTable" />
				<table class="ibn-stylebox-light text" cellspacing="0" cellpadding="5" width="100%" border="0" runat="server" id="AdditionalInfoTable">
					<tr>
						<td width="50%" valign="top" runat="server" id="LeftTextCell">
							<table class="text" cellspacing="3" cellpadding="3" width="100%" border="0">
								<tr runat="server" id="GoalsRow">
									<td>
										<asp:label id="lblGoalsTitle" EnableViewState="False" CssClass="ibn-label" Runat="server"></asp:label>:<br />
										<asp:textbox id="txtGoals" runat="server" CssClass="text" Width="370px" TextMode="MultiLine" Height="150px"></asp:textbox>
									</td>
								</tr>
								<tr runat="server" id="DeleverablesRow">
									<td>
										<asp:label id="lblDeliverablesTitle" EnableViewState="False" CssClass="ibn-label" Runat="server"></asp:label>:<br />
										<asp:textbox id="txtDeliverables" runat="server" CssClass="text" Width="370px" TextMode="MultiLine" Height="150px"></asp:textbox>
									</td>
								</tr>
							</table>
						</td>
						<td width="50%" valign=top>
							<table class="text" cellspacing="3" cellpadding="3" width="100%" border="0">
								<tr>
									<td>
										<asp:label id="lblDescriptionTitle" EnableViewState="False" CssClass="ibn-label" Runat="server"></asp:label>:<br />
										<asp:textbox id="txtDescription" runat="server" CssClass="text" Width="350px" TextMode="MultiLine" Height="150"></asp:textbox>
									</td>
								</tr>
								<tr runat="server" id="ScopeRow">
									<td>
										<asp:label id="lblScopeTitle" EnableViewState="False" CssClass="ibn-label" Runat="server"></asp:label>:<br />
										<asp:textbox id="txtScope" runat="server" CssClass="text" Width="350px" TextMode="MultiLine" Height="150px"></asp:textbox>
									</td>
								</tr>
							</table>
						</td>
					</tr>
				</table>
		</td>
	</tr>
	<tr>
		<td colspan="2">
			<ibn:MetaDataInternalEditControl runat="server" id="EditControl"></ibn:MetaDataInternalEditControl>
		</td>
	</tr>
	<tr>
		<td colspan="2" align=right vAlign="top">
			<table>
				<tr>
					<td>
						<btn:imbutton class="text" id="btnSave" Runat="server" style="width:110px;" onserverclick="btnSave_ServerClick"></btn:imbutton>&nbsp;&nbsp;
						<btn:imbutton class="text" id="btnCancel" Runat="server" style="width:110px;" IsDecline="true" CausesValidation="false" onserverclick="btnCancel_ServerClick"></btn:imbutton>
					</td>
				</tr>
				<tr>
					<td>
						<asp:CheckBox Checked=False CssClass="text" Runat=server ID="cbOneMore"></asp:CheckBox>
					</td>
				</tr>
			</table>
		</td>
	</tr>
</table>
<div align="center">
<input id="iGroups" style="VISIBILITY: hidden" name="iGroups" runat="server">
<input id="iManagerId" style="VISIBILITY: hidden" name="iManagerId" runat="server">
</div>
<asp:Button id="btnRefreshGen" CausesValidation="False" runat="server" Text="Button" style="visibility:hidden;"></asp:Button>
<asp:Button id="btnRefreshProj" CausesValidation="False" runat="server" Text="Button" style="visibility:hidden;"></asp:Button>
<asp:Button id="btnRefreshClient" CausesValidation="False" runat="server" Text="Button" style="visibility:hidden;"></asp:Button>
<asp:Button id="btnRefreshPhase" CausesValidation="False" runat="server" Text="Button" style="visibility:hidden;"></asp:Button>
