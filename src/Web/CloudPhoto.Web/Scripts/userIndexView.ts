
class UserIndexViewHelper {
    constructor(pPagingData: userCookieHelper) {
        this.pagingData = pPagingData;

        var context = this;
        $(document).ready(
            function () {
                RegisterFloatPaging(context.pagingData.readSearchData.bind(context.pagingData),
                    context.pagingData.saveSearchData.bind(context.pagingData),
                    '/Users/GetPagingData');
                startSearchData();

                context.hookToArrowKey();
            }
        );
    }

    myAllertHelper = new myAlertHelper("myMessageContainer");
    pagingData: userCookieHelper;

    maxImageIndex;

    getUploadImages(control) {
        this.pagingData.userSearchData.type = "uploads";
        control.setAttribute("class", "nav-link");
        document.getElementById("likeTab").setAttribute("class", "nav-link active");
        this.pagingData.saveSearchData();
        startSearchData();
    }

    getLikeImages(control) {
        this.pagingData.userSearchData.type = "likes";
        control.setAttribute("class", "nav-link");
        document.getElementById("uploadTab").setAttribute("class", "nav-link active");
        this.pagingData.saveSearchData();
        startSearchData();
    }

    uploadAvatar(inputId) {
        var input = document.getElementById(inputId);
        var files = (<any>input).files;
        var formData = new FormData();

        for (var i = 0; i != files.length; i++) {
            formData.append("file", files[i]);
        }
        uploadFile(formData);
    }

    uploadFile(formData) {

        formData.append("userId", this.pagingData.userSearchData.userId);
        var token = $("#dragFileForm input[name=__RequestVerificationToken]").val();
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
                        this.updateAvatar(data.imageUrl);
                    }
                    else {
                        // show message (myAlets.js)
                        myAllertHelper.myShowErrorMessage(data.errorMessage);
                    }
                }.bind(this)
            }
        );
    }

    updateAvatar(avatarUrl) {
        var formData = new FormData();

        formData.append("userId", this.pagingData.userSearchData.userId);
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
                }
            }
        );
    }

    showModalImage(imageIndex) {
        this.pagingData.userSearchData.currentSelectImage = imageIndex;
        (<any>$('#myModal')).modal('show');
        this.GetPreviewImageData(this.pagingData.userSearchData.currentSelectImage);
    }

    GetPreviewImageData(index) {
        var token = $("#keyForm input[name=__RequestVerificationToken]").val();
        var formData = new FormData();
        formData.append("id", index);
        var context = this;
        $.ajax({
            url: '/users/PreviewImage',
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
                    context.pagingData.userSearchData.currentSelectImage = context.pagingData.userSearchData.currentSelectImage - 1;
                    context.maxImageIndex = context.pagingData.userSearchData.currentSelectImage;
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
        if (this.pagingData.userSearchData.currentSelectImage > 1) {
            this.pagingData.userSearchData.currentSelectImage = this.pagingData.userSearchData.currentSelectImage - 1;
            this.GetPreviewImageData(this.pagingData.userSearchData.currentSelectImage);
        }
    }

    navigationToRigth() {
        if (!this.maxImageIndex
            || this.maxImageIndex - 1 >= this.pagingData.userSearchData.currentSelectImage) {
            this.pagingData.userSearchData.currentSelectImage = this.pagingData.userSearchData.currentSelectImage + 1;
            this.GetPreviewImageData(this.pagingData.userSearchData.currentSelectImage);
        }
    }
}