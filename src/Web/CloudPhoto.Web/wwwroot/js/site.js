var siteHelper = /** @class */ (function () {
    function siteHelper() {
        this.searchControlName = '#headerSearchControl';
        this.searchData = new mainCookieHelper();
    }
    siteHelper.prototype.hookToSearchControlEvents = function () {
        var autoCompleateHelper = new myAutocompleteHelper();
        autoCompleateHelper.configAutoCompleteTags(this.onStartAutoCompleteSearch.bind(this), this.seachImageByInputData.bind(this), this.searchControlName);
        var searchHTLElement = document.getElementById(this.searchControlName.substring(1));
        searchHTLElement.addEventListener("keydown", function (event) {
            if (event.key === "Enter") {
                if (searchHTLElement.value
                    && searchHTLElement.value.length >= 2) {
                    this.seachImageByInputData();
                }
            }
        }.bind(this));
    };
    siteHelper.prototype.onStartAutoCompleteSearch = function () {
        this.searchData.readSearchData();
        var searchValue = $(this.searchControlName).val();
        if (searchValue) {
            this.searchData.mainSearchData.searchText = $(this.searchControlName).val().toString();
            this.searchData.saveSearchData();
        }
    };
    siteHelper.prototype.seachImageByInputData = function () {
        this.searchData.readSearchData();
        var searchValue = $(this.searchControlName).val();
        if (searchValue) {
            this.searchData.clearSearchData();
            this.searchData.mainSearchData.searchText = $(this.searchControlName).val().toString();
            this.searchData.saveSearchData();
            window.location.href = '/Images/Index';
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