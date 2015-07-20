<%@ Reference Control="~/Reports/Modules/FilterControls/StringFilter.ascx" %>
<%@ Reference Control="~/Reports/Modules/FilterControls/IntFilter.ascx" %>
<%@ Reference Control="~/Reports/Modules/FilterControls/DictionaryFilter.ascx" %>
<%@ Reference Control="~/Reports/Modules/FilterControls/DateFilter.ascx" %>
<%@ Reference Control="~/Reports/Modules/FilterControls/TimeFilter.ascx" %>
<%@ Reference Control="~/Modules/BlockHeaderLightWithMenu.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Reports.Modules.XMLReport" Codebehind="XMLReport.ascx.cs" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeaderLight" src="~/Modules/BlockHeaderLightWithMenu.ascx" %>
<%@ Register TagPrefix="ftc" TagName="DateFltr" src="~/Reports/Modules/FilterControls/DateFilter.ascx" %>
<%@ Register TagPrefix="ftc" TagName="DictFltr" src="~/Reports/Modules/FilterControls/DictionaryFilter.ascx" %>
<%@ Register TagPrefix="ftc" TagName="IntFltr" src="~/Reports/Modules/FilterControls/IntFilter.ascx" %>
<%@ Register TagPrefix="ftc" TagName="StringFltr" src="~/Reports/Modules/FilterControls/StringFilter.ascx" %>
<%@ Register TagPrefix="ftc" TagName="TimeFltr" src="~/Reports/Modules/FilterControls/TimeFilter.ascx" %>
<script type="text/javascript" src='<%= ResolveClientUrl("~/scripts/List2List.js") %>'></script>
<script type="text/javascript">
	function FieldExistence (sender,args)
	{
		if((document.forms[0].<%=lbSelectedFields.ClientID%> != null)&& (document.forms[0].<%=lbSelectedFields.ClientID%>.options.length>0))
		{
			args.IsValid = true;	
			return;
		}
		args.IsValid = false;	
	}
	function MoveSort()
	{
		<%=Page.ClientScript.GetPostBackEventReference(btnAdd,"") %>
	}
	function SaveFields()	
	{
		var sControl=document.forms[0].<%=lbSelectedFields.ClientID%>;
		var str="";
		if(sControl != null)
		{
			for(var i=0;i<sControl.options.length;i++)
			{
				str += sControl.options[i].value + ",";
			}
		}
		document.getElementById('<%=iFields.ClientID%>').value = str;
	}
	function CheckField(obj)
	{
		var str = obj.parentNode.lastChild.id;
		document.getElementById('<%=changedCheck.ClientID%>').value = str;
		<%= Page.ClientScript.GetPostBackEventReference(lbFilterCheck, "")%>
	}
	function ChangePicture(obj)
	{
		var fobj = document.getElementById(obj.id + "_0");
		var newobj = document.forms[0].<%=imgType.ClientID%>;
		if(fobj!=null && fobj.checked && newobj!=null)
			newobj.src = "../layouts/images/pp2.gif";
		else if(fobj!=null && !fobj.checked && newobj!=null)
			newobj.src = "../layouts/images/pp1.gif";
	}
	function ChangeTemplate(obj)
	{
		if(obj.id=="rbNewRep")
		{
			document.forms[0].<%=ddTemplates.ClientID%>.disabled = true;
			document.forms[0].<%=valMode.ClientID%>.value = "Rep";
		}
		else
		{
			document.forms[0].<%=ddTemplates.ClientID%>.disabled = false;
			document.forms[0].<%=valMode.ClientID%>.value = "Both1";
		}
	}
</script>
<script type="text/javascript">
function OpenWindow(query,w,h,scroll)
	{
		var l = (screen.width - w) / 2;
		var t = (screen.height - h) / 2;
		
		winprops = 'resizable=1, height='+h+',width='+w+',top='+t+',left='+l;
		if (scroll) winprops+=',scrollbars=1';
		var f = window.open(query, "_blank", winprops);
	}
