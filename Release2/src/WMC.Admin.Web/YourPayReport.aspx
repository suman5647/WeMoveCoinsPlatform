<%@ Page Language="vb" MasterPageFile="~/MasterPageAdmin.master" AutoEventWireup="false" CodeBehind="YourPayReport.aspx.vb" Inherits="WMC.Admin.Web.YourPayReport" %>
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
        <asp:ListItem Value="1">WeMoveCoins</asp:ListItem>
        <asp:ListItem Value="2">123bitcoin</asp:ListItem>
    </asp:DropDownList>
    <%--<asp:DropDownList ID="DropDownListType" runat="server" CssClass="searchTextBox">
        <asp:ListItem Value="0">Open</asp:ListItem>
        <asp:ListItem Value="1">Swollen</asp:ListItem>
        <asp:ListItem Value="2">Deleted</asp:ListItem>
    </asp:DropDownList>--%>
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
