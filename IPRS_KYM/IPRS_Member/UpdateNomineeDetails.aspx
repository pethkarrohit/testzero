<%@ Page Title="" Language="C#" MasterPageFile="~/ApplicationMember.Master" AutoEventWireup="true" CodeBehind="UpdateNomineeDetails.aspx.cs" Inherits="IPRS_Member.UpdateNomineeDetails" EnableEventValidation="false" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Src="~/User_Controls/ucDropDown.ascx" TagPrefix="ucDropDown" TagName="ucDropDown" %>
<%@ Register Src="~/User_Controls/ucTooltip.ascx" TagPrefix="ucTooltip" TagName="ucTooltip" %>
<%@ Register Src="~/User_Controls/ucNomineeDetails.ascx" TagPrefix="ucNomineeDetails" TagName="ucNomineeDetails" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" />

      
    <asp:UpdatePanel ID="UpdtPnl" UpdateMode="Always" runat="server">
        <ContentTemplate>                                                    
            <div>                                                         
                    <ucNomineeDetails:ucNomineeDetails runat="server" id="ucNomineeDetails" />
            </div>
        </ContentTemplate>
        <Triggers>
        </Triggers>
    </asp:UpdatePanel>
      

</asp:Content>
