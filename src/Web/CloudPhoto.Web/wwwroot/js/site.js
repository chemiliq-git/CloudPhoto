var siteHelper = /** @class */ (function () {
    function siteHelper() {
        this.searchControlName = '#headerSearchControl';
        this.searchData = new mainImageCookieHelper();
    }
    siteHelper.prototype.hookToSearchControlEvents = function () {
        new myAutocompleteHelper(this.onStartAutoCompleteSearch.bind(this), this.seachImageByInputData.bind(this), this.searchControlName);
        var searchHTLElement = document.getElementById(this.searchControlName.substring(1));
        searchHTLElement.addEventListener("keydown", function (event) {
            if (event.key === "Enter") {
                if (searchHTLElement.value
                    && searchHTLElement.value.length >= 2) {
                    this.seachImageByInputData();
                    event.preventDefault();
                    return false;
                }
            }
        }.bind(this));
    };
    siteHelper.prototype.onStartAutoCompleteSearch = function () {
        this.searchData.readCookieData();
        var searchValue = $(this.searchControlName).val();
        if (searchValue) {
            this.searchData.cookieData.searchText = $(this.searchControlName).val().toString();
            this.searchData.saveCookieData();
        }
    };
    siteHelper.prototype.seachImageByInputData = function () {
        this.searchData.readCookieData();
        var searchValue = $(this.searchControlName).val();
        if (searchValue) {
            this.searchData.clearCookieData();
            this.searchData.cookieData.searchText = $(this.searchControlName).val().toString();
            this.searchData.saveCookieData();
            console.log("Show images");
            window.location.href = '/Images/Index';
        }
    };
    return siteHelper;
}());
$(document).ready(function () {
    var helper = new siteHelper();
    helper.hookToSearchControlEvents();
});
//# sourceMappingURL=site.js.map