$(document).ready(function () {
    $('#crawl').click(Crawl);
    $('#clearindex').click(ClearIndex);
    $('#refresh').click(Refresh);
});

function Crawl() {
    if ($('#crawl').text() == "Start Crawl") {
        $('#crawl').text("Stop Crawl");
        Dashboard.WebService.StartCrawl($('#begin').val(), Callback);
    } else {
        $('#crawl').text("Start Crawl");
        Dashboard.WebService.StopCrawl(Callback);
    }
    Dashboard.WebService.GetStatus(Callback);
    return false;
}

function ClearIndex() {
    Dashboard.WebService.ClearIndex(Callback);
    Dashboard.WebService.GetStatus(Callback);
    alert("Index cleared!");
    return false;
}

function Refresh() {
    Dashboard.WebService.QueueSize(Callback);
    Dashboard.WebService.GetLastTenURLs(Callback);
    Dashboard.WebService.Errors(Callback); 
    Dashboard.WebService.GetRam(Callback);
    Dashboard.WebService.GetCPU(Callback);
    Dashboard.WebService.NumberCrawled(Callback);
    Dashboard.WebService.IndexSize(Callback);
    Dashboard.WebService.GetLastTenURLs(Callback);
    Dashboard.WebService.GetStatus(Callback);
    Dashboard.WebService.titleInserted(Callback);
    Dashboard.WebService.trieCount(Callback);
    return false;
}

function Callback(result, userContext, methodName) {
    if (methodName == "QueueSize") {
        $('#queuesize').text(result);
    } else if (methodName == "titleInserted") {
        $('#title').text(result);
    } else if (methodName == "trieCount") {
        $('#trieCount').text(result);
    } else if (methodName == "GetRam") {   
        $('#ram').text(value);
    } else if (methodName == "GetCPU") {
        $('#cpu').text(value);
    } else if (methodName == "NumberCrawled") {
        $('#urlsnumber').text(value);
    } else if (methodName == "IndexSize") {
        $('#indexsize').text(value);
    } else if (methodName == "GetLastTenURLs") {
        $('#list').empty();
        $.each(result, function (index, value) {
            $('#list').append(value + "<br />");
        });
    } else if (methodName == "Errors") {
        $('#console').empty();
        $.each(result, function (index, value) {
            $('#console').append(value + "<br />");
        });
    } else if (methodName == "GetStatus") {
        $('#status').text(result);
    }
}

if (typeof (Sys) !== "undefined") Sys.Application.notifyScriptLoaded();