$(document).ready(function () {
    $("#GovernorateId").on("change", function () {
        var governorateId = $(this).val();
        var areaList = $("#AreaId");

        areaList.empty();
        areaList.append("<option></option>");

        if (governorateId != "") {
            $.ajax({
                url: "/Subscribers/GetAreas?governorateId=" + governorateId,
                success: function (data) {
                    console.log(data);
                    $.each(data, function (i, e) {
                        areaList.append($("<option></option>").attr("value", e.value).text(e.text));
                    });
                },
                error: function () {
                    showErrorMessage();
                }
            });
        }
    });
});