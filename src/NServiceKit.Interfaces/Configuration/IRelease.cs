namespace NServiceKit.Configuration
{
    /// <summary>Interface for release.</summary>
    public interface IRelease
    {
        /// <summary>Releases the given instance.</summary>
        ///
        /// <param name="instance">The instance.</param>
        void Release(object instance);
    }
}