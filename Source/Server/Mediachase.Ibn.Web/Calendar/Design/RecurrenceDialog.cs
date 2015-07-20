using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace Mediachase.Web.UI.WebControls.Design
{
	/// <summary>
	/// Summary description for RecurrenceDialog.
	/// </summary>
	public class RecurrenceDialog : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.DateTimePicker TimeStart;
		private System.Windows.Forms.DateTimePicker dateTimePicker1;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.RadioButton DailyRadio;
		private System.Windows.Forms.RadioButton WeeklyRadio;
		private System.Windows.Forms.RadioButton MonthlyRadio;
		private System.Windows.Forms.RadioButton YearlyRadio;
		private System.Windows.Forms.Panel DailyPanel;
		private System.Windows.Forms.RadioButton DRecType1;
		private System.Windows.Forms.RadioButton DRecType2;
		private System.Windows.Forms.TextBox DDayFreq;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Panel MonthlyPanel;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.TextBox M1;
		private System.Windows.Forms.RadioButton MRecType2;
		private System.Windows.Forms.RadioButton MRecType1;
		private System.Windows.Forms.TextBox M2;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.ComboBox MDaySel;
		private System.Windows.Forms.ComboBox DayNameSel;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.TextBox M3;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.Label label13;
		private System.Windows.Forms.ComboBox YDayNameSel;
		private System.Windows.Forms.ComboBox YDaySel1;
		private System.Windows.Forms.TextBox RecMonth;
		private System.Windows.Forms.RadioButton radioButton1;
		private System.Windows.Forms.RadioButton YRecType1;
		private System.Windows.Forms.ComboBox YRecMonth;
		private System.Windows.Forms.ComboBox YMonthSel;
		private System.Windows.Forms.DateTimePicker RStartDateTime;
		private System.Windows.Forms.Label label14;
		private System.Windows.Forms.RadioButton RType1;
		private System.Windows.Forms.RadioButton RType2;
		private System.Windows.Forms.RadioButton RType3;
		private System.Windows.Forms.DateTimePicker dateTimePicker2;
		private System.Windows.Forms.TextBox REndAfter;
		private System.Windows.Forms.Label label15;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Panel WeeklyPanel;
		private System.Windows.Forms.CheckBox WSelDay4;
		private System.Windows.Forms.CheckBox WSelDay3;
		private System.Windows.Forms.CheckBox WSelDay7;
		private System.Windows.Forms.CheckBox WSelDay6;
		private System.Windows.Forms.CheckBox WSelDay5;
		private System.Windows.Forms.CheckBox WSelDay2;
		private System.Windows.Forms.CheckBox WSelDay1;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TextBox WeekFreq;
		private System.Windows.Forms.Panel YearlyPanel;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		/// <summary>
		/// 
		/// </summary>
		public RecurrenceDialog()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();


			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		private void HideAllPanels()
		{
			DailyPanel.Visible = false;
			WeeklyPanel.Visible = false;
			MonthlyPanel.Visible = false;
			YearlyPanel.Visible = false;
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.TimeStart = new System.Windows.Forms.DateTimePicker();
			this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
			this.label3 = new System.Windows.Forms.Label();
			this.DailyRadio = new System.Windows.Forms.RadioButton();
			this.WeeklyRadio = new System.Windows.Forms.RadioButton();
			this.MonthlyRadio = new System.Windows.Forms.RadioButton();
			this.YearlyRadio = new System.Windows.Forms.RadioButton();
			this.DailyPanel = new System.Windows.Forms.Panel();
			this.label5 = new System.Windows.Forms.Label();
			this.DDayFreq = new System.Windows.Forms.TextBox();
			this.DRecType2 = new System.Windows.Forms.RadioButton();
			this.DRecType1 = new System.Windows.Forms.RadioButton();
			this.MonthlyPanel = new System.Windows.Forms.Panel();
			this.label11 = new System.Windows.Forms.Label();
			this.M3 = new System.Windows.Forms.TextBox();
			this.label10 = new System.Windows.Forms.Label();
			this.DayNameSel = new System.Windows.Forms.ComboBox();
			this.MDaySel = new System.Windows.Forms.ComboBox();
			this.label9 = new System.Windows.Forms.Label();
			this.M2 = new System.Windows.Forms.TextBox();
			this.label8 = new System.Windows.Forms.Label();
			this.M1 = new System.Windows.Forms.TextBox();
			this.MRecType2 = new System.Windows.Forms.RadioButton();
			this.MRecType1 = new System.Windows.Forms.RadioButton();
			this.YearlyPanel = new System.Windows.Forms.Panel();
			this.YMonthSel = new System.Windows.Forms.ComboBox();
			this.YRecMonth = new System.Windows.Forms.ComboBox();
			this.label13 = new System.Windows.Forms.Label();
			this.YDayNameSel = new System.Windows.Forms.ComboBox();
			this.YDaySel1 = new System.Windows.Forms.ComboBox();
			this.RecMonth = new System.Windows.Forms.TextBox();
			this.radioButton1 = new System.Windows.Forms.RadioButton();
			this.YRecType1 = new System.Windows.Forms.RadioButton();
			this.RStartDateTime = new System.Windows.Forms.DateTimePicker();
			this.label14 = new System.Windows.Forms.Label();
			this.RType1 = new System.Windows.Forms.RadioButton();
			this.RType2 = new System.Windows.Forms.RadioButton();
			this.RType3 = new System.Windows.Forms.RadioButton();
			this.dateTimePicker2 = new System.Windows.Forms.DateTimePicker();
			this.REndAfter = new System.Windows.Forms.TextBox();
			this.label15 = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.WeeklyPanel = new System.Windows.Forms.Panel();
			this.WSelDay4 = new System.Windows.Forms.CheckBox();
			this.WSelDay3 = new System.Windows.Forms.CheckBox();
			this.WSelDay7 = new System.Windows.Forms.CheckBox();
			this.WSelDay6 = new System.Windows.Forms.CheckBox();
			this.WSelDay5 = new System.Windows.Forms.CheckBox();
			this.WSelDay2 = new System.Windows.Forms.CheckBox();
			this.WSelDay1 = new System.Windows.Forms.CheckBox();
			this.label7 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.WeekFreq = new System.Windows.Forms.TextBox();
			this.DailyPanel.SuspendLayout();
			this.MonthlyPanel.SuspendLayout();
			this.YearlyPanel.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.WeeklyPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(204)));
			this.label1.Location = new System.Drawing.Point(16, 16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(264, 16);
			this.label1.TabIndex = 0;
			this.label1.Text = "Select Recurrence Times:";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(16, 40);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(32, 16);
			this.label2.TabIndex = 1;
			this.label2.Text = "Start:";
			// 
			// TimeStart
			// 
			this.TimeStart.Format = System.Windows.Forms.DateTimePickerFormat.Time;
			this.TimeStart.Location = new System.Drawing.Point(54, 37);
			this.TimeStart.Name = "TimeStart";
			this.TimeStart.Size = new System.Drawing.Size(88, 20);
			this.TimeStart.TabIndex = 3;
			// 
			// dateTimePicker1
			// 
			this.dateTimePicker1.Format = System.Windows.Forms.DateTimePickerFormat.Time;
			this.dateTimePicker1.Location = new System.Drawing.Point(184, 38);
			this.dateTimePicker1.Name = "dateTimePicker1";
			this.dateTimePicker1.Size = new System.Drawing.Size(88, 20);
			this.dateTimePicker1.TabIndex = 5;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(152, 40);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(32, 16);
			this.label3.TabIndex = 4;
			this.label3.Text = "End:";
			// 
			// DailyRadio
			// 
			this.DailyRadio.Checked = true;
			this.DailyRadio.Location = new System.Drawing.Point(24, 96);
			this.DailyRadio.Name = "DailyRadio";
			this.DailyRadio.Size = new System.Drawing.Size(72, 16);
			this.DailyRadio.TabIndex = 7;
			this.DailyRadio.TabStop = true;
			this.DailyRadio.Text = "Daily";
			this.DailyRadio.CheckedChanged += new System.EventHandler(this.DailyRadio_CheckedChanged);
			// 
			// WeeklyRadio
			// 
			this.WeeklyRadio.Location = new System.Drawing.Point(24, 118);
			this.WeeklyRadio.Name = "WeeklyRadio";
			this.WeeklyRadio.Size = new System.Drawing.Size(72, 16);
			this.WeeklyRadio.TabIndex = 8;
			this.WeeklyRadio.Text = "Weekly";
			this.WeeklyRadio.CheckedChanged += new System.EventHandler(this.WeeklyRadio_CheckedChanged);
			// 
			// MonthlyRadio
			// 
			this.MonthlyRadio.Location = new System.Drawing.Point(24, 140);
			this.MonthlyRadio.Name = "MonthlyRadio";
			this.MonthlyRadio.Size = new System.Drawing.Size(72, 16);
			this.MonthlyRadio.TabIndex = 9;
			this.MonthlyRadio.Text = "Monthly";
			this.MonthlyRadio.CheckedChanged += new System.EventHandler(this.MonthlyRadio_CheckedChanged);
			// 
			// YearlyRadio
			// 
			this.YearlyRadio.Location = new System.Drawing.Point(24, 162);
			this.YearlyRadio.Name = "YearlyRadio";
			this.YearlyRadio.Size = new System.Drawing.Size(72, 16);
			this.YearlyRadio.TabIndex = 10;
			this.YearlyRadio.Text = "Yearly";
			// 
			// DailyPanel
			// 
			this.DailyPanel.Controls.AddRange(new System.Windows.Forms.Control[] {
																					 this.label5,
																					 this.DDayFreq,
																					 this.DRecType2,
																					 this.DRecType1});
			this.DailyPanel.Location = new System.Drawing.Point(112, 88);
			this.DailyPanel.Name = "DailyPanel";
			this.DailyPanel.Size = new System.Drawing.Size(424, 112);
			this.DailyPanel.TabIndex = 11;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(112, 20);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(40, 16);
			this.label5.TabIndex = 3;
			this.label5.Text = "day(s)";
			// 
			// DDayFreq
			// 
			this.DDayFreq.Location = new System.Drawing.Point(64, 18);
			this.DDayFreq.Name = "DDayFreq";
			this.DDayFreq.Size = new System.Drawing.Size(40, 20);
			this.DDayFreq.TabIndex = 2;
			this.DDayFreq.Text = "1";
			// 
			// DRecType2
			// 
			this.DRecType2.Location = new System.Drawing.Point(8, 40);
			this.DRecType2.Name = "DRecType2";
			this.DRecType2.TabIndex = 1;
			this.DRecType2.Text = "Every weekday";
			// 
			// DRecType1
			// 
			this.DRecType1.Checked = true;
			this.DRecType1.Location = new System.Drawing.Point(8, 16);
			this.DRecType1.Name = "DRecType1";
			this.DRecType1.TabIndex = 0;
			this.DRecType1.TabStop = true;
			this.DRecType1.Text = "Every";
			// 
			// MonthlyPanel
			// 
			this.MonthlyPanel.Controls.AddRange(new System.Windows.Forms.Control[] {
																					   this.label11,
																					   this.M3,
																					   this.label10,
																					   this.DayNameSel,
																					   this.MDaySel,
																					   this.label9,
																					   this.M2,
																					   this.label8,
																					   this.M1,
																					   this.MRecType2,
																					   this.MRecType1});
			this.MonthlyPanel.Location = new System.Drawing.Point(112, 88);
			this.MonthlyPanel.Name = "MonthlyPanel";
			this.MonthlyPanel.Size = new System.Drawing.Size(424, 112);
			this.MonthlyPanel.TabIndex = 13;
			// 
			// label11
			// 
			this.label11.Location = new System.Drawing.Point(320, 52);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(104, 16);
			this.label11.TabIndex = 11;
			this.label11.Text = "month(s)";
			// 
			// M3
			// 
			this.M3.Location = new System.Drawing.Point(281, 50);
			this.M3.Name = "M3";
			this.M3.Size = new System.Drawing.Size(32, 20);
			this.M3.TabIndex = 10;
			this.M3.Text = "1";
			// 
			// label10
			// 
			this.label10.Location = new System.Drawing.Point(236, 53);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(48, 16);
			this.label10.TabIndex = 9;
			this.label10.Text = "of every ";
			// 
			// DayNameSel
			// 
			this.DayNameSel.Items.AddRange(new object[] {
															"Monday",
															"Tuesday",
															"Wednesday",
															"Thursday",
															"Friday",
															"Saturday",
															"Sunday"});
			this.DayNameSel.Location = new System.Drawing.Point(136, 50);
			this.DayNameSel.Name = "DayNameSel";
			this.DayNameSel.Size = new System.Drawing.Size(96, 21);
			this.DayNameSel.TabIndex = 8;
			// 
			// MDaySel
			// 
			this.MDaySel.Items.AddRange(new object[] {
														 "First",
														 "Second",
														 "Third",
														 "Fourth",
														 "Fifth",
														 "Last"});
			this.MDaySel.Location = new System.Drawing.Point(60, 50);
			this.MDaySel.Name = "MDaySel";
			this.MDaySel.Size = new System.Drawing.Size(68, 21);
			this.MDaySel.TabIndex = 7;
			// 
			// label9
			// 
			this.label9.Location = new System.Drawing.Point(208, 19);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(72, 16);
			this.label9.TabIndex = 5;
			this.label9.Text = "month(s)";
			// 
			// M2
			// 
			this.M2.Location = new System.Drawing.Point(160, 18);
			this.M2.Name = "M2";
			this.M2.Size = new System.Drawing.Size(40, 20);
			this.M2.TabIndex = 4;
			this.M2.Text = "1";
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(112, 20);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(48, 16);
			this.label8.TabIndex = 3;
			this.label8.Text = "of every";
			// 
			// M1
			// 
			this.M1.Location = new System.Drawing.Point(64, 18);
			this.M1.Name = "M1";
			this.M1.Size = new System.Drawing.Size(40, 20);
			this.M1.TabIndex = 2;
			this.M1.Text = "1";
			// 
			// MRecType2
			// 
			this.MRecType2.Location = new System.Drawing.Point(8, 48);
			this.MRecType2.Name = "MRecType2";
			this.MRecType2.Size = new System.Drawing.Size(56, 24);
			this.MRecType2.TabIndex = 1;
			this.MRecType2.Text = "The";
			// 
			// MRecType1
			// 
			this.MRecType1.Checked = true;
			this.MRecType1.Location = new System.Drawing.Point(8, 17);
			this.MRecType1.Name = "MRecType1";
			this.MRecType1.TabIndex = 0;
			this.MRecType1.TabStop = true;
			this.MRecType1.Text = "Day";
			// 
			// YearlyPanel
			// 
			this.YearlyPanel.Controls.AddRange(new System.Windows.Forms.Control[] {
																					  this.YMonthSel,
																					  this.YRecMonth,
																					  this.label13,
																					  this.YDayNameSel,
																					  this.YDaySel1,
																					  this.RecMonth,
																					  this.radioButton1,
																					  this.YRecType1});
			this.YearlyPanel.Location = new System.Drawing.Point(112, 88);
			this.YearlyPanel.Name = "YearlyPanel";
			this.YearlyPanel.Size = new System.Drawing.Size(424, 112);
			this.YearlyPanel.TabIndex = 14;
			// 
			// YMonthSel
			// 
			this.YMonthSel.Items.AddRange(new object[] {
														   "January",
														   "February",
														   "March",
														   "April",
														   "May",
														   "June",
														   "July",
														   "August",
														   "September",
														   "October",
														   "November",
														   "December"});
			this.YMonthSel.Location = new System.Drawing.Point(256, 50);
			this.YMonthSel.Name = "YMonthSel";
			this.YMonthSel.Size = new System.Drawing.Size(96, 21);
			this.YMonthSel.TabIndex = 13;
			// 
			// YRecMonth
			// 
			this.YRecMonth.Items.AddRange(new object[] {
														   "January",
														   "February",
														   "March",
														   "April",
														   "May",
														   "June",
														   "July",
														   "August",
														   "September",
														   "October",
														   "November",
														   "December"});
			this.YRecMonth.Location = new System.Drawing.Point(56, 18);
			this.YRecMonth.Name = "YRecMonth";
			this.YRecMonth.Size = new System.Drawing.Size(96, 21);
			this.YRecMonth.TabIndex = 12;
			// 
			// label13
			// 
			this.label13.Location = new System.Drawing.Point(236, 53);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(20, 16);
			this.label13.TabIndex = 9;
			this.label13.Text = "of";
			// 
			// YDayNameSel
			// 
			this.YDayNameSel.Items.AddRange(new object[] {
															 "Monday",
															 "Tuesday",
															 "Wednesday",
															 "Thursday",
															 "Friday",
															 "Saturday",
															 "Sunday"});
			this.YDayNameSel.Location = new System.Drawing.Point(136, 50);
			this.YDayNameSel.Name = "YDayNameSel";
			this.YDayNameSel.Size = new System.Drawing.Size(96, 21);
			this.YDayNameSel.TabIndex = 8;
			// 
			// YDaySel1
			// 
			this.YDaySel1.Items.AddRange(new object[] {
														  "First",
														  "Second",
														  "Third",
														  "Fourth",
														  "Fifth",
														  "Last"});
			this.YDaySel1.Location = new System.Drawing.Point(60, 50);
			this.YDaySel1.Name = "YDaySel1";
			this.YDaySel1.Size = new System.Drawing.Size(68, 21);
			this.YDaySel1.TabIndex = 7;
			// 
			// RecMonth
			// 
			this.RecMonth.Location = new System.Drawing.Point(160, 18);
			this.RecMonth.Name = "RecMonth";
			this.RecMonth.Size = new System.Drawing.Size(40, 20);
			this.RecMonth.TabIndex = 4;
			this.RecMonth.Text = "1";
			// 
			// radioButton1
			// 
			this.radioButton1.Location = new System.Drawing.Point(8, 48);
			this.radioButton1.Name = "radioButton1";
			this.radioButton1.Size = new System.Drawing.Size(56, 24);
			this.radioButton1.TabIndex = 1;
			this.radioButton1.Text = "The";
			// 
			// YRecType1
			// 
			this.YRecType1.Checked = true;
			this.YRecType1.Location = new System.Drawing.Point(8, 17);
			this.YRecType1.Name = "YRecType1";
			this.YRecType1.TabIndex = 0;
			this.YRecType1.TabStop = true;
			this.YRecType1.Text = "Every";
			// 
			// RStartDateTime
			// 
			this.RStartDateTime.Format = System.Windows.Forms.DateTimePickerFormat.Time;
			this.RStartDateTime.Location = new System.Drawing.Point(54, 21);
			this.RStartDateTime.Name = "RStartDateTime";
			this.RStartDateTime.Size = new System.Drawing.Size(88, 20);
			this.RStartDateTime.TabIndex = 17;
			// 
			// label14
			// 
			this.label14.Location = new System.Drawing.Point(16, 24);
			this.label14.Name = "label14";
			this.label14.Size = new System.Drawing.Size(32, 16);
			this.label14.TabIndex = 16;
			this.label14.Text = "Start:";
			// 
			// RType1
			// 
			this.RType1.Checked = true;
			this.RType1.Location = new System.Drawing.Point(160, 21);
			this.RType1.Name = "RType1";
			this.RType1.TabIndex = 18;
			this.RType1.TabStop = true;
			this.RType1.Text = "No end date";
			// 
			// RType2
			// 
			this.RType2.Location = new System.Drawing.Point(160, 45);
			this.RType2.Name = "RType2";
			this.RType2.TabIndex = 19;
			this.RType2.Text = "End after:";
			// 
			// RType3
			// 
			this.RType3.Location = new System.Drawing.Point(160, 69);
			this.RType3.Name = "RType3";
			this.RType3.TabIndex = 20;
			this.RType3.Text = "End by:";
			// 
			// dateTimePicker2
			// 
			this.dateTimePicker2.Format = System.Windows.Forms.DateTimePickerFormat.Time;
			this.dateTimePicker2.Location = new System.Drawing.Point(234, 71);
			this.dateTimePicker2.Name = "dateTimePicker2";
			this.dateTimePicker2.Size = new System.Drawing.Size(88, 20);
			this.dateTimePicker2.TabIndex = 21;
			// 
			// REndAfter
			// 
			this.REndAfter.Location = new System.Drawing.Point(234, 46);
			this.REndAfter.Name = "REndAfter";
			this.REndAfter.Size = new System.Drawing.Size(32, 20);
			this.REndAfter.TabIndex = 22;
			this.REndAfter.Text = "";
			// 
			// label15
			// 
			this.label15.Location = new System.Drawing.Point(272, 48);
			this.label15.Name = "label15";
			this.label15.Size = new System.Drawing.Size(100, 12);
			this.label15.TabIndex = 23;
			this.label15.Text = "occurrences";
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.AddRange(new System.Windows.Forms.Control[] {
																					this.REndAfter,
																					this.dateTimePicker2,
																					this.label14,
																					this.RType2,
																					this.RType3,
																					this.RStartDateTime,
																					this.label15,
																					this.RType1});
			this.groupBox1.Location = new System.Drawing.Point(8, 224);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(536, 104);
			this.groupBox1.TabIndex = 24;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Range if Recurrence";
			// 
			// groupBox2
			// 
			this.groupBox2.Location = new System.Drawing.Point(8, 72);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(536, 144);
			this.groupBox2.TabIndex = 25;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Recurrence Pattern";
			// 
			// WeeklyPanel
			// 
			this.WeeklyPanel.Controls.AddRange(new System.Windows.Forms.Control[] {
																					  this.WSelDay4,
																					  this.WSelDay3,
																					  this.WSelDay7,
																					  this.WSelDay6,
																					  this.WSelDay5,
																					  this.WSelDay2,
																					  this.WSelDay1,
																					  this.label7,
																					  this.label6,
																					  this.WeekFreq});
			this.WeeklyPanel.Location = new System.Drawing.Point(112, 88);
			this.WeeklyPanel.Name = "WeeklyPanel";
			this.WeeklyPanel.Size = new System.Drawing.Size(328, 112);
			this.WeeklyPanel.TabIndex = 12;
			// 
			// WSelDay4
			// 
			this.WSelDay4.Location = new System.Drawing.Point(246, 50);
			this.WSelDay4.Name = "WSelDay4";
			this.WSelDay4.Size = new System.Drawing.Size(74, 16);
			this.WSelDay4.TabIndex = 11;
			this.WSelDay4.Text = "Thursday";
			// 
			// WSelDay3
			// 
			this.WSelDay3.Location = new System.Drawing.Point(160, 50);
			this.WSelDay3.Name = "WSelDay3";
			this.WSelDay3.Size = new System.Drawing.Size(88, 16);
			this.WSelDay3.TabIndex = 10;
			this.WSelDay3.Text = "Wednesday";
			// 
			// WSelDay7
			// 
			this.WSelDay7.Location = new System.Drawing.Point(160, 76);
			this.WSelDay7.Name = "WSelDay7";
			this.WSelDay7.Size = new System.Drawing.Size(64, 16);
			this.WSelDay7.TabIndex = 9;
			this.WSelDay7.Text = "Sunday";
			// 
			// WSelDay6
			// 
			this.WSelDay6.Location = new System.Drawing.Point(87, 76);
			this.WSelDay6.Name = "WSelDay6";
			this.WSelDay6.Size = new System.Drawing.Size(73, 16);
			this.WSelDay6.TabIndex = 8;
			this.WSelDay6.Text = "Saturday";
			// 
			// WSelDay5
			// 
			this.WSelDay5.Location = new System.Drawing.Point(17, 76);
			this.WSelDay5.Name = "WSelDay5";
			this.WSelDay5.Size = new System.Drawing.Size(64, 16);
			this.WSelDay5.TabIndex = 7;
			this.WSelDay5.Text = "Friday";
			// 
			// WSelDay2
			// 
			this.WSelDay2.Location = new System.Drawing.Point(87, 49);
			this.WSelDay2.Name = "WSelDay2";
			this.WSelDay2.Size = new System.Drawing.Size(72, 16);
			this.WSelDay2.TabIndex = 6;
			this.WSelDay2.Text = "Tuesday";
			// 
			// WSelDay1
			// 
			this.WSelDay1.Location = new System.Drawing.Point(16, 49);
			this.WSelDay1.Name = "WSelDay1";
			this.WSelDay1.Size = new System.Drawing.Size(64, 16);
			this.WSelDay1.TabIndex = 5;
			this.WSelDay1.Text = "Monday";
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(8, 16);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(72, 16);
			this.label7.TabIndex = 4;
			this.label7.Text = "Recure every";
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(128, 16);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(64, 16);
			this.label6.TabIndex = 3;
			this.label6.Text = "week(s) on:";
			// 
			// WeekFreq
			// 
			this.WeekFreq.Location = new System.Drawing.Point(80, 14);
			this.WeekFreq.Name = "WeekFreq";
			this.WeekFreq.Size = new System.Drawing.Size(40, 20);
			this.WeekFreq.TabIndex = 2;
			this.WeekFreq.Text = "1";
			// 
			// RecurrenceDialog
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(552, 336);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.YearlyRadio,
																		  this.MonthlyRadio,
																		  this.WeeklyRadio,
																		  this.DailyRadio,
																		  this.dateTimePicker1,
																		  this.label3,
																		  this.TimeStart,
																		  this.label2,
																		  this.label1,
																		  this.groupBox1,
																		  this.DailyPanel,
																		  this.YearlyPanel,
																		  this.MonthlyPanel,
																		  this.WeeklyPanel,
																		  this.groupBox2});
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "RecurrenceDialog";
			this.Text = "Recurrence Pattern";
			this.DailyPanel.ResumeLayout(false);
			this.MonthlyPanel.ResumeLayout(false);
			this.YearlyPanel.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.WeeklyPanel.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void DailyRadio_CheckedChanged(object sender, System.EventArgs e)
		{
			this.HideAllPanels();
			DailyPanel.Visible = true;
			DailyPanel.BringToFront();
		}

		private void WeeklyRadio_CheckedChanged(object sender, System.EventArgs e)
		{
			this.HideAllPanels();
			WeeklyPanel.Visible = true;
			WeeklyPanel.BringToFront();
		}

		private void MonthlyRadio_CheckedChanged(object sender, System.EventArgs e)
		{
			this.HideAllPanels();
			MonthlyPanel.Visible = true;	
			MonthlyPanel.BringToFront();
		}

		private void YearlyRadio_CheckedChanged(object sender, System.EventArgs e)
		{
			this.HideAllPanels();
			YearlyPanel.Visible = true;
			YearlyPanel.BringToFront();
		}
	}
}
