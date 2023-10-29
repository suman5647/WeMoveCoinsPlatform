<%@ Page Title="" Language="VB" MasterPageFile="~/MasterPageAdmin.master" AutoEventWireup="false" Inherits="WMC.Admin.Web._Default" Codebehind="Default.aspx.vb" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
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
        &nbsp;<asp:Label ID="Label2" runat="server" Text="New Password: "></asp:Label>
        <asp:TextBox ID="TextBoxNewPassword" runat="server"></asp:TextBox>

        <br />
        <br />
        <asp:Button ID="ButtonChangesPassword" runat="server" Text="Change Password" />
        &nbsp;
        <asp:Button ID="ButtonCancelPasswordChange" runat="server" Text="Cancel" />

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
                    <td align="center" class="auto-style1" bgcolor="#72FA83" style="padding: 7px; margin: 5px">Awaits KYC approval</td>
                    <td align="center" class="auto-style1" bgcolor="#72FA83" style="padding: 7px; margin: 5px">Operational Status</td>
                    <td align="center" class="auto-style1" bgcolor="#72FA83" style="padding: 7px; margin: 5px">Revenue</td>
                    <td align="center" class="auto-style1" bgcolor="#72FA83" style="padding: 7px; margin: 5px"># Orders total</td>
                    <td align="center" class="auto-style1" bgcolor="#72FA83" style="padding: 7px; margin: 5px"># KYC Declined</td>
                    <td align="center" class="auto-style1" bgcolor="#72FA83" style="padding: 7px; margin: 5px"># Completed</td>
                    <td align="center" class="auto-style1" bgcolor="#72FA83" style="padding: 7px; margin: 5px"># Customers</td>
                </tr>
                <tr>
                    <td valign="middle" align="center" class="auto-style3">
                        <asp:LinkButton ID="LinkButtonExceptions" runat="server" CssClass="verdana13" PostBackUrl="~/SystemException.aspx" Font-Bold="True">0</asp:LinkButton>
                        <br />
                    </td>
                    <td valign="middle" align="center" class="auto-style5">
                        <asp:LinkButton ID="LinkButtonUserKYC" runat="server" CssClass="verdana13" PostBackUrl="~/KycFile.aspx" Font-Bold="True">0</asp:LinkButton>
                        <br />
                    </td>
                    <td valign="middle" align="center" class="auto-style7">
                        <br />
                        <asp:DropDownList ID="DropDownListOperationalStatus" runat="server" AutoPostBack="True" CssClass="verdana13" Width="100px">
                            <asp:ListItem>Open</asp:ListItem>
                            <asp:ListItem Value="Soldout">SoldOut</asp:ListItem>
                        </asp:DropDownList>

                        <br />
                        <asp:DropDownList ID="DropDownListCreditCardGatewayName" runat="server" AutoPostBack="True" CssClass="verdana13" Width="100px">
                            <asp:ListItem>Auto</asp:ListItem>
                            <asp:ListItem Value="PayLike">PayLike</asp:ListItem>
                            <asp:ListItem>YourPay</asp:ListItem>
                        </asp:DropDownList>
                        <br />
                        <br />
                    </td>
                    <td valign="middle" align="center" class="auto-style3">
                        <asp:Label ID="LabelRevenueYesterday" runat="server" CssClass="verdana13" Font-Bold="True" Text="Label"></asp:Label><br />
                        <asp:Label ID="LabelRevenueToday" runat="server" CssClass="verdana13" Font-Bold="False" Text="Label"></asp:Label>
                        <%--<asp:LinkButton ID="LinkButtonRevenueYesterday" runat="server" CssClass="verdana13" PostBackUrl="~/KycFile.aspx" Font-Bold="True">0</asp:LinkButton>--%>
                        <br />
                    </td>
                    <td valign="middle" align="center" class="auto-style5">
                        <asp:LinkButton ID="LinkButtonOrderTotal" runat="server" CssClass="verdana13" PostBackUrl="~/KycFile.aspx" Font-Bold="True">0</asp:LinkButton>
                        <br />
                    </td>
                    <td valign="middle" align="center" class="auto-style3">
                        <asp:LinkButton ID="LinkButtonKycDeclined" runat="server" CssClass="verdana13" PostBackUrl="~/SystemException.aspx" Font-Bold="True">0</asp:LinkButton>
                        <br />
                    </td>
                    <td valign="middle" align="center" class="auto-style5">
                        <asp:LinkButton ID="LinkButtonCompleted" runat="server" CssClass="verdana13" PostBackUrl="~/KycFile.aspx" Font-Bold="True">0</asp:LinkButton>
                        <br />
                    </td>
                    <td valign="middle" align="center" class="auto-style5">
                        <asp:LinkButton ID="LinkButtonCustomers" runat="server" CssClass="verdana13" PostBackUrl="~/KycFile.aspx" Font-Bold="True">0</asp:LinkButton>
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
            <asp:Button ID="ButtonApprovePayout" runat="server" PostBackUrl="~/ApprovePayout.aspx" Text="Approve Payout" Width="200px" />
            <br />
            <br />
            <asp:Button ID="ButtonApproveBankPayment" runat="server" PostBackUrl="~/ApproveBank.aspx" Text="Approve Bank Payment" Width="200px" />
            <br />
            <br />
            <br />
            <asp:Button ID="ButtonCompliance" runat="server" PostBackUrl="~/Compliance.aspx" Text="Compliance Handling" Width="200px" />
            <br />
            <br />
            <asp:Button ID="ButtonReviewOrders" runat="server" PostBackUrl="~/ReviewOrders.aspx" Text="Review Orders" Width="200px" />
            <br />
            <br />
            <asp:Button ID="ButtonFindOrders" runat="server" PostBackUrl="~/FindOrders.aspx" Text="Find Order/User" Width="200px" />
            <br />
            <br />
            <br />
            <asp:Label ID="Label8" runat="server" CssClass="verdana13" Text="KYC handling:"></asp:Label>
            <br />
            <br />
            <asp:Button ID="ButtonPendingKyc" runat="server" PostBackUrl="~/KycFile.aspx" Text="Pending KYC File approval" Width="200px" />
            <br />
            <br />
            <asp:Button ID="ButtonAllKyc" runat="server" PostBackUrl="~/KycFileAll.aspx" Text="Explorer All KYC" Width="200px" />
            <br />
            <br />
                     
            <br />
            <br />
            
            <asp:Label ID="Label5" runat="server" CssClass="verdana13" Text="Backend stuff:"></asp:Label>
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
            <asp:Button ID="ButtonPayLikeExport0" runat="server" PostBackUrl="~/RiskList.aspx" Text="Risk Score List" Width="200px" />
            <br />
            <br />
        </asp:Panel>
        <asp:Panel ID="PanelSiteMgrTools" runat="server">
            <asp:Label ID="Label9" runat="server" CssClass="verdana13" Text="Revenue and Commission Overview:"></asp:Label>
            <br />
            <br />
            <table bgcolor="White" style="border-style: solid; border-width: 1px;">
                <tr>
                    <td align="center" class="auto-style1" bgcolor="#72FA83" style="padding: 7px; margin: 5px">&nbsp;</td>
                    <td align="center" class="auto-style1" bgcolor="#72FA83" style="padding: 7px; margin: 5px">app.simplekoin.com (15)</td>
                    <td align="center" class="auto-style1" bgcolor="#72FA83" style="padding: 7px; margin: 5px">app.mycoins.fr (16)</td>
                    <td align="center" class="auto-style1" bgcolor="#72FA83" style="padding: 7px; margin: 5px">app.wesbit.net (17)</td>
                    <td align="center" class="auto-style1" bgcolor="#72FA83" style="padding: 7px; margin: 5px">...</td>
                    <td align="center" class="auto-style1" bgcolor="#72FA83" style="padding: 7px; margin: 5px">...</td>
                    <td align="center" class="auto-style1" bgcolor="#72FA83" style="padding: 7px; margin: 5px">...</td>
                    <td align="center" class="auto-style1" bgcolor="#72FA83" style="padding: 7px; margin: 5px">...</td>
                    <td align="center" class="auto-style1" bgcolor="#72FA83" style="padding: 7px; margin: 5px">...</td>
                </tr>
                <tr>
                    <td valign="middle" align="center" class="auto-style3">
                        Total&nbsp; Revenue<br />
                    </td>
                    <td valign="middle" align="center" class="auto-style5">
                        <asp:Label ID="LabelTotalRev15" runat="server" Text="Label"></asp:Label>
                        <br />
                    </td>
                    <td valign="middle" align="center" class="auto-style3">
                        <asp:Label ID="LabelTotalRev16" runat="server" Text="Label"></asp:Label>
                        <br />
                    </td>
                    <td valign="middle" align="center" class="auto-style5">
                        <asp:Label ID="LabelTotalRev17" runat="server" Text="Label"></asp:Label>
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
                    <td align="center" class="auto-style5" valign="middle">
                        <asp:Label ID="LabelYesterdaysRev15" runat="server" Text="Label"></asp:Label>
                    </td>
                    <td align="center" class="auto-style3" valign="middle">
                        <asp:Label ID="LabelYesterdaysRev16" runat="server" Text="Label"></asp:Label>
                    </td>
                    <td align="center" class="auto-style5" valign="middle">
                        <asp:Label ID="LabelYesterdaysRev17" runat="server" Text="Label"></asp:Label>
                    </td>
                    <td align="center" class="auto-style3" valign="middle">&nbsp;</td>
                    <td align="center" class="auto-style5" valign="middle">&nbsp;</td>
                    <td align="center" class="auto-style3" valign="middle">&nbsp;</td>
                    <td align="center" class="auto-style5" valign="middle">&nbsp;</td>
                    <td align="center" class="auto-style5" valign="middle">&nbsp;</td>
                </tr>
                <tr>
                    <td align="center" class="auto-style3" valign="middle">Todays Revenue</td>
                    <td align="center" class="auto-style5" valign="middle">
                        <asp:Label ID="LabelTodaysRev15" runat="server" Text="Label"></asp:Label>
                    </td>
                    <td align="center" class="auto-style3" valign="middle">
                        <asp:Label ID="LabelTodaysRev16" runat="server" Text="Label"></asp:Label>
                    </td>
                    <td align="center" class="auto-style5" valign="middle">
                        <asp:Label ID="LabelTodaysRev17" runat="server" Text="Label"></asp:Label>
                    </td>
                    <td align="center" class="auto-style3" valign="middle">&nbsp;</td>
                    <td align="center" class="auto-style5" valign="middle">&nbsp;</td>
                    <td align="center" class="auto-style3" valign="middle">&nbsp;</td>
                    <td align="center" class="auto-style5" valign="middle">&nbsp;</td>
                    <td align="center" class="auto-style5" valign="middle">&nbsp;</td>
                </tr>
                <tr>
                    <td align="center" class="auto-style3" valign="middle">Commision Total</td>
                    <td align="center" class="auto-style5" valign="middle">
                        <asp:Label ID="LabelTotalCommision15" runat="server" Text="Label"></asp:Label>
                    </td>
                    <td align="center" class="auto-style3" valign="middle">
                        <asp:Label ID="LabelTotalCommision16" runat="server" Text="Label"></asp:Label>
                    </td>
                    <td align="center" class="auto-style5" valign="middle">
                        <asp:Label ID="LabelTotalCommision17" runat="server" Text="Label"></asp:Label>
                    </td>
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
</asp:Content>

