class IMainSearchData {
    pageSize: number;
    pageIndex: number;
    currentSelectImage: number;
    selectCategory: Array<string>;
    searchText: string;
}

interface pagingData {

}

class mainCookieHelper {

    mainCookieName: string = "searchData";
    mainSearchData: IMainSearchData;

    constructor(imagePerPage: number) {
        this.mainSearchData = new IMainSearchData();
        this.mainSearchData.pageSize = imagePerPage;
        this.mainSearchData.pageIndex = 0;
        this.mainSearchData.selectCategory = new Array<string>();
        this.mainSearchData.searchText = "";
    }

    readSearchData(): IMainSearchData {
        var txtSearchData = $.cookie(this.mainCookieName);
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