using NServiceKit.ServiceHost;

namespace NServiceKit.Razor.Tests
{
    /// <summary>A check box service.</summary>
    public class CheckBoxService : ServiceInterface.Service
    {
        /// <summary>Gets the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>A CheckBoxData.</returns>
        public CheckBoxData Get(GetCheckBox request)
        {
            return new CheckBoxData
                       {
                           BooleanValue = true
                       };
        }

        /// <summary>Post this message.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>A CheckBoxData.</returns>
        public CheckBoxData Post(CheckBoxData request)
        {
            return Get(new GetCheckBox());
        }
    }

    /// <summary>A get check box.</summary>
    [Route("/checkbox", "GET")]
    public class GetCheckBox
    {
        
    }

    /// <summary>A check box data.</summary>
    [Route("/checkbox", "POST")]
    public class CheckBoxData
    {
        /// <summary>Gets or sets a value indicating whether the boolean value.</summary>
        ///
        /// <value>true if boolean value, false if not.</value>
        public bool BooleanValue { get; set; }
    }
}