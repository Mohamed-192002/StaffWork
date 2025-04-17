$(document).ready(function () {
    var tbody = $('#WorkDailys').find('tbody');
    $('[data-kt-filter="search"]').on('keyup', function () {
        var input = $(this);
        datatable.search(this.value).draw();
    });

    let columns = [
        { "data": "id", "name": "Id", "className": "d-none" },
        { "data": "fullName", "name": "FullName", "className": "text-center" },
        { "data": "deptName", "name": "DeptName", "className": "text-center" },
        { "data": "workType", "name": "WorkType", "className": "text-center" },
       {
    "data": "note",
    "name": "Note",
    "className": "text-center",
    "render": function (data, type, row, meta) {
        if (data) {
            // Split the note into words and take the first 5 words
            var firstFiveWords = data.split(' ').slice(0, 5).join(' ');

            // Escape single quotes in the note to prevent breaking the string
            var escapedNote = row.note
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
        {
            "name": "Date",
            "className": "text-center",
            "render": function (data, type, row) {
                return moment(row.date).format('ll')
            }
        },
        {
            "data": "status", "name": "Status", "className": "text-center"
            ,
            "render": function (data, type, row) {
                // Mapping status to Arabic values
                let statusText = "";
                let badgeClass = "";

                if (data === "Pending" || data === 0) {
                    statusText = "قيد الانتظار";
                    badgeClass = "warning";
                } else if (data === "Accepted" || data === 1) {
                    statusText = "مقبول";
                    badgeClass = "success";
                } else if (data === "Rejected" || data === 2) {
                    statusText = "مرفوض";
                    badgeClass = "danger";
                }

                // Return the rendered HTML with the badge and status text
                return '<span class="js-status fs-5 badge badge-light-' + badgeClass + '">' + statusText + '</span>';
            }
        },
        {
            "data": "isCompleted", "name": "IsCompleted", "className": "text-center"
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
                return '<span class="js-complete fs-5 badge badge-light-' + badgeClass + '">' + statusText + '</span>';
            }
        },
        {
            "name": "CompletionDate",
            "className": "text-center",
            "render": function (data, type, row) {
                if (row.completionDate) {
                    return moment.utc(row.completionDate).local().format('ll'); // تحويل من UTC إلى Local
                } else {
                    return ''; // لو مفيش تاريخ
                }
            }
        },
        { "data": "timeDifferenceFormatted", "name": "TimeDifferenceFormatted", "className": "text-center" },
    ];
    //if (isSuperAdminOrAdmin === false) {
    columns.push(
        {
            "className": 'text-start',
            "orderable": false,
            "render": function (data, type, row) {
                return `
                            <a href="javascript:;" class="btn btn-sm btn-outline btn-outline-dashed btn-outline-warning btn-active-light-warning js-Complete-status"
                               data-title="انجز" data-url="/${tbody.data('controller')}/Complete/${row.id}" data-update="true" data-message="هل متأكد من انجز الحاله؟">
                               انجز
                            </a>
                           `;
            }
        });
    //}
    if (isSuperAdminOrAdmin === true) {
        columns.push(
            {
                "className": 'text-start',
                "orderable": false,
                "render": function (data, type, row) {
                    return `
                            <a href="javascript:;" class="btn btn-sm btn-outline btn-outline-dashed btn-outline-success btn-active-light-success js-Accepted-status"
                               data-title="قبول" data-url="/${tbody.data('controller')}/AcceptStatus/${row.id}" data-update="true" data-message="هل متأكد من قبول الحاله؟">
                               قبول
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
                 <a href="javascript:;" class="btn btn-sm btn-outline btn-outline-dashed btn-outline-danger btn-active-light-danger js-Rejected-status"
                   data-title="رفض" data-url="/${tbody.data('controller')}/RejectedStatus/${row.id}" data-update="true" data-message="هل متأكد من رفض الحاله؟">
                    رفض 
                </a>`;
                }
            });
        columns.push(
            {
                "className": 'text-start',
                "orderable": false,
                "render": function (data, type, row) {
                    return `
                <a href="javascript:;" class="btn btn-sm btn-outline btn-outline-dashed btn-outline-info btn-active-light-info js-confirm"
                   data-title="حذف" data-url="/${tbody.data('controller')}/Delete/${row.id}" data-update="true"
                            data-message="هل متأكد من حذف هذا العنصر؟">
                    حذف 
                </a> `;
                }
            });
    }

    columns.push(
        {
            "className": 'text-start',
            "orderable": false,
            "render": function (data, type, row) {
                return `
                 <a href="javascript:;" class="btn btn-sm btn-outline btn-outline-dashed btn-outline-primary btn-active-light-primary js-render-modal"
                   data-title="تعديل" data-url="/${tbody.data('controller')}/Edit/${row.id}" data-update="true">
                    تعديل 
                </a>`;
            }
        });
  

    // Handle title filter change event
    var titleFilter = $('#titleSearchId');
    titleFilter.on('change', function () {
        var selectedTitle = $(this).val();
        datatable.search(selectedTitle).draw();
    });

    datatable = $('#WorkDailys').DataTable({
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