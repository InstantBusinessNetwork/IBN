using System;
using System.Collections.Generic;
using System.Text;

namespace Mediachase.Ibn.Web.Interfaces
{
	/// <summary>
	/// Интерфейс используется для контролов, которые в разных ситуациях отображаются 
	/// под разными заголовками на странице. Вызывается со всех страниц PageTemplate.
	/// </summary>
	public interface IPageTemplateTitle
	{
		string Modify(string oldValue);
	}
}
