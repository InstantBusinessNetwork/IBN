using System;

namespace Mediachase.UI.Web.Modules
{
	public interface IToolbarLight
	{
		BlockHeaderLightWithMenu GetToolBar();
	}

	public interface IPageViewMenu
	{
		PageViewMenu GetToolBar();
	}

	public interface ITopTabs
	{
		TopTabs GetTopTabs();
	}
}
