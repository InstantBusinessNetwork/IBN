<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Documents.Modules.DocumentEdit" Codebehind="DocumentEdit.ascx.cs" %>
<%@ Reference Control="~/Modules/ObjectDropDownNew.ascx" %>
<%@ Reference Control="~/Apps/MetaUIEntity/Modules/EntityDropDown.ascx" %>
<%@ Reference Control="~/Modules/TimeControl.ascx" %>
<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Reference Control="~/Modules/BlockHeaderLightWithMenu.ascx" %>
<%@ Reference Control="~/Modules/MetaDataInternalEditControl.ascx" %>
<%@ Register TagPrefix="btn" namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="~/Modules/BlockHeader.ascx" %>
<%@ Register TagPrefix="cc1" Namespace="Mediachase.FileUploader.Web.UI" Assembly="Mediachase.FileUploader" %>
<%@ Register TagPrefix="FTB" Namespace="FreeTextBoxControls" Assembly="FreeTextBox" %>
<%@ Register TagPrefix="ibn" TagName="Time" src="~/Modules/TimeControl.ascx" %>
<%@ Register TagPrefix="ibn" TagName="ObjectDD" src="~/Modules/ObjectDropDownNew.ascx" %>
<%@ Register TagPrefix="ibn" TagName="EntityDD" src="~/Apps/MetaUIEntity/Modules/EntityDropDown.ascx" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeaderLight" src="~/Modules/BlockHeaderLightWithMenu.ascx" %>
<%@ Register TagPrefix="ibn" TagName="MetaDataInternalEditControl" src="~/Modules/MetaDataInternalEditControl.ascx" %>

<script type="text/javascript">
<!--
		var objtrFile,objtrHTML1,objtrHTML2,objspHTML,objtrLink;
		
		function GetObjects()
		{
			objtrFile = document.getElementById('<%=trFile.ClientID %>');
			objtrHTML1 = document.getElementById('<%=trHTML1.ClientID %>');
			objtrHTML2 = document.getElementById("<%=trHTML2.ClientID %>");
			objtrLink1 = document.getElementById("<%=trLink1.ClientID %>");
			objtrLink2 = document.getElementById("<%=trLink2.ClientID %>");			
			objspHTML = document.getElementById("spHTML");			
		}
			
		function ShowFile()
		{
			GetObjects();
			if(objtrFile)
				objtrFile.style.display = "";
			if(objtrHTML1)
				objtrHTML1.style.display = "none";
			if (objspHTML)
				objspHTML.style.display = "none";
			if(objtrHTML2)
				objtrHTML2.style.display = "none";
			if(objtrLink1)
				objtrLink1.style.display = "none";			
			if(objtrLink2)
				objtrLink2.style.display = "none";
		}
		
		function ShowHTML()
		{
			GetObjects();
			if(objtrFile)
				objtrFile.style.display = "none";
			if(objtrHTML1)
				objtrHTML1.style.display = "";
			if(objtrHTML2)
				objtrHTML2.style.display = "";
			if(objspHTML) objspHTML.style.display = "";
			else
			{
				GetObjects();
				if(objspHTML) objspHTML.style.display = "";
			}
			if(objtrLink1)
				objtrLink1.style.display = "none";
			if(objtrLink2)
				objtrLink2.style.display = "none";
		}
		function ShowLink()
		{
			GetObjects();
			if(objtrFile)
				objtrFile.style.display = "none";
			if(objtrHTML1)
				objtrHTML1.style.display = "none";
			if (objspHTML) objspHTML.style.display = "none";
			if(objtrHTML2)
				objtrHTML2.style.display = "none";
			if(objtrLink1)
				objtrLink1.style.display = "";
			if(objtrLink2)
				objtrLink2.style.display = "";			
		}		
		
	function AddCategory(CategoryType)
	{
			var w = 640;
			var h = 350;
			var l = (screen.width - w) / 2;
			var t = (screen.height - h) / 2;
			winprops = 'resizable=0, height='+h+',width='+w+',top='+t+',left='+l;
			var f = window.open('../Common/AddCategory.aspx?DictType=' + CategoryType, "AddCategory", winprops);
	}		
		
