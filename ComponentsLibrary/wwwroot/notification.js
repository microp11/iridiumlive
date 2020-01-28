"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/_notifications").build();

connection.on("ReceiveNotification", function (message) {
    console.debug(message);
});

connection.start().then(function () {
    console.debug("Connection started");
}).catch(function (err) {
    return console.error(err.toString());
});