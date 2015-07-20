<%@ Reference Control="~/Modules/BlockHeaderLightWithMenu.ascx" %>
<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Admin.Modules.EMailPop3Boxes" Codebehind="EMailPop3Boxes.ascx.cs" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="~/Modules/BlockHeader.ascx" %>
<%@ Register TagPrefix="ibn" TagName="BHLight" src="~/Modules/BlockHeaderLightWithMenu.ascx" %>
<%@ Register TagPrefix="btn" namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>
<script type="text/javascript">
	function OpenWindow(query,w,h,scroll)
		{
			var l = (screen.width - w) / 2;
			var t = (screen.height - h) / 2;
			
			winprops = 'resizable=1, height='+h+',width='+w+',top='+t+',left='+l;
			if (scroll) winprops+=',scrollbars=1';
			var f = window.open(query, "_blank", winprops);
		}
		
	function CheckLogStatus()
	{
		var cb = document.getElementById("<%=cbLogActive.ClientID%>");
		if(cb!=null)
		{
			var tb = document.getElementById("<%=tbLogPeriod.ClientID%>");
			if(tb!=null)
			{
				tb.disabled = !cb.checked;
			}
		}
	}	
</script>
<table class="ibn-stylebox2" cellspacing="0" cellpadding="0" width="100%" border="0">
	<tr>
		<td colspan="2"><ibn:blockheader id="secHeader" runat="server" /></td>
	</tr>
	<tr>
		<td style="padding: 5px" colspan="2">
			<table cellspacing="0" cellpadding="0" border="0" width="100%">
				<tr>
					<td>
						<ibn:BHLight id="secExternal" runat="server" />
					</td>
				</tr>
				<tr>
					<td>
						<table class="ibn-stylebox-light"  cellspacing="0" cellpadding="0" border="0" width="100%" 
							<tr>
								<td>
									<div style="padding:7px;margin:5px;background-color:#FFE0E0;" runat="server" id="divError" class="text">
										<%=LocRM.GetString("tMappingWarning")%>
									</div>
									<asp:DataGrid Runat="server" ID="dgBoxes" AutoGenerateColumns="False" 
										AllowPaging="False" AllowSorting="False" cellpadding="5" 
										gridlines="None" CellSpacing="0" borderwidth="0px" Width="100%" 
										ShowHeader="True">
										<Columns>
											<asp:BoundColumn DataField="EMailRouterPop3BoxId" Visible="False"></asp:BoundColumn>
											<asp:TemplateColumn>
												<ItemStyle CssClass="ibn-vb2"></ItemStyle>
												<HeaderStyle CssClass="ibn-vh2" />
												<ItemTemplate>
													<%# GetBoxLink((int)DataBinder.Eval(Container.DataItem, "EMailRouterPop3BoxId"), DataBinder.Eval(Container.DataItem, "Name").ToString())%>
												</ItemTemplate>
											</asp:TemplateColumn>
											<asp:TemplateColumn>
												<ItemStyle CssClass="ibn-vb2" Width="150px"></ItemStyle>
												<HeaderStyle CssClass="ibn-vh2" Width="150px"/>
												<ItemTemplate>
													<%# GetStatus(DataBinder.Eval(Container.DataItem, "IsActive"),DataBinder.Eval(Container.DataItem, "LastRequest"),DataBinder.Eval(Container.DataItem, "LastSuccessfulRequest"),DataBinder.Eval(Container.DataItem, "LastErrorText")) %>
													<asp:LinkButton id="ibChangeStatus" runat="server" CommandName="ChangeStatus" CausesValidation="False" 
														Text='<%# GetStatusDG(DataBinder.Eval(Container.DataItem, "IsActive"))%>'>
													</asp:LinkButton>
												</ItemTemplate>
											</asp:TemplateColumn>
											<asp:TemplateColumn>
												<ItemStyle CssClass="ibn-vb2" Width="90px"></ItemStyle>
												<HeaderStyle CssClass="ibn-vh2" Width="90px"/>
												<ItemTemplate>
													<%#DataBinder.Eval(Container.DataItem, "TotalMessageCount")%>
												</ItemTemplate>
											</asp:TemplateColumn>
											<asp:TemplateColumn sortExpression="LastRequest">
												<ItemStyle CssClass="ibn-vb2" ></ItemStyle>
												<HeaderStyle CssClass="ibn-vh2"  />
												<ItemTemplate>
													<%# ((DateTime)DataBinder.Eval(Container.DataItem, "LastRequest")==DateTime.MinValue) ? "&nbsp;" :
													((DateTime)DataBinder.Eval(Container.DataItem, "LastRequest")).ToString("g") %>
												</ItemTemplate>
											</asp:TemplateColumn>
											<asp:TemplateColumn sortExpression="LastSuccessfulRequest">
												<ItemStyle CssClass="ibn-vb2" ></ItemStyle>
												<HeaderStyle CssClass="ibn-vh2" />
												<ItemTemplate>
													<%# ((DateTime)DataBinder.Eval(Container.DataItem, "LastSuccessfulRequest")==DateTime.MinValue) ? "&nbsp;" :
													((DateTime)DataBinder.Eval(Container.DataItem, "LastSuccessfulRequest")).ToString("g") %>
												</ItemTemplate>
											</asp:TemplateColumn>
											<asp:TemplateColumn>
												<ItemStyle CssClass="ibn-vb2" Width="75px"></ItemStyle>
												<HeaderStyle CssClass="ibn-vh-right" Width="75px" />
												<ItemTemplate>
													<%#GetMappingButton((int)DataBinder.Eval(Container.DataItem, "EMailRouterPop3BoxId"), LocRM.GetString("tMapping"))%>&nbsp;
													<%#GetEditButton((int)DataBinder.Eval(Container.DataItem, "EMailRouterPop3BoxId"), LocRM.GetString("tEdit"))%>&nbsp;
													<asp:imagebutton ImageAlign="AbsMiddle" id="ibDelete" runat="server" borderwidth="0" imageurl="~/layouts/images/delete.gif" commandname="Delete" causesvalidation="False">
													</asp:imagebutton>
												</ItemTemplate>
											</asp:TemplateColumn>
										</Columns>
									</asp:DataGrid>
								</td>
							</tr>
						</table>
					</td>
				</tr>
			</table>
		</td>
	</tr>
	<tr>
		<td style="padding: 5px" valign="top" width="50%">
			<table cellspacing="0" cellpadding="0" border="0" width="100%">
				<tr>
					<td >
						<ibn:BHLight id="secInternal" runat="server" />
					</td>
				</tr>
				<tr>
					<td >
						<table class="ibn-stylebox-light"  cellspacing="0" cellpadding="0" border="0" width="100%" >
							<tr>
								<td>
									<table id="intBox" runat="server"  cellpadding="7" cellspacing="0" width="100%" border="0" class="text">
										<tr>
											<td><b><%=LocRM.GetString("tStatus")%>:</b></td>
											<td>
												<asp:Label ID="lblIntStatus" Runat="server"></asp:Label>
												<asp:LinkButton ID="lbIntChangeStatus" Runat="server"></asp:LinkButton>
											</td>	
										</tr>
										<tr>
											<td><b><%=LocRM.GetString("tName")%>:</b></td>
											<td><asp:Label ID="lblIntName" Runat="server"></asp:Label></td>	
										</tr>
										<tr>
											<td><b><%=LocRM.GetString("tServer")%>:</b></td>
											<td><asp:Label ID="lblIntServer" Runat="server"></asp:Label></td>	
										</tr>
										<tr>
											<td><b><%=LocRM.GetString("tPort")%>:</b></td>
											<td><asp:Label ID="lblIntPort" Runat="server"></asp:Label></td>	
										</tr>
										<tr>
											<td><b><%=LocRM.GetString("tInternalEmail")%>:</b></td>
											<td><asp:Label ID="lblIntAddress" Runat="server"></asp:Label></td>	
										</tr>
										<tr>
											<td><b><%=LocRM.GetString("tLastReq")%>:</b></td>
											<td><asp:Label ID="lblLastReq" Runat="server"></asp:Label></td>	
										</tr>
										<tr>
											<td><b><%=LocRM.GetString("tLastSuccReq")%>:</b></td>
											<td><asp:Label ID="lblLastSuccReq" Runat="server"></asp:Label></td>	
										</tr>
										<tr>
											<td><b><%=LocRM.GetString("tMessageCount")%>:</b></td>
											<td><asp:Label ID="lbMessageCount" Runat="server"></asp:Label></td>	
										</tr>
									</table>
									<div style="margin:10px" id="divNoInt" runat="server">
									<asp:Label ID="lblNoIntBox" Runat="server" CssClass="text"></asp:Label>
									</div>
								</td>
							</tr>
						</table>
					</td>
				</tr>
			</table>
		</td>
		<td style="padding: 5px" valign="top">
				<table cellspacing="0" cellpadding="0" border="0" width="100%">
					<tr>
						<td>
							<ibn:BHLight id="secLog" runat="server" />
						</td>
					</tr>
					<tr>
						<td style="padding-top:0px">
							<table class="ibn-stylebox-light text"  cellspacing="3" cellpadding="0" border="0" width="100%" >
								<tr>
									<td style="padding-left:15px">
										<asp:CheckBox Runat="server" ID="cbLogActive" onclick="javascript:CheckLogStatus()"></asp:CheckBox>&nbsp;
									</td>
								</tr>
								<tr>
									<td style="padding-left:40px">
										<%=LocRM.GetString("tEmailLogPeriodHint1")%>&nbsp;
										<asp:TextBox Runat="server" ID="tbLogPeriod" Width="30px" CssClass="text"></asp:TextBox>
										&nbsp;
										<%=LocRM.GetString("tEmailLogPeriodHint2")%>
									</td>
								</tr>
								<tr>
									<td align="right" style="padding-right:20px">
										<btn:ImButton ID="btnUpdateLogSettings" Runat="server" Class="text" style="width:110px;"></btn:ImButton>
									</td>
								</tr>
							</table>
						</td>
					</tr>	
				</table>
			</td>
	</tr>
</table>