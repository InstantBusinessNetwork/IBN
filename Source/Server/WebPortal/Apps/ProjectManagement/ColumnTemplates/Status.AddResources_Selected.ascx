<%@ Control Language="C#" AutoEventWireup="true" ClassName="Mediachase.Ibn.Web.UI.ProjectManagement.ColumnTemplates.Status_AddResources_Selected" %>
<script runat="server" type="text/C#">
	protected string GetStatus(object _mbc, object _rp, object _ic)
	{
		bool mbc = false;
		if (_mbc != DBNull.Value)
			mbc = (bool)_mbc;

		bool rp = false;
		if (_rp != DBNull.Value)
			rp = (bool)_rp;

		bool ic = false;
		if (_ic != DBNull.Value)
			ic = (bool)_ic;

		if (!mbc) return String.Empty;
		else
			if (rp) return GetGlobalResourceObject("IbnFramework.Task", "Waiting").ToString();
			else
				if (ic) return GetGlobalResourceObject("IbnFramework.Task", "Accepted").ToString();
				else return GetGlobalResourceObject("IbnFramework.Task", "Denied").ToString();
	}
</script>
<div style="cursor:default;"><%#	GetStatus
(
	Eval("MustBeConfirmed"),
	Eval("ResponsePending"),
	Eval("IsConfirmed")
)%></div>