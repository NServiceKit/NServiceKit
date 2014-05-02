using System;

namespace Funq
{
	internal sealed class ServiceKey
	{
		int hash;

        /// <summary>Initializes a new instance of the Funq.ServiceKey class.</summary>
        ///
        /// <param name="factoryType">Type of the factory.</param>
        /// <param name="serviceName">Name of the service.</param>
		public ServiceKey(Type factoryType, string serviceName)
		{
			FactoryType = factoryType;
			Name = serviceName;

			hash = factoryType.GetHashCode();
			if (serviceName != null)
				hash ^= serviceName.GetHashCode();
		}

        /// <summary>Type of the factory.</summary>
		public Type FactoryType;
        /// <summary>The name.</summary>
		public string Name;

		#region Equality

        /// <summary>Determines whether the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:System.Object" />.</summary>
        ///
        /// <param name="other">The service key to compare to this object.</param>
        ///
        /// <returns>true if the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:System.Object" />; otherwise, false.</returns>
		public bool Equals(ServiceKey other)
		{
			return ServiceKey.Equals(this, other);
		}

        /// <summary>Determines whether the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:System.Object" />.</summary>
        ///
        /// <param name="obj">The <see cref="T:System.Object" /> to compare with the current <see cref="T:System.Object" />.</param>
        ///
        /// <returns>true if the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:System.Object" />; otherwise, false.</returns>
		public override bool Equals(object obj)
		{
			return ServiceKey.Equals(this, obj as ServiceKey);
		}

        /// <summary>Tests if two ServiceKey objects are considered equal.</summary>
        ///
        /// <param name="obj1">Service key to be compared.</param>
        /// <param name="obj2">Service key to be compared.</param>
        ///
        /// <returns>true if the objects are considered equal, false if they are not.</returns>
		public static bool Equals(ServiceKey obj1, ServiceKey obj2)
		{
			if (Object.Equals(null, obj1) ||
				Object.Equals(null, obj2))
				return false;

			return obj1.FactoryType == obj2.FactoryType && 
				obj1.Name == obj2.Name;
		}

        /// <summary>Serves as a hash function for a particular type.</summary>
        ///
        /// <returns>A hash code for the current <see cref="T:System.Object" />.</returns>
		public override int GetHashCode()
		{
			return hash;
		}

		#endregion
	}
}