﻿// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function mySendImageVote(imageId) {
    var token = $("#keyForm input[name=__RequestVerificationToken]").val();

    var formData = new FormData();
    formData.append("imageId", imageId);

    let style = document.getElementById(imageId).getAttribute('style');
    if (style.includes("red")) {
        formData.append("isLike", false);
    }
    else {
        formData.append("isLike", true);
    }

    $.ajax(
        {
            url: "/api/votes",
            data: formData,
            processData: false,
            contentType: false,
            type: "POST",
            headers: { 'X-CSRF-TOKEN': token },
            complete: function () {
                return false;
            },
            success: function (data) {
                if (data.result == true) {
                    if (style.includes("red")) {
                        document.getElementById(imageId).setAttribute("style", "color:");
                    }
                    else {
                        document.getElementById(imageId).setAttribute("style", "color:red");
                    }
                }
            },
            error: function (e, xhr) {
                if (e.status == 401) {
                    window.location = '/Identity/Account/Login';
                }
            }
        }
    );

    return false;
}

function mySendImageVoteOnPreviewForm(button, likeIconId, imageId, likeCounts, userIsLike ) {
    var token = $("#keyForm input[name=__RequestVerificationToken]").val();

    var formData = new FormData();
    formData.append("imageId", imageId);

    let style = document.getElementById(likeIconId).getAttribute('style');
    if (style.includes("red")) {
        formData.append("isLike", false);
    }
    else {
        formData.append("isLike", true);
    }

    $.ajax(
        {
            url: "/api/votes",
            data: formData,
            processData: false,
            contentType: false,
            type: "POST",
            headers: { 'X-CSRF-TOKEN': token },
            complete: function () {
                return false;
            },
            success: function (data) {
                if (data.result == true) {
                    textNode = button.lastChild;
                    if (style.includes("red")) {
                        document.getElementById(likeIconId).setAttribute("style", "color:");
                        if (userIsLike == 0) {
                            textNode.nodeValue = " " + (likeCounts - 1) + " likes";
                        }
                        else {
                            textNode.nodeValue = " " + (likeCounts) + " likes";
                        }
                    }
                    else {
                        document.getElementById(likeIconId).setAttribute("style", "color:red");
                        if (userIsLike == 0) {
                            textNode.nodeValue = " " + (likeCounts) + " likes";
                        }
                        else {
                            textNode.nodeValue = " " + (likeCounts + 1) + " likes";
                        }
                    }
                }
            },
            error: function (e, xhr) {
                if (e.status == 401) {
                    window.location = '/Identity/Account/Login';
                }
            }
        }
    );

    return false;
}