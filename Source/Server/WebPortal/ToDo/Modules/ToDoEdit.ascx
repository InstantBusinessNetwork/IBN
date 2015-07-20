<%@ Control Language="c#" Inherits="Mediachase.UI.Web.ToDo.Modules.ToDoEdit" CodeBehind="ToDoEdit.ascx.cs" %>
<%@ Reference Control="~/Modules/ObjectDropDownNew.ascx" %>
<%@ Reference Control="~/Apps/MetaUIEntity/Modules/EntityDropDown.ascx" %>
<%@ Reference Control="~/Modules/TimeControl.ascx" %>
<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Reference Control="~/Modules/BlockHeaderLightWithMenu.ascx" %>
<%@ Reference Control="~/Apps/Common/DateTimePickerAjax/PickerControl.ascx" %>
<%@ Reference Control="~/Modules/MetaDataInternalEditControl.ascx" %>
<%@ Register TagPrefix="mc" TagName="Picker" Src="~/Apps/Common/DateTimePickerAjax/PickerControl.ascx" %>
<%@ Register TagPrefix="btn" namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" Src="~/Modules/BlockHeader.ascx" %>
<%@ Register TagPrefix="cc1" Namespace="Mediachase.FileUploader.Web.UI" Assembly="Mediachase.FileUploader" %>
<%@ Register TagPrefix="FTB" Namespace="FreeTextBoxControls" Assembly="FreeTextBox" %>
<%@ Register TagPrefix="ibn" TagName="Time" Src="~/Modules/TimeControl.ascx" %>
<%@ Register TagPrefix="ibn" TagName="ObjectDD" Src="~/Modules/ObjectDropDownNew.ascx" %>
<%@ Register TagPrefix="ibn" TagName="EntityDD" Src="~/Apps/MetaUIEntity/Modules/EntityDropDown.ascx" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeaderLight" Src="~/Modules/BlockHeaderLightWithMenu.ascx" %>
<%@ Register TagPrefix="ibn" TagName="MetaDataInternalEditControl" src="~/Modules/MetaDataInternalEditControl.ascx" %>

<script type="text/javascript">
<!--
		var objtrFile,objtrHTML1,objtrHTML2,objspHTML,objtrLink;
		
		function GetObjects()
		{
			objtrFile = document.getElementById('<%=trFile.ClientID %>');
			objtrHTML1 = document.getElementById('<%=trHTML1.ClientID %>');
			objtrHTML2 = document.getElementById("<%=trHTML2.ClientID %>");
			objspHTML = document.getElementById("spHtml");
			objtrLink1 = document.getElementById("<%=trLink1.ClientID %>");
			objtrLink2 = document.getElementById("<%=trLink2.ClientID %>");	
		}
		
		function ShowFile() 
		{
			if(typeof(FTB_API) != "undefined" && FTB_API['<%=fckEditor.ClientID %>'])
			{
				GetObjects();
				if(objtrFile) objtrFile.style.display = "";
				if(objtrHTML1) objtrHTML1.style.display = "none";
				if(objtrHTML2) objtrHTML2.style.display = "none";
				if(objspHTML) objspHTML.style.display = "none";
				if(objtrLink1) objtrLink1.style.display = "none";			
				if(objtrLink2) objtrLink2.style.display = "none";
			}
			else
				window.setTimeout('ShowFile()', 200);
		}
		
		function ShowHTML()
		{
			if(typeof(FTB_API) != "undefined" && FTB_API['<%=fckEditor.ClientID %>'])
			{
				GetObjects();
				if(objtrFile) objtrFile.style.display = "none";
				if(objtrHTML1) objtrHTML1.style.display = "";
				if(objtrHTML2) objtrHTML2.style.display = "";
				if(objspHTML) objspHTML.style.display = "";			
				if(objtrLink1) objtrLink1.style.display = "none";			
				if(objtrLink2) objtrLink2.style.display = "none";
			}
			else
				window.setTimeout('ShowHTML()', 200);
		}
		function ShowLink()
		{
			if(typeof(FTB_API) != "undefined" && FTB_API['<%=fckEditor.ClientID %>'])
			{
				GetObjects();
				if(objtrFile) objtrFile.style.display = "none";
				if(objtrHTML1) objtrHTML1.style.display = "none";
				if(objtrHTML2) objtrHTML2.style.display = "none";
				if(objspHTML) objspHTML.style.display = "none";			
				if(objtrLink1) objtrLink1.style.display = "";						
				if(objtrLink2) objtrLink2.style.display = "";
			}
			else
				window.setTimeout('ShowHTML()', 200);
		}		
		function AddCategory(CategoryType)
		{
			var w = 640;
			var h = 350;
			var l = (screen.width - w) / 2;
			var t = (screen.height - h) / 2;
			winprops = 'resizable=0, height='+h+',width='+w+',top='+t+',left='+l;
			var f = window.open('../Common/AddCategory.aspx?BtnID=<%=btnRefresh.ClientID %>&DictType=' + CategoryType, "AddCategory", winprops);
		}	
		
		function ChangeProject()
		{
			<%= Page.ClientScript.GetPostBackEventReference(lbChangeProject, "")%>
		}			
