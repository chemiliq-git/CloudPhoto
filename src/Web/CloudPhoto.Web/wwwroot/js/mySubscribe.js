function myInvokeSubscribe(button, userId) {
    var token = $("#keyForm input[name=__RequestVerificationToken]").val();
    var formData = new FormData();
    formData.append("userId", userId);
    if (button.innerText.indexOf("Following") != -1) {
        formData.append("follow", "false");
    }
    else {
        formData.append("follow", "true");
    }
    $.ajax({
        url: "/api/subscribes",
        data: formData,
        processData: false,
        contentType: false,
        type: "POST",
        headers: { 'X-CSRF-TOKEN': token.toString() },
        complete: function () {
            return false;
        },
        success: function (data) {
            if (data.result == true) {
                if (button.innerText.indexOf("Following") != -1) {
                    button.innerText = "Follow";
                }
                else {
                    button.innerText = "Following";
                }
            }
        },
        error: function (e, xhr) {
            if (e.status == 401) {
                window.location.href = '/Identity/Account/Login';
            }
        }
    });
    return false;
}
//# sourceMappingURL=mySubscribe.js.map