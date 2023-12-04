<%@ Page Title="" Language="C#" MasterPageFile="~/ApplicationMember.Master" AutoEventWireup="true" CodeBehind="UpdateBankInfo.aspx.cs" Inherits="IPRS_Member.UpdateBankInfo" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
    <%@ Register Src="~/User_Controls/ucTooltip.ascx" TagPrefix="ucTooltip" TagName="ucTooltip" %>
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
                                <h4>
                                    <asp:Label ID="lblFormTitle" runat="server">Update Bank Information</asp:Label></h4>
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
                                    <div class="col-xs-12 col-sm-12 col-lg-6">
                                        <div class="input-group">
                                            <span class="input-group-addon">A/C Holder Name.</span>
                                            <asp:TextBox ID="txtAccountHoldername" MaxLength="50" runat="server" CssClass="form-control" AutoCompleteType="Disabled" autocomplete="off" ReadOnly
                                                placeholder="Enter Account Holder Name." required />
                                            <asp:HiddenField ID="hdnAccountUpdateField" runat="server" />
                                            <asp:HiddenField ID="hdnregType" runat="server" />
                                            <asp:HiddenField ID="hdnReUpdateInfo" runat="server" />

                                        </div>
                                    </div>
                                    <div class="col-xs-12 col-sm-12 col-lg-6">
                                        <div class="input-group">
                                            <span class="input-group-addon">Bank Name*
                                            </span>
                                            <asp:TextBox ID="txtBankName" MaxLength="100" runat="server" CssClass="form-control" AutoCompleteType="Disabled" autocomplete="off"
                                                placeholder="Enter Bank Name" required />
                                        </div>
                                    </div>
                                    <div class="col-xs-12 col-sm-12 col-lg-6">
                                        <div class="input-group">
                                            <span class="input-group-addon">Account No.*</span>
                                            <asp:TextBox ID="txtBankAcNo" MaxLength="15" runat="server" CssClass="form-control" AutoCompleteType="Disabled" autocomplete="off"
                                                placeholder="Enter Account No." required />
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
                                            <span class="input-group-addon">IFSC Code*</span>
                                            <asp:TextBox ID="txtIFSC" MaxLength="15" runat="server" CssClass="form-control" AutoCompleteType="Disabled" min="0" autocomplete="off"
                                                placeholder="Enter IFSC Code" required pattern="[a-zA-Z0-9]+" />
                                            <ajaxToolkit:FilteredTextBoxExtender ID="filterIfscCode" runat="server" TargetControlID="txtIFSC"
                                                FilterType="Numbers,UppercaseLetters,LowercaseLetters"></ajaxToolkit:FilteredTextBoxExtender>


                                        </div>
                                    </div>
                                    <div class="col-xs-12 col-sm-12 col-lg-6">
                                        <div class="input-group">
                                            <span class="input-group-addon">MICR Code*</span>
                                            <asp:TextBox ID="txtMICR" MaxLength="15" runat="server" CssClass="form-control" AutoCompleteType="Disabled" autocomplete="off" TextMode="Number"
                                                placeholder="Enter MICR Code" required />
                                            <ucTooltip:ucTooltip tooltipanchor="Click to see example" imgsrc="Images/cheque-locate-ifsc-micr-code.png" runat="server" ID="ucTooltip" />
                                        </div>
                                    </div>
                                </div>
                                <div class="row">
                                    <ucDropDown:UCDocUpload runat="server" ID="UCDocUpload" DocIds="3" />

                                </div>
                                <div class="row">
                                    <div class="col-xs-12 col-sm-12 col-lg-12" style="text-align: right">
                                        <br />
                                        <asp:Button ID="btnBankInfo" runat="server" OnClick="btnBankInfo_Click" Text="SAVE" CssClass="btn btn-primary" />
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
