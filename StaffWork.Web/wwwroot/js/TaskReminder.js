$(document).ready(function () {
    var tbody = $('#TaskReminders').find('tbody');
    $('[data-kt-filter="search"]').on('keyup', function () {
        var input = $(this);
        datatable.search(this.value).draw();
    });

    let columns = [
        { "data": "id", "name": "Id", "className": "d-none" },
        { "data": "title", "name": "Title", "className": "text-center" },
        { "data": "taskModelTitle", "name": "TaskModelTitle", "className": "text-center" },
        { "data": "createdByUserName", "name": "CreatedByUserName", "className": "text-center" },
        {
            "name": "ReminderDate",
            "className": "text-center",
            "render": function (data, type, row) {
                if (row.reminderDate) {
                    return moment.utc(row.reminderDate).local().format('ll'); // تحويل من UTC إلى Local
                } else {
                    return ''; // لو مفيش تاريخ
                }
            }
        },
        {
            "data": "isReminderCompleted", "name": "IsReminderCompleted", "className": "text-center"
            ,
            "render": function (data, type, row) {
                // Mapping status to Arabic values
                let statusText = "";
                let badgeClass = "";

                if (data === true) {
                    statusText = "نعم";
                    badgeClass = "success";
                } else if (data === false) {
                    statusText = "لا";
                    badgeClass = "danger";
                }

                // Return the rendered HTML with the badge and status text
                return '<span class="js-received fs-5 badge badge-light-' + badgeClass + '">' + statusText + '</span>';
            }
        },
        {
            "name": "ReminderCompletedDate",
            "className": "text-center",
            "render": function (data, type, row) {
                if (row.reminderCompletedDate) {
                    return moment.utc(row.reminderCompletedDate).local().format('ll'); // تحويل من UTC إلى Local
                } else {
                    return ''; // لو مفيش تاريخ
                }
            }
        },
        {
            "data": "notes",
            "name": "Notes",
            "className": "text-center",
            "render": function (data, type, row, meta) {
                if (data) {
                    // Split the note into words and take the first 5 words
                    var firstFiveWords = data.split(' ').slice(0, 5).join(' ');

                    // Escape single quotes in the note to prevent breaking the string
                    var escapedNote = row.notes
                        .replace(/\\/g, '\\\\')  // Escape backslashes
                        .replace(/'/g, "\\'")    // Escape single quotes
                        .replace(/"/g, '\\"')    // Escape double quotes
                        .replace(/\n/g, '\\n')   // Escape newlines
                        .replace(/\r/g, '\\r');  // Escape carriage returns
                    // Create a clickable element for the first five words
                    return `
            <span style="cursor: pointer;" onclick="showNote('${escapedNote}')">
                ${firstFiveWords}${data.split(' ').length > 5 ? '...' : ''}
            </span>`;
                }
                return data; // return data as it is if it's empty or undefined
            }
        },
    ];
    if (isSuperAdminOrAdmin === false) {
        columns.push(
            {
                "className": 'text-start',
                "orderable": false,
                "render": function (data, type, row) {
                    return `
                            <a href="javascript:;" class="btn btn-sm btn-outline btn-outline-dashed btn-outline-warning btn-active-light-warning js-Complete-status"
                               data-title="انجاز" data-url="/${tbody.data('controller')}/Complete/${row.id}" data-update="true" data-message="هل متأكد من انجاز الحاله؟">
                               انجاز
                            </a>
                           `;
                }
            });
        columns.push(
            {
                "className": 'text-start',
                "orderable": false,
                "render": function (data, type, row) {
                    return `
                 <a href="/${tbody.data('controller')}/Edit/${row.id}" class="btn btn-sm btn-outline btn-outline-dashed btn-outline-primary btn-active-light-primary"
                   data-title="تعديل" data-url="/${tbody.data('controller')}/Edit/${row.id}" data-update="true">
                    تعديل 
                </a>`;
                }
            });
        columns.push(
            {
                "className": 'text-start',
                "orderable": false,
                "render": function (data, type, row) {
                    return `
                <a href="javascript:;" class="btn btn-sm btn-outline btn-outline-dashed btn-outline-danger btn-active-light-danger js-confirm"
                   data-title="حذف" data-url="/${tbody.data('controller')}/Delete/${row.id}" data-update="true"
                            data-message="هل متأكد من حذف هذا العنصر؟">
                    حذف 
                </a> `;
                }
            });
    }



    // Handle title filter change event
    var titleFilter = $('#titleSearchId');
    titleFilter.on('change', function () {
        var selectedTitle = $(this).val();
        datatable.search(selectedTitle).draw();
    });

    datatable = $('#TaskReminders').DataTable({
        serverSide: true,
        processing: true,
        stateSave: false,
        paging: false,
        ajax: {
            url: tbody.data('url'),
            type: 'POST'
        },
        language: {
            emptyTable: 'لا يوجد بيانات فى هذا البحث',
        },
        'drawCallback': function () {
            KTMenu.createInstances();
        },
        order: [[1, 'asc']],
        columnDefs: [{
            targets: [0],
            visible: false,
            searchable: false
        }],
        columns: columns,
    });
});