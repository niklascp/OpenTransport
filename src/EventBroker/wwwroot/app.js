$(function () {
    var eventHub = $.connection.eventHub;

    eventHub.client.deliver = function deliver(clientId) {
        console.log('deliver(' + clientId + ')');
    }

    eventHub.client.vehicleEvent = function vehicleEvent(event) {
        console.log("server calling...", event);
    }

    $.connection.hub.start().done(function () {
        eventHub.server.subscribe(['line.300s', 'line.330e']);

        $('#addEvent').click(function () {
            eventHub.server.publish('DATA IS HERE', ['line.300s'])
        })
    });
});