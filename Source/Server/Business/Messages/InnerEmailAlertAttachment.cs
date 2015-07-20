using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Alert.Service;
using System.IO;

namespace Mediachase.Ibn.Business.Messages
{
	/// <summary>
	/// Represents InnerEmailAlertAttachment.
	/// </summary>
	//internal sealed class InnerEmailAlertAttachment : IEmailAlertAttachment
	//{
	//    #region Const
	//    #endregion

	//    #region Fields
	//    #endregion

	//    #region .Ctor
	//    /// <summary>
	//    /// Initializes a new instance of the <see cref="InnerEmailAlertAttachment"/> class.
	//    /// </summary>
	//    /// <param name="fileName">Name of the file.</param>
	//    public InnerEmailAlertAttachment(string fileName)
	//    {
	//        this.FileName = fileName;
	//        this.IsComplete = false;
	//        this.InnerStream = new MemoryStream();
	//    }
	//    #endregion

	//    #region Properties

	//    /// <summary>
	//    /// Gets or sets a value indicating whether this instance is complete.
	//    /// </summary>
	//    /// <value>
	//    /// 	<c>true</c> if this instance is complete; otherwise, <c>false</c>.
	//    /// </value>
	//    public bool IsComplete { get; private set; }

	//    /// <summary>
	//    /// Gets or sets the inner stream.
	//    /// </summary>
	//    /// <value>The inner stream.</value>
	//    public MemoryStream InnerStream { get; private set; }

	//    /// <summary>
	//    /// Gets or sets the name of the file.
	//    /// </summary>
	//    /// <value>The name of the file.</value>
	//    public string FileName { get; private set; }
		
	//    #endregion

	//    #region Methods
	//    #endregion

	//    #region IEmailAlertAttachment Members

	//    /// <summary>
	//    /// Completes this instance.
	//    /// </summary>
	//    void IEmailAlertAttachment.Complete()
	//    {
	//        this.IsComplete = true;
	//    }

	//    /// <summary>
	//    /// Gets a value indicating whether this instance is complete.
	//    /// </summary>
	//    /// <value>
	//    /// 	<c>true</c> if this instance is complete; otherwise, <c>false</c>.
	//    /// </value>
	//    bool IEmailAlertAttachment.IsComplete
	//    {
	//        get { return this.IsComplete; }
	//    }

	//    /// <summary>
	//    /// Writes the specified data.
	//    /// </summary>
	//    /// <param name="data">The data.</param>
	//    /// <param name="count">The count.</param>
	//    void IEmailAlertAttachment.Write(byte[] data, int count)
	//    {
	//        this.InnerStream.Write(data, 0, count);
	//    }

	//    #endregion
	//}
}
