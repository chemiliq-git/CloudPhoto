﻿enum UserPageTabEnum {
    uploads,
    likes,
    followers,
    following,
}

class UserIndexViewHelper {

    constructor(
        imagePagingData: imageRelationByUserCookieHelper,
        userPagingData: userRelationByUserCookieHelper) {
        this.imagePagingData = imagePagingData;
        this.imagePagingData.saveCookieData();

        this.userPagingData = userPagingData;
        this.userPagingData.saveCookieData();

        var context = this;
        $(document).ready(
            function () {
                context.myImagePagingHelper.RegisterFloatPaging(
                    '/images/GetUserImagesData',
                    context.imagePagingData);
                context.myImagePagingHelper.startSearchData();

                context.myUserPagingHelper.pausePaging();
                context.myUserPagingHelper.RegisterFloatPaging(
                    '/users/GetLinkedUsers',
                    context.userPagingData);

                context.hookToArrowKey();
            }
        );
    }

    myImagePagingHelper = new myFloatPagingHelper<imageRelationByUserCookieHelper>();
    imagePagingData: imageRelationByUserCookieHelper;

    myUserPagingHelper = new myFloatPagingHelper<userRelationByUserCookieHelper>();
    userPagingData: userRelationByUserCookieHelper;

    myAllertHelper = new myAlertHelper("myMessageContainer");
    maxImageIndex;

    getUploadImages(control) {
        this.imagePagingData.cookieData.type = UserPageTabEnum[UserPageTabEnum.uploads].toString();
        this.imagePagingData.saveCookieData();
        this.maxImageIndex = undefined;

        this.myImagePagingHelper.startSearchData();
        this.myUserPagingHelper.pausePaging();
    }

    getLikeImages(control) {
        this.imagePagingData.cookieData.type = UserPageTabEnum[UserPageTabEnum.likes].toString();;
        this.imagePagingData.saveCookieData();
        this.maxImageIndex = undefined;

        this.myImagePagingHelper.startSearchData();
        this.myUserPagingHelper.pausePaging();
    }

    getFollowers(control: HTMLElement) {
        this.userPagingData.cookieData.type = UserPageTabEnum[UserPageTabEnum.followers].toString();
        this.userPagingData.saveCookieData();

        this.myImagePagingHelper.pausePaging();
        this.myUserPagingHelper.startSearchData();
    }

    getFollowing(control: HTMLElement) {
        this.userPagingData.cookieData.type = UserPageTabEnum[UserPageTabEnum.following].toString();
        this.userPagingData.saveCookieData();

        this.myImagePagingHelper.pausePaging();
        this.myUserPagingHelper.startSearchData();
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

        formData.append("userId", this.imagePagingData.cookieData.userId);
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

        formData.append("userId", this.imagePagingData.cookieData.userId);
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
        this.imagePagingData.cookieData.currentSelectImage = imageIndex;
        (<any>$('#myModal')).modal('show');
        this.GetPreviewImageData(this.imagePagingData.cookieData.currentSelectImage);
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
                    context.imagePagingData.cookieData.currentSelectImage = context.imagePagingData.cookieData.currentSelectImage - 1;
                    context.maxImageIndex = context.imagePagingData.cookieData.currentSelectImage;
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
        if (this.imagePagingData.cookieData.currentSelectImage > 1) {
            this.imagePagingData.cookieData.currentSelectImage = this.imagePagingData.cookieData.currentSelectImage - 1;
            this.GetPreviewImageData(this.imagePagingData.cookieData.currentSelectImage);
        }
    }

    navigationToRigth() {
        if (!this.maxImageIndex
            || this.maxImageIndex - 1 >= this.imagePagingData.cookieData.currentSelectImage) {
            this.imagePagingData.cookieData.currentSelectImage = this.imagePagingData.cookieData.currentSelectImage + 1;
            this.GetPreviewImageData(this.imagePagingData.cookieData.currentSelectImage);
        }
    }
}