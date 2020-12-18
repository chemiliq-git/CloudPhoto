var cookieHelperBase = /** @class */ (function () {
    function cookieHelperBase(cookieName) {
        this.cookieName = cookieName;
    }
    cookieHelperBase.prototype.readCookieData = function () {
        var txtSearchData = $.cookie(this.cookieName);
        if (txtSearchData) {
            this.cookieData = JSON.parse(txtSearchData);
            return this.cookieData;
        }
        else {
            return undefined;
        }
    };
    cookieHelperBase.prototype.saveCookieData = function () {
        var txtSearchData = JSON.stringify(this.cookieData);
        $.cookie(this.cookieName, txtSearchData, { path: "/" });
    };
    return cookieHelperBase;
}());
//# sourceMappingURL=cookieHelperBase.js.map