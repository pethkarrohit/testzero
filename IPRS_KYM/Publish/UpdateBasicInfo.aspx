<%@ Page Title="" Language="C#" MasterPageFile="~/ApplicationMember.Master" AutoEventWireup="true" CodeBehind="UpdateBasicInfo.aspx.cs" Inherits="IPRS_Member.UpdateBasicInfo" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
    <%@ Register Src="~/User_Controls/ucTooltip.ascx" TagPrefix="ucTooltip" TagName="ucTooltip" %>
    <%@ Register Src="~/User_Controls/UCDocUpload.ascx" TagPrefix="ucDropDown" TagName="UCDocUpload" %>
    <script src="Javascript/FCalender.js"></script>
    <link href="Style/zcCalendar.css" rel="stylesheet" />
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
                            <div class="x_title">
                                <h4>
                                    <asp:Label ID="lblFormTitle" runat="server">Basic Information</asp:Label></h4>
                                <div class="clearfix"></div>
                            </div>
                            <div id="wizard" class="form_wizard wizard_horizontal">
                                <div class="row">
                                    <br />
                                    <div class="col-xs-12 col-sm-12 col-lg-12"><span style="font-weight: bold; color: #ff0000;">Comments From Back Office : - </span><span id="spComments" runat="server"></span></div>
                                    <br />
                                    <br />
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
                                                        <asp:HiddenField ID="hdnAccountId" runat="server" Value="" />
                                                        <asp:HiddenField ID="hdnAccountUpdateField" runat="server" Value="" />
                                                        <asp:HiddenField ID="hdnRegistrationType" runat="server" Value="" />
                                                        <asp:HiddenField ID="hdnContactId" Value="0" runat="server" />
                                                        <asp:HiddenField ID="hdnReUpdateInfo" runat="server" />
                                                    </div>
                                                </div>


                                                <div class="col-xs-12 col-sm-12 col-lg-4">
                                                    <div class="input-group">
                                                        <span class="input-group-addon" id="spnDate" runat="server">Date of Establishment*</span>
                                                        <asp:TextBox ID="txtDateofEstablishment" runat="server" required CssClass="form-control" AutoCompleteType="Disabled" autocomplete="off" placeholder="dd-MMM-yyyy" onBlur="GetDateField(this,false);" onFocus="SetDateField(this);" ToolTip="Format : [ddmmyyyy], [d] or [dd], [ddm] or [ddmm], [ ].&#xa;Keys : Day-[Up/Down], Month-[alt+Up/Down], Year-[shift+Up/Down], Today-[Home]." />
                                                        <asp:CalendarExtender ID="CalendarExtender1" CssClass="zcCalendar" runat="server" TargetControlID="txtDateofEstablishment" Format="dd-MMM-yyyy"></asp:CalendarExtender>
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-12 col-lg-12">

                                                    <div class="input-group">
                                                        <span class="input-group-addon" runat="server">Entity Type</span>
                                                        <asp:RadioButtonList ID="rbtEntityType" RepeatDirection="Horizontal" RepeatColumns="4" CssClass="form-control" runat="server">
                                                            <asp:ListItem Text="Sole Proprietary Concern" Selected="True" Value="SP"></asp:ListItem>
                                                            <asp:ListItem Text="Partnership" Value="PR"></asp:ListItem>
                                                            <asp:ListItem Text="Corporate (Pvt Ltd/Ltd Company)" Value="CP"></asp:ListItem>
                                                        </asp:RadioButtonList>
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
                                                <div class="col-xs-12 col-sm-12 col-lg-4">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">First Name*</span>
                                                        <asp:TextBox ID="txtFname" MaxLength="35" runat="server" CssClass="form-control" AutoCompleteType="Disabled" autocomplete="off"
                                                            placeholder="Enter First Name" required />
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-12 col-lg-4">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">Middle Name</span>
                                                        <asp:TextBox ID="txtMname" MaxLength="35" runat="server" CssClass="form-control" AutoCompleteType="Disabled" autocomplete="off"
                                                            placeholder="Enter Middle Name" />
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-12 col-lg-4">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">Last Name*</span>
                                                        <asp:TextBox ID="txtLname" MaxLength="30" runat="server" CssClass="form-control" AutoCompleteType="Disabled" autocomplete="off"
                                                            placeholder="Enter Last Name" required />
                                                    </div>
                                                </div>
                                            </div>


                                            <div class="row">

                                                <div class="col-xs-12 col-sm-12 col-lg-6" runat="server" visible="false" id="divDesignation">
                                                    <div class="input-group" runat="server">
                                                        <span class="input-group-addon" id="spnDegination" runat="server">Designation</span>
                                                        <asp:TextBox ID="txtDesignation" MaxLength="25" runat="server" CssClass="form-control" AutoCompleteType="Disabled" autocomplete="off"
                                                            placeholder="Enter Designation" />
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-12 col-lg-6">
                                                    <div class="input-group">
                                                        <span class="input-group-addon" id="spnAlias" runat="server">Alias</span>
                                                        <asp:TextBox ID="txtAccountAlias" MaxLength="100" title="Alias Name / Trade Name / Credit Name" runat="server" CssClass="form-control" AutoCompleteType="Disabled" autocomplete="off"
                                                            placeholder="Enter Alias/ Trader Name" />
                                                    </div>
                                                </div>

                                            </div>


                                            <div class="row">
                                                <div class="col-xs-12 col-sm-12 col-lg-6" id="divAadhar" runat="server" visible="false">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">Aadhar No.
                                                        </span>
                                                        <asp:TextBox ID="txtDetails3_Aadh" MaxLength="12" runat="server" CssClass="form-control" AutoCompleteType="Disabled" autocomplete="off"
                                                            placeholder="Enter Aadhar No" pattern="^([0-9]{12,})$" title="Aadhar No. 12 numbers minimum" />
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
                                                            placeholder="Enter Mobile" required pattern="^([0-9]{10,})$" title="Mobile No. 10 numbers minimum" Width="70%" />
                                                        <ajaxToolkit:FilteredTextBoxExtender ID="fittxtMobile" runat="server" TargetControlID="txtMobile"
                                                            FilterType="Custom" ValidChars="1234567890"></ajaxToolkit:FilteredTextBoxExtender>
                                                        <ucTooltip:ucTooltip runat="server" tooltipanchor="Eg : 0123456789" ID="ucTooltip7" />
                                                    </div>
                                                </div>

                                            </div>
                                            <div class="row">
                                                <div class="col-xs-12 col-sm-12 col-lg-6">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">Telephone
                                                        </span>

                                                        <asp:TextBox ID="txtTelephone" runat="server" CssClass="form-control" AutoCompleteType="Disabled" autocomplete="off" minlength="8" MaxLength="50"
                                                            placeholder="Enter TelePhone" pattern="[^a-zA-z]+" title="Telephone Number Character in not Accept" />
                                                        <ajaxToolkit:FilteredTextBoxExtender ID="filterPhoneno" runat="server" TargetControlID="txtTelephone"
                                                            FilterType="Numbers,Custom" ValidChars=".,()/-"></ajaxToolkit:FilteredTextBoxExtender>
                                                        <ucTooltip:ucTooltip runat="server" tooltipanchor="Eg : 02228698745" ID="ucTooltip8" />

                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-12 col-lg-6">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">Pan No.*</span>
                                                        <asp:TextBox ID="txtDetails2_Pan" MaxLength="10" runat="server" pattern="^[a-zA-Z]{5}[0-9]{4}[a-zA-Z]{1}$" required class="form-control" AutoCompleteType="Disabled" autocomplete="off"
                                                            placeholder="Enter Pan No." title="Pan No Format- CEQPK4956K" />
                                                        <ajaxToolkit:FilteredTextBoxExtender ID="filterpancard" runat="server" TargetControlID="txtDetails2_Pan"
                                                            FilterType="Numbers,UppercaseLetters,LowercaseLetters"></ajaxToolkit:FilteredTextBoxExtender>
                                                        <ucTooltip:ucTooltip runat="server" tooltipanchor="Eg : CEQPK4956K" ID="ucTooltip9" />

                                                    </div>
                                                </div>

                                            </div>



                                            <div class="row">


                                                <div class="col-xs-12 col-sm-12 col-lg-6">
                                                    <div class="input-group">
                                                        <span class="input-group-addon">Upload Image
                                                        </span>
                                                        <asp:FileUpload ID="FPuploadPhoto" Width="100%" CssClass="form-control" runat="server" AllowMultiple="false" accept="image/gif, image/jpeg, image/png" />
                                                    </div>
                                                </div>
                                                <div class="col-xs-12 col-sm-4 col-lg-6">
                                                    <div class="btn-group">
                                                        <asp:Button ID="btnPhotoUpload" runat="server" OnClick="btnPhotoUpload_Click" OnClientClick="return PhotoValidate();" Text="Upload Image" CssClass="btn btn-primary" />
                                                        <asp:HyperLink ID="btnphotoView" runat="server" Target="_blank" CssClass="btn btn-primary">View</asp:HyperLink>
                                                        <asp:HiddenField ID="hdnphotoImageName" runat="server" />

                                                    </div>

                                                </div>
                                            </div>

                                        </div>


                                    </div>
                                </div>
                                <div class="row">
                                    <ucDropDown:UCDocUpload runat="server" ID="UCDocUpload" />

                                </div>
                                <div class="row">
                                    <div class="col-xs-12 col-sm-12 col-lg-12" style="text-align: right">
                                        <br />
                                        <asp:Button ID="btnSave" runat="server" OnClick="btnSave_Click" Text="SAVE" CssClass="btn btn-primary" />
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
