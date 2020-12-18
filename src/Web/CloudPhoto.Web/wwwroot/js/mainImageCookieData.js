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
var IMainImageSearchData = /** @class */ (function () {
    function IMainImageSearchData() {
    }
    return IMainImageSearchData;
}());
var mainImageCookieHelper = /** @class */ (function (_super) {
    __extends(mainImageCookieHelper, _super);
    function mainImageCookieHelper() {
        return _super.call(this, "searchData") || this;
    }
    mainImageCookieHelper.prototype.initCookieData = function (imagePerPage) {
        this.cookieData = new IMainImageSearchData();
        this.cookieData.pageSize = imagePerPage;
        this.cookieData.pageIndex = 0;
        this.cookieData.currentSelectImage = 0;
        this.cookieData.selectCategory = new Array();
        this.cookieData.searchText = "";
        this.saveCookieData();
    };
    mainImageCookieHelper.prototype.clearCookieData = function () {
        this.cookieData.selectCategory = new Array();
        this.cookieData.searchText = "";
        this.cookieData.currentSelectImage = 0;
        this.cookieData.pageIndex = 0;
        _super.prototype.saveCookieData.call(this);
    };
    //region Implement IPagingData
    mainImageCookieHelper.prototype.readPagingData = function () {
        return this.cookieData;
    };
    mainImageCookieHelper.prototype.onChangePageIndex = function (pageIndex) {
        this.cookieData.pageIndex = pageIndex;
        this.saveCookieData();
    };
    return mainImageCookieHelper;
}(cookieHelperBase));
//# sourceMappingURL=mainImageCookieData.js.map