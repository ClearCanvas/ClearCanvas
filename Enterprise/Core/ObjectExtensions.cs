using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClearCanvas.Enterprise.Core
{
	internal static class ObjectExtensions
	{
		internal static Type GetClass(this object obj)
		{
			var domainObj = obj as DomainObject;
			return domainObj != null ? domainObj.GetClass() : obj.GetType();
		}
	}
}
