<%@ Page Title="" Language="VB" MasterPageFile="~/MasterPageAdmin.master" AutoEventWireup="false" Inherits="WMC.Admin.Web.TransactionList" Codebehind="TransactionList.aspx.vb" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:LinkButton ID="LinkButtonBACK" runat="server" PostBackUrl="~/Default.aspx">Back</asp:LinkButton>
    <br />
    <br />
    <asp:DropDownList ID="DropDownListMonth" runat="server" AutoPostBack="True">
    </asp:DropDownList>
    <asp:DropDownList ID="DropDownListYear" runat="server" AutoPostBack="True">
        <asp:ListItem>2016</asp:ListItem>
        <asp:ListItem>2017</asp:ListItem>
        <asp:ListItem>2018</asp:ListItem>
    </asp:DropDownList>
    <p>
        <asp:GridView ID="GridView1" runat="server" AllowSorting="True" AutoGenerateColumns="False" DataSourceID="SqlDataSource1">
            <Columns>
                <asp:BoundField DataField="Id" HeaderText="Id" SortExpression="Id" />
                <asp:BoundField DataField="Number" HeaderText="Number" SortExpression="Number" />
                <asp:BoundField DataField="Text" HeaderText="Text" SortExpression="Text" />
                <asp:BoundField DataField="ExtRef" HeaderText="ExtRef" SortExpression="ExtRef" />
                <asp:BoundField DataField="Amount" HeaderText="Amount" SortExpression="Amount" />
                <asp:BoundField DataField="Code" HeaderText="Code" SortExpression="Code" />
                <asp:BoundField DataField="Rate" HeaderText="Rate" SortExpression="Rate" />
                <asp:BoundField DataField="RateBase" HeaderText="RateBase" SortExpression="RateBase" />
                <asp:BoundField DataField="RateHome" HeaderText="RateHome" SortExpression="RateHome" />
                <asp:BoundField DataField="RateBooks" HeaderText="RateBooks" SortExpression="RateBooks" />
                <asp:BoundField DataField="CommissionProcent" HeaderText="CommissionProcent" SortExpression="CommissionProcent" />
                <asp:BoundField DataField="Completed" HeaderText="Completed" SortExpression="Completed" />
                <asp:BoundField DataField="FromAccountID" HeaderText="FromAccountID" SortExpression="FromAccountID" />
                <asp:BoundField DataField="FromAccount" HeaderText="FromAccount" SortExpression="FromAccount" />
                <asp:BoundField DataField="ToAccountID" HeaderText="ToAccountID" SortExpression="ToAccountID" />
                <asp:BoundField DataField="ToAccount" HeaderText="ToAccount" SortExpression="ToAccount" />
            </Columns>
        </asp:GridView>
        <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:LocalConnectionString %>" SelectCommand="SELECT Id, Number, Text, ExtRef, Amount, Code, Rate, CommissionProcent, Completed, FromAccountID, FromAccount, ToAccountID, ToAccount, RateBase, RateHome, RateBooks FROM TransactionList WHERE (MONTH(Completed) = @Month) AND (YEAR(Completed) = @Year)">
            <SelectParameters>
                <asp:ControlParameter ControlID="DropDownListMonth" Name="Month" PropertyName="SelectedValue" />
                <asp:ControlParameter ControlID="DropDownListYear" Name="Year" PropertyName="SelectedValue" />
            </SelectParameters>
        </asp:SqlDataSource>
    </p>
</asp:Content>

