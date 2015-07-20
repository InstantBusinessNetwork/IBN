using System;
using System.Data;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace Mediachase.Ibn.Web.UI.Apps.WidgetEngine
{
	public class IbnWidgetContainer : CompositeDataBoundControl
	{
		//private UpdatePanel _up = null;
		private Control _wrapControl = null;
		public Control WrapControl
		{
			get
			{
				return _wrapControl;
			}
			set
			{
				_wrapControl = value;
			}
		}

		#region prop: Collapsed
		private bool _collapsed = false;
		public bool Collapsed
		{
			get { return _collapsed; }
			set { _collapsed = value; }
		} 
		#endregion

		public IbnWidgetContainer()
		{
		}

		public IbnWidgetContainer(Control c) : this()
		{
			this.WrapControl = c;
		}

		public IbnWidgetContainer(Control c, bool Collapsed)
			: this(c)
		{
			this.Collapsed = Collapsed;
		}

		protected override void Render(HtmlTextWriter writer)
		{
			writer.AddAttribute("id", this.ClientID);
			writer.AddAttribute("class", "IbnWidgetContainer");
			if (this.Collapsed)
				writer.AddStyleAttribute(HtmlTextWriterStyle.Display, "none");
			writer.RenderBeginTag("div");
			
			this.RenderContents(writer);		
			
			writer.RenderEndTag();
		}

		protected override int CreateChildControls(System.Collections.IEnumerable dataSource, bool dataBinding)
		{
			if (this.WrapControl != null)
			{
				UpdatePanel up = new UpdatePanel();

				up.ID = String.Format("up_{0}", this.WrapControl.ID);
				up.ChildrenAsTriggers = true;
				up.EnableViewState = true;
				up.UpdateMode = UpdatePanelUpdateMode.Conditional;
				up.ContentTemplateContainer.Controls.Add(this.WrapControl);

				this.Controls.Add(up);
				//this.LoadViewState(HttpContext.Current.Request.Form["__VIEWSTATE"]);
				return 1;
			}
			else
			{
				return 0;
			}
		}
	}
}