//-->
</script>
<table class="ibn-stylebox" cellspacing="0" cellpadding="0" width="100%" border="0" style="MARGIN-TOP:1px">
	<tr>
		<td colspan="2"><ibn:blockheader id="tbSave" runat="server"></ibn:blockheader></td>
	</tr>
	<tr>
		<td style="padding:5px" width="50%" valign="top">
			<ibn:BlockHeaderLight runat="server" id="hdrBasicInfo" CollapsibleControlId="BasicInfoTable"></ibn:BlockHeaderLight>
			<table class="ibn-stylebox-light text" cellspacing="0" cellpadding="7" width="100%" border="0" runat="server" id="BasicInfoTable">
				<tr>
					<td width="120">
						<b><%=LocRM.GetString("Title")%>:</b>
						<asp:requiredfieldvalidator id="RequiredFieldValidator1" runat="server" ControlToValidate="txtTitle" ErrorMessage="*"></asp:requiredfieldvalidator>
					</td>
					<td class="ibn-value">
						<asp:textbox id="txtTitle" runat="server" CssClass="text" Width="99%"></asp:textbox>
					</td>
				</tr>
				<tr>
					<td valign="top"><b><%=LocRM.GetString("Description")%>:</b></td>
					<td valign="top">
						<asp:TextBox Runat="server" ID="ftbDescription" Width="99%" Height="120" CssClass="text" TextMode="MultiLine"></asp:TextBox>
					</td>
				</tr>
				<tr id="trProject" runat="server">
					<td><b><%=LocRM.GetString("Project")%>:</b></td>
					<td class="ibn-value">
						<ibn:ObjectDD ID="ucProject" ObjectTypes="3" runat="server" Width="260px" />
						<asp:label id="lblProject" runat="server" Font-Bold="True"></asp:label>
					</td>
				</tr>
				<tr>
					<td><b><%=LocRM.GetString("Manager")%>:</b></td>
					<td>
						<asp:DropDownList ID="ddlManager" runat="server" CssClass="text" Width="99%"></asp:DropDownList>
					</td>
				</tr>
				<tr runat="server" id="trStatus">
					<td valign="top"><b><%=LocRM.GetString("Status")%>:</b></td>
					<td><asp:dropdownlist id="ddlStatus" runat="server" Width="260px"></asp:dropdownlist></td>
				</tr>
			</table>
		</td>
		<td style="padding:5px" width="50%" valign="top" id="tdRight" runat="server">
			<ibn:BlockHeaderLight runat="server" id="hdrCategoryInfo" CollapsibleControlId="CategoryInfoTable"></ibn:BlockHeaderLight>
			<table class="ibn-stylebox-light text" cellspacing="0" cellpadding="7" width="100%" border="0" runat="server" id="CategoryInfoTable">
				<tr id="trPriority" runat="server">
					<td><b><%=LocRM.GetString("Priority")%>:</b></td>
					<td><asp:dropdownlist id="ddlPriority" runat="server" Width="260px"></asp:dropdownlist></td>
				</tr>
				<tr id="trTaskTime" runat="server">
					<td><b><%=LocRM2.GetString("taskTime")%>:</b></td>
					<td class="ibn-value">
						<ibn:Time id="ucTaskTime" ShowTime="HM" HourSpinMaxValue="999" ViewStartDate="True" runat="server" />
					</td>
				</tr>
				<tr id="trClient" runat="server">
					<td><b><%=LocRM2.GetString("tClient")%>:</b></td>
					<td class="ibn-value"><ibn:EntityDD ObjectTypes="Contact,Organization" id="ClientControl" runat="server" Width="260px"/></td>
				</tr>
				<tr id="trCategory" runat="server">
					<td vAlign="top"><b><%=LocRM.GetString("Category")%>:</b></td>
					<td class="ibn-value">
						<asp:listbox id="lbCategory" runat="server" Width="260px" Rows="6" SelectionMode="Multiple"></asp:listbox><br />
						<div align="right" style="width:260px">
							<button id="btnAddGeneralCategory" runat="server" style="border:0px;padding:0;position:relative;top:0px;left:0px;height:20px;width:22px;background-color:transparent" type="button"><img alt="" height="20" src="../layouts/images/icons/dictionary_edit.gif" width="22" border="0" /></button>
						</div>
					</td>
				</tr>
			</table>
		</td>
	</tr>
	<tr id="trHtmlAttach" runat="server">
		<td style="padding:5px;" width="100%" valign="top" colspan="2">
			<ibn:BlockHeaderLight runat="server" id="hdrAttach" CollapsibleControlId="AttachTable"></ibn:BlockHeaderLight>
			<table class="ibn-stylebox-light text" cellspacing="0" cellpadding="5" width="100%" border="0" runat="server" id="AttachTable">
				<tr id="trLoad" runat="server">
					<td width="120px"><b><%=LocRM.GetString("Load")%>:</b></td>
					<td class="ibn-value">
						<asp:RadioButton ID="rbFile" Runat="server" GroupName="f"></asp:RadioButton>
						<asp:RadioButton id="rbLink" runat="server" GroupName="f"></asp:RadioButton>											
						<asp:RadioButton ID="rbHTML" Runat="server" GroupName="f"></asp:RadioButton>
					</td>
				</tr>
				<tr id="trFile" runat="server">
					<td><b><%=LocRM.GetString("File")%>:</b></td>
					<td class="ibn-value">
						<cc1:mchtmlinputfile id="fAssetFile" runat="server" Width="260px" CssClass="text"></cc1:mchtmlinputfile>
						<span class="text" id="vFile" style="COLOR: red;" runat="server">*</span>
					</td>
				</tr>
				<tr id="trLink1" runat="server">
					<td class="text" valign="middle"><b><%=LocRM.GetString("LinkTitle")%>:</b></td>
					<td align="left" class="ibn-value">
						<asp:textbox id="tbLinkTitle" runat="server" width="330px" cssclass="text" textmode="SingleLine" autopostback="false" style="MARGIN:0px"></asp:textbox>
					</td>
				</tr>										
				<tr id="trLink2" runat="server">
					<td class="text" valign="middle"><b><%=LocRM.GetString("Link")%>:</b></td>
					<td align="left" class="ibn-value">
						<asp:textbox id="tbLink" runat="server" width="330px" cssclass="text" textmode="SingleLine" autopostback="false" style="MARGIN:0px"></asp:textbox>
					</td>
				</tr>								
				<tr id="trHTML1" runat="server">
					<td><b><%=LocRM.GetString("HTMLTitle")%>:</b></td>
					<td class="ibn-value">
						<asp:TextBox ID="tbHtmlFileTitle" runat="server" Width="330" CssClass="text"></asp:TextBox>
					</td>
				</tr>
				<tr id="trHTML2" runat="server">
					<td valign="top"><b><%=LocRM.GetString("HTMLText")%>:</b></td>
					<td align="left" class="ibn-value">
						<span id="spHtml">
							<FTB:FreeTextBox id="fckEditor" 
							ToolbarLayout="fontsizesmenu,undo,redo,bold,italic,underline, createlink,fontforecolorsmenu,fontbackcolorsmenu" 
							runat="server" Width="100%" Height="300px" EnableHtmlMode="true" 
							DropDownListCssClass="text"  StartMode="DesignMode"
							GutterBackColor="#F5F5F5" BreakMode="LineBreak" BackColor="#F5F5F5"
							SupportFolder="~/Scripts/FreeTextBox/"
							JavaScriptLocation="ExternalFile" 
							ButtonImagesLocation="ExternalFile"
							ToolBarImagesLocation="ExternalFile" />
						</span>
					</td>
				</tr>		
			</table>
		</td>
	</tr>
	<tr>
		<td colspan="2">
			<ibn:MetaDataInternalEditControl runat="server" id="EditControl" MetaClassName="DocumentsEx"></ibn:MetaDataInternalEditControl>
		</td>
	</tr>
	<tr>
		<td colspan="2" align="right" style="PADDING-RIGHT:10px">
			<table>
				<tr>
					<td>
						<btn:imbutton class="text" id="btnSave" style="width:110px;" Runat="server" onserverclick="btnSave_ServerClick" />&nbsp;&nbsp;
						<btn:imbutton class="text" id="btnCancel" style="width:110px;" Runat="server" IsDecline="true" CausesValidation="false" onserverclick="btnCancel_ServerClick" />
					</td>
				</tr>
				<tr>
					<td>
						<asp:CheckBox Checked="False" CssClass="text" Runat="server" ID="cbOneMore"></asp:CheckBox>
					</td>
				</tr>
			</table>
		</td>
	</tr>
</table>
<script type="text/javascript">
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
