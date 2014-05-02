using System;
using System.Web;
using System.Web.UI.WebControls;

namespace NServiceKit.WebHost.Endpoints.Metadata
{
    /// <summary>A base wsdl page.</summary>
    public abstract class BaseWsdlPage : System.Web.UI.Page
    {
        /// <summary>Data bind.</summary>
        ///
        /// <param name="repeaters">A variable-length parameters list containing repeaters.</param>
        public static void DataBind(params Repeater[] repeaters)
        {
            foreach (var repeater in repeaters)
            {
                repeater.DataBind();
            }
        }
    }
}