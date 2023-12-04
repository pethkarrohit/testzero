<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCDocUpload.ascx.cs" Inherits="IPRS_Member.User_Controls.UCDocUpload" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
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
</style>
<div class="col-xs-12 col-sm-12 col-lg-12">
    <asp:HiddenField ID="hdnUcRecordId" runat="server" Value="0" />
    <asp:HiddenField ID="hdnDocSubtype" runat="server" Value="" />
    <asp:HiddenField ID="hdnfilecount" runat="server" Value="0" />
    <asp:UpdatePanel ID="upnlGrid" UpdateMode="Conditional" runat="server">
        <ContentTemplate>
            <div id="dvScroll1" onscroll="SetDivPosition(this)" runat="server" style="overflow: auto; overflow-x: hidden;">


                <asp:GridView CssClass="table table-bordered dt-responsive nowrap" ID="grdDocumentsPreApproval" AutoGenerateColumns="false"
                    ViewStateMode="Enabled" runat="server" Width="100%" ShowHeaderWhenEmpty="false" OnRowDataBound="grdDocumentsPreApproval_RowDataBound">
                    <Columns>
                        <asp:TemplateField HeaderText="Sr No" ItemStyle-Width="10%" ItemStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="top" HeaderStyle-HorizontalAlign="Center" HeaderStyle-CssClass="text-center">
                            <ItemTemplate>
                                <%# ((GridViewRow)Container).RowIndex + 1%>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="List of Documents" ItemStyle-Width="45%" HeaderStyle-CssClass="text-center">
                            <ItemTemplate>
                                <asp:HiddenField ID="hdnDocLookupId" Value='<%#Eval("DocumentLookupId") %>' runat="server" />
                                <asp:HiddenField ID="hdnIsCompulsary" Value='<%#Eval("IsCompulsary") %>' runat="server" />
                                <asp:HiddenField ID="hdnUploadedCount" Value='<%#Eval("UploadedCount") %>' runat="server" />
                                <asp:Label ID="lblDocumentName" Text='<%#Eval("DocumentName") %>' runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField ItemStyle-Width="30%" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="text-center" HeaderText="Note : Only jpeg, jpg , gif, png, pdf,doc,xls,xlsx allowed">
                            <ItemTemplate>
                                <asp:UpdatePanel ID="upnlfile" UpdateMode="Conditional" runat="server">
                                    <ContentTemplate>
                                        <asp:FileUpload ID="FileUpload1" Width="99%" runat="server" AllowMultiple="true" />
                                        <br />
                                        <span id="spfileErrormsg"></span>
                                        <asp:Button ID="btnfileupload" CssClass="btn btn-success btn-sm" Text="Upload" OnClientClick="return DocFileValidate(this)" OnClick="btnfileupload_Click" runat="server" ValidationGroup="upload" />
                                    </ContentTemplate>
                                    <Triggers>
                                        <asp:PostBackTrigger ControlID="btnfileupload" />
                                    </Triggers>
                                </asp:UpdatePanel>

                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField ItemStyle-CssClass="15%" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="text-center" HeaderText="Note : Only jpeg, jpg , gif, png, pdf,doc,xls,xlsx allowed">
                            <ItemTemplate>

                                <asp:LinkButton ID="btnViewUploadedFiles" runat="server" title="Click to View Files" OnClick="btnViewUploadedFiles_Click" Text="VIEW UPLOADED FILES" CssClass="btn btn-link" Style="padding: 0 10px">
                                </asp:LinkButton>
                                <br />
                                Upload Count :
                              <asp:Label ID="LblFilecount" runat="server" Text='<%# Eval("UploadedCount") %>'></asp:Label>

                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>

                <asp:LinkButton ID="lnkviewfiles" runat="server"></asp:LinkButton>


                <asp:Panel ID="pnlViewFiles" Width="70%" runat="server" CssClass="modalPopup"
                    Style="display: none; z-index: 1070;">
                    <div style="overflow-y: auto; overflow-x: hidden; max-height: 550px;">
                        <div class="modal-header">
                            <asp:Label ID="lblHeadingofFiles" runat="server" CssClass="modal-title"></asp:Label>
                        </div>
                        <div class="modal-body">
                            <div class="form-group">
                                <div class="row">


                                    <asp:GridView CssClass="grid-header" Width="100%" ID="grdViewUploadedFiles" AutoGenerateColumns="false"
                                        ViewStateMode="Enabled" runat="server">
                                        <EmptyDataRowStyle HorizontalAlign="Center" />
                                        <EmptyDataTemplate>
                                            No files have been uploaded
                                        </EmptyDataTemplate>
                                        <Columns>
                                            <asp:TemplateField HeaderText="File Name" ItemStyle-Width="60%" HeaderStyle-CssClass="">
                                                <ItemTemplate>
                                                    <asp:HiddenField ID="hdnDocumentLookupId" Value='<%#Eval("DocumentLookupId") %>' runat="server" />
                                                    <asp:HiddenField ID="hdnAccountDocId" Value='<%#Eval("AccountDocId") %>' runat="server" />
                                                    <asp:HiddenField ID="hdnFileName" Value='<%#Eval("DocFileName") %>' runat="server" />
                                                    <asp:HyperLink Text='<%#Eval("DocumentName") %>' CssClass="btn btn-link" Target="_blank" NavigateUrl='<%#"../MemberRegDocs/"+Eval("DocFileName") %>' runat="server" />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Date" ItemStyle-Width="20%" HeaderStyle-CssClass="" ItemStyle-HorizontalAlign="Center">
                                                <ItemTemplate><%#Eval("ModifedDate") %></ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField>
                                                <ItemStyle HorizontalAlign="Center" />
                                                <ItemTemplate>
                                                    <asp:LinkButton ID="ibtFileDelete" runat="server" OnClientClick="return ConfirmDelete(this.id);" CssClass="btn btn-danger" OnClick="ibtFileDelete_Click">
                                                                 <span class="fa fa-trash-o"></span>
                                                    </asp:LinkButton>

                                                </ItemTemplate>
                                            </asp:TemplateField>
                                        </Columns>
                                    </asp:GridView>
                                </div>
                            </div>
                            <div align="center" class="modal-footer">
                                <div class="row">
                                    <div class="col-md-12">

                                        <button id="btnClosefiles" runat="server" formnovalidate="true" class="btn btn-primary">
                                            Cancel
                                        </button>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </asp:Panel>

                <asp:ModalPopupExtender ID="modViewFiles" runat="server" PopupControlID="pnlViewFiles"
                    TargetControlID="lnkviewfiles" BehaviorID="modViewFiles"
                    BackgroundCssClass="modalBackground">
                </asp:ModalPopupExtender>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</div>

