$(document).ready(function () {
    $('#crawl').click(Crawl);
    $('#clearindex').click(ClearIndex);
    $('#getpagetitle').click(GetPageTitle);
    $('#refresh').click(Refresh);
});

function Crawl() {
    if ($('#crawl').text() == "Start Crawl") {
        $('#crawl').text("Stop Crawl");
        Dashboard.Admin.StartCrawl($('#begin').val(), Callback);
        Dashboard.Admin.GetStatus(Callback);
    } else {
        $('#crawl').text("Start Crawl");
        Dashboard.Admin.StopCrawl(Callback);
        Dashboard.Admin.GetStatus(Callback);
    }
    return false;
}

function ClearIndex() {
    Dashboard.Admin.ClearIndex(Callback);
    Dashboard.Admin.GetStatus(Callback);
    alert("Index cleared!");
    return false;
}

function GetPageTitle() {
    Dashboard.Admin.GetPageTitle($('#textbox').val(), Callback);
    return false;
}

function Refresh() {
    Dashboard.Admin.QueueSize(Callback);
    Dashboard.Admin.GetLastTenURLs(Callback);
    Dashboard.Admin.Errors(Callback);
    Dashboard.Admin.GetStatistics(Callback);
    return false;
}

function Callback(result, userContext, methodName) {
    if (methodName == "GetPageTitle") {
        $('#title').text(result);
    } else if (methodName == "QueueSize") {
        $('#queuesize').text(result);
    } else if (methodName == "GetStatistics") {
        $.each(result, function (index, value) {
            if (index == 1)
                $('#ram').text(value);
            else if (index == 2)
                $('#cpu').text(value);
            else if (index == 3)
                $('#urlsnumber').text(value);
            else if (index == 4)
                $('#indexsize').text(value);
        });
    } else if (methodName == "GetLastTenURLs") {
        $('#list').empty();
        $.each(result, function(index, value) {
            $('#list').append(value + "<br />");
        });
    } else if (methodName == "Errors") {
        $('#console').empty();
        $.each(result, function(index, value) {
            $('#console').append(value + "<br />");
        });
    } else if (methodName == "GetStatus") {
        $('#status').text(result);
    }
}

if (typeof (Sys) !== "undefined") Sys.Application.notifyScriptLoaded();