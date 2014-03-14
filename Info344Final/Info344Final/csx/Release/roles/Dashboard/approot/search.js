$(document).ready(function () {
    $('#search').click(ajax);
    $("#inputbox").keyup(function () {
        $.ajax(
            {
                type: "POST",
                url: "WebService.asmx/GetSuggestions",
                data: "{ 'word':'" + $("#inputbox").val() + "'}",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    $("#test").html("");
                    $.each($.parseJSON(data.d), function (index, value) {
                        $("#test").append(value + "<br/>");
                    });
                },
                error: function (data) {
                    alert("Error");
                }
            }
        );
        $.ajax(
            {
                type: "POST",
                url: "WebService.asmx/GetCache",
                data: "{ 'word':'" + $("#inputbox").val() + "'}",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    $("#cache").html("");
                    $.each($.parseJSON(data.d), function (index, value) {
                        $("#cache").append(value + "<br/>");
                    });
                },
                error: function (data) {
                    alert("Error");
                }
            }
        );
    });
});

function ajax() {
    var test = $('#inputbox').val();
    $('#playerbox').empty();
    if (test.search(" ") > 0) {
        $.ajax({
            crossDomain: true,
            contentType: "application/json; charset=utf-8",
            url: "http://ec2-54-201-242-233.us-west-2.compute.amazonaws.com/jsonp.php",
            data: { name: test },
            dataType: "jsonp",
            success: updateResults
        });
    }
    getUrlFromTitle();
}

function updateResults(data) {
    $('#playerbox').html(
        "<div class='playerdata'>"
            + data[0].PlayerName
				+ "<table>"
					+ "<thead>"
						+ "<tr>"
							+ "<th>GP</th>"
							+ "<th>FGP</th>"
							+ "<th>TPP</th>"
							+ "<th>FTP</th>"
							+ "<th>PPG</th>"
						+ "</tr>"
					+ "</thead>"
					+ "<tbody>"
						+ "<tr>"
							+ "<td>" + data[0].GP + "</td>"
							+ "<td>" + data[0].FGP + "</td>"
							+ "<td>" + data[0].TPP + "</td>"
							+ "<td>" + data[0].FTP + "</td>"
							+ "<td>" + data[0].PPG + "</td>"
						+ "</tr>"
					+ "</tbody>"
				+ "</table>"
			+ "</div>");
}



function getUrlFromTitle() {
    Dashboard.WebService.getUrlFromTitle($('#inputbox').val(), getUrlFromTitleCallBack);

}

function getUrlFromTitleCallBack(result) {
    $('#result').html("");
    for (var i in result) {
        var link = jQuery("<p/>");
        link.text(result[i]);
        $('#result').append(link);
    }
}