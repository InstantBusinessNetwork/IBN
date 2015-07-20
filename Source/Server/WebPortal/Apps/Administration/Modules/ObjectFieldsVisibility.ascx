<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ObjectFieldsVisibility.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.Administration.Modules.ObjectFieldsVisibility" %>
<%@ Reference Control="~/Apps/Common/Design/BlockHeader2.ascx" %>
<%@ Reference Control="~/Apps/MetaUIEntity/Modules/EntityDropDown.ascx" %>
<%@ Reference Control="~/Modules/TimeControl.ascx" %>
<%@ Register TagPrefix="mc" TagName="BlockHeader2" Src="~/Apps/Common/Design/BlockHeader2.ascx" %>
<%@ Register TagPrefix="mc" TagName="EntityList" Src="~/Apps/MetaUIEntity/Modules/EntityDropDown.ascx" %>
<%@ Register TagPrefix="mc" TagName="Time" src="~/Modules/TimeControl.ascx" %>
<%@ Register TagPrefix="btn" namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>
<script type="text/javascript" src='<%=this.Page.ResolveUrl("~/Scripts/mcCalendScript.js") %>'></script>
<script type="text/javascript" src='<%=this.Page.ResolveUrl("~/scripts/entityDD.js") %>'></script>
<script type="text/javascript">
	function closeWarning() {
		var obj = document.getElementById('<%=divWarning.ClientID %>');
		if (obj)
			obj.style.display = "none";
	}

	function LoadFunc() {
		setTimeout('closeWarning()', 3000);
	}
