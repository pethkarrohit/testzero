<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FileDownload.aspx.cs" Inherits="IPRS_Member.FileDownload" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
            <asp:ScriptManager runat="server" />
        <div>

            <asp:UpdatePanel ID="UpdtPnl" UpdateMode="Always" runat="server">
                <ContentTemplate>
                    <asp:HiddenField ID="hdnquery" runat="server" Value="" />
                    <div style="height: 175px">
                    </div>
                    <div>
                        <asp:UpdateProgress ID="UpdateProgress2" ClientIDMode="Static" runat="server" AssociatedUpdatePanelID="UpdtPnl">
                            <ProgressTemplate>
                                <div class="row">
                                    <div class="col-xs-12 col-sm-12 col-lg-12" style="text-align: center">
                                        <img alt="Downloading Files ..." runat="server" src="~/Images/AnimatedProgressBar.gif" />
                                        <br />
                                       Downloading Files Please Wait ...
                                    </div>
                                </div>
                            </ProgressTemplate>
                        </asp:UpdateProgress>
                        <asp:Timer ID="Timer1" runat="server" Enabled="false" Interval="1000" OnTick="Timer1_Tick">
                        </asp:Timer>
                    </div>
                </ContentTemplate>
                <Triggers>
                    <asp:PostBackTrigger    ControlID="Timer1" />
                </Triggers>
            </asp:UpdatePanel>

        </div>
    </form>
</body>
</html>
