"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/_notifications").build();
var instance = null;

connection.on("ReceiveNotification", function (message) {
    console.debug(message);
    instance.invokeMethodAsync('UpdateMessagesAsync', message);
});

window.NotifyLive = (instance1) => {
    instance = instance1;
}

connection.start().then(function () {
    console.debug("Connection started");

}).catch(function (err) {
    return console.error(err.toString());
});