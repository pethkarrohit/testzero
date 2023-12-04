﻿<%@ Page Title="" Language="C#" MasterPageFile="~/ApplicationMember.Master" AutoEventWireup="true" CodeBehind="UpdateProfile.aspx.cs" Inherits="IPRS_Member.UpdateProfile" EnableEventValidation="false" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Src="~/User_Controls/ucDropDown.ascx" TagPrefix="ucDropDown" TagName="ucDropDown" %>
<%@ Register Src="~/User_Controls/ucTooltip.ascx" TagPrefix="ucTooltip" TagName="ucTooltip" %>
<%@ Register Src="~/User_Controls/ucNomineeDetails.ascx" TagPrefix="ucNomineeDetails" TagName="ucNomineeDetails" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <style type="text/css">
        .address {
            border-bottom: 2px solid #e8e8e8;
            /*padding: 1px 5px 6px;*/
            margin-bottom: 10px;
        }
    </style>
    <script src="Javascript/FCalender.js"></script>
    <link href="Style/zcCalendar.css" rel="stylesheet" />
    <script type="text/javascript">
        window.onload = function () {


            var h1 = document.getElementById("<%=div_position.ClientID%>");
            document.getElementById("<%=dvScroll1.ClientID%>").scrollTop = h1.value;

        }

        function dateSelectionChanged(sender, args){
            
            selectedDate = sender.get_selectedDate();
            var date = moment(new Date(selectedDate));

            sender._textbox._element.value=(moment(date).format('DD-MM-YYYY'));
            SetDateField(sender._textbox._element);
        }


        function SetDivPosition(obj) {


            var intY1 = obj.scrollTop;
            var h1 = document.getElementById("<%=div_position.ClientID%>");
            h1.value = intY1;

        }

        function blockSpecialChar(e){
            var k;
            document.all ? k = e.keyCode : k = e.which;
            return ((k > 64 && k < 91) || (k > 96 && k < 123) || k == 8 || k == 32 || (k >= 48 && k <= 57));
        }

        function validateFileExtension(component,msg_id,msg,extns)
        {
            debugger;
            var flag=0;
            with(component)
            {
                var ext=value.substring(value.lastIndexOf('.')+1);
                for(i=0;i<extns.length;i++)
                {
                    if(ext.toUpperCase()==extns[i].toUpperCase())
                    {
                        flag=0;
                        break;
                    }
                    else
                    {
                        flag=1;
                    }
                }
                if(flag!=0)
                {
                    msg_id.innerHTML=msg;
                    component.value="";
                    component.style.backgroundColor="#eab1b1";
                    component.style.border="thin solid #000000";
                    component.focus();
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        function WorkFileValidate()
        {
            var obj = document.getElementById("<%=fpworkfileupload.ClientID%>");
            if(obj.value!="")
            {
                if(validateFileExtension(<%# FPWork.ClientID %>, document.getElementById("valid_msg"), "jpg/pdf/jpeg/gif/png/doc/docx/xls/xlsx/PDF files are only allowed!",
                     new Array("jpg", "pdf", "jpeg", "gif", "png", "doc", "docx","pdf","xls","xlsx")) == false)
                {
                    return false;
                }
            }
        }
        function PhotoValidate()
        {

            if(validateFileExtension(<%# FPuploadPhoto.ClientID %>, document.getElementById("Photo_valid_msg"), "jpg/jpeg/gif/png files are only allowed!",
                new Array("jpg",  "jpeg", "gif", "png")) == false)
            {
                return false;
            }
        }

        function PhotoValidated()
        {

            if(validateFileExtension(<%# FPuploadPhotod.ClientID %>, document.getElementById("Photo_valid_msg"), "jpg/jpeg/gif/png files are only allowed!",
               new Array("jpg",  "jpeg", "gif", "png")) == false)
           {
               return false;
           }
       }



       function WorkFileValidatepnl()
       {
           if(validateFileExtension(<%# fpworkfileupload.ClientID %>, document.getElementById("valid_msg"), "jpg/pdf/jpeg/gif/png/doc/docx/xls/xlsx/PDF files are only allowed!",
                        new Array("jpg", "pdf", "jpeg", "gif", "png", "doc", "docx","pdf","xls","xlsx")) == false)
            {
                return false;
            }
        }
        
        function isSpecialKey(evt)
        {
           
            var charCode = (evt.which) ? evt.which : event.keyCode

            if ((charCode>=65 && charCode <=91) || (charCode>=97 && charCode <=123) || (charCode >= 48 || charCode <= 57))
                return true;

            return false;
        }
        
        function DocFileValidate(obj)
        {
            with (obj){
                debugger;
                var row = obj.parentNode.parentNode;
                var comp = GetChildControl(row, "FileUpload1");
                var spfileErrormsg=GetChildControl(row,"spfileErrormsg");
                if(validateFileExtension(comp, spfileErrormsg, "jpg/pdf/jpeg/gif/png/doc/docx/xls/xlsx/PDF files are only allowed!",
                 new Array("jpg", "pdf", "jpeg", "gif", "png", "doc", "docx","pdf","xls","xlsx")) == false)
                {
                    return false;
                }
            }
        }

        function GetChildControl(element, id) {
            var child_elements = element.getElementsByTagName("*");
            for (var i = 0; i < child_elements.length; i++) {
                if (child_elements[i].id.indexOf(id) != -1) {
                    return child_elements[i];
                }
            }
        };

        function RedirectToWizardIndex(element){           
            var obj = document.getElementById("<%=hdnWizardIndex.ClientID%>");
            obj.value = element.innerHTML;    
           
            var objButton = document.getElementById("<%=lnkBtnRedirectToWizrd.ClientID%>");
            objButton.click();  
        }

         
        
        function onlyDotsAndNumbers(event)
        {

            var charCode = (event.which) ? event.which : event.keyCode
            if (charCode == 46) {
                return true;
            }
            if (charCode > 31 && (charCode < 48 || charCode > 57))
                return false;

            return true;
        }
        

      
        function myFun()
        {

           
            
            
            
       
            var date1 = new Date(deger);
            
            var date2 = new Date();
                       
            

            if (date1 >= date2)
            {             

                //ctl00$ContentPlaceHolder1$wzMain$ucNomineeDetails$txtDOB.value="";                
                
                alert("Date of birth cannot be greater than today date");                                                                

            }
         
            
            var tDays= Math.floor((Date.UTC(date2.getFullYear(), date2.getMonth(), date2.getDate()) - Date.UTC(date1.getFullYear(), date1.getMonth(), date1.getDate()) ) /(1000 * 60 * 60 * 24));
            
            var tYear= tDays/365;
            
         
            if (tYear < 18)
            {                        
                //RB1 = document.getElementById("ContentPlaceHolder1_wzMain_ucNomineeDetails_RBLMinor_0") 
                RB2 = document.getElementById("ContentPlaceHolder1_wzMain_ucNomineeDetails_RBLMinor_1") 
                //RB1.checked=false;         
                RB2.checked=true;         
            }            
            else
            {
                RB1 = document.getElementById("ContentPlaceHolder1_wzMain_ucNomineeDetails_RBLMinor_0") 
                //RB2 = document.getElementById("ContentPlaceHolder1_wzMain_ucNomineeDetails_RBLMinor_1") 
                RB1.checked=true;         
                //RB2.checked=false;         

            }


        }
       
                
         
        
        function ValidatePAN(event)
        {
            var PANNo = (document.getElementById("ContentPlaceHolder1_wzMain_txtDetails2_Pan").value).toUpperCase();

            var RegType = document.getElementById('<%= hdnRegistrationType.ClientID %>').value;

            var Enttype = $("input[name='<%=rbtEntityType.UniqueID %>']:checked").val();//add by Rohit on 17/05/2023 validation for Sole Pro. 

            if (PANNo.value != "") {
                if (RegType == "I" || RegType == "NI") {
                    if (PANNo.substr(3, 1) != "P") {
                        document.forms[0].ContentPlaceHolder1_wzMain_txtDetails2_Pan.value = "";
                        alert("Please enter valid PAN card for Individual");

                        //ContentPlaceHolder1_wzMain_txtDetails2_Pan.focus();
                        //lblPANCard.style.visibility = "visible";
                        //return false;

                    }
                    else {
                        var ObjVal = PANNo;
                        var panPattern = /^([a-zA-Z]{5})(\d{4})([a-zA-Z]{1})$/;
                        var matchArray = ObjVal.match(panPattern);

                        if (matchArray == null) {
                            document.forms[0].ContentPlaceHolder1_wzMain_txtDetails2_Pan.value = "";
                            alert('Invalid PAN Card No.');
                            //ContentPlaceHolder1_wzMain_txtDetails2_Pan.focus(); 
                            //lblPANCard.style.visibility = "visible";
                            //return false;
                        }
                        else {
                            return true;
                        }
                    }
                }

                else if (RegType == "C" || RegType == "NC")
                {
                    debugger;
                    if (Enttype == "SP") //add by Rohit on 17/05 / 2023 validation for Sole Pro. 
                    {
                        var panPattern = /^([a-zA-Z]{5})(\d{4})([a-zA-Z]{1})$/;
                        if (panPattern.test(PANNo)) {
                          // alert("Correct pan");
                            return true;
                        }
                        else {
                           // alert("incorrect");
                            alert("Please enter valid PAN card for Company SP");
                            return false;
                        }
                    }
                    if (Enttype == "PR")//add by Rohit on 22/05 / 2023 validation for PartnerShip Firm. 
                    {
                        PANSUB = PANNo.substr(3, 1);
                        if (PANSUB == "F" || PANSUB == "C")
                        {

                            var ObjVal = PANNo;
                            var panPattern = /^([a-zA-Z]{5})(\d{4})([a-zA-Z]{1})$/;
                            var matchArray = ObjVal.match(panPattern);
                            if (matchArray == null) {
                                document.forms[0].ContentPlaceHolder1_wzMain_txtDetails2_Pan.value = "";
                                alert('Invalid PAN Card No.');

                            }
                            else {
                                return true;
                            }

                        }
                                             
                       else {
                            document.forms[0].ContentPlaceHolder1_wzMain_txtDetails2_Pan.value = "";
                            alert("Please enter valid PAN card for Partner Ship Firm");

                            }
                    }
                    if (Enttype == "CP") //add by Rohit on 22/05 / 2023 validation for Company. 
                    {
                        if (PANNo.substr(3, 1) != "C")
                        {
                            document.forms[0].ContentPlaceHolder1_wzMain_txtDetails2_Pan.value = "";
                            alert("Please enter valid PAN card for Company");
                            //ContentPlaceHolder1_wzMain_txtDetails2_Pan.focus();                         
                            //lblPANCard.style.visibility = "visible";
                            //return false;

                        }
                        else {
                            var ObjVal = PANNo;
                            var panPattern = /^([a-zA-Z]{5})(\d{4})([a-zA-Z]{1})$/;
                            var matchArray = ObjVal.match(panPattern);
                            if (matchArray == null) {
                                document.forms[0].ContentPlaceHolder1_wzMain_txtDetails2_Pan.value = "";
                                alert('Invalid PAN Card No.');
                                //ContentPlaceHolder1_wzMain_txtDetails2_Pan.focus();                           
                                //return false;

                            }
                            else {
                                //lblPANCard.style.visibility = "hidden";
                                return true;
                            }

                        }
                    }
                }
            }

        }

        
    </script>

    <style type="text/css">
        .grid-header th {
            font-weight: bold;
            font-family: Verdana;
            font-size: 11px;
            background-color: #1a76b3;
            color: White;
            text-align: center;
            position: relative;
            padding: 5px;
            border: 1px solid #377cc7;
        }

        .grid-header td {
            padding: 5px;
        }

        @media (min-width:500px) {
            .adjGrid {
                margin: 0;
                position: relative;
                bottom: 0;
                height: 10%;
                width: 100%;
            }
        }

        input[type=text] {
            text-transform: uppercase;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" />
    <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="upnlwzMain">
        <ProgressTemplate>
            <div class="loadingmodal">
                <div class="loadingcenter">
                </div>
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
    <asp:UpdatePanel ID="upnlwzMain" UpdateMode="Conditional" runat="server">
        <ContentTemplate>
            <div class="row">
                <div class="col-md-12 col-sm-12 col-xs-12">
                    <div class="x_panel">
                        <div class="x_content">
                            <asp:HiddenField ID="hdnRecordId" Value="0" runat="server" />
                            <asp:HiddenField ID="hdnAccountStatus" Value="0" runat="server" />
                            <asp:HiddenField ID="hdnApplicationStatus" Value="0" runat="server" />
                            <asp:HiddenField ID="hdnRecordIds" Value="" runat="server" />
                            <asp:HiddenField ID="div_position" Value="0" runat="server" />
                            <asp:HiddenField ID="hdnRowIndex" Value="0" runat="server" />
                            <asp:HiddenField ID="hdnRegistrationType" Value="" runat="server" />
                            <asp:HiddenField ID="hdnContactId" Value="0" runat="server" />
                            <asp:HiddenField ID="hdnAccountCode" Value="" runat="server" />
                            <asp:HiddenField ID="hdnRefAccountCode" Value="" runat="server" />
                            <asp:HiddenField ID="hdnRefAccountId" Value="" runat="server" />
                            <asp:HiddenField ID="hdnAddressId" Value="0" runat="server" />
                            <asp:HiddenField ID="hdnPaymentRecieptId" Value="" runat="server" />
                            <asp:HiddenField ID="hdnRoleTypeIds" Value="0" runat="server" />
                            <asp:HiddenField ID="hdnRoleTypes" Value="0" runat="server" />
                            <asp:HiddenField ID="hdnRegistrationDate" Value="" runat="server" />
                            <asp:HiddenField ID="hdnAccountName" Value="" runat="server" />
                            <asp:HiddenField ID="hdnWizardIndex" Value="0" runat="server" />
                            <asp:HiddenField ID="hdnBookId" Value="0" runat="server" />
                            <asp:HiddenField ID="hdnpincode" Value="0" runat="server" />
                            <asp:HiddenField ID="hdnsocietyName" Value="0" runat="server" />
                            <asp:HiddenField ID="hdnmothertounge" Value="0" runat="server" />
                            <asp:HiddenField ID="hdnpincodePM" Value="0" runat="server" />
                            <asp:HiddenField ID="hdnIFSC_Val" Value="" runat="server" />
                            <asp:LinkButton runat="server" ID="lnkBtnRedirectToWizrd" OnClick="lnkBtnRedirectToWizrd_Click"></asp:LinkButton>
                            <div class="col-xs-12 col-sm-12 col-lg-12" align="right">


                                <a href="https://twitter.com/IPRSmusic" target="_blank" title="Twitter">
                                    <img width="20" height="20" src="Images/twitter.png" />
                                </a>
                                <a href="https://www.facebook.com/iprsorg" target="_blank" title="Facebook">
                                    <img width="20" height="20" src="Images/facebook.png" />
                                </a>
                                <a href="https://www.instagram.com/iprsmusic/" target="_blank" title="Instagram">
                                    <img width="20" height="20" src="Images/Instagram.png" />
                                </a>
                                <a href="https://in.linkedin.com/company/iprsmusic" target="_blank" title="LinkedIn">
                                    <img width="20" height="20" src="Images/Linkin.png" />
                                </a>
                                <a href="https://www.youtube.com/c/IPRSmusic" target="_blank" title="Youtube">
                                    <img width="30" height="30" src="Images/Youtube.png" />
                                </a>


                            </div>
                            <div class="x_title">
                                <h4>
                                    <asp:Label ID="lblFormTitle" runat="server">Update Profile</asp:Label></h4>
                                <div class="clearfix"></div>
                            </div>





                            <div id="wizard" class="form_wizard wizard_horizontal">
                                <ul class="wizard_steps">
                                    <li>
                                        <a runat="server" id="aStep1" class="selected">
                                            <span class="step_no" style="cursor: pointer" runat="server" id="sp1" onclick="javascript:void(0);">1</span>
                                            <small>Personal Information</small>
                                            </span>
                                        </a>
                                    </li>


                                    <li>
                                        <a runat="server" id="aStep2" class="disabled">
                                            <span class="step_no small" style="cursor: pointer" runat="server" id="sp3" onclick="javascript:void(0);">2</span>
                                            <small>Bank Details</small>
                                            </span>
                                        </a>
                                    </li>
                                    <li>
                                        <a runat="server" id="aStep3" class="disabled">
                                            <span class="step_no small" style="cursor: pointer" runat="server" id="sp4" onclick="javascript:void(0);">3</span>
                                            <small>Work Notification</small>
                                            </span>
                                        </a>
                                    </li>

                                    <li>
                                        <a runat="server" id="aStep4" class="disabled">
                                            <span class="step_no" style="cursor: pointer" runat="server" id="sp2" onclick="javascript:void(0);">4</span>
                                            <small>Nominee Detail Information</small>
                                            </span>
                                        </a>
                                    </li>

                                    <li>
                                        <a runat="server" id="aStep5" class="disabled">
                                            <span class="step_no small" style="cursor: pointer" runat="server" id="sp5" onclick="javascript:void(0);">5</span>
                                            <small>Document Upload</small>
                                            </span>
                                        </a>
                                    </li>

                                    <li>
                                        <a runat="server" id="aStep6" class="disabled">
                                            <span class="step_no small" style="cursor: pointer" runat="server" id="sp6" onclick="javascript:void(0);">6</span>
                                            <small>Submit Application</small>
                                            </span>
                                        </a>
                                    </li>


                                </ul>
                            </div>

                            <br />

                            <asp:UpdatePanel ID="UpdatePanel3" UpdateMode="Conditional" runat="server">
                                <ContentTemplate>

                                    <div class="row" id="divLegDetail" runat="server" visible="false">

                                        <div class="fieldset panel">
                                            <div class="fieldset-body panel-body">
                                                <h5 class="legend-Panel"><strong class="text-uppercase"><span id="Span5" runat="server">Deceased Member Details</span> </strong></h5>
                                                <div class="row" id="div6" runat="server">
                                                    <div class="col-xs-12 col-sm-12 col-lg-6">
                                                        <div class="input-group">
                                                            <span class="input-group-addon" runat="server">Deceased Member Name</span>
                                                            <asp:TextBox ID="txtdmembername" MaxLength="100" runat="server" CssClass="form-control" AutoCompleteType="Disabled" autocomplete="off"
                                                                placeholder="Enter Deceased Member Name" required />
                                                        </div>
                                                    </div>
                                                    <div class="col-xs-12 col-sm-12 col-lg-6" id="div7" runat="server">
                                                        <div class="input-group">

                                                            <span class="input-group-addon" id="Span6" runat="server">Relationship</span>
                                                            <asp:TextBox ID="txtdreationship" MaxLength="100" runat="server" class="form-control" Style="width: 98%" AutoCompleteType="Disabled" autocomplete="off">
                                                            </asp:TextBox>
                                                        </div>
                                                    </div>

                                                </div>


                                                <div class="row" id="div8" runat="server">
                                                    <div class="col-xs-12 col-sm-12 col-lg-6">
                                                        <div class="input-group">
                                                            <span class="input-group-addon">Date of Birth
                                                            </span>
                                                            <asp:TextBox ID="txtddatebirth" runat="server" onblur="myFun();" required class="form-control" Style="width: 98%" AutoCompleteType="Disabled" autocomplete="off" placeholder="dd-MMM-yyyy" ToolTip="Format : [ddmmyyyy], [d] or [dd], [ddm] or [ddmm], [ ].&#xa;Keys : Day-[Up/Down], Month-[alt+Up/Down], Year-[shift+Up/Down], Today-[Home].">
                                                            </asp:TextBox>
                                                            <%--<asp:CalendarExtender ID="calDOB" OnClientDateSelectionChanged="dateSelectionChanged" CssClass="zcCalendar" runat="server" TargetControlID="txtDOB" Format="dd-MMM-yyyy"></asp:CalendarExtender> --%>
                                                        </div>
                                                    </div>
                                                    <div class="col-xs-12 col-sm-12 col-lg-6">
                                                        <div class="input-group">
                                                            <span class="input-group-addon">Date of death
                                                            </span>
                                                            <asp:TextBox ID="txtddeathofdate" runat="server" required class="form-control" Style="width: 98%" AutoCompleteType="Disabled" autocomplete="off" placeholder="dd-MMM-yyyy" ToolTip="Format : [ddmmyyyy], [d] or [dd], [ddm] or [ddmm], [ ].&#xa;Keys : Day-[Up/Down], Month-[alt+Up/Down], Year-[shift+Up/Down], Today-[Home].">
                                                            </asp:TextBox>
                                                            <%--<asp:CalendarExtender ID="calDOB1" OnClientDateSelectionChanged="dateSelectionChanged" CssClass="zcCalendar" runat="server" TargetControlID="txtdateofdeath" Format="dd-MMM-yyyy"></asp:CalendarExtender>                                                                                --%>
                                                        </div>
                                                    </div>


                                                </div>

                                                <div class="row" id="div9" runat="server">
                                                    <div class="col-xs-12 col-sm-12 col-lg-6">
                                                        <div class="input-group">
                                                            <span class="input-group-addon">Death Certficate Number
                                                            </span>
                                                            <asp:TextBox ID="txtddeathofcert" placeholder="Enter Death Certificate Number." runat="server" MaxLength="100" class="form-control" Style="width: 98%" AutoCompleteType="Disabled" autocomplete="off">
                                                            </asp:TextBox>

                                                        </div>
                                                    </div>

                                                    <div class="col-xs-12 col-sm-6 col-lg-6">
                                                        <div class="input-group">
                                                            <span class="input-group-addon">Address
                                                            </span>
                                                            <textarea id="txtdadd" maxlength="255" runat="server" class="form-control" autocompletetype="Disabled" autocomplete="off"
                                                                placeholder="Enter Permanent Address" textmode="MultiLine" rows="3" cols="20" />
                                                        </div>
                                                    </div>

                                                    <%--Added by Hariom 20-02-2023--%>
                                                    <div class="col-xs-12 col-sm-6 col-lg-6">
                                                        <div class="input-group">
                                                            <span class="input-group-addon">IPI Number
                                                            </span>
                                                            <asp:TextBox ID="txtIPI" placeholder="IPI No" runat="server" MaxLength="100" class="form-control" Style="width: 98%" AutoCompleteType="Disabled" autocomplete="off" ReadOnly="true">
                                                            </asp:TextBox>
                                                        </div>
                                                    </div>
                                                        <%--end--%>

                                                </div>


                                                <div class="row">
                                                    <div class="col-xs-12 col-sm-6 col-lg-6" id="div11" runat="server">
                                                        <div class="input-group">
                                                            <span class="input-group-addon">Pincode</span>
                                                            <asp:TextBox ID="txtdpin" runat="server" class="form-control" Style="width: 98%" AutoCompleteType="Disabled" autocomplete="off">
                                                            </asp:TextBox>
                                                        </div>
                                                    </div>

                                                    <div class="col-xs-12 col-sm-6 col-lg-6" id="div12" runat="server">
                                                        <div class="input-group">
                                                            <span class="input-group-addon">Geographical</span>
                                                            <asp:TextBox ID="txtdgeo" runat="server" class="form-control" Style="width: 98%" AutoCompleteType="Disabled" autocomplete="off">
                                                            </asp:TextBox>
                                                        </div>
                                                    </div>

                                                </div>
                                                <div class="row">
                                                    <div runat="server" id="divTrader" visible="false">

                                                        <div class="col-xs-12 col-sm-6 col-lg-6">
                                                            <div class="input-group ">
                                                                <span class="input-group-addon">Alias/ Trader Name</span>
                                                                <asp:TextBox ID="txtDAccountAlias" MaxLength="100" runat="server" CssClass="form-control uppercase" AutoCompleteType="Disabled" autocomplete="off"
                                                                    placeholder="Enter Alias/ Trader Name" title="Alias Name / Trade Name / Credit Name" />
                                                                <a id="helpDTooltip" title="NOTE:Pseudo Name/Pen Name, please use (,) comma as separator of multiple names">
                                                                    <img src="Images/help-icon.png" style="position: absolute; right: 3px; z-index: 100; cursor: pointer; top: 5px">
                                                                </a>
                                                            &nbsp;&nbsp;</div>
                                                        </div>

                                                    </div>


                                                    <div id="divroletyped" runat="server">
                                                        <span class="input-group-addon" runat="server">Role Type</span>
                                                        <div class="col-xs-12 col-sm-6 col-lg-6">
                                                            <div class="checkbox">
                                                                <asp:CheckBoxList ID="cbxRollTyped" AutoPostBack="true" RepeatDirection="Vertical" RepeatLayout="Flow" Style="list-style: none" runat="server" CssClass="form-control-static checkbox-inline">
                                                                </asp:CheckBoxList>
                                                            </div>
                                                        </div>

                                                    </div>

                                                </div>


                                                <div class="row">

                                                    <div class="col-xs-12 col-sm-12 col-lg-6">
                                                        <div class="input-group">
                                                            <span class="input-group-addon" runat="server" id="Span7">Deceased Member Photo
                                                            </span>
                                                            <asp:FileUpload ID="FPuploadPhotod" Width="100%" CssClass="form-control" runat="server" placeholder="ABC" AllowMultiple="false" accept="image/gif, image/jpeg, image/png" />
                                                        </div>
                                                    </div>

                                                    <asp:Button ID="btnPhotoUploadd" runat="server" OnClick="btnPhotoUploadd_Click" OnClientClick="return PhotoValidated();" Text="Upload Image" CssClass="btn btn-primary" />
                                                    <asp:HyperLink ID="btnphotoViewd" runat="server" data-toggle="modal" data-target="#myModal">
                                                        <asp:Image runat="server" ID="ImgUserd" Style="border: 1px solid; cursor: pointer" Width="70px" Height="70px" />
                                                    </asp:HyperLink>
                                                    <asp:HiddenField ID="hdnphotoImageNamed" runat="server" />
                                                    <div class="modal" id="myModal">
                                                        <div class="modal-dialog">
                                                            <div class="modal-content">

                                                                <!-- Modal body -->
                                                                <div class="modal-body" style="text-align: center">
                                                                    <asp:Image runat="server" ID="ImgUserLarged" Style="border: 0px" />
                                                                </div>

                                                                <!-- Modal footer -->
                                                                <div class="modal-footer">
                                                                    <button type="button" class="btn btn-danger" data-dismiss="modal">Close</button>
                                                                </div>

                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>





                                                <br />


                                            </div>



                                        </div>
                                    </div>

                                </ContentTemplate>
                                <Triggers>
                                    <asp:PostBackTrigger ControlID="btnPhotoUploadd" />
                                </Triggers>
                            </asp:UpdatePanel>



                            <br />








                            <div class="col-xs-12 col-sm-12 col-lg-12">
                                <asp:Wizard ID="wzMain" CssClass="col-sm-12" DisplaySideBar="false" OnActiveStepChanged="wzMain_ActiveStepChanged"
                                    runat="server" class="form_wizard wizard_horizontal" OnNextButtonClick="wzMain_NextButtonClick" OnPreviousButtonClick="wzMain_PreviousButtonClick">
                                    <WizardSteps>
                                        <asp:WizardStep ID="Step0" StepType="Start" runat="server">
                                            <asp:UpdatePanel ID="UpdatePanel2" UpdateMode="Conditional" runat="server">
                                                <ContentTemplate>
                                                    <div class="row" runat="server" visible="true">
                                                        <div class="col-xs-12 col-sm-12 col-lg-12">
                                                            <div class="input-group">
                                                                <%--<asp:RadioButtonList ID="rbtRegistrationType" Enabled="true" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="rbtRegistrationType_SelectedIndexChanged">
                                                                    <asp:ListItem class="radio-inline" Selected="True" Text="Individual" Value="I"></asp:ListItem>
                                                                    <asp:ListItem class="radio-inline" Text="Company" Value="C"></asp:ListItem>
                                                                    <asp:ListItem Text="NRI Individual" class="radio-inline" Value="NI"></asp:ListItem>
                                                                    <asp:ListItem Text="NRI Company" class="radio-inline" Value="NC"></asp:ListItem>
                                                                    <asp:ListItem Text="Self Release" class="radio-inline" Value="SC"></asp:ListItem>
                                                                    <asp:ListItem Text="NRI Self Release" class="radio-inline" Value="NSC"></asp:ListItem>
                                                                </asp:RadioButtonList>--%>
                                                            </div>
                                                        </div>
                                                        <div class="col-xs-12 col-sm-12 col-lg-9">
                                                            <div class="input-group">
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <div class="row">
                                                        <div class="fieldset panel">
                                                            <div class="fieldset-body panel-body">
                                                                <h5 class="legend-Panel"><strong class="text-uppercase"><span id="SectionTitle" runat="server">Basic Info</span> </strong></h5>

                                                                <div class="row" id="divCompany" runat="server">
                                                                    <div class="col-xs-12 col-sm-12 col-lg-8">
                                                                        <div class="input-group">
                                                                            <span class="input-group-addon" runat="server">Company Name*</span>
                                                                            <asp:TextBox ID="txtcompanyName" MaxLength="100" runat="server" CssClass="form-control" AutoCompleteType="Disabled" autocomplete="off"
                                                                                placeholder="Enter Company Name" required />
                                                                        </div>
                                                                    </div>


                                                                    <div class="col-xs-12 col-sm-12 col-lg-4" id="divDtofEst" runat="server">
                                                                        <div class="input-group">
                                                                            <span class="input-group-addon" id="spnDate" runat="server">Date of Establishment*</span>
                                                                            <asp:TextBox ID="txtDateofEstablishment" runat="server" required CssClass="form-control" AutoCompleteType="Disabled" autocomplete="off" placeholder="dd-MMM-yyyy" onBlur="GetDateField(this,false);" onFocus="SetDateField(this);" ToolTip="Format : [ddmmyyyy], [d] or [dd], [ddm] or [ddmm], [ ].&#xa;Keys : Day-[Up/Down], Month-[alt+Up/Down], Year-[shift+Up/Down], Today-[Home]." />
                                                                            <asp:CalendarExtender ID="CalendarExtender1" CssClass="zcCalendar" OnClientDateSelectionChanged="dateSelectionChanged" runat="server" TargetControlID="txtDateofEstablishment" Format="dd-MMM-yyyy"></asp:CalendarExtender>
                                                                        </div>
                                                                    </div>
                                                                    <div class="col-xs-12 col-sm-12 col-lg-7">

                                                                        <div class="input-group">
                                                                            <span class="input-group-addon" runat="server">Entity Type</span>
                                                                            <asp:RadioButtonList ID="rbtEntityType" RepeatDirection="Horizontal" RepeatColumns="4" CssClass="form-control" runat="server">
                                                                                <asp:ListItem Text="Sole Proprietary Concern" Value="SP"></asp:ListItem>
                                                                                <asp:ListItem Text="Partnership" Value="PR"></asp:ListItem>
                                                                                <asp:ListItem Text="Corporate (Pvt Ltd/Ltd Company)" Value="CP"></asp:ListItem>
                                                                            </asp:RadioButtonList>
                                                                        </div>
                                                                    </div>

                                                                    <div class="col-xs-12 col-sm-12 col-lg-5" style="display: none">
                                                                        <div class="input-group">
                                                                            <span class="input-group-addon">Short Name</span>
                                                                            <asp:TextBox ID="txtLastName" MaxLength="45" runat="server" CssClass="form-control" AutoCompleteType="Disabled" autocomplete="off"
                                                                                placeholder="Short Name" />
                                                                        </div>
                                                                    </div>
                                                                </div>
                                                                <div class="row" id="divCompanyNote" runat="server">
                                                                    <div class="col-xs-12 col-sm-12 col-lg-8">
                                                                        <span runat="server" style="font-style: italic; text-decoration: underline; color: #ed620d">PLEASE ENTER NAME OF PROPRIETOR / DIRECTOR  / PARTNER / COMPANY REPRESENTATIVE</span><br />
                                                                        <br />
                                                                    </div>
                                                                </div>
                                                                <div class="row">
                                                                    <div class="col-xs-12 col-sm-12 col-lg-6">
                                                                        <div class="input-group">
                                                                            <span id="strFirstName" class="input-group-addon">First Name*</span>
                                                                            <asp:TextBox ID="txtFname" MaxLength="30" runat="server" CssClass="form-control" AutoCompleteType="Disabled" autocomplete="off"
                                                                                placeholder="Enter First Name" required />
                                                                        </div>
                                                                    </div>

                                                                    <div class="col-xs-12 col-sm-12 col-lg-6" id="divFather" runat="Server">
                                                                        <div class="input-group">
                                                                            <span class="input-group-addon">Father Name</span>
                                                                            <asp:TextBox ID="txtFatherName" MaxLength="50" runat="server" CssClass="form-control" AutoCompleteType="Disabled" autocomplete="off"
                                                                                placeholder="Enter Father Name" />
                                                                        </div>
                                                                    </div>

                                                                    <div class="col-xs-12 col-sm-12 col-lg-6">
                                                                        <div class="input-group">
                                                                            <span class="input-group-addon">Last Name</span>
                                                                            <asp:TextBox ID="txtLname" MaxLength="45" runat="server" CssClass="form-control" AutoCompleteType="Disabled" autocomplete="off"
                                                                                placeholder="Enter Last Name" />
                                                                        </div>
                                                                    </div>
                                                                </div>


                                                                <div class="row">

                                                                    <div class="col-xs-12 col-sm-12 col-lg-6" runat="server" visible="false" id="divDesignation">
                                                                        <div class="input-group" runat="server">
                                                                            <span class="input-group-addon" id="spnDegination" runat="server">Designation*</span>
                                                                            <asp:TextBox ID="txtDesignation" MaxLength="25" runat="server" CssClass="form-control" AutoCompleteType="Disabled" autocomplete="off"
                                                                                placeholder="Enter Designation" required />
                                                                        </div>
                                                                    </div>
                                                                    <div class="col-xs-12 col-sm-12 col-lg-6" id="divAlias" runat="server" visible="true">
                                                                        <div class="input-group">
                                                                            <span class="input-group-addon" id="spnAlias" runat="server">Alias</span>
                                                                            <asp:TextBox ID="txtAccountAlias" MaxLength="100" title="Alias Name / Trade Name / Credit Name" runat="server" CssClass="form-control" AutoCompleteType="Disabled" autocomplete="off"
                                                                                placeholder="Enter Alias [',' Separated for More than 1] / Trader Name" />
                                                                        </div>
                                                                    </div>
                                                                    <div class="col-xs-12 col-sm-12 col-lg-6" id="divGender" runat="server">
                                                                        <div class="input-group">
                                                                            <span class="input-group-addon">Gender*</span>
                                                                            <asp:RadioButtonList ID="rbtnGender" RepeatDirection="Horizontal" RepeatColumns="4" CssClass="form-control" runat="server">
                                                                                <asp:ListItem Text="Male" Value="Male" Selected="True"></asp:ListItem>
                                                                                <asp:ListItem Text="Female" Value="Female"></asp:ListItem>
                                                                                <asp:ListItem Text="Other" Value="Other"></asp:ListItem>
                                                                            </asp:RadioButtonList>
                                                                        </div>
                                                                    </div>

                                                                </div>



                                                                <div class="row">

                                                                    <div class="col-xs-12 col-sm-12 col-lg-6" id="divMotherTounge" runat="server">
                                                                        <div class="input-group">
                                                                            <span class="input-group-addon">Mother Tongue</span>
                                                                            <ucDropDown:ucDropDown runat="server" ID="ddlMotherLang" MinimumPrefixLength="2" strMessage="Mother Tounge" />
                                                                            <a id="helpTooltip4" title="Minimum 2 Letter">
                                                                                <img src="Images/help-icon.png" style="position: absolute; right: 3px; z-index: 100; cursor: pointer; top: 5px">
                                                                            </a>
                                                                        &nbsp;&nbsp;</div>
                                                                    </div>

                                                                </div>







                                                                <div class="row">
                                                                    <div class="col-xs-12 col-sm-12 col-lg-6">
                                                                        <div class="input-group">
                                                                            <span class="input-group-addon">GST</span>
                                                                            <asp:RadioButtonList ID="rbtGstApl" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="rbtGstApl_SelectedIndexChanged">
                                                                                <asp:ListItem class="radio-inline" Selected="True" Text="Applicable" Value="AP"></asp:ListItem>
                                                                                <asp:ListItem class="radio-inline" Text="Not Applicable" Value="NAP"></asp:ListItem>
                                                                            </asp:RadioButtonList>
                                                                        </div>
                                                                    </div>
                                                                    <div class="col-xs-12 col-sm-12 col-lg-6" id="divGst" runat="server">
                                                                        <div class="input-group">
                                                                            <span class="input-group-addon">GST No.</span>
                                                                            <asp:TextBox ID="txtDetails1_gst" runat="server" class="form-control" AutoCompleteType="Disabled" autocomplete="off" pattern="^([a-zA-Z0-9]{15,15})$" title="eg : 27AASCS2460H1Z0"
                                                                                placeholder="Enter GST" OnTextChanged="txtDetails1_gst_TextChanged" AutoPostBack="true" />
                                                                            <ucTooltip:ucTooltip runat="server" tooltipanchor="eg : 27AASCS2460H1Z0" ID="ucTooltip2" />
                                                                        </div>
                                                                    </div>
                                                                </div>

                                                                <div class="row">
                                                                    <div class="col-xs-12 col-sm-12 col-lg-6">
                                                                        <div class="input-group">
                                                                            <span class="input-group-addon">Alternate Email ID
                                                                            </span>
                                                                            <asp:TextBox ID="txtAltEmail" MaxLength="100" runat="server" CssClass="form-control" AutoCompleteType="Disabled" autocomplete="off"
                                                                                placeholder="ENTER EMAIL" pattern="[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"></asp:TextBox><a title="Eg : xxxx@gmail.com">
                                                                                    <img src="Images/help-icon.png" style="position: absolute; right: 3px; z-index: 100; cursor: pointer; top: 5px">
                                                                                </a>
                                                                        &nbsp;&nbsp;</div>
                                                                    </div>

                                                                    <div class="col-xs-12 col-sm-12 col-lg-6">
                                                                        <div class="input-group">
                                                                            <span class="input-group-addon">Email ID*
                                                                            </span>
                                                                            <asp:TextBox ID="txtEmail" MaxLength="100" runat="server" CssClass="form-control" AutoCompleteType="Disabled" autocomplete="off"
                                                                                placeholder="ENTER EMAIL" required pattern="[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"></asp:TextBox><a title="Eg : xxxx@gmail.com">
                                                                                    <img src="Images/help-icon.png" style="position: absolute; right: 3px; z-index: 100; cursor: pointer; top: 5px">
                                                                                </a>
                                                                        &nbsp;&nbsp;</div>
                                                                    </div>

                                                                </div>

                                                                <div class="row">
                                                                    <div class="col-xs-12 col-sm-12 col-lg-6" id="divAadhar" runat="server" visible="true">
                                                                        <div class="input-group">
                                                                            <span class="input-group-addon">Aadhar No.
                                                                            </span>
                                                                            <asp:TextBox ID="txtDetails3_Aadh" MaxLength="12" runat="server" CssClass="form-control" AutoCompleteType="Disabled" autocomplete="off"
                                                                                placeholder="Enter Aadhar No" pattern="^([0-9]{12,})$" title="Aadhar No. 12 numbers minimum"></asp:TextBox>
                                                                            <ajaxToolkit:FilteredTextBoxExtender ID="fittxtAadhar" runat="server" TargetControlID="txtDetails3_Aadh"
                                                                                FilterType="Custom" ValidChars="1234567890"></ajaxToolkit:FilteredTextBoxExtender>
                                                                            </a>
                                                                        </div>
                                                                    </div>
                                                                    <div class="col-xs-12 col-sm-12 col-lg-6">
                                                                        <div class="input-group">
                                                                            <span class="input-group-addon">Mobile*
                                                                            </span>
                                                                            <asp:TextBox ID="txtCountryCode" MaxLength="3" runat="server" Width="15%" CssClass="form-control" AutoCompleteType="Disabled" autocomplete="off"
                                                                                placeholder="+91" required />
                                                                            <asp:Label ID="label1" class="form-control" runat="server" Text="-" Style="border-color: white" Width="5%" />
                                                                            <asp:TextBox ID="txtMobile" runat="server" CssClass="form-control" AutoCompleteType="Disabled" autocomplete="off" minlength="10" MaxLength="10"
                                                                                placeholder="Enter Mobile" required pattern="^([0-9]{10,})$" title="Mobile No. 10 numbers minimum" Width="70%"></asp:TextBox>
                                                                            <ajaxToolkit:FilteredTextBoxExtender ID="fittxtMobile" runat="server" TargetControlID="txtMobile"
                                                                                FilterType="Custom" ValidChars="1234567890"></ajaxToolkit:FilteredTextBoxExtender>
                                                                            <ucTooltip:ucTooltip runat="server" tooltipanchor="Eg : 0123456789" ID="ucTooltip7" />
                                                                        </div>
                                                                    </div>

                                                                    <div class="col-xs-12 col-sm-12 col-lg-6" runat="server" id="divPanNo">
                                                                        <div class="input-group">
                                                                            <span class="input-group-addon">Pan No.*</span>
                                                                            <%--<asp:TextBox ID="txtDetails2_Pan" MaxLength="10" runat="server" pattern="^[a-zA-Z]{5}[0-9]{4}[a-zA-Z]{1}$" required class="form-control" AutoCompleteType="Disabled" autocomplete="off"
                                                                                placeholder="Enter Pan No." title="Pan No Format- CEQPK4956K" onBlur="ValidatePAN();" ></asp:TextBox>
                                                                            <ajaxToolkit:FilteredTextBoxExtender ID="filterpancard" runat="server" TargetControlID="txtDetails2_Pan"
                                                                                FilterType="Numbers,UppercaseLetters,LowercaseLetters"></ajaxToolkit:FilteredTextBoxExtender>                                                                                
                                                                            <ucTooltip:ucTooltip runat="server" tooltipanchor="Eg : CEQPK4956K" ID="ucTooltip9" />--%>
                                                                            <asp:TextBox ID="txtDetails2_Pan" name="txtDetails2_Pan" MaxLength="10" runat="server" AutoPostBack="true" required class="form-control"
                                                                                placeholder="Enter Pan No." title="Pan No Format- CEQPK4956K" onBlur="ValidatePAN(event)"></asp:TextBox>
                                                                            <%--<span id="lblPANCard"  class="error"  style="display: none;" >Invalid PAN Number</span>--%>
                                                                        </div>
                                                                    </div>

                                                                </div>
                                                                <div class="row">
                                                                    <div class="col-xs-12 col-sm-12 col-lg-6">
                                                                        <div class="input-group">
                                                                            <span class="input-group-addon">Telephone
                                                                            </span>

                                                                            <asp:TextBox ID="txtTelephone" runat="server" CssClass="form-control" AutoCompleteType="Disabled" autocomplete="off" minlength="8" MaxLength="50"
                                                                                placeholder="Enter TelePhone" pattern="[^a-zA-z]+" title="Telephone Number Character in not Accept"></asp:TextBox>
                                                                            <ajaxToolkit:FilteredTextBoxExtender ID="filterPhoneno" runat="server" TargetControlID="txtTelephone"
                                                                                FilterType="Numbers,Custom" ValidChars=".,()/-"></ajaxToolkit:FilteredTextBoxExtender>
                                                                            <ucTooltip:ucTooltip runat="server" tooltipanchor="Eg : 02228698745" ID="ucTooltip8" />

                                                                        </div>
                                                                    </div>

                                                                    <div class="col-xs-12 col-sm-12 col-lg-4" id="divlang" runat="server" visible="false">
                                                                        <div class="input-group">
                                                                            <span class="input-group-addon">Preferred Language *</span>
                                                                            <asp:DropDownList ID="DropDownLang" runat="server" CssClass="form-control" Visible="false" AutoCompleteType="Disabled" autocomplete="off">
                                                                                <asp:ListItem Text="Select Language" Value="Select Language"></asp:ListItem>
                                                                                <asp:ListItem Text="English" Value="English" Selected="True"></asp:ListItem>
                                                                                <asp:ListItem Text="Hindi" Value="Hindi">
                                                                                </asp:ListItem>
                                                                            </asp:DropDownList>
                                                                        </div>
                                                                    </div>



                                                                </div>
                                                                <div class="row">
                                                                    <div class="col-xs-12 col-sm-12 col-lg-6" id="divAltMobile" runat="server">
                                                                        <div class="input-group">

                                                                            <span class="input-group-addon">Alternate/WhatsApp Mobile
                                                                            </span>
                                                                            <asp:TextBox ID="txtAlternateMobile" runat="server" CssClass="form-control" MaxLength="10" AutoCompleteType="Disabled" autocomplete="off" min="0"
                                                                                placeholder="Enter Alternate Mobile"></asp:TextBox>
                                                                            <ajaxToolkit:FilteredTextBoxExtender ID="FilteredTextBoxExtender1" runat="server" TargetControlID="txtAlternateMobile"
                                                                                FilterType="Numbers,Custom" ValidChars=".,()/-"></ajaxToolkit:FilteredTextBoxExtender>


                                                                        </div>

                                                                    </div>
                                                                    <div class="row" id="divBirthInfo" runat="server" visible="false">
                                                                        <div class="col-xs-12 col-sm-12 col-lg-4">
                                                                            <div class="input-group">
                                                                                <span class="input-group-addon">Date of Birth*
                                                                                </span>
                                                                                <asp:TextBox ID="txtDOB" runat="server" required class="form-control" Style="width: 98%" AutoCompleteType="Disabled" autocomplete="off" placeholder="dd-MMM-yyyy" onBlur="GetDateField(this,false);" onFocus="SetDateField(this);" ToolTip="Format : [ddmmyyyy], [d] or [dd], [ddm] or [ddmm], [ ].&#xa;Keys : Day-[Up/Down], Month-[alt+Up/Down], Year-[shift+Up/Down], Today-[Home].">
                                                                                </asp:TextBox>
                                                                                <asp:CalendarExtender ID="calDOB" OnClientDateSelectionChanged="dateSelectionChanged" CssClass="zcCalendar" runat="server" TargetControlID="txtDOB" Format="dd-MMM-yyyy"></asp:CalendarExtender>



                                                                            </div>
                                                                        </div>
                                                                    </div>
                                                                </div>
                                                                <div class="row">
                                                                    <div class="col-xs-12 col-sm-12 col-lg-6" id="divBirthPlace" runat="server">
                                                                        <div class="input-group">
                                                                            <span class="input-group-addon">Place of Birth*</span>
                                                                            <asp:TextBox ID="txtPOB" MaxLength="100" runat="server" pattern="[a-zA-Z ]*$" title="Only Alphabets Allowed" required CssClass="form-control" AutoCompleteType="Disabled" autocomplete="off"
                                                                                placeholder="Enter Place of Birth"></asp:TextBox>


                                                                        </div>
                                                                    </div>
                                                                    <div class="col-xs-12 col-sm-12 col-lg-6" id="divNatinality" runat="server">
                                                                        <div class="input-group">
                                                                            <span class="input-group-addon">Nationality * </span>
                                                                            <asp:TextBox ID="txtNationality" MaxLength="50" runat="server" required CssClass="form-control" AutoCompleteType="Disabled" Text="INDIAN" autocomplete="off"
                                                                                placeholder="Enter Nationality"></asp:TextBox>
                                                                        </div>
                                                                    </div>
                                                                </div>








                                                                <div class="row">

                                                                    <div class="col-xs-12 col-sm-12 col-lg-6" id="divSocial" runat="server" visible="true">
                                                                        <div class="input-group">
                                                                            <span class="input-group-addon">Social Security No
                                                                            </span>
                                                                            <asp:TextBox ID="txtSocialNo" MaxLength="100" runat="server" CssClass="form-control" AutoCompleteType="Disabled" autocomplete="off"
                                                                                placeholder="Enter Social Security No"></asp:TextBox>
                                                                            </a>
                                                                        </div>
                                                                    </div>


                                                                    <div class="col-xs-12 col-sm-12 col-lg-6" id="divTRCNo" runat="server">
                                                                        <div class="input-group">
                                                                            <span class="input-group-addon">TRC No*
                                                                            </span>
                                                                            <asp:TextBox ID="txtTrcNo" MaxLength="100" runat="server" CssClass="form-control" AutoCompleteType="Disabled" autocomplete="off"
                                                                                placeholder="Enter TRC No" required></asp:TextBox>
                                                                            </a>
                                                                        </div>
                                                                    </div>




                                                                </div>
                                                                <div class="row">
                                                                    <div class="col-xs-12 col-sm-12 col-lg-6" id="divFForm" runat="server">
                                                                        <div class="input-group">
                                                                            <span class="input-group-addon">10 F form*
                                                                            </span>
                                                                            <asp:TextBox ID="txtfForm" MaxLength="100" runat="server" CssClass="form-control" AutoCompleteType="Disabled" autocomplete="off"
                                                                                placeholder="Enter 10 F form"></asp:TextBox>
                                                                            </a>
                                                                        </div>
                                                                    </div>


                                                                    <div class="col-xs-12 col-sm-12 col-lg-4" id="divDNationality" runat="server">
                                                                        <div class="input-group">
                                                                            <span class="input-group-addon">Dual Nationality</span>
                                                                            <asp:RadioButtonList ID="RBLNationality" RepeatDirection="Horizontal" RepeatColumns="4" CssClass="form-control" runat="server">
                                                                                <asp:ListItem Text="Yes" Value="1" Selected="True"></asp:ListItem>
                                                                                <asp:ListItem Text="No" Value="0"></asp:ListItem>
                                                                            </asp:RadioButtonList>
                                                                        </div>
                                                                    </div>


                                                                </div>


                                                                <div class="row">
                                                                    <div class="col-xs-12 col-sm-12 col-lg-12" id="divchannel" runat="Server">
                                                                        <div class="input-group">
                                                                            <span class="input-group-addon">Brief about channel (Minimum 100 character)*
                                                                            </span>
                                                                            <textarea id="txtchannel" maxlength="1000" runat="server" class="form-control" autocompletetype="Disabled" autocomplete="off"
                                                                                placeholder="Enter About Channel" rows="3" cols="20" required></textarea>
                                                                        </div>
                                                                    </div>
                                                                </div>
                                                                <div class="row">

                                                                    <div class="col-xs-12 col-sm-12 col-lg-6">
                                                                        <div class="input-group">
                                                                            <span class="input-group-addon" runat="server" id="spnUpload">Upload Photo*
                                                                            </span>
                                                                            <asp:FileUpload ID="FPuploadPhoto" Width="100%" CssClass="form-control" runat="server" placeholder="ABC" AllowMultiple="false" accept="image/gif, image/jpeg, image/png" />
                                                                        </div>
                                                                    </div>
                                                                    <div class="col-xs-12 col-sm-4 col-lg-6">
                                                                        <%--   <div class="btn-group">--%>
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
                                                                        <%-- </div>--%>
                                                                    </div>
                                                                    <div id="Photo_valid_msg" style="color: red; vertical-align: bottom" />
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <div class="row">

                                                        <div class="fieldset panel">
                                                            <div class="fieldset-body panel-body">
                                                                <h5 class="legend-Panel"><strong class="text-uppercase"><span id="Span2" runat="server">Address</span> </strong></h5>
                                                                <div class="row">
                                                                    <div class="address">
                                                                        <h4>
                                                                            <span id="SpPrmnAddressType" runat="server">Present Address</span></h4>
                                                                        <div class="clearfix"></div>
                                                                    </div>
                                                                    <div class="col-xs-12 col-sm-12 col-lg-12">
                                                                        <div class="input-group">
                                                                            <span class="input-group-addon">Address*
                                                                            </span>
                                                                            <textarea id="txtAddress_PR" maxlength="255" runat="server" class="form-control" autocompletetype="Disabled" autocomplete="off"
                                                                                placeholder="Enter Present Address" rows="3" cols="20" />
                                                                        </div>
                                                                    </div>
                                                                </div>
                                                                <div class="row">
                                                                    <div class="col-xs-12 col-sm-12 col-lg-6" id="divPincode" runat="server" visible="false">
                                                                        <div class="input-group">
                                                                            <span class="input-group-addon">Pincode*</span>
                                                                            <ucDropDown:ucDropDown runat="server" ID="ddlPincode_PR" MinimumPrefixLength="2" blnChangeEvent="true" strMessage="Pincode" />

                                                                            <a id="helpTooltip2" title="Minimum 2 Letter">
                                                                                <img src="Images/help-icon.png" style="position: absolute; right: 3px; z-index: 100; cursor: pointer; top: 5px">
                                                                            </a>
                                                                        &nbsp;&nbsp;</div>
                                                                    </div>
                                                                    <div class="col-xs-12 col-sm-12 col-lg-6" id="divGeoPer" runat="server">
                                                                        <div class="input-group">
                                                                            <span class="input-group-addon">Geographical*</span>
                                                                            <ucDropDown:ucDropDown runat="server" ID="ddlGeo_PR" blEnabled="false" MinimumPrefixLength="2" strMessage="City" />
                                                                            <%--   <ucTooltip:ucTooltip runat="server" tooltipanchor="Minimum 2 Letter" ID="ucTooltip4" />--%>
                                                                        </div>
                                                                    </div>
                                                                </div>

                                                                <div class="row">
                                                                    <div class="col-xs-12 col-sm-12 col-lg-6" id="divPreCountry" runat="server">
                                                                        <div class="input-group">
                                                                            <span class="input-group-addon">Country*</span>
                                                                            <ucDropDown:ucDropDown runat="server" ID="ddlCountry" MinimumPrefixLength="2" strMessage="Country" />

                                                                            <a id="helpTooltip3" title="Minimum 2 Letter">
                                                                                <img src="Images/help-icon.png" style="position: absolute; right: 3px; z-index: 100; cursor: pointer; top: 5px">
                                                                            </a>
                                                                            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                                                        </div>
                                                                    </div>
                                                                    <div class="col-xs-12 col-sm-12 col-lg-6" id="divPreState" runat="server">
                                                                        <div class="input-group">
                                                                            <span class="input-group-addon">State*
                                                                            </span>
                                                                            <asp:TextBox ID="txtState" MaxLength="100" runat="server" CssClass="form-control" AutoCompleteType="Disabled" autocomplete="off"
                                                                                placeholder="Enter State" required></asp:TextBox>
                                                                            </a>
                                                                        </div>
                                                                    </div>

                                                                    <div class="col-xs-12 col-sm-12 col-lg-6" id="divPreCity" runat="server">
                                                                        <div class="input-group">
                                                                            <span class="input-group-addon">City*
                                                                            </span>
                                                                            <asp:TextBox ID="txtCity" MaxLength="100" runat="server" CssClass="form-control" AutoCompleteType="Disabled" autocomplete="off"
                                                                                placeholder="Enter City" required></asp:TextBox>
                                                                            </a>
                                                                        </div>
                                                                    </div>

                                                                    <div class="col-xs-12 col-sm-12 col-lg-6" runat="server" id="divZipcode" visible="false">
                                                                        <div class="input-group">
                                                                            <span class="input-group-addon">Zipcode*</span>
                                                                            <%--<ucDropDown:ucDropDown runat="server" ID="ddlZipcode"  MinimumPrefixLength="2" strMessage="ZipCode" />--%>
                                                                            <%--   <ucTooltip:ucTooltip runat="server" tooltipanchor="Minimum 2 Letter" ID="ucTooltip4" />--%>
                                                                            <asp:TextBox ID="txtzipcode" MaxLength="100" runat="server" CssClass="form-control" AutoCompleteType="Disabled" autocomplete="off"
                                                                                placeholder="Enter ZipCode" required></asp:TextBox>
                                                                        </div>

                                                                    </div>
                                                                </div>

                                                                <div class="row">
                                                                    <div class="col-xs-12 col-sm-12 col-lg-12">
                                                                        <div class="input-group">
                                                                            <span class="input-group-addon" id="SpAddressYesNo" runat="server">Is Permanent address same as the Present address ?</span>
                                                                            <asp:RadioButtonList ID="rbtnlPDCheckAddress" runat="server" CssClass="form-control" RepeatDirection="Horizontal" OnSelectedIndexChanged="rbtnlPDCheckAddress_SelectedIndexChanged" AutoPostBack="true">
                                                                                <asp:ListItem Selected="True" Text="Yes&nbsp;&nbsp;" Value="Y" style="width: 30px"></asp:ListItem>
                                                                                <asp:ListItem Text="No" Value="N"></asp:ListItem>
                                                                            </asp:RadioButtonList>
                                                                        </div>
                                                                    </div>
                                                                </div>


                                                                <asp:Panel ID="pnlPDPermanentAddress" runat="server" Visible="false">
                                                                    <div class="row">
                                                                        <div class="address">
                                                                            <h4>
                                                                                <asp:Label ID="Label3" runat="server">Permanent Address</asp:Label></h4>
                                                                            <div class="clearfix"></div>
                                                                        </div>
                                                                        <div class="col-xs-12 col-sm-12 col-lg-12">
                                                                            <div class="input-group">
                                                                                <span class="input-group-addon">Address
                                                                                </span>
                                                                                <textarea id="txtAddress_PM" maxlength="255" runat="server" class="form-control" autocompletetype="Disabled" autocomplete="off"
                                                                                    placeholder="Enter Present Address" textmode="MultiLine" rows="3" cols="20" />
                                                                            </div>
                                                                        </div>
                                                                    </div>
                                                                    <div class="row">
                                                                        <div class="col-xs-12 col-sm-12 col-lg-6" id="divPerPincode" runat="server" visible="false">
                                                                            <div class="input-group">
                                                                                <span class="input-group-addon">Pincode*</span>
                                                                                <ucDropDown:ucDropDown runat="server" ID="ddlPincode_PM" MinimumPrefixLength="2" blnChangeEvent="true" strMessage="Pincode" />
                                                                                <a id="helpTooltip3" title="Minimum 2 Letter">
                                                                                    <img src="Images/help-icon.png" style="position: absolute; right: 3px; z-index: 100; cursor: pointer; top: 5px">
                                                                                </a>
                                                                                &nbsp;&nbsp;<%--<asp:TextBox ID="txtPincode_PR" MaxLength="6" runat="server" CssClass="form-control" AutoCompleteType="Disabled" autocomplete="off" pattern="^([0-9]{6,6})$"
                                                                                        placeholder="Enter Pincode" />
                                                                                    <ajaxToolkit:FilteredTextBoxExtender ID="FilteredTextBoxExtender1" runat="server" TargetControlID="txtPincode_PR"
                                                                                        FilterType="Custom" ValidChars="1234567890"></ajaxToolkit:FilteredTextBoxExtender>--%></div>
                                                                        </div>

                                                                        <div class="col-xs-12 col-sm-12 col-lg-6" id="divgeopre" runat="server">
                                                                            <div class="input-group">
                                                                                <span class="input-group-addon">Geographical*</span>
                                                                                <ucDropDown:ucDropDown runat="server" ID="ddlGeo_PM" MinimumPrefixLength="2" blEnabled="false" strmessage="Country / State / City" />
                                                                                <ucTooltip:ucTooltip runat="server" tooltipanchor="Minimum 2 Letter" ID="ucTooltip5" />
                                                                            </div>
                                                                        </div>

                                                                    </div>





                                                                    <div class="row">
                                                                        <div class="col-xs-12 col-sm-12 col-lg-6" id="divPerCountry" runat="server">
                                                                            <div class="input-group">
                                                                                <span class="input-group-addon">Country*</span>
                                                                                <ucDropDown:ucDropDown runat="server" ID="DDLPerCountry" MinimumPrefixLength="2" strMessage="Country" />

                                                                                <a id="helpTooltip6" title="Minimum 2 Letter">
                                                                                    <img src="Images/help-icon.png" style="position: absolute; right: 3px; z-index: 100; cursor: pointer; top: 5px">
                                                                                </a>
                                                                                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                                                            </div>
                                                                        </div>
                                                                        <div class="col-xs-12 col-sm-12 col-lg-6" id="divPerState" runat="server">
                                                                            <div class="input-group">
                                                                                <span class="input-group-addon">State*
                                                                                </span>
                                                                                <asp:TextBox ID="txtPerState" MaxLength="100" runat="server" CssClass="form-control" AutoCompleteType="Disabled" autocomplete="off"
                                                                                    placeholder="Enter State" required></asp:TextBox>
                                                                                </a>
                                                                            </div>
                                                                        </div>

                                                                        <div class="col-xs-12 col-sm-12 col-lg-6" id="divPerCity" runat="server">
                                                                            <div class="input-group">
                                                                                <span class="input-group-addon">City*
                                                                                </span>
                                                                                <asp:TextBox ID="txtPerCity" MaxLength="100" runat="server" CssClass="form-control" AutoCompleteType="Disabled" autocomplete="off"
                                                                                    placeholder="Enter City" required></asp:TextBox>
                                                                                </a>
                                                                            </div>
                                                                        </div>

                                                                        <div class="col-xs-12 col-sm-12 col-lg-6" runat="server" id="divPerZip" visible="false">
                                                                            <div class="input-group">
                                                                                <span class="input-group-addon">Zipcode*</span>
                                                                                <%--<ucDropDown:ucDropDown runat="server" ID="ddlPerZip"  MinimumPrefixLength="2" strMessage="ZipCode" />--%>
                                                                                <%--   <ucTooltip:ucTooltip runat="server" tooltipanchor="Minimum 2 Letter" ID="ucTooltip4" />--%>
                                                                                <asp:TextBox ID="txtperZipCode" MaxLength="100" runat="server" CssClass="form-control" AutoCompleteType="Disabled" autocomplete="off"
                                                                                    placeholder="Enter ZipCode" required></asp:TextBox>
                                                                            </div>
                                                                        </div>
                                                                    </div>


                                                                </asp:Panel>
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <div class="row">
                                                        <br />
                                                        <br />
                                                    </div>

                                                    <div class="row">
                                                        <div class="fieldset panel">
                                                            <div class="fieldset-body panel-body" id="divroletype" runat="server">
                                                                <h5 class="legend-Panel"><strong class="text-uppercase"><span id="Span3" runat="server">Role Type</span> </strong></h5>
                                                                <div class="row">
                                                                    <div class="col-xs-12 col-sm-12 col-lg-6">

                                                                        <div class="checkbox">
                                                                            <asp:CheckBoxList ID="cbxRollType" AutoPostBack="true" RepeatDirection="Vertical" RepeatLayout="Flow" Style="list-style: none" runat="server" CssClass="form-control-static checkbox-inline">
                                                                            </asp:CheckBoxList>
                                                                        </div>
                                                                    </div>
                                                                    <div class="col-xs-12 col-sm-12 col-lg-6" style="display: none">
                                                                        <%-- <div class="col-xs-12 col-sm-12 col-lg-12">
                                                                                <asp:ListBox ID="lstlanguages" runat="server" CssClass="form-control"></asp:ListBox>
                                                                            </div>--%>
                                                                    </div>
                                                                </div>
                                                            </div>

                                                        </div>
                                                    </div>

                                                    <div class="row">
                                                        <br />
                                                    </div>

                                                    <div class="row">
                                                        <div class="col-xs-12 col-sm-12 col-lg-6">
                                                            <div class="input-group">
                                                                <span class="input-group-addon">Whether Member of Other Overseas Society?</span>
                                                                <asp:RadioButtonList ID="rbtnlMemberoverseas" runat="server" CssClass="form-control" RepeatDirection="Horizontal" RepeatLayout="Flow" OnSelectedIndexChanged="rbtnlMemberoverseas_SelectedIndexChanged" AutoPostBack="true">
                                                                    <asp:ListItem Text="Yes&nbsp;&nbsp;" Value="Y"></asp:ListItem>
                                                                    <asp:ListItem Text="No" Value="N" Selected="True"></asp:ListItem>
                                                                </asp:RadioButtonList>
                                                            </div>
                                                        </div>
                                                        <div class="col-xs-12 col-sm-12 col-lg-6" id="pnlMemberOverseas" runat="server" visible="false">
                                                            <div class="input-group">
                                                                <span class="input-group-addon">Name*</span>
                                                                <ucDropDown:ucDropDown runat="server" ID="ddlOverseasSocietyName" MinimumPrefixLength="2" strMessage="Overseas Society Name" />
                                                            </div>
                                                        </div>
                                                    </div>

                                                    <div class="row">
                                                        <div class="col-xs-12 col-sm-12 col-lg-6" id="divTerAppFor" runat="server">
                                                            <div class="input-group">
                                                                <span class="input-group-addon">Teritory applied for
                                                                </span>
                                                                <asp:DropDownList ID="DDLTerAppFor" runat="server" CssClass="form-control" AutoCompleteType="Disabled" autocomplete="off">
                                                                    <asp:ListItem Text="Select Teritory Applied for" Value="Select Teritory Applied for"></asp:ListItem>
                                                                    <asp:ListItem Text="INDIA" Value="0356"></asp:ListItem>
                                                                    <asp:ListItem Text="WORLD" Value="2136"></asp:ListItem>
                                                                </asp:DropDownList>
                                                            </div>
                                                        </div>
                                                        <div class="col-xs-12 col-sm-12 col-lg-6" id="divIPINumber" runat="Server" visible="true">
                                                            <div class="input-group">
                                                                <span class="input-group-addon">IPI Number*
                                                                </span>
                                                                <asp:TextBox ID="txtIPINumber" MaxLength="100" runat="server" CssClass="form-control" AutoCompleteType="Disabled" autocomplete="off"
                                                                    placeholder="Enter IPI Number" min="0" max="11" onkeypress="return blockSpecialChar(event)"></asp:TextBox>
                                                                </a>
                                                            </div>
                                                        </div>

                                                    </div>

                                                    <div class="row" id="divInternalNumber" runat="Server" visible="false">
                                                        <div class="col-xs-12 col-sm-12 col-lg-6">
                                                            <div class="input-group">
                                                                <span id="strInternalIdName" class="input-group-addon">Internal Identification Name*</span>
                                                                <asp:TextBox ID="txtInternalName" MaxLength="50" runat="server" CssClass="form-control" AutoCompleteType="Disabled" autocomplete="off"
                                                                    placeholder="Enter Internal Identification Name"></asp:TextBox>
                                                            </div>
                                                        </div>
                                                    </div>

                                                    <div class="row">
                                                        <div class="col-xs-12 col-sm-12 col-lg-12">
                                                            <div class="input-group">
                                                                <span class="input-group-addon">Whether Member of any Association in INDIA
                                                                </span>
                                                                <asp:TextBox ID="txtAssociationMember" MaxLength="100" runat="server" CssClass="form-control" AutoCompleteType="Disabled" autocomplete="off"
                                                                    placeholder="Eg.MCAI,SWA etc." />
                                                            </div>
                                                        </div>
                                                    </div>
                                                </ContentTemplate>
                                                <Triggers>
                                                    <asp:PostBackTrigger ControlID="btnPhotoUpload" />

                                                </Triggers>




                                            </asp:UpdatePanel>
                                        </asp:WizardStep>



                                        <asp:WizardStep ID="Step1" runat="server" StepType="Step" AllowReturn="true">

                                            <div class="row">
                                                <div class="col-xs-12 col-sm-12 col-lg-6">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">A/C Holder Name.</span>
                                                        <asp:DropDownList ID="ddlAccountHoldername" runat="server" CssClass="form-control"  />
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-12 col-lg-6">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">Account No.*</span>
                                                        <asp:TextBox ID="txtBankAcNo" MaxLength="20" runat="server" CssClass="form-control" AutoCompleteType="Disabled" autocomplete="off"
                                                            placeholder="Enter Account No." pattern="^([0-9]+)$" title="Only Numbers Allowed" required onkeypress="return blockSpecialChar(event)" />
                                                    </div>
                                                </div>

                                                <div class="col-xs-12 col-sm-12 col-lg-6">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">IFSC Code*</span>
                                                        <asp:TextBox ID="txtIFSC" MaxLength="11" runat="server" CssClass="form-control" AutoCompleteType="Disabled" min="0" autocomplete="off"
                                                            placeholder="Enter IFSC Code" required title="abcd1234567" Width="85%" /><%--pattern="^[A-Za-z]{4}[0-9]{7}$" --%>
                                                        <ajaxToolkit:FilteredTextBoxExtender ID="filterIfscCode" runat="server" TargetControlID="txtIFSC"
                                                            FilterType="Numbers,UppercaseLetters,LowercaseLetters"></ajaxToolkit:FilteredTextBoxExtender>
                                                        <ucTooltip:ucTooltip tooltipanchor="Click to see example" imgsrc="Images/cheque-locate-ifsc-micr-code.png" runat="server" ID="ucTooltip3" />
                                                        <%--<asp:ImageButton ImageUrl="~/Images/IFSC.png" runat="server" ID="imgValidate" formnovalidate="true"  for ToolTip="Validate IFSC Code" OnClick="imgValidate_Click" Style="cursor: pointer" />--%>
                                                        <%--<asp:Button ID="btnvalidate"  ImageUrl="~/Images/IFSC.png"  runat="server" Text="Validate" formnovalidate="true" ToolTip="Validate IFSC Code"  OnClick="btnvalidate_Click" CssClass="btn btn-primary btn-lg" />--%>
                                                        <asp:Button ID="btnvalidate" ImageUrl="~/Images/IFSC.png" runat="server" Text="Validate" formnovalidate="true" ToolTip="Validate IFSC Code" OnClick="btnvalidate_Click" CssClass="btn btn-success btn-sm" />
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-12 col-lg-12">
                                                    <span style="color: red; font-size: 14px; font-weight: bold" runat="server" id="lblInfo" visible="true"></span>
                                                </div>
                                                <div class="col-xs-12 col-sm-12 col-lg-6">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">Bank Name*
                                                        </span>
                                                        <%--AutoCompleteType="Disabled" autocomplete="off"--%>
                                                        <asp:TextBox ID="txtBankName" MaxLength="100" runat="server" CssClass="form-control"
                                                            placeholder="Enter Bank Name" pattern="[a-zA-Z ]*$" title="Only Alphabets Allowed" onkeypress="return blockSpecialChar(event)" required />
                                                    </div>
                                                </div>

                                                <div class="col-xs-12 col-sm-12 col-lg-6">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">Branch Name*
                                                        </span>
                                                        <asp:TextBox ID="txtBankBranchName" MaxLength="100" runat="server" CssClass="form-control" AutoCompleteType="Disabled" autocomplete="off"
                                                            placeholder="Enter Branch Name" required />
                                                    </div>
                                                </div>

                                                <div class="col-xs-12 col-sm-12 col-lg-6">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">MICR Code</span>
                                                        <asp:TextBox ID="txtMICR" MaxLength="15" runat="server" CssClass="form-control" AutoCompleteType="Disabled" autocomplete="off" TextMode="Number"
                                                            placeholder="Enter MICR Code" />
                                                        <ucTooltip:ucTooltip tooltipanchor="Click to see example" imgsrc="Images/cheque-locate-ifsc-micr-code.png" runat="server" ID="ucTooltip" />
                                                    </div>
                                                </div>




                                                <div class="col-xs-12 col-sm-12 col-lg-6" id="divCurrency" runat="server">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">Currency Type</span>
                                                        <ucDropDown:ucDropDown runat="server" ID="DDLCurrency" MinimumPrefixLength="2" strMessage="Currency Type" />
                                                        <a id="helpTooltip5" title="Select currency Type">
                                                            <img src="Images/help-icon.png" style="position: absolute; right: 3px; z-index: 100; cursor: pointer; top: 5px">
                                                        </a>
                                                    &nbsp;&nbsp;</div>
                                                </div>


                                                <div class="col-xs-12 col-sm-12 col-lg-6" id="divswift" runat="server">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">Swift Code</span>
                                                        <asp:TextBox ID="txtswiftcode" CssClass="form-control" runat="server" MaxLength="20"></asp:TextBox>
                                                    </div>
                                                </div>

                                            </div>

                                        </asp:WizardStep>

                                        <asp:WizardStep ID="Step2" runat="server" StepType="Step" AllowReturn="true">
                                            <asp:UpdatePanel ID="UpdatePanel1" UpdateMode="Conditional" runat="server">

                                                <ContentTemplate>
                                                    <div class="row">
                                                        <div class="col-xs-12 col-sm-12 col-lg-12">
                                                            <div class="input-group">
                                                                <span style="text-decoration: underline; font-style: italic">Note : Minimum 1 Work Notification Required And Maximum 5 .</span>

                                                            </div>
                                                        </div>
                                                    </div>
                                                    <div class="row">
                                                        <div class="col-xs-12 col-sm-12 col-lg-6">
                                                            <div class="input-group">
                                                                <span class="input-group-addon">Song Name*
                                                                </span>
                                                                <asp:TextBox ID="txtSongName" MaxLength="100" runat="server" CssClass="form-control" AutoCompleteType="Disabled" autocomplete="off"
                                                                    placeholder="Enter Song Name" oncopy="return false" onpaste="return false" oncut="return false" required />


                                                            </div>
                                                        </div>
                                                        <div class="col-xs-12 col-sm-12 col-lg-6">
                                                            <div class="input-group">
                                                                <span class="input-group-addon">Film / Non Film Album Name*</span>
                                                                <asp:TextBox ID="txtFilm_AlbumName" MaxLength="100" runat="server" CssClass="form-control" AutoCompleteType="Disabled" autocomplete="off"
                                                                    placeholder="Enter Film / Non Film Album Name" oncopy="return false" onpaste="return false" oncut="return false" required />
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <div class="row">
                                                        <div class="col-xs-12 col-sm-12 col-lg-6">
                                                            <div class="input-group">
                                                                <span class="input-group-addon">Language*
                                                                </span>
                                                                <asp:TextBox ID="txtLanguageNames" MaxLength="100" runat="server" CssClass="form-control" AutoCompleteType="Disabled" autocomplete="off"
                                                                    placeholder="Enter Language" required />
                                                            </div>
                                                        </div>
                                                        <div class="col-xs-12 col-sm-12 col-lg-6">
                                                            <div class="input-group">
                                                                <span class="input-group-addon">Category*</span>

                                                                <asp:DropDownList ID="ddlWorkCategory" runat="server" CssClass="form-control" AutoCompleteType="Disabled" autocomplete="off">
                                                                    <asp:ListItem Text="Select Category" Value="Select Category"></asp:ListItem>
                                                                    <asp:ListItem Text="Film" Value="Film"></asp:ListItem>
                                                                    <asp:ListItem Text="Non-Film" Value="Non-Film">
                                                                    </asp:ListItem>
                                                                    <asp:ListItem Text="BG-TV" Value="BG-TV">
                                                                    </asp:ListItem>
                                                                    <asp:ListItem Text="BG-Film" Value="BG-Film">
                                                                    </asp:ListItem>
                                                                    <asp:ListItem Text="AD-Jingle" Value="AD-Jingle">
                                                                    </asp:ListItem>
                                                                </asp:DropDownList>
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <div class="row">
                                                        <div class="col-xs-12 col-sm-12 col-lg-12">
                                                            <div class="input-group">
                                                                <span class="input-group-addon">Artist/Singer-For Multiple Singers*
                                                                </span>
                                                                <textarea id="txtArtistorMultipleSingers" maxlength="500" runat="server" class="form-control" autocompletetype="Disabled" autocomplete="off" rows="3" cols="20"
                                                                    placeholder="please use(Comma(,) as a seperator between singers :)" required />
                                                                <ucTooltip:ucTooltip tooltipanchor="use(Comma(,) as a seperator between singers" runat="server" ID="ucTooltip1" />
                                                            </div>
                                                        </div>

                                                    </div>
                                                    <div class="row">
                                                        <div class="col-xs-12 col-sm-12 col-lg-12">
                                                            <div class="input-group">
                                                                <span class="input-group-addon">Author(Music Composer / Composer)*
                                                                </span>

                                                                <asp:TextBox ID="txtAuthorMusicComposer" MaxLength="100" runat="server" CssClass="form-control" AutoCompleteType="Disabled" autocomplete="off"
                                                                    placeholder="Author(Music Composer / Composer)" required />
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <div class="row">
                                                        <div class="col-xs-12 col-sm-12 col-lg-12">
                                                            <div class="input-group">
                                                                <span class="input-group-addon">Author (Lyricist)*
                                                                </span>
                                                                <asp:TextBox ID="txtAuthorLyricist" MaxLength="100" runat="server" CssClass="form-control" AutoCompleteType="Disabled" autocomplete="off"
                                                                    placeholder="Enter Author Lyricist" required />
                                                            </div>
                                                        </div>
                                                        <div class="col-xs-12 col-sm-12 col-lg-12">
                                                            <div class="input-group">
                                                                <span class="input-group-addon">Publisher*</span>
                                                                <asp:TextBox ID="txtPublisher" MaxLength="100" runat="server" CssClass="form-control" AutoCompleteType="Disabled" autocomplete="off"
                                                                    placeholder="Enter Publisher" required />
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <div class="row">
                                                        <div class="col-xs-12 col-sm-12 col-lg-3">
                                                            <div class="input-group">
                                                                <span class="input-group-addon">Release Year*
                                                                </span>
                                                                <asp:TextBox ID="txtRelYear" MaxLength="4" runat="server" CssClass="form-control" type="number" step="1" min="1900" AutoCompleteType="Disabled" autocomplete="off"
                                                                    placeholder="Enter Rel. Yr" required />
                                                            </div>
                                                        </div>
                                                        <div class="col-xs-12 col-sm-12 col-lg-9">
                                                            <div class="input-group">
                                                                <span class="input-group-addon">Digital Link*</span>
                                                                <asp:TextBox ID="txtDigitalLink" MaxLength="500" runat="server" CssClass="form-control" AutoCompleteType="Disabled" autocomplete="off"
                                                                    placeholder="Enter Digital Link" required />
                                                            </div>
                                                        </div>
                                                    </div>

                                                    <div class="row">
                                                        <div class="col-xs-12 col-sm-12 col-lg-7">
                                                            <div class="input-group">
                                                                <span class="input-group-addon">Upload Files*
                                                                </span>
                                                                <table>
                                                                    <tr>
                                                                        <td>
                                                                            <%--CHANGED BY RENU ON 3/11/2020--%>

                                                                            <asp:FileUpload ID="FPWork" runat="server" accept="image/gif, image/jpeg, image/png, .pdf" AllowMultiple="false" CssClass="form-control" required /></td>
                                                                        <td><span style="color: red; font-size: 15px; font-family: monospace; padding-left: 10px; font-weight: bold;">Note:Upload file within 5MB size</span></td>
                                                                    </tr>
                                                                </table>

                                                            </div>
                                                        </div>
                                                        <div class="col-xs-12 col-sm-12 col-lg-6">
                                                            <div class="input-group">
                                                                <div id="valid_msg" style="color: red; vertical-align: bottom" />
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <div class="row"></div>
                                                    <div class="row">
                                                        <div class="col-xs-12 col-sm-12 col-lg-5"></div>
                                                        <div class="col-xs-12 col-sm-12 col-lg-2">
                                                            <div class="input-group">
                                                                <asp:Button ID="btnAddWork" runat="server" Text="Add" ToolTip="ADD RECORDS" OnClientClick="return WorkFileValidate();" OnClick="btnAddWork_Click" CssClass="btn btn-primary btn-lg" />
                                                            </div>
                                                        </div>
                                                        <div class="col-xs-12 col-sm-12 col-lg-5"></div>
                                                    </div>
                                                    <div class="row">

                                                        <div align="center" style="padding-top: 20px;" id="divGridWork" runat="server">
                                                            <div class="row">

                                                                <div class="col-xs-12 col-sm-12 col-lg-12">

                                                                    <div id="DivWork" onscroll="SetDivPosition(this)" runat="server" style="min-height: 150px; overflow-x: scroll; overflow-y: auto;">

                                                                        <asp:GridView ID="gvWork" runat="server" CssClass="grid-header" OnRowDataBound="gvWork_RowDataBound"
                                                                            ShowHeader="true" AutoGenerateColumns="false" ShowHeaderWhenEmpty="true">
                                                                            <Columns>

                                                                                <asp:BoundField DataField="SongName" ItemStyle-Width="300px" HeaderText="Song Name" ItemStyle-HorizontalAlign="Left" />
                                                                                <asp:BoundField DataField="Film_AlbumName" ItemStyle-Width="300px" HeaderText="Film AlbumName" ItemStyle-HorizontalAlign="Left" />
                                                                                <asp:BoundField DataField="LanguageNames" ItemStyle-Width="300px" HeaderText="Languages" ItemStyle-HorizontalAlign="Left" />
                                                                                <asp:BoundField DataField="WorkCategory" ItemStyle-Width="300px" HeaderText="Category" ItemStyle-HorizontalAlign="Left" />
                                                                                <asp:BoundField DataField="Artist_Singers" ItemStyle-Width="300px" HeaderText="Artist Singers" ItemStyle-HorizontalAlign="Left" />
                                                                                <asp:BoundField DataField="Author_Composer" ItemStyle-Width="300px" HeaderText="Author Composer" ItemStyle-HorizontalAlign="Left" />
                                                                                <asp:BoundField DataField="Publisher" ItemStyle-Width="300px" HeaderText="Publisher" ItemStyle-HorizontalAlign="Left" />
                                                                                <asp:BoundField DataField="Author_Lyricist" ItemStyle-Width="300px" HeaderText="Author Lyricist" ItemStyle-HorizontalAlign="Left" />
                                                                                <asp:HyperLinkField DataNavigateUrlFields="DigitalLink" DataTextField="DigitalLink" ItemStyle-Width="300px" HeaderText="DigitalLink" ItemStyle-HorizontalAlign="Left" Target="_blank" />
                                                                                <asp:BoundField DataField="ReleaseYear" ItemStyle-Width="100px" HeaderText="ReleaseYear" ItemStyle-HorizontalAlign="Left" />
                                                                                <asp:TemplateField ItemStyle-CssClass="50px" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="text-center" HeaderText="Docs">
                                                                                    <ItemTemplate>

                                                                                        <asp:HyperLink ID="hypworkfile" Text='View' CssClass="btn btn-link" Target="_blank" runat="server" />
                                                                                    </ItemTemplate>
                                                                                </asp:TemplateField>
                                                                                <asp:TemplateField ItemStyle-CssClass="50px" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="text-center" HeaderText="Docs">
                                                                                    <ItemTemplate>
                                                                                        <asp:ImageButton ID="imgtWorkfile" formnovalidate="true" ImageUrl="~/Images/file_add.png" runat="server" ImageAlign="Bottom" ToolTip="upload file" OnClick="imgtWorkfile_Click" />
                                                                                    </ItemTemplate>
                                                                                </asp:TemplateField>
                                                                                <asp:TemplateField ItemStyle-Width="100px" HeaderText="">
                                                                                    <ItemTemplate>
                                                                                        <asp:LinkButton ID="btnWorkDelete" runat="server" formnovalidate="true" OnClientClick="return ConfirmDelete(this.id);" class="btn btn-danger" OnClick="btnWorkDelete_Click" title="Delete" CommandArgument='<%# Eval("WorkNotificationId") %>'>
                                                                                        <span class="fa fa-trash-o"></span>
                                                                                        </asp:LinkButton>

                                                                                        <asp:HiddenField ID="hdnWorkNotificationId" runat="server" Value='<%# Eval("WorkNotificationId") %>' />
                                                                                    </ItemTemplate>
                                                                                </asp:TemplateField>
                                                                            </Columns>
                                                                            <EmptyDataTemplate>
                                                                                <div class="center">
                                                                                    <table cellpadding="0" cellspacing="0" width="50%">

                                                                                        <tr>
                                                                                            <td align="center" style="text-align: center;">
                                                                                                <b>No Records Found</b>
                                                                                            </td>
                                                                                        </tr>
                                                                                    </table>
                                                                                </div>
                                                                            </EmptyDataTemplate>

                                                                        </asp:GridView>

                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>


                                                    <asp:LinkButton ID="lnkuploadfiles" runat="server"></asp:LinkButton>


                                                    <asp:Panel ID="pnluploadFiles" Width="70%" runat="server" CssClass="modalPopup"
                                                        Style="display: none; z-index: 1070;">
                                                        <div style="overflow-y: auto; overflow-x: hidden; max-height: 550px;">
                                                            <div class="modal-header">
                                                                <asp:Label ID="Label2" Text="Upload Work File" runat="server" CssClass="modal-title"></asp:Label>
                                                            </div>
                                                            <div class="modal-body">
                                                                <div class="form-group">
                                                                    <div class="row">
                                                                        <div class="col-xs-12 col-sm-12 col-lg-6">
                                                                            <asp:HiddenField ID="hdnWorkNotificationId" runat="server" />
                                                                            <asp:FileUpload ID="fpworkfileupload" Width="99%" runat="server" AllowMultiple="true" />
                                                                        </div>

                                                                        <div class="col-xs-12 col-sm-12 col-lg-5">
                                                                            <asp:Button ID="btnworkfileupload" formnovalidate="true" CssClass="btn btn-success btn-sm" Text="Upload" OnClientClick="return WorkFileValidatepnl();" OnClick="btnworkfileupload_Click" runat="server" ValidationGroup="upload" />
                                                                        </div>
                                                                        <br />
                                                                        <div class="col-xs-12 col-sm-12 col-lg-12">
                                                                            <span id="spfileErrormsg"></span>
                                                                        </div>

                                                                    </div>
                                                                </div>
                                                                <div align="center" class="modal-footer">
                                                                    <div class="row">
                                                                        <div class="col-md-12">

                                                                            <button id="Button1" runat="server" formnovalidate="true" class="btn btn-primary">
                                                                                Cancel
                                                                            </button>
                                                                        </div>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </asp:Panel>

                                                    <asp:ModalPopupExtender ID="moduploadFiles" runat="server" PopupControlID="pnluploadFiles"
                                                        TargetControlID="lnkuploadfiles" BehaviorID="moduploadFiles"
                                                        BackgroundCssClass="modalBackground">
                                                    </asp:ModalPopupExtender>


                                                </ContentTemplate>

                                                <Triggers>

                                                    <asp:PostBackTrigger ControlID="btnAddWork" />
                                                    <asp:PostBackTrigger ControlID="btnworkfileupload" />

                                                </Triggers>
                                            </asp:UpdatePanel>
                                        </asp:WizardStep>

                                        <asp:WizardStep ID="Step3" runat="server" StepType="Step" AllowReturn="true">
                                            <asp:UpdatePanel ID="UpdtPnlLogin" UpdateMode="Always" runat="server">
                                                <ContentTemplate>
                                                    <div>
                                                        <%--<ucNomineeDetails:ucNomineeDetails runat="server" ID="ucNomineeDetails" data-usercontrol="ucNomineeDetails" />--%>
                                                    </div>
                                                </ContentTemplate>
                                                <Triggers>
                                                </Triggers>
                                            </asp:UpdatePanel>
                                        </asp:WizardStep>


                                        <asp:WizardStep ID="Step4" runat="server" StepType="Step" AllowReturn="true">
                                            <div class="row">
                                                <div class="col-xs-12 col-sm-12 col-lg-12">
                                                    <div id="dvScroll1" onscroll="SetDivPosition(this)" runat="server" style="overflow: auto; overflow-x: hidden;">

                                                        <asp:GridView CssClass="table table-bordered dt-responsive nowrap" ID="grdDocumentsPreApproval" AutoGenerateColumns="false"
                                                            ViewStateMode="Enabled" runat="server" ShowFooter="true" Width="100%" OnRowDataBound="grdDocumentsPreApproval_RowDataBound">
                                                            <Columns>
                                                                <asp:TemplateField HeaderText="Sr No" ItemStyle-Width="10%" ItemStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="top" HeaderStyle-HorizontalAlign="Center" HeaderStyle-CssClass="text-center">
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="lblSrno" runat="server" Text='<%# ((GridViewRow)Container).RowIndex + 1%>'></asp:Label>
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="List of Documents" ItemStyle-Width="45%" HeaderStyle-CssClass="text-center">
                                                                    <ItemTemplate>
                                                                        <asp:HiddenField ID="hdnDocLookupId" Value='<%#Eval("DocumentLookupId") %>' runat="server" />
                                                                        <asp:HiddenField ID="hdnIsCompulsary" Value='<%#Eval("IsCompulsary") %>' runat="server" />
                                                                        <asp:HiddenField ID="hdnUploadedCount" Value='<%#Eval("UploadedCount") %>' runat="server" />
                                                                        <asp:Label ID="lblDocumentName" Text='<%#Eval("DocumentName") %>' runat="server" />
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>
                                                                <asp:TemplateField ItemStyle-Width="30%" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="text-center" HeaderText="Note : Only jpeg, jpg , gif, png, pdf,doc,xls,xlsx allowed. Max File Size : 5MB">
                                                                    <ItemTemplate>
                                                                        <asp:UpdatePanel ID="upnlfile" UpdateMode="Conditional" runat="server">
                                                                            <ContentTemplate>
                                                                                <asp:FileUpload ID="FileUpload1" Width="99%" runat="server" AllowMultiple="true" />
                                                                                <br />
                                                                                <span id="spfileErrormsg"></span>
                                                                                <asp:Button ID="btnfileupload" CssClass="btn btn-success btn-sm" Text="Upload" OnClientClick="return DocFileValidate(this)" OnClick="btnfileupload_Click" runat="server" ValidationGroup="upload" />
                                                                            </ContentTemplate>
                                                                            <Triggers>
                                                                                <asp:PostBackTrigger ControlID="btnfileupload" />
                                                                            </Triggers>
                                                                        </asp:UpdatePanel>

                                                                    </ItemTemplate>
                                                                    <FooterTemplate>
                                                                        <asp:Label ID="lblfooterErrorMsg" runat="server"></asp:Label>
                                                                    </FooterTemplate>
                                                                </asp:TemplateField>
                                                                <asp:TemplateField ItemStyle-CssClass="15%" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="text-center" HeaderText="Note : Only jpeg, jpg , gif, png, pdf,doc,xls,xlsx allowed">
                                                                    <ItemTemplate>

                                                                        <asp:LinkButton ID="btnViewUploadedFiles" runat="server" title="Click to View Files" OnClick="btnViewUploadedFiles_Click" Text="VIEW UPLOADED FILES" CssClass="btn btn-link" Style="padding: 0 10px">
                                                                        </asp:LinkButton>
                                                                        <br />
                                                                        Upload Count :
                                                                        <asp:Label ID="LblFilecount" runat="server" Text='<%# Eval("UploadedCount") %>'></asp:Label>

                                                                    </ItemTemplate>
                                                                </asp:TemplateField>
                                                            </Columns>
                                                        </asp:GridView>
                                                    </div>
                                                </div>
                                            </div>


                                            <%--//lnkviewfiles--%>
                                            <asp:LinkButton ID="lnkviewfiles" runat="server"></asp:LinkButton>

                                            <asp:Panel ID="pnlViewFiles" Width="70%" runat="server" CssClass="modalPopup"
                                                Style="display: none; z-index: 1070;">
                                                <div style="overflow-y: auto; overflow-x: hidden; max-height: 550px;">
                                                    <div class="modal-header">
                                                        <asp:Label ID="lblHeadingofFiles" runat="server" CssClass="modal-title"></asp:Label>
                                                    </div>
                                                    <div class="modal-body">
                                                        <div class="form-group">
                                                            <div class="row">
                                                                <asp:GridView CssClass="grid-header" Width="100%" ID="grdViewUploadedFiles" AutoGenerateColumns="false"
                                                                    ViewStateMode="Enabled" runat="server">
                                                                    <EmptyDataRowStyle HorizontalAlign="Center" />
                                                                    <EmptyDataTemplate>
                                                                        No files have been uploaded
                                                                    </EmptyDataTemplate>
                                                                    <Columns>
                                                                        <asp:TemplateField HeaderText="File Name" ItemStyle-Width="80%" HeaderStyle-CssClass="">
                                                                            <ItemTemplate>
                                                                                <asp:HiddenField ID="hdnDocumentLookupId" Value='<%#Eval("DocumentLookupId") %>' runat="server" />
                                                                                <asp:HiddenField ID="hdnAccountDocId" Value='<%#Eval("AccountDocId") %>' runat="server" />
                                                                                <asp:HiddenField ID="hdnFileName" Value='<%#Eval("DocFileName") %>' runat="server" />
                                                                                <asp:HyperLink Text='<%#Eval("DocumentName") %>' CssClass="btn btn-link" Target="_blank" NavigateUrl='<%#"MemberRegDocs/"+Eval("DocFileName") %>' runat="server" />
                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>

                                                                        <asp:TemplateField>
                                                                            <ItemStyle HorizontalAlign="Center" />
                                                                            <ItemTemplate>
                                                                                <asp:LinkButton ID="ibtFileDelete" runat="server" OnClientClick="return ConfirmDelete(this.id);" CssClass="btn btn-danger" OnClick="ibtFileDelete_Click">
                                                                 <span class="fa fa-trash-o"></span>
                                                                                </asp:LinkButton>

                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>
                                                                    </Columns>
                                                                </asp:GridView>
                                                            </div>
                                                        </div>
                                                        <div align="center" class="modal-footer">
                                                            <div class="row">
                                                                <div class="col-md-12">

                                                                    <button id="btnClosefiles" runat="server" formnovalidate="true" class="btn btn-primary">
                                                                        Cancel
                                                                    </button>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </asp:Panel>

                                            <asp:ModalPopupExtender ID="modViewFiles" runat="server" PopupControlID="pnlViewFiles"
                                                TargetControlID="lnkviewfiles" BehaviorID="modViewFiles"
                                                BackgroundCssClass="modalBackground">
                                            </asp:ModalPopupExtender>

                                        </asp:WizardStep>

                                        <asp:WizardStep ID="Step5" StepType="Step" runat="server">
                                            <div id="divPayment" runat="server" class="row" visible="false">
                                                <div class="input-group">
                                                    <span class="input-group-addon">Membership Amount</span>
                                                    <asp:TextBox ID="txtMemberShipAmt" CssClass="form-control" ReadOnly runat="server"></asp:TextBox>

                                                </div>

                                            </div>
                                            <div class="row">
                                                <div class="col-xs-12 col-sm-12 col-lg-12" style="text-align: center; margin-top: 5vh">

                                                    <div class="col-xs-12 col-sm-12 col-lg-4">
                                                        <asp:ImageButton ID="btnReviewApplication" ImageUrl="~/Images/review.png" OnClick="btnReviewApplication_Click" runat="server" />
                                                    </div>
                                                    <div class="col-xs-12 col-sm-12 col-lg-4">
                                                        <asp:ImageButton ID="btnSubmitApplication" ImageUrl="~/Images/submit-application.png" OnClick="btnSubmitApplication_Click" OnClientClick="return confirm('Are you sure, you want to Submit the Application? \nAfter submission, you will not able to make the changes in Application.');" runat="server" />
                                                    </div>


                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="col-xs-12 col-sm-12 col-lg-12" id="frmError" runat="server"></div>
                                            </div>
                                            <div class="row">
                                                <div class="col-xs-12 col-sm-12 col-lg-12">
                                                    <asp:Panel GroupingText="Important Note :" CssClass="fa-border col-xs-12 col-sm-12 col-lg-12" Style="margin-top: 8vh; font-size: small" runat="server">
                                                        <span><b>Review Application Button :-</b> Click here to review the completed application.<br></br>
                                                            <br></br>
                                                            <b>Payment & Submit Button :-</b> Click here to pay the Membership amount and proceed for submission. Note that upon submission, you will not be able to make changes to the application until the admin approves.<br></br>
                                                            <br></br>

                                                        </span>

                                                    </asp:Panel>
                                                </div>
                                            </div>

                                            <asp:LinkButton ID="lnkPayment" runat="server"></asp:LinkButton>
                                            <asp:ModalPopupExtender ID="modPayment" runat="server" PopupControlID="PnlPayment"
                                                TargetControlID="lnkPayment" BehaviorID="modPayment"
                                                BackgroundCssClass="modalBackground">
                                            </asp:ModalPopupExtender>

                                            <asp:Panel ID="PnlPayment" Width="40%" runat="server" CssClass="modalPopup"
                                                Style="display: none; z-index: 1070;">
                                                <div style="overflow-y: auto; overflow-x: hidden; max-height: 550px;">
                                                    <div class="modal-header">
                                                        <div class="col-xs-12 col-sm-12 col-lg-12" style="text-align: right">
                                                            <button id="Button3" runat="server" formnovalidate="true" class="close">
                                                                &times;
                                                            </button>
                                                        </div>
                                                    </div>
                                                    <div class="modal-body">
                                                        <div class="form-group">
                                                            <div class="row">
                                                                <div class="col-xs-12 col-sm-12 col-lg-12" style="text-align: center">
                                                                    <span id="spPayMsg" runat="server"></span>
                                                                </div>
                                                            </div>

                                                            <div class="row">

                                                                <%--   <div class="col-xs-12 col-sm-12 col-lg-4">
                                                                    <asp:Button ID="btnContinueToPay" runat="server" Text="Continue To Pay" OnClick="btnContinueToPay_Click" CssClass="btn btn-primary btn-lg" />
                                                                </div>
                                                                <div class="col-xs-12 col-sm-12 col-lg-1" style="text-align:center">
                                                                   <span style="color:#808080"> OR</span>
                                                                </div>--%>
                                                                <div class="col-xs-12 col-sm-12 col-lg-2"></div>
                                                                <div class="col-xs-12 col-sm-12 col-lg-4">
                                                                    <asp:Button ID="bntMakePayment" runat="server" Text="Make Payment" OnClick="bntMakePayment_Click" CssClass="btn btn-primary btn-lg" />
                                                                </div>
                                                                <div class="col-xs-12 col-sm-12 col-lg-4">
                                                                    <button id="Button2" runat="server" formnovalidate="true" class="btn btn-primary btn-lg">
                                                                        Cancel
                                                                    </button>
                                                                </div>
                                                                <div class="col-xs-12 col-sm-12 col-lg-2"></div>
                                                            </div>
                                                        </div>
                                                        <%-- <div align="center" class="modal-footer">
                                                                    <div class="row">
                                                                        <div class="col-md-12">

                                                                            <button id="Button3" runat="server" formnovalidate="true" class="btn btn-primary">
                                                                                Cancel
                                                                            </button>
                                                                        </div>
                                                                    </div>
                                                                </div>--%>
                                                    </div>
                                                </div>
                                            </asp:Panel>

                                        </asp:WizardStep>

                                        <asp:WizardStep ID="Step6" StepType="Complete" runat="server">
                                            <div class="row">

                                                <div class="col-xs-12 col-sm-12 col-lg-12">
                                                    <h3 style="text-align: center">Thank you for completing the  application and providing all the details.</h3>
                                                </div>
                                                <div class="col-xs-12 col-sm-12 col-lg-12">
                                                    <h3 style="text-align: center">We will review your application and get back to you with our decision.<br />
                                                        In the meanwhile we have sent you an email with  a PDF of your Profile details.</h3>
                                                </div>
                                                <div class="col-xs-12 col-sm-12 col-lg-12" style="text-align: center">
                                                    <span style="font-size: medium;"><b>Note:</b> Your Application is not yet approved. It is Under Approval Process.
                                                    </span>
                                                </div>
                                                <div class="col-xs-12 col-sm-12 col-lg-12 " style="text-align: center">
                                                    <asp:HyperLink NavigateUrl="Default.aspx" Style="font-size: medium; text-align: center; font-weight: bolder" Text="CLICK HERE TO CONTINUE" CssClass="btn btn-link" runat="server" />
                                                </div>
                                            </div>
                                        </asp:WizardStep>
                                    </WizardSteps>
                                    <StartNavigationTemplate>
                                        <br />
                                        <asp:Button ID="NextButton" Visible="true" runat="server" Text="Save & Continue" ToolTip="SAVE RECORDS AND CONTINUE TO THE NEXT" CommandName="MoveNext" CssClass="btn btn-primary btn-sm" />
                                        <br />
                                    </StartNavigationTemplate>
                                    <StepNavigationTemplate>
                                        <br />
                                        <asp:Button ID="btnStepPrevious" formnovalidate="true" runat="server" ToolTip="CONTINUE TO PREVIOUS SCREEN WITHOUT SAVING RECORDS" CommandName="MovePrevious" Text="Back" CssClass="btn btn-success btn-sm" />
                                        <asp:Button ID="btnStepNext" formnovalidate="true" runat="server" ToolTip="SAVE RECORDS AND CONTINUE TO THE NEXT" Text="Continue" CommandName="MoveNext" CssClass="btn btn-primary btn-sm" />
                                        <asp:Button ID="btnContinue" formnovalidate="true" runat="server" ToolTip="CONTINUE TO THE NEXT" Text="Continue" Visible="false" OnClick="btnContinue_Click" CssClass="btn btn-primary btn-sm" />

                                    </StepNavigationTemplate>
                                    <FinishNavigationTemplate>
                                    </FinishNavigationTemplate>
                                </asp:Wizard>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </ContentTemplate>


    </asp:UpdatePanel>

</asp:Content>
