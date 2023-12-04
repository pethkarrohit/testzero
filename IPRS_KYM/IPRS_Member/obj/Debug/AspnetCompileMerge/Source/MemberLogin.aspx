<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MemberLogin.aspx.cs" Inherits="IPRS_Member.MemberLogin" %>




<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>::: APPLICATION LOGIN :::</title>
    <meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />
    <!-- Meta, title, CSS, favicons, etc. -->
    <meta charset="utf-8" />
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <!-- Bootstrap -->
    <link href="~/Style/bootstrap.min.css" rel="stylesheet" />
    <!-- Font Awesome -->
    <link href="Style/font-awesome/font-awesome.min.css" rel="stylesheet" />
    <!-- NProgress -->
    <link href="~/Style/nprogress.css" rel="stylesheet" />
    <!-- iCheck -->
    <link href="Style/skins/flat/green.css" rel="stylesheet" />
    <!-- bootstrap-progressbar -->
    <link href="~/Style/bootstrap-progressbar-3.3.4.min.css" rel="stylesheet" />
    <!-- Custom Theme Style -->
    <link href="~/Style/custom.min.css" rel="stylesheet" />
    <%@ Register Src="User_Controls/ucMemberLogin.ascx" TagPrefix="ucMemberLogin" TagName="ucMemberLogin" %>
 
</head>
<body class="login" style="background: url(images/Login_App_Bg.jpg) no-repeat  center top;">
    <form id="form1" runat="server">
        <asp:ScriptManager runat="server" />
        <asp:UpdatePanel ID="UpdtPnlLogin" UpdateMode="Always" runat="server">
            <ContentTemplate>
                <div style="height: 175px">
                </div>
                <div>
                    <%-- Here we use usercontrol for become memebr or eixst member can view thier profile. 
                        Comment By Rohit --%>
                    <ucMemberLogin:ucMemberLogin runat="server" id="ucMemberLogin" strDisplayForgotPassword="Y"  strDisplayRegistration="Y"/>
                </div>
            </ContentTemplate>
            <Triggers>
            </Triggers>
        </asp:UpdatePanel>
    </form>


</body>
</html>
