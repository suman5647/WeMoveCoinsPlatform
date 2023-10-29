var photoIdFileCount = 0;
var porFileCount = 0;
var documentFileCount = 0;
var kycRequirement = "";
function sendFileToServer(formData, fileType, status) { //function to upload file to data base

    var uploadURL = "/KYCFileUpload";
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
            $("#checkboxFileUpload").change(function () {
                if (this.checked) {
                    validateKYCForm();
                }
            });
            status.setProgress(100);
            $.each(resultData, function (index, value) {
                if (fileType === 'PhotoID') { //append photo id if file type is 'PhotoID'
                    $("#photoIDUploadedFiles").append('<div id=' + value.id + ' style="font-size: 7pt"><span><img src="images/close.png" alt="" onclick="removeFile('
                        + value.id + ',\'' + fileType + '\');"/> ' + value.originalFilename + '<br></span></div>');
                    photoIdFileCount++;
                    $("#loader").hide();
                    $('#id-box').removeClass("disable");

                }
                else if (fileType === 'ProofOfRecidency') { //append Proof Of Recidency if file type is 'ProofOfRecidency'
                    $("#proofOfRecidencyUploadedFiles").append('<div id=' + value.id + ' style="font-size: 7pt"><span><img src="images/close.png" alt="" onclick="removeFile(' + value.id + ',\'' + fileType + '\');"/> '
                        + value.originalFilename + "<br></span></div>");
                    porFileCount++;
                    $("#loader").hide();
                    $('#id-box').removeClass("disable");
                }
                else if (fileType === 'SelfieID') { //append PhotoID2 if file type is 'ProofOfRecidency'
                    $("#documentIDUploadedFiles").append('<div id=' + value.id + ' style="font-size: 7pt"><span><img src="images/close.png" alt="" onclick="removeFile(' + value.id + ',\'' + fileType + '\');"/> '
                        + value.originalFilename + "<br></span></div>");
                    documentFileCount++;
                    $("#loader").hide();
                    $('#id-box').removeClass("disable");
                }
            });
            validateKYCForm();
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            $("#loader").hide();
            $('#id-box').removeClass("disable");
            if (XMLHttpRequest.responseJSON.errorMessage != null) {
                var text = XMLHttpRequest.responseJSON.errorMessage;
                createToast(text, 'danger');
            }
            if (fileType === 'PhotoID') {
                $("#photoIDUploadedFiles").removeClass("file-upload-border");
                $("#photoIdThumbnail").remove();
                $('#fileUploadPhotoID').val('');
                //photoIdFileCount--;
            } else if (fileType === 'ProofOfRecidency') {
                //porFileCount--;
                $("#proofOfRecidencyUploadedFiles").removeClass("file-upload-border");
                $("#proofOfRecidencyThumbnail").remove();
                $("#fileUploadProofOfRecidency").val('');
            } else if (fileType === 'SelfieID') {
                //documentFileCount--;
                $("#documentIDUploadedFiles").removeClass("file-upload-border");
                $("#selfieIdThumbnail").remove();
                $("#fileUploadDocumentID").val('');
            }           
        }
    });

    status.setAbort(jqXHR);
}
var createToastProxy = (text, type) => { };

function registerToast(tostAction) {
    createToastProxy = tostAction;
}
function createToast(text, type) {
    createToastProxy(text, type);
};

function getKycRequirement(kycValue) {
    var decoded = kycValue.replace(/&amp;/g, '&');
    kycRequirement = decoded;
}


