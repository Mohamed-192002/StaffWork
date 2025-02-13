$(document).ready(function () {
    var tbody = $('#Vacations').find('tbody');
    $('[data-kt-filter="search"]').on('keyup', function () {
        var input = $(this);
        datatable.search(this.value).draw();
    });

    let columns = [
        { "data": "id", "name": "Id", "className": "d-none" },
        { "data": "employeeName", "name": "EmployeeName", "className": "text-center" },
        { "data": "vacationType", "name": "VacationType", "className": "text-center" },
        {
            "name": "StartDate",
            "className": "text-center",
            "render": function (data, type, row) {
                return moment(row.startDate).format('ll')
            }
        },
        { "data": "vacationDays", "name": "VacationDays", "className": "text-center" },
        {
            "data": "description",
            "name": "Description",
            "className": "text-center",
            "render": function (data, type, row, meta) {
                if (data) {
                    // Split the Description into words and take the first 5 words
                    var firstFiveWords = data.split(' ').slice(0, 5).join(' ');

                    // Escape single quotes in the Description to prevent breaking the string
                    var escapedDescription = row.description
                        .replace(/\\/g, '\\\\')  // Escape backslashes
                        .replace(/'/g, "\\'")    // Escape single quotes
                        .replace(/"/g, '\\"')    // Escape double quotes
                        .replace(/\n/g, '\\n')   // Escape newlines
                        .replace(/\r/g, '\\r');  // Escape carriage returns
                    // Create a clickable element for the first five words
                    return `
            <span style="cursor: pointer;" onclick="showDescription('${escapedDescription}')">
                ${firstFiveWords}${data.split(' ').length > 5 ? '...' : ''}
            </span>`;
                }
                return data; // return data as it is if it's empty or undefined
            }
        },
        { "data": "isReturned", "name": "IsReturned", "className": "text-center" },

        {
            "name": "ReturnedDate",
            "className": "text-center",
            "render": function (data, type, row) {
                if (row.returnedDate) {
                    return moment(row.returnedDate).format('ll')
                }
                return "";
            }
        }
    ];
    columns.push(
        {
            "className": 'text-start',
            "orderable": false,
            "render": function (data, type, row) {
                return `
                 <a href="/${tbody.data('controller')}/Edit/${row.id}" class="btn btn-sm btn-outline btn-outline-dashed btn-outline-primary btn-active-light-primary"
                   data-title="تعديل" data-update="true">
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





    // Handle title filter change event
    var titleFilter = $('#titleSearchId');
    titleFilter.on('change', function () {
        var selectedTitle = $(this).val();
        datatable.search(selectedTitle).draw();
    });

    datatable = $('#Vacations').DataTable({
        serverSide: true,
        processing: true,
        stateSave: false,
        paging: true,
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