$(document).ready(function () {
    let selectedEmployeeId = $("#EmployeeDropdown").data("selected-id");

    if (selectedEmployeeId) {
        // Fetch employee name if an ID is set
        $.ajax({
            url: "/Vacation/GetEmployees",
            data: { selectedId: selectedEmployeeId },
            success: function (response) {
                if (response.id) {
                    let option = new Option(response.name, response.id, true, true);
                    $("#EmployeeDropdown").append(option).trigger("change");
                }
            }
        });
    }

    $("#EmployeeDropdown").select2({
        placeholder: "Select an Employee",
   //     minimumInputLength: 2,
        ajax: {
            url: "/Vacation/GetEmployees",
            dataType: "json",
            delay: 250,
            data: function (params) {
                return {
                    search: params.term,
                    page: params.page || 1
                };
            },
            processResults: function (data, params) {
                params.page = params.page || 1;
                return {
                    results: $.map(data.items, function (item) {
                        return {
                            id: item.id,
                            text: item.name
                        };
                    }),
                    pagination: {
                        more: data.hasMore
                    }
                };
            }
        }
    });
});