</script>
<asp:panel id="step1" Runat="server">
	<br />
	<table height="60%" width="70%" align="center">
		<tr>
			<td valign="middle" width="50%">
				<ibn:BlockHeaderLight id="secElements" runat="server" />
				<table class="ibn-stylebox-light text" cellspacing="0" cellpadding="7" width="100%" border="0">
					<tr>
						<td width="50%" align="center">
							<asp:RadioButtonList id="rbReportItem" Runat="server" CssClass="text"></asp:RadioButtonList></td>
						<td valign="top" align="center" width="60"><img alt="" src='<%=ResolveClientUrl("~/layouts/images/quicktip.gif") %>' border="0" />
						</td>
						<td class="text" style="PADDING-RIGHT: 15px" valign="top"><%=LocRM.GetString("s1Comments") %></td>
					</tr>
				</table>
				<div id="fsChoice" runat="server">
				<ibn:BlockHeaderLight id="secChoice" runat="server" />
				<table class="ibn-stylebox-light text" cellspacing="0" cellpadding="7" width="100%" border="0">
					<tr height="35px">
						<td style="padding-left:56px; width:100px">
							<input onclick="ChangeTemplate(this)" type="radio" id="rbNewRep" checked name="radNewByTemp"/>
							<label for="rbNewRep" id="lblNewRep">
							<%=LocRM.GetString("tNewRep")%></label>
						</td>
						<td>
							<input onclick="ChangeTemplate(this)" type="radio" id="rbByTemp" name="radNewByTemp"/>
							<label for="rbByTemp">
								<%=LocRM.GetString("tByTemp")%>
							</label>
						</td>
						<td></td>
					</tr>
					<tr height="35px">
						<td></td>
						<td colspan="2" align="left" style="padding-left:8px">
							<asp:DropDownList Enabled="false" ID="ddTemplates" runat="server" Width="250px" CssClass="text"></asp:DropDownList>
						</td>
					</tr>
				</table>
				</div>
			</td>
		</tr>
	</table>
</asp:panel>
<asp:panel id="step2" Runat="server" align="center">
	<br />
	<table height="60%" width="70%">
		<tr>
			<td valign="middle">
				<ibn:BlockHeaderLight id="secSelField" runat="server" />
				<table class="ibn-stylebox-light text" cellspacing="0" cellpadding="7" width="100%" border="0">
					<tr>
						<td style="padding:15px 0px 15px 15px;"><b><%=LocRM.GetString("tReportFields")%>:</b></td>
					</tr>
					<tr>
						<td style="padding:0px 0px 15px 15px;" align="center" colspan="3">
							<table class="text" id="tblGroups" cellpadding="0" border="0">
								<tr>
									<td valign="top" noWrap width="200px" style="PADDING-RIGHT: 6px; PADDING-BOTTOM: 6px">
										<asp:Label id="lblAvailable" Runat="server" CssClass="text ibn-label"></asp:Label><br />
										<asp:listbox id="lbAvailableFields" runat="server" cssclass="text" Rows="9" width="195px"></asp:listbox>
										<asp:customvalidator id="FieldValidator" style="VERTICAL-ALIGN: top" runat="server" ClientValidationFunction="FieldExistence" ErrorMessage="*"></asp:customvalidator></td>
									<td style="PADDING-RIGHT: 6px; PADDING-LEFT: 6px; PADDING-BOTTOM: 6px">
										<p align="center">
											<asp:button id="btnAddOneGr" style="MARGIN: 1px" runat="server" cssclass="text" width="30px" CausesValidation="False" text=">"></asp:button><br />
											<asp:button id="btnAddAllGr" style="MARGIN: 1px" runat="server" cssclass="text" width="30px" CausesValidation="False" text=">>"></asp:button><br />
											<br />
											<asp:button id="btnRemoveOneGr" style="MARGIN: 1px" runat="server" cssclass="text" width="30px" CausesValidation="False" text="<"></asp:button><br />
											<asp:button id="btnRemoveAllGr" style="MARGIN: 1px" runat="server" cssclass="text" width="30px" CausesValidation="False" text="<<"></asp:button></p>
									</td>
									<td valign="top" width="200px" style="PADDING-RIGHT: 20px; PADDING-LEFT: 6px; PADDING-BOTTOM: 6px">
										<asp:Label id="lblSelected" Runat="server" CssClass="text ibn-label"></asp:Label><br />
										<asp:listbox id="lbSelectedFields" runat="server" cssclass="text" Rows="9" width="195px"></asp:listbox>
									</td>
								</tr>
							</table>
						</td>
					</tr>
				</table>
			</td>
		</tr>
	</table>
	<input id="iFields" type="hidden" name="iFields" runat="server" />
