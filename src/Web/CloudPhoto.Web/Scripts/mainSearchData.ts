class iMainSearchData {
    pageSize: number;
    pageIndex: number;
    currentSelectImage: number;
    selectCategory: Array<string>;
    searchText: string;
}

class mainCookieHelper {

    mainCookieName: string = "searchData";
    mainSearchData: iMainSearchData;

    constructor(imagePerPage: number) {
        this.mainSearchData = new iMainSearchData();
        this.mainSearchData.pageSize = imagePerPage;
        this.mainSearchData.pageIndex = 0;
        this.mainSearchData.selectCategory = new Array<string>();
        this.mainSearchData.searchText = "";
    }

    readSearchData(): iMainSearchData {
        var txtSearchData = $.cookie("searchData");
        if (txtSearchData) {
            this.mainSearchData = JSON.parse(txtSearchData);
            return this.mainSearchData;
        }
        else {
            return this.mainSearchData;
        }
    }

    saveSearchData() {
        var txtSearchData = JSON.stringify(this.mainSearchData);
        $.cookie(this.mainCookieName, txtSearchData, { path: "/" });
    }

    clearSearchData() {
        $.cookie(this.mainCookieName, "", { path: "/" });
        this.mainSearchData.selectCategory = new Array<string>();
        this.mainSearchData.searchText = "";
    }
}