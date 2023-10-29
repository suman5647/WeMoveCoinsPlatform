<%@ Page Title="" Language="VB" MasterPageFile="~/MasterPageAdmin.master" AutoEventWireup="false" CodeFile="Compliance.aspx.vb" Inherits="Compliance" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/2.1.1/jquery.min.js"></script>
    <style type="text/css">
        .auto-style1 {
            height: 16px;
        }

        .modalBackground {
            background-color: rgba(128, 128, 128, 0.55);
            width: 700px;
            height: 700px;
        }

        .panelStyle {
            display: block;
            top: auto;
            padding: 10px;
            background: white;
            border: 1px solid rgb(0, 0, 0);
            border-radius: 5px;
        }

        /* The Modal (background) */
        .modal {
            display: none; /* Hidden by default */
            position: fixed; /* Stay in place */
            z-index: 1; /* Sit on top */
            padding-top: 100px; /* Location of the box */
            left: 0;
            top: 0;
            width: 100%; /* Full width */
            height: 100%; /* Full height */
            overflow: auto; /* Enable scroll if needed */
            background-color: rgb(0,0,0); /* Fallback color */
            background-color: rgba(0,0,0,0.4); /* Black w/ opacity */
        }

        /* Modal Content */
        .modal-content {
            background-color: #fefefe;
            margin: auto;
            padding: 20px;
            border: 1px solid #888;
            width: 80%;
        }

        /* The Close Button */
        .close {
            color: #aaaaaa;
            float: right;
            font-size: 28px;
            font-weight: bold;
        }

            .close:hover,
            .close:focus {
                color: #000;
                text-decoration: none;
                cursor: pointer;
            }
        /* The Modal (background) */

        #frame {
            display: inline-block;
        }

        .controls {
            z-index: 1;
            position: relative;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">

    <div id="myModal" class="modal">
        <!-- Modal content -->
        <div class="modal-content">
            <span class="close">&times;</span>
            <div>
                <div id="frame" data-angle="0" data-flipped="false" style="width: 300px; height: 300px;">
                    <img id="imageToTransform" src="" style="max-height: 300px; max-width: 300px;" />
                </div>
                <div class="controls">
                    <input type="hidden" id="imageIDToTransform" />
                    <input type="hidden" id="flipped" value="false" />
                    <input type="hidden" id="angle" value="0" />
                    <input type="button" id="rotateLeftButton" value="&#8634; Rotate Left" />
                    <input type="button" id="rotateRightButton" value="&#8635; Rotate Right" />
                    <input type="button" id="flipButton" value="&#8644; Flip Horizondal" />
                    <input type="button" id="saveImageTrans" value="Save" />
                </div>
            </div>
        </div>
    </div>
    <table>
        <tr>
            <td style="padding: 10px; margin: 10px">
                <asp:LinkButton ID="LinkButtonBACK" runat="server" PostBackUrl="~/Default.aspx">Back</asp:LinkButton>
            </td>
            <td style="padding: 10px; margin: 10px">
                <asp:Button ID="ButtonEnhancedDueDiligence" runat="server" Text="Enhanced Due Diligence" />
            </td>
            <td style="padding: 10px; margin: 10px">
                <asp:Button ID="ButtonComplianceOfficerApproval" runat="server" Text="Compliance Officer Approval" Font-Bold="False" />
            </td>
            <td style="padding: 10px; margin: 10px">
                <asp:Button ID="ButtonCustomerResponsePending" runat="server" Text="Customer Response Pending" />
            </td>
            <td style="padding: 10px; margin: 10px">
                <asp:Button ID="ButtonKYCApprovalPending" runat="server" Text="KYC Approval Pending" />
            </td>
            <td>
                <asp:Button ID="ButtonFindOrders" runat="server" PostBackUrl="~/FindOrders.aspx" Text="Find Order/User" Width="190px" />
            </td>
        </tr>
        <tr>
            <td colspan="4">
                <p class="verdana13">
                    <%--<asp:DropDownList ID="DropDownListStatus" runat="server" CssClass="DropdownList" AutoPostBack="True" Font-Size="Small" Enabled="False">
                        <asp:ListItem Value="Enhanced Due Diligence">Enhanced Due Diligence</asp:ListItem>
                        <asp:ListItem Value="Compliance Officer Approval">Compliance Officer Approval</asp:ListItem>
                        <asp:ListItem Value="Customer Response Pending">Customer Response Pending</asp:ListItem>
                        <asp:ListItem Value="KYC Approval Pending">KYC Approval Pending</asp:ListItem>
                    </asp:DropDownList>--%>
                </p>
            </td>
        </tr>
    </table>
    <table aria-orientation="horizontal">
        <tr>
            <td></td>
            <asp:Panel ID="PanelNote" runat="server" Visible="False">
                <p class="verdana13">
                    <td style="text-align: right">
                        <asp:TextBox ID="TextBoxNote" runat="server" TextMode="MultiLine" Width="300px"></asp:TextBox>&nbsp;
                    <asp:Button ID="ButtonAddNote" runat="server" Text="Add Note" />
                        &nbsp; &nbsp; &nbsp;
                    <asp:DropDownList ID="DropDownListNoteOptions" CssClass="DropdownList" runat="server" AutoPostBack="True" Width="300px">
                        <asp:ListItem Selected="True">[blank]</asp:ListItem>
                        <asp:ListItem>MobilePay OK</asp:ListItem>
                        <asp:ListItem>Contacted regarding delay</asp:ListItem>
                        <asp:ListItem>NOT MobilePay</asp:ListItem>
                        <asp:ListItem>Requested via HappyFox</asp:ListItem>
                        <asp:ListItem>ID looks photoshopped?</asp:ListItem>
                        <asp:ListItem>Phishing?</asp:ListItem>
                        <asp:ListItem>Accepted (seems legit)</asp:ListItem>
                        <asp:ListItem>Need ID in better quality</asp:ListItem>
                        <asp:ListItem>Need Selfie in better quality</asp:ListItem>
                    </asp:DropDownList>
                        &nbsp;                
                    </td>
                </p>
            </asp:Panel>
        </tr>
        <tr>
            <td colspan="2" style="text-align: right">
                <p>
                    <asp:GridView ID="GridViewOrder" runat="server" OnRowDataBound="GridViewOrder_RowDataBound" AllowSorting="True" AutoGenerateColumns="False" DataKeyNames="Id" BackColor="White" BorderColor="#336666" BorderStyle="Double" BorderWidth="3px" CellPadding="4" EmptyDataText="No orders awaits approval" GridLines="Horizontal" Width="100%">
                        <Columns>
                            <asp:TemplateField HeaderText="">
                                <ItemTemplate>
                                    <asp:CheckBox ID="cbSelect" runat="server"></asp:CheckBox>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:CommandField ShowSelectButton="True" SelectText="Review" />
                            <asp:BoundField DataField="Quoted" DataFormatString="{0:G}" HeaderText="Quoted" SortExpression="Quoted" />
                            <asp:BoundField DataField="Id" HeaderText="Id" ReadOnly="True" SortExpression="Id" Visible="False" />
                            <asp:BoundField DataField="Number" HeaderText="Number" SortExpression="Number" ReadOnly="True" />
                            <asp:BoundField DataField="Fname" HeaderText="Name" SortExpression="Fname" ReadOnly="True" Visible="False" />
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
                            <asp:TemplateField HeaderText="Currency" SortExpression="Code" Visible="False">
                                <EditItemTemplate>
                                    <asp:Label ID="Label2" runat="server" Text='<%# Eval("Code") %>'></asp:Label>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="Label3" runat="server" Text='<%# Bind("Code") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="Quoted" HeaderText="Quoted" SortExpression="Quoted" ReadOnly="True" Visible="False" />
                            <asp:BoundField DataField="Name" HeaderText="Name" SortExpression="Name" ReadOnly="True">
                                <HeaderStyle HorizontalAlign="Left" />
                            </asp:BoundField>
                            <asp:BoundField DataField="Email" HeaderText="Email" SortExpression="Email" ReadOnly="True">
                                <HeaderStyle HorizontalAlign="Left" />
                            </asp:BoundField>
                            <asp:BoundField DataField="IP" HeaderText="IP" SortExpression="IP" ReadOnly="True" Visible="False" />
                            <asp:TemplateField HeaderText="Approve">
                                <ItemTemplate>
                                    <asp:LinkButton ID="ButtonApprove" runat="server" BackColor="#66FF66" ForeColor="#333333" BorderWidth="1px" BorderStyle="ridge" Style="padding: 1px" Font-Underline="false" CommandName="Approve" Text="Approve" />
                                </ItemTemplate>
                                <HeaderStyle HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Move to:">
                                <ItemTemplate>
                                    <asp:LinkButton ID="ButtonMoveToCompliance" runat="server" ForeColor="#333333" Style="padding: 1px" Font-Underline="false" BackColor="#D0E8FF" BorderStyle="ridge" CommandName="MoveToCompliance" Text="Compliance" />
                                </ItemTemplate>
                                <HeaderStyle HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Move to:">
                                <ItemTemplate>
                                    <asp:LinkButton ID="ButtonCustomerPending" runat="server" BackColor="#D0E8FF" BorderStyle="ridge" ForeColor="#333333" Style="padding: 1px" Font-Underline="false" CommandName="CustomerPending" Text="Customer" />
                                </ItemTemplate>
                                <HeaderStyle HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Move to:">
                                <ItemTemplate>
                                    <asp:LinkButton ID="ButtonMoveToEDD" runat="server" BackColor="#D0E8FF" BorderStyle="ridge" ForeColor="#333333" Style="padding: 1px" Font-Underline="false" CommandName="MoveToEDD" Text="Enhanced" />
                                </ItemTemplate>
                                <HeaderStyle HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Ask For:">
                                <ItemTemplate>
                                    <asp:LinkButton ID="ButtonReviewKYC" runat="server" BackColor="#FFFF66" BorderStyle="ridge" Font-Underline="false" CommandName="ReviewKYC" Text="KYCDoc" ForeColor="Black" />
                                </ItemTemplate>
                                <HeaderStyle HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Ask For:">
                                <ItemTemplate>
                                    <asp:LinkButton ID="ButtonTxSecret" runat="server" BackColor="#FFFF66" BorderStyle="ridge" Style="padding: 1px" Font-Underline="false" CommandName="TxSecret" Text="TxSecret" ForeColor="Black" />
                                </ItemTemplate>
                                <HeaderStyle HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="KYC Declined">
                                <ItemTemplate>
                                    <asp:LinkButton ID="ButtonKYCDeclined" runat="server" BackColor="#FFA8C5" BorderStyle="ridge" Style="padding: 1px" Font-Underline="false" CommandName="KYCDeclined" Text="Decline" ForeColor="Black" />
                                </ItemTemplate>
                                <HeaderStyle HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Cancel">
                                <ItemTemplate>
                                    <asp:LinkButton ID="ButtonCancelOrder" runat="server" BackColor="#FFA8C5" BorderStyle="ridge" Style="padding: 1px" Font-Underline="false" CommandName="CancelOrder" Text="Cancel" ForeColor="Black" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="EDD">
                                <ItemTemplate>
                                    <asp:LinkButton ID="ButtonApproveEDD" runat="server" BackColor="#66FF66" BorderStyle="ridge" Style="padding: 1px" Font-Underline="false" CommandName="ApproveEDD" Text="ApproveEDD" ForeColor="Black" />
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Center" />
                            </asp:TemplateField>
                            <asp:BoundField DataField="UserRiskLevel" HeaderText="User RiskLevel" ReadOnly="True">
                                <ItemStyle HorizontalAlign="Center" />
                            </asp:BoundField>
                            <asp:BoundField DataField="CountryCode" SortExpression="CountryCode" />
                            <asp:BoundField DataField="SiteId" HeaderText="Site" ReadOnly="True" SortExpression="SiteId" />
                            <asp:TemplateField HeaderText="Note">
                                <EditItemTemplate>
                                    <asp:TextBox ID="TextBoxNote" runat="server" Height="175px" Text='<%# Bind("Note") %>' TextMode="MultiLine" Width="377px"></asp:TextBox>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="Label1" runat="server" Text='<%# Bind("Note") %>' Height="16px" Width="250px"></asp:Label>
                                </ItemTemplate>
                                <HeaderStyle HorizontalAlign="Center" />
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
                <asp:Panel ID="PanelSellDetails" runat="server" Visible="False">
                    <asp:GridView ID="GridViewSellDetails" runat="server" AutoGenerateColumns="False" DataSourceID="SqlDataSourceSellDetails" BackColor="White" BorderColor="#CCCCCC" BorderStyle="None" BorderWidth="1px" CellPadding="3">
                        <Columns>
                            <asp:TemplateField HeaderText="Amount" SortExpression="Amount">
                                <EditItemTemplate>
                                    <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("Amount") %>'></asp:TextBox>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="Label4" runat="server" Text='<%# Bind("Amount", "{0:0,0}") %>'></asp:Label>
                                    <asp:Label ID="Label5" runat="server" Text='<%# Bind("Code") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="BTCAmount" HeaderText="BTCAmount" SortExpression="BTCAmount" />
                            <asp:BoundField DataField="Rate" HeaderText="Rate" SortExpression="Rate" />
                            <asp:BoundField DataField="Reg" HeaderText="Registration Number" SortExpression="Reg" />
                            <asp:BoundField DataField="AccountNumber" HeaderText="Account Number" SortExpression="AccountNumber" />
                            <asp:BoundField DataField="SwiftBIC" HeaderText="SwiftBIC" SortExpression="SwiftBIC" />
                            <asp:BoundField DataField="IBAN" HeaderText="IBAN" SortExpression="IBAN" />
                            <asp:BoundField DataField="RateBase" HeaderText="RateBase" SortExpression="RateBase" />
                            <asp:BoundField DataField="RateHome" HeaderText="RateHome" SortExpression="RateHome" />
                            <asp:BoundField DataField="RateBooks" HeaderText="RateBooks" SortExpression="RateBooks" />
                            <asp:BoundField DataField="TransactionHash" HeaderText="TransactionHash" SortExpression="TransactionHash" />
                        </Columns>
                        <FooterStyle BackColor="White" ForeColor="#000066" />
                        <HeaderStyle BackColor="#006699" Font-Bold="True" ForeColor="White" />
                        <PagerStyle BackColor="White" ForeColor="#000066" HorizontalAlign="Left" />
                        <RowStyle ForeColor="#000066" />
                        <SelectedRowStyle BackColor="#669999" Font-Bold="True" ForeColor="White" />
                        <SortedAscendingCellStyle BackColor="#F1F1F1" />
                        <SortedAscendingHeaderStyle BackColor="#007DBB" />
                        <SortedDescendingCellStyle BackColor="#CAC9C9" />
                        <SortedDescendingHeaderStyle BackColor="#00547E" />
                    </asp:GridView>
                    <table>
                        <tr>
                            <td>
                                 <table style="text-align: left; border-style: solid; border-width: 1px; background-color: lightyellow;">
                                    <tr style="text-align:center;">
                                        <td><strong>Amount</strong></td>
                                        <td><strong>Rate</strong></td>
                                        <td><strong>Rate Base</strong></td>
                                        <td><strong>Rate Home</strong></td>
                                        <td><strong>Rate Books</strong></td>
                                    </tr>
                                    <tr>
                                        <td><asp:TextBox runat="server" TabIndex="1000" ID="TxtOrderAmount"></asp:TextBox></td>
                                        <td><asp:TextBox runat="server" TabIndex="1001" ID="TxtOrderRate"></asp:TextBox></td>
                                        <td><asp:TextBox runat="server" TabIndex="1002" ID="TxtOrderRateBase"></asp:TextBox></td>
                                        <td><asp:TextBox runat="server" TabIndex="1003" ID="TxtOrderRateHome"></asp:TextBox></td>
                                        <td><asp:TextBox runat="server" TabIndex="1004" ID="TxtOrderRateBooks"></asp:TextBox></td>
                                    </tr>
                                    <tr>
                                        <td colspan="5" style="text-align:right"><asp:Button ID="BtnUpdateOrderRates" runat="server" Text="Update Order Rates" TabIndex="1005" OnClientClick='confirm("Are you sure you want to update order rates?");' /></td>
                                    </tr>
                                </table>
                            </td>
                            <td style="vertical-align: top;margin-left:30px; border-style: solid; border-width: 1px; background-color: lightcyan;">
                                <table>
                                    <tr>
                                        <td><strong>Amount</strong></td>
                                        <td><strong>Currency</strong></td>
                                    </tr>
                                    <tr>
                                        <td><asp:TextBox ID="TextBoxSellAmount" runat="server"></asp:TextBox></td>
                                        <td>
                                            <asp:DropDownList ID="DropDownListSellAmountCurrency" runat="server" DataSourceID="SqlDataSourceCurrency" DataTextField="Code" DataValueField="Id" CssClass="DropdownList">
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="2" style="text-align:right">
                                            <asp:Button ID="ButtonSellApprove" runat="server" Text="Payout is Completed and Send Email" />
                                        </td>
                                    </tr>
                                </table>                                
                            </td>
                        </tr>
                    </table>
                    <asp:SqlDataSource ID="SqlDataSourceCurrency" runat="server" ConnectionString="<%$ ConnectionStrings:LocalConnectionString %>" SelectCommand="SELECT [Id], [Code], [Text] FROM [Currency]"></asp:SqlDataSource>
                </asp:Panel>

                <asp:SqlDataSource ID="SqlDataSourceSellDetails" runat="server" ConnectionString="<%$ ConnectionStrings:LocalConnectionString %>" SelectCommand="SELECT [Order].Rate, [Order].Amount, Currency.Code, [Order].BTCAmount, [Order].Reg, [Order].AccountNumber, [Order].SwiftBIC, [Order].IBAN, [Order].RateBase, [Order].RateHome, [Order].RateBooks, [Order].TransactionHash FROM [Order] INNER JOIN Currency ON [Order].CurrencyId = Currency.Id WHERE ([Order].Id = @Id)">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="GridViewOrder" Name="Id" PropertyName="SelectedValue" />
                    </SelectParameters>
                </asp:SqlDataSource>

                <p>
                    &nbsp;
                </p>
                <asp:Button ID="ButtonDeleteSelected" runat="server" Text="Cancelled Selected" />
                &nbsp;
                <asp:Button ID="ButtonUpdateMinersFee" runat="server" Text="Update MinersFee Selected" Visible="False" />
                <asp:TextBox ID="TextBoxMinersFee" runat="server" Visible="False">0,0005</asp:TextBox>
                </p>
            </td>
        </tr>
    </table>
    <asp:Panel ID="PanelTrustCalculation" runat="server" Visible="False">
        <table bgcolor="White" style="border-style: solid; border-width: 1px; width: 100%;">
            <tr>
                <td align="left" colspan="7">
                    <asp:Label ID="LabelTrustMessage" runat="server" Text="Label" BackColor="#FFFF66" Font-Bold="True"></asp:Label>&nbsp;&nbsp;&nbsp;&nbsp;
                    <asp:Label ID="LabelTrustMessageNew" runat="server" Text="Label"></asp:Label>
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
                            <td>Speed alert</td>
                            <td>
                                <asp:Label ID="LabelSpeedAlert" runat="server" Font-Bold="True" Text="" Width="55px"></asp:Label></td>
                        </tr>
                        <%--<tr>
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
                        </tr>--%>
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
                        <%--<tr>
                            <td>US-Card</td>
                            <td>
                                <asp:Label ID="LabelCardUS" runat="server" Font-Bold="True" Text="Label" Width="55px"></asp:Label>
                            </td>
                        </tr>--%>
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
                            <td>Card Span</td>
                            <td>
                                <asp:Label ID="LabelCardSpan" runat="server" Font-Bold="True" Text="Label" Width="55px"></asp:Label><asp:Button ID="ButtonApproveCard" runat="server" BackColor="LightGreen" Height="16px" Text="Auto Approve Card" Visible="False" />

                            </td>
                        </tr>
                        <tr>
                            <td>Order Sum</td>
                            <td>
                                <asp:Label ID="LabelOrderSUM" runat="server" Font-Bold="True" Text="Label" Width="55px"></asp:Label></td>
                        </tr>
                        <%--<tr>
                            <td>Sum Total</td>
                            <td>
                                <asp:Label ID="LabelOrderTotal" runat="server" Font-Bold="True" Text="Label" Width="55px"></asp:Label></td>
                        </tr>
                        <tr>
                            <td>Sum 30 Days</td>
                            <td>
                                <asp:Label ID="LabelOrder30Days" runat="server" Font-Bold="True" Text="Label" Width="55px"></asp:Label>
                            </td>
                        </tr>--%>
                    </table>
                    <br />
                    <table style="border-style: solid; border-width: 1px; width: 340px; text-align: left;">
                        <tr>
                            <td>
                                <asp:CheckBox ID="CheckBoxIsUserTrusted" runat="server" AutoPostBack="True" Text="Trusted User" Width="200px" />
                            </td>
                            <td>
                                <asp:Button ID="ButtonResetTxAttempt" runat="server" Text="RzTx" />
                                &nbsp;
                               <asp:HyperLink ID="HyperLinkTx" runat="server" Text="Test" Target="_blank"></asp:HyperLink>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2" class="auto-style1">Chainalysis:
                                <asp:Label ID="LabelChainalysis" runat="server" Font-Bold="True" Text="Label" Width="130px"></asp:Label>
                                <asp:HyperLink ID="HyperLinkChainalysis" Target="_blank" runat="server">Open...</asp:HyperLink>
                            </td>
                        </tr>
                    </table>
                </td>
                <td valign="top" align="center">
                    <table style="border-style: solid; border-width: 1px; width: 340px; text-align: left;">
                        <tr>
                            <td>Trusted</td>
                            <td>
                                <asp:Label ID="LabelUserIsTrusted" runat="server" Font-Bold="True" Text="Label" Width="55px" Height="16px"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>Number of Cards</td>
                            <td>
                                <asp:Label ID="LabelCards" runat="server" Font-Bold="True" Text="Label" Width="55px"></asp:Label>
                            </td>
                        </tr>
                        <%--<tr>
                            <td>Used Names</td>
                            <td>
                                <asp:Label ID="LabelNames" runat="server" Font-Bold="True" Text="Label"></asp:Label></td>
                        </tr>
                        <tr>
                            <td>Used Emails</td>
                            <td>
                                <asp:Label ID="LabelEmails" runat="server" Font-Bold="True" Text="Label"></asp:Label></td>
                        </tr>--%>
                        <tr>
                            <td>KYC Approved</td>
                            <td>
                                <asp:Label ID="LabelKycApproved" runat="server" Font-Bold="True" Text="Label"></asp:Label></td>
                        </tr>
                        <tr>
                            <td>KYC Declined</td>
                            <td>
                                <asp:Label ID="LabelKYCDeclined" runat="server" Font-Bold="True" Text="Label" Width="55px"></asp:Label></td>
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
    <p class="verdana13">
        User Details:
    </p>
    <p>
        <asp:GridView ID="GridViewUser" runat="server" AutoGenerateColumns="False" DataKeyNames="Id" DataSourceID="SqlDataSourceUser" EmptyDataText="No record" BackColor="#DEBA84" BorderColor="#DEBA84" BorderStyle="None" BorderWidth="1px" CellPadding="3" CellSpacing="2">
            <Columns>
                <asp:TemplateField HeaderText="User Id" InsertVisible="False" SortExpression="Id">
                    <EditItemTemplate>
                        <asp:Label ID="Label1" runat="server" Text='<%# Bind("Id") %>'></asp:Label>
                    </EditItemTemplate>
                    <ItemTemplate>
                        <asp:HyperLink ID="HyperLinkUserLookup" runat="server" Target="_blank" NavigateUrl='<%# Eval("Id")%>' Text='<%# Bind("Id")%>'></asp:HyperLink>

                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="Fname" HeaderText="Name" SortExpression="Fname" />
                <asp:BoundField DataField="Phone" HeaderText="Phone" SortExpression="Phone" />
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
                        <asp:Label ID="TextBox1" runat="server" Text='<%# Bind("Message") %>' ReadOnly="True" TextMode="MultiLine" Style="word-wrap: break-word;"></asp:Label>
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
    <%--<asp:SqlDataSource ID="SqlDataSourceUserAuditTrail" runat="server" ConnectionString="<%$ ConnectionStrings:LocalConnectionString %>" SelectCommand="SELECT AuditTrail.OrderId, AuditTrail.Status, AuditTrail.Message, AuditTrail.Created, AuditTrailStatus.Text FROM AuditTrail INNER JOIN AuditTrailStatus ON AuditTrail.Status = AuditTrailStatus.Id WHERE AuditTrail.Message LIKE 'CUSTOMER:'+  CAST((@UserId) AS NVARCHAR(10)) + '%' ORDER BY AuditTrail.Created">
        <SelectParameters>
            <asp:ControlParameter ControlID="GridViewOrder" Name="UserId" PropertyName="Select" />
        </SelectParameters>
    </asp:SqlDataSource>--%>
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
    <asp:Panel ID="PanelKeyIndicators" runat="server" BorderWidth="3px" Visible="False" BorderColor="#FF3300" Width="100%">
        <br />
        <p class="verdana13">Key Indicator Details</p>
        <p class="verdana13">
            <asp:Button ID="ButtonPanelClose" runat="server" Text="Close" />
        </p>
        <asp:GridView ID="GridViewKeyIndicator" runat="server" EmptyDataText="No Date!">
        </asp:GridView>
    </asp:Panel>
    <p class="verdana13">
        KYC Files:&nbsp;
    </p>
    <table style="width: 100%;">
        <tr>
            <td>
                <asp:GridView ID="GridViewKycFile" runat="server" AutoGenerateColumns="False" DataKeyNames="Id" DataSourceID="SqlDataSourceKycFileAll" CellPadding="3" BackColor="White" BorderColor="#CCCCCC" BorderStyle="None" BorderWidth="1px">
                    <Columns>
                        <asp:TemplateField HeaderText="">
                            <ItemTemplate>
                                <asp:CheckBox ID="cbSelectKyc" runat="server"></asp:CheckBox>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="Id" HeaderText="Id" ReadOnly="True" SortExpression="Id" />
                        <asp:TemplateField ShowHeader="False" Visible="False">
                            <ItemTemplate>
                                <asp:LinkButton ID="LinkButtonShowDoc" runat="server" CausesValidation="False" CommandName="ShowDoc" Text="ShowNew"></asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField ShowHeader="False">
                            <ItemTemplate>
                                <asp:LinkButton ID="LinkButton1" runat="server" CausesValidation="False" CommandArgument="<%# Container.DataItemIndex %>" CommandName="Select" Text="Show"></asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:CommandField ShowEditButton="True" />
                        <asp:TemplateField ShowHeader="False">
                            <ItemTemplate>
                                <asp:Button ID="ButtonApproveNow" runat="server" CausesValidation="False" CommandName="ApproveNow" Text="Approve"></asp:Button>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Type" SortExpression="Type">
                            <EditItemTemplate>
                                <asp:DropDownList ID="DropDownListKycType0" runat="server" DataSourceID="SqlDataSourceKycType" DataTextField="Text" DataValueField="Id" SelectedValue='<%# Bind("Type") %>'>
                                </asp:DropDownList>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="LabelKycType" runat="server" Text='<%# Bind("Text") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="OriginalFilename" HeaderText="OriginalFilename" SortExpression="OriginalFilename" ReadOnly="True" />
                        <asp:BoundField DataField="UniqueFilename" HeaderText="UniqueFilename" ReadOnly="True" SortExpression="UniqueFilename" />
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
                        <asp:BoundField DataField="RejectedByName" HeaderText="RejectedBy" SortExpression="RejectedByName" />
                        <asp:TemplateField HeaderText="Approved" SortExpression="Approved">
                            <EditItemTemplate>
                                <asp:Button ID="ButtonApprove" runat="server" CommandName="Approve" Text="Approve" />
                                <asp:Button ID="ButtonUndoApprove" runat="server" CommandName="UndoApprove" Text="X" />
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="LabelApprove" runat="server" Text='<%# Bind("Approved") %>' Width="100px"></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="ApprovedByName" HeaderText="ApprovedBy" SortExpression="ApprovedByName" />
                        <asp:TemplateField HeaderText="Obsolete" SortExpression="Obsolete" Visible="False">
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
                                <asp:LinkButton ID="LinkButtonDelete" runat="server" CausesValidation="False" CommandName="Delete" CommandArgument='<%# Eval("Id") %>' Text="Delete"></asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <FooterStyle BackColor="White" ForeColor="#000066" />
                    <HeaderStyle BackColor="#006699" Font-Bold="True" ForeColor="White" />
                    <PagerStyle BackColor="White" ForeColor="#000066" HorizontalAlign="Left" />
                    <RowStyle ForeColor="#000066" />
                    <SelectedRowStyle BackColor="#669999" Font-Bold="True" ForeColor="White" />
                </asp:GridView>
                <br />
                <asp:Button ID="ButtonDeleteKyc" runat="server" Text="Deleted Selected" />
            </td>
            <td>&nbsp;<asp:Button ID="btnApproveCardDoc" runat="server" Text="Approve Card Docs" /></td>
        </tr>
    </table>
    <asp:SqlDataSource ID="SqlDataSourceKycFileAll" runat="server" ConnectionString="<%$ ConnectionStrings:LocalConnectionString %>" DeleteCommand="DELETE FROM [KycFile] WHERE [Id] = @Id" InsertCommand="INSERT INTO [KycFile] ([Note], [Uploaded], [Rejected], [RejectedBy], [Approved], [ApprovedBy]) VALUES (@Note, @Uploaded, @Rejected, @RejectedBy, @Approved, @ApprovedBy)" SelectCommand="SELECT KycFile.Id, KycFile.UserId,NULL as [File] , KycFile.Note, KycFile.Uploaded, KycFile.Rejected, KycFile.RejectedBy, KycFile.Approved, KycFile.ApprovedBy, KycType.Text, KycFile.Obsolete, KycFile.OriginalFilename, KycFile.Type, [Order].Id AS OrderId, KycFile.UniqueFilename, User_1.Lname AS ApprovedByName, [User].Lname AS RejectedByName FROM KycFile INNER JOIN KycType ON KycFile.Type = KycType.Id INNER JOIN [Order] ON KycFile.UserId = [Order].UserId LEFT OUTER JOIN [User] ON KycFile.RejectedBy = [User].Id LEFT OUTER JOIN [User] AS User_1 ON KycFile.ApprovedBy = User_1.Id WHERE ([Order].Id = @OrderId) ORDER BY KycFile.Uploaded DESC" UpdateCommand="UPDATE KycFile SET Note = @Note, Type = @Type WHERE (Id = @Id)">
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
    <br />
    <br />
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <asp:Panel ID="Popup" runat="server" CssClass="panelStyle">
        <h1>KYC File</h1>
        <asp:Image ID="ImageDoc" runat="server" Width="300px" />
        <img runat="server" id="imagePreview" style="max-width: 400px;" visible="false" />
        <div>
            <asp:Literal ID="ltEmbed" Visible="false" runat="server" />
        </div>
        <input runat="server" type="hidden" id="imageId" value="0" />
        <br />
        <br />
        <asp:Button ID="btnCancel" runat="server" Text="Close" Style="float: right" OnClick="btncancel_Click" CausesValidation="false" />
    </asp:Panel>
    <asp:ModalPopupExtender ID="ModalPopupExtender1" runat="server" TargetControlID="btnfakecancel"
        PopupControlID="Popup" CancelControlID="btnfakecancel" BackgroundCssClass="modalBackground">
    </asp:ModalPopupExtender>
    <asp:Button ID="btnfakecancel" Style="display: none;" runat="server" />
    <br />
    <br />
    <p>
        <input type="button" runat="server" id="editImageBtn" value="Edit Image" onclick="editImageInPopup" visible="false" />
    </p>
    <%-- <asp:Image ID="ImageDoc" runat="server" Width="300px" />
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
    </asp:GridView>--%><%-- <asp:SqlDataSource ID="SqlDataSourceKycImage" runat="server" ConnectionString="<%$ ConnectionStrings:LocalConnectionString %>" SelectCommand="SELECT [File], Id FROM KycFile WHERE (Id = @Id)">
        <SelectParameters>
            <asp:ControlParameter ControlID="GridViewKycFile" Name="Id" PropertyName="SelectedValue" />
        </SelectParameters>
    </asp:SqlDataSource>--%>
    <p>
        <asp:DropDownList ID="DropDownListKycType" runat="server" DataSourceID="SqlDataSourceKycType" DataTextField="Text" DataValueField="Id" CssClass="DropdownList">
        </asp:DropDownList>
        &nbsp;&nbsp;
    <asp:FileUpload ID="FileUpload1" runat="server" />
        <asp:Button ID="btnUpload" runat="server" Text="Upload" />
        <asp:SqlDataSource ID="SqlDataSourceKycType" runat="server" ConnectionString="<%$ ConnectionStrings:LocalConnectionString %>" SelectCommand="SELECT * FROM [KycType]"></asp:SqlDataSource>
        &nbsp;
    <br />
        <asp:Label ID="lblMessage" runat="server" Text="" Font-Names="Arial"></asp:Label>
        <br />
    </p>
    <p class="verdana13">Orders in total:</p>
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
            <asp:BoundField DataField="OurFee" HeaderText="Fee (%)" SortExpression="OurFee" />
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
                    <asp:Label ID="Label1" runat="server" Text='<%# Bind("Note") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="Status" HeaderText="Status" SortExpression="Status" />
            <asp:BoundField DataField="CardNumber" HeaderText="CardNumber" SortExpression="CardNumber" />
            <asp:BoundField DataField="TxSecret" HeaderText="TxSecret" SortExpression="TxSecret" />
            <asp:BoundField DataField="BrowserCulture" HeaderText="Culture" SortExpression="BrowserCulture" />
            <asp:BoundField DataField="SiteId" HeaderText="SiteId" SortExpression="SiteId" />
            <asp:BoundField DataField="PartnerId" HeaderText="PartnerId" SortExpression="PartnerId" />
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
    <asp:SqlDataSource ID="SqlDataSourceOrderChild" runat="server" ConnectionString="<%$ ConnectionStrings:LocalConnectionString %>" SelectCommand="SELECT [Order].Id, [Order].Number, [User].Fname, [Order].Amount, Currency.Code, [Order].Quoted, [Order].CryptoAddress, [Order].Name, [Order].Email, [Order].IP, OrderStatus.Text AS Status, [Order].Note, [Order].Rate,  (CASE WHEN [Order].CouponId is NULL THEN [Order].OurFee ELSE ([Order].OurFee *(1 - (CONVERT(DECIMAL(15,2),[Coupon].Discount))/100)) END) AS OurFee, [Order].CardNumber, [Order].SiteId, SUBSTRING([Order].CountryCode, 1, 5) AS BrowserCulture, [Order].TxSecret, [Order].PartnerId, [Order].MinersFee, [Order].CouponId, [Coupon].Discount as CouponValue, [Coupon].CouponCode as CouponCode  FROM Currency INNER JOIN [Order] ON Currency.Id = [Order].CurrencyId INNER JOIN OrderStatus ON [Order].Status = OrderStatus.Id LEFT JOIN Coupon on [Order].CouponId = Coupon.Id INNER JOIN [User] ON [Order].UserId = [User].Id INNER JOIN [Order] AS Order_1 ON [User].Id = Order_1.UserId WHERE (Order_1.Id = @Id) ORDER BY [Order].Quoted DESC">
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
                        <asp:Label ID="Label1" runat="server" Text='<%# Bind("Message")%>' Height="16px" Width="1000" CssClass="verdana10"></asp:Label>
                        <%--<asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("Message") %>' ReadOnly="True" TextMode="MultiLine" Width="450px"></asp:TextBox>--%>
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
    <asp:SqlDataSource ID="SqlDataSourceOrder" runat="server" ConnectionString="<%$ ConnectionStrings:LocalConnectionString %>" SelectCommand="SELECT [User].Tier,[User].UserRiskLevel ,[Order].Id, [Order].Number, [User].Fname,[Order].Amount, Currency.Code, [Order].Quoted, [Order].CryptoAddress, [Order].Name, [Order].Email, [Order].IP, [Order].Note, [Order].SiteId, OrderStatus.Text AS Status, Country.Code AS CountryCode FROM Currency INNER JOIN [Order] ON Currency.Id = [Order].CurrencyId INNER JOIN OrderStatus ON [Order].Status = OrderStatus.Id INNER JOIN [User] ON [Order].UserId = [User].Id INNER JOIN Country ON [User].CountryId = Country.Id WHERE (OrderStatus.Text = @Status) ORDER BY [Order].Quoted DESC" UpdateCommand="UPDATE [Order] SET Note = @Note WHERE (Id = @Id)">
        <SelectParameters>
            <asp:SessionParameter Name="Status" SessionField="ComplianceView" />
        </SelectParameters>
        <UpdateParameters>
            <asp:Parameter Name="Note" />
            <asp:Parameter Name="Id" />
        </UpdateParameters>
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="SqlDataSourceOrder1" runat="server" ConnectionString="<%$ ConnectionStrings:LocalConnectionString %>" SelectCommand="SELECT DISTINCT [User].Tier, UserRiskLevelType.Text AS UserRiskLevel, [Order].Id, [Order].Number, [User].Fname,[User].Id AS UserId,[Order].Type, [Order].Amount, Currency.Code, [Order].Quoted, [Order].CryptoAddress, [Order].Name, [Order].Email, [Order].IP, [Order].Note, [Order].SiteId, OrderStatus.Text AS Status, Country.Code AS CountryCode FROM Currency INNER JOIN [Order] ON Currency.Id = [Order].CurrencyId  INNER JOIN OrderStatus ON (OrderStatus.Id = [Order].Status)  INNER JOIN OrderType ON ([Order].Type = 1 OR [Order].Type=2) INNER JOIN [User] ON [Order].UserId = [User].Id INNER JOIN Country ON [User].CountryId = Country.Id INNER JOIN UserRiskLevelType ON (UserRiskLevelType.Id = [User].UserRiskLevel) WHERE ([User].Tier = 3 AND [Order].Status = 3) OR  ([Order].Status = 26) OR ([User].UserRiskLevel = 2 AND [Order].Status = 7 ) ORDER BY [Order].Quoted DESC" UpdateCommand="UPDATE [Order] SET Note = @Note WHERE (Id = @Id)">
        <SelectParameters>
            <asp:SessionParameter Name="Status" SessionField="ComplianceView" />
        </SelectParameters>
        <UpdateParameters>
            <asp:Parameter Name="Note" />
            <asp:Parameter Name="Id" />
        </UpdateParameters>
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="SqlDataSourceOrder2" runat="server" ConnectionString="<%$ ConnectionStrings:LocalConnectionString %>" SelectCommand="SELECT [User].Tier,UserRiskLevelType.Text AS UserRiskLevel , [Order].Id, [Order].Number,[User].Fname, [Order].Amount, Currency.Code, [Order].Quoted, [Order].CryptoAddress, [Order].Name, [Order].Email, [Order].IP, [Order].Note, [Order].SiteId, OrderStatus.Text AS Status, Country.Code AS CountryCode FROM Currency INNER JOIN [Order] ON Currency.Id = [Order].CurrencyId INNER JOIN OrderStatus ON [Order].Status = OrderStatus.Id INNER JOIN [User] ON [Order].UserId = [User].Id INNER JOIN Country ON [User].CountryId = Country.Id  INNER JOIN UserRiskLevelType ON (UserRiskLevelType.Id = [User].UserRiskLevel) WHERE (OrderStatus.Text = 'Compliance Officer Approval') OR ([User].UserRiskLevel = 1 AND[Order].Status = 7) ORDER BY [Order].Quoted DESC" UpdateCommand="UPDATE [Order] SET Note = @Note WHERE (Id = @Id)">
        <SelectParameters>
            <asp:SessionParameter Name="Status" SessionField="ComplianceView" />
        </SelectParameters>
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
    <asp:SqlDataSource ID="SqlDataSourceTransaction" runat="server" ConnectionString="<%$ ConnectionStrings:LocalConnectionString %>" SelectCommand="SELECT [Transaction].ExtRef, [Transaction].Amount, [Transaction].Currency, [Transaction].Info, Currency.Code FROM [Transaction] INNER JOIN Currency ON [Transaction].Currency = Currency.Id WHERE ([Transaction].OrderId = @OrderId)">
        <SelectParameters>
            <asp:ControlParameter ControlID="GridViewOrder" Name="OrderId" PropertyName="SelectedValue" />
        </SelectParameters>
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

    <script type="text/javascript">
        /* The Modal (background) */
        // Get the modal
        var modal = document.getElementById('myModal');

        // Get the button that opens the modal
        var btn = document.getElementById("<%=editImageBtn.ClientID %>");

        // Get the <span> element that closes the modal
        var span = document.getElementsByClassName("close")[0];
        var saveImageTrans = document.getElementById("saveImageTrans");

        var rotateLeftButton = document.getElementById("rotateLeftButton");
        var rotateRightButton = document.getElementById("rotateRightButton");
        var flipButton = document.getElementById("flipButton");

        var imageId = document.getElementById("<%=imageId.ClientID %>");
        var imageIDToTransform = document.getElementById('imageIDToTransform');
        var imageToTransform = document.getElementById('imageToTransform');
        var isFlipped = document.getElementById('flipped');
        var angleNow = document.getElementById('angle');

        // When the user clicks the button, open the modal
        if (btn) btn.onclick = function (event) {
            editImageInPopup();
        };
        function editImageInPopup() {
            modal.style.display = "block";
            imageIDToTransform.value = imageId.value;
            imageToTransform.src = 'KYCImageHandler.ashx?ImageId=' + imageId.value + "&time=" + new Date().getTime();
            ResetValues();
        }
        if (saveImageTrans) {
            saveImageTrans.onclick = function (event) {
                saveImage();
            };
        }

        function ResetValues() {
            if (isFlipped.value === "true") {
                flip();
            }
            if (angleNow.value && angleNow.value != "0") {
                rotate(-parseInt(angleNow.value));
            }

            isFlipped.value = false;
            angleNow.value = 0;
        }

        // When the user clicks on <span> (x), close the modal
        if (span) span.onclick = function () {
            resetTransform();
        }

        // When the user clicks anywhere outside of the modal, close it
        window.onclick = function (event) {
            if (event.target == modal) {
                resetTransform();
            }
        }
        /* The Modal (background) */
        /*Transform panel*/
        if (rotateLeftButton) rotateLeftButton.onclick = function (event) {
            rotate(-90);
        };
        if (rotateRightButton) rotateRightButton.onclick = function (event) {
            rotate(90);
        };
        if (flipButton) flipButton.onclick = function (event) {
            flip();
        };
        function resetTransform() {
            modal.style.display = "none";
            imageToTransform.src = '';
            ResetValues();
        }
        function rotate(angle) {
            var FRAME = $('#frame');
            var IMAGE = FRAME.find('img');
            // var isFlipped = FRAME.data('flipped');
            if (isFlipped.value && isFlipped.value == "true") angle *= -1;
            // var angleNow = Number(FRAME.data('angle'));
            var newAngleValue = parseInt(angleNow.value) + angle;
            // FRAME.data('angle', newAngle);
            angleNow.value = newAngleValue;
            IMAGE.css('transform', 'rotate(' + newAngleValue + 'deg)');
        }

        function flip() {
            // var FRAME = document.getElementById('frame');
            // var isFlipped = FRAME.data('flipped');
            // FRAME.data('flipped', !isFlipped);
            isFlipped.value = isFlipped.value == "true" ? false : true;
            if (isFlipped.value != "true") $('#imageToTransform').css('transform', 'rotateY(0deg)');
            else $('#imageToTransform').css('transform', 'rotateY(180deg)');
        }

        function saveImage() {
            var updateImageUrl = 'KYCImageHandler.ashx?edit=true&ImageId=' + imageIDToTransform.value + '&flipped=' + isFlipped.value + '&angle=' + parseInt(angleNow.value).toString() + "&time=" + new Date().getTime();
            // alert(updateImageUrl);
            $.get(updateImageUrl, function (data, status) {
                var imagePreview = document.getElementById("<%=imagePreview.ClientID %>");
                imagePreview.src = 'KYCImageHandler.ashx?ImageId=' + imageId.value + "&time=" + new Date().getTime();
                resetTransform();
            });
        }
        /*Transform panel*/
    </script>

</asp:Content>

