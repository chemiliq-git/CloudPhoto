class siteHelper {
    searchControlName: string = '#headerSearchControl';
    searchData: mainImageCookieHelper;

    constructor() {
        this.searchData = new mainImageCookieHelper();
    }

    hookToSearchControlEvents() {
        new myAutocompleteHelper(
            this.onStartAutoCompleteSearch.bind(this),
            this.seachImageByInputData.bind(this),
            this.searchControlName);

        let searchHTLElement: HTMLInputElement = <HTMLInputElement>document.getElementById(this.searchControlName.substring(1));
        searchHTLElement.addEventListener("keydown", function (event: KeyboardEvent) {
            if (event.key === "Enter") {
                if (searchHTLElement.value
                    && searchHTLElement.value.length >= 2) {
                    this.seachImageByInputData();
                    event.preventDefault();
                    return false;
                }
            }
        }.bind(this));
    }

    onStartAutoCompleteSearch() {
        this.searchData.readCookieData();
        var searchValue = $(this.searchControlName).val();
        if (searchValue) {
            this.searchData.cookieData.searchText = $(this.searchControlName).val().toString();
            this.searchData.saveCookieData();
        }
    }

    seachImageByInputData() {
        this.searchData.readCookieData();
        var searchValue = $(this.searchControlName).val();
        if (searchValue) {
            this.searchData.clearCookieData();
            this.searchData.cookieData.searchText = $(this.searchControlName).val().toString();
            this.searchData.saveCookieData();

            console.log("Show images");
            window.location.href = '/Images/Index';
        }
    }
}

$(document).ready(
    function () {
        var helper = new siteHelper();
        helper.hookToSearchControlEvents();
    });