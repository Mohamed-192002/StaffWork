$(document).ready(function () {
    var tbody = $('#PersonalReminders').find('tbody');
    $('[data-kt-filter="search"]').on('keyup', function () {
        var input = $(this);
        datatable.search(this.value).draw();
    });

    let columns = [
        { "data": "id", "name": "Id", "className": "d-none" },
        { "data": "title", "name": "Title", "className": "text-center" },
        { "data": "createdByUserName", "name": "CreatedByUserName", "className": "text-center" },
        {
            "name": "ReminderDate",
            "className": "text-center",
            "render": function (data, type, row) {
                if (row.reminderDate) {
                    return moment.utc(row.reminderDate).local().format('ll');
                } else {
                    return '';
                }
            }
        },
        {
            "name": "ReminderDate",
            "className": "text-center",
            "render": function (data, type, row) {
                if (!row.reminderDate) {
                    return "";
                }
                var reminderDate = moment(row.reminderDate);
                var today = moment();
                var diffDays = reminderDate.diff(today, 'days');
                if (diffDays > 3) {
                    return `<span class="badge badge-dark fw-bold">متبقي  ${Math.abs(diffDays)} يوم</span>`;
                }
                if (diffDays < 0) {
                    return `<span class="badge badge-danger fw-bold">متأخر بـ ${Math.abs(diffDays)} يوم</span>`;
                }
                switch (diffDays) {
                    case 0:
                        return '<span class="badge badge-danger fw-bold">اليوم</span>';
                    case 1:
                        return '<span class="badge badge-danger fw-bold">متبقي يوم</span>';
                    case 2:
                        return '<span class="badge badge-warning fw-bold">متبقي يومان</span>';
                    case 3:
                        return '<span class="badge badge-success fw-bold">متبقي 3 أيام</span>';
                    default:
                        return `<span class="badge badge-danger fw-bold">متبقى  ${Math.abs(diffDays)} يوم</span>`;
                }
            }
        },
        {
            "data": "isReminderCompleted", "name": "IsReminderCompleted", "className": "text-center"
            ,
            "render": function (data, type, row) {
                let statusText = "";
                let badgeClass = "";

                if (data === true) {
                    statusText = "نعم";
                    badgeClass = "success";
                } else if (data === false) {
                    statusText = "لا";
                    badgeClass = "danger";
                }
                return '<span class="js-received fs-5 badge badge-light-' + badgeClass + '">' + statusText + '</span>';
            }
        },
        {
            "name": "ReminderCompletedDate",
            "className": "text-center",
            "render": function (data, type, row) {
                if (row.reminderCompletedDate) {
                    return moment.utc(row.reminderCompletedDate).local().format('ll');
                } else {
                    return '';
                }
            }
        },
        {
            "data": "notes",
            "name": "Notes",
            "className": "text-center",
            "render": function (data, type, row, meta) {
                if (data) {
                    var firstFiveWords = data.split(' ').slice(0, 5).join(' ');
                    var escapedNote = row.notes
                        .replace(/\\/g, '\\\\')
                        .replace(/'/g, "\\'")
                        .replace(/"/g, '\\"')
                        .replace(/\n/g, '\\n')
                        .replace(/\r/g, '\\r');
                    return `
            <span style="cursor: pointer;" onclick="showNote('${escapedNote}')">
                ${firstFiveWords}${data.split(' ').length > 5 ? '...' : ''}
            </span>`;
                }
                return data;
            }
        },
    ];
    columns.push(
        {
            "className": 'text-start',
            "orderable": false,
            "render": function (data, type, row) {
                if (row.isReminderCompleted) {
                    return '';
                }
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
                <a href="javascript:;" class="btn btn-sm btn-light-danger js-confirm"
                   data-title="حذف" data-url="/${tbody.data('controller')}/Delete/${row.id}" data-update="true"
                            data-message="هل متأكد من حذف هذا العنصر؟">
                    حذف
                </a>`;
            }
        });

    var titleFilter = $('#titleSearchId');
    titleFilter.on('change', function () {
        var selectedTitle = $(this).val();
        datatable.search(selectedTitle).draw();
    });

    datatable = $('#PersonalReminders').DataTable({
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
        order: [[4, 'asc']],
        columnDefs: [{
            targets: [0],
            visible: false,
            searchable: false
        }],
        columns: columns,
        "rowCallback": function (row, data) {
            if (!data.reminderDate) return;

            var reminderDate = moment(data.reminderDate);
            var today = moment();
            var diffDays = reminderDate.diff(today, 'days');

            var customNotifiDate = moment(data.customNotifiDate);

            if (diffDays > 3) {
                return $(row).addClass("bg-Default");
            }
            switch (diffDays) {
                case 0:
                    return $(row).addClass("bg-danger");
                case 1:
                    return $(row).addClass("bg-danger");
                case 2:
                    return $(row).addClass("bg-warning-coustom");
                case 3:
                    return $(row).addClass("bg-success");
                default:
                    return $(row).addClass("bg-danger");
            }
        }
    });
});