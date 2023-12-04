<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MemberVerification.aspx.cs" Inherits="IPRS_Member.MemberVerification" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">

<head runat="server">
    <title>Member Verification</title>

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
    <form id="form1" runat="server">
        <asp:ScriptManager runat="server" />
        <asp:UpdatePanel runat="server">
            <ContentTemplate>
                <asp:Timer ID="tmrVerificationMessage" ViewStateMode="Enabled" ValidateRequestMode="Enabled" Enabled="true" OnTick="tmrVerificationMessage_Tick" Interval="1000" runat="server"></asp:Timer>
                <div id="DivHeader" runat="server">
                    <asp:Image runat="server" ImageUrl="~/Images/header.jpg" class="divtable" Height="100%" Width="100%" />
                </div>
                <div class="x_content">
                    <div class="x_title">
                        <h4>
                            <asp:Label ID="lblFormTitle" runat="server" Style="font-weight: bold; color: #1f72ba;">VERIFICATION</asp:Label></h4>
                        <asp:HiddenField ID="hdnRecordKeyId" runat="server" Value="0" />
                        <div class="clearfix"></div>
                        <div id="divPrograss" runat="server">
                            <div class="col-sm-12" style="text-align: center; margin-top: 10vh">
                                <asp:Image ImageUrl="~/Images/AnimatedProgressBar.gif" class="pageloading" runat="server" />
                            </div>
                            <div class="col-sm-12" style="text-align: center;">
                                <div class="col-sm-12">
                                    <h1>Please wait... We are sending you the Verification Message.</h1>
                                </div>
                            </div>
                        </div>
                        <div id="divActivated" visible="false" runat="server">
                            <div class="col-sm-12" style="text-align: center; height: 55vh; margin-top: 25vh">
                                <div class="col-sm-12">
                                    <h1>Login Id has been activated successfully.</h1>
                                </div>
                                <div class="col-sm-12">
                                    <asp:HyperLink NavigateUrl="Default.aspx" Style="font-weight: bold; font-size: medium" Text="CLICK HERE TO LOGIN." runat="server" />
                                </div>
                            </div>
                            <div class="col-sm-12">
                                <div class="col-sm-10"></div>
                                <div class="col-sm-2">
                                </div>
                            </div>
                        </div>
                        <div id="divRejected" visible="false" runat="server">
                            <div class="col-sm-12" style="text-align: center; height: 55vh; margin-top: 25vh">
                                <div class="col-sm-12">
                                    <h1>Invalid Request</h1>
                                    <br />
                                    <h1>Error:</h1>
                                    <h1><span id="spError" runat="server"></span></h1>
                                </div>
                                <div class="col-sm-12">
                                    <h1>Contact to <a href="mailto:membership@iprs.org">membership@iprs.org</a> for further queries.</h1>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="tmrVerificationMessage" EventName="Tick" />
            </Triggers>
        </asp:UpdatePanel>
    </form>

</body>
</html>
