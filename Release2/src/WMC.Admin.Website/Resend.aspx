<%@ Page Title="" Language="VB" MasterPageFile="~/MasterPageAdmin.master"  AutoEventWireup="false" CodeFile="Resend.aspx.vb" Inherits="Resend" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
<asp:LinkButton ID="LinkButtonBACK" runat="server" PostBackUrl="~/Default.aspx">Back</asp:LinkButton>
    <br />
    <br />
    <br />
    <div class="verdana13">
        <p>Resend email for completed order</p>
        Send mail by:
        <asp:DropDownList ID="DropDownListWhereType" runat="server" CssClass="DropdownList">
            <asp:ListItem>Number</asp:ListItem>
            <asp:ListItem>OrderId</asp:ListItem>
        </asp:DropDownList>&nbsp;
        <asp:TextBox ID="TextBoxValue" runat="server" Width="200px" onkeydown = "return (!(event.keyCode>=65) && event.keyCode!=32);"></asp:TextBox>
        &nbsp;<asp:Button ID="ResendButton" runat="server" Text="Send Email" />
        <br />
        <br />
        <asp:Label ID="lblMessage" runat="server" Text="" Font-Names="Arial"></asp:Label>
    </div>
</asp:Content>