using System;
using System.Collections.Generic;
using System.Text;
using System.Workflow.ComponentModel.Design;
using System.Workflow.ComponentModel;
using System.ComponentModel.Design;

namespace Mediachase.Ibn.Assignments
{
	/// <summary>
	/// Represents workflow loader.
	/// </summary>
	internal class WorkflowLoader : WorkflowDesignerLoader
	{
		#region Const
		#endregion

		#region Properties
		protected Activity WorkflowDefinition { get; set; }
		#endregion

		#region .Ctor
		/// <summary>
		/// Initializes a new instance of the <see cref="WorkflowLoader"/> class.
		/// </summary>
		/// <param name="workflowDefinition">The workflow definition.</param>
		public WorkflowLoader(Activity workflowDefinition)
		{
			if (workflowDefinition == null)
				throw new ArgumentNullException("workflowDefinition");

			WorkflowDefinition = workflowDefinition;
		}
		#endregion

		#region Methods
		/// <summary>
		/// Loads the designer from a design document.
		/// </summary>
		/// <param name="serializationManager">Class that implements the <see cref="T:System.ComponentModel.Design.Serialization.IDesignerSerializationManager"/> interface, which manages design-time serialization.</param>
		protected override void PerformLoad(System.ComponentModel.Design.Serialization.IDesignerSerializationManager serializationManager)
		{
			base.PerformLoad(serializationManager);

			IDesignerHost designerHost = (IDesignerHost)this.GetService(typeof(IDesignerHost));

			LoadActivity(designerHost, this.WorkflowDefinition);
		}

		/// <summary>
		/// Loads the activity.
		/// </summary>
		/// <param name="designerHost">The designer host.</param>
		/// <param name="activity">The activity.</param>
		private void LoadActivity(IDesignerHost designerHost, Activity activity)
		{
			if (designerHost == null)
				throw new ArgumentNullException("designerHost");
			if (activity == null)
				throw new ArgumentNullException("activity");

			designerHost.Container.Add(activity, activity.QualifiedName);

			CompositeActivity composite = activity as CompositeActivity;

			if (composite != null)
			{
				foreach (Activity childActivity in composite.Activities)
				{
					LoadActivity(designerHost, childActivity);
				}
			}
		}
		#endregion

		/// <summary>
		/// When overridden in a derived class, gets the file name of the designer to load.
		/// </summary>
		/// <value></value>
		/// <returns>A string that contains the file name of the designer to load.</returns>
		public override string FileName
		{
			get { return string.Empty; }
		}

		/// <summary>
		/// When overridden in a derived class, retrieves an object that <see cref="T:System.Workflow.ComponentModel.Design.WorkflowDesignerLoader"/> uses to read from the specified file.
		/// </summary>
		/// <param name="filePath">A string that contains a path to the file to read from.</param>
		/// <returns>
		/// A <see cref="T:System.IO.TextReader"/> to read the specified file.
		/// </returns>
		public override System.IO.TextReader GetFileReader(string filePath)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// When overridden in a derived class, gets an object that the <see cref="T:System.Workflow.ComponentModel.Design.WorkflowDesignerLoader"/> uses to write to the specified file.
		/// </summary>
		/// <param name="filePath">A string that contains the path to the file to write to.</param>
		/// <returns>
		/// A <see cref="T:System.IO.TextWriter"/> to write to the file.
		/// </returns>
		public override System.IO.TextWriter GetFileWriter(string filePath)
		{
			throw new NotImplementedException();
		}
	}
}
