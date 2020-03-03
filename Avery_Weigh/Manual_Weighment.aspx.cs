using Avery_Weigh.Model;
using Avery_Weigh.Repository;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using Advantech.Adam;
//using System.Linq;
using DIOWebService;
using System.Threading;
using NetSDK;
using PlaySDK;
using System.Drawing;
using System.Linq;
using System.Globalization;
//using System.Drawing;
//using System.Windows.Forms;

namespace Avery_Weigh
{
    public partial class Manual_Weighment : System.Web.UI.Page
    {
        private static TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
        DataClasses1DataContext db = new DataClasses1DataContext();
        MaterialRepository _materialrepo = new MaterialRepository();
        SupplierRepository _supplierrepo = new SupplierRepository();
        TransporterRepository _transrepo = new TransporterRepository();
        PackingRepository _Packingrepo = new PackingRepository();
        TransactionRepository _transactionRepo = new TransactionRepository();
        UserMasterRepository umRepo = new UserMasterRepository();
        DataClasses1DataContext db1 = new DataClasses1DataContext();
        private Socket s3;

       
        bool Flag_FirstWt_Records = false;
        bool Flag_SecondWt_Records = false;

        string checkdisplayMessage = string.Empty;

        public string mySrc2;
        public string mySrc1;
        public string mySrc3;

      
        protected void Page_Load(object sender, EventArgs e)
        {

           
                if (!IsPostBack)
                {
                    bindFormLabels();
                   CheckTareWeightToleranceLimit();

                    getuserAccess();
                    timer1.Interval = 100;
                    timer1.Enabled = true;
                    lblUnit.Text = Session["WEIGHINGUNIT"].ToString();
                    lblUnit1.Text = Session["WEIGHINGUNIT"].ToString();
                    lblUnit2.Text = Session["WEIGHINGUNIT"].ToString();
                    lblUnit3.Text = Session["WEIGHINGUNIT"].ToString();
                    CheckAndBindTripId();
                    BindMaterialDropDown();
                    BindMaterialClassificationDropdown();
                    BindSupplierDropdown();
                    BindTransporterDropdown();
                    BindPackingDropDown();
                   
                    //this.txtMsgInfo.Text = "Vehicle is not proper position at Weighbridge.";
                    try
                    {
                        UserControl UC;
                        System.Web.UI.WebControls.Image imgUC;
                        UC = (UserControl)Page.FindControl("Manual_Weighment");
                        imgUC = (System.Web.UI.WebControls.Image)UC.FindControl("Image5");
                        imgUC.ImageUrl = "~/images/type1/configure_disable.png";
                    }
                    catch { }

                    //if (Session["WBFORM"].ToString() == "1")
                    //{
                    //    try
                    //    {
                    //        UserControl UC;
                    //        System.Web.UI.WebControls.Image imgUC;
                    //        UC = (UserControl)Page.FindControl("Manual_Weighment");
                    //        imgUC = (System.Web.UI.WebControls.Image)UC.FindControl("Image4");
                    //        imgUC.ImageUrl = "~/images/type1/weigh_disable.png";
                    //    }
                    //    catch { }
                    //}


                }

           
            



        }

        private void bindFormLabels()
        {
            try

            {
                IList<DynamicFieldName> model = _transrepo.GetFieldNameByMachine(Session["WBID"].ToString(), Session["PlantID"].ToString());
                if (model.Count > 0)
                {
                    foreach (DynamicFieldName names in model)
                    {
                        switch (names.FieldName)
                        {
                            case "Trip Id":
                                lblTripId.InnerText = names.FieldValue;
                                break;

                            case "Weighing Type":
                                lblWeighingType.InnerText = names.FieldValue;
                                break;
                            case "Multi Product":
                                lblMultiProduct.InnerText = names.FieldValue;
                                break;
                            case "Truck No":
                                lbltruckno.InnerText = names.FieldValue;
                                break;
                            case "Material":
                                lblMaterial.InnerText = names.FieldValue;
                                break;
                            case "Material Classification":
                                lblMC.InnerText = names.FieldValue;
                                break;
                            case "Supplier/customer":
                                lblsupplier.InnerText = names.FieldValue;
                                break;
                            case "Transporter":
                                lblTransporter.InnerText = names.FieldValue;
                                break;
                            case "Packing":
                                lblPacking.InnerText = names.FieldValue;
                                break;
                            case "Packing qty":
                                lblPackingQty.InnerText = names.FieldValue;
                                break;
                            case "Challan/Invoice no":
                                lblChallanNo.InnerText = names.FieldValue;
                                break;
                            case "Challan weight":
                                lblChallanwt.InnerText = names.FieldValue;
                                break;
                            case "PO /SO/DO no":
                                lblPOSODONo.InnerText = names.FieldValue;
                                break;
                            case "Remarks":
                                lblRemrks.InnerText = names.FieldValue;
                                break;
                            case "1st weight":
                                lblFirstWt.InnerText = names.FieldValue;
                                break;
                            case "2nd weight":
                                lbl2ndWt.InnerText = names.FieldValue;
                                break;
                            case "Net weight":
                                lblNetWt.InnerText = names.FieldValue;
                                break;
                            case "Security name":
                                //lblTripId.InnerText = names.FieldValue;
                                break;
                            case "Security Remarks":
                                //lblTripId.InnerText = names.FieldValue;
                                break;
                        }
                    }
                }
            }
            catch { }
        }

      
        private void getuserAccess()
        {
            if (!User.Identity.IsAuthenticated)
                Response.Redirect("/Login");

            int userid = Convert.ToInt32(User.Identity.Name);
            UserClassification uc = umRepo.GetUserAuthorization(userid);
            if (uc != null)
            {
                #region check Weighing page access
                if (uc.Weighment == false)
                    WeighMenu.Style.Add("display", "none");
               //else if (uc. == false)
               //     ManageMasters.Style.Add("display", "none");
                else
                    WeighMenu.Style.Add("display", "block");
                #endregion
                #region check Configuration page access
                if (uc.Weighment == false)
                    configurationMenu.Style.Add("display", "none");
                else
                    configurationMenu.Style.Add("display", "block");
                #endregion
                #region check Configuration page access
                if (uc.Weighment == false)
                    configurationMenu.Style.Add("display", "none");
                else
                    configurationMenu.Style.Add("display", "block");
                #endregion
            }

           
        }

        private void CheckAndBindTripId()
        {
            txtTripId.Text = _transactionRepo.GetTripId().ToString();
        }

