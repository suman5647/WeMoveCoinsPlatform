<%@ Page Title="" Language="VB" MasterPageFile="~/MasterPageAdmin.master" AutoEventWireup="false" CodeFile="RiskList.aspx.vb" Inherits="TransactionList" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:LinkButton ID="LinkButtonBACK" runat="server" PostBackUrl="~/Default.aspx">Back</asp:LinkButton>
    <br />
    <br />
    
    <asp:DropDownList ID="DropDownListTop" runat="server" AutoPostBack="True">
        <asp:ListItem Value="10">TOP10</asp:ListItem>
        <asp:ListItem Value="20">TOP20</asp:ListItem>
        <asp:ListItem Value="300">TOP300</asp:ListItem>
        <asp:ListItem Value="ALL">ALL!!</asp:ListItem>
    </asp:DropDownList>
    <p>
        <asp:GridView ID="GridViewRiskScore" runat="server">
        </asp:GridView>
    </p>
</asp:Content>

