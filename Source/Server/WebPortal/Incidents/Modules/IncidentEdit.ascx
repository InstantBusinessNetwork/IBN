<%@ Reference Control="~/Modules/ObjectDropDownNew.ascx" %>
<%@ Reference Control="~/Apps/MetaUIEntity/Modules/EntityDropDown.ascx" %>
<%@ Reference Control="~/Modules/TimeControl.ascx" %>
<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Reference Control="~/Modules/BlockHeaderLightWithMenu.ascx" %>
<%@ Reference Control="~/Modules/MetaDataInternalEditControl.ascx" %>
<%@ Register TagPrefix="btn" namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Incidents.Modules.IncidentEdit"
	CodeBehind="IncidentEdit.ascx.cs" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" Src="~/Modules/BlockHeader.ascx" %>
<%@ Register TagPrefix="cc1" Namespace="Mediachase.FileUploader.Web.UI" Assembly="Mediachase.FileUploader" %>
<%@ Register TagPrefix="FTB" Namespace="FreeTextBoxControls" Assembly="FreeTextBox" %>
<%@ Register TagPrefix="ibn" TagName="Time" Src="~/Modules/TimeControl.ascx" %>
<%@ Register TagPrefix="ibn" TagName="ObjectDD" Src="~/Modules/ObjectDropDownNew.ascx" %>
<%@ Register TagPrefix="ibn" TagName="EntityDD" Src="~/Apps/MetaUIEntity/Modules/EntityDropDown.ascx" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeaderLight" Src="~/Modules/BlockHeaderLightWithMenu.ascx" %>
<%@ Register TagPrefix="ibn" TagName="MetaDataInternalEditControl" src="~/Modules/MetaDataInternalEditControl.ascx" %>
<style type="text/css">
	p
	{
		padding: 0;
		margin: 0;
	}
</style>

<script type="text/javascript">
<!--
	var objtrFile, objtrHTML1, objtrHTML2, objspHTML, objtrLink;

	function GetObjects() {
		objtrFile = document.getElementById('<%=trFile.ClientID %>');
		objtrHTML1 = document.getElementById('<%=trHTML1.ClientID %>');
		objtrHTML2 = document.getElementById("<%=trHTML2.ClientID %>");
		objtrLink1 = document.getElementById("<%=trLink1.ClientID %>");
		objtrLink2 = document.getElementById("<%=trLink2.ClientID %>");
		objspHTML = document.getElementById("spHTML");
	}

	function ShowFile() {
		if (typeof(FTB_API) != "undefined" && FTB_API['<%=fckEditor.ClientID %>']) {
			GetObjects();
			if (objtrFile) objtrFile.style.display = "";
			if (objtrHTML1) objtrHTML1.style.display = "none";
			if (objspHTML) objspHTML.style.display = "none";
			if (objtrHTML2) objtrHTML2.style.display = "none";
			if (objtrLink1) objtrLink1.style.display = "none";
			if (objtrLink2) objtrLink2.style.display = "none";
		}
		else
			window.setTimeout('ShowFile()', 200);
	}

	function ShowHTML() {
		if (typeof(FTB_API) != "undefined" && FTB_API['<%=fckEditor.ClientID %>']) {
			GetObjects();
			if (objtrFile) objtrFile.style.display = "none";
			if (objtrHTML1) objtrHTML1.style.display = "";
			if (objtrHTML2) objtrHTML2.style.display = "";
			if (objspHTML) objspHTML.style.display = "";
			else {
				GetObjects();
				if (objspHTML) objspHTML.style.display = "";
			}
			if (objtrLink1) objtrLink1.style.display = "none";
			if (objtrLink2) objtrLink2.style.display = "none";
		}
		else
			window.setTimeout('ShowHTML()', 200);
	}
	function ShowLink() {
		if (typeof(FTB_API) != "undefined" && FTB_API['<%=fckEditor.ClientID %>']) {
			GetObjects();
			if (objtrFile) objtrFile.style.display = "none";
			if (objtrHTML1) objtrHTML1.style.display = "none";
			if (objspHTML) objspHTML.style.display = "none";
			if (objtrHTML2) objtrHTML2.style.display = "none";
			if (objtrLink1) objtrLink1.style.display = "";
			if (objtrLink2) objtrLink2.style.display = "";
		}
		else
			window.setTimeout('ShowLink()', 200);
	}

	function AddCategory(CategoryType, ButtonId) {
		var w = 640;
		var h = 350;
		var l = (screen.width - w) / 2;
		var t = (screen.height - h) / 2;
		winprops = 'resizable=0, height=' + h + ',width=' + w + ',top=' + t + ',left=' + l;
		var f = window.open('../Common/AddCategory.aspx?BtnID=' + ButtonId + '&DictType=' + CategoryType, "AddCategory", winprops);
	}		
		
