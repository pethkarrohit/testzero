<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ucNomineeDetails.ascx.cs" Inherits="IPRS_Member.User_Controls.ucNomineeDetails" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Src="~/User_Controls/ucDropDown.ascx" TagPrefix="ucDropDown" TagName="ucDropDown" %>
<%@ Register Src="~/User_Controls/ucTooltip.ascx" TagPrefix="ucTooltip" TagName="ucTooltip" %>


<style type="text/css">
    .address {
        border-bottom: 2px solid #e8e8e8;
        /*padding: 1px 5px 6px;*/
        margin-bottom: 10px;
    }
</style>
<script src="Javascript/FCalender.js"></script>
<link href="Style/zcCalendar.css" rel="stylesheet" />
<script type="text/javascript">

    function dateSelectionChanged(sender, args) {


        selectedDate = sender.get_selectedDate();
        var date = moment(new Date(selectedDate));

        sender._textbox._element.value = (moment(date).format('DD-MM-YYYY'));
        SetDateField(sender._textbox._element);
    }


    function SetDivPosition(obj) {


        var intY1 = obj.scrollTop;
        var h1 = document.getElementById("<%=div_position.ClientID%>");
        h1.value = intY1;

    }
             
    function onlyDotsAndNumbers(event)
    {
        
        var charCode = (event.which) ? event.which : event.keyCode
        if (charCode == 46) {
            return true;
        }
        if (charCode > 31 && (charCode < 48 || charCode > 57))
            return false;

        return true;
    }




</script>


<style type="text/css">
    .grid-header th {
        font-weight: bold;
        font-family: Verdana;
        font-size: 11px;
        background-color: #1a76b3;
        color: White;
        text-align: center;
        position: relative;
        padding: 5px;
        border: 1px solid #377cc7;
    }

    .grid-header td {
        padding: 5px;
    }

    @media (min-width:500px) {
        .adjGrid {
            margin: 0;
            position: relative;
            bottom: 0;
            height: 10%;
            width: 100%;
        }
    }

    input[type=text] {
        text-transform: uppercase;
    }
</style>


