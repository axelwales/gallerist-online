$(function () {
	var listManager = $.connection.gameListHub;
	listManager.client.updateListItem = function (openUrl, gameId) {
		$('#start-open-button-container-' + gameId + ' .open-button-container').append('<a href="' + openUrl + '" class="btn btn-sm btn-success">Open</a>');
	};
	listManager.client.refresh = function () {
	    location.reload();
	};
	$.connection.hub.logging = true;
	$.connection.hub.start().done(function () {
		listManager.server.activate();
	});
});