enum UserPageTabEnum {
    uploads,
    likes,
    followers,
    following,
}

class UserIndexViewHelper {

    constructor(pPagingData: imageRelationByUserCookieHelper) {
        this.pagingData = pPagingData;
        this.pagingData.saveCookieData();

        var context = this;
        $(document).ready(
            function () {
                context.myPagingHelper.RegisterFloatPaging(
                    '/images/GetUserImagesData',
                    context.pagingData);
                context.myPagingHelper.startSearchData();

                context.hookToArrowKey();
            }
        );
    }

    myPagingHelper = new myFloatPagingHelper<imageRelationByUserCookieHelper>();
    myAllertHelper = new myAlertHelper("myMessageContainer");
    pagingData: imageRelationByUserCookieHelper;

    maxImageIndex;

    getUploadImages(control) {
        this.pagingData.cookieData.type = "uploads";
        control.setAttribute("class", "nav-link");
        document.getElementById("likeTab").setAttribute("class", "nav-link active");
        this.pagingData.saveCookieData();
        this.maxImageIndex = undefined;
        this.myPagingHelper.startSearchData();
    }

    getLikeImages(control) {
        this.pagingData.cookieData.type = "likes";
        control.setAttribute("class", "nav-link");
        document.getElementById("uploadTab").setAttribute("class", "nav-link active");
        this.pagingData.saveCookieData();
        this.maxImageIndex = undefined;
        this.myPagingHelper.startSearchData();
    }

    getFollowers(control: HTMLElement) {
        this.pagingData.cookieData.type = UserPageTabEnum[UserPageTabEnum.followers].toString();
        control.setAttribute("class", "nav-link");
        control.parentElement.setAttribute("class", "nav-link active");
        this.pagingData.saveCookieData();
        this.maxImageIndex = undefined;
        this.myPagingHelper.startSearchData();
    }

    getFollowing(control: HTMLElement) {
        this.pagingData.cookieData.type = UserPageTabEnum[UserPageTabEnum.following].toString();
        control.setAttribute("class", "nav-link");
        control.parentElement.setAttribute("class", "nav-link active");
        this.pagingData.saveCookieData();
        this.maxImageIndex = undefined;
        this.myPagingHelper.startSearchData();
    }

    uploadAvatar(inputId) {
        var input = document.getElementById(inputId);
        var files = (<any>input).files;
        var formData = new FormData();

        for (var i = 0; i != files.length; i++) {
            formData.append("file", files[i]);
        }
        this.uploadFile(formData);
    }

    uploadFile(formData) {

        formData.append("userId", this.pagingData.cookieData.userId);
        var token = $("#dragFileForm input[name=__RequestVerificationToken]").val();
        var context = this;
        $.ajax(
            {
                url: "/api/uploadfiles/UploadAvatart",
                data: formData,
                processData: false,
                contentType: false,
                type: "POST",
                headers: { 'X-CSRF-TOKEN': token.toString() },
                beforeSend: function () {
                    $('#loader').show();
                },
                complete: function () {
                    $('#loader').hide();
                },
                success: function (data) {
                    if (data.result == true) {
                        //set image source on image control
                        $("#avatarImage").attr("src", data.imageUrl);
                        context.updateAvatar(data.imageUrl);
                    }
                    else {
                        // show message (myAlets.js)
                        context.myAllertHelper.myShowErrorMessage(data.errorMessage);
                    }
                }.bind(this)
            }
        );
    }

    updateAvatar(avatarUrl) {
        var formData = new FormData();

        formData.append("userId", this.pagingData.cookieData.userId);
        formData.append("avatarUrl", avatarUrl);

        var token = $("#dragFileForm input[name=__RequestVerificationToken]").val();
        $.ajax(
            {
                url: "/Users/UpdateAvatar",
                data: formData,
                processData: false,
                contentType: false,
                type: "POST",
                headers: { 'X-CSRF-TOKEN': token.toString() },
                beforeSend: function () {
                    $('#loader').show();
                },
                complete: function () {
                    $('#loader').hide();
                },
                success: function (data) {
                    if (data == true) {
                        // show message (myAlets.js)
                        this.myAllertHelper.myShowSuccessMessage("Avatar updated. Change must applied after logout.");
                    }
                }.bind(this)
            }
        );
    }

    showModalImage(imageIndex) {
        this.pagingData.cookieData.currentSelectImage = imageIndex;
        (<any>$('#myModal')).modal('show');
        this.GetPreviewImageData(this.pagingData.cookieData.currentSelectImage);
    }

    GetPreviewImageData(index) {
        var token = $("#keyForm input[name=__RequestVerificationToken]").val();
        var formData = new FormData();
        formData.append("id", index);
        var context = this;
        $.ajax({
            url: '/images/PreviewUserImage',
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
                    context.pagingData.cookieData.currentSelectImage = context.pagingData.cookieData.currentSelectImage - 1;
                    context.maxImageIndex = context.pagingData.cookieData.currentSelectImage;
                }
            },
            error: function () {
                alert("Error while retrieving data!");
            }
        });
    }

    hookToArrowKey() {
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
    }

    navigationToLeft() {
        if (this.pagingData.cookieData.currentSelectImage > 1) {
            this.pagingData.cookieData.currentSelectImage = this.pagingData.cookieData.currentSelectImage - 1;
            this.GetPreviewImageData(this.pagingData.cookieData.currentSelectImage);
        }
    }

    navigationToRigth() {
        if (!this.maxImageIndex
            || this.maxImageIndex - 1 >= this.pagingData.cookieData.currentSelectImage) {
            this.pagingData.cookieData.currentSelectImage = this.pagingData.cookieData.currentSelectImage + 1;
            this.GetPreviewImageData(this.pagingData.cookieData.currentSelectImage);
        }
    }
}