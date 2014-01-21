"use strict";
$(document).ready(function ()
{
	$('#searchbutton').click(function(e) {
		var playername = $('#searchbox').val();
		$.ajax({
			type: 'POST',
			url: 'staging.php',
			data: { 'name' : playername },
			dataType: 'html',
			success: function(data)
			{
				$('#results').hide('blind', 'easeInOutCubic', 1200);
				$('#results').empty();
				$('#results').append(data);
				$('#results').show('blind', 'easeInOutCubic', 1200);
			}
		})
	});
});