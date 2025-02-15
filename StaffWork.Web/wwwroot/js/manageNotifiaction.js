function updateNotificationCount() {
    $.get("/Notification/GetUnreadNotificationCount", function (data) {
        $("#notifiCount").text(data.count);
    });
}

$(document).ready(function () {
    // Update notification count
    updateNotificationCount();

    // Handle notification click
    $(document).on("click", ".notification-item", function () {
        var notificationId = $(this).data("id");
        var vacation_Id = $(this).data("vacationid");
        var isRead = $(this).data("read");

        // Update notification status as read before redirecting
        $.ajax({
            url: '/Notification/MarkAsRead', // Adjust according to your controller route
            type: 'POST',
            data: { id: notificationId },
            success: function () {
                // Redirect to the Edit page
                window.location.href = '/Vacation/Edit/' + vacation_Id;
            },
            error: function () {
                console.error("Failed to mark notification as read.");
                window.location.href = '/Vacation/Edit/' + vacation_Id;
            }
        });
    });
});