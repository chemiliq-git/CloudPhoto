class IImageRelateByUserSearchData  {
    pageSize: number;
    pageIndex: number;
    currentSelectImage: number;
    userId: string;
    type: string;
}

class imageRelationByUserCookieHelper extends cookieHelperBase<IImageRelateByUserSearchData> implements IPagingData{

    constructor(imagePerPage: number,
        forUserId: string,
        searchType: string) {

        super("imageRelateByUserData");
        this.cookieData = new IImageRelateByUserSearchData();
        this.cookieData.pageSize = imagePerPage;
        this.cookieData.pageIndex = 0;
        this.cookieData.currentSelectImage = 0;
        this.cookieData.userId = forUserId;
        this.cookieData.type = searchType;

        this.saveCookieData();
    }

    clearCookieData() {
        this.cookieData.userId = "";
        this.cookieData.type = "";
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