<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MemberRegistration.aspx.cs" EnableEventValidation="false" Inherits="IPRS_Member.MemberRegistration" %>

<%-- For Ajax controls like HiddenFiled, UpdateProgress : Commented By Rohit --%>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<%-- Here we use this user control to find the geographic information by entering the pincode. : Commented By Rohit --%>
<%@ Register Src="~/User_Controls/ucDropDown.ascx" TagPrefix="ucDropDown" TagName="ucDropDown" %>

<%-- Here we use this user control to give inline expression to textbox and other control. : Commented By Rohit --%>
<%@ Register Src="~/User_Controls/ucTooltip.ascx" TagPrefix="ucTooltip" TagName="ucTooltip" %>



<script src="Javascript/FCalender.js"></script>
<link href="Style/zcCalendar.css" rel="stylesheet" />

<script type="text/javascript">
   /* Convert Date Format into DD-MM-YYYY : Commented By Rohit */
    function dateSelectionChanged(sender, args) {
        selectedDate = sender.get_selectedDate();
        var date = moment(new Date(selectedDate));

        sender._textbox._element.value = (moment(date).format('DD-MM-YYYY'));
        SetDateField(sender._textbox._element);
    }   

    /* Pan Card No. Validation asper Individual,Firm and Company : Commented By Rohit*/
    function ValidatePAN(event) {
        var PANNo = document.getElementById('<%=txtPan.ClientID %>').value;
        var RegType = document.getElementById('<%= hdnAccountRegType.ClientID %>').value;
        if (PANNo.value != "") {
            if (RegType == "I" || RegType == "C" || RegType == "LH" || RegType == "LHN") {
                var ObjVal = PANNo;
                var panPattern = /^([a-zA-Z]{5})(\d{4})([a-zA-Z]{1})$/;
                var matchArray = ObjVal.match(panPattern);
                if (matchArray == null) {
                    alert('Invalid PAN Card No.');
                }
            }
        }
    }
</script>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Member Registration</title>

    <meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />
    <!-- Meta, title, CSS, favicons, etc. -->
    <meta charset="utf-8" />
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <!-- Bootstrap -->
    <link href="~/Style/bootstrap.min.css" rel="stylesheet" />
    <!-- Font Awesome -->
    <link href="~/Style/font-awesome/font-awesome.min.css" rel="stylesheet" />
    <!-- NProgress -->
    <link href="~/Style/nprogress.css" rel="stylesheet" />
    <!-- iCheck -->
    <link href="~/Style/skins/flat/green.css" rel="stylesheet" />
    <!-- bootstrap-progressbar -->
    <link href="~/Style/bootstrap-progressbar-3.3.4.min.css" rel="stylesheet" />

    <!-- Custom Theme Style -->
    <link href="~/Style/custom.min.css" rel="stylesheet" />

    <!-- Custom Theme Scripts -->
    <script src="~/javascript/custom.min.js"></script>

    <script src="javascript/jquery/jquery.min.js"></script>
    <!-- Bootstrap -->
    <script src="javascript/bootstrap/bootstrap.min.js"></script>

    <!-- Bootstrap Alert Box -->
    <script src="javascript/bootstrap/bootbox.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/jquery/2.2.2/jquery.min.js"></script>

    <!--TTHIS FUNCTION IS TO RESTRICT  ENTER EKY FORM SUMISSION -->
    <script type="text/javascript">
        window.addEventListener('keydown', function (e) {
            if (e.keyIdentifier == 'U+000A' || e.keyIdentifier == 'Enter' || e.keyCode == 13) {
                if (e.target.nodeName == 'INPUT' && e.target.type == 'text') {
                    e.preventDefault();
                    return false;
                }
            }
        }, true);
    </script>

    <style>
        .field-icon {
            float: right;
            margin-left: -35px;
            margin-top: 5px;
            position: absolute;
            z-index: 2;
        }

        .uppercase {
            text-transform: uppercase;
        }

        .loadingmodal {
            position: fixed;
            z-index: 999 !important;
            height: 100%;
            width: 100%;
            top: 0;
            background-color: Black;
            filter: alpha(opacity=60);
            opacity: 0.8;
            -moz-opacity: 1;
        }

        .loadingcenter {
            z-index: 1000 !important;
            width: 200px;
            height: 100px;
            position: absolute;
            left: 45%;
            top: 60%;
            margin-left: -150px;
            margin-top: -150px;
            opacity: 1;
            /*-moz-opacity: 1;*/
            background: url("Images/loading.gif") no-repeat;
        }
    </style>

