class IUserSearchData {
    pageSize: number;
    pageIndex: number;
    currentSelectImage: number;
    userId: string;
    type: string;
}

class userCookieHelper {

    userCookieName: string = "pagingData";
    userSearchData: IUserSearchData;

    constructor(imagePerPage: number,
        forUserId: string,
        searchType: string) {
        this.userSearchData = new IUserSearchData();
        this.userSearchData.pageSize = imagePerPage;
        this.userSearchData.pageIndex = 0;
        this.userSearchData.currentSelectImage = 0;
        this.userSearchData.userId = forUserId;
        this.userSearchData.type = searchType;
    }

    readSearchData(): IUserSearchData {
        var txtSearchData = $.cookie(this.userCookieName);
        if (txtSearchData) {
            this.userSearchData = JSON.parse(txtSearchData);
            return this.userSearchData;
        }
        else {
            return this.userSearchData;
        }
    }

    saveSearchData() {
        var txtSearchData = JSON.stringify(this.userSearchData);
        $.cookie(this.userCookieName, txtSearchData, { path: "/" });
    }

    clearSearchData() {
        $.cookie(this.userCookieName, "", { path: "/" });
        this.userSearchData.userId = "";
        this.userSearchData.type = "";
    }
}