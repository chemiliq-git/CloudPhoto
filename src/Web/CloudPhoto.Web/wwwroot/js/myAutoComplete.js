// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function configAutoCompleteTags(
    startSearchCallback,
    selectResultCallback,
    autoCompleteControlName = '#searchImageTag') {
    $(autoCompleteControlName).keyup(function (event) {
        var token = $("#keyForm input[name=__RequestVerificationToken]").val();

        var formData = new FormData();
        formData.append("searchData", $(autoCompleteControlName).val());

        if (startSearchCallback) {
            startSearchCallback($(autoCompleteControlName).val())
        }

        $(autoCompleteControlName).autocomplete(
            {
                scroll: true,
                selectFirst: false,
                autoFocus: false,
                source: function (request, response) {
                    $.ajax(
                        {
                            type: "POST",
                            data: formData,
                            url: "/tags/autocompletesearch",
                            processData: false,
                            contentType: false,
                            headers: { 'X-CSRF-TOKEN': token },
                            success: function (data) {
                                response($.map(data, function (item) {
                                    return {
                                        label: item.name,
                                        val: item.id
                                    }
                                }));
                            },
                            error: function (result) { }
                        });
                },
                minLength: 2,
                select: function (event, ui) {
                    $(autoCompleteControlName).val(ui.item.value);
                    if (selectResultCallback) {
                        selectResultCallback(ui.item);
                    }
                    event.returnValue = false;
                    event.cancel = true;

                    return false;
                }
            });
    });
}