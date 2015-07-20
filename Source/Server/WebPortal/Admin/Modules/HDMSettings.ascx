<%@ Control Language="C#" AutoEventWireup="true" Codebehind="HDMSettings.ascx.cs" Inherits="Mediachase.UI.Web.Admin.Modules.HDMSettings" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="~/Modules/BlockHeader.ascx" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeaderLight" src="~/Modules/BlockHeaderLightWithMenu.ascx" %>
<%@ register TagPrefix="dg" namespace="Mediachase.UI.Web.Modules.DGExtension" Assembly="Mediachase.UI.Web" %>

<script type="text/javascript">
	
	function OpenWindow(query,w,h,scroll)
		{
			var l = (screen.width - w) / 2;
			var t = (screen.height - h) / 2;
			
			winprops = 'resizable=1, height='+h+',width='+w+',top='+t+',left='+l;
			if (scroll) winprops+=',scrollbars=1';
			var f = window.open(query, "_blank", winprops);
		}
		
	function SetActive2(obj)
	{
		if(obj!=null)
		{
			obj.className = "active";
		}
	}
		
		
	function SetInactive2(obj)
	{
		if(obj!=null)
		{
			obj.className = "inactive";
		}
	}
	
	function ShowMenu2(obj, e)
	{
		e = (e) ? e : ((window.event) ? event : null);
		if(obj!=null)
		{
			var md = document.getElementById("menudiv");
				if(md!=null)
				{
					var objTop;
					var objLeft;

					if (e.pageY)
					{
						objTop = e.pageY;
						objLeft = e.pageX;
					}
					else
					{
						objTop = getObjectTop(obj) + e.offsetY;
						objLeft = getObjectLeft(obj) + e.offsetX;
					}

					md.style.top = (objTop).toString() + "px";
					md.style.left = (objLeft - 25).toString() + "px";
					md.style.visibility = "visible";
					md.innerHTML = obj.childNodes[0].tBodies[0].rows[0].cells[0].childNodes[0].innerHTML;
				}		
		}
	}
	
	function ShowStatMenu(id, e)
	{
		var obj = document.getElementById(id);
		if(obj!=null)
		{
			var md = document.getElementById("menudiv");
				if(md!=null)
				{
					var scrollLeft = document.body.scrollLeft;
					if((e.clientX-120+scrollLeft)>0)
						md.style.left = (e.clientX - 120 + scrollLeft).toString() + "px";
					else 
						md.style.left = "0px";
					md.innerHTML = obj.childNodes[0].tBodies[0].rows[0].cells[0].childNodes[1].innerHTML;
				}
		}
	}
	
	function HideMenu2(e)
	{
		var md = document.getElementById("menudiv");
		if(md!=null && md.style.visibility == "visible")
		{
			if(e.clientX<parseInt(md.style.left) || e.clientX>(parseInt(md.style.left)+md.offsetWidth))
				md.style.visibility = "hidden";
		}
	}
	
</script>
<style type="text/css" >
	.active
	{
		background-color:#eeeeee;
		FONT-FAMILY: Verdana, Arial, Helvetica;
		font-size:8pt;
	}
	.inactive
	{
		background-color:white;
		FONT-FAMILY: Verdana, Arial, Helvetica;
		font-size:8pt;
	}
</style>
<div style="visibility:hidden; padding:3px; position:absolute; background-color:#fafafa; border:solid 1px #bbbbbb; z-index:30;" 
	id="menudiv">
