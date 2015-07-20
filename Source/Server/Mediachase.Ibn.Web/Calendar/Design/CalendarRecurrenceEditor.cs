using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Web.UI.Design;
using System.Windows.Forms.Design;
using System.Windows.Forms;

namespace Mediachase.Web.UI.WebControls.Design
{
	/// <summary>
	/// Recurrence editor
	/// </summary>
	public class CalendarRecurrenceEditor : UITypeEditor
	{

		/// <summary>
		/// Recurrence constructor
		/// </summary>
		public CalendarRecurrenceEditor() : base()
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="context"></param>
		/// <param name="provider"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public override object EditValue(ITypeDescriptorContext context,
			IServiceProvider provider, object value)
		{
			if(provider != null)
			{
				IWindowsFormsEditorService winEdit = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
				ISite site = null;
				if(winEdit != null)
				{
					if(context.Instance is IComponent)
					{
						site = ((IComponent)context.Instance).Site;
					} 
					else if(context.Instance is object[] &&
						((object[])context.Instance)[0] is IComponent)
					{
						site = ((IComponent)(((object[])context.Instance)[0])).Site;
					}
					RecurrenceDialog dlg = new RecurrenceDialog();
					//dlg.RegularExpression = value.ToString();
					DialogResult res = dlg.ShowDialog();
					//if(res == DialogResult.OK)
					//	value = dlg.RegularExpression;
					
				}
			}
			return value;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		public override UITypeEditorEditStyle GetEditStyle(
			ITypeDescriptorContext context)
		{
			return UITypeEditorEditStyle.Modal;
		}
	}
} 