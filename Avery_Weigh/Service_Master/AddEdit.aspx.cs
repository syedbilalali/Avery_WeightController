using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Avery_Weigh.Repository;

namespace Avery_Weigh.Service_Master
{
    public partial class AddEdit : System.Web.UI.Page
    {
        DataClasses1DataContext db = new DataClasses1DataContext();
        ServiceMasterRepository smrepo = new ServiceMasterRepository();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Get_ServiceMaster();
                bindData();
            }
        }

        protected void Btnsave_Click(object sender, EventArgs e)
        {
            ServiceMaster sm = db.ServiceMasters.FirstOrDefault(x => x.Id == 1);
            if (sm==null)   // (string.IsNullOrEmpty(Request.QueryString["Id"]))
            {
                Add();
            }
            else
            {
                Update();
            }
        }

        private void bindData()
        {
            IEnumerable<ServiceMaster> service = db.ServiceMasters.ToList();
            if (service.Count() >= 2)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "myalert", "toastr.error('Multiple Record found. Please contact with service engineer.');", true);
            }
            else
            {
                ServiceMaster sm = db.ServiceMasters.FirstOrDefault(x => x.Id == 1);
                if (sm != null)
                {
                    ddlamctype.SelectedValue = sm.AMCType;
                    ddlWarrantee.SelectedValue = sm.Warrantee;
                    ddlGarrantee.SelectedValue = sm.Garrantee;
                    txtamccontactnumber.Text = sm.AMCContactNo.ToString();
                    txtamcreminder.Text = sm.AMCReminder.ToString();
                    DateTime? validupto = sm.AMCValidUpto;
                    string AMCValidUpto = String.Format("{0:dd/MM/yyyy}", validupto);
                    DateTime? stampingdate = sm.StampingDate;
                    string Stampingdate = string.Format("{0:dd/MM/yyyy}", stampingdate);
                    txtamcvalidupto.Text = AMCValidUpto;
                    txtstampingdate.Text = Stampingdate;
                    txtstampingreminder.Text = sm.StampingReminder.ToString();
                }
            }
        }

        protected void Add()
        {
            try
            {
                if (!string.IsNullOrEmpty(ddlamctype.SelectedValue))
                { 
                    ServiceMaster sm = new ServiceMaster();
                    sm.AMCType = ddlamctype.SelectedValue;
                    sm.AMCContactNo = txtamccontactnumber.Text.Trim().ToString();
                    DateTime AMCValidUpto = DateTime.ParseExact(txtamcvalidupto.Text, "dd/MM/yyyy", new CultureInfo("en-GB"));
                    sm.AMCValidUpto = AMCValidUpto;
                    sm.AMCReminder = Convert.ToInt32(txtamcreminder.Text.Trim());
                    sm.StampingReminder = Convert.ToInt32(txtstampingreminder.Text.Trim());
                    DateTime Stampingdate = DateTime.ParseExact(txtstampingdate.Text, "dd/MM/yyyy", new CultureInfo("en-GB"));
                    sm.StampingDate = Stampingdate;
                    sm.IsDeleted = false;
                    sm.Warrantee=ddlWarrantee.SelectedValue;
                    sm.Garrantee=ddlGarrantee.SelectedValue;
                    if (smrepo.Add_ServiceMaster(sm))
                    {
                        ScriptManager.RegisterStartupScript(this,this.GetType(),"toastr","toastr.success('Save Successfully');",true);
                        HtmlMeta meta = new HtmlMeta();
                        meta.HttpEquiv = "Refresh";
                        meta.Content = "1.30;url=AddEdit.aspx";
                        this.Page.Controls.Add(meta);
                    }
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "toastr", "toastr.success('"+ex.Message.ToString()+"');", true);
            }
        }  

        protected void Get_ServiceMaster()
        {
            if (!string.IsNullOrEmpty(Request.QueryString["Id"]))
            {
                int id = Convert.ToInt32(Request.QueryString["Id"]);
                ServiceMaster sm = smrepo.Get_ServiceMasterById(id);
                if (sm != null)
                {
                    ddlamctype.SelectedValue = sm.AMCType;
                    txtamccontactnumber.Text = sm.AMCContactNo.ToString();
                    txtamcreminder.Text = sm.AMCReminder.ToString();
                    DateTime? validupto = sm.AMCValidUpto;
                    string AMCValidUpto = String.Format("{0:dd/MM/yyyy}", validupto);
                    DateTime? stampingdate = sm.StampingDate;
                    string Stampingdate = string.Format("{0:dd/MM/yyyy}", stampingdate);
                    txtamcvalidupto.Text = AMCValidUpto;
                    txtstampingdate.Text = Stampingdate;
                    txtstampingreminder.Text = sm.StampingReminder.ToString();
                    ddlWarrantee.SelectedValue = sm.Warrantee;
                    ddlGarrantee.SelectedValue = sm.Garrantee;
                }
            }
        }

        protected void Update()
        {
            //if (!string.IsNullOrEmpty(Request.QueryString["Id"]))
            //{
            //    RegexRepository r = new RegexRepository();
            int id = 1;  // Convert.ToInt32(Request.QueryString["Id"]);
            ServiceMaster sm = db.ServiceMasters.FirstOrDefault(x => x.Id == 1 && x.IsDeleted == false);             
                if (sm != null)
                {
                    sm.AMCType = ddlamctype.SelectedValue;
                    sm.AMCContactNo = txtamccontactnumber.Text.Trim().ToString();
                    DateTime AMCValidUpto = DateTime.ParseExact(txtamcvalidupto.Text, "dd/MM/yyyy", new CultureInfo("en-GB"));
                    sm.AMCValidUpto = AMCValidUpto;
                    sm.AMCReminder = Convert.ToInt32(txtamcreminder.Text.Trim());
                    sm.StampingReminder = Convert.ToInt32(txtstampingreminder.Text.Trim());
                    DateTime Stampingdate = DateTime.ParseExact(txtstampingdate.Text, "dd/MM/yyyy", new CultureInfo("en-GB"));
                    sm.StampingDate = Stampingdate;
                    sm.Warrantee = ddlWarrantee.SelectedValue;
                    sm.Garrantee = ddlGarrantee.SelectedValue;
                    db.SubmitChanges();
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "toastr", "toastr.success('Update Successfully');", true);
                    HtmlMeta meta = new HtmlMeta();
                    meta.HttpEquiv = "Refresh";
                    meta.Content = "1.30;url=AddEdit.aspx?id="+id;
                    this.Page.Controls.Add(meta);
                }
            //}
        }
    }
}