        public tblMachineWorkingParameter getLoggedInUserWeigh()
        {
                string machineId = string.Empty;
                try
                {
                    machineId = Session["WBID"].ToString();
                }
                catch { }
                tblMachineWorkingParameter mwparameter = _transactionRepo.getMachineWorkingParameters(machineId);
                return mwparameter;
            
        }

        
        public string GetWeightFromTCPIP_SS()
        {
            string output = "No weight";
            // for inicator ip address and port no
            string strIndicatorIPAddress = string.Empty;
            string strIndicatorPortNo = string.Empty;
            tblMachineWorkingParameter tbl = getLoggedInUserWeigh();
            try
            {
                strIndicatorPortNo = tbl.PortNo;
                strIndicatorIPAddress = tbl.IPPort;

            }
            catch { }

            s3 = new Socket(AddressFamily.InterNetwork, SocketType.Stream,
                ProtocolType.Tcp);
           
                IPAddress hostadd = IPAddress.Parse(strIndicatorIPAddress);
           
                IPEndPoint EPhost = new IPEndPoint(hostadd, Convert.ToInt32(strIndicatorPortNo));
        
            string weight = "";
            s3.ReceiveTimeout = 500;
            try
            {

                s3.Connect(EPhost);

                if (s3.Connected)
                {
                    try
                    {
                        Byte[] sbyte1 = new Byte[] { 0x5 };

                        s3.Send(sbyte1);
                        System.Threading.Thread.Sleep(500);
                    }
                    catch { }

                    Byte[] receive = new Byte[37];


                    int ret = s3.Receive(receive, receive.Length, 0);
                    if (ret > 0)
                    {
                        string tmp = null;

                        tmp = System.Text.Encoding.ASCII.GetString(receive);
                        if (tmp.Length > 0)
                        {
                            weight = tmp.Substring(tmp.IndexOf("") + "".Length, 7).Trim();

                        }
                    }


                    s3.Disconnect(true);
                }
            }
            catch (Exception e1)
            {


                return output;

            }
           

            return weight;
          
        }

       
        protected void timer1_Tick(object sender, EventArgs e)
        {
            


            //Enable this code if weight machine connected on local client machine 
            //string Weight = GetWeightFromIP();  //comented by ss and added new code

            string Weight = GetWeightFromTCPIP_SS();
            
            //Enable this code if code is run on public server 
            //string Weight = GetIPfromServerPort();
            string TruckNo = txtTruckNo.Text.Trim().ToUpper();
            RuntimeWeight.Text = Weight.Trim();
            //check truck is already in pending record or not
            if (!_transactionRepo.checkTruckIsPendingOrNot(TruckNo))
            {
                //check Truck trip is saved under transaction file and id checked first weight and date time will bot update.
                if (!_transactionRepo.checkTruckTripClosed(TruckNo))
                {
                    FirstWeight.Text = Weight.Trim();
                    DateTime indianTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
                    FirstDate.Value = indianTime.ToString("dd/MM/yyyy");
                    FirstTime.Value = indianTime.ToString("HH:mm:ss tt");
                }
            }
            else
            {
                tblTransaction tbltrans = _transactionRepo.getPendingTransactionById(TruckNo);
                if (tbltrans != null)
                {
                    FirstWeight.Text = tbltrans.FirstWeight.Value.ToString("0");
                    FirstDate.Value = tbltrans.FirstWtDateTime.Value.ToString("dd/MM/yyyy");
                    FirstTime.Value = tbltrans.FirstWtDateTime.Value.ToString("HH:mm:ss tt");
                }

                //If truck in on pending record and need to take next weight
                SecondWeight.Value = Weight;
                DateTime indianTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
                SecondDate.Value = indianTime.ToString("dd/MM/yyyy");
                SecondTime.Value = indianTime.ToString("HH:mm:ss tt");
                if (ddlinoutna.SelectedItem.Value == "0" || ddlinoutna.SelectedItem.Value == "2")
                {
                    try
                    {
                        decimal FinalWeight =Math.Abs(Convert.ToDecimal(SecondWeight.Value) - Convert.ToDecimal(FirstWeight.Text));
                        txtFinalWeight.Value = FinalWeight.ToString("0");
                    }catch{ }
                }
                else
                {
                    try
                    {
                        decimal FinalWeight = Convert.ToDecimal(FirstWeight.Text) - Convert.ToDecimal(SecondWeight.Value);
                        txtFinalWeight.Value = FinalWeight.ToString("0");
                    }
                    catch { }
                }
            }


            

            if (Session["DIOIP"].ToString() != "0" && Session["SENSORACTIVE"].ToString() == "Y")
            this.timer2_Tick(null, null);



        }

        protected void RuntimeWeight_Click(object sender, EventArgs e)
        {
            if (timer1.Enabled == true)
            {
                FirstWeight.Text = RuntimeWeight.Text;
                FirstDate.Value = DateTime.Now.ToShortDateString();
                FirstTime.Value = DateTime.Now.TimeOfDay.ToString();
            }
        }

        private void BindMaterialDropDown()
        {
            ddlmaterial.DataTextField = "Name";
            ddlmaterial.DataValueField = "MaterialCode";
            ddlmaterial.DataSource = _materialrepo.GetModelMaterials();
            ddlmaterial.DataBind();
            ddlmaterial.Items.Insert(0, new ListItem("Select", ""));
        }

        private void BindMaterialClassificationDropdown()
        {
            MaterialClassificationRepository repository = new MaterialClassificationRepository();
            ddlmc.DataTextField = "Name";
            ddlmc.DataValueField = "Code";
            ddlmc.DataSource = repository.GetMaterialClassifications_Code();
            ddlmc.DataBind();
            ddlmc.Items.Insert(0, new ListItem("Select", "0"));
        }

        private void BindSupplierDropdown()
        {
            ddlsupplier.DataTextField = "Name";
            ddlsupplier.DataValueField = "Code";
            ddlsupplier.DataSource = _supplierrepo.Get_SupplierCode();
            ddlsupplier.DataBind();
            ddlsupplier.Items.Insert(0, new ListItem("Select", "0"));
        }
        private void BindTransporterDropdown()
        {
            ddltransporter.DataTextField = "Name";
            ddltransporter.DataValueField = "ddCode";
            ddltransporter.DataSource = _transrepo.Get_Transporters();
            ddltransporter.DataBind();
            ddltransporter.Items.Insert(0, new ListItem("Select", "0"));
        }
        private void BindPackingDropDown()
        {
            ddlpacking.DataTextField = "Name";
            ddlpacking.DataValueField = "PackingCode";
            ddlpacking.DataSource = _Packingrepo.Get_PackingCode();
            ddlpacking.DataBind();
            ddlpacking.Items.Insert(0, new ListItem("Select", "0"));
        }

