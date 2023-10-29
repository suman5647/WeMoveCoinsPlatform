<%@ Page Language="VB" AutoEventWireup="false" CodeFile="MobilePage.aspx.vb" Inherits="MobilePage" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">
        TABLE {
            FONT-WEIGHT: normal;
            FONT-SIZE: 14px;
            COLOR: #333333;
            FONT-FAMILY: verdana;
        }

        TR {
            FONT-WEIGHT: normal;
            FONT-SIZE: 14px;
            COLOR: #333333;
            FONT-FAMILY: verdana;
        }

        TD {
            FONT-WEIGHT: normal;
            FONT-SIZE: 14px;
            COLOR: #333333;
            FONT-FAMILY: verdana;
            margin-left: 40px;
        }

        .auto-style1 {
            height: 20px;
        }
    .verdana13 {
	FONT-WEIGHT: bold; FONT-SIZE: 13px; COLOR: #333333; FONT-FAMILY: verdana; TEXT-DECORATION: none; caption: Heading
}
        .otptxt {}
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:Panel ID="PanelLogin" runat="server">
                <table style="width:100%;">
                    <tr>
                        <td>
                            <asp:Button ID="ButtonNumber1" runat="server" Font-Size="XX-Large" Text="1" Width="150px" />
                            <br />
                            <br />
                            <br />
                        </td>
                        <td>
                            <asp:Button ID="ButtonNumber2" runat="server" Font-Size="XX-Large" Text="2" Width="150px" />
                            <br />
                            <br />
                            <br />
                        </td>
                        <td>
                            <asp:Button ID="ButtonNumber3" runat="server" Font-Size="XX-Large" Text="3" Width="150px" />
                            <br />
                            <br />
                            <br />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Button ID="ButtonNumber4" runat="server" Font-Size="XX-Large" Text="4" Width="150px" />
                            <br />
                            <br />
                            <br />
                        </td>
                        <td>
                            <asp:Button ID="ButtonNumber5" runat="server" Font-Size="XX-Large" Text="5" Width="150px" />
                            <br />
                            <br />
                            <br />
                        </td>
                        <td>
                            <asp:Button ID="ButtonNumber6" runat="server" Font-Size="XX-Large" Text="6" Width="150px" />
                            <br />
                            <br />
                            <br />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Button ID="ButtonNumber7" runat="server" Font-Size="XX-Large" Text="7" Width="150px" />
                            <br />
                        </td>
                        <td>
                            <asp:Button ID="ButtonNumber8" runat="server" Font-Size="XX-Large" Text="8" Width="150px" />
                            <br />
                        </td>
                        <td>
                            <asp:Button ID="ButtonNumber9" runat="server" Font-Size="XX-Large" Text="9" Width="150px" />
                            <br />
                        </td>
                    </tr>
                </table>
                <br />
                <br />
                <br />
                <br />
                <asp:Label ID="LabelCode" runat="server" Text="Label"></asp:Label>
                &nbsp;
                <asp:Button ID="ButtonLogin" runat="server" Text="Login" Font-Size="XX-Large" />
                <br />
                <asp:Label ID="LabelWrongPasswordText" runat="server" Text="Wrong password!" Visible="False"></asp:Label>
            </asp:Panel>
            <asp:Panel ID="PanelContent" runat="server" Visible="False">
                I&#39;m&nbsp;<asp:Label ID="LabelRole" runat="server" Text="Label" Font-Bold="True"></asp:Label>&nbsp;and my name is&nbsp;
                <asp:Label ID="LabelMe" runat="server" Text="Label" Font-Bold="True"></asp:Label>
                &nbsp;&nbsp;&nbsp;&nbsp;
        <asp:LinkButton ID="LinkButtonLogout" runat="server">Log out</asp:LinkButton>
                &nbsp;
                <asp:DropDownList ID="DropDownListComplianceAvailability" runat="server" AutoPostBack="True" CssClass="verdana13" Width="100px">
                    <asp:ListItem Value="OnDuty">OnDuty</asp:ListItem>
                    <asp:ListItem Value="Away">Away</asp:ListItem>
                </asp:DropDownList>
                
                <br />
                
               <%-- <asp:TextBox ID="txtOTP" runat="server" CssClass="otptxt" Font-Size="XX-Large" Width="254px"></asp:TextBox>&nbsp;&nbsp;                        
                <asp:Button ID="BtnUnlock" runat="server" Text="Unlock" Font-Size="XX-Large" />&nbsp;
                <asp:DropDownList ID="DropDownListUnlockPeriode" runat="server" Font-Size="Large">
                    <asp:ListItem Value="300">5min</asp:ListItem>
                    <asp:ListItem Value="600">10min</asp:ListItem>
                    <asp:ListItem Value="1200">20min</asp:ListItem>
                    <asp:ListItem Value="2400">40min</asp:ListItem>
                    <asp:ListItem Value="3600" Selected="True">1hr</asp:ListItem>
                </asp:DropDownList>
                &nbsp;<asp:Label ID="Label1" runat="server" Font-Size="Large"></asp:Label>--%>
            
                &nbsp;&nbsp;
                <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="~/Compliance.aspx" Font-Size="XX-Large">Approve...</asp:HyperLink>
                &nbsp;<table style="width: 100%;">
                    <tr>
                        <td>
                            <p>Orders waiting for payout approval:</p>
                        </td>
                        <td>&nbsp;</td>
                        <td style="text-align: right">
                            <asp:Panel ID="PanelNote" runat="server" Visible="False">
                                <asp:TextBox ID="TextBoxNote" runat="server" TextMode="MultiLine" Width="300px"></asp:TextBox>&nbsp;
                                <asp:Button ID="ButtonAddNote" runat="server" Text="Add Note" />
                            </asp:Panel>
                        </td>
                    </tr>
                </table>
                <asp:GridView ID="GridViewOrder" runat="server" AutoGenerateColumns="False" DataSourceID="SqlDataSourceOrder" Width="100%" DataKeyNames="Id">
                    <Columns>
                        <asp:BoundField DataField="Id" HeaderText="Id" ReadOnly="True" SortExpression="Id" Visible="False" />
                        <asp:CommandField SelectText="Review" ShowSelectButton="True" />
                        <asp:BoundField DataField="Number" HeaderText="Number" SortExpression="Number" ReadOnly="True" />
                        <asp:BoundField DataField="Quoted" DataFormatString="{0:G}" HeaderText="Quoted" SortExpression="Quoted" />
                        <asp:BoundField DataField="Fname" HeaderText="Name" SortExpression="Fname" ReadOnly="True" />
                        <asp:BoundField DataField="Phone" HeaderText="Phone" SortExpression="Phone" />
                        <asp:BoundField DataField="Email" HeaderText="Email" SortExpression="Email" />
                        <asp:TemplateField HeaderText="Amount" SortExpression="Amount">
                            <EditItemTemplate>
                                <asp:Label ID="LabelAmount" runat="server" Text='<%# Eval("Amount", "{0:0,0}") %>'></asp:Label>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="LabelAmount" runat="server" Text='<%# Bind("Amount", "{0:0,0}")%>'></asp:Label>
                                <asp:Label ID="LabelAmount22" runat="server" Text='<%# Bind("Code") %>'></asp:Label>
                            </ItemTemplate>
                            <ItemStyle Width="100px" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Fee" SortExpression="Fee">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("Fee") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label4" runat="server" Text='<%# Bind("Fee") %>'></asp:Label>
                                %
                            </ItemTemplate>
                            <ItemStyle Width="80px" />
                        </asp:TemplateField>
                        <asp:BoundField DataField="SiteId" HeaderText="Site" ReadOnly="True" SortExpression="SiteId" />
                        <asp:TemplateField HeaderText="Note" SortExpression="Note">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBoxNote" runat="server" Height="175px" Text='<%# Bind("Note") %>' TextMode="MultiLine" Width="377px"></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Bind("Note") %>' Height="16px"></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <SelectedRowStyle BackColor="#E6E6E6" Font-Bold="True" ForeColor="#333333" />
                </asp:GridView>
                <asp:SqlDataSource ID="SqlDataSourceOrder" runat="server" ConnectionString="<%$ ConnectionStrings:LocalConnectionString %>" SelectCommand="SELECT [Order].Id, [Order].Number, [User].Fname, [Order].Amount, Currency.Code, [Order].Quoted, [Order].CryptoAddress, [Order].Name, [Order].Email, [Order].IP, [Order].Note, [Order].SiteId, [Order].CommissionProcent AS Fee, [Order].RateBase, [Order].RateHome, [Order].RateBooks, [Order].Amount / [Order].RateHome AS AmountEUR, [User].Phone, [User].Email AS Expr1 FROM Currency INNER JOIN [Order] ON Currency.Id = [Order].CurrencyId INNER JOIN OrderStatus ON [Order].Status = OrderStatus.Id INNER JOIN [User] ON [Order].UserId = [User].Id WHERE (OrderStatus.Text = N'Compliance Officer Approval') ORDER BY [Order].Quoted DESC" UpdateCommand="UPDATE [Order] SET Note = @Note WHERE (Id = @Id)">
                    <UpdateParameters>
                        <asp:Parameter Name="Note" />
                        <asp:Parameter Name="Id" />
                    </UpdateParameters>
                </asp:SqlDataSource>
                <br />

                <asp:Panel ID="PanelTrustCalculation" runat="server" Visible="False">
        <table bgcolor="White" style="border-style: solid; border-width: 1px; width: 100%;">
            <tr>
                <td align="left" colspan="3">
                    <asp:Label ID="LabelTrustMessage" runat="server" Text="Label"></asp:Label>&nbsp;&nbsp;&nbsp;&nbsp;                                        
                                    <asp:Button ID="ButtonRunAutoSettle" runat="server" Text="Excecute Now!" Visible="False" />
                </td>
            </tr>
            <tr>
                <td style="padding: 7px; margin: 5px; text-align: center;" class="auto-style1">TRUST LOGIC</td>
                <td style="padding: 7px; margin: 5px; text-align: center;" class="auto-style1">ORDERS</td>
                <td style="padding: 7px; margin: 5px; text-align: center;" class="auto-style1">USER</td>
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
                            <td>IP used Elsewhere</td>
                            <td>
                                <asp:LinkButton ID="LinkButtonIpUsedElsewhere" runat="server" Font-Bold="True" Width="55px">-</asp:LinkButton>
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
                            <td class="auto-style1">Completed</td>
                            <td class="auto-style1">
                                <asp:Label ID="LabelCompleted" runat="server" Font-Bold="True" Text="Label" Width="55px"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>Span</td>
                            <td>
                                <asp:Label ID="LabelOrderSpan" runat="server" Font-Bold="True" Text="Label" Width="55px"></asp:Label></td>
                        </tr>
                         <tr>
                            <td>Card Span</td>
                            <td>
                                <%--<asp:Label ID="LabelCardSpan" runat="server" Font-Bold="True" Text="Label" Width="55px"></asp:Label>&nbsp;<asp:Button ID="ButtonApproveCard" runat="server" BackColor="LightGreen" Height="16px" Text="Auto Approve Card" Visible="False" />--%>

                            </td>
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
                    <table style="border-style: solid; border-width: 1px; width: 340px; text-align: left;">
                        <tr>
                            <td>User is Trusted?</td>
                            <td>
                                <asp:Button ID="ButtonTrustUser" runat="server" Text="Trust Now!" />
                            </td>
                        </tr>
                        <tr>
                            <td>&nbsp;</td>
                            <td>
                                &nbsp;</td>
                        </tr>
                    </table>
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
                            <td >Number of Cards</td>
                            <td >
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
                <%--<asp:Panel ID="PanelTrustCalculation" runat="server" Visible="False">
                    <table bgcolor="White" style="border-style: solid; border-width: 1px; width: 100%;">
                        <tr>
                            <td align="left" colspan="3">
                                <asp:Label ID="LabelTrustMessage" runat="server" Text="Label"></asp:Label>&nbsp;&nbsp;&nbsp;&nbsp;
                                <asp:Button ID="ButtonRunAutoSettle" runat="server" Text="Excecute Now!" />
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
                                <table style="border-style: solid; border-width: 1px; width: 340px; text-align: left;">
                                    <tr>
                                        <td>User is Trusted?</td>
                                        <td>
                                            <asp:Button ID="ButtonTrustUser" runat="server" Text="Trust Now!" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>Card Approved</td>
                                        <td>
                                            <asp:Button ID="ButtonApproveCard" runat="server" Text="Approve Now!" />
                                        </td>
                                    </tr>
                                </table>
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
                </asp:Panel>--%>
            </asp:Panel>
        </div>
    </form>
</body>
</html>
