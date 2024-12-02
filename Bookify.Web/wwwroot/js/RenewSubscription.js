$(document).ready(function () {
    $(".js-renew").on("click", function () {
        var subscriberKey = $(this).data("key");

        bootbox.confirm({
            message: 'Are you sure that you need to renew this subscription?',
            buttons: {
                confirm: {
                    label: 'Yes',
                    className: 'btn-success'
                },
                cancel: {
                    label: 'No',
                    className: 'btn-secondary'
                }
            },
            callback: function (result) {
                if (result) {
                    console.log("Ok");
                    $.post({
                        url: `/Subscribers/RenewSubscription?sKey=${subscriberKey}`,
                        data: {
                            "__RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val()
                        },
                        success: function (data) {
                            
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
});