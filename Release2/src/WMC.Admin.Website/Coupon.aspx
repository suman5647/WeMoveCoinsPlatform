<%@ Page Title="" Language="VB" MasterPageFile="~/MasterPageAdmin.master" AutoEventWireup="false" CodeFile="Coupon.aspx.vb" Inherits="SystemExceptions" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:LinkButton ID="LinkButtonBACK" runat="server" PostBackUrl="~/Default.aspx">Back</asp:LinkButton>
    <br />
    <br />
    <br />

    <asp:Panel ID="PanelInsert" runat="server" HorizontalAlign="Left" Visible="False">

        <asp:DetailsView ID="DetailsViewCoupons" runat="server" AutoGenerateRows="False" DataKeyNames="Id" DataSourceID="SqlDataSourceCoupon" DefaultMode="Insert" Height="50px" Width="400px" CellPadding="4" ForeColor="#333333" GridLines="None">
            <AlternatingRowStyle BackColor="White" />
            <CommandRowStyle BackColor="#D1DDF1" Font-Bold="True" />
            <EditRowStyle BackColor="#2461BF" />
            <FieldHeaderStyle BackColor="#DEE8F5" Font-Bold="True" />
            <Fields>
                <asp:BoundField DataField="CouponCode" HeaderText="Coupon Code" SortExpression="CouponCode" />
                <asp:BoundField DataField="Description" HeaderText="Description" SortExpression="Description" />
                <asp:BoundField DataField="Discount" HeaderText="Discount" SortExpression="Discount" />
                <asp:CommandField ShowInsertButton="True" />
            </Fields>
            <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
            <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
            <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
            <RowStyle BackColor="#EFF3FB" />
        </asp:DetailsView>
    </asp:Panel>

    <asp:Panel ID="PanelSelect" runat="server" HorizontalAlign="Left">
        <br />
        <table>
            <tr>
                <td>
                    <h3>Coupon Codes:</h3>
                    <asp:GridView ID="GridViewCoupons" runat="server" AutoGenerateColumns="False" DataKeyNames="Id" DataSourceID="SqlDataSourceCoupon" CellPadding="4" ForeColor="#333333" GridLines="None">
                        <AlternatingRowStyle BackColor="White" />
                        <Columns>
                            <asp:CommandField ShowSelectButton="True" />
                            <%--<asp:BoundField DataField="Id" HeaderText="Id" SortExpression="Id" ReadOnly="True" Visible="False" />--%>
                            <asp:BoundField DataField="CouponCode" HeaderText="Coupon Code" SortExpression="CouponCode" />
                            <asp:BoundField DataField="Description" HeaderText="Description" SortExpression="Description" />
                            <asp:TemplateField HeaderText="Discount" SortExpression="Discount">
                                <EditItemTemplate>
                                    <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("Discount", "{0:0,0}") %>'></asp:TextBox>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="Label1" runat="server" Text='<%# Bind("Discount", "{0:0,0}") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="From Date" SortExpression="FromDate">
                                <EditItemTemplate>
                                    <asp:Calendar ID="Calendar1" runat="server" SelectedDate='<%# Bind("FromDate") %>'></asp:Calendar>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="Label2" runat="server" Text='<%# Bind("FromDate", "{0:d}") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="To Date" SortExpression="ToDate">
                                <EditItemTemplate>
                                    <asp:Calendar ID="Calendar2" runat="server" SelectedDate='<%# Bind("ToDate") %>'></asp:Calendar>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="Label3" runat="server" Text='<%# Bind("ToDate", "{0:d}") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:CheckBoxField DataField="IsActive" HeaderText="Active" SortExpression="IsActive" />
                            <asp:CommandField ShowEditButton="True" />
                            <asp:TemplateField ShowHeader="False">
                                <ItemTemplate>
                                    <asp:LinkButton ID="LinkButtonDelete" runat="server" CausesValidation="False" CommandName="Delete" Text="Delete"></asp:LinkButton>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                        <EditRowStyle BackColor="#ECECEC" />
                        <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                        <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                        <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                        <RowStyle BackColor="#EFF3FB" />
                        <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
                        <SortedAscendingCellStyle BackColor="#F5F7FB" />
                        <SortedAscendingHeaderStyle BackColor="#6D95E1" />
                        <SortedDescendingCellStyle BackColor="#E9EBEF" />
                        <SortedDescendingHeaderStyle BackColor="#4870BE" />
                    </asp:GridView>
                </td>
                <td>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;</td>
                <td>
                    <asp:LinkButton ID="LinkButtonInsert" runat="server">New Coupon Code...</asp:LinkButton></td>
            </tr>
        </table>

       
        <br />
        <br />
        <br />
        <h3>Users used this code:</h3>
        <asp:GridView ID="GridViewCouponOrders" runat="server" EmptyDataText="No records" AutoGenerateColumns="False" DataSourceID="SqlDataSourceCouponOrders" AllowPaging="True" AllowSorting="True" CellPadding="4" ForeColor="#333333" GridLines="None" PageSize="100">
            <AlternatingRowStyle BackColor="White" />
            <Columns>
                <asp:BoundField DataField="Fname" HeaderText="Name" SortExpression="Fname" />
                <asp:BoundField DataField="Phone" HeaderText="Phone" SortExpression="Phone" />
                <asp:BoundField DataField="Email" HeaderText="Email" SortExpression="Email" />
                <asp:BoundField DataField="Text" HeaderText="Country" SortExpression="Text" />
            </Columns>
            <EditRowStyle BackColor="#2461BF" />
            <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
            <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
            <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
            <RowStyle BackColor="#EFF3FB" />
            <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
            <SortedAscendingCellStyle BackColor="#F5F7FB" />
            <SortedAscendingHeaderStyle BackColor="#6D95E1" />
            <SortedDescendingCellStyle BackColor="#E9EBEF" />
            <SortedDescendingHeaderStyle BackColor="#4870BE" />
        </asp:GridView>
        <br /><br />
        <h3>Orders used the code:</h3>
        
        <asp:GridView ID="GridViewSubOrders" runat="server" AutoGenerateColumns="False" DataKeyNames="Id" DataSourceID="SqlDataSourceOrderChild" EmptyDataText="No record" BackColor="White" BorderColor="#CCCCCC" BorderStyle="None" BorderWidth="1px" CellPadding="3" AllowSorting="True">
            <Columns>
                <asp:BoundField DataField="Id" HeaderText="Id" InsertVisible="False" ReadOnly="True" SortExpression="Id" />
                <asp:BoundField DataField="Number" HeaderText="Number" SortExpression="Number" />
                <asp:TemplateField HeaderText="Amount" SortExpression="Amount">
                    <EditItemTemplate>
                        <asp:Label ID="Label1" runat="server" Text='<%# Eval("Amount", "{0:0,0}") %>'></asp:Label>
                    </EditItemTemplate>
                    <ItemTemplate>
                        <asp:Label ID="Label2" runat="server" Text='<%# Bind("Amount", "{0:0,0}") %>'></asp:Label>
                        <asp:Label ID="Label22" runat="server" Text='<%# Bind("Code") %>'></asp:Label>
                    </ItemTemplate>
                    <ItemStyle Width="60px" />
                </asp:TemplateField>
                <asp:BoundField DataField="Quoted" HeaderText="Quoted" SortExpression="Quoted" />
                <asp:BoundField DataField="Rate" HeaderText="Rate" SortExpression="Rate" />
                <asp:BoundField DataField="CommissionProcent" HeaderText="Fee" SortExpression="CommissionProcent" />
                <asp:BoundField DataField="MinersFee" HeaderText="MinersFee" SortExpression="MinersFee" />
                <asp:BoundField DataField="CryptoAddress" HeaderText="CryptoAddress" SortExpression="CryptoAddress" />
                <asp:BoundField DataField="Name" HeaderText="Name" SortExpression="Name" />
                <asp:BoundField DataField="Email" HeaderText="Email" SortExpression="Email" />
                <asp:TemplateField HeaderText="IP" SortExpression="IP">
                    <EditItemTemplate>
                        <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("IP") %>'></asp:TextBox>
                    </EditItemTemplate>
                    <ItemTemplate>
                        <asp:HyperLink ID="HyperLinkIP" runat="server" Target="_blank" NavigateUrl='<%# Eval("IP")%>' Text='<%# Bind("IP")%>'></asp:HyperLink>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Note" SortExpression="Note">
                    <EditItemTemplate>
                        <asp:TextBox ID="TextBox2" runat="server" Text='<%# Bind("Note") %>'></asp:TextBox>
                    </EditItemTemplate>
                    <ItemTemplate>
                        <asp:Label ID="Label23" runat="server" Text='<%# Bind("Note") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="Status" HeaderText="Status" SortExpression="Status" />
                <asp:BoundField DataField="CardNumber" HeaderText="CardNumber" SortExpression="CardNumber" />
                <asp:BoundField DataField="TxSecret" HeaderText="TxSecret" SortExpression="TxSecret" />
                <asp:BoundField DataField="BrowserCulture" HeaderText="Culture" SortExpression="BrowserCulture" />
                <asp:BoundField DataField="SiteId" HeaderText="SiteId" SortExpression="SiteId" />
                <asp:BoundField DataField="PartnerId" HeaderText="PartnerId" SortExpression="PartnerId" />
            </Columns>
            <FooterStyle BackColor="White" ForeColor="#000066" />
            <HeaderStyle BackColor="#006699" Font-Bold="True" ForeColor="White" />
            <PagerStyle ForeColor="#000066" HorizontalAlign="Left" BackColor="White" />
            <RowStyle ForeColor="#000066" />
            <SelectedRowStyle BackColor="#669999" Font-Bold="True" ForeColor="White" />
            <SortedAscendingCellStyle BackColor="#F1F1F1" />
            <SortedAscendingHeaderStyle BackColor="#007DBB" />
            <SortedDescendingCellStyle BackColor="#CAC9C9" />
            <SortedDescendingHeaderStyle BackColor="#00547E" />
        </asp:GridView>
    </asp:Panel>

    <br />
    <asp:SqlDataSource ID="SqlDataSourceCoupon" runat="server" ConnectionString="<%$ ConnectionStrings:LocalConnectionString %>" SelectCommand="SELECT * FROM [Coupon]" DeleteCommand="DELETE FROM [Coupon] WHERE [Id] = @Id" InsertCommand="INSERT INTO Coupon(CouponCode, Description, Discount, MaxTxnCount, MinTxnLimit, MaxTxnLimit, MaxTotalTxnLimit, FromDate, ToDate, Region, CryptoCurrency, Type, ReferredBy, IsActive) VALUES (@CouponCode, @Description, @Discount, @MaxTxnCount, @MinTxnLimit, @MaxTxnLimit, @MaxTotalTxnLimit, { fn NOW() }, { fn NOW() }, @Region, @CryptoCurrency, @Type, @ReferredBy, 1)" UpdateCommand="UPDATE [Coupon] SET [CouponCode] = @CouponCode, [Description] = @Description, [Discount] = @Discount, [MaxTxnCount] = @MaxTxnCount, [MinTxnLimit] = @MinTxnLimit, [MaxTxnLimit] = @MaxTxnLimit, [MaxTotalTxnLimit] = @MaxTotalTxnLimit, [FromDate] = @FromDate, [ToDate] = @ToDate, [Region] = @Region, [CryptoCurrency] = @CryptoCurrency, [Type] = @Type, [ReferredBy] = @ReferredBy, [IsActive] = @IsActive WHERE [Id] = @Id">
        <DeleteParameters>
            <asp:Parameter Name="Id" Type="Int32" />
        </DeleteParameters>
        <InsertParameters>
            <asp:Parameter Name="CouponCode" Type="String" />
            <asp:Parameter Name="Description" Type="String" />
            <asp:Parameter Name="Discount" Type="Decimal" />
            <asp:Parameter Name="MaxTxnCount" Type="Int32" />
            <asp:Parameter Name="MinTxnLimit" Type="Decimal" />
            <asp:Parameter Name="MaxTxnLimit" Type="Decimal" />
            <asp:Parameter Name="MaxTotalTxnLimit" Type="Decimal" />
            <asp:Parameter Name="Region" Type="String" />
            <asp:Parameter Name="CryptoCurrency" Type="String" />
            <asp:Parameter Name="Type" Type="String" />
            <asp:Parameter Name="ReferredBy" Type="String" />
        </InsertParameters>
        <UpdateParameters>
            <asp:Parameter Name="CouponCode" Type="String" />
            <asp:Parameter Name="Description" Type="String" />
            <asp:Parameter Name="Discount" Type="Decimal" />
            <asp:Parameter Name="MaxTxnCount" Type="Int32" />
            <asp:Parameter Name="MinTxnLimit" Type="Decimal" />
            <asp:Parameter Name="MaxTxnLimit" Type="Decimal" />
            <asp:Parameter Name="MaxTotalTxnLimit" Type="Decimal" />
            <asp:Parameter DbType="Date" Name="FromDate" />
            <asp:Parameter DbType="Date" Name="ToDate" />
            <asp:Parameter Name="Region" Type="String" />
            <asp:Parameter Name="CryptoCurrency" Type="String" />
            <asp:Parameter Name="Type" Type="String" />
            <asp:Parameter Name="ReferredBy" Type="String" />
            <asp:Parameter Name="IsActive" Type="Boolean" />
            <asp:Parameter Name="Id" Type="Int32" />
        </UpdateParameters>
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="SqlDataSourceCouponOrders" runat="server" ConnectionString="<%$ ConnectionStrings:LocalConnectionString %>" SelectCommand="SELECT DISTINCT [User].Fname, [User].Phone, [User].Email, Country.Code, Country.Text FROM [Order] INNER JOIN [User] ON [Order].UserId = [User].Id INNER JOIN Country ON [User].CountryId = Country.Id WHERE ([Order].CouponId = @CouponId)">
        <SelectParameters>
            <asp:ControlParameter ControlID="GridViewCoupons" Name="CouponId" PropertyName="SelectedValue" />
        </SelectParameters>
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="SqlDataSourceOrderChild" runat="server" ConnectionString="<%$ ConnectionStrings:LocalConnectionString %>" SelectCommand="SELECT DISTINCT [Order].Id, [Order].Number, [User].Fname, [Order].Amount, Currency.Code, [Order].Quoted, [Order].CryptoAddress, [Order].Name, [Order].Email, [Order].IP, OrderStatus.Text AS Status, [Order].Note, [Order].Rate, [Order].CommissionProcent, [Order].CardNumber, [Order].SiteId, SUBSTRING([Order].CountryCode, 1, 5) AS BrowserCulture, [Order].TxSecret, [Order].PartnerId, [Order].MinersFee, [Order].CouponId FROM Currency INNER JOIN [Order] ON Currency.Id = [Order].CurrencyId INNER JOIN OrderStatus ON [Order].Status = OrderStatus.Id INNER JOIN [User] ON [Order].UserId = [User].Id INNER JOIN [Order] AS Order_1 ON [User].Id = Order_1.UserId WHERE ([Order].CouponId = @CouponId) ORDER BY [Order].Quoted DESC">
        <SelectParameters>
            <asp:ControlParameter ControlID="GridViewCoupons" Name="CouponId" PropertyName="SelectedValue" />
        </SelectParameters>
    </asp:SqlDataSource>
</asp:Content>

