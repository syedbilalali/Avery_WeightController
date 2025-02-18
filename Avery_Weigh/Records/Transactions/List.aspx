﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="Avery_Weigh.Records.Transactions.List" %>


<%@ Register Src="~/View/Header.ascx" TagPrefix="uc1" TagName="Header" %>
<%@ Register Src="~/View/UserMenu.ascx" TagPrefix="uc1" TagName="UserMenu" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>Avery Weigh Tronix - Weigh</title>
    <link href="/css/style.css" rel="stylesheet" type="text/css" />
    <link rel="stylesheet" href="/css/fonts.css" type="text/css" charset="utf-8" />
    <link rel="icon" href="/images/favicon.png" type="image/gif" sizes="16x16"/>
    <script src="/js/jquery.min.js"></script>
    <script src="/Scripts/jquery-3.3.1.min.js"></script>
    <link href="/Content/toastr.min.css" rel="stylesheet" />
    <script type="text/javascript">
        function MM_swapImgRestore() { //v3.0
            var i, x, a = document.MM_sr; for (i = 0; a && i < a.length && (x = a[i]) && x.oSrc; i++) x.src = x.oSrc;
        }
        function MM_preloadImages() { //v3.0
            var d = document; if (d.images) {
                if (!d.MM_p) d.MM_p = new Array();
                var i, j = d.MM_p.length, a = MM_preloadImages.arguments; for (i = 0; i < a.length; i++)
                    if (a[i].indexOf("#") != 0) { d.MM_p[j] = new Image; d.MM_p[j++].src = a[i]; }
            }
        }

        function MM_findObj(n, d) { //v4.01
            var p, i, x; if (!d) d = document; if ((p = n.indexOf("?")) > 0 && parent.frames.length) {
                d = parent.frames[n.substring(p + 1)].document; n = n.substring(0, p);
            }
            if (!(x = d[n]) && d.all) x = d.all[n]; for (i = 0; !x && i < d.forms.length; i++) x = d.forms[i][n];
            for (i = 0; !x && d.layers && i < d.layers.length; i++) x = MM_findObj(n, d.layers[i].document);
            if (!x && d.getElementById) x = d.getElementById(n); return x;
        }

        function MM_swapImage() { //v3.0
            var i, j = 0, x, a = MM_swapImage.arguments; document.MM_sr = new Array; for (i = 0; i < (a.length - 2); i += 3)
                if ((x = MM_findObj(a[i])) != null) { document.MM_sr[j++] = x; if (!x.oSrc) x.oSrc = x.src; x.src = a[i + 2]; }
        }
</script>
    <script>
        $(document).ready(function () {
            $(".MainPopBtn").click(function () {
                $(".Popup").show();
            });
            $(".Close").click(function () {
                $(".Popup").hide();
            });

        });
</script>
    <style type="text/css">
        .toast {
            opacity: 1 !important;
        }


        .trclass:hover {
            color: black;
            background-color: lightgray;
        }

        .selectedrow {
            background-color: coral;
            color: black;
        }
    </style>
    <script src="/Scripts/toastr.min.js"></script>
    <script type="text/javascript">
        toastr.options = {
            "closeButton": false,
            "debug": false,
            "newestOnTop": true,
            "progressBar": true,
            "positionClass": "toast-top-right",
            "preventDuplicates": false,
            "onclick": null,
            "showDuration": "300",
            "hideDuration": "1000",
            "timeOut": "5000",
            "extendedTimeOut": "1000",
            "showEasing": "swing",
            "hideEasing": "linear",
            "showMethod": "fadeIn",
            "hideMethod": "fadeOut"
        }
        function isEmail(email) {
            var regex = /^([a-zA-Z0-9_.+-])+\@(([a-zA-Z0-9-])+\.)+([a-zA-Z0-9]{2,4})+$/;
            return regex.test(email);
        }
    </script>
</head>

