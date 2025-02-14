var table;
var datatable;
var updatedRow;
var exportedCols = [];

function updateNotificationCount() {
    $.get("/Notification/GetUnreadNotificationCount", function (data) {
        $("#notifiCount").text(data.count);
    });
}
//setInterval(updateNotificationCount, 5000); // Update count every 5 seconds
function showNote(message) {
    Swal.fire({
        title: message,
        confirmButtonText: "تمت القراءه",
        customClass: {
            confirmButton: "btn btn-outline btn-outline-dashed btn-outline-primary btn-active-light-primary"
        }
    });
}

function showSuccessMessage(message = 'اكتمل الحفظ بنجاح') {
    Swal.fire({
        icon: 'success',
        title: message,
        customClass: {
            confirmButton: "btn btn-outline btn-outline-dashed btn-outline-primary btn-active-light-primary"
        }
    });
}

function showErrorMessage(message = 'حدث خطأ') {
    Swal.fire({
        icon: 'error',
        title: message,
        customClass: {
            confirmButton: "btn btn-outline btn-outline-dashed btn-outline-primary btn-active-light-primary"
        }
    });
}

function onModalBegin() {
    $('body :submit').attr('disabled', 'disabled').attr('data-kt-indicator', 'on');
}

function onModalSuccess(item) {
    showSuccessMessage();
    $('#Modal').modal('hide');

    if (updatedRow === undefined) {
        $('tbody').append(item);
    } else {
        $(updatedRow).replaceWith(item);
        updatedRow = undefined;
    }

    KTMenu.init();
    KTMenu.initHandlers();
}

function onModalComplete() {
    $('body :submit').removeAttr('disabled').removeAttr('data-kt-indicator');
}

//DataTables
var headers = $('th');
$.each(headers, function (i) {
    if (!$(this).hasClass('js-no-export'))
        exportedCols.push(i);
});
var KTDatatables = function () {
    // Private functions
    var initDatatable = function () {
        // Init datatable --- more info on datatables: https://datatables.net/manual/
        datatable = $(table).DataTable({
            "info": false,
            'pageLength': 10,
        });
    }

    // Search Datatable --- official docs reference: https://datatables.net/reference/api/search()
    var handleSearchDatatable = () => {
        const filterSearch = document.querySelector('[data-kt-filter="search"]');
        filterSearch.addEventListener('keyup', function (e) {
            datatable.search(e.target.value).draw();
        });
    }

    // Public methods
    return {
        init: function () {
            table = document.querySelector('.js-datatables', {
                scrollX: true,
            });
            if (!table) {
                return;
            }

            initDatatable();
            handleSearchDatatable();
        }
    };
}();

//Select2
function applySelect2() {
    $('.js-select2').select2();
    $('.js-select2').on('select2:select', function (e) {
        $('form').not('#SignOut').validate().element('#' + $(this).attr('id'));
    });
}

