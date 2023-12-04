<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ucTooltip.ascx.cs" Inherits="IPRS_Member.User_Controls.ucTooltip" %>

<%
    if (tooltiptext == "" && imgsrc == "")
    {%>

<a title='<%=tooltipanchor %>'>
    <img src="Images/help-icon.png" style="position: absolute; right: 3px; z-index: 100; cursor: pointer; top: 5px">
</a>

<% } %>


<%
    else
    {%>
<a data-toggle="modal" data-target="#ucTooltip" title='<%=tooltipanchor %>'>
    <img src="Images/help-icon.png" style="position: absolute; right: 3px; z-index: 100; cursor: pointer; top: 5px">
</a>
<div class="modal fade" id="ucTooltip" role="dialog">
    <div class="modal-dialog">

        <!-- Modal content-->
        <div class="modal-content">

            <div class="modal-body">
                <p id="tooltipPara" runat="server">
                    <%
                        if (tooltiptext != "")
                        {%>
                    <br />

                    <%=tooltiptext %>

                    <br />
                    <% } %>


                    <%
                        if (imgsrc != "")
                        {%>

                    <img id="Img" src=' <%=imgsrc %>' height="100%" width="100%" />

                    <% } %>
                </p>
            </div>

        </div>

    </div>
    
</div>
<% } %>
