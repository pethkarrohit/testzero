<%@ Page Title="" Language="C#" MasterPageFile="~/ApplicationMember.Master" AutoEventWireup="true" CodeBehind="UpdateGSTDetails.aspx.cs" Inherits="IPRS_Member.UpdateGSTDetails" %>

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
                                    <asp:Label ID="lblFormTitle" runat="server">Update GST Information</asp:Label></h4>
                                <div class="clearfix"></div>
                            </div>
                            <div id="wizard" class="form_wizard wizard_horizontal">
                                <div class="row">
                                    <br />
                                    <div class="col-xs-12 col-sm-12 col-lg-12"><span style="font-weight: bold;color:#ff0000;">Comments From Back Office : - </span><span id="spComments" runat="server"></span></div>
                                     <br />  <br />
                                </div>
                                <div class="row">
                                    <div class="col-xs-12 col-sm-12 col-lg-4">
                                        <div class="input-group">
                                            <span class="input-group-addon">GST</span>
                                            <asp:RadioButtonList ID="rbtGstApl" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="rbtGstApl_SelectedIndexChanged">
                                                <asp:ListItem class="radio-inline" Selected="True" Text="Applicable" Value="AP"></asp:ListItem>
                                                <asp:ListItem class="radio-inline" Text="Not Applicable" Value="NAP"></asp:ListItem>
                                            </asp:RadioButtonList>

                                        </div>
                                    </div>
                                    <div class="col-xs-12 col-sm-12 col-lg-6" id="divGst" runat="server" visible="false">
                                        <div class="input-group">
                                            <span class="input-group-addon">GST No.</span>
                                            <asp:TextBox ID="txtDetails1_gst" runat="server" pattern="^([a-zA-Z0-9]{15,15})$" title="eg:27AASCS2460H1Z0" class="form-control" required AutoCompleteType="Disabled" autocomplete="off" minlength="15" MaxLength="15"
                                                placeholder="Enter GST" />
                                            <ucTooltip:ucTooltip runat="server" tooltipanchor="eg : 27AASCS2460H1Z0" ID="ucTooltip2" />
                                            <asp:HiddenField ID="hdnAccountUpdateField" runat="server" />
                                            <asp:HiddenField ID="hdnRegType" runat="server" />
                                            <asp:HiddenField ID="hdnAccountId" runat="server" />
                                            <asp:HiddenField ID="hdnReUpdateInfo" runat="server" />
                                        </div>
                                    </div>
                                    <div class="col-xs-12 col-sm-12 col-lg-2">
                                        <div class="input-group">
                                            <asp:LinkButton ID="lnkRefresh" runat="server" CssClass="btn btn-danger" OnClick="lnkRefresh_Click"><span class="fa fa-refresh"></span></asp:LinkButton>

                                        </div>
                                    </div>
                                    <div class="row">
                                        <ucDropDown:UCDocUpload runat="server" ID="UCDocUpload" />

                                    </div>
                                    <div class="row">
                                        <div class="col-xs-12 col-sm-12 col-lg-12" style="text-align: right">
                                            <br />
                                            <asp:Button ID="btnGSTInfo" runat="server" OnClick="btnGSTInfo_Click" Text="SAVE" CssClass="btn btn-primary" />
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
