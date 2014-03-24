using System;
using System.Web;
using System.Web.UI;

namespace NServiceKit.WebHostApp
{
	public partial class Default : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			Response.Redirect("api/metadata");
		}	
		
	}
}

