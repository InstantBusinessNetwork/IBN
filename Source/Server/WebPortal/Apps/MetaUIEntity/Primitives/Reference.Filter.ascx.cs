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
using Mediachase.Ibn.Web.UI.WebControls;
using System.Text;
using Mediachase.Ibn.Core.Business;
using Mediachase.FileUploader.Configuration;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Web.UI;
using Mediachase.Ibn.Core;
using Mediachase.Ibn.Data.Meta.Management;

namespace Mediachase.UI.Web.Apps.MetaUIEntity.Primitives
{
	public partial class Reference_Filter : System.Web.UI.UserControl, IConditionValue
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			//lblText.Attributes.Add("onclick", "this.previousSibling.style.display = 'inline'; this.style.display = 'none';");
			lbAddItems.Click +=new EventHandler(lbAddItems_Click);
			customNotNull.ServerValidate += new ServerValidateEventHandler(customNotNull_ServerValidate);
			customNotNull.ErrorMessage = String.Format("*{0}", CHelper.GetResFileString("{IbnFramework.Common:tFilterSelectObject}"));
			GenerateRefreshScript();

		}

		protected void Page_PreRender(object sender, EventArgs e)
		{
			if (this.Value != null)
				customNotNull.ErrorMessage = string.Empty;
		}


		#region customNotNull_ServerValidate
		/// <summary>
		/// Handles the ServerValidate event of the customNotNull control.
		/// </summary>
		/// <param name="source">The source of the event.</param>
		/// <param name="args">The <see cref="System.Web.UI.WebControls.ServerValidateEventArgs"/> instance containing the event data.</param>
		void customNotNull_ServerValidate(object source, ServerValidateEventArgs args)
		{
			args.IsValid = (this.Value != null);
		} 
		#endregion

		#region GenerateRefreshScript
		/// <summary>
		/// Generates the refresh script.
		/// </summary>
		private void GenerateRefreshScript()
		{
			StringBuilder sb = new StringBuilder();
			
			sb.AppendFormat("function Refresh{0}(params){{", this.ID);
			sb.Append("var obj = Sys.Serialization.JavaScriptSerializer.deserialize(params);");
			sb.Append("if(obj && obj.CommandArguments && obj.CommandArguments.SelectedValue)");
			sb.AppendFormat("__doPostBack('{0}', obj.CommandArguments.SelectedValue);", lbAddItems.UniqueID);
			sb.Append("}");

			ClientScript.RegisterStartupScript(this.Page, this.Page.GetType(), Guid.NewGuid().ToString("N"),
			sb.ToString(), true);
		} 
		#endregion

		#region lbAddItems_Click
		/// <summary>
		/// Handles the Click event of the lbAddItems control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void lbAddItems_Click(object sender, EventArgs e)
		{
			string s = Request["__EVENTARGUMENT"];
			if (!String.IsNullOrEmpty(s))
			{
				string[] mas = s.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
				foreach (string item in mas)
				{
					PrimaryKeyId id = PrimaryKeyId.Parse(MetaViewGroupUtil.GetIdFromUniqueKey(item));
					this.Value = MetaViewGroupUtil.GetIdFromUniqueKey(item);

					string className = MetaViewGroupUtil.GetMetaTypeFromUniqueKey(item);
					EntityObject obj = BusinessManager.Load(className, id);
					
					MetaClass mc = MetaDataWrapper.GetMetaClassByName(obj.MetaClassName);
					lblValue.Text = obj.Properties[mc.TitleFieldName].Value.ToString();
				}
				this.RaiseBubbleEvent(this, e);
			}
		}
		#endregion

		#region BindFromValue
		/// <summary>
		/// Binds from value.
		/// </summary>
		private void BindFromValue(string metaClassName)
		{
			CommandParameters cp = new CommandParameters("MC_MUI_EntityDDSmall");
			cp.CommandArguments = new System.Collections.Generic.Dictionary<string, string>();
			if (!String.IsNullOrEmpty(metaClassName))
				cp.AddCommandArgument("Classes", metaClassName);
			cp.AddCommandArgument("Refresh", String.Format("Refresh{0}", this.ID));
			//Mediachase.Ibn.Web.UI.WebControls.CommandHandler.RegisterCloseOpenedFrameScript(this.Page, cp.ToString());

			string scriptToExecute = CommandManager.GetCurrent(this.Page).AddCommand(metaClassName, string.Empty, string.Empty, cp);

			if (this.Value == null)
			{
				lblValue.Text = CHelper.GetResFileString("{IbnFramework.Common:tFilterSelectObject}");
			}
			else
			{
				PrimaryKeyId id = PrimaryKeyId.Parse(this.Value.ToString());
				EntityObject obj = BusinessManager.Load(metaClassName, id);
				MetaClass mc = MetaDataWrapper.GetMetaClassByName(obj.MetaClassName);
				lblValue.Text = obj.Properties[mc.TitleFieldName].Value.ToString();
			}

			lblValue.Attributes.Add("onclick", scriptToExecute);
		} 
		#endregion

		#region IConditionValue Members

		#region BindData
		/// <summary>
		/// Binds the data.
		/// </summary>
		/// <param name="expressionPlace">The expression place.</param>
		/// <param name="expressionKey">The expression key.</param>
		/// <param name="node">The node.</param>
		/// <param name="condition">The condition.</param>
		public void BindData(string expressionPlace, string expressionKey, FilterExpressionNode node, ConditionElement condition)
		{
			if (node.Attributes == null)
				BindFromValue(string.Empty);
			else if (node.Attributes["ReferenceToMetaClassName"] != null)
				BindFromValue(node.Attributes["ReferenceToMetaClassName"]);
			else
				throw new ArgumentNullException("ReferenceToMetaClassName");

		} 
		#endregion

		#region Value
		/// <summary>
		/// Gets or sets the value.
		/// </summary>
		/// <value>The value.</value>
		public object Value
		{
			get
			{
				return ViewState["_Value"];
			}
			set
			{
				ViewState["_Value"] = value;
			}
		} 
		#endregion

		#endregion
	}
}