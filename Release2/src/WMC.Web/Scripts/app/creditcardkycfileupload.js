function sendFileToServer(formData, fileType, status) { //function to upload file to data base
    var uploadURL = "/CreditCardKYCFileUpload";
    var jqXHR = $.ajax({
        xhr: function () {
            var xhrobj = $.ajaxSettings.xhr();
            if (xhrobj.upload) {
                xhrobj.upload.addEventListener('progress', function (event) {
                    var percent = 0;
                    var position = event.loaded || event.position;
                    var total = event.total;
                    if (event.lengthComputable) {
                        percent = Math.ceil(position / total * 100);
                    }
                    status.setProgress(percent);
                }, false);
            }
            return xhrobj;
        },
        url: uploadURL,
        type: "POST",
        contentType: false,
        processData: false,
        cache: false,
        data: formData,
        success: function (resultData) {
            status.setProgress(100);
            $.each(resultData, function (index, value) {
                $("#btn-submit").prop('disabled', false);
                if (fileType == 'PhotoID') { //append photo id if file type is 'PhotoID'
                    $("#photoIDUploadedFiles").append("<span id=" + value.id + "><img src='images/close.png' alt='' onclick='removeFile(" + value.id + ");'/> "
                        + value.originalFilename + "<br></span>");
                }
                else if (fileType == 'ProofOfRecidency') { //append Proof Of Recidency if file type is 'ProofOfRecidency'
                    $("#proofOfRecidencyUploadedFiles").append("<span id=" + value.id + "><img src='images/close.png' alt='' onclick='removeFile(" + value.id + ");'/> "
                        + value.originalFilename + "<br></span>");
                }
            });
        },
        error: (function (response, status) { // on error show proper error message
            $('#KYCFileTypeErrorModal').find('#modalContent').html(response.responseJSON.errorMessage);
            $('#KYCFileTypeErrorModal').modal('show');
        })
    });

    status.setAbort(jqXHR);
}

var rowCount = 0;
function createStatusbar(obj) { //create status bar
    rowCount++;
    var row = "odd";
    if (rowCount % 2 == 0) row = "even";
    this.statusbar = $("<div class='statusbar " + row + "'></div>");
    this.filename = $("<div class='filename'></div>").appendTo(this.statusbar);
    this.size = $("<div class='filesize'></div>").appendTo(this.statusbar);
    this.progressBar = $("<div class='progressBar'><div></div></div>").appendTo(this.statusbar);
    this.abort = $("<div class='abort'>Abort</div>").appendTo(this.statusbar);

    this.setFileNameSize = function (name, size) {
        var sizeStr = "";
        var sizeKB = size / 1024; // calculate size of file
        if (parseInt(sizeKB) > 1024) {
            var sizeMB = sizeKB / 1024;
            sizeStr = sizeMB.toFixed(2) + " MB";
        }
        else {
            sizeStr = sizeKB.toFixed(2) + " KB";
        }

        this.filename.html(name);
        this.size.html(sizeStr);
    }
    this.setProgress = function (progress) { //set progress bar
        var progressBarWidth = progress * this.progressBar.width() / 100;
        this.progressBar.find('div').animate({ width: progressBarWidth }, 10).html(progress + "% ");
        if (parseInt(progress) >= 100) {
            this.abort.hide();
        }
    }
    this.setAbort = function (jqxhr) {
        var sb = this.statusbar;
        this.abort.click(function () {
            jqxhr.abort();
            sb.hide();
        });
    }
}
//function to upload multiple files which accepts only PNG, JPG and JPEG extensions
function handleMultipleFileUpload(files, obj, fileType, userIdentity) {
    for (var i = 0; i < files.length; i++) {
        var ext = files[i].name.split('.').pop(); //get file extension from file name
        switch (ext.toUpperCase()) {
            case 'PNG':
            case 'JPG':
            case 'JPEG':
            //case 'PDF':
                break;
            default:
                alert('File types with .' + ext + ' extensions is not allowed.');
                return;
        }
        var formData = new FormData();
        formData.append('kycFile' + i, files[i]);
        formData.append('fileType', fileType);
        formData.append('userIdentity', userIdentity);

        var status = new createStatusbar(obj); //Using this we can set progress.
        status.setFileNameSize(files[i].name, files[i].size); //function to set size of file
        sendFileToServer(formData, fileType, status); //function for sending files to server
    }
}

