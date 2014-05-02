using System;
using System.ComponentModel;

namespace NServiceKit.Html
{
    /// <summary>Information about the view data.</summary>
	public class ViewDataInfo
	{
		private object value;
		private Func<object> valueAccessor;

        /// <summary>Initializes a new instance of the NServiceKit.Html.ViewDataInfo class.</summary>
		public ViewDataInfo()
		{
		}

        /// <summary>Initializes a new instance of the NServiceKit.Html.ViewDataInfo class.</summary>
        ///
        /// <param name="valueAccessor">The value accessor.</param>
		public ViewDataInfo(Func<object> valueAccessor)
		{
			this.valueAccessor = valueAccessor;
		}

        /// <summary>Gets or sets the container.</summary>
        ///
        /// <value>The container.</value>
		public object Container { get; set; }

        /// <summary>Gets or sets the property descriptor.</summary>
        ///
        /// <value>The property descriptor.</value>
		public PropertyDescriptor PropertyDescriptor { get; set; }

        /// <summary>Gets or sets the value.</summary>
        ///
        /// <value>The value.</value>
		public object Value
		{
			get
			{
				if (valueAccessor != null)
				{
					value = valueAccessor();
					valueAccessor = null;
				}

				return value;
			}
			set
			{
				this.value = value;
				valueAccessor = null;
			}
		}

	}
}
