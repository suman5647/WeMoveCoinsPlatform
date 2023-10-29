<%@ Page Title="" Language="VB" MasterPageFile="~/MasterPageAdmin.master" AutoEventWireup="false" ValidateRequest="false" Inherits="WMC.Admin.Web.ApprovePayout" Codebehind="ApprovePayout.aspx.vb" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <style type="text/css">
        .auto-style3 {
            height: 16px;
        }
        .auto-style4 {
            width: 100px;
            height: 16px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:LinkButton ID="LinkButtonBACK" runat="server" PostBackUrl="~/Default.aspx">Back</asp:LinkButton>

    <br />
    <br />
    <div class="container body-content">
        <div class="row">
            <div class="col-md-4">
                <asp:TextBox ID="txtOTP" runat="server" CssClass="otptxt" Font-Size="XX-Large" Width="256px"></asp:TextBox>&nbsp;&nbsp;
                        <br />
                <asp:Button ID="BtnUnlock" runat="server" Text="Unlock" Font-Size="XX-Large" />&nbsp;
                <asp:DropDownList ID="DropDownListUnlockPeriode" runat="server" Font-Size="XX-Large">
                    <asp:ListItem Value="300">5min</asp:ListItem>
                    <asp:ListItem Value="600">10min</asp:ListItem>
                    <asp:ListItem Value="1200">20min</asp:ListItem>
                    <asp:ListItem Value="2400">40min</asp:ListItem>
                    <asp:ListItem Value="3600">1hr</asp:ListItem>
                </asp:DropDownList>
                &nbsp;<asp:Label ID="Label1" runat="server" Font-Size="XX-Large"></asp:Label>
            </div>
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
                    <asp:TextBox ID="TextBoxNote" runat="server" TextMode="MultiLine" Width="300px"></asp:TextBox>&nbsp;
                    <asp:Button ID="ButtonAddNote" runat="server" Text="Add Note" />
                </asp:Panel>
            </td>

        </tr>
        <tr>
            <td colspan="3" style="text-align: right">
                <p>
                    <asp:GridView ID="GridViewOrder" runat="server" AllowPaging="True" AllowSorting="True" AutoGenerateColumns="False" DataKeyNames="Id" DataSourceID="SqlDataSourceOrder" BackColor="White" BorderColor="#336666" BorderStyle="Double" BorderWidth="3px" CellPadding="4" EmptyDataText="No orders awaits approval" GridLines="Horizontal" Width="100%">
                        <Columns>
                            <asp:CommandField ShowSelectButton="True" SelectText="Review" />
                            <asp:BoundField DataField="Quoted" DataFormatString="{0:G}" HeaderText="Quoted" SortExpression="Quoted" />
                            <asp:BoundField DataField="Id" HeaderText="Id" ReadOnly="True" SortExpression="Id" Visible="False" />
                            <asp:BoundField DataField="Number" HeaderText="Number" SortExpression="Number" ReadOnly="True" />
                            <asp:BoundField DataField="Fname" HeaderText="Name" SortExpression="Fname" ReadOnly="True" Visible="False" />
                            <asp:TemplateField HeaderText="Amount" SortExpression="Amount">
                                <EditItemTemplate>
                                    <asp:Label ID="LabelAmount" runat="server" Text='<%# Eval("Amount", "{0:0,0}") %>'></asp:Label>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="LabelAmount" runat="server" Text='<%# Bind("Amount", "{0:0,0}")%>'></asp:Label>
                                    <asp:Label ID="LabelAmount22" runat="server" Text='<%# Bind("Code") %>'></asp:Label>
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
                            <asp:BoundField DataField="AmountEUR" HeaderText="EUR" DataFormatString="{0:N0}" SortExpression="AmountEUR" Visible="False">
                                <ItemStyle Font-Bold="True" />
                            </asp:BoundField>
                            <asp:TemplateField HeaderText="Fee" SortExpression="Fee">
                                <EditItemTemplate>
                                    <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("Fee") %>'></asp:TextBox>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="Label4" runat="server" Text='<%# Bind("Fee") %>'></asp:Label>
                                    %
                                </ItemTemplate>
                                <ItemStyle Width="50px" />
                            </asp:TemplateField>
                            <asp:BoundField DataField="CryptoAddress" HeaderText="CryptoAddress" SortExpression="CryptoAddress" ReadOnly="True" Visible="False" />
                            <asp:BoundField DataField="Quoted" HeaderText="Quoted" SortExpression="Quoted" ReadOnly="True" Visible="False" />
                            <asp:BoundField DataField="Name" HeaderText="Name" SortExpression="Name" ReadOnly="True" />
                            <asp:BoundField DataField="Email" HeaderText="Email" SortExpression="Email" ReadOnly="True" />
                            <asp:BoundField DataField="IP" HeaderText="IP" SortExpression="IP" ReadOnly="True" Visible="False" />
                            <asp:TemplateField HeaderText="Approve">
                                <ItemTemplate>
                                    <asp:Button ID="ButtonApprove" runat="server" BackColor="#66FF66" BorderStyle="Solid" CommandName="Approve" Text="Approve" />
                                </ItemTemplate>
                                <HeaderStyle HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Move To:">
                                <ItemTemplate>
                                    <asp:Button ID="ButtonReviewKYC" runat="server" BackColor="#FFFF66" BorderStyle="Solid" CommandName="ReviewKYC" Text="Compliance" ForeColor="Black" />
                                </ItemTemplate>
                                <HeaderStyle HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </asp:TemplateField>
                             <asp:TemplateField HeaderText="Card">
                                <ItemTemplate>
                                    <asp:Button ID="ButtonApproveCard" runat="server" BackColor="#66FF66" BorderStyle="Solid" CommandName="ApproveCard" Text="Tx OK" />
                                </ItemTemplate>
                                <HeaderStyle HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="KYC Declined">
                                <ItemTemplate>
                                    <asp:Button ID="ButtonKYCDeclined" runat="server" BackColor="#FFA8C5" BorderStyle="Solid" CommandName="KYCDeclined" Text="Decline" ForeColor="Black" />
                                </ItemTemplate>
                                <HeaderStyle HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Cancel">
                                <ItemTemplate>
                                    <asp:Button ID="ButtonCancelOrder" runat="server" BackColor="#FFA8C5" BorderStyle="Solid" CommandName="CancelOrder" Text="Cancel" ForeColor="Black" />
                                </ItemTemplate>
                                <HeaderStyle HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
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
                        <FooterStyle BackColor="White" ForeColor="#333333" />
                        <HeaderStyle BackColor="#336666" Font-Bold="True" ForeColor="White" />
                        <PagerStyle BackColor="#336666" ForeColor="White" HorizontalAlign="Center" />
                        <RowStyle BackColor="White" ForeColor="#333333" />
                        <SelectedRowStyle BackColor="#E6E6E6" Font-Bold="True" ForeColor="#333333" />
                        <SortedAscendingCellStyle BackColor="#F7F7F7" />
                        <SortedAscendingHeaderStyle BackColor="#487575" />
                        <SortedDescendingCellStyle BackColor="#E5E5E5" />
                        <SortedDescendingHeaderStyle BackColor="#275353" />
                    </asp:GridView>
                </p>
            </td>
        </tr>
    </table>

   <%-- <p>
        <asp:Button ID="ButtonTrustLevelCalc" runat="server" Text="Trust Level Calculation" Visible="False" />
    </p>--%>
    <asp:Panel ID="PanelTrust2" runat="server" BorderWidth="3px"  BorderColor="#FF3300" Visible="False">
        
        <table bgcolor="White" style="border-style: solid; border-width: 1px; width: 100%;">
            <tr>
                <td align="left"  colspan="7">
                    <asp:Panel ID="Panel2" runat="server">
                        <p class="verdana13">
                            <asp:Label ID="LabelTrustLevelAction" runat="server" Text="Label" Width="393px"></asp:Label>
                            &nbsp;
                            <asp:Button ID="ButtonRunAutoSettle" runat="server" Text="Run Auto Settle" />
                            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Key Indicators Overview</p>
                    </asp:Panel>
                </td>
            </tr>
            <tr>
                <td  bgcolor="#72FA83" style="padding: 7px; margin: 5px; text-align: center;">ORDER</td>
                <td  bgcolor="#72FA83" style="padding: 7px; margin: 5px; text-align: center;">CREDIT CARD</td>
                <td  bgcolor="#72FA83" style="padding: 7px; margin: 5px; text-align: center;">DATA COHERENCE</td>
            </tr>
            <tr>
                <td valign="top" align="center">
                    <table style="width: 340px; text-align: left;">
                        <tr>
                            <td >Completed Orders</td>
                            <td style="width: 100px"><asp:Label ID="LabelCompletedOrders" runat="server" Font-Bold="True" Text="Label" Width="55px"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td class="auto-style3">KYC Declined</td>
                            <td class="auto-style3"><asp:Label ID="LabelKycDeclined" runat="server" Font-Bold="True" Text="Label" Width="55px"></asp:Label></td>
                        </tr>
                        <tr>
                            <td>Order Span (days)</td>
                            <td><asp:Label ID="LabelOrderSpan" runat="server" Font-Bold="True" Text="Label" Width="55px"></asp:Label></td>
                        </tr>
                        <tr>
                            <td>Order Sum(EUR)</td>
                            <td>
                                <asp:Label ID="LabelOrderSumEUR" runat="server" Font-Bold="True" Text="Label" Width="55px"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>Order Sum Total (EUR)</td>
                            <td>
                                <asp:Label ID="LabelOrderSumTotalEUR" runat="server" Font-Bold="True" Text="Label" Width="55px"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>Order Sum 30 Days (EUR)</td>
                            <td>
                                <asp:Label ID="LabeltOrderSum30Days" runat="server" Font-Bold="True" Text="Label" Width="55px"></asp:Label>
                            </td>
                        </tr>
                         <tr>
                            <td>Country Trust Level</td>
                            <td>
                                <asp:Label ID="LabelCountryTrustLevel" runat="server" Font-Bold="True" Text="Label" Width="55px"></asp:Label>
                            </td>
                        </tr>
                    </table>
                </td>
                <td valign="top" align="center">
                    <table style="width: 340px; text-align: left;">
                        <tr>
                            <td class="auto-style3">US-Card</td>
                            <td class="auto-style4">
                                <asp:Label ID="LabelCC_IsUS" runat="server" Font-Bold="True" Text="Label" Width="55px"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>Card is Approved</td>
                            <td><asp:Label ID="LabelCardApproved" runat="server" Font-Bold="True" Text="Label" Width="55px"></asp:Label></td>
                        </tr>
                        <tr>
                            <td class="auto-style3">Cards are Rejected</td>
                            <td class="auto-style3"><asp:Label ID="LabelCardRejected" runat="server" Font-Bold="True" Text="Label" Width="55px"></asp:Label></td>
                        </tr>
                        <tr>
                            <td>Number of Cards</td>
                            <td><asp:Label ID="LabelNumberOfCC" runat="server" Font-Bold="True" Text="Label" Width="55px"></asp:Label></td>
                        </tr>
                        <tr>
                            <td>Card used elsewhere</td>
                            <td><asp:LinkButton ID="LinkButtonCreditCardUsedElsewhere" runat="server" Font-Bold="True" Width="55px">-</asp:LinkButton></td>
                        </tr>
                    </table>
                </td>
                <td valign="top" align="center">
                    <table style="width: 340px; text-align: left;">
                        <tr>
                            <td>Email used elsewhere</td>
                            <td><asp:LinkButton ID="LinkButtonEmailUsedElsewhere" runat="server" Font-Bold="True" Width="55px">-</asp:LinkButton></td>
                        </tr>
                        <tr>
                            <td>IP used elsewhere</td>
                            <td><asp:LinkButton ID="LinkButtonIpUsedElsewhere" runat="server" Font-Bold="True" Width="55px">-</asp:LinkButton></td>
                        </tr>
                        <tr>
                            <td>Phone-CC</td>
                            <td><asp:Label ID="LabelPhoneCC" runat="server" Font-Bold="True" Text="Label"></asp:Label></td>
                        </tr>
                         <tr>
                            <td>Phone-IP</td>
                            <td><asp:Label ID="LabelPhoneIP" runat="server" Font-Bold="True" Text="Label"></asp:Label></td>
                        </tr>
                         <tr>
                            <td>Phone-Culture</td>
                            <td><asp:Label ID="LabelPhoneCulture" runat="server" Font-Bold="True" Text="Label"></asp:Label></td>
                        </tr> 
                        <%--<tr>
                            <td class="auto-style3">CC-IP</td>
                            <td class="auto-style3"><asp:Label ID="LabelCcIP" runat="server" Font-Bold="True" Text="Label"></asp:Label></td>
                        </tr>   --%>                     
                        <tr>
                            <td>Number of used Names</td>
                            <td><asp:Label ID="LabelDistinctNames" runat="server" Font-Bold="True" Text="Label" Width="55px"></asp:Label></td>
                        </tr>
                         <tr>
                            <td>Number of used Emails</td>
                            <td><asp:Label ID="LabelDistinctEmails" runat="server" Font-Bold="True" Text="Label" Width="55px"></asp:Label></td>
                        </tr>
                    </table>
                </td>

            </tr>
        </table>
    </asp:Panel>
    <%--<asp:Panel ID="PanelTrustLevel" runat="server" BorderWidth="3px" Visible="False" BorderColor="#FF3300">
        <br />
        <p class="verdana13">Trust Level Calculation</p>
        <table bgcolor="White" style="border-style: solid; border-width: 1px;">
            <tr>
                <td align="center" class="auto-style2" colspan="7">
                    <asp:Panel ID="PanelTotalTrustScore" runat="server">
                        <asp:Label ID="LabelTotalScore" runat="server" Text="-" Font-Bold="True" Font-Overline="False" Font-Size="XX-Large" Height="50px" Width="80px"></asp:Label>
                    </asp:Panel>
                </td>
            </tr>
            <tr>
                <td align="center" class="auto-style2" bgcolor="#72FA83" style="padding: 7px; margin: 5px">CreditCard Origin</td>
                <td align="center" class="auto-style1" bgcolor="#72FA83" style="padding: 7px; margin: 5px">Phone Origin</td>
                <td align="center" class="auto-style4" bgcolor="#72FA83" style="padding: 7px; margin: 5px">Order Span</td>
                <td align="center" class="auto-style6" bgcolor="#72FA83" style="padding: 7px; margin: 5px">Country Coherence</td>
                <td align="center" class="auto-style8" bgcolor="#72FA83" style="padding: 7px; margin: 5px">Data Consistency </td>
                <td align="center" class="style2" bgcolor="#72FA83" style="padding: 7px; margin: 5px">KYC Approved</td>
                <td align="center" class="style2" bgcolor="#72FA83" style="padding: 7px; margin: 5px">Order Size</td>
            </tr>
            <tr>
                <td valign="middle" align="center" class="auto-style3">
                    <br />
                    <br />
                    <asp:Label ID="LabelCreditCardScore" runat="server" Text="-" Font-Bold="True" Font-Overline="False" Font-Size="XX-Large" Height="50px" Width="80px"></asp:Label>
                    <br />
                    <br />
                </td>
                <td valign="middle" align="center" class="auto-style1">
                    <br />
                    <br />
                    <asp:Label ID="LabelPhoneScore" runat="server" Text="-" Font-Bold="True" Font-Overline="False" Font-Size="XX-Large" Height="50px" Width="80px"></asp:Label>
                    <br />
                    <br />
                </td>
                <td valign="middle" align="center" class="auto-style5">
                    <br />
                    <br />
                    <asp:Label ID="LabelOrderSpanScore" runat="server" Text="-" Font-Bold="True" Font-Overline="False" Font-Size="XX-Large" Height="50px" Width="80px"></asp:Label>
                    <br />
                    <br />
                </td>
                <td valign="middle" align="center" class="auto-style7">
                    <br />
                    <br />
                    <asp:Label ID="LabelCountryCoherenceScore" runat="server" Font-Bold="True" Font-Overline="False" Font-Size="XX-Large" Height="50px" Text="-" Width="80px"></asp:Label>
                    <br />
                    <br />
                </td>
                <td valign="middle" align="center" class="auto-style9">
                    <br />
                    <br />
                    <asp:Label ID="LabelDataConsistencyScore" runat="server" Font-Bold="True" Font-Overline="False" Font-Size="XX-Large" Height="50px" Text="-" Width="80px"></asp:Label>
                    <br />
                    <br />
                </td>
                <td valign="middle" align="center">
                    <br />
                    <asp:Label ID="LabelKycApprovedScore" runat="server" Font-Bold="True" Font-Overline="False" Font-Size="XX-Large" Height="50px" Text="-" Width="80px"></asp:Label>
                    <br />
                </td>
                <td valign="middle" align="center">
                    <br />
                    <asp:Label ID="LabelAmountScore" runat="server" Font-Bold="True" Font-Overline="False" Font-Size="XX-Large" Height="50px" Text="-" Width="80px"></asp:Label>
                    <br />
                </td>
            </tr>
        </table>
        <asp:Label ID="Label4" runat="server" BackColor="SpringGreen" Text="34 - 100" Font-Size="Large" Height="33px"></asp:Label>
        <asp:Label ID="Label3" runat="server" BackColor="Yellow" Text="20 - 34" Font-Size="Large" Height="33px"></asp:Label>
        <asp:Label ID="Label2" runat="server" BackColor="Red" Text="0 - 20" Font-Size="Large" Height="33px"></asp:Label>
        <p class="verdana13">
            <asp:Button ID="ButtonCloseTrustLevelPanel" runat="server" Text="Close" />
        </p>
    </asp:Panel>--%>
    <asp:Panel ID="PanelKeyIndicators" runat="server" BorderWidth="3px" Visible="False" BorderColor="#FF3300">
        <p class="verdana13">Data Used Elsewhere&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;  <asp:Button ID="ButtonPanelClose" runat="server" Text="Close" /></p>
       
        <asp:GridView ID="GridViewKeyIndicator" runat="server" EmptyDataText="No Date!" Width="100%">
        </asp:GridView>
    </asp:Panel>

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
    <p>
        <asp:DetailsView ID="DetailsViewNoteEdit" runat="server" AutoGenerateRows="False" BackColor="#DEBA84" BorderColor="#DEBA84" BorderStyle="None" BorderWidth="1px" CellPadding="3" CellSpacing="2" DataKeyNames="Id" DataSourceID="SqlDataSourceOrderChildNote" EmptyDataText="No records" Height="50px" Width="700px" Visible="False">
            <EditRowStyle BackColor="#738A9C" Font-Bold="True" ForeColor="White" />
            <Fields>
                <asp:BoundField DataField="Id" HeaderText="Id" InsertVisible="False" ReadOnly="True" SortExpression="Id" Visible="False" />
                <asp:TemplateField HeaderText="Note:" SortExpression="Note">
                    <EditItemTemplate>
                        <asp:TextBox ID="TextBox1" runat="server" Height="109px" Text='<%# Bind("Note") %>' TextMode="MultiLine" Width="382px"></asp:TextBox>
                    </EditItemTemplate>
                    <InsertItemTemplate>
                        <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("Note") %>'></asp:TextBox>
                    </InsertItemTemplate>
                    <ItemTemplate>
                        <asp:Label ID="Label1" runat="server" Text='<%# Bind("Note") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:CommandField ShowEditButton="True" />
            </Fields>
            <FooterStyle BackColor="#F7DFB5" ForeColor="#8C4510" />
            <HeaderStyle BackColor="#A55129" Font-Bold="True" ForeColor="White" />
            <PagerStyle ForeColor="#8C4510" HorizontalAlign="Center" />
            <RowStyle BackColor="#FFF7E7" ForeColor="#8C4510" />
        </asp:DetailsView>
    </p>
    <p class="verdana13">Orders in total:</p>
    <asp:GridView ID="GridViewSubOrders" runat="server" AutoGenerateColumns="False" DataKeyNames="Id" DataSourceID="SqlDataSourceOrderChild" EmptyDataText="No record" BackColor="White" BorderColor="#CCCCCC" BorderStyle="None" BorderWidth="1px" CellPadding="3" AllowSorting="True">
        <Columns>
            <asp:BoundField DataField="Id" HeaderText="Order Id" InsertVisible="False" ReadOnly="True" SortExpression="Id" />
            <asp:BoundField DataField="Number" HeaderText="Number" SortExpression="Number" />
            <asp:TemplateField HeaderText="Amount" SortExpression="Amount">
                <EditItemTemplate>
                    <asp:Label ID="LabelAmount" runat="server" Text='<%# Eval("Amount", "{0:0,0}") %>'></asp:Label>
                </EditItemTemplate>
                <ItemTemplate>
                    <asp:Label ID="LabelAmount" runat="server" Text='<%# Bind("Amount", "{0:0,0}") %>'></asp:Label>
                    <asp:Label ID="LabelAmount22" runat="server" Text='<%# Bind("Code") %>'></asp:Label>
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
            <asp:BoundField DataField="Quoted" HeaderText="Quoted" SortExpression="Quoted" />
            <asp:BoundField DataField="Rate" HeaderText="Rate" SortExpression="Rate" DataFormatString="{0:0.00}" />
            <asp:BoundField DataField="CommissionProcent" HeaderText="Fee" SortExpression="CommissionProcent" />
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
        Transaction on Order:
    </p>
    <p>
        <asp:GridView ID="GridViewTransaction" runat="server" AutoGenerateColumns="False" DataSourceID="SqlDataSourceTransaction" EmptyDataText="No records" BackColor="#DEBA84" BorderColor="#DEBA84" BorderStyle="None" BorderWidth="1px" CellPadding="3" CellSpacing="2">
            <Columns>
                <asp:BoundField DataField="Id" HeaderText="Id" InsertVisible="False" ReadOnly="True" SortExpression="Id" />
                <asp:BoundField DataField="ExtRef" HeaderText="ExtRef" SortExpression="ExtRef" />
                <asp:BoundField DataField="Amount" HeaderText="Amount" SortExpression="Amount" DataFormatString="{0:0,0.00}" />
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
                <asp:BoundField DataField="Text" HeaderText="Type" SortExpression="Text" />
                <asp:BoundField DataField="Note" HeaderText="Note" SortExpression="Note" />
                <asp:BoundField DataField="OriginalFilename" HeaderText="OriginalFilename" SortExpression="OriginalFilename" />
                <asp:BoundField DataField="Uploaded" HeaderText="Uploaded" SortExpression="Uploaded" />
                <asp:BoundField DataField="Approved" HeaderText="Approved" SortExpression="Approved" />
                <asp:BoundField DataField="Rejected" HeaderText="Rejected" SortExpression="Rejected" />
                <asp:BoundField DataField="Obsolete" HeaderText="Obsolete" SortExpression="Obsolete" />
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
    <asp:SqlDataSource ID="SqlDataSourceOrder" runat="server" ConnectionString="<%$ ConnectionStrings:LocalConnectionString %>" SelectCommand="SELECT [Order].Id, [Order].Number, [User].Fname, [Order].Amount, Currency.Code, [Order].Quoted, [Order].CryptoAddress, [Order].Name, [Order].Email, [Order].IP, [Order].Note, [Order].SiteId, [Order].CommissionProcent AS Fee, [Order].RateBase, [Order].RateHome, [Order].RateBooks, [Order].Amount / [Order].RateHome AS AmountEUR FROM Currency INNER JOIN [Order] ON Currency.Id = [Order].CurrencyId INNER JOIN OrderStatus ON [Order].Status = OrderStatus.Id INNER JOIN [User] ON [Order].UserId = [User].Id WHERE (OrderStatus.Text = N'Payout awaits approval') ORDER BY [Order].Quoted DESC" UpdateCommand="UPDATE [Order] SET Note = @Note WHERE (Id = @Id)">
        <UpdateParameters>
            <asp:Parameter Name="Note" />
            <asp:Parameter Name="Id" />
        </UpdateParameters>
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="SqlDataSourceOrderChildNote" runat="server" ConnectionString="<%$ ConnectionStrings:LocalConnectionString %>" SelectCommand="SELECT Id, Note FROM [Order] WHERE (Id = @Id)" UpdateCommand="UPDATE [Order] SET Note = @Note WHERE (Id = @Id)">
        <SelectParameters>
            <asp:ControlParameter ControlID="GridViewOrder" Name="Id" PropertyName="SelectedValue" />
        </SelectParameters>
        <UpdateParameters>
            <asp:Parameter Name="Note" />
            <asp:Parameter Name="Id" />
        </UpdateParameters>
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="SqlDataSourceTransaction" runat="server" ConnectionString="<%$ ConnectionStrings:LocalConnectionString %>" SelectCommand="SELECT [Transaction].Id, [Transaction].ExtRef, [Transaction].Amount, [Transaction].Currency, [Transaction].Info, Currency.Code FROM [Transaction] INNER JOIN Currency ON [Transaction].Currency = Currency.Id WHERE ([Transaction].OrderId = @OrderId)">
        <SelectParameters>
            <asp:ControlParameter ControlID="GridViewOrder" Name="OrderId" PropertyName="SelectedValue" />
        </SelectParameters>
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="SqlDataSourceAuditTrail" runat="server" ConnectionString="<%$ ConnectionStrings:LocalConnectionString %>" SelectCommand="SELECT AuditTrail.OrderId, AuditTrail.Status, AuditTrail.Message, AuditTrail.Created, AuditTrailStatus.Text FROM AuditTrail INNER JOIN AuditTrailStatus ON AuditTrail.Status = AuditTrailStatus.Id WHERE (AuditTrail.OrderId = @OrderId)">
        <SelectParameters>
            <asp:ControlParameter ControlID="GridViewOrder" Name="OrderId" PropertyName="SelectedValue" />
        </SelectParameters>
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="SqlDataSourceUserzz" runat="server" ConnectionString="<%$ ConnectionStrings:LocalConnectionString %>" SelectCommand="SELECT [User].Id, [User].Fname, [User].Phone, [User].Email, Country.Text AS CountryCode, [User].Commission, [User].KycNote, [User].Created FROM [User] INNER JOIN [Order] ON [User].Id = [Order].UserId LEFT OUTER JOIN Country ON [User].CountryId = Country.Id WHERE ([Order].Id = @Id)">
        <SelectParameters>
            <asp:ControlParameter ControlID="GridViewOrder" Name="Id" PropertyName="SelectedValue" />
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