window.addEventListener("OnSucccess", function (evt) {
    if (evt.detail.isSuccess) {
        createToast('Success', 'info');
        var faceTecResponse = evt.detail;
        var uploadURL = "/FaceTecKYCFileUpload";
        var formData = {
            faceTecSession: faceTecResponse
        };
        $("#loader").show();
        $('#id-box').addClass("disable");        
        $.ajax({
            url: uploadURL,
            type: "POST",
            data: JSON.stringify(formData),
            contentType: 'application/json',
            success: function (resultData) {
                if (kycRequirement === "PhotoID&ProofOfRecidency") {
                    photoIdFileCount++;
                    document.getElementById("proofOfRes").style.display = 'inline';
                    document.getElementById("verification1").style.display = 'inline';
                    document.getElementById("verification2").style.display = 'inline';
                    document.getElementById("go-back").style.display = 'none';
                    document.getElementById("privacy-policy").style.display = 'none'; 
                    document.getElementById("checkboxTC").style.display = 'none';
                    document.getElementById("privacy-policy1").style.display = 'none';
                    document.getElementById("checkboxTC1").style.display = 'none';
                    document.getElementById("facetec").style.display = 'none';
                    document.getElementById("automated-id1").style.display = 'none';
                    document.getElementById("automated-id2").style.display = 'none';
                    $("#loader").hide();
                    $('#id-box').removeClass("disable");
                } else {
                    $("#loader").hide();
                    $('#id-box').removeClass("disable");
                    var evt = new CustomEvent("submitKYC");
                    window.dispatchEvent(evt);
                }
                
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                $("#loader").hide();
                $('#id-box').removeClass("disable");
                if (XMLHttpRequest.responseJSON.errorMessage != null) {
                    var text = XMLHttpRequest.responseJSON.errorMessage;
                    createToast(text, 'danger');
                }
            }
        });
    }
    else {
        createToast('Unsuccessful. Please retry again', 'danger');
        validateFaceTecTC();
    }
}, false);

function validateKYCForm() {
    $("#btn-submit").prop('disabled', true);
    if (kycRequirement === 'PhotoID') {
        if ($("#checkboxFileUpload").prop("checked") === true && photoIdFileCount > 0) {
            $("#btn-submit").prop('disabled', false);
        }
    } else if (kycRequirement === 'ProofOfRecidency') {
        if ($("#checkboxFileUpload").prop("checked") === true && porFileCount > 0) {
            $("#btn-submit").prop('disabled', false);
        }
    } else if (kycRequirement === 'SelfieID') {
        if ($("#checkboxFileUpload").prop("checked") === true && documentFileCount > 0) {
            $("#btn-submit").prop('disabled', false);
        }
    } else if (kycRequirement === 'PhotoID&ProofOfRecidency') {
        if (porFileCount > 0) {
            $("#btn-submit").prop('disabled', false);
        }
    }
    else {
        if ($("#checkboxFileUpload").prop("checked") === true && photoIdFileCount > 0 && documentFileCount > 0) {
            $("#btn-submit").prop('disabled', false);
        }
        else if ($("#checkboxFileUpload").prop("checked") === true && porFileCount > 0 && photoIdFileCount > 0) {
            $("#btn-submit").prop('disabled', false);
        }
    }
}

function validateFaceTecTC() {
    $("#photo-id-match-button").prop('disabled', true);
    if ($("#checkboxFaceTec").prop("checked") === true)
    {
        $("#photo-id-match-button").prop('disabled', false);
    }
}