</asp:panel>
<asp:panel id="step3" Runat="server">
	<br />
	<table height="60%" width="70%" align="center">
		<tr>
			<td valign="middle" align="center">
				<ibn:BlockHeaderLight id="secGroup" runat="server" />
				<table class="ibn-stylebox-light text" style="padding:15px 0px 15px 20px;" cellspacing="0" cellpadding="7" width="100%" border="0">
					<tr>
						<td width="50%">
							<asp:DropDownList id="ddGroup1" Runat="server" CssClass="text" Width="200px"></asp:DropDownList><br />
							<br />
							<asp:DropDownList id="ddGroup2" Runat="server" CssClass="text" Width="200px"></asp:DropDownList>
							<asp:CompareValidator id="compareGroups" Runat="server" CssClass="text" ErrorMessage="*" Operator="NotEqual" ControlToValidate="ddGroup2" ControlToCompare="ddGroup1"></asp:CompareValidator></td>
						<td valign="top" align="center" width="60"><img alt="" src='<%=ResolveClientUrl("~/layouts/images/quicktip.gif") %>' border="0" />
						</td>
						<td class="text" style="PADDING-RIGHT: 15px" valign="top"><%=LocRM.GetString("s3Comments") %></td>
					</tr>
				</table>
			</td>
		</tr>
	</table>
	<input id="iGroups" type="hidden" name="iGroups" runat="server" />
</asp:panel>
<asp:panel id="step4" Runat="server">
<table class="ibn-stylesheet" width="100%">
	<tr>
		<td width="175px" valign="top">
			<ibn:BlockHeaderLight id="secFilterFields" runat="server" />
			<table style="HEIGHT: 320px;margin:0;padding:2px" class="ibn-stylebox-light text" cellspacing="0" cellpadding="0" width="100%" border="0">
			<tr><td>
			<div style="WIDTH: 165px; OVERFLOW-Y: auto;OVERFLOW:auto; HEIGHT: 290px; PADDING-BOTTOM:20px">
			<asp:DataList Runat="server" ItemStyle-CssClass="text" id="dlFilterFields" CellPadding="0" CellSpacing="0" ShowHeader="False" ShowFooter="False">
				<ItemStyle CssClass="ibn-propertysheet" Font-Size="10"></ItemStyle>
				<SelectedItemStyle CssClass="UserCellSelected ibn-menuimagecell" Font-Size="10"></SelectedItemStyle>
				<ItemTemplate>
					<nobr>
					<input onclick='CheckField(this);' type="checkbox" class="text" runat="server" id="cbFilter" name="cbFilter"
						checked='<%# (bool)DataBinder.Eval(Container.DataItem, "IsChecked") %>'/>
					<asp:linkbutton id="lbField" commandname='<%# DataBinder.Eval(Container.DataItem, "FieldName") %>' runat="server" causesvalidation="False">
						<%# DataBinder.Eval(Container.DataItem, "FriendlyName") %>
					</asp:linkbutton></nobr>
				</ItemTemplate>
			</asp:DataList>
			</div></td></tr>
			</table>
		</td>
		<td valign="top">
			<ibn:BlockHeaderLight id="secFilterFieldInfo" runat="server" />
			<table style="HEIGHT: 160px;margin:0;padding:2px;table-layout:fixed;" class="ibn-stylebox-light text" cellspacing="0" cellpadding="0" width="100%" border="0">
				<tr>
					<td valign="top" align="left" style="PADDING-TOP:15px; PADDING-LEFT:15px;">
						<ftc:DateFltr id="dtFltr" runat="server" Visible="False"></ftc:DateFltr>
						<ftc:DictFltr id="dictFltr" runat="server" Visible="False"></ftc:DictFltr>
						<ftc:IntFltr id="intFltr" runat="server" Visible="False"></ftc:IntFltr>
						<ftc:StringFltr id="strFltr" runat="server" Visible="False"></ftc:StringFltr>
						<ftc:TimeFltr id="timeFltr" runat="server" Visible="False"></ftc:TimeFltr>
					</td>
				</tr>
			</table>
			<ibn:BlockHeaderLight id="secFilterValues" runat="server" />
			<table style="HEIGHT: 140px;margin:0;padding:2px;table-layout:fixed;" class="ibn-stylebox-light text" cellspacing="0" cellpadding="0" width="100%" border="0">
				<tr><td>
				<div style="WIDTH: 95%; OVERFLOW-Y: auto;OVERFLOW-X:auto; HEIGHT: 110px; PADDING-BOTTOM:10px">
					<table>
						<tr>
							<td valign="top" width="70px" align="right"><%=LocRM.GetString("s6Filter")%>:&nbsp;</td>
							<td>
								<asp:Label ID="lblCurrentFilter" Runat="server" CssClass="text"></asp:Label>
							</td>
						</tr>
					</table>
				</div>
				</td></tr>
			</table>
		</td>
	</tr>
