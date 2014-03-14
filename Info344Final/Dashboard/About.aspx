<%@ Page Title="Dashboard" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="About.aspx.cs" Inherits="Dashboard.About" %>

<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    <script src="dashboard.js"></script>
    <hgroup class="title">
        <h1><%: Title %>.</h1>
        <h2>Dashboard Statistics</h2>
    </hgroup>
    <div>
        <h1>Commands</h1>
        <p>
            Crawler Status: <span id="status">loading</span><br />
            Hit Start Crawl to begin parsing.
        </p>
        <button id="crawl" type="button">Start Crawl</button>
        <button id="clearindex" type="button">Clear Index</button>
        <br />
        <p id="title">

        </p>
        <button id="refresh" type="button">Refresh</button>
    </div>
    <table>
        <tr>
            <td>RAM Available</td>
            <td id="ram"></td>
        </tr>
        <tr>
            <td>CPU Utilization</td>
            <td id="cpu"></td>
        </tr>
        <tr>
            <td>URLs Crawled</td>
            <td id="urlsnumber"></td>
        </tr>
        <tr>
            <td>Queue Size</td>
            <td id="queuesize"></td>
        </tr>
        <tr>
            <td>Index Size</td>
            <td id="indexsize"></td>
        </tr>
        <tr>
            <td>Trie Count</td>
            <td id="trieCount"></td>
        </tr>
    </table>
    <p>Last 10 URLs Crawled:</p>
    <div id="list">

    </div>
    <p>Errors:</p>
    <div id="console" style="height: 400px; overflow:auto">
        
    </div>
</asp:Content>