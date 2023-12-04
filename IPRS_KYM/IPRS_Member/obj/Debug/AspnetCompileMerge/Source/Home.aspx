<%@ Page Title="" Language="C#" MasterPageFile="ApplicationMember.master" AutoEventWireup="true" CodeBehind="Home.aspx.cs" Inherits="IPRS_Member.Home" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .portfolio-item {
            padding: 3%;
            width: 100%;
            min-height: 150px;
            border: 2px solid #006aad;
            border-radius: 5px;
            -moz-border-radius: 5px;
            -webkit-border-radius: 5px;
            margin-bottom: 20px;
        }

        .portfolio-image img {
            margin: 0 auto;
            padding: 0px;
            display: block;
            position: relative;
            overflow: hidden;
        }

        .portfolio-title {
            margin-bottom: 10px;
        }

        .portfolio-text .title {
            font-weight: 500;
            margin-top: 20px;
            margin-bottom: 0px;
            font-size: 15px;
            text-align: center;
            text-transform: uppercase;
        }

        .portfolio-text .subtitle {
            display: block;
            font-style: normal;
            font-size: 11px;
            opacity: 0.8;
            margin-bottom: 10px;
        }

        .portfolio-detail {
            opacity: 0.6;
            color: #666666;
            display: inline-block;
            padding-right: 10px;
            padding-left: 10px;
            margin-bottom: 5px;
            margin-top: 0px;
            font-size: 11px;
        }

        .portfolio-links a {
            color: #666666;
            opacity: 0.6;
        }

        .portfolio-details a {
            opacity: 0.5;
            color: #666666;
            display: inline-block;
            padding-right: 10px;
            padding-left: 10px;
            margin-bottom: 5px;
            margin-top: 10px;
            font-size: 11px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="row">
        <div class="col-md-12 col-sm-12 col-xs-12">
            <div class="x_panel">
                <div class="x_content">

                    <div>
                        <h4>
                            <asp:Label ID="lblFormTitle" runat="server">Main Menu</asp:Label></h4>
                    </div>
                    <div class="clearfix"></div>

                    <div class="fieldset panel">
                        <div class="fieldset-body panel-body">
                            <div class="row">
                                <div class="col-xs-6 col-sm-6 col-lg-3" id="divUpdateProfile" runat="server">
                                    <div class="portfolio-item">
                                        <a href="UpdateProfile">
                                            <div class="portfolio-image">

                                                <img src="Images/update-profile.png">
                                            </div>
                                            <div class="portfolio-text">
                                                <h3 class="title">Make Payment</h3>
                                            </div>
                                        </a>
                                    </div>

                                </div>


                                <div id="divUpdateData" runat="server">
                                    <div class="col-xs-6 col-sm-6 col-lg-3" id="divUpdateAddressDetails" runat="server">
                                        <div class="portfolio-item">
                                            <a href="UpdateAddressDetails">
                                                <div class="portfolio-image">

                                                    <img src="Images/update-address.png">
                                                </div>
                                                <div class="portfolio-text">
                                                    <h3 class="title">Update Address</h3>
                                                </div>
                                            </a>
                                        </div>
                                    </div>
                                    <div class="col-xs-6 col-sm-6 col-lg-3" id="divUpdateBankInfo" runat="server">
                                        <div class="portfolio-item">
                                            <a href="UpdateBankInfo">
                                                <div class="portfolio-image">

                                                    <img src="Images/update-bank-info.png">
                                                </div>
                                                <div class="portfolio-text">
                                                    <h3 class="title">Update Bank Info</h3>
                                                </div>
                                            </a>
                                        </div>
                                    </div>
                                    <div class="col-xs-6 col-sm-6 col-lg-3" id="divUpdateGSTDetails" runat="server">
                                        <div class="portfolio-item">
                                            <a href="UpdateGSTDetails">
                                                <div class="portfolio-image">

                                                    <img src="Images/update-gst-info.png">
                                                </div>
                                                <div class="portfolio-text">
                                                    <h3 class="title">Update GST Info</h3>
                                                </div>
                                            </a>
                                        </div>
                                    </div>



                                    <div class="col-xs-6 col-sm-6 col-lg-3" id="divUpdateBasicInfo" runat="server">
                                        <div class="portfolio-item">
                                            <a href="UpdateBasicInfo">
                                                <div class="portfolio-image">

                                                    <img src="Images/update-basic-info.png">
                                                </div>
                                                <div class="portfolio-text">
                                                    <h3 class="title">Update Basic Info</h3>
                                                </div>
                                            </a>
                                        </div>
                                    </div>
                                </div>


                                <div class="col-xs-6 col-sm-6 col-lg-3" id="divPreviewApplication" runat="server">
                                    <div class="portfolio-item">
                                        <a id="anurl" runat="server" href="#"   target="_blank">
                                            <div class="portfolio-image">

                                                <img src="Images/update-profile.png">
                                            </div>
                                            <div class="portfolio-text">
                                                <h3 class="title">Preview Application</h3>
                                            </div>
                                        </a>
                                    </div>

                                </div>







                         <%--       <div class="col-xs-6 col-sm-6 col-lg-3" id="divUpdateNominee" runat="server">
                                    <div class="portfolio-item">
                                        <a href="UpdateNominee">
                                            <div class="portfolio-image">

                                                <img src="Images/update-basic-info.png">
                                            </div>
                                            <div class="portfolio-text">
                                                <h3 class="title">Update Nominee</h3>
                                            </div>--%>
                                        <%--</a>--%>
                                    </div>

                                </div>






                            </div>

                        </div>
                    </div>




                </div>
            </div>

        </div>
    </div>

</asp:Content>
