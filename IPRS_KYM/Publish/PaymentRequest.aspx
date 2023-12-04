<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PaymentRequest.aspx.cs" Inherits="IPRS_Member.PaymentRequest" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="~/Style/bootstrap.min.css" rel="stylesheet" />
    
     <script type="text/javascript">
         function CallClick() {
             debugger;
             var obj = document.getElementById("btnsubmit");
             obj.click();
         }

     </script>

</head>
<body onload="CallClick()">
    <form name="Form1" runat="server" method="POST">



        <input type="hidden" runat="server" id="key" name="key" />
        <input type="hidden" runat="server" id="hash" name="hash" />
        <input type="hidden" runat="server" id="txnid" name="txnid" />

        <asp:HiddenField ID="amount" runat="server" Value="" />


        <asp:HiddenField ID="email" Value="" runat="server"/>
        <asp:HiddenField ID="firstname" Value="" runat="server"/>
        <asp:HiddenField ID="phone" Value="" runat="server"/>

        <asp:HiddenField ID="productinfo" Value="" runat="server"/>

        <asp:HiddenField ID="surl" Value="" runat="server"/>

        <asp:HiddenField ID="furl" Value="" runat="server"/>

        <asp:HiddenField ID="lastname" runat="server"/>

        <asp:HiddenField ID="curl" Value="" runat="server"/>



        <asp:HiddenField ID="address1" Value="Malad" runat="server"/>

        <asp:HiddenField ID="address2" runat="server"/>


        <asp:HiddenField ID="city" runat="server"/>

        <asp:HiddenField ID="state" runat="server"/>

        <asp:HiddenField ID="country" runat="server"/>

        <asp:HiddenField ID="zipcode" runat="server"/>

        <asp:HiddenField ID="udf1" Value="12" runat="server"/>

        <asp:HiddenField ID="udf2" runat="server"/>

        <asp:HiddenField ID="udf3" runat="server"/>

        <asp:HiddenField ID="udf4" runat="server"/>

        <asp:HiddenField ID="udf5" runat="server"/>

        <asp:HiddenField ID="pg" runat="server"/>
          <asp:HiddenField ID="hdnTxnId" runat="server"/>
          <asp:HiddenField ID="HiddenField1" runat="server"/>

        <asp:Button ID="btnsubmit" Value="submit" Width="100px" runat="server" Style="display: none" OnClick="btnsubmit_Click" />
        <br>
       
    </form>
</body>
</html>
