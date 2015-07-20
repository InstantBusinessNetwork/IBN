namespace Mediachase.UI.Web.Modules
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Collections;

	/// <summary>
	///		Summary description for BlockControl.
	/// </summary>
	public partial class BlockControl : System.Web.UI.UserControl, IToolbarLight
	{
		private ArrayList tabItems = new ArrayList();

		public void AddRightLink(string Text, string Url)
		{
			secHeader.AddRightLink(Text, Url);
		}

		#region method AddTab
		public void AddTab(string Tab, string Text, string Link, string Control)
		{
			TabItem tab = new TabItem(Tab, Text, Link, Control);
			tabItems.Add(tab);
		}
		#endregion

		#region method SelectTab
		public void SelectTab(string Tab)
		{
			ClearSelection();

			Tab = Tab.ToLower();

			bool found = false;
			foreach (TabItem ti in tabItems)
			{
				if (ti.Tab == Tab)
				{
					ti.Selected = true;
					found = true;
					break;
				}
			}
			if (!found && tabItems.Count > 0)
				((TabItem)tabItems[0]).Selected = true;
		}
		#endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
			BindTabs();
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
		}
		#endregion

		public void BindTabs()
		{
			foreach (TabItem ti in tabItems)
			{
				if (ti.Selected)
				{
					secHeader.AddText(ti.Text);

					System.Web.UI.UserControl control = (System.Web.UI.UserControl)LoadControl(ti.Control);
					phItems.Controls.Add(control);
				}
				else
				{
					secHeader.AddLeftLink(ti.Text, ti.Link);
				}
			}
		}

		#region ClearSelection
		private void ClearSelection()
		{
			foreach (TabItem ti in tabItems)
				if (ti.Selected)
					ti.Selected = false;
		}
		#endregion

		#region class TabItem
		private class TabItem
		{
			#region Tab
			private string tab;
			public string Tab
			{
				set
				{
					tab = value;
				}
				get 
				{
					return tab;
				}
			}
			#endregion

			#region Text
			private string text;
			public string Text
			{
				set
				{
					text = value;
				}
				get 
				{
					return text;
				}
			}
			#endregion

			#region Link
			private string link;
			public string Link
			{
				set
				{
					link = value;
				}
				get 
				{
					return link;
				}
			}
			#endregion

			#region Control
			private string control;
			public string Control
			{
				set
				{
					control = value;
				}
				get 
				{
					return control;
				}
			}
			#endregion

			#region Selected
			private bool selected;
			public bool Selected
			{
				set
				{
					selected = value;
				}
				get 
				{
					return selected;
				}
			}
			#endregion

			public TabItem(string Tab, string Text, string Link, string Control) 
			{
				this.tab = Tab.ToLower();
				this.text = Text; 
				this.link = Link;
				this.control = Control;
				this.selected = false;
			}
		}
		#endregion

		#region IToolbarLight Members

		BlockHeaderLightWithMenu IToolbarLight.GetToolBar()
		{
			return secHeader;
		}

		#endregion
	}
}
