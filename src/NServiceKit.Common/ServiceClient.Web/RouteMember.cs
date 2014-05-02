using System.Reflection;
using NServiceKit.Text;

namespace NServiceKit.ServiceClient.Web
{
	internal abstract class RouteMember
	{
        /// <summary>Gets a value.</summary>
        ///
        /// <param name="target">        Target for the.</param>
        /// <param name="excludeDefault">true to exclude, false to include the default.</param>
        ///
        /// <returns>The value.</returns>
		public abstract object GetValue(object target, bool excludeDefault = false);
	}

	internal class FieldRouteMember : RouteMember
	{
		private readonly FieldInfo field;

        /// <summary>Initializes a new instance of the NServiceKit.ServiceClient.Web.FieldRouteMember class.</summary>
        ///
        /// <param name="field">The field.</param>
		public FieldRouteMember(FieldInfo field)
		{
			this.field = field;
		}

        /// <summary>Gets a value.</summary>
        ///
        /// <param name="target">        Target for the.</param>
        /// <param name="excludeDefault">true to exclude, false to include the default.</param>
        ///
        /// <returns>The value.</returns>
		public override object GetValue(object target, bool excludeDefault)
		{
			var v = field.GetValue(target);
			if (excludeDefault && Equals(v, field.FieldType.GetDefaultValue())) return null;
			return v;
		}
	}

	internal class PropertyRouteMember : RouteMember
	{
		private readonly PropertyInfo property;

        /// <summary>Initializes a new instance of the NServiceKit.ServiceClient.Web.PropertyRouteMember class.</summary>
        ///
        /// <param name="property">The property.</param>
		public PropertyRouteMember(PropertyInfo property)
		{
			this.property = property;
		}

        /// <summary>Gets a value.</summary>
        ///
        /// <param name="target">        Target for the.</param>
        /// <param name="excludeDefault">true to exclude, false to include the default.</param>
        ///
        /// <returns>The value.</returns>
		public override object GetValue(object target, bool excludeDefault)
		{
			var v = property.GetValue(target, null);
			if (excludeDefault && Equals(v, property.PropertyType.GetDefaultValue())) return null;
			return v;
		}
	}
}