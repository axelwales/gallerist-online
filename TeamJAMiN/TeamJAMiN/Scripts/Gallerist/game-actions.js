//JS file for signalR communication

$(function () {
    var actionManager = $.connection.gameActionHub;
    actionManager.client.update = function (elementIdentifier, newElement) {
        updateHelper(elementIdentifier, newElement);
    };
    $.connection.hub.logging = true;
    $.connection.hub.start().done(function () {
        actionManager.server.activate();
        console.log($.connection.hub.id);
    });
});