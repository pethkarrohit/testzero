<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Response.aspx.cs" Inherits="IPRS_Member.Response" %>


<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Response</title>

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


    <script src="Javascript/jquery/jquery.min.js"></script>
    <!-- Bootstrap -->
    <script src="javascript/bootstrap/bootstrap.min.js"></script>

    <!-- Custom Theme Scripts -->
    <script src="javascript/custom.min.js"></script>
</head>
<body>
    <form id="form1" runat="server" method="post">
        <div id="DivHeader" runat="server">
            <asp:Image runat="server" ImageUrl="~/Images/header.jpg" class="divtable" Height="100%"
                Width="100%" />
        </div>
        <asp:ScriptManager runat="server" />
        <asp:UpdatePanel runat="server">
            <ContentTemplate>
                <div class="row">
                    <div class="x_panel">
                        <div class="x_content">
                           
                           
                          
                            <div class="x_title">
                                <h4>
                                    <asp:Label ID="lblFormTitle" runat="server" Style="font-weight: bold; color: #1f72ba;">Payment Reponse</asp:Label></h4>
                            </div>
                           

                            <div id="divsuccess" class="col-xs-12 col-sm-12 col-lg-12" style="text-align: center; font-weight: bold; color: #1f72ba; font-size: x-large"
                                runat="server" >
                                <h4>Thank you for the Registering.</h4>
                                <br />
                                <h4>We have sent you an email for successfully registering at our website.</h4>
                                <br />
                                <h4><a href="Default.aspx">Continue To Home Page</a></h4>
                            </div>
                             
                        </div>
                    </div>
                </div>
            </ContentTemplate>
            
        </asp:UpdatePanel>
    </form>
</body>
</html>