</div>
<div onmousemove="HideMenu2(event)">
<table class="ibn-stylebox2" width="100%" cellspacing="0" cellpadding="0" border="0" runat="server" id="tbExternal">
	<tr>
		<td >
			<ibn:blockheader id="bhExternal" runat="server" Title="Email ящики" />
		</td>
	</tr>
	<tr runat="server" id="trExternalOnly">
		<td style="padding:10px;">
			<ibn:BlockHeaderLight runat="server" ID="bhlExtOnly" />
			<div style="overflow-x:hidden; padding-left:5px; padding-right:5px; overflow-y:auto; height:150px;" class="ibn-stylebox-light" >
				<asp:Repeater runat="server" ID="rpExtOnly">
					<ItemTemplate>
						<div id='divEmailBox_<%#Eval("EMailRouterPop3BoxId").ToString()%>' onmouseover="javascript:SetActive2(this)" onmouseout="javascript:SetInactive2(this)" onclick="javascript:ShowMenu2(this, event)" style="float:left; margin:5px"><table border="0" cellpadding="0" cellspacing="0">
								<tr valign="top">
									<td colspan="3" align="right" style="text-align:right; vertical-align:top;" valign="top"><div class="text" style="position:absolute; visibility:hidden; z-index:20; background-color:Aquamarine; width:110px; text-align:left;" >
									<div style="padding-bottom:3px">
									<asp:LinkButton id="ibChangeStatus" runat="server" CommandName="ChangeStatus" CausesValidation="False" 
										Text='<%# GetStatusDG(DataBinder.Eval(Container.DataItem, "IsActive"))%>' CommandArgument='<%#Eval("EMailRouterPop3BoxId")%>'>
									</asp:LinkButton>
									</div>
									<div style="padding-bottom:3px">
									<%#GetMappingButton((int)DataBinder.Eval(Container.DataItem, "EMailRouterPop3BoxId"), LocRM.GetString("tMapping"))%>
									</div>
									<div style="padding-bottom:3px">
									<%#GetEditButton((int)DataBinder.Eval(Container.DataItem, "EMailRouterPop3BoxId"), LocRM.GetString("tEdit"))%>
									</div>
									<div style="padding-bottom:3px">
										<a class="text" href='#' onclick='javascript:ShowStatMenu("divEmailBox_<%#Eval("EMailRouterPop3BoxId").ToString()%>", event)'><img border='0' align='absmiddle' src='<%# ResolveUrl("~/layouts/images/info.gif") %>'  />&nbsp;<%=LocRM.GetString("tStatistics")%></a>
									</div>
									<asp:LinkButton id="ibDelete" runat="server" commandname="Delete" causesvalidation="False" CommandArgument='<%#Eval("EMailRouterPop3BoxId")%>'
										Text='<%#GetDeleteButton()%>'>
									</asp:LinkButton>
									</div><div style="display:none;" class="text">
										<table border="0" cellpadding="5" cellspacing="0" class="text" >
											<tr>
												<td align="right">
													<b>
													<%=LocRM.GetString("tEmailBox")%>:
													</b>
												</td>
												<td align="left">
													<%#Eval("Name")%>
												</td>
											</tr>
											<tr>
												<td align="right">
													<b>
													<%=LocRM.GetString("tIsActive")%>:
													</b>
												</td>
												<td align="left">
													<%#(bool)Eval("IsActive")?LocRM.GetString("tYes"):LocRM.GetString("tNo")%>
												</td>
											</tr>
											<tr>
												<td align="right">
													<b>
													<%=LocRM.GetString("tMessageCount")%>:
													</b>
												</td>
												<td align="left">
													<%#Eval("TotalMessageCount")%>
												</td>
											</tr>
											<tr>
												<td align="right">
													<b>
													<%=LocRM.GetString("tLastReq")%>:
													</b>
												</td>
												<td align="left">
													<%#Eval("LastRequest")%>
												</td>
											</tr>
											<tr>
												<td align="right">
													<b>
													<%=LocRM.GetString("tLastSuccReq")%>:
													</b>
												</td>
												<td align="left">
													<%#Eval("LastSuccessfulRequest")%>
												</td>
											</tr>
											<tr>
												<td colspan="2" align="center">
													<b>
														<font color="red">
															<%#Eval("LastErrorText")%>
														</font>
													</b>
												</td>
											</tr>
											</table>
										</div>
									</td>
								</tr>
								<tr>
									<td>
									&nbsp;
									</td>
									<td align="center"><%#GetMailBoxIcon((int)Eval("EMailRouterPop3BoxId"))%>
									</td>
									<td>
									&nbsp;
									</td>
								</tr>
								<tr>
									<td colspan="3" style='padding-left:3px; padding-right:3px;text-align:center; color:<%#(bool)Eval("IsActive")?"black":"#aaaaaa"%>' class="text" >
										<%#Eval("Name")%>
									</td>
								</tr>
							</table>
						</div>
					</ItemTemplate>
				</asp:Repeater>
			</div>
		</td>
	</tr>
	<tr runat="server" id="trExternalInternal">
		<td style="padding-left:10px; padding-right:10px; padding-bottom:10px;">
			<table border="0" cellpadding="0" cellspacing="0" width="100%">
				<tr>
					<td>
						<ibn:BlockHeaderLight runat="server" ID="bhlExtInt" />
						<div style="overflow-x:hidden; padding-left:5px; padding-right:5px; overflow-y:auto; height:150px;" class="ibn-stylebox-light" >
							<asp:Repeater runat="server" ID="rpExtInt">
								<ItemTemplate>
									<div id='divEmailBox_<%#Eval("EMailRouterPop3BoxId").ToString()%>' onmouseover="javascript:SetActive2(this)" onmouseout="javascript:SetInactive2(this)" onclick="javascript:ShowMenu2(this, event)" style="float:left; margin:5px;"><table border="0" cellpadding="0" cellspacing="0">
											<tr  valign="top">
												<td colspan="3"  align="right" style="text-align:right; vertical-align:top;" valign="top"><div class="text" style="position:absolute; visibility:hidden; z-index:20; background-color:Aquamarine; width:110px; text-align:left;" >
												<div style="padding-bottom:3px">
												<asp:LinkButton id="ibChangeStatus" runat="server" CommandName="ChangeStatus" CausesValidation="False" 
													Text='<%# GetStatusDG(DataBinder.Eval(Container.DataItem, "IsActive"))%>' CommandArgument='<%#Eval("EMailRouterPop3BoxId")%>'>
												</asp:LinkButton>
												</div>
												<div style="padding-bottom:3px">
												<%#GetMappingButton((int)DataBinder.Eval(Container.DataItem, "EMailRouterPop3BoxId"), LocRM.GetString("tMapping"))%>
												</div>
												<div style="padding-bottom:3px">
												<%#GetEditButton((int)DataBinder.Eval(Container.DataItem, "EMailRouterPop3BoxId"), LocRM.GetString("tEdit"))%>
												</div>
												<div style="padding-bottom:3px">
													<a onclick='javascript:ShowStatMenu("divEmailBox_<%#Eval("EMailRouterPop3BoxId").ToString()%>", event)' class="text" href='#'><img border='0' align='absmiddle' src='<%# ResolveUrl("~/layouts/images/info.gif") %>'  />&nbsp;<%=LocRM.GetString("tStatistics")%></a>
												</div>
												<asp:LinkButton id="ibDelete" runat="server" commandname="Delete" causesvalidation="False" CommandArgument='<%#Eval("EMailRouterPop3BoxId")%>'
													Text='<%#GetDeleteButton()%>'>
												</asp:LinkButton>
												</div><div style="display:none;" class="text">
												<table border="0" cellpadding="5" cellspacing="0" class="text" >
													<tr>
														<td align="right">
															<b>
															<%=LocRM.GetString("tEmailBox")%>:
															</b>
														</td>
														<td align="left">
															<%#Eval("Name")%>
														</td>
													</tr>
													<tr>
														<td align="right">
															<b>
															<%=LocRM.GetString("tIsActive")%>:
															</b>
														</td>
														<td align="left">
															<%#(bool)Eval("IsActive")?LocRM.GetString("tYes"):LocRM.GetString("tNo")%>
														</td>
													</tr>
													<tr>
														<td align="right">
															<b>
															<%=LocRM.GetString("tMessageCount")%>:
															</b>
														</td>
														<td align="left">
															<%#Eval("TotalMessageCount")%>
														</td>
													</tr>
													<tr>
														<td align="right">
															<b>
															<%=LocRM.GetString("tLastReq")%>:
															</b>
														</td>
														<td align="left">
															<%#Eval("LastRequest")%>
														</td>
													</tr>
													<tr>
														<td align="right">
															<b>
															<%=LocRM.GetString("tLastSuccReq")%>:
															</b>
														</td>
														<td align="left">
															<%#Eval("LastSuccessfulRequest")%>
														</td>
													</tr>
													<tr>
														<td colspan="2" align="center">
															<b>
																<font color="red">
																	<%#Eval("LastErrorText")%>
																</font>
															</b>
														</td>
													</tr>
													</table>
												</div>
												</td>
											</tr>
											<tr>
												<td>
												&nbsp;
												</td>
												<td align="center"><%#GetMailBoxIcon((int)Eval("EMailRouterPop3BoxId"))%>
												</td>
												<td>
												&nbsp;
												</td>
											</tr>
											<tr>
												<td colspan="3" style='padding-left:3px; padding-right:3px;text-align:center; color:<%#(bool)Eval("IsActive")?"black":"#aaaaaa"%>' class="text" >
													<%#Eval("Name")%>
												</td>
											</tr>
										</table>
									</div>
								</ItemTemplate>
							</asp:Repeater>
						</div>
					</td>
					<td style="width:200px; padding-left:10px;" >
						<ibn:BlockHeaderLight runat="server" ID="bhlIntExt" />
						<table border="0" cellpadding="0" cellspacing="0" class="ibn-stylebox-light" width="200px;">
							<tr>
								<td>
									<div class="text" style="height:150px; text-align:center;">
										<table cellpadding="0" cellspacing="0" border="0" align="center"><tr><td>
										<div style="width:70px;" onmouseover="javascript:SetActive2(this)" onmouseout="javascript:SetInactive2(this)" onclick="javascript:ShowMenu2(this, event)" id='divIntEmailBox'><table align="center" border="0" cellpadding="0" cellspacing="0">
											<tr  valign="top">
												<td colspan="3"  align="right" style="text-align:right; vertical-align:top;" valign="top"><div class="text" style="position:absolute; visibility:hidden; z-index:20; width:110px; text-align:left;" >
													<div style="padding-bottom:3px">
													<asp:LinkButton id="lbChangeStatusInternal" runat="server" CausesValidation="False" Text="">
													</asp:LinkButton>
													</div>
													<div style="padding-bottom:3px">
														<asp:Label runat="server" ID="lbIntEdit" Text=""></asp:Label>
													</div>
													<div style="padding-bottom:3px">
														<asp:Label runat="server" ID="lbIntStat" Text="" style="cursor:pointer;"></asp:Label>
													</div>
													<asp:LinkButton id="lbDeleteInternalBox" runat="server" causesvalidation="False" Text='<%#GetDeleteButton()%>'>
													</asp:LinkButton>
													</div><div class="text" style="display:none;">
													<table border="0" cellpadding="5" cellspacing="0" class="text" >
													<tr>
														<td align="right">
															<b>
															<%=LocRM.GetString("tEmailBox")%>:
															</b>
														</td>
														<td align="left">
															<asp:Label runat="server" ID="lbIntName" CssClass="text"></asp:Label>
														</td>
													</tr>
													<tr>
														<td align="right">
															<b>
															<%=LocRM.GetString("tIsActive")%>:
															</b>
														</td>
														<td align="left">
															<asp:Label runat="server" ID="lbIntIsActive" CssClass="text"></asp:Label>
														</td>
													</tr>
													<tr>
														<td align="right">
															<b>
															<%=LocRM.GetString("tMessageCount")%>:
															</b>
														</td>
														<td align="left">
															<asp:Label runat="server" ID="lbIntMessageCount" CssClass="text"></asp:Label>
														</td>
													</tr>
													<tr>
														<td align="right">
															<b>
															<%=LocRM.GetString("tLastReq")%>:
															</b>
														</td>
														<td align="left">
															<asp:Label runat="server" ID="lbIntLastReq" CssClass="text"></asp:Label>
														</td>
													</tr>
													<tr>
														<td align="right">
															<b>
															<%=LocRM.GetString("tLastSuccReq")%>:
															</b>
														</td>
														<td align="left">
															<asp:Label runat="server" ID="lbIntLastSuccReq" CssClass="text"></asp:Label>
														</td>
													</tr>
													<tr>
														<td colspan="2" align="center">
															<b>
																<font color="red">
																	<%=InternalBoxProblem%>
																</font>
															</b>
														</td>
													</tr>
													</table>
													</div>
												</td>
											</tr>
											<tr>
												<td>
												&nbsp;
												</td>
												<td align="center">
												<%=GetMailBoxIcon(InternalBoxId)%>
												</td>
												<td>
												&nbsp;
												</td>
											</tr>
											<tr>
												<td colspan="3" style='padding-left:3px; padding-right:3px;text-align:center;' class="text" >
													<asp:Label runat="server" ID="lbIntBoxName" Text=""></asp:Label>
												</td>
											</tr>
										</table>
										</div>
										</td></tr></table>
									</div>
								</td>
							</tr>
						</table>	
					</td>
				</tr>
			</table>
		</td>
	</tr>
	<tr runat="server" id="trNoInternal">
		<td style="padding-left:10px; padding-right:10px; padding-bottom:10px;">
			<ibn:BlockHeaderLight runat="server" ID="bhlNoInternal"  />
			<div class="ibn-stylebox-light">
			<table border="0" cellpadding="0" cellspacing="0">
				<tr>
				<td style="padding:5px; background-color:#ffffe1;" class="text">
				<blockquote style="border-left:solid 2px #CE3431; padding-left:10px; margin:5px; margin-left:15px; padding-top:3px; padding-bottom:3px">
					<%=LocRM.GetString("tNoIntBoxMessage")%>
					<br />
					<div style="white-space:nowrap; text-align:center; padding-top:5px;">
						<asp:HyperLink ID="hlNoInternal" runat="server" Text="Создать внутренний почтовый ящик" ForeColor="red" Font-Underline="true" Font-Bold="true"></asp:HyperLink>
					</div>
				</blockquote>
				</td>
				</tr>
			</table>
			</div>
		</td>
	</tr>
	<tr runat="server" id="trSmtpNotChecked">
		<td style="padding-left:10px; padding-right:10px; padding-bottom:10px;">
			<ibn:BlockHeaderLight runat="server" ID="bhlNoSmtp" />
			<div  class="ibn-stylebox-light text" style="vertical-align:middle; padding-top:10px; padding-bottom:10px;  background-color:#ffffe1;">
				<blockquote style="border-left:solid 2px #CE3431; padding-left:10px; margin:5px; margin-left:15px; padding-top:3px; padding-bottom:3px">
				<%=LocRM.GetString("tSMTPNotChecked")%>
				<br />
				<div style="text-align:center; padding-top:3px">
				<asp:HyperLink runat="server" Text="" Font-Bold="true" ForeColor="red" Font-Underline="true" Id="hlNoSmtp"></asp:HyperLink>
				</div>
				</blockquote>
			</div>
		</td>
	</tr>
