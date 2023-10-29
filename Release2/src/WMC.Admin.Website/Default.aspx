<%@ Page Title="" Language="VB" MasterPageFile="~/MasterPageAdmin.master" AutoEventWireup="false" CodeFile="Default.aspx.vb" Inherits="_Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/2.1.1/jquery.min.js"></script>
    <style type="text/css">
        .auto-style1 {
            height: 28px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:Panel ID="PanelLogin" runat="server">
        <br />
        <br />
        <br />
        <asp:DropDownList ID="DropDownListUser" runat="server" AppendDataBoundItems="True" DataSourceID="SqlDataSourceAdmin" DataTextField="Fname" DataValueField="Id" Height="27px" Width="216px" CssClass="verdana13">
            <asp:ListItem Value="0">Vælg...</asp:ListItem>
        </asp:DropDownList>
        <br />
        <br />
        <asp:Label ID="Label1" runat="server" Text="Password:"></asp:Label>
        &nbsp;<asp:TextBox ID="TextBoxPassword" runat="server" TextMode="Password"></asp:TextBox>
        <br />
        <br />
        <asp:Button ID="ButtonLogin" runat="server" Text="Login" />
        <br />
        <br />
        <asp:Label ID="LabelWrongPasswordText" runat="server" Text="Wrong password!" Visible="False"></asp:Label>
        <br />
    </asp:Panel>

    <asp:Panel ID="PanelChangePassword" runat="server" Visible="False">
        <br />
        &nbsp;<asp:Label ID="Label3" runat="server" Text="Old Password: "></asp:Label>
        <asp:TextBox ID="TextBoxCurrentPassword" runat="server"></asp:TextBox>
        <br />
        <br />
        &nbsp;<asp:Label ID="Label2" runat="server" Text="New Password: "></asp:Label>
        <asp:TextBox ID="TextBoxNewPassword" runat="server"></asp:TextBox>
        <br />
        <br />
        <asp:Button ID="ButtonChangesPassword" runat="server" Text="Change Password" />
        &nbsp;
        <asp:Button ID="ButtonCancelPasswordChange" runat="server" Text="Cancel" />
        <br />
        <asp:Label ID="LabelPasswordChangeFailed" Visible="false" runat="server" ForeColor="Red" Text="Failed to change password!"></asp:Label>
    </asp:Panel>

    <asp:Panel ID="PanelUpdateStatus" runat="server" Visible="False">
        <br />
        Current Status:&nbsp;<asp:Label ID="LabelCurrentStatus" runat="server" Text="Label"></asp:Label>
        &nbsp;&nbsp;&nbsp;<br />
        <br />
        <asp:DropDownList ID="DropDownStatus" runat="server" AppendDataBoundItems="True" CssClass="verdana13" Height="29px" Width="115px">
            <asp:ListItem Value="Test">Test</asp:ListItem>
            <asp:ListItem>Prod</asp:ListItem>
        </asp:DropDownList>
        &nbsp;
        <asp:Button ID="ButtonUpdateStatus" runat="server" Text="Update Status" />
        &nbsp;
        <asp:Button ID="ButtonCancelYourPayStatus" runat="server" Text="Cancel" />
        <br />
        <br />
         <br />
        <br />
        <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" CellPadding="4" DataKeyNames="Id" DataSourceID="SqlDataSourceYourPay" EnableModelValidation="True" ForeColor="#333333" GridLines="None">
            <AlternatingRowStyle BackColor="White" />
            <Columns>
                <asp:BoundField DataField="Id" HeaderText="Id" InsertVisible="False" ReadOnly="True" SortExpression="Id" Visible="False" />
                <asp:BoundField DataField="ConfigKey" HeaderText="ConfigKey" SortExpression="ConfigKey" />
                <asp:BoundField DataField="ConfigValue" HeaderText="ConfigValue" SortExpression="ConfigValue" />
                <asp:BoundField DataField="ConfigDescription" HeaderText="ConfigDescription" SortExpression="ConfigDescription" Visible="False" />
            </Columns>
            <EditRowStyle BackColor="#2461BF" />
            <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
            <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
            <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
            <RowStyle BackColor="#EFF3FB" />
            <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
        </asp:GridView>
    </asp:Panel>

    <asp:Panel ID="PanelAccess" runat="server" Visible="False">
        I&#39;m
        <asp:Label ID="LabelRole" runat="server" Text="Label" Font-Bold="True"></asp:Label>
        and my name is <%--<asp:Button ID="ButtonCreateOrder" runat="server" Text="CreateOrder" PostBackUrl="~/AdminCreateOrder.aspx" />--%><asp:Label ID="LabelMe" runat="server" Text="Label" Font-Bold="True"></asp:Label>
        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        <asp:LinkButton ID="LinkButtonChangePassword" runat="server">Change Pasword?</asp:LinkButton>
        &nbsp;
        <asp:LinkButton ID="LinkButtonLogout" runat="server">Log out</asp:LinkButton>
        &nbsp;&nbsp;&nbsp;
        <asp:DropDownList ID="DropDownListComplianceAvailability" runat="server" AutoPostBack="True" CssClass="verdana13" Width="100px">
            <asp:ListItem Value="OnDuty">OnDuty</asp:ListItem>
            <asp:ListItem Value="Away">Away</asp:ListItem>
        </asp:DropDownList>
        &nbsp;<asp:Label ID="LabelBitGoUnlocked" runat="server" Text="Label" Visible="False"></asp:Label>
        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        <asp:Button ID="ButtonLockBitGo" runat="server" BackColor="#FFFF66" Text="LOCK!" />
        <br />
        <br />
        <br />       
        <asp:Panel ID="PanelAdminTools" runat="server">
            <asp:Label ID="Label6" runat="server" CssClass="verdana13" Text="State overview:"></asp:Label>
            <br />
            <br />
            <table bgcolor="White" style="border-style: solid; border-width: 1px;">
                <tr>
                    <td align="center" class="auto-style1" bgcolor="#72FA83" style="padding: 7px; margin: 5px">System exceptions</td>
                    <td align="center" class="auto-style1" bgcolor="#72FA83" style="padding: 7px; margin: 5px">QuickPay exceptions (Last 24Hours)</td>
                    <td align="center" class="auto-style1" bgcolor="#72FA83" style="padding: 7px; margin: 5px">Awaits approval</td>
                    <td align="center" class="auto-style1" bgcolor="#72FA83" style="padding: 7px; margin: 5px">Operational Status</td>
                    <td align="center" bgcolor="#72FA83" class="auto-style1" style="padding: 7px; margin: 5px">&nbsp;</td>
                    <td align="center" class="auto-style1" bgcolor="#72FA83" style="padding: 7px; margin: 5px">Revenue</td>
                    <td align="center" class="auto-style1" bgcolor="#72FA83" style="padding: 7px; margin: 5px"># Orders total</td>
                    <td align="center" class="auto-style1" bgcolor="#72FA83" style="padding: 7px; margin: 5px"># Completed</td>
                    <td align="center" class="auto-style1" bgcolor="#72FA83" style="padding: 7px; margin: 5px"># Customers</td>
                </tr>
                <tr>
                    <td valign="middle" align="center" class="auto-style3">
                        <asp:LinkButton ID="LinkButtonExceptions" runat="server" CssClass="verdana13" PostBackUrl="~/SystemException.aspx?days=5" Font-Bold="True">0</asp:LinkButton>
                        <br />
                        
                        <asp:Label ID="LabelPayoutAwaitsApproval11" runat="server" Font-Bold="True" ForeColor="Red" Text="Payout awaits approval" Visible="False"></asp:Label>
                        <asp:Label ID="LabelReleasedPayment16" runat="server" Font-Bold="True" ForeColor="Red" Text="Released payment" Visible="False"></asp:Label>
                        <asp:Label ID="LabelPaymentAborted19" runat="server" Font-Bold="True" ForeColor="Red" Text="Payment Aborted" Visible="False"></asp:Label>
                        <asp:Label ID="LabelSendingAborted12" runat="server" Font-Bold="True" ForeColor="Red" Text="Sending Aborted" Visible="False"></asp:Label>
                        <asp:Label ID="LabelCaptureErrored20" runat="server" Font-Bold="True" ForeColor="Red" Text="Capture Errored" Visible="False"></asp:Label>
                        <asp:Label ID="LabelReleasingPaymentAborted15" runat="server" Font-Bold="True" ForeColor="Red" Text="Releasing Payment Aborted" Visible="False"></asp:Label>
                        <asp:Label ID="LabelPaid3" runat="server" Font-Bold="True" ForeColor="Red" Text="Paid" Visible="False"></asp:Label>
                        <asp:Label ID="LabelDoublePayouts" runat="server" Font-Bold="True" ForeColor="Red" Text="DOUBLEPAYOUT" Visible="False"></asp:Label><asp:Label ID="LabelDoublePayments" runat="server" Font-Bold="True" ForeColor="Red" Text="doublepayments" Visible="False"></asp:Label>
                    </td>
                    <%-- <td valign="middle" align="center" class="auto-style5">
                        <table>
                            <tr>
                                <td>EUR</td>
                                <td>BTC</td>
                            </tr>
                            <tr>
                                <td><span id="acbalanceEUR"></span></td>
                                <td><span id="acbalanceBTC"></span></td>
                            </tr>
                        </table>
                    </td>--%>

                    <td valign="middle" align="center" class="auto-style5">
                          <asp:LinkButton ID="LinkButtonAccepted" runat="server" CssClass="verdana13" PostBackUrl="~/QuickPayException.aspx?status=6&days=1" Font-Bold="True"></asp:LinkButton>
                          <asp:LinkButton ID="LinkButtonRejected" runat="server" CssClass="verdana13" PostBackUrl="~/QuickPayException.aspx?status=7&days=1" Font-Bold="True"></asp:LinkButton>
                          <asp:LinkButton ID="LinkButtonTerminated" runat="server" CssClass="verdana13" PostBackUrl="~/QuickPayException.aspx?status=8&days=1" Font-Bold="True"></asp:LinkButton>
                    </td>

                    <td valign="middle" align="center" class="auto-style5">
                        <asp:LinkButton ID="LinkButtonUserKYC" runat="server" CssClass="verdana13" PostBackUrl="~/KycFile.aspx" Font-Bold="True">0</asp:LinkButton>
                        <br />
                        <asp:LinkButton ID="LinkButtonEDDUser" runat="server" CssClass="verdana13" PostBackUrl="~/KycFile.aspx" Font-Bold="True"></asp:LinkButton>
                    </td>
                    <td valign="middle" align="center" class="auto-style7">
                        <br />
                        <asp:DropDownList ID="DropDownListOperationalStatus" runat="server" AutoPostBack="True" CssClass="verdana13">
                            <asp:ListItem Value="ServiceOpen">ServiceOpen</asp:ListItem>
                            <asp:ListItem Value="ServiceClose">ServiceClose</asp:ListItem>
                            <asp:ListItem Value="OpenSellCloseBuy">OpenSellCloseBuy</asp:ListItem>
                            <asp:ListItem Value="CloseSellOpenBuy">CloseSellOpenBuy</asp:ListItem>
                            <asp:ListItem Value="BankOnly">BankOnly</asp:ListItem>
                        </asp:DropDownList>
                        <br />
                        <asp:DropDownList ID="DropDownListCreditCardGatewayName" runat="server" AutoPostBack="True" CssClass="verdana13" Width="100px">
                            <asp:ListItem>Auto</asp:ListItem>
                            <asp:ListItem Value="PayLike">PayLike</asp:ListItem>
                            <asp:ListItem>YourPay</asp:ListItem>
                            <asp:ListItem>TrustPay</asp:ListItem>
                            <asp:ListItem>QuickPay</asp:ListItem>
                        </asp:DropDownList>
                        <br />
                        <br />
                    </td>
                    <td align="center" valign="middle">
                        <br />
                        BitGO:
                        <asp:Label ID="LabelBitGoBalance" runat="server" CssClass="verdana13" Font-Bold="True" Text="Label"></asp:Label><br />
                        K(EUR): <span id="acbalanceEUR"></span>
                        <br />
                        K(BTC): <span id="acbalanceBTC"></span>
                    </td>
                    <td valign="middle" align="center" class="auto-style3">
                        <asp:Label ID="LabelRevenueThisMonth" runat="server" CssClass="verdana13" Font-Bold="False" Text="Label" Font-Italic="True"></asp:Label><br />
                        <asp:Label ID="LabelRevenueYesterday" runat="server" CssClass="verdana13" Font-Bold="True" Text="Label"></asp:Label><br />
                        <asp:Label ID="LabelRevenueToday" runat="server" CssClass="verdana13" Font-Bold="False" Text="Label"></asp:Label>
                        <br />
                    </td>
                    <td valign="middle" align="center" class="auto-style5">
                        <asp:Label ID="LabelOrderTotal" runat="server" CssClass="verdana13"  Font-Bold="True">0</asp:Label>
                        <br />
                    </td>
                    <td valign="middle" align="center" class="auto-style5">
                        <asp:Label ID="LabelCompleted" runat="server"  CssClass="verdana13"  Font-Bold="True">0</asp:Label>
                        <br />
                    </td>
                    <td valign="middle" align="center" class="auto-style5">
                        <asp:LinkButton ID="LinkButtonCustomers" runat="server" CssClass="verdana13" PostBackUrl="~/User.aspx" Font-Bold="True">0</asp:LinkButton>
                        <br />
                    </td>
                </tr>
            </table>
            <br />
            <br />
            <br />
            <asp:Label ID="Label7" runat="server" CssClass="verdana13" Text="Order handling:"></asp:Label>
            <br />
            <br />
            <table>
                <tr>
                    <td style="padding: 10px; margin: 10px">
                        <asp:Button ID="ButtonEnhancedDueDiligence" runat="server" Text="Enhanced Due Diligence" />
                    </td>
                    <td style="padding: 10px; margin: 10px">
                        <asp:Button ID="ButtonComplianceOfficerApproval" runat="server" Text="Compliance Officer Approval" /></td>
                    <td style="padding: 10px; margin: 10px">
                        <asp:Button ID="ButtonCustomerResponsePending" runat="server" Text="Customer Response Pending" /></td>
                    <td style="padding: 10px; margin: 10px 10px 20px 10px">
                        <asp:Button ID="ButtonKYCApprovalPending" runat="server" Text="KYC Approval Pending" /></td>
                </tr>
                <tr>
                    <td colspan="4" style="padding: 10px; margin: 10px"></td>
                </tr>
                <tr>
                    <td style="padding: 10px; margin: 10px">
                        <asp:Button ID="ButtonApproveBankPayment" runat="server" PostBackUrl="~/ApproveBank.aspx" Text="Approve Bank Payment" Width="190px" Visible="False" /></td>
                    <td>
                        <asp:Button ID="ButtonApproveSell2Bank" runat="server" Text="Approve Sell to Bank" Visible="False" Width="190px" />
                    </td>
                    <td></td>
                    <td></td>
                </tr>
                <tr>
                    <td style="padding: 10px; margin: 10px">
                        <asp:Button ID="ButtonReviewOrders" runat="server" PostBackUrl="~/ReviewOrders.aspx" Text="Review Orders" Width="190px" /></td>
                    <td></td>
                    <td></td>
                    <td></td>
                </tr>
                <tr>
                    <td style="padding: 10px; margin: 10px">
                        <asp:Button ID="ButtonFindOrders" runat="server" PostBackUrl="~/FindOrders.aspx" Text="Find Order/User" Width="190px" /></td>
                    <td></td>
                    <td></td>
                    <td></td>
                </tr>
                <tr>
                    <td colspan="4"></td>
                </tr>
            </table>

            <br />
            <br />
            <br />

            <br />
            <br />

            <br />
            <br />
            <asp:Button ID="ButtonUser" runat="server" PostBackUrl="~/User.aspx" Text="Trusted Users" Width="200px" Visible="False" />
            <br />
            <br />
            <br />
            <%--<asp:Label ID="Label8" runat="server" CssClass="verdana13" Text="KYC handling:"></asp:Label>
            <br />
            <br />
            <asp:Button ID="ButtonPendingKyc" runat="server" PostBackUrl="~/KycFile.aspx" Text="Pending KYC File approval" Width="200px" />
            <br />
            <br />
            <asp:Button ID="ButtonAllKyc" runat="server" PostBackUrl="~/KycFileAll.aspx" Text="Explorer All KYC" Width="200px" />
            <br />
            <br />
            <br />--%>
            <br />
            <asp:Label ID="Label5" runat="server" CssClass="verdana13" Text="Backend stuff:"></asp:Label>
            <br />
            <br />
             Sanctions File Upload:&nbsp;
                <asp:DropDownList ID="DropDownListCSVFile" runat="server" CssClass="verdana13" Width="150px">
                <asp:ListItem Value="1">EU_SanctionsFiles</asp:ListItem>
                <asp:ListItem Value="2">HMT_UK_SanctionsFiles </asp:ListItem>
                <asp:ListItem Value="3">OFAC_SanctionsFiles</asp:ListItem>
                <asp:ListItem Value="4">UN_SanctionsFiles</asp:ListItem>
              </asp:DropDownList>&nbsp;&nbsp;
            <asp:FileUpload ID="FileUpload1" runat="server" />
            <asp:Button ID="btnUpload" runat="server" Text="Upload" />
                   &nbsp;
             <br />
             <asp:Label ID="lblMessage" runat="server" Text="" Font-Names="Arial"></asp:Label>
            <br />
            <br />
            <br />
            Miners Fee:&nbsp;
            <%--<asp:Label ID="LabelMinersFee" runat="server" Text="Label"></asp:Label>--%>
            <asp:DropDownList ID="DropDownListMinersFee" runat="server" AutoPostBack="True" CssClass="verdana13" Width="100px">
                <asp:ListItem>0,0037</asp:ListItem>
                <asp:ListItem>0,0036</asp:ListItem>
                <asp:ListItem>0,0035</asp:ListItem>
                <asp:ListItem>0,0034</asp:ListItem>
                <asp:ListItem>0,0033</asp:ListItem>
                <asp:ListItem>0,0032</asp:ListItem>
                <asp:ListItem>0,0031</asp:ListItem>
                <asp:ListItem>0,0030</asp:ListItem>
                <asp:ListItem>0,0029</asp:ListItem>
                <asp:ListItem>0,0028</asp:ListItem>
                <asp:ListItem>0,0027</asp:ListItem>
                <asp:ListItem>0,0026</asp:ListItem>
                <asp:ListItem>0,0025</asp:ListItem>
                <asp:ListItem>0,0024</asp:ListItem>
                <asp:ListItem>0,0023</asp:ListItem>
                <asp:ListItem>0,0022</asp:ListItem>
                <asp:ListItem>0,0021</asp:ListItem>
                <asp:ListItem>0,0020</asp:ListItem>
                <asp:ListItem>0,0019</asp:ListItem>
                <asp:ListItem>0,0018</asp:ListItem>
                <asp:ListItem>0,0017</asp:ListItem>
                <asp:ListItem>0,0016</asp:ListItem>
                <asp:ListItem>0,0015</asp:ListItem>
                <asp:ListItem>0,0014</asp:ListItem>
                <asp:ListItem>0,0013</asp:ListItem>
                <asp:ListItem>0,0012</asp:ListItem>
                <asp:ListItem>0,0011</asp:ListItem>
                <asp:ListItem>0,0010</asp:ListItem>
                <asp:ListItem>0,0009</asp:ListItem>
                <asp:ListItem>0,0008</asp:ListItem>
                <asp:ListItem>0,0007</asp:ListItem>
                <asp:ListItem>0,0006</asp:ListItem>
                <asp:ListItem>0,0005</asp:ListItem>
                <asp:ListItem>0,0004</asp:ListItem>
                <asp:ListItem>0,0003</asp:ListItem>
                <asp:ListItem>0,0002</asp:ListItem>
                <asp:ListItem>0,0001</asp:ListItem>
                <asp:ListItem>0,00005</asp:ListItem>
            </asp:DropDownList>
            <br />
            <br />


            <asp:Panel ID="PanelBackEndStuff" runat="server" Visible="False">
                <asp:Button ID="ButtonBestCustomer" runat="server" PostBackUrl="~/CustomerByVolumen.aspx" Text="Customer By Volumen" Width="200px" />
                <br />
                <br />
                <asp:Button ID="ButtonCoupons" runat="server" PostBackUrl="~/Coupon.aspx" Text="Coupon Codes" Width="200px" />
                <br />
                <br />
                <asp:Button ID="ButtonAppSettings" runat="server" PostBackUrl="~/Appsettings.aspx" Text="AppSettings" Width="200px" />
                <br />
                <br />
                <asp:Button ID="ButtonTransactionList" runat="server" PostBackUrl="~/TransactionList.aspx" Text="Transaction List" Width="200px" />
                <br />
                <br />
                <asp:Button ID="ButtonYourPayStatus" runat="server" Text="YourPay Status" Width="200px" />
                <br />
                <br />
                <asp:Button ID="ButtonDomainTables" runat="server" PostBackUrl="~/Domain.aspx" Text="Domain tables" Width="200px" />
                <br />
                <br />
                <asp:Button ID="ButtonTransactionSpecial" runat="server" PostBackUrl="~/AddTransaction.aspx" Text="Add transaction record (Find)" Width="200px" />
                <br />
                <br />
                <asp:Button ID="ButtonPayLikeExport" runat="server" PostBackUrl="~/PaylikeReport.aspx" Text="Export Paylike" Width="200px" />
                <br />
                <br />
                <asp:Button ID="ButtonYourPayExport" runat="server" PostBackUrl="~/YourPayReport.aspx" Text="Export YourPay" Width="200px" />
                <br />
                <br />
                <asp:Button ID="ButtonYourPayExport1" runat="server" PostBackUrl="~/YourPayReport1.aspx" Text="Export YourPay1" Width="200px" />
                <br />
                <br />
                <asp:Button ID="ButtonPayLikeExport0" runat="server" PostBackUrl="~/RiskList.aspx" Text="Risk Score List" Width="200px" />
                <br />
                <br />
                <asp:Button ID="ButtonTestEmailAdmin" runat="server" PostBackUrl="~/TestEmailCredentials.aspx" Text="Test Email Credentials" Width="200px" />
                <br />
                <br />
                <asp:Button ID="ButtonSanctionList" runat="server" PostBackUrl="~/SanctionsList.aspx" Text="SanctionListView" Width="200px" />
                <br />
                <br />
                <asp:Button ID="ButtonResendEmail" runat="server" PostBackUrl="~/Resend.aspx" Text="ResendEmail" Width="200px" />
                <br />
                <br />
                <asp:Button ID="ButtonLanguage" runat="server" PostBackUrl="~/LanguageResource.aspx" Text="LanguageResource" Width="200px" />
                <br />
                <br />
            </asp:Panel>
        </asp:Panel>
        <asp:Panel ID="PanelSiteMgrTools" runat="server">
            <asp:Label ID="Label9" runat="server" CssClass="verdana13" Text="Revenue and Commission Overview:"></asp:Label>
            <br />
            <br />
            <table bgcolor="White" style="border-style: solid; border-width: 1px;">
                <tr>
                    <td align="center" class="auto-style1" bgcolor="#72FA83" style="padding: 7px; margin: 5px">&nbsp;</td>
                    <td align="center" bgcolor="#72FA83" class="auto-style1" style="padding: 7px; margin: 5px">app.123bitcoin.dk (2)</td>
                    <td align="center" class="auto-style1" bgcolor="#72FA83" style="padding: 7px; margin: 5px">app.simplekoin.com (15)</td>
                    <td align="center" class="auto-style1" bgcolor="#72FA83" style="padding: 7px; margin: 5px">app.mycoins.fr (16)</td>
                    <td align="center" class="auto-style1" bgcolor="#72FA83" style="padding: 7px; margin: 5px">app.dacapital.dk (19)</td>
                    <td align="center" class="auto-style1" bgcolor="#72FA83" style="padding: 7px; margin: 5px">...</td>
                    <td align="center" class="auto-style1" bgcolor="#72FA83" style="padding: 7px; margin: 5px">...</td>
                    <td align="center" class="auto-style1" bgcolor="#72FA83" style="padding: 7px; margin: 5px">...</td>
                    <td align="center" class="auto-style1" bgcolor="#72FA83" style="padding: 7px; margin: 5px">...</td>
                    <td align="center" class="auto-style1" bgcolor="#72FA83" style="padding: 7px; margin: 5px">...</td>
                </tr>
                <tr>
                    <td valign="middle" align="center" class="auto-style3">Total&nbsp; Revenue<br />
                    </td>
                    <td align="center" class="auto-style3" valign="middle">
                        <asp:Label ID="LabelTotalRev2" runat="server" Text="Label"></asp:Label></td>
                    <td valign="middle" align="center" class="auto-style5">
                        <asp:Label ID="LabelTotalRev15" runat="server" Text="Label"></asp:Label>
                        <br />
                    </td>
                    <td valign="middle" align="center" class="auto-style3">
                        <asp:Label ID="LabelTotalRev16" runat="server" Text="Label"></asp:Label>
                        <br />
                    </td>
                    <td valign="middle" align="center" class="auto-style5">
                        <asp:Label ID="LabelTotalRev19" runat="server" Text="Label"></asp:Label>
                        <br />
                    </td>
                    <td valign="middle" align="center" class="auto-style3">
                        <br />
                    </td>
                    <td valign="middle" align="center" class="auto-style5">
                        <br />
                    </td>
                    <td valign="middle" align="center" class="auto-style3">
                        <br />
                    </td>
                    <td valign="middle" align="center" class="auto-style5">
                        <br />
                    </td>
                    <td valign="middle" align="center" class="auto-style5">
                        <br />
                    </td>
                </tr>
                <tr>
                    <td align="center" class="auto-style3" valign="middle">Yesterdays revenue</td>
                    <td align="center" class="auto-style3" valign="middle">
                        <asp:Label ID="LabelYesterdaysRev2" runat="server" Text="Label"></asp:Label></td>
                    <td align="center" class="auto-style5" valign="middle">
                        <asp:Label ID="LabelYesterdaysRev15" runat="server" Text="Label"></asp:Label>
                    </td>
                    <td align="center" class="auto-style3" valign="middle">
                        <asp:Label ID="LabelYesterdaysRev16" runat="server" Text="Label"></asp:Label>
                    </td>
                    <td align="center" class="auto-style5" valign="middle">
                        <asp:Label ID="LabelYesterdaysRev19" runat="server" Text="Label"></asp:Label>
                    </td>
                    <td align="center" class="auto-style3" valign="middle">&nbsp;</td>
                    <td align="center" class="auto-style5" valign="middle">&nbsp;</td>
                    <td align="center" class="auto-style3" valign="middle">&nbsp;</td>
                    <td align="center" class="auto-style5" valign="middle">&nbsp;</td>
                    <td align="center" class="auto-style5" valign="middle">&nbsp;</td>
                </tr>
                <tr>
                    <td align="center" class="auto-style3" valign="middle">Todays Revenue</td>
                    <td align="center" class="auto-style3" valign="middle">
                        <asp:Label ID="LabelTodaysRev2" runat="server" Text="Label"></asp:Label></td>
                    <td align="center" class="auto-style5" valign="middle">
                        <asp:Label ID="LabelTodaysRev15" runat="server" Text="Label"></asp:Label>
                    </td>
                    <td align="center" class="auto-style3" valign="middle">
                        <asp:Label ID="LabelTodaysRev16" runat="server" Text="Label"></asp:Label>
                    </td>
                    <td align="center" class="auto-style5" valign="middle">
                        <asp:Label ID="LabelTodaysRev19" runat="server" Text="Label"></asp:Label>
                    </td>
                    <td align="center" class="auto-style3" valign="middle">&nbsp;</td>
                    <td align="center" class="auto-style5" valign="middle">&nbsp;</td>
                    <td align="center" class="auto-style3" valign="middle">&nbsp;</td>
                    <td align="center" class="auto-style5" valign="middle">&nbsp;</td>
                    <td align="center" class="auto-style5" valign="middle">&nbsp;</td>
                </tr>
                <tr>
                    <td align="center" class="auto-style3" valign="middle">Commision Total</td>
                    <td align="center" class="auto-style3" valign="middle">
                        <asp:Label ID="LabelTotalCommision2" runat="server" Text="Label"></asp:Label></td>
                    <td align="center" class="auto-style5" valign="middle">
                        <asp:Label ID="LabelTotalCommision15" runat="server" Text="Label"></asp:Label>
                    </td>
                    <td align="center" class="auto-style3" valign="middle">
                        <asp:Label ID="LabelTotalCommision16" runat="server" Text="Label"></asp:Label>
                    </td>
                    <td align="center" class="auto-style5" valign="middle"></td>
                    <td align="center" class="auto-style3" valign="middle">&nbsp;</td>
                    <td align="center" class="auto-style5" valign="middle">&nbsp;</td>
                    <td align="center" class="auto-style3" valign="middle">&nbsp;</td>
                    <td align="center" class="auto-style5" valign="middle">&nbsp;</td>
                    <td align="center" class="auto-style5" valign="middle">&nbsp;</td>
                </tr>
            </table>
            <br />
            <br />
            <asp:Label ID="Label55" runat="server" CssClass="verdana13" Text="Order Review:"></asp:Label>
            <br />
            <br />
            <asp:Button ID="ButtonReviewOrderSiteMgr" runat="server" PostBackUrl="~/ReviewOrders.aspx" Text="Review Orders" Width="200px" />
            <br />
            <br />
            <asp:Button ID="ButtonFindOrderSiteMgr" runat="server" PostBackUrl="~/FindOrders.aspx" Text="Find Order/User" Width="200px" />
            <br />
            <br />
            <asp:Button ID="ButtonTestEmail" runat="server" PostBackUrl="~/TestEmailCredentials.aspx" Text="Test Email Credentials" Width="200px" />
            <br />
            <br />
            <br />
        </asp:Panel>
        <br />
        <br />
        <br />
        <br />        
    </asp:Panel>
    <asp:SqlDataSource ID="SqlDataSourceAdmin" runat="server" ConnectionString="<%$ ConnectionStrings:LocalConnectionString %>" SelectCommand="SELECT [User].Id, [User].Fname FROM [User] INNER JOIN UserRole ON [User].RoleId = UserRole.Id WHERE (UserRole.Text = N'Admin') OR (UserRole.Text = N'SiteMgr') ORDER BY [User].RoleId, [User].Id"></asp:SqlDataSource>
    <asp:SqlDataSource ID="SqlDataSourceYourPay" runat="server" ConnectionString="<%$ ConnectionStrings:LocalConnectionString %>" SelectCommand="SELECT Id, ConfigKey, ConfigValue, ConfigDescription FROM AppSettings WHERE (ConfigKey = N'YourPayTestDetails') OR (ConfigKey = N'YourPayProdDetails')"></asp:SqlDataSource>
    &nbsp;
    
    <script type="text/javascript">
        // function getACBalance() {
        var acBalanceElement = document.getElementById('acbalanceBTC');
        var acbalanceEURElement = document.getElementById('acbalanceEUR');
        if (acBalanceElement && acbalanceEURElement) {
            var request = $.get('KrakenHandler.ashx?query=balance');
            request.success(function (result) {
                var acBalanceElement = document.getElementById('acbalanceBTC');
                var acbalanceEURElement = document.getElementById('acbalanceEUR');
                acBalanceElement.textContent = result.result.XXBT.toLocaleString('da-DK', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
                acbalanceEURElement.textContent = Math.round(result.result.ZEUR).toLocaleString('da-DK');
            });
        }
    // }
    </script>
</asp:Content>

