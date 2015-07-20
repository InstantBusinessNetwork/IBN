using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Ibn.Assignments.Schemas;

namespace Mediachase.Ibn.Assignments
{
	/// <summary>
	/// Represents create assignment and wait result activity instance factory.
	/// </summary>
	public class CreateAssignmentAndWaitResultActivityInstanceFactory : ObjectInstanceFactory
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CreateAssignmentAndWaitResultActivityInstanceFactory"/> class.
		/// </summary>
		public CreateAssignmentAndWaitResultActivityInstanceFactory()
		{
			this.UseRequest = false;
		}

		#region Properties
		/// <summary>
		/// Gets or sets the assignment properties.
		/// </summary>
		/// <value>The assignment properties.</value>
		public PropertyValueCollection AssignmentProperties
		{ get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether [use request].
		/// </summary>
		/// <value><c>true</c> if [use request]; otherwise, <c>false</c>.</value>
		public bool UseRequest
		{ get; set; }

		/// <summary>
		/// Gets or sets the request properties.
		/// </summary>
		/// <value>The request properties.</value>
		public PropertyValueCollection RequestProperties
		{ get; set; }

		/// <summary>
		/// Gets or sets the requested users.
		/// </summary>
		/// <value>The requested users.</value>
		public object RequestedUsers
		{ get; set; }
		#endregion

		#region ObjectInstanceFactory Members
		/// <summary>
		/// Creates the instance.
		/// </summary>
		/// <param name="master">The master.</param>
		/// <returns></returns>
		public override object CreateInstance()
		{
			CreateAssignmentAndWaitResultActivity retVal = new CreateAssignmentAndWaitResultActivity();
			retVal.Name = "createAssignmentAndWait_"+ Guid.NewGuid().ToString("N");

			retVal.AssignmentProperties = new PropertyValueCollection();
			retVal.AssignmentProperties.AddRange(this.AssignmentProperties);

			retVal.UseRequest = this.UseRequest;

			if (this.UseRequest)
			{
				retVal.RequestProperties = new PropertyValueCollection();

				if(this.RequestProperties!=null)
					retVal.RequestProperties.AddRange(this.RequestProperties);

				retVal.RequestedUsers = this.RequestedUsers;
			}

			// Copy Properties
			return retVal;
		}

		#endregion


	}
}
