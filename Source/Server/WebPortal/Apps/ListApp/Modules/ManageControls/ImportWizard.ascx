<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ImportWizard.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.ListApp.Modules.ManageControls.ImportWizard" %>
<%@ Reference Control="~/Modules/ObjectDropDownNew.ascx" %>
<%@ Register TagPrefix="cc1" Namespace="Mediachase.FileUploader.Web.UI" Assembly="Mediachase.FileUploader" %>
<%@ Register TagPrefix="frm" Namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>
<%@ Register TagPrefix="ComponentArt" Namespace="ComponentArt.Web.UI" Assembly="ComponentArt.Web.UI" %>
<%@ Register TagPrefix="ibn" TagName="ObjectDD" src="~/Modules/ObjectDropDownNew.ascx" %>

<link rel="Stylesheet" type="text/css" href='<%=this.ResolveClientUrl("~/styles/IbnFramework/dialog.css") %>' />
<link href='<%= this.ResolveClientUrl("~/styles/IbnFramework/treeStyle.css")%>' type="text/css" rel="stylesheet" />
<script type="text/javascript">
	function ChangeProject()
	{
		<%= Page.ClientScript.GetPostBackEventReference(lbChangeProject, "")%>
	}	
	
	function onNodeClick(node)
	{
		if(node.Value!=-1)
		{
			document.forms[0].<%= destFolderId.ClientID%>.value = node.Value;
		}
		return false;
	}		
	function ChangeCb(obj, stext)
	{
		var aInputs = document.getElementsByTagName("input");
		var newValue = obj.firstChild.checked;
		for (var i=0; i<aInputs.length; i++)
		{
			var oInput = aInputs[i];
			if(oInput.type == "checkbox" && oInput.name.indexOf(stext) >= 0 && oInput.checked)
			{
				oInput.checked = false;
			}
		}
		obj.firstChild.checked = newValue;
	}
	
	function resizeTable()
	{
	   var obj = document.getElementById('stepDiv');
	   var toolbarRow = document.getElementById('tableHeader');

	   var intHeight = 0;
	   if (typeof(window.innerWidth) == "number")
	   {
		  intHeight = window.innerHeight;
	   }
	   else if (document.documentElement && (document.documentElement.clientWidth || document.documentElement.clientHeight))
	   {
		  intHeight = document.documentElement.clientHeight;
	   }
	   else if (document.body && (document.body.clientWidth || document.body.clientHeight))
	   {
		  intHeight = document.body.clientHeight;
	   }
	   
		if(obj && toolbarRow)
			obj.style.height = (intHeight - toolbarRow.offsetHeight - 47) + "px";
	} 
	window.onresize=resizeTable; 
	window.onload=resizeTable; 
	
</script>
<style type="text/css">
	.SelTrNode 
	{ 
		font-family: tahoma; 
		font-size: 11px; 
		background-color: gray; 
		color:white; 
		padding-top:2px;
		padding-bottom:1px;
		padding-left: 3px; 
		padding-right: 3px; 
		cursor: pointer;
	}
</style>
<style type="text/css">
	table.padTable5 tbody tr td
	{
		padding: 5px ! important;
	}
	table.padTable3 tbody tr td
	{
		padding: 3px ! important;
	}
	table.padTable2 tbody tr td
	{
		padding: 2px ! important;
	}
