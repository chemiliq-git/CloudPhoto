var siteHelper = /** @class */ (function () {
    function siteHelper() {
        this.searchData = new mainCookieHelper(6);
    }
    siteHelper.prototype.hookToSearchControlEvents = function () {
        var autoCompleateHelper = new myAutocompleteHelper();
        autoCompleateHelper.configAutoCompleteTags(this.onStartAutoCompleteSearch.bind(this), this.onStartAutoCompleteSearch.bind(this), '#headerSearchControl');
    };
    siteHelper.prototype.onStartAutoCompleteSearch = function () {
        this.searchData.readSearchData();
        var searchValue = $('#headerSearchControl').val();
        if (searchValue) {
            this.searchData.mainSearchData.searchText = $('#headerSearchControl').val().toString();
            this.searchData.saveSearchData();
            var data1 = this.searchData.readSearchData();
        }
    };
    return siteHelper;
}());
$(document).ready(function () {
    var helper = new siteHelper();
    helper.hookToSearchControlEvents();
});
//# sourceMappingURL=site.js.map