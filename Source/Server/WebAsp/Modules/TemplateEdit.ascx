<%@ Control Language="c#" Inherits="Mediachase.Ibn.WebAsp.Modules.TemplateEdit" Codebehind="TemplateEdit.ascx.cs" %>
<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Register TagPrefix="FTB" Namespace="FreeTextBoxControls" Assembly="FreeTextBox" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="~/Modules/BlockHeader.ascx" %>
<%@ Register TagPrefix="cc1" Namespace="Mediachase.FileUploader.Web.UI" Assembly="Mediachase.FileUploader" %>
<script language="javascript">
	function InsertHTML(arg)
	{
		var mobj = FTB_API['<%=fckEditor.ClientID%>'];
		if(mobj)
		{
			if(mobj.mode != FTB_MODE_HTML)	
				mobj.InsertHtml("[=" +arg + "=]");
			else
			{
				mobj.GoToDesignMode();
				mobj.InsertHtml("[=" +arg + "=]");
				mobj.GoToHtmlMode();
			}
		}
	}
	
	function DeleteFile(filename)
	{
		if(confirm("Do you really want to delete this attachment?"))
		{
			document.forms[0].<%=hdnFileName.ClientID%>.value = filename;
			<%= Page.GetPostBackClientEvent(btnDelete, "")%>
		}
	}
