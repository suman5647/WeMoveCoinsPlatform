<%@ Page Title="" Language="VB" MasterPageFile="~/MasterPageAdmin.master" AutoEventWireup="false" CodeFile="FindEdit.aspx.vb" Inherits="ApprovePayout" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:LinkButton ID="LinkButtonBACK" runat="server" PostBackUrl="~/Default.aspx">Back</asp:LinkButton>
    <br />
    <br />
    <div class="verdana13">
        Find by:
        <asp:DropDownList ID="DropDownListWhereType" runat="server" CssClass="DropdownList">
            <asp:ListItem>Number</asp:ListItem>
            <asp:ListItem>CC Number</asp:ListItem>
            <asp:ListItem>Email</asp:ListItem>
            <asp:ListItem>Name</asp:ListItem>
            <asp:ListItem>YourPay ID</asp:ListItem>
            <asp:ListItem>CryptoAddress</asp:ListItem>
            <asp:ListItem>TransactionHash</asp:ListItem>
            <asp:ListItem>CryptoAddress</asp:ListItem>
            <asp:ListItem>TxSecret</asp:ListItem>
            <asp:ListItem>Phone</asp:ListItem>
            <asp:ListItem>SiteID</asp:ListItem>
            <asp:ListItem>PartnerID</asp:ListItem>
            <asp:ListItem>OrderId</asp:ListItem>
            <asp:ListItem>UserId</asp:ListItem>
           <%-- <asp:ListItem Value="Trusted Only">&quot;Trusted Only&quot;</asp:ListItem>--%>
        </asp:DropDownList>&nbsp;
        <asp:TextBox ID="TextBoxOrderID" runat="server" Width="200px"></asp:TextBox>
        &nbsp;<asp:Button ID="ButtonFindByOrderID" runat="server" Text="Find" />
    </div>
    <table style="width: 100%;">
        <tr>
            <td>
                <p class="verdana13">Found Order(s):</p>
            </td>
            <td>&nbsp;</td>
            <td style="text-align: right">
                <%-- <asp:ListItem Value="Trusted Only">&quot;Trusted Only&quot;</asp:ListItem>--%>
                <asp:Panel ID="PanelNote" runat="server" Visible="False">
                    <p class="verdana13">
                        <asp:TextBox ID="TextBoxNote" runat="server" TextMode="MultiLine" Width="300px"></asp:TextBox>&nbsp;
                    <asp:Button ID="ButtonAddNote" runat="server" Text="Add Note" />
                        &nbsp; &nbsp; &nbsp;
                    <asp:DropDownList ID="DropDownListNoteOptions" CssClass="DropdownList" runat="server" AutoPostBack="True" Width="300px">
                        <asp:ListItem Selected="True">[blank]</asp:ListItem>
                        <asp:ListItem>MobilePay OK</asp:ListItem>
                        <asp:ListItem>Appoved by photo upload</asp:ListItem>
                        <asp:ListItem>NOT on mobilepay</asp:ListItem>
                        <asp:ListItem>Card accepted (span)</asp:ListItem>
                        <asp:ListItem>Undelivered Mail. Wait till customer reach out</asp:ListItem>
                        <asp:ListItem>Accepted (seems legit)</asp:ListItem>
                    </asp:DropDownList>&nbsp;        
                    </p>
                </asp:Panel>
            </td>

        </tr>
        <tr>
            <td colspan="3" style="text-align: right">
                <p>
                    <asp:GridView ID="GridViewOrder" runat="server" AllowPaging="True" AllowSorting="True" AutoGenerateColumns="False" DataKeyNames="Id" BackColor="White" BorderColor="#336666" BorderStyle="Double" BorderWidth="3px" CellPadding="4" EmptyDataText="No found orders" GridLines="Horizontal" PageSize="100" Width="100%">
                        <Columns>
                            <asp:CommandField ShowSelectButton="True" SelectText="Review" />
                            <asp:BoundField DataField="Id" HeaderText="Id" ReadOnly="True" SortExpression="Id" Visible="False" />
                            <asp:TemplateField HeaderText="Order Number" SortExpression="Number">
                                <EditItemTemplate>
                                    <asp:Label ID="Label2" runat="server" Text='<%# Eval("Number") %>'></asp:Label>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="LabelOrderNumber" runat="server" Text='<%# Eval("Number") %>'></asp:Label>
                                    <asp:HyperLink ID="HyperLinkOrder" runat="server" NavigateUrl='<%# Bind("Number") %>' Target="_blank" Text='>'></asp:HyperLink>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Amount" SortExpression="Amount">
                                <EditItemTemplate>
                                    <asp:Label ID="LabelAmount" runat="server" Text='<%# Eval("Amount", "{0:0,0}") %>'></asp:Label>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="LabelAmount" runat="server" Text='<%# Bind("Amount", "{0:0,0}") %>'></asp:Label>
                                    <asp:Label ID="LabelAmount1" runat="server" Text='<%# Bind("Code") %>'></asp:Label>
                                </ItemTemplate>
                                <ItemStyle Width="60px" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Currency" SortExpression="Code" Visible="False">
                                <EditItemTemplate>
                                    <asp:Label ID="LabelCurrency" runat="server" Text='<%# Eval("Code") %>'></asp:Label>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="LabelCurrency" runat="server" Text='<%# Bind("Code") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="CryptoAddress" HeaderText="CryptoAddress" SortExpression="CryptoAddress" ReadOnly="True" Visible="False" />
                            <asp:BoundField DataField="Quoted" HeaderText="Quoted" SortExpression="Quoted" ReadOnly="True" />
                            <asp:BoundField DataField="Name" HeaderText="Name" SortExpression="Name" ReadOnly="True" />
                            <asp:BoundField DataField="Email" HeaderText="Email" SortExpression="Email" ReadOnly="True" />
                            <asp:BoundField DataField="Phone" HeaderText="Phone" SortExpression="Phone" ReadOnly="True" />
                            <asp:BoundField DataField="IP" HeaderText="IP" SortExpression="IP" ReadOnly="True" Visible="False" />
                            <asp:TemplateField HeaderText="Move to:" Visible="False">
                                <ItemTemplate>
                                    <asp:Button ID="ButtonMoveCompliance" runat="server" BackColor="#66FF66" BorderStyle="Solid" CommandName="MoveCompliance" Text="Compliance" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Cancel" Visible="False">
                                <ItemTemplate>
                                    <asp:Button ID="ButtonCancelOrder" runat="server" BackColor="#FFA8C5" BorderStyle="Solid" CommandName="CancelOrder" Text="Cancel" ForeColor="Black" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="KYC" Visible="False">
                                <ItemTemplate>
                                    <asp:Button ID="ButtonKYCDeclined" runat="server" BackColor="#FFA8C5" BorderStyle="Solid" CommandName="KYCDeclined" Text="Decline" ForeColor="Black" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Rate" SortExpression="Rate" Visible="False">
                                <EditItemTemplate>
                                    <asp:Label ID="Label1" runat="server" Text='<%# Bind("Rate") %>'></asp:Label>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="Label1" runat="server" Text='<%# Bind("Rate") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="SiteId" HeaderText="SiteId" SortExpression="SiteId" />
                            <asp:BoundField DataField="PartnerId" HeaderText="PartnerId" SortExpression="PartnerId" />
                            <asp:BoundField DataField="OrderStatus" HeaderText="OrderStatus" SortExpression="OrderStatus" />
                            <asp:TemplateField HeaderText="Note" SortExpression="Note">
                                <EditItemTemplate>
                                    <asp:TextBox ID="TextBoxNote" runat="server" Height="175px" Text='<%# Bind("Note") %>' TextMode="MultiLine" Width="377px"></asp:TextBox>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="Label2" runat="server" Text='<%# Bind("Note") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                        <FooterStyle BackColor="White" ForeColor="#333333" />
                        <HeaderStyle BackColor="#336666" Font-Bold="True" ForeColor="White" />
                        <PagerStyle BackColor="#336666" ForeColor="White" HorizontalAlign="Center" />
                        <RowStyle BackColor="White" ForeColor="#333333" />
                        <SelectedRowStyle BackColor="#E6E6E6" Font-Bold="True" ForeColor="White" />
                        <SortedAscendingCellStyle BackColor="#F7F7F7" />
                        <SortedAscendingHeaderStyle BackColor="#487575" />
                        <SortedDescendingCellStyle BackColor="#E5E5E5" />
                        <SortedDescendingHeaderStyle BackColor="#275353" />
                    </asp:GridView>
                </p>
            </td>
        </tr>
    </table>
     <p class="verdana13">
        Order:
    </p>
    mmm<asp:GridView ID="GridViewOrderDetail" runat="server" AutoGenerateColumns="False" DataKeyNames="Id" DataSourceID="SqlDataSourceOrderDetail">
        <Columns>
            <asp:CommandField ShowEditButton="True" />
            <asp:BoundField DataField="Number" HeaderText="Number" SortExpression="Number" />
            <asp:BoundField DataField="Status" HeaderText="Status" SortExpression="Status" />
            <asp:BoundField DataField="Type" HeaderText="Type" SortExpression="Type" />
            <asp:BoundField DataField="Amount" HeaderText="Amount" SortExpression="Amount" />
            <asp:BoundField DataField="BTCAmount" HeaderText="BTCAmount" SortExpression="BTCAmount" />
            <asp:BoundField DataField="CurrencyId" HeaderText="CurrencyId" SortExpression="CurrencyId" />
            <asp:BoundField DataField="CommissionProcent" HeaderText="CommissionProcent" SortExpression="CommissionProcent" />
            <asp:BoundField DataField="CardNumber" HeaderText="CardNumber" SortExpression="CardNumber" />
            <asp:BoundField DataField="PaymentType" HeaderText="PaymentType" SortExpression="PaymentType" />
            <asp:BoundField DataField="CryptoAddress" HeaderText="CryptoAddress" SortExpression="CryptoAddress" />
            <asp:BoundField DataField="ExtRef" HeaderText="ExtRef" SortExpression="ExtRef" />
            <asp:BoundField DataField="Name" HeaderText="Name" SortExpression="Name" />
            <asp:BoundField DataField="Email" HeaderText="Email" SortExpression="Email" />
            <asp:BoundField DataField="TransactionHash" HeaderText="TransactionHash" SortExpression="TransactionHash" />
            <asp:BoundField DataField="SiteId" HeaderText="SiteId" SortExpression="SiteId" />
            <asp:BoundField DataField="TxSecret" HeaderText="TxSecret" SortExpression="TxSecret" />
            <asp:BoundField DataField="CardApproved" HeaderText="CardApproved" SortExpression="CardApproved" />
            <asp:BoundField DataField="PartnerId" HeaderText="PartnerId" SortExpression="PartnerId" />
        </Columns>
    </asp:GridView>
    <asp:SqlDataSource ID="SqlDataSourceOrderDetail" runat="server" ConnectionString="<%$ ConnectionStrings:LocalConnectionString %>" DeleteCommand="DELETE FROM [Order] WHERE [Id] = @Id" InsertCommand="INSERT INTO [Order] ([Number], [UserId], [Status], [Type], [RequestInfo], [TermsIsAgreed], [Quoted], [Rate], [QuoteSource], [Amount], [BTCAmount], [CurrencyId], [CommissionProcent], [CardNumber], [PaymentType], [CryptoAddress], [AccountNumber], [SwiftBIC], [RecieverName], [RecieverRef], [RecieverText], [CurrencyCode], [WireType], [WireCost], [Note], [ExtRef], [Name], [Email], [IP], [TransactionHash], [SiteId], [PaymentGatewayType], [RateBase], [RateHome], [RateBooks], [Approved], [ApprovedBy], [CountryCode], [TxSecret], [CardApproved], [RiskScore], [IpCode], [CreditCardUserIdentity], [TxSecrectVerificationAttempts], [Referrer], [Origin], [MinersFee], [BccAddress], [PartnerId]) VALUES (@Number, @UserId, @Status, @Type, @RequestInfo, @TermsIsAgreed, @Quoted, @Rate, @QuoteSource, @Amount, @BTCAmount, @CurrencyId, @CommissionProcent, @CardNumber, @PaymentType, @CryptoAddress, @AccountNumber, @SwiftBIC, @RecieverName, @RecieverRef, @RecieverText, @CurrencyCode, @WireType, @WireCost, @Note, @ExtRef, @Name, @Email, @IP, @TransactionHash, @SiteId, @PaymentGatewayType, @RateBase, @RateHome, @RateBooks, @Approved, @ApprovedBy, @CountryCode, @TxSecret, @CardApproved, @RiskScore, @IpCode, @CreditCardUserIdentity, @TxSecrectVerificationAttempts, @Referrer, @Origin, @MinersFee, @BccAddress, @PartnerId)" SelectCommand="SELECT Id, Number, UserId, Status, Type, RequestInfo, TermsIsAgreed, Quoted, Rate, QuoteSource, Amount, BTCAmount, CurrencyId, CommissionProcent, CardNumber, PaymentType, CryptoAddress, AccountNumber, SwiftBIC, RecieverName, RecieverRef, RecieverText, CurrencyCode, WireType, WireCost, Note, ExtRef, Name, Email, IP, TransactionHash, SiteId, PaymentGatewayType, RateBase, RateHome, RateBooks, Approved, ApprovedBy, CountryCode, TxSecret, CardApproved, RiskScore, IpCode, CreditCardUserIdentity, TxSecrectVerificationAttempts, Referrer, Origin, MinersFee, BccAddress, PartnerId FROM [Order] WHERE (Id = @Id)" UpdateCommand="UPDATE [Order] SET Status = @Status, Type = @Type, Amount = @Amount, BTCAmount = @BTCAmount, CurrencyId = @CurrencyId, CommissionProcent = @CommissionProcent, CardNumber = @CardNumber, PaymentType = @PaymentType, CryptoAddress = @CryptoAddress, AccountNumber = @AccountNumber, Note = @Note, ExtRef = @ExtRef, Name = @Name, Email = @Email, IP = @IP, TransactionHash = @TransactionHash, SiteId = @SiteId, TxSecret = @TxSecret, CardApproved = @CardApproved, PartnerId = @PartnerId WHERE (Id = @Id)">
        <DeleteParameters>
            <asp:Parameter Name="Id" Type="Int32" />
        </DeleteParameters>
        <InsertParameters>
            <asp:Parameter Name="Number" Type="String" />
            <asp:Parameter Name="UserId" Type="Int32" />
            <asp:Parameter Name="Status" Type="Int32" />
            <asp:Parameter Name="Type" Type="Int32" />
            <asp:Parameter Name="RequestInfo" Type="String" />
            <asp:Parameter Name="TermsIsAgreed" Type="DateTime" />
            <asp:Parameter Name="Quoted" Type="DateTime" />
            <asp:Parameter Name="Rate" Type="Decimal" />
            <asp:Parameter Name="QuoteSource" Type="String" />
            <asp:Parameter Name="Amount" Type="Decimal" />
            <asp:Parameter Name="BTCAmount" Type="Decimal" />
            <asp:Parameter Name="CurrencyId" Type="Int32" />
            <asp:Parameter Name="CommissionProcent" Type="Decimal" />
            <asp:Parameter Name="CardNumber" Type="String" />
            <asp:Parameter Name="PaymentType" Type="Int32" />
            <asp:Parameter Name="CryptoAddress" Type="String" />
            <asp:Parameter Name="AccountNumber" Type="String" />
            <asp:Parameter Name="SwiftBIC" Type="String" />
            <asp:Parameter Name="RecieverName" Type="String" />
            <asp:Parameter Name="RecieverRef" Type="String" />
            <asp:Parameter Name="RecieverText" Type="String" />
            <asp:Parameter Name="CurrencyCode" Type="String" />
            <asp:Parameter Name="WireType" Type="String" />
            <asp:Parameter Name="WireCost" Type="String" />
            <asp:Parameter Name="Note" Type="String" />
            <asp:Parameter Name="ExtRef" Type="String" />
            <asp:Parameter Name="Name" Type="String" />
            <asp:Parameter Name="Email" Type="String" />
            <asp:Parameter Name="IP" Type="String" />
            <asp:Parameter Name="TransactionHash" Type="String" />
            <asp:Parameter Name="SiteId" Type="Int32" />
            <asp:Parameter Name="PaymentGatewayType" Type="String" />
            <asp:Parameter Name="RateBase" Type="Decimal" />
            <asp:Parameter Name="RateHome" Type="Decimal" />
            <asp:Parameter Name="RateBooks" Type="Decimal" />
            <asp:Parameter Name="Approved" Type="DateTime" />
            <asp:Parameter Name="ApprovedBy" Type="Int32" />
            <asp:Parameter Name="CountryCode" Type="String" />
            <asp:Parameter Name="TxSecret" Type="String" />
            <asp:Parameter Name="CardApproved" Type="DateTime" />
            <asp:Parameter Name="RiskScore" Type="Decimal" />
            <asp:Parameter Name="IpCode" Type="String" />
            <asp:Parameter Name="CreditCardUserIdentity" Type="Object" />
            <asp:Parameter Name="TxSecrectVerificationAttempts" Type="Int32" />
            <asp:Parameter Name="Referrer" Type="String" />
            <asp:Parameter Name="Origin" Type="String" />
            <asp:Parameter Name="MinersFee" Type="Decimal" />
            <asp:Parameter Name="BccAddress" Type="String" />
            <asp:Parameter Name="PartnerId" Type="String" />
        </InsertParameters>
        <SelectParameters>
            <asp:ControlParameter ControlID="GridViewOrder" Name="Id" PropertyName="SelectedValue" />
        </SelectParameters>
        <UpdateParameters>
            <asp:Parameter Name="Status" Type="Int32" />
            <asp:Parameter Name="Type" Type="Int32" />
            <asp:Parameter Name="Amount" Type="Decimal" />
            <asp:Parameter Name="BTCAmount" Type="Decimal" />
            <asp:Parameter Name="CurrencyId" Type="Int32" />
            <asp:Parameter Name="CommissionProcent" Type="Decimal" />
            <asp:Parameter Name="CardNumber" Type="String" />
            <asp:Parameter Name="PaymentType" Type="Int32" />
            <asp:Parameter Name="CryptoAddress" Type="String" />
            <asp:Parameter Name="AccountNumber" Type="String" />
            <asp:Parameter Name="Note" Type="String" />
            <asp:Parameter Name="ExtRef" Type="String" />
            <asp:Parameter Name="Name" Type="String" />
            <asp:Parameter Name="Email" Type="String" />
            <asp:Parameter Name="IP" Type="String" />
            <asp:Parameter Name="TransactionHash" Type="String" />
            <asp:Parameter Name="SiteId" Type="Int32" />
            <asp:Parameter Name="TxSecret" Type="String" />
            <asp:Parameter Name="CardApproved" Type="DateTime" />
            <asp:Parameter Name="PartnerId" Type="String" />
            <asp:Parameter Name="Id" Type="Int32" />
        </UpdateParameters>
    </asp:SqlDataSource>
