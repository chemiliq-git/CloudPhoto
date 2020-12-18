interface IPagingData {
    readPagingData: () => any;
    onChangePageIndex: (pageIndex: number) => void;
}

class myFloatPagingHelper<T extends IPagingData> {

    constructor(
        pagingContainerName: string = '#partialView',
        pagingProcessControlName: string = '#progress') {
        this.pagingContainer = pagingContainerName;
        this.pagingProgress = pagingProcessControlName;
    }

    pagingContainer: string;
    pagingProgress: string;
    pagingData: T;
    pageIndex: number;

    pagingUrl: string;

    hasAnoutherPages: boolean = true;
    hasStartRequest: boolean = false;
    isPausePaging: boolean = false;

    RegisterFloatPaging(pSearchUrl: string, pagingData: T) {
        this.pagingUrl = pSearchUrl;
        this.pagingData = pagingData;

        let context = this;
        $(window).scroll(function () {
            if ($(window).scrollTop() >=
                $(document).height() - $(window).height()) {
                if (!context.isPausePaging
                    && !context.hasStartRequest) {
                    context.GetData();
                }
            }
        });
    }

    pausePaging() {
        this.isPausePaging = true;
    }

    playPaging() {
        this.isPausePaging = false;
    }


    startSearchData() {
        var pagingView = $(this.pagingContainer);
        pagingView.html("");

        this.pageIndex = 1;
        this.pagingData.onChangePageIndex(this.pageIndex);

        this.hasAnoutherPages = true;
        this.GetData();
    }

    private GetData() {
        this.hasStartRequest = true;

        if (!this.hasAnoutherPages) {
            return;
        }

        var token = $("#keyForm input[name=__RequestVerificationToken]").val();

        var searchData = this.pagingData.readPagingData();

        var formData = new FormData();
        let context = this;
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
    }
}