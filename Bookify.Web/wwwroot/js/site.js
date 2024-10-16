﻿var table;
var datatable;
var updatedRow;
var exportedCols = [];

function showSuccessMessage(message = "Saved Successfully!") {
    Swal.fire({
        icon: "success",
        title: "Success",
        text: message,
        customClass: {
            confirmButton: "btn btn-primary"
        }
    });
}

function showErrorMessage(message = "Something went wrong!") {
    Swal.fire({
        icon: "error",
        title: "Oops...",
        text: message,
        customClass: {
            confirmButton: "btn btn-primary"
        }
    });
}

function onModalBegin() {
    $("body form :submit").attr("disabled", "disabled").attr("data-kt-indicator", "on");
}

function onModalSuccess(row) {
    showSuccessMessage();
    $("#Modal").modal("hide");

    if (updatedRow != undefined) {
        datatable.row(updatedRow).remove().draw();
        updatedRow = undefined;
    }

    var newRow = $(row);
    datatable.row.add(newRow).draw();

    // KTMenu.init();
}

function onModalComplete() {
    $("body form :submit").removeAttr("disabled").removeAttr("data-kt-indicator");
}

// DataTables
// 1 - Class Definition
var KTDatatables = function () {
    // Private functions
    var initDatatable = function () {
        // Init datatable --- more info on datatables: https://datatables.net/manual/
        datatable = $(table).DataTable({
            "info": false,
            'pageLength': 5,
        });
    }

    // Hook export buttons
    var exportButtons = () => {
        const documentTitle = $(".js-datatables").data("document-title");
        var buttons = new $.fn.dataTable.Buttons(table, {
            buttons: [
                {
                    extend: 'copyHtml5',
                    title: documentTitle,
                    exportOptions: {
                        columns: exportedCols
                    }
                },
                {
                    extend: 'excelHtml5',
                    title: documentTitle,
                    exportOptions: {
                        columns: exportedCols
                    }
                },
                {
                    extend: 'csvHtml5',
                    title: documentTitle,
                    exportOptions: {
                        columns: exportedCols
                    }
                },
                {
                    extend: 'pdfHtml5',
                    title: documentTitle,
                    exportOptions: {
                        columns: exportedCols
                    }
                }
            ]
        }).container().appendTo($('#kt_datatable_example_buttons'));

        // Hook dropdown menu click event to datatable export buttons
        const exportButtons = document.querySelectorAll('#kt_datatable_example_export_menu [data-kt-export]');
        exportButtons.forEach(exportButton => {
            exportButton.addEventListener('click', e => {
                e.preventDefault();

                // Get clicked export value
                const exportValue = e.target.getAttribute('data-kt-export');
                const target = document.querySelector('.dt-buttons .buttons-' + exportValue);

                // Trigger click event on hidden datatable export buttons
                target.click();
            });
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
            table = document.querySelector('.js-datatables');

            if (!table) {
                return;
            }

            initDatatable();
            exportButtons();
            handleSearchDatatable();
        }
    };
}();

// 2 - Exclude Non Exported Cols
var headers = $("th");
$.each(headers, (i, e) => {
    if (!$(e).hasClass("js-no-export"))
        exportedCols.push(i);
});

$(document).ready(function () {
    // SweetAlert
    var message = $("#Message").text();
    if (message !== '') {
        showSuccessMessage();
    }

    // DataTables
     KTDatatables.init();

    // Handle Modal
    $("body").delegate(".js-render-modal", "click", function () {
        var btn = $(this);
        var modal = $("#Modal");

        if (btn.data("update") !== undefined) {
            updatedRow = btn.parents("tr");
        }

        modal.find(".modal-title").text(btn.data("title"));

        $.get({
            url: btn.data("url"),
            success: function (form) {
                modal.find(".modal-body").html(form);
                $.validator.unobtrusive.parse(modal);
            },
            error: function () {
                showErrorMessage();
            }
        });
        modal.modal("show");
    });

    // Handle Toggle Status
    $("body").delegate(".js-toggle-status", "click", function () {
        var btn = $(this);

        bootbox.confirm({
            message: 'Are you sure that you need to toggle this item status?',
            buttons: {
                confirm: {
                    label: 'Yes',
                    className: 'btn-danger'
                },
                cancel: {
                    label: 'No',
                    className: 'btn-secondary'
                }
            },
            callback: function (result) {
                if (result) {
                    $.post({
                        url: btn.data("url"),
                        data: {
                            "__RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val()
                        },
                        success: function (data) {
                            var row = btn.parents("tr");
                            var statusBadge = row.find(".js-status-badge");
                            var newStatus = statusBadge.text().trim() === "Available" ? "Deleted" : "Available";
                            statusBadge.text(newStatus).toggleClass("badge-light-success badge-light-danger");
                            row.find(".js-last-updated-on").html(data);
                            row.addClass("animate__animated animate__fadeIn");
                            row.on('animationend', (e) => {
                                $(e.target).removeClass("animate__animated animate__fadeIn");
                            });
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

    // Listen for draw event to reinitialize KTMenu
    $(table).on('draw.dt', function () {
        // Reinitialize KTMenu for dynamically added rows
        $('[data-kt-menu="true"]').each(function () {
            var menuElement = this;
            if (KTMenu.getInstance(menuElement)) {
                KTMenu.getInstance(menuElement).update(); // Update existing menu instance
            } else {
                KTMenu.createInstances(); // Initialize new menu instances
            }
        });
    });
});