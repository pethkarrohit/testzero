<%@ Page Title="" Language="C#" MasterPageFile="~/ApplicationMember.Master" AutoEventWireup="true" CodeBehind="Information.aspx.cs" Inherits="IPRS_Member.Information" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="row" style="height: 15vh">
    </div>
    <div class="row" id="divUnderProcess_Reg" runat="server">

        <div class="col-xs-12 col-sm-12 col-lg-12">
            <h4 style="text-align: center">Thank you for completing the  application and providing all the details.</h4>
        </div>
        <div class="col-xs-12 col-sm-12 col-lg-12">
            <h4 style="text-align: center">We will review your application and get back to you with our decision.<br />
                In the meanwhile we have sent you an email with  a PDF of your Profile details.</h4>
        </div>
        <div class="col-xs-12 col-sm-12 col-lg-12" style="text-align: center">
            <span style="font-size: medium;"><b>Note:</b> Your Application is not yet approved. It is Under Approval Process.
            </span>
        </div>
        <div class="col-xs-12 col-sm-12 col-lg-12 " style="text-align: center">
            <asp:HyperLink NavigateUrl="MemberLogin" Style="font-size: medium; text-align: center; font-weight: bolder" Text="Click Here For Home" CssClass="btn btn-link" runat="server" />
        </div>
    </div>
    <div class="row" id="DivRejection" runat="server">


        <div class="col-xs-12 col-sm-12 col-lg-12">
            <h4 style="text-align: center">Your Application Is Rejected<br />
                Please Contact Administrator</h4>
        </div>


    </div>
    <div class="row" id="divUpdate_Details" runat="server">

        <div class="col-xs-12 col-sm-12 col-lg-12">
            <h4 style="text-align: center">Thank you for Updating Your Details.</h4>
        </div>
        <br />

        <div class="col-xs-12 col-sm-12 col-lg-12" style="text-align: center">
            <span style="font-size: medium;">Your Updated Data is not yet approved. It is Under Approval Process.
                You will recieve an email regarding the updates of approval
            </span>
        </div>
        <div class="col-xs-12 col-sm-12 col-lg-12">
            <h4 style="text-align: center">We will review the details provided and get back to you .<br />
            </h4>
        </div>
        <br />
        <div class="col-xs-12 col-sm-12 col-lg-12 " style="text-align: center">
            <asp:HyperLink NavigateUrl="Home" Style="font-size: medium; text-align: center; font-weight: bolder" Text="Click Here For Home" CssClass="btn btn-link" runat="server" />
        </div>
    </div>
    <div class="row" id="divUpdate_Details_Revert" runat="server">

        <div class="col-xs-12 col-sm-12 col-lg-12">
            <h4 style="text-align: center">Your Data Verification process  Failed.
            </h4>

            <br />
            <br />
            <h4 style="text-align: center">
                <span style="font-weight: bold;color:#ff0000;">Comments From Back Office : - </span><span id="spRevertMsg_RE" runat="server"></span>
            </h4>
        </div>
         <div class="clearfix"></div><div class="clearfix"></div>
        <div class="col-xs-12 col-sm-12 col-lg-12 " style="text-align: center">
            <asp:LinkButton runat="server" ID="lnkSubmit" OnClick="lnkSubmit_Click" Style="font-size: medium; text-align: center; font-weight: bolder" CssClass="btn btn-link">CLICK HERE FOR MAIN MENU</asp:LinkButton>

        </div>

        

    </div>
    <div class="row" id="divUpdate_Profile_Revert" runat="server">

        <div class="col-xs-12 col-sm-12 col-lg-12">
            <h4 style="text-align: center">Your Data Verification process is Failed 
                <br />

            </h4>

            <h4 style="text-align: center">

                <span style="font-weight: bold;color:#ff0000;">Comments From Back Office : - </span><span id="spRevertMsg" runat="server"></span>
                <br>
            </h4>
            
        </div>

        <div class="col-xs-12 col-sm-12 col-lg-12 " style="text-align: center">
            <asp:HyperLink NavigateUrl="UpdateProfile" Style="font-size: medium; text-align: center; font-weight: bolder" Text="CLICK HERE TO UPDATE YOUR PROFILE" CssClass="btn btn-link" runat="server" />
        </div>

        <br />

    </div>
    <div class="row" id="ReviewApplication" runat="server">
        <div class="col-xs-12 col-sm-12 col-lg-12" style="text-align: center; margin-top: 5vh">
            <div class="col-xs-12 col-sm-12 col-lg-4"></div>
            <div class="col-xs-12 col-sm-12 col-lg-4">
                <asp:ImageButton ID="btnReviewApplication" ImageUrl="~/Images/review.png" OnClick="btnReviewApplication_Click" runat="server" />
            </div>


        </div>
    </div>
</asp:Content>
