<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebForm2.aspx.cs" Inherits="IPRS_Member.WebForm2" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/javascript">


        function ClientuploadStart(sender, args) {
            debugger;
            alert(args._fileName);

            var files = fileUpload.files;
            var test = new FormData();
            for (var i = 0; i < _fileQueueLength; i++) {
                test.append(_fileName, files[i]);
            }
            $.ajax({
                url: "Filehandler.ashx",
                type: "POST",
                contentType: false,
                processData: false,
                data: test,
                // dataType: "json",
                success: function (result) {
                    alert(result);
                },
                error: function (err) {
                    alert(err.statusText);
                }
            });
        }
        function validateFileExtension() {
           
            var flag = 0;
            var component = <%# FPWork.ClientID %>;
           
            var extns = new Array("jpg", "pdf", "jpeg", "gif", "png", "doc", "docx");
            with (component) {
                var ext = value.substring(value.lastIndexOf('.') + 1);
                for (i = 0; i < extns.length; i++) {
                    if (ext == extns[i]) {
                        flag = 0;
                        break;
                    }
                    else {
                        flag = 1;
                    }
                }
                if (flag != 0) {
                    debugger;
                    var divMsg = valid_msg;
                    
                    divMsg.innerHTML = "files with extension jpg/pdf/jpeg/gif/png/doc/docx Allowed ";
                    component.value = "";
                    component.style.backgroundColor = "#eab1b1";
                    component.style.border = "thin solid #000000";
                    component.focus();
                    return false;
                }
                else {
                    return true;
                }
            }
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:HiddenField ID="hdnRecordId" Value="3" runat="server" />
            <asp:ScriptManager ID="ScriptManager1" runat="server" />
            <asp:UpdatePanel ID="upnlwzMain" UpdateMode="Conditional" runat="server">
                <ContentTemplate>
                    <ajaxToolkit:AjaxFileUpload ID="fuplDocs" MaximumNumberOfFiles="2" OnClientUploadComplete="ClientuploadStart" AllowedFileTypes="gif,png,jpg" runat="server" />
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
        <div class="row">
            <div class="col-xs-12 col-sm-12 col-lg-6">
                <div class="input-group">
                    <span class="input-group-addon">Upload Files
                    </span>
                    <asp:FileUpload ID="FPWork" runat="server" required accept="image/gif, image/jpeg, image/png" AllowMultiple="false" CssClass="form-control" />
                </div>
            </div>
            <div class="col-xs-12 col-sm-12 col-lg-6">
                <div class="input-group">
                    <div id="valid_msg" />
                    <asp:Button ID="btnAddWork" runat="server" Text="Add" ToolTip="ADD RECORDS" OnClientClick="return validateFileExtension();" CssClass="btn btn-primary btn-sm" />
                </div>
            </div>
        </div>
    </form>
</body>
</html>
