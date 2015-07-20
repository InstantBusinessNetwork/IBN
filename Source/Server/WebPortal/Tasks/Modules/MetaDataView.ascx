<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Tasks.Modules.MetaDataView" Codebehind="MetaDataView.ascx.cs" %>
<%@ Register TagPrefix="ibn" TagName="MetaDataViewControl" src="..\..\Modules\MetaDataViewControl.ascx" %>
<%@ Register TagPrefix="ibn" TagName="MdpCustomization" src="..\..\Admin\Modules\MdpCustomization.ascx" %>
<%@ Register TagPrefix="ibn" TagName="TaskFullInfo" src="TaskFullInfo.ascx" %>
<%@ Register TagPrefix="ibn" TagName="Successors" src="..\..\Tasks\Modules\TaskSuccessors.ascx" %>
<%@ Register TagPrefix="ibn" TagName="Predecessors" src="..\..\Tasks\Modules\TaskPredecessors.ascx" %>
<%@ Register TagPrefix="ibn" TagName="Sub" src="..\..\Tasks\Modules\SubTasks.ascx" %>
<ibn:TaskFullInfo id="ucTaskFullInfo" runat="server" />
<ibn:MdpCustomization id="mdView" EnableCustomize="false" runat="server" ClassName="Tasks" />
<ibn:predecessors id="Predecessors" runat="server" />
<ibn:successors id="Successors" runat="server"></ibn:successors>
<ibn:Sub id="subTasks" runat="server"></ibn:Sub>