var rowCount = 0;
function createStatusbar(obj) { //create status bar
    rowCount++;
    var row = "odd";
    if (rowCount % 2 === 0) row = "even";
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

function showLoadingIndicator(show = false) {
    if (show === false) {
        $("#loader").hide();
        $("#id-box").removeClass("disable");
    }
    else {
        $("#loader").show();
        $("#id-box").addClass("disable");
    }

}
//function to upload multiple files which accepts only PNG, JPG and JPEG extensions
function handleMultipleFileUpload(files, obj, fileType) {
    if (fileType === 'PhotoID') {
        $("#photoIDUploadedFiles").addClass("file-upload-border");
    }
    else if (fileType === 'SelfieID') {
        $("#documentIDUploadedFiles").addClass("file-upload-border");
    }
    else if (fileType === 'ProofOfRecidency') {
        $("#proofOfRecidencyUploadedFiles").addClass("file-upload-border");
    }
    this.showLoadingIndicator(true);
    const reader = new FileReader();
    for (var i = 0; i < files.length; i++) {
        //var ext = files[i].name.match(/\.(.+)$/)[1];
        //const file = document.querySelector('input[type=file]').files[i];
        const file = files[i];
        var ext = files[i].name.split('.').pop();
        switch (ext.toUpperCase()) {
            case 'PNG':
            case 'JPG':
            case 'JPEG':
            case 'PDF':
                break;
            default:
                this.showLoadingIndicator(false)
                alert('File types with .' + ext + ' extensions is not allowed.');
                return;
        }
        if (file) {
            reader.readAsDataURL(file);
        }
        var formData = new FormData();
        formData.append('kycFile' + i, files[i]);
        formData.append('fileType', fileType);

        var status = new createStatusbar(obj); //Using this we can set progress.
        status.setFileNameSize(files[i].name, files[i].size);
        sendFileToServer(formData, fileType, status);
    }
    if (fileType === 'PhotoID') {
        $("#photoIDUploadedFiles").append('<img id="photoIdThumbnail" src=""  style ="max-width: 100%;max-height: 90%;" alt="">');
        const previewPhotoID = document.querySelector("#photoIdThumbnail");
        reader.addEventListener("load", function () {
            //    // convert image file to base64 string
            previewPhotoID.src = reader.result;
        }, false);
    }
    else if (fileType === 'SelfieID') {
        $("#documentIDUploadedFiles").append('<img id="selfieIdThumbnail" src=""  style ="max-width: 100%;max-height: 90%;" alt="">');
        const previewSelfieID = document.querySelector("#selfieIdThumbnail");
        reader.addEventListener("load", function () {
            //    // convert image file to base64 string
            previewSelfieID.src = reader.result;
        }, false);
    }
    else if (fileType === 'ProofOfRecidency') {
        $("#proofOfRecidencyUploadedFiles").append('<img id="proofOfRecidencyThumbnail" src=""  style ="max-width: 90%;max-height: 90%;" alt="">');
        const previewRecidenceID = document.querySelector("#proofOfRecidencyThumbnail");
        reader.addEventListener("load", function () {
            //    // convert image file to base64 string
            previewRecidenceID.src = reader.result;
        }, false);
    }
}function handleFileUpload(file, obj, fileType) {
    var formData = new FormData();
    formData.append('kycFile', file);
    formData.append('fileType', fileType);

    var status = new createStatusbar(obj); //Using this we can set progress.
    status.setFileNameSize(file.name, file.size); //function to set size of file
    sendFileToServer(formData, fileType, status); //function for sending files to server
}
function addDragDropHandlers(obj, fileType) {
    obj.on('dragenter', function (e) { //on drag enter make changes to border from dash to solid
        e.stopPropagation();
        e.preventDefault();
        $(this).css('border', '2px solid #92AAB0');
    });
    obj.on('dragleave', function (e) { //on drag leave make changes to border from solid to dash
        e.stopPropagation();
        e.preventDefault();
        $(this).css('border', '2px dashed #92AAB0');
    });
    obj.on('dragover', function (e) {
        e.stopPropagation();
        e.preventDefault();
    });
    obj.on('drop', function (e) { //on drop call handleMultipleFileUpload function
        $(this).css('border', '2px dashed #92AAB0');
        e.preventDefault();
        handleMultipleFileUpload(e.originalEvent.dataTransfer.files, obj, fileType);
    });
}
//function to delete uploaded KYC file
function removeFile(id, fileType) {
    var jqXHR = $.ajax({
        url: '/DeleteKYCFile',
        type: "POST",
        data: { id: id },
        success: function (resultData) {
            $('#' + id).remove();
            if (fileType === 'PhotoID') {
                $("#photoIDUploadedFiles").removeClass("file-upload-border");
                $("#photoIdThumbnail").remove();
                $('#fileUploadPhotoID').val('');
                photoIdFileCount--;
            } else if (fileType === 'ProofOfRecidency') {
                porFileCount--;
                $("#proofOfRecidencyUploadedFiles").removeClass("file-upload-border");
                $("#proofOfRecidencyThumbnail").remove();
                $("#fileUploadProofOfRecidency").val('');
            } else if (fileType === 'SelfieID') {
                documentFileCount--;
                $("#documentIDUploadedFiles").removeClass("file-upload-border");
                $("#selfieIdThumbnail").remove();
                $("#fileUploadDocumentID").val('');
            }
            validateKYCForm();
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
        }
    });
}

function initkycFileUpload(kycRequirementType) {
    var decoded = kycRequirementType.replace(/&amp;/g, '&');
    kycRequirement = decoded;
    addDragDropHandlers($("#dragdrophandlerfileProofOfRecidency"), 'ProofOfRecidency');  //call addDragDropHandlers function for uploading Proof Of Recidency file
    addDragDropHandlers($("#dragdrophandlerfilePhotoID"), 'PhotoID'); //call addDragDropHandlers function for uploading Photo ID
    addDragDropHandlers($("#dragdrophandlerfileDocument"), 'SelfieID'); //call addDragDropHandlers function for uploading PhotoID2
    $(document).on('dragenter', function (e) {
        e.stopPropagation();
        e.preventDefault();
    });
    $(document).on('dragover', function (e) { //on drag over make changes to border from solid to dash
        e.stopPropagation();
        e.preventDefault();
        $("#dragdrophandlerfileProofOfRecidency").css('border', '2px dashed #92AAB0');
        $("#dragdrophandlerfilePhotoID").css('border', '2px dashed #92AAB0');
        $("#dragdrophandlerfileDocument").css('border', '2px dashed #92AAB0');
    });
    $(document).on('drop', function (e) {
        e.stopPropagation();
        e.preventDefault();
    });
}

//Payment procedure for Paylike service
function initPayLikePaymentFrame(MerchantNumber, OrderNumber, TxSecret, CurrencyCode, Amount, SiteName, orderNumber, amount, commission, googleTagManagerId) {
    //On load of paylike popup resetting height of tab bar
    $('.tab-pages').height(100);

    //open PayLike popup
    Paylike(MerchantNumber).popup({
        currency: CurrencyCode,
        amount: Amount,
        custom: { OrderNumber: OrderNumber },
        descriptor: SiteName + ":" + TxSecret,
        overwrite_usage: SiteName + ":" + TxSecret
    }, function (err, r) {
        if (err)
            return console.warn(err);
        var formData = new FormData();
        formData.append('tid', r.transaction.id);

        $('#paymentFrameLoading').show();

        //call PayLikeAccept action method
        var jqXHR = $.ajax({
            url: "/PayLikeAccept",
            type: "POST",
            contentType: false,
            processData: false,
            cache: false,
            data: formData,
            success: function (resultData) {
                updateDataLayerTransaction(orderNumber, amount, CurrencyCode, commission, googleTagManagerId)
                $('#paymentFrameLoading').hide();
                $('#tabContent').html(resultData);
                $("#orderInfoProgressBarItem").removeClass("active");
                $("#verificationProgressBarItem").removeClass("active");
                $("#paymentProgressBarItem").removeClass("active");
                $("#receiptProgressBarItem").addClass("active");
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
            }
        });
    });
}

//function updateDataLayerTransaction(orderNumber, amount, currencyCode, commission, googleTagManagerId) {
//    dataLayer.push({
//        'virtualPage': '/' + userLang + '/funnel/receipt',
//        'event': 'virtualPageview'
//    });
//    dataLayer.push({
//        'transactionId': orderNumber,
//        'transactionTotal': amount,
//        'transactionTax': '0',
//        'transactionShipping': commission,
//        'transactionProducts': [{
//            'name': 'SellBitcoins',
//            'price': amount,
//            'currency': currencyCode,
//            'quantity': '1'
//        }],
//        'event': 'transactionComplete'
//    });
//    //(function (w, d, s, l, i) { w[l] = w[l] || []; w[l].push({ 'gtm.start': new Date().getTime(), event: 'gtm.js' }); var f = d.getElementsByTagName(s)[0], j = d.createElement(s), dl = l != 'dataLayer' ? '&l=' + l : ''; j.async = true; j.src = 'https://www.googletagmanager.com/gtm.js?id=' + i + dl; f.parentNode.insertBefore(j, f); })(window, document, 'script', 'dataLayer', googleTagManagerId);
//}