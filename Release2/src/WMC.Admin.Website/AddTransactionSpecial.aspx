<%@ Page Title="" Language="VB" MasterPageFile="~/MasterPageAdmin.master" AutoEventWireup="false" CodeFile="AddTransactionSpecial.aspx.vb" Inherits="AddTransaction" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:LinkButton ID="LinkButtonBACK" runat="server" PostBackUrl="~/Default.aspx">Back</asp:LinkButton>
    <br />
    <br />





    <br />
    <br />
    <div class="verdana13">
        OrderID:
        <asp:TextBox ID="TextBoxOrderID" runat="server"></asp:TextBox>
        &nbsp;<asp:Button ID="ButtonFindByOrderID" runat="server" Text="Find" />
        <br />
        <br />
    </div>
    <asp:GridView ID="GridViewOrder" runat="server" AllowPaging="True" AllowSorting="True" AutoGenerateColumns="False" DataKeyNames="Id" DataSourceID="SqlDataSourceOrder" BackColor="White" BorderColor="#336666" BorderStyle="Double" BorderWidth="3px" CellPadding="4" EmptyDataText="No orders awaits approval" GridLines="Horizontal">
        <Columns>
            <asp:CommandField ShowSelectButton="True" SelectText="Review" />
            <asp:BoundField DataField="Id" HeaderText="Id" ReadOnly="True" SortExpression="Id" Visible="False" />
            <asp:BoundField DataField="Number" HeaderText="Order Number" SortExpression="Number" ReadOnly="True" />
            <asp:BoundField DataField="Amount" HeaderText="Amount" SortExpression="Amount" DataFormatString="{0:0.00}" ReadOnly="True" />
            <asp:BoundField DataField="CurrencyId" HeaderText="CurrencyId" SortExpression="CurrencyId" ReadOnly="True" />
            <asp:BoundField DataField="CryptoAddress" HeaderText="CryptoAddress" SortExpression="CryptoAddress" ReadOnly="True" Visible="False" />
            <asp:BoundField DataField="Quoted" HeaderText="Quoted" SortExpression="Quoted" ReadOnly="True" />
            <asp:BoundField DataField="Name" HeaderText="Name" SortExpression="Name" ReadOnly="True" />
            <asp:BoundField DataField="Email" HeaderText="Email" SortExpression="Email" ReadOnly="True" />
            <asp:BoundField DataField="IP" HeaderText="IP" SortExpression="IP" ReadOnly="True" />
            <asp:BoundField DataField="BTCAmount" HeaderText="BTCAmount" SortExpression="BTCAmount" />
            <asp:BoundField DataField="TransactionHash" HeaderText="TransactionHash" SortExpression="TransactionHash" />
            <asp:CommandField ShowEditButton="True" />
        </Columns>
        <FooterStyle BackColor="White" ForeColor="#333333" />
        <HeaderStyle BackColor="#336666" Font-Bold="True" ForeColor="White" />
        <PagerStyle BackColor="#336666" ForeColor="White" HorizontalAlign="Center" />
        <RowStyle BackColor="White" ForeColor="#333333" />
        <SelectedRowStyle BackColor="#339966" Font-Bold="True" ForeColor="White" />
        <SortedAscendingCellStyle BackColor="#F7F7F7" />
        <SortedAscendingHeaderStyle BackColor="#487575" />
        <SortedDescendingCellStyle BackColor="#E5E5E5" />
        <SortedDescendingHeaderStyle BackColor="#275353" />
    </asp:GridView>
    <br />
    <br />
    <p class="verdana13">
        Transaction on Order:
    </p>
    <asp:GridView ID="GridViewTransaction" runat="server" AutoGenerateColumns="False" DataSourceID="SqlDataSourceTransaction" EmptyDataText="No records" BackColor="#DEBA84" BorderColor="#DEBA84" BorderStyle="None" BorderWidth="1px" CellPadding="3" CellSpacing="2" DataKeyNames="Id">
        <Columns>
            <asp:CommandField ShowEditButton="True" />
            <asp:BoundField DataField="Id" HeaderText="Id" SortExpression="Id" InsertVisible="False" ReadOnly="True" />
            <asp:BoundField DataField="OrderId" HeaderText="OrderId" SortExpression="OrderId" />
            <asp:BoundField DataField="MethodId" HeaderText="MethodId" SortExpression="MethodId" />
            <asp:BoundField DataField="Type" HeaderText="Type" SortExpression="Type" />
            <asp:BoundField DataField="ExtRef" HeaderText="ExtRef" SortExpression="ExtRef" />
            <asp:BoundField DataField="Amount" HeaderText="Amount" SortExpression="Amount" />
            <asp:BoundField DataField="Currency" HeaderText="Currency" SortExpression="Currency" />
            <asp:BoundField DataField="Info" HeaderText="Info" SortExpression="Info" />
            <asp:BoundField DataField="Completed" HeaderText="Completed" SortExpression="Completed" />
            <asp:BoundField DataField="FromAccount" HeaderText="FromAccount" SortExpression="FromAccount" />
            <asp:BoundField DataField="ToAccount" HeaderText="ToAccount" SortExpression="ToAccount" />
            <asp:BoundField DataField="Reconsiled" HeaderText="Reconsiled" SortExpression="Reconsiled" />
            <asp:BoundField DataField="Exported" HeaderText="Exported" SortExpression="Exported" />
        </Columns>
        <FooterStyle BackColor="#F7DFB5" ForeColor="#8C4510" />
        <HeaderStyle BackColor="#A55129" Font-Bold="True" ForeColor="White" />
        <PagerStyle ForeColor="#8C4510" HorizontalAlign="Center" />
        <RowStyle BackColor="#FFF7E7" ForeColor="#8C4510" />
        <SelectedRowStyle BackColor="#738A9C" Font-Bold="True" ForeColor="White" />
        <SortedAscendingCellStyle BackColor="#FFF1D4" />
        <SortedAscendingHeaderStyle BackColor="#B95C30" />
        <SortedDescendingCellStyle BackColor="#F1E5CE" />
        <SortedDescendingHeaderStyle BackColor="#93451F" />
    </asp:GridView>
    <br />
    <p class="verdana13">
        Transaction on Order:
    </p>
    <asp:DetailsView ID="DetailsViewAddTransaction" runat="server" AutoGenerateRows="False" BackColor="#DEBA84" BorderColor="#DEBA84" BorderStyle="None" BorderWidth="1px" CellPadding="3" CellSpacing="2" DataKeyNames="Id" DataSourceID="SqlDataSourceTransaction" Height="50px" Width="557px" DefaultMode="Insert">
        <EditRowStyle BackColor="#738A9C" Font-Bold="True" ForeColor="White" />
        <Fields>
            <asp:BoundField DataField="Id" HeaderText="Id" InsertVisible="False" ReadOnly="True" SortExpression="Id" />
            <asp:TemplateField HeaderText="OrderId" SortExpression="OrderId" InsertVisible="False">
                <EditItemTemplate>
                    <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("OrderId") %>'></asp:TextBox>
                </EditItemTemplate>
                <InsertItemTemplate>
                    <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("OrderId") %>'></asp:TextBox>
                </InsertItemTemplate>
                <ItemTemplate>
                    <asp:Label ID="Label1" runat="server" Text='<%# Bind("OrderId") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="MethodId" HeaderText="MethodId" SortExpression="MethodId" InsertVisible="False" />
            <asp:BoundField DataField="Type" HeaderText="Type" SortExpression="Type" InsertVisible="False" />
            <asp:TemplateField HeaderText="ExtRef" SortExpression="ExtRef">
                <EditItemTemplate>
                    <asp:TextBox ID="TextBox2" runat="server" Text='<%# Bind("ExtRef") %>'></asp:TextBox>
                </EditItemTemplate>
                <InsertItemTemplate>
                    <asp:TextBox ID="TextBox2" runat="server" Text='<%# Bind("ExtRef") %>' Width="600px"></asp:TextBox>
                </InsertItemTemplate>
                <ItemTemplate>
                    <asp:Label ID="Label2" runat="server" Text='<%# Bind("ExtRef") %>'></asp:Label>
                </ItemTemplate>
                <ItemStyle Width="700px" />
            </asp:TemplateField>
            <asp:BoundField DataField="Amount" HeaderText="Amount (BTC):" SortExpression="Amount" />
            <asp:BoundField DataField="Currency" HeaderText="Currency" SortExpression="Currency" InsertVisible="False" />
            <asp:BoundField DataField="Info" HeaderText="Info" SortExpression="Info" />
            <asp:BoundField DataField="Completed" HeaderText="Completed" SortExpression="Completed" InsertVisible="False" />
            <asp:BoundField DataField="FromAccount" HeaderText="FromAccount" SortExpression="FromAccount" InsertVisible="False" />
            <asp:BoundField DataField="ToAccount" HeaderText="ToAccount" SortExpression="ToAccount" InsertVisible="False" />
            <asp:CommandField ShowInsertButton="True" InsertText="Insert Payout record" />
        </Fields>
        <FooterStyle BackColor="#F7DFB5" ForeColor="#8C4510" />
        <HeaderStyle BackColor="#A55129" Font-Bold="True" ForeColor="White" />
        <PagerStyle ForeColor="#8C4510" HorizontalAlign="Center" />
        <RowStyle BackColor="#FFF7E7" ForeColor="#8C4510" />
    </asp:DetailsView>
    <br />
    <br />
    <asp:SqlDataSource ID="SqlDataSourceOrder" runat="server" ConnectionString="<%$ ConnectionStrings:LocalConnectionString %>" SelectCommand="SELECT Id, Number, UserId, Status, Type, RequestInfo, TermsIsAgreed, Quoted, Rate, QuoteSource, Amount, BTCAmount, CurrencyId, CommissionProcent, CardNumber, PaymentType, CryptoAddress, AccountNumber, SwiftBIC, RecieverName, RecieverRef, RecieverText, CurrencyCode, WireType, WireCost, Note, ExtRef, Name, Email, IP, TransactionHash FROM [Order] WHERE (Number = @Number) ORDER BY Id" UpdateCommand="UPDATE [Order] SET BTCAmount = @BTCAmount, TransactionHash = @TransactionHash WHERE (Id = @Id)">
        <SelectParameters>
            <asp:ControlParameter ControlID="TextBoxOrderID" Name="Number" PropertyName="Text" />
        </SelectParameters>
        <UpdateParameters>
            <asp:Parameter Name="BTCAmount"  />
            <asp:Parameter Name="TransactionHash"  />
            <asp:Parameter Name="Id" />
        </UpdateParameters>
    </asp:SqlDataSource>

    <asp:SqlDataSource ID="SqlDataSourceTransaction" runat="server" ConnectionString="<%$ ConnectionStrings:LocalConnectionString %>" DeleteCommand="DELETE FROM [Transaction] WHERE [Id] = @Id" InsertCommand="INSERT INTO [Transaction] (OrderId, MethodId, Type, ExtRef, Amount, Currency, Info, Completed, FromAccount, ToAccount, Reconsiled, Exported) VALUES (@OrderId, 1, 2, @ExtRef, @Amount, 26, @Info, { fn NOW() }, 4, 1, @Reconsiled, @Exported)" SelectCommand="SELECT Id, OrderId, MethodId, Type, ExtRef, Amount, Currency, Info, Completed, FromAccount, ToAccount, Reconsiled, Exported FROM [Transaction] WHERE (OrderId = @OrderId)" UpdateCommand="UPDATE [Transaction] SET [OrderId] = @OrderId, [MethodId] = @MethodId, [Type] = @Type, [ExtRef] = @ExtRef, [Amount] = @Amount, [Currency] = @Currency, [Info] = @Info, [Completed] = @Completed, [FromAccount] = @FromAccount, [ToAccount] = @ToAccount, [Reconsiled] = @Reconsiled, [Exported] = @Exported WHERE [Id] = @Id">
        <DeleteParameters>
            <asp:Parameter Name="Id" Type="Int32" />
        </DeleteParameters>
        <InsertParameters>
            <%--<asp:Parameter Name="OrderId" Type="Int32" />--%>
            <asp:ControlParameter ControlID="GridViewOrder" Name="OrderId" PropertyName="SelectedValue" />
            <asp:Parameter Name="ExtRef" Type="String" />
            <asp:Parameter Name="Amount" Type="Decimal" />
            <asp:Parameter Name="Info" />
            <asp:Parameter Name="Reconsiled" Type="DateTime" />
            <asp:Parameter Name="Exported" Type="DateTime" />
        </InsertParameters>
        <SelectParameters>
            <asp:ControlParameter ControlID="GridViewOrder" Name="OrderId" PropertyName="SelectedValue" />
        </SelectParameters>
        <UpdateParameters>
            <asp:Parameter Name="OrderId" Type="Int32" />
            <asp:Parameter Name="MethodId" Type="Int32" />
            <asp:Parameter Name="Type" Type="Int32" />
            <asp:Parameter Name="ExtRef" Type="String" />
            <asp:Parameter Name="Amount" Type="Decimal" />
            <asp:Parameter Name="Currency" Type="Int32" />
            <asp:Parameter Name="Info" Type="String" />
            <asp:Parameter Name="Completed" Type="DateTime" />
            <asp:Parameter Name="FromAccount" Type="Int32" />
            <asp:Parameter Name="ToAccount" Type="Int32" />
            <asp:Parameter Name="Reconsiled" Type="DateTime" />
            <asp:Parameter Name="Exported" Type="DateTime" />
            <asp:Parameter Name="Id" Type="Int32" />
        </UpdateParameters>
    </asp:SqlDataSource>

    <br />
</asp:Content>

