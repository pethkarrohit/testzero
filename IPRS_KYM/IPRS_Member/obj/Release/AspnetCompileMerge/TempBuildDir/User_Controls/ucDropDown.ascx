<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ucDropDown.ascx.cs" Inherits="IPRS_Member.User_Controls.ucDropDown" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:ScriptManagerProxy ID="sm1" runat="server">
    <Scripts>
        <asp:ScriptReference Path="PopulateDropDown.js" />
    </Scripts>
</asp:ScriptManagerProxy>

<style type="text/css">
    /**/
    .completionListElement {
        visibility: hidden;
        margin: 0px !important;
        background-color: inherit;
        color: black;
        border: solid 1px gray;
        cursor: pointer;
        text-align: left;
        list-style-type: none;
        /*font-family: Verdana;*/
        font-size: 14px;
        padding: 0;
        z-index: 10010 !important;
        max-height: 200px;
        overflow-y: scroll;
    }

    .listItem {
        background-color: white;
        cursor: pointer;
        padding: 1px;
        z-index: 10010 !important;
    }

    .highlightedListItem {
        background-color: #c3ebf9;
        cursor: pointer;
        padding: 1px;
        z-index: 10010 !important;
        max-height: 200px;
        overflow-y: scroll;
    }

    .loading {
        background-image: url(images/loader-SM.gif);
        background-position: right;
        background-repeat: no-repeat;
    }
</style>

<div style="position: relative">
    <asp:TextBox ID="txtDropDown" MaxLength="100" runat="server" AutoCompleteType="Disabled" autocomplete="off" class="form-control" onblur="checkItemSelected(this)" />
    <asp:HiddenField ID="hdnComboId" runat="server" />
    <asp:AutoCompleteExtender MinimumPrefixLength="1" CompletionInterval="10"
        EnableCaching="false" CompletionSetCount="10" TargetControlID="txtDropDown" ID="AutoCompleteExtender1"
        runat="server" FirstRowSelected="false" OnClientItemSelected="ItemSelected"
        CompletionListCssClass="completionListElement"
        ServicePath="User_Controls/PopulateDropDown.asmx"
        CompletionListItemCssClass="listItem"
        CompletionListHighlightedItemCssClass="highlightedListItem">
    </asp:AutoCompleteExtender>
    <asp:HiddenField ID="hdnSelectedValue" runat="server" />
    <asp:Button runat="server" ID="btnPostBack" Text="" Style="display: none;" OnClick="btnPostBack_Click" />
</div>