</script>
<table cellspacing="0" cellpadding="0" border="0" width="100%" class="ibn-stylebox2">
	<tr>
		<td colspan="2">
			<mc:BlockHeader2 runat="server" ID="BlockHeaderMain" />
		</td>
	</tr>
	<tr>
		<td colspan="2" class="ibn-alternating ibn-navline" style="padding:10px;">
			<b><%=GetGlobalResourceObject("IbnFramework.Admin", "ObjectType").ToString()%>:</b>&nbsp;&nbsp;<asp:DropDownList ID="ddObjectType" runat="server" Width="150px" AutoPostBack="true"></asp:DropDownList>
		</td>
	</tr>
	<tr>
		<td style="padding: 5px;">
			<asp:UpdatePanel ID="upMain" runat="server">
				<ContentTemplate>
					<table cellpadding="7" border="0" width="100%">
						<tr>
							<td>
								<asp:DataGrid ID="dgMain" runat="server" AllowPaging="false" AllowSorting="false"
									AutoGenerateColumns="false" GridLines="None" Width="100%">
									<Columns>
										<asp:TemplateColumn>
											<HeaderStyle CssClass="ibn-vh2" />
											<ItemStyle CssClass="ibn-vb2" />
											<ItemTemplate>
												<%# Eval("FieldName") %>
											</ItemTemplate>
										</asp:TemplateColumn>
										<asp:TemplateColumn>
											<HeaderStyle CssClass="ibn-vh2" />
											<ItemStyle CssClass="ibn-vb2" Width="70px" HorizontalAlign="Center" />
											<ItemTemplate>
												<asp:CheckBox ID="cbAllowEdit" runat="server" Checked='<%#(bool)Eval("AllowEdit") %>' />
											</ItemTemplate>
										</asp:TemplateColumn>
										<asp:TemplateColumn>
											<HeaderStyle CssClass="ibn-vh2" />
											<ItemStyle CssClass="ibn-vb2" Width="70px" HorizontalAlign="Center" />
											<ItemTemplate>
												<asp:CheckBox ID="cbAllowView" runat="server" Checked='<%#(bool)Eval("AllowView") %>' />
											</ItemTemplate>
										</asp:TemplateColumn>
										<asp:TemplateColumn>
											<HeaderStyle CssClass="ibn-vh2" />
											<ItemStyle CssClass="ibn-vb2" Width="200px" />
											<ItemTemplate>
												<%# GetFriendlyDefaultValue(Eval("FieldId").ToString(), Eval("DefaultValue").ToString()) %>
											</ItemTemplate>
											<EditItemTemplate>
												<asp:CheckBox ID="cbDefault" runat="server" Visible="false" />
												<asp:DropDownList ID="ddDefault" runat="server" Visible="false" Width="200px"></asp:DropDownList>
												<asp:ListBox ID="lbDefault" runat="server" Visible="false" Width="200px" Rows="5" SelectionMode="Multiple"></asp:ListBox>
												<mc:EntityList ID="clientDefault" RegisterScript="false" runat="server" ObjectTypes="Contact,Organization" Width="200px" Visible="false" />
												<mc:Time id="timeDefault" ShowTime="HM" HourSpinMaxValue="999" ViewStartDate="True" runat="server" Visible="false" />
												<asp:TextBox ID="tbDefault" runat="server" Visible="false" TextMode="MultiLine" Width="190" Rows="3"></asp:TextBox>
											</EditItemTemplate>
										</asp:TemplateColumn>
										<asp:TemplateColumn>
											<HeaderStyle CssClass="ibn-vh-right" />
											<ItemStyle CssClass="ibn-vb2" Width="50px" HorizontalAlign="Right" />
											<ItemTemplate>
												<asp:ImageButton ID="editBtn" runat="server" CommandName="Edit" CommandArgument='<%# Eval("FieldId") %>' ImageUrl="~/layouts/images/edit.gif" ImageAlign="AbsMiddle" ToolTip='<%# GetGlobalResourceObject("IbnFramework.Admin", "EditDefValue") %>' />
											</ItemTemplate>
											<EditItemTemplate>
												<asp:ImageButton ID="saveBtn" runat="server" CommandName="Update" CommandArgument='<%# Eval("FieldId") %>' ImageUrl="~/layouts/images/saveitem.gif" ImageAlign="AbsMiddle" />&nbsp;
												<asp:ImageButton ID="cancelBtn" runat="server" CommandName="Cancel" CommandArgument='<%# Eval("FieldId") %>' ImageUrl="~/layouts/images/cancel.gif" ImageAlign="AbsMiddle" />
											</EditItemTemplate>
										</asp:TemplateColumn>
									</Columns>	
								</asp:DataGrid>
							</td>
						</tr>			
						<tr>
							<td style="padding-top:15px;" align="right">
								<btn:ImButton ID="btnSave" Runat="server" Class="text" style="width:110px;" onserverclick="btnSave_Click"></btn:ImButton>&nbsp;
								<btn:ImButton ID="btnCancel" CausesValidation="false" Runat="server" Class="text" IsDecline="true" style="width:110px;" onserverclick="btnCancel_Click" />
							</td>
						</tr>
					</table>		
				</ContentTemplate>
				<Triggers>
					<asp:AsyncPostBackTrigger ControlID="ddObjectType" />
				</Triggers>
			</asp:UpdatePanel>
		</td>
		<td style="padding:5px;" class="text" valign="top" width="300px">
			<div style="background-color:#ffffe1;border:1px solid #bbb;" class="text">
			<blockquote style="border-left:solid 2px #CE3431; padding-left:10px; margin:5px; margin-left:15px; padding-top:3px; padding-bottom:3px">
				<%=GetGlobalResourceObject("IbnFramework.Admin", "ObjectVisibilityWarning").ToString()%>
			</blockquote>
			</div>
			<asp:UpdatePanel ID="upWarning" runat="server" UpdateMode="Conditional">
				<ContentTemplate>
					<div id="divWarning" runat="server" style="padding-top:10px;">
					<div style="background-color:#ffffe1;border:1px solid #bbb;" class="text">
						<blockquote style="border-left:solid 2px #CE3431; padding-left:10px; margin:5px; margin-left:15px; padding-top:3px; padding-bottom:3px">
							<%=GetGlobalResourceObject("IbnFramework.Admin", "SaveSuccessfull").ToString()%>
						</blockquote>
					</div>
					</div>
				</ContentTemplate>
			</asp:UpdatePanel>
		</td>
	</tr>
</table>