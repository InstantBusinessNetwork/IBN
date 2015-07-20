<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Documents.Modules.MetaDataView" Codebehind="MetaDataView.ascx.cs" %>
<%@ Register TagPrefix="ibn" TagName="MetaDataViewControl" src="..\..\Modules\MetaDataViewControl.ascx" %>
<%@ Register TagPrefix="ibn" TagName="MdpCustomization" src="..\..\Admin\Modules\MdpCustomization.ascx" %>
<%@ Register TagPrefix="ibn" TagName="DocumentInfo" src="DocumentInfo3.ascx" %>
<ibn:DocumentInfo runat="server" id="DocumentInfoTemplate" />
<ibn:MdpCustomization id="mdView" EnableCustomize="false" runat="server" ClassName="Documents" />