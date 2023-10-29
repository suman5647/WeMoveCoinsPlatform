<%@ Page Language="VB" AutoEventWireup="false" Inherits="WMC.Admin.Web.AdminDomainUser"  Codebehind="Domain.aspx.vb" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link href="StyleSheets/TGM_Styles.css" rel="stylesheet" type="text/css" />
    <title></title>
   
</head>
<body>

    <form id="form1" runat="server">
        <asp:LinkButton ID="LinkButtonBACK" runat="server" PostBackUrl="~/Default.aspx">Back</asp:LinkButton>
        <table style="width: 100%;" border="1">            
             <tr>
                <td >                   
                    Hard delete order:
                    <asp:TextBox ID="TextBoxOrderId" runat="server"></asp:TextBox>&nbsp;<asp:Button ID="ButtonDeleteOrder" runat="server" Text="WARNING: Delete Order" />                   
                </td>
                <td>                    
                </td>
            </tr>
            <tr>
                <td > 
                    <br />
                    <br />
                    <asp:Label ID="Label6" runat="server" Text="Currency"></asp:Label>
                    <asp:GridView ID="GridView6" runat="server" AutoGenerateColumns="False" CellPadding="4" DataKeyNames="Id" DataSourceID="SqlDataSourceCurrency" ForeColor="#333333" GridLines="None">
                        <AlternatingRowStyle BackColor="White" />
                        <Columns>
                            <asp:CommandField ShowEditButton="True" />
                            <asp:BoundField DataField="Id" HeaderText="Id" InsertVisible="False" ReadOnly="True" SortExpression="Id" />
                            <asp:BoundField DataField="Code" HeaderText="Code" SortExpression="Code" />
                            <asp:BoundField DataField="CurrencyTypeId" HeaderText="CurrencyTypeId" SortExpression="CurrencyTypeId" />
                            <asp:BoundField DataField="Text" HeaderText="Text" SortExpression="Text" />
                            <asp:BoundField DataField="YourPayCurrencyCode" HeaderText="YourPayCurrencyCode" SortExpression="YourPayCurrencyCode" />
                            <asp:CommandField ShowDeleteButton="True" />
                        </Columns>
                        <FooterStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
                        <HeaderStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
                        <PagerStyle BackColor="#FFCC66" ForeColor="#333333" HorizontalAlign="Center" />
                        <RowStyle BackColor="#FFFBD6" ForeColor="#333333" />
                        <SelectedRowStyle BackColor="#FFCC66" Font-Bold="True" ForeColor="Navy" />
                    </asp:GridView>
                </td>
                <td>
                    <asp:DetailsView ID="DetailsView6" runat="server" AutoGenerateRows="False" CellPadding="4" DataKeyNames="Id" DataSourceID="SqlDataSourceCurrency" DefaultMode="Insert" ForeColor="#333333" GridLines="None" Height="50px" Width="125px">
                        <AlternatingRowStyle BackColor="White" />
                        <CommandRowStyle BackColor="#FFFFC0" Font-Bold="True" />
                        <FieldHeaderStyle BackColor="#FFFF99" Font-Bold="True" />
                        <Fields>
                            <asp:BoundField DataField="Id" HeaderText="Id" SortExpression="Id" InsertVisible="False" ReadOnly="True" />
                            <asp:BoundField DataField="Code" HeaderText="Code" SortExpression="Code" />
                            <asp:BoundField DataField="Text" HeaderText="Text" SortExpression="Text" />
                            <asp:BoundField DataField="YourPayCurrencyCode" HeaderText="YourPayCurrencyCode" SortExpression="YourPayCurrencyCode" />
                            <asp:CommandField ShowInsertButton="True" />
                        </Fields>
                        <FooterStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
                        <HeaderStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
                        <PagerStyle BackColor="#FFCC66" ForeColor="#333333" HorizontalAlign="Center" />
                        <RowStyle BackColor="#FFFBD6" ForeColor="#333333" />
                    </asp:DetailsView>
                </td>
            </tr>
            <tr>
                <td >                    
                    <asp:Label ID="Label4" runat="server" Text="Country"></asp:Label>
                    <asp:GridView ID="GridView4" runat="server" AutoGenerateColumns="False" CellPadding="4" DataKeyNames="Id" DataSourceID="SqlDataSourceCountry" EnableModelValidation="True" ForeColor="#333333" GridLines="None">
                        <AlternatingRowStyle BackColor="White" />
                        <Columns>
                            <asp:CommandField ShowEditButton="True" />
                            <asp:BoundField DataField="Id" HeaderText="Id" InsertVisible="False" ReadOnly="True" SortExpression="Id" />
                            <asp:BoundField DataField="Code" HeaderText="Code" SortExpression="Code" />
                            <asp:BoundField DataField="Text" HeaderText="Text" SortExpression="Text" />
                            <asp:BoundField DataField="PhoneCode" HeaderText="PhoneCode" SortExpression="PhoneCode" />
                            <asp:BoundField DataField="CurrencyId" HeaderText="CurrencyId" SortExpression="CurrencyId" />
                            <asp:BoundField DataField="PhoneNumberStyle" HeaderText="PhoneNumberStyle" SortExpression="PhoneNumberStyle" />
                            <asp:BoundField DataField="CultureCode" HeaderText="CultureCode" SortExpression="CultureCode" />
                            <asp:CommandField ShowDeleteButton="True" />
                        </Columns>
                        <FooterStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
                        <HeaderStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
                        <PagerStyle BackColor="#FFCC66" ForeColor="#333333" HorizontalAlign="Center" />
                        <RowStyle BackColor="#FFFBD6" ForeColor="#333333" />
                        <SelectedRowStyle BackColor="#FFCC66" Font-Bold="True" ForeColor="Navy" />
                    </asp:GridView>
                </td>
                <td>
                    <asp:DetailsView ID="DetailsView4" runat="server" AutoGenerateRows="False" CellPadding="4" DataKeyNames="Id" DataSourceID="SqlDataSourceCountry" DefaultMode="Insert" EnableModelValidation="True" ForeColor="#333333" GridLines="None" Height="50px" Width="125px">
                        <AlternatingRowStyle BackColor="White" />
                        <CommandRowStyle BackColor="#FFFFC0" Font-Bold="True" />
                        <FieldHeaderStyle BackColor="#FFFF99" Font-Bold="True" />
                        <Fields>
                            <asp:BoundField DataField="Id" HeaderText="Id" SortExpression="Id" InsertVisible="False" ReadOnly="True" />
                            <asp:BoundField DataField="Code" HeaderText="Code" SortExpression="Code" />
                            <asp:BoundField DataField="Text" HeaderText="Text" SortExpression="Text" />
                            <asp:BoundField DataField="PhoneCode" HeaderText="PhoneCode" SortExpression="PhoneCode" />
                            <asp:BoundField DataField="CurrencyId" HeaderText="CurrencyId" SortExpression="CurrencyId" />
                            <asp:BoundField DataField="PhoneNumberStyle" HeaderText="PhoneNumberStyle" SortExpression="PhoneNumberStyle" />
                            <asp:BoundField DataField="CultureCode" HeaderText="CultureCode" SortExpression="CultureCode" />
                            <asp:CommandField ShowInsertButton="True" />
                        </Fields>
                        <FooterStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
                        <HeaderStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
                        <PagerStyle BackColor="#FFCC66" ForeColor="#333333" HorizontalAlign="Center" />
                        <RowStyle BackColor="#FFFBD6" ForeColor="#333333" />
                    </asp:DetailsView>
                </td>
            </tr>
            <tr>
                <td >
                    <asp:Label ID="Label2" runat="server" Text="UserRole"></asp:Label>
                    <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" CellPadding="4" DataKeyNames="Id" DataSourceID="SqlDataSourceUserRole" EnableModelValidation="True" ForeColor="#333333" GridLines="None">
                        <AlternatingRowStyle BackColor="White" />
                        <Columns>
                            <asp:BoundField DataField="Id" HeaderText="Id" InsertVisible="False" ReadOnly="True" SortExpression="Id" />
                            <asp:BoundField DataField="Text" HeaderText="Text" SortExpression="Text" />
                            <asp:CommandField ShowEditButton="True" />
                        </Columns>
                        <FooterStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
                        <HeaderStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
                        <PagerStyle BackColor="#FFCC66" ForeColor="#333333" HorizontalAlign="Center" />
                        <RowStyle BackColor="#FFFBD6" ForeColor="#333333" />
                        <SelectedRowStyle BackColor="#FFCC66" Font-Bold="True" ForeColor="Navy" />
                    </asp:GridView>
                </td>
                <td>
                    <asp:DetailsView ID="DetailsView1" runat="server" AutoGenerateRows="False" CellPadding="4" DataKeyNames="Id" DataSourceID="SqlDataSourceUserRole" DefaultMode="Insert" EnableModelValidation="True" ForeColor="#333333" GridLines="None" Height="50px" Width="125px">
                        <AlternatingRowStyle BackColor="White" />
                        <CommandRowStyle BackColor="#FFFFC0" Font-Bold="True" />
                        <FieldHeaderStyle BackColor="#FFFF99" Font-Bold="True" />
                        <Fields>
                            <asp:BoundField DataField="Text" HeaderText="Text" SortExpression="Text" />
                            <asp:CommandField ShowCancelButton="False" ShowEditButton="True" ShowInsertButton="True" />
                        </Fields>
                        <FooterStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
                        <HeaderStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
                        <PagerStyle BackColor="#FFCC66" ForeColor="#333333" HorizontalAlign="Center" />
                        <RowStyle BackColor="#FFFBD6" ForeColor="#333333" />
                    </asp:DetailsView>
                </td>
            </tr>
            <tr>
                <td >
                    <asp:Label ID="Label3" runat="server" Text="UserType"></asp:Label>
                    <asp:GridView ID="GridView2" runat="server" AutoGenerateColumns="False" CellPadding="4" DataKeyNames="Id" DataSourceID="SqlDataSourceUserType" EnableModelValidation="True" ForeColor="#333333" GridLines="None">
                        <AlternatingRowStyle BackColor="White" />
                        <Columns>
                            <asp:BoundField DataField="Id" HeaderText="Id" InsertVisible="False" ReadOnly="True" SortExpression="Id" />
                            <asp:BoundField DataField="Text" HeaderText="Text" SortExpression="Text" />
                            <asp:CommandField ShowEditButton="True" />
                        </Columns>
                        <FooterStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
                        <HeaderStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
                        <PagerStyle BackColor="#FFCC66" ForeColor="#333333" HorizontalAlign="Center" />
                        <RowStyle BackColor="#FFFBD6" ForeColor="#333333" />
                        <SelectedRowStyle BackColor="#FFCC66" Font-Bold="True" ForeColor="Navy" />
                    </asp:GridView>
                </td>
                <td>

                    <asp:DetailsView ID="DetailsView2" runat="server" AutoGenerateRows="False" CellPadding="4" DataKeyNames="Id" DataSourceID="SqlDataSourceUserType" DefaultMode="Insert" EnableModelValidation="True" ForeColor="#333333" GridLines="None" Height="50px" Width="125px">
                        <AlternatingRowStyle BackColor="White" />
                        <CommandRowStyle BackColor="#FFFFC0" Font-Bold="True" />
                        <FieldHeaderStyle BackColor="#FFFF99" Font-Bold="True" />
                        <Fields>
                            <asp:BoundField DataField="Id" HeaderText="Id" SortExpression="Id" InsertVisible="False" ReadOnly="True" />
                            <asp:BoundField DataField="Text" HeaderText="Text" SortExpression="Text" />
                            <asp:CommandField ShowCancelButton="False" ShowInsertButton="True" />
                        </Fields>
                        <FooterStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
                        <HeaderStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
                        <PagerStyle BackColor="#FFCC66" ForeColor="#333333" HorizontalAlign="Center" />
                        <RowStyle BackColor="#FFFBD6" ForeColor="#333333" />
                    </asp:DetailsView>
                </td>
            </tr>
            <tr>
                <td >
                    <asp:Label ID="Label5" runat="server" Text="Language"></asp:Label>
                    <asp:GridView ID="GridView5" runat="server" AutoGenerateColumns="False" CellPadding="4" DataKeyNames="Id" DataSourceID="SqlDataSourceLanguage" EnableModelValidation="True" ForeColor="#333333" GridLines="None">
                        <AlternatingRowStyle BackColor="White" />
                        <Columns>
                            <asp:CommandField ShowEditButton="True" />
                            <asp:BoundField DataField="Id" HeaderText="Id" InsertVisible="False" ReadOnly="True" SortExpression="Id" />
                            <asp:BoundField DataField="Code" HeaderText="Code" SortExpression="Code" />
                            <asp:BoundField DataField="Text" HeaderText="Text" SortExpression="Text" />
                        </Columns>
                        <FooterStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
                        <HeaderStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
                        <PagerStyle BackColor="#FFCC66" ForeColor="#333333" HorizontalAlign="Center" />
                        <RowStyle BackColor="#FFFBD6" ForeColor="#333333" />
                        <SelectedRowStyle BackColor="#FFCC66" Font-Bold="True" ForeColor="Navy" />
                    </asp:GridView>
                </td>
                <td>
                    <asp:DetailsView ID="DetailsView5" runat="server" AutoGenerateRows="False" CellPadding="4" DataKeyNames="Id" DataSourceID="SqlDataSourceLanguage" DefaultMode="Insert" EnableModelValidation="True" ForeColor="#333333" GridLines="None" Height="50px" Width="125px">
                        <AlternatingRowStyle BackColor="White" />
                        <CommandRowStyle BackColor="#FFFFC0" Font-Bold="True" />
                        <FieldHeaderStyle BackColor="#FFFF99" Font-Bold="True" />
                        <Fields>
                            <asp:BoundField DataField="Id" HeaderText="Id" SortExpression="Id" InsertVisible="False" ReadOnly="True" />
                            <asp:BoundField DataField="Code" HeaderText="Code" SortExpression="Code" />
                            <asp:BoundField DataField="Text" HeaderText="Text" SortExpression="Text" />
                            <asp:CommandField ShowInsertButton="True" />
                        </Fields>
                        <FooterStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
                        <HeaderStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
                        <PagerStyle BackColor="#FFCC66" ForeColor="#333333" HorizontalAlign="Center" />
                        <RowStyle BackColor="#FFFBD6" ForeColor="#333333" />
                    </asp:DetailsView>
                </td>
            </tr>
            <tr>
                <td >
                    <asp:Label ID="Label1" runat="server" Text="Site"></asp:Label>
                    <asp:GridView ID="GridView3" runat="server" AutoGenerateColumns="False" CellPadding="4" DataKeyNames="Id" DataSourceID="SqlDataSourceUserSite" EnableModelValidation="True" ForeColor="#333333" GridLines="None">
                        <AlternatingRowStyle BackColor="White" />
                        <Columns>
                            <asp:CommandField ShowEditButton="True" />
                            <asp:BoundField DataField="Id" HeaderText="Id" InsertVisible="False" ReadOnly="True" SortExpression="Id" />
                            <asp:BoundField DataField="Text" HeaderText="Text" SortExpression="Text" />
                            <asp:BoundField DataField="Url" HeaderText="Url" SortExpression="Url" />
                        </Columns>
                        <FooterStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
                        <HeaderStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
                        <PagerStyle BackColor="#FFCC66" ForeColor="#333333" HorizontalAlign="Center" />
                        <RowStyle BackColor="#FFFBD6" ForeColor="#333333" />
                        <SelectedRowStyle BackColor="#FFCC66" Font-Bold="True" ForeColor="Navy" />
                    </asp:GridView>
                </td>
                <td>
                    <asp:DetailsView ID="DetailsView3" runat="server" AutoGenerateRows="False" CellPadding="4" DataKeyNames="Id" DataSourceID="SqlDataSourceUserSite" DefaultMode="Insert" EnableModelValidation="True" ForeColor="#333333" GridLines="None" Height="50px" Width="125px">
                        <AlternatingRowStyle BackColor="White" />
                        <CommandRowStyle BackColor="#FFFFC0" Font-Bold="True" />
                        <FieldHeaderStyle BackColor="#FFFF99" Font-Bold="True" />
                        <Fields>
                            <asp:BoundField DataField="Id" HeaderText="Id" SortExpression="Id" InsertVisible="False" ReadOnly="True" />
                            <asp:BoundField DataField="Text" HeaderText="Text" SortExpression="Text" />
                            <asp:BoundField DataField="Url" HeaderText="Url" SortExpression="Url" />
                            <asp:CommandField ShowInsertButton="True" />
                        </Fields>
                        <FooterStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
                        <HeaderStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
                        <PagerStyle BackColor="#FFCC66" ForeColor="#333333" HorizontalAlign="Center" />
                        <RowStyle BackColor="#FFFBD6" ForeColor="#333333" />
                    </asp:DetailsView>
                </td>
            </tr>
            
            
            
            <tr>
                <td >
                    <asp:Label ID="Label7" runat="server" Text="KYC Type"></asp:Label>
                    <asp:GridView ID="GridView7" runat="server" AutoGenerateColumns="False" CellPadding="4" DataKeyNames="Id" DataSourceID="SqlDataSourceKycType" EnableModelValidation="True" ForeColor="#333333" GridLines="None">
                        <AlternatingRowStyle BackColor="White" />
                        <Columns>
                            <asp:CommandField ShowEditButton="True" />
                            <asp:BoundField DataField="Id" HeaderText="Id" InsertVisible="False" ReadOnly="True" SortExpression="Id" />
                            <asp:BoundField DataField="Text" HeaderText="Text" SortExpression="Text" />
                        </Columns>
                        <FooterStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
                        <HeaderStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
                        <PagerStyle BackColor="#FFCC66" ForeColor="#333333" HorizontalAlign="Center" />
                        <RowStyle BackColor="#FFFBD6" ForeColor="#333333" />
                        <SelectedRowStyle BackColor="#FFCC66" Font-Bold="True" ForeColor="Navy" />
                    </asp:GridView>
                </td>
                <td>
                    <asp:DetailsView ID="DetailsView7" runat="server" AutoGenerateRows="False" CellPadding="4" DataKeyNames="Id" DataSourceID="SqlDataSourceKycType" DefaultMode="Insert" EnableModelValidation="True" ForeColor="#333333" GridLines="None" Height="50px" Width="125px">
                        <AlternatingRowStyle BackColor="White" />
                        <CommandRowStyle BackColor="#FFFFC0" Font-Bold="True" />
                        <FieldHeaderStyle BackColor="#FFFF99" Font-Bold="True" />
                        <Fields>
                            <asp:BoundField DataField="Id" HeaderText="Id" SortExpression="Id" InsertVisible="False" ReadOnly="True" />
                            <asp:BoundField DataField="Text" HeaderText="Text" SortExpression="Text" />
                            <asp:CommandField ShowInsertButton="True" />
                        </Fields>
                        <FooterStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
                        <HeaderStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
                        <PagerStyle BackColor="#FFCC66" ForeColor="#333333" HorizontalAlign="Center" />
                        <RowStyle BackColor="#FFFBD6" ForeColor="#333333" />
                    </asp:DetailsView>
                </td>
            </tr>
            <tr>
                <td >
                    <asp:Label ID="Label8" runat="server" Text="OrderStatus"></asp:Label>
                    <asp:GridView ID="GridView8" runat="server" AutoGenerateColumns="False" CellPadding="4" DataKeyNames="Id" DataSourceID="SqlDataSourceOrderStatus" EnableModelValidation="True" ForeColor="#333333" GridLines="None">
                        <AlternatingRowStyle BackColor="White" />
                        <Columns>
                            <asp:CommandField ShowEditButton="True" />
                            <asp:BoundField DataField="Id" HeaderText="Id" InsertVisible="False" ReadOnly="True" SortExpression="Id" />
                            <asp:BoundField DataField="Text" HeaderText="Text" SortExpression="Text" />
                        </Columns>
                        <FooterStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
                        <HeaderStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
                        <PagerStyle BackColor="#FFCC66" ForeColor="#333333" HorizontalAlign="Center" />
                        <RowStyle BackColor="#FFFBD6" ForeColor="#333333" />
                        <SelectedRowStyle BackColor="#FFCC66" Font-Bold="True" ForeColor="Navy" />
                    </asp:GridView>
                </td>
                <td>
                    <asp:DetailsView ID="DetailsView8" runat="server" AutoGenerateRows="False" CellPadding="4" DataKeyNames="Id" DataSourceID="SqlDataSourceOrderStatus" DefaultMode="Insert" EnableModelValidation="True" ForeColor="#333333" GridLines="None" Height="50px" Width="125px">
                        <AlternatingRowStyle BackColor="White" />
                        <CommandRowStyle BackColor="#FFFFC0" Font-Bold="True" />
                        <FieldHeaderStyle BackColor="#FFFF99" Font-Bold="True" />
                        <Fields>
                            <asp:BoundField DataField="Id" HeaderText="Id" SortExpression="Id" InsertVisible="False" ReadOnly="True" />
                            <asp:BoundField DataField="Text" HeaderText="Text" SortExpression="Text" />
                            <asp:CommandField ShowInsertButton="True" />
                        </Fields>
                        <FooterStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
                        <HeaderStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
                        <PagerStyle BackColor="#FFCC66" ForeColor="#333333" HorizontalAlign="Center" />
                        <RowStyle BackColor="#FFFBD6" ForeColor="#333333" />
                    </asp:DetailsView>
                </td>
            </tr>
            <tr>
                <td >
                    <asp:Label ID="Label9" runat="server" Text="OrderType"></asp:Label>
                    <asp:GridView ID="GridView9" runat="server" AutoGenerateColumns="False" CellPadding="4" DataKeyNames="Id" DataSourceID="SqlDataSourceOrderType" EnableModelValidation="True" ForeColor="#333333" GridLines="None">
                        <AlternatingRowStyle BackColor="White" />
                        <Columns>
                            <asp:CommandField ShowEditButton="True" />
                            <asp:BoundField DataField="Id" HeaderText="Id" InsertVisible="False" ReadOnly="True" SortExpression="Id" />
                            <asp:BoundField DataField="Text" HeaderText="Text" SortExpression="Text" />
                        </Columns>
                        <FooterStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
                        <HeaderStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
                        <PagerStyle BackColor="#FFCC66" ForeColor="#333333" HorizontalAlign="Center" />
                        <RowStyle BackColor="#FFFBD6" ForeColor="#333333" />
                        <SelectedRowStyle BackColor="#FFCC66" Font-Bold="True" ForeColor="Navy" />
                    </asp:GridView>
                </td>
                <td>
                    <asp:DetailsView ID="DetailsView9" runat="server" AutoGenerateRows="False" CellPadding="4" DataKeyNames="Id" DataSourceID="SqlDataSourceOrderType" DefaultMode="Insert" EnableModelValidation="True" ForeColor="#333333" GridLines="None" Height="50px" Width="125px">
                        <AlternatingRowStyle BackColor="White" />
                        <CommandRowStyle BackColor="#FFFFC0" Font-Bold="True" />
                        <FieldHeaderStyle BackColor="#FFFF99" Font-Bold="True" />
                        <Fields>
                            <asp:BoundField DataField="Id" HeaderText="Id" SortExpression="Id" InsertVisible="False" ReadOnly="True" />
                            <asp:BoundField DataField="Text" HeaderText="Text" SortExpression="Text" />
                            <asp:CommandField ShowInsertButton="True" />
                        </Fields>
                        <FooterStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
                        <HeaderStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
                        <PagerStyle BackColor="#FFCC66" ForeColor="#333333" HorizontalAlign="Center" />
                        <RowStyle BackColor="#FFFBD6" ForeColor="#333333" />
                    </asp:DetailsView>
                </td>
            </tr>
                      
            <tr>
                <td >
                    <asp:Label ID="Label12" runat="server" Text="PaymentType"></asp:Label>
                    <asp:GridView ID="GridView11" runat="server" AutoGenerateColumns="False" CellPadding="4" DataKeyNames="Id" DataSourceID="SqlDataSourcePaymentType" EnableModelValidation="True" ForeColor="#333333" GridLines="None">
                        <AlternatingRowStyle BackColor="White" />
                        <Columns>
                            <asp:CommandField ShowEditButton="True" />
                            <asp:BoundField DataField="Id" HeaderText="Id" InsertVisible="False" ReadOnly="True" SortExpression="Id" />                            
                            <asp:BoundField DataField="Text" HeaderText="Text" SortExpression="Text" />
                        </Columns>
                        <FooterStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
                        <HeaderStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
                        <PagerStyle BackColor="#FFCC66" ForeColor="#333333" HorizontalAlign="Center" />
                        <RowStyle BackColor="#FFFBD6" ForeColor="#333333" />
                        <SelectedRowStyle BackColor="#FFCC66" Font-Bold="True" ForeColor="Navy" />
                    </asp:GridView>
                </td>
                <td>
                    <asp:DetailsView ID="DetailsView12" runat="server" AutoGenerateRows="False" CellPadding="4" DataKeyNames="Id" DataSourceID="SqlDataSourcePaymentType" DefaultMode="Insert" EnableModelValidation="True" ForeColor="#333333" GridLines="None" Height="50px" Width="125px">
                        <AlternatingRowStyle BackColor="White" />
                        <CommandRowStyle BackColor="#FFFFC0" Font-Bold="True" />
                        <FieldHeaderStyle BackColor="#FFFF99" Font-Bold="True" />
                        <Fields>
                            <asp:BoundField DataField="Id" HeaderText="Id" SortExpression="Id" InsertVisible="False" ReadOnly="True" />
                            <asp:BoundField DataField="Code" HeaderText="Code" SortExpression="Code" />
                            <asp:BoundField DataField="Text" HeaderText="Text" SortExpression="Text" />
                            <asp:CommandField ShowInsertButton="True" />
                        </Fields>
                        <FooterStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
                        <HeaderStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
                        <PagerStyle BackColor="#FFCC66" ForeColor="#333333" HorizontalAlign="Center" />
                        <RowStyle BackColor="#FFFBD6" ForeColor="#333333" />
                    </asp:DetailsView>
                </td>
            </tr>
            <tr>
                <td >
                    <asp:Label ID="Label13" runat="server" Text="AuditTrailStatus"></asp:Label>
                    <asp:GridView ID="GridView12" runat="server" AutoGenerateColumns="False" CellPadding="4" DataKeyNames="Id" DataSourceID="SqlDataSourceAuditTrailStatus" EnableModelValidation="True" ForeColor="#333333" GridLines="None">
                        <AlternatingRowStyle BackColor="White" />
                        <Columns>
                            <asp:CommandField ShowEditButton="True" />
                            <asp:BoundField DataField="Id" HeaderText="Id" InsertVisible="False" ReadOnly="True" SortExpression="Id" />
                            <asp:BoundField DataField="Text" HeaderText="Text" SortExpression="Text" />
                        </Columns>
                        <FooterStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
                        <HeaderStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
                        <PagerStyle BackColor="#FFCC66" ForeColor="#333333" HorizontalAlign="Center" />
                        <RowStyle BackColor="#FFFBD6" ForeColor="#333333" />
                        <SelectedRowStyle BackColor="#FFCC66" Font-Bold="True" ForeColor="Navy" />
                    </asp:GridView>
                </td>
                <td>
                    <asp:DetailsView ID="DetailsView13" runat="server" AutoGenerateRows="False" CellPadding="4" DataKeyNames="Id" DataSourceID="SqlDataSourceAuditTrailStatus" DefaultMode="Insert" EnableModelValidation="True" ForeColor="#333333" GridLines="None" Height="50px" Width="125px">
                        <AlternatingRowStyle BackColor="White" />
                        <CommandRowStyle BackColor="#FFFFC0" Font-Bold="True" />
                        <FieldHeaderStyle BackColor="#FFFF99" Font-Bold="True" />
                        <Fields>
                            <asp:BoundField DataField="Id" HeaderText="Id" SortExpression="Id" InsertVisible="False" ReadOnly="True" />
                            <asp:BoundField DataField="Text" HeaderText="Text" SortExpression="Text" />
                            <asp:CommandField ShowInsertButton="True" />
                        </Fields>
                        <FooterStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
                        <HeaderStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
                        <PagerStyle BackColor="#FFCC66" ForeColor="#333333" HorizontalAlign="Center" />
                        <RowStyle BackColor="#FFFBD6" ForeColor="#333333" />
                    </asp:DetailsView>
                </td>
            </tr>
            <tr>
                <td >
                    <asp:Label ID="Label14" runat="server" Text="TransactionMethod"></asp:Label>
                    <asp:GridView ID="GridView13" runat="server" AutoGenerateColumns="False" CellPadding="4" DataKeyNames="Id" DataSourceID="SqlDataSourceTransactionMethod" EnableModelValidation="True" ForeColor="#333333" GridLines="None">
                        <AlternatingRowStyle BackColor="White" />
                        <Columns>
                            <asp:CommandField ShowEditButton="True" />
                            <asp:BoundField DataField="Id" HeaderText="Id" InsertVisible="False" ReadOnly="True" SortExpression="Id" />
                            <asp:BoundField DataField="Text" HeaderText="Text" SortExpression="Text" />
                        </Columns>
                        <FooterStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
                        <HeaderStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
                        <PagerStyle BackColor="#FFCC66" ForeColor="#333333" HorizontalAlign="Center" />
                        <RowStyle BackColor="#FFFBD6" ForeColor="#333333" />
                        <SelectedRowStyle BackColor="#FFCC66" Font-Bold="True" ForeColor="Navy" />
                    </asp:GridView>
                </td>
                <td>
                    <asp:DetailsView ID="DetailsView14" runat="server" AutoGenerateRows="False" CellPadding="4" DataKeyNames="Id" DataSourceID="SqlDataSourceTransactionMethod" DefaultMode="Insert" EnableModelValidation="True" ForeColor="#333333" GridLines="None" Height="50px" Width="125px">
                        <AlternatingRowStyle BackColor="White" />
                        <CommandRowStyle BackColor="#FFFFC0" Font-Bold="True" />
                        <FieldHeaderStyle BackColor="#FFFF99" Font-Bold="True" />
                        <Fields>
                            <asp:BoundField DataField="Id" HeaderText="Id" SortExpression="Id" InsertVisible="False" ReadOnly="True" />
                            <asp:BoundField DataField="Text" HeaderText="Text" SortExpression="Text" />
                            <asp:CommandField ShowInsertButton="True" />
                        </Fields>
                        <FooterStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
                        <HeaderStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
                        <PagerStyle BackColor="#FFCC66" ForeColor="#333333" HorizontalAlign="Center" />
                        <RowStyle BackColor="#FFFBD6" ForeColor="#333333" />
                    </asp:DetailsView>
                </td>
            </tr>
            <tr>
                <td >
                    <asp:Label ID="Label15" runat="server" Text="TransactionType"></asp:Label>
                    <asp:GridView ID="GridView14" runat="server" AutoGenerateColumns="False" CellPadding="4" DataKeyNames="Id" DataSourceID="SqlDataSourceTransactionType" EnableModelValidation="True" ForeColor="#333333" GridLines="None">
                        <AlternatingRowStyle BackColor="White" />
                        <Columns>
                            <asp:CommandField ShowEditButton="True" />
                            <asp:BoundField DataField="Id" HeaderText="Id" InsertVisible="False" ReadOnly="True" SortExpression="Id" />
                            <asp:BoundField DataField="Text" HeaderText="Text" SortExpression="Text" />
                        </Columns>
                        <FooterStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
                        <HeaderStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
                        <PagerStyle BackColor="#FFCC66" ForeColor="#333333" HorizontalAlign="Center" />
                        <RowStyle BackColor="#FFFBD6" ForeColor="#333333" />
                        <SelectedRowStyle BackColor="#FFCC66" Font-Bold="True" ForeColor="Navy" />
                    </asp:GridView>
                </td>
                <td>
                    <asp:DetailsView ID="DetailsView15" runat="server" AutoGenerateRows="False" CellPadding="4" DataKeyNames="Id" DataSourceID="SqlDataSourceTransactionType" DefaultMode="Insert" EnableModelValidation="True" ForeColor="#333333" GridLines="None" Height="50px" Width="125px">
                        <AlternatingRowStyle BackColor="White" />
                        <CommandRowStyle BackColor="#FFFFC0" Font-Bold="True" />
                        <FieldHeaderStyle BackColor="#FFFF99" Font-Bold="True" />
                        <Fields>
                            <asp:BoundField DataField="Id" HeaderText="Id" SortExpression="Id" InsertVisible="False" ReadOnly="True" />
                            <asp:BoundField DataField="Text" HeaderText="Text" SortExpression="Text" />
                            <asp:CommandField ShowInsertButton="True" />
                        </Fields>
                        <FooterStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
                        <HeaderStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
                        <PagerStyle BackColor="#FFCC66" ForeColor="#333333" HorizontalAlign="Center" />
                        <RowStyle BackColor="#FFFBD6" ForeColor="#333333" />
                    </asp:DetailsView>
                </td>
            </tr>
            <tr>
                <td >
                    <asp:Label ID="Label16" runat="server" Text="Account"></asp:Label>
                    <asp:GridView ID="GridView15" runat="server" AutoGenerateColumns="False" CellPadding="4" DataKeyNames="Id" DataSourceID="SqlDataSourceAccount" EnableModelValidation="True" ForeColor="#333333" GridLines="None">
                        <AlternatingRowStyle BackColor="White" />
                        <Columns>
                            <asp:CommandField ShowEditButton="True" />
                            <asp:BoundField DataField="Id" HeaderText="Id" InsertVisible="False" ReadOnly="True" SortExpression="Id" />
                            <asp:BoundField DataField="Text" HeaderText="Text" SortExpression="Text" />
                        </Columns>
                        <FooterStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
                        <HeaderStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
                        <PagerStyle BackColor="#FFCC66" ForeColor="#333333" HorizontalAlign="Center" />
                        <RowStyle BackColor="#FFFBD6" ForeColor="#333333" />
                        <SelectedRowStyle BackColor="#FFCC66" Font-Bold="True" ForeColor="Navy" />
                    </asp:GridView>
                </td>
                <td>
                    <asp:DetailsView ID="DetailsView16" runat="server" AutoGenerateRows="False" CellPadding="4" DataKeyNames="Id" DataSourceID="SqlDataSourceAccount" DefaultMode="Insert" EnableModelValidation="True" ForeColor="#333333" GridLines="None" Height="50px" Width="125px">
                        <AlternatingRowStyle BackColor="White" />
                        <CommandRowStyle BackColor="#FFFFC0" Font-Bold="True" />
                        <FieldHeaderStyle BackColor="#FFFF99" Font-Bold="True" />
                        <Fields>
                            <asp:BoundField DataField="Id" HeaderText="Id" SortExpression="Id" InsertVisible="False" ReadOnly="True" />
                            <asp:BoundField DataField="Text" HeaderText="Text" SortExpression="Text" />
                            <asp:CommandField ShowInsertButton="True" />
                        </Fields>
                        <FooterStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
                        <HeaderStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
                        <PagerStyle BackColor="#FFCC66" ForeColor="#333333" HorizontalAlign="Center" />
                        <RowStyle BackColor="#FFFBD6" ForeColor="#333333" />
                    </asp:DetailsView>
                </td>
            </tr>
           
        </table>




        <asp:SqlDataSource ID="SqlDataSourceUserRole" runat="server" ConnectionString="<%$ ConnectionStrings:LocalConnectionString %>" DeleteCommand="DELETE FROM [UserRole] WHERE [Id] = @Id" InsertCommand="INSERT INTO [UserRole] ([Text]) VALUES (@Text)" SelectCommand="SELECT * FROM [UserRole]" UpdateCommand="UPDATE [UserRole] SET [Text] = @Text WHERE [Id] = @Id">
            <DeleteParameters>
                <asp:Parameter Name="Id" Type="Int32" />
            </DeleteParameters>
            <InsertParameters>
                <asp:Parameter Name="Text" Type="String" />
            </InsertParameters>
            <UpdateParameters>
                <asp:Parameter Name="Text" Type="String" />
                <asp:Parameter Name="Id" Type="Int32" />
            </UpdateParameters>
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="SqlDataSourceUserType" runat="server" ConnectionString="<%$ ConnectionStrings:LocalConnectionString %>" DeleteCommand="DELETE FROM [UserType] WHERE [Id] = @Id" InsertCommand="INSERT INTO [UserType] ([Text]) VALUES (@Text)" SelectCommand="SELECT * FROM [UserType]" UpdateCommand="UPDATE [UserType] SET [Text] = @Text WHERE [Id] = @Id">
            <DeleteParameters>
                <asp:Parameter Name="Id" Type="Int32" />
            </DeleteParameters>
            <InsertParameters>
                <asp:Parameter Name="Text" Type="String" />
            </InsertParameters>
            <UpdateParameters>
                <asp:Parameter Name="Text" Type="String" />
                <asp:Parameter Name="Id" Type="Int32" />
            </UpdateParameters>
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="SqlDataSourceUserSite" runat="server" ConnectionString="<%$ ConnectionStrings:LocalConnectionString %>" DeleteCommand="DELETE FROM [Site] WHERE [Id] = @Id" InsertCommand="INSERT INTO [Site] ([Text], [Url]) VALUES (@Text, @Url)" SelectCommand="SELECT * FROM [Site]" UpdateCommand="UPDATE [Site] SET [Text] = @Text, [Url] = @Url WHERE [Id] = @Id">
            <DeleteParameters>
                <asp:Parameter Name="Id" Type="Int32" />
            </DeleteParameters>
            <InsertParameters>
                <asp:Parameter Name="Text" Type="String" />
                <asp:Parameter Name="Url" Type="String" />
            </InsertParameters>
            <UpdateParameters>
                <asp:Parameter Name="Text" Type="String" />
                <asp:Parameter Name="Url" Type="String" />
                <asp:Parameter Name="Id" Type="Int32" />
            </UpdateParameters>
        </asp:SqlDataSource>

        <asp:SqlDataSource ID="SqlDataSourceCountry" runat="server" ConnectionString="<%$ ConnectionStrings:LocalConnectionString %>" DeleteCommand="DELETE FROM [Country] WHERE [Id] = @Id" InsertCommand="INSERT INTO [Country] ([Code], [Text], [PhoneCode]) VALUES (@Code, @Text, @PhoneCode)" SelectCommand="SELECT * FROM [Country]" UpdateCommand="UPDATE Country SET Code = @Code, Text = @Text, PhoneCode = @PhoneCode, CurrencyId = @CurrencyId, PhoneNumberStyle = @PhoneNumberStyle, CultureCode = @CultureCode WHERE (Id = @Id)">
            <DeleteParameters>
                <asp:Parameter Name="Id" Type="Int32" />
            </DeleteParameters>
            <InsertParameters>
                <asp:Parameter Name="Code" Type="String" />
                <asp:Parameter Name="Text" Type="String" />
                <asp:Parameter Name="PhoneCode" Type="Int32" />
            </InsertParameters>
            <UpdateParameters>
                <asp:Parameter Name="Code" Type="String" />
                <asp:Parameter Name="Text" Type="String" />
                <asp:Parameter Name="PhoneCode" Type="Int32" />
                <asp:Parameter Name="CurrencyId" />
                <asp:Parameter Name="PhoneNumberStyle" />
                <asp:Parameter Name="CultureCode" />
                <asp:Parameter Name="Id" Type="Int32" />
            </UpdateParameters>
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="SqlDataSourceLanguage" runat="server" ConnectionString="<%$ ConnectionStrings:LocalConnectionString %>" DeleteCommand="DELETE FROM [Language] WHERE [Id] = @Id" InsertCommand="INSERT INTO [Language] ([Code], [Text]) VALUES (@Code, @Text)" SelectCommand="SELECT * FROM [Language]" UpdateCommand="UPDATE [Language] SET [Code] = @Code, [Text] = @Text WHERE [Id] = @Id">
            <DeleteParameters>
                <asp:Parameter Name="Id" Type="Int32" />
            </DeleteParameters>
            <InsertParameters>
                <asp:Parameter Name="Code" Type="String" />
                <asp:Parameter Name="Text" Type="String" />
            </InsertParameters>
            <UpdateParameters>
                <asp:Parameter Name="Code" Type="String" />
                <asp:Parameter Name="Text" Type="String" />
                <asp:Parameter Name="Id" Type="Int32" />
            </UpdateParameters>
        </asp:SqlDataSource>

        <asp:SqlDataSource ID="SqlDataSourceCurrency" runat="server" ConnectionString="<%$ ConnectionStrings:LocalConnectionString %>" DeleteCommand="DELETE FROM [Currency] WHERE [Id] = @Id" InsertCommand="INSERT INTO Currency(Code, Text, CurrencyTypeId, YourPayCurrencyCode) VALUES (@Code, @Text, 1, @YourPayCurrencyCode)" SelectCommand="SELECT * FROM [Currency]" UpdateCommand="UPDATE Currency SET CurrencyTypeId = @CurrencyTypeId, Code = @Code, Text = @Text, YourPayCurrencyCode = @YourPayCurrencyCode WHERE (Id = @Id)">
            <DeleteParameters>
                <asp:Parameter Name="Id" Type="Int32" />
            </DeleteParameters>
            <InsertParameters>
                <asp:Parameter Name="Code" Type="String" />
                <asp:Parameter Name="Text" Type="String" />
                <asp:Parameter Name="YourPayCurrencyCode" />
            </InsertParameters>
            <UpdateParameters>
                <asp:Parameter Name="CurrencyTypeId" />
                <asp:Parameter Name="Code" Type="String" />
                <asp:Parameter Name="Text" Type="String" />
                <asp:Parameter Name="YourPayCurrencyCode" />
                <asp:Parameter Name="Id" Type="Int32" />
            </UpdateParameters>
        </asp:SqlDataSource>

        <asp:SqlDataSource ID="SqlDataSourceKycType" runat="server" ConnectionString="<%$ ConnectionStrings:LocalConnectionString %>" DeleteCommand="DELETE FROM [KycType] WHERE [Id] = @Id" InsertCommand="INSERT INTO [KycType] ([Text]) VALUES (@Text)" SelectCommand="SELECT * FROM [KycType]" UpdateCommand="UPDATE [KycType] SET [Text] = @Text WHERE [Id] = @Id">
            <DeleteParameters>
                <asp:Parameter Name="Id" Type="Int32" />
            </DeleteParameters>
            <InsertParameters>
                <asp:Parameter Name="Text" Type="String" />
            </InsertParameters>
            <UpdateParameters>
                <asp:Parameter Name="Text" Type="String" />
                <asp:Parameter Name="Id" Type="Int32" />
            </UpdateParameters>
        </asp:SqlDataSource>

        <asp:SqlDataSource ID="SqlDataSourceOrderStatus" runat="server" ConnectionString="<%$ ConnectionStrings:LocalConnectionString %>" DeleteCommand="DELETE FROM [OrderStatus] WHERE [Id] = @Id" InsertCommand="INSERT INTO [OrderStatus] ([Text]) VALUES (@Text)" SelectCommand="SELECT * FROM [OrderStatus]" UpdateCommand="UPDATE [OrderStatus] SET [Text] = @Text WHERE [Id] = @Id">
            <DeleteParameters>
                <asp:Parameter Name="Id" Type="Int32" />
            </DeleteParameters>
            <InsertParameters>
                <asp:Parameter Name="Text" Type="String" />
            </InsertParameters>
            <UpdateParameters>
                <asp:Parameter Name="Text" Type="String" />
                <asp:Parameter Name="Id" Type="Int32" />
            </UpdateParameters>
        </asp:SqlDataSource>

        <asp:SqlDataSource ID="SqlDataSourceOrderType" runat="server" ConnectionString="<%$ ConnectionStrings:LocalConnectionString %>" DeleteCommand="DELETE FROM [OrderType] WHERE [Id] = @Id" InsertCommand="INSERT INTO [OrderType] ([Text]) VALUES (@Text)" SelectCommand="SELECT * FROM [OrderType]" UpdateCommand="UPDATE [OrderType] SET [Text] = @Text WHERE [Id] = @Id">
            <DeleteParameters>
                <asp:Parameter Name="Id" Type="Int32" />
            </DeleteParameters>
            <InsertParameters>
                <asp:Parameter Name="Text" Type="String" />
            </InsertParameters>
            <UpdateParameters>
                <asp:Parameter Name="Text" Type="String" />
                <asp:Parameter Name="Id" Type="Int32" />
            </UpdateParameters>
        </asp:SqlDataSource>

        <asp:SqlDataSource ID="SqlDataSourcePaymentType" runat="server" ConnectionString="<%$ ConnectionStrings:LocalConnectionString %>" DeleteCommand="DELETE FROM [PaymentType] WHERE [Id] = @Id" InsertCommand="INSERT INTO [PaymentType] ([Code], [Text]) VALUES (@Code, @Text)" SelectCommand="SELECT * FROM [PaymentType]" UpdateCommand="UPDATE [PaymentType] SET [Code] = @Code, [Text] = @Text WHERE [Id] = @Id">
            <DeleteParameters>
                <asp:Parameter Name="Id" Type="Int32" />
            </DeleteParameters>
            <InsertParameters>
                <asp:Parameter Name="Code" Type="String" />
                <asp:Parameter Name="Text" Type="String" />
            </InsertParameters>
            <UpdateParameters>
                <asp:Parameter Name="Code" Type="String" />
                <asp:Parameter Name="Text" Type="String" />
                <asp:Parameter Name="Id" Type="Int32" />
            </UpdateParameters>
        </asp:SqlDataSource>

        <asp:SqlDataSource ID="SqlDataSourceAuditTrailStatus" runat="server" ConnectionString="<%$ ConnectionStrings:LocalConnectionString %>" DeleteCommand="DELETE FROM [AuditTrailStatus] WHERE [Id] = @Id" InsertCommand="INSERT INTO [AuditTrailStatus] ([Text]) VALUES (@Text)" SelectCommand="SELECT * FROM [AuditTrailStatus]" UpdateCommand="UPDATE [AuditTrailStatus] SET [Text] = @Text WHERE [Id] = @Id">
            <DeleteParameters>
                <asp:Parameter Name="Id" Type="Int32" />
            </DeleteParameters>
            <InsertParameters>
                <asp:Parameter Name="Text" Type="String" />
            </InsertParameters>
            <UpdateParameters>
                <asp:Parameter Name="Text" Type="String" />
                <asp:Parameter Name="Id" Type="Int32" />
            </UpdateParameters>
        </asp:SqlDataSource>

        <asp:SqlDataSource ID="SqlDataSourceTransactionMethod" runat="server" ConnectionString="<%$ ConnectionStrings:LocalConnectionString %>" DeleteCommand="DELETE FROM [TransactionMethod] WHERE [Id] = @Id" InsertCommand="INSERT INTO [TransactionMethod] ([Text]) VALUES (@Text)" SelectCommand="SELECT * FROM [TransactionMethod]" UpdateCommand="UPDATE [TransactionMethod] SET [Text] = @Text WHERE [Id] = @Id">
            <DeleteParameters>
                <asp:Parameter Name="Id" Type="Int32" />
            </DeleteParameters>
            <InsertParameters>
                <asp:Parameter Name="Text" Type="String" />
            </InsertParameters>
            <UpdateParameters>
                <asp:Parameter Name="Text" Type="String" />
                <asp:Parameter Name="Id" Type="Int32" />
            </UpdateParameters>
        </asp:SqlDataSource>

        <asp:SqlDataSource ID="SqlDataSourceTransactionType" runat="server" ConnectionString="<%$ ConnectionStrings:LocalConnectionString %>" DeleteCommand="DELETE FROM [TransactionType] WHERE [Id] = @Id" InsertCommand="INSERT INTO [TransactionType] ([Text]) VALUES (@Text)" SelectCommand="SELECT * FROM [TransactionType]" UpdateCommand="UPDATE [TransactionType] SET [Text] = @Text WHERE [Id] = @Id">
            <DeleteParameters>
                <asp:Parameter Name="Id" Type="Int32" />
            </DeleteParameters>
            <InsertParameters>
                <asp:Parameter Name="Text" Type="String" />
            </InsertParameters>
            <UpdateParameters>
                <asp:Parameter Name="Text" Type="String" />
                <asp:Parameter Name="Id" Type="Int32" />
            </UpdateParameters>
        </asp:SqlDataSource>

        <asp:SqlDataSource ID="SqlDataSourceAccount" runat="server" ConnectionString="<%$ ConnectionStrings:LocalConnectionString %>" DeleteCommand="DELETE FROM [Account] WHERE [Id] = @Id" InsertCommand="INSERT INTO Account(Text) VALUES (@Text)" SelectCommand="SELECT * FROM [Account]" UpdateCommand="UPDATE Account SET Text = @Text WHERE (Id = @Id)">
            <DeleteParameters>
                <asp:Parameter Name="Id" Type="Int32" />
            </DeleteParameters>
            <InsertParameters>
                <asp:Parameter Name="Text" Type="String" />
            </InsertParameters>
            <UpdateParameters>
                <asp:Parameter Name="Text" Type="String" />
                <asp:Parameter Name="Id" Type="Int32" />
            </UpdateParameters>
        </asp:SqlDataSource>

        <div>
        </div>
    </form>
</body>
</html>
