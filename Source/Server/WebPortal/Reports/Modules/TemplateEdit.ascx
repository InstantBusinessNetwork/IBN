<%@ Reference Control="~/Reports/Modules/FilterControls/StringFilter.ascx" %>
<%@ Reference Control="~/Reports/Modules/FilterControls/IntFilter.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Reports.Modules.TemplateEdit" CodeBehind="TemplateEdit.ascx.cs" %>
<%@ Reference Control="~/Reports/Modules/FilterControls/DictionaryFilter.ascx" %>
<%@ Reference Control="~/Reports/Modules/FilterControls/DateFilter.ascx" %>
<%@ Reference Control="~/Reports/Modules/FilterControls/TimeFilter.ascx" %>
<%@ Reference Control="~/Modules/TopTabs.ascx" %>
<%@ Reference Control="~/Modules/BlockHeaderLightWithMenu.ascx" %>
<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Register TagPrefix="ctrl" TagName="TopTab" Src="~/Modules/TopTabs.ascx" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" Src="~/Modules/BlockHeader.ascx" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeaderLight" Src="~/Modules/BlockHeaderLightWithMenu.ascx" %>
<%@ Register TagPrefix="btn" Namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>
<%@ Register TagPrefix="ftc" TagName="DateFltr" Src="~/Reports/Modules/FilterControls/DateFilter.ascx" %>
<%@ Register TagPrefix="ftc" TagName="DictFltr" Src="~/Reports/Modules/FilterControls/DictionaryFilter.ascx" %>
<%@ Register TagPrefix="ftc" TagName="IntFltr" Src="~/Reports/Modules/FilterControls/IntFilter.ascx" %>
<%@ Register TagPrefix="ftc" TagName="StringFltr" Src="~/Reports/Modules/FilterControls/StringFilter.ascx" %>
<%@ Register TagPrefix="ftc" TagName="TimeFltr" Src="~/Reports/Modules/FilterControls/TimeFilter.ascx" %>

