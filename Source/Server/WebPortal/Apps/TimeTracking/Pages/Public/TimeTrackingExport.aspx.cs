using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using Mediachase.Ibn.Core;
using Mediachase.Ibn.Data.Meta.Management;
using Mediachase.Ibn.Data.Meta;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Data.Services;

namespace Mediachase.Ibn.Web.UI.TimeTracking.Pages.Public
{
    public partial class TimeTrackingExport : System.Web.UI.Page
    {
        #region prop: ViewName
        public string ViewName
        {
            get
            {
                if (Request["ViewName"] != null)
                    return Request["ViewName"];

                if (CHelper.GetFromContext("MetaViewName") != null)
					return (string)CHelper.GetFromContext("MetaViewName");

                return string.Empty;
            }
        }
        #endregion

        #region prop: StartDate
        public DateTime StartDate
        {
            get
            {
                if (Request["StartDate"] != null)
                    return Convert.ToDateTime(Request["StartDate"]).Date;

				return CHelper.GetWeekStartByDate(DateTime.Now).Date;
            }
        }
        #endregion

        #region prop: CurrentView
        private MetaView currentView;

        public MetaView CurrentView
        {
            get
            {
                if (currentView == null)
                {
                    if (Mediachase.Ibn.Data.DataContext.Current.MetaModel.MetaViews[ViewName] == null)
                        throw new ArgumentException(String.Format("Cant find meta view: {0}", ViewName));

                    currentView = Mediachase.Ibn.Data.DataContext.Current.MetaModel.MetaViews[ViewName];

                    if (currentView.MetaClass.Fields["DayT"] != null &&
                        !currentView.AvailableFields.Contains(currentView.MetaClass.Fields["DayT"]))
                    {
                        using (MetaClassManagerEditScope scope = DataContext.Current.MetaModel.BeginEdit())
                        {
                            currentView.AvailableFields.Add(currentView.MetaClass.Fields["DayT"]);
                            scope.SaveChanges();
                        }
                    }
                }

                return currentView;
            }
        }
        #endregion

        #region GetMetaViewPreference
        public McMetaViewPreference GetMetaViewPreference()
        {
            McMetaViewPreference pref = Mediachase.Ibn.Core.UserMetaViewPreference.Load(CurrentView, Mediachase.IBN.Business.Security.CurrentUser.UserID);

            if (pref == null || pref.Attributes.Count == 0)
            {
				McMetaViewPreference.CreateDefaultUserPreference(CurrentView);
                pref = Mediachase.Ibn.Core.UserMetaViewPreference.Load(CurrentView, Mediachase.IBN.Business.Security.CurrentUser.UserID);
            }

            return pref;
        }
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            dgMain.ItemDataBound += new DataGridItemEventHandler(dgMain_ItemDataBound);

			#region lock
			lock (this.GetType())
			{
				switch (this.ViewName)
				{
					case "TT_MyGroupByWeek":
						{
							MyGroupByWeek();
							break;
						}
					case "TT_MyGroupByWeekProject":
						{
							MyGroupByWeekProject();
							break;
						}
					case "TT_MyRejectedGroupByWeekProject":
						{
							MyRejectedGroupByWeekProject();
							break;
						}
					case "TT_CurrentProjectGroupByWeekUser":
						{
							CurrentProjectGroupByWeekUser();
							break;
						}
					case "TT_ManagerGroupByWeekUser":
						{
							ManagerGroupByWeekUser();
							break;
						}
					case "TT_ManagerGroupByWeekProject":
						{
							ManagerGroupByWeekProject();
							break;
						}
					case "TT_ManagerGroupByUserProject":
						{
							ManagerGroupByUserProject();
							break;
						}
					case "TT_ManagerGroupByProjectUser":
						{
							ManagerGroupByProjectUser();
							break;
						}
					default:
						{
							//GroupForTimeSheet();
							break;
						}
				}
			} 
			#endregion

