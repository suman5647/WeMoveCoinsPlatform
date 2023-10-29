<%--<%@ Page Language="VB" AutoEventWireup="false" CodeFile="PaylikeReport.aspx.vb" Inherits="PaylikeReport" %>--%>
<%@ Page Title="" Language="VB" MasterPageFile="~/MasterPageAdmin.master" AutoEventWireup="false" Inherits="WMC.Admin.Web.PaylikeReport" Codebehind="PaylikeReport.aspx.vb" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
  <asp:ScriptManager runat="server">
  
  </asp:ScriptManager>

  <script>
        function checkDate(sender, args) {
            debugger;
            sender._textbox.set_Value(sender._selectedDate.format(sender._format))
        }
    </script>

    <asp:LinkButton ID="LinkButtonBACK" runat="server" PostBackUrl="~/Default.aspx">Back</asp:LinkButton>
    <br />
    <br />
    <asp:DropDownList ID="DropDownListSite" runat="server" CssClass="searchTextBox">
        <asp:ListItem Value="f29e3f49-4de6-4137-9f1e-01d8b63da8ea;581c4abf0dd9477469013410">WeMoveCoins</asp:ListItem>
        <asp:ListItem Value="91913afa-ec58-4052-9046-7ce856003fdd;581c4b467cb2057463e8daba">123bitcoin</asp:ListItem>
        <asp:ListItem Value="73856349-d3d3-4134-8d50-0438cea5ba77;585a6acfdc0547750d61e58b">Simplekoin</asp:ListItem>
        <asp:ListItem Value="94ec42a7-af75-455d-ab8c-c0c86e4f476b">WesBit</asp:ListItem>
    </asp:DropDownList>
    &nbsp;
    <asp:Label ID="lblStartDate" runat="server" Text="Start date:" Font-Bold="True" />
    &nbsp;<asp:TextBox ID="txtStartDate" runat="server" Width="100px" Font-Bold="True" TabIndex="1"></asp:TextBox>
    <ajaxToolkit:calendarextender ViewStateMode="Enabled" ID="txtStartDateCE" runat="server"
                TargetControlID="txtStartDate"
                PopupPosition="BottomRight" OnClientDateSelectionChanged="checkDate"/>

    &nbsp;&nbsp;&nbsp;

    <asp:Label ID="lblEndDate" runat="server" Text="End date:" Font-Bold="True" />
    &nbsp;<asp:TextBox ID="txtEndDate" runat="server" Width="100px" Font-Bold="True" TabIndex="1"></asp:TextBox>
    <ajaxToolkit:calendarextender ID="txtEndDateCE" runat="server"
                TargetControlID="txtEndDate"
                PopupPosition="BottomRight" OnClientDateSelectionChanged="checkDate"/>

    &nbsp;&nbsp;&nbsp;

    <asp:Button ID="ButtonSearch" runat="server" OnClick="ButtonSearch_Click" Text="Search" />
    &nbsp;
    <asp:Button ID="ButtonExport" runat="server" Text="Export" OnClick="ButtonExport_Click" />
    <br />
    <br />
    <asp:GridView ID="GridView1" runat="server"/>
</asp:Content>
