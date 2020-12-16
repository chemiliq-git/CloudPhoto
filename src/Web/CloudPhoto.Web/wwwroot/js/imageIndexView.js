var ImageIndexViewHelper = /** @class */ (function () {
    function ImageIndexViewHelper(pSearchData) {
        this.hasAnoutherPages = true;
        this.hasStartRequest = false;
        this.searchData = pSearchData;
        var context = this;
        $(document).ready(function () {
            context.searchData = new mainCookieHelper();
            context.hookToCloseSideMenu();
            context.hookToClearAllFilter();
            context.hookToArrowKey();
            context.configPage();
            var autoCompleateHelper = new myAutocompleteHelper();
            autoCompleateHelper.configAutoCompleteTags(context.onStartAutoCompleteSearch.bind(context), context.onStartAutoCompleteSearch.bind(context));
            RegisterFloatPaging(context.searchData.readSearchData.bind(context.searchData), context.searchData.saveSearchData.bind(context.searchData), '/images/GetSearchingData');
            startSearchData();
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
    ImageIndexViewHelper.prototype.GetPreviewImageData = function (index) {
        var token = $("#keyForm input[name=__RequestVerificationToken]").val();
        var formData = new FormData();
        formData.append("id", index);
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
        this.searchData.readSearchData();
        this.searchData.mainSearchData.searchText = $('#searchImageTag').val().toString();
        this.searchData.saveSearchData();
    };
    ImageIndexViewHelper.prototype.checkUncheckCategory = function (checkBox) {
        this.searchData.readSearchData();
        if (checkBox.checked == true) {
            this.searchData.mainSearchData.selectCategory.push(checkBox.id);
        }
        else {
            var filtered = this.searchData.mainSearchData.selectCategory.filter(function (value, index, arr) {
                return value != checkBox.id;
            });
            this.searchData.mainSearchData.selectCategory = filtered;
        }
        this.searchData.saveSearchData();
        startSearchData();
    };
    ImageIndexViewHelper.prototype.configPage = function () {
        var showSearchBar = $.cookie("show-search-bar");
        if (showSearchBar != null) {
            this.toggleMenu();
        }
        this.searchData.readSearchData();
        this.showCheckedCategory(this.searchData.mainSearchData.selectCategory);
        $('#searchImageTag').val(this.searchData.mainSearchData.searchText);
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
        node.addEventListener("click", function (event) {
            this.searchData.clearSearchData();
            $('#groupCheckBox input:checked').each(function () {
                this.checked = false;
            });
            $('#searchImageTag').val(null);
            startSearchData();
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