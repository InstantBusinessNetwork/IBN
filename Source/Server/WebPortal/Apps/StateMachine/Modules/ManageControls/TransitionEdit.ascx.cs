using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using Mediachase.Ibn.Core;
using Mediachase.Ibn.Data.Meta;
using Mediachase.Ibn.Data.Services;

namespace Mediachase.Ibn.Web.UI.Apps.StateMachine.Modules.ManageControls
{
	public partial class TransitionEdit : System.Web.UI.UserControl
	{
		protected Mediachase.Ibn.Data.Services.StateMachine sm = null;

		#region RefreshButton
		public string RefreshButton
		{
			get
			{
				string retval = String.Empty;
				if (Request.QueryString["btn"] != null)
					retval = Request.QueryString["btn"];
				return retval;
			}
		}
		#endregion

		#region ClassName
		public string ClassName
		{
			get
			{
				string retval = String.Empty;
				if (Request.QueryString["ClassName"] != null)
					retval = Request.QueryString["ClassName"];
				return retval;
			}
		}
		#endregion

		#region SMId
		public int SMId
		{
			get
			{
				int retval = -1;
				if (Request.QueryString["SMId"] != null)
					retval = int.Parse(Request.QueryString["SMId"]);
				return retval;
			}
		}
		#endregion

		#region FromState
		public string FromState
		{
			get
			{
				string retval = String.Empty;
				if (Request.QueryString["FromState"] != null)
					retval = Request.QueryString["FromState"];
				return retval;
			}
		}
		#endregion

		protected void Page_Load(object sender, EventArgs e)
		{
			sm = StateMachineManager.GetStateMachine(ClassName, SMId);

			GenerateStructure();

			if (!IsPostBack)
			{
				MetaObject mo = StateMachineManager.GetState(ClassName, FromState);
				lblFrom.Text = CHelper.GetResFileString(mo.Properties["FriendlyName"].Value.ToString());
			}
		}

		#region GenerateStructure
		private void GenerateStructure()
		{
			foreach (State toState in sm.States)
			{
				string stateName = toState.Name;

				if (stateName == FromState)
					continue;

				MetaObject mo = StateMachineManager.GetState(ClassName, stateName);

				HtmlTableRow tr = new HtmlTableRow();
				tr.Style.Add(HtmlTextWriterStyle.Height, "25px");

				HtmlTableCell td1 = new HtmlTableCell();
				HtmlTableCell td2 = new HtmlTableCell();
				td1.NoWrap = true;

				CheckBox cb = new CheckBox();
				cb.ID = String.Format("chk{0}", stateName);

				TextBox txt = new TextBox();
				txt.ID = String.Format("txt{0}", stateName);
				txt.Width = Unit.Percentage(100);


				if (!IsPostBack)
				{
					cb.Text = CHelper.GetResFileString(mo.Properties["FriendlyName"].Value.ToString());

					StateTransition st = sm.FindTransition(FromState, stateName);
					if (st != null)
					{
						cb.Checked = true;
						txt.Text = st.Name;
					}
					else
					{
						txt.Text = mo.Properties["FriendlyName"].Value.ToString();
						txt.Style.Add(HtmlTextWriterStyle.Display, "none");
					}
				}

				td1.Controls.Add(cb);
				td2.Controls.Add(txt);
				tr.Cells.Add(td1);
				tr.Cells.Add(td2);
				tblMain.Rows.Add(tr);

				cb.Attributes.Add("onclick", String.Format("ShowHide('{0}', '{1}')", cb.ClientID, txt.ClientID));
			}
		}
		#endregion

		#region btnSave_Click
		protected void btnSave_Click(object sender, EventArgs e)
		{
			StateTransition[] stList = sm.GetAvailableTransitions(sm.GetState(FromState));
			foreach (StateTransition st in stList)
				sm.Transitions.Remove(st);

			for (int i = 1; i < tblMain.Rows.Count; i++)
			{
				HtmlTableRow tr = tblMain.Rows[i];
				HtmlTableCell td1 = tr.Cells[0];
				HtmlTableCell td2 = tr.Cells[1];

				CheckBox chk = td1.Controls[0] as CheckBox;
				TextBox txt = td2.Controls[0] as TextBox;

				string toState = txt.ID.Substring(3);

				if (chk.Checked)
				{
					sm.Transitions.Add(new StateTransition(txt.Text, FromState, toState));
				}
			}
			sm.Save();

			// Closing window
			if (RefreshButton == String.Empty)
			{
				CHelper.CloseItAndRefresh(Response);
			}
			else  // Dialog Mode
			{
				CHelper.CloseItAndRefresh(Response, RefreshButton);
			}
		}
		#endregion
	}
}