</table>
</asp:panel>
<asp:panel id="step5" Runat="server">
	<br />
	<table height="60%" width="70%" align="center">
		<tr>
			<td valign="middle" align="center">
				<ibn:BlockHeaderLight id="secSort" runat="server" />
				<table class="ibn-stylebox-light text" style="padding:15px 0px 15px 10px;" cellspacing="0" cellpadding="7" width="100%" border="0">
					<tr>
						<td width="260px"><b><%=LocRM.GetString("tAvailable") %>:</b></td>
						<td><b><%=LocRM.GetString("tSelected") %>:</b></td>
					</tr>
					<tr>
						<td valign="top" width="260px">
							<!-- Fields -->
							<table class="text" style="MARGIN-TOP: 5px" cellspacing="0" cellpadding="3" width="100%">
								
								<tr>
									<td valign="top"><b><%=LocRM.GetString("tFields") %>:</b></td>
									<td valign="top"><asp:listbox id="lbFields" runat="server" Width="190px" CssClass="text" Rows="12" SelectionMode="Single"></asp:listbox></td>
								</tr>
								<tr>
									<td valign="top"></td>
									<td valign="top"><asp:RadioButtonList ID="rbAscDesc" runat="server" CssClass="text" RepeatDirection="Horizontal"></asp:RadioButtonList></td>
								</tr>
								<tr>
									<td valign="top" height="28">&nbsp;</td>
									<td><asp:button id="btnAdd" runat="server" Width="90px" CssClass="text" CausesValidation="False" onclick="btnAdd_Click"></asp:button></td>
								</tr>
							</table> 
							<!-- End Groups & Users -->
						</td>
						<td valign="top" height="100%">
							<!-- Data GRID -->
							<div style="OVERFLOW-Y: auto; HEIGHT: 220px">
								<asp:DataGrid id="dgSortFields" Runat="server" Width="100%" borderwidth="0px" CellSpacing="3" gridlines="None" cellpadding="3" AllowSorting="False" AllowPaging="False" AutoGenerateColumns="False">
									<ItemStyle CssClass="text"></ItemStyle>
									<HeaderStyle CssClass="text" Font-Italic="True" Font-Bold="True"></HeaderStyle>
									<Columns>
										<asp:BoundColumn HeaderText='Sort Field' DataField="Field" Visible="False"></asp:BoundColumn> 
										<asp:BoundColumn HeaderText='Sort Field' DataField="FieldText"></asp:BoundColumn> 
										<asp:TemplateColumn HeaderText='Sort Direct' itemstyle-width="50">
											<ItemTemplate>
												<%# GetSortDirect( (int)DataBinder.Eval(Container.DataItem, "SortDirect"))%>
											</ItemTemplate>
										</asp:TemplateColumn>
										<asp:templatecolumn itemstyle-width="30" Visible="True">
											<itemtemplate>
												<asp:imagebutton id="ibDelete" runat="server" borderwidth="0" width="16" height="16" imageurl="~/layouts/images/DELETE.GIF" commandname="Delete" causesvalidation="False"></asp:imagebutton>
											</itemtemplate>
										</asp:templatecolumn>
									</Columns>
								</asp:DataGrid>
								<!-- End Data GRID -->
							</div>
						</td>
					</tr>
				</table>
			</td>
		</tr>
	</table>
</asp:panel>
<asp:panel id="step6" Runat="server">
	<div class="text"><img alt="" height="20" src='<%=ResolveClientUrl("~/layouts/images/help.gif") %>' width="20" align="absMiddle" />
		&nbsp;<%=LocRM.GetString("s6TopText") %>
	</div>
	<br />
	<table height="60%" width="70%" align="center">
		<tr>
			<td valign="top" align="center">
				<ibn:BlockHeaderLight id="secSettings" runat="server" />
				<table class="ibn-stylebox-light text" style="padding:15px 0px 15px 10px;" cellspacing="0" cellpadding="7" width="100%" border="0">
					<tr>
						<td width="90px"><b><%=( (_pc["Cust_Rep_valMode"].ToString()=="Temp")? LocRM.GetString("tTemplateTitle") : LocRM.GetString("s6ReportName")) %>:</b>
						</td>
						<td colspan="4">
							<asp:TextBox id="lblReportName" Runat="server" CssClass="text" Width="220px"></asp:TextBox>
						</td>
					</tr>
					<tr runat="server" id="trRadioGroup">
						<td><b><%=LocRM.GetString("s6GroupType") %>:</b>
						</td>
						<td>
							<asp:RadioButtonList onclick="ChangePicture(this)" runat="server" ID="rblGrouptype" RepeatColumns="2" RepeatDirection="Horizontal">
							</asp:RadioButtonList>
						</td>
						<td>
							<asp:Image ID="imgType" runat="server" BorderWidth="0" Height="30px" Width="30px" ImageAlign="AbsMiddle" ImageUrl="~/layouts/images/pp1.gif"></asp:Image>
						</td>
						<td valign="top"></td>
						<td>
							<asp:CheckBox ID="cbShowEmptyItems" runat="server" CssClass="text" Checked="true"></asp:CheckBox>
						</td>
					</tr>
					<tr runat="server" id="trTemplateSetsRow">
						<td valign="top"></td>
						<td colspan="2">
							
						</td>
						<td valign="top"></td>
						<td>
							<asp:CheckBox ID="cbGenerateNow" runat="server" CssClass="text"></asp:CheckBox>
						</td>
					</tr>
					<tr>
						<td colspan="5" align="right">
							<asp:Button ID="btnPreview" runat="server" CssClass="text"></asp:Button>
						</td>
					</tr>
				</table>
			</td>
		</tr>
	</table>
