<%@ Page Title="" Language="VB" MasterPageFile="~/MasterPageAdmin.master" AutoEventWireup="false" CodeFile="ApproveBank.aspx.vb" Inherits="ApproveBank2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:LinkButton ID="LinkButtonBACK" runat="server" PostBackUrl="~/Default.aspx">Back</asp:LinkButton>

    <br />
    <br />
    <div class="container body-content">
        <div class="row">
            <%--<div class="col-md-4">
                <asp:TextBox ID="txtOTP" runat="server" CssClass="otptxt" Font-Size="XX-Large" Width="256px"></asp:TextBox>&nbsp;&nbsp;
                        <br />
                <asp:Button ID="BtnUnlock" runat="server" Text="Unlock" Font-Size="XX-Large" />&nbsp;
                <asp:DropDownList ID="DropDownListUnlockPeriode" runat="server" Font-Size="XX-Large">
                    <asp:ListItem Value="300">5min</asp:ListItem>
                    <asp:ListItem Value="600">10min</asp:ListItem>
                    <asp:ListItem Value="1200">20min</asp:ListItem>
                    <asp:ListItem Value="2400">40min</asp:ListItem>
                    <asp:ListItem Value="3600" Selected="True">1hr</asp:ListItem>
                </asp:DropDownList>
                &nbsp;<asp:Label ID="Label1" runat="server" Font-Size="XX-Large"></asp:Label>
            </div>--%>
            <asp:RadioButtonList ID="RadioButtonListSelectMode" runat="server" AutoPostBack="True" CssClass="verdana13" Font-Bold="False" Font-Size="Large" Height="50px">
                <asp:ListItem Selected="True">Auto</asp:ListItem>
                <asp:ListItem>Manual</asp:ListItem>
            </asp:RadioButtonList>
        </div>
    </div>
    <asp:Panel ID="PanelMode" runat="server">
        <table style="width: 100%;">
            <tr>
                <td>
                    <p class="verdana13">ORDER NUMBER:</p>
                </td>
                <td>
                    <asp:TextBox ID="TextBoxOrderNumber" runat="server"></asp:TextBox></td>
                <td>
                    <p class="verdana13">AMOUNT (DKK):</p>
                </td>
                <td>
                    <asp:TextBox ID="TextBoxAmount" runat="server"></asp:TextBox></td>
                <td>
                    <p class="verdana13">TEXT:</p>
                </td>
                <td>
                    <asp:TextBox ID="TextBoxText" runat="server" TextMode="MultiLine" Width="300px"></asp:TextBox></td>
                <td>
                    <asp:Button ID="ButtonSearch" runat="server" Text="SEARCH" /></td>
                <td></td>
            </tr>
        </table>
    </asp:Panel>



    </div>
    </div>
    <table style="width: 100%;">
        <tr>
            <td>
                <p class="verdana13">Orders waiting for payout approval:</p>
            </td>
            <td>&nbsp;</td>
            <td style="text-align: right">
                <asp:Panel ID="PanelNote" runat="server" Visible="False">
                                        &nbsp;&nbsp;
                    <asp:TextBox ID="TextBoxFee" runat="server"  Width="80px"></asp:TextBox>
                    &nbsp;
                    <asp:Button ID="ButtonUpdateFee" runat="server" Text="Update Fee" />
                    &nbsp;&nbsp;
                    <asp:TextBox ID="TextBoxNote" runat="server" TextMode="MultiLine" Width="300px"></asp:TextBox>
                    &nbsp;
                    <asp:Button ID="ButtonAddNote" runat="server" Text="Add Note" />
                </asp:Panel>
            </td>

        </tr>
        <tr>
            <td colspan="3" style="text-align: right">
                <p>
                    <asp:GridView ID="GridViewOrder" runat="server" AllowSorting="True" AutoGenerateColumns="False" DataKeyNames="Id" BackColor="White" BorderColor="#336666" BorderStyle="Double" BorderWidth="3px" CellPadding="4" EmptyDataText="No orders awaits approval" GridLines="Horizontal" Width="100%">
                        <Columns>
                            <asp:TemplateField HeaderText="">
                                <ItemTemplate>
                                    <asp:CheckBox ID="cbSelect" runat="server"></asp:CheckBox>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:CommandField ShowSelectButton="True" SelectText="Review" />
                            <asp:BoundField DataField="Id" HeaderText="Id" ReadOnly="True" SortExpression="Id" Visible="False" />
                            <asp:BoundField DataField="Number" HeaderText="Order Number" SortExpression="Number" ReadOnly="True" />
                            <asp:BoundField DataField="Name" HeaderText="Name" SortExpression="Fname" ReadOnly="True" />
                            <asp:TemplateField HeaderText="Amount" SortExpression="Amount">
                                <EditItemTemplate>
                                    <asp:Label ID="LabelAmount" runat="server" Text='<%# Eval("Amount", "{0:0.00}") %>'></asp:Label>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="LabelAmount" runat="server" Text='<%# Bind("Amount", "{0:0.00}") %>'></asp:Label>
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
                            <asp:BoundField DataField="Email" HeaderText="Email" SortExpression="Email" ReadOnly="True" />
                            <asp:BoundField DataField="IP" HeaderText="IP" SortExpression="IP" ReadOnly="True" />
                            <asp:TemplateField HeaderText="Ask for:">
                                <ItemTemplate>
                                    <asp:Button ID="ButtonReviewKYC" runat="server" BackColor="#FFFF66" BorderStyle="Solid" CommandName="ReviewKYC" Text="KYC Doc" ForeColor="Black" />
                                </ItemTemplate>
                            </asp:TemplateField>
                             <asp:TemplateField HeaderText="Payment">
                                <ItemTemplate>
                                    <asp:Button ID="ButtonRegister" runat="server" BackColor="YellowGreen" BorderStyle="Solid" CommandName="Register" Text="Register" ForeColor="Black" HorizontalAlign="Center"/>
                                </ItemTemplate><HeaderStyle HorizontalAlign="Center" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Cancel" Visible="False">
                                <ItemTemplate>
                                    <asp:Button ID="ButtonCancelOrder" runat="server" BackColor="#FFA8C5" BorderStyle="Solid" CommandName="CancelOrder" Text="Cancel" ForeColor="Black" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="KYC Declined" Visible="False">
                                <ItemTemplate>
                                    <asp:Button ID="ButtonKYCDeclined" runat="server" BackColor="#FFA8C5" BorderStyle="Solid" CommandName="KYCDeclined" Text="Decline" ForeColor="Black" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="SiteId" HeaderText="Site" SortExpression="SiteId" ReadOnly="True" />
                            <asp:TemplateField HeaderText="Note" SortExpression="Note">
                                <EditItemTemplate>
                                    <asp:TextBox ID="TextBoxNote" runat="server" Height="175px" Text='<%# Bind("Note") %>' TextMode="MultiLine" Width="377px"></asp:TextBox>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="Label1" runat="server" Text='<%# Bind("Note") %>' Height="16px"></asp:Label>
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
                    <asp:Button ID="ButtonDeleteSelected" runat="server" Text="Cancelled Selected" />
                    <asp:SqlDataSource ID="SqlDataSourceOrder" runat="server" ConnectionString="<%$ ConnectionStrings:LocalConnectionString %>" SelectCommand="SELECT [Order].Id, [Order].Number, [User].Fname, [Order].Amount, Currency.Code, [Order].Quoted, [Order].CryptoAddress, [Order].Name, [Order].Email, [Order].IP, [Order].Rate, [Order].Note, [Order].SiteId, [Order].CouponId, (CASE WHEN [Order].CouponId is NULL THEN  [Order].OurFee ELSE ([Order].OurFee *(1 - (CONVERT(DECIMAL(15,2),[Coupon].Discount))/100)) END) AS OurFee, [Coupon].Discount as CouponValue, [Coupon].CouponCode as CouponCode FROM Currency INNER JOIN [Order] ON Currency.Id = [Order].CurrencyId LEFT JOIN Coupon on [Order].CouponId = Coupon.Id INNER JOIN OrderStatus ON [Order].Status = OrderStatus.Id INNER JOIN [User] ON [Order].UserId = [User].Id INNER JOIN PaymentType ON [Order].PaymentType = PaymentType.Id WHERE (OrderStatus.Text = N'Quoted') AND (PaymentType.Text = N'Bank') ORDER BY [Order].Quoted DESC" UpdateCommand="UPDATE [Order] SET Note= @Note WHERE (Id = @Id)">
                        <UpdateParameters>
                            <asp:Parameter Name="Note" />
                            <asp:Parameter Name="Id" />
                        </UpdateParameters>
                    </asp:SqlDataSource>
                    <asp:SqlDataSource ID="SqlDataSourceOrderManual" runat="server" ConnectionString="<%$ ConnectionStrings:LocalConnectionString %>" SelectCommand="SELECT [Order].Id, [Order].Number, [User].Fname, [Order].Amount, Currency.Code, [Order].Quoted, [Order].CryptoAddress, [Order].Name, [Order].Email, [Order].IP, [Order].CommissionProcent, [Order].Note, [Order].SiteId FROM Currency INNER JOIN [Order] ON Currency.Id = [Order].CurrencyId INNER JOIN OrderStatus ON [Order].Status = OrderStatus.Id INNER JOIN [User] ON [Order].UserId = [User].Id INNER JOIN PaymentType ON [Order].PaymentType = PaymentType.Id WHERE (OrderStatus.Text = N'Quoted' AND [Order].Type = 1) AND (PaymentType.Text = N'Bank') ORDER BY [Order].Quoted DESC" UpdateCommand="UPDATE [Order] SET Note= @Note WHERE (Id = @Id)">
                        <UpdateParameters>
                            <asp:Parameter Name="Note" />
                            <asp:Parameter Name="Id" />
                        </UpdateParameters>
                    </asp:SqlDataSource>
                    <asp:SqlDataSource ID="SqlDataSourceOrderAuto" runat="server" ConnectionString="<%$ ConnectionStrings:LocalConnectionString %>" SelectCommand="SELECT [Order].Id, [Order].Number, [User].Fname, [Order].Amount, Currency.Code, [Order].Quoted, [Order].CryptoAddress, [Order].Name, [Order].Email, [Order].IP, [Order].CommissionProcent, [Order].Note, [Order].SiteId FROM Currency INNER JOIN [Order] ON Currency.Id = [Order].CurrencyId INNER JOIN OrderStatus ON [Order].Status = OrderStatus.Id INNER JOIN [User] ON [Order].UserId = [User].Id INNER JOIN PaymentType ON [Order].PaymentType = PaymentType.Id WHERE (OrderStatus.Text = N'Quoted' AND [Order].Type = 1) AND (PaymentType.Text = N'Bank') AND ([Order].Number = @Number) AND ([Order].Amount = @Amount) ORDER BY [Order].Quoted DESC" UpdateCommand="UPDATE [Order] SET Note= @Note WHERE (Id = @Id)">
                        <SelectParameters>
                            <asp:ControlParameter ControlID="TextBoxOrderNumber" Name="Number" PropertyName="Text" />
                            <asp:ControlParameter ControlID="TextBoxAmount" Name="Amount" PropertyName="Text" />
                        </SelectParameters>
                        <UpdateParameters>
                            <asp:Parameter Name="Note" />
                            <asp:Parameter Name="Id" />
                        </UpdateParameters>
                    </asp:SqlDataSource>
                </p>
                <p>
                    &nbsp;
                </p>
            </td>
        </tr>
    </table>
    <p class="verdana13">
        Transaction on Order:
    </p>
    <p>
        <asp:GridView ID="GridViewTransaction" runat="server" AutoGenerateColumns="False" DataSourceID="SqlDataSourceTransaction" EmptyDataText="No records" BackColor="#DEBA84" BorderColor="#DEBA84" BorderStyle="None" BorderWidth="1px" CellPadding="3" CellSpacing="2" DataKeyNames="Id">
            <Columns>
                <asp:CommandField ShowEditButton="True" />
                <asp:BoundField DataField="Amount" HeaderText="Amount" SortExpression="Amount" />
                <asp:BoundField DataField="Currency" HeaderText="Currency" SortExpression="Currency" />
                <asp:TemplateField HeaderText="Info" SortExpression="Info">
                    <EditItemTemplate>
                        <asp:TextBox ID="TextBox1" runat="server" Height="233px" Text='<%# Bind("Info") %>' TextMode="MultiLine" Width="492px"></asp:TextBox>
                    </EditItemTemplate>
                    <ItemTemplate>
                        <asp:Label ID="Label1" runat="server" Text='<%# Bind("Info") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
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
        User Details:
    </p>
    <p>
        <asp:GridView ID="GridViewUser" runat="server" AutoGenerateColumns="False" DataKeyNames="Id" DataSourceID="SqlDataSourceUser" EmptyDataText="No record" BackColor="#DEBA84" BorderColor="#DEBA84" BorderStyle="None" BorderWidth="1px" CellPadding="3" CellSpacing="2">
            <Columns>
                <asp:BoundField DataField="Id" HeaderText="User Id" InsertVisible="False" ReadOnly="True" SortExpression="Id" />
                <asp:BoundField DataField="Fname" HeaderText="Name" SortExpression="Fname" />
                <asp:TemplateField HeaderText="Phone" SortExpression="Phone">
                    <EditItemTemplate>
                        <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("Phone") %>'></asp:TextBox>
                    </EditItemTemplate>
                    <ItemTemplate>

                        <asp:HyperLink ID="HyperLinkTwilio" runat="server" Target="_blank" NavigateUrl='<%# Eval("Phone")%>' Text='<%# Bind("Phone")%>'></asp:HyperLink>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="Email" HeaderText="Email" SortExpression="Email" />
                <asp:BoundField DataField="CountryCode" HeaderText="CountryCode" SortExpression="CountryCode" />
                <asp:BoundField DataField="KycNote" HeaderText="KycNote" SortExpression="KycNote" />
                <asp:BoundField DataField="Created" HeaderText="Created" SortExpression="Created" />
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
     <p class="verdana13">User Audit Trail:</p>
    <p>
        <asp:GridView ID="GridViewUserAuditTrail" runat="server" AutoGenerateColumns="False" EmptyDataText="No records" BackColor="#DEBA84" BorderColor="#DEBA84" BorderStyle="None" BorderWidth="1px" CellPadding="3" CellSpacing="2">
            <Columns>
                <asp:BoundField DataField="Text" HeaderText="Origin" SortExpression="Text" />
                <asp:TemplateField HeaderText="Message" SortExpression="Message">
                    <EditItemTemplate>
                        <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("Message") %>'></asp:TextBox>
                    </EditItemTemplate>
                    <ItemTemplate>
                        <asp:Label ID="Label1" runat="server" Text='<%# Bind("Message") %>' ReadOnly="True" TextMode="MultiLine"  style="word-wrap:break-word;"></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="Created" HeaderText="Created" SortExpression="Created" />
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
      <%--<asp:SqlDataSource ID="SqlDataSourceUserAuditTrail" runat="server" ConnectionString="<%$ ConnectionStrings:LocalConnectionString %>" SelectCommand="SELECT AuditTrail.OrderId, AuditTrail.Status, AuditTrail.Message, AuditTrail.Created, AuditTrailStatus.Text FROM AuditTrail INNER JOIN AuditTrailStatus ON AuditTrail.Status = AuditTrailStatus.Id INNER JOIN [Order] ON [Order].UserId = (SELECT UserId FROM [Order] WHERE ID = @Id) WHERE AuditTrail.Message LIKE '%CUSTOMER:'+  CAST((SELECT UserId FROM [Order] WHERE ID = @Id) AS NVARCHAR(3)) + '%' ORDER BY AuditTrail.Created">
        <SelectParameters>
            <asp:ControlParameter ControlID="GridViewOrder" Name="Id" PropertyName="SelectedValue" />
        </SelectParameters>
    </asp:SqlDataSource>--%>
    <asp:Panel ID="PanelTrustCalculation" runat="server" Visible="False">
        <table bgcolor="White" style="border-style: solid; border-width: 1px; width: 100%;">
            <tr>
                <td align="left" colspan="3">
                    <asp:Label ID="LabelTrustMessage" runat="server" Text="Label"></asp:Label>&nbsp;&nbsp;&nbsp;&nbsp;
                                <asp:Button ID="ButtonRunAutoSettle" runat="server" Text="Excecute Now!" Visible="False" />
                </td>
            </tr>
            <tr>
                <td style="padding: 7px; margin: 5px; text-align: center;">TRUST LOGIC</td>
                <td style="padding: 7px; margin: 5px; text-align: center;">ORDERS</td>
                <td style="padding: 7px; margin: 5px; text-align: center;">USER</td>
            </tr>
            <tr>
                <td valign="top" align="center">
                    <table style="border-style: solid; border-width: 1px; width: 340px; text-align: left;">
                        <tr>
                            <td>Card Approved</td>
                            <td style="width: 100px">
                                <asp:Label ID="LabelCardApproved" runat="server" Font-Bold="True" Text="Label" Width="55px"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>Tx Limit</td>
                            <td>
                                <asp:Label ID="LabelTxLimit" runat="server" Font-Bold="True" Text="Label" Width="55px"></asp:Label></td>
                        </tr>
                        <tr>
                            <td>Card Sum</td>
                            <td>
                                <asp:Label ID="LabelCardTotal" runat="server" Font-Bold="True" Text="Label" Width="55px"></asp:Label></td>
                        </tr>
                        <tr>
                            <td>Virtual Phone</td>
                            <td>
                                <asp:Label ID="LabelPhoneVirtual" runat="server" Font-Bold="True" Text="Label" Width="55px"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>Phone/Card Match</td>
                            <td>
                                <asp:Label ID="LabelPhoneCardMatch" runat="server" Font-Bold="True" Text="Label" Width="55px"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>Phone/IP Match</td>
                            <td>
                                <asp:Label ID="LabelPhoneIpMatch" runat="server" Font-Bold="True" Text="Label" Width="55px"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>Card Used Elsewhere</td>
                            <td>
                                <asp:LinkButton ID="LinkButtonCreditCardUsedElsewhere" runat="server" Font-Bold="True" Width="55px">-</asp:LinkButton>
                            </td>
                        </tr>
                        <tr>
                            <td>Email used Elsewhere</td>
                            <td>
                                <asp:LinkButton ID="LinkButtonEmailUsedElsewhere" runat="server" Font-Bold="True" Width="55px">-</asp:LinkButton>
                            </td>
                        </tr>
                        <tr>
                            <td>US-Card</td>
                            <td>
                                <asp:Label ID="LabelCardUS" runat="server" Font-Bold="True" Text="Label" Width="55px"></asp:Label>
                            </td>
                        </tr>
                    </table>
                </td>
                <td valign="top" align="center">
                    <table style="border-style: solid; border-width: 1px; width: 340px; text-align: left;">
                        <tr>
                            <td>Completed</td>
                            <td>
                                <asp:Label ID="LabelCompleted" runat="server" Font-Bold="True" Text="Label" Width="55px"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>Span</td>
                            <td>
                                <asp:Label ID="LabelOrderSpan" runat="server" Font-Bold="True" Text="Label" Width="55px"></asp:Label></td>
                        </tr>
                        <tr>
                            <td>Order Sum</td>
                            <td>
                                <asp:Label ID="LabelOrderSUM" runat="server" Font-Bold="True" Text="Label" Width="55px"></asp:Label></td>
                        </tr>
                        <tr>
                            <td>Sum Total</td>
                            <td>
                                <asp:Label ID="LabelOrderTotal" runat="server" Font-Bold="True" Text="Label" Width="55px"></asp:Label></td>
                        </tr>
                        <tr>
                            <td>Sum 30 Days</td>
                            <td>
                                <asp:Label ID="LabelOrder30Days" runat="server" Font-Bold="True" Text="Label" Width="55px"></asp:Label>
                            </td>
                        </tr>
                    </table>
                    <br />
                </td>
                <td valign="top" align="center">
                    <table style="border-style: solid; border-width: 1px; width: 340px; text-align: left;">
                        <tr>
                            <td>Trusted</td>
                            <td>
                                <asp:Label ID="LabelUserIsTrusted" runat="server" Font-Bold="True" Text="Label" Width="55px"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td class="auto-style1">Number of Cards</td>
                            <td class="auto-style1">
                                <asp:Label ID="LabelCards" runat="server" Font-Bold="True" Text="Label" Width="55px"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>Used Names</td>
                            <td>
                                <asp:Label ID="LabelNames" runat="server" Font-Bold="True" Text="Label"></asp:Label></td>
                        </tr>
                        <tr>
                            <td>Used Emails</td>
                            <td>
                                <asp:Label ID="LabelEmails" runat="server" Font-Bold="True" Text="Label"></asp:Label></td>
                        </tr>
                        <tr>
                            <td>KYC Approved</td>
                            <td>
                                <asp:Label ID="LabelKycApproved" runat="server" Font-Bold="True" Text="Label"></asp:Label></td>
                        </tr>
                        <tr>
                            <td>KYC Declined</td>
                            <td>
                                <asp:Label ID="LabelKycDeclined" runat="server" Font-Bold="True" Text="Label" Width="55px"></asp:Label></td>
                        </tr>
                        <tr>
                            <td>Phone</td>
                            <td>
                                <asp:Label ID="LabelPhoneCountry" runat="server" Font-Bold="True" Text="Label" Width="55px"></asp:Label></td>
                        </tr>
                        <tr>
                            <td>Card</td>
                            <td>
                                <asp:Label ID="LabelCardCountry" runat="server" Font-Bold="True" Text="Label" Width="55px"></asp:Label></td>
                        </tr>
                        <tr>
                            <td>IP</td>
                            <td>
                                <asp:Label ID="LabelIPCountry" runat="server" Font-Bold="True" Text="Label" Width="55px"></asp:Label></td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
    </asp:Panel>
    <p class="verdana13">Orders in total:</p>
    <asp:GridView ID="GridViewSubOrders" runat="server" AutoGenerateColumns="False" DataKeyNames="Id" DataSourceID="SqlDataSourceOrderChild" EmptyDataText="No record" BackColor="White" BorderColor="#CCCCCC" BorderStyle="None" BorderWidth="1px" CellPadding="3" AllowSorting="True">
        <Columns>
            <asp:BoundField DataField="Id" HeaderText="Id" InsertVisible="False" ReadOnly="True" SortExpression="Id" />
            <asp:BoundField DataField="Number" HeaderText="Number" SortExpression="Number" />
            <asp:BoundField DataField="Amount" HeaderText="Amount" SortExpression="Amount" DataFormatString="{0:0.00}" />
            <asp:BoundField DataField="Code" HeaderText="Code" SortExpression="Code" />
            <asp:BoundField DataField="Quoted" HeaderText="Quoted" SortExpression="Quoted" />
            <asp:BoundField DataField="Rate" HeaderText="Rate" SortExpression="Rate" />
            <asp:BoundField DataField="OurFee" HeaderText="Fee (%)" SortExpression="OurFee" />
            <asp:BoundField DataField="CryptoAddress" HeaderText="CryptoAddress" SortExpression="CryptoAddress" />
            <asp:BoundField DataField="Name" HeaderText="Name" SortExpression="Name" />
            <asp:BoundField DataField="Email" HeaderText="Email" SortExpression="Email" />
            <asp:BoundField DataField="IP" HeaderText="IP" SortExpression="IP" />
            <asp:TemplateField HeaderText="Note" SortExpression="Note">
                <EditItemTemplate>
                    <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("Note") %>'></asp:TextBox>
                </EditItemTemplate>
                <ItemTemplate>
                    <asp:Label ID="Label1" runat="server" Text='<%# Bind("Note") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="Status" HeaderText="Status" SortExpression="Status" />
            <asp:BoundField DataField="CardNumber" HeaderText="CardNumber" SortExpression="CardNumber" />
            <asp:BoundField DataField="TxSecret" HeaderText="TxSecret" SortExpression="TxSecret" />
            <asp:BoundField DataField="BrowserCulture" HeaderText="Culture" SortExpression="BrowserCulture" />
            <asp:BoundField DataField="SiteId" HeaderText="SiteId" SortExpression="SiteId" />
            <asp:BoundField DataField="CouponId" HeaderText="CouponId" SortExpression="CouponId" />
            <asp:BoundField DataField="CouponValue" HeaderText="CouponValue" SortExpression="CouponValue" />
            <asp:BoundField DataField="CouponCode" HeaderText="CouponCode" SortExpression="CouponCode" />
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
    <asp:SqlDataSource ID="SqlDataSourceOrderChild" runat="server" ConnectionString="<%$ ConnectionStrings:LocalConnectionString %>" SelectCommand="SELECT [Order].Id, [Order].Number, [User].Fname, [Order].Amount, Currency.Code, [Order].Quoted, [Order].CryptoAddress, [Order].Name, [Order].Email, [Order].IP, OrderStatus.Text AS Status, [Order].Note, [Order].Rate, (CASE WHEN [Order].CouponId is NULL THEN  [Order].OurFee ELSE ([Order].OurFee *(1 - (CONVERT(DECIMAL(15,2),[Coupon].Discount))/100)) END) AS OurFee, [Order].CardNumber, [Order].SiteId, SUBSTRING([Order].CountryCode, 1, 5) AS BrowserCulture, [Order].TxSecret, [Order].CouponId, [Coupon].Discount as CouponValue, [Coupon].CouponCode as CouponCode FROM Currency INNER JOIN [Order] ON Currency.Id = [Order].CurrencyId LEFT JOIN Coupon on [Order].CouponId = Coupon.Id INNER JOIN OrderStatus ON [Order].Status = OrderStatus.Id INNER JOIN [User] ON [Order].UserId = [User].Id INNER JOIN [Order] AS Order_1 ON [User].Id = Order_1.UserId WHERE (Order_1.Id = @Id) ORDER BY [Order].Quoted DESC">
        <SelectParameters>
            <asp:ControlParameter ControlID="GridViewOrder" Name="Id" PropertyName="SelectedValue" />
        </SelectParameters>
    </asp:SqlDataSource>

    <p class="verdana13">
        KYC Files:
    </p>
    <asp:GridView ID="GridViewKycFile" runat="server" AutoGenerateColumns="False" DataKeyNames="Id" DataSourceID="SqlDataSourceKycFileAll" AllowPaging="True" AllowSorting="True" CellPadding="3" BackColor="White" BorderColor="#CCCCCC" BorderStyle="None" BorderWidth="1px">
        <Columns>
            <asp:TemplateField ShowHeader="False">
                <ItemTemplate>
                    <asp:LinkButton ID="LinkButton1" runat="server" CausesValidation="False" CommandName="Select" Text="Show Image"></asp:LinkButton>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:CommandField ShowEditButton="True" ButtonType="Button" />
            <asp:TemplateField HeaderText="Type" SortExpression="Type">
                <EditItemTemplate>
                    <asp:DropDownList ID="DropDownListKycType0" runat="server" DataSourceID="SqlDataSourceKycType" DataTextField="Text" DataValueField="Id" SelectedValue='<%# Bind("Type") %>'>
                    </asp:DropDownList>
                </EditItemTemplate>
                <ItemTemplate>
                    <asp:Label ID="Label5" runat="server" Text='<%# Bind("Text") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="OriginalFilename" HeaderText="OriginalFilename" SortExpression="OriginalFilename" ReadOnly="True" />
            <asp:BoundField DataField="Uploaded" HeaderText="Uploaded" SortExpression="Uploaded" ReadOnly="True" />

            <asp:TemplateField HeaderText="Rejected" SortExpression="Rejected">
                <EditItemTemplate>
                    <asp:Button ID="ButtonReject" runat="server" CommandName="Reject" Text="Reject" />
                    <asp:Button ID="ButtonUndoReject" runat="server" CommandName="UndoReject" Text="X" />
                </EditItemTemplate>
                <ItemTemplate>
                    <asp:Label ID="LabelReject" runat="server" Text='<%# Bind("Rejected") %>' Width="100px"></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Approved" SortExpression="Approved">
                <EditItemTemplate>
                    <asp:Button ID="ButtonApprove" runat="server" CommandName="Approve" Text="Approve" CommandArgument="<%# Container.DataItemIndex %>"/>
                    <asp:Button ID="ButtonUndoApprove" runat="server" CommandName="UndoApprove" Text="X" CommandArgument="<%# Container.DataItemIndex %>"/>
                </EditItemTemplate>
                <ItemTemplate>
                    <asp:Label ID="LabelApprove" runat="server" Text='<%# Bind("Approved") %>' Width="100px"></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Obsolete" SortExpression="Obsolete">
                <EditItemTemplate>
                    <asp:Button ID="ButtonObsolete" runat="server" CommandName="Obsolete" Text="Make Obsolete" />
                    <asp:Button ID="ButtonUndoObsolete" runat="server" CommandName="UndoObsolete" Text="X" />
                </EditItemTemplate>
                <ItemTemplate>
                    <asp:Label ID="LabelObsolete" runat="server" Text='<%# Bind("Obsolete") %>' Width="100px"></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Note" SortExpression="Note">
                <EditItemTemplate>
                    <asp:TextBox ID="TextBox3" runat="server" Height="85px" Text='<%# Bind("Note") %>' TextMode="MultiLine" Width="206px"></asp:TextBox>
                </EditItemTemplate>
                <ItemTemplate>
                    <asp:Label ID="Label6" runat="server" Text='<%# Bind("Note") %>' Width="100px"></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField ShowHeader="False">
                <ItemTemplate>
                    <asp:LinkButton ID="LinkButtonDelete" runat="server" CausesValidation="False" CommandName="Delete" Text="Delete"></asp:LinkButton>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
        <FooterStyle BackColor="White" ForeColor="#000066" />
        <HeaderStyle BackColor="#006699" Font-Bold="True" ForeColor="White" />
        <PagerStyle BackColor="White" ForeColor="#000066" HorizontalAlign="Left" />
        <RowStyle ForeColor="#000066" />
        <SelectedRowStyle BackColor="#669999" Font-Bold="True" ForeColor="White" />
    </asp:GridView>
    <asp:SqlDataSource ID="SqlDataSourceKycFileAll" runat="server" ConnectionString="<%$ ConnectionStrings:LocalConnectionString %>" DeleteCommand="DELETE FROM [KycFile] WHERE [Id] = @Id" InsertCommand="INSERT INTO [KycFile] ([Note], [Uploaded], [Rejected], [RejectedBy], [Approved], [ApprovedBy]) VALUES (@Note, @Uploaded, @Rejected, @RejectedBy, @Approved, @ApprovedBy)" SelectCommand="SELECT KycFile.Id, KycFile.UserId, NULL as [File] ,KycFile.Note, KycFile.Uploaded, KycFile.Rejected, KycFile.RejectedBy, KycFile.Approved, KycFile.ApprovedBy, KycType.Text, KycFile.Obsolete, KycFile.OriginalFilename, KycFile.Type, [Order].Id AS OrderId FROM KycFile INNER JOIN KycType ON KycFile.Type = KycType.Id INNER JOIN [Order] ON KycFile.UserId = [Order].UserId WHERE ([Order].Id = @OrderId)" UpdateCommand="UPDATE KycFile SET Note = @Note, Type = @Type WHERE (Id = @Id)">
        <DeleteParameters>
            <asp:Parameter Name="Id" Type="Int32" />
        </DeleteParameters>
        <InsertParameters>
            <asp:Parameter Name="Note" Type="String" />
            <asp:Parameter Name="Uploaded" Type="DateTime" />
            <asp:Parameter Name="Rejected" Type="DateTime" />
            <asp:Parameter Name="RejectedBy" Type="Int32" />
            <asp:Parameter Name="Approved" Type="DateTime" />
            <asp:Parameter Name="ApprovedBy" Type="Int32" />
        </InsertParameters>
        <SelectParameters>
            <asp:ControlParameter ControlID="GridViewOrder" Name="OrderId" PropertyName="SelectedValue" />
        </SelectParameters>
        <UpdateParameters>
            <asp:Parameter Name="Note" Type="String" />
            <asp:Parameter Name="Type" />
            <asp:Parameter Name="Id" Type="Int32" />
        </UpdateParameters>
    </asp:SqlDataSource>
    </p>
    <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" CellPadding="4" DataKeyNames="Id" DataSourceID="SqlDataSourceKycImage" EnableModelValidation="True" ForeColor="#333333" GridLines="None" Width="700">
        <AlternatingRowStyle BackColor="White" />
        <Columns>
            <asp:ImageField DataImageUrlField="Id" DataImageUrlFormatString="ImageVB.aspx?ImageID={0}" HeaderText="Preview Image" ControlStyle-Width="900">
            </asp:ImageField>
            <asp:BoundField DataField="Id" HeaderText="Id" InsertVisible="False" ReadOnly="True" SortExpression="Id" Visible="False" />
        </Columns>
        <EditRowStyle BackColor="#2461BF" />
        <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
        <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
        <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
        <RowStyle BackColor="#EFF3FB" />
        <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
    </asp:GridView>
    <asp:SqlDataSource ID="SqlDataSourceKycImage" runat="server" ConnectionString="<%$ ConnectionStrings:LocalConnectionString %>" SelectCommand="SELECT Id ,NULL as [File] FROM KycFile WHERE (Id = @Id)">
        <SelectParameters>
            <asp:ControlParameter ControlID="GridViewKycFile" Name="Id" PropertyName="SelectedValue" />
        </SelectParameters>
    </asp:SqlDataSource>
    <p>
        <asp:DropDownList ID="DropDownListKycType" runat="server" DataSourceID="SqlDataSourceKycType" DataTextField="Text" DataValueField="Id" CssClass="DropdownList">
        </asp:DropDownList>&nbsp;&nbsp;
    <asp:FileUpload ID="FileUpload1" runat="server" />
        <asp:Button ID="btnUpload" runat="server" Text="Upload" />
        <asp:SqlDataSource ID="SqlDataSourceKycType" runat="server" ConnectionString="<%$ ConnectionStrings:LocalConnectionString %>" SelectCommand="SELECT * FROM [KycType]"></asp:SqlDataSource>
        &nbsp;
    <br />
        <asp:Label ID="lblMessage" runat="server" Text="" Font-Names="Arial"></asp:Label>
        <br />
    </p>
    <p class="verdana13">Audit Trail:</p>
    <p>
        <asp:GridView ID="GridViewAuditTrail" runat="server" AutoGenerateColumns="False" DataSourceID="SqlDataSourceAuditTrail" EmptyDataText="No records" BackColor="#DEBA84" BorderColor="#DEBA84" BorderStyle="None" BorderWidth="1px" CellPadding="3" CellSpacing="2">
            <Columns>
                <asp:BoundField DataField="Text" HeaderText="Origin" SortExpression="Text" />
                <asp:TemplateField HeaderText="Message" SortExpression="Message">
                    <EditItemTemplate>
                        <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("Message") %>'></asp:TextBox>
                    </EditItemTemplate>
                    <ItemTemplate>
                        <asp:Label ID="TextBox1" runat="server" Text='<%# Bind("Message") %>' ReadOnly="True" TextMode="MultiLine" Height="16px" Width="1000" ></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="Created" HeaderText="Created" SortExpression="Created" />
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
    <asp:SqlDataSource ID="SqlDataSourceOrderChildNote" runat="server" ConnectionString="<%$ ConnectionStrings:LocalConnectionString %>" SelectCommand="SELECT Id, Note FROM [Order] WHERE (Id = @Id)" UpdateCommand="UPDATE [Order] SET Note = @Note WHERE (Id = @Id)">
        <SelectParameters>
            <asp:ControlParameter ControlID="GridViewOrder" Name="Id" PropertyName="SelectedValue" />
        </SelectParameters>
        <UpdateParameters>
            <asp:Parameter Name="Note" />
            <asp:Parameter Name="Id" />
        </UpdateParameters>
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="SqlDataSourceTransaction" runat="server" ConnectionString="<%$ ConnectionStrings:LocalConnectionString %>" SelectCommand="SELECT [Transaction].Id, [Transaction].ExtRef, [Transaction].Amount, [Transaction].Currency, [Transaction].Info, Currency.Code FROM [Transaction] INNER JOIN Currency ON [Transaction].Currency = Currency.Id WHERE ([Transaction].OrderId = @OrderId)" UpdateCommand="UPDATE [Transaction] SET Amount = @Amount, Currency = @Currency, Info = @Info, Completed = @Completed WHERE (Id = @id)">
        <SelectParameters>
            <asp:ControlParameter ControlID="GridViewOrder" Name="OrderId" PropertyName="SelectedValue" />
        </SelectParameters>
        <UpdateParameters>
            <asp:Parameter Name="Amount" Type="Decimal" />
            <asp:Parameter Name="Currency" Type="Int16" />
            <asp:Parameter Name="Info" />
            <asp:Parameter Name="Completed" Type="DateTime" />
            <asp:Parameter Name="id" />
        </UpdateParameters>
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="SqlDataSourceAuditTrail" runat="server" ConnectionString="<%$ ConnectionStrings:LocalConnectionString %>" SelectCommand="SELECT AuditTrail.OrderId, AuditTrail.Status, AuditTrail.Message, AuditTrail.Created, AuditTrailStatus.Text FROM AuditTrail INNER JOIN AuditTrailStatus ON AuditTrail.Status = AuditTrailStatus.Id WHERE (AuditTrail.OrderId = @OrderId) ORDER BY AuditTrail.Created">
        <SelectParameters>
            <asp:ControlParameter ControlID="GridViewOrder" Name="OrderId" PropertyName="SelectedValue" />
        </SelectParameters>
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="SqlDataSourceUser" runat="server" ConnectionString="<%$ ConnectionStrings:LocalConnectionString %>" SelectCommand="SELECT [User].Id, [User].Fname, [User].Phone, [User].Email, Country.Text AS CountryCode, [User].KycNote, [User].Created, [Order].CommissionProcent, [User].PaymentMethodDetails FROM [User] INNER JOIN [Order] ON [User].Id = [Order].UserId LEFT OUTER JOIN Country ON [User].CountryId = Country.Id WHERE ([Order].Id = @Id)">
        <SelectParameters>
            <asp:ControlParameter ControlID="GridViewOrder" Name="Id" PropertyName="SelectedValue" />
        </SelectParameters>
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="SqlDataSourceKycFile" runat="server" ConnectionString="<%$ ConnectionStrings:LocalConnectionString %>" SelectCommand="SELECT [Order].Id, KycFile.Note, KycFile.OriginalFilename, KycFile.Uploaded, KycFile.Approved, KycType.Text, KycFile.Rejected, KycFile.Obsolete FROM [User] INNER JOIN [Order] ON [User].Id = [Order].UserId INNER JOIN KycFile ON [User].Id = KycFile.UserId INNER JOIN KycType ON KycFile.Type = KycType.Id WHERE ([Order].Id = @Id)">
        <SelectParameters>
            <asp:ControlParameter ControlID="GridViewOrder" Name="Id" PropertyName="SelectedValue" />
        </SelectParameters>
    </asp:SqlDataSource>
</asp:Content>

