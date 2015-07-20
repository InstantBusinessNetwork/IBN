namespace Mediachase.UI.Web.Wizards.Modules
{
	using System;
	using System.Collections;
	using System.ComponentModel;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.SessionState;
	using System.Web.UI;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Globalization;
	using System.Resources;
	using Mediachase.IBN.Business;
	using Mediachase.UI.Web.Util;
	using Mediachase.Ibn.Web.Interfaces;

	/// <summary>
	///		Summary description for CommonWizard.
	/// </summary>
	public partial class CommonWizard : System.Web.UI.UserControl, IWizardControl
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Wizards.Resources.strComWd", typeof(CommonWizard).Assembly);

		ArrayList subtitles = new ArrayList();
		ArrayList steps = new ArrayList();
		private int _stepCount = 2;



		protected int ObjectType = 0;
		private int ObjectID = 0;
		protected string GlobalString = "";

		protected string ObjectName
		{
			get
			{
				try
				{
					return Enum.GetName(typeof(ObjectTypes), ObjectType) != null ? Enum.GetName(typeof(ObjectTypes), ObjectType) : "blank";
				}
				catch
				{
					return "blank";
				}
			}
		}


		protected void Page_Load(object sender, System.EventArgs e)
		{
			//  [3/18/2004]
		}

		#region LoadControls
		private void LoadControls()
		{
			if (Request["ObjectType"] != null)
			{
				ObjectType = int.Parse(Request["ObjectType"]);
				ViewState["ObjectType"] = ObjectType;
			}
			if (Request["ObjectID"] != null)
			{
				ObjectID = int.Parse(Request["ObjectID"]);
				ViewState["ObjectID"] = ObjectID;
			}
			if (ObjectID == 0 || ObjectType == 0)
				return;
			if (!Page.IsPostBack)
				BindStep1();



			if (rbActions.SelectedIndex == 0)
			{
				if (ObjectType == (int)ObjectTypes.Issue)
					subtitles.Add(LocRM.GetString("s1SubTitleInc"));
				if (ObjectType == (int)ObjectTypes.ToDo)
					subtitles.Add(LocRM.GetString("s1SubTitleToDo"));
				if (ObjectType == (int)ObjectTypes.CalendarEntry)
					subtitles.Add(LocRM.GetString("s1SubTitleCalEntry"));
				subtitles.Add(LocRM.GetString("s2SubTitle") + LocRM.GetString(ObjectName.ToLower() + "DP"));
				steps.Add(basic);
				steps.Add(upload);
			}
			else if (rbActions.SelectedIndex == 1)
			{
				if (ObjectType == (int)ObjectTypes.Issue)
					subtitles.Add(LocRM.GetString("s1SubTitleInc"));
				if (ObjectType == (int)ObjectTypes.ToDo)
					subtitles.Add(LocRM.GetString("s1SubTitleToDo"));
				if (ObjectType == (int)ObjectTypes.CalendarEntry)
					subtitles.Add(LocRM.GetString("s1SubTitleCalEntry"));
				subtitles.Add(LocRM.GetString("s3SubTitle1") + LocRM.GetString(ObjectName.ToLower() + "RP") + LocRM.GetString("s3SubTitle2"));
				steps.Add(basic);
				steps.Add(categories);
			}
			else
			{
				if (ObjectType == (int)ObjectTypes.Issue)
					subtitles.Add(LocRM.GetString("s1SubTitleInc"));
				if (ObjectType == (int)ObjectTypes.ToDo)
					subtitles.Add(LocRM.GetString("s1SubTitleToDo"));
				if (ObjectType == (int)ObjectTypes.CalendarEntry)
					subtitles.Add(LocRM.GetString("s1SubTitleCalEntry"));
				subtitles.Add(LocRM.GetString("s4SubTitle") + LocRM.GetString(ObjectName.ToLower() + "DP"));
				steps.Add(basic);
				steps.Add(comments);
			}

			GlobalString = LocRM.GetString(ObjectName.ToLower() + "_for") + " ";
			GlobalString += LocRM.GetString("QuickTipBasic1") + " ";
			GlobalString += LocRM.GetString(ObjectName.ToLower() + "RP") + " ";
			GlobalString += LocRM.GetString("QuickTipBasic2");
		}
		#endregion

		#region GetCategoriesReader
		IDataReader GetCategoriesReader(int ObjectType, int ObjectID, Label lblTextHeader, ListBox lbCategories)
		{
			if (ObjectType == (int)ObjectTypes.Issue)
			{
				lblTextHeader.Text = LocRM.GetString("TextHeaderInc");
				lbCategories.DataSource = Incident.GetListCategoriesAll();
				return Incident.GetListCategories(ObjectID);
			}
			if (ObjectType == (int)ObjectTypes.ToDo)
			{
				lblTextHeader.Text = LocRM.GetString("TextHeaderToDo");
				lbCategories.DataSource = ToDo.GetListCategoriesAll();
				return ToDo.GetListCategories(ObjectID);
			}
			if (ObjectType == (int)ObjectTypes.CalendarEntry)
			{
				lblTextHeader.Text = LocRM.GetString("TextHeaderCalEntry");
				lbCategories.DataSource = CalendarEntry.GetListCategoriesAll();
				return CalendarEntry.GetListCategories(ObjectID);
			}
			return null;
		}
		#endregion

		#region Step1
		private void BindStep1()
		{
			rbActions.Items.Add(new ListItem(" " + LocRM.GetString("tUpload") + " " + LocRM.GetString(ObjectName.ToLower() + "DP") + ".", "0"));
			rbActions.Items.Add(new ListItem(" " + LocRM.GetString("tCategories"), "1"));
			rbActions.Items.Add(new ListItem(" " + LocRM.GetString("tComments"), "2"));
			if (Request["item"] != null)
				rbActions.SelectedIndex = int.Parse(Request["item"]);
			else
				rbActions.SelectedIndex = 0;

			using (IDataReader reader = GetCategoriesReader(ObjectType, ObjectID, lblTextHeader, lbCategories))
			{
				lbCategories.DataTextField = "CategoryName";
				lbCategories.DataValueField = "CategoryId";
				lbCategories.DataBind();
				while (reader.Read())
				{
					for (int i = 0; i < lbCategories.Items.Count; i++)
					{
						ListItem lItem = lbCategories.Items.FindByText(reader["CategoryName"].ToString());
						if (lItem != null) lItem.Selected = true;
					}
				}
			}
		}
		#endregion

		private void ShowStep(int step)
		{
			HtmlButton btn = null;
			object template = this.Parent.Parent.Parent;
			if (template is WizardTemplate)
				btn = ((WizardTemplate)template).GetbtnNext();

			basic.Visible = false;
			upload.Visible = false;
			categories.Visible = false;
			comments.Visible = false;

			if (step == 2 && rbActions.SelectedIndex == 0 && btn != null)
				btn.Attributes.Add("onclick", "DisableButtons(this);ShowProgress();");

			#region Save
			if (step == _stepCount + 1)
			{
				string sItem = "";
				if (ObjectType == 7)
				{
					if (rbActions.SelectedIndex == 0)
					{
						sItem = "0";
						if ((ffileUp.PostedFile != null && ffileUp.PostedFile.ContentLength > 0))
							Incident.UploadFile(ObjectID, ffileUp.PostedFile.FileName, ffileUp.PostedFile.InputStream);
					}
					if (rbActions.SelectedIndex == 1)
					{
						sItem = "1";
						ArrayList alCategories = new ArrayList();
						for (int i = 0; i < lbCategories.Items.Count; i++)
							if (lbCategories.Items[i].Selected)
								alCategories.Add(int.Parse(lbCategories.Items[i].Value));
						Issue2.AddIssueCategories(ObjectID, alCategories);
					}
					if (rbActions.SelectedIndex == 2)
					{
						sItem = "2";
						Incident.AddDiscussion(ObjectID, txtComments.Text);
					}
					string sPath = "../Wizards/CommonWizard.aspx?ObjectType=7&ObjectID=" + ObjectID.ToString();
					if (sItem != "") sPath = sPath + "&item=" + sItem;
					Response.Redirect(sPath);
				}
				if (ObjectType == 6)
				{
					if (rbActions.SelectedIndex == 0)
					{
						sItem = "0";
						if ((ffileUp.PostedFile != null && ffileUp.PostedFile.ContentLength > 0))
							ToDo.UploadFile(ObjectID, ffileUp.PostedFile.FileName, ffileUp.PostedFile.InputStream);
					}
					if (rbActions.SelectedIndex == 1)
					{
						sItem = "1";
						ArrayList alCategories = new ArrayList();
						for (int i = 0; i < lbCategories.Items.Count; i++)
							if (lbCategories.Items[i].Selected)
								alCategories.Add(int.Parse(lbCategories.Items[i].Value));
						ToDo2.SetGeneralCategories(ObjectID, alCategories);
					}
					if (rbActions.SelectedIndex == 2)
					{
						sItem = "2";
						ToDo.AddDiscussion(ObjectID, txtComments.Text);
					}
					string sPath = "../Wizards/CommonWizard.aspx?ObjectType=6&ObjectID=" + ObjectID.ToString();
					if (sItem != "") sPath = sPath + "&item=" + sItem;
					Response.Redirect(sPath);
				}
				if (ObjectType == 4)
				{
					if (rbActions.SelectedIndex == 0)
					{
						sItem = "0";
						if ((ffileUp.PostedFile != null && ffileUp.PostedFile.ContentLength > 0))
							CalendarEntry.UploadFile(ObjectID, ffileUp.PostedFile.FileName, ffileUp.PostedFile.InputStream);
					}
					if (rbActions.SelectedIndex == 1)
					{
						sItem = "1";
						ArrayList alCategories = new ArrayList();
						for (int i = 0; i < lbCategories.Items.Count; i++)
							if (lbCategories.Items[i].Selected)
								alCategories.Add(int.Parse(lbCategories.Items[i].Value));
						CalendarEntry2.SetGeneralCategories(ObjectID, alCategories);
					}
					if (rbActions.SelectedIndex == 2)
					{
						sItem = "2";
						CalendarEntry.AddDiscussion(ObjectID, txtComments.Text);
					}
					string sPath = "../Wizards/CommonWizard.aspx?ObjectType=4&ObjectID=" + ObjectID.ToString();
					if (sItem != "") sPath = sPath + "&item=" + sItem;
					Response.Redirect(sPath);
				}
				return;
			}
			#endregion

			((Panel)steps[step - 1]).Visible = true;

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

		}
		#endregion

		#region Implementation of IWizardControl

		public int StepCount { get { return _stepCount; } }
		public string TopTitle { get { return LocRM.GetString("tTopTitle"); } }
		public bool ShowSteps { get { return false; } }
		public string Subtitle { get; private set; }
		public string MiddleButtonText { get; private set; }
		public string CancelText { get; private set; }

		public void SetStep(int stepNumber)
		{
			LoadControls();
			ShowStep(stepNumber);
			Subtitle = (string)subtitles[stepNumber - 1];
			if (stepNumber > 1 && rbActions.SelectedIndex == 0)
				MiddleButtonText = LocRM.GetString("tUploadBtn");
			else if (stepNumber > 1 && rbActions.SelectedIndex > 0)
				MiddleButtonText = LocRM.GetString("tSave");
			else
				MiddleButtonText = null;
			CancelText = LocRM.GetString("tClose");
		}


		public string GenerateFinalStepScript()
		{
			if (ViewState["ObjectType"] != null && ViewState["ObjectID"] != null)
			{
				int iObjType = int.Parse(ViewState["ObjectType"].ToString());
				int iObjID = int.Parse(ViewState["ObjectID"].ToString());
				if (iObjType == (int)ObjectTypes.Issue)
					return "try{window.opener.top.right.location.href='../Incidents/IncidentView.aspx?IncidentID=" + iObjID.ToString() + "';} catch (e) {} window.close();";
				else if (iObjType == (int)ObjectTypes.ToDo)
					return "try{window.opener.top.right.location.href='../ToDo/ToDoView.aspx?ToDoID=" + iObjID.ToString() + "';} catch (e) {} window.close();";
				else if (iObjType == (int)ObjectTypes.CalendarEntry)
					return "try{window.opener.top.right.location.href='../Events/EventView.aspx?EventID=" + iObjID.ToString() + "';} catch (e) {} window.close();";
				else
					return String.Empty;
			}
			else
				return String.Empty;
		}

		public void CancelAction()
		{
			Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), GenerateFinalStepScript(), true);
		}
		#endregion
	}
}
