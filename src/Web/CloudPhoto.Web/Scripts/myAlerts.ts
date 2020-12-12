class myAlertHelper {
    
    messageHtmElement: string =
    `<div id="messageBody" class="alert alert-dismissible fade show" role="alert" style="display:none">
       <p id="textMessage" class="text-center">Your message must show here</p>
          <button type="button" class="close" data-dismiss="alert" aria-label="Close">
             <span aria-hidden="true">&times;</span>
          </button>
     </div>`;
    

    myShowErrorMessage(errorText: string): void {
        var messageBody = document.getElementById("messageBody");
        if (messageBody) {
            messageBody.classList.add("alert-danger");
            var txtMessage = document.getElementById("textMessage");
            txtMessage.innerHTML = errorText;
            messageBody.style.display = "block";
        }
    }

    myShowInfoMessage(infoText: string): void {
        var messageBody = document.getElementById("messageBody");
        if (messageBody) {
            messageBody.classList.add("alert-info");
            var txtMessage = document.getElementById("textMessage");
            txtMessage.innerHTML = infoText;
            messageBody.style.display = "block";
        }
    }

    myShowSuccessMessage(successText: string): void {
        var messageBody = document.getElementById("messageBody");
        if (messageBody) {
            messageBody.classList.add("alert-success");
            var txtMessage = document.getElementById("textMessage");
            txtMessage.innerHTML = successText;
            messageBody.style.display = "block";
        }
    }
}


let myAllertHelper = new myAlertHelper();

$(document).ready(
    function () {
        addMessageElement();
    }
);

function addMessageElement() {
    var messageContainer = document.getElementById("myMessageContainer");
    if (messageContainer) {
        messageContainer.innerHTML = myAllertHelper.messageHtmElement;
        $('#messageBody').on('closed.bs.alert', addMessageElement);
    }
}