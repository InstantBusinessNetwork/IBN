<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MetaGridServer.ascx.cs" Inherits="Mediachase.UI.Web.Apps.MetaUI.Grid.MetaGridServer" %>
<%@ Register Assembly="Mediachase.Ibn.Web.UI.WebControls" Namespace="Mediachase.Ibn.Web.UI.WebControls" TagPrefix="mc" %>
<%@ Register Assembly="Mediachase.UI.Web" Namespace="Mediachase.UI.Web.Apps.MetaUI.Grid" TagPrefix="mc3" %>
<asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="panelMainGridServer" EnableViewState="true" DynamicLayout="true" DisplayAfter="10">
	<ProgressTemplate>
		<div style="height: 20%; width: 30%; position: absolute; left: 35%; top: 40%; z-index: 10000; background-color: White; border: solid 1px #AAAAAA;">
			<div style="position: absolute; left: 45%; top: 40%;">
				<img src='<%= this.ResolveClientUrl("~/Images/IbnFramework/loading_rss.gif") %>' alt='loading' />
			</div>
		</div>
	</ProgressTemplate>
</asp:UpdateProgress>
<asp:UpdatePanel runat="server" ID="panelMainGridServer" ChildrenAsTriggers="true" UpdateMode="Conditional" EnableViewState="true" RenderMode="Inline">
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
		<PagerStyle HorizontalAlign="right" CssClass="serverGridPagingRight" />
		<PagerTemplate>	
			<div style="padding-top: 5px;">
				<div style="float: left; padding-left: 7px;">
					<asp:Literal ID="Literal1" runat="server" Text="<% $Resources: IbnFramework.GlobalMetaInfo, PageSize %>" />
					<asp:DropDownList runat="server" ID="ddPaging" AutoPostBack="true" CssClass="GridPaging" OnSelectedIndexChanged="ddPaging_SelectedIndexChanged" Height="18">
						<asp:ListItem Text="<% $Resources: IbnFramework.GlobalMetaInfo, PageSize10 %>" Value="10"></asp:ListItem>
						<asp:ListItem Text="<% $Resources: IbnFramework.GlobalMetaInfo, PageSize50 %>" Value="50"></asp:ListItem>
						<asp:ListItem Text="<% $Resources: IbnFramework.GlobalMetaInfo, PageSize100 %>" Value="100"></asp:ListItem>
						<asp:ListItem Text="<% $Resources: IbnFramework.GlobalMetaInfo, PageSizeAll %>" Value="-1"></asp:ListItem>
					</asp:DropDownList>
					<asp:Literal ID="Literal2" runat="server" Text="<% $Resources: IbnFramework.Common, tOf %>" />
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
		</PagerTemplate>
<%--		<EmptyDataRowStyle CssClass="serverGridBodyEmpty" />
		<EmptyDataTemplate>
				<asp:Literal ID="Literal2" runat="server" Text="<%$Resources: IbnFramework.Global, _mc_NoItems%>" />
		</EmptyDataTemplate>--%>
	</mc3:IbnGridView>

	<mc:GridViewHeaderExtender runat="server" TargetControlID="MainGrid" ID="MainGridExt" WrapperDivCssClass="WrapperDiv" HeaderHeight="25" BottomHeight="25"></mc:GridViewHeaderExtender>
	<input runat="server" id="hfCheckedCollection" type="hidden"/>
	</ContentTemplate>
</asp:UpdatePanel>