//-->
</script>

<table align="center" width="700" cellpadding="3" cellspacing="0" class="ibn-alerttext"
	id="tblWarning" style="display: none">
	<tr>
		<td width="20" align="center" valign="middle">
			<asp:Image ID="imgCaution" runat="server" ImageUrl="~/Layouts/Images/warning.gif"
				Width="16" Height="16" ImageAlign="AbsMiddle"></asp:Image>
		</td>
		<td>
			<%=LocRM.GetString("AssignWarning") %>&nbsp;<a href="#" id="aAssignLink"></a>
		</td>
	</tr>
</table>
<table class="ibn-stylebox" cellspacing="0" cellpadding="0" width="100%" border="0"
	style="margin-top: 1px">
	<tr>
		<td colspan="2">
			<ibn:BlockHeader ID="tbSave" runat="server"></ibn:BlockHeader>
		</td>
	</tr>
	<tr>
		<td style="padding: 5px" width="50%" valign="top">
			<ibn:BlockHeaderLight ID="hdrBasicInfo" runat="server" CollapsibleControlId="BasicInfoDiv" />
			<div runat="server" id="BasicInfoDiv">
				<table class="ibn-stylebox-notopbottom text" cellspacing="0" cellpadding="7" width="100%" border="0">
					<colgroup>
						<col width="80" />
						<col />
					</colgroup>
					<tr>
						<td class="ibn-value" colspan="2">
							<asp:Label ID="lblTitleTitle" CssClass="boldtext" runat="server"></asp:Label>:
							<asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="*"
								ControlToValidate="txtTitle" Display="Dynamic"></asp:RequiredFieldValidator>
							<br />
							<asp:TextBox ID="txtTitle" runat="server" CssClass="text" Width="99%"></asp:TextBox>
						</td>
					</tr>
					<tr>
						<td class="ibn-value" colspan="2">
							<asp:Label ID="lblDescriptionTitle" CssClass="boldtext" runat="server"></asp:Label>:<br />
							<asp:TextBox ID="txtDescription" runat="server" CssClass="text" Width="99%" TextMode="MultiLine"
								Height="100px" Rows="5"></asp:TextBox>
						</td>
					</tr>
				</table>
				<asp:UpdatePanel runat="server" ID="MainPanel" ChildrenAsTriggers="true">
					<ContentTemplate>
						<table class="ibn-stylebox-light text" cellspacing="0" cellpadding="7" width="100%"
							border="0">
							<colgroup>
								<col width="80" />
								<col />
							</colgroup>
							<tr id="trProject" runat="server">
								<td>
									<asp:Label ID="lblProjectTitle" CssClass="boldtext" runat="server"></asp:Label>:
								</td>
								<td>
									<ibn:ObjectDD ID="ucProject" OnChange="ChangeProject()" ObjectTypes="3" runat="server"
										Width="99%" />
									<asp:HyperLink runat="server" ID="ProjectLink"></asp:HyperLink>
								</td>
							</tr>
							<tr id="trIncident" runat="server">
								<td>
									<asp:Label ID="lblIncidentTitle" CssClass="boldtext" runat="server"></asp:Label>:
								</td>
								<td>
									<asp:Label ID="lblIncident" runat="server" CssClass="text" Font-Bold="True"></asp:Label>
								</td>
							</tr>
							<tr id="trDocument" runat="server">
								<td>
									<asp:Label ID="lblDocumentTitle" CssClass="boldtext" runat="server"></asp:Label>:
								</td>
								<td>
									<asp:Label ID="lblDocument" runat="server" CssClass="text" Font-Bold="True"></asp:Label>
								</td>
							</tr>
							<tr id="trTask" runat="server">
								<td>
									<asp:Label ID="lblTaskTitle" CssClass="boldtext" runat="server"></asp:Label>:
								</td>
								<td>
									<asp:Label ID="lblTask" runat="server" CssClass="text" Font-Bold="True"></asp:Label>
								</td>
							</tr>
							<tr id="trStatus" runat="server">
								<td>
								</td>
								<td>
									<asp:CheckBox ID="cbCompleteAfterToDo" runat="server"></asp:CheckBox>
								</td>
							</tr>
							<tr>
								<td>
									<asp:Label ID="lblManagerTitle" runat="server" CssClass="boldtext"></asp:Label>:
								</td>
								<td>
									<asp:DropDownList ID="ddlManager" runat="server" CssClass="text" Width="99%">
									</asp:DropDownList>
								</td>
							</tr>
						</table>
						<asp:LinkButton ID="lbChangeProject" runat="server" OnClick="lbChangeProject_Click"></asp:LinkButton>
					</ContentTemplate>
				</asp:UpdatePanel>
			</div>
		</td>
		<td style="padding: 5px" width="50%" valign="top">
			<ibn:BlockHeaderLight ID="hdrCategoryInfo" runat="server" CollapsibleControlId="CategoryInfoTable" />
			<table class="ibn-stylebox-light text" cellspacing="0" cellpadding="7" width="100%" border="0" runat="server" id="CategoryInfoTable">
				<tr id="trPriority" runat="server">
					<td>
						<asp:Label ID="lblPriorityTitle" CssClass="boldtext" runat="server"></asp:Label>:
					</td>
					<td width="200">
						<asp:DropDownList ID="ddlPriority" runat="server" Width="260px">
						</asp:DropDownList>
					</td>
				</tr>
				<tr>
					<td>
						<asp:Label ID="lblStartDateTitle" CssClass="boldtext" runat="server"></asp:Label>:
					</td>
					<td>
						<mc:Picker ID="dtcStartDate" runat="server" DateCssClass="text" TimeCssClass="text"
							DateWidth="85px" TimeWidth="60px" ShowImageButton="false" ShowTime="true" />
						<asp:Button ID="Button1" runat="server" CssClass="text" Text="Reset" CausesValidation="False"
							Visible="False" OnClick="Button1_Click"></asp:Button>
					</td>
				</tr>
				<tr>
					<td>
						<asp:Label ID="lblEndDateTitle" runat="server" CssClass="boldtext"></asp:Label>:
					</td>
					<td>
						<mc:Picker ID="dtcEndDate" runat="server" DateCssClass="text" TimeCssClass="text"
							DateWidth="85px" TimeWidth="60px" ShowImageButton="false" ShowTime="true" />
						<asp:CustomValidator ID="CustomValidator1" runat="server" ErrorMessage="Wrong Date"
							Display="Dynamic"></asp:CustomValidator>
					</td>
				</tr>
				<tr id="trTaskTime" runat="server">
					<td>
						<asp:Label ID="lblTaskTimeTitle" runat="server" CssClass="boldtext"></asp:Label>:
					</td>
					<td>
						<ibn:Time ID="ucTaskTime" ShowTime="HM" HourSpinMaxValue="999" ViewStartDate="True"
							runat="server" />
					</td>
				</tr>
				<tr id="trActivation" runat="server">
					<td>
						<asp:Label ID="lblActivationTitle" runat="server" CssClass="boldtext"></asp:Label>:
					</td>
					<td>
						<asp:DropDownList ID="ddlActivationType" runat="server" Width="260px">
						</asp:DropDownList>
					</td>
				</tr>
				<tr id="trCompletion" runat="server">
					<td>
						<asp:Label ID="lblCompletionTitle" CssClass="boldtext" runat="server"></asp:Label>:
					</td>
					<td>
						<asp:DropDownList ID="ddlCompletionType" runat="server" Width="260px">
						</asp:DropDownList>
					</td>
				</tr>
				<tr id="trMustConfirm" runat="server">
					<td>
						&nbsp;
					</td>
					<td>
						<asp:CheckBox ID="chbMustBeConfirmed" CssClass="text" runat="server"></asp:CheckBox>
					</td>
				</tr>
				<tr id="trClient" runat="server">
					<td>
						<asp:Label ID="lblClient" CssClass="boldtext" runat="server"></asp:Label>:
					</td>
					<td>
						<ibn:EntityDD ID="ClientControl" ObjectTypes="Contact,Organization" runat="server" Width="260px" />
					</td>
				</tr>
				<tr id="trCategory" runat="server">
					<td valign="top">
						<asp:Label ID="lblCategoryTitle" runat="server" CssClass="boldtext"></asp:Label>:
					</td>
					<td width="260" class="text">
						<asp:ListBox ID="lbCategory" runat="server" CssClass="text" Width="260px" Rows="5"
							SelectionMode="Multiple"></asp:ListBox>
						<div align="right" style="margin-right: 0px; width: 260px">
							<button id="btnAddGeneralCategory" runat="server" style="border: 0px; padding: 0;
								position: relative; top: 0px; left: 0px; height: 20px; width: 22px; background-color: transparent"
								type="button">
								<img height="20" src="../layouts/images/icons/dictionary_edit.gif" width="22" border="0" /></button>
						</div>
					</td>
				</tr>
			</table>
		</td>
	</tr>
	<tr id="trAttachFile" runat="server">
		<td style="padding:5px;" width="100%" valign="top" colspan="2">
			<ibn:BlockHeaderLight ID="hdrAttach" runat="server" CollapsibleControlId="AttachTable" />
			<table class="ibn-stylebox-light text" cellspacing="0" cellpadding="5" width="100%" border="0" runat="server" id="AttachTable">
				<tr id="trLoad" runat="server">
					<td style="width: 120px">
						<nobr><b><%=LocRM.GetString("Load")%>:</b></nobr>
					</td>
					<td>
						<asp:RadioButton CssClass="text" ID="rbFile" runat="server" GroupName="f"></asp:RadioButton>
						<asp:RadioButton CssClass="text" ID="rbLink" runat="server" GroupName="f"></asp:RadioButton>
						<asp:RadioButton CssClass="text" ID="rbHTML" runat="server" GroupName="f"></asp:RadioButton>
					</td>
				</tr>
				<tr id="trFile" runat="server">
					<td style="width: 120px">
						<b>
							<%=LocRM.GetString("File")%>:</b>
					</td>
					<td>
						<cc1:McHtmlInputFile ID="fAssetFile" runat="server" Width="260px" CssClass="text">
						</cc1:McHtmlInputFile>
						<span class="text" id="vFile" style="color: red" runat="server">*</span>
					</td>
				</tr>
				<tr id="trLink1" runat="server">
					<td class="text" valign="middle" style="width: 120px">
						<b>
							<%=LocRM.GetString("LinkTitle")%>:</b>
					</td>
					<td align="left">
						<asp:TextBox ID="tbLinkTitle" runat="server" Width="400px" CssClass="text" TextMode="SingleLine"
							AutoPostBack="false" Style="margin: 0px"></asp:TextBox>
					</td>
				</tr>
				<tr id="trLink2" runat="server">
					<td class="text" valign="middle" style="width: 120px">
						<b>
							<%=LocRM.GetString("Link")%>:</b>
					</td>
					<td align="left">
						<asp:TextBox ID="tbLink" runat="server" Width="400px" CssClass="text" TextMode="SingleLine"
							AutoPostBack="false" Style="margin: 0px"></asp:TextBox>
					</td>
				</tr>
				<tr id="trHTML1" runat="server">
					<td style="width: 120px">
						<b>
							<%=LocRM.GetString("HTMLTitle")%>:</b>
					</td>
					<td>
						<asp:TextBox ID="tbHtmlFileTitle" runat="server" Width="260px" CssClass="text"></asp:TextBox>
					</td>
				</tr>
				<tr id="trHTML2" runat="server">
					<td valign="top" style="width: 120px">
						<b>
							<%=LocRM.GetString("HTMLText")%>:</b>
					</td>
					<td align="left">
						<span id="spHtml">
							<FTB:FreeTextBox ID="fckEditor" ToolbarLayout="fontsizesmenu,undo,redo,bold,italic,underline, createlink"
								runat="Server" Width="98%" Height="300px" EnableHtmlMode="true" DropDownListCssClass="text"
								GutterBackColor="#F5F5F5" BreakMode="LineBreak" BackColor="#F5F5F5" ToolbarBackgroundImage="false"
								SupportFolder="~/Scripts/FreeTextBox/" JavaScriptLocation="ExternalFile" ButtonImagesLocation="ExternalFile"
								ToolbarImagesLocation="ExternalFile" />
						</span>
					</td>
				</tr>
			</table>
		</td>
	</tr>
	<tr>
		<td colspan="2">
			<ibn:MetaDataInternalEditControl runat="server" id="EditControl" MetaClassName="TodoEx"></ibn:MetaDataInternalEditControl>
		</td>
	</tr>
	<tr>
		<td colspan="2" valign="bottom" height="30" align="right" style="padding-right: 5px">
			<table>
				<tr>
					<td>
						<btn:IMButton ID="btnSaveAssign" runat="server" Class="text" OnServerClick="btnSave_ServerClick"></btn:IMButton>
						&nbsp;&nbsp;
						<btn:IMButton class="text" ID="btnSave" runat="server" style="width: 110px;" OnServerClick="btnSave_ServerClick"></btn:IMButton>
						&nbsp;&nbsp;
						<btn:IMButton class="text" ID="btnCancel" runat="server" style="width: 110px;" IsDecline="true" CausesValidation="false" OnServerClick="btnCancel_ServerClick">
						</btn:IMButton>
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
<asp:Button ID="btnRefresh" CausesValidation="False" runat="server" Text="Button" Style="visibility: hidden;"></asp:Button>
<div align="center">
	<asp:TextBox ID="txtIncidentId" runat="server" Visible="False"></asp:TextBox>
	<asp:TextBox ID="txtDocumentId" runat="server" Visible="False"></asp:TextBox>
	<asp:TextBox ID="txtTaskId" runat="server" Visible="False"></asp:TextBox>
</div>

<script language="javascript" type="text/javascript">
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

