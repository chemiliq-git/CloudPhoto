var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (Object.prototype.hasOwnProperty.call(b, p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
var IUserRelateByUserSearchData = /** @class */ (function () {
    function IUserRelateByUserSearchData() {
    }
    return IUserRelateByUserSearchData;
}());
var userRelationByUserCookieHelper = /** @class */ (function (_super) {
    __extends(userRelationByUserCookieHelper, _super);
    function userRelationByUserCookieHelper(userPerPage, forUserId) {
        var _this = _super.call(this, "imageRelateByUserData") || this;
        _this.cookieData = new IUserRelateByUserSearchData();
        _this.cookieData.pageSize = userPerPage;
        _this.cookieData.pageIndex = 0;
        _this.cookieData.currentSelectImage = 0;
        _this.cookieData.userId = forUserId;
        _this.saveCookieData();
        return _this;
    }
    userRelationByUserCookieHelper.prototype.clearCookieData = function () {
        this.cookieData.userId = "";
        this.cookieData.type = "";
        _super.prototype.saveCookieData.call(this);
    };
    //region Implement IPagingData
    userRelationByUserCookieHelper.prototype.readPagingData = function () {
        return this.cookieData;
    };
    userRelationByUserCookieHelper.prototype.onChangePageIndex = function (pageIndex) {
        this.cookieData.pageIndex = pageIndex;
        this.saveCookieData();
    };
    return userRelationByUserCookieHelper;
}(cookieHelperBase));
//# sourceMappingURL=userRalateByUserCookieData.js.map