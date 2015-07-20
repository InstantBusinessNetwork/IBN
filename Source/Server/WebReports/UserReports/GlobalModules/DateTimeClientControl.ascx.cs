namespace Mediachase.UI.Web.UserReports.GlobalModules
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Globalization;
	using System.Resources;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using Mediachase.IBN.Business;

	/// <summary>
	///		Summary description for DateTimeClientControl.
	/// </summary>
	public partial class DateTimeClientControl : System.Web.UI.UserControl
	{




		//protected System.Web.UI.HtmlControls.HtmlTableRow trDays1;
		//protected System.Web.UI.HtmlControls.HtmlTableRow trDays2;
		//protected System.Web.UI.HtmlControls.HtmlTableRow trDays3;
		//protected System.Web.UI.HtmlControls.HtmlTableRow trDays4;
		//protected System.Web.UI.HtmlControls.HtmlTableRow trDays5;
		//protected System.Web.UI.HtmlControls.HtmlTableRow trDays6;

		private int max_length_MonFN;
		private int max_length_WDsSN;

    ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.UserReports.GlobalModules.Resources.strDateTimeControl", typeof(DateTimeClientControl).Assembly);

		#region Path_File_JS
		private string path_js = "";
		public string Path_JS
		{
			set
			{
				string ValueToSet = value;
				if( ValueToSet.Length > 0 )
				{
					if(ValueToSet.Substring(ValueToSet.Length-1,1) != "/") ValueToSet += "/";
				}
				path_js = ValueToSet;
			}
			get
			{
				return path_js;
			}
		}

		private string file_js = "mcCalendar.js";
		public string File_JS
		{
			set
			{
				file_js = value;
			}
			get
			{
				return file_js;
			}
		}

		private bool check_exists_js_file = true;
		public bool Check_Exists_JS_File
		{
			set
			{
				check_exists_js_file = value;
			}
			get
			{
				return check_exists_js_file;
			}
		}
		#endregion

		#region Path_Img
		private string path_img = "test_files/";
		public string Path_Img
		{
			set
			{
				string ValueToSet = value;
				if( ValueToSet.Length > 0 )
				{
					if(ValueToSet.Substring(ValueToSet.Length-1,1) != "/") ValueToSet += "/";
				}
				path_img = ValueToSet;
			}
			get
			{
				return path_img;
			}
		}
		#endregion

		#region Calend_Culture
		private string strCalendCulture = CultureInfo.CurrentCulture.ToString();
		public string Calend_Culture
		{
			set
			{
				strCalendCulture = CultureInfo.CreateSpecificCulture(value).Name;
			}
			get
			{
				return strCalendCulture;
			}
		}
		#endregion

		#region FirstDayWeek
		private int firstdayweek = Company.GetCompanyFirstDayOfWeek();//-1;
		public int FirstDayWeek
		{
			set
			{
				firstdayweek = value;
			}
			get
			{
				return firstdayweek;
			}
		}
		#endregion

		#region SelectionDateWeek
		private bool selectionDateWeek = false;
		public bool SelectionDateWeek
		{
			set
			{
				selectionDateWeek = value;
			}
			get
			{
				return selectionDateWeek;
			}
		}
		//
		public enum E_SelectionDateType 
		{
			Day,
			Week
		}
		private E_SelectionDateType selectiondatetype;
		public E_SelectionDateType SelectionDateType
		{
			set
			{
				selectiondatetype = value;
				if( selectiondatetype.GetHashCode() == 1 ) selectionDateWeek = true;
			}
			get
			{
				return selectiondatetype;
			}
		}
		//
		#endregion

		#region YearWeekWrap
		private bool yearweekwrap = true;
		public bool YearWeekWrap
		{
			set
			{
				yearweekwrap = value;
			}
			get
			{
				return yearweekwrap;
			}
		}
		#endregion

		#region Drop_gAlign
		private int i_drop_galign = 2;
		public enum E_Drop_gAlign
		{
			Left,
			Center,
			Right,
			left,
			center,
			right
		}
		private E_Drop_gAlign drop_galign;
		public E_Drop_gAlign Drop_gAlign
		{
			set
			{
				drop_galign = value;
				i_drop_galign = (drop_galign.GetHashCode())%3;
			}
			get
			{
				return drop_galign;
			}
		}
		#endregion

		#region AutoPostBack
		private bool autopostback = false;
		public bool AutoPostBack
		{
			set
			{
				autopostback = value;
			}
			get
			{
				return autopostback;
			}
		}

		private string postbackcontrolid;
		public string PostBackControlId
		{
			set
			{
				autopostback = true;
				postbackcontrolid = value;
			}
			get
			{
				return postbackcontrolid;
			}
		}

		private bool postbackoldvalue = false;
		public bool PostBackOldValue
		{
			set
			{
				postbackoldvalue = value;
			}
			get
			{
				return postbackoldvalue;
			}
		}
		#endregion

		#region View2Rows
		private bool view2rows = false;
		public bool View2Rows
		{
			set
			{
				view2rows = value;
			}
			get
			{
				return view2rows;
			}
		}
		#endregion

		#region btnImgResetVisible
		public bool btnImgResetVisible
		{
			set
			{
				tdBtnImgReset.Visible = value;
				btnImgReset.Visible = value;
			}
			get
			{
				return btnImgReset.Visible;
			}
		}
		#endregion

		#region Enabled
		private bool enabled = true;
		public bool Enabled
		{
			set
			{
				enabled = value;
				txtDtInt.Enabled = value;
				btnMain.Style.Remove("cursor");
				if(value) btnMain.Style.Add("cursor","pointer;cursor:hand");
				btnMain.Disabled = !value;
				//showtime
				selDtHours.Enabled = value;
				selDtMinutes.Enabled = value;
				selDtSeconds.Enabled = value;
				//spin
				txtSpinHours.Enabled = value;
				txtSpinMinutes.Enabled = value;
				txtSpinSeconds.Enabled = value;

				btnImgSpinUp.Disabled = !value;
				btnImgSpinUp.Style.Remove("cursor");
				btnImgSpinUp.Attributes.Remove("onclick");
				if(value)
				{
					btnImgSpinUp.Style.Add("cursor","pointer;cursor:hand");
					btnImgSpinUp.Attributes.Add ("onclick", "javascript:mcCalend_FimgSpin('"+dvControl.ClientID+"',0);" );
				}

				btnImgSpinDown.Disabled = !value;
				btnImgSpinDown.Style.Remove("cursor");
				btnImgSpinDown.Attributes.Remove("onclick");
				if(value)
				{
					btnImgSpinDown.Style.Add("cursor","pointer;cursor:hand");
					btnImgSpinDown.Attributes.Add ("onclick", "javascript:mcCalend_FimgSpin('"+dvControl.ClientID+"',1);" );
				}
				//spin end
				//showtime end
				btnImgReset.Style.Remove("cursor");
				if(value) btnImgReset.Style.Add("cursor","pointer;cursor:hand");
				btnImgReset.Disabled = !value;
			}
			get
			{
				return enabled;
			}
		}
		#endregion

		#region ShowTime
		public enum E_ShowTime 
		{
			None,
			H,
			HM,
			HMS
		}
		private bool showtime = false;
		private bool showtime_hours = false;
		private bool showtime_minutes = false;
		private bool showtime_seconds = false;
		private E_ShowTime showtime_value;
		public E_ShowTime ShowTime
		{
			set
			{
				showtime_value = value;
				if( showtime_value.GetHashCode() > 0 )
				{
					showtime = true;
					showtime_hours = true;
					if( showtime_value.GetHashCode() > 1 ) showtime_minutes = true;
					if( showtime_value.GetHashCode() > 2 ) showtime_seconds = true;
				}
			}
			get
			{
				return showtime_value;
			}
		}

		#region ShowDate
		private bool showdate = true;
		public bool ShowDate
		{
			set
			{
				showdate = value;
				txtDtInt.Visible = value;
				btnMain.Visible = value;
				dvMain.Visible = value;
				reqtxtDtInt.Visible = value;
			}
			get
			{
				return showdate;
			}
		}
		#endregion

		private int minutes_step = 1;
		public int Minutes_Step
		{
			set
			{
				if( value >=0 && value <60 && 60%value == 0)
					minutes_step = value;
			}
			get
			{
				return minutes_step;
			}
		}

		private int seconds_step = 1;
		public int Seconds_Step
		{
			set
			{
				if( value >=0 && value <60 && 60%value == 0)
					seconds_step = value;
			}
			get
			{
				return seconds_step;
			}
		}

		private bool timespin = false;
		public bool TimeSpin
		{
			set
			{
				timespin = value;
			}
			get
			{
				return timespin;
			}
		}

		private bool hourspinunlimited = false;
		public bool HourSpinUnLimited
		{
			set
			{
				if(value)hourspinmaxvalue=0;
				hourspinunlimited = value;
			}
			get
			{
				return hourspinunlimited;
			}
		}

		private int hourspinmaxvalue = 24;
		public int HourSpinMaxValue
		{
			set
			{
				if(value>0)
				{
					this.HourSpinUnLimited = false;
					hourspinmaxvalue = value;
				}
			}
			get
			{
				return hourspinmaxvalue;
			}
		}
		#endregion

		#region ControlViewType
		public enum E_ControlViewType
		{
			Default,
			NoDateBox,
			OnlyTime,
			NoButton,
			StaticCalendar
		}

		private bool controlviewtype_onlytime = false;
		private bool controlviewtype_staticcalendar = false;

		private E_ControlViewType controlviewtype_value;
		public E_ControlViewType ControlViewType
		{
			set
			{
				controlviewtype_value = value;
				if( controlviewtype_value.GetHashCode() == 1 )
				{
					//NoDateBox
					txtDtInt.Visible = false;//1
					reqtxtDtInt.Visible = false;//5
					txtDtInt.Parent.Visible = false;
					OnChangeValidator.Visible = false;//6
					OnChangeValidator.Parent.Visible = false;
				}
				if( controlviewtype_value.GetHashCode() == 2 )
				{
					//OnlyTime
					controlviewtype_onlytime = true;
					txtDtInt.Visible = false;//1
					txtDtInt.Parent.Visible = false;
					btnMain.Visible = false;//2
					btnMain.Parent.Visible = false;
					spSelTimePad.Visible = false;
					tdSpinPad.Visible = false;
					dvMain.Visible = false;//3
					reqtxtDtInt.Visible = false;//5
					OnChangeValidator.Visible = false;//6
					OnChangeValidator.Parent.Visible = false;
					tdtxtHiddens.Visible = false;
				}
				if( controlviewtype_value.GetHashCode() == 3 )
				{
					//NoButton
					btnMain.Visible = false;//2
					btnMain.Parent.Visible = false;
					dvMain.Style.Add("display","block");//3
					dvMain.Style.Add("position","relative");//3
					dvMain.Style.Remove("z-index");//3
					controlviewtype_staticcalendar = true;
				}
				if( controlviewtype_value.GetHashCode() == 4 )
				{
					//StaticCalendar
					txtDtInt.Visible = false;//1
					txtDtInt.Parent.Visible = false;
					btnMain.Visible = false;//2
					btnMain.Parent.Visible = false;
					dvMain.Style.Add("display","block");//3
					dvMain.Style.Add("position","relative");//3
					dvMain.Style.Remove("z-index");//3
					reqtxtDtInt.Visible = false;//5
					OnChangeValidator.Visible = false;//6
					OnChangeValidator.Parent.Visible = false;
					controlviewtype_staticcalendar = true;
				}
			}
			get
			{
				return controlviewtype_value;
			}
		}
		#endregion

		#region strTextBoxWidth
		private string strtextboxwidth;
		public string strTextBoxWidth
		{
			set
			{
				strtextboxwidth = value;
			}
			get
			{
				return strtextboxwidth;
			}
		}
		#endregion

		#region strElsBGColor
		private string strelsbgcolor = null;
		public string strElsBGColor
		{
			set
			{
				strelsbgcolor = value;
			}
			get
			{
				return strelsbgcolor;
			}
		}
		#endregion

		#region SpanClass
		private string strspanclass = "text";
		public string SpanClass
		{
			set
			{
				strspanclass = value;
			}
			get
			{
				return strspanclass;
			}
		}
		#endregion

		protected System.Web.UI.WebControls.RequiredFieldValidator Requiredfieldvalidator1;

		#region PostBackInit
		private bool postbackinit = false;
		public bool PostBackInit
		{
			set
			{
				postbackinit = value;
			}
			get
			{
				return postbackinit;
			}
		}
		#endregion

		private bool value_is_changed = false;
		//private DateTime dtcStartDate = System.DateTime.Now;
		private DateTime dtcStartDate = Mediachase.IBN.Business.User.GetLocalDate(DateTime.UtcNow);
		public DateTime StartDate 
		{
			set
			{
				viewstartdate = true;
				dtcStartDate = value;
				value_is_changed = true;
			}
			get
			{
				return dtcStartDate;
			}
		}

		#region Value
		public DateTime Value
		{
			set
			{
				//SetDate(value);
				StartDate = value;
			}
			get
			{
				/*
				DateTime retVal = new DateTime(int.Parse(txtYear.Value.ToString()),1+int.Parse(txtMonth.Value.ToString()),int.Parse(txtDay.Value.ToString()));
				if(showtime)
				{
					if(showtime_hours) retVal = retVal.AddHours(selDtHours.SelectedIndex);
					if(showtime_minutes) retVal = retVal.AddMinutes(int.Parse(selDtMinutes.SelectedItem.Value));
					if(showtime_seconds) retVal = retVal.AddSeconds(int.Parse(selDtSeconds.SelectedItem.Value));
				}
				return retVal;
				*/
				DateTime retVal = DateTime.MinValue;
				if(IsPostBack)
				{
					if( txtDtInt.Text != "" || !txtDtInt.Visible ) retVal = SelectedDayValue;
				}
				else
				{
					retVal = SelectedDayValue;
				}
				return retVal;
			}
		}
		#endregion

		#region ViewStartDate
		private bool viewstartdate = false;
		public bool ViewStartDate
		{
			set
			{
				viewstartdate = value;
			}
			get
			{
				return viewstartdate;
			}
		}
		#endregion

		public DateTime SelectedDayValue 
		{
			get
			{
				DateTime retVal;
				if(IsPostBack)
				{
					retVal = new DateTime(int.Parse(txtYear.Value.ToString()),1+int.Parse(txtMonth.Value.ToString()),int.Parse(txtDay.Value.ToString()));
					if(controlviewtype_onlytime) retVal = DateTime.MinValue;
					if(showtime)
					{
						if(showtime_hours)
						{
							if(timespin)
							{
								try
								{
									retVal = retVal.AddHours(int.Parse(txtSpinHours.Text));
								}
								catch(Exception)
								{
								}
							}
							else retVal = retVal.AddHours(selDtHours.SelectedIndex);
						}
						if(showtime_minutes)
						{
							if(timespin)
							{
								try
								{
									retVal = retVal.AddMinutes(int.Parse(txtSpinMinutes.Text));
								}
								catch(Exception)
								{
								}
							}
							else retVal = retVal.AddMinutes(int.Parse(selDtMinutes.SelectedItem.Value));
						}
						if(showtime_seconds)
						{
							if(timespin)
							{
								try
								{
									retVal = retVal.AddSeconds(int.Parse(txtSpinSeconds.Text));
								}
								catch(Exception)
								{
								}
							}
							else retVal = retVal.AddSeconds(int.Parse(selDtSeconds.SelectedItem.Value));
						}
					}
				}
				else retVal=dtcStartDate;
				return retVal;
			}
		}

		public DateTime BeginDayValue 
		{
			get
			{
				DateTime retVal = new DateTime(int.Parse(txtYear0.Value.ToString()),1+int.Parse(txtMonth0.Value.ToString()),int.Parse(txtDay0.Value.ToString()));
				return retVal;
			}
		}

		public DateTime EndDayValue 
		{
			get
			{
				DateTime retVal = new DateTime(int.Parse(txtYear1.Value.ToString()),1+int.Parse(txtMonth1.Value.ToString()),int.Parse(txtDay1.Value.ToString()));
				return retVal;
			}
		}

		protected string sRelatedControlName = "";
		public string RelatedControlName
		{
			get
			{
				return sRelatedControlName;
			}
			set
			{
				sRelatedControlName = value;
			}
		}

		#region Hour
		public DropDownList Hour
		{
			get
			{
				return selDtHours;
			}
		}
		#endregion

		#region Minute
		public DropDownList Minute
		{
			get
			{
				return selDtMinutes;
			}
		}
		#endregion

		#region Seconds
		public DropDownList Seconds
		{
			get
			{
				return selDtSeconds;
			}
		}
		#endregion

		#region Date
		public TextBox Date
		{
			get
			{
				return txtDtInt;
			}
		}
		#endregion

		#region RequiredFieldSettings
		private string reqvalidatorerror = "*";
		public string ReqValidatorError
		{
			get
			{
				return reqvalidatorerror;
			}
			set
			{
				reqvalidatorerror = value;
				reqtxtDtInt.ErrorMessage = reqvalidatorerror;
			}
		}

		private bool requiredfield = true;
		public bool RequiredField
		{
			set 
			{
				requiredfield = value;
				reqtxtDtInt.Enabled = value;
			}
			get
			{
				return requiredfield;
			}
		}
		#endregion

		#region OnChangeValidatorSettings
		private string onchangevalidatorerror = "";
		public string OnChangeValidatorError
		{
			get
			{
				return onchangevalidatorerror;
			}
			set
			{
				onchangevalidatorerror = value;
				OnChangeValidator.ErrorMessage = onchangevalidatorerror;
			}
		}
		#endregion

		#region SpinValidatorSettings
		private string spinvalidatorerror = "";
		public string SpinValidatorError
		{
			get
			{
				return spinvalidatorerror;
			}
			set
			{
				spinvalidatorerror = value;
				SpinValidator.ErrorMessage = spinvalidatorerror;
			}
		}
		#endregion

		#region TextCanChange
		private bool textcanchange = true;
		public bool TextCanChange
		{
			set
			{
				textcanchange = value;
			}
			get
			{
				return textcanchange;
			}
		}
		#endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
			//Set TextBox width
			if(strtextboxwidth == null )
			{
				if(selectionDateWeek) strtextboxwidth = "150px";
				else  strtextboxwidth = "75px";
			}
			txtDtInt.Style.Add("width",strtextboxwidth);
			//disabling control if needed
			
			//set reqvalidate empty error message
			if(textcanchange && !selectionDateWeek)
			{
				String strOnChangeExecuteCode = "javascript:mcCalend_FdateOnChange('"+dvControl.ClientID+"',this);";
				//if(sRelatedControlName != "")
				//	strOnChangeExecuteCode += "if(document.getElementById('"+sRelatedControlName+"')!=null)document.getElementById('"+sRelatedControlName+"').value=this.value;";
					//txtDtInt.Attributes.Add("onchange","if(document.getElementById('"+sRelatedControlName+"')!=null)document.getElementById('"+sRelatedControlName+"').value=this.value;");
				txtDtInt.Attributes.Add( "onchange", strOnChangeExecuteCode);
				txtDtInt.Attributes.Add( "onclick", "javascript:mcCalend_FGetById('"+txtDtInt.ClientID+"').focus();");
				txtDtInt.Attributes.Add( "onkeydown", "javascript:mcCalend_FdateOnKeyDown('"+dvControl.ClientID+"',this,event);");
			}
			else
			{
				txtDtInt.ReadOnly = true;
				OnChangeValidator.Visible = false;
				OnChangeValidator.Enabled = false;
			}
			if(selectionDateWeek && yearweekwrap)
				txtFirstDW.Attributes.Add("mcCalend_AWrap","1");
			// show/view tr/td
			if(!view2rows) trYear.Visible=false;
			if(!showtime)
			{
				tdSelHMS.Visible=false;
				tdSpinHMS.Visible=false;
				SpinValidator.Visible = false;
				SpinValidator.Enabled = false;
				tdBtnsImgSpin.Visible=false;
			}
			else
			{
				tdSelHMS.Visible=true;
				tdSpinHMS.Visible=true;
				SpinValidator.Visible = true;
				SpinValidator.Enabled = true;
				tdBtnsImgSpin.Visible=true;
				if(timespin)
					tdSelHMS.Visible=false;
				else
				{
					tdSpinHMS.Visible=false;
					tdBtnsImgSpin.Visible=false;
				}
			}
			//inserting js file
			string ControlCurDir = "";
			bool JS_File_Exists = true;
			if(check_exists_js_file)
			{
				ControlCurDir = this.Page.MapPath("");
				if(ControlCurDir.Length > 0)
				{
					if( ControlCurDir.Substring(ControlCurDir.Length-1,1) != "\\" ) ControlCurDir += "\\";
				}
				JS_File_Exists = System.IO.File.Exists( ControlCurDir + path_js.Replace("/","\\") + file_js );
			}
			if( JS_File_Exists )
			{
				if(!Page.ClientScript.IsClientScriptBlockRegistered("mcCalendar_js"))
					Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "mcCalendar_js","<script language=\"javascript\" type=\"text/javascript\" src=\"" + path_js + file_js + "\"></script>");
				if(controlviewtype_staticcalendar)
					Page.ClientScript.RegisterStartupScript(this.GetType(), "paint"+dvControl.ClientID,"<script language=\"javascript\">mcCalend_FStaticPaint('"+dvControl.ClientID+"');</script>");
			}
			//attaching onclick events to images
			if(autopostback)
			{
				System.Web.UI.Control cntrlPostBack = this;
				if( postbackcontrolid != null)
				{
					if( FindControl(Page,postbackcontrolid) != null)
					{
						cntrlPostBack = FindControl(Page,postbackcontrolid);
					}
				}
				dvControl.Attributes.Add( "mcCalend_doPostBack", Page.ClientScript.GetPostBackEventReference(cntrlPostBack,"") + ";");
				selDtHours.Attributes.Add( "onchange", "javascript:" + Page.ClientScript.GetPostBackEventReference(cntrlPostBack,"") + ";");
				selDtMinutes.Attributes.Add( "onchange", "javascript:" + Page.ClientScript.GetPostBackEventReference(cntrlPostBack,"") + ";");
				selDtSeconds.Attributes.Add( "onchange", "javascript:" + Page.ClientScript.GetPostBackEventReference(cntrlPostBack,"") + ";");
				txtSpinHours.Attributes.Add( "onchange", "javascript:" + Page.ClientScript.GetPostBackEventReference(cntrlPostBack,"") + ";");
				txtSpinMinutes.Attributes.Add( "onchange", "javascript:" + Page.ClientScript.GetPostBackEventReference(cntrlPostBack,"") + ";");
				txtSpinSeconds.Attributes.Add( "onchange", "javascript:" + Page.ClientScript.GetPostBackEventReference(cntrlPostBack,"") + ";");
				if(postbackoldvalue)
					dvControl.Attributes.Add( "mcCalend_PostBackOldValue", "1");
			}
			btnMain.Attributes.Add( "onclick", "javascript:mcCalend_FbtnImgMain('"+dvControl.ClientID+"','"+this.btnMain.ClientID+"','"+this.dvMain.ClientID+"');" );
			if( !JS_File_Exists )
				btnMain.Attributes.Add( "onclick", "javascript:alert('JScript file \"" + ControlCurDir.Replace("\\","/") + path_js + file_js + "\" not found!');" );
			if (!this.IsPostBack || PostBackInit)
			{
				if(selectionDateWeek) btnMain.Src = "mcCalendar_w.gif";
				btnMain.Src = path_img + btnMain.Src;
			}
			btnImgDecMonth.Attributes.Add ("onclick", "javascript:mcCalend_FdecMonth('"+dvControl.ClientID+"');" );
			imgArrowsAttachEvents(btnImgDecMonth, "#b8d7f5", "#8eb8e8" );
			if (!this.IsPostBack || PostBackInit)btnImgDecMonth.Src = path_img + btnImgDecMonth.Src;

			btnImgIncMonth.Attributes.Add ("onclick", "javascript:mcCalend_FincMonth('"+dvControl.ClientID+"');" );
			imgArrowsAttachEvents(btnImgIncMonth, "#b8d7f5", "#8eb8e8" );
			if (!this.IsPostBack || PostBackInit)btnImgIncMonth.Src = path_img + btnImgIncMonth.Src;

			btnImgDecYear.Attributes.Add ("onclick", "javascript:mcCalend_FdecYear('"+dvControl.ClientID+"');" );
			imgArrowsAttachEvents(btnImgDecYear, "#bedafa", "#e6f3ff" );
			if (!this.IsPostBack || PostBackInit)btnImgDecYear.Src = path_img + btnImgDecYear.Src;

			btnImgIncYear.Attributes.Add ("onclick", "javascript:mcCalend_FincYear('"+dvControl.ClientID+"');" );
			imgArrowsAttachEvents(btnImgIncYear, "#bedafa", "#e6f3ff" );
			if (!this.IsPostBack || PostBackInit)btnImgIncYear.Src = path_img + btnImgIncYear.Src;

			btnImgReset.Attributes.Add ("onclick", "javascript:mcCalend_ClearSelection('"+dvControl.ClientID+"');" );
			if (!this.IsPostBack || PostBackInit)btnImgReset.Src = path_img + btnImgReset.Src;

			//spin
			if (!this.IsPostBack || PostBackInit)
			{
				btnImgSpinUp.Src = path_img + btnImgSpinUp.Src;
				btnImgSpinDown.Src = path_img + btnImgSpinDown.Src;
			}
			txtSpinHours.Attributes.Add("onclick","javascript:mcCalend_FspinOnClick();" );
			txtSpinMinutes.Attributes.Add("onclick","javascript:mcCalend_FspinOnClick();" );
			txtSpinSeconds.Attributes.Add("onclick","javascript:mcCalend_FspinOnClick();" );

			//txtSpinHours.Attributes.Add( "onkeydown", "javascript:mcCalend_FspinOnKeyDown('"+dvControl.ClientID+"',this,event);");
			//txtSpinMinutes.Attributes.Add( "onkeydown", "javascript:mcCalend_FspinOnKeyDown('"+dvControl.ClientID+"',this,event);");
			//txtSpinSeconds.Attributes.Add( "onkeydown", "javascript:mcCalend_FspinOnKeyDown('"+dvControl.ClientID+"',this,event);");

			txtSpinHours.Attributes.Add( "onkeydown", "javascript:mcCalend_FspinOnKeyDown('"+dvControl.ClientID+"',null,this,'"+txtSpinMinutes.ClientID+"',event);");
			txtSpinMinutes.Attributes.Add( "onkeydown", "javascript:mcCalend_FspinOnKeyDown('"+dvControl.ClientID+"','"+txtSpinHours.ClientID+"',this,'"+txtSpinSeconds.ClientID+"',event);");
			txtSpinSeconds.Attributes.Add( "onkeydown", "javascript:mcCalend_FspinOnKeyDown('"+dvControl.ClientID+"','"+txtSpinMinutes.ClientID+"',this,null,event);");

			txtSpinHours.Attributes.Add("onselect","javascript:mcCalend_FspinOnSelect('"+txtSpinHours.ClientID+"');" );
			txtSpinMinutes.Attributes.Add("onselect","javascript:mcCalend_FspinOnSelect('"+txtSpinMinutes.ClientID+"');" );
			txtSpinSeconds.Attributes.Add("onselect","javascript:mcCalend_FspinOnSelect('"+txtSpinSeconds.ClientID+"');" );

			txtSpinHours.Attributes.Add("onfocus","javascript:mcCalend_FspinOnFocus('"+dvControl.ClientID+"','"+txtSpinHours.ClientID+"',0);" );
			txtSpinMinutes.Attributes.Add("onfocus","javascript:mcCalend_FspinOnFocus('"+dvControl.ClientID+"','"+txtSpinMinutes.ClientID+"',1);" );
			txtSpinSeconds.Attributes.Add("onfocus","javascript:mcCalend_FspinOnFocus('"+dvControl.ClientID+"','"+txtSpinSeconds.ClientID+"',2);" );

			txtSpinHours.Attributes.Add("onblur","javascript:mcCalend_FspinOnBlur('"+dvControl.ClientID+"','"+txtSpinHours.ClientID+"');" );
			txtSpinMinutes.Attributes.Add("onblur","javascript:mcCalend_FspinOnBlur('"+dvControl.ClientID+"','"+txtSpinMinutes.ClientID+"');" );
			txtSpinSeconds.Attributes.Add("onblur","javascript:mcCalend_FspinOnBlur('"+dvControl.ClientID+"','"+txtSpinSeconds.ClientID+"');" );

			btnImgSpinUp.Attributes.Remove("onclick");
			if(this.Enabled)
				btnImgSpinUp.Attributes.Add ("onclick", "javascript:mcCalend_FimgSpin('"+dvControl.ClientID+"',0);" );
			btnImgSpinDown.Attributes.Remove("onclick");
			if(this.Enabled)
				btnImgSpinDown.Attributes.Add ("onclick", "javascript:mcCalend_FimgSpin('"+dvControl.ClientID+"',1);" );

			//if(HourSpinUnLimited)
				dvControl.Attributes.Add( "mcCalend_ASpinHourLimit", hourspinmaxvalue.ToString());//"0");
			//spin end

			//attaching events to trDays
			if(selectionDateWeek)
			{
				trDaysAttachEvents(trDays1);
				trDaysAttachEvents(trDays2);
				trDaysAttachEvents(trDays3);
				trDaysAttachEvents(trDays4);
				trDaysAttachEvents(trDays5);
				trDaysAttachEvents(trDays6);
			}

			Page.ClientScript.RegisterArrayDeclaration("g_mcCalend_dvMainClIds", "'" + dvMain.ClientID + "'");
			//set defined culture param
			CultureInfo calend_culture = new CultureInfo(strCalendCulture);
			string[] CurLangMonthNames = calend_culture.DateTimeFormat.MonthNames;
			int max_length=0;
			string CurLangMonthName, strToDeclare="";
			for(int i=0;i<12;i++)
			{
				CurLangMonthName = CurLangMonthNames[i];
				if(i>0) strToDeclare = strToDeclare + ", ";
				strToDeclare = strToDeclare + "'" + CurLangMonthName + "'";
				if(max_length<CurLangMonthName.Length) max_length=CurLangMonthName.Length;
			}
			strToDeclare = strToDeclare + ", " + max_length.ToString();
			Page.ClientScript.RegisterArrayDeclaration("g_mcCalend_MonFN", "'" + dvControl.ClientID + "', new Array(" + strToDeclare + ")");
			max_length_MonFN = max_length;

			DateTimeFormatInfo dtf =  calend_culture.DateTimeFormat;
			string lbl;			 
			//int findex = 1;//(int)dtf.FirstDayOfWeek;
			//int findex = int.Parse(dtf.FirstDayOfWeek.GetHashCode());
			int findex = (int)dtf.FirstDayOfWeek;//.GetHashCode();
			if(firstdayweek >= 0) findex = firstdayweek;
			if(findex >= 7) findex = findex % 7;
			txtFirstDW.Value = (findex==0)?"7":findex.ToString();
			max_length=0;
			strToDeclare="";
			for( int i=0; i<7; i++)
			{
				lbl = dtf.GetAbbreviatedDayName((DayOfWeek)findex);
				if(i>0) strToDeclare = strToDeclare + ", ";
				strToDeclare = strToDeclare + "'" + lbl + "'";
				if( max_length < lbl.Length ) max_length = lbl.Length;
				findex++;
				if (findex==7) findex = 0;
			}
			strToDeclare = strToDeclare + ", " + max_length.ToString();
			Page.ClientScript.RegisterArrayDeclaration("g_mcCalend_DayWeekSN", "'" + dvControl.ClientID + "', new Array(" + strToDeclare + ")");
			max_length_WDsSN = max_length;

			strToDeclare="";
			strToDeclare = strToDeclare + "'" + dtf.ShortDatePattern.ToString() + "'";
			int d_digits_count = NumStringsCount(dtf.ShortDatePattern.ToString(), "d");
			int M_digits_count = NumStringsCount(dtf.ShortDatePattern.ToString(), "M");
			int y_digits_count = NumStringsCount(dtf.ShortDatePattern.ToString(), "y");
			int d_pos=0, M_pos=0, y_pos=0;
			if( strToDeclare.IndexOf("d") > strToDeclare.IndexOf("M") ) d_pos++;
			else M_pos++;
			if( strToDeclare.IndexOf("d") > strToDeclare.IndexOf("y") ) d_pos++;
			else y_pos++;
			if( strToDeclare.IndexOf("y") > strToDeclare.IndexOf("M") ) y_pos++;
			else M_pos++;
			//txtDtInt.Attributes.Add("title","Date format: "+dtf.ShortDatePattern.ToString());
			//txtDtInt.ToolTip = "Date format: " + dtf.ShortDatePattern.ToString();
			txtDtInt.Attributes.Remove("title");
			txtDtInt.ToolTip = LocRM.GetString("DateFormat") + ": " + dtf.ShortDatePattern.ToString();
			btnImgReset.Alt = LocRM.GetString("btnImgResetText");

			strToDeclare = strToDeclare + ", '" + string.Empty.PadRight(d_digits_count, 'd') + "'" +
				", '" + string.Empty.PadRight(M_digits_count, 'M') + "'" +
				", '" + string.Empty.PadRight(y_digits_count, 'y') + "'" +
				", " + d_digits_count.ToString() + 
				", " + M_digits_count.ToString() + 
				", " + y_digits_count.ToString() + 
				", '" + dtf.DateSeparator.ToString() + "'" +
				", " + d_pos.ToString() + 
				", " + M_pos.ToString() + 
				", " + y_pos.ToString();
			Page.ClientScript.RegisterArrayDeclaration("g_mcCalend_SDP", "'" + dvControl.ClientID + "', new Array(" + strToDeclare + ")");

			/*
			System.Reflection.FieldInfo[] fieldlist =  this.GetType().GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);// | System.Reflection.BindingFlags.DeclaredOnly);
			foreach( System.Reflection.FieldInfo fi in fieldlist )
			{
				object Value =  fi.GetValue(this);
				if( Value is System.Web.UI.Control )
				{
					if( ((System.Web.UI.Control)Value).ID != null  && ((System.Web.UI.Control)Value).ClientID != null )
					{
						Page.RegisterArrayDeclaration("g_mcCalend_ElsId_"+this.ID, "'"+((System.Web.UI.Control)Value).ID+"'");
						Page.RegisterArrayDeclaration("g_mcCalend_ElsClId_"+this.ID, "'"+((System.Web.UI.Control)Value).ClientID+"'");
						if( Value is System.Web.UI.HtmlControls.HtmlImage ) continue;
						if( Value is System.Web.UI.WebControls.RequiredFieldValidator ) continue;
						if( Value is System.Web.UI.HtmlControls.HtmlGenericControl && ( ((System.Web.UI.Control)Value).ID == "spTimeHM" || ((System.Web.UI.Control)Value).ID == "spTimeMS" )) continue;
						if( Value is System.Web.UI.HtmlControls.HtmlTableCell && ((System.Web.UI.Control)Value).ID != "tdHeaderM" && ((System.Web.UI.Control)Value).ID != "tdHeaderY" ) continue;
						if( Value is System.Web.UI.HtmlControls.HtmlTableRow && ((System.Web.UI.Control)Value).ID != "trWDs" ) continue;
						Page.RegisterArrayDeclaration("g_mcCalend_ElsId1_"+this.ID, "'"+((System.Web.UI.Control)Value).ID+"'");
						Page.RegisterArrayDeclaration("g_mcCalend_ElsClId1_"+this.ID, "'"+((System.Web.UI.Control)Value).ClientID+"'");
						if( Value is System.Web.UI.WebControls.WebControl )
						{
							Page.RegisterArrayDeclaration("g_mcCalend_ElsId2_"+this.ID, "'"+((System.Web.UI.Control)Value).ID+"'");
							Page.RegisterArrayDeclaration("g_mcCalend_ElsClId2_"+this.ID, "'"+((System.Web.UI.WebControls.WebControl)Value).Visible.ToString()+"'");
							Page.RegisterArrayDeclaration("g_mcCalend_ElsClId2_"+this.ID, "'"+((System.Web.UI.WebControls.WebControl)Value).Enabled.ToString()+"'");
							if( !((System.Web.UI.WebControls.WebControl)Value).Visible || !((System.Web.UI.WebControls.WebControl)Value).Enabled ) continue;
							((System.Web.UI.WebControls.WebControl)Value).Attributes.Add("mcCalend_AElId",((System.Web.UI.Control)Value).ID);
						}
						if( Value is System.Web.UI.HtmlControls.HtmlControl )
						{
							if( !((System.Web.UI.HtmlControls.HtmlControl)Value).Visible || ((System.Web.UI.HtmlControls.HtmlControl)Value).Disabled ) continue;
							((System.Web.UI.HtmlControls.HtmlControl)Value).Attributes.Add("mcCalend_AElId",((System.Web.UI.Control)Value).ID);
						}
					}
				}
			}
			*/

			if (!this.IsPostBack || PostBackInit)
			{
				if(strelsbgcolor != null) txtDtInt.Style.Add("background-color",strelsbgcolor);//"#e6f3ff");
				if(showtime)
				{
					if(showtime_hours)
					{
						BindHours();
						if(strelsbgcolor != null) selDtHours.Style.Add("background-color",strelsbgcolor);
					}
					else
					{
						selDtHours.Visible = false;
						txtSpinHours.Visible = false;
					}
					if(showtime_minutes)
					{
						BindMinutes();
						if(strelsbgcolor != null) selDtMinutes.Style.Add("background-color",strelsbgcolor);
						if(strspanclass != null)
						{
							spTimeHM.Attributes.Add("class",strspanclass);
							spSpinHM.Attributes.Add("class",strspanclass);
						}
					}
					else
					{
						spTimeHM.Visible = false;
						selDtMinutes.Visible = false;
						spSpinHM.Visible = false;
						txtSpinMinutes.Visible = false;
					}
					if(showtime_seconds)
					{
						BindSeconds();
						if(strelsbgcolor != null) selDtSeconds.Style.Add("background-color",strelsbgcolor);
						if(strspanclass != null)
						{
							spTimeMS.Attributes.Add("class",strspanclass);
							spSpinMS.Attributes.Add("class",strspanclass);
						}
					}
					else
					{
						spTimeMS.Visible = false;
						selDtSeconds.Visible = false;
						spSpinMS.Visible = false;
						txtSpinSeconds.Visible = false;
					}
				}
				/*
				txtYear.Value = System.DateTime.Today.Year.ToString();
				txtMonth.Value = (System.DateTime.Today.Month-1).ToString();
				txtDay.Value = System.DateTime.Today.Day.ToString();

				System.DateTime temp_time = System.DateTime.Now;*/
				txtYear.Value = dtcStartDate.Year.ToString();
				txtMonth.Value = (dtcStartDate.Month-1).ToString();
				txtDay.Value = dtcStartDate.Day.ToString();
				if(showtime && viewstartdate)
				{
					if(showtime_hours) selDtHours.SelectedIndex = dtcStartDate.Hour;
					if(showtime_minutes) selDtMinutes.SelectedIndex = ( dtcStartDate.Minute - (dtcStartDate.Minute % minutes_step) ) / minutes_step;
					if(showtime_seconds) selDtSeconds.SelectedIndex = ( dtcStartDate.Second - (dtcStartDate.Second % seconds_step) ) / seconds_step;

					if(!HourSpinUnLimited)
						txtSpinHours.Text = string.Format("{0:D"+(hourspinmaxvalue-1).ToString().Length+"}",dtcStartDate.Hour);
					else
						txtSpinHours.Text = string.Format("{0:D2}",dtcStartDate.Hour+(dtcStartDate.Day-1)*24);

					if(showtime_minutes) txtSpinMinutes.Text = string.Format("{0:D2}",dtcStartDate.Minute);
					if(showtime_seconds) txtSpinSeconds.Text = string.Format("{0:D2}",dtcStartDate.Second);
				}

				System.DateTime temp_time = dtcStartDate;
				int FirstWeekDayDelta = findex - temp_time.DayOfWeek.GetHashCode();
				//int FirstWeekDayDelta = findex - (int)temp_time.DayOfWeek;
				if( FirstWeekDayDelta > 0 ) FirstWeekDayDelta = FirstWeekDayDelta - 7;
				//System.DateTime temp_time1 = temp_time.AddDays( 1-temp_time.DayOfWeek.GetHashCode() );
				System.DateTime temp_time1 = DateTime.MinValue;
				try
				{	
					temp_time1 = temp_time.AddDays( FirstWeekDayDelta );
				}
				catch{}
				System.DateTime temp_time2 = temp_time1.AddDays( 6 );

				if(yearweekwrap)
				{
					if(temp_time2.Year>temp_time1.Year)
					{
						System.DateTime first_year_day = new DateTime(temp_time2.Year,1,1);
						if(dtcStartDate>=first_year_day)
							temp_time1 = first_year_day.AddDays( 0 );
						if(dtcStartDate<first_year_day && temp_time2.Year>1)
							temp_time2 = first_year_day.AddDays( -1 );
					}
				}

				txtYear0.Value = temp_time1.Year.ToString();
				txtMonth0.Value = (temp_time1.Month-1).ToString();
				txtDay0.Value = temp_time1.Day.ToString();

				txtYear1.Value = temp_time2.Year.ToString();
				txtMonth1.Value = (temp_time2.Month-1).ToString();
				txtDay1.Value = temp_time2.Day.ToString();

				//System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.DateSeparator.Insert(0,"//");
				//CultureInfo.CurrentUICulture.DateTimeFormat.ShortDatePattern.ToString();
				if(selectionDateWeek)
				{
					//string str_txtDtInt1 = temp_time1.ToShortDateString();//ToString("d").Replace(".","/");
					//string str_txtDtInt2 = temp_time2.ToShortDateString();//ToString("d").Replace(".","/");
					string str_txtDtInt1 = temp_time1.ToString(dtf.ShortDatePattern.ToString(), DateTimeFormatInfo.InvariantInfo);
					string str_txtDtInt2 = temp_time2.ToString(dtf.ShortDatePattern.ToString(), DateTimeFormatInfo.InvariantInfo);
					//str_txtDtInt1 = str_txtDtInt1.Substring(0,str_txtDtInt1.Length-1);
					//str_txtDtInt2 = str_txtDtInt2.Substring(0,str_txtDtInt2.Length-1);
					if(viewstartdate) txtDtInt.Text = str_txtDtInt1 + "-" + str_txtDtInt2;
					txtSelDayWeek.Value = "1";
				}
				else
				{
					//txtDtInt.Text = temp_time.ToString("d").Replace(".","/");
					//txtDtInt.Text = txtDay.Value.ToString() + "/" + txtMonth.Value.ToString() + "/" + txtYear.Value.ToString();
					//txtDtInt.Text = ((temp_time.Day.ToString().Length<2)?"0"+temp_time.Day.ToString():temp_time.Day.ToString()) + "/" + ((temp_time.Month.ToString().Length<2)?"0"+temp_time.Month.ToString():temp_time.Month.ToString()) + "/" + txtYear.Value.ToString();
					if(viewstartdate) txtDtInt.Text = temp_time.ToString(dtf.ShortDatePattern.ToString(), DateTimeFormatInfo.InvariantInfo);//temp_time.ToShortDateString();

					//txtDtInt.Text = txtDtInt.Text.Substring(0,txtDtInt.Text.Length-1);
					txtSelDayWeek.Value = "0";
				}
				
				//txtDtInt.Text = calend_culture.DateTimeFormat.ShortDatePattern.ToString();
				//txtDtInt.Text = calend_culture.DateTimeFormat.ShortTimePattern.ToString();
				string strPSDP = PrintShortDatePattern("en-US") + "\r\n";
				strPSDP += PrintShortDatePattern("ja-JP") + "\r\n";
				strPSDP += PrintShortDatePattern("fr-FR") + "\r\n";
				strPSDP += PrintShortDatePattern("ru-RU") + "\r\n";
				strPSDP += PrintShortDatePattern("uk-UA") + "\r\n";
				strPSDP += PrintShortDatePattern("de-DE") + "\r\n";
				strPSDP += PrintShortDatePattern("lv-LV") + "\r\n";
				strPSDP += PrintShortDatePattern("kk-KZ") + "\r\n";
				strPSDP += PrintShortDatePattern("he-IL") + "\r\n";
				strPSDP += PrintShortDatePattern("et-EE") + "\r\n";
				strPSDP += PrintShortDatePattern("ar-EG") + "\r\n";
				//txtDtInt.Text = strPSDP;
				//txtDtInt.Text = this.UniqueID.ToString();
			}
			else
			{
				//IsPostBack
				if(value_is_changed)
				{
					if(!selectionDateWeek)
					{
						txtYear.Value = dtcStartDate.Year.ToString();
						txtMonth.Value = (dtcStartDate.Month-1).ToString();
						txtDay.Value = dtcStartDate.Day.ToString();
						if(showtime)
						{
							if(showtime_hours) selDtHours.SelectedIndex = dtcStartDate.Hour;
							if(showtime_minutes) selDtMinutes.SelectedIndex = ( dtcStartDate.Minute - (dtcStartDate.Minute % minutes_step) ) / minutes_step;
							if(showtime_seconds) selDtSeconds.SelectedIndex = ( dtcStartDate.Second - (dtcStartDate.Second % seconds_step) ) / seconds_step;

							if(!HourSpinUnLimited)
								txtSpinHours.Text = string.Format("{0:D"+(hourspinmaxvalue-1).ToString().Length+"}",dtcStartDate.Hour);
							else
								txtSpinHours.Text = string.Format("{0:D2}",dtcStartDate.Hour+(dtcStartDate.Day-1)*24);

							if(showtime_minutes) txtSpinMinutes.Text = string.Format("{0:D2}",dtcStartDate.Minute);
							if(showtime_seconds) txtSpinSeconds.Text = string.Format("{0:D2}",dtcStartDate.Second);
						}

						txtDtInt.Text = dtcStartDate.ToShortDateString();
						txtSelDayWeek.Value = "0";
					}
				}
			}
			//HtmlTextWriter
			//show data
			txtShowYear.Value = txtYear.Value;
			txtShowMonth.Value = txtMonth.Value;
			//set week days width
			float width_tdWds, width_tdWds_unit;
			if(view2rows)
			{
				width_tdWds_unit = 0.5f;
				width_tdWds = width_tdWds_unit * ( 1 + max_length_WDsSN );
				//if(width_tdWds<1.5) width_tdWds = 1.5f;
				if(width_tdWds<2.0) width_tdWds = 2.0f;
			}
			else
			{
				width_tdWds_unit = 2.0f/3;// 0.(6)
				width_tdWds = width_tdWds_unit * ( 1 + max_length_WDsSN );
				if(width_tdWds<2.0) width_tdWds = 2.0f;
			}
			//trYear.Cells[0].Style.Add("width",(width_tdWds * 7).ToString().Replace(",",".")+"em");
			//txtDtInt.Text = max_length_WDsSN.ToString() + "_" + width_tdWds_unit.ToString() + "_" + width_tdWds.ToString();
			for(int i=0;i<7;i++)
			{
				trWDs.Cells[i].Style.Add("width",width_tdWds.ToString().Replace(",",".")+"em");
			}
			//set month div width
			float width_dvM;
			if(view2rows)
			{
				width_dvM = 4 + max_length_MonFN;
				if( width_dvM > 14.8 && width_tdWds<=1.5 ) width_dvM = 14.8f;
			}
			else
			{
				width_dvM = 6 + max_length_MonFN;
				if( width_dvM > 17.8 && width_tdWds<=2.0 ) width_dvM = 17.8f;
			}
			if( width_dvM < 11.0 ) width_dvM = 11.0f;
			dvM.Style.Add("width",width_dvM.ToString().Replace(",",".")+"em");

			if(showtime)
			{
				int hours_len = (hourspinmaxvalue-1).ToString().Length;
				if(hours_len > 2)
				{
					float width_hours = 1.5f + 0.8f*(hours_len-2);
					txtSpinHours.Style.Remove("width");
					txtSpinHours.Style.Add("width",width_hours.ToString().Replace(",",".")+"em");
					txtSpinHours.MaxLength = hours_len;
				}
			}
		}

		#region trAttachEvents
		private void trDaysAttachEvents( System.Web.UI.HtmlControls.HtmlTableRow EltoAttach )
		{
			EltoAttach.Attributes.Add ( "onmouseover", "javascript:mcCalend_FtrOver(this,event);" );
			EltoAttach.Attributes.Add ( "onmouseout", "javascript:mcCalend_FtrOut(this,event);" );
			EltoAttach.Attributes.Add ( "onclick", "javascript:mcCalend_FtrClick('"+dvControl.ClientID+"',this,event);" );
		}
		#endregion

		#region imgArrowsAttachEvents
		private void imgArrowsAttachEvents( System.Web.UI.HtmlControls.HtmlImage EltoAttach, String OverColor, String DownColor )
		{
			EltoAttach.Attributes.Add ("onmouseover", "javascript:mcCalend_FButArrowOver(this,'"+OverColor+"');" );
			EltoAttach.Attributes.Add ("onmouseout", "javascript:mcCalend_FButArrowOut(this);" );
			EltoAttach.Attributes.Add ("onmousedown", "javascript:mcCalend_FButArrowDown(this,'"+DownColor+"');" );
			EltoAttach.Attributes.Add ("onmouseup", "javascript:mcCalend_FButArrowUp(this,'"+OverColor+"');" );
		}
		#endregion

		#region PrintKeysAndValues
		private string htPrintKeysAndValues( System.Collections.Hashtable myList )
		{
			System.Collections.IDictionaryEnumerator myEnumerator = myList.GetEnumerator();
			string htConsole = "";
			while ( myEnumerator.MoveNext() )
				htConsole = htConsole + "key='" + myEnumerator.Key + "' value='" + myEnumerator.Value + "'";
			return htConsole;
		}
		#endregion

		public void Reset()
		{
			txtDtInt.Text = String.Empty;
			selDtHours.ClearSelection();
			selDtHours.SelectedIndex = 0;
			selDtMinutes.ClearSelection();
			selDtMinutes.SelectedIndex = 0;
			selDtSeconds.ClearSelection();
			selDtSeconds.SelectedIndex = 0;
		}

		#region BindHours
		private void BindHours()
		{
			//DateTime dtNow = UserDateTime.UserNow;
			CultureInfo calend_culture = new CultureInfo(strCalendCulture);
			string strTimePattern = calend_culture.DateTimeFormat.ShortTimePattern.ToString();
			int t_counts = 0, pos_search = 0;
			while( pos_search >= 0 && pos_search < strTimePattern.Length )
			{
				pos_search = strTimePattern.IndexOf("t",pos_search);
				if(pos_search!=-1)
				{
					t_counts++;	
					pos_search++;
				}
			}
			
			//int widthselDtHours = 40+t_counts*20, i_ampm, t_counts_temp;
			int widthselDtHours = 4+t_counts, i_ampm, t_counts_temp;
			selDtHours.Style.Add("width", widthselDtHours.ToString()+"em");
			//selDtHours.Style.Add("width", widthselDtHours.ToString()+"px");
			for(int i=0;i < 24;i++)
			{
				string str = i.ToString();
				if(i<10)
					str = "0" + str;
				if(t_counts>0)
				{
					i_ampm = i;
					if(i_ampm>12) i_ampm = i_ampm-12;
					if(i_ampm==0) i_ampm = 12;
					str = i_ampm.ToString();
					if(i_ampm<10) str = "0" + str;
					if(i<12)
					{
						t_counts_temp = (calend_culture.DateTimeFormat.AMDesignator.Length<t_counts)?calend_culture.DateTimeFormat.AMDesignator.Length:t_counts;
						str = str + " ";
						str = str + calend_culture.DateTimeFormat.AMDesignator.Substring(0,t_counts_temp);
					}
					else
					{
						t_counts_temp = (calend_culture.DateTimeFormat.PMDesignator.Length<t_counts)?calend_culture.DateTimeFormat.PMDesignator.Length:t_counts;
						str = str + " ";
						str = str + calend_culture.DateTimeFormat.PMDesignator.Substring(0,t_counts_temp);
					}
				}
				selDtHours.Items.Add(str);
			}
		}
		#endregion

		#region BindMinutes
		private void BindMinutes()
		{
			//selDtMinutes.Style.Add("width","40px");
			selDtMinutes.Style.Add("width","4em");
			for(int i=0; i < 60; i = i + minutes_step)
			{
				string str = i.ToString();
				if(i<10)
					str = "0" + str;
				ListItem lItem = new ListItem(str,i.ToString()); 
				selDtMinutes.Items.Add(lItem);
			}
		}
		#endregion

		#region BindSeconds
		private void BindSeconds()
		{
			//selDtSeconds.Style.Add("width","40px");
			selDtSeconds.Style.Add("width","4em");
			for(int i=0; i < 60; i = i + seconds_step)
			{
				string str = i.ToString();
				if(i<10)
					str = "0" + str;
				ListItem lItem = new ListItem(str,i.ToString()); 
				selDtSeconds.Items.Add(lItem);
			}
		}
		#endregion

		private int NumStringsCount(string strToExplore, string strToFind)
		{
			int t_counts = 0, pos_search = 0;
			while( pos_search >= 0 && pos_search < strToExplore.Length )
			{
				pos_search = strToExplore.IndexOf(strToFind,pos_search);
				if(pos_search!=-1)
				{
					t_counts++;	
					pos_search++;
				}
			}
			return t_counts;
		}

		private static string PrintShortDatePattern( string myCulture )  
		{
			DateTimeFormatInfo myDTFI = new CultureInfo( myCulture, false ).DateTimeFormat;
			return myCulture + ":" + myDTFI.ShortDatePattern;
		}

		protected System.Web.UI.Control FindControl(System.Web.UI.Control rootControl, String id)
		{
			foreach(System.Web.UI.Control ctrl in rootControl.Controls)
			{
				if(ctrl.ID==id)
				{
					return ctrl;
				}
				System.Web.UI.Control subRes=FindControl(ctrl,id);
				if(subRes!=null) return subRes;        
			}
			return null;
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
			this.OnChangeValidator.ServerValidate += new System.Web.UI.WebControls.ServerValidateEventHandler(this.CustomValidator);
			this.SpinValidator.ServerValidate += new System.Web.UI.WebControls.ServerValidateEventHandler(this.CustomValidatorSpin);
		}
		#endregion

		#region CustomValidators
		private void CustomValidator(Object sender, ServerValidateEventArgs args)
		{
			DateTime dt = DateTime.MinValue;
			if(!SelectionDateWeek)
			{
				CultureInfo calend_culture = new CultureInfo(strCalendCulture);
				String strText = txtDtInt.Text.Replace(" ","");
				if(strText != "")
				{
					try
					{
						dt = DateTime.Parse(txtDtInt.Text,calend_culture.DateTimeFormat);
						//dt = DateTime.ParseExact(strText,calend_culture.DateTimeFormat.ShortDatePattern,calend_culture);
					}
					catch(Exception)
					{
						//OnChangeValidator.ErrorMessage = "";
						txtDtInt.Style.Add("background-color","#ffcccc");
						txtDtInt.Style.Add("color","#ff0000");
						txtDtInt.Attributes.Add("mc_Calend_OCError","1");
						args.IsValid = false;
						return;
					}
				}
			}
			txtDtInt.Style.Remove("background-color");
			txtDtInt.Style.Remove("color");
			txtDtInt.Style.Add("color","");
			txtDtInt.Attributes.Remove("mc_Calend_OCError");
			args.IsValid = true;
		}

		private void CustomValidatorSpin(Object sender, ServerValidateEventArgs args)
		{
			DateTime dt;//DateTime.MinValue;
			int spinHours = 0;
			int spinMinutes = 0;
			int spinSeconds = 0;
			try
			{
				if(!(!requiredfield && txtSpinHours.Text==""))
					spinHours = int.Parse(txtSpinHours.Text);
				if(showtime_minutes)
				{
					if(!(!requiredfield && txtSpinMinutes.Text==""))
						spinMinutes = int.Parse(txtSpinMinutes.Text);
				}
				if(showtime_seconds)
				{
					if(!(!requiredfield && txtSpinSeconds.Text==""))
						spinSeconds = int.Parse(txtSpinSeconds.Text);
				}
				if(!controlviewtype_onlytime)
					dt = new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.Day,spinHours,spinMinutes,spinSeconds);
				else
				{
					dt = DateTime.MinValue;
					dt.AddHours(spinHours);
					dt.AddMinutes(spinMinutes);
					dt.AddSeconds(spinSeconds);
				}
			}
			catch(Exception)
			{
				try
				{
					if(!(!requiredfield && txtSpinHours.Text==""))
						spinHours = int.Parse(txtSpinHours.Text);
				}
				catch(Exception)
				{
					spinHours = -1;
				}

				try
				{
					if(!(!requiredfield && txtSpinMinutes.Text==""))
						spinMinutes = int.Parse(txtSpinMinutes.Text);
				}
				catch(Exception)
				{
					spinMinutes = -1;
				}

				try
				{
					if(!(!requiredfield && txtSpinSeconds.Text==""))
						spinSeconds = int.Parse(txtSpinSeconds.Text);
				}
				catch(Exception)
				{
					spinSeconds = -1;
				}
			}

			args.IsValid = true;

			txtSpinHours.Style.Remove("background-color");
			txtSpinHours.Style.Remove("color");
			txtSpinHours.Attributes.Remove("mc_Calend_OCError");
			if((spinHours<0 || (spinHours>=hourspinmaxvalue && hourspinmaxvalue>0)) && !HourSpinUnLimited)
			{
				txtSpinHours.Style.Add("background-color","#ffcccc");
				txtSpinHours.Style.Add("color","#ff0000");
				txtSpinHours.Attributes.Add("mc_Calend_OCError","1");
				args.IsValid = false;
			}

			txtSpinMinutes.Style.Remove("background-color");
			txtSpinMinutes.Style.Remove("color");
			txtSpinMinutes.Attributes.Remove("mc_Calend_OCError");
			if(showtime_minutes && (spinMinutes<0 || spinMinutes>59))
			{
				txtSpinMinutes.Style.Add("background-color","#ffcccc");
				txtSpinMinutes.Style.Add("color","#ff0000");
				txtSpinMinutes.Attributes.Add("mc_Calend_OCError","1");
				args.IsValid = false;
			}

			txtSpinSeconds.Style.Remove("background-color");
			txtSpinSeconds.Style.Remove("color");
			txtSpinSeconds.Attributes.Remove("mc_Calend_OCError");
			if(showtime_seconds && (spinSeconds<0 || spinSeconds>59))
			{
				txtSpinSeconds.Style.Add("background-color","#ffcccc");
				txtSpinSeconds.Style.Add("color","#ff0000");
				txtSpinSeconds.Attributes.Add("mc_Calend_OCError","1");
				args.IsValid = false;
			}

			if(!args.IsValid)
				return;

		}
		#endregion

		#region Page_PreRender
		protected void Page_PreRender(object sender, System.EventArgs e)
		{
			System.Reflection.FieldInfo[] fieldlist =  this.GetType().GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);// | System.Reflection.BindingFlags.DeclaredOnly);
			foreach( System.Reflection.FieldInfo fi in fieldlist )
			{
				object Value =  fi.GetValue(this);
				if( Value is System.Web.UI.Control )
				{
					if( ((System.Web.UI.Control)Value).ID != null  && ((System.Web.UI.Control)Value).ClientID != null )
					{
						if( Value is System.Web.UI.HtmlControls.HtmlImage ) continue;
						if( Value is System.Web.UI.WebControls.RequiredFieldValidator ) continue;
						if( Value is System.Web.UI.HtmlControls.HtmlGenericControl && ( ((System.Web.UI.Control)Value).ID == "spTimeHM" || ((System.Web.UI.Control)Value).ID == "spTimeMS" )) continue;
						if( Value is System.Web.UI.HtmlControls.HtmlTableCell && ((System.Web.UI.Control)Value).ID != "tdHeaderM" && ((System.Web.UI.Control)Value).ID != "tdHeaderY" ) continue;
						if( Value is System.Web.UI.HtmlControls.HtmlTableRow && ((System.Web.UI.Control)Value).ID != "trWDs" ) continue;
						if( Value is System.Web.UI.WebControls.WebControl )
						{
							if( !((System.Web.UI.WebControls.WebControl)Value).Visible || !((System.Web.UI.WebControls.WebControl)Value).Enabled ) continue;
							((System.Web.UI.WebControls.WebControl)Value).Attributes.Add("mcCalend_AElId",((System.Web.UI.Control)Value).ID);
						}
						if( Value is System.Web.UI.HtmlControls.HtmlControl )
						{
							if( !((System.Web.UI.HtmlControls.HtmlControl)Value).Visible || ((System.Web.UI.HtmlControls.HtmlControl)Value).Disabled ) continue;
							((System.Web.UI.HtmlControls.HtmlControl)Value).Attributes.Add("mcCalend_AElId",((System.Web.UI.Control)Value).ID);
						}
					}
				}
			}
			//txtDtInt.Text = "!"+dtcStartDate.ToShortDateString();
			dvMain.Attributes.Add("mcCalend_AElAlign",i_drop_galign.ToString());

			if(IsPostBack)
			{
				if(value_is_changed)
				{
					if(!selectionDateWeek)
					{
						txtYear.Value = dtcStartDate.Year.ToString();
						txtMonth.Value = (dtcStartDate.Month-1).ToString();
						txtDay.Value = dtcStartDate.Day.ToString();
						if(showtime)
						{
							if(showtime_hours) selDtHours.SelectedIndex = dtcStartDate.Hour;
							if(showtime_minutes) selDtMinutes.SelectedIndex = ( dtcStartDate.Minute - (dtcStartDate.Minute % minutes_step) ) / minutes_step;
							if(showtime_seconds) selDtSeconds.SelectedIndex = ( dtcStartDate.Second - (dtcStartDate.Second % seconds_step) ) / seconds_step;

							if(!HourSpinUnLimited)
								txtSpinHours.Text = string.Format("{0:D"+(hourspinmaxvalue-1).ToString().Length+"}",dtcStartDate.Hour);
							else
								txtSpinHours.Text = string.Format("{0:D2}",dtcStartDate.Hour+(dtcStartDate.Day-1)*24);

							if(showtime_minutes) txtSpinMinutes.Text = string.Format("{0:D2}",dtcStartDate.Minute);
							if(showtime_seconds) txtSpinSeconds.Text = string.Format("{0:D2}",dtcStartDate.Second);
						}

						txtDtInt.Text = dtcStartDate.ToShortDateString();
						txtSelDayWeek.Value = "0";
					}
				}
			}
			PostBackInit = false;

		}
		#endregion
	}
}