//-->
</script>

<table class="ibn-stylebox" cellspacing="0" cellpadding="0" width="100%" border="0"
	style="margin-top: 1px">
	<tr>
		<td>
			<ibn:BlockHeader ID="tbSave" runat="server"></ibn:BlockHeader>
		</td>
	</tr>
	<tr>
		<td>
			<asp:UpdatePanel runat="server" ID="MainPanel" ChildrenAsTriggers="true">
				<ContentTemplate>
					<table cellpadding="0" cellspacing="0" width="100%" border="0">
						<tr>
							<td style="padding: 5px" width="50%" valign="top">
								<ibn:BlockHeaderLight ID="hdrBasicInfo" runat="server" CollapsibleControlId="BasicInfoTable" />
								<table class="ibn-stylebox-light text" cellspacing="0" cellpadding="7" width="100%" border="0" runat="server" id="BasicInfoTable">
									<colgroup>
										<col width="150px" />
										<col />
									</colgroup>
									<tr>
										<td class="ibn-label" colspan="2">
											<%=LocRM.GetString("Title")%>:
											<asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtTitle"
												ErrorMessage="*"></asp:RequiredFieldValidator>
											<br />
											<asp:TextBox ID="txtTitle" runat="server" CssClass="text" Width="100%"></asp:TextBox>
										</td>
									</tr>
									<tr>
										<td class="ibn-label" colspan="2">
											<%=LocRM.GetString("Description")%>:<br />
											<asp:TextBox runat="server" ID="ftbDescription" Width="100%" Height="180" CssClass="text"
												TextMode="MultiLine"></asp:TextBox>
										</td>
									</tr>
									<tr id="trPriority" runat="server">
										<td class="ibn-label">
											<%=LocRM.GetString("Priority")%>:
										</td>
										<td class="ibn-value">
											<asp:DropDownList ID="ddlPriority" runat="server" CssClass="text" Width="250px">
											</asp:DropDownList>
										</td>
									</tr>
									<tr id="trTaskTime" runat="server">
										<td class="ibn-label">
											<%=LocRM2.GetString("taskTime")%>:
										</td>
										<td class="ibn-value">
											<table class="text">
												<tr>
													<td>
														<ibn:Time ID="ucTaskTime" ShowTime="HM" ViewStartDate="True" runat="server" HourSpinMaxValue="999" />
													</td>
													<td class="ibn-description">
														<%=LocRM4.GetString("WorkHours")%>
													</td>
												</tr>
											</table>
										</td>
									</tr>
									<tr id="trExpAssTime" runat="server">
										<td class="ibn-label">
											<%=LocRM3.GetString("tExpAssignTime")%>:
										</td>
										<td class="ibn-value">
											<table class="text">
												<tr>
													<td>
														<ibn:Time ID="ucExpectedAssignTime" ShowTime="HM" ViewStartDate="True" runat="server"
															HourSpinMaxValue="999" />
													</td>
													<td class="ibn-description">
														<%=LocRM4.GetString("WorkHours")%>
													</td>
												</tr>
											</table>
										</td>
									</tr>
									<tr id="trExpReplyTime" runat="server">
										<td class="ibn-label">
											<%=LocRM3.GetString("tExpRespTime")%>:
										</td>
										<td class="ibn-value">
											<table class="text">
												<tr>
													<td>
														<ibn:Time ID="ucExpectedResponseTime" ShowTime="HM" ViewStartDate="True" runat="server"
															HourSpinMaxValue="999" />
													</td>
													<td class="ibn-description">
														<%=LocRM4.GetString("WorkHours")%>
													</td>
												</tr>
											</table>
										</td>
									</tr>
									<tr id="trExpDurationTime" runat="server">
										<td class="ibn-label">
											<%=LocRM3.GetString("tExpDuration")%>:
										</td>
										<td class="ibn-value">
											<table class="text">
												<tr>
													<td>
														<ibn:Time ID="ucExpectedDuration" ShowTime="HM" ViewStartDate="True" runat="server"
															HourSpinMaxValue="999" />
													</td>
													<td class="ibn-description">
														<%=LocRM4.GetString("WorkHours")%>
													</td>
												</tr>
											</table>
										</td>
									</tr>
								</table>
							</td>
							<td style="padding: 5px" width="50%" valign="top">
								<ibn:BlockHeaderLight ID="hdrCategoryInfo" runat="server" CollapsibleControlId="CategoryInfoTable" />
								<table class="ibn-stylebox-light text" cellspacing="0" cellpadding="7" width="100%" border="0" runat="server" id="CategoryInfoTable">
									<tr id="trProject" runat="server">
										<td class="ibn-label">
											<%=LocRM.GetString("Project")%>:
										</td>
										<td class="ibn-value">
											<ibn:ObjectDD ID="ddProject" ObjectTypes="3" runat="server" Width="260px" />
											<asp:Label ID="lblProject" runat="server" Font-Bold="True"></asp:Label>
										</td>
									</tr>
									<tr>
										<td class="ibn-label">
											<%=LocRM.GetString("tIssBox")%>:
										</td>
										<td class="ibn-value">
											<asp:DropDownList ID="ddlFolder" runat="server" Width="260px" CssClass="text" AutoPostBack="true"
												OnSelectedIndexChanged="ddlFolder_SelectedIndexChanged">
											</asp:DropDownList>
										</td>
									</tr>
									<tr id="trClient" runat="server">
										<td class="ibn-label">
											<%=LocRM4.GetString("Client")%>:
										</td>
										<td class="ibn-value">
											<ibn:EntityDD ID="ClientControl" ObjectTypes="Contact,Organization" runat="server"
												runat="server" Width="260px" />
										</td>
									</tr>
									<tr id="trType" runat="server">
										<td class="ibn-label">
											<%=LocRM.GetString("Type")%>:
										</td>
										<td class="ibn-value">
											<asp:DropDownList ID="ddlType" runat="server" CssClass="text" Width="260px">
											</asp:DropDownList>
										</td>
									</tr>
									<tr id="trSeverity" runat="server">
										<td valign="top" class="ibn-label">
											<%=LocRM.GetString("Severity")%>:
										</td>
										<td class="ibn-value">
											<asp:DropDownList ID="ddlSeverity" runat="server" CssClass="text" Width="260px">
											</asp:DropDownList>
										</td>
									</tr>
									<tr id="trCategories" runat="server">
										<td valign="top" class="ibn-label">
											<%=LocRM.GetString("Category")%>:
										</td>
										<td class="ibn-value">
											<asp:ListBox ID="lbCategory" runat="server" Width="260px" Rows="6" SelectionMode="Multiple">
											</asp:ListBox>
											<div align="right" style="width: 260px">
												<button id="btnAddGeneralCategory" runat="server" style="border: 0px; padding: 0px;
													position: relative; top: 0px; left: 0px; height: 20px; width: 22px; background-color: transparent"
													type="button">
													<img height="20" src="../layouts/images/icons/dictionary_edit.gif" width="22" border="0"></button>
											</div>
										</td>
									</tr>
									<tr id="trIssCategories" runat="server">
										<td valign="top" class="ibn-label">
											<%=LocRM.GetString("IncidentCategory")%>:
										</td>
										<td class="ibn-value">
											<asp:ListBox ID="lbIncidentCategory" runat="server" Width="260px" SelectionMode="Multiple" Rows="6"></asp:ListBox>
											<div align="right" style="width: 260px">
												<button id="btnAddIncidentCategory" runat="server" style="border: 0px; padding: 0px;
													position: relative; top: 0px; left: 0px; height: 20px; width: 22px; background-color: transparent"
													type="button">
													<img height="20" src="../layouts/images/icons/dictionary_edit.gif" width="22" border="0"></button>
											</div>
										</td>
									</tr>
								</table>
							</td>
						</tr>
					</table>
				</ContentTemplate>
			</asp:UpdatePanel>
		</td>
	</tr>
	<tr id="trHtmlAttach" runat="server">
		<td style="padding: 0px 5px 5px 5px;" width="100%" valign="top">
			<ibn:BlockHeaderLight ID="hdrAttach" runat="server" CollapsibleControlId="AttachTable" />
			<table class="ibn-stylebox-light text" cellspacing="0" cellpadding="5" width="100%" border="0" style="table-layout: fixed;" runat="server" id="AttachTable">
				<tr id="trLoad" runat="server">
					<td width="160px" class="ibn-label">
						<nobr><%=LocRM.GetString("Load")%>:</nobr>
					</td>
					<td class="ibn-value">
						<asp:RadioButton ID="rbFile" runat="server" GroupName="f"></asp:RadioButton>
						<asp:RadioButton ID="rbLink" runat="server" GroupName="f"></asp:RadioButton>
						<asp:RadioButton ID="rbHTML" runat="server" GroupName="f"></asp:RadioButton>
					</td>
				</tr>
				<tr id="trFile" runat="server">
					<td class="ibn-label">
						<%=LocRM.GetString("File")%>:
					</td>
					<td class="ibn-value">
						<cc1:McHtmlInputFile ID="fAssetFile" runat="server" Width="260px" CssClass="text">
						</cc1:McHtmlInputFile>
						<span class="text" id="vFile" style="color: red" runat="server">*</span>
					</td>
				</tr>
				<tr id="trLink1" runat="server">
					<td class="text ibn-label" valign="center">
						<%=LocRM.GetString("LinkTitle")%>:
					</td>
					<td align="left" class="ibn-value">
						<asp:TextBox ID="tbLinkTitle" runat="server" Width="330px" CssClass="text" TextMode="SingleLine"
							AutoPostBack="false" Style="margin: 0px"></asp:TextBox>
					</td>
				</tr>
				<tr id="trLink2" runat="server">
					<td class="text ibn-label" valign="center">
						<%=LocRM.GetString("Link")%>:
					</td>
					<td align="left" class="ibn-value">
						<asp:TextBox ID="tbLink" runat="server" Width="330px" CssClass="text" TextMode="SingleLine"
							AutoPostBack="false" Style="margin: 0px"></asp:TextBox>
					</td>
				</tr>
				<tr id="trHTML1" runat="server">
					<td class="ibn-label">
						<%=LocRM.GetString("HTMLTitle")%>:
					</td>
					<td class="ibn-value">
						<asp:TextBox ID="tbHtmlFileTitle" runat="server" Width="330" CssClass="text"></asp:TextBox>
					</td>
				</tr>
				<tr id="trHTML2" runat="server">
					<td valign="top" class="ibn-label">
						<%=LocRM.GetString("HTMLText")%>:
					</td>
					<td align="left" class="ibn-value">
						<span id="spHtml">
							<FTB:FreeTextBox ID="fckEditor" ToolbarLayout="fontsizesmenu,undo,redo,bold,italic,underline, createlink,fontforecolorsmenu,fontbackcolorsmenu"
								runat="Server" Width="98%" Height="300px" EnableHtmlMode="true" DropDownListCssClass="text"
								GutterBackColor="#F5F5F5" BreakMode="LineBreak" BackColor="#F5F5F5" StartMode="DesignMode"
								SupportFolder="~/Scripts/FreeTextBox/" JavaScriptLocation="ExternalFile" ButtonImagesLocation="ExternalFile"
								ToolbarImagesLocation="ExternalFile" />
						</span>
					</td>
				</tr>
			</table>
		</td>
	</tr>
	<tr id="trEmail" runat="server" visible="false">
		<td style="padding: 0 5 5 5" width="100%" valign="top">
			<fieldset style="width: 100%" align="top">
				<legend class="text ibn-legend-default" id="lgdEmail" runat="server"></legend>
				<div style="padding: 10;" class="text">
					<asp:Label runat="server" ID="lblEmail"></asp:Label>
				</div>
			</fieldset>
		</td>
	</tr>
	<tr>
		<td>
			<ibn:MetaDataInternalEditControl runat="server" id="EditControl" MetaClassName="IncidentsEx"></ibn:MetaDataInternalEditControl>
		</td>
	</tr>
	<tr>
		<td align="right" style="padding-right: 10px">
			<table>
				<tr>
					<td>
						<btn:IMButton class="text" ID="btnSave" style="width: 110px;" runat="server" OnServerClick="btnSave_ServerClick" />
						&nbsp;&nbsp;
						<btn:IMButton class="text" ID="btnCancel" style="width: 110px;" runat="server" IsDecline="true"
							CausesValidation="false" OnServerClick="btnCancel_ServerClick" />
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

<script language="javascript" type="text/javascript">
	
	function ShowProgress()
	{
	 	if (document.forms[0].<%=fAssetFile.ClientID %> && document.forms[0].<%=fAssetFile.ClientID %>.value!="" && document.forms[0].<%=txtTitle.ClientID %>.value!="")
	 	{
	 		if(!browseris.nav)
	 		{
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

<asp:Button ID="btnRefreshGen" CausesValidation="False" runat="server" Text="Button"
	Style="visibility: hidden;"></asp:Button>
<asp:Button ID="btnRefreshInc" CausesValidation="False" runat="server" Text="Button"
	Style="visibility: hidden;"></asp:Button>
