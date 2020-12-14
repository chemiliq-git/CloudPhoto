class siteHelper {
    searchControlName: string = '#headerSearchControl';
    searchData: mainCookieHelper;

    constructor() {
        this.searchData = new mainCookieHelper(6);
    }

    hookToSearchControlEvents() {
        var autoCompleateHelper: myAutocompleteHelper = new myAutocompleteHelper();
        autoCompleateHelper.configAutoCompleteTags(
            this.onStartAutoCompleteSearch.bind(this),
            this.seachImageByInputData.bind(this),
            this.searchControlName);

        let searchHTLElement: HTMLInputElement = <HTMLInputElement>document.getElementById(this.searchControlName);
        searchHTLElement.addEventListener("keydown", function (event) {
            if (event.key === "Enter") {
                if (searchHTLElement.value
                    && searchHTLElement.value.length >= 2) {
                    this.seachImageByInputData();
                }
            }
        }.bind(this));
    }

    onStartAutoCompleteSearch() {
        this.searchData.readSearchData();
        var searchValue = $(this.searchControlName).val();
        if (searchValue) {
            this.searchData.mainSearchData.searchText = $(this.searchControlName).val().toString();
            this.searchData.saveSearchData();
        }
    }

    seachImageByInputData() {
        this.searchData.readSearchData();
        var searchValue = $(this.searchControlName).val();
        if (searchValue) {
            this.searchData.clearSearchData();
            this.searchData.mainSearchData.searchText = $(this.searchControlName).val().toString();
            this.searchData.saveSearchData();

            window.location.href = '/Images/Index'; var data1 = this.searchData.readSearchData();
        }
    }
}

$(document).ready(
    function () {
        var helper = new siteHelper();
        helper.hookToSearchControlEvents();
    });