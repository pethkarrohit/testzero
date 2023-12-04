<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ForgotPassword.aspx.cs" Inherits="IPRS_Member.ForgotPassword" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>::: APPLICAITON FORGOT PASSWORD:::</title>
    <%@ Register Src="User_Controls/ucMemberChangePassword.ascx" TagPrefix="ucChangePassword" TagName="ucChangePassword" %>
    <meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />
    <!-- Meta, title, CSS, favicons, etc. -->
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
    <link href="Style/modalPopup.css" rel="stylesheet" />
    <!-- Custom Theme Scripts -->
    <script src="~/javascript/custom.min.js"></script>
    <script src="javascript/jquery/jquery.min.js"></script>
    <!-- Bootstrap -->
    <script src="javascript/bootstrap/bootstrap.min.js"></script>
    <!-- Bootstrap Alert Box -->
    <script src="javascript/bootstrap/bootbox.min.js"></script>
    <!-- Custom Theme Scripts -->
    <script src="javascript/custom.min.js"></script>
</head>
    <%-- Attach Image(Login_App_Bg.jpg) to Background : Comment By Rohit --%>
<body class="login" style="background: url(Images/Login_App_Bg.jpg) no-repeat  center top;">
    <form id="form1" runat="server">
        <div style="height: 160px">
        </div>
        <div class="login_wrapper">
            <div class="animate form login_form">
                <section class="login_content">
           <%-- Region Create Secton For Forgot Password : Comment By Rohit --%>        
                    <span class="title" >Forgot Password</span> 
           <%-- Create TextBox For Login Name : Comment By Rohit --%>
            <div>
                <asp:TextBox ID="txtLoginName" runat="server" CssClass="form-control" required placeholder="Enter Login Name"  />
            </div>
           <%-- Create Button For Get Password on EmailID: Comment By Rohit --%>
            <div>
                <br />
                <asp:Button ID="btnForgotPassword" Text="Get Password" OnClick="btnForgotPassword_Click" runat="server" class="btn btn-primary" />
            </div>
           <%-- EndRegion Create Secton For Forgot Password : Comment By Rohit --%>    
            <br />
           <%-- Create Link For login: Comment By Rohit --%>
            <div id="divForgotPassword" runat="server">
                <p>Back to Login? <a href="MemberLogin">click here</a></p>
            </div>
            <br />
            <div id="dvMessage" runat="server" visible="false"  style="color:red">
                <strong>Error!</strong>
                <asp:Label ID="lblMessage" runat="server" />
            </div>
            <div class="clearfix"></div>
        </section>
            </div>
        </div>
    </form>
</body>
</html>
