<%@ Page Title="" Language="VB" MasterPageFile="~/MasterPageAdmin.master" AutoEventWireup="false" CodeFile="User.aspx.vb" Inherits="User" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:LinkButton ID="LinkButtonBACK" runat="server" PostBackUrl="~/Default.aspx">Back</asp:LinkButton>
    <asp:Button ID="Button1" runat="server"  Height="18px" Visible="false" Text="Clear" Width="65px"  style="float:right"/>
    <br />
    <br />
    <asp:GridView ID="GridViewUser" runat="server" AllowPaging="True" AllowSorting="True" AutoGenerateColumns="False" DataKeyNames="Id" DataSourceID="SqlDataSourceUser" BackColor="White" BorderColor="#336666" BorderStyle="Double" BorderWidth="3px" CellPadding="4" EmptyDataText="No User found!!" GridLines="Horizontal" Width="100%">
        <Columns>

            <asp:CommandField ShowSelectButton="True" />
           <asp:TemplateField > 
                    <ItemTemplate>  
                        <asp:Button ID="btn_Edit" runat="server" Text="Edit" CommandName="Edit" />  
                    </ItemTemplate>
                    <EditItemTemplate>                
                        <asp:Button ID="btn_Update" runat="server" Text="Update" CommandName="Update" CommandArgument='<%# DataBinder.Eval(Container, "RowIndex") %>'/>
                        <asp:Button ID="btn_Cancel" runat="server" Text="Cancel" CommandName="Cancel" CommandArgument='<%# DataBinder.Eval(Container, "RowIndex") %>'/>                        
                    </EditItemTemplate>
                  <ItemStyle Width="120px" />
                </asp:TemplateField>
            <asp:BoundField DataField="Id" HeaderText="Id"  InsertVisible="False" ReadOnly="True" />
            <asp:BoundField DataField="Fname" HeaderText="Name"   ReadOnly="True"/>
            <asp:BoundField DataField="Phone" HeaderText="Phone"   ReadOnly="True" />
            <asp:BoundField DataField="Email" HeaderText="Email"   ReadOnly="True" />
            <%--<asp:BoundField DataField="Amount" HeaderText="Amount" SortExpression="Amount" DataFormatString="{0:0,0.00}" ReadOnly="True" />
                            <asp:BoundField DataField="Code" HeaderText="." SortExpression="Code" ReadOnly="True" />--%>
            <asp:BoundField DataField="KycNote" HeaderText="KycNote"  ReadOnly="True" />
            <asp:BoundField DataField="Created" HeaderText="Created"  ReadOnly="True"></asp:BoundField>
            <asp:BoundField DataField="LanguageId" HeaderText="LanguageId" SortExpression="LanguageId" ReadOnly="True" Visible="false"></asp:BoundField>
            <asp:BoundField DataField="Trusted" HeaderText="Trusted"   ReadOnly="True" />
            <asp:BoundField DataField="TrustedBy" HeaderText="TrustedBy"   ReadOnly="True"/>
            <asp:BoundField DataField="CountryId" HeaderText="CountryId"   ReadOnly="True" Visible="false"/>
            <asp:BoundField DataField="CountryCode" HeaderText="CountryCode"   ReadOnly="True" />
            <asp:BoundField DataField="Tier" HeaderText="Tier"   ReadOnly="True"/>
            <asp:TemplateField HeaderText="TransactionLimits  Details">
                 <EditItemTemplate>
                        <asp:TextBox ID="TextBox2" runat="server" Text='<%# Bind("TransactionLimitsDetails") %>' TextMode="MultiLine" Height="60px"  Width="300px" style="overflow-y:auto;overflow-x:auto; word-break:break-all;"></asp:TextBox>
                    </EditItemTemplate>
                    <ItemTemplate>
                        <asp:Label ID="Label1" runat="server" Text='<%# Bind("TransactionLimitsDetails") %>' Width="300px" style="overflow-y:auto;overflow-x:auto; word-break:break-all;"></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
            <asp:TemplateField HeaderText="CreditCardLimits  Details">
                 <EditItemTemplate>
                        <asp:TextBox ID="TextBox3" runat="server" Text='<%# Bind("CreditCardLimitsDetails") %>' TextMode="MultiLine" Height="60px"  Width="300px" style="overflow-y:auto;overflow-x:auto; word-break:break-all;"></asp:TextBox>
                    </EditItemTemplate>
                    <ItemTemplate>
                        <asp:Label ID="Label2" runat="server" Text='<%# Bind("CreditCardLimitsDetails") %>' Width="300px" style="overflow-y:auto;overflow-x:auto; word-break:break-all;"></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
            <asp:TemplateField HeaderText="Date of Birth">
                <EditItemTemplate>
                        <asp:TextBox ID="TextBoxDOB" runat="server" Text='<%# Bind("DateOfBirth") %>' TextMode="MultiLine" Height="60px"  Width="300px" style="overflow-y:auto;overflow-x:auto; word-break:break-all;"></asp:TextBox>
                    </EditItemTemplate>
                    <ItemTemplate>
                        <asp:Label ID="LabelDOB" runat="server" Text='<%# Bind("DateOfBirth") %>' Width="300px" style="overflow-y:auto;overflow-x:auto; word-break:break-all;"></asp:Label>
                    </ItemTemplate>
                
            </asp:TemplateField>
            <%--<asp:BoundField DataField="UserRiskLevel" HeaderText="UserRiskLevel" SortExpression="UserRiskLevel" />--%>   
            <%--<asp:TemplateField HeaderText="CustomerID" SortExpression="CustomerID">
                PaymentMethodDetails
                <EditItemTemplate>
                        <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("CustomerID") %>'></asp:TextBox>
                    </EditItemTemplate>
                <HeaderTemplate>
                    <asp:DropDownList ID="DropDownList1" runat="server" AutoPostBack="True"
                            DataSourceID="SqlDataSource2" DataTextField="CompanyName"
                            DataValueField="CustomerID">
                    </asp:DropDownList>
                       <asp:SqlDataSource ID="SqlDataSource2" runat="server"
                           ConnectionString="<%$ ConnectionStrings:LocalConnectionString %>"
                            SelectCommand="SELECT [CustomerID], [CompanyName] FROM [Customers]">
                        </asp:SqlDataSource>
                </HeaderTemplate>
            </asp:TemplateField>--%>
            <asp:TemplateField HeaderText="User RiskLevel">  
                
                    <ItemTemplate>  
                        <asp:DropDownList ID="DropDownListRiskLevel" runat="server" AutoPostBack="True"
                            DataSourceID="SqlDataSource2" DataTextField="Text"
                            DataValueField="Id" OnSelectedIndexChanged = "SelectedIndexChanged" >  
                        </asp:DropDownList>  
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
    <asp:SqlDataSource ID="SqlDataSourceUser" runat="server" ConnectionString="<%$ ConnectionStrings:LocalConnectionString %>" SelectCommand="SELECT [User].Id, Fname, Phone, Email, KycNote, Created, LanguageId, Trusted, TrustedBy, CountryId, Tier, UserRiskLevel, TransactionLimitsDetails ,CreditCardLimitsDetails,DateOfBirth, Country.Code AS CountryCode FROM Country INNER JOIN [User] ON [Country].Id = [User].CountryId  ORDER BY KycNote DESC"  UpdateCommand="UPDATE [User] SET TransactionLimitsDetails = @TransactionLimitsDetails, CreditCardLimitsDetails =@CreditCardLimitsDetails,DateOfBirth = @DateOfBirth WHERE (Id = @Id)">
        <UpdateParameters>
            <asp:Parameter Name="TransactionLimitsDetails" />
            <asp:Parameter Name="CreditCardLimitsDetails" />
            <asp:Parameter Name="DateOfBirth" />
            <asp:Parameter Name="Id" />
        </UpdateParameters>
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="SqlDataSource2" runat="server" ConnectionString="<%$ ConnectionStrings:LocalConnectionString %>" SelectCommand="SELECT [UserRiskLevelType].Id,[UserRiskLevelType].Text FROM [UserRiskLevelType]"></asp:SqlDataSource>
    <br />
    <asp:Panel ID="PanelTrustCalculation" runat="server" Visible="False">
        <table bgcolor="White" style="border-style: solid; border-width: 1px; width: 100%;">
            <tr>
                <td style="padding: 7px; margin: 5px; text-align: center;">ORDERS</td>
                <td style="padding: 7px; margin: 5px; text-align: center;">USER</td>
            </tr>
            <tr>
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
                            <td>
                                <asp:CheckBox ID="CheckBoxIsUserTrusted" runat="server" AutoPostBack="True" Text="Trusted User" />
                            </td>
                            <td>&nbsp;</td>
                        </tr>
                        <tr>
                            <td colspan="2" class="auto-style1">
                                <asp:Label ID="LabelOrigin" runat="server" Text="Label"></asp:Label></td>                            
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
                                <asp:Label ID="LabelKYCDeclined" runat="server" Font-Bold="True" Text="Label" Width="55px"></asp:Label></td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
    </asp:Panel>
    <p class="verdana13">
        Orders by the User:
    </p>
    <asp:GridView ID="GridViewSubOrders" runat="server" AutoGenerateColumns="False" DataKeyNames="Id" DataSourceID="SqlDataSourceOrderChild" EmptyDataText="No record" BackColor="White" BorderColor="#CCCCCC" BorderStyle="None" BorderWidth="1px" CellPadding="3" AllowSorting="True">
        <Columns>
            <asp:BoundField DataField="Id" HeaderText="Id" InsertVisible="False" ReadOnly="True" SortExpression="Id" />
            <asp:BoundField DataField="UserId" HeaderText="UserId" SortExpression="UserId" />
            <asp:BoundField DataField="Number" HeaderText="Number" SortExpression="Number" />
            <asp:BoundField DataField="Amount" HeaderText="Amount" SortExpression="Amount" />
            <asp:BoundField DataField="Code" HeaderText="Code" SortExpression="Code" />
            <asp:BoundField DataField="Quoted" HeaderText="Quoted" SortExpression="Quoted" />
            <asp:BoundField DataField="CryptoAddress" HeaderText="CryptoAddress" SortExpression="CryptoAddress" />
            <asp:BoundField DataField="Name" HeaderText="Name" SortExpression="Name" />
            <asp:BoundField DataField="Email" HeaderText="Email" SortExpression="Email" />
            <asp:BoundField DataField="IP" HeaderText="IP" SortExpression="IP" />
            <asp:BoundField DataField="Status" HeaderText="Status" SortExpression="Status" />
            <asp:BoundField DataField="Note" HeaderText="Note" SortExpression="Note" />
            <asp:BoundField DataField="Rate" HeaderText="Rate" SortExpression="Rate" />
            <asp:BoundField DataField="CommissionProcent" HeaderText="CommissionProcent" SortExpression="CommissionProcent" />
            <asp:BoundField DataField="CardNumber" HeaderText="CardNumber" SortExpression="CardNumber" />
            <asp:BoundField DataField="SiteId" HeaderText="SiteId" SortExpression="SiteId" />
            <asp:BoundField DataField="BrowserCulture" HeaderText="BrowserCulture" ReadOnly="True" SortExpression="BrowserCulture" />
            <asp:BoundField DataField="TxSecret" HeaderText="TxSecret" SortExpression="TxSecret" />
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
    <p class="verdana13">
        User:
    </p>
    <p>
        <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" DataKeyNames="Id" DataSourceID="SqlDataSource3" EmptyDataText="No record" BackColor="#DEBA84" BorderColor="#DEBA84" BorderStyle="None" BorderWidth="1px" CellPadding="3" CellSpacing="2">
            <Columns>
                <asp:CommandField ShowEditButton="True" />
                <asp:BoundField DataField="Id" HeaderText="User Id" InsertVisible="False" ReadOnly="True" SortExpression="Id" />
                <asp:BoundField DataField="Fname" HeaderText="Name" SortExpression="Fname" ReadOnly="True" />
                <asp:BoundField DataField="Phone" HeaderText="Phone" SortExpression="Phone" ReadOnly="True" />
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
                <asp:TemplateField HeaderText="SellPaymentMethodDetails" SortExpression="SellPaymentMethodDetails">
                    <EditItemTemplate>
                        <asp:TextBox ID="TextBox3" runat="server" Text='<%# Bind("SellPaymentMethodDetails") %>' TextMode="MultiLine" Height="237px" Width="531px"></asp:TextBox>
                    </EditItemTemplate>
                    <ItemTemplate>
                        <asp:Label ID="Label3" runat="server" Text='<%# Bind("SellPaymentMethodDetails") %>'></asp:Label>
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
        <asp:SqlDataSource ID="SqlDataSource3" runat="server" ConnectionString="<%$ ConnectionStrings:LocalConnectionString %>" SelectCommand="SELECT DISTINCT [User].Id, [User].Fname, [User].Phone, [User].Email, Country.Text AS CountryCode, [User].KycNote, [User].Created, [User].Commission, [User].PaymentMethodDetails ,[User].SellPaymentMethodDetails FROM [User] INNER JOIN [Order] ON [User].Id = [Order].UserId LEFT OUTER JOIN Country ON [User].CountryId = Country.Id WHERE ([Order].UserId = @Id)" UpdateCommand="UPDATE [User] SET PaymentMethodDetails = @PaymentMethodDetails ,SellPaymentMethodDetails=@SellPaymentMethodDetails WHERE (Id = @Id)">
            <SelectParameters>
                <asp:ControlParameter ControlID="GridViewUser" Name="Id" PropertyName="SelectedValue" />
            </SelectParameters>
            <UpdateParameters>
                <asp:Parameter Name="PaymentMethodDetails" />
                <asp:Parameter Name="SellPaymentMethodDetails" />
                <asp:Parameter Name="Id" />
            </UpdateParameters>
        </asp:SqlDataSource>
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
                        <asp:Label ID="TextBox1" runat="server" Text='<%# Bind("Message") %>' ReadOnly="True" TextMode="MultiLine" style="word-wrap:break-word;" ></asp:Label>
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
      <asp:SqlDataSource ID="SqlDataSourceAuditTrail" runat="server" ConnectionString="<%$ ConnectionStrings:LocalConnectionString %>" SelectCommand="SELECT AuditTrail.OrderId, AuditTrail.Status, AuditTrail.Message, AuditTrail.Created, AuditTrailStatus.Text FROM AuditTrail INNER JOIN AuditTrailStatus ON AuditTrail.Status = AuditTrailStatus.Id WHERE AuditTrail.Message LIKE 'CUSTOMER:'+  CAST(@Id AS NVARCHAR(10)) + '%' ORDER BY AuditTrail.Created">
        <SelectParameters>
            <asp:ControlParameter ControlID="GridViewUser" Name="Id" PropertyName="SelectedValue" />
        </SelectParameters>
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="SqlDataSourceOrderChild" runat="server" ConnectionString="<%$ ConnectionStrings:LocalConnectionString %>" SelectCommand="SELECT [Order].Id, [Order].UserId, [Order].Number, [Order].Amount, Currency.Code, [Order].Quoted, [Order].CryptoAddress, [Order].Name, [Order].Email, [Order].IP, OrderStatus.Text AS Status, [Order].Note, [Order].Rate, [Order].CommissionProcent, [Order].CardNumber, [Order].SiteId, SUBSTRING([Order].CountryCode, 1, 5) AS BrowserCulture, [Order].TxSecret FROM Currency INNER JOIN [Order] ON Currency.Id = [Order].CurrencyId INNER JOIN OrderStatus ON [Order].Status = OrderStatus.Id WHERE ([Order].UserId = @UserId) ORDER BY [Order].Quoted DESC">
        <SelectParameters>
            <asp:ControlParameter ControlID="GridViewUser" Name="UserId" PropertyName="SelectedValue" />
        </SelectParameters>
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:LocalConnectionString %>" SelectCommand="SELECT [User].Id, [User].Fname, [User].Phone, [User].Email, Country.Text AS CountryCode, [User].KycNote, [User].Created, [Order].CommissionProcent, [User].PaymentMethodDetails FROM [User] INNER JOIN [Order] ON [User].Id = [Order].UserId LEFT OUTER JOIN Country ON [User].CountryId = Country.Id WHERE ([Order].Id = @Id)">
        <SelectParameters>
            <asp:ControlParameter ControlID="GridViewUser" Name="Id" PropertyName="SelectedValue" />
        </SelectParameters>
    </asp:SqlDataSource>
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
    <asp:SqlDataSource ID="SqlDataSourceKycFileAll" runat="server" ConnectionString="<%$ ConnectionStrings:LocalConnectionString %>" DeleteCommand="DELETE FROM [KycFile] WHERE [Id] = @Id" InsertCommand="INSERT INTO [KycFile] ([Note], [Uploaded], [Rejected], [RejectedBy], [Approved], [ApprovedBy]) VALUES (@Note, @Uploaded, @Rejected, @RejectedBy, @Approved, @ApprovedBy)" SelectCommand="SELECT DISTINCT KycFile.Id, KycFile.UserId,NULL as [File] , KycFile.Note, KycFile.Uploaded, KycFile.Rejected, KycFile.RejectedBy, KycFile.Approved, KycFile.ApprovedBy, KycType.Text, KycFile.Obsolete, KycFile.OriginalFilename, KycFile.Type,  KycFile.UniqueFilename, User_1.Lname AS ApprovedByName, [User].Lname AS RejectedByName FROM KycFile INNER JOIN KycType ON KycFile.Type = KycType.Id INNER JOIN [Order] ON KycFile.UserId = [Order].UserId LEFT OUTER JOIN [User] ON KycFile.RejectedBy = [User].Id LEFT OUTER JOIN [User] AS User_1 ON KycFile.ApprovedBy = User_1.Id WHERE ([Order].UserId = @Id) ORDER BY KycFile.Uploaded DESC" UpdateCommand="UPDATE KycFile SET Note = @Note, Type = @Type WHERE (Id = @Id)">
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
            <asp:ControlParameter ControlID="GridViewUser" Name="Id" PropertyName="SelectedValue" />
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
            <asp:Panel ID="Popup" runat="server" CssClass="panelStyle" >
<h1>KYC File</h1>
                <asp:Image ID="ImageDoc" runat="server" Width="300px" />
   <img runat="server" id="imagePreview" style="max-width: 400px;" visible="false" />
    <div>
    <asp:Literal ID="ltEmbed" Visible="false" runat="server" />
    </div>
    <input runat="server" type="hidden" id="imageId" value="0" />
    <br />
                <br />
                     <asp:Button ID="btnCancel" runat="server" Text="Close" Style="float:right" OnClick="btncancel_Click" CausesValidation="false" />
            </asp:Panel>
            <asp:ModalPopupExtender ID="ModalPopupExtender1" runat="server" TargetControlID="btnfakecancel"
                PopupControlID="Popup" CancelControlID="btnfakecancel" BackgroundCssClass="modalBackground"
>
    </asp:ModalPopupExtender>
                <asp:Button ID="btnfakecancel" Style="display: none;"  runat="server" />
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
        <table style="width: 100%;">
        <tr>
             <td>
                <p class="verdana13">Note:</p>
            </td>
            <td>&nbsp;</td>
            <td style="text-align: left">
                <asp:Panel ID="PanelNote" runat="server" Visible="True">
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
       </table>

</asp:Content>