</head>
    
    <body style="background-color: white;">
    <form id="form1" runat="server" enctype="multipart/form-data">
        <asp:ScriptManager ID="ScriptManager1" runat="server" />
            <div id="DivHeader" runat="server">
                <asp:Image runat="server" ImageUrl="~/Images/header.jpg" class="divtable" Width="100%" />
            </div>

        <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="upnlwzMain">
            <ProgressTemplate>
                <div class="loadingmodal">
                    <div class="loadingcenter">
                    </div>
                </div>
            </ProgressTemplate>
        </asp:UpdateProgress>

        <asp:UpdatePanel ID="upnlwzMain" UpdateMode="Conditional" runat="server"  >
            <ContentTemplate>
                <div class="row">
                    <div class="x_panel">
                        <div class="x_content">
                            <div class="x_title">
                                <h4>
                                <asp:Label ID="lblFormTitle" runat="server" Style="font-weight: bold; color: #1f72ba;"></asp:Label>
                                </h4>
                        <%-- Here we assign value in HiddenField for store in App_Accounts_Temp table : Commented By Rohit --%>
                                <asp:HiddenField ID="hdnRecordKeyId" runat="server" Value="0" />
                                <asp:HiddenField ID="hdnAccountRegType" runat="server" Value="" />
                                <asp:HiddenField ID="hdnAccountCode" runat="server" Value="" />
                                <asp:HiddenField ID="hdnAccountId" runat="server" Value="0" />
                                <asp:HiddenField ID="hdndAccountId" runat="server" Value="0" />
                                <asp:HiddenField ID="hdnReturnMessage" runat="server" Value="0" />
                                <asp:HiddenField ID="hdnRoleTypeIds" runat="server" Value="" />
                                <asp:HiddenField ID="hdnOTP" runat="server" Value="0" />
                                <asp:HiddenField ID="hdnOTPCount" runat="server" Value="0" />
                                <asp:HiddenField ID="hdnEmail" runat="server" Value="0" />
                                <asp:HiddenField ID="hdnOTPmobile" runat="server" Value="0" />
                                <asp:HiddenField ID="hdnOTPmobileCount" runat="server" Value="0" />
                                <asp:HiddenField ID="hdnMobile" runat="server" Value="0" />
                                <asp:HiddenField ID="hdnEcapValue"  runat="server"  Value="" />                                                               
                                <div class="clearfix"></div>
                            </div>
                            <br />

                            <div class="col-xs-12 col-sm-12 col-lg-12">
                                <asp:Wizard ID="wzMain" CssClass="col-sm-12" DisplaySideBar="false"
                                    runat="server" class="form_wizard wizard_horizontal"
                                    OnNextButtonClick="wzMain_NextButtonClick" ActiveStepIndex="0"
                                    OnPreviousButtonClick="wzMain_PreviousButtonClick">
                                    <WizardSteps>
                                        <%--Details Of Member Along with Deceased Member : Commented By Rohit--%>
                                        <asp:WizardStep ID="Step0" StepType="Start" runat="server">

                                            <%--Details Of Deceased Member : Commented By Rohit--%> 
                                            <div class="row" id="divLegDetail" runat="server" visible="false">
                                                    <div class="fieldset panel">

                                                   <div class="fieldset-body panel-body">
                                                <h5 class="legend-Panel"><strong class="text-uppercase"><span id="SectionTitle" runat="server">Deceased Member Details</span> </strong></h5>
                                                            <div class="row" id="divLegelHirePre" runat="server" >
                                                            <div class="col-xs-12 col-sm-12 col-lg-6">
                                                                <div class="input-group">
                                                                    <span class="input-group-addon" runat="server">Deceased Member Name</span>
                                                                    <asp:TextBox ID="txtdeceasedMemName" MaxLength="100" runat="server" CssClass="form-control" AutoCompleteType="Disabled" autocomplete="off"
                                                                        placeholder="Enter Deceased Member Name" required />
                                                                </div>                                                            
                                                             </div> 
                                                            <div class="col-xs-12 col-sm-12 col-lg-6" id="div3" runat="server">
                                                                <div class="input-group">
                                                                 <span class="input-group-addon" id="Span2" runat="server">Relationship</span>
                                                                        <asp:DropDownList ID="DDLRelationship" runat="server" CssClass="form-control" AutoCompleteType="Disabled" autocomplete="off">
                                                                        <asp:ListItem Text="Select Relationship" Value="Select Relationship"></asp:ListItem>
                                                                        <asp:ListItem Text="Parent" Value="Parent"></asp:ListItem>
                                                                        <asp:ListItem Text="Child" Value="Child"></asp:ListItem>
                                                                        <asp:ListItem Text="Spouse" Value="Spouse"></asp:ListItem>
                                                                        <asp:ListItem Text="Sibling" Value="Sibling"></asp:ListItem>
                                                                        <asp:ListItem Text="Grandparents" Value="Grandparents"></asp:ListItem>
                                                                        <asp:ListItem Text="Grandchild" Value="Grandchild"></asp:ListItem>
                                                                        <asp:ListItem Text="Parent sibling" Value="Parent sibling"></asp:ListItem>
                                                                        <asp:ListItem Text="Sibling child" Value="Sibling child"></asp:ListItem>
                                                                        <asp:ListItem Text="Aunt_Uncle child" Value="Aunt_Uncle child"></asp:ListItem>
                                                                        </asp:DropDownList>
                                                                </div>
                                                            </div>
                                                        </div> 
                                                            <div class="row" id="divBirthInfo" runat="server" >
                                                                    <div class="col-xs-12 col-sm-12 col-lg-6">
                                                                            <div class="input-group">
                                                                                <span class="input-group-addon">Date of Birth</span>
                                                                                <asp:TextBox ID="txtDOB" runat="server" required class="form-control" Style="width: 98%" AutoCompleteType="Disabled" autocomplete="off" placeholder="dd-MMM-yyyy" onBlur="GetDateField(this,false);" onFocus="SetDateField(this);" ToolTip="Format : [ddmmyyyy], [d] or [dd], [ddm] or [ddmm], [ ].&#xa;Keys : Day-[Up/Down], Month-[alt+Up/Down], Year-[shift+Up/Down], Today-[Home]."  >
                                                                                </asp:TextBox> 
                                                                                <asp:CalendarExtender ID="calDOB" OnClientDateSelectionChanged="dateSelectionChanged" CssClass="zcCalendar" runat="server" TargetControlID="txtDOB" Format="dd-MMM-yyyy"></asp:CalendarExtender> 
                                                                            </div>
                                                                    </div>
                                                                    <div class="col-xs-12 col-sm-12 col-lg-6">
                                                                            <div class="input-group">
                                                                                <span class="input-group-addon">Date of death
                                                                                </span>
                                                                                <asp:TextBox ID="txtdateofdeath" runat="server" required class="form-control" Style="width: 98%" AutoCompleteType="Disabled" autocomplete="off" placeholder="dd-MMM-yyyy" onBlur="GetDateField(this,false);" onFocus="SetDateField(this);" ToolTip="Format : [ddmmyyyy], [d] or [dd], [ddm] or [ddmm], [ ].&#xa;Keys : Day-[Up/Down], Month-[alt+Up/Down], Year-[shift+Up/Down], Today-[Home]."  >
                                                                                </asp:TextBox> 
                                                                                <asp:CalendarExtender ID="calDOB1" OnClientDateSelectionChanged="dateSelectionChanged" CssClass="zcCalendar" runat="server" TargetControlID="txtdateofdeath" Format="dd-MMM-yyyy"></asp:CalendarExtender>                                                                                
                                                                            </div>
                                                                    </div>
                                                            </div>
                                                                                                           
                                                            <div class="row" id="divcertficate" runat="server" >
                                                                    <div class="col-xs-12 col-sm-12 col-lg-6">
                                                                            <div class="input-group">
                                                                                <span class="input-group-addon">Death Certificate Number
                                                                                </span>
                                                                                <asp:TextBox ID="txtdatecertNumber" placeholder="Enter Death Certificate Number." runat="server" maxlength="100" class="form-control" Style="width: 98%" AutoCompleteType="Disabled" autocomplete="off"   >
                                                                                </asp:TextBox> 
                                                                            </div>
                                                                    </div>
                                                                <div class="col-xs-12 col-sm-6 col-lg-6">
                                                                            <div class="input-group">
                                                                                <span class="input-group-addon">Address
                                                                                </span>
                                                                                <textarea id="txtAddress_PM" maxlength="255" runat="server" class="form-control" autocompletetype="Disabled" autocomplete="off"
                                                                                    placeholder="Enter Permanent Address" textmode="MultiLine" rows="3" cols="20" />
                                                                            </div>
                                                                 </div>
                                                            </div> 

                                                            <div class="row" runat ="server" id="divaddress">                                                                        
                                                                        
                                                                    </div>
                                                            <div class="row">
                                                                        <div class="col-xs-12 col-sm-6 col-lg-6" id="divPerPincode" runat="server" >
                                                                            <div class="input-group">
                                                                                <span class="input-group-addon">Pincode</span>
                                                                                <ucDropDown:ucDropDown runat="server" ID="ddlPincode_PM" MinimumPrefixLength="2"  blnChangeEvent="true" strMessage="Pincode" />
                                                                                <a id="helpTooltip3" title="Minimum 2 Letter">
                                                                                    <img src="Images/help-icon.png" style="position: absolute; right: 3px; z-index: 100; cursor: pointer; top: 5px">
                                                                                </a>
                                                                               
                                                                                <%--<asp:TextBox ID="txtPincode_PR" MaxLength="6" runat="server" CssClass="form-control" AutoCompleteType="Disabled" autocomplete="off" pattern="^([0-9]{6,6})$"
                                                                                        placeholder="Enter Pincode" />
                                                                                    <ajaxToolkit:FilteredTextBoxExtender ID="FilteredTextBoxExtender1" runat="server" TargetControlID="txtPincode_PR"
                                                                                        FilterType="Custom" ValidChars="1234567890"></ajaxToolkit:FilteredTextBoxExtender>--%>
                                                                            </div>
                                                                        </div>

                                                                        <div class="col-xs-12 col-sm-12 col-lg-6" id="divgeopre" runat="server" >
                                                                            <div class="input-group">
                                                                                <span class="input-group-addon">Geographical*</span>
                                                                                
                                                                                <ucDropDown:ucDropDown runat="server" ID="ddlGeo_PM" MinimumPrefixLength="2" blEnabled="false" strmessage="Country / State / City" />
                                                                                <ucTooltip:ucTooltip runat="server" tooltipanchor="Minimum 2 Letter" ID="ucTooltip5" />
                                                                            </div>
                                                                        </div>
                                                              </div>

                                                       <div class="row" id="DivDTrader"  runat ="server"  >
                                                <div class="col-xs-12 col-sm-12 col-lg-6">
                                                    <div class="input-group ">
                                                        <span class="input-group-addon">Alias/ Trader Name</span>
                                                        <asp:TextBox ID="txtDAccountAlias" MaxLength="100" runat="server" CssClass="form-control uppercase" AutoCompleteType="Disabled" autocomplete="off"
                                                            placeholder="Enter Alias/ Trader Name" title="Alias Name / Trade Name / Credit Name" />
                                                        <a id="helpDTooltip" title="NOTE:Pseudo Name/Pen Name, please use (,) comma as separator of multiple names">
                                                            <img src="Images/help-icon.png" style="position: absolute; right: 3px; z-index: 100; cursor: pointer; top: 5px">
                                                            </a>
                                                    </div>
                                                </div>

                                                <div class="col-xs-12 col-sm-12 col-lg-6" id="divroletyped" runat ="server" >
                                                    <div>
                                                        <span class="input-group-addon" style="border-right: 1px">Role Type
                                                        </span>
                                                        <div class="checkbox" style="margin-top: 0px !important; margin-bottom: 0px !important;">
                                                            <asp:CheckBoxList ID="cbxRoleTyped"  AutoPostBack="true" RepeatDirection="Vertical" RepeatLayout="Flow" Style="list-style: none" runat="server" CssClass="form-control-static checkbox-inline">
                                                            </asp:CheckBoxList>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                                           <%--  <div class="row">
                                                                            <div class="col-xs-12 col-sm-6 col-lg-6">
                                                                            <div class="input-group">
                                                                                <span class="input-group-addon" runat="server" id="spnUpload">Deceased Member Photo
                                                                                </span>     
                                                                                <asp:FileUpload ID="FPuploadPhoto" Width="100%" CssClass="form-control" runat="server" placeholder="ABC" AllowMultiple="false" accept="image/gif, image/jpeg, image/png" />                                                                                                                                                               
                                                                            </div>
                                                                            </div>
                                                                            <asp:Button ID="btnPhotoUpload" runat="server" OnClick="btnPhotoUpload_Click" OnClientClick="return PhotoValidate();" Text="Upload Image" CssClass="btn btn-primary" />
                                                                            <asp:HyperLink ID="btnphotoView" runat="server" data-toggle="modal" data-target="#myModal">
                                                                            <asp:Image runat="server" ID="ImgUser" Style="border: 1px solid; cursor: pointer" Width="70px" Height="70px" />
                                                                        </asp:HyperLink>
                                                                        <asp:HiddenField ID="hdnphotoImageName" runat="server" />
                                                                        <div class="modal" id="myModal">
                                                                            <div class="modal-dialog">
                                                                                <div class="modal-content">
                                                                                    <!-- Modal body -->
                                                                                    <div class="modal-body" style="text-align: center">
                                                                                        <asp:Image runat="server" ID="ImgUserLarge" Style="border: 0px" />
                                                                                    </div>
                                                                                    <!-- Modal footer -->
                                                                                    <div class="modal-footer">
                                                                                        <button type="button" class="btn btn-danger" data-dismiss="modal">Close</button>
                                                                                    </div>
                                                                                </div>
                                                                            </div>
                                                                        </div>
                                                                </div>    --%>
                                         </div> 
                                         </div> 
                                         </div>
                                             <%--End Of Details Of Deceased Member : Commented By Rohit--%> 
                                            
                                            <br />
                                            <br />

                                            <h5 class="legend-Panel"><strong class="text-uppercase"><span id="SpLH" runat="server"></span> </strong></h5>
                                             <%--Details of Members : Commented By Rohit--%> 

                                            <div class="row">
                                            <div class="col-xs-12 col-sm-12 col-lg-8">
                                                    <div class="input-group">
                                                       
                                                    </div>
                                                </div>
                                            <div class="col-xs-12 col-sm-12 col-lg-10" style="display: none">
                                                    <div class="input-group">
                                                        <span id="SPCompanyNote" runat="server" visible="false" style="font-weight: bold">NOTE: PLEASE ENTER NAME OF PROPRIETOR / DIRECTOR  / PARTNER / COMPANY REPRESENTATIVE</span>
                                                    </div>
                                                </div>
                                            </div>

                                            <%--Only for Publisher Member : Commented By Rohit--%> 
                                            <div class="row">
                                                <div class="col-xs-12 col-sm-12 col-lg-6" id="divCompany" runat="server" visible="false">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">Company Name*</span>
                                                        <asp:TextBox ID="txtCompanyName" MaxLength="100" runat="server" CssClass="form-control uppercase" AutoCompleteType="Disabled" autocomplete="off"
                                                            placeholder="Enter Company Name" required />
                                                    </div>
                                                </div>
                                            </div>
                                             <%--End of Only for Publisher Member : Commented By Rohit--%> 

                                            <%--added by Hariom 17-04-2023--%>
                                            <h5 style="color:red">Note: Name should be same as the name in Pancard</h5>

                                            <%--end--%>
                                             <%--Personal Details of Members : Commented By Rohit--%>
                                            <div class="row">
                                                <div class="col-xs-12 col-sm-12 col-lg-6">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">First & Middle Name*</span>
                                                        <asp:TextBox ID="txtfn" MaxLength="35" runat="server" CssClass="form-control uppercase" AutoCompleteType="Disabled" autocomplete="off"
                                                            placeholder="Enter First and Middle Name" required />
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-12 col-lg-6">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">Last Name</span>
                                                        <asp:TextBox ID="txtLname" MaxLength="30" runat="server" CssClass="form-control uppercase" AutoCompleteType="Disabled" autocomplete="off"
                                                            placeholder="Enter Last Name"  />
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="row" id="DivTrader"  runat ="server" visible="true"  >
                                                <div class="col-xs-12 col-sm-12 col-lg-6">
                                                    <div class="input-group ">
                                                        <span class="input-group-addon">Alias/ Trader Name</span>
                                                        <asp:TextBox ID="txtAccountAlias" MaxLength="100" runat="server" CssClass="form-control uppercase" AutoCompleteType="Disabled" autocomplete="off"
                                                            placeholder="Enter Alias/ Trader Name" title="Alias Name / Trade Name / Credit Name" /><a id="helpTooltip" title="NOTE:Pseudo Name/Pen Name, please use (,) comma as separator of multiple names">
                                                                <img src="Images/help-icon.png" style="position: absolute; right: 3px; z-index: 100; cursor: pointer; top: 5px">
                                                            </a>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="col-xs-12 col-sm-12 col-lg-6">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">Present<br />
                                                            Address*
                                                        </span>
                                                        <textarea id="txtAddress" maxlength="255" runat="server" class="form-control uppercase" cols="20" autocomplete="off" rows="5"
                                                            placeholder="Enter Address" required />
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-12 col-lg-6" id="divroletype" runat ="server" >
                                                    <div>
                                                        <span class="input-group-addon" style="border-right: 1px">Role Type
                                                        </span>
                                                        <div class="checkbox" style="margin-top: 0px !important; margin-bottom: 0px !important;">
                                                            <asp:CheckBoxList ID="cbxRoleType"  AutoPostBack="true" RepeatDirection="Vertical" RepeatLayout="Flow" Style="list-style: none" runat="server" CssClass="form-control-static checkbox-inline">
                                                            </asp:CheckBoxList>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                           
                                            <%--Not Populate for NRI : Commented By Rohit --%>
                                            <div class="row">
                                            <div class="col-xs-12 col-sm-12 col-lg-6">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">Pincode/Zip*</span>
                                                        <ucDropDown:ucDropDown runat="server" ID="ddlpincode" MinimumPrefixLength="2" blnChangeEvent="true" strMessage="Pincode With Area" />
                                                        <a id="helpTooltip2" title="Minimum 2 Letter">
                                                            <img src="Images/help-icon.png" style="position: absolute; right: 3px; z-index: 100; cursor: pointer; top: 5px">
                                                        </a>
                                                    </div>
                                                </div>
                                            <div class="col-xs-12 col-sm-12 col-lg-6">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">Geographical*</span>
                                                        <div class="uppercase">
                                                            <ucDropDown:ucDropDown runat="server" ID="ddlGeographical" blEnabled="false" MinimumPrefixLength="2" strMessage="City" />
                                                        </div>
                                                        <a id="helpTooltip1" title="Minimum 2 Letter">
                                                            <img src="Images/help-icon.png" style="position: absolute; right: 3px; z-index: 100; cursor: pointer; top: 5px">
                                                        </a>
                                                    </div>
                                                </div>
                                            </div>
                                            <%--End of Not Populate for NRI : Commented By Rohit --%>
                                            <%--End Of Personal Details of Members : Commented By Rohit--%>
                                            <%--Members Contact Details : Commented By Rohit--%>
                                            <div class="row">
                                            <div class="col-xs-6 col-sm-6 col-lg-2">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">Mobile*
                                                        </span>
                                                        <asp:TextBox ID="txtCountryCode" MaxLength="3" runat="server" Width="70%" CssClass="form-control" AutoCompleteType="Disabled" autocomplete="off"
                                                            placeholder="+91" value="+91" required />
                                                        <ajaxToolkit:FilteredTextBoxExtender ID="fittxtCountryCode" runat="server" TargetControlID="txtCountryCode"
                                                            FilterType="Custom" ValidChars="+1234567890"></ajaxToolkit:FilteredTextBoxExtender>
                                                        <asp:Label ID="label1" class="form-control" runat="server" Text="-" Style="border-color: white" Width="25%" />
                                                    </div>
                                                </div>
                                            <div class="col-xs-6 col-sm-6 col-lg-2">
                                                    <asp:TextBox ID="txtMobile" runat="server" CssClass="form-control uppercase" AutoCompleteType="Disabled" autocomplete="off" pattern="^([0-9]{10,10})$"
                                                        title="Mobile No. 10 numbers minimum" placeholder="Enter Mobile" required minlength="10" MaxLength="10" />
                                                    <ajaxToolkit:FilteredTextBoxExtender ID="fittxtMobile" runat="server" TargetControlID="txtMobile"
                                                        FilterType="Custom" ValidChars="1234567890"></ajaxToolkit:FilteredTextBoxExtender>
                                                </div>

                                            <%--Not for NRI : Commented By Rohit --%>
                                            <div class="col-xs-6 col-sm-6 col-lg-2" align="right" >
                                                    <div class="input-group">
                                                         <asp:Timer ID="Timer3" runat="server" Enabled="false" Interval="15000" OnTick="Timer3_Tick" >
                                                         </asp:Timer>
                                                        <asp:Button ID="btnSendOTPmobile" runat="server" BackColor="LawnGreen" OnClick="btnSendOTPmobile_Click" Text="Send OTP" formnovalidate="true" /><%--formnovalidate="true" --%>
                                                    </div>
                                                </div>
                                            <%--End Not for NRI : Commented By Rohit--%> 

                                            <div class="col-xs-12 col-sm-12 col-lg-6">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">Telephone</span>
                                                        <asp:TextBox ID="txtTelephone" MaxLength="40" runat="server" CssClass="form-control uppercase" AutoCompleteType="Disabled" autocomplete="off"
                                                            placeholder="Enter Telephone" />
                                                        <ajaxToolkit:FilteredTextBoxExtender ID="filterPhoneno" runat="server" TargetControlID="txtTelephone"
                                                            FilterType="Numbers,Custom" ValidChars=".,()/-"></ajaxToolkit:FilteredTextBoxExtender>

                                                    </div>
                                                </div>
                                            </div>
                                            <div class="row">

                                            <%-- mobile OTP --%> <%--Not for NRI : Commented By Rohit --%>
                                            <div class="col-xs-12 col-sm-6 col-lg-3" >
                                                    <div class="input-group" id="Divotp"  runat ="server" visible="true">
                                                        <span class="input-group-addon">Enter OTP*</span>
                                                        <asp:TextBox ID="txtOTPmobile" runat="server" CssClass="form-control" autocomplete="off" ></asp:TextBox>
                                                        
                                                    </div>
                                                </div>
                                            <%-- End Not for NRI : Commented By Rohit --%>

                                            <div class="col-xs-12 col-sm-6 col-lg-1">
                                                    
                                                </div>

                                            <%--Not for NRI : Commented By Rohit --%>
                                            <div class="col-xs-12 col-sm-6 col-lg-2" align="right">
                                                    <div class="input-group" >
                                                        <asp:Button ID="btnVerifyMobile" runat="server" BackColor="LawnGreen" OnClick="btnVerifyMobile_Click" Text="Verify Mobile" formnovalidate="true" />
                                                       
                                                    </div>
                                                </div>
                                            <%--Not for NRI : Commented By Rohit --%>

                                            <div class="col-xs-12 col-sm-12 col-lg-6">
                                                    <div class="input-group" id="DivAadhar" runat ="server" visible="true">
                                                        <span class="input-group-addon">Aadhar No.
                                                        </span>
                                                        <asp:TextBox ID="txtAadhar" MaxLength="12" runat="server" CssClass="form-control uppercase" AutoCompleteType="Disabled" autocomplete="off"
                                                            placeholder="Enter Aadhar No" />
                                                        <ajaxToolkit:FilteredTextBoxExtender ID="fittxtAadhar" runat="server" TargetControlID="txtAadhar"
                                                            FilterType="Custom" ValidChars="1234567890"></ajaxToolkit:FilteredTextBoxExtender>
                                                       <%-- <span class="input-group-addon">Aadhar No.
                                                        </span>
                                                        <asp:TextBox ID="TextBox2" MaxLength="12" runat="server" CssClass="form-control uppercase" AutoCompleteType="Disabled" autocomplete="off"
                                                            placeholder="Enter Aadhar No" />
                                                        <ajaxToolkit:FilteredTextBoxExtender ID="FilteredTextBoxExtender1" runat="server" TargetControlID="txtAadhar"
                                                            FilterType="Custom" ValidChars="1234567890"></ajaxToolkit:FilteredTextBoxExtender>--%>
                                                    </div>
                                                  <div class="input-group" id="DivSocial" runat ="server" visible="true">
                                                        <span class="input-group-addon">Social Security No.
                                                        </span>
                                                        <asp:TextBox ID="TextBox1" MaxLength="12" runat="server" CssClass="form-control uppercase" AutoCompleteType="Disabled" autocomplete="off"
                                                            placeholder="Enter Social Security No" />
                                                        <ajaxToolkit:FilteredTextBoxExtender ID="FilteredTextBoxExtender1" runat="server" TargetControlID="txtAadhar"
                                                            FilterType="Custom" ValidChars="1234567890"></ajaxToolkit:FilteredTextBoxExtender>
                                                       <%-- <span class="input-group-addon">Aadhar No.
                                                        </span>
                                                        <asp:TextBox ID="TextBox2" MaxLength="12" runat="server" CssClass="form-control uppercase" AutoCompleteType="Disabled" autocomplete="off"
                                                            placeholder="Enter Aadhar No" />
                                                        <ajaxToolkit:FilteredTextBoxExtender ID="FilteredTextBoxExtender1" runat="server" TargetControlID="txtAadhar"
                                                            FilterType="Custom" ValidChars="1234567890"></ajaxToolkit:FilteredTextBoxExtender>--%>
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="row">
                                                <div class="col-xs-12 col-sm-6 col-lg-5">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">Email / Login Name*
                                                        </span>
                                                        <asp:TextBox ID="txtEmail" MaxLength="100" runat="server" CssClass="form-control" AutoCompleteType="Disabled" autocomplete="off"
                                                            placeholder="Enter Email / User Name" required pattern="[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"></asp:TextBox><a title="Eg : xxxx@gmail.com">
                                                                <img src="Images/help-icon.png" style="position: absolute; right: 3px; z-index: 100; cursor: pointer; top: 5px">
                                                            </a>
                                                        </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-6 col-lg-1" align="right">
                                                    <div class="input-group">
                                                         <asp:Timer ID="Timer2" runat="server" Enabled="false" Interval="15000" OnTick="Timer2_Tick">
                                            </asp:Timer>
                                                        <asp:Button ID="btnSendOTP" runat="server" BackColor="LawnGreen" OnClick="btnSendOTP_Click" Text="Send OTP" formnovalidate="true"/><%--formnovalidate="true" --%>
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-12 col-lg-6">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">Set Password*</span>
                                                        <asp:TextBox ID="txtPassword" ClientIDMode="Static" MaxLength="15" pattern="^([a-zA-Z0-9@*#]{6,15})$" runat="server" CssClass="form-control" AutoCompleteType="Disabled" autocomplete="off" TextMode="Password"
                                                            placeholder="Enter Password" required data-toggle="password" />
                                                        <span toggle="#txtPassword" class="fa fa-fw fa-eye field-icon toggle-password"></span>
                                                        <a title="Minimum 6 Characters Allowed &#10 Eg : 1#Zv96g@*Yfasd4">
                                                            <img src="Images/help-icon.png" style="position: absolute; right: 3px; z-index: 100; cursor: pointer; top: 5px">
                                                        </a>
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-12 col-lg-4" style="display: none">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">Alternate Email ID
                                                        </span>
                                                        <asp:TextBox ID="txtAltEmail" MaxLength="100" runat="server" CssClass="form-control" AutoCompleteType="Disabled" autocomplete="off"
                                                            placeholder="ENTER EMAIL" pattern="[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"></asp:TextBox><a title="Eg : xxxx@gmail.com">
                                                                <img src="Images/help-icon.png" style="position: absolute; right: 3px; z-index: 100; cursor: pointer; top: 5px">
                                                            </a>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="row">
                                                <%--  --%>
                                                 <div class="col-xs-12 col-sm-6 col-lg-3">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">Enter OTP*</span>
                                                        <asp:TextBox ID="txtOTP" runat="server" CssClass="form-control" autocomplete="off" ></asp:TextBox>
                                                        
                                                       <%-- <asp:HiddenField ID="HiddenField2" runat="server" />--%>
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-6 col-lg-2">
                                                    
                                                </div>
                                                <div class="col-xs-12 col-sm-6 col-lg-1" align="right">
                                                    <div class="input-group">
                                                        <asp:Button ID="btnVerifyEmail" runat="server" BackColor="LawnGreen" OnClick="btnVerifyEmail_Click" Text="Verify Email" formnovalidate="true" />
                                                       
                                                    </div>
                                                </div>
                                                <%--<div class="col-xs-12 col-sm-6 col-lg-2">
                                                    <div class="input-group">
                                                        <asp:Button ID="btnVerifyEmail" runat="server" OnClick="btnVerifyEmail_Click" Text="Verify Email" formnovalidate="true" />
                                                    </div>
                                                </div>--%>
                                                
                                               <%--  --%>

                                                <div class="col-xs-12 col-sm-12 col-lg-6">
                                                    <div class="input-group">
                                                        <%--<span class="input-group-addon">Aadhar No.
                                                        </span>
                                                        <asp:TextBox ID="txtAadhar" MaxLength="12" runat="server" CssClass="form-control uppercase" AutoCompleteType="Disabled" autocomplete="off"
                                                            placeholder="Enter Aadhar No" />
                                                        <ajaxToolkit:FilteredTextBoxExtender ID="fittxtAadhar" runat="server" TargetControlID="txtAadhar"
                                                            FilterType="Custom" ValidChars="1234567890"></ajaxToolkit:FilteredTextBoxExtender>--%>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="row">
                                                 <div class="col-xs-12 col-sm-12 col-lg-6">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">Pan / TRC No*</span>
                                                        <%--[A-Za-z]{5}\d{4}[A-Za-z]{1}--%>
                                                        <%--<asp:TextBox ID="txtPan" MaxLength="10" runat="server" pattern="^[a-zA-Z]{5}[0-9]{4}[a-zA-Z]{1}$" CssClass="form-control uppercase" AutoCompleteType="Disabled" autocomplete="off" title="Eg : CEQPK4956K"
                                                            placeholder="Enter Pan No" required /><a title="Eg : CEQPK4956K">
                                                                <img src="Images/help-icon.png" style="position: absolute; right: 3px; z-index: 100; cursor: pointer; top: 5px">
                                                                <ajaxToolkit:FilteredTextBoxExtender ID="filterpancard" runat="server" TargetControlID="txtPan"
                                                                    FilterType="Numbers,UppercaseLetters,LowercaseLetters"></ajaxToolkit:FilteredTextBoxExtender>
                                                            </a>--%>

                                                        <asp:TextBox ID="txtPan" name="txtDetails2_Pan" MaxLength="10" runat="server" AutoPostBack ="true" required class="form-control"  
                                                                                placeholder="Enter Pan No." title="Pan No Format- CEQPK4956K" onBlur="ValidatePAN(event)" ></asp:TextBox>


                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-6 col-lg-2">
                                                    <img src="JpegImage.aspx?random=<%=hdnEcapcha.Value %>" border="1" />
                                                </div>
                                                <div class="col-xs-12 col-sm-6 col-lg-3">
                                                    <div class="input-group">
                                                        <asp:TextBox ID="txtcode" runat="server" CssClass="form-control" autocomplete="off" required></asp:TextBox>
                                                        <span class="input-group-addon">Enter Text*</span>
                                                        <asp:HiddenField ID="hdnEcapcha" runat="server" />                                                       
                                                    </div>
                                                </div>

                                            </div>
                                             <%--Members Contact Details : Commented By Rohit--%>
                                             <%--End Details of Members--%> 
                                        </asp:WizardStep>

                                        <asp:WizardStep StepType="Step" ID="Step1" runat="server">
                                            <asp:Timer ID="Timer1" runat="server" Enabled="false" Interval="5000" OnTick="Timer1_Tick">
                                            </asp:Timer>
                                            <div style="width: 100%; height: 50vh;">

                                                <asp:Image runat="server" class="img-responsive center-block" ID="imgloading" ImageUrl="~/Images/Saving.gif" />
                                            </div>
                                        </asp:WizardStep>

                                        <%-- Step2 (Finsh Transaction) is for given transaction status means Success, fail, email not send --%>

                                        <asp:WizardStep ID="Step2" StepType="Complete" runat="server">
                                            <div class="row">
                                                <div id="divsuccess" class="col-xs-12 col-sm-12 col-lg-12" style="text-align: center; font-weight: bold; color: #1f72ba; font-size: x-large" runat="server" visible="false">
                                                    <h4>Thank you for registering on our website.</h4>
                                                    <h4>We have sent you an email for verification.  </h4>
                                                    <h4>After verification please  login & provide other details to register within 72 hrs from now.
                                                    </h4>
                                                </div>
                                                <div id="divfail" class="col-xs-12 col-sm-12 col-lg-12" runat="server" visible="false">
                                                    <h4 style="text-align: center; font-weight: bold;">Failed to Save the records. 
                                                        <br />
                                                        <br />
                                                        <span id="SPRetMsg" runat="server"></span>
                                                        <br />
                                                        Please try  again
                                                        <br />
                                                    </h4>
                                                    <h4 style="text-align: center; font-weight: bold;">Or
                                                        <br />
                                                        Email at <a href="mailto:membership@iprs.org">membership@iprs.org</a></h4>
                                                </div>
                                                <div id="divMessage" class="col-xs-12 col-sm-12 col-lg-12" runat="server" visible="false">
                                                    <br />
                                                    <h4 style="text-align: center; font-weight: bold;">Failed to send verification mail. Please  contact <a href="mailto:membership@iprs.org">membership@iprs.org</a>. 
                                                        <br />
                                                    </h4>
                                                </div>
                                            </div>
                                            <div class="row">
                                                <br />
                                                <br />
                                                <div class="col-xs-8 col-sm-8 col-lg-8"></div>
                                                <div class="col-xs-4 col-sm-4 col-lg-4" align="right">
                                                    <asp:Button ID="btnfinish" runat="server" Text="Home" CssClass="btn btn-success btn-sm" ToolTip="Home Page" ClientIDMode="Static" OnClick="btnfinish_Click" />
                                                </div>
                                                <br />
                                                <br />
                                            </div>
                                        </asp:WizardStep>

                                        <%--end Step2 (Finsh Transaction)--%>

                                    </WizardSteps>

                                    <StartNavigationTemplate>                                       
                                        <asp:Button ID="btnNextButton"  runat="server" ToolTip="CONTINUE TO REGISTER" Text="Next" CommandName="MoveNext" CssClass="btn btn-primary btn-md"  />                                                                                
                                         <Triggers>
                                        <asp:PostBackTrigger ControlID="btnNextButton" />
                                    </Triggers>
                                    </StartNavigationTemplate>
                                    
                                    <StepNavigationTemplate>
                                        <asp:Button ID="btnStepNext"  runat="server" ToolTip="CONTINUE TO NEXT SCREEN WITHOUT SAVING RECORDS" Text="Continue" CssClass="btn btn-primary btn-md" Visible="false" />
                                    </StepNavigationTemplate>
                                    
                                    
                                </asp:Wizard>
                            </div>
                        </div>
                    </div>
                
                </div> 
            </ContentTemplate>
            
        </asp:UpdatePanel>

    </form>
     
    <script>  $(".toggle-password").click(function () {

      $(this).toggleClass("fa-eye fa-eye-slash");
      var input = $($(this).attr("toggle"));
      if (input.attr("type") == "password") {
          input.attr("type", "text");
      } else {
          input.attr("type", "password");
      }
  });</script>

</body>
</html>
