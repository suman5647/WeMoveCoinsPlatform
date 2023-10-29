<%@ Page Title="" Language="VB" MasterPageFile="~/MasterPageAdmin.master" AutoEventWireup="false" Inherits="WMC.Admin.Web.TransactionView" Codebehind="TransactionView.aspx.vb" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <br />
<asp:LinkButton ID="LinkButtonBACK" runat="server" PostBackUrl="~/Default.aspx">Back</asp:LinkButton>
    <br /><br /><br /><br /><br /><br /><br />
    <asp:GridView ID="GridView1" runat="server" AllowSorting="True" AutoGenerateColumns="False" BackColor="White" BorderColor="#CCCCCC" BorderStyle="None" BorderWidth="1px" CellPadding="3" DataSourceID="SqlDataSourceTransactionView" EnableModelValidation="True">
        <Columns>
            <asp:BoundField DataField="TransactionId" HeaderText="TransactionId" SortExpression="TransactionId" />
            <asp:BoundField DataField="TransactionMethod" HeaderText="TransactionMethod" SortExpression="TransactionMethod" />
            <asp:BoundField DataField="TransactionExtRef" HeaderText="TransactionExtRef" SortExpression="TransactionExtRef" />
            <asp:BoundField DataField="TransactionAmount" HeaderText="TransactionAmount" SortExpression="TransactionAmount" />
            <asp:BoundField DataField="TransactionCurrency" HeaderText="TransactionCurrency" SortExpression="TransactionCurrency" />
            <asp:BoundField DataField="TransactionInfo" HeaderText="TransactionInfo" SortExpression="TransactionInfo" />
            <asp:BoundField DataField="TransactionCompleted" HeaderText="TransactionCompleted" SortExpression="TransactionCompleted" />
            <asp:BoundField DataField="FromAccount" HeaderText="FromAccount" SortExpression="FromAccount" />
            <asp:BoundField DataField="ToAccount" HeaderText="ToAccount" SortExpression="ToAccount" />
            <asp:BoundField DataField="Reconsiled" HeaderText="Reconsiled" SortExpression="Reconsiled" />
            <asp:BoundField DataField="Exported" HeaderText="Exported" SortExpression="Exported" />
            <asp:BoundField DataField="OrderNumber" HeaderText="OrderNumber" SortExpression="OrderNumber" />
            <asp:BoundField DataField="OrderStatus" HeaderText="OrderStatus" SortExpression="OrderStatus" />
            <asp:BoundField DataField="PaymentType" HeaderText="PaymentType" SortExpression="PaymentType" />
            <asp:BoundField DataField="CryptoAddress" HeaderText="CryptoAddress" SortExpression="CryptoAddress" />
            <asp:BoundField DataField="OrderType" HeaderText="OrderType" SortExpression="OrderType" />
            <asp:BoundField DataField="Quoted" HeaderText="Quoted" SortExpression="Quoted" />
            <asp:BoundField DataField="Rate" HeaderText="Rate" ReadOnly="True" SortExpression="Rate" />
            <asp:BoundField DataField="QuoteSource" HeaderText="QuoteSource" ReadOnly="True" SortExpression="QuoteSource" />
            <asp:BoundField DataField="OrderAmount" HeaderText="OrderAmount" SortExpression="OrderAmount" />
            <asp:BoundField DataField="OrderCurrency" HeaderText="OrderCurrency" SortExpression="OrderCurrency" />
            <asp:BoundField DataField="CommissionProcent" HeaderText="CommissionProcent" SortExpression="CommissionProcent" />
            <asp:BoundField DataField="OrderNote" HeaderText="OrderNote" SortExpression="OrderNote" />
            <asp:BoundField DataField="UserId" HeaderText="UserId" SortExpression="UserId" />
            <asp:BoundField DataField="SiteId" HeaderText="SiteId" SortExpression="SiteId" />
            <asp:BoundField DataField="UserFirstName" HeaderText="UserFirstName" SortExpression="UserFirstName" />
            <asp:BoundField DataField="UserLastName" HeaderText="UserLastName" SortExpression="UserLastName" />
            <asp:BoundField DataField="PhoneCode" HeaderText="PhoneCode" SortExpression="PhoneCode" />
            <asp:BoundField DataField="Phone" HeaderText="Phone" SortExpression="Phone" />
            <asp:BoundField DataField="Email" HeaderText="Email" SortExpression="Email" />
            <asp:BoundField DataField="Created" HeaderText="Created" SortExpression="Created" />
            <asp:CheckBoxField DataField="Newsletter" HeaderText="Newsletter" SortExpression="Newsletter" />
        </Columns>
        <FooterStyle BackColor="White" ForeColor="#000066" />
        <HeaderStyle BackColor="#006699" Font-Bold="True" ForeColor="White" />
        <PagerStyle BackColor="White" ForeColor="#000066" HorizontalAlign="Left" />
        <RowStyle ForeColor="#000066" />
        <SelectedRowStyle BackColor="#669999" Font-Bold="True" ForeColor="White" />
    </asp:GridView>   
    <asp:SqlDataSource ID="SqlDataSourceTransactionView" runat="server" ConnectionString="<%$ ConnectionStrings:LocalConnectionString %>" SelectCommand="SELECT TransactionId, TransactionMethod, TransactionExtRef, TransactionAmount, TransactionCurrency, TransactionInfo, TransactionCompleted, FromAccount, ToAccount, Reconsiled, Exported, OrderNumber, OrderStatus, PaymentType, CryptoAddress, OrderType, Quoted, Rate, QuoteSource, OrderAmount, OrderCurrency, CommissionProcent, OrderNote, UserId, SiteId, UserFirstName, UserLastName, PhoneCode, Phone, Email, Created, Newsletter FROM TransactionDetail ORDER BY TransactionId DESC">
    </asp:SqlDataSource>   
    </asp:Content>

