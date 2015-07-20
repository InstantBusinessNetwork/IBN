<%@ Control Language="C#" AutoEventWireup="true" Inherits="Mediachase.UI.Web.Incidents.Modules.ArticleList" Codebehind="ArticleList.ascx.cs" %>
<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Reference Control="~/Modules/BlockHeaderLightWithMenu.ascx" %>
<%@ Reference Control="~/Modules/TagCloud.ascx"  %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" Src="~/Modules/BlockHeader.ascx" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeaderLight" src="../../Modules/BlockHeaderLightWithMenu.ascx" %>
<%@ register TagPrefix="dg" namespace="Mediachase.UI.Web.Modules.DGExtension" Assembly="Mediachase.UI.Web" %>
<%@ Register TagPrefix="ibn" TagName="TagCloud" Src="~/Modules/TagCloud.ascx" %>
<script type="text/javascript">
	function resizeTable()
	{
		var obj = document.getElementById('mainDiv');
		var toolbarRow = document.getElementById('trToolbar');
		var filterRow = document.getElementById('trFilter');

		var intHeight = 0;
		var intWidth = 0;
		if (typeof(window.innerWidth) == "number") 
		{
			intWidth = window.innerWidth;
			intHeight = window.innerHeight;
		} 
		else if (document.documentElement && (document.documentElement.clientWidth || document.documentElement.clientHeight)) 
		{
			intWidth = document.documentElement.clientWidth;
			intHeight = document.documentElement.clientHeight;
		} 
		else if (document.body && (document.body.clientWidth || document.body.clientHeight)) 
		{
			intWidth = document.body.clientWidth;
			intHeight = document.body.clientHeight;
		}
		obj.style.height = (intHeight - toolbarRow.offsetHeight - filterRow.offsetHeight) + "px";
		obj.style.width = intWidth + "px";
	}
	window.onresize=resizeTable;
	window.onload=resizeTable;
	
	function ShowHide(articleId)
	{
		var div1 = document.getElementById('div1_' + articleId);
		var div2 = document.getElementById('div2_' + articleId);
		if(div1 && div2)
		{
			if (div1.style.display == "block")
			{
				div1.style.display = "none";
				div2.style.display = "block";
			}
			else
			{
				div1.style.display = "block";
				div2.style.display = "none";
			}
		}
	}
	
	function CloseAll(sFiles, sText)
	{
	  window.opener.top.GetRefreshArticle(sFiles, sText);
	  window.close();
	}

</script>
<style type="text/css">
.clsEee {background-color:#eeeeee;}
.clsBeige {background-color:ffffe1;} /*FEF4B1*/
.clsWhite {background-color:White;}
.clsBeige div.clsEee {background-color:#FFD275;} /*FFD275*/
</style>

<table cellspacing="0" cellpadding="0" border="0" width="100%" height="100%" class="ibn-propertysheet">
	<tr id="trToolbar">
		<td height="22">
			<ibn:BlockHeader id="secHeader" runat="server" />
		</td>
	</tr>
	<tr id="trFilter">
		<td class="ibn-light ibn-navline" height="40">
			<table cellspacing="0" cellpadding="0" width="100%" border="0" style="padding:5px"><tr><td>
				<table class="ibn-propertysheet" width="100%" border="0" cellpadding="5" cellspacing="0">
					<tr>
						<td class="ibn-value">
							<asp:TextBox ID="txtSearch" runat="server" Width="150px"></asp:TextBox>
							<asp:Button ID="btnSearch" runat="server" OnClick="btnSearch_Click" />
						</td>
					</tr>
				</table>
			</td></tr></table>
		</td>
	</tr>
	<tr>
		<td valign="top">
			<div id="mainDiv" style="overflow:auto;">
				<div style="height:100%">
					<table cellpadding="0" cellspacing="0" width="100%" border="0">
						<tr>
							<td style="padding: 2 0 7 7;" width="60%" valign=top>
								<ibn:BlockHeaderLight id="hdrList" runat="server" />
								<table class="ibn-stylebox-light text" cellspacing="0" cellpadding="0" width="100%" border="0"><tr><td>
									<dg:datagridextended runat="server" ID="grdMain" AutoGenerateColumns="false" Width="100%" 
										CellPadding="0" CellSpacing="0" GridLines="None" AllowPaging="true" AllowSorting="false" PageSize="10" PagerStyle-CssClass="ibn-vb2" ShowHeader="false" 
										OnPageSizeChanged="grdMain_PageSizeChanged" OnPageIndexChanged="grdMain_PageIndexChanged"
										OnItemCommand="grdMain_ItemCommand">
										<Columns>
											<asp:boundcolumn visible="false" datafield="ArticleId"></asp:boundcolumn>
											<asp:TemplateColumn >
												<ItemTemplate>
												<div style="border-top: solid 1px #95B7F3; padding:0px;" 
													onmouseover="this.className='clsBeige';" 
													onmouseout="this.className='clsWhite';">
													<asp:ImageButton CommandName="Select" ImageAlign="right" id="ibSelect" runat="server" imageurl="../../Layouts/Images/Select.gif" Width="16" Height="16" style="cursor:pointer; margin:3px;"></asp:ImageButton>
													<div style="padding:5px; cursor:default;" class="clsEee" onclick='<%# "ShowHide(" + Eval("ArticleId").ToString() + ")" %>'>
														<b><%# Eval("Question") %></b>
													</div>
													<div style="display:block; padding:5px;" id='<%# "div1_" + Eval("ArticleId").ToString() %>'>
														<%# GetSubstring((string)Eval("Answer"), 130)%>...
													</div>
													<div style="display:none; padding:5px;" id='<%# "div2_" + Eval("ArticleId").ToString() %>'>
														<%# Eval("AnswerHTML") %>
													</div>
												</div>
												</ItemTemplate>
											</asp:TemplateColumn>
										</Columns>
									</dg:datagridextended>
								</td></tr></table>
							</td>
							<td style="padding: 2 7 7 7;" width="40%" valign=top>
								<ibn:BlockHeaderLight id="hdrTags" runat="server" />
								<table class="ibn-stylebox-light" cellspacing="0" width="100%" border="0"><tr><td style="padding: 0 7 7 7;">
								<ibn:TagCloud runat="server" id="ctrlTagCloud" OnTagClick="ctrlTagCloud_TagClick" ObjectType="20" TagCount="30" TagSizeCount="10"></ibn:TagCloud>
								</td></tr></table>
							</td>
						</tr>
					</table>
				</div>
			</div>
		</td>
  </tr>
</table>

<asp:Button runat="server" ID="btnReset" OnClick="btnReset_Click" style="display:none" />