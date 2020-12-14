var searchData;
var hasAnoutherPages = true;
var hasStartRequest = false;
var maxImageIndex;
var currentSelectImage;
var userVode;
$(document).ready(function () {
    searchData = new mainCookieHelper(6);
    hookToCloseSideMenu();
    hookToClearAllFilter();
    hookToArrowKey();
    configPage();
    var autoCompleateHelper = new myAutocompleteHelper();
    autoCompleateHelper.configAutoCompleteTags(onStartAutoCompleteSearch, onStartAutoCompleteSearch);
    RegisterFloatPaging(searchData.readSearchData.bind(searchData), searchData.saveSearchData.bind(searchData), '/images/GetSearchingData');
    startSearchData();
});
function showModalImage(imageIndex) {
    currentSelectImage = imageIndex;
    $('#myModal').modal('show');
    GetPreviewImageData(currentSelectImage);
}
function hookToArrowKey() {
    document.addEventListener('keydown', function (e) {
        switch (e.keyCode) {
            case 37:
                navigationToLeft();
                break;
            case 39:
                navigationToRigth();
                break;
        }
    });
}
function navigationToLeft() {
    if (currentSelectImage > 1) {
        currentSelectImage = currentSelectImage - 1;
        GetPreviewImageData(currentSelectImage);
    }
}
function navigationToRigth() {
    if (!maxImageIndex
        || maxImageIndex - 1 >= currentSelectImage) {
        currentSelectImage = currentSelectImage + 1;
        GetPreviewImageData(currentSelectImage);
    }
}
function GetPreviewImageData(index) {
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
                currentSelectImage = currentSelectImage - 1;
                maxImageIndex = currentSelectImage;
            }
        },
        error: function () {
            alert("Error while retrieving data!");
        }
    });
}
// execute when selected element on tag search control
function onStartAutoCompleteSearch() {
    searchData.readSearchData();
    searchData.mainSearchData.searchText = $('#searchImageTag').val().toString();
    searchData.saveSearchData();
}
function checkUncheckCategory(checkBox) {
    searchData.readSearchData();
    if (checkBox.checked == true) {
        searchData.mainSearchData.selectCategory.push(checkBox.id);
    }
    else {
        var filtered = searchData.mainSearchData.selectCategory.filter(function (value, index, arr) {
            return value != checkBox.id;
        });
        searchData.mainSearchData.selectCategory = filtered;
    }
    searchData.saveSearchData();
    startSearchData();
}
function configPage() {
    var showSearchBar = $.cookie("show-search-bar");
    if (showSearchBar != null) {
        toggleMenu();
    }
    searchData.readSearchData();
    showCheckedCategory(searchData.mainSearchData.selectCategory);
    $('#searchImageTag').val(searchData.mainSearchData.searchText);
}
function showCheckedCategory(arraySelectCategories) {
    arraySelectCategories.forEach(function (element) {
        return document.getElementById(element).checked = true;
    });
}
function onclickShowSearcBar() {
    $.cookie("show-search-bar", "true", { path: "/" });
    toggleMenu();
}
function hookToCloseSideMenu() {
    var node = document.getElementById("sideCloseBtn");
    node.addEventListener("click", function (event) {
        $.cookie("show-search-bar", "false", { path: "/" });
        toggleMenu();
    });
}
function hookToClearAllFilter() {
    var node = document.getElementById("clearFilter");
    node.addEventListener("click", function (event) {
        searchData.clearSearchData();
        $('#groupCheckBox input:checked').each(function () {
            this.checked = false;
        });
        $('#searchImageTag').val(null);
        startSearchData();
    });
}
function toggleMenu() {
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
}
//# sourceMappingURL=imageIndexView.js.map