<%@ Page Language="VB" AutoEventWireup="false" MasterPageFile="~/MasterPageAdmin.master" CodeFile="SanctionsList.aspx.vb" Inherits="SanctionsList" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:LinkButton ID="LinkButtonBACK" runat="server" PostBackUrl="~/Default.aspx">Back</asp:LinkButton>

    <br />
    <br />
    <asp:Panel ID="PanelMode" runat="server">
        <table>
            <tr>
                <td>
                    <p class="verdana13">Enter Name:</p>
                </td>
                <td>
                    <asp:TextBox ID="TextBoxText" runat="server" TextMode="MultiLine" Width="200px" Height="20px"></asp:TextBox></td>
                <td>
                    <asp:Button ID="ButtonSearch" runat="server" Text="SEARCH" /></td>
                <td></td>
            </tr>
        </table>
    </asp:Panel>

    <table style="width: 100%;">
        <tr>
            <td colspan="3" style="text-align: center">
                <p>
                    <asp:GridView ID="ListGridView" runat="server"  AllowPaging="True" AllowSorting="True" AutoGenerateColumns="False" BackColor="White" DataKeyNames="Id" BorderColor="#336666" BorderStyle="Double" BorderWidth="3px" CellPadding="4" EmptyDataText="No User's found" GridLines="Horizontal" Width="100%">
                       <Columns>                          
                           <asp:TemplateField > 
                              <ItemTemplate>  
                              <asp:Button ID="btn_Select" runat="server" Text="Select" CommandName="Select" CommandArgument='<%# DataBinder.Eval(Container, "RowIndex") %>' />  
                              </ItemTemplate> 
                             <ItemStyle Width="120px" />
                           </asp:TemplateField>
                           <asp:BoundField DataField="Id" HeaderText="Id" ReadOnly="True" SortExpression="Id" Visible="False" />
                           <asp:BoundField DataField = "Name1" HeaderText="Name1" SortExpression="Name1" ReadOnly="True"/>
                           <asp:BoundField DataField = "Name2" HeaderText="Name2" SortExpression="Name2" ReadOnly="True"/>
                           <asp:BoundField DataField = "Name3" HeaderText="Name3" SortExpression="Name3" ReadOnly="True"/>
                           <asp:boundfield datafield = "Name4" headertext="Name4" sortexpression="Name4" readonly="true"/>
                           <asp:boundfield datafield = "Name5" headertext="Name5" sortexpression="Name5" readonly="true"/>
                           <asp:boundfield datafield = "Name6" headertext="Name6" sortexpression="Name6" readonly="true"/>
                           <asp:BoundField DataField = "DOB" HeaderText="DOB" SortExpression="DOB" ReadOnly="True"/>
                           <asp:BoundField DataField = "CountryOfResidance" HeaderText="CountryOfResidance" SortExpression="CountryOfResidance" ReadOnly="True"/>
                           <asp:BoundField DataField = "FromSourceValue"  HeaderText="FromSource" SortExpression="FromSource" ReadOnly="True"/>
                            <asp:TemplateField Visible="false">
                            <ItemTemplate>
                              <asp:Label id="SummaryId" runat ="server" text='<%# Eval("Summary")%>'></asp:Label>
                           </ItemTemplate>
                         </asp:TemplateField>
 
                        <%--   <asp:TemplateField HeaderText="FromSource">
                         <ItemTemplate>
                         <asp:Label ID="lblSource" runat="server" Text='<% #Eval("FromSource") %>'></asp:Label>
                        </ItemTemplate>
                        </asp:TemplateField>--%>
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
                <p>
                    &nbsp;
                </p>
            </td>
        </tr>
    </table>
    <p class="verdana13">Summary:</p>
    <asp:TextBox ID="TextBox1" runat="server" Visible="false" Width="100%" Height="100px" TextMode="MultiLine" style="overflow:hidden"></asp:TextBox>
</asp:Content>