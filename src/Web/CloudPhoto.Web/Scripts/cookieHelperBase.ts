class cookieHelperBase<T> {

    cookieName: string;
    cookieData: T;

    constructor(cookieName: string) {
        this.cookieName = cookieName;
    }

    readCookieData(): T {
        var txtSearchData = $.cookie(this.cookieName);
        if (txtSearchData) {
            this.cookieData = JSON.parse(txtSearchData);
            return this.cookieData;
        }
        else {
            return undefined;
        }
    }

    saveCookieData() {
        var txtSearchData = JSON.stringify(this.cookieData);
        $.cookie(this.cookieName, txtSearchData, { path: "/" });
    }
}