<%@ Page Title="" Language="VB" MasterPageFile="~/MasterPageAdmin.master" AutoEventWireup="false" Inherits="WMC.Admin.Web.FindOrders" Codebehind="FindOrders.aspx.vb" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:LinkButton ID="LinkButtonBACK" runat="server" PostBackUrl="~/Default.aspx">Back</asp:LinkButton>
    <br />
    <br />
    <div class="verdana13">
        Find by:
        <asp:DropDownList ID="DropDownListWhereType" runat="server" CssClass="DropdownList">
            <asp:ListItem>OrderID</asp:ListItem>
            <asp:ListItem>CC Number</asp:ListItem>
            <asp:ListItem>Email</asp:ListItem>
            <asp:ListItem>Name</asp:ListItem>
            <asp:ListItem>YourPay ID</asp:ListItem>
            <asp:ListItem>TransactionHash</asp:ListItem>
            <asp:ListItem>TxSecret</asp:ListItem>
            <asp:ListItem>Phone</asp:ListItem>
        </asp:DropDownList>&nbsp;
        <asp:TextBox ID="TextBoxOrderID" runat="server"></asp:TextBox>
        &nbsp;<asp:Button ID="ButtonFindByOrderID" runat="server" Text="Find" />
    </div>
    <p class="verdana13">
        Found orders:
    </p>
    <table style="width: 100%;">
        <tr>
            <td>
                <p class="verdana13">Orders:</p>
            </td>
            <td>&nbsp;</td>
            <td style="text-align: right">
                <asp:Panel ID="PanelNote" runat="server" Visible="False">
                    <asp:TextBox ID="TextBoxNote" runat="server" TextMode="MultiLine" Width="300px"></asp:TextBox>&nbsp;
                    <asp:Button ID="ButtonAddNote" runat="server" Text="Add Note" />
                </asp:Panel>
            </td>

        </tr>
        <tr>
            <td colspan="3" style="text-align: right">
                <p>
                    <asp:GridView ID="GridViewOrder" runat="server" AllowPaging="True" AllowSorting="True" AutoGenerateColumns="False" DataKeyNames="Id" BackColor="White" BorderColor="#336666" BorderStyle="Double" BorderWidth="3px" CellPadding="4" EmptyDataText="No orders awaits approval" GridLines="Horizontal" PageSize="100" Width="100%">
                        <Columns>
                            <asp:CommandField ShowSelectButton="True" SelectText="Review" />
                            <asp:BoundField DataField="Id" HeaderText="Id" ReadOnly="True" SortExpression="Id" Visible="False" />
                            <asp:BoundField DataField="Number" HeaderText="Order Number" SortExpression="Number" ReadOnly="True" />
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
                            <asp:BoundField DataField="IP" HeaderText="IP" SortExpression="IP" ReadOnly="True" />
                            <asp:TemplateField HeaderText="Approve" Visible="False">
                                <ItemTemplate>
                                    <asp:Button ID="ButtonApprove" runat="server" BackColor="#66FF66" BorderStyle="Solid" CommandName="Approve" Text="Approve" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Cancel">
                                <ItemTemplate>
                                    <asp:Button ID="ButtonCancelOrder" runat="server" BackColor="#FFA8C5" BorderStyle="Solid" CommandName="CancelOrder" Text="Cancel" ForeColor="Black" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="KYC Declined">
                                <ItemTemplate>
                                    <asp:Button ID="ButtonKYCDeclined" runat="server" BackColor="#FFA8C5" BorderStyle="Solid" CommandName="KYCDeclined" Text="Decline" ForeColor="Black" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Rate" SortExpression="Rate">
                                <EditItemTemplate>
                                    <asp:Label ID="Label1" runat="server" Text='<%# Bind("Rate") %>'></asp:Label>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="Label1" runat="server" Text='<%# Bind("Rate") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="SiteId" HeaderText="SiteId" SortExpression="SiteId" />
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
        User:
    </p>
    <p>
        <asp:GridView ID="GridViewUser" runat="server" AutoGenerateColumns="False" DataKeyNames="Id" DataSourceID="SqlDataSourceUser" EmptyDataText="No record" BackColor="#DEBA84" BorderColor="#DEBA84" BorderStyle="None" BorderWidth="1px" CellPadding="3" CellSpacing="2">
            <Columns>
                <asp:BoundField DataField="Id" HeaderText="User Id" InsertVisible="False" ReadOnly="True" SortExpression="Id" />
                <asp:BoundField DataField="Fname" HeaderText="Name" SortExpression="Fname" ReadOnly="True" />
                <asp:TemplateField HeaderText="Phone" SortExpression="Phone">
                    <EditItemTemplate>
                        <asp:Label ID="Label5" runat="server" Text='<%# Bind("Phone") %>'></asp:Label>
                    </EditItemTemplate>
                    <ItemTemplate>
                        <asp:HyperLink ID="HyperLinkTwilio" runat="server" Target="_blank" NavigateUrl='<%# Eval("Phone")%>' Text='<%# Bind("Phone")%>'></asp:HyperLink>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="Email" HeaderText="Email" SortExpression="Email" ReadOnly="True" />
                <asp:BoundField DataField="CountryCode" HeaderText="CountryCode" SortExpression="CountryCode" ReadOnly="True" />
                <asp:BoundField DataField="KycNote" HeaderText="KycNote" SortExpression="KycNote" ReadOnly="True" />
                <asp:BoundField DataField="Created" HeaderText="Created" SortExpression="Created" ReadOnly="True" />
                <asp:BoundField DataField="Commission" HeaderText="Commission" SortExpression="Commission" ReadOnly="True" />
                <asp:TemplateField HeaderText="PaymentMethodDetails" SortExpression="PaymentMethodDetails">
                    <EditItemTemplate>
                        <asp:TextBox ID="TextBox2" runat="server" Text='<%# Bind("PaymentMethodDetails") %>' TextMode="MultiLine" Height="237px" Width="531px"></asp:TextBox>
                    </EditItemTemplate>
                    <ItemTemplate>
                        <asp:Label ID="Label1" runat="server" Text='<%# Bind("PaymentMethodDetails") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:CommandField ShowEditButton="True"></asp:CommandField>
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
    <%--<table bgcolor="White" style="border-style: solid; border-width: 1px;">
        <tr>
            <td align="center" class="auto-style2" bgcolor="#72FA83" style="padding: 7px; margin: 5px">Completed orders</td>
            <td align="center" class="auto-style2" bgcolor="#72FA83" style="padding: 7px; margin: 5px">KYC Declined orders</td>
            <td align="center" class="auto-style4" bgcolor="#72FA83" style="padding: 7px; margin: 5px">Quoted orders
            </td>
            <td align="center" class="auto-style6" bgcolor="#72FA83" style="padding: 7px; margin: 5px">Crypto used elsewhere</td>
            <td align="center" class="auto-style8" bgcolor="#72FA83" style="padding: 7px; margin: 5px">E-mail used elsewhere </td>
            <td align="center" class="style2" bgcolor="#72FA83" style="padding: 7px; margin: 5px">IP used elsewhere</td>
            <td align="center" class="style2" bgcolor="#72FA83" style="padding: 7px; margin: 5px">Card used elsewhere</td>
        </tr>
        <tr>
            <td valign="middle" align="center" class="auto-style3">
                <br />
                <br />
                <asp:Label ID="LabelCompletedOrders" runat="server" Text="-" Font-Bold="True" Font-Overline="False" Font-Size="XX-Large" Height="50px" Width="80px"></asp:Label>
                <br />
                <br />
            </td>
            <td valign="middle" align="center" class="auto-style3">
                <br />
                <br />
                <asp:Label ID="LabelKYCDeclined" runat="server" Text="-" Font-Bold="True" Font-Overline="False" Font-Size="XX-Large" Height="50px" Width="80px"></asp:Label>
                <br />
                <br />
            </td>
            <td valign="middle" align="center" class="auto-style5">
                <br />
                <br />
                <asp:Label ID="LabelPendingOrders" runat="server" Text="-" Font-Bold="True" Font-Overline="False" Font-Size="XX-Large" Height="50px" Width="80px"></asp:Label>
                <br />
                <br />
            </td>
            <td valign="middle" align="center" class="auto-style7">
                <br />
                <br />
                <asp:LinkButton ID="LinkButtonCryptoAddressSum" runat="server" Font-Size="XX-Large" Font-Bold="True" Font-Overline="False" Height="50px" Width="80px">-</asp:LinkButton>
                <br />
                <br />
            </td>
            <td valign="middle" align="center" class="auto-style9">
                <br />
                <br />
                <asp:LinkButton ID="LinkButtonEmailSum" runat="server" Font-Size="XX-Large" Font-Bold="True" Font-Overline="False" Height="50px" Width="80px">-</asp:LinkButton>
                <br />
                <br />
            </td>
            <td valign="middle" align="center">
                <br />

                <asp:LinkButton ID="LinkButtonIpSum" runat="server" Font-Size="XX-Large" Font-Bold="True" Font-Overline="False" Height="50px" Width="80px">-</asp:LinkButton>
                <br />
            </td>
            <td valign="middle" align="center">
                <br />
                <asp:LinkButton ID="LinkButtonCreditCard" runat="server" Font-Size="XX-Large" Font-Bold="True" Font-Overline="False" Height="50px" Width="80px">-</asp:LinkButton>
                <br />
            </td>
        </tr>
    </table>--%>
    <table bgcolor="White" style="border-style: solid; border-width: 1px;">
        <tr>
            <td align="center" class="auto-style2" bgcolor="#72FA83" style="padding: 7px; margin: 5px">Completed orders</td>
            <td align="center" class="auto-style2" bgcolor="#72FA83" style="padding: 7px; margin: 5px">KYC Declined orders</td>
            <td align="center" class="auto-style4" bgcolor="#72FA83" style="padding: 7px; margin: 5px">Quoted orders</td>
            <td align="center" class="auto-style4" bgcolor="#72FA83" style="padding: 7px; margin: 5px">Number of CC</td>
            <td align="center" class="auto-style6" bgcolor="#72FA83" style="padding: 7px; margin: 5px">Crypto used elsewhere</td>
            <td align="center" class="auto-style8" bgcolor="#72FA83" style="padding: 7px; margin: 5px">E-mail used elsewhere </td>
            <td align="center" class="style2" bgcolor="#72FA83" style="padding: 7px; margin: 5px">IP used elsewhere</td>
            <td align="center" class="style2" bgcolor="#72FA83" style="padding: 7px; margin: 5px">Card used elsewhere</td>
        </tr>
        <tr>
            <td valign="middle" align="center" class="auto-style3">
                <br />
                <br />
                <asp:Label ID="LabelCompletedOrders" runat="server" Text="-" Font-Bold="True" Font-Overline="False" Font-Size="XX-Large" Height="50px" Width="80px"></asp:Label>
                <br />
                <br />
            </td>
            <td valign="middle" align="center" class="auto-style3">
                <br />
                <br />
                <asp:Label ID="LabelKYCDeclined" runat="server" Text="-" Font-Bold="True" Font-Overline="False" Font-Size="XX-Large" Height="50px" Width="80px"></asp:Label>
                <br />
                <br />
            </td>
            <td valign="middle" align="center" class="auto-style5">
                <br />
                <br />
                <asp:Label ID="LabelPendingOrders" runat="server" Text="-" Font-Bold="True" Font-Overline="False" Font-Size="XX-Large" Height="50px" Width="80px"></asp:Label>
                <br />
                <br />
            </td>
            <td valign="middle" align="center" class="auto-style5">
                <br />
                <br />
                <asp:Label ID="LabelNumberOfCC" runat="server" Text="-" Font-Bold="True" Font-Overline="False" Font-Size="XX-Large" Height="50px" Width="80px"></asp:Label>
                <br />
                <br />
            </td>
            <td valign="middle" align="center" class="auto-style7">
                <br />
                <br />

                <asp:LinkButton ID="LinkButtonCryptoAddressSum" runat="server" Font-Size="XX-Large" Font-Bold="True" Font-Overline="False" Height="50px" Width="80px">-</asp:LinkButton>
                <br />
                <br />
            </td>
            <td valign="middle" align="center" class="auto-style9">
                <br />
                <br />
                <asp:LinkButton ID="LinkButtonEmailSum" runat="server" Font-Size="XX-Large" Font-Bold="True" Font-Overline="False" Height="50px" Width="80px">-</asp:LinkButton>
                <br />
                <br />
            </td>
            <td valign="middle" align="center">
                <br />
                <asp:LinkButton ID="LinkButtonIpSum" runat="server" Font-Size="XX-Large" Font-Bold="True" Font-Overline="False" Height="50px" Width="80px">-</asp:LinkButton>
                <br />
            </td>
            <td valign="middle" align="center">
                <br />
                <asp:LinkButton ID="LinkButtonCreditCard" runat="server" Font-Size="XX-Large" Font-Bold="True" Font-Overline="False" Height="50px" Width="80px">-</asp:LinkButton>
                <br />
            </td>
        </tr>
    </table>
    <asp:Panel ID="PanelKeyIndicators" runat="server" BorderWidth="3px" Visible="False" BorderColor="#FF3300">
        <br />
        <p class="verdana13">Key Indicator Details</p>
        <p class="verdana13">
            <asp:Button ID="ButtonPanelClose" runat="server" Text="Close" />
        </p>
        <asp:GridView ID="GridViewKeyIndicator" runat="server" EmptyDataText="No Date!">
        </asp:GridView>
    </asp:Panel>
    <br />
    <p class="verdana13">Users orders in total:</p>
    <%--<asp:GridView ID="GridViewSubOrders" runat="server" AutoGenerateColumns="False" DataKeyNames="Id" DataSourceID="SqlDataSourceOrderChild" EmptyDataText="No record" BackColor="White" BorderColor="#CCCCCC" BorderStyle="None" BorderWidth="1px" CellPadding="3" AllowSorting="True">
        <Columns>
            <asp:BoundField DataField="Id" HeaderText="Id" InsertVisible="False" ReadOnly="True" SortExpression="Id" />
            <asp:BoundField DataField="Number" HeaderText="Number" SortExpression="Number" />
            <asp:BoundField DataField="Amount" HeaderText="Amount" SortExpression="Amount" DataFormatString="{0:0.00}" />
            <asp:BoundField DataField="Code" HeaderText="Code" SortExpression="Code" />
            <asp:BoundField DataField="Quoted" HeaderText="Quoted" SortExpression="Quoted" />
            <asp:BoundField DataField="CryptoAddress" HeaderText="CryptoAddress" SortExpression="CryptoAddress" />
            <asp:BoundField DataField="Name" HeaderText="Name" SortExpression="Name" />
            <asp:BoundField DataField="Email" HeaderText="Email" SortExpression="Email" />
            <asp:BoundField DataField="IP" HeaderText="IP" SortExpression="IP" />
            <asp:BoundField DataField="Note" HeaderText="Note" SortExpression="Note" />
            <asp:BoundField DataField="CardNumber" HeaderText="CardNumber" SortExpression="CardNumber" />
            <asp:BoundField DataField="TxSecret" HeaderText="TxSecret" SortExpression="TxSecret" />
            <asp:BoundField DataField="Status" HeaderText="Status" SortExpression="Status" />
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
    </asp:GridView>--%>
    <asp:GridView ID="GridViewSubOrders" runat="server" AutoGenerateColumns="False" DataKeyNames="Id" DataSourceID="SqlDataSourceOrderChild" EmptyDataText="No record" BackColor="White" BorderColor="#CCCCCC" BorderStyle="None" BorderWidth="1px" CellPadding="3" AllowSorting="True">
        <Columns>
            <asp:BoundField DataField="Id" HeaderText="Id" InsertVisible="False" ReadOnly="True" SortExpression="Id" />
            <asp:BoundField DataField="Number" HeaderText="Number" SortExpression="Number" />
            <asp:BoundField DataField="Amount" HeaderText="Amount" SortExpression="Amount" DataFormatString="{0:0.00}" />
            <asp:BoundField DataField="Code" HeaderText="Code" SortExpression="Code" />
            <asp:BoundField DataField="Quoted" HeaderText="Quoted" SortExpression="Quoted" />
            <asp:BoundField DataField="Rate" HeaderText="Rate" SortExpression="Rate" />
            <asp:BoundField DataField="CommissionProcent" HeaderText="Fee" SortExpression="CommissionProcent" />
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
    <asp:SqlDataSource ID="SqlDataSourceOrderChild" runat="server" ConnectionString="<%$ ConnectionStrings:LocalConnectionString %>" SelectCommand="SELECT [Order].Id, [Order].Number, [User].Fname, [Order].Amount, Currency.Code, [Order].Quoted, [Order].CryptoAddress, [Order].Name, [Order].Email, [Order].IP, OrderStatus.Text AS Status, [Order].Note, [Order].Rate, [Order].CommissionProcent, [Order].CardNumber, [Order].SiteId, SUBSTRING([Order].CountryCode, 1, 5) AS BrowserCulture, [Order].TxSecret FROM Currency INNER JOIN [Order] ON Currency.Id = [Order].CurrencyId INNER JOIN OrderStatus ON [Order].Status = OrderStatus.Id INNER JOIN [User] ON [Order].UserId = [User].Id INNER JOIN [Order] AS Order_1 ON [User].Id = Order_1.UserId WHERE (Order_1.Id = @Id) ORDER BY [Order].Quoted DESC">
        <SelectParameters>
            <asp:ControlParameter ControlID="GridViewOrder" Name="Id" PropertyName="SelectedValue" />
        </SelectParameters>
    </asp:SqlDataSource>
    <p class="verdana13">
        Transaction:
    </p>
    <p>
        <asp:GridView ID="GridViewTransaction" runat="server" AutoGenerateColumns="False" DataSourceID="SqlDataSourceTransaction" EmptyDataText="No records" BackColor="#DEBA84" BorderColor="#DEBA84" BorderStyle="None" BorderWidth="1px" CellPadding="3" CellSpacing="2">
            <Columns>
                <asp:BoundField DataField="ExtRef" HeaderText="ExtRef" SortExpression="ExtRef" />
                <asp:BoundField DataField="Amount" HeaderText="Amount" SortExpression="Amount" DataFormatString="{0:0.00}" />
                <asp:BoundField DataField="Code" HeaderText="Currency" SortExpression="Code" />
                <asp:TemplateField HeaderText="Info" SortExpression="Info">
                    <EditItemTemplate>
                        <asp:TextBox ID="TextBox2" runat="server" Text='<%# Bind("Info") %>'></asp:TextBox>
                    </EditItemTemplate>
                    <ItemTemplate>
                        <asp:Label ID="LabelInfo" runat="server" Text='<%# Bind("Info") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="BINList lookup" SortExpression="Info">
                    <EditItemTemplate>
                        <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("Info") %>'></asp:TextBox>
                    </EditItemTemplate>
                    <ItemTemplate>
                        <asp:Label ID="LabelBINInfo" runat="server" Text='<%# Bind("Info") %>'></asp:Label>
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
        KYC Files:
    </p>
    <p>
        <asp:GridView ID="GridViewKyc" runat="server" AutoGenerateColumns="False" DataKeyNames="Id" DataSourceID="SqlDataSourceKycFile" EmptyDataText="No records" BackColor="#DEBA84" BorderColor="#DEBA84" BorderStyle="None" BorderWidth="1px" CellPadding="3" CellSpacing="2">
            <Columns>
                <asp:BoundField DataField="Id" HeaderText="Id" InsertVisible="False" ReadOnly="True" SortExpression="Id" />
                <asp:BoundField DataField="Type" HeaderText="Type" SortExpression="Type" />
                <asp:BoundField DataField="Note" HeaderText="Note" SortExpression="Note" />
                <asp:BoundField DataField="UniqueFilename" HeaderText="UniqueFilename" SortExpression="UniqueFilename" />
                <asp:BoundField DataField="OriginalFilename" HeaderText="OriginalFilename" SortExpression="OriginalFilename" />
                <asp:BoundField DataField="Uploaded" HeaderText="Uploaded" SortExpression="Uploaded" />
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
                        <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("Message") %>' ReadOnly="True" TextMode="MultiLine" Width="450px"></asp:TextBox>
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
    <asp:SqlDataSource ID="SqlDataSourceOrder" runat="server" ConnectionString="<%$ ConnectionStrings:LocalConnectionString %>" SelectCommand="SELECT [Order].Id, [Order].Number, [User].Fname, [Order].Amount, Currency.Code, [Order].Quoted, [Order].CryptoAddress, [Order].Name, [Order].Email, [Order].IP, [Order].Rate, [Order].Note, [Order].Status, [Order].SiteId, OrderStatus.Text AS OrderStatus, [Order].TransactionHash, [Order].ExtRef, [User].Phone FROM Currency INNER JOIN [Order] ON Currency.Id = [Order].CurrencyId INNER JOIN OrderStatus ON [Order].Status = OrderStatus.Id INNER JOIN [User] ON [Order].UserId = [User].Id ORDER BY [Order].Quoted DESC" UpdateCommand="UPDATE [Order] SET Note = @Note WHERE (Id = @Id)">
        <UpdateParameters>
            <asp:Parameter Name="Note" />
            <asp:Parameter Name="Id" />
        </UpdateParameters>
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="SqlDataSourceOrderStatus" runat="server" ConnectionString="<%$ ConnectionStrings:LocalConnectionString %>" SelectCommand="SELECT Id, Text FROM OrderStatus WHERE (Id IN (SELECT DISTINCT Status FROM [Order])) ORDER BY Id"></asp:SqlDataSource>
    <%--<asp:SqlDataSource ID="SqlDataSourceOrderChild" runat="server" ConnectionString="<%$ ConnectionStrings:LocalConnectionString %>" SelectCommand="SELECT [Order].Id, [Order].Number, [User].Fname, [Order].Amount, Currency.Code, [Order].Quoted, [Order].CryptoAddress, [Order].Name, [Order].Email, [Order].IP, OrderStatus.Text AS Status, [Order].Note, [Order].CardNumber, [Order].TxSecret, [Order].CountryCode FROM Currency INNER JOIN [Order] ON Currency.Id = [Order].CurrencyId INNER JOIN OrderStatus ON [Order].Status = OrderStatus.Id INNER JOIN [User] ON [Order].UserId = [User].Id INNER JOIN [Order] AS Order_1 ON [User].Id = Order_1.UserId WHERE (Order_1.Id = @Id) ORDER BY [Order].Quoted DESC">
        <SelectParameters>
            <asp:ControlParameter ControlID="GridViewOrder" Name="Id" PropertyName="SelectedValue" />
        </SelectParameters>
    </asp:SqlDataSource>--%>
    <asp:SqlDataSource ID="SqlDataSourceTransaction" runat="server" ConnectionString="<%$ ConnectionStrings:LocalConnectionString %>" SelectCommand="SELECT [Transaction].ExtRef, [Transaction].Amount, [Transaction].Currency, [Transaction].Info, Currency.Code FROM [Transaction] INNER JOIN Currency ON [Transaction].Currency = Currency.Id WHERE ([Transaction].OrderId = @OrderId)">
        <SelectParameters>
            <asp:ControlParameter ControlID="GridViewOrder" Name="OrderId" PropertyName="SelectedValue" />
        </SelectParameters>
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="SqlDataSourceAuditTrail" runat="server" ConnectionString="<%$ ConnectionStrings:LocalConnectionString %>" SelectCommand="SELECT AuditTrail.OrderId, AuditTrail.Status, AuditTrail.Message, AuditTrail.Created, AuditTrailStatus.Text FROM AuditTrail INNER JOIN AuditTrailStatus ON AuditTrail.Status = AuditTrailStatus.Id WHERE (AuditTrail.OrderId = @OrderId)">
        <SelectParameters>
            <asp:ControlParameter ControlID="GridViewOrder" Name="OrderId" PropertyName="SelectedValue" />
        </SelectParameters>
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="SqlDataSourceUser" runat="server" ConnectionString="<%$ ConnectionStrings:LocalConnectionString %>" SelectCommand="SELECT [User].Id, [User].Fname, [User].Phone, [User].Email, Country.Text AS CountryCode, [User].Commission, [User].KycNote, [User].Created, [User].PaymentMethodDetails FROM [User] INNER JOIN [Order] ON [User].Id = [Order].UserId LEFT OUTER JOIN Country ON [User].CountryId = Country.Id WHERE ([Order].Id = @Id)" UpdateCommand="UPDATE [User] SET PaymentMethodDetails = @PaymentMethodDetails WHERE (Id = @Id)">
        <SelectParameters>
            <asp:ControlParameter ControlID="GridViewOrder" Name="Id" PropertyName="SelectedValue" />
        </SelectParameters>
        <UpdateParameters>
            <asp:Parameter Name="PaymentMethodDetails" />
            <asp:Parameter Name="Id" />
        </UpdateParameters>
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="SqlDataSourceKycFile" runat="server" ConnectionString="<%$ ConnectionStrings:LocalConnectionString %>" SelectCommand="SELECT [Order].Id, KycFile.Type, KycFile.Note, KycFile.UniqueFilename, KycFile.OriginalFilename, KycFile.Uploaded FROM [User] INNER JOIN [Order] ON [User].Id = [Order].UserId INNER JOIN KycFile ON [User].Id = KycFile.UserId WHERE ([Order].Id = @Id)">
        <SelectParameters>
            <asp:ControlParameter ControlID="GridViewOrder" Name="Id" PropertyName="SelectedValue" />
        </SelectParameters>
    </asp:SqlDataSource>
    <br />
</asp:Content>

