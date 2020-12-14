var mPagingContainer = '#partialView'
var mPagingProgress = '#progress'
var mPaginData;

var mPagingUrl;
var mReadPagingData;
var mSavePagingData;

var mHasAnoutherPages = true;
var mHasStartRequest = false;

function RegisterFloatPaging(
    pReadPagingData, pSavePaginData, pSearchUrl) {
    mReadPagingData = pReadPagingData;
    mSavePagingData = pSavePaginData;
    mPagingUrl = pSearchUrl;
}

$(window).scroll(function () {
    if ($(window).scrollTop() ==
        $(document).height() - $(window).height()) {
        if (!mHasStartRequest) {
            GetData();
        }
    }
});

function startSearchData() {
    var pagingView = $(mPagingContainer);
    pagingView.html("");

    mPaginData = mReadPagingData();
    mPaginData.pageIndex = 1;
    mSavePagingData(mPaginData);

    mHasAnoutherPages = true;
    GetData();
}

function GetData() {
    mHasStartRequest = true;

    if (!mHasAnoutherPages) {
        return;
    }

    var token = $("#keyForm input[name=__RequestVerificationToken]").val();

    mPaginData = mReadPagingData();

    var formData = new FormData();
    Object.keys(mPaginData).forEach(function (key, index) {
        // key: the name of the object key
        // index: the ordinal position of the key within the object
        if (Array.isArray(mPaginData[key])) {
            formData.append(key, JSON.stringify(mPaginData[key]));
        }
        else {
            formData.append(key, mPaginData[key]);
        }
    });

    $.ajax({
        url: mPagingUrl,
        data: formData,
        processData: false,
        contentType: false,
        type: "POST",
        headers: { 'X-CSRF-TOKEN': token.toString() },
        xhrFields: {
            withCredentials: true
        },
        success: function (data) {
            var pageContainer = $(mPagingContainer);
            if (data != '') {
                pageContainer.append(data);
                if (mPaginData.pageIndex == 1) {
                    $('html, body').animate({ scrollTop: 0 }, 'slow');
                }
                mPaginData.pageIndex++;
                mSavePagingData(mPaginData);
            }
            else {
                mHasAnoutherPages = false;
                if (mPaginData.pageIndex == 1) {
                    pageContainer.html("");
                }
            }
        },
        beforeSend: function () {
            $(mPagingProgress).show();
        },
        complete: function () {
            $(mPagingProgress).hide();
            mHasStartRequest = false;
        },
        error: function () {
            alert("Error while retrieving data!");
        }
    });
}