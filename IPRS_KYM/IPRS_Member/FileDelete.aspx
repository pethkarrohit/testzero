<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FileDelete.aspx.cs" Inherits="IPRS_Member.FileDelete" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script src="Javascript/jquery/jquery.js"></script>
    <script type="text/javascript">

        function CallMethod() {
            debugger;
            var URL = document.getElementById("url");
            var name = document.getElementById("name");
            $.ajax({

                type: "POST",
                url: URL.value,
                data: "{'MemberIds':'" + name.value + "'}",
                contentType: "application/json",
                datatype: "json",
                success: function (responseFromServer) {
                    alert(responseFromServer.d)
                }
            });

        }
    </script>

</head>
<body>
    <form id="form1" runat="server">
        <div>
            Name:-<input type="text" name="name" id="name" />
             URL:-<input type="text" name="url" value="FileDelete.aspx/DeleteFile_Member" id="url" />

            <asp:Button ID="btn" OnClientClick="CallMethod();" runat="server" Text="ClickMe" />
        </div>
    </form>
</body>
</html>
