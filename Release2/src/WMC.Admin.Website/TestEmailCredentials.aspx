<%@ Page Title="" Language="VB" MasterPageFile="~/MasterPageAdmin.master" AutoEventWireup="false" CodeFile="TestEmailCredentials.aspx.vb" Inherits="TestEmailCredentials" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:LinkButton ID="LinkButtonBACK" runat="server" PostBackUrl="~/Default.aspx">Back</asp:LinkButton>
    <br />
    <br />
    <p class="verdana13">
        Send test-email to: 
    <asp:TextBox ID="TextBoxSendTo" runat="server" Width="300px"></asp:TextBox>
    </p>
    <p class="verdana13">
        &nbsp;</p>
        <p class="verdana13">
            From-address: 
    <asp:TextBox ID="TextBoxFromAddress" runat="server" Width="300px"></asp:TextBox>
    </p>
        <p class="verdana13">
            Password: 
    <asp:TextBox ID="TextBoxPassword" runat="server" Width="300px"></asp:TextBox>
    </p>
        <p class="verdana13">
            Port: 
    <asp:TextBox ID="TextBoxPort" runat="server" Width="300px"></asp:TextBox>
    </p>
        <p class="verdana13">
            Host: 
    <asp:TextBox ID="TextBoxHost" runat="server" Width="300px"></asp:TextBox>
    </p>
        <p class="verdana13">
        &nbsp;<asp:CheckBox ID="CheckBoxEnableSSL" runat="server" Text="EnableSsl" />
    </p>
    <p class="verdana13">
        <asp:Button ID="ButtonSend" runat="server" Text="Send Now!" />
    &nbsp;
        <asp:Label ID="LabelResponse" runat="server" Text="Label" Visible="False"></asp:Label>
    </p>
</asp:Content>