<script type="text/javascript">
	//<![CDATA[
	function OpenWindow(query,w,h,scroll)
	{
		var l = (screen.width - w) / 2;
		var t = (screen.height - h) / 2;
		
		winprops = 'resizable=1, height='+h+',width='+w+',top='+t+',left='+l;
		if (scroll) winprops+=',scrollbars=1';
		var f = window.open(query, "_blank", winprops);
	}
	function FieldExistence (sender,args)
	{
		if((document.forms[0].<%=lbSelectedFields.ClientID%> != null)&& (document.forms[0].<%=lbSelectedFields.ClientID%>.options.length>0))
		{
			args.IsValid = true;	
			return;
		}
		args.IsValid = false;	
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
	function MoveSort()
	{
		<%=Page.ClientScript.GetPostBackEventReference(btnAdd,"") %>
	}
	function CheckField(obj)
	{
		var str = obj.parentNode.lastChild.id;
		document.getElementById('<%=changedCheck.ClientID%>').value = str;
		<%= Page.ClientScript.GetPostBackEventReference(lbFilterCheck, "")%>
	}
	//]]>
</script>

<ctrl:TopTab ID="ctrlTopTab" runat="server" BaseUrl="TemplateEdit.aspx" />
<table cellpadding="7" cellspacing="0" border="0" width="100%" class="ibn-WPBorder">
	<tr>
		<td valign="top" style="padding-right: 8px">
			<table class="ibn-stylebox" cellspacing="0" cellpadding="0" width="100%" border="0">
				<tr>
					<td>
						<ibn:BlockHeader ID="secHeader" Title="Edit Template" runat="server"></ibn:BlockHeader>
					</td>
				</tr>
				<tr>
					<td>
						<table runat="server" id="tblName" cellpadding="0" cellspacing="0" width="700px" align="center">
							<tr>
								<td valign="middle" style="padding-top: 10px">
									<ibn:BlockHeaderLight ID="secRepName" runat="server" />
									<table class="ibn-stylebox-light text" cellspacing="0" cellpadding="7" width="100%" border="0">
										<tr>
											<td align="center">
												<asp:TextBox ID="repName" runat="server" Width="70%" CssClass="text"></asp:TextBox>
												<asp:RequiredFieldValidator ID="rfName" runat="server" CssClass="text" Display="Dynamic" ErrorMessage="*" ControlToValidate="repName"></asp:RequiredFieldValidator>
											</td>
										</tr>
									</table>
								</td>
							</tr>
						</table>
						<table runat="server" id="tblFields" cellpadding="0" cellspacing="0" width="700px" align="center">
							<tr>
								<td valign="middle" style="padding-top: 10px">
									<ibn:BlockHeaderLight ID="secRepFields" runat="server" />
									<table class="ibn-stylebox-light text" cellspacing="0" cellpadding="7" width="100%" border="0">
										<tr>
											<td style="padding: 15 0 15 15;">
												<b>
													<%=LocRM.GetString("tReportFields")%>:</b>
											</td>
										</tr>
										<tr>
											<td style="padding: 0 0 15 15;" align="middle" colspan="3">
												<table class="text" id="tblGroups" cellpadding="0" border="0">
													<tr>
														<td valign="top" nowrap width="220px" style="padding-right: 6px; padding-bottom: 6px">
															<asp:Label ID="lblAvailable" runat="server" CssClass="text ibn-label"></asp:Label><br>
															<asp:ListBox ID="lbAvailableFields" runat="server" CssClass="text" Rows="9" Width="215px"></asp:ListBox>
															<asp:CustomValidator ID="FieldValidator" Style="vertical-align: top" runat="server" ClientValidationFunction="FieldExistence" ErrorMessage="*"></asp:CustomValidator>
														</td>
														<td style="padding-right: 6px; padding-left: 6px; padding-bottom: 6px">
															<p align="center">
																<asp:Button ID="btnAddOneGr" Style="margin: 1px" runat="server" CssClass="text" Width="30px" CausesValidation="False" Text=">"></asp:Button><br>
																<asp:Button ID="btnAddAllGr" Style="margin: 1px" runat="server" CssClass="text" Width="30px" CausesValidation="False" Text=">>"></asp:Button><br>
																<br>
																<asp:Button ID="btnRemoveOneGr" Style="margin: 1px" runat="server" CssClass="text" Width="30px" CausesValidation="False" Text="<"></asp:Button><br>
																<asp:Button ID="btnRemoveAllGr" Style="margin: 1px" runat="server" CssClass="text" Width="30px" CausesValidation="False" Text="<<"></asp:Button></p>
														</td>
														<td valign="top" width="220px" style="padding-right: 20px; padding-left: 6px; padding-bottom: 6px">
															<asp:Label ID="lblSelected" runat="server" CssClass="text ibn-label"></asp:Label><br>
															<asp:ListBox ID="lbSelectedFields" runat="server" CssClass="text" Rows="9" Width="215px"></asp:ListBox>
														</td>
													</tr>
												</table>
											</td>
										</tr>
									</table>
								</td>
							</tr>
						</table>
						<table runat="server" id="tblGroupFields" cellpadding="0" cellspacing="0" width="700px" align="center">
							<tr>
								<td valign="middle" style="padding-top: 10px">
									<ibn:BlockHeaderLight ID="secRepGroup" runat="server" />
									<table class="ibn-stylebox-light text" style="padding: 15 0 15 40;" cellspacing="0" cellpadding="7" width="100%" border="0">
										<tr>
											<td width="50%">
												<asp:DropDownList ID="ddGroup1" runat="server" CssClass="text" Width="200px">
												</asp:DropDownList>
												<br>
												<br>
												<asp:DropDownList ID="ddGroup2" runat="server" CssClass="text" Width="200px">
												</asp:DropDownList>
												<asp:CompareValidator ID="compareGroups" runat="server" CssClass="text" ErrorMessage="*" Operator="NotEqual" ControlToValidate="ddGroup2" ControlToCompare="ddGroup1"></asp:CompareValidator>
											</td>
											<td valign="top" align="middle" width="60">
												<img alt="" src="../layouts/images/quicktip.gif" border="0">
											</td>
											<td class="text" style="padding-right: 15px" valign="top">
												<%=LocRM.GetString("s3Comments") %>
											</td>
										</tr>
									</table>
								</td>
							</tr>
						</table>
						<table runat="server" id="tblFilters" cellpadding="0" cellspacing="0" width="700px" align="center">
							<tr>
								<td valign="middle" style="padding-top: 10px">
									<table class="ibn-stylesheet" width="100%">
										<tr>
											<td width="220px" valign="top">
												<ibn:BlockHeaderLight ID="secRepFilterFields" runat="server" />
												<table class="ibn-stylebox-light text" style="height: 320px; margin: 0; padding: 2px" cellspacing="0" cellpadding="0" width="100%" border="0">
													<tr>
														<td>
															<div style="width: 210px; overflow-y: auto; overflow-x: auto; height: 290px; padding-bottom: 20px">
																<asp:DataList runat="server" ItemStyle-CssClass="text" ID="dlFilterFields" CellPadding="0" CellSpacing="0" ShowHeader="False" ShowFooter="False">
																	<ItemStyle CssClass="ibn-propertysheet" Font-Size="10"></ItemStyle>
																	<SelectedItemStyle CssClass="UserCellSelected ibn-menuimagecell" Font-Size="10"></SelectedItemStyle>
																	<ItemTemplate>
																		<nobr>
														<input onclick='CheckField(this);' type="checkbox" class="text" runat="server" id="cbFilter" NAME="cbFilter"
															checked='<%# (bool)DataBinder.Eval(Container.DataItem, "IsChecked") %>'/>
														<asp:linkbutton id="lbField" commandname='<%# DataBinder.Eval(Container.DataItem, "FieldName") %>' runat="server" causesvalidation="False">
															<%# DataBinder.Eval(Container.DataItem, "FriendlyName") %>
														</asp:linkbutton></nobr>
																	</ItemTemplate>
																</asp:DataList>
															</div>
														</td>
													</tr>
												</table>
											</td>
											<td valign="top">
												<ibn:BlockHeaderLight ID="secFilterFieldInfo" runat="server" />
												<table style="height: 170px; margin: 0; padding: 2px; table-layout: fixed;" class="ibn-stylebox-light text" cellspacing="0" cellpadding="0" width="100%" border="0">
													<tr>
														<td class="text" valign="top" align="left" style="padding-top: 15px; padding-left: 15px;">
															<ftc:DateFltr ID="dtFltr" runat="server" Visible="False"></ftc:DateFltr>
															<ftc:DictFltr ID="dictFltr" runat="server" Visible="False"></ftc:DictFltr>
															<ftc:IntFltr ID="intFltr" runat="server" Visible="False"></ftc:IntFltr>
															<ftc:StringFltr ID="strFltr" runat="server" Visible="False"></ftc:StringFltr>
															<ftc:TimeFltr ID="timeFltr" runat="server" Visible="False"></ftc:TimeFltr>
														</td>
													</tr>
												</table>
												<ibn:BlockHeaderLight ID="secFilterValues" runat="server" />
												<table style="height: 139px; margin: 0; padding: 2px; table-layout: fixed;" class="ibn-stylebox-light text" cellspacing="0" cellpadding="0" width="100%" border="0">
													<tr>
														<td>
															<div style="width: 95%; overflow-y: auto; overflow-x: auto; height: 100px; padding-bottom: 10px">
																<table>
																	<tr>
																		<td valign="top" class="text" width="70px" align="right">
																			<%=LocRM.GetString("s6Filter")%>
																			:&nbsp;
																		</td>
																		<td>
																			<asp:Label ID="lblCurrentFilter" runat="server" CssClass="text"></asp:Label>
																		</td>
																	</tr>
																</table>
															</div>
														</td>
													</tr>
												</table>
											</td>
										</tr>
									</table>
								</td>
							</tr>
						</table>
						<table runat="server" id="tblFiltersGenerate" cellpadding="0" cellspacing="0" width="100%">
							<tr>
								<td valign="middle" align="left" style="padding-left: 15px; padding-top: 15px">
									<table id="tblFiltersValues" class="ibn-propertysheet" cellspacing="0" cellpadding="5" border="0" runat="server">
									</table>
								</td>
							</tr>
						</table>
						<table runat="server" id="tblSorts" cellpadding="0" cellspacing="0" width="700px" align="center">
							<tr>
								<td valign="middle" style="padding-top: 10px;">
									<ibn:BlockHeaderLight ID="secSort" runat="server" />
									<table class="ibn-stylebox-light text" style="padding: 15 0 15 10;" cellspacing="0" cellpadding="7" width="100%" border="0">
										<tr height="22px">
											<td class="text" width="220px" height="22px">
												<b>
													<%=LocRM.GetString("tSelected") %>
													:</b>
											</td>
											<td>
												<span class="text"><b>
													<%=LocRM.GetString("tAvailable") %>
													:</b></span>
											</td>
										</tr>
										<tr style="height: 220px">
											<td valign="top" width="300px" height="100%">
												<!-- Data GRID -->
												<div style="overflow-y: auto; height: 220px">
													<asp:DataGrid ID="dgSortFields" runat="server" Width="100%" BorderWidth="0px" CellSpacing="3" GridLines="None" CellPadding="3" AllowSorting="False" AllowPaging="False" AutoGenerateColumns="False">
														<ItemStyle CssClass="text"></ItemStyle>
														<HeaderStyle CssClass="text" Font-Italic="True" Font-Bold="True"></HeaderStyle>
														<Columns>
															<asp:BoundColumn HeaderText='Sort Field' DataField="Field" Visible="False"></asp:BoundColumn>
															<asp:BoundColumn HeaderText='Sort Field' DataField="FieldText"></asp:BoundColumn>
															<asp:TemplateColumn HeaderText='Sort Direct' ItemStyle-Width="50">
																<ItemTemplate>
																	<%# GetSortDirect( (int)DataBinder.Eval(Container.DataItem, "SortDirect"))%>
																</ItemTemplate>
															</asp:TemplateColumn>
															<asp:TemplateColumn ItemStyle-Width="30" Visible="True">
																<ItemTemplate>
																	<asp:ImageButton ID="ibDelete" runat="server" BorderWidth="0" Width="16" Height="16" ImageUrl="../../layouts/images/DELETE.GIF" CommandName="Delete" CausesValidation="False"></asp:ImageButton>
																</ItemTemplate>
															</asp:TemplateColumn>
														</Columns>
													</asp:DataGrid><!-- End Data GRID --></div>
											</td>
											<td valign="top">
												<table class="text" style="margin-top: 5px" cellspacing="0" cellpadding="3" width="100%">
													<tr>
														<td valign="top">
															<b>
																<%=LocRM.GetString("tFields") %>
																:</b>
														</td>
														<td valign="top">
															<asp:ListBox ID="lbFields" runat="server" Width="190px" CssClass="text" Rows="12" SelectionMode="Single"></asp:ListBox>
														</td>
													</tr>
													<tr>
														<td valign="top">
														</td>
														<td valign="top">
															<asp:RadioButtonList ID="rbAscDesc" runat="server" CssClass="text" RepeatDirection="Horizontal">
															</asp:RadioButtonList>
														</td>
													</tr>
													<tr>
														<td valign="top" height="28">
															&nbsp;
														</td>
														<td>
															<asp:Button ID="btnAdd" runat="server" Width="90px" CssClass="text" CausesValidation="False" OnClick="btnAdd_Click"></asp:Button>
														</td>
													</tr>
												</table>
											</td>
										</tr>
									</table>
								</td>
							</tr>
						</table>
						<table runat="server" id="tblButton" cellpadding="0" cellspacing="0" width="700px" align="center">
							<tr height="60px">
								<td style="padding-top: 10px; padding-right: 10px" align="right">
									<btn:IMButton ID="btnSave" runat="server" Class="text" style="width: 110px;" OnServerClick="btnSave_ServerClick">
									</btn:IMButton>
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
<input id="ResultXML" type="hidden" value="" name="ResultXML" runat="server" />
<input id="pastCommand" type="hidden" value="" name="pastCommand" runat="server" />
<input id="changedCheck" type="hidden" value="" name="changedCheck" runat="server" />
<asp:LinkButton ID="lbFilterCheck" runat="server" Visible="False" OnClick="lbFilterCheck_Click"></asp:LinkButton>

<script type="text/javascript">
	//<![CDATA[
	SaveFields();
	//]]>
</script>
