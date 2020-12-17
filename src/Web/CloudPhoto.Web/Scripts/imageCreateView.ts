var myAllertHelper = new myAlertHelper("myMessageContainer");

function uploadControlFile(inputId) {
    var input = document.getElementById(inputId);
    var files = (<any>input).files;
    var formData = new FormData();

    for (var i = 0; i != files.length; i++) {
        formData.append("file", files[i]);
    }
    uploadFile(formData);
}

function uploadFile(formData) {

    var token = $("#dragFileForm input[name=__RequestVerificationToken]").val();
    $.ajax(
        {
            url: "/api/uploadfiles/UploadImage",
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
                    $("#uploadImageId").attr("src", data.imageUrl);
                    //fill input form image sourse
                    $("#image-url").attr("value", data.imageUrl);
                    $("#image-id").attr("value", data.fileId);
                    //show input form
                    (<any>$("#main-form")).collapse('show');
                }
                else {
                    // show error (myAlets.js) 
                    myAllertHelper.myShowErrorMessage(data.errorMessage);
                }
            }
        }
    );
}

function initializeDragAndDropArea() {
    if (typeof (window["FileReader"]) == "undefined") {
        return;
    }

    var dragAndDropArea = $("#dragAndDropArea");

    if (dragAndDropArea.length == 0) {
        return;
    }

    dragAndDropArea[0].ondragover = function () {
        dragAndDropArea.addClass("drag-and-drop-area-dragging");
        return false;
    };

    dragAndDropArea[0].ondragleave = function () {
        dragAndDropArea.removeClass("drag-and-drop-area-dragging");
        return false;
    };

    dragAndDropArea[0].ondrop = function (event) {
        dragAndDropArea.removeClass("drag-and-drop-area-dragging");

        var formData = new FormData();

        for (var i = 0; i != event.dataTransfer.files.length; i++) {
            formData.append("file", event.dataTransfer.files[i]);
        }
        uploadFile(formData);

        return false;
    }
}

function selectTag(selectData: AutocompleteParam) {
    addNewTag(selectData.value);
}

function addNewTag(newTag: string) {
    //change hide field
    var data = (<HTMLInputElement>document.getElementById("added-tags")).value;
    var js;
    if (data != "") {
        js = JSON.parse(data);
        if (js.includes(newTag)) {
            (<HTMLInputElement>document.getElementById("inputTag")).value = "";
            return;
        }

        js.push(newTag);
    }
    else {
        js = new Array(newTag);
    }


    var btntag = document.createElement("button");
    btntag.id = newTag;
    btntag.className = "btn-success rounded m-1";
    btntag.onclick = function () {
        var data = (<HTMLInputElement>document.getElementById("added-tags")).value
        var js = JSON.parse(data);
        var filtered = js.filter(function (value, index, arr) {
            return value != btntag.id;
        });

        if (filtered != '') {
            (<HTMLInputElement>document.getElementById("added-tags")).value = JSON.stringify(filtered);
        }
        else {
            (<HTMLInputElement>document.getElementById("added-tags")).value = '';
        }
        btntag.remove();
    };

    //add icon
    var icon = document.createElement("i");
    icon.className = "far fa-times-circle";
    btntag.appendChild(icon);
    //add text
    var text = document.createTextNode(newTag);
    btntag.appendChild(text);
    //add btntag on document
    var element = document.getElementById("newElements");
    element.appendChild(btntag);

    (<HTMLInputElement>document.getElementById("added-tags")).value = JSON.stringify(js);

    (<HTMLInputElement>document.getElementById("inputTag")).value = "";
}

function lisiningForAddTag() {
    let node: HTMLInputElement = <HTMLInputElement>document.getElementById("inputTag");
    node.addEventListener("keydown", function (event) {
        if (event.key === "Enter") {
            if (node.value) {
                addNewTag(node.value);
                //delete input data
                node.value = "";

                event.returnValue = false;
                (<any>event).cancel = true;
            }
        }
    });
}

$(document).ready(
    function () {
        initializeDragAndDropArea();

        lisiningForAddTag();

        var autoCompleateHelper = new myAutocompleteHelper(undefined, selectTag, '#inputTag');
        autoCompleateHelper.configAutoCompleteTags();
    }
);

(<any>$).validator.setDefaults({ ignore: '' });