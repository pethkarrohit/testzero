<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebForm3.aspx.cs" Inherits="IPRS_Member.WebForm3" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div style="text-align: center">
            <table style="width: 70%">
                <tr>
                    <td>
                        <asp:TextBox ID="txtTransno" runat="server" Width="100%"></asp:TextBox>

                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Button ID="btnTest" runat="server" Text="Verify" OnClick="btnTest_Click" /></td>
                </tr>
                <tr>
                    <td>
                        <div id="divudf1" runat="server"></div>
                    </td>
                </tr>
                <tr>
                    <td>
                        <div id="divResponseString" runat="server"></div>
                    </td>
                </tr>
            </table>
            <br />
              <table style="width: 70%">
                
                <tr>
                    <td>
                        <asp:Button ID="btnWorkRegFolder" runat="server" Text="GET File NAmes" OnClick="btnWorkRegFolder_Click" /></td>
                </tr>
              
                <tr>
                    <td>
                     <div id="divwork" runat="server" ></div>
                    </td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>
