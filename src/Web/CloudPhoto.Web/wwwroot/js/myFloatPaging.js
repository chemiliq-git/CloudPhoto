var myFloatPagingHelper = /** @class */ (function () {
    function myFloatPagingHelper(pagingContainerName, pagingProcessControlName) {
        if (pagingContainerName === void 0) { pagingContainerName = '#partialView'; }
        if (pagingProcessControlName === void 0) { pagingProcessControlName = '#progress'; }
        this.hasAnoutherPages = true;
        this.hasStartRequest = false;
        this.isPausePaging = false;
        this.pagingContainer = pagingContainerName;
        this.pagingProgress = pagingProcessControlName;
    }
    myFloatPagingHelper.prototype.RegisterFloatPaging = function (pSearchUrl, pagingData) {
        this.pagingUrl = pSearchUrl;
        this.pagingData = pagingData;
        var context = this;
        $(window).scroll(function () {
            if ($(window).scrollTop() >=
                $(document).height() - $(window).height()) {
                if (!context.isPausePaging
                    && !context.hasStartRequest) {
                    context.GetData();
                }
            }
        });
    };
    myFloatPagingHelper.prototype.pausePaging = function () {
        this.isPausePaging = true;
    };
    myFloatPagingHelper.prototype.playPaging = function () {
        this.isPausePaging = false;
    };
    myFloatPagingHelper.prototype.startSearchData = function () {
        var pagingView = $(this.pagingContainer);
        pagingView.html("");
        this.pageIndex = 1;
        this.pagingData.onChangePageIndex(this.pageIndex);
        this.hasAnoutherPages = true;
        this.GetData();
    };
    myFloatPagingHelper.prototype.GetData = function () {
        this.hasStartRequest = true;
        if (!this.hasAnoutherPages) {
            return;
        }
        var token = $("#keyForm input[name=__RequestVerificationToken]").val();
        var searchData = this.pagingData.readPagingData();
        var formData = new FormData();
        var context = this;
        Object.keys(searchData).forEach(function (key, index) {
            // key: the name of the object key
            // index: the ordinal position of the key within the object
            if (Array.isArray(searchData[key])) {
                formData.append(key, JSON.stringify(searchData[key]));
            }
            else {
                formData.append(key, searchData[key]);
            }
        });
        $.ajax({
            url: this.pagingUrl,
            data: formData,
            processData: false,
            contentType: false,
            type: "POST",
            headers: { 'X-CSRF-TOKEN': token.toString() },
            xhrFields: {
                withCredentials: true
            },
            success: function (data) {
                var pageContainer = $(context.pagingContainer);
                if (data != '') {
                    pageContainer.append(data);
                    if (context.pageIndex == 1) {
                        $('html, body').animate({ scrollTop: 0 }, 'slow');
                    }
                    context.pageIndex++;
                    context.pagingData.onChangePageIndex(context.pageIndex);
                }
                else {
                    context.hasAnoutherPages = false;
                    if (context.pageIndex == 1) {
                        pageContainer.html("");
                    }
                }
            },
            beforeSend: function () {
                $(context.pagingProgress).show();
            },
            complete: function () {
                $(context.pagingProgress).hide();
                context.hasStartRequest = false;
            },
            error: function () {
                alert("Error while retrieving data!");
            }
        });
    };
    return myFloatPagingHelper;
}());
//# sourceMappingURL=myFloatPaging.js.map