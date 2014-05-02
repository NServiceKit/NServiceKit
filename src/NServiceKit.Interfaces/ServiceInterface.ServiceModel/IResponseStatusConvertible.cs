namespace NServiceKit.ServiceInterface.ServiceModel
{
    /// <summary>Interface for response status convertible.</summary>
    public interface IResponseStatusConvertible
    {
        /// <summary>Converts this object to a response status.</summary>
        ///
        /// <returns>This object as the ResponseStatus.</returns>
        ResponseStatus ToResponseStatus();
    }
}