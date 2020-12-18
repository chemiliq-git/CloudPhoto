var ImageIndexViewHelper = /** @class */ (function () {
    function ImageIndexViewHelper(pSearchData) {
        this.myPagingHelper = new myFloatPagingHelper();
        this.searchData = pSearchData;
        var context = this;
        $(document).ready(function () {
            context.searchData = new mainImageCookieHelper();
            context.hookToCloseSideMenu();
            context.hookToClearAllFilter();
            context.hookToArrowKey();
            context.configPage();
            new myAutocompleteHelper(context.onStartAutoCompleteSearch.bind(context), context.onStartAutoCompleteSearch.bind(context));
            context.myPagingHelper.RegisterFloatPaging('/images/GetSearchingData', context.searchData);
            context.maxImageIndex = undefined;
            context.myPagingHelper.startSearchData();
        });
    }
    ImageIndexViewHelper.prototype.showModalImage = function (imageIndex) {
        this.currentSelectImage = imageIndex;
        $('#myModal').modal('show');
        this.GetPreviewImageData(this.currentSelectImage);
    };
    ImageIndexViewHelper.prototype.hookToArrowKey = function () {
        document.addEventListener('keydown', function (e) {
            switch (e.keyCode) {
                case 37:
                    this.navigationToLeft();
                    break;
                case 39:
                    this.navigationToRigth();
                    break;
            }
        }.bind(this));
    };
    ImageIndexViewHelper.prototype.navigationToLeft = function () {
        if (this.currentSelectImage > 1) {
            this.currentSelectImage = this.currentSelectImage - 1;
            this.GetPreviewImageData(this.currentSelectImage);
        }
    };
    ImageIndexViewHelper.prototype.navigationToRigth = function () {
        if (!this.maxImageIndex
            || this.maxImageIndex - 1 >= this.currentSelectImage) {
            this.currentSelectImage = this.currentSelectImage + 1;
            this.GetPreviewImageData(this.currentSelectImage);
        }
    };
    ImageIndexViewHelper.prototype.GetPreviewImageData = function (imageIndex) {
        var token = $("#keyForm input[name=__RequestVerificationToken]").val();
        var formData = new FormData();
        formData.append("id", imageIndex.toString());
        $.ajax({
            url: '/images/PreviewImage',
            data: formData,
            processData: false,
            contentType: false,
            type: "POST",
            headers: { 'X-CSRF-TOKEN': token.toString() },
            xhrFields: {
                withCredentials: true
            },
            success: function (data) {
                var pageContainer = $('#modalPartialView');
                if (data != ''
                    && pageContainer) {
                    pageContainer.html("");
                    pageContainer.append(data);
                }
                else {
                    this.currentSelectImage = this.currentSelectImage - 1;
                    this.maxImageIndex = this.currentSelectImage;
                }
            }.bind(this),
            error: function () {
                alert("Error while retrieving data!");
            }
        });
    };
    // execute when selected element on tag search control
    ImageIndexViewHelper.prototype.onStartAutoCompleteSearch = function () {
        this.searchData.readCookieData();
        this.searchData.cookieData.searchText = $('#searchImageTag').val().toString();
        this.searchData.saveCookieData();
    };
    ImageIndexViewHelper.prototype.checkUncheckCategory = function (checkBox) {
        this.searchData.readCookieData();
        if (checkBox.checked == true) {
            this.searchData.cookieData.selectCategory.push(checkBox.id);
        }
        else {
            var filtered = this.searchData.cookieData.selectCategory.filter(function (value, index, arr) {
                return value != checkBox.id;
            });
            this.searchData.cookieData.selectCategory = filtered;
        }
        this.searchData.saveCookieData();
        this.maxImageIndex = undefined;
        this.myPagingHelper.startSearchData();
    };
    ImageIndexViewHelper.prototype.configPage = function () {
        var showSearchBar = $.cookie("show-search-bar");
        if (showSearchBar != null) {
            this.toggleMenu();
        }
        this.searchData.readCookieData();
        this.showCheckedCategory(this.searchData.cookieData.selectCategory);
        $('#searchImageTag').val(this.searchData.cookieData.searchText);
    };
    ImageIndexViewHelper.prototype.showCheckedCategory = function (arraySelectCategories) {
        arraySelectCategories.forEach(function (element) {
            return document.getElementById(element).checked = true;
        });
    };
    ImageIndexViewHelper.prototype.onclickShowSearcBar = function () {
        $.cookie("show-search-bar", "true", { path: "/" });
        this.toggleMenu();
    };
    ImageIndexViewHelper.prototype.hookToCloseSideMenu = function () {
        var node = document.getElementById("sideCloseBtn");
        node.addEventListener("click", function (event) {
            $.cookie("show-search-bar", "false", { path: "/" });
            this.toggleMenu();
        }.bind(this));
    };
    ImageIndexViewHelper.prototype.hookToClearAllFilter = function () {
        var node = document.getElementById("clearFilter");
        var context = this;
        node.addEventListener("click", function (event) {
            context.searchData.clearCookieData();
            $('#groupCheckBox input:checked').each(function () {
                this.checked = false;
            });
            $('#searchImageTag').val(null);
            context.maxImageIndex = undefined;
            context.myPagingHelper.startSearchData();
        }.bind(this));
    };
    ImageIndexViewHelper.prototype.toggleMenu = function () {
        var show = $.cookie("show-search-bar");
        var searchBox = document.getElementById("side-search");
        if (show === "true") {
            searchBox.style.display = "block";
        }
        else {
            searchBox.style.display = "none";
        }
        var hideSearchBox = document.getElementById("hide-side-search");
        if (show === "false") {
            hideSearchBox.style.display = "block";
        }
        else {
            hideSearchBox.style.display = "none";
        }
        var containerImage = document.getElementById("containerImages");
        if (show === "false") {
            containerImage.classList.remove("col-md-10");
            containerImage.classList.add("col-md-12");
        }
        else {
            containerImage.classList.remove("col-md-12");
            containerImage.classList.add("col-md-10");
        }
    };
    return ImageIndexViewHelper;
}());
//# sourceMappingURL=imageIndexView.js.map