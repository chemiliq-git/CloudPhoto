var myAlertHelper = /** @class */ (function () {
    function myAlertHelper(controlName) {
        this.messageHtmElement = "<div id=\"messageBody\" class=\"alert alert-dismissible fade show\" role=\"alert\" style=\"display:none\">\n       <p id=\"textMessage\" class=\"text-center\">Your message must show here</p>\n          <button type=\"button\" class=\"close\" data-dismiss=\"alert\" aria-label=\"Close\">\n             <span aria-hidden=\"true\">&times;</span>\n          </button>\n     </div>";
        this.controlName = controlName;
        var selfI = this;
        $(document).ready(function () {
            selfI.addMessageElement();
        });
    }
    myAlertHelper.prototype.addMessageElement = function () {
        var messageContainer = document.getElementById(this.controlName);
        if (messageContainer) {
            messageContainer.innerHTML = this.messageHtmElement;
            var selfI = this;
            $('#messageBody').on('closed.bs.alert', function () {
                selfI.addMessageElement();
            });
        }
    };
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
//# sourceMappingURL=myAlerts.js.map