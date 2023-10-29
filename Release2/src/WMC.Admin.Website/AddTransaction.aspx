<%@ Page Title="" Language="VB" MasterPageFile="~/MasterPageAdmin.master" AutoEventWireup="false" CodeFile="AddTransaction.aspx.vb" Inherits="AddTransaction" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <asp:LinkButton ID="LinkButtonBACK" runat="server" PostBackUrl="~/Default.aspx">Back</asp:LinkButton>
        <br />
        <br />

     <div class="verdana13">
        OrderID:
        <asp:TextBox ID="TextBoxOrderID" runat="server"></asp:TextBox>
        &nbsp;<asp:Button ID="ButtonFindByOrderID" runat="server" Text="Find" />
        <br />
        <br />
    </div>



    <p class="verdana13">
        Orders before semi-automated solution:
    </p>
        <asp:GridView ID="GridViewOrder" runat="server" AllowPaging="True" AllowSorting="True" AutoGenerateColumns="False" DataKeyNames="Id" DataSourceID="SqlDataSourceOrder" BackColor="White" BorderColor="#336666" BorderStyle="Double" BorderWidth="3px" CellPadding="4" EmptyDataText="No orders awaits approval" GridLines="Horizontal">
            <Columns>
                <asp:CommandField ShowSelectButton="True" />
                <asp:BoundField DataField="Id" HeaderText="Id" ReadOnly="True" SortExpression="Id" Visible="False" />
                <asp:BoundField DataField="Number" HeaderText="Order Number" SortExpression="Number" ReadOnly="True" />
                <asp:BoundField DataField="Amount" HeaderText="Amount" SortExpression="Amount" DataFormatString="{0:0.00}" ReadOnly="True" />
                <asp:BoundField DataField="CurrencyId" HeaderText="CurrencyId" SortExpression="CurrencyId" InsertVisible="False" ReadOnly="True" />
                <asp:BoundField DataField="Quoted" HeaderText="Quoted" SortExpression="Quoted" ReadOnly="True" />
                <asp:BoundField DataField="CryptoAddress" HeaderText="CryptoAddress" SortExpression="CryptoAddress" ReadOnly="True" Visible="False" />
