<%@ Control Language="c#" Inherits="Mediachase.UI.Web.ToDo.Modules.MetaDataView" Codebehind="MetaDataView.ascx.cs" %>
<%@ Register TagPrefix="ibn" TagName="MetaDataViewControl" src="..\..\Modules\MetaDataViewControl.ascx" %>
<%@ Register TagPrefix="ibn" TagName="MdpCustomization" src="..\..\Admin\Modules\MdpCustomization.ascx" %>
<%@ Register TagPrefix="ibn" TagName="GeneralInfo" src="GeneralInfo.ascx" %>
<%@ Register TagPrefix="ibn" TagName="Successors" src="..\..\ToDo\Modules\TodoSuccessors.ascx" %>
<%@ Register TagPrefix="ibn" TagName="Predecessors" src="..\..\ToDo\Modules\TodoPredecessors.ascx" %>
<ibn:GeneralInfo id="ucGeneralInfo" runat="server"></ibn:GeneralInfo>
<ibn:MdpCustomization id="mdView" EnableCustomize="false" runat="server" ClassName="Todo" />
<ibn:Predecessors id="Predecessors" runat="server" />
<ibn:Successors id="Successors" runat="server" />