</asp:panel>
<asp:panel id="step7" Runat="server">
	<div class="text"><img alt="" height="20" src='<%=ResolveClientUrl("~/layouts/images/help.gif")%>' width="20" align="absMiddle" />
		&nbsp;<%=LocRM.GetString("s7TopText") %>
	</div>
	<br />
	<table width="70%" align="center">
		<tr>
			<td valign="middle" align="center">
				<ibn:BlockHeaderLight id="secPreview" runat="server" />
				<table class="ibn-stylebox-light text" style="padding:15px 0px 15px 10px; HEIGHT:280px;" cellspacing="0" cellpadding="7" width="100%" border="0">
					<tr><td valign="top">
					<div style="WIDTH: 100%; OVERFLOW-Y: auto; HEIGHT: 230px; PADDING-BOTTOM:10px">
					<table cellspacing="0" cellpadding="0" width="100%" border="0">
						<tr>
							<td valign="top" width="50%">
								<table cellspacing="0" cellpadding="0" width="100%" border="0">
									<tr>
										<td valign="top">
											<table class="subHeader" cellspacing="0" cellpadding="5" width="100%" border="0">
												<tr>
													<td width="100"><b><%=LocRM.GetString("s6ReportName") %>:</b></td>
													<td><asp:TextBox id="txtReportTitle" Runat="server" CssClass="text" Width="220px"></asp:TextBox></td>
												</tr>
												<tr>
													<td></td>
													<td><asp:CheckBox ID="cbOnlyForMe" runat="server" CssClass="text"></asp:CheckBox></td>
												</tr>
												<tr>
													<td valign="top"><b><%=LocRM.GetString("s6Fields") %>:</b>
													</td>
													<td>
														<asp:Label id="lblFields" Runat="server"></asp:Label></td>
												</tr>
												<tr>
													<td valign="top"><b><%=LocRM.GetString("s6GroupFields") %>:</b>
													</td>
													<td>
														<asp:Label id="lblGroupFields" Runat="server"></asp:Label></td>
												</tr>
												<tr>
													<td valign="top"><b><%=LocRM.GetString("s6SortFields") %>:</b>
													</td>
													<td>
														<asp:Label id="lblSortFields" Runat="server"></asp:Label></td>
												</tr>
												<tr>
													<td valign="top"><b><%=LocRM.GetString("s6Filter") %>:</b>
													</td>
													<td>
														<asp:Label id="lblFilter" Runat="server"></asp:Label></td>
												</tr>
											</table>
										</td>
									</tr>
								</table>
							</td>
						</tr>
					</table>
					</div>
					</td></tr>
				</table>
			</td>
		</tr>
	</table>
</asp:panel>
<asp:panel id="step8" Runat="server">
	<div style="width:100%">&nbsp;</div>
</asp:panel>
<input id="pastStep" type="hidden" value="0" name="pastStep" runat="server" />
<input id="ResultXML" type="hidden" value="" name="ResultXML" runat="server" />
<input id="pastCommand" type="hidden" value="" name="pastCommand" runat="server" />
<input id="changedCheck" type="hidden" value="" name="changedCheck" runat="server" />
<input id="Elem" type="hidden" value="0" name="Elem" runat="server" />
<input id="valMode" type="hidden" value="0" name="valMode" runat="server" />
<asp:LinkButton ID="lbFilterCheck" Runat="server" Visible="False" onclick="lbFilterCheck_Click"></asp:LinkButton>