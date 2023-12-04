<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ApplicationForm_Rpt.aspx.cs" Inherits="IPRS_Member.App_Reports.ApplicationForm_Rpt" %>

<!DOCTYPE html>
<%--<script src="https://code.jquery.com/jquery-1.11.3.min.js"></script>--%>


<html xmlns="http://www.w3.org/1999/xhtml"
<head runat="server">
    <title>Application Report</title>

    <style>
        .loader {
            position: fixed;
            left: 0px;
            top: 0px;
            width: 100%;
            height: 100%;
            z-index: 9999;
            background: url('/Images/loading.gif') 50% 50% no-repeat rgb(249,249,249);
        }

        #dvLoading {
            background: #000 url(/Images/Processing-Report-Please-Wait.gif) no-repeat center center;
        }
    </style>
    <script>
        $(window).load(function () {
            $('#dvLoading').fadeOut(2000);
        });

        function CallButtonClick() {
          
            document.getElementById('<%=btnDisplayReport.ClientID%>').click();
            return true;
        }



        //function CallFileDeleteMethod() {
        //    debugger;
        //    var URL = "http://iprstest.dreamsoftindia.com/Webservice/licservice.asmx/HelloWorld";

        //    $.ajax({

        //        type: "GET",
        //        url: 'http://iprstest.dreamsoftindia.com/Webservice/licservice.asmx/getemployeeImage?UserID=13',
        //      //  data: "'UserID':'13'",
        //        //  data: "{'one':\"" + one+ "\", 'two':\"" + two + "\" }",
        //        crossDomain: true,
        //        contentType: "application/json; charset=utf-8",
        //        //datatype: "json",
        //        success: function (responseFromServer) {
        //            alert(responseFromServer.d)

        //        },
        //        error: function (data, errorThrown) {
        //            alert("Fail");
        //            alert(errorThrown);
        //        }
        //    });
        //}
    </script>

</head>
<body onload="CallButtonClick();">
    <form id="form1" runat="server">
        <%-- <div>
            <uc1:ucPdfHeader runat="server" Visible="false" ID="ucPdfHeader" />
        </div>--%>
        <div id="dvLoading"></div>
        <asp:HiddenField ID="hdnRecordId" runat="server" Value="" />
        <asp:HiddenField ID="hdnRefAccountId" runat="server" Value="" />
             <asp:HiddenField ID="hdnoutOfPage" runat="server" Value="" />
        <asp:Button ID="btnDisplayReport" runat="server" Text="Button" OnClick="btnDisplayReport_Click" Style="display: none;" />
        <div style="width: 100%; height: 100vh;" class="loader">
            <img id="imgProcess" runat="server" alt="Prcessing" visible="true" src="~/Images/Processing-Report-Please-Wait.gif" style="height: auto; margin: 22% 33%" />
            <img id="imgSession" runat="server" alt="Session" visible="false" src="~/Images/conection-expired.png" style="height: auto; margin: 22% 33%" />
            <img id="imgDataSet" runat="server" alt="DataSet Error" visible="false" src="~/Images/dataset-error.png" style="height: auto; margin: 22% 33%" />
        </div>

    </form>
</body>
</html>
