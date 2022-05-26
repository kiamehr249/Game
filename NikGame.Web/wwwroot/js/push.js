"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/DartHub").build();

connection.on("ReceiveMessage", sendMessage);
connection.on("AllowStartGame", allowStartMatch);
connection.on("UpdateScoreList", updatePlayerList);

connection.start().then(function () {
    //document.getElementById("sendButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

function sendMessage(user, message) {
    alert(`${user} says ${message}`);
}

function allowStartMatch(matchId) {
    alert(`Darts Game ${matchId} was started`);
}

function updatePlayerList(matchId) {
    alert(`Darts Game ${matchId}, player list was started`);
}


/*
$('#btnsend').on('click', function () {
    var message = $('#msg').val();
    var user = 'Kiamehr';
    connection.invoke("SendMessage", user, message).catch(function (err) {
        return console.error(err.toString());
    });
});
*/