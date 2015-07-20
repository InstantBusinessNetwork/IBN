using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Hosting;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.XPath;

using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.Ibn.Web.UI.Controls.Util;
using Mediachase.Ibn.XmlTools;

namespace Mediachase.UI.Web.Apps.MetaUI.Grid
{
	public partial class MetaGridServerEventAction : System.Web.UI.UserControl
	{
		public enum Mode
		{
			MetaView = 1,
			ListViewUI = 2
		}

		#region prop: GridId
		/// <summary>
		/// Gets or sets the grid id.
		/// </summary>
		/// <value>The grid id.</value>
		public string GridId
		{
			get
			{
				if (ViewState["__GridId"] != null)
					return ViewState["__GridId"].ToString();

				return string.Empty;
			}
			set
			{
				ViewState["__GridId"] = value;
			}
		}
		#endregion

		#region prop: GetCurrent
		/// <summary>
		/// Gets the get current ControlUpdateExtender.
		/// </summary>
		/// <value>The get current.</value>
		public GridViewUpdateExtender GetCurrent
		{
			get
			{
				return ctrlUpdater;
			}
		}
		#endregion

		#region prop: ClassName
		/// <summary>
		/// Gets or sets the name of the class.
		/// </summary>
		/// <value>The name of the class.</value>
		public string ClassName
		{
			get
			{
				string retval = String.Empty;
				if (ViewState["ClassName"] != null)
					retval = ViewState["ClassName"].ToString();
				return retval;
			}
			set
			{
				ViewState["ClassName"] = value;
			}
		}
		#endregion

		#region prop: ViewName
		/// <summary>
		/// Gets or sets the name of the view.
		/// </summary>
		/// <value>The name of the view.</value>
		public string ViewName
		{
			get
			{
				string retval = String.Empty;
				if (ViewState["ViewName"] != null)
					retval = ViewState["ViewName"].ToString();
				return retval;
			}
			set
			{
				ViewState["ViewName"] = value;
			}
		}
		#endregion

		#region prop: PlaceName
		/// <summary>
		/// Gets or sets the name of the place.
		/// </summary>
		/// <value>The name of the place.</value>
		public string PlaceName
		{
			get
			{
				string retval = String.Empty;
				if (ViewState["PlaceName"] != null)
					retval = ViewState["PlaceName"].ToString();
				return retval;
			}
			set
			{
				ViewState["PlaceName"] = value;
			}
		}
		#endregion

		#region GridActionMode
		/// <summary>
		/// Gets or sets the toolbar mode.
		/// </summary>
		/// <value>The toolbar mode.</value>
		public Mode GridActionMode
		{
			get
			{
				Mode retval = Mode.MetaView;
				if (ViewState["GridActionMode"] != null)
					retval = (Mode)Enum.Parse(typeof(Mode), ViewState["GridActionMode"].ToString());
				return retval;
			}
			set
			{
				ViewState["GridActionMode"] = value;
			}
		}
		#endregion

		#region Page_Load
		/// <summary>
		/// Handles the Load event of the Page control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_Load(object sender, EventArgs e)
		{
			BindGridEventAction();
		}
		#endregion

		#region Page_PreRedner
		/// <summary>
		/// Handles the PreRender event of the Page control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_PreRender(object sender, EventArgs e)
		{
			ctrlUpdater.GridId = this.GridId;
		}
		#endregion

		#region BindGridEventAction()
		/// <summary>
		/// Binds the grid event action.
		/// </summary>
		private void BindGridEventAction()
		{
			XPathNavigator actions;
			Selector selector = new Selector(ClassName, ViewName, PlaceName);

			if (GridActionMode == Mode.MetaView)
			{
				IXPathNavigable navigable = Mediachase.Ibn.XmlTools.XmlBuilder.GetXml(StructureType.MetaView, selector);
				actions = navigable.CreateNavigator().SelectSingleNode("MetaView/Grid");
			}
			else
			{
				IXPathNavigable navigable = Mediachase.Ibn.XmlTools.XmlBuilder.GetXml(StructureType.ListViewUI, selector);
				actions = navigable.CreateNavigator().SelectSingleNode("ListViewUI/Grid");
			}

			foreach (XPathNavigator gridItem in actions.SelectChildren("GridAction", string.Empty))
			{
				string commandName = gridItem.GetAttribute("commandName", string.Empty);
				string eventName = gridItem.GetAttribute("eventName", string.Empty);
				bool isEnable = true;
				//string controlId = gridItem.GetAttribute("controlId", string.Empty);

				Dictionary<string, string> dic = new Dictionary<string, string>();
				dic.Add("primaryKeyId", "%primaryKeyId%");
				CommandParameters cp = new CommandParameters(commandName, dic);

				ClientGridAction action = new ClientGridAction();

				try
				{
					action.EventName = (ClientGridEvents)Enum.Parse(typeof(ClientGridEvents), eventName);
				}
				catch
				{
					throw new ArgumentException(String.Format("Unkown value for enum ClientGridEvents: {0}", eventName));
				}

				if (commandName != string.Empty)
				{
					action.ActionScript = CommandManager.GetCurrent(this.Page).AddCommand(this.ClassName, this.ViewName, this.PlaceName, cp, out isEnable);

					action.ActionName = commandName;
					action.ActionParams = string.Empty;
					action.EventType = ClientGridEventType.Action;
					action.ControlUpdateId = string.Empty;
				}
				//else if (controlId != string.Empty)
				//{
				//    action.ControlUpdateId = controlId;
				//    action.EventType = ClientGridEventType.Postback;
				//}

				if (isEnable)
					ctrlUpdater.ActionList.Add(action);
			}
		}
		#endregion
	}
}