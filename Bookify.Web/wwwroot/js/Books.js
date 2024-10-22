$(document).ready(function () {
    $('[data-kt-filter="search"]').on('keyup', function () {
        var input = $(this); 
        datatable.search(input.val()).draw();
    });

    datatable = $("#Books").DataTable({
        serverSide: true,
        processing: true, // Important
        stateSave: true,
        pageLength: 5,
        order: [[1, "asc"]],
        drawCallback: function () {
            KTMenu.createInstances();
        },
        ajax: {
            url: "/Books/GetBooks",
            type: "POST"
        },
        columnDefs: [
            {
                targets: [0],
                visible: false,
                searchable: false
            }
        ],
        columns: [
            { "data": "id", "name": "Id", "className": "d-none"},
            {
                "name": "Title",
                "className": "d-flex align-items-center",
                render: function (data, type, row) {
                    return `<div class="symbol symbol-50px overflow-hidden me-3">
							    <a href="/books/Details/${row.id}">
								    <div class="symbol-label h-70px"> 
									    <img src="${(row.imageThumbnailUrl == null ? "/images/books/no-book.jpg" : row.imageThumbnailUrl)}" alt="cover" class="w-100">
								    </div>
							    </a>
						    </div>
                            <div class="d-flex flex-column">
								<a href="/books/Details/${row.id}" class="text-primary fw-bolder mb-1">${row.title}</a>
								<span>${row.author}</span>
							</div>`;
                }
            },
            { "data": "publisher", "name": "Publisher"},
            {
                "name": "PublishingDate",
                render: function (data, type, row) {
                    return moment(row.publishingDate).format("ll");
                }
            },
            { "data": "hall", "name": "Hall" },
            { "data": "categories", "name": "Categories", "orderable": false },
            {
                "name": "IsAvailableForRental",
                render: function (data, type, row) {
                    return `<span class="badge badge-light-${(row.isAvailableForRental ? "success" : "warning")}">
                                ${(row.isAvailableForRental ? "Available" : "Not Available")}
                            </span>`;
                }
            },
            {
                "name": "IsDeleted",
                render: function (data, type, row) {
                    return `<span class="badge badge-light-${(row.isDeleted ? "danger" : "success")} js-status-badge">
                                ${(row.isDeleted ? "Deleted" : "Available")}
                            </span>`;
                }
            },
            {
                "orderable": false,
                render: function (data, type, row) {
                    return `<!--begin::Menu-->
                            <button class="btn btn-sm btn-icon btn-bg-light btn-active-light-primary" data-kt-menu-trigger="click" data-kt-menu-placement="bottom-end" data-kt-menu-overflow="true" data-kt-menu="true">
                                <i class="ki-solid ki-setting-4 fs-1"></i>
                            </button>
                            <div class="menu menu-sub menu-sub-dropdown menu-column menu-rounded menu-gray-800 menu-state-bg-light-primary fw-semibold w-200px py-3" data-kt-menu="true">
                                <!--begin::Menu item-->
                                <div class="menu-item px-3">
                                    <a href="/Books/Edit/${row.id}" class="menu-link px-3">
                                        Edit
                                    </a>
                                </div>
                                <!--end::Menu item-->
                                <!--begin::Menu item-->
                                <div class="menu-item px-3">
                                    <a href="JavaScript:;" class="menu-link px-3 js-toggle-status" data-url="/Books/ToggleStatus/${row.id}">
                                        Toggle Status
                                    </a>
                                </div>
                                <!--end::Menu item-->
                            </div>
                            <!--end::Menu-->`;
                }
            }
        ]
    });
});