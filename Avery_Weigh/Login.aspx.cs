using Avery_Weigh.Model;
using Avery_Weigh.Repository;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Avery_Weigh
{
    public partial class Login : System.Web.UI.Page
    {
        DataClasses1DataContext db = new DataClasses1DataContext();
        UserMasterRepository umRepo = new UserMasterRepository();
        protected void Page_Load(object sender, EventArgs e)
        {
           
            
            if (!IsPostBack)
            {
                IEnumerable<SiteParameterSetting> setting = db.SiteParameterSettings.ToList();
                if (setting.Count() >= 2)
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "myalert", "toastr.error('Multiple Record found. Please contact with service engineer.');", true);
                }
                else
                {
                    BindSettingsFromDB();
                }
            }
        }

        private void BindSettingsFromDB()
        {
            Image5.ImageUrl = "images/Type5/wi_base_normal.png";
            SiteParameterSetting setting = db.SiteParameterSettings.FirstOrDefault();
            if (setting != null)
            {
                if(setting.Cameras == 1)
                {
                    Image6.ImageUrl = "images/Type5/wi_view_normal.png";
                }
                else
                {
                    Image6.ImageUrl = "images/Type5/wi_view_disable.png";
                }
                if (setting.Sensors == 1)
                {
                    Image7.ImageUrl = "images/Type5/wi_sense_normal.png";
                }
                else
                {
                    Image7.ImageUrl = "images/Type5/wi_sense_disable.png";
                }
                if (setting.RFIDReader == 1)
                {
                    Image8.ImageUrl = "images/Type5/wi_tag_normal.png";
                }
                else
                {
                    Image8.ImageUrl = "images/Type5/wi_tag_disable.png";
                }
                if (setting.ConnectivityToCustomer == 1)
                {
                    Image9.ImageUrl = "images/Type5/wi_connect_normal.png";
                }
                else
                {
                    Image9.ImageUrl = "images/Type5/wi_connect_disable.png";
                }
            }
        }

        protected void BtnLogin_Click(object sender, EventArgs e)
        {
            string _UserName = UserName.Value;
            string _Password = Password.Value;
            string _WBID = WBID.Value;
            string _PlantID = PlantID.Value;
            UserMaster um = umRepo.CheckUserCredentials(_UserName, _Password, _WBID, _PlantID);
            SiteParameterSetting setting = db.SiteParameterSettings.FirstOrDefault();
            PlantMaster _plantCode=db.PlantMasters.Where(x => x.PlantCode == _PlantID).FirstOrDefault();
            if (um!= null && um.Id != 0)
            {
                Session["UserName"] = _UserName;
                Session["Password"] = _Password;
                Session["WBID"] = _WBID;
                Session["UserId"] = um.Id;
                Session["PlantID"] = _PlantID;
                Session["CompanyCode"] =_plantCode.CompanyCode;
                Session["WBFORM"] = "0";
                GLOBALVARIABLE();

                FormsAuthentication.RedirectFromLoginPage(um.Id.ToString(), true);
                UserClassification uc = umRepo.GetUserAuthorization(um.Id);
                New_Weighing_Unit(_WBID, _PlantID);
                
                Session["FIRSTWT_RCD"] = "0";
                Session["SECONDWT_RCD"] = "0";
                if (uc != null)
                {
                    if (uc.GateEntry == true)
                        Response.Redirect("/GateEntryForm");
                    else if (uc.Weighment == false)
                    {
                        Session["WBFORM"] = "1";
                        Response.Redirect("/ManageMasters");
                    }
                    else
                        Response.Redirect("/Manual_Weighment");
                }
                
            }
            else
            {
                ClientScript.RegisterStartupScript(this.GetType(), "myalert", "toastr.error('No User account found. Please try again.');", true);
            }
        }

        private void GLOBALVARIABLE()
        {
            Session["IMAGE1"] = 0;
            Session["IMAGE2"] = 0;
            Session["SENSORCONNECT"] = "0";
            Session["DIOIP"] = "0";
            Session["DIOPORTNO"] = "0";
            Session["RFIDTAGUID"] = "0";
            Session["BALANCE"] = "0";
            Session["RFIDCARDLENGTH"] = "24";
            Session["CAMERAIP1"] = "0";
            Session["CAMERAPORTNO1"] = "0";
            Session["CAMERAIP2"] = "0";
            Session["CAMERAPORTNO2"] = "0";
            Session["CAMERAACTIVE"] = "N";
            Session["CAMERAUSER1"] = "0";
            Session["CAMERAPWD1"] = "0";
            Session["CAMERAUSER2"] = "0";
            Session["CAMERAPWD2"] = "0";
            Session["CAMERAUSER3"] = "0";
            Session["CAMERAPWD3"] = "0";
            Session["CAMERAIP3"] = "0";
            Session["CAMERAPORTNO3"] = "0";
            Session["FIRSTWT_RCD"] = "0";
            Session["SECONDWT_RCD"] = "0";
            Session["ALPHADISPLAYIP1"] = "0";
            Session["ALPHADISPLAYPORTNO1"] = "0";
            Session["ALPHADISPLAYIP2"] = "0";
            Session["ALPHADISPLAYPORTNO2"] = "0";
            Session["ALPHADISPLAYACTIVE"] = "N";
            Session["FLAGREADER1"] = "0";
            Session["FLAGREADER2"] = "0";
        }

      
        //Check Assign Sensor IP Address 
        private void SensorMaster_Glb(string WBId, string PlantId)
        {
            //tblSensorMaster sensor_glb_1 = new tblSensorMaster();
            var sensor_glb = (from a in db.tblSensorMasters
                              join b in db.PlantMasters on a.PlantCode equals b.PlantCode
                              where a.MachineId == WBId && b.PlantCode == PlantId
                              select a).FirstOrDefault();
            if (sensor_glb != null)
            {
                Session["DIOIP"] = sensor_glb.SensorIP.ToString();
                Session["DIOPORTNO"] = sensor_glb.SensorPort.ToString();
                Session["SENSORSA"] = 0;
                Session["SENSORSB"] = 0;
                Session["SENSORSC"] = 0;
            }
            else
            {
                Session["DIOIP"] = "0";
                Session["DIOPORTNO"] = "0";
            }
            
        }

        //Check Assign Alpha Display IP Address 
        private void AlphaDisplayMaster_Glb(string WBId, string PlantId)
        {
            //tblSensorMaster sensor_glb_1 = new tblSensorMaster();
            var alphadisplay_glb = (from a in db.AlphaDisplayMasters 
                              join b in db.PlantMasters on a.PlantCodeId equals b.PlantCode
                              where a.MachineId == WBId && b.PlantCode == PlantId
                              select a).FirstOrDefault();
            if (alphadisplay_glb.AlphaDisplayIdentification =="1")
            {
                Session["ALPHADISPLAYIP1"] = alphadisplay_glb.AlphaDisplayIP.ToString();
                Session["ALPHADISPLAYPORTNO1"] = alphadisplay_glb.AlphaDisplayPort.ToString();
                Session["ALPHADISPLAYMSG1"] = string.Empty;
            }
            else
            {
                Session["ALPHADISPLAYIP1"] = "0";
                Session["ALPHADISPLAYPORTNO1"] = "0";
            }

            if (alphadisplay_glb.AlphaDisplayIdentification == "2")
            {
                Session["ALPHADISPLAYIP2"] = alphadisplay_glb.AlphaDisplayIP.ToString();
                Session["ALPHADISPLAYPORTNO2"] = alphadisplay_glb.AlphaDisplayPort.ToString();

            }
            else
            {
                Session["ALPHADISPLAYIP2"] = "0";
                Session["ALPHADISPLAYPORTNO2"] = "0";
                Session["ALPHADISPLAYMSG2"] = string.Empty;
            }

        }

        //Check Assign Camera IP Address 
        private void CameraMaster_Glb(string WBId, string PlantId)
        {
            //tblSensorMaster sensor_glb_1 = new tblSensorMaster();
            var CameraMaster_glb = (from a in db.CameraMasters
                                    join b in db.PlantMasters on a.PlantCodeID equals b.PlantCode
                                    where a.MachineId == WBId && b.PlantCode == PlantId
                                    select a).FirstOrDefault();
            if (CameraMaster_glb.CameraIndentification == "1")
            {
                Session["CAMERAIP1"] = CameraMaster_glb.CameraIP.ToString();
                Session["CAMERAPORTNO1"] = CameraMaster_glb.CameraPort.ToString();
               
            }
            else
            {
                Session["CAMERAIP1"] = "0";
                Session["CAMERAPORTNO1"] = "0";
            }

            if (CameraMaster_glb.CameraIndentification == "2")
            {
                Session["CAMERAIP2"] = CameraMaster_glb.CameraIP.ToString();
                Session["CAMERAPORTNO2"] = CameraMaster_glb.CameraPort.ToString();

            }
            else
            {
                Session["CAMERAIP2"] = "0";
                Session["CAMERAPORTNO2"] = "0";
            }

        }

        private void New_Cameramaster_Glb(string WBID, string PlantId)
        {
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["AveryDBConnectionString"].ConnectionString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("select * from CameraMaster where PlantCodeID='" + PlantId + "' and MachineId='" + WBID + "'", con))
                {
                    using (SqlDataAdapter ds = new SqlDataAdapter(cmd))
                    {
                        using (DataTable dtbl = new DataTable())
                        {
                            ds.Fill(dtbl);
                            
                            if (dtbl.Rows.Count > 0)
                            {
                                for (int i=0; i< dtbl.Rows.Count;i++)
                                {
                                    if (dtbl.Rows[i]["CameraIndentification"].ToString() == "1")
                                    {
                                        Session["CAMERAIP1"] = dtbl.Rows[i]["CameraIP"].ToString();
                                        Session["CAMERAPORTNO1"] = dtbl.Rows[i]["CameraPort"].ToString();
                                        Session["CAMERAUSER1"] = dtbl.Rows[i]["CameraUserName"].ToString();
                                        Session["CAMERAPWD1"] = dtbl.Rows[i]["CameraPwd"].ToString();

                                    }
                                    if (dtbl.Rows[i]["CameraIndentification"].ToString() == "2")
                                    {
                                        Session["CAMERAIP2"] = dtbl.Rows[i]["CameraIP"].ToString();
                                        Session["CAMERAPORTNO2"] = dtbl.Rows[i]["CameraPort"].ToString();
                                        Session["CAMERAUSER2"] = dtbl.Rows[i]["CameraUserName"].ToString();
                                        Session["CAMERAPWD2"] = dtbl.Rows[i]["CameraPwd"].ToString();

                                    }
                                    if (dtbl.Rows[i]["CameraIndentification"].ToString() == "3")
                                    {
                                        Session["CAMERAIP3"] = dtbl.Rows[i]["CameraIP"].ToString();
                                        Session["CAMERAPORTNO3"] = dtbl.Rows[i]["CameraPort"].ToString();
                                        Session["CAMERAUSER3"] = dtbl.Rows[i]["CameraUserName"].ToString();
                                        Session["CAMERAPWD3"] = dtbl.Rows[i]["CameraPwd"].ToString();

                                    }
                                   
                                }
                               
                            }
                        }

                    }
                }

            }
        }

        private void New_Weighing_Unit(string WBID, string PlantId)
        {
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["AveryDBConnectionString"].ConnectionString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("select * from WeightMachineMaster where PlantCodeID='" + PlantId + "' and MachineId='" + WBID + "'", con))
                {
                    using (SqlDataAdapter ds = new SqlDataAdapter(cmd))
                    {
                        using (DataTable dtbl = new DataTable())
                        {
                            ds.Fill(dtbl);

                            if (dtbl.Rows.Count > 0)
                            {
                                
                                Session["WEIGHINGUNIT"] = dtbl.Rows[0]["WeighingUnit"].ToString();
                                        
                            }
                        }

                    }
                }

            }
        }
    }
}