</style>
<asp:Wizard runat="server" ID="ucWizard" Width="100%" CssClass="text">
	<HeaderStyle VerticalAlign="Top" />
	<CancelButtonStyle Width="90px" />
	<NavigationButtonStyle Width="90px" />
	<FinishCompleteButtonStyle Width="90px" />
	<StepStyle CssClass="wizardStep" VerticalAlign="top" />
	<NavigationStyle CssClass="wizardFooter" />
	<HeaderTemplate>
		<table width="100%" class="borderBottom" id="tableHeader">
			<tr>
				<td class="topHeader headerPadding" valign="top">
					<%=HeaderText%>
				</td>
			</tr>
			<tr>
				<td valign="top" class="subHeaderPadding">
					<table cellspacing="0" cellpadding="0" width="100%" border="0">
						<tr>
							<td class="topSubHeader">
								<%=SubHeaderText%>
							</td>
							<td class="step" align="right">
								<%=StepText%>
							</td>
						</tr>
					</table>
				</td>
			</tr>
		</table>
	</HeaderTemplate>
	<WizardSteps>
		<asp:WizardStep ID="step1" runat="server" StepType="Start">
			<div id="stepDiv">
			<table width="100%" class="text">
				<tr>
					<td valign="top" align="center" width="50%">
						<div style="width:70%;">
						<frm:BlockHeaderLight ID="lgdSourceType" runat="server" HeaderCssClass="ibn-toolbar-light" />
						<table width="100%" class="ibn-stylebox-light padTable5">
							<tr>
								<td width="50%" valign="top">
									<asp:RadioButtonList id="rbSourceType" Runat="server" CssClass="padTable2"></asp:RadioButtonList>
								</td>
								<td valign="top" align="center" width="60">
									<img alt="" src='<% =ResolveClientUrl("~/layouts/images/quicktip.gif") %>' border="0" />
								</td>
								<td class="text" style="PADDING-RIGHT: 15px" valign="top"><asp:Literal ID="Literal1" runat="server" Text="<%$Resources: IbnFramework.ListInfo, imStep1Comments%>" /></td>
							</tr>
						</table>
						<br />
						<frm:BlockHeaderLight ID="lgdFile" runat="server" HeaderCssClass="ibn-toolbar-light" />
						<table width="100%" class="ibn-stylebox-light padTable5" height="50px" cellpadding="5">
							<tr>
								<td width="50%">
									<cc1:McHtmlInputFile id="fSourceFile" runat="server" style="width:350px;" Size="40" CssClass="text"></cc1:McHtmlInputFile>
									<asp:RequiredFieldValidator ID="rfFile" runat="server" ControlToValidate="fSourceFile" Display="Static" ErrorMessage="*" CssClass="text"></asp:RequiredFieldValidator>
								</td>
							</tr>
						</table>
						</div>
					</td>
				</tr>
			</table>
			</div>
		</asp:WizardStep>
		<asp:WizardStep ID="step2" runat="server" StepType="Step">
			<div style="width:98%" id="stepDiv">
				<frm:BlockHeaderLight ID="lgdListType" runat="server" HeaderCssClass="ibn-toolbar-light" />
				<table width="100%" class="ibn-stylebox-light padTable5">
					<tr>
						<td>
							<asp:RadioButton AutoPostBack="true" ID="rbNewList" runat="server" GroupName="radListType" />
						</td>
						<td valign="top" align="center" width="60" rowspan="2">
							<img alt="" src='<%= ResolveClientUrl("~/layouts/images/quicktip.gif") %>' border="0" />
						</td>
						<td class="text" style="PADDING-RIGHT: 15px;width:45%;" valign="top" rowspan="2"><%=GetGlobalResourceObject("IbnFramework.ListInfo", "imStep3Comments")%></td>
					</tr>
					<tr>
						<td>
							<asp:RadioButton AutoPostBack="true" ID="rbUpdList" runat="server" GroupName="radListType" />
						</td>
					</tr>
				</table>
				<br />
				<div id="fsUpdateList" runat="server">
					<frm:BlockHeaderLight ID="lgdUpdateList" runat="server" HeaderCssClass="ibn-toolbar-light" />
				</div>
				<div id="fsNewList" runat="server">
					<frm:BlockHeaderLight ID="lgdNewList" runat="server" HeaderCssClass="ibn-toolbar-light" />
					<table width="100%" class="ibn-stylebox-light">
						<tr>
							<td style="padding:5px;">
								<b><%=GetGlobalResourceObject("IbnFramework.ListInfo", "imStep3ListTitle")%>:</b>&nbsp;&nbsp;
								<asp:textbox id="txtTitle" runat="server" CssClass="text" Width="455px"></asp:textbox>
								<asp:RequiredFieldValidator ID="rfTitle" runat="server" Display="Static" CssClass="text" ErrorMessage="*" ControlToValidate="txtTitle"></asp:RequiredFieldValidator>
							</td>
						</tr>
						<tr>
							<td style="padding:5px;">
								<b><%=GetGlobalResourceObject("IbnFramework.ListInfo", "Type")%>:</b>&nbsp;&nbsp;
								<asp:DropDownList ID="ddType" runat="server" Width="240px" CssClass="text"></asp:DropDownList>&nbsp;&nbsp;
								<b><%=GetGlobalResourceObject("IbnFramework.ListInfo", "Status")%>:</b>&nbsp;&nbsp;
								<asp:DropDownList ID="ddStatus" runat="server" Width="240px" CssClass="text"></asp:DropDownList>&nbsp;&nbsp;
							</td>
						</tr>
					</table>
					<asp:LinkButton ID="lbChangeProject" runat="server" OnClick="lbChangeProject_Click" style="display:none;"></asp:LinkButton>
				</div>
				<div>
					<table width="100%" class="ibn-stylebox-light">
						<tr>
							<td class="ibn-navline" style="padding:5px;" colspan="2">
								<asp:RadioButtonList ID="rbList" runat="server" CssClass="padTable2" AutoPostBack="true" OnSelectedIndexChanged="rbList_SelectedIndexChanged" RepeatDirection="Horizontal"></asp:RadioButtonList>
							</td>
						</tr>
						<tr id="trProject" runat="server">
							<td class="ibn-navline" colspan="2">
								<table cellspacing="0" border="0" width="100%">
									<tr>
										<td width="150px" style="padding:5px;"><b><%=GetGlobalResourceObject("IbnFramework.ListInfo", "tSelectProject") %>:</b></td>
										<td style="padding:5px;">
											<ibn:ObjectDD ID="ucProject" OnChange="ChangeProject()" ObjectTypes="3" runat="server" Width="99%" ItemCount="4" ClassName="" ViewName="" PlaceName="ListInfoList" CommandName="MC_HDM_PM_ObjectDD" />
										</td>
									</tr>
								</table>
							</td>
						</tr>
						<tr id="trTree" runat="server">
							<td style="padding:5px;" valign="top">
								<ComponentArt:TreeView id="MoveTree" Width="98%" Height="120px" 
									AutoScroll = "True" 
									BackColor="#ece9d8"
									BorderWidth = "0"
									DragAndDropEnabled="false"
									NodeEditingEnabled="false" 
									CssClass="TreeView" 
									NodeCssClass="TreeNode" 
									SelectedNodeCssClass="SelTrNode"
									HoverNodeCssClass="HoverTreeNode"
									NodeEditCssClass="NodeEdit"
									DefaultImageWidth="16" 
									DefaultImageHeight="16"
									NodeLabelPadding="2"
									ParentNodeImageUrl="~/layouts/images/folder.gif" 
									ExpandedParentNodeImageUrl="~/layouts/images/folder_open.gif"
									LeafNodeImageUrl="~/layouts/images/folder.gif" 
									ShowLines="true" 
									ClientScriptLocation = "~/Scripts/componentart_webui_client/"
									EnableViewState="True"
									LineImagesFolderUrl="~/layouts/images/lines/"
									runat="server">
								</ComponentArt:TreeView>
							</td>
							<td style="padding:5px;width:46%;" valign="top" id="tdLists" runat="server">
								<b><asp:Literal ID="Literal5" runat="server" Text="<%$ Resources:IbnFramework.ListInfo, imStep3SelectList %>"></asp:Literal>:</b>
								<div style="WIDTH: 100%; OVERFLOW-Y: auto;OVERFLOW: auto; HEIGHT: 120px; PADDING-BOTTOM:10px;">
									<asp:DataGrid ID="dgLists" runat="server" AutoGenerateColumns="false"
										AllowPaging="False" AllowSorting="false" GridLines="None" Width="90%" ShowHeader="false">
										<Columns>
											<asp:BoundColumn DataField="ListId" Visible="false"></asp:BoundColumn>
											<asp:TemplateColumn>
												<ItemStyle CssClass="ibn-vb2" />
												<ItemTemplate>
													<asp:CheckBox onchange="ChangeCb(this, 'cbListItem')" ID="cbListItem" runat="server" Text='<%# Eval("Title") %>' />
												</ItemTemplate>
											</asp:TemplateColumn>
										</Columns>	 
									</asp:DataGrid>
								</div>
							</td>
						</tr>
					</table>
				</div>
			</div>
		</asp:WizardStep>
		<asp:WizardStep ID="step3" runat="server" StepType="Finish">
			<div id="stepDiv">
			<frm:BlockHeaderLight ID="bhCSV" runat="server" HeaderCssClass="ibn-toolbar-light" />
			<table width="100%" class="ibn-stylebox-light padTable5">
				<tr id="trCSV" runat="server">
					<td><asp:Literal ID="Literal2" runat="server" Text="<%$Resources: IbnFramework.ListInfo, tDelimeter%>" />:&nbsp;&nbsp;<asp:DropDownList ID="ddDelimeter" runat="server" Width="80px" AutoPostBack="true"></asp:DropDownList></td>
					<td><asp:Literal ID="Literal3" runat="server" Text="<%$Resources: IbnFramework.ListInfo, tTextQualifier%>" />:&nbsp;&nbsp;<asp:DropDownList ID="ddTextQualifier" runat="server" Width="55px" AutoPostBack="true"></asp:DropDownList></td>
					<td><asp:Literal ID="Literal4" runat="server" Text="<%$Resources: IbnFramework.ListInfo, tEncoding%>" />:&nbsp;&nbsp;<asp:DropDownList ID="ddEncoding" runat="server" Width="125px" AutoPostBack="true"></asp:DropDownList></td>
					<td></td>
				</tr>	
				<tr id="trList" runat="server">
					<td colspan="4">
						<asp:Label id="lblName" runat="server"></asp:Label>
					</td>
				</tr>
			</table>
			<div class="ibn-propertysheet" style="padding-top:10px;">
			<frm:BlockHeaderLight ID="bhMapping" runat="server" HeaderCssClass="ibn-toolbar-light" />
			<table width="100%" class="ibn-stylebox-light" cellpadding="5">
				<tr>
					<td>
						<div style="WIDTH: 100%; OVERFLOW-Y: auto;OVERFLOW: auto; HEIGHT: 230px; PADDING-BOTTOM:20px;">
						<asp:DataGrid ID="dgMapping" runat="server" AllowPaging="false" AllowSorting="false"
							AutoGenerateColumns="false" GridLines="None" Width="97%">
							<Columns>
								<asp:BoundColumn DataField="metaFieldName" Visible="false"></asp:BoundColumn>
								<asp:BoundColumn DataField="metaField" 
									HeaderStyle-CssClass="ibn-vh2" ItemStyle-CssClass="ibn-vb2"
									HeaderText="<%$Resources: IbnFramework.ListInfo, tMetaField%>">
								</asp:BoundColumn>
								<asp:TemplateColumn HeaderText="<%$Resources : IbnFramework.ListInfo, DefaultField %>">
									<HeaderStyle CssClass="ibn-vh2" />
									<ItemStyle CssClass="ibn-vb2" Width="50px" HorizontalAlign="Center" />
									<ItemTemplate>
										<asp:CheckBox onchange="ChangeCb(this, 'cbDefaultItem')" ID="cbDefaultItem" runat="server" Visible='<%# (bool)Eval("canBeDefault") %>' Checked='<%# (bool)Eval("IsTitleField") %>' />
									</ItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="<%$Resources: IbnFramework.ListInfo, tAvailable%>">
									<HeaderStyle CssClass="ibn-vh2" />
									<ItemStyle CssClass="ibn-vb2" Width="190px" />
									<ItemTemplate>
										<asp:DropDownList ID="ddColumns" AutoPostBack="true" OnSelectedIndexChanged="ddi_SelectedIndexChanged" CssClass="text" runat="server" Width="170px"></asp:DropDownList>
									</ItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn>
									<HeaderStyle CssClass="ibn-vh2" />
									<ItemStyle CssClass="ibn-vb2" Width="170px" />
									<ItemTemplate>
										<asp:TextBox ID="tbColumn" runat="server" CssClass="text" Width="150px" Visible="false"></asp:TextBox>
										<asp:DropDownList ID="ddColumn" runat="server" CssClass="text" Width="150px" Visible="false"></asp:DropDownList>
										<asp:Label ID="lblColumn" runat="server" CssClass="text" Visible="false"></asp:Label>
									</ItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn>
									<HeaderStyle CssClass="ibn-vh-right" Width="25px" />
									<ItemStyle CssClass="ibn-vb2" Width="25px" />
									<ItemTemplate>
										<asp:ImageButton ID="imgDelete" runat="server" CommandName="Delete" CommandArgument='<%# Eval("metaFieldName") %>' CausesValidation="false" Visible='<%# (bool)Eval("canDelete") %>' ImageAlign="AbsMiddle" ImageUrl="~/layouts/images/delete.gif" />
									</ItemTemplate>
								</asp:TemplateColumn>
							</Columns>
						</asp:DataGrid>
						</div>
					</td>
				</tr>
			</table>
			</div>
			</div>
		</asp:WizardStep>
		<asp:WizardStep ID="step4" runat="server" StepType="Complete">
			<table width="100%" cellpadding="0" cellspacing="0" border="0" id="tableHeader"
				style="border-collapse:collapse; width:100%;">
				<tr>
					<td class="topHeader headerPadding" valign="top">
						<%=HeaderText%>
					</td>
				</tr>
				<tr>
					<td valign="top" class="subHeaderPadding borderBottom">
						<table cellspacing="0" cellpadding="0" width="100%" border="0">
							<tr>
								<td class="topSubHeader">
									<%=SubHeaderText%>
								</td>
								<td class="step" align="right">
									<%=StepText%>
								</td>
							</tr>
						</table>
					</td>
				</tr>
			</table>
			<div id="stepDiv" class="wizardStep" style="vertical-align: top;">
				<table style="padding:50px;" cellpadding="5" cellspacing="15" class="ibn-propertysheet">
					<tr>
						<td><img alt="" border="0" src='<%=this.Page.ResolveClientUrl("~/layouts/images/check.gif") %>' /></td>
						<td valign="top">
							<asp:Label ID="lblResult" CssClass="text" runat="server"></asp:Label>
						</td>
					</tr>
				</table>
			</div>
			<table width="100%" cellpadding="0" cellspacing="0" border="0"
				style="border-collapse:collapse; width:100%;">
				<tr>
					<td class="wizardCompleteStepFooter" align="right">
						<asp:Button ID="btnClose" runat="server" Text="Close" Width="90px" OnClick="btnClose_Click" />
					</td>
				</tr>
			</table>
		</asp:WizardStep>
	</WizardSteps>
</asp:Wizard>
<asp:HiddenField ID="hdnFilePath" Value="0" runat="server" />
<asp:HiddenField ID="hdnListId" Value="0" runat="server" />
<asp:LinkButton ID="lbErrorLog" runat="server" Visible="false"></asp:LinkButton>
<input type="hidden" id="destFolderId" runat="server" />