</table>
<table class="ibn-stylebox2" width="100%" cellspacing="0" cellpadding="0" border="0" runat="server" id="tbNoBoxes">
	<tr>
		<td >
			<ibn:blockheader id="secHeader1" runat="server" Title="Email ящики" />
		</td>
	</tr>
	<tr class="text">
		<td style="padding:5px; background-color:#ffffe1;">
		<blockquote style="border-left:solid 2px #CE3431; padding-left:10px; margin:5px; margin-left:15px; padding-top:3px; padding-bottom:3px">
		<%=LocRM.GetString("tNoExtBoxesMessage")%> <br />
		<div style="white-space:nowrap; text-align:center; padding-top:5px;">
			<asp:HyperLink runat="Server" Font-Bold="true" Font-Underline="true" ForeColor="Red" ID="hlCreateNewEmailBox"></asp:HyperLink>
		</div>
		</blockquote>
		</td>
	</tr>
</table>	
<br />
<table class="ibn-stylebox2" width="100%" cellspacing="0" cellpadding="0" border="0" runat="server" id="tbNoAntiSpam">
	<tr>
		<td >
			<ibn:blockheader id="bhNoAntiSpam" runat="server" Title="Анти-спам" />
		</td>
	</tr>
	<tr class="text">
		<td style="padding:5px; background-color:#ffffe1;" class="text">
			<blockquote style="border-left:solid 2px #CE3431; padding-left:10px; margin:5px; margin-left:15px; padding-top:3px; padding-bottom:3px">
			<%=LocRM.GetString("tNoAntiSpamMessage")%>
			<br />
			<div style="white-space:nowrap; text-align:center; padding-top:5px;">
			<asp:LinkButton ID="lbAntiSpam" runat="server" Text="Включить анти-спам" CssClass="text" Font-Bold="true" Font-Underline="true" ForeColor="red" />
			</div>
			</blockquote>
		</td>
	</tr>
