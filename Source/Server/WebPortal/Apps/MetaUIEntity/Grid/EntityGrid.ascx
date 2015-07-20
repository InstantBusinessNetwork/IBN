<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EntityGrid.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.EntityGrid" %>
<%@ Register Assembly="Mediachase.Ibn.Web.UI.WebControls" Namespace="Mediachase.Ibn.Web.UI.WebControls" TagPrefix="mc" %>
<%@ Register Assembly="Mediachase.UI.Web" Namespace="Mediachase.UI.Web.Apps.MetaUI.Grid" TagPrefix="mc3" %>
<mc:GridViewHeaderExtender2 runat="server" TargetControlID="MainGrid" ID="MainGridExt" WrapperDivCssClass="WrapperDiv" HeaderHeight="25" BottomHeight="30" PaddingWidth="10">
	<ContentTemplate>
		<mc3:IbnGridView runat="server" ID="MainGrid"				
					AllowSorting="true"
					AllowPaging="True"
					AutoGenerateColumns="False"
					ShowFooter="False"
					EnableViewState="True"
					TextBoxControlId="tbCurrentPage"
					PostbackControlId="btnRefresh"
					CssClass="WrapperDiv"
					GridLines="None"
					CellSpacing="0"
					CellPadding="0"
					BorderStyle="None"
					EnableSortingAndPagingCallbacks="false"
					>
			<HeaderStyle CssClass="serverGridHeaderOnly"/>
			<PagerSettings Mode="NumericFirstLast" />
			<PagerTemplate>	
				<div style="padding-top: 5px;" id="DivPaging" runat="server">
					<div style="float: left; padding-left: 7px;">
						<asp:Literal ID="Literal1" runat="server" Text="<% $Resources: IbnFramework.GlobalMetaInfo, PageSize %>" />
						<asp:DropDownList runat="server" ID="ddPaging" AutoPostBack="true" CssClass="GridPaging" OnSelectedIndexChanged="ddPaging_SelectedIndexChanged" Height="18">
							<asp:ListItem Text="<% $Resources: IbnFramework.GlobalMetaInfo, PageSize10 %>" Value="10"></asp:ListItem>
							<asp:ListItem Text="<% $Resources: IbnFramework.GlobalMetaInfo, PageSize25 %>" Value="25"></asp:ListItem>
							<asp:ListItem Text="<% $Resources: IbnFramework.GlobalMetaInfo, PageSize50 %>" Value="50"></asp:ListItem>
							<asp:ListItem Text="<% $Resources: IbnFramework.GlobalMetaInfo, PageSize100 %>" Value="100"></asp:ListItem>
						</asp:DropDownList>
							<span>&nbsp;</span>
							<asp:Literal ID="ltTotalElements2" runat="server" Text="<% $Resources: IbnFramework.Common, tTotalRecords %>" />
							<asp:Literal ID="ltTotalElements" runat="server" Text="" />	
					</div>
					<div style="float: right; padding-right: 7px;" runat="server" id="pagingContainer">
						<span style="padding: 0px; vertical-align: bottom;">
							<asp:ImageButton runat="server" ID="ImageButtonFirst" CommandName="Page" CommandArgument="First" ImageUrl="~/Images/IbnFramework/page-first.gif" ToolTip="<% $Resources: IbnFramework.ListInfo, pagingFirst %>"/>
							<asp:ImageButton runat="server" ID="ImageButtonPrevious" CommandName="Page" CommandArgument="Prev" ImageUrl="~/Images/IbnFramework/page-prev.gif" ToolTip="<% $Resources: IbnFramework.ListInfo, pagingPrev %>"/>
							<asp:Image runat="server" ID="split1" ImageUrl="~/Images/IbnFramework/grid-blue-split.gif" />
						</span>
						<span style="padding: 2px 4px 2px 4px;">
							<asp:TextBox runat="server" ID="tbCurrentPage" Width="30" Height="12" AutoCompleteType="none"></asp:TextBox>
							<span style="vertical-align: top;">
							<asp:Literal ID="Literal3" runat="server" Text="<% $Resources: IbnFramework.Common, tOf %>" />
							<asp:Literal ID="ltTotalPage" runat="server" Text="" />
							</span>										
						</span>
						<span style="padding: 0px; vertical-align: bottom;">
							<asp:ImageButton runat="server" ID="btnRefresh" CommandName="Page" ImageUrl="~/Images/IbnFramework/refresh.gif"/>
							<asp:Image runat="server" ID="split2" ImageUrl="~/Images/IbnFramework/grid-blue-split.gif" />
							<asp:ImageButton runat="server" ID="ImageButtonNext" CommandName="Page" CommandArgument="Next" ImageUrl="~/Images/IbnFramework/page-next.gif" ToolTip="<% $Resources: IbnFramework.ListInfo, pagingNext %>" />
							<asp:ImageButton runat="server" ID="ImageButtonLast" CommandName="Page" CommandArgument="Last" ImageUrl="~/Images/IbnFramework/page-last.gif" ToolTip="<% $Resources: IbnFramework.ListInfo, pagingLast %>"/>
						</span>
					</div>
				</div>
				<div style="display: none;" id="DivPagingDashboard">
				</div>
			</PagerTemplate>
		</mc3:IbnGridView>	
	</ContentTemplate>
</mc:GridViewHeaderExtender2>
<input runat="server" id="hfCheckedCollection" type="hidden"/>