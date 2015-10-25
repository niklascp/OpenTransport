$(function () {
    var eventHub = $.connection.eventHub;

    eventHub.client.vehicleEvent = function vehicleEvent(event) {
        console.log("server calling...", event);
    }

    $.connection.hub.start().done(function () {
        eventHub.server.subscribe('line.300s');
        eventHub.server.subscribe('line.330e');

        $('#addEvent').click(function () {
            eventHub.server.publish('line.300s', 'DATA IS HERE')
        });
    });
});