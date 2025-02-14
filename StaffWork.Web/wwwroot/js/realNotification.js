"use strict"

function PlayNotify() {
    $('#NotifyAudio')[0].play();
}


var connection = new signalR.HubConnectionBuilder().withUrl("/notificationHub", {
    accessTokenFactory: () => "testAccessToken"
}).build();

function invoke(message) {
    connection.invoke("ReceiveNotification", message).catch(function (error) {
        return console.error(error);
    });
}

//var connection = new signalR.HubConnectionBuilder()
//    .withUrl("/notificationHub")
//    .build();

connection.on("RegisterOnlineUser", function (MyConnectionID) {
    //  var Userid = $('#UserId').val();
  //  console.log(MyConnectionID);


});
connection.on("ReceiveNotification", function (notification) {
  //  console.log(notification);
   // updateNotification(notification);
    updateNotificationCount();
    PlayNotify();

});
connection.start().then(function () {
  //  console.log('connection start');
}).catch(function (err) {
    return console.error(err.toString());
});