$(document).ready(function () {

    updateNotificationCount();

    //SweetAlert
    var message = $('#Message').text();
    if (message !== '') {
        showSuccessMessage(message);
    }

    //Select2
    applySelect2();

    //Datepicker
    $('.js-datepicker').daterangepicker({
        singleDatePicker: true,
        autoApply: true,
        drops: 'auto',
        maxDate: false,
    });
    //DataTables
    KTUtil.onDOMContentLoaded(function () {
        KTDatatables.init();
    });
    //Handle bootstrap modal
    $('body').delegate('.js-render-modal', 'click', function () {
        var btn = $(this);
        var modal = $('#Modal');

        modal.find('#ModalLabel').text(btn.data('title'));

        if (btn.data('update') !== undefined) {
            updatedRow = btn.parents('tr');
        }

        $.get({
            url: btn.data('url'),
            success: function (form) {
                modal.find('.modal-body').html(form);
                $.validator.unobtrusive.parse(modal);
            },
            error: function () {
                showErrorMessage();
            }
        });

        modal.modal('show');
    });

    //Hanlde signout
    $('.js-signout').on('click', function () {
        $('#SignOut').submit();
    });
    //Handle Toggle Status
    $('body').delegate('.js-toggle-status', 'click', function () {
        var btn = $(this);

        bootbox.confirm({
            message: "هل متأكد من تغير حاله المستخدم؟",
            buttons: {
                confirm: {
                    label: 'نعم',
                    className: 'btn-danger'
                },
                cancel: {
                    label: 'لا',
                    className: 'btn-secondary'
                }
            },
            callback: function (result) {
                if (result) {
                    $.post({
                        url: btn.data('url'),
                        data: {
                            '__RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
                        },
                        success: function (lastUpdatedOn) {
                            var row = btn.parents('tr');
                            var status = row.find('.js-status');
                            var newStatus = status.text().trim() === 'غير متاح' ? 'متاح' : 'غير متاح';
                            status.text(newStatus).toggleClass('badge-light-success badge-light-danger');
                            row.find('.js-updated-on').html(lastUpdatedOn);
                            row.addClass('animate__animated animate__flash');

                            showSuccessMessage();
                        },
                        error: function () {
                            showErrorMessage();
                        }
                    });
                }
            }
        });
    });

    //Handle Accepted Status
    $('body').delegate('.js-Accepted-status', 'click', function () {
        var btn = $(this);

        bootbox.confirm({
            message: "هل متأكد من قبول الحاله؟",
            buttons: {
                confirm: {
                    label: 'نعم',
                    className: 'btn-danger'
                },
                cancel: {
                    label: 'لا',
                    className: 'btn-secondary'
                }
            },
            callback: function (result) {
                if (result) {
                    $.post({
                        url: btn.data('url'),
                        data: {
                            '__RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
                        },
                        success: function (lastUpdatedOn) {
                            var row = btn.parents('tr');
                            var status = row.find('.js-status');
                            var newStatus = 'مقبول';
                            status.text(newStatus).removeClass('badge-light-danger badge-light-warning').addClass('badge-light-success');
                            row.find('.js-updated-on').html(lastUpdatedOn);
                            row.addClass('animate__animated animate__flash');

                          //  showSuccessMessage();
                        },
                        error: function () {
                            showErrorMessage();
                        }
                    });
                }
            }
        });
    });

    //Handle Rejected Status
    $('body').delegate('.js-Rejected-status', 'click', function () {
        var btn = $(this);

        bootbox.confirm({
            message: "هل متأكد من رفض الحاله؟",
            buttons: {
                confirm: {
                    label: 'نعم',
                    className: 'btn-danger'
                },
                cancel: {
                    label: 'لا',
                    className: 'btn-secondary'
                }
            },
            callback: function (result) {
                if (result) {
                    $.post({
                        url: btn.data('url'),
                        data: {
                            '__RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
                        },
                        success: function (lastUpdatedOn) {
                            var row = btn.parents('tr');
                            var status = row.find('.js-status');
                            var newStatus = 'مرفوض';
                            status.text(newStatus).removeClass('badge-light-success badge-light-warning').addClass('badge-light-danger');
                            row.find('.js-updated-on').html(lastUpdatedOn);
                            row.addClass('animate__animated animate__flash');

                           // showSuccessMessage();
                        },
                        error: function () {
                            showErrorMessage();
                        }
                    });
                }
            }
        });
    });
    //Handle Confirm
    $('body').delegate('.js-confirm', 'click', function () {
        var btn = $(this);
        var errormassege = btn.data('errormessage');
        bootbox.confirm({
            message: btn.data('message'),
            buttons: {
                confirm: {
                    label: 'نعم',
                    className: 'btn-success'
                },
                cancel: {
                    label: 'لا',
                    className: 'btn-secondary'
                }
            },
            callback: function (result) {
                if (result) {
                    $.post({
                        url: btn.data('url'),
                        data: {
                            '__RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
                        },
                        success: function () {
                            showSuccessMessage();
                            location.reload();
                        },
                        error: function () {
                            showErrorMessage(errormassege);
                        }
                    });
                }
            }
        });

    });

});

/*==========================*/
// ---------Responsive-navbar-active-animation-----------
function test() {
    var tabsNewAnim = $('#navbarSupportedContent');
    var selectorNewAnim = $('#navbarSupportedContent').find('li').length;
    var activeItemNewAnim = tabsNewAnim.find('.active');
    var activeWidthNewAnimHeight = activeItemNewAnim.innerHeight();
    var activeWidthNewAnimWidth = activeItemNewAnim.innerWidth();
    var itemPosNewAnimTop = activeItemNewAnim.position();
    var itemPosNewAnimLeft = activeItemNewAnim.position();
    $(".hori-selector").css({
        "top": itemPosNewAnimTop.top + "px",
        "left": itemPosNewAnimLeft.left + "px",
        "height": activeWidthNewAnimHeight + "px",
        "width": activeWidthNewAnimWidth + "px"
    });
    $("#navbarSupportedContent").on("click", "li", function (e) {
        $('#navbarSupportedContent ul li').removeClass("active");
        $(this).addClass('active');
        var activeWidthNewAnimHeight = $(this).innerHeight();
        var activeWidthNewAnimWidth = $(this).innerWidth();
        var itemPosNewAnimTop = $(this).position();
        var itemPosNewAnimLeft = $(this).position();
        $(".hori-selector").css({
            "top": itemPosNewAnimTop.top + "px",
            "left": itemPosNewAnimLeft.left + "px",
            "height": activeWidthNewAnimHeight + "px",
            "width": activeWidthNewAnimWidth + "px"
        });
    });
}


$(document).ready(function () {
    setTimeout(function () { test(); });
});
$(window).on('resize', function () {
    setTimeout(function () { test(); }, 500);
});
$(".navbar-toggler").click(function () {
    $(".navbar-collapse").slideToggle(300);
    setTimeout(function () { test(); });
});



// --------------add active class-on another-page move----------
jQuery(document).ready(function ($) {
    // Get current path and find target link
    var path = window.location.pathname.split("/").pop();

    // Account for home page with empty path
    if (path == '') {
        path = 'index.html';
    }

    var target = $('#navbarSupportedContent ul li a[href="' + path + '"]');
    // Add active class to target link
    target.parent().addClass('active');
});




// Add active class on another page linked
// ==========================================
// $(window).on('load',function () {
//     var current = location.pathname;
//     console.log(current);
//     $('#navbarSupportedContent ul li a').each(function(){
//         var $this = $(this);
//         // if the current path is like this link, make it active
//         if($this.attr('href').indexOf(current) !== -1){
//             $this.parent().addClass('active');
//             $this.parents('.menu-submenu').addClass('show-dropdown');
//             $this.parents('.menu-submenu').parent().addClass('active');
//         }else{
//             $this.parent().removeClass('active');
//         }
//     })
// });