</table>	
<table class="ibn-stylebox2" width="100%" cellspacing="0" cellpadding="0" border="0" runat="server" id="tbAntiSpam">
	<tr>
		<td >
			<ibn:blockheader id="bhAntiSpam" runat="server" Title="Анти-спам" />
		</td>
	</tr>
	<tr class="text">
		<td>
			<asp:DataGrid id="dgRules" runat="server" allowpaging="False" 
				allowsorting="False" cellpadding="0" gridlines="None" 
				CellSpacing="0" borderwidth="0px" autogeneratecolumns="False" width="100%">
				<columns>
					<asp:boundcolumn DataField="Id" visible="false"></asp:boundcolumn>
					<asp:boundcolumn HeaderStyle-Width="50px" DataField="Weight" ItemStyle-CssClass="ibn-vb2" HeaderStyle-CssClass="ibn-vh2"></asp:boundcolumn>
					<asp:templatecolumn>
						<headerstyle CssClass="ibn-vh2" width="60px"></headerstyle>
						<itemstyle CssClass="ibn-vb2" width="60px"></itemstyle>
						<itemtemplate>
							<%# GetIcon((bool)DataBinder.Eval(Container.DataItem, "IsAccept"))%>
						</itemtemplate>
					</asp:templatecolumn>
					<asp:templatecolumn>
						<headerstyle CssClass="ibn-vh2"></headerstyle>
						<itemstyle CssClass="ibn-vb2"></itemstyle>
						<itemtemplate>
							<%# GetLink_1((int)DataBinder.Eval(Container.DataItem, "Id"), DataBinder.Eval(Container.DataItem, "Key").ToString())%>
						</itemtemplate>
					</asp:templatecolumn>
					<asp:templatecolumn>
						<headerstyle CssClass="ibn-vh2"></headerstyle>
						<itemstyle CssClass="ibn-vb2"></itemstyle>
						<itemtemplate>
							<%# DataBinder.Eval(Container.DataItem, "Type")%>
						</itemtemplate>
					</asp:templatecolumn>
					<asp:templatecolumn>
						<headerstyle CssClass="ibn-vh2"></headerstyle>
						<itemstyle CssClass="ibn-vb2"></itemstyle>
						<itemtemplate>
							<%# DataBinder.Eval(Container.DataItem, "Value")%>
						</itemtemplate>
					</asp:templatecolumn>
					<asp:templatecolumn>
						<headerstyle CssClass="ibn-vh-right" Width="52px"></headerstyle>
						<itemstyle CssClass="ibn-vb2" Width="52px"></itemstyle>
						<itemtemplate>
							<%#GetEditButton_1((int)DataBinder.Eval(Container.DataItem, "Id"), LocRM.GetString("tEdit"))%>&nbsp;
							<asp:ImageButton ImageAlign="AbsMiddle" ID="ibDelete" title="Delete" Runat=server CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Id")%>' CommandName="Delete" ImageUrl="~/layouts/Images/Delete.gif"></asp:ImageButton>
						</itemtemplate>
					</asp:templatecolumn>
				</columns>
			</asp:DataGrid>
		</td>
	</tr>
