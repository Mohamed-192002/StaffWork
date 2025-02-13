$(document).ready(function () {
    var tbody = $('#Employees').find('tbody');
    $('[data-kt-filter="search"]').on('keyup', function () {
        var input = $(this);
        datatable.search(this.value).draw();
    });

    let columns = [
        { "data": "id", "name": "Id", "className": "d-none" },
        { "data": "fullName", "name": "FullName", "className": "text-center" },
        { "data": "court", "name": "Court", "className": "text-center" },
        { "data": "appeal", "name": "Appeal", "className": "text-center" },
    ];
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

    datatable = $('#Employees').DataTable({
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