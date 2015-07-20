using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace Mediachase.UI.Web.Apps.MetaUI.Grid
{
	/// <summary>
	/// ѕрослойка над GridView, чтобы была возможность пропихнуть атрибуты после LoadPostData(second try), но до RaisePostBackEvent
	/// </summary>
	public class IbnGridView : GridView
	{
		public static readonly string primaryKeyIdAttr = "ibn_primaryKeyId";
		public static readonly string ibnServerGridAttr = "ibn_serverGridCheckbox";

		#region prop: CommandArgumentLoaded
		private bool _commandArgumentLoaded;

		/// <summary>
		/// Gets or sets a value indicating whether [command argument loaded].
		/// </summary>
		/// <value>
		/// 	<c>true</c> if [command argument loaded]; otherwise, <c>false</c>.
		/// </value>
		public bool CommandArgumentLoaded
		{
			get { return _commandArgumentLoaded; }
			set { _commandArgumentLoaded = value; }
		} 
		#endregion

		#region prop: TextBoxControlId
		/// <summary>
		/// Gets or sets the text box control.
		/// </summary>
		/// <value>The text box control.</value>
		public string TextBoxControlId
		{
			get
			{
				if (ViewState["_TextBoxControlId"] != null)
					return ViewState["_TextBoxControlId"].ToString();

				return string.Empty;
			}
			set
			{
				ViewState["_TextBoxControlId"] = value;
			}
		} 
		#endregion		

		#region prop: PostbackControlId
		/// <summary>
		/// Gets or sets the text box control.
		/// </summary>
		/// <value>The text box control.</value>
		public string PostbackControlId
		{
			get
			{
				if (ViewState["_PostbackControlId"] != null)
					return ViewState["_PostbackControlId"].ToString();

				return string.Empty;
			}
			set
			{
				ViewState["_PostbackControlId"] = value;
			}
		}
		#endregion		

		#region prop: ShowCheckboxes
		/// <summary>
		/// Gets or sets a value indicating whether [show checkboxes].
		/// </summary>
		/// <value><c>true</c> if [show checkboxes]; otherwise, <c>false</c>.</value>
		public bool ShowCheckboxes
		{
			get
			{
				if (ViewState["_ShowCheckboxes"] == null)
					return true;

				return Convert.ToBoolean(ViewState["_ShowCheckboxes"].ToString());
			}
			set { ViewState["_ShowCheckboxes"] = value; }
		} 
		#endregion

		#region prop: ForceShowPaging
		/// <summary>
		/// Gets or sets a value indicating whether [force show paging].
		/// </summary>
		/// <value><c>true</c> if [force show paging]; otherwise, <c>false</c>.</value>
		public bool ForceShowPaging
		{
			get
			{
				if (ViewState["_ForceShowPaging"] == null)
					return true;

				return Convert.ToBoolean(ViewState["_ForceShowPaging"].ToString());
			}
			set { ViewState["_ForceShowPaging"] = value; }
		}
		#endregion

		#region prop: ResetTemplate
		/// <summary>
		/// Gets or sets a value indicating whether [reset template].
		/// </summary>
		/// <value><c>true</c> if [reset template]; otherwise, <c>false</c>.</value>
		public bool ResetTemplate
		{
			get
			{
				if (ViewState["_ResetTemplate"] == null)
					return false;

				return Convert.ToBoolean(ViewState["_ResetTemplate"].ToString());
			}
			set { ViewState["_ResetTemplate"] = value; }
		}
		#endregion
	

		protected override void OnLoad(EventArgs e)
		{			
			base.OnLoad(e);
			this.CommandArgumentLoaded = false;
		}

		#region OnRowCreated
		/// <summary>
		/// Raises the <see cref="E:System.Web.UI.WebControls.GridView.RowCreated"></see> event.
		/// </summary>
		/// <param name="e">A <see cref="T:System.Web.UI.WebControls.GridViewRowEventArgs"></see> that contains event data.</param>
		protected override void OnRowCreated(GridViewRowEventArgs e)
		{
			if (this.ShowCheckboxes && (e.Row.RowType == DataControlRowType.DataRow || e.Row.RowType == DataControlRowType.Header))
			{
				TableCell cell = new TableCell();

				CheckBox cb = new CheckBox();
				cb.AutoPostBack = false;
				cb.ID = String.Format("{0}_cb", e.Row.ID);

				if (e.Row.RowType == DataControlRowType.DataRow)
					cb.InputAttributes.Add(ibnServerGridAttr, "1");
				else
					cb.Attributes.Add("onclick", "ibn_serverGridCheckboxHandler(this)");				

				cell.Controls.Add(cb);
				e.Row.Cells.AddAt(0, cell);
			}

			base.OnRowCreated(e);
		} 
		#endregion

		#region CreateChildControls
		/// <summary>
		/// Creates the control hierarchy used to render the <see cref="T:System.Web.UI.WebControls.GridView"></see> control using the specified data source.
		/// </summary>
		/// <param name="dataSource">An <see cref="T:System.Collections.IEnumerable"></see> that contains the data source for the <see cref="T:System.Web.UI.WebControls.GridView"></see> control.</param>
		/// <param name="dataBinding">true to indicate that the child controls are bound to data; otherwise, false.</param>
		/// <returns>The number of rows created.</returns>
		protected override int CreateChildControls(System.Collections.IEnumerable dataSource, bool dataBinding)
		{
			if (this.ResetTemplate)
				this.PagerTemplate = null;

			int retVal = base.CreateChildControls(dataSource, dataBinding);
			//fix GridView "feature", when it hides bottomPagerRow
			//see last lines  base.CreateChildControls(System.Collections.IEnumerable dataSource, bool dataBinding)
			if (this.BottomPagerRow != null && this.ForceShowPaging)
				this.BottomPagerRow.Visible = true;

			return retVal;
		} 
		#endregion

		#region OnBubbleEvent
		/// <summary>
		/// Determines whether the event for the Web server control is passed up the page's user interface (UI) server control hierarchy.
		/// </summary>
		/// <param name="source">The source of the event.</param>
		/// <param name="e">An <see cref="T:System.EventArgs"></see> that contains event data.</param>
		/// <returns>
		/// true if the event has been canceled; otherwise, false.
		/// </returns>
		protected override bool OnBubbleEvent(object source, EventArgs e)
		{
			GridViewCommandEventArgs eNew = (e as GridViewCommandEventArgs);
			if (!this.CommandArgumentLoaded && this.BottomPagerRow != null)
			{
				TextBox tb = (TextBox)this.BottomPagerRow.FindControl(this.TextBoxControlId);
				Control commandSource = (Control)(e as GridViewCommandEventArgs).CommandSource;

				if (commandSource.ID == this.PostbackControlId)
					eNew = new GridViewCommandEventArgs(((GridViewCommandEventArgs)e).CommandSource, new CommandEventArgs("Page", tb.Text));

				this.CommandArgumentLoaded = true;
			}

			return base.OnBubbleEvent(source, eNew);
		} 
		#endregion
	}
}
