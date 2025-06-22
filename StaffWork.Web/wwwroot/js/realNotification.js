"use strict"

function PlayNotify() {
    $('#NotifyAudio')[0].play();
}
function updateNotification(notification) {
    // Construct the notification HTML
    var notificationHtml = `
        <div class="notification-item d-flex justify-content-center"
             data-id="${notification.id}" 
             data-read="${notification.isRead}" 
             data-taskModelId="${notification?.TaskReminder?.TaskModelId}">
            <div class="notification-card bg-light shadow-lg rounded-3 p-4 w-75 position-relative mb-4
                    ${notification.isRead ? "border-start-success" : "border-start-warning"}">
                <div class="d-flex justify-content-between align-items-center">
                    <h5 class="fw-bold text-dark mb-1">${notification.title}</h5>
                    <span class="badge ${notification.isRead ? "bg-success" : "bg-warning text-dark"}">
                        ${notification.isRead ? "مقروء" : "غير مقروء"}
                    </span>
                </div>
                <hr />
                <div class="vacation-details mt-3">
                    <div class="d-flex row">
                        <h4>${notification.content || "غير متوفر"}</h4>
                    </div>
                </div>
                <div class="text-start mt-1">
                    <small class="text-muted">⏳ تم الإرسال في: ${notification.dateCreated}</small>
                </div>
            </div>
        </div>
    `;

    // Append to the notification list
    $("#notificationList").prepend(notificationHtml);
}


var connection = new signalR.HubConnectionBuilder().withUrl("/notificationHub", {
    accessTokenFactory: () => "testAccessToken"
}).build();

function invoke(notification) {
    connection.invoke("ReceiveNotification", notification).catch(function (error) {
        return console.error(error);
    });
}

connection.on("RegisterOnlineUser", function (MyConnectionID) {
    //  var Userid = $('#UserId').val();
    //  console.log(MyConnectionID);

});
connection.on("ReceiveNotification", function (notification) {
  //  console.log(notification);
    updateNotification(notification);
    updateNotificationCount();
    PlayNotify();

});
connection.start().then(function () {
  //  console.log('connection start');
}).catch(function (err) {
    return console.error(err.toString());
});