&nbsp;<p class="verdana13">
        User:
    </p>
    <p>
        <asp:GridView ID="GridViewUser" runat="server" AutoGenerateColumns="False" DataKeyNames="Id" DataSourceID="SqlDataSourceUser" EmptyDataText="No record" BackColor="#DEBA84" BorderColor="#DEBA84" BorderStyle="None" BorderWidth="1px" CellPadding="3" CellSpacing="2">
            <Columns>
                <asp:CommandField ShowEditButton="True" />
                <asp:BoundField DataField="Id" HeaderText="Id" InsertVisible="False" ReadOnly="True" SortExpression="Id" />
                <asp:BoundField DataField="Fname" HeaderText="Fname" SortExpression="Fname" />
                <asp:BoundField DataField="Phone" HeaderText="Phone" SortExpression="Phone" />
                <asp:BoundField DataField="Email" HeaderText="Email" SortExpression="Email" />
                <asp:BoundField DataField="CountryCode" HeaderText="CountryCode" SortExpression="CountryCode" />
                <asp:BoundField DataField="Commission" HeaderText="Commission" SortExpression="Commission" />
                <asp:BoundField DataField="Created" HeaderText="Created" SortExpression="Created" />
                <asp:BoundField DataField="PaymentMethodDetails" HeaderText="PaymentMethodDetails" SortExpression="PaymentMethodDetails" />
                <asp:BoundField DataField="CountryId" HeaderText="CountryId" SortExpression="CountryId" />
                <asp:BoundField DataField="Trusted" HeaderText="Trusted" SortExpression="Trusted" />
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
    </p>
    <p class="verdana13">
        Key indicators:
    </p>
    <br />
    <p class="verdana13">
        Transaction:
    </p>
    <p>
        <asp:GridView ID="GridViewTransaction" runat="server" AutoGenerateColumns="False" DataSourceID="SqlDataSourceTransaction" EmptyDataText="No records" BackColor="#DEBA84" BorderColor="#DEBA84" BorderStyle="None" BorderWidth="1px" CellPadding="3" CellSpacing="2" DataKeyNames="Id">
            <Columns>
                <asp:CommandField ShowEditButton="True" />
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
        <asp:SqlDataSource ID="SqlDataSourceTransaction" runat="server" ConnectionString="<%$ ConnectionStrings:LocalConnectionString %>" DeleteCommand="DELETE FROM [Transaction] WHERE [Id] = @Id" InsertCommand="INSERT INTO [Transaction] ([OrderId], [MethodId], [Type], [ExtRef], [Amount], [Currency], [Info], [Completed], [FromAccount], [ToAccount], [Reconsiled], [Exported]) VALUES (@OrderId, @MethodId, @Type, @ExtRef, @Amount, @Currency, @Info, @Completed, @FromAccount, @ToAccount, @Reconsiled, @Exported)" SelectCommand="SELECT Id, OrderId, MethodId, Type, ExtRef, Amount, Currency, Info, Completed, FromAccount, ToAccount, Reconsiled, Exported FROM [Transaction] WHERE (OrderId = @OrderId)" UpdateCommand="UPDATE [Transaction] SET [OrderId] = @OrderId, [MethodId] = @MethodId, [Type] = @Type, [ExtRef] = @ExtRef, [Amount] = @Amount, [Currency] = @Currency, [Info] = @Info, [Completed] = @Completed, [FromAccount] = @FromAccount, [ToAccount] = @ToAccount, [Reconsiled] = @Reconsiled, [Exported] = @Exported WHERE [Id] = @Id">
            <DeleteParameters>
                <asp:Parameter Name="Id" Type="Int32" />
            </DeleteParameters>
            <InsertParameters>
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
    </p>
   
    <asp:SqlDataSource ID="SqlDataSourceOrder" runat="server" ConnectionString="<%$ ConnectionStrings:LocalConnectionString %>" SelectCommand="SELECT TOP (200) [Order].Id, [Order].Number, [User].Fname, [Order].Amount, Currency.Code, [Order].Quoted, [Order].CryptoAddress, [Order].Name, [Order].Email, [Order].IP, [Order].Rate, [Order].Note, [Order].Status, [Order].SiteId, OrderStatus.Text AS OrderStatus, [Order].TransactionHash, [Order].ExtRef, [User].Phone, [Order].PartnerId FROM Currency INNER JOIN [Order] ON Currency.Id = [Order].CurrencyId INNER JOIN OrderStatus ON [Order].Status = OrderStatus.Id INNER JOIN [User] ON [Order].UserId = [User].Id ORDER BY [Order].Quoted DESC" UpdateCommand="UPDATE [Order] SET Note = @Note WHERE (Id = @Id)">
        <UpdateParameters>
            <asp:Parameter Name="Note" />
            <asp:Parameter Name="Id" />
        </UpdateParameters>
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="SqlDataSourceOrderStatus" runat="server" ConnectionString="<%$ ConnectionStrings:LocalConnectionString %>" SelectCommand="SELECT Id, Text FROM OrderStatus WHERE (Id IN (SELECT DISTINCT Status FROM [Order])) ORDER BY Id"></asp:SqlDataSource>
    <asp:SqlDataSource ID="SqlDataSourceTransaction1" runat="server" ConnectionString="<%$ ConnectionStrings:LocalConnectionString %>" SelectCommand="SELECT [Transaction].ExtRef, [Transaction].Amount, [Transaction].Currency, [Transaction].Info, Currency.Code, [Transaction].FromAccount, [Transaction].Completed, [Transaction].ToAccount, [Transaction].MethodId, [Transaction].Type FROM [Transaction] INNER JOIN Currency ON [Transaction].Currency = Currency.Id WHERE ([Transaction].OrderId = @OrderId)" UpdateCommand="UPDATE [Transaction] SET MethodId = @MethodId, Type = @Type, ExtRef = @ExtRef, Amount = @Amount, Currency = @Currency, Info = @Info, Completed = @Completed, FromAccount = @FromAccount, ToAccount = @ToAccount WHERE (OrderId = @OrderId)">
        <SelectParameters>
            <asp:ControlParameter ControlID="GridViewOrder" Name="OrderId" PropertyName="SelectedValue" />
        </SelectParameters>
        <UpdateParameters>
            <asp:Parameter Name="MethodId" />
            <asp:Parameter Name="Type" />
            <asp:Parameter Name="ExtRef" />
            <asp:Parameter Name="Amount" />
            <asp:Parameter Name="Currency" />
            <asp:Parameter Name="Info" />
            <asp:Parameter Name="Completed" />
            <asp:Parameter Name="FromAccount" />
            <asp:Parameter Name="ToAccount" />
            <asp:Parameter Name="OrderId" />
        </UpdateParameters>
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="SqlDataSourceUser" runat="server" ConnectionString="<%$ ConnectionStrings:LocalConnectionString %>" SelectCommand="SELECT [User].Id, [User].Fname, [User].Phone, [User].Email, Country.Text AS CountryCode, [User].Commission, [User].KycNote, [User].Created, [User].PaymentMethodDetails, [User].CountryId, [User].Trusted FROM [User] INNER JOIN [Order] ON [User].Id = [Order].UserId LEFT OUTER JOIN Country ON [User].CountryId = Country.Id WHERE ([Order].Id = @Id)" UpdateCommand="UPDATE [User] SET PaymentMethodDetails = @PaymentMethodDetails, Fname = @Fname, Phone = @Phone, PhoneVerificationCode = @PhoneVerificationCode, Email = @Email, Address = @Address, Zip = @Zip, CountryId = @CountryId, Trusted = @Trusted WHERE (Id = @Id)">
        <SelectParameters>
            <asp:ControlParameter ControlID="GridViewOrder" Name="Id" PropertyName="SelectedValue" />
        </SelectParameters>
        <UpdateParameters>
            <asp:Parameter Name="PaymentMethodDetails" />
            <asp:Parameter Name="Fname" />
            <asp:Parameter Name="Phone" />
            <asp:Parameter Name="PhoneVerificationCode" />
            <asp:Parameter Name="Email" />
            <asp:Parameter Name="Address" />
            <asp:Parameter Name="Zip" />
            <asp:Parameter Name="CountryId" />
            <asp:Parameter Name="Trusted" />
            <asp:Parameter Name="Id" />
        </UpdateParameters>
    </asp:SqlDataSource>
    <br />
</asp:Content>

