﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MemberChangePassword.aspx.cs" Inherits="IPRS_Member.MemberChangePassword" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>::: APPLICAITON PASSWORD CHANGE:::</title>
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
<body class="login" style="background: url(images/Login_App_Bg.jpg) no-repeat  center top;">
    <form id="form1" runat="server">
        <asp:ScriptManager runat="server" />
        <asp:UpdatePanel ID="UpdtPnlChangePwd" UpdateMode="Always" runat="server">
            <ContentTemplate>
                <div style="height: 160px">
                </div>
                <div>
                    <ucChangePassword:ucChangePassword runat="server" ID="ucChangePassword" strStrongPassword="Y" />
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </form>


</body>
</html>
