var myAlertHelper = /** @class */ (function () {
    function myAlertHelper() {
        this.messageHtmElement = "<div id=\"messageBody\" class=\"alert alert-dismissible fade show\" role=\"alert\" style=\"display:none\">\n       <p id=\"textMessage\" class=\"text-center\">Your message must show here</p>\n          <button type=\"button\" class=\"close\" data-dismiss=\"alert\" aria-label=\"Close\">\n             <span aria-hidden=\"true\">&times;</span>\n          </button>\n     </div>";
    }
    myAlertHelper.prototype.myShowErrorMessage = function (errorText) {
        var messageBody = document.getElementById("messageBody");
        if (messageBody) {
            messageBody.classList.add("alert-danger");
            var txtMessage = document.getElementById("textMessage");
            txtMessage.innerHTML = errorText;
            messageBody.style.display = "block";
        }
    };
    myAlertHelper.prototype.myShowInfoMessage = function (infoText) {
        var messageBody = document.getElementById("messageBody");
        if (messageBody) {
            messageBody.classList.add("alert-info");
            var txtMessage = document.getElementById("textMessage");
            txtMessage.innerHTML = infoText;
            messageBody.style.display = "block";
        }
    };
    myAlertHelper.prototype.myShowSuccessMessage = function (successText) {
        var messageBody = document.getElementById("messageBody");
        if (messageBody) {
            messageBody.classList.add("alert-success");
            var txtMessage = document.getElementById("textMessage");
            txtMessage.innerHTML = successText;
            messageBody.style.display = "block";
        }
    };
    return myAlertHelper;
}());
var myAllertHelper = new myAlertHelper();
$(document).ready(function () {
    addMessageElement();
});
function addMessageElement() {
    var messageContainer = document.getElementById("myMessageContainer");
    if (messageContainer) {
        messageContainer.innerHTML = myAllertHelper.messageHtmElement;
        $('#messageBody').on('closed.bs.alert', addMessageElement);
    }
}
//# sourceMappingURL=myAlerts.js.map