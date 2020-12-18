class IMainImageSearchData {
    pageSize: number;
    pageIndex: number;
    currentSelectImage: number;
    selectCategory: Array<string>;
    searchText: string;
}

class mainImageCookieHelper extends cookieHelperBase<IMainImageSearchData> implements IPagingData {

    constructor() {
        super("searchData");
    } 

    initCookieData(imagePerPage: number) {
        this.cookieData = new IMainImageSearchData();
        this.cookieData.pageSize = imagePerPage;
        this.cookieData.pageIndex = 0;
        this.cookieData.currentSelectImage = 0;
        this.cookieData.selectCategory = new Array<string>();
        this.cookieData.searchText = "";
        this.saveCookieData();
    }

    clearCookieData() {
        this.cookieData.selectCategory = new Array<string>();
        this.cookieData.searchText = "";
        this.cookieData.currentSelectImage = 0;
        this.cookieData.pageIndex = 0;
        super.saveCookieData();
    }

    //region Implement IPagingData
    readPagingData(): any {
        return this.cookieData;
    }


    onChangePageIndex(pageIndex: number) {
        this.cookieData.pageIndex = pageIndex;
        this.saveCookieData();
    }
    //region Implement IPagingData
}