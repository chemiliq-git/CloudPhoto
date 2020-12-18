class IUserRelateByUserSearchData  {
    pageSize: number;
    pageIndex: number;
    currentSelectImage: number;
    userId: string;
    type: string;
}

class userRelationByUserCookieHelper extends cookieHelperBase<IUserRelateByUserSearchData> implements IPagingData{

    constructor(userPerPage: number,
        forUserId: string) {

        super("imageRelateByUserData");
        this.cookieData = new IUserRelateByUserSearchData();
        this.cookieData.pageSize = userPerPage;
        this.cookieData.pageIndex = 0;
        this.cookieData.currentSelectImage = 0;
        this.cookieData.userId = forUserId;

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