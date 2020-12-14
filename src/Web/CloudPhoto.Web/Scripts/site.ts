class siteHelper {
    searchData: mainCookieHelper;

    constructor() {
        this.searchData = new mainCookieHelper(6);
    }

    hookToSearchControlEvents() {
        var autoCompleateHelper: myAutocompleteHelper = new myAutocompleteHelper();
        autoCompleateHelper.configAutoCompleteTags(
            this.onStartAutoCompleteSearch.bind(this),
            this.onStartAutoCompleteSearch.bind(this),
            '#headerSearchControl');
    }

    onStartAutoCompleteSearch() {
        this.searchData.readSearchData();
        var searchValue = $('#headerSearchControl').val();
        if (searchValue) {
            this.searchData.mainSearchData.searchText = $('#headerSearchControl').val().toString();
            this.searchData.saveSearchData();

            var data1 = this.searchData.readSearchData();
        }
    }
}

$(document).ready(
    function () {
        var helper = new siteHelper();
        helper.hookToSearchControlEvents();
    });