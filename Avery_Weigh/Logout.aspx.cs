using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Avery_Weigh
{
    public partial class Logout : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Session["UserName"] = null;
            Session["Password"] = null;
            Session["WBID"] = null;
            Session["PlantID"] = null;
            FormsAuthentication.SignOut();
            Response.Redirect("/Login.aspx");
        }
    }
}