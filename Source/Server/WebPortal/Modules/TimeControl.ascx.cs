namespace Mediachase.UI.Web.Modules
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;

	/// <summary>
	///		Summary description for TimeControl.
	/// </summary>
	public partial class TimeControl : System.Web.UI.UserControl
	{

		#region prop: IsValid
		/// <summary>
		/// Gets a value indicating whether this instance is valid.
		/// </summary>
		/// <value><c>true</c> if this instance is valid; otherwise, <c>false</c>.</value>
		public bool IsValid
		{
			get
			{
				SpinValidator.Validate();
				return SpinValidator.IsValid;
			}
		} 
		#endregion	

		#region prop: ValidationGroup
		/// <summary>
		/// Gets or sets the validation group.
		/// </summary>
		/// <value>The validation group.</value>
		public string ValidationGroup
		{
			get
			{
				if (ViewState["_ValidationGroup"] == null)
					return string.Empty;

				return ViewState["_ValidationGroup"].ToString();
			}
			set
			{
				ViewState["_ValidationGroup"] = value;
			}
		} 
		#endregion

		#region prop: ShowSpin
		public bool ShowSpin
		{
			get
			{
				return tdBtnsImgSpin.Visible;
			}
			set
			{
				tdBtnsImgSpin.Visible = value;
			}
		}
		#endregion

		#region HTML controls

		#endregion

		#region Path_File_JS
		private string path_js = "~/Scripts/";
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

		private string file_js = "mcCalendScript.js";
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
		private string path_img = "~/Layouts/Images/";
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

		#region ShowTime
		public enum E_ShowTime 
		{
			None,
			H,
			HM,
			HMS
		}
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
					showtime_hours = true;
					if( showtime_value.GetHashCode() > 1 ) 
						showtime_minutes = true;
					if( showtime_value.GetHashCode() > 2 ) 
						showtime_seconds = true;
				}

				txtSpinMinutes.Visible = showtime_minutes;
				spSpinHM.Visible = showtime_minutes;
				
				txtSpinSeconds.Visible = showtime_seconds;
				spSpinMS.Visible = showtime_seconds;
			}
			get
			{
				return showtime_value;
			}
		}

		#endregion

		#region HourSpinMaxValue
		private int hourspinmaxvalue = 24;
		public int HourSpinMaxValue
		{
			set
			{
				if(value>0)
				{
					hourspinmaxvalue = value;
				}
			}
			get
			{
				return hourspinmaxvalue;
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

				txtSpinHours.Enabled = value;
				txtSpinMinutes.Enabled = value;
				txtSpinSeconds.Enabled = value;

				btnImgSpinUp.Disabled = !value;
				btnImgSpinUp.Style.Remove("cursor");
				btnImgSpinUp.Attributes.Remove("onclick");
				if(value)
				{
					btnImgSpinUp.Style.Add("cursor","pointer");
					btnImgSpinUp.Attributes.Add ("onclick", "javascript:mcCalend_FimgSpin('"+dvControl.ClientID+"',0);" );
				}

				btnImgSpinDown.Disabled = !value;
				btnImgSpinDown.Style.Remove("cursor");
				btnImgSpinDown.Attributes.Remove("onclick");
				if(value)
				{
					btnImgSpinDown.Style.Add("cursor","pointer");
					btnImgSpinDown.Attributes.Add ("onclick", "javascript:mcCalend_FimgSpin('"+dvControl.ClientID+"',1);" );
				}

				btnImgReset.Style.Remove("cursor");
				if(value) btnImgReset.Style.Add("cursor","pointer");
				btnImgReset.Disabled = !value;
			}
			get
			{
				return enabled;
			}
		}
		#endregion

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

		#region Value
		
		private bool value_is_changed = false;
		//private DateTime tcStartDateTime = DateTime.MinValue.Add(System.DateTime.Now.TimeOfDay);
		private DateTime tcStartDateTime = DateTime.MinValue.Add(Mediachase.IBN.Business.User.GetLocalDate(DateTime.UtcNow).TimeOfDay);

		public DateTime Value
		{
			set
			{
				viewstartdate = true;
				tcStartDateTime = value;
				value_is_changed = true;
			}
			get
			{
				DateTime retVal;
				if(IsPostBack)
				{
					if(value_is_changed)
						value_is_changed=true;

					retVal = DateTime.MinValue;

					if(showtime_hours)
					{
						try	{retVal = retVal.AddHours(int.Parse(txtSpinHours.Text));}
						catch(Exception){}
					}

					if(showtime_minutes)
					{
						//txtSpinMinutes.Text = currentValue.Minutes.ToString().PadLeft(2, '0');
						try	{retVal = retVal.AddMinutes(int.Parse(txtSpinMinutes.Text));}
						catch(Exception){}
					}
					if(showtime_seconds)
					{
						try	{retVal = retVal.AddSeconds(int.Parse(txtSpinSeconds.Text));}
						catch(Exception){}
					}
				}
				else retVal=tcStartDateTime;
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

		#region RequiredField
		private bool requiredfield = true;
		public bool RequiredField
		{
			set 
			{
				requiredfield = value;
			}
			get
			{
				return requiredfield;
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

		#region HoursControlClientId
		public string HoursControlClientId
		{
			get
			{
				return txtSpinHours.ClientID;
			}
		}
		#endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
			ReInit();
			SpinValidator.ValidationGroup = this.ValidationGroup;
		}

		#region ReInit
		public void ReInit()
		{
			bool JS_File_Exists = true;
			if(check_exists_js_file)
			{
				JS_File_Exists = System.IO.File.Exists(Server.MapPath(path_js + file_js));
			}
			if( JS_File_Exists )
			{
				if(!Page.ClientScript.IsClientScriptBlockRegistered(this.GetType(), "mcCalendar_js"))
					Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "mcCalendar_js","<script language=\"javascript\" type=\"text/javascript\" src=\"" + ResolveClientUrl(path_js + file_js) + "\"></script>");
			}
			

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
				txtSpinHours.Attributes.Add( "onchange", "javascript:" + Page.ClientScript.GetPostBackEventReference(cntrlPostBack,"") + ";");
				txtSpinMinutes.Attributes.Add( "onchange", "javascript:" + Page.ClientScript.GetPostBackEventReference(cntrlPostBack,"") + ";");
				txtSpinSeconds.Attributes.Add( "onchange", "javascript:" + Page.ClientScript.GetPostBackEventReference(cntrlPostBack,"") + ";");
				if(postbackoldvalue)
					dvControl.Attributes.Add( "mcCalend_PostBackOldValue", "1");
			}
			
			btnImgReset.Attributes.Add ("onclick", "javascript:mcCalend_ClearSelection('"+dvControl.ClientID+"');" );
			btnImgReset.Src = path_img + "reset17.gif";

			// Select on keydown
			txtSpinHours.Attributes.Add("onkeydown", "javascript:mcCalend_FspinOnKeyDown('" + dvControl.ClientID + "',null,this,'" + txtSpinMinutes.ClientID + "',event);");
			txtSpinMinutes.Attributes.Add("onkeydown", "javascript:mcCalend_FspinOnKeyDown('" + dvControl.ClientID + "','" + txtSpinHours.ClientID + "',this,'" + txtSpinSeconds.ClientID + "',event);");
			txtSpinSeconds.Attributes.Add("onkeydown", "javascript:mcCalend_FspinOnKeyDown('" + dvControl.ClientID + "','" + txtSpinMinutes.ClientID + "',this,null,event);");

			txtSpinHours.Attributes.Add("onclick", "javascript:mcCalend_FspinOnClick();");
			txtSpinMinutes.Attributes.Add("onclick", "javascript:mcCalend_FspinOnClick();");
			txtSpinSeconds.Attributes.Add("onclick", "javascript:mcCalend_FspinOnClick();");

			txtSpinHours.Attributes.Add("onselect", "javascript:mcCalend_FspinOnSelect('" + txtSpinHours.ClientID + "');");
			txtSpinMinutes.Attributes.Add("onselect", "javascript:mcCalend_FspinOnSelect('" + txtSpinMinutes.ClientID + "');");
			txtSpinSeconds.Attributes.Add("onselect", "javascript:mcCalend_FspinOnSelect('" + txtSpinSeconds.ClientID + "');");

			txtSpinHours.Attributes.Add("onblur", "javascript:mcCalend_FspinOnBlur('" + dvControl.ClientID + "','" + txtSpinHours.ClientID + "');");
			txtSpinMinutes.Attributes.Add("onblur", "javascript:mcCalend_FspinOnBlur('" + dvControl.ClientID + "','" + txtSpinMinutes.ClientID + "');");
			txtSpinSeconds.Attributes.Add("onblur", "javascript:mcCalend_FspinOnBlur('" + dvControl.ClientID + "','" + txtSpinSeconds.ClientID + "');");

			if (ShowSpin)
			{
				txtSpinHours.Attributes.Add("onfocus", "javascript:mcCalend_FspinOnFocus('" + dvControl.ClientID + "','" + txtSpinHours.ClientID + "',0);");
				txtSpinMinutes.Attributes.Add("onfocus", "javascript:mcCalend_FspinOnFocus('" + dvControl.ClientID + "','" + txtSpinMinutes.ClientID + "',1);");
				txtSpinSeconds.Attributes.Add("onfocus", "javascript:mcCalend_FspinOnFocus('" + dvControl.ClientID + "','" + txtSpinSeconds.ClientID + "',2);");

				btnImgSpinUp.Src = path_img + "mcCalendarSpinUp.gif";
				btnImgSpinDown.Src = path_img + "mcCalendarSpinDn.gif";

				btnImgSpinUp.Attributes.Remove("onclick");
				if (this.Enabled)
					btnImgSpinUp.Attributes.Add("onclick", "javascript:mcCalend_FimgSpin('" + dvControl.ClientID + "',0);");
				btnImgSpinDown.Attributes.Remove("onclick");
				if (this.Enabled)
					btnImgSpinDown.Attributes.Add("onclick", "javascript:mcCalend_FimgSpin('" + dvControl.ClientID + "',1);");
			}
			else
			{
				// O.R. [2008-06-18]: select text inside element by click
				txtSpinHours.Attributes.Add("onfocus", "javascript:mcCalend_FfocusElement('" + txtSpinHours.ClientID + "');");
				txtSpinMinutes.Attributes.Add("onfocus", "javascript:mcCalend_FfocusElement('" + txtSpinMinutes.ClientID + "');");
				txtSpinSeconds.Attributes.Add("onfocus", "javascript:mcCalend_FfocusElement('" + txtSpinSeconds.ClientID + "');");
			}

			dvControl.Attributes.Add( "mcCalend_ASpinHourLimit", hourspinmaxvalue.ToString());
			//spin end

			if (!this.IsPostBack || PostBackInit)
			{
				if(!showtime_hours)
					txtSpinHours.Visible = false;

				if(showtime_minutes)
				{
					if(strspanclass != null)
						spSpinHM.Attributes.Add("class",strspanclass);
				}
				else
				{
					spSpinHM.Visible = false;
					txtSpinMinutes.Visible = false;
				}
				if(showtime_seconds)
				{
					if(strspanclass != null)
						spSpinMS.Attributes.Add("class",strspanclass);
				}
				else
				{
					spSpinMS.Visible = false;
					txtSpinSeconds.Visible = false;
				}
				
				if(viewstartdate)
				{
					TimeSpan ts = new TimeSpan(tcStartDateTime.Ticks);
					txtSpinHours.Text = string.Format("{0:D" + (hourspinmaxvalue - 1).ToString().Length + "}", (int)ts.TotalHours);

					if (showtime_minutes) txtSpinMinutes.Text = string.Format("{0:D2}", ts.Minutes);
					if (showtime_seconds) txtSpinSeconds.Text = string.Format("{0:D2}", ts.Seconds);
				}
			}
			else
			{
				//IsPostBack
				if(value_is_changed)
				{
					TimeSpan ts = new TimeSpan(tcStartDateTime.Ticks);
					txtSpinHours.Text = string.Format("{0:D" + (hourspinmaxvalue - 1).ToString().Length + "}", (int)ts.TotalHours);

					if (showtime_minutes) txtSpinMinutes.Text = string.Format("{0:D2}", ts.Minutes);
					if (showtime_seconds) txtSpinSeconds.Text = string.Format("{0:D2}", ts.Seconds);
				}
			}

			int hours_len = (hourspinmaxvalue-1).ToString().Length;
			if(hours_len > 2)
			{
				float width_hours = 1.5f + 0.8f*(hours_len-2);
				txtSpinHours.Style.Remove("width");
				txtSpinHours.Style.Add("width",width_hours.ToString().Replace(",",".")+"em");
				txtSpinHours.MaxLength = hours_len;
			}
		}
		#endregion

		#region FindControl
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
			this.SpinValidator.ServerValidate += new System.Web.UI.WebControls.ServerValidateEventHandler(this.CustomValidatorSpin);
		}
		#endregion

		#region CustomValidatorSpin
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
				dt = DateTime.MinValue;
				dt.AddHours(spinHours);
				dt.AddMinutes(spinMinutes);
				dt.AddSeconds(spinSeconds);
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
			if(spinHours<0 || (spinHours>=hourspinmaxvalue && hourspinmaxvalue>0))
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
						if( Value is System.Web.UI.HtmlControls.HtmlGenericControl && ( ((System.Web.UI.Control)Value).ID == "spSpinHM" || ((System.Web.UI.Control)Value).ID == "spSpinMS" )) continue;
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

			if(IsPostBack)
			{
				if(value_is_changed)
				{
          TimeSpan ts = new TimeSpan(tcStartDateTime.Ticks);
					txtSpinHours.Text = string.Format("{0:D"+(hourspinmaxvalue-1).ToString().Length+"}", (int)ts.TotalHours);

					if(showtime_minutes) txtSpinMinutes.Text = string.Format("{0:D2}",ts.Minutes);
					if(showtime_seconds) txtSpinSeconds.Text = string.Format("{0:D2}",ts.Seconds);
				}
			}
			PostBackInit = false;
		}
		#endregion
	}
}
