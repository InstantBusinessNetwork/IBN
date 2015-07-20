//------------------------------------------------------------------------------
// Copyright (c) 2003 Mediachase. All Rights Reserved.
//------------------------------------------------------------------------------

namespace Mediachase.Web.UI.WebControls.Design
{
    using System;
	using System.Collections;
	using System.Drawing;
	using System.Windows.Forms;
    using System.ComponentModel;
    using System.Drawing.Design;
    using System.Windows.Forms.Design;
    using System.Web.UI.Design;
    using Mediachase.Web.UI.WebControls;
	using Mediachase.Web.UI.WebControls.Util;

    /// <summary>
    /// Provides an editor for visually picking an URL.
    /// </summary>
	public class CheckBoxEditor : UITypeEditor 
	{
		private IWindowsFormsEditorService edSvc; 

		/// <summary>
		/// Default Constructor
		/// </summary>
		public CheckBoxEditor() : base() 
		{
			this.edSvc = null;
		}

		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			EnumCheckedListBox box1;
			if (((context != null) && (context.Instance != null)) && (provider != null))
			{
				this.edSvc = ((IWindowsFormsEditorService) provider.GetService(typeof(IWindowsFormsEditorService)));
				if (this.edSvc != null)
				{
					box1 = new EnumCheckedListBox(value);
					this.edSvc.DropDownControl(box1);
					value = box1.GetNewValue();
				}
			}
			return value; 
		}

		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
		{
			if ((context != null) && (context.Instance != null))
			{
				return UITypeEditorEditStyle.DropDown; //3; 
			}
			return base.GetEditStyle(context); 
		}

	}

	public class EnumCheckedListBox : CheckedListBox
	{
		private object editValue;
		public EnumCheckedListBox(object editValue) : base()
		{
			this.editValue = null;
			this.editValue = editValue;
			base.BorderStyle = 0;
			this.FillList();
		}

		public object GetNewValue()
		{
			int num1;
			object obj1;
			int num2;
			IEnumerator enumerator1;
			IDisposable disposable1;
			num1 = 0;
			enumerator1 = base.CheckedItems.GetEnumerator();
			try
			{
				while (enumerator1.MoveNext())
				{
					obj1 = enumerator1.Current;
					num2 = ((int) Enum.Parse(this.editValue.GetType(), ((string) obj1)));
					num1 = (num1 | num2);
				}
			}
			finally
			{
				disposable1 = (enumerator1 as IDisposable);
				if (disposable1 != null)
				{
					disposable1.Dispose();
				}
			}
			return Enum.ToObject(this.editValue.GetType(), num1); 
		}

		private void FillList()
		{
			int num1;
			object obj1;
			int num2;
			bool flag1;
			IEnumerator enumerator1;
			IDisposable disposable1;
			if ((this.editValue as Enum) == null)
			{
				throw new ArgumentException("UI type editor may be set for the enumerations only.");
			}
			if (Enum.GetUnderlyingType(this.editValue.GetType()) != typeof(int))
			{
				throw new ArgumentException("UI type editor may be set for the enumerations with Int32 underlying type only.");
			}
			num1 = ((int) this.editValue);
			enumerator1 = Enum.GetValues(this.editValue.GetType()).GetEnumerator();
			try
			{
				while (enumerator1.MoveNext())
				{
					obj1 = enumerator1.Current;
					num2 = ((int) obj1);
					if (num2 != 0)
					{
						flag1 = ((num1 & num2) == num2);
						base.Items.Add(Enum.GetName(this.editValue.GetType(), obj1), flag1);
					}
				}
			}
			finally
			{
				disposable1 = (enumerator1 as IDisposable);
				if (disposable1 != null)
				{
					disposable1.Dispose();
				}
			}
		}
	}
}
