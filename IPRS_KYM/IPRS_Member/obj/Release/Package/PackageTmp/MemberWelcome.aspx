<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MemberWelcome.aspx.cs" Inherits="IPRS_Member.MemberWelcome" %>

<%-- For Ajax controls like HiddenFiled, UpdateProgress : Commented By Rohit --%>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<%-- Here we use this user control to find the geographic information by entering the pincode. : Commented By Rohit --%>
<%@ Register Src="~/User_Controls/ucDropDown.ascx" TagPrefix="ucDropDown" TagName="ucDropDown" %>

<%-- Here use this user control to give inline expression to textbox and other control. : Commented By Rohit --%>
<%@ Register Src="~/User_Controls/ucTooltip.ascx" TagPrefix="ucTooltip" TagName="ucTooltip" %>

<!DOCTYPE html>

<script src="Javascript/FCalender.js"></script>
<link href="Style/zcCalendar.css" rel="stylesheet" />

<%--Script for slected date in Specific Date Format DD-MM-YYYY : Commented By Rohit--%>

<%--this script create for date format change on selected but this we cant use in code : Commented By Rohit--%>
<script type="text/javascript">

    function dateSelectionChanged(sender, args)
    {
        selectedDate = sender.get_selectedDate();
        var date = moment(new Date(selectedDate));

        sender._textbox._element.value = (moment(date).format('DD-MM-YYYY'));
        SetDateField(sender._textbox._element);
    }

</script>


<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>..::..Welcome..::..</title>

    <meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />
    <!-- Meta, title, CSS, favicons, etc. -->
    <meta charset="utf-8" />
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />

    <%--#region style sheet--%>
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
    <link href="Style/modalPopup.css" rel="stylesheet" />
    <%--#endregion style sheet--%>

    <%--#region java script--%>
    <!-- Custom Theme Scripts -->
    <script src="~/javascript/custom.min.js"></script>
    <script src="javascript/jquery/jquery.min.js"></script>
    <!-- Bootstrap -->
    <script src="javascript/bootstrap/bootstrap.min.js"></script>
    <!-- Bootstrap Alert Box -->
    <script src="javascript/bootstrap/bootbox.min.js"></script>
    <!-- Custom Theme Scripts -->
    <script src="javascript/custom.min.js"></script>
    <%--#endregion java script--%>

    <%--#region progress bar--%>
    <%--this style and script for wait to progress--%>
    <style>
        #dvLoading {
            background: #000 url(Images/AnimatedProgressBar.gif) no-repeat center center;
        }
    </style>

    <script>
        $(window).load(function () {
            $('#dvLoading').fadeOut(2000);
        });
    </script>
    <%--#endregion progress bar--%>

    <%--#region restrict key in textbox--%>
    <!--THIS FUNCTION IS TO RESTRICT MAX WITH NUMBER TYPE TEXT BOX-->
    <script type="text/javascript" lang="JavaScript">
     function maxLengthCheck(object, MaxLength) {
        if (object.value.length == MaxLength && event.keyCode != 8 && event.keyCode != 46 && event.keyCode != 37 && event.keyCode != 39)
            object.value = object.value.slice(0, object.maxLength);
    }
    </script>

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
    <%--#endregion restrict key in textbox--%>


    <style type="text/css">
        @media (min-width:1200px) {
            .col-lg-6 {
                float: left;
                width: 49% !important;
            }

            .last {
                margin-right: 0;
                float: right;
            }
        }

        @media (max-width:480px) {
            .col-lg-6 {
                float: left;
                width: 100% !important;
            }
        }
    </style>


</head>

