using System;
using System.Collections;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Mediachase.Ibn.Data.Meta;

namespace Mediachase.Ibn.Web.UI.EventService
{
	public class EventsList: Repeater
	{
		private const string _defaultTemplate = "Value_defaultTemplate";
		private object _dataItem;

		#region DataItem
		public object DataItem
		{
			get { return _dataItem; }
			set { _dataItem = value; }
		}
		#endregion

		#region ItemCssClass
		public string ItemCssClass
		{
			get
			{
				if (ViewState["__itemCssClass"] == null)
					ViewState["__itemCssClass"] = "";
				return ViewState["__itemCssClass"].ToString();
			}
			set
			{
				ViewState["__itemCssClass"] = value;
			}
		}
		#endregion

		protected override void CreateChildControls()
		{
			if (DataItem != null && DataItem is IList)
				this.DataSource = (IList)DataItem;
			
			base.CreateChildControls();
		}

		protected override RepeaterItem CreateItem(int itemIndex, ListItemType itemType)
		{
			ItemTemplate = null;
			return base.CreateItem(itemIndex, itemType);
		}

		protected override void OnItemCreated(RepeaterItemEventArgs e)
		{
			if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
			{
				string sControl = "";

				MetaObject mo = (MetaObject)e.Item.DataItem;
				if (mo != null)
				{
					string className = CHelper.GetFromContext("ClassName").ToString();
					//hack IBN4.6
					if (className.ToLower().Equals("timetrackingentry"))
					{
						className = "TimeTrackingBlock";
					}
					//
					sControl = EventCode.GetEventControlVirtualPath("~/Apps", mo, className);
					if (String.IsNullOrEmpty(sControl))
                        sControl = _defaultTemplate;

					ViewState["Template_" + e.Item.ItemIndex.ToString()] = sControl;
				}
				else
				{
					if (ViewState["Template_" + e.Item.ItemIndex.ToString()] != null)
				        sControl = ViewState["Template_" + e.Item.ItemIndex.ToString()].ToString();
				    else
				        return;
				}
				
				if (String.IsNullOrEmpty(sControl))
				{  
				}
				else if (sControl == _defaultTemplate)
				{
					ItemTemplate = new EventDefaultTemp(ItemCssClass);
				}
				else
				{
					ItemTemplate = this.Page.LoadTemplate(sControl);
				}
				this.InitializeItem(e.Item);
			}

			base.OnItemCreated(e);
		}
	}

	public class EventDefaultTemp : ITemplate
	{
		private string _cssClass = "";
		public EventDefaultTemp(string cssClass)
		{
			_cssClass = cssClass;
		}

		#region ITemplate Members

		public void InstantiateIn(Control container)
		{
			HtmlGenericControl div = new HtmlGenericControl("div");
			if (!String.IsNullOrEmpty(_cssClass))
				div.Attributes.Add("class", _cssClass);
			div.DataBinding += new EventHandler(div_DataBinding);
			container.Controls.Add(div);
		}

		void div_DataBinding(object sender, EventArgs e)
		{
			HtmlGenericControl div = (HtmlGenericControl)sender;
			if (div.NamingContainer is IDataItemContainer)
			{
				IDataItemContainer container = (IDataItemContainer)div.NamingContainer;
				div.InnerHtml = CHelper.GetEventResourceString((MetaObject)container.DataItem);
			}
		}

		#endregion
	}
}
