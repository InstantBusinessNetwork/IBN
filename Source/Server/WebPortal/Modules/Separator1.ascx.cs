namespace Mediachase.UI.Web.Modules
{
	using System;
	using System.Collections;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using Mediachase.Ibn.Web.UI;

	/// <summary>
	///		Summary description for Separator.
	/// </summary>
	public partial class Separator1 : System.Web.UI.UserControl
	{
		private string _title = "";
		private ArrayList items = new ArrayList();

		public event SeparatorChangeEventHandler SeparatorChanged;

		public string Title
		{
			get
			{
				return _title;
			}
			set
			{
				_title = value;
			}
		}

		System.Web.UI.WebControls.Panel _pan = null;
		public System.Web.UI.WebControls.Panel pan
		{
			get
			{
				return _pan;
			}

			set
			{
				_pan = value;
			}

		}

		public bool IsMinimized
		{
			get
			{
				if (ViewState["IsMinimized"] != null)
					return (bool)ViewState["IsMinimized"];
				else
				{
					ViewState["IsMinimized"] = true;
					return true;
				}
			}

			set
			{
				ViewState["IsMinimized"] = value;
			}
		}

		public bool _IsClickable = true;
		public bool IsClickable
		{
			get
			{
				return _IsClickable;
			}

			set
			{
				_IsClickable = value;
			}
		}

		protected void Page_Load(object sender, System.EventArgs e)
		{
		}

		protected void Page_PreRender(object sender, System.EventArgs e)
		{
		}

		protected override void Render(HtmlTextWriter writer)
		{
			lblLinks.Text = "";

			bool setseparator = false;
			foreach (Object obj in items)
			{
				if (setseparator)
				{
					VSeparator1 sep = new VSeparator1();
					lblLinks.Text += sep.GetHTML();
				}
				else setseparator = true;

				if (obj is IGetHTML)
					lblLinks.Text += ((IGetHTML)obj).GetHTML();
			}
			SetImage();
			if (IsClickable)
				lbTitle.Text = Title;
			else
			{
				lblTitle.Text = Title;
				ibCompact.Visible = false;
			}
			base.Render(writer);
		}

		private void SetImage()
		{
			if (IsMinimized)
			{
				if (_pan != null) pan.Visible = false;
				spFont.Attributes.Add("class", "ibn-splink");
				ibCompact.ImageUrl = "../layouts/images/plusxp.gif";
			}
			else
			{
				spFont.Attributes.Add("class", "ibn-WPTitle");
				if (_pan != null) pan.Visible = true;
				ibCompact.ImageUrl = "../layouts/images/minusxp.gif";
			}
		}

		public void AddLinkItem(string text, string url)
		{
			items.Add(new LinkItem1(text, url));
		}

		public void AddVSeparator()
		{
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

		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.ibCompact.Click += new System.Web.UI.ImageClickEventHandler(this.ibCompact_Click);

		}
		#endregion

		private void ibCompact_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
			if (IsMinimized)
			{
				IsMinimized = false;
				OnSeparatorChange();
			}
			else
			{
				IsMinimized = true;
				OnSeparatorChange();
			}
		}

		public virtual void OnSeparatorChange()
		{
			if (SeparatorChanged != null)
			{
				SeparatorChanged(this);
			}
		}

		protected void lbTitle_Click(object sender, System.EventArgs e)
		{
			if (IsMinimized)
			{
				IsMinimized = false;
				OnSeparatorChange();
			}
			else
			{
				IsMinimized = true;
				OnSeparatorChange();
			}
		}
	}

	public delegate void SeparatorChangeEventHandler(Separator1 source);

	#region VSeparator1 class
	public class VSeparator1 : IGetHTML
	{

		#region Implementation of IGetHTML
		public string GetHTML()
		{
			return "&nbsp|&nbsp;";
		}
		#endregion
	} 
	#endregion
}