</script>
<table class="ibn-stylebox text" style="MARGIN-TOP: 10px" cellspacing="0" cellpadding="0" width="100%" border="0">
	<tr>
		<td><ibn:blockheader id="secHeader" runat="server"></ibn:blockheader></td>
	</tr>
	<tr>
		<td style="PADDING-LEFT: 15px; PADDING-BOTTOM: 15px; PADDING-TOP: 15px" align="left">
			<table class="text" cellspacing="3" cellpadding="3" width="100%" border="0">
				<tr>
					<td vAlign="top" align="right" width="160" colSpan="2" class="ibn-sectionheader">Language:</td>
					<td class=ibn-sectionheader vAlign=top align=left>
						<asp:DropDownList id=ddLanguage runat="server" AutoPostBack="True" onselectedindexchanged="ddLanguage_Change"></asp:DropDownList></td>
					<td></td>
				</tr>
				<tr>
					<td vAlign="top" align="right" width="160" colSpan="2" class="ibn-sectionheader">Template 
						Name:</td>
					<td vAlign="top" align="left" class="ibn-sectionheader"><%=TemplateName %></td>
					<td></td>
				</tr>
				<tr>
					<td vAlign="top" align="right" width="160" colSpan="2" class="ibn-sectionheader">Subject:</td>
					<td vAlign="center" align="left" class="ibn-sectionheader">
						<asp:TextBox ID="txtSubject" Runat="server" Width="100%" CssClass="text"></asp:TextBox>
					</td>
					<td></td>
				</tr>
				<tr>
					<td style="PADDING-TOP: 30px" vAlign="top" width="80">
						<!-- Left Frame -->
						<asp:datalist id="dlSysVar" Width="100%" RepeatColumns="1" ShowFooter="False" ShowHeader="False" CellSpacing="5" ItemStyle-CssClass="ms-propertysheet" Runat="server">
							<ItemStyle CssClass="ibn-propertysheet"></ItemStyle>
							<SelectedItemStyle CssClass="UserCellSelected ibn-menuimagecell"></SelectedItemStyle>
							<ItemTemplate>
								<a href='javascript:InsertHTML("<%# DataBinder.Eval(Container.DataItem, "Name") %>")' title='<%# DataBinder.Eval(Container.DataItem, "Description") %>'>
									<%# DataBinder.Eval(Container.DataItem, "Name") %>
								</a>
							</ItemTemplate>
						</asp:datalist>
						<!-- End Left Frame -->
					</td>
					<td style="PADDING-TOP: 10px" vAlign="top" align="left" width="60" class="ibn-sectionheader">Body:</td>
					<td style="PADDING-TOP: 10px" vAlign="top" align="left" width="50%">
					  <FTB:FreeTextBox id="fckEditor" ToolbarLayout="fontsizesmenu,undo,redo,bold,italic,underline, createlink,fontforecolorsmenu,fontbackcolorsmenu" 
						  runat="Server" Width="98%" Height="300px" DropDownListCssClass = "text" 
						  GutterBackColor="#F5F5F5"  BreakMode = "LineBreak" BackColor="#F5F5F5"
						  StartMode="DesignMode"
						  SupportFolder = "~/Scripts/FreeTextBox/"
						  JavaScriptLocation="ExternalFile" 
						  ButtonImagesLocation="ExternalFile"
						  ToolBarImagesLocation="ExternalFile" />
					</td>

					<td style="PADDING-RIGHT: 15px; PADDING-LEFT: 10px; PADDING-TOP: 10px" vAlign="top">
						<table cellpadding="0" cellspacing="4" border="0" width="100%">
							<tr>
								<td colspan="3" class="ibn-sectionheader">Attachments:</td>
							</tr>
							<tr>
								<td colspan="3" class="ibn-sectionline" height="1"><IMG height="1" alt="" src="../layouts/images/blank.gif" width="1"></td>
							</tr>
							<tr height="10">
								<td></td>
							</tr>
							<tr>
								<td class="text">Select File:</td>
								<td width="70%">
									<cc1:McHtmlInputFile width="100%" id="fAssetFile" class="text" runat="server" />
								</td>
								<td>
									<asp:imagebutton AlternateText="Upload!" imagealign="AbsMiddle" runat="server" id="btnSubmit" imageurl="../layouts/images/ICONGO.GIF"></asp:imagebutton>
								</td>
							</tr>
							<tr height="5">
								<td></td>
							</tr>
							<tr>
								<td colspan="3" class="ibn-sectionline" height="1"><IMG height="1" alt="" src="../layouts/images/blank.gif" width="1"></td>
							</tr>
							<tr>
								<td class="ibn-propertysheet" valign="top" colspan="3">
									<asp:datagrid id="dgFiles" runat="server" allowpaging="False" allowsorting="False" cellpadding="1" gridlines="Horizontal" CellSpacing="3" borderwidth="0px" autogeneratecolumns="False" width="100%">
										<Columns>
											<asp:BoundColumn DataField="Name" HeaderText="File Name">
												<HeaderStyle CssClass="ibn-vh2"></HeaderStyle>
												<ItemStyle CssClass="ibn-vb2"></ItemStyle>
											</asp:BoundColumn>
											<asp:boundcolumn headerstyle-cssclass="ibn-vh2" itemstyle-cssclass="ibn-vb2" datafield="ActionDelete" headerstyle-width="25" itemstyle-width="25"></asp:boundcolumn>
										</Columns>
									</asp:datagrid>
								</td>
							</tr>
						</table>
					</td>
				</tr>
			</table>
			<table cellspacing="3" cellpadding="3" width="100%" align="right" border="0">
				<tr>
					<td align="right" id="diidSubmitsection">
						<asp:Button ID="btnSave" Runat="server" CssClass="text" Width="80px" onclick="btnSave_Click"></asp:Button>
					</td>
					<td align="right" width="85">
						<asp:Button ID="btnCancel" CausesValidation="false" Runat="server" CssClass="text" Width="80px" onclick="btnCancel_Click"></asp:Button>
					</td>
					<td width="6" id="idSpace"><img src="../layouts/images/blank.gif" width="6" height="1" alt=""></td>
				</tr>
			</table>
		</td>
	</tr>
</table>
<asp:button runat="server" id="btnDelete" Visible="false" onclick="btnDelete_Click"></asp:button>
<input type="hidden" id="hdnFileName" runat="server">
<script language="javascript">
	function trapsubmit()
	{
		try
		{
 		if(document.forms[0].<%=fAssetFile.ClientID%>.value!="")
		{
			ShowProgress();
		}
		
		else return false;
		}
		catch(e){}
	}
	
	function ShowProgress()
	{
			var w = 300;
			var h = 140;
			var l = (screen.width - w) / 2;
			var t = (screen.height - h) / 2;
			winprops = 'resizable=0, height='+h+',width='+w+',top='+t+',left='+l+'w';
			var f = window.open('../Pages/Progress.aspx?ID='+document.forms[0].__MEDIACHASE_FORM_UNIQUEID.value, "_blank", winprops);
	}
</script>
