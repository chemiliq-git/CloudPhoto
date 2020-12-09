// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


var messageHtmElement =
    `<div id="messageBody" class="alert alert-dismissible fade show" role="alert" style="display:none">
       <p id="textMessage" class="text-center">Your message must show here</p>
          <button type="button" class="close" data-dismiss="alert" aria-label="Close">
             <span aria-hidden="true">&times;</span>
          </button>
     </div>`;

$(document).ready(
    function () {
        addMessageElement();
    }
);

function addMessageElement() {
    var messageContainer = document.getElementById("myMessageContainer");
    if (messageContainer) {
        messageContainer.innerHTML = messageHtmElement;
        $('#messageBody').on('closed.bs.alert', addMessageElement);
    }
}

function myShowErrorMessage(errorText) {
    var messageBody = document.getElementById("messageBody");
    if (messageBody) {
        messageBody.classList.add("alert-danger");
        var txtMessage = document.getElementById("textMessage");
        txtMessage.innerHTML = errorText;
        messageBody.style.display = "block";
    }
}

function myShowInfoMessage(infoText) {
    var messageBody = document.getElementById("messageBody");
    if (messageBody) {
        messageBody.classList.add("alert-info");
        var txtMessage = document.getElementById("textMessage");
        txtMessage.innerHTML = infoText;
        messageBody.style.display = "block";
    }
}

function myShowSuccessMessage(successText) {
    var messageBody = document.getElementById("messageBody");
    if (messageBody) {
        messageBody.classList.add("alert-success");
        var txtMessage = document.getElementById("textMessage");
        txtMessage.innerHTML = successText;
        messageBody.style.display = "block";
    }
}