<%--<%@ Page Language="VB" AutoEventWireup="false" CodeFile="PaylikeReport.aspx.vb" Inherits="PaylikeReport" %>--%>
<%@ Page Title="" Language="VB" MasterPageFile="~/MasterPageAdmin.master" AutoEventWireup="false" CodeFile="PaylikeReport.aspx.vb" Inherits="PaylikeReport" %>
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
    <asp:DropDownList ID="DropDownListSite" runat="server" CssClass="searchTextBox" Width="150px">
        <asp:ListItem Value="f29e3f49-4de6-4137-9f1e-01d8b63da8ea;581c4abf0dd9477469013410">WeMoveCoins DKK</asp:ListItem>
		<asp:ListItem Value="9308e747-8630-4f2f-9c08-0f8bd4fdbf8e;5a64660d5e549811b9550aa7">WeMoveCoins EUR</asp:ListItem>
		<asp:ListItem Value="b343b60c-12b1-44bc-8ccb-985f90da90f4;5a9421d03d71e50fde61b07e">WeMoveCoins USD</asp:ListItem>
		<asp:ListItem Value="74b9aeac-4648-462f-9f09-5227431db209;5a942201657e400fd8e1ef45">WeMoveCoins GBP</asp:ListItem>
        <asp:ListItem Value="91913afa-ec58-4052-9046-7ce856003fdd;581c4b467cb2057463e8daba">123bitcoin DKK</asp:ListItem>
        <asp:ListItem Value="feb5041d-8171-4e4b-bcba-a8527e8aa9ce;59a033c49b817c6c21050bdd">CoinBox DKK</asp:ListItem>
        <asp:ListItem Value="2a9dd0ca-d593-40b8-803b-463b76518bd3;5a981e205e549811b9559dc0">Guarda EUR</asp:ListItem>
        <asp:ListItem Value="77ba4cbf-cb43-4292-90c4-1b9b4c7c7b44;59e0b4539b817c6c21074d43">Guarda DKK</asp:ListItem>
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
