<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ApplicationError.aspx.cs" Inherits="IPRS_Member.ApplicationError" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />
    <!-- Meta, title, CSS, favicons, etc. -->
    <meta charset="utf-8" />
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <!-- Bootstrap -->
    <link href="Style/bootstrap.min.css" rel="stylesheet" />
    <!-- Font Awesome -->
    <link href="Style/font-awesome/font-awesome.min.css" rel="stylesheet" />
    <!-- NProgress -->
    <link href="Style/nprogress.css" rel="stylesheet" />
    <!-- iCheck -->
    <link href="Style/skins/flat/green.css" rel="stylesheet" />
    <!-- bootstrap-progressbar -->
    <link href="Style/bootstrap-progressbar-3.3.4.min.css" rel="stylesheet" />
    <!-- Custom Theme Style -->
    <link href="Style/custom.min.css" rel="stylesheet" />

    <!-- Bootstrap Alert Box -->
    <script src="javascript/jquery/jquery.min.js"></script>
    <script src="javascript/bootstrap/bootstrap.js"></script>
    <script src="javascript/bootstrap/bootbox.min.js"></script>

</head>
<body>
    <form id="form1" runat="server">
        <div>
            <div class="col-md-12 col-sm-12 col-xs-12">
                <div class="x_panel">
                    <div class="x_content">
                        <section>
                            <div class="col-xs-12 col-sm-12 col-lg-12 x_title">
                                <div class="col-xs-12 col-sm-12 col-lg-12">
                                    <h4>
                                        <asp:Label ID="lblFormTitle" runat="server">Error Decription</asp:Label></h4>
                                </div>
                                <div class="clearfix"></div>
                            </div>
                            <div class="col-xs-12 col-sm-12 col-lg-12">
                                <img class="img-responsive center-block" runat="server" id="imgError" src="Images/error.png" />
                            </div>
                            <br />
                            <br />
                            <div class="col-xs-12 col-sm-12 col-lg-12" id="DivError" runat="server" visible="false">
                                <asp:GridView ID="grdError" runat="server" ShowHeaderWhenEmpty="true" CellPadding="0" CellSpacing="0"
                                    AllowPaging="false" AllowSorting="false" AutoGenerateColumns="False"
                                    CssClass="table table-striped table-bordered dt-responsive nowrap"
                                    ShowHeader="true">
                                    <HeaderStyle CssClass="bg-success" />
                                    <EmptyDataRowStyle HorizontalAlign="Center" />
                                    <EmptyDataTemplate>
                                        No Records Found
                                    </EmptyDataTemplate>
                                    <Columns>
                                        <asp:BoundField DataField="LogDateTime" HeaderText="Date Time">
                                            <ItemStyle Width="15%" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="ErrorLog" HeaderText="Error Description">
                                            <ItemStyle Width="85%" />
                                        </asp:BoundField>
                                    </Columns>
                                </asp:GridView>
                            </div>
                            <div class="col-xs-12 col-sm-12 col-lg-12">
                                <asp:Label ID="lblMessage" runat="server">Please <a href="Default.aspx"><b>Click here</b></a> to login</asp:Label>
                            </div>
                        </section>
                    </div>
                </div>
            </div>
        </div>
    </form>
</body>
</html>
