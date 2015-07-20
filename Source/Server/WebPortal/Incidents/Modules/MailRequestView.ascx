<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Incidents.Modules.MailRequestView" Codebehind="MailRequestView.ascx.cs" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="..\..\Modules\BlockHeader.ascx" %>
<%@ Register TagPrefix="btn" namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>
<table class="ibn-stylebox" style="MARGIN-TOP: 1px" cellspacing="0" cellpadding="0" width="100%" border="0">
	<tr>
		<td><ibn:blockheader id="secHeader" runat="server"></ibn:blockheader></td>
	</tr>
	<tr>
		<td style="padding:7">
			<table class="ibn-propertysheet" cellspacing="0" cellpadding="5" width="100%" border="0">
				<tr>
					<td width="100px"><b><%=LocRM.GetString("Subject")%>:</b>
						<asp:RequiredFieldValidator id="rfSubject" runat="server" ErrorMessage="*" ControlToValidate="txtSubject" CssClass="text" />
					</td>
					<td>
						<asp:textbox id="txtSubject" Width="350" runat="server" CssClass="text"></asp:textbox>
					</td>
					<td width="120px"><b><%=LocRM.GetString("Sender")%>:</b></td>
					<td>
						<asp:label id="lblSender" runat="server" CssClass="text"></asp:label>
					</td>
				</tr>
				<tr>
					<td><b><%=LocRM.GetString("Project")%>:</b></td>
					<td>
						<asp:dropdownlist id=ddlProjects runat="server" Width="350px"></asp:dropdownlist>
					</td>
					<td><b><%=LocRM.GetString("Received")%>:</b></td>
					<td><asp:label id="lblReceived" runat="server" CssClass="text"></asp:label></td>
				</tr>
				<tr>
					<td><b><%=LocRM.GetString("Priority")%>:</b></td>
					<td><asp:dropdownlist id="ddlPriority" runat="server" Width="200px"></asp:dropdownlist></td>
					<td<b>><%=LocRM.GetString("MailBox")%>:</b></td>
					<td class="ibn-value">
						<asp:HyperLink Runat="server" ID="lnkMailBox"></asp:HyperLink>
					</td>
				</tr>
				<tr>
					<td valign="top" ><b><%=LocRM.GetString("Description")%>:</b></td>
					<td colspan="3">
						<asp:textbox id="txtDescription" Width="100%" Rows="8" TextMode="MultiLine" runat="server" CssClass="text"></asp:textbox>
					</td>
				</tr>
				<tr runat="server" id="trMHT">
					<td valign="top"><b><%=LocRM.GetString("EmailBody")%>:</b></td>
					<td colSpan="3"><IFRAME id="iMHTFrame" marginWidth=0 style="border:1px solid #CCCCCC" 
            marginHeight=0 src="<%=iSrc %>" frameBorder=0 width="100%" onreadystatechange = "Check()" 
            height=150></IFRAME>
					</td>
				</tr>
			</table>
		</td>
	</tr>
</table>
<table cellspacing="0" cellpadding="0" width="100%" border="0">
	<tr>
		<td valign="top" width="49%">
			<table cellspacing="0" cellpadding="0" border="0" width="100%" class="ibn-stylebox" style="MARGIN-TOP:10px">
				<tr>
					<td>
						<ibn:blockheader id="secTracking" runat="server" />
					</td>
				</tr>
				<tr>
					<td>
						<table cellpadding="3" cellspacing="0" border="0" width="100%" class="text">
							<tr>
								<td width="80" align="middle" rowspan="2">
									<img src="../Layouts/Images/check.gif" alt="" width="60" height="98" border="0">
								</td>
								<td>
									<asp:Label id="lblText" Runat="server" CssClass="text"></asp:Label>
								</td>
							</tr>
							<tr>
								<td align="right" valign="bottom" style="PADDING-RIGHT:10px; PADDING-LEFT:10px; PADDING-BOTTOM:10px; PADDING-TOP:10px">
									<btn:ImButton ID="btnApproveCrete" Runat="server" Class="text" style="WIDTH:150px" Text='<%# LocRM.GetString("Accept") %>'>
									</btn:ImButton>&nbsp;&nbsp;
									<btn:ImButton ID="btnApprove" Runat="server" Class="text" style="width:110px;" Text='<%# LocRM.GetString("Accept") %>'>
									</btn:ImButton>&nbsp;&nbsp;
									<btn:ImButton ID="btnDelete" Runat="server" Class="text" style="width:110px;" Text='<%# LocRM.GetString("Decline") %>' IsDecline=true>
									</btn:ImButton>
								</td>
							</tr>
						</table>
					</td>
				</tr>
			</table>

		</td>
		<td width="10">&nbsp;</td>
		<td valign="top">
			<table class="ibn-stylebox" cellspacing="0" cellpadding="0" width="100%" border="0">
				<tr>
					<td><ibn:blockheader id="secFiles" runat="server"></ibn:blockheader></td>
				</tr>
				<tr>
					<td>
						<asp:DataGrid id="dgFiles" runat="server" width="100%" ShowHeader="True" AutoGenerateColumns="False" cellpadding="3" gridlines="None" CellSpacing="3" borderwidth="0px">
							<ItemStyle CssClass="ibn-propertysheet"></ItemStyle>
							<HeaderStyle CssClass="ibn-vh2"></HeaderStyle>
							<Columns>
								<asp:templatecolumn itemstyle-width="16">
									<itemtemplate>
										<img src='<%# DataBinder.Eval(Container.DataItem, "Icon")%>' width="16" height="16">
									</itemtemplate>
								</asp:templatecolumn>
								<asp:BoundColumn DataField="FileName" HeaderText="Title"></asp:BoundColumn>
								<asp:BoundColumn DataField="Size" itemstyle-width="60"></asp:BoundColumn>
							</Columns>
						</asp:DataGrid>
					</td>
				</tr>
			</table>
		</td>
	</tr>
</table>
<script language="javascript">
	function Check()
	{
		var rs = "";
		try
		{
			rs = iMHTFrame.document.readyState;
		}
		catch (e) {}
		if(rs == "complete")
		{
			var links = iMHTFrame.document.getElementsByTagName("A");
			for(var i=0; i<links.length;i++)
			{
				var obj = links[i];
				obj.target="_blank";
			}
		}
	}
</script>