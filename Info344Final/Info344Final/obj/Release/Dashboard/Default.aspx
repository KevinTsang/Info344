<%@ Page Title="Search Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Dashboard._Default" %>

<asp:Content runat="server" ID="FeaturedContent" ContentPlaceHolderID="FeaturedContent">
</asp:Content>
<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    <script type='text/javascript' src='http://ads1.qadabra.com/t?id=4e33513c-a66f-4ed4-8fd7-3b6e82d0c943&size=468x60'></script>
    <div>
        <input id="inputbox" type="text" />
        <button id="search" type="button">Search</button>
        Cache:
        <div id="cache"></div>
    </div>
    <div>
        <div id="test"></div>
        <div id="playerbox"></div>
        <div id="result"></div>
        <div id="urls"></div>
    </div>
</asp:Content>
