<%@ Page Title="" Language="VB" MasterPageFile="~/MasterPageAdmin.master"  AutoEventWireup="false" CodeFile="Appsettings.aspx.vb" Inherits="Appsettings" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <style type="text/css">
        .auto-style1 {
            width: 634px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:LinkButton ID="LinkButtonBACK" runat="server" PostBackUrl="~/Default.aspx">Back</asp:LinkButton>
    <br />
    <br />
    <br />

    <asp:Panel ID="PanelSelect" runat="server" HorizontalAlign="Left">
        <br />
        <table>
            <tr>
                <td class="auto-style1">
                    <h3>App Settings:</h3>
                    <asp:GridView ID="GridViewAppSettings" runat="server" AutoGenerateColumns="False" DataKeyNames="Id"  data CellPadding="4" ForeColor="#333333" GridLines="None" Width="568px">
                        <AlternatingRowStyle BackColor="White" />
                        <Columns>
                          
                            <%--<asp:BoundField DataField="Id" HeaderText="Id" SortExpression="Id" ReadOnly="True" Visible="False" />--%>
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
                            <asp:BoundField DataField="ConfigKey" HeaderText="ConfigKey" SortExpression="ConfigKey" ReadOnly="true" />
                            <asp:TemplateField HeaderText="ConfigValue" SortExpression="ConfigValue">
                               <EditItemTemplate>
                                   <asp:TextBox ID="TextConfigValue" runat="server" Text='<%# Bind("ConfigValue") %>' TextMode="MultiLine" Height="237px" Width="531px"></asp:TextBox>
                               </EditItemTemplate>
                               <ItemTemplate>
                                   <asp:Label ID="Label1" runat="server" Text='<%# Bind("ConfigValue") %>'></asp:Label>
                               </ItemTemplate>
                           </asp:TemplateField>
                            <asp:BoundField DataField="ConfigDescription" HeaderText="ConfigDescription" ReadOnly="true" SortExpression="ConfigDescription" /> 
                            <asp:BoundField DataField="IsEncrypted" HeaderText="IsEncrypted" SortExpression="IsEncrypted" ReadOnly="true" />
                            <asp:TemplateField ShowHeader="False">
                                <ItemTemplate>
                                    <asp:LinkButton ID="LinkButtonDelete" runat="server" CausesValidation="False" CommandName="Delete" Text="Delete"></asp:LinkButton>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                        <EditRowStyle BackColor="#ECECEC" />
                        <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                        <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                        <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                        <RowStyle BackColor="#EFF3FB" />
                        <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
                        <SortedAscendingCellStyle BackColor="#F5F7FB" />
                        <SortedAscendingHeaderStyle BackColor="#6D95E1" />
                        <SortedDescendingCellStyle BackColor="#E9EBEF" />
                        <SortedDescendingHeaderStyle BackColor="#4870BE" />
                    </asp:GridView>
                </td>
                <td>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;</td>
                <td>
                    <asp:LinkButton ID="LinkButtonInsert" runat="server">New AppSettings</asp:LinkButton></td>
            </tr>
        </table>
        <br />
    </asp:Panel>

    <br />
    <%--<asp:SqlDataSource ID="SqlDataSourceAppSettings" runat="server" ConnectionString="<%$ ConnectionStrings:LocalConnectionString %>" SelectCommand="SELECT Id,ConfigKey,ConfigValue,ConfigDescription,IsEncrypted FROM [AppSettings]" DeleteCommand="DELETE FROM [AppSettings] WHERE [Id] = @Id" InsertCommand="INSERT INTO AppSettings(ConfigKey, ConfigValue, ConfigDescription, IsEncrypted) VALUES (@ConfigKey, @ConfigValue, @ConfigDescription, @IsEncrypted)">
        <DeleteParameters>
            <asp:Parameter Name="Id" Type="Int32" />
        </DeleteParameters>
        <InsertParameters>
            <asp:Parameter Name="ConfigKey" Type="String" />
            <asp:Parameter Name="ConfigValue" Type="String" />
            <asp:Parameter Name="ConfigDescription" Type="String" />
            <asp:Parameter Name="IsEncrypted" Type="String" />
        </InsertParameters>
    </asp:SqlDataSource>--%>
</asp:Content>