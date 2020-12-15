var IUserSearchData = /** @class */ (function () {
    function IUserSearchData() {
    }
    return IUserSearchData;
}());
var userCookieHelper = /** @class */ (function () {
    function userCookieHelper(imagePerPage, forUserId, searchType) {
        this.userCookieName = "pagingData";
        this.userSearchData = new IUserSearchData();
        this.userSearchData.pageSize = imagePerPage;
        this.userSearchData.pageIndex = 0;
        this.userSearchData.currentSelectImage = 0;
        this.userSearchData.userId = forUserId;
        this.userSearchData.type = searchType;
    }
    userCookieHelper.prototype.readSearchData = function () {
        var txtSearchData = $.cookie(this.userCookieName);
        if (txtSearchData) {
            this.userSearchData = JSON.parse(txtSearchData);
            return this.userSearchData;
        }
        else {
            return this.userSearchData;
        }
    };
    userCookieHelper.prototype.saveSearchData = function () {
        var txtSearchData = JSON.stringify(this.userSearchData);
        $.cookie(this.userCookieName, txtSearchData, { path: "/" });
    };
    userCookieHelper.prototype.clearSearchData = function () {
        $.cookie(this.userCookieName, "", { path: "/" });
        this.userSearchData.userId = "";
        this.userSearchData.type = "";
    };
    return userCookieHelper;
}());
//# sourceMappingURL=userSearchData.js.map