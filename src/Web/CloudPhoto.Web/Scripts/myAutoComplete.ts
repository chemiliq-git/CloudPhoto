interface ServerResponseData {
    id: string;
    name: string;
}

interface AutocompleteParam {
    id: string;
    value: string;
}

class myAutocompleteHelper {

    startSearchCallback: (searchText: string) => any;
    selectResultCallback: (selectData: AutocompleteParam) => any;
    autoCompleteControlName: string;

    constructor(
        startSearchCallback: (searchText: string) => any,
        selectResultCallback: (selectData: AutocompleteParam) => any,
        autoCompleteControlName: string = '#searchImageTag') {
        this.startSearchCallback = startSearchCallback;
        this.selectResultCallback = selectResultCallback;
        this.autoCompleteControlName = autoCompleteControlName;

        this.configAutoCompleteTags();
    }

    configAutoCompleteTags() {
        let context: myAutocompleteHelper = this;
        $(this.autoCompleteControlName).keyup(function (event) {
            var token = $("#keyForm input[name=__RequestVerificationToken]").val();

            let searchText = $(context.autoCompleteControlName).val().toString()
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
                success: function (data: Array<ServerResponseData>) {
                    var availableData = [];
                    data.forEach((element) => {
                        availableData.push({ id: element.id, label: element.name });
                    });
                    $(context.autoCompleteControlName).autocomplete({
                        source: availableData,
                        select: function (event, ui) {
                            $(context.autoCompleteControlName).val(ui.item.value);
                            if (context.selectResultCallback) {
                                context.selectResultCallback(<AutocompleteParam>ui.item);
                            }
                            event.returnValue = false;
                            return false;
                        }
                    });
                },
                error: function (data) {
                    alert(data);
                }
            });
        });
    }
}