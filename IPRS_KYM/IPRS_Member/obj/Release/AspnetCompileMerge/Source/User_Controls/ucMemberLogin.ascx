<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ucMemberLogin.ascx.cs" Inherits="IPRS_Member.User_Controls.ucMemberLogin" %>

<div class="login_wrapper">
    <div class="animate form login_form">
        <section class="login_content">
            <div>
                <%-- if member is exist then use thier login crediantial to view or update profile.
                    :Comment By Rohit --%>
                <asp:TextBox ID="txtLoginName" runat="server" CssClass="form-control" placeholder="Enter Login Name"  required />
            </div>
            <div>
                <br />
                <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" class="form-control" AutoCompleteType="Disabled" autocomplete="off"
                    placeholder="Enter Password" required />
            </div>
            <div class="checkbox" id="divRememberMe" runat="server">
                <br />
                  <%-- if member want to save the login crediantial then select remember me
                    :Comment By Rohit --%>
                <asp:CheckBox ID="chkRememberMe" Text="Remember Me" runat="server" />
            </div>
            <div id="divForgotPassword" runat="server">
                <br />
                <%-- Regenrate Password and send on the member registerd Email. : Comment By Rohit --%>
                <p>forgot your password? <a href="ForgotPassword">click here</a></p>
            </div>
            <div id="divRegistration" runat="server">
                <br />
                <%-- For becoming new mamber. redirect to member welcome page.: Comment By Rohit --%>
                <p>New Registration <a href="MemberWelcome.aspx">click here</a></p>
            </div>
            <div>
                <br />
                <%-- for view or update profile using login credential. : Comment By Rohit  --%>
                <asp:Button ID="btnLogin" Text="Login" runat="server" class="btn btn-primary" OnClick="btnLogin_Click" />
            </div>
          
            <div id="dvMessage" runat="server" style="margin-top:1%" visible="false" class="alert-warning">
                <%-- For Show Error Massage --%>
                <asp:Label ID="lblMessage" runat="server" />
            </div>
            <div class="clearfix"></div>

        </section>


    </div>

</div>