</table>	
<br />

<table class="ibn-stylebox2" width="100%" cellspacing="0" cellpadding="0" border="0" runat="server" id="tbIssBoxes">
	<tr>
		<td >
			<ibn:blockheader id="bhIssueBoxes" runat="server" Title="" />
		</td>
	</tr>
	<tr class="text">
		<td style="padding-bottom:10px;" >
			<div style="overflow-x:hidden; padding:10px; overflow-y:auto;" id="divissboxes">
				<asp:Repeater runat="server" ID="rpIssBoxes">
					<ItemTemplate>
						<div onmouseover="javascript:SetActive2(this);" onmouseout="javascript:SetInactive2(this);" onclick="javascript:ShowMenu2(this, event)" style="float:left; margin:5px;"><table border="0" cellpadding="0" cellspacing="0"  >
								<tr>
									<td colspan="3" align="right" style="text-align:right; vertical-align:top;" valign="top"><div class="text" style="position:absolute; visibility:hidden; height:0px;" >
									<div style="padding-bottom:3px">
									<%#GetEditButton((int)Eval("IncidentBoxId"))%>
									</div>
									<div style="padding-bottom:3px">
									<%#GetRuleButton((int)DataBinder.Eval(Container.DataItem, "IncidentBoxId"))%>
									</div>
									<asp:LinkButton id="ibDelete" runat="server" commandname="Delete" causesvalidation="False" CommandArgument='<%#Eval("IncidentBoxId")%>'
										Text='<%#GetDeleteButton()%>' style='<%#(bool)Mediachase.IBN.Business.EMail.IncidentBox.CanDelete((int)Eval("IncidentBoxId"))?"display:block":"display:none"%>'>
									</asp:LinkButton>
									</div>
									</td>
								</tr>
								<tr>
									<td>
									&nbsp;
									</td>
									<td valign="top" align="center">
										<table border="0" cellpadding="0" cellspacing="0">
										<tr>
											<td>
											<img border="0" src='<%#(bool)Eval("IsDefault")? ResolveUrl("~/layouts/images/incidentbox_default.gif"): ResolveUrl("~/layouts/images/incidentbox_normal.gif") %>'/>
											</td>
											<td>
												<%#Eval("Rules") %><br />
												<%#Eval("Routing") %>
											</td>
										</tr>
										</table>
									</td>
									<td>
									&nbsp;
									</td>
								</tr>
								<tr>
									<td colspan="3" style="text-align:center; padding-left:3px; padding-right:3px;" class="text">
										<%#Eval("Name")%>
									</td>
								</tr>
							</table>
						</div>
					</ItemTemplate>
				</asp:Repeater>
			</div>
		</td>
	</tr>
</table>	
</div>