<body onload="MM_preloadImages('images/contact/contact_hover.png','images/contact/info_hover.png','images/contact/login_hover.png','images/close_hover.png','images/fullscreen_hover.png','images/exit_fullscreen_hover.png','images/type3/copy_hover.png','images/type3/previous_page_hover.png','images/type3/previous_record_hover.png','images/type3/next_record_hover.png','images/type3/next_page_hover.png')">
   
    <form id="form1" runat="server">
        <div class="Wrapper WeighMain">
            <!--Header start here-->
            <uc1:Header runat="server" ID="Header" />
            <!--Header start here-->

            <!--MiddelSection Starts-->
            <div class="MiddleSection">
                <div class="LeftSec">
                    <uc1:UserMenu runat="server" ID="UserMenu" />
                    <span>
                        <img src="/images/intellegence-left-logo.png" alt="" /></span>
                </div>

                <div class="RightSec">
                    <div class="RightInn11">
                        <h4>MASTERS</h4>
                        <h4 class="SupplierSearchBig">TRANSACTION RECORDS<a href="/ManageMasters" onmouseout="MM_swapImgRestore()" onmouseover="MM_swapImage('Image32','','/images/close_hover.png',1)" class="CloseButtonA">
                            <img src="/images/close_normal.png" class="Close" alt="Close" name="Image32" width="26" height="25" border="0" id="Image32" /></a></h4>
                    </div>
                    <div class="RightInn11">
                        <!-- <div class="RightInn22">3333</div> -->
                        <div class="RightInnMasters">
                            <div class="ConatctIcons2">
                                <ul style="display: none;">
                                    <li><a href="javascript:void(0);" onmouseout="MM_swapImgRestore()" onmouseover="MM_swapImage('Copy','','images/type3/copy_hover.png',1)">
                                        <img src="/images/type3/copy_normal.png" name="Copy" width="50" height="50" border="0" id="Copy" /></a></li>
                                    <li><a href="javascript:void(0);" onmouseout="MM_swapImgRestore()" onmouseover="MM_swapImage('previous_page','','images/type3/previous_page_hover.png',1)">
                                        <img src="/images/type3/previous_page_normal.png" name="previous_page" width="50" height="50" border="0" id="previous_page" /></a></li>
                                    <li><a href="javascript:void(0);" onmouseout="MM_swapImgRestore()" onmouseover="MM_swapImage('previous_record','','images/type3/previous_record_hover.png',1)">
                                        <img src="/images/type3/previous_record_normal.png" name="previous_record" width="50" height="50" border="0" id="previous_record" /></a></li>
                                    <li><a href="javascript:void(0);" onmouseout="MM_swapImgRestore()" onmouseover="MM_swapImage('next_record','','images/type3/next_record_hover.png',1)">
                                        <img src="/images/type3/next_record_normal.png" name="next_record" width="50" height="50" border="0" id="next_record" /></a></li>
                                    <li><a href="login.html" onmouseout="MM_swapImgRestore()" onmouseover="MM_swapImage('next_page','','images/type3/next_page_hover.png',1)">
                                        <img src="/images/type3/next_page_normal.png" name="next_page" width="50" height="50" border="0" id="next_page" /></a></li>
                                </ul>
                            </div>
                          
                            <div id="dbMain" runat="server" style="display:none;">
                               <asp:HiddenField ID="RecordId" runat="server" />
                            <table style="width: 100%;" border="1" bordercolor="#72aac0" cellspacing="0" cellpadding="0" bgcolor="#FFFFFF" class="MainLTable">
                                <asp:Repeater ID="rptList" runat="server" OnItemCreated="rptList_ItemCreated">
                                    <HeaderTemplate>
                                        <tr>
                                            <td align="center" valign="middle" bgcolor="#d8e7ee">No.</td>
                                            <td align="center" valign="middle" bgcolor="#d8e7ee">Truck No</td>
                                            <td align="center" valign="middle" bgcolor="#d8e7ee">Trip Type</td>
                                            <td align="center" valign="middle" bgcolor="#d8e7ee">IsMulti-Product</td> 
                                            <td align="center" valign="middle" bgcolor="#d8e7ee">Material</td> 
                                            <td align="center" valign="middle" bgcolor="#d8e7ee">Gross Wt.</td>
                                            <td align="center" valign="middle" bgcolor="#d8e7ee">Date In</td>
                                            <td align="center" valign="middle" bgcolor="#d8e7ee">Time In</td>
                                            <td align="center" valign="middle" bgcolor="#d8e7ee">Tare Wt.</td>
                                            <td align="center" valign="middle" bgcolor="#d8e7ee">Date Out</td>
                                            <td align="center" valign="middle" bgcolor="#d8e7ee">Time Out</td>
                                            <td align="center" valign="middle" bgcolor="#d8e7ee">Net Weight</td>
                                            <td align="center" valign="middle" bgcolor="#d8e7ee">Supplier</td>
                                            <td align="center" valign="middle" bgcolor="#d8e7ee">Transporter</td>
                                        </tr>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <tr class="trclass" data-RowId="<%#Eval("Id")%>"">
                                            <td align="center" valign="middle"><asp:Label ID="lblindex" runat="server"></asp:Label></td>
                                            <td align="center" valign="middle"><%#Eval("TruckNo") %></td>
                                            <td align="center" valign="middle"><%#Eval("TripType") %></td>
                                            <td align="center" valign="middle"><%#Eval("IsMultiProduct") %></td>
                                            <td align="center" valign="middle"><%#Eval("Materials") %></td>
                                            <td align="center" valign="middle"><%#Eval("GrossWt") %></td>
                                            <td align="center" valign="middle"><%#Eval("DateIn") %></td>
                                            <td align="center" valign="middle"><%#Eval("TimeIn") %></td>
                                            <td align="center" valign="middle"><%#Eval("TareWt") %></td>
                                            <td align="center" valign="middle"><%#Eval("DateOut") %></td>
                                            <td align="center" valign="middle"><%#Eval("TimeOut") %></td>
                                            <td align="center" valign="middle"><%#Eval("NetWt") %></td>
                                            <td align="center" valign="middle"><%#Eval("Supplier") %></td>
                                            <td align="center" valign="middle"><%#Eval("Transporter") %></td>
                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </table>
                                </div>
                            <table id="tblNone" runat="server" visible="false" style="width: 100%;" border="1" bordercolor="#72aac0" cellspacing="0" cellpadding="0" bgcolor="#FFFFFF" class="MainLTable">
                                <thead>
                                    <tr>
                                        <td align="center" valign="middle" bgcolor="#d8e7ee">No.</td>
                                            <td align="center" valign="middle" bgcolor="#d8e7ee">Truck No</td>
                                            <td align="center" valign="middle" bgcolor="#d8e7ee">Trip Type</td>
                                            <td align="center" valign="middle" bgcolor="#d8e7ee">IsMulti-Product</td>
                                            <td align="center" valign="middle" bgcolor="#d8e7ee">Material</td> 
                                            <td align="center" valign="middle" bgcolor="#d8e7ee">Gross Wt.</td>
                                            <td align="center" valign="middle" bgcolor="#d8e7ee">Date In</td>
                                            <td align="center" valign="middle" bgcolor="#d8e7ee">Time In</td>
                                            <td align="center" valign="middle" bgcolor="#d8e7ee">Tare Wt.</td>
                                            <td align="center" valign="middle" bgcolor="#d8e7ee">Date Out</td>
                                            <td align="center" valign="middle" bgcolor="#d8e7ee">Time Out</td>
                                            <td align="center" valign="middle" bgcolor="#d8e7ee">Net Weight</td>
                                            <td align="center" valign="middle" bgcolor="#d8e7ee">Supplier</td>
                                            <td align="center" valign="middle" bgcolor="#d8e7ee">Transporter</td>                           
                                    </tr>
                                </thead>
                                <tr>
                                    <td colspan="14" style="text-align:center;color:red">No Record Found</td>
                                </tr>
                            </table>
                        </div>
                    </div>
                </div>

            </div>
            <!--MiddelSection Ends-->

           

            <!--Footer Ends-->
            <div class="footer">
                <ul class="selected">
                    <li><a href="Search.aspx" onmouseout="MM_swapImgRestore()" onmouseover="MM_swapImage('Image11','','/images/type4/search_hover.png',1)">
                        <img src="/images/type4/search_normal.png" name="Image11" width="80" height="50" border="0" id="Image11" /></a></li>
                    <li><asp:LinkButton runat="server" ID="AddEdit" OnClick="AddEdit_Click"  onmouseout="MM_swapImgRestore()" onmouseover="MM_swapImage('Image12','','/images/type4/edit_hover.png',1)">
                        <img src="/images/type4/edit_normal.png" name="Image12" width="80" height="50" border="0" id="Image12" /></asp:LinkButton></li>
                    <li><asp:LinkButton ID="btnSave" runat="server" OnClick="btnSave_Click" Visible="false" onmouseout="MM_swapImgRestore()" onmouseover="MM_swapImage('Image13','','/images/type4/save_hover.png',1)">
                        <img src="/images/type4/save_normal.png" name="Image13" width="80" height="50" border="0" id="Image13" /></asp:LinkButton></li>
                    <li><asp:LinkButton ID="BtnExport" PostBackUrl="~/Camera_Master/Import.aspx"  runat="server" onmouseout="MM_swapImgRestore()" onmouseover="MM_swapImage('Imag14','','/images/type4/export_hover.png',1)">
                        <img src="/images/type4/export_normal.png" alt="Export" name="Imag14" width="80" height="50" border="0" id="Imag14" /></asp:LinkButton></li>

                    <li><asp:LinkButton ID="BtnDownloadExcel" OnClick="BtnDownloadExcel_Click" runat="server" onmouseout="MM_swapImgRestore()" onmouseover="MM_swapImage('Image15','','/images/type4/import_hover.png',1)">
                        <img src="/images/type4/import_normal.png" name="Image15" width="80" height="50" border="0" id="Image15" /></asp:LinkButton></li>
                    <li><a href="javascript:void(0)" onmouseout="MM_swapImgRestore()" onmouseover="MM_swapImage('Image16','','/images/type4/print_hover.png',1)">
                        <img src="/images/type4/print_normal.png" name="Image16" width="80" height="50" border="0" id="Image16" /></a></li>
                    <li><asp:LinkButton ID="Delete" OnClick="Delete_Click" runat="server"  OnClientClick="return Delete();"
                        CommandName="Delete" onmouseout="MM_swapImgRestore()" onmouseover="MM_swapImage('Image17','','/images/type4/delete_hover.png',1)">
                        <img src="/images/type4/delete_normal.png" name="Image17" width="80" height="50" border="0" id="Image17" /></asp:LinkButton></li>
                </ul>
            </div>
            <script type="text/javascript">

                $('.trclass').click(function () {
                    if ($('.trclass').hasClass("selectedrow")) {
                        $('.trclass').removeClass('selectedrow');
                        $("#RecordId").val('');
                    }
                    else if (!$('.trclass').hasClass("selectedrow")) {
                        $(this).addClass('selectedrow');
                        $("#RecordId").val(($(this).attr("data-RowId")));
                        $(this).siblings().removeClass('selectedrow');
                    }
                });
                function Delete() {
                    var result = true;
                    var value = $('#RecordId').val();
                    if (value != '') {
                        return confirm('Do You want to delete');
                    }
                    else {
                        toastr.info('Please Select A Record To Delete!');
                        result = false;
                    }
                    return result;
                }
            </script>
        </div>
    </form>
</body>
</html>
