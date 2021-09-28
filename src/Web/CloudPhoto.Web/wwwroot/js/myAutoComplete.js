var myAutocompleteHelper = /** @class */ (function () {
    function myAutocompleteHelper(startSearchCallback, selectResultCallback, autoCompleteControlName) {
        if (autoCompleteControlName === void 0) { autoCompleteControlName = '#searchImageTag'; }
        this.startSearchCallback = startSearchCallback;
        this.selectResultCallback = selectResultCallback;
        this.autoCompleteControlName = autoCompleteControlName;
        this.configAutoCompleteTags();
    }
    myAutocompleteHelper.prototype.configAutoCompleteTags = function () {
        var context = this;
        $(this.autoCompleteControlName).attr("autocomplete", "off");
        $(this.autoCompleteControlName).keyup(function (event) {
            var token = $("#keyForm input[name=__RequestVerificationToken]").val();
            var searchText = $(context.autoCompleteControlName).val().toString();
            if (searchText.length < 2) {
                return;
            }
            var formData = new FormData();
            formData.append("searchData", searchText);
            if (context.startSearchCallback) {
                context.startSearchCallback(searchText);
            }
            $.ajax({
                url: '/tags/AutoCompleteSearch',
                data: formData,
                processData: false,
                contentType: false,
                type: 'POST',
                headers: {
                    'X-CSRF-TOKEN': token.toString(),
                },
                success: function (data) {
                    var availableData = [];
                    data.forEach(function (element) {
                        availableData.push({ id: element.id, label: element.name });
                    });
                    $(context.autoCompleteControlName).autocomplete({
                        source: availableData,
                        select: function (event, ui) {
                            $(context.autoCompleteControlName).val(ui.item.value);
                            if (context.selectResultCallback) {
                                context.selectResultCallback(ui.item);
                            }
                            event.returnValue = false;
                            return false;
                        }
                    });
                },
                error: function (data) {
                    alert("Error with autocomplite functionality");
                }
            });
        });
    };
    return myAutocompleteHelper;
}());
//# sourceMappingURL=myAutoComplete.js.map