<body style="background-color: white;">
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" />

        <%--Display header image : Commented By Rohit--%>
        <asp:Image runat="server" ImageUrl="~/Images/header.jpg" Height="100%" Width="100%" />

        <%--this main panel : Commented By Rohit--%>

        <asp:UpdatePanel ID="upnlwzMain" UpdateMode="Always" runat="server">
            <ContentTemplate>
                <div class="x_panel">
                    <%--#region hidden field and informative files : Commented By Rohit--%>
                    <%--Required necessary data information relating to member so that information we are store in hidden fields : Commented By Rohit--%>
                    <asp:HiddenField ID="hdnRoleTypeIds_I" runat="server" />
                    <asp:HiddenField ID="hdnRoleTypeIds_C" runat="server" />
                    <asp:HiddenField ID="hdnDAccountId" Value="" runat="server" />
                    <%--here we given link to member how to register with IPRS and what is necessary document for registration : Commented By Rohit--%>
                    <span style="float: right; padding: 5px;">
                        <a href="UserDocs/UserGuide.pdf" target="_blank" class="button button-green">User Guide</a>
                        <a href="UserDocs/MANDATORYDOCUMENTS.pdf" target="_blank" class="button button-green">Mandatory Document</a>
                    </span>
                    <%--#endregion hidden field and informative files : Commented By Rohit--%>
                    
                    <div class="x_content">
                        <div class="container">

                            <%--#region information and terms : Commented By Rohit--%>

                            <div class="panel panel-default">
                                <div class="panel-heading">
                                    <%--information, terms and condition related to IPRS : Commented By Rohit--%>
                                    <h2>About IPRS </h2>
                                </div>
                                <div class="panel-body">
                                    <h4 class="justify">IPRS administers the rights of its members by issuing Licenses to users / platforms of music and by collecting Royalties
                        for and on behalf of its Members i.e. the Authors, the Composers and the Publishers of Music and distribute this Royalty amongst
                        them.
                                    </h4>
                                    <%--<a href="#" class="red" style="float: right">Read More >> </a>--%>
                                </div>
                                <div class="panel-heading">
                                    <h2>Terms & Conditions</h2>
                                </div>
                                <div class="panel-body">
                                    <div style="height: 150px; overflow-y: scroll">
                                        <h4 class="justify">For Online Membership Form, it is approximately 15 minutes (In case of Session timeout, reactivate the Session by
                            pressing the REFRESH tab). There are MANDATORY DOCUMENTS required that need to be uploaded. Please read the user-guide
                            for detailed process information to complete the online membership form and keep the necessary documents ready for
                            upload when required. The Indian Performing Right Society Limited Web Site may contain links to other Web Sites ("Linked Sites"). The Linked Sites are not under the control of The Indian Performing Right Society Limited and The Indian Performing Right Society Limited is not responsible for the contents of any Linked Site, including without limitation any link contained in a Linked Site, or any changes or updates to a Linked Site. The Indian Performing Right Society Limited is not responsible for webcasting or any other form of transmission received from any Linked Site. The Indian Performing Right Society Limited is providing these links to you only as a convenience, and the inclusion of any link does not imply endorsement by The Indian Performing Right Society Limited of the site or any association with its operators.
                                        </h4>
                                    </div>
                                </div>
                            </div>

                             <%--#endregion information and terms : Commented By Rohit--%>

                            <br />

                             <%--#region Assign Value to DropDownTypeEntity : Commented By Rohit--%>

                            <div class="col-xs-12 col-sm-12 col-lg-12" id="divlang" runat="server" style="padding-left: 15%; padding-right: 15%;">
                                <div class="input-group">
                                    <%--we fill data to dropdown entity througe the database : Commented By Rohit--%>
                                    <span class="input-group-addon" style="font-weight: bold">Type of Entity *</span>
                                    <asp:DropDownList ID="DropDownTypeEntity" runat="server" AutoPostBack="true" CssClass="form-control" AutoCompleteType="Disabled" autocomplete="off" OnSelectedIndexChanged="DropDownTypeEntity_SelectedIndexChanged">
                                        <asp:ListItem Text="Select Type of Entity" Value="Select Type of Entity"></asp:ListItem>

                                        <%-- <asp:ListItem Text="Author/Composer (Individual)" Value="I" Selected="True"></asp:ListItem>
                                        <asp:ListItem Text="Author/Composer (NRI Individual)" Value="NI"></asp:ListItem>
                                        <asp:ListItem Text="Owner Publisher" Value="C"></asp:ListItem>
                                        <asp:ListItem Text="Owner Publisher (NRI)" Value="NC"></asp:ListItem>
                                        <asp:ListItem Text="Self Release" Value="SC"></asp:ListItem>
                                            <asp:ListItem Text="NRI Self Release" Value="NSC"></asp:ListItem>
                                        <asp:ListItem Text="Legal Heir - Existing Deceased Member" Value="LH"></asp:ListItem>
                                        <asp:ListItem Text="Legal Heir - Non member" Value="LHN"></asp:ListItem>--%>
                                    </asp:DropDownList>
                                </div>
                            </div>

                            <%--#endregion Assign Value to DropDownTypeEntity : Commented By Rohit--%>

                            <br />
                            <br />
                            <br />

                            <div style="padding-left: 15%; padding-right: 15%;">

                                <%--#region Legal Heir - Existing Deceased Member : Commented By Rohit--%>

                                <div class="col-lg-12 div-border" runat="server" id="divEntityType" style="min-height: 800px;">

                                    <h2 align="center">
                                        <%-- Here we assign Roletype value to lblType which is selected by Member : Commented By Rohit --%>
                                        <img src="images/individual-icon.png" alt="Individual">
                                        <asp:Label ID="lblType" runat="server" Value=' <%# Eval("RoleType" )%>' />
                                    </h2>

                                    <%--#region Search Legal Heir - Existing Deceased Member : Commented By Rohit--%>

                                    <asp:UpdatePanel ID="UpdatePaneLegelHire" UpdateMode="Conditional" runat="server">
                                        <ContentTemplate>
                                            <asp:HiddenField ID="hdnAccountCode" Value="0" runat="server" />
                                            <div class="row" id="divradio" runat="server">

                                                <%-- Here we call RadioButtonList with Member Name or IPI Number for seraching Existing Member : Commented By Rohit --%>

                                                <div class="col-xs-12 col-sm-12 col-lg-6">
                                                    <asp:RadioButtonList ID="rbtValOption" Enabled="true" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="rbtValOption_SelectedIndexChanged">
                                                        <asp:ListItem class="radio-inline" Selected="True" Text="Member Name" Value="A"></asp:ListItem>
                                                        <asp:ListItem class="radio-inline" Text="IPI Number" Value="I"></asp:ListItem>
                                                    </asp:RadioButtonList>
                                                </div>

                                            </div>
                                            <br />
                                            <div class="row" id="divLegelHireAccount" runat="server" visible="false">

                                                <%-- If Member search by Member Name then txtfirstname and txtlastname are Visible for enter related value and txtIPINumber are Hidden : Commented By Rohit --%>

                                                <div class="col-xs-12 col-sm-12 col-lg-6">
                                                    <div class="input-group">
                                                        <span class="input-group-addon" runat="server">First Name*</span>
                                                        <asp:TextBox ID="txtfirstname" MaxLength="30" runat="server" CssClass="form-control" AutoCompleteType="Disabled" autocomplete="off"
                                                            placeholder="Enter First Name" required />
                                                    </div>
                                                </div>

                                                <div class="col-xs-12 col-sm-12 col-lg-6" id="divDtofEst" runat="server">
                                                    <div class="input-group">
                                                        <span class="input-group-addon" id="spnDate" runat="server">Last Name</span>
                                                        <asp:TextBox ID="txtlastname" MaxLength="45" runat="server" CssClass="form-control" AutoCompleteType="Disabled" autocomplete="off"
                                                            placeholder="Enter Last Name" />
                                                    </div>
                                                </div>
                                            </div>

                                            <%-- If Member search by IPI Number then txtIPINumber are Visible for enter related value and txtfirstname and txtlastname are Hidden : Commented By Rohit --%>
                                            <div class="row" id="divLegelHireIPINumber" runat="server" visible="false">
                                                <div class="col-xs-12 col-sm-12 col-lg-6" id="div1" runat="server">
                                                    <div class="input-group">
                                                        <span class="input-group-addon" id="Span1" runat="server">IPI Number*</span>
                                                        <asp:TextBox ID="txtIPINumber" MaxLength="100" runat="server" CssClass="form-control" AutoCompleteType="Disabled" autocomplete="off"
                                                            placeholder="Enter IPI Number" required />
                                                    </div>
                                                </div>
                                            </div>

                                             <%-- When Member Select Legal Heir - Existing Deceased Member in DropDownTypeEntity the btnValidate are Visible : Commented By Rohit --%>

                                            <div class="row" id="divvalidate" runat="server">
                                                <div class="col-xs-12 col-sm-12 col-lg-12" runat="server" style="padding-left: 50%; padding-right: 50%;">
                                                    <div class="input-group">
                                                        <p>
                                                            <asp:Button ID="btnValidate" runat="server" Text="Validate" ToolTip="Validate" CssClass="button button-blue" OnClick="btnValidate_Click" AutoPostBack="true" />

                                                        </p>
                                                    </div>
                                                </div>
                                            </div>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>

                                    <%--#endregion Search Legal Heir - Existing Deceased Member : Commented By Rohit--%>

                                    <%--#region Display Basic Deatils Of Legal Heir - Existing Deceased Member : Commented By Rohit--%>

                                    <div class="row" id="divLegDetail" runat="server" visible="false">
                                        <div class="fieldset panel">
                                            <div class="fieldset-body panel-body">
                                                <h5 class="legend-Panel"><strong class="text-uppercase"><span id="SectionTitle" runat="server">Deceased Member Details</span> </strong></h5>
                                                <div class="row" id="divLegelHirePre" runat="server">
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
                                                            <asp:TextBox ID="txtrelationship" MaxLength="100" runat="server" class="form-control" Style="width: 98%" AutoCompleteType="Disabled" autocomplete="off">
                                                            </asp:TextBox>
                                                        </div>
                                                    </div>
                                                </div>

                                                <div class="row" id="divBirthInfo" runat="server">
                                                    <div class="col-xs-12 col-sm-12 col-lg-6">
                                                        <div class="input-group">
                                                            <span class="input-group-addon">Date of Birth
                                                            </span>
                                                            <asp:TextBox ID="txtDOB" runat="server" required class="form-control" Style="width: 98%" AutoCompleteType="Disabled" autocomplete="off" placeholder="dd-MMM-yyyy" ToolTip="Format : [ddmmyyyy], [d] or [dd], [ddm] or [ddmm], [ ].&#xa;Keys : Day-[Up/Down], Month-[alt+Up/Down], Year-[shift+Up/Down], Today-[Home].">
                                                            </asp:TextBox>
                                                            <%--<asp:CalendarExtender ID="calDOB" OnClientDateSelectionChanged="dateSelectionChanged" CssClass="zcCalendar" runat="server" TargetControlID="txtDOB" Format="dd-MMM-yyyy"></asp:CalendarExtender> --%>
                                                        </div>
                                                    </div>
                                                    <div class="col-xs-12 col-sm-12 col-lg-6">
                                                        <div class="input-group">
                                                            <span class="input-group-addon">Date of death
                                                            </span>
                                                            <asp:TextBox ID="txtdateofdeath" runat="server" required class="form-control" Style="width: 98%" AutoCompleteType="Disabled" autocomplete="off" placeholder="dd-MMM-yyyy" ToolTip="Format : [ddmmyyyy], [d] or [dd], [ddm] or [ddmm], [ ].&#xa;Keys : Day-[Up/Down], Month-[alt+Up/Down], Year-[shift+Up/Down], Today-[Home].">
                                                            </asp:TextBox>
                                                            <%--<asp:CalendarExtender ID="calDOB1" OnClientDateSelectionChanged="dateSelectionChanged" CssClass="zcCalendar" runat="server" TargetControlID="txtdateofdeath" Format="dd-MMM-yyyy"></asp:CalendarExtender>                                                                                --%>
                                                        </div>
                                                    </div>
                                                </div>

                                                <div class="row" id="divcertficate" runat="server">
                                                    <div class="col-xs-12 col-sm-12 col-lg-6">
                                                        <div class="input-group">
                                                            <span class="input-group-addon">Death Certficate Number
                                                            </span>
                                                            <asp:TextBox ID="txtdatecertNumber" placeholder="Enter Death Certificate Number." runat="server" MaxLength="100" class="form-control" Style="width: 98%" AutoCompleteType="Disabled" autocomplete="off">
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

                                                <div class="row" runat="server" id="divaddress">
                                                </div>
                                                <div class="row">
                                                    <div class="col-xs-12 col-sm-6 col-lg-6" id="divPerPincode" runat="server">
                                                        <div class="input-group">
                                                            <span class="input-group-addon">Pincode</span>
                                                            <asp:TextBox ID="txtpincode" runat="server" class="form-control" Style="width: 98%" AutoCompleteType="Disabled" autocomplete="off">
                                                            </asp:TextBox>
                                                        </div>
                                                    </div>

                                                    <div class="col-xs-12 col-sm-6 col-lg-6" id="divgeopre" runat="server">
                                                        <div class="input-group">
                                                            <span class="input-group-addon">Geographical</span>
                                                            <asp:TextBox ID="txtgeo" runat="server" class="form-control" Style="width: 98%" AutoCompleteType="Disabled" autocomplete="off">
                                                            </asp:TextBox>
                                                        </div>
                                                    </div>
                                                </div>


                                                <%--added by Hariom 25-02-2023--%>
                                                <div class="col-xs-12 col-sm-12 col-lg-6" id="divipino" runat="server">
                                                    <%--<div class="col-xs-12 col-sm-3 col-lg-8">--%>
                                                    <div class="input-group">
                                                        <span class="input-group-addon">IPI Number
                                                        </span>
                                                        <asp:TextBox ID="txtIPINo" placeholder="IPI Number" runat="server" MaxLength="100" class="form-control" Style="width: 98%" AutoCompleteType="Disabled" autocomplete="off">
                                                        </asp:TextBox>

                                                    </div>
                                                </div>

                                                <%--end--%>


                                                <div class="row">

                                                    <div class="col-xs-12 col-sm-3 col-lg-3">
                                                        <div class="input-group">
                                                            <span class="input-group-addon" runat="server" id="spnUpload">DECEASED MEMBER Photo
                                                            </span>
                                                        </div>
                                                    </div>




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



                                                </div>

                                                <br />


                                            </div>



                                        </div>
                                    </div>

                                    <%--#endregion Display Basic Deatils Of Legal Heir - Existing Deceased Member : Commented By Rohit--%>

                                    <%--#region Datalist For Roly Type and assgin value to the hdnRoleType and hdnRoleTypeId : Commented By Rohit--%>

                                    <div class="min-height">
                                        <asp:DataList ID="DLRoleType_I" runat="server" Width="100%" BorderStyle="None" CellSpacing="0" CellPadding="0" CssClass="normal-table" RepeatColumns="1">
                                            <HeaderTemplate>
                                                <tr>
                                                    <th>Role Type</th>
                                                </tr>
                                            </HeaderTemplate>

                                            <ItemTemplate>
                                                <tr>
                                                    <td>
                                                        <asp:CheckBox ID="cbxSelect" OnCheckedChanged="cbxSelect_I_CheckedChanged" Enabled='<%# Eval("RoleType" ).ToString().Contains("P")?false:true%>' AutoPostBack="true" runat="server" />
                                                        <%# Eval("MemberRoleType")%>
                                                        <asp:HiddenField ID="hdnRoleType" runat="server" Value=' <%# Eval("RoleType" )%>' />
                                                        <asp:HiddenField ID="hdnRoleTypeId" runat="server" Value=' <%# Eval("MemberRoleTypeId" )%>' />
                                                    </td>
                                                </tr>
                                            </ItemTemplate>
                                        </asp:DataList>
                                    </div>
                                    
                                    <%--#endregion Datalist For Roly Type and assgin value to the hdnRoleType and hdnRoleTypeId : Commented By Rohit--%>

                                    <h5>Requirements</h5>

                                    <%--#region attched document list to DLDocuments_I : Commented By Rohit--%>

                                    <div class="row">
                                        <div class="col-lg-12 col-xs-12">
                                            <asp:Repeater ID="DLDocuments_I" runat="server">
                                                <HeaderTemplate>
                                                    <ul class="customlist tick-list">
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <li><%#Eval("DocumentName")%></li>

                                                </ItemTemplate>
                                                <%-- <FooterTemplate>
                                                    <li style="color: red; background: none">More ..</li>
                                                    </ul>
                                                </FooterTemplate>--%>
                                            </asp:Repeater>
                                        </div>
                                    </div>

                                    <%--#endregion attched document list to DLDocuments_I : Commented By Rohit--%>

                                    <%--#region Calculated Membership Fee Value assgin to lblfees_I ,Click on cbx_I and Finally Click On btnSubmit_I_Click to next procced : Commented By Rohit--%>

                                    <div class="fee-text">
                                        <h3 class="membership-price">
                                            <img src="images/rs-symbol.png" style="vertical-align: middle">
                                            <asp:Label ID="lblfees_I" runat="server"></asp:Label>
                                        </h3>
                                        <h4 class="blue">APPLICATION FEE </h4>
                                        <h6><strong>Non Refundable</strong> </h6>

                                        <h6><u><strong>Consent for collection and processing of your personal/sensitive personal data</strong></u></h6>
                                        <h6 class="justify">
                                            <p>
                                                If you agree to our collection and processing of your personal/sensitive personal data in accordance with the terms laid out in our <a href="AppDocs\Privacy_Notice.pdf" target="_blank"><u>Privacy Notice</u></a>, please indicate your consent by clicking the ‘I Accept’ button. By clicking this button, you confirm that you have read, understood and consent to the Privacy Notice. You understand that without providing your personal data as required under the Privacy Notice, you will not be able to use/access the IPRS Membership Portal.
                                            </p>
                                            <h6></h6>
                                            <p class="red">
                                                <asp:CheckBox ID="cbx_I" runat="server" />
                                                I Accept
                                            </p>
                                            <p>
                                                <asp:Button ID="btnSubmit_I" runat="server" CssClass="button button-blue" OnClick="btnSubmit_I_Click" Text="Register" />
                                            </p>

                                        </h6>

                                    </div>

                                     <%--#endregion Calculated Membership Fee Value assgin to lblfees_I ,Click on cbx_I and Finally Click On btnSubmit_I_Click to next procced : Commented By Rohit--%>

                                </div>

                                <%--#endregion Legal Heir - Existing Deceased Member : Commented By Rohit--%>

                            </div>

                        </div>

                    </div>
                </div>

            </ContentTemplate>
        </asp:UpdatePanel>

    </form>
</body>
</html>
