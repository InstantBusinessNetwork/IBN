namespace Mediachase.UI.Web.Admin.Modules
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Reflection;
	using System.Resources;
	using System.Web;
	using System.Web.UI.HtmlControls;
	using System.Web.UI.WebControls;
	
	using Mediachase.IBN.Business;

	/// <summary>
	///		Summary description for TimeControl.
	/// </summary>
	public partial class TimeControl : System.Web.UI.UserControl
	{
		protected ResourceManager LocRM;

		protected void Page_Load(object sender, System.EventArgs e)
		{
			LocRM = new ResourceManager("Mediachase.Ibn.WebResources.App_GlobalResources.Admin.Resources.strIntervalsEditor", Assembly.Load(new AssemblyName("Mediachase.Ibn.WebResources")));
			if (!IsPostBack)
			{
				EnsureBindHours();
				EnsureBindMinutes();
			}
		}

		public int getFrom
		{
			get
			{
				return int.Parse(ddlHour.SelectedItem.Value)*60 + int.Parse(ddlMinute.SelectedItem.Value);
			}
		}

		public int getTo
		{
			get
			{
				return int.Parse(ddlHour1.SelectedItem.Value)*60 + int.Parse(ddlMinute1.SelectedItem.Value);
			}
		}

		public bool IsActive
		{
			get
			{
				if (ddlHour.SelectedIndex==0 && ddlHour1.SelectedIndex==0 && ddlMinute.SelectedIndex==0 && ddlMinute1.SelectedIndex==0)
					return false;

				else
					return true;
			}
		}

		#region EnsureBindHours
		private void EnsureBindHours()
		{
			if (ddlHour.Items.Count > 0)
				return;

			ddlHour.Items.Add("");
			ddlHour1.Items.Add("");

			DateTime dtNow = UserDateTime.UserNow;
			string sNow = dtNow.ToShortTimeString();

			if(sNow.IndexOf("M",0,sNow.Length)>=0)
			{
				ddlHour.Width = new Unit(60,UnitType.Pixel);
				ddlHour1.Width = new Unit(60,UnitType.Pixel);
				
				for (int i=0;i<=24;i++)
				{
					DateTime dt = new DateTime(1990,1,1,i % 24,0,0);
					string hts = dt.ToString("hh tt");
					if(i != 24)
						ddlHour.Items.Add(new ListItem(hts,i.ToString()));
					if(i != 0)
						ddlHour1.Items.Add(new ListItem(hts,i.ToString()));
				}
			}
			else
			{
				ddlHour.Width = new Unit(40,UnitType.Pixel);
				ddlHour1.Width = new Unit(40,UnitType.Pixel);
				for(int i=0;i <= 24;i++)
				{
					string str = i.ToString();
					if(i<10)
						str = "0" + str;
					if(i != 24)
						ddlHour.Items.Add(str);
					if(i != 0)
						ddlHour1.Items.Add(str);
				}
			}
			//ddlHour.ClearSelection();
		}
		#endregion

		#region EnsureBindMinutes
		private void EnsureBindMinutes()
		{
			if (ddlMinute.Items.Count > 0)
				return;

			ddlMinute.Items.Add("");
			ddlMinute1.Items.Add("");
			for(int i=0;i < 60;i = i+5)
			{
				string str = i.ToString();
				if(i<10)
					str = "0" + str;
				ddlMinute.Items.Add(new ListItem(str,i.ToString()));
				ddlMinute1.Items.Add(new ListItem(str,i.ToString()));
			}
			//ddlMinute.ClearSelection();
		}
		#endregion

		#region SetDate
		public void SetDate(DateTime dtDate, DateTime dtDatet)
		{
			EnsureBindHours();
			EnsureBindMinutes();

			if(dtDate.Hour == 0)
			{
				ddlHour.ClearSelection();
				ddlHour.Items[1].Selected = true;
			}			
			else
				for(int i=1;i < ddlHour.Items.Count; i++)
				{
					if((dtDate.Hour>i-1) && (dtDate.Hour<=i))
					{
						ddlHour.ClearSelection();
						ddlHour.Items[i+1].Selected = true;
					}
				}

			if(dtDate.Minute == 0)
			{
				ddlMinute.ClearSelection();
				ddlMinute.Items[1].Selected = true;
			}	
			else
				for(int i=1;i < ddlMinute.Items.Count; i++)
				{
					if((dtDate.Minute > int.Parse(ddlMinute.Items[i].Value)) && (dtDate.Minute <= int.Parse(ddlMinute.Items[i+1].Value)))
					{
						ddlMinute.ClearSelection();
						ddlMinute.Items[i+1].Selected = true;
					}
				}
//---------------------------------------
			if(dtDatet.Hour == 0)
			{
				ddlHour1.ClearSelection();
				ddlHour1.Items[ddlHour1.Items.Count-1].Selected = true;
			}			
			else
				for(int i=1;i < ddlHour1.Items.Count; i++)
				{
					if((dtDatet.Hour>i-1) && (dtDatet.Hour<=i))
					{
						ddlHour1.ClearSelection();
						ddlHour1.Items[i].Selected = true;
					}
				}

			if(dtDatet.Minute == 0)
			{
				ddlMinute1.ClearSelection();
				ddlMinute1.Items[1].Selected = true;
			}	
			else
				for(int i=1;i < ddlMinute1.Items.Count; i++)
				{
					if((dtDatet.Minute > int.Parse(ddlMinute1.Items[i].Value)) && (dtDatet.Minute <= int.Parse(ddlMinute.Items[i+1].Value)))
					{
						ddlMinute1.ClearSelection();
						ddlMinute1.Items[i+1].Selected = true;
					}
				}
		}

		#endregion

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
			this.cvCheckSelection.ServerValidate += new System.Web.UI.WebControls.ServerValidateEventHandler(this.cv_validate);

		}
		#endregion

		private void cv_validate(object source, System.Web.UI.WebControls.ServerValidateEventArgs args)
		{
			if (ddlHour.SelectedIndex==0 && ddlHour1.SelectedIndex==0 && ddlMinute.SelectedIndex==0 && ddlMinute1.SelectedIndex==0)
			{
				args.IsValid = true;
				return;
			}

			if (ddlHour.SelectedIndex==0 || ddlHour1.SelectedIndex==0 || ddlMinute.SelectedIndex==0 || ddlMinute1.SelectedIndex==0)
			{
				args.IsValid = false;
				return;
			}

			int hrsfrom = int.Parse(ddlHour.SelectedItem.Value);
			int hrsto = int.Parse(ddlHour1.SelectedItem.Value);

			int minfrom = int.Parse(ddlMinute.SelectedItem.Value);
			int minto = int.Parse(ddlMinute1.SelectedItem.Value);

			if (hrsfrom*60 + minfrom >= hrsto*60 + minto)
				args.IsValid = false;
		}

		public void Disable()
		{
			cvCheckSelection.Enabled = false;
			ddlHour.Enabled = false;
			ddlHour1.Enabled = false;
			ddlMinute.Enabled = false;
			ddlMinute1.Enabled = false;
		}

		public void Enable()
		{
			cvCheckSelection.Enabled = true;
			ddlHour.Enabled = true;
			ddlHour1.Enabled = true;
			ddlMinute.Enabled = true;
			ddlMinute1.Enabled = true;
		}
	}
}