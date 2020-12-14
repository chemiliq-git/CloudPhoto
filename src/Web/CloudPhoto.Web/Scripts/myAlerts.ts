class myAlertHelper {
    
    messageHtmElement: string =
    `<div id="messageBody" class="alert alert-dismissible fade show" role="alert" style="display:none">
       <p id="textMessage" class="text-center">Your message must show here</p>
          <button type="button" class="close" data-dismiss="alert" aria-label="Close">
             <span aria-hidden="true">&times;</span>
          </button>
     </div>`;

    controlName: string;

    constructor(controlName: string) {
        this.controlName = controlName;
        var selfI = this;

        $(document).ready(
            function () {
                selfI.addMessageElement();
            }
        );
    }

    addMessageElement(this: myAlertHelper): void {
        var messageContainer = document.getElementById(this.controlName);
        if (messageContainer) {
            messageContainer.innerHTML = this.messageHtmElement;
            var selfI = this;
            $('#messageBody').on('closed.bs.alert', function () {
                selfI.addMessageElement();
            });
        }
    }

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