            CHelper.AddToContext("MetaViewName", this.ViewName);
            BindGrid();
            Mediachase.UI.Web.Util.CommonHelper.ExportExcel(dgMain, "ListView.xls", null);
        }

		#region dgMain_ItemDataBound
		void dgMain_ItemDataBound(object sender, DataGridItemEventArgs e)
		{
			return;
			//McMetaViewPreference mvPref = GetMetaViewPreference();
			//for (int i = 0; i < e.Item.Cells.Count; i++)
			//{
			//    //if (e.Item.ItemType != ListItemType.Footer && i > 0) //if (e.Row.Cells[i].Controls[0] is Mediachase.UI.Web.IbnNext.Modules.FieldControls.BaseType)  DataControlRowType.DataRow
			//    //{
			//        if (e.Item.Cells[i].Controls.Count > 0)
			//        {
			//            ((Mediachase.UI.Web.IbnNext.Modules.FieldControls.BaseType)e.Item.Cells[i].Controls[0]).DataItem = (MetaObject)e.Item.DataItem;
			//            if (i < mvPref.GetVisibleMetaField().Length)
			//            {
			//                ((Mediachase.UI.Web.IbnNext.Modules.FieldControls.BaseType)e.Item.Cells[i].Controls[0]).FieldName = mvPref.GetVisibleMetaField()[i].Name;
			//            }
			//        }
			//    //}

			//}
			//e.Item.DataBind();
		} 
		#endregion