<asp:Panel runat="server">
    <div class="row">
        
        <asp:HiddenField ID="hdnRecordId" Value="0" runat="server" />
        <asp:HiddenField ID="div_position" Value="0" runat="server" />
        <asp:HiddenField ID="hdnRegistrationType" runat="server" />
        <div class="x_title">
            <<%--h4>
                <asp:Label ID="lblFormTitle" runat="server">Update Nominee Details</asp:Label></h4>--%>
            <div class="clearfix"></div>
        </div>


        <div class="row" runat="server" visible="true">
            <div class="col-xs-12 col-sm-12 col-lg-12">
                <div class="input-group">
                    <%--<asp:RadioButtonList ID="rbtRegistrationType" Enabled="true" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="rbtRegistrationType_SelectedIndexChanged">
                        <asp:ListItem class="radio-inline" Selected="True" Text="Individual" Value="I"></asp:ListItem>
                        <asp:ListItem Text="NRI Individual" class="radio-inline" Value="NI"></asp:ListItem>
                        <asp:ListItem Text="Self Release" class="radio-inline" Value="SC"></asp:ListItem>
                        <asp:ListItem Text="NRI Self Release" class="radio-inline" Value="NSC"></asp:ListItem>
                    </asp:RadioButtonList>--%>
                </div>
            </div>
            <div class="col-xs-12 col-sm-12 col-lg-9">
                <div class="input-group">
                </div>
            </div>
        </div>


        <div class="row">
            <div class="col-xs-12 col-sm-12 col-lg-6" id="divFirstName" runat="server">
                <div class="input-group">
                    <span class="input-group-addon">First Name*
                    </span>
                    <asp:TextBox ID="txtFirstName" MaxLength="50" runat="server" CssClass="form-control" AutoCompleteType="Disabled" autocomplete="off"
                        placeholder="Enter First Name" required />
                </div>
            </div>
            <div class="col-xs-12 col-sm-12 col-lg-6" id="divLastName" runat="server">
                <div class="input-group">
                    <span class="input-group-addon">Last Name*
                    </span>
                    <asp:TextBox ID="TxtLastName" MaxLength="50" runat="server" CssClass="form-control" AutoCompleteType="Disabled" autocomplete="off"
                        placeholder="Enter Last Name" required />
                </div>
            </div>
        </div>
        <div class="row">
            <div class="col-xs-12 col-sm-12 col-lg-6" id="divDOB" runat="server">
                <div class="input-group">
                    <span class="input-group-addon">Date of Birth*
                    </span>
                    <asp:TextBox ID="txtDOB" runat="server"  AutoPostBack ="true"  required class="form-control" Style="width: 98%" AutoCompleteType="Disabled" autocomplete="off" placeholder="dd-MMM-yyyy" onBlur="GetDateField(this,false);myFun();" onFocus="SetDateField(this);" ToolTip="Format : [ddmmyyyy], [d] or [dd], [ddm] or [ddmm], [ ].&#xa;Keys : Day-[Up/Down], Month-[alt+Up/Down], Year-[shift+Up/Down], Today-[Home]." />
                    <asp:CalendarExtender ID="calDOB" OnClientDateSelectionChanged="dateSelectionChanged" CssClass="zcCalendar" runat="server" TargetControlID="txtDOB" Format="dd-MMM-yyyy"></asp:CalendarExtender>
                </div>
            </div>
            <div class="col-xs-12 col-sm-12 col-lg-6" id="divGender" runat="server">
                <div class="input-group">
                    <span class="input-group-addon">Gender*</span>
                    <asp:RadioButtonList ID="rbtnGender" RepeatDirection="Horizontal" RepeatColumns="4" CssClass="form-control" runat="server">
                        <asp:ListItem Text="Male" Value="Male" Selected="True"></asp:ListItem>
                        <asp:ListItem Text="Female" Value="Female"></asp:ListItem>
                        <asp:ListItem Text="Other" Value="Other"></asp:ListItem>
                    </asp:RadioButtonList>
                </div>
            </div>


        </div>

        <div class="row">
            <div class="col-xs-12 col-sm-12 col-lg-6" id="divRelationship" runat="server">
                <div class="input-group">
                    <span class="input-group-addon">Relationship*
                    </span>
                    <asp:DropDownList ID="DDLRelationship" runat="server" CssClass="form-control" AutoCompleteType="Disabled" autocomplete="off">
                    <asp:ListItem Text="Select Relationship" Value="Select Relationship"></asp:ListItem>
                    <asp:ListItem Text="Parent" Value="Parent"></asp:ListItem>
                    <asp:ListItem Text="Child" Value="Child"></asp:ListItem>
                    <asp:ListItem Text="Spouse" Value="Spouse"></asp:ListItem>
                    <asp:ListItem Text="Sibling" Value="Sibling"></asp:ListItem>
                    <asp:ListItem Text="Grandparents" Value="Grandparents"></asp:ListItem>
                    <asp:ListItem Text="Grandchild" Value="Grandchild"></asp:ListItem>
                    <asp:ListItem Text="Parent sibling" Value="Parent sibling"></asp:ListItem>
                    <asp:ListItem Text="Sibling child" Value="Sibling child"></asp:ListItem>
                    <asp:ListItem Text="Aunt_Uncle child" Value="Aunt_Uncle child"></asp:ListItem>
                </asp:DropDownList>
                </div>
            </div>
            <div class="col-xs-12 col-sm-12 col-lg-6" id="divMinor" runat="server">
                <div class="input-group">
                    <span class="input-group-addon">Minor (Yes/No)
                    </span>
                    <asp:RadioButtonList ID="RBLMinor" runat="server" Enabled ="false" RepeatDirection="Horizontal" AutoPostBack="true"  CssClass="form-control"  OnSelectedIndexChanged="RBLMinor_SelectedIndexChanged">                   
                        <asp:ListItem Text="No" Value="0"  Selected ="True" ></asp:ListItem>
                        <asp:ListItem Text="Yes" Value="1" ></asp:ListItem>                        
                    </asp:RadioButtonList>
                </div>
            </div>
        </div>

        <div class="row">
            <div class="col-xs-12 col-sm-12 col-lg-6" id="divGuardian" runat="server">
                <div class="input-group">
                    <span class="input-group-addon">Guardian Name
                    </span>
                    <asp:TextBox ID="txtGuardianName" MaxLength="50" runat="server" CssClass="form-control" AutoCompleteType="Disabled" autocomplete="off"
                        placeholder="Enter Guardian Name"  />
                </div>
            </div>

            <div class="col-xs-12 col-sm-12 col-lg-5" runat="server" id="divGuardMobile">
                <div class="input-group">
                    <span class="input-group-addon">Guardian Mobile
                    </span>
                    <asp:TextBox ID="txtCountryCodeG" MaxLength="3" runat="server" Width="15%" CssClass="form-control" AutoCompleteType="Disabled" autocomplete="off"
                        placeholder="+91" value="+91" required />
                    <asp:Label ID="label3" class="form-control" runat="server" Text="-" Style="border-color: white" Width="5%" />
                    <asp:TextBox ID="txtGuardMobileNo" runat="server" CssClass="form-control" AutoCompleteType="Disabled" autocomplete="off" MaxLength="10"
                        placeholder="Enter Guardian Mobile"  step="1" Width="70%" min="0000000000" max="9999999999"></asp:TextBox>
                    <ajaxToolkit:FilteredTextBoxExtender ID="filterno" runat="server" TargetControlID="txtGuardMobileNo"
                    FilterType="Numbers,Custom" ValidChars=".,()/-"></ajaxToolkit:FilteredTextBoxExtender>

                </div>
            </div>


        </div>

     

        <div class="row">

            <div class="col-xs-12 col-sm-12 col-lg-6" runat="server" id="divPanNo">
                <div class="input-group">
                    <span class="input-group-addon">Pan / TRC No.*</span>
                    <asp:TextBox ID="txtpanno" MaxLength="10" runat="server" pattern="^[a-zA-Z]{5}[0-9]{4}[a-zA-Z]{1}$" required class="form-control" AutoCompleteType="Disabled" autocomplete="off"
                        placeholder="Enter Pan No." title="Pan No Format- CEQPK4956K"></asp:TextBox>
                    <ajaxToolkit:FilteredTextBoxExtender ID="filterpancard" runat="server" TargetControlID="txtpanno"
                        FilterType="Numbers,UppercaseLetters,LowercaseLetters"></ajaxToolkit:FilteredTextBoxExtender>
                    <ucTooltip:ucTooltip runat="server" tooltipanchor="Eg : CEQPK4956K" ID="ucTooltip9" />

                </div>
            </div>

            <div class="col-xs-12 col-sm-12 col-lg-6" id="divAadhar" runat="server" visible="true">
                <div class="input-group">
                    <span class="input-group-addon">Aadhar No.
                    </span>
                    <asp:TextBox ID="txtAadhar" MaxLength="12" runat="server" CssClass="form-control" AutoCompleteType="Disabled" autocomplete="off"
                        placeholder="Enter Aadhar No" pattern="^([0-9]{12,})$" title="Aadhar No. 12 numbers minimum"></asp:TextBox>
                    <ajaxToolkit:FilteredTextBoxExtender ID="fittxtAadhar" runat="server" TargetControlID="txtAadhar"
                        FilterType="Custom" ValidChars="1234567890"></ajaxToolkit:FilteredTextBoxExtender>
                    </a>
                </div>
            </div>

        </div>
      

        <div class="row">

            <div class="col-xs-12 col-sm-12 col-lg-6" id="divEmail" runat="server">
                <div class="input-group">
                    <span class="input-group-addon">Email ID*
                    </span>
                    <asp:TextBox ID="txtEmail" MaxLength="50" runat="server" CssClass="form-control" AutoCompleteType="Disabled" autocomplete="off"
                        placeholder="ENTER EMAIL" required pattern="[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"></asp:TextBox><a title="Eg : xxxx@gmail.com">
                            <img src="Images/help-icon.png" style="position: absolute; right: 3px; z-index: 100; cursor: pointer; top: 5px">
                        </a>
                </div>
            </div>

            <div class="col-xs-12 col-sm-12 col-lg-5" runat="server" id="divMobile">
                <div class="input-group">
                    <span class="input-group-addon">Mobile*
                    </span>
                    <asp:TextBox ID="txtCountryCode" MaxLength="3" runat="server" Width="15%" CssClass="form-control" AutoCompleteType="Disabled" autocomplete="off"
                        placeholder="+91" value="+91" required />
                    <asp:Label ID="label1" class="form-control" runat="server" Text="-" Style="border-color: white" Width="5%" />
                    <asp:TextBox ID="txtMobile" runat="server" CssClass="form-control" AutoCompleteType="Disabled" autocomplete="off" MaxLength="10"
                     placeholder="Enter Mobile"  step="1" Width="70%" min="0000000000" max="9999999999"></asp:TextBox>
                     <ajaxToolkit:FilteredTextBoxExtender ID="filterPhoneno" runat="server" TargetControlID="txtMobile"
                     FilterType="Numbers,Custom" ValidChars=".,()/-"></ajaxToolkit:FilteredTextBoxExtender>


                </div>
            </div>


        </div>

        <div class="row">
            <div class="col-xs-12 col-sm-12 col-lg-4" id="divshare" runat="server">
                <div class="input-group">
                    <span class="input-group-addon">Share%
                    </span>
                    <asp:TextBox ID="txtshare" TextMode="Number" min="1" MaxLength="3" max="100" runat="server" CssClass="form-control" AutoCompleteType="Disabled" autocomplete="off"
                        placeholder="Enter Share"  onkeypress="return onlyDotsAndNumbers(event)"></asp:TextBox>
                                                                    
                </div>
            </div>

        </div>

        <div class="row">
            <div class="col-xs-12 col-sm-12 col-lg-6">
                <div class="input-group">
                    <span class="input-group-addon">Upload Files
                    </span>
                    <asp:FileUpload ID="FPWork" runat="server" accept="image/gif, image/jpeg, image/png, .pdf" AllowMultiple="false" CssClass="form-control" />

                </div>
            </div>
            <div class="col-xs-12 col-sm-12 col-lg-6">
                <div class="input-group">
                    <div id="valid_msg" style="color: red; vertical-align: bottom" />
                    <span style="color: red; font-size: 15px; font-family: monospace; padding-left: 10px; font-weight: bold;">Note:Upload file within 5MB size</span>
                </div>
            </div>
        </div>


        <div class="row">
            <br />
            <br />
            <br />
            <div class="col-xs-12 col-sm-12 col-lg-5"></div>
            <div class="col-xs-12 col-sm-12 col-lg-2" align="center">
                <div class="input-group">
                    <asp:Button ID="btnAddWork" runat="server" Text="Add" ToolTip="ADD RECORDS" OnClientClick="return WorkFileValidate();" OnClick="btnAddWork_Click" CssClass="btn btn-primary btn-lg" />
                </div>
            </div>
            <div class="col-xs-12 col-sm-12 col-lg-5"></div>
        </div>



        <div class="col-xs-12 col-sm-12 col-lg-12">

            <asp:UpdatePanel ID="UpdatePanel1" UpdateMode="Conditional" runat="server">
                <ContentTemplate>

                    <div align="center" style="padding-top: 20px;" id="divGridWork" runat="server">
                        <div class="row">

                            <div class="col-xs-12 col-sm-12 col-lg-12">

                                <div id="DivWork" onscroll="SetDivPosition(this)" runat="server" style="min-height: 150px; overflow-x: scroll; overflow-y: auto;">

                                    <asp:GridView ID="gvWork" runat="server" CssClass="grid-header" Visible="true" OnRowDataBound="gvWork_RowDataBound"
                                        ShowHeader="true" ShowHeaderWhenEmpty="true" AutoGenerateColumns="false">
                                        <Columns>
                                            <asp:BoundField DataField="FirstName" ItemStyle-Width="200px" HeaderText="First Name" ItemStyle-HorizontalAlign="Left" />
                                            <asp:BoundField DataField="LastName" ItemStyle-Width="200px" HeaderText="Last Name" ItemStyle-HorizontalAlign="Left" />
                                            <asp:BoundField DataField="RelationShip" ItemStyle-Width="140px" HeaderText="Relationship" ItemStyle-HorizontalAlign="Left" />
                                            <asp:BoundField DataField="DOB" ItemStyle-Width="200px" HeaderText="      DOB       " ItemStyle-HorizontalAlign="Left" />
                                            <asp:BoundField DataField="Minor" ItemStyle-Width="120px" HeaderText="Minor" ItemStyle-HorizontalAlign="Left" />
                                            <asp:BoundField DataField="GuardianName" ItemStyle-Width="200px" HeaderText="Guardian Name" ItemStyle-HorizontalAlign="Left" />
                                            <asp:BoundField DataField="GuardianMobile" ItemStyle-Width="200px" HeaderText="Guardian Mobile" ItemStyle-HorizontalAlign="Left" />
                                            <asp:BoundField DataField="PanNo" ItemStyle-Width="200px" HeaderText="Pan No" ItemStyle-HorizontalAlign="Left" />
                                            <asp:BoundField DataField="AadharNo" ItemStyle-Width="200px" HeaderText="Aadhar No" ItemStyle-HorizontalAlign="Left" />
                                            <asp:BoundField DataField="NomineeGender" ItemStyle-Width="80px" HeaderText="Gender" ItemStyle-HorizontalAlign="Left" />
                                            <asp:BoundField DataField="NomineeEmailId" ItemStyle-Width="200px" HeaderText="EmailId" ItemStyle-HorizontalAlign="Left" />
                                            <asp:BoundField DataField="NomineeMobile" ItemStyle-Width="125px" HeaderText="Mobile" ItemStyle-HorizontalAlign="Left" />

                                            <asp:BoundField DataField="Share" ItemStyle-Width="20px" HeaderText="Share(%)" ItemStyle-HorizontalAlign="Left" />


                                            <asp:TemplateField ItemStyle-CssClass="50px" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="text-center" HeaderText="Image">
                                                <ItemTemplate>
                                                    <asp:HyperLink ID="hypworkfile" Text='View' CssClass="btn btn-link" Target="_blank" runat="server" />
                                                    <asp:HiddenField ID="hdnshare" runat="server" Value='<%# Eval("Share") %>' />
                                                    <asp:HiddenField ID="hdnPan" runat="server" Value='<%# Eval("PanNo") %>' />
                                                    <asp:HiddenField ID="hdnAadharNo" runat="server" Value='<%# Eval("AadharNo") %>' />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <%--<asp:TemplateField ItemStyle-CssClass="50px" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="text-center" HeaderText="Docs">
                                                <ItemTemplate>
                                                    <asp:ImageButton ID="imgtWorkfile" formnovalidate="true" ImageUrl="~/Images/file_add.png" runat="server" ImageAlign="Bottom" ToolTip="upload file" OnClick="imgtWorkfile_Click" />
                                                </ItemTemplate>
                                            </asp:TemplateField>--%>
                                            <asp:TemplateField ItemStyle-Width="100px" HeaderText="Edit" ItemStyle-HorizontalAlign="Center">
                                                <ItemTemplate>
                                                    <asp:LinkButton ID="btnEdit" runat="server" class="btn btn-danger" OnClick="btnEdit_Click" title="Edit" CommandArgument='<%# Eval("NomineeId") %>'>
                                <span class="fa fa-edit"></span>
                                                    </asp:LinkButton>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField ItemStyle-Width="100px" HeaderText="Delete" ItemStyle-HorizontalAlign="Center">
                                                <ItemTemplate>
                                                    <asp:LinkButton ID="btnWorkDelete" runat="server" OnClientClick="return ConfirmDelete(this.id);" class="btn btn-danger" OnClick="btnWorkDelete_Click" title="Delete" CommandArgument='<%# Eval("NomineeId") %>'>
                                <span class="fa fa-trash-o"></span>
                                                    </asp:LinkButton>

                                                    <asp:HiddenField ID="hdnNomineeId" runat="server" Value='<%# Eval("NomineeId") %>' />
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                        </Columns>
                                        <EmptyDataTemplate>
                                            <div class="center">
                                                <table cellpadding="0" cellspacing="0" width="50%">

                                                    <tr>
                                                        <td align="center" style="text-align: center;">
                                                            <b>No Records Found</b>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </div>
                                        </EmptyDataTemplate>

                                    </asp:GridView>

                                </div>
                            </div>
                        </div>


                    </div>


                </ContentTemplate>
                <Triggers>
                    <asp:PostBackTrigger ControlID="btnAddWork" />
                </Triggers>
            </asp:UpdatePanel>


        </div>

    </div>
</asp:Panel>