function addDragDropHandlers(obj, fileType, userIdentity) {
    obj.on('dragenter', function (e) { //on drag enter make changer border from dash to solid
        e.stopPropagation();
        e.preventDefault();
        $(this).css('border', '2px solid #92AAB0');
    });
    obj.on('dragleave', function (e) { //on drag leave make changer border from solid to dash
        e.stopPropagation();
        e.preventDefault();
        $(this).css('border', '2px dashed #92AAB0');
    });
    obj.on('dragover', function (e) {
        e.stopPropagation();
        e.preventDefault();
    });
    obj.on('drop', function (e) {
        $(this).css('border', '2px dashed #92AAB0');
        e.preventDefault();
        handleMultipleFileUpload(e.originalEvent.dataTransfer.files, obj, fileType, userIdentity); //on drop call handleMultipleFileUpload function
    });
}
//function to delete uploaded KYC file
function removeFile(id) {
    var jqXHR = $.ajax({
        url: '/CreditCardDeleteKYCFile',
        type: "POST",
        data: { id: id },
        success: function (resultData) {
            $('#' + id).remove();
            //if ($("#photoIDUploadedFiles").text() == "" || $("#proofOfRecidencyUploadedFiles").text() == "")
            $("#btn-submit").prop('disabled', 'disabled'); //disable send button
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
        }
    });
}

function initkycFileUpload(userIdentity) {
    addDragDropHandlers($("#dragdrophandlerfileProofOfRecidency"), 'ProofOfRecidency', userIdentity); //call addDragDropHandlers function for uploading Proof Of Recidency file
    //addDragDropHandlers($("#dragdrophandlerfilePhotoID"), 'PhotoID', userIdentity);
    $(document).on('dragenter', function (e) {
        e.stopPropagation();
        e.preventDefault();
    });
    $(document).on('dragover', function (e) {
        e.stopPropagation();
        e.preventDefault();
        $("#dragdrophandlerfileProofOfRecidency").css('border', '2px dashed #92AAB0');
        //$("#dragdrophandlerfilePhotoID").css('border', '2px dashed #92AAB0');
    });
    $(document).on('drop', function (e) {
        e.stopPropagation();
        e.preventDefault();
    });
}

//function initPaymentFrame() {
//    var loadCounter = 0
//    $('#paymentFrame').load(function (a) {
//        $('#paymentFrameLoading').hide()
//        loadCounter++;
//        if (loadCounter == 2) {
//            $("#orderDetail").hide();
//            $("#orderInfoProgressBarItem").removeClass("active");
//            $("#verificationProgressBarItem").removeClass("active");
//            $("#paymentProgressBarItem").removeClass("active");
//            $("#receiptProgressBarItem").addClass("active");
//            $("#recieptPageActions").show();
//        }
//    });
//}

//function initPayLikePaymentFrame(MerchantNumber, OrderNumber, TxSecret, CurrencyCode, Amount, SiteName) {
//    //On load of paylike popup resetting height of tab bar
//    $('.tab-pages').height(100);

//    Paylike(MerchantNumber).popup({
//        currency: CurrencyCode,
//        amount: Amount,
//        custom: { OrderNumber: OrderNumber },
//        descriptor: SiteName + ":" + TxSecret
//    }, function (err, r) {
//        if (err)
//            return console.warn(err);
//        var formData = new FormData();
//        formData.append('tid', r.transaction.id);

//        $('#paymentFrameLoading').show();

//        var jqXHR = $.ajax({
//            url: "/PayLikeAccept",
//            type: "POST",
//            contentType: false,
//            processData: false,
//            cache: false,
//            data: formData,
//            success: function (resultData) {
//                $('#paymentFrameLoading').hide();
//                $('#tabContent').html(resultData);
//                $("#orderInfoProgressBarItem").removeClass("active");
//                $("#verificationProgressBarItem").removeClass("active");
//                $("#paymentProgressBarItem").removeClass("active");
//                $("#receiptProgressBarItem").addClass("active");
//            },
//            error: function (XMLHttpRequest, textStatus, errorThrown) {
//            }
//        });
//    });
//}