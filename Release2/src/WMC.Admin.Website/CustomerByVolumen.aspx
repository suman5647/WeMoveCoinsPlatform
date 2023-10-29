<%@ Page Title="" Language="VB" MasterPageFile="~/MasterPageAdmin.master" AutoEventWireup="false" CodeFile="CustomerByVolumen.aspx.vb" Inherits="TransactionList" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:LinkButton ID="LinkButtonBACK" runat="server" PostBackUrl="~/Default.aspx">Back</asp:LinkButton>
    <br />
    <br />
    <br />
    <asp:DropDownList ID="DropDownListCountryList" runat="server" DataSourceID="SqlDataSourceCountry" DataTextField="Text" DataValueField="Id" AutoPostBack="True">
    </asp:DropDownList>
        <asp:GridView ID="GridView1" runat="server" AllowSorting="True" AutoGenerateColumns="False" DataSourceID="SqlDataSource1">
            <Columns>
                <asp:BoundField DataField="AmountEUR" HeaderText="AmountEUR" SortExpression="AmountEUR" DataFormatString="{0:n0}" />
               <asp:BoundField DataField="Navn" HeaderText="Navn" SortExpression="Navn" />
                <asp:BoundField DataField="Email" HeaderText="Email" SortExpression="Email" />
                <asp:BoundField DataField="Telefon" HeaderText="Telefon" SortExpression="Telefon" />
                <asp:BoundField DataField="Land" HeaderText="Land" SortExpression="Land" />
                <asp:BoundField DataField="AntalOrdre" HeaderText="AntalOrdre" SortExpression="AntalOrdre" />
            </Columns>
        </asp:GridView>
    <p>
        <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:LocalConnectionString %>" SelectCommand="SELECT [Order].Id, [Order].Number, [Order].Status, [Order].Rate, [Order].Amount, [Order].RateHome, AMOUNT / RateHome AS [AmountEUR], [Order].CurrencyCode, [user].Id AS UserID INTO #temp_filteredOrders FROM [Order] LEFT JOIN [User] ON [Order].UserId = [User].Id WHERE [Order].Status = 17 AND [USER].UserType = 1 AND year([order].Quoted) &gt;= 2017 SELECT TOP 1000 x.Sum_AmountEUR AS AmountEUR, x.UserID, [user].Fname AS Navn, [user].Email, [user].Phone AS Telefon, country.Text AS Land, ordercount.OrderCount AS AntalOrdre FROM (SELECT sum(AmountEUR) AS Sum_AmountEUR, UserID FROM #temp_filteredOrders filteredOrders GROUP BY UserID) x LEFT JOIN [user] ON [user].Id = x.UserID LEFT JOIN country ON country.Id = [user].CountryId LEFT JOIN (SELECT userid, count(id) AS OrderCount FROM #temp_filteredOrders GROUP BY userid) orderCount ON orderCount.UserID = x.userId WHERE country.Id = @countryId ORDER BY Sum_AmountEUR DESC DROP TABLE #temp_filteredOrders">
            <SelectParameters>
                <asp:ControlParameter ControlID="DropDownListCountryList" Name="countryId" PropertyName="SelectedValue" />
            </SelectParameters>
        </asp:SqlDataSource>
    </p>
        <asp:SqlDataSource ID="SqlDataSourceCountry" runat="server" ConnectionString="<%$ ConnectionStrings:LocalConnectionString %>" SelectCommand="SELECT DISTINCT Country.Code, Country.Text, Country.Id FROM [Order] INNER JOIN [User] ON [Order].UserId = [User].Id INNER JOIN Country ON [User].CountryId = Country.Id WHERE ([Order].Status = 17) ORDER BY Country.Text">
        </asp:SqlDataSource>
    </asp:Content>