        protected void save_Click(object sender, EventArgs e)
        {
            
            bool SecondWt = false;
            if (RuntimeWeight.Text == "No weight" || RuntimeWeight.Text == "0")
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "toastr", "toastr.success('Record cannot be captured. No Weight comes.');", true);
            }
            else
            {
                //if (string.IsNullOrEmpty(txtTruckNo.Text))
                //    ScriptManager.RegisterStartupScript(this, this.GetType(), "toastr", "toastr.success('Truck no cannot be leave blank.');", true);
                //else
                //{
                string res = "";
                if (!_transactionRepo.checkTruckIsPendingOrNot(txtTruckNo.Text))
                {
                    res = CheckValidation();
                }
                else
                {
                    res = CheckSecondValidation();
                }
                if (res.Split(':')[0] == "1")
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "toastr", "toastr.error('Field " + res.Split(':')[1].ToString() + " Is Mandatory.');", true);
                }
                else
                {
                    bool IsFirstWeight = false;
                    decimal LiveWeight = 0;
                    bool isInbound = false;
                    try
                    {
                        LiveWeight = Convert.ToDecimal(RuntimeWeight.Text);
                    }
                    catch { }

                    if (LiveWeight > 0)
                    {
                        decimal diff = 0;
                        //if (timer1.Enabled == true)
                        //{
                            string TruckNo = txtTruckNo.Text.Trim().ToUpper();
                            if (!_transactionRepo.checkTruckIsPendingOrNot(TruckNo))
                            {
                                IsFirstWeight = true;
                                FirstWeight.Text = RuntimeWeight.Text;
                                FirstDate.Value = DateTime.Now.ToShortDateString();
                                FirstTime.Value = DateTime.Now.TimeOfDay.ToString();
                                this.Flag_FirstWt_Records = true;
                                this.Flag_SecondWt_Records = false;
                                Session["FIRSTWT_RCD"] = "1";
                                Session["SECONDWT_RCD"] = "0";
                               
                            }
                            else
                            {
                                SecondWeight.Value = RuntimeWeight.Text;
                                SecondDate.Value = DateTime.Now.ToShortDateString();
                                SecondTime.Value = DateTime.Now.TimeOfDay.ToString();
                                this.Flag_FirstWt_Records = false;
                                this.Flag_SecondWt_Records = true;
                                Session["FIRSTWT_RCD"] = "0";
                                Session["SECONDWT_RCD"] = "1";

                                

                            }
                        //}
                        if (ddlinoutna.SelectedItem.Value == "1")
                        {
                            try
                            {
                                diff = Convert.ToDecimal(FirstWeight.Text) - Convert.ToDecimal(SecondWeight.Value);
                            }
                            catch { }
                        }
                        else if (ddlinoutna.SelectedItem.Value == "2")
                        {
                            try
                            {
                                diff = Convert.ToDecimal(SecondWeight.Value) - Convert.ToDecimal(FirstWeight.Text);
                            }
                            catch { }
                        }
                        else
                        {
                            if (checkmultiproduct.Checked)
                            {
                                int transid = 0;
                                try
                                {
                                    transid = Convert.ToInt32(txtTripId.Text);
                                }
                                catch { }
                                IList<tblTransactionMaterial> tbltransWei = _transactionRepo.getmaterialsByTransactionId(transid);
                                try
                                {
                                    diff = Convert.ToDecimal(FirstWeight.Text) - Convert.ToDecimal(SecondWeight.Value);
                                }
                                catch { }
                                if (tbltransWei.Count == 1)
                                {
                                    if (diff > 0)
                                        isInbound = true;

                                    else
                                    {
                                        diff = diff * -1;
                                        isInbound = false;
                                    }
                                }
                            }
                            else
                            {
                                try
                                {
                                    diff = Convert.ToDecimal(FirstWeight.Text) - Convert.ToDecimal(SecondWeight.Value);
                                }
                                catch { }
                                if (diff > 0)
                                    isInbound = true;
                                else
                                {
                                    diff = diff * -1;
                                    isInbound = false;
                                }
                            }
                        }
                        if (diff > 0 || IsFirstWeight)
                        {
                            Model_ManualWeight model = new Model_ManualWeight();
                            model.trans = new tblTransaction();
                            try
                            {
                                //DateTime vchallandate = DateTime.ParseExact(txtInvoiceDate.Text, "dd/MM/yyyy", new CultureInfo("en-GB"));
                                model.trans.ChallanDate = Convert.ToDateTime(txtInvoiceDate.Text);
                            }
                            catch { }
                            model.trans.ChallanNo = txtInvoiceNo.Text;
                            model.trans.ChallanWeight = txtChallanWeight.Text;
                            model.trans.CreateDate = DateTime.Now;
                            model.trans.GateEntryNo = txtgateentryno.Text;
                            model.trans.IsMultiProduct = checkmultiproduct.Checked;
                            if (ddlmc.SelectedItem.Value == "0")
                            {
                                model.trans.MaterialCalssificationCode = ddlmc.SelectedItem.Value;
                                model.trans.MaterialClassificationName = "";
                            }
                            else
                            {
                                model.trans.MaterialCalssificationCode = ddlmc.SelectedItem.Value;
                                model.trans.MaterialClassificationName = ddlmc.SelectedItem.Text.Split('(')[0].Replace("(", "");
                            }

                            try
                            {
                                if (ddlmaterial.SelectedItem.Value == "0")
                                {
                                    model.trans.MaterialCode = ddlmaterial.SelectedItem.Value;
                                    model.trans.MaterialName = ""; ;
                                }
                                else
                                {
                                    model.trans.MaterialCode = ddlmaterial.SelectedItem.Value;
                                    model.trans.MaterialName = ddlmaterial.SelectedItem.Text.Split('(')[0].Replace("(", "");
                                }
                            }
                            catch { }
                            if (ddlpacking.SelectedItem.Value == "0")
                            {
                                model.trans.PackingCode = ddlpacking.SelectedItem.Value;
                                model.trans.PackingName = "";
                            }
                            else
                            {
                                model.trans.PackingCode = ddlpacking.SelectedItem.Value;
                                model.trans.PackingName = ddlpacking.SelectedItem.Text.Split('(')[0].Replace("(", "");
                            }
                            try
                            {
                                model.trans.PackingQty = Convert.ToInt32(txtpackingqty.Text);
                            }
                            catch { }
                            try
                            {
                                //DateTime vpodatedate = DateTime.ParseExact(txtPODate.Text, "dd/MM/yyyy", new CultureInfo("en-GB"));
                                model.trans.PODate =  Convert.ToDateTime(txtPODate.Text);
                            }
                            catch { }
                            model.trans.PONo = txtPONo.Text;
                            model.trans.Remarks = txtremarks.Text;
                            if (ddlsupplier.SelectedItem.Value == "0")
                            {
                                model.trans.SupplierCode = ddlsupplier.SelectedItem.Value;
                                model.trans.SupplierName = "";
                            }
                            else
                            {
                               
                                model.trans.SupplierCode = ddlsupplier.SelectedItem.Value;
                                model.trans.SupplierName = ddlsupplier.SelectedItem.Text.Split('(')[0].Replace("(", "");
                            }

                            model.trans.TransactionStatus = 1;
                            if (ddltransporter.SelectedItem.Value == "0")
                            {
                                model.trans.TransporterCode = ddltransporter.SelectedItem.Value;
                                model.trans.TransporterName = "";
                            }
                            else
                            {
                                model.trans.TransporterCode = ddltransporter.SelectedItem.Value;
                                model.trans.TransporterName = ddltransporter.SelectedItem.Text.Split('(')[0].Replace("(", "");
                            }

                            model.trans.TripId = Convert.ToInt32(txtTripId.Text);
                            model.trans.TripType = ddlinoutna.SelectedItem.Value != "0" || IsFirstWeight ? Convert.ToInt32(ddlinoutna.SelectedItem.Value) : isInbound == true ? 1 : 2;
                            ddlinoutna.SelectedValue = ddlinoutna.SelectedItem.Value != "0" || IsFirstWeight ? ddlinoutna.SelectedItem.Value : isInbound == true ? "1" : "2";
                            model.trans.TruckNo = txtTruckNo.Text.ToUpper();

                            
                            if (!_transactionRepo.checkTruckIsPendingOrNot(txtTruckNo.Text.ToUpper()))
                            {
                                model.trans.FirstWeight = Convert.ToDecimal(FirstWeight.Text);
                                model.trans.FirstWtDateTime = Convert.ToDateTime(FirstTime.Value);
                                SecondWt = false;
                                this.Flag_FirstWt_Records = true;
                                this.Flag_SecondWt_Records = false;
                                Session["FIRSTWT_RCD"] = "1";
                                Session["SECONDWT_RCD"] = "0";
                                
                            }
                            else
                            {
                                model.trans.SecondWeight = Convert.ToDecimal(SecondWeight.Value);
                                model.trans.SecondWtDateTime = Convert.ToDateTime(SecondTime.Value);
                                string FinalWeightCaptured = txtFinalWeight.Value;
                                model.trans.NetWeight = Convert.ToDecimal(FinalWeightCaptured);
                                model.trans.Shift= Shift(DateTime.Now).Substring(0, 1);
                                model.trans.SHIFTDATE= Convert.ToDateTime(Shift(DateTime.Now).Substring(2));
                                SecondWt = true;
                                this.Flag_FirstWt_Records = false;
                                this.Flag_SecondWt_Records = true;
                                Session["FIRSTWT_RCD"] = "0";
                                Session["SECONDWT_RCD"] = "1";
                                
                               
                            }
                            if (checkmultiproduct.Checked)
                            {
                                tblTransaction trans = _transactionRepo.getPendingTransactionById(txtTruckNo.Text);
                                if (trans.FirstWeight != null)
                                //if (!string.IsNullOrEmpty(trans.FirstWeight.ToString()))
                                {
                                    tblTransactionMaterial mat = new tblTransactionMaterial();
                                    mat.CreteDate = DateTime.Now;
                                    try
                                    {
                                        mat.MaterialCode = ddlmaterial.SelectedItem.Value;
                                    }
                                    catch { }
                                    try
                                    {
                                        mat.MaterialName = ddlmaterial.SelectedItem.Text.Split('(')[0].Replace("(", "");
                                    }
                                    catch { }
                                    mat.TransactionId = Convert.ToInt32(txtTripId.Text);
                                    if (trans.TripType == 1)
                                        mat.Weight = (Convert.ToDecimal(SecondWeight.Value) - Convert.ToDecimal(trans.SecondWeight)).ToString();
                                    else
                                        mat.Weight = (Convert.ToDecimal(trans.SecondWeight) - Convert.ToDecimal(SecondWeight.Value)).ToString();

                                    model.material = mat;
                                }
                                
                            }
                            model.UserName = Session["UserName"].ToString();
                            model.WeibridgeId = Session["WBID"].ToString();
                            model.trans.PlantCode = Session["PlantID"].ToString();
                            model.trans.WeighingUnit= Session["WEIGHINGUNIT"].ToString();
                            model.trans.CompanyCode = Session["CompanyCode"].ToString();  // "com1";
                            model.trans.WeighbridgeId  = Session["WBID"].ToString();
                            model.plantCode = Session["PlantID"].ToString();
                            
                            model.companyCode = Session["CompanyCode"].ToString();  // "com1";

                            // Save Transactio record in database
                            _transactionRepo.saveTransactionRecord(model);
                           
                            
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "toastr", "toastr.success('Record added successfully.');", true);
                            if (SecondWt)
                            {
                                ScriptManager.RegisterStartupScript(this, this.GetType(), "toastr", "COnfirmClick();", true);
                            }
                        }
                        else
                        {
                            if (ddlinoutna.SelectedItem.Value == "1")
                            {
                                ScriptManager.RegisterStartupScript(this, this.GetType(), "toastr", "toastr.error('Weight should me less than First weight.');", true);
                            }
                            else
                            {
                                ScriptManager.RegisterStartupScript(this, this.GetType(), "toastr", "toastr.error('Weight should be more than First Weight.');", true);
                            }
                        }
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "toastr", "toastr.error('Weight comes 0. Weight Cannot be captured');", true);
                    }
                }
            }
        }

        public void StoredTareWeight()
        {
            //TruckTareWeight _trucktarewt = db.TruckTareWeights.FirstOrDefault();
            //_trucktarewt = new TruckTareWeight();
            //if (ddlinoutna.SelectedItem.Value == "1")
            //{
            //    _trucktarewt.TruckNo = txtTruckNo.Text.Trim().ToUpper();
            //    _trucktarewt.TareWeight = Convert.ToDecimal(SecondWeight.Value);
            //    _trucktarewt.TareWtDateTime = Convert.ToDateTime(SecondTime.Value);
            //}
            //else
            //{
            //    _trucktarewt.TruckNo = txtTruckNo.Text.Trim().ToUpper();
            //    _trucktarewt.TareWeight = Convert.ToDecimal(SecondWeight.Value);
            //    _trucktarewt.TareWtDateTime = Convert.ToDateTime(SecondTime.Value);
            //}
            //db.TruckTareWeights.InsertOnSubmit(_trucktarewt);
            //db.SubmitChanges();
        }

        public void OnConfirm(object sender, EventArgs e)
        {
            string confirmValue = Request.Form["confirm_value"];
            int transId = Convert.ToInt32(txtTripId.Text);
            if (confirmValue == "Yes")
            {
                _transactionRepo.CloseTicket(transId);
            }
            else
            {
                
            }
        }

        public void CheckTareWeightToleranceLimit()
        {
            //tblMachineWorkingParameter _tmachine = db.tblMachineWorkingParameters.Where(x => x.PlantCode == Session["PlantID"].ToString() && x.MachineId == Session["WBID"].ToString()).FirstOrDefault();

            //if (_tmachine.TareCheck == 1 && _tmachine.TareScheme != "")
            //{
            //    var query=string.Empty;
            //    var query1 = "No";
            //    double dbl_uppertolerance = 0;
            //    double dbl_lowertolerance = 0;

            //    if (_tmachine.TareScheme == "Average(All)")
            //    {
            //        query = (from q in db.TruckTareWeights where q.TruckNo == txtTruckNo.Text.Trim() select q.TareWeight).Average().ToString();
            //        dbl_uppertolerance = Convert.ToDouble(query) + Convert.ToDouble(_tmachine.TareWeightValue);
            //        dbl_lowertolerance = Convert.ToDouble(query) - Convert.ToDouble(_tmachine.TareWeightValue);

            //        if (Convert.ToDouble(RuntimeWeight.Text) > dbl_uppertolerance || Convert.ToDouble(RuntimeWeight.Text) < dbl_lowertolerance)
            //        {
            //            query1 = "Yes";
            //            //if (Convert.ToDecimal(query)<_tmachine.TareWeightValue  && Convert.ToDecimal(query) < _tmachine.TareWeightValue)
            //            //query1=
            //        }

            //    if (_tmachine.TareScheme == "Tolerance(%)")
            //    {
            //        query = (from q in db.TruckTareWeights where q.TruckNo == txtTruckNo.Text.Trim() orderby q.TareWtDateTime descending select q.TareWeight).First().ToString();
            //    }

            //    if (_tmachine.TareScheme == "Tolerance(kg)")
            //    {
            //        query = (from q in db.TruckTareWeights where q.TruckNo == txtTruckNo.Text.Trim() orderby q.TareWtDateTime descending select q.TareWeight).First().ToString();
            //    }



            //    //TruckTareWeight rept_max = (from c in db.TruckTareWeights.Where(x=> x.TruckNo == txtTruckNo.Text.Trim())
            //    //                    select c).Average(c => c.TareWeight.Value));
            //}
        }

        protected void lnkPrint_Click(object sender, EventArgs e)
        {
            string strTripId = string.Empty;
            if (!string.IsNullOrEmpty(this.txtTruckNo.Text.Trim()))
            {
                //strTripId = _transactionRepo.GetTripId_new(Session["WBID"].ToString(),this.txtTruckNo.Text.Trim()).ToString();
                strTripId = _transactionRepo.GetTripId_new(Session["WBID"].ToString(),this.txtTripId.Text.Trim(), this.txtTruckNo.Text.Trim()).ToString();
                strTripId = this.txtTripId.Text.Trim();
                if (strTripId == "0")
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "toastr", "toastr.error('No Records');", true);
                else
                {
                    if (File.Exists(Server.MapPath("~/pdfs/" + strTripId + ".pdf")))
                    {
                        File.Delete(Server.MapPath("~/pdfs/" + strTripId + ".pdf"));
                    }

                    Ticket ts = new Ticket();
                    ts.GetTicket(Convert.ToInt32(strTripId));
                    //lnkPrint.OnClientClick = "target='_blank'";
                    //Response.Redirect("~/pdfs/" + strTripId + ".pdf");
                    //lnkPrint.Attributes.Add("href",String.Format("/pdfs/" + strTripId + ".pdf"));
                    //lnkPrint.Attributes.Add("target","_blank");
                    var varurl = "/pdfs/" + strTripId + ".pdf";
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "OpenWindow", "window.open('" + varurl + "','_newtab');", true);

                    //if (File.Exists(Server.MapPath("~/pdfs/" + strTripId + ".pdf")))
                    //{
                    //    File.Delete(Server.MapPath("~/pdfs/" + strTripId + ".pdf"));
                    //}

                }
            }
            
        }

        protected void timer2_Tick(object sender, EventArgs e)
        {

            
            
        }

        protected void timer3_Tick(object sender, EventArgs e)
        {

            


        }

        protected  string Shift(DateTime dtCurrent)
        {
            string _strShift = string.Empty;
           
            SHIFTTIME _shift = db.SHIFTTIMEs.FirstOrDefault(x => x.Id == 1);
            //DataTable _dtblShift = ModuleClasses.ClsDatabase.GetFieldValues("SHIFTTIME", "*");
            //OleDbDataAdapter oleAdpt = new OleDbDataAdapter("Select * from SHIFTTIME", GlobalConnection.oleGlobalConnection); //GlobalConnection.GlobalConnectionString); //objConnection.oleCon);
            //oleAdpt.Fill(_dtblShift);
            if (_shift != null)
            {
                string _STA = _shift.STA.ToString();
                string _EDA = _shift.EDA.ToString();
                string _STB = _shift.STB.ToString();
                string _EDB = _shift.EDB.ToString();
                string _STC = _shift.STC.ToString();
                string _EDC = _shift.EDC.ToString();
                switch (_shift.NOOFSHIFTS.ToString())
                {
                    case "ONE":
                        if (DateTime.Parse(_EDA).TimeOfDay < DateTime.Parse(_STA).TimeOfDay)
                        {
                            if (dtCurrent.TimeOfDay >= DateTime.Parse("00:00:00").TimeOfDay && dtCurrent.TimeOfDay < DateTime.Parse(_EDA).TimeOfDay)
                                _strShift = "A," + ShiftDate().ToShortDateString();
                            else if (dtCurrent.TimeOfDay >= DateTime.Parse(_STA).TimeOfDay && dtCurrent.TimeOfDay < DateTime.Parse("23:59:59").TimeOfDay)
                                _strShift = "A," + DateTime.Now.ToShortDateString();
                            else if (dtCurrent.TimeOfDay >= DateTime.Parse("00:00:00").TimeOfDay && dtCurrent.TimeOfDay > DateTime.Parse(_EDA).TimeOfDay)
                                _strShift = "G," + ShiftDate().ToShortDateString();
                        }
                        else if (DateTime.Parse(_EDA).TimeOfDay > DateTime.Parse(_STA).TimeOfDay)
                        {
                            if (dtCurrent.TimeOfDay >= DateTime.Parse(_STA).TimeOfDay && dtCurrent.TimeOfDay < DateTime.Parse(_EDA).TimeOfDay)
                                _strShift = "A," + DateTime.Now.ToShortDateString();
                            else if (dtCurrent.TimeOfDay >= DateTime.Parse(_EDA).TimeOfDay)
                                _strShift = "G," + DateTime.Now.ToShortDateString();
                            else if (dtCurrent.TimeOfDay >= DateTime.Parse("00:00:00").TimeOfDay && dtCurrent.TimeOfDay < DateTime.Parse(_STA).TimeOfDay)
                                _strShift = "G," + ShiftDate().ToShortDateString();
                        }
                        else if (DateTime.Parse(_EDA).TimeOfDay == DateTime.Parse(_STA).TimeOfDay)
                        {
                            if (dtCurrent.TimeOfDay >= DateTime.Parse("00:00:00").TimeOfDay && dtCurrent.TimeOfDay < DateTime.Parse(_EDA).TimeOfDay)
                                _strShift = "A," + ShiftDate().ToShortDateString();
                            else if (dtCurrent.TimeOfDay >= DateTime.Parse(_STA).TimeOfDay && dtCurrent.TimeOfDay <= DateTime.Parse("23:59:59").TimeOfDay)
                                _strShift = "A," + DateTime.Now.ToShortDateString();
                        }

                        break;
                    case "TWO":
                        if (DateTime.Parse(_EDA).TimeOfDay < DateTime.Parse(_STA).TimeOfDay)
                        {
                            if (dtCurrent.TimeOfDay >= DateTime.Parse("00:00:00").TimeOfDay && dtCurrent.TimeOfDay < DateTime.Parse(_EDA).TimeOfDay)
                                _strShift = "A," + ShiftDate().ToShortDateString();
                            else if (dtCurrent.TimeOfDay >= DateTime.Parse(_STA).TimeOfDay && dtCurrent.TimeOfDay <= DateTime.Parse("23:59:59").TimeOfDay)
                                _strShift = "A," + DateTime.Now.ToShortDateString();
                            else if (dtCurrent.TimeOfDay >= DateTime.Parse(_STB).TimeOfDay && dtCurrent.TimeOfDay < DateTime.Parse(_EDB).TimeOfDay)
                                _strShift = "B," + ShiftDate().ToShortDateString();
                            else if (dtCurrent.TimeOfDay >= DateTime.Parse("00:00:00").TimeOfDay && dtCurrent.TimeOfDay >= DateTime.Parse(_EDA).TimeOfDay && dtCurrent.TimeOfDay < DateTime.Parse(_STB).TimeOfDay)
                                _strShift = "G," + ShiftDate().ToShortDateString();
                            else if (dtCurrent.TimeOfDay >= DateTime.Parse("00:00:00").TimeOfDay && dtCurrent.TimeOfDay >= DateTime.Parse(_EDB).TimeOfDay && dtCurrent.TimeOfDay < DateTime.Parse(_STA).TimeOfDay)
                                _strShift = "G," + ShiftDate().ToShortDateString();
                        }
                        else if (DateTime.Parse(_EDB).TimeOfDay < DateTime.Parse(_STB).TimeOfDay)
                        {
                            if (dtCurrent.TimeOfDay >= DateTime.Parse(_STA).TimeOfDay && dtCurrent.TimeOfDay < DateTime.Parse(_EDA).TimeOfDay)
                                _strShift = "A," + DateTime.Now.ToShortDateString();
                            else if (dtCurrent.TimeOfDay >= DateTime.Parse(_STB).TimeOfDay && dtCurrent.TimeOfDay <= DateTime.Parse("23:59:59").TimeOfDay)
                                _strShift = "B," + DateTime.Now.ToShortDateString();
                            else if (dtCurrent.TimeOfDay >= DateTime.Parse("00:00:00").TimeOfDay && dtCurrent.TimeOfDay < DateTime.Parse(_EDB).TimeOfDay)
                                _strShift = "B," + ShiftDate().ToShortDateString();
                            else if (dtCurrent.TimeOfDay >= DateTime.Parse("00:00:00").TimeOfDay && dtCurrent.TimeOfDay >= DateTime.Parse(_EDB).TimeOfDay && dtCurrent.TimeOfDay < DateTime.Parse(_STA).TimeOfDay)
                                _strShift = "G," + ShiftDate().ToShortDateString();
                            else if (dtCurrent.TimeOfDay >= DateTime.Parse(_EDA).TimeOfDay && dtCurrent.TimeOfDay < DateTime.Parse(_STB).TimeOfDay)
                                _strShift = "G," + DateTime.Now.ToShortDateString();
                        }
                        else if (DateTime.Parse(_EDB).TimeOfDay > DateTime.Parse(_STB).TimeOfDay)
                        {
                            if (dtCurrent.TimeOfDay >= DateTime.Parse(_STA).TimeOfDay && dtCurrent.TimeOfDay < DateTime.Parse(_EDA).TimeOfDay)
                                _strShift = "A," + DateTime.Now.ToShortDateString();
                            else if (dtCurrent.TimeOfDay >= DateTime.Parse(_STB).TimeOfDay && dtCurrent.TimeOfDay < DateTime.Parse(_EDB).TimeOfDay)
                                _strShift = "B," + DateTime.Now.ToShortDateString();
                            else if (dtCurrent.TimeOfDay >= DateTime.Parse("00:00:00").TimeOfDay && dtCurrent.TimeOfDay < DateTime.Parse(_STA).TimeOfDay)
                                _strShift = "G," + ShiftDate().ToShortDateString();
                            else if (dtCurrent.TimeOfDay >= DateTime.Parse(_EDA).TimeOfDay && dtCurrent.TimeOfDay < DateTime.Parse(_STB).TimeOfDay)
                                _strShift = "G," + DateTime.Now.ToShortDateString();
                            else if (dtCurrent.TimeOfDay >= DateTime.Parse(_EDB).TimeOfDay && dtCurrent.TimeOfDay <= DateTime.Parse("23:59:59").TimeOfDay)
                                _strShift = "G," + DateTime.Now.ToShortDateString();
                        }
                        break;
                    case "THREE":
                        if (DateTime.Parse(_EDA).TimeOfDay < DateTime.Parse(_STA).TimeOfDay)
                        {
                            if (dtCurrent.TimeOfDay >= DateTime.Parse("00:00:00").TimeOfDay && dtCurrent.TimeOfDay < DateTime.Parse(_EDA).TimeOfDay)
                                _strShift = "A," + ShiftDate().ToShortDateString();
                            else if (dtCurrent.TimeOfDay >= DateTime.Parse(_STA).TimeOfDay && dtCurrent.TimeOfDay <= DateTime.Parse("23:59:59").TimeOfDay)
                                _strShift = "A," + DateTime.Now.ToShortDateString();
                            else if (dtCurrent.TimeOfDay >= DateTime.Parse(_STB).TimeOfDay && dtCurrent.TimeOfDay < DateTime.Parse(_EDB).TimeOfDay)
                                _strShift = "B," + ShiftDate().ToShortDateString();
                            else if (dtCurrent.TimeOfDay >= DateTime.Parse(_STC).TimeOfDay && dtCurrent.TimeOfDay < DateTime.Parse(_EDC).TimeOfDay)
                                _strShift = "C," + ShiftDate().ToShortDateString();
                            else if (dtCurrent.TimeOfDay >= DateTime.Parse("00:00:00").TimeOfDay && dtCurrent.TimeOfDay >= DateTime.Parse(_EDA).TimeOfDay && dtCurrent.TimeOfDay < DateTime.Parse(_STB).TimeOfDay)
                                _strShift = "G," + ShiftDate().ToShortDateString();
                            else if (dtCurrent.TimeOfDay >= DateTime.Parse("00:00:00").TimeOfDay && dtCurrent.TimeOfDay >= DateTime.Parse(_EDB).TimeOfDay && dtCurrent.TimeOfDay < DateTime.Parse(_STC).TimeOfDay)
                                _strShift = "G," + ShiftDate().ToShortDateString();
                            else if (dtCurrent.TimeOfDay >= DateTime.Parse("00:00:00").TimeOfDay && dtCurrent.TimeOfDay >= DateTime.Parse(_EDC).TimeOfDay && dtCurrent.TimeOfDay < DateTime.Parse(_STA).TimeOfDay)
                                _strShift = "G," + ShiftDate().ToShortDateString();
                        }
                        else if (DateTime.Parse(_EDB).TimeOfDay < DateTime.Parse(_STB).TimeOfDay)
                        {
                            if (dtCurrent.TimeOfDay >= DateTime.Parse(_STA).TimeOfDay && dtCurrent.TimeOfDay < DateTime.Parse(_EDA).TimeOfDay)
                                _strShift = "A," + DateTime.Now.ToShortDateString();
                            else if (dtCurrent.TimeOfDay >= DateTime.Parse(_STB).TimeOfDay && dtCurrent.TimeOfDay <= DateTime.Parse("23:59:59").TimeOfDay)
                                _strShift = "B," + DateTime.Now.ToShortDateString();
                            else if (dtCurrent.TimeOfDay >= DateTime.Parse("00:00:00").TimeOfDay && dtCurrent.TimeOfDay < DateTime.Parse(_EDB).TimeOfDay)
                                _strShift = "B," + ShiftDate().ToShortDateString();
                            else if (dtCurrent.TimeOfDay >= DateTime.Parse("00:00:00").TimeOfDay && dtCurrent.TimeOfDay >= DateTime.Parse(_STC).TimeOfDay && dtCurrent.TimeOfDay < DateTime.Parse(_EDC).TimeOfDay)
                                _strShift = "C," + ShiftDate().ToShortDateString();
                            else if (dtCurrent.TimeOfDay >= DateTime.Parse("00:00:00").TimeOfDay && dtCurrent.TimeOfDay >= DateTime.Parse(_EDA).TimeOfDay && dtCurrent.TimeOfDay < DateTime.Parse(_STB).TimeOfDay)
                                _strShift = "G," + DateTime.Now.ToShortDateString();
                            else if (dtCurrent.TimeOfDay >= DateTime.Parse("00:00:00").TimeOfDay && dtCurrent.TimeOfDay >= DateTime.Parse(_EDB).TimeOfDay && dtCurrent.TimeOfDay < DateTime.Parse(_STC).TimeOfDay)
                                _strShift = "G," + ShiftDate().ToShortDateString();
                            else if (dtCurrent.TimeOfDay >= DateTime.Parse("00:00:00").TimeOfDay && dtCurrent.TimeOfDay >= DateTime.Parse(_EDC).TimeOfDay && dtCurrent.TimeOfDay < DateTime.Parse(_STA).TimeOfDay)
                                _strShift = "G," + ShiftDate().ToShortDateString();
                        }
                        else if (DateTime.Parse(_EDC).TimeOfDay < DateTime.Parse(_STC).TimeOfDay)
                        {
                            if (dtCurrent.TimeOfDay >= DateTime.Parse(_STA).TimeOfDay && dtCurrent.TimeOfDay < DateTime.Parse(_EDA).TimeOfDay)
                                _strShift = "A," + DateTime.Now.ToShortDateString();
                            else if (dtCurrent.TimeOfDay >= DateTime.Parse(_STB).TimeOfDay && dtCurrent.TimeOfDay < DateTime.Parse(_EDB).TimeOfDay)
                                _strShift = "B," + DateTime.Now.ToShortDateString();
                            else if (dtCurrent.TimeOfDay >= DateTime.Parse(_STC).TimeOfDay && dtCurrent.TimeOfDay <= DateTime.Parse("23:59:59").TimeOfDay)
                                _strShift = "C," + DateTime.Now.ToShortDateString();
                            else if (dtCurrent.TimeOfDay >= DateTime.Parse("00:00:00").TimeOfDay && dtCurrent.TimeOfDay < DateTime.Parse(_EDC).TimeOfDay)
                                _strShift = "C," + ShiftDate().ToShortDateString();
                            else if (dtCurrent.TimeOfDay >= DateTime.Parse("00:00:00").TimeOfDay && dtCurrent.TimeOfDay >= DateTime.Parse(_EDA).TimeOfDay && dtCurrent.TimeOfDay < DateTime.Parse(_STB).TimeOfDay)
                                _strShift = "G," + DateTime.Now.ToShortDateString();
                            else if (dtCurrent.TimeOfDay >= DateTime.Parse("00:00:00").TimeOfDay && dtCurrent.TimeOfDay >= DateTime.Parse(_EDB).TimeOfDay && dtCurrent.TimeOfDay < DateTime.Parse(_STC).TimeOfDay)
                                _strShift = "G," + DateTime.Now.ToShortDateString();
                            else if (dtCurrent.TimeOfDay >= DateTime.Parse("00:00:00").TimeOfDay && dtCurrent.TimeOfDay >= DateTime.Parse(_EDC).TimeOfDay && dtCurrent.TimeOfDay < DateTime.Parse(_STA).TimeOfDay)
                                _strShift = "G," + ShiftDate().ToShortDateString();
                        }
                        else if (DateTime.Parse(_EDC).TimeOfDay > DateTime.Parse(_STC).TimeOfDay)
                        {
                            if (dtCurrent.TimeOfDay >= DateTime.Parse(_STA).TimeOfDay && dtCurrent.TimeOfDay < DateTime.Parse(_EDA).TimeOfDay)
                                _strShift = "A," + DateTime.Now.ToShortDateString();
                            else if (dtCurrent.TimeOfDay >= DateTime.Parse(_STB).TimeOfDay && dtCurrent.TimeOfDay < DateTime.Parse(_EDB).TimeOfDay)
                                _strShift = "B," + DateTime.Now.ToShortDateString();
                            else if (dtCurrent.TimeOfDay >= DateTime.Parse(_STC).TimeOfDay && dtCurrent.TimeOfDay < DateTime.Parse(_EDC).TimeOfDay)
                                _strShift = "C," + DateTime.Now.ToShortDateString();
                            else if (dtCurrent.TimeOfDay >= DateTime.Parse(_EDA).TimeOfDay && dtCurrent.TimeOfDay < DateTime.Parse(_STB).TimeOfDay)
                                _strShift = "G," + DateTime.Now.ToShortDateString();
                            else if (dtCurrent.TimeOfDay >= DateTime.Parse(_EDB).TimeOfDay && dtCurrent.TimeOfDay < DateTime.Parse(_STC).TimeOfDay)
                                _strShift = "G," + DateTime.Now.ToShortDateString();
                            else if (dtCurrent.TimeOfDay >= DateTime.Parse(_EDC).TimeOfDay && dtCurrent.TimeOfDay <= DateTime.Parse("23:59:59").TimeOfDay)
                                _strShift = "G," + DateTime.Now.ToShortDateString();
                            else if (dtCurrent.TimeOfDay >= DateTime.Parse("00:00:00").TimeOfDay && dtCurrent.TimeOfDay <= DateTime.Parse(_STA).TimeOfDay)
                                _strShift = "G," + ShiftDate().ToShortDateString();
                        }
                        break;
                }
            }
            return _strShift;
        }

        private static DateTime ShiftDate()
        {
           

            System.DateTime dtPreDate = DateTime.Today.AddDays(-1);

            return dtPreDate;
        }

      

        private string CheckValidation()
        {
            bool check = false;
            string _FieldName = "";
            IList<DynamicFieldName> model = _transrepo.GetFieldNameByMachine(Session["WBID"].ToString(), Session["PlantID"].ToString());
            if (model.Count > 0)
            {
                foreach (DynamicFieldName names in model)
                {

                    if (names.FieldName == "Trip Id")
                    {
                        if (names.IsMandatory1 == true)
                        {
                            if (string.IsNullOrEmpty(txtTripId.Text))
                            {
                                check = true;
                                _FieldName = _FieldName + names.FieldValue + ",";
                                break;
                            }
                        }

                    }

                    //if (names.FieldName == "Weighing Type")
                    //{
                    //    if (names.IsMandatory1 == true)
                    //    {
                    //        if (ddlinoutna.SelectedItem.Value == "0")
                    //        {
                    //            check = true;
                    //            _FieldName = _FieldName + names.FieldValue + ",";
                    //            break;
                    //        }
                    //    }

                    //}
                    //if (names.FieldName == "Multi Product")
                    //{
                    //    if (names.IsMandatory1 == true)
                    //    {
                    //        if (!checkmultiproduct.Checked)
                    //        {
                    //            check = true;
                    //            _FieldName = _FieldName + names.FieldValue + ",";
                    //            break;
                    //        }
                    //    }

                    //}
                    if (names.FieldName == "Truck No")
                    {
                        if (names.IsMandatory1 == true)
                        {
                            if (string.IsNullOrEmpty(txtTruckNo.Text))
                            {
                                check = true;
                                _FieldName = _FieldName + names.FieldValue + ",";
                                break;
                            }
                        }

                    }
                    if (names.FieldName == "Material")
                    {
                        if (names.IsMandatory1 == true)
                        {
                            if (ddlmaterial.SelectedItem.Value == "")
                            {
                                check = true;
                                _FieldName = _FieldName + names.FieldValue + ",";
                                break;
                            }
                        }

                    }
                    if (names.FieldName == "Material Classification")
                    {
                        if (names.IsMandatory1 == true)
                        {
                            if (ddlmc.SelectedItem.Value == "0")
                            {
                                check = true;
                                _FieldName = _FieldName + names.FieldValue + ",";
                                break;
                            }
                        }

                    }
                    if (names.FieldName == "Supplier/customer")
                    {
                        if (names.IsMandatory1 == true)
                        {
                            if (ddlsupplier.SelectedItem.Value == "0")
                            {
                                check = true;
                                _FieldName = _FieldName + names.FieldValue + ",";
                                break;
                            }
                        }

                    }
                    if (names.FieldName == "Transporter")
                    {
                        if (names.IsMandatory1 == true)
                        {
                            if (ddltransporter.SelectedItem.Value == "0")
                            {
                                check = true;
                                _FieldName = _FieldName + names.FieldValue + ",";
                                break;
                            }
                        }

                    }
                    if (names.FieldName == "Packing")
                    {
                        if (names.IsMandatory1 == true)
                        {
                            if (ddlpacking.SelectedItem.Value == "0")
                            {
                                check = true;
                                _FieldName = _FieldName + names.FieldValue + ",";
                                break;
                            }
                        }

                    }
                    if (names.FieldName == "Packing qty")
                    {
                        if (names.IsMandatory1 == true)
                        {
                            if (txtpackingqty.Text.Trim() == "")
                            {
                                check = true;
                                _FieldName = _FieldName + names.FieldValue + ",";
                                break;
                            }
                        }

                    }
                    if (names.FieldName == "Challan/Invoice no")
                    {
                        if (names.IsMandatory1 == true)
                        {
                            if (txtInvoiceNo.Text.Trim() == "")
                            {
                                check = true;
                                _FieldName = _FieldName + names.FieldValue + ",";
                                break;
                            }
                        }

                    }
                    if (names.FieldName == "Challan weight")
                    {
                        if (names.IsMandatory1 == true)
                        {
                            if (txtChallanWeight.Text == "")
                            {
                                check = true;
                                _FieldName = _FieldName + names.FieldValue + ",";
                                break;
                            }
                        }

                    }
                    if (names.FieldName == "PO /SO/DO no")
                    {
                        if (names.IsMandatory1 == true)
                        {
                            if (txtPONo.Text == "")
                            {
                                check = true;
                                _FieldName = _FieldName + names.FieldValue + ",";
                                break;
                            }
                        }

                    }
                    if (names.FieldName == "Remarks")
                    {
                        if (names.IsMandatory1 == true)
                        {
                            if (txtremarks.Text == "")
                            {
                                check = true;
                                _FieldName = _FieldName + names.FieldValue + ",";
                                break;
                            }
                        }

                    }
                    if (names.FieldName == "1st weight")
                    {
                        if (names.IsMandatory1 == true)
                        {
                            if (FirstWeight.Text == "")
                            {
                                check = true;
                                _FieldName = _FieldName + names.FieldValue + ",";
                                break;
                            }
                        }

                    }
                    if (names.FieldName == "2nd weight")
                    {
                        if (names.IsMandatory1 == true)
                        {
                            if (SecondWeight.Value == "")
                            {
                                check = true;
                                _FieldName = _FieldName + names.FieldValue + ",";
                                break;
                            }
                        }

                    }
                    if (names.FieldName == "Net weight")
                    {
                        if (names.IsMandatory1 == true)
                        {
                            if (lblNetWt.InnerText == "")
                            {
                                check = true;
                                _FieldName = _FieldName + names.FieldValue + ",";
                                break;
                            }
                        }

                    }
                }
            }
            if (_FieldName.Length > 0)
            {
                _FieldName = _FieldName.Remove(_FieldName.Length - 1, 1);
            }
            return check == false ? "0" : "1:" + _FieldName;
        }
        private string CheckSecondValidation()
        {
            bool check = false;
            string _FieldName = "";
            IList<DynamicFieldName> model = _transrepo.GetFieldNameByMachine(Session["WBID"].ToString(), Session["PlantID"].ToString());
            if (model.Count > 0)
            {
                foreach (DynamicFieldName names in model)
                {

                    if (names.FieldName == "Trip Id")
                    {
                        if (names.IsMandatory2 == true)
                        {
                            if (string.IsNullOrEmpty(txtTripId.Text))
                            {
                                check = true;
                                _FieldName = _FieldName + names.FieldValue + ",";
                                break;
                            }
                        }

                    }

                    //if (names.FieldName == "Weighing Type")
                    //{
                    //    if (names.IsMandatory1 == true)
                    //    {
                    //        if (ddlinoutna.SelectedItem.Value == "0")
                    //        {
                    //            check = true;
                    //            _FieldName = _FieldName + names.FieldValue + ",";
                    //            break;
                    //        }
                    //    }

                    //}
                    //if (names.FieldName == "Multi Product")
                    //{
                    //    if (names.IsMandatory1 == true)
                    //    {
                    //        if (!checkmultiproduct.Checked)
                    //        {
                    //            check = true;
                    //            _FieldName = _FieldName + names.FieldValue + ",";
                    //            break;
                    //        }
                    //    }

                    //}
                    if (names.FieldName == "Truck No")
                    {
                        if (names.IsMandatory2 == true)
                        {
                            if (string.IsNullOrEmpty(txtTruckNo.Text))
                            {
                                check = true;
                                _FieldName = _FieldName + names.FieldValue + ",";
                                break;
                            }
                        }

                    }
                    if (names.FieldName == "Material")
                    {
                        if (names.IsMandatory2 == true)
                        {
                            if (ddlmaterial.SelectedItem.Value == "")
                            {
                                check = true;
                                _FieldName = _FieldName + names.FieldValue + ",";
                                break;
                            }
                        }

                    }
                    if (names.FieldName == "Material Classification")
                    {
                        if (names.IsMandatory2 == true)
                        {
                            if (ddlmc.SelectedItem.Value == "0")
                            {
                                check = true;
                                _FieldName = _FieldName + names.FieldValue + ",";
                                break;
                            }
                        }

                    }
                    if (names.FieldName == "Supplier/customer")
                    {
                        if (names.IsMandatory2 == true)
                        {
                            if (ddlsupplier.SelectedItem.Value == "0")
                            {
                                check = true;
                                _FieldName = _FieldName + names.FieldValue + ",";
                                break;
                            }
                        }

                    }
                    if (names.FieldName == "Transporter")
                    {
                        if (names.IsMandatory2 == true)
                        {
                            if (ddltransporter.SelectedItem.Value == "0")
                            {
                                check = true;
                                _FieldName = _FieldName + names.FieldValue + ",";
                                break;
                            }
                        }

                    }
                    if (names.FieldName == "Packing")
                    {
                        if (names.IsMandatory2 == true)
                        {
                            if (ddlpacking.SelectedItem.Value == "0")
                            {
                                check = true;
                                _FieldName = _FieldName + names.FieldValue + ",";
                                break;
                            }
                        }

                    }
                    if (names.FieldName == "Packing qty")
                    {
                        if (names.IsMandatory2 == true)
                        {
                            if (txtpackingqty.Text.Trim() == "")
                            {
                                check = true;
                                _FieldName = _FieldName + names.FieldValue + ",";
                                break;
                            }
                        }

                    }
                    if (names.FieldName == "Challan/Invoice no")
                    {
                        if (names.IsMandatory2 == true)
                        {
                            if (txtInvoiceNo.Text.Trim() == "")
                            {
                                check = true;
                                _FieldName = _FieldName + names.FieldValue + ",";
                                break;
                            }
                        }

                    }
                    if (names.FieldName == "Challan weight")
                    {
                        if (names.IsMandatory2 == true)
                        {
                            if (txtChallanWeight.Text == "")
                            {
                                check = true;
                                _FieldName = _FieldName + names.FieldValue + ",";
                                break;
                            }
                        }

                    }
                    if (names.FieldName == "PO /SO/DO no")
                    {
                        if (names.IsMandatory2 == true)
                        {
                            if (txtPONo.Text == "")
                            {
                                check = true;
                                _FieldName = _FieldName + names.FieldValue + ",";
                                break;
                            }
                        }

                    }
                    if (names.FieldName == "Remarks")
                    {
                        if (names.IsMandatory2 == true)
                        {
                            if (txtremarks.Text == "")
                            {
                                check = true;
                                _FieldName = _FieldName + names.FieldValue + ",";
                                break;
                            }
                        }

                    }
                    if (names.FieldName == "1st weight")
                    {
                        if (names.IsMandatory2 == true)
                        {
                            if (FirstWeight.Text == "")
                            {
                                check = true;
                                _FieldName = _FieldName + names.FieldValue + ",";
                                break;
                            }
                        }

                    }
                    if (names.FieldName == "2nd weight")
                    {
                        if (names.IsMandatory2 == true)
                        {
                            if (SecondWeight.Value == "")
                            {
                                check = true;
                                _FieldName = _FieldName + names.FieldValue + ",";
                                break;
                            }
                        }

                    }
                    if (names.FieldName == "Net weight")
                    {
                        if (names.IsMandatory2 == true)
                        {
                            if (lblNetWt.InnerText == "")
                            {
                                check = true;
                                _FieldName = _FieldName + names.FieldValue + ",";
                                break;
                            }
                        }

                    }
                }
            }
            if (_FieldName.Length > 0)
            {
                _FieldName = _FieldName.Remove(_FieldName.Length - 1, 1);
            }
            return check == false ? "0" : "1:" + _FieldName;
        }


    }
}
