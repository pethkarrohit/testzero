<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ucMemberChangePassword.ascx.cs" Inherits="IPRS_Member.User_Controls.ucMemberChangePassword" %>
<%--<style type="text/css">
    .messagealert {
        width: 100%;
        position: fixed;
        top: 0px;
        z-index: 100000;
        padding: 0;
        font-size: 15px;
    }
</style>--%>

<div class="login_wrapper">
    <div class="animate form login_form">
        <section class="login_content">
            <h4 >Change Password</h4> 
            <div>
                <asp:TextBox ID="txtLoginName" runat="server" CssClass="form-control" placeholder="Enter Login Name" ReadOnly required/>
                <asp:HiddenField id="hdnUserName" runat="server"/>
                <asp:HiddenField id="hdnAuditTrail" runat="server"/>
            </div>
            <div>
                <br />
                <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" class="form-control" AutoCompleteType="Disabled" autocomplete="off"
                    placeholder="Enter Existing Password" required />
            </div>
            <div>
                <br />
                 <asp:TextBox ID="txtNewPassword" MaxLength="15" pattern="^([a-zA-Z0-9@*#]{6,15})$" runat="server" CssClass="form-control" AutoCompleteType="Disabled" autocomplete="off" TextMode="Password"
                                                            placeholder="Enter Password" required /><a title="Minimum 6 Characters Allowed &#10 Eg : 1#Zv96g@*Yfasd4">
                                                                <img src="Images/help-icon.png" style="position: absolute; right: 3px; z-index: 100; cursor: pointer; top: 5px">
                                                            </a>
              
            </div>
            <div>
                <br />
                <asp:TextBox ID="txtConfirmPassword" runat="server" TextMode="Password" class="form-control" AutoCompleteType="Disabled" autocomplete="off"
                    placeholder="Enter Confirm Password" required />
            </div>
            <div>
                <br />
                <asp:Button ID="btnChangePassword" Text="Change Password" runat="server" class="btn btn-primary" OnClick="btnChangePassword_Click" />

            </div>
        
            <div id="divForgotPassword" runat="server">
                <p>Back to Login? <a href="MemberLogout.aspx">click here</a></p>
            </div>
          
            <div id="dvMessage" runat="server" visible="false" style="color:red" >
                <strong>ERROR!</strong>
                <asp:Label ID="lblMessage" runat="server" />
            </div>
            <div class="clearfix"></div>

        </section>
    </div>

    <!-- Small modal -->
    <div class="modal fade bs-example-modal-sm" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-sm">
            <div class="modal-content">

                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                        <span aria-hidden="true">×</span>
                    </button>
                    <h4 class="modal-title" id="myModalLabel2">
                        <asp:Label runat="server" ID="lblTitle"></asp:Label></h4>
                </div>
                <div class="modal-body">
                    <asp:Label runat="server" ID="lblAlertMessage"></asp:Label>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-default" data-dismiss="modal">Close</button>
                </div>

            </div>
        </div>
    </div>
    <!-- Small modal -->
</div>


