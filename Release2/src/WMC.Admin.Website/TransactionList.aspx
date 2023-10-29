<%@ Page Title="" Language="VB" MasterPageFile="~/MasterPageAdmin.master" AutoEventWireup="false" CodeFile="TransactionList.aspx.vb" Inherits="TransactionList" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <div>
        <div>
            <asp:LinkButton ID="LinkButtonBACK" runat="server" PostBackUrl="~/Default.aspx">Back</asp:LinkButton>
            <br />
            <br />
        </div>
        <div>
            <asp:DropDownList ID="DropDownListMonth" runat="server" AutoPostBack="True">
            </asp:DropDownList>
            <asp:DropDownList ID="DropDownListYear" runat="server" AutoPostBack="True">
                <asp:ListItem>2016</asp:ListItem>
                <asp:ListItem>2017</asp:ListItem>
                <asp:ListItem>2018</asp:ListItem>
                <asp:ListItem>2019</asp:ListItem>
                <asp:ListItem>2020</asp:ListItem>
            </asp:DropDownList>
        </div>
        <div>
            <asp:GridView ID="GridView1" runat="server" AllowSorting="True" AutoGenerateColumns="False" DataSourceID="SqlDataSource1" EmptyDataText="No records found">
                <Columns>
                    <asp:BoundField DataField="Id" HeaderText="Id" SortExpression="Id" />
                    <asp:BoundField DataField="Number" HeaderText="Number" SortExpression="Number" />
                    <asp:BoundField DataField="Text" HeaderText="Text" SortExpression="Text" />
                    <asp:BoundField DataField="ExtRef" HeaderText="ExtRef" SortExpression="ExtRef" />
                    <asp:BoundField DataField="Amount" HeaderText="Amount" SortExpression="Amount" />
                    <asp:BoundField DataField="Code" HeaderText="Code" SortExpression="Code" />
                    <asp:BoundField DataField="AmountDKK" HeaderText="AmountDKK" SortExpression="AmountDKK" />
                    <asp:BoundField DataField="Rate" HeaderText="Rate" SortExpression="Rate" />
                    <asp:BoundField DataField="RateBase" HeaderText="RateBase" SortExpression="RateBase" />
                    <asp:BoundField DataField="RateHome" HeaderText="RateHome" SortExpression="RateHome" />
                    <asp:BoundField DataField="RateBooks" HeaderText="RateBooks" SortExpression="RateBooks" />
                    <asp:BoundField DataField="CommissionProcent" HeaderText="CommissionProcent" SortExpression="CommissionProcent" />
                    <asp:BoundField DataField="Completed" HeaderText="Completed" SortExpression="Completed" />
                    <asp:BoundField DataField="FromAccount" HeaderText="FromAccount" SortExpression="FromAccount" />
                    <asp:BoundField DataField="ToAccount" HeaderText="ToAccount" SortExpression="ToAccount" />
                    <asp:BoundField DataField="SiteId" HeaderText="SiteId" SortExpression="SiteId" />
                    <asp:BoundField DataField="PartnerId" HeaderText="PartnerId" SortExpression="PartnerId" />
                </Columns>

            </asp:GridView>
            <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:LocalConnectionString %>" SelectCommand="SELECT dbo.[Transaction].Id, dbo.[Order].Number, dbo.TransactionType.Text, dbo.[Transaction].ExtRef, dbo.[Transaction].Amount, dbo.Currency.Code, (CASE WHEN (Code LIKE 'BTC') THEN (dbo.[Transaction].Amount * RateBase * RateBooks) ELSE ((dbo.[Transaction].Amount / RateHome) * RateBooks) END) AS AmountDKK, dbo.[Order].Rate, RateBase, RateHome, RateBooks,dbo.[Order].CommissionProcent, dbo.[Transaction].Completed, dbo.Account.Text AS FromAccount, Account_1.Text AS ToAccount, SiteId, PartnerId FROM dbo.[Transaction] INNER JOIN dbo.[Order] ON dbo.[Transaction].OrderId = dbo.[Order].Id INNER JOIN dbo.Currency ON dbo.[Transaction].Currency = dbo.Currency.Id LEFT JOIN dbo.Account ON dbo.[Transaction].FromAccount = dbo.Account.Id LEFT JOIN dbo.TransactionType ON dbo.[Transaction].Type = dbo.TransactionType.Id LEFT JOIN dbo.Account AS Account_1 ON dbo.[Transaction].ToAccount = Account_1.Id WHERE (dbo.[Order].Status = 17) AND (MONTH(Completed) = @Month) AND (YEAR(Completed) = @YEAR)">
                <SelectParameters>
                    <asp:ControlParameter ControlID="DropDownListMonth" Name="Month" PropertyName="SelectedValue" />
                    <asp:ControlParameter ControlID="DropDownListYear" Name="Year" PropertyName="SelectedValue" />
                </SelectParameters>
            </asp:SqlDataSource>
            <div>
                Day:<asp:DropDownList ID="ddlDay" runat="server"  AutoPostBack="True"/>
                Month:<asp:DropDownList ID="ddlMonth" runat="server"  AutoPostBack="True"/>
                Year:<asp:DropDownList ID="ddlYear" runat="server" />
            </div>
            <div>
                <asp:Button ID="btntoCsv" runat="server" Text="Export to CSV" OnClick="btntoCsv_Click"></asp:Button>
                <asp:CheckBox ID="chkIncludeHeader" runat="server" Text="Include Header?" />
            </div>
        </div>
    </div>
</asp:Content>