<asp:BoundField DataField="Rate" HeaderText="Rate" SortExpression="Rate"></asp:BoundField>
                <asp:TemplateField HeaderText="Note" SortExpression="Note">
                    <EditItemTemplate>
                        <asp:TextBox ID="TextBox2" runat="server" Text='<%# Bind("Note") %>' TextMode="MultiLine"></asp:TextBox>
                    </EditItemTemplate>
                    <ItemTemplate>
                        <asp:Label ID="Label2" runat="server" Text='<%# Bind("Note") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="Name" HeaderText="Name" SortExpression="Name" ReadOnly="True" />
                <asp:BoundField DataField="Email" HeaderText="Email" SortExpression="Email" ReadOnly="True" />
                <asp:TemplateField HeaderText="Status" SortExpression="Status">
                    <EditItemTemplate>
                        <asp:DropDownList ID="DropDownList1" runat="server" DataSourceID="SqlDataSourceStatus" DataTextField="Text" DataValueField="Id" SelectedValue='<%# Bind("Status") %>'>
                            <asp:ListItem Value="6">KYC Decline</asp:ListItem>
                            <asp:ListItem Value="6">Completed</asp:ListItem>
                        </asp:DropDownList>
                        <%--<asp:TextBox ID="TextBox3" runat="server" Text='<%# Bind("Status") %>'></asp:TextBox>--%>
                    </EditItemTemplate>
                    <ItemTemplate>
                        <asp:Label ID="Label3" runat="server" Text='<%# Bind("Status") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:CommandField SelectText="Review" ShowEditButton="True" />
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
                <asp:BoundField DataField="OrderId" HeaderText="OrderId" SortExpression="OrderId" ReadOnly="True" />
                <asp:BoundField DataField="MethodId" HeaderText="MethodId" SortExpression="MethodId" ReadOnly="True" />
                <asp:BoundField DataField="Type" HeaderText="Type" SortExpression="Type" ReadOnly="True" />
                <asp:BoundField DataField="ExtRef" HeaderText="ExtRef" SortExpression="ExtRef" ReadOnly="True" />
                <asp:BoundField DataField="Amount" HeaderText="Amount" SortExpression="Amount" ReadOnly="True" />
                <asp:BoundField DataField="Currency" HeaderText="Currency" SortExpression="Currency" ReadOnly="True" />
                <asp:BoundField DataField="Info" HeaderText="Info" SortExpression="Info" ReadOnly="True" />
                <asp:BoundField DataField="Completed" HeaderText="Completed" SortExpression="Completed" />
                <asp:BoundField DataField="FromAccount" HeaderText="FromAccount" SortExpression="FromAccount" ReadOnly="True" />
                <asp:BoundField DataField="ToAccount" HeaderText="ToAccount" SortExpression="ToAccount" ReadOnly="True" />
                <asp:BoundField DataField="Reconsiled" HeaderText="Reconsiled" SortExpression="Reconsiled" ReadOnly="True" Visible="False" />
                <asp:BoundField DataField="Exported" HeaderText="Exported" SortExpression="Exported" ReadOnly="True" Visible="False" />
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
            <asp:BoundField DataField="ExtRef" HeaderText="ExtRef" SortExpression="ExtRef" />
            <asp:BoundField DataField="Amount" HeaderText="Amount (BTC):" SortExpression="Amount" />
            <asp:BoundField DataField="Currency" HeaderText="Currency" SortExpression="Currency" InsertVisible="False" />
            <asp:BoundField DataField="Info" HeaderText="Info" SortExpression="Info" InsertVisible="False" />
            <asp:TemplateField HeaderText="Completed" InsertVisible="False" SortExpression="Completed">
                <EditItemTemplate>
                    <asp:TextBox ID="TextBox2" runat="server" Text='<%# Bind("Completed") %>'></asp:TextBox>
                </EditItemTemplate>
                <InsertItemTemplate>
                    <asp:TextBox ID="TextBox2" runat="server" Text='<%# Bind("Completed", "{0:G}") %>' TextMode="Date"></asp:TextBox>
                </InsertItemTemplate>
                <ItemTemplate>
                    <asp:Label ID="Label2" runat="server" Text='<%# Bind("Completed") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
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
    <asp:SqlDataSource ID="SqlDataSourceOrder" runat="server" ConnectionString="<%$ ConnectionStrings:LocalConnectionString %>" SelectCommand="SELECT Id, Number, UserId, Status, Type, RequestInfo, TermsIsAgreed, Quoted, Rate, QuoteSource, Amount, BTCAmount, CurrencyId, CommissionProcent, CardNumber, PaymentType, CryptoAddress, AccountNumber, SwiftBIC, RecieverName, RecieverRef, RecieverText, CurrencyCode, WireType, WireCost, Note, ExtRef, Name, Email, IP, TransactionHash FROM [Order] WHERE (Number = @Number) ORDER BY Id" UpdateCommand="UPDATE [Order] SET Rate = @Rate, Note = @Note, Status = @Status WHERE (Id = @Id)">
         <SelectParameters>
            <asp:ControlParameter ControlID="TextBoxOrderID" Name="Number" PropertyName="Text" />
        </SelectParameters>
        <UpdateParameters>
            <asp:Parameter Name="Rate"  Type="Decimal"/>            
            <asp:Parameter Name="Note" />
            <asp:Parameter Name="Status" Type="Int32"/>
            <asp:Parameter Name="Id" />
        </UpdateParameters>
    </asp:SqlDataSource>

    <asp:SqlDataSource ID="SqlDataSourceTransaction" runat="server" ConnectionString="<%$ ConnectionStrings:LocalConnectionString %>" DeleteCommand="DELETE FROM [Transaction] WHERE [Id] = @Id" InsertCommand="INSERT INTO [Transaction] (OrderId, MethodId, Type, ExtRef, Amount, Currency, Completed, FromAccount, ToAccount, Reconsiled, Exported) VALUES (@OrderId, 1, 2, @ExtRef, @Amount, 26, @Completed, 4, 1, @Reconsiled, @Exported)" SelectCommand="SELECT Id, OrderId, MethodId, Type, ExtRef, Amount, Currency, Info, Completed, FromAccount, ToAccount, Reconsiled, Exported FROM [Transaction] WHERE (OrderId = @OrderId)" UpdateCommand="UPDATE [Transaction] SET Completed = @Completed WHERE (Id = @Id)">
        <DeleteParameters>
            <asp:Parameter Name="Id" Type="Int32" />
        </DeleteParameters>
        <InsertParameters>
            <%--<asp:Parameter Name="OrderId" Type="Int32" />--%>
            <asp:ControlParameter ControlID="GridViewOrder" Name="OrderId" PropertyName="SelectedValue" />
            <asp:Parameter Name="ExtRef" Type="String" />
            <asp:Parameter Name="Amount" Type="Decimal" />
            <asp:Parameter Name="Completed" />
            <asp:Parameter Name="Reconsiled" Type="DateTime" />
            <asp:Parameter Name="Exported" Type="DateTime" />
        </InsertParameters>
        <SelectParameters>
            <asp:ControlParameter ControlID="GridViewOrder" Name="OrderId" PropertyName="SelectedValue" />
        </SelectParameters>
        <UpdateParameters>
            <asp:Parameter Name="Completed" Type="DateTime" />
            <asp:Parameter Name="Id" Type="Int32" />
        </UpdateParameters>
    </asp:SqlDataSource>

    <asp:SqlDataSource ID="SqlDataSourceStatus" runat="server" ConnectionString="<%$ ConnectionStrings:LocalConnectionString %>" SelectCommand="SELECT Id, Text FROM OrderStatus "></asp:SqlDataSource>

    <br />
    </asp:Content>

