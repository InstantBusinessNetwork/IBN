<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Admin.Modules.SearchSettings"
	CodeBehind="SearchSettings.ascx.cs" %>
<%@ Reference Control="~/Modules/BlockHeaderLightWithMenu.ascx" %>
<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Reference Control="~/Modules/FormHelper.ascx" %>
<%@ Register TagPrefix="ibn" TagName="FormHelper" Src="~/Modules/FormHelper.ascx" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" Src="~/Modules/BlockHeader.ascx" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeaderLight" Src="~/Modules/BlockHeaderLightWithMenu.ascx" %>
<table border="0" cellpadding="0" cellspacing="0" width="100%" class="ibn-stylebox2">
	<tr>
		<td>
			<ibn:BlockHeader ID="secHeader2" runat="server" Title="" />
		</td>
	</tr>
	<tr>
		<td style="padding: 5px;">
			<table border="0" cellpadding="5" cellspacing="0" width="100%">
				<tr>
					<td width="50%" valign="top">
						<ibn:BlockHeaderLight ID="bhlFTS" runat="server" />
						<table class="ibn-stylebox-light text" cellspacing="0" cellpadding="7" width="100%"
							border="0">
							<tr runat="server" id="trFTSNotInstalled">
								<td style="background-color: #ffffe1">
									<blockquote style="border-left: solid 2px #CE3431; padding-left: 10px; margin: 5px;
										margin-left: 15px; padding-top: 3px; padding-bottom: 3px">
										<%=LocRM.GetString("tFTSNotInstalledMessage")%>
									</blockquote>
								</td>
							</tr>
							<tr runat="server" id="trFTSNotEnabled">
								<td style="background-color: #ffffe1">
									<blockquote style="border-left: solid 2px #CE3431; padding-left: 10px; margin: 5px;
										margin-left: 15px; padding-top: 3px; padding-bottom: 3px">
										<%=LocRM.GetString("tFTSNotEnabledMessage")%>
										<div class="text" align="center" style="padding: 10px;">
											<asp:LinkButton runat="server" ID="lbRunFTS" Text="" Font-Bold="true" Font-Underline="true"
												ForeColor="Red"></asp:LinkButton>
										</div>
									</blockquote>
								</td>
							</tr>
							<tr runat="server" id="trFTSEnabled">
								<td>
									<div runat="server" id="ftsStat" class="text">
										<div runat="server" id="ftsInfoOK">
											<%=LocRM.GetString("tFTSIsWorking")%>
											<ibn:FormHelper ID="fhFTS" runat="server" ResKey="SearchSettings_FullTextSearchHint"
												Width="350px" Position="BR" />
											<br />
											<b>
												<%=LocRM.GetString("tIndexSize")%>&nbsp;:&nbsp;</b><asp:Label runat="server" CssClass="text"
													ID="lbIndSize"></asp:Label><br />
											<br />
											<b>
												<%=LocRM.GetString("tStatus")%>&nbsp;:&nbsp;</b><asp:Label runat="server" ID="lbStat"
													CssClass="text"></asp:Label>
										</div>
										<div runat="server" id="ftsInfoFailed" class="text">
											<br />
											<b>
												<%=LocRM.GetString("tFTSError")%>&nbsp;:&nbsp;</b>
											<asp:Label runat="server" ID="lbFTSErrorMessage" CssClass="text" ForeColor="Red"></asp:Label>
											<br />
											<%=LocRM.GetString("tFTSErrorHint")%>
										</div>
										<br />
										<div align="center" style="padding: 15px 5px 5px 5px;">
											<asp:LinkButton runat="server" ID="lbDisableFTS" Font-Bold="true" Font-Underline="true"
												ForeColor="red"></asp:LinkButton>
										</div>
									</div>
								</td>
							</tr>
						</table>
						<div>&nbsp;</div>
						<ibn:BlockHeaderLight ID="bhSendFile" runat="server" />
						<table class="ibn-stylebox-light text" cellspacing="0" cellpadding="7" width="100%"
							border="0">
							<tr>
								<td>
									<div class="text">
										<b><%=LocRM2.GetString("tSmtpBox")%>:</b>&nbsp;&nbsp;
										<asp:DropDownList ID="ddSmtpBoxes" runat="server" Width="220px"></asp:DropDownList>
										<br /><br />
										<asp:Button ID="btnSave" runat="server" CssClass="text" /> 
									</div>
								</td>
							</tr>
						</table>
					</td>
					<td width="50%" valign="top">
						<ibn:BlockHeaderLight ID="bhlWebDAV" runat="server" />
						<table class="ibn-stylebox-light text" cellspacing="0" cellpadding="7" width="100%"
							border="0">
							<tr runat="server" id="trWDDisabled">
								<td style="background-color: #ffffe1">
									<blockquote style="border-left: solid 2px #CE3431; padding-left: 10px; margin: 5px;
										margin-left: 15px; padding-top: 3px; padding-bottom: 3px">
										<%=LocRM.GetString("tWebDAVNotDefinedMessage")%>
										<div class="text" align="center" style="padding: 10px;">
											<asp:LinkButton runat="server" ID="lbEnableWD" Font-Bold="true" Font-Underline="true"
												ForeColor="Red"></asp:LinkButton>
										</div>
									</blockquote>
								</td>
							</tr>
							<tr runat="server" id="trWDEnabled">
								<td class="text">
									<%=LocRM.GetString("tWebDAVIsEnabled")%>
									<ibn:FormHelper ID="fhWebDAV" runat="server" ResKey="SearchSettings_WebDAVHint" Width="350px"
										Position="BL" />
									<div align="center" style="padding: 15px 5px 5px 5px;">
										<asp:LinkButton runat="server" ID="lbDisableWD" Font-Bold="true" Font-Underline="true"
											ForeColor="red"></asp:LinkButton>
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
