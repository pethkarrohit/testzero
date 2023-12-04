<%@ Page Title="" Language="C#" MasterPageFile="~/ApplicationMember.Master" AutoEventWireup="true" CodeBehind="UpdateAddressDetails.aspx.cs" Inherits="IPRS_Member.UpdateAddressDetails" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
    <%@ Register Src="~/User_Controls/ucTooltip.ascx" TagPrefix="ucTooltip" TagName="ucTooltip" %>
    <%@ Register Src="~/User_Controls/ucDropDown.ascx" TagPrefix="ucDropDown" TagName="ucDropDown" %>
    <%@ Register Src="~/User_Controls/UCDocUpload.ascx" TagPrefix="ucDropDown" TagName="UCDocUpload" %>

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
                                <div class="col-xs-12 col-sm-12 col-lg-12">
                                    <h4>
                                        <asp:Label ID="lblFormTitle" runat="server">Update Address</asp:Label></h4>
                                </div>

                                <div class="clearfix"></div>
                            </div>
                            <div id="wizard" class="form_wizard wizard_horizontal">

                                <div class="col-xs-12 col-sm-12 col-lg-12">
                                    <div class="row">
                                        <br />
                                        <div class="col-xs-12 col-sm-12 col-lg-12"><span style="font-weight: bold; color: #ff0000;">Comments From Back Office : - </span><span id="spComments" runat="server"></span></div>
                                        <br />
                                        <br />
                                    </div>
                                    <div class="row">
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
                                        <div class="col-xs-12 col-sm-12 col-lg-6" id="divAltMobile" runat="server">
                                            <div class="input-group">

                                                <span class="input-group-addon">Alternate Mobile
                                                </span>
                                                <asp:TextBox ID="txtAlternateMobile" MaxLength="10" runat="server" CssClass="form-control" AutoCompleteType="Disabled" autocomplete="off" min="0" TextMode="Number"
                                                    placeholder="Enter Alternate Mobile" />
                                                <asp:HiddenField ID="hdnAccountUpdateField" runat="server" />

                                                <asp:HiddenField ID="hdnregType" runat="server" Value="" />
                                                <asp:HiddenField ID="hdnReUpdateInfo" runat="server" Value="" />

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
                                    </div>

                                    <div class="row">

                                        <div class="fieldset panel">
                                            <div class="fieldset-body panel-body">

                                                <div class="row">
                                                    <div class="address">
                                                        <h4>
                                                            <span id="SpPrmnAddressType" runat="server">Permanent Address</span></h4>
                                                        <div class="clearfix"></div>
                                                    </div>
                                                    <div class="col-xs-12 col-sm-12 col-lg-12">
                                                        <div class="input-group">
                                                            <span class="input-group-addon">Address*
                                                            </span>
                                                            <textarea id="txtAddress_PM" maxlength="255" runat="server" class="form-control" autocompletetype="Disabled" autocomplete="off"
                                                                placeholder="Enter Permanent Address" rows="3" cols="20" />
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="row">
                                                    <div class="col-xs-12 col-sm-12 col-lg-6">
                                                        <div class="input-group">
                                                            <span class="input-group-addon">Pincode*</span>
                                                            <ucDropDown:ucDropDown runat="server" ID="ddlPincode_PM" MinimumPrefixLength="2" blnChangeEvent="true" strMessage="Pincode" />
                                                            <a id="helpTooltip2" title="Minimum 2 Letter">
                                                                <img src="Images/help-icon.png" style="position: absolute; right: 3px; z-index: 100; cursor: pointer; top: 5px">
                                                            </a>

                                                        </div>
                                                    </div>
                                                    <div class="col-xs-12 col-sm-12 col-lg-6">
                                                        <div class="input-group">
                                                            <span class="input-group-addon">Geographical*</span>
                                                            <ucDropDown:ucDropDown runat="server" ID="ddlGeo_PM" blEnabled="false" MinimumPrefixLength="2" strMessage="City" />
                                                            <%--   <ucTooltip:ucTooltip runat="server" tooltipanchor="Minimum 2 Letter" ID="ucTooltip4" />--%>
                                                        </div>
                                                    </div>

                                                </div>
                                                <div class="row">
                                                    <div class="col-xs-10 col-sm-10 col-lg-10">
                                                        <div class="input-group">

                                                            <span class="input-group-addon" id="SpAddressYesNo" runat="server">Is Present address same as the Permanent address ?</span>

                                                            <asp:RadioButtonList ID="rbtnlPDCheckAddress" runat="server" OnSelectedIndexChanged="rbtnlPDCheckAddress_SelectedIndexChanged" CssClass="form-control" RepeatDirection="Horizontal" RepeatLayout="Flow" AutoPostBack="true">
                                                                <asp:ListItem Selected="True" class="radio-inline" Text="Yes" Value="Y"></asp:ListItem>
                                                                <asp:ListItem Text="No" class="radio-inline" Value="N"></asp:ListItem>
                                                            </asp:RadioButtonList>
                                                        </div>
                                                    </div>
                                                    <div class="col-xs-2 col-sm-2 col-lg-2">
                                                        <div class="input-group">
                                                            <asp:LinkButton ID="lnkRefresh" runat="server" CssClass="btn btn-danger" OnClick="lnkRefresh_Click"><span class="fa fa-refresh"></span></asp:LinkButton>

                                                        </div>
                                                    </div>
                                                </div>

                                                <asp:Panel ID="pnlPDPresentAddress" runat="server" Visible="false">
                                                    <div class="row">
                                                        <div class="address">
                                                            <h4>
                                                                <asp:Label ID="Label3" runat="server">Present Address</asp:Label></h4>
                                                            <div class="clearfix"></div>
                                                        </div>
                                                        <div class="col-xs-12 col-sm-12 col-lg-12">
                                                            <div class="input-group">
                                                                <span class="input-group-addon">Address
                                                                </span>
                                                                <textarea id="txtAddress_PR" maxlength="255" runat="server" class="form-control" autocompletetype="Disabled" autocomplete="off"
                                                                    placeholder="Enter Present Address" textmode="MultiLine" rows="3" cols="20" />
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <div class="row">
                                                        <div class="col-xs-12 col-sm-12 col-lg-6">
                                                            <div class="input-group">
                                                                <span class="input-group-addon">Pincode*</span>
                                                                <ucDropDown:ucDropDown runat="server" ID="ddlPincode_PR" MinimumPrefixLength="2" blnChangeEvent="true" strMessage="Pincode" />
                                                                <a id="helpTooltip3" title="Minimum 2 Letter">
                                                                    <img src="Images/help-icon.png" style="position: absolute; right: 3px; z-index: 100; cursor: pointer; top: 5px">
                                                                </a>
                                                                <%--<asp:TextBox ID="txtPincode_PR" MaxLength="6" runat="server" CssClass="form-control" AutoCompleteType="Disabled" autocomplete="off" pattern="^([0-9]{6,6})$"
                                                                                        placeholder="Enter Pincode" />
                                                                                    <ajaxToolkit:FilteredTextBoxExtender ID="FilteredTextBoxExtender1" runat="server" TargetControlID="txtPincode_PR"
                                                                                        FilterType="Custom" ValidChars="1234567890"></ajaxToolkit:FilteredTextBoxExtender>--%>
                                                            </div>
                                                        </div>

                                                        <div class="col-xs-12 col-sm-12 col-lg-6">
                                                            <div class="input-group">
                                                                <span class="input-group-addon">Geographical*</span>
                                                                <ucDropDown:ucDropDown runat="server" ID="ddlGeo_PR" MinimumPrefixLength="2" blEnabled="false" strmessage="Country / State / City" />
                                                                <ucTooltip:ucTooltip runat="server" tooltipanchor="Minimum 2 Letter" ID="ucTooltip5" />
                                                            </div>
                                                        </div>

                                                    </div>

                                                </asp:Panel>
                                                <div class="row">

                                                    <ucDropDown:UCDocUpload runat="server" ID="UCDocUpload" />

                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-xs-12 col-sm-12 col-lg-12" style="text-align: right">
                                            <br />
                                            <asp:Button ID="btnUpdateAddress" runat="server" OnClick="btnUpdateAddress_Click" Text="SAVE" CssClass="btn btn-primary" />
                                        </div>
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
