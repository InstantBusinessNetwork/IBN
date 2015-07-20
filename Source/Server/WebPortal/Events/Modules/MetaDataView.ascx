<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Events.Modules.MetaDataView" Codebehind="MetaDataView.ascx.cs" %>
<%@ Register TagPrefix="ibn" TagName="MetaDataViewControl" src="..\..\Modules\MetaDataViewControl.ascx" %>
<%@ Register TagPrefix="ibn" TagName="MdpCustomization" src="..\..\Admin\Modules\MdpCustomization.ascx" %>
<%@ Register TagPrefix="ibn" TagName="EventFullInfo" src="EventFullInfo.ascx" %>
<ibn:EventFullInfo id="ucEventFullInfo" runat="server" />
<ibn:MdpCustomization id="mdView" EnableCustomize="false" runat="server" ClassName="Events" />
