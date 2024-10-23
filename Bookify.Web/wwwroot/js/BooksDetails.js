function onAddCopySuccess(row) {
    showSuccessMessage();
    $("#Modal").modal("hide");
    $("tbody").prepend(row);

    var count = $("#CopiesCount");
    var newCount = parseInt(count.text()) + 1;
    count.text(newCount);

    $(".js-alert").addClass("d-none");
    $(".table-responsive").removeClass("d-none");

    KTMenu.createInstances();
}

function onEditCopySuccess(row) {
    showSuccessMessage();
    $("#Modal").modal("hide");

    $(updatedRow).replaceWith(row);
    KTMenu.createInstances();
}