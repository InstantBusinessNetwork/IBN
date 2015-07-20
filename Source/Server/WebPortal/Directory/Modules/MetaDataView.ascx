<%@ Register TagPrefix="ibn" TagName="MdpCustomization" src="..\..\Admin\Modules\MdpCustomization.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Directory.Modules.MetaDataView" Codebehind="MetaDataView.ascx.cs" %>
<%@ Register TagPrefix="ibn" TagName="UserFullInfo" src="UserFullInfo.ascx" %>
<ibn:UserFullInfo id="ucUserFullInfo" title="" runat="server" />
<ibn:MdpCustomization id="mdView" EnableCustomize="false" runat="server" ClassName="Users" />