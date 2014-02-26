<%@ Page Title="Web Crawler Dashboard" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Dashboard._Default" %>
<asp:Content runat="server" ID="FeaturedContent" ContentPlaceHolderID="FeaturedContent">
    <script type="text/javascript" src="Scripts/jquery-1.8.2.min.js"></script>
    <section class="featured">
        <div class="content-wrapper">
            <hgroup class="title">
                <h1><%: Title %></h1>
            </hgroup>
    <!--Need state of each worker role
    machine counters
    last 10 URLs crawled
    size of queue
    size of index
    any errors and their URLs-->
        </div>
    </section>
</asp:Content>
<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">

    <div>
        <h1>Commands</h1>
        <p>
            Crawler Status: <span id="status">Loading</span><br />
            Type in a root URL here and hit Start Crawl to begin parsing.
        </p>
        <input id="begin" type="url" />
        <button id="crawl" type="button">Start Crawl</button>
        <button id="clearindex" type="button">Clear Index</button>
        <br />
        <p>
            Gets a page title based on URL entered.
        </p>
        <input id="textbox" type="url" />
        <button id="getpagetitle" type="button">Get Page Title</button>
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
    </table>
    <p>Last 10 URLs Crawled:</p>
    <div id="list">

    </div>
    <p>Errors:</p>
    <div id="console">
        
    </div>
</asp:Content>