		#region BindGrid
		void BindGrid()
		{
			MetaViewPreference mvPref = GetMetaViewPreference();

			//dgMain.Columns.Clear();

			//foreach (MetaField field in mvPref.GetVisibleMetaField())
			//{
			//    dgMain.Columns.Add((new ListColumnFactory(this.ViewName)).GetExcelColumn(this.Page, field));
			//}

			Guid uidToRemove = Guid.Empty;
			foreach (FilterElement fe in mvPref.Filters.GetListBySource("StartDate"))
			{
				uidToRemove = fe.Uid;
				break;
			}

			if (uidToRemove != Guid.Empty)
			{
				mvPref.Filters.Remove(uidToRemove);
			}

			mvPref.Filters.Add(new FilterElement("StartDate", FilterElementType.Equal, this.StartDate));

			MetaObject[] list = CurrentView.List(mvPref);
			DataTable dt = new DataTable();
			dt.Columns.Add(new DataColumn("Title", typeof(string)));
			dt.Columns.Add(new DataColumn("Day1", typeof(float)));
			dt.Columns.Add(new DataColumn("Day2", typeof(float)));
			dt.Columns.Add(new DataColumn("Day3", typeof(float)));
			dt.Columns.Add(new DataColumn("Day4", typeof(float)));
			dt.Columns.Add(new DataColumn("Day5", typeof(float)));
			dt.Columns.Add(new DataColumn("Day6", typeof(float)));
			dt.Columns.Add(new DataColumn("Day7", typeof(float)));
			dt.Columns.Add(new DataColumn("DayT", typeof(float)));
			dt.Columns.Add(new DataColumn("StateFriendlyName", typeof(string)));
			int paddingValue = 0;

			if (CurrentView.PrimaryGroupBy != null)
				paddingValue += 25;

			if (CurrentView.SecondaryGroupBy != null)
				paddingValue += 20;

			foreach (MetaObject mo in list)
			{
				//if (mo.Properties["ParentBlockId"].OriginalValue == null && CurrentView.SecondaryGroupBy != null)
				//    continue;

				DataRow row = dt.NewRow();

				if (mo.Properties["MetaViewGroupByType"] != null && mo.Properties["MetaViewGroupByType"].OriginalValue.ToString() == MetaViewGroupByType.Primary.ToString())
				{
					if (this.ViewName == "TT_ManagerGroupByUserProject")
					{
						string sReferencedClass = mo.Properties["OwnerId"].GetMetaType().Attributes[McDataTypeAttribute.ReferenceMetaClassName].ToString();
						MetaClass mc = MetaDataWrapper.GetMetaClassByName(sReferencedClass);
						MetaObject obj = MetaObjectActivator.CreateInstance(mc, int.Parse(mo.Properties["MetaViewGroupByKey"].OriginalValue.ToString()));

						row["Title"] = String.Format("<b>{0}</b>", obj.Properties[mc.TitleFieldName].Value.ToString());
					}
					else
					{
						row["Title"] = String.Format("<b>{0}</b>", mo.Properties["Title"].Value.ToString());
					}
				}
				else if (mo.Properties["MetaViewGroupByType"] != null && mo.Properties["MetaViewGroupByType"].OriginalValue.ToString() == MetaViewGroupByType.Secondary.ToString())
				{
					if (this.ViewName == "TT_ManagerGroupByProjectUser")
					{
						string sReferencedClass = mo.Properties["OwnerId"].GetMetaType().Attributes[McDataTypeAttribute.ReferenceMetaClassName].ToString();
						MetaClass mc = MetaDataWrapper.GetMetaClassByName(sReferencedClass);
						MetaObject obj = MetaObjectActivator.CreateInstance(mc, int.Parse(mo.Properties["MetaViewGroupByKey"].OriginalValue.ToString()));

						row["Title"] = String.Format("<div style='padding-left: 25px;'>{0}</div>", obj.Properties[mc.TitleFieldName].Value.ToString());
					}
					else
					{
						row["Title"] = String.Format("<div style='padding-left: 25px;'>{0}</div>", mo.Properties["Title"].Value.ToString());
					}
				}
				else
				{
					row["Title"] = String.Format("<div style='padding-left: {1}px;'>{0}</div>", mo.Properties["Title"].Value.ToString(), paddingValue);
				}

				row["Day1"] = Convert.ToDouble(mo.Properties["Day1"].Value);
				row["Day2"] = Convert.ToDouble(mo.Properties["Day2"].Value);
				row["Day3"] = Convert.ToDouble(mo.Properties["Day3"].Value);
				row["Day4"] = Convert.ToDouble(mo.Properties["Day4"].Value);
				row["Day5"] = Convert.ToDouble(mo.Properties["Day5"].Value);
				row["Day6"] = Convert.ToDouble(mo.Properties["Day6"].Value);
				row["Day7"] = Convert.ToDouble(mo.Properties["Day7"].Value);
				row["DayT"] = Convert.ToDouble(mo.Properties["DayT"].Value);

				if (mo.Properties["StateFriendlyName"].Value != null)
					row["StateFriendlyName"] = CHelper.GetResFileString(mo.Properties["StateFriendlyName"].Value.ToString());
				else
					row["StateFriendlyName"] = "";

				dt.Rows.Add(row);
			}

			for (int i = 0; i < dgMain.Columns.Count; i++)
			{
				// don't show AreFinancesRegistered field
				if (ViewName == "TT_ManagerGroupByProjectUser" || ViewName == "TT_ManagerGroupByUserProject")
					dgMain.Columns[i].HeaderText = CHelper.GetResFileString(mvPref.GetVisibleMetaField()[i + 1].FriendlyName);
				else
					dgMain.Columns[i].HeaderText = CHelper.GetResFileString(mvPref.GetVisibleMetaField()[i].FriendlyName);
			}

			dgMain.DataSource = dt;

			dgMain.PageSize = 1000;

			dgMain.DataBind();
		} 
		#endregion


