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
                    return firstFiveWords + (data.split(' ').length > 5 ? '...' : '');
                }
                return data; // return data as it is if it's empty or undefined
            }
        },        {
            "name": "Date",
            "className": "text-center",
            "render": function (data, type, row) {
                return moment(row.date).format('ll')
            }
        },
    ];
    columns.push(
        {
            "className": 'text-start',
            "orderable": false,
            "render": function (data, type, row) {
                return `
                 <a href="javascript:;" class="btn btn-sm btn-outline btn-outline-dashed btn-outline-success btn-active-light-success js-render-modal"
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