        #region ManagerGroupByProjectUser
        void ManagerGroupByProjectUser()
        {
            CurrentView.PrimaryGroupBy = new GroupByElement("BlockTypeInstanceId");
            CurrentView.PrimaryGroupBy.IsPreGroupObjectVisible = true;
            CurrentView.PrimaryGroupBy.IsPostGroupObjectVisible = false;

            CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Title", ValueSourceType.Field, "ParentBlock", false));
            //CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("StateFriendlyName", ValueSourceType.Field, "StateFriendlyName", true));
            CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day1", ValueSourceType.Function, GroupByObjectField.SumFunction));
            CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day2", ValueSourceType.Function, GroupByObjectField.SumFunction));
            CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day3", ValueSourceType.Function, GroupByObjectField.SumFunction));
            CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day4", ValueSourceType.Function, GroupByObjectField.SumFunction));
            CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day5", ValueSourceType.Function, GroupByObjectField.SumFunction));
            CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day6", ValueSourceType.Function, GroupByObjectField.SumFunction));
            CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day7", ValueSourceType.Function, GroupByObjectField.SumFunction));
            CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("DayT", ValueSourceType.Function, GroupByObjectField.SumFunction));

            CurrentView.SecondaryGroupBy = new GroupByElement("OwnerId");
            CurrentView.SecondaryGroupBy.IsPreGroupObjectVisible = true;
            CurrentView.SecondaryGroupBy.IsPostGroupObjectVisible = false;
            CurrentView.SecondaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Title", ValueSourceType.Field, "ParentBlock", false));
            CurrentView.SecondaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("ParentBlockId", ValueSourceType.Field, "ParentBlockId"));
            CurrentView.SecondaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("StateFriendlyName", ValueSourceType.Field, "StateFriendlyName", true));
            CurrentView.SecondaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day1", ValueSourceType.Function, GroupByObjectField.SumFunction));
            CurrentView.SecondaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day2", ValueSourceType.Function, GroupByObjectField.SumFunction));
            CurrentView.SecondaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day3", ValueSourceType.Function, GroupByObjectField.SumFunction));
            CurrentView.SecondaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day4", ValueSourceType.Function, GroupByObjectField.SumFunction));
            CurrentView.SecondaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day5", ValueSourceType.Function, GroupByObjectField.SumFunction));
            CurrentView.SecondaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day6", ValueSourceType.Function, GroupByObjectField.SumFunction));
            CurrentView.SecondaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day7", ValueSourceType.Function, GroupByObjectField.SumFunction));
            CurrentView.SecondaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("DayT", ValueSourceType.Function, GroupByObjectField.SumFunction));

        }
        #endregion

        #region ManagerGroupByUserProject
        void ManagerGroupByUserProject()
        {
            CurrentView.PrimaryGroupBy = new GroupByElement("OwnerId");
            CurrentView.PrimaryGroupBy.IsPreGroupObjectVisible = true;
            CurrentView.PrimaryGroupBy.IsPostGroupObjectVisible = false;

            CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Title", ValueSourceType.Field, "ParentBlock", false));
            CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day1", ValueSourceType.Function, GroupByObjectField.SumFunction));
            CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day2", ValueSourceType.Function, GroupByObjectField.SumFunction));
            CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day3", ValueSourceType.Function, GroupByObjectField.SumFunction));
            CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day4", ValueSourceType.Function, GroupByObjectField.SumFunction));
            CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day5", ValueSourceType.Function, GroupByObjectField.SumFunction));
            CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day6", ValueSourceType.Function, GroupByObjectField.SumFunction));
            CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day7", ValueSourceType.Function, GroupByObjectField.SumFunction));
            CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("DayT", ValueSourceType.Function, GroupByObjectField.SumFunction));

            CurrentView.SecondaryGroupBy = new GroupByElement("ParentBlockId");
            CurrentView.SecondaryGroupBy.IsPreGroupObjectVisible = true;
            CurrentView.SecondaryGroupBy.IsPostGroupObjectVisible = false;
            CurrentView.SecondaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Title", ValueSourceType.Field, "ParentBlock", false));
            CurrentView.SecondaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("ParentBlockId", ValueSourceType.Field, "ParentBlockId"));
            CurrentView.SecondaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("StateFriendlyName", ValueSourceType.Field, "StateFriendlyName", true));
            CurrentView.SecondaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day1", ValueSourceType.Function, GroupByObjectField.SumFunction));
            CurrentView.SecondaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day2", ValueSourceType.Function, GroupByObjectField.SumFunction));
            CurrentView.SecondaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day3", ValueSourceType.Function, GroupByObjectField.SumFunction));
            CurrentView.SecondaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day4", ValueSourceType.Function, GroupByObjectField.SumFunction));
            CurrentView.SecondaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day5", ValueSourceType.Function, GroupByObjectField.SumFunction));
            CurrentView.SecondaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day6", ValueSourceType.Function, GroupByObjectField.SumFunction));
            CurrentView.SecondaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day7", ValueSourceType.Function, GroupByObjectField.SumFunction));
            CurrentView.SecondaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("DayT", ValueSourceType.Function, GroupByObjectField.SumFunction));

        }
        #endregion

        #region ManagerGroupByWeekProject
        void ManagerGroupByWeekProject()
        {
            CurrentView.PrimaryGroupBy = new GroupByElement("StartDate");
            CurrentView.PrimaryGroupBy.IsPreGroupObjectVisible = true;
            CurrentView.PrimaryGroupBy.IsPostGroupObjectVisible = false;

            CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Title", ValueSourceType.Field, "ParentBlock", false));
            CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day1", ValueSourceType.Function, GroupByObjectField.SumFunction));
            CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day2", ValueSourceType.Function, GroupByObjectField.SumFunction));
            CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day3", ValueSourceType.Function, GroupByObjectField.SumFunction));
            CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day4", ValueSourceType.Function, GroupByObjectField.SumFunction));
            CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day5", ValueSourceType.Function, GroupByObjectField.SumFunction));
            CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day6", ValueSourceType.Function, GroupByObjectField.SumFunction));
            CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day7", ValueSourceType.Function, GroupByObjectField.SumFunction));
            CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("DayT", ValueSourceType.Function, GroupByObjectField.SumFunction));

            CurrentView.SecondaryGroupBy = new GroupByElement("ParentBlockId");
            CurrentView.SecondaryGroupBy.IsPreGroupObjectVisible = true;
            CurrentView.SecondaryGroupBy.IsPostGroupObjectVisible = false;
            CurrentView.SecondaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Title", ValueSourceType.Field, "ParentBlock", false));
            CurrentView.SecondaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("ParentBlockId", ValueSourceType.Field, "ParentBlockId"));
            CurrentView.SecondaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("StateFriendlyName", ValueSourceType.Field, "StateFriendlyName", true));
            CurrentView.SecondaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day1", ValueSourceType.Function, GroupByObjectField.SumFunction));
            CurrentView.SecondaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day2", ValueSourceType.Function, GroupByObjectField.SumFunction));
            CurrentView.SecondaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day3", ValueSourceType.Function, GroupByObjectField.SumFunction));
            CurrentView.SecondaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day4", ValueSourceType.Function, GroupByObjectField.SumFunction));
            CurrentView.SecondaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day5", ValueSourceType.Function, GroupByObjectField.SumFunction));
            CurrentView.SecondaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day6", ValueSourceType.Function, GroupByObjectField.SumFunction));
            CurrentView.SecondaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day7", ValueSourceType.Function, GroupByObjectField.SumFunction));
            CurrentView.SecondaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("DayT", ValueSourceType.Function, GroupByObjectField.SumFunction));

        }
        #endregion

        #region ManagerGroupByWeekUser
        void ManagerGroupByWeekUser()
        {
            CurrentView.PrimaryGroupBy = new GroupByElement("StartDate");
            CurrentView.PrimaryGroupBy.IsPreGroupObjectVisible = true;
            CurrentView.PrimaryGroupBy.IsPostGroupObjectVisible = false;

            CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Title", ValueSourceType.Field, "ParentBlock", false));
            CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day1", ValueSourceType.Function, GroupByObjectField.SumFunction));
            CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day2", ValueSourceType.Function, GroupByObjectField.SumFunction));
            CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day3", ValueSourceType.Function, GroupByObjectField.SumFunction));
            CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day4", ValueSourceType.Function, GroupByObjectField.SumFunction));
            CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day5", ValueSourceType.Function, GroupByObjectField.SumFunction));
            CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day6", ValueSourceType.Function, GroupByObjectField.SumFunction));
            CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day7", ValueSourceType.Function, GroupByObjectField.SumFunction));
            CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("DayT", ValueSourceType.Function, GroupByObjectField.SumFunction));

            CurrentView.SecondaryGroupBy = new GroupByElement("OwnerId");
            CurrentView.SecondaryGroupBy.IsPreGroupObjectVisible = true;
            CurrentView.SecondaryGroupBy.IsPostGroupObjectVisible = false;
            CurrentView.SecondaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Title", ValueSourceType.Field, "ParentBlock", false));
            CurrentView.SecondaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("ParentBlockId", ValueSourceType.Field, "ParentBlockId"));
            CurrentView.SecondaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("StateFriendlyName", ValueSourceType.Field, "StateFriendlyName", true));
            CurrentView.SecondaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day1", ValueSourceType.Function, GroupByObjectField.SumFunction));
            CurrentView.SecondaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day2", ValueSourceType.Function, GroupByObjectField.SumFunction));
            CurrentView.SecondaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day3", ValueSourceType.Function, GroupByObjectField.SumFunction));
            CurrentView.SecondaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day4", ValueSourceType.Function, GroupByObjectField.SumFunction));
            CurrentView.SecondaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day5", ValueSourceType.Function, GroupByObjectField.SumFunction));
            CurrentView.SecondaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day6", ValueSourceType.Function, GroupByObjectField.SumFunction));
            CurrentView.SecondaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day7", ValueSourceType.Function, GroupByObjectField.SumFunction));
            CurrentView.SecondaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("DayT", ValueSourceType.Function, GroupByObjectField.SumFunction));

        }
        #endregion

        #region CurrentProjectGroupByWeekUser
        void CurrentProjectGroupByWeekUser()
        {
            CurrentView.PrimaryGroupBy = new GroupByElement("StartDate");
            CurrentView.PrimaryGroupBy.IsPreGroupObjectVisible = true;
            CurrentView.PrimaryGroupBy.IsPostGroupObjectVisible = false;

            CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Title", ValueSourceType.Field, "ParentBlock", false));
            CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day1", ValueSourceType.Function, GroupByObjectField.SumFunction));
            CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day2", ValueSourceType.Function, GroupByObjectField.SumFunction));
            CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day3", ValueSourceType.Function, GroupByObjectField.SumFunction));
            CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day4", ValueSourceType.Function, GroupByObjectField.SumFunction));
            CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day5", ValueSourceType.Function, GroupByObjectField.SumFunction));
            CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day6", ValueSourceType.Function, GroupByObjectField.SumFunction));
            CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day7", ValueSourceType.Function, GroupByObjectField.SumFunction));
            CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("DayT", ValueSourceType.Function, GroupByObjectField.SumFunction));

            CurrentView.SecondaryGroupBy = new GroupByElement("OwnerId");
            CurrentView.SecondaryGroupBy.IsPreGroupObjectVisible = true;
            CurrentView.SecondaryGroupBy.IsPostGroupObjectVisible = false;
            CurrentView.SecondaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Title", ValueSourceType.Field, "ParentBlock", false));
            CurrentView.SecondaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("ParentBlockId", ValueSourceType.Field, "ParentBlockId"));
            CurrentView.SecondaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("StateFriendlyName", ValueSourceType.Field, "StateFriendlyName", true));
            CurrentView.SecondaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day1", ValueSourceType.Function, GroupByObjectField.SumFunction));
            CurrentView.SecondaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day2", ValueSourceType.Function, GroupByObjectField.SumFunction));
            CurrentView.SecondaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day3", ValueSourceType.Function, GroupByObjectField.SumFunction));
            CurrentView.SecondaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day4", ValueSourceType.Function, GroupByObjectField.SumFunction));
            CurrentView.SecondaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day5", ValueSourceType.Function, GroupByObjectField.SumFunction));
            CurrentView.SecondaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day6", ValueSourceType.Function, GroupByObjectField.SumFunction));
            CurrentView.SecondaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day7", ValueSourceType.Function, GroupByObjectField.SumFunction));
            CurrentView.SecondaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("DayT", ValueSourceType.Function, GroupByObjectField.SumFunction));

        }
        #endregion

        #region MyGroupByWeekProject
        void MyGroupByWeekProject()
        {
            //CurrentView.PrimaryGroupBy = new GroupByElement("StartDate");
            //CurrentView.PrimaryGroupBy.IsPreGroupObjectVisible = true;
            //CurrentView.PrimaryGroupBy.IsPostGroupObjectVisible = false;

            //CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Title", ValueSourceType.Field, "ParentBlock", false));
            //CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day1", ValueSourceType.Function, GroupByObjectField.SumFunction));
            //CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day2", ValueSourceType.Function, GroupByObjectField.SumFunction));
            //CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day3", ValueSourceType.Function, GroupByObjectField.SumFunction));
            //CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day4", ValueSourceType.Function, GroupByObjectField.SumFunction));
            //CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day5", ValueSourceType.Function, GroupByObjectField.SumFunction));
            //CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day6", ValueSourceType.Function, GroupByObjectField.SumFunction));
            //CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day7", ValueSourceType.Function, GroupByObjectField.SumFunction));
            //CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("DayT", ValueSourceType.Function, GroupByObjectField.SumFunction));

            CurrentView.PrimaryGroupBy = new GroupByElement("ParentBlockId");
            CurrentView.PrimaryGroupBy.IsPreGroupObjectVisible = true;
            CurrentView.PrimaryGroupBy.IsPostGroupObjectVisible = false;
            CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Title", ValueSourceType.Field, "ParentBlock", false));
            CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("ParentBlockId", ValueSourceType.Field, "ParentBlockId"));
            CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("BlockTypeInstanceId", ValueSourceType.Field, "BlockTypeInstanceId"));
            CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("StateFriendlyName", ValueSourceType.Field, "StateFriendlyName", true));
            CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day1", ValueSourceType.Function, GroupByObjectField.SumFunction));
            CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day2", ValueSourceType.Function, GroupByObjectField.SumFunction));
            CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day3", ValueSourceType.Function, GroupByObjectField.SumFunction));
            CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day4", ValueSourceType.Function, GroupByObjectField.SumFunction));
            CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day5", ValueSourceType.Function, GroupByObjectField.SumFunction));
            CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day6", ValueSourceType.Function, GroupByObjectField.SumFunction));
            CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day7", ValueSourceType.Function, GroupByObjectField.SumFunction));
            CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("DayT", ValueSourceType.Function, GroupByObjectField.SumFunction));

            CurrentView.SecondaryGroupBy = null;
            //CurrentPreferences.ChangeMetaFieldOrder(1, 9);
            //Mediachase.Ibn.Core.UserMetaViewPreference.Save(65, CurrentPreferences);
        }
        #endregion

        #region MyRejectedGroupByWeekProject
        void MyRejectedGroupByWeekProject()
        {
            //CurrentView.PrimaryGroupBy = new GroupByElement("StartDate");
            //CurrentView.PrimaryGroupBy.IsPreGroupObjectVisible = true;
            //CurrentView.PrimaryGroupBy.IsPostGroupObjectVisible = false;

            //CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Title", ValueSourceType.Field, "ParentBlock", false));
            //CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day1", ValueSourceType.Function, GroupByObjectField.SumFunction));
            //CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day2", ValueSourceType.Function, GroupByObjectField.SumFunction));
            //CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day3", ValueSourceType.Function, GroupByObjectField.SumFunction));
            //CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day4", ValueSourceType.Function, GroupByObjectField.SumFunction));
            //CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day5", ValueSourceType.Function, GroupByObjectField.SumFunction));
            //CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day6", ValueSourceType.Function, GroupByObjectField.SumFunction));
            //CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day7", ValueSourceType.Function, GroupByObjectField.SumFunction));
            //CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("DayT", ValueSourceType.Function, GroupByObjectField.SumFunction));

            CurrentView.PrimaryGroupBy = new GroupByElement("ParentBlockId");
            CurrentView.PrimaryGroupBy.IsPreGroupObjectVisible = true;
            CurrentView.PrimaryGroupBy.IsPostGroupObjectVisible = false;
            CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Title", ValueSourceType.Field, "ParentBlock", false));
            CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("ParentBlockId", ValueSourceType.Field, "ParentBlockId"));
            CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("BlockTypeInstanceId", ValueSourceType.Field, "BlockTypeInstanceId"));
            CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("StateFriendlyName", ValueSourceType.Field, "StateFriendlyName", true));
            CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day1", ValueSourceType.Function, GroupByObjectField.SumFunction));
            CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day2", ValueSourceType.Function, GroupByObjectField.SumFunction));
            CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day3", ValueSourceType.Function, GroupByObjectField.SumFunction));
            CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day4", ValueSourceType.Function, GroupByObjectField.SumFunction));
            CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day5", ValueSourceType.Function, GroupByObjectField.SumFunction));
            CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day6", ValueSourceType.Function, GroupByObjectField.SumFunction));
            CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day7", ValueSourceType.Function, GroupByObjectField.SumFunction));
            CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("DayT", ValueSourceType.Function, GroupByObjectField.SumFunction));

            CurrentView.SecondaryGroupBy = null;
        }
        #endregion

        #region MyGroupByWeek
        void MyGroupByWeek()
        {
            CurrentView.PrimaryGroupBy = new GroupByElement("StartDate");
            CurrentView.PrimaryGroupBy.IsPreGroupObjectVisible = true;
            CurrentView.PrimaryGroupBy.IsPostGroupObjectVisible = false;

            CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Title", ValueSourceType.Field, "ParentBlock", false));
            CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("ParentBlockId", ValueSourceType.Field, "ParentBlockId"));
            CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("StateFriendlyName", ValueSourceType.Field, "StateFriendlyName", true));
            //CurrentView.PrimaryGroupBy.PostGroupObjectFields.Add(new GroupByObjectField("Day1", ValueSourceType.Function, GroupByObjectField.SumFunction));
            //CurrentView.PrimaryGroupBy.PostGroupObjectFields.Add(new GroupByObjectField("Day2", ValueSourceType.Function, GroupByObjectField.SumFunction));
            //CurrentView.PrimaryGroupBy.PostGroupObjectFields.Add(new GroupByObjectField("Day3", ValueSourceType.Function, GroupByObjectField.SumFunction));
            //CurrentView.PrimaryGroupBy.PostGroupObjectFields.Add(new GroupByObjectField("Day4", ValueSourceType.Function, GroupByObjectField.SumFunction));
            //CurrentView.PrimaryGroupBy.PostGroupObjectFields.Add(new GroupByObjectField("Day5", ValueSourceType.Function, GroupByObjectField.SumFunction));
            //CurrentView.PrimaryGroupBy.PostGroupObjectFields.Add(new GroupByObjectField("Day6", ValueSourceType.Function, GroupByObjectField.SumFunction));
            //CurrentView.PrimaryGroupBy.PostGroupObjectFields.Add(new GroupByObjectField("Day7", ValueSourceType.Function, GroupByObjectField.SumFunction));		
        }
        #endregion
    }
}
