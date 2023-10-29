(function () {
    'use strict';
    //define controller
    window.app.controller('vm.creditcardVerificationController', creditcardVerificationController)
        .directive('droppableArea', function () {
            return {
                scope: {
                    drop: '&' // parent
                },
                link: function (scope, element) {
                    var el = element[0];
                    var fileType = el.id === "dragdrophandlerfilePhotoID" ? "PhotoID" : "ProofOfRecidency";
                }
            };
        });

    creditcardVerificationController.$inject = ['$scope', '$window', '$http', '$interval', '$compile', 'model', 'ngToast'];

    function creditcardVerificationController($scope, $window, $http, $interval, $compile, model, ngToast) {

        var vm = this;
        vm.model = model;

        //create Toast
        vm.createToast = function (text, type) {
            var options = {
                content: text,
                className: type + ' ngtoast-message',
            };
            ngToast.create(options);
        };

        //On click of Upload Documents buttons enable Document upload view
        vm.enableDocumentUploadView = function () {
            $("#verifyTxtSecret-box").addClass("disable");
            $("#verifyTxtSecret-box").hide('slow');
            $("#documentUpload-box").removeClass("disable");
        }

        //on click of VerifyMe button call 'TxSecretRequest' Action Method 
        vm.verifyTxtSecrete = function () {
            $http.post("/TxSecretRequest", { useridentity: vm.model.userIdentity, txSecret: vm.verificationCode })
            .success(function (response, status) { //On success show verify modal 
                $('#verifyModal').find('#modalContent').html(response);
                $('#verifyModal').modal('show');
            }).error(function (data, status) { //If entered code is incorrect show error message in Toast
                if (data.errorCode === 'NoCustomerResponsePendingForOrder') {
                    $('#TxSecreteAttemptsExceededModal').find('#modalContent').html(data.errorMessage);
                    $('#TxSecreteAttemptsExceededModal').modal('show');
                }
                else if (data.errorCode === 'IncorrectTxsecretCode')
                    vm.createToast(data.errorMessage, "danger");
                else if (data.errorCode === 'TxAttemptExceedLimit') {
                    $('#TxSecreteAttemptsExceededModal').find('#modalContent').html(data.errorMessage);
                    $('#TxSecreteAttemptsExceededModal').modal('show');
                }
            });
        }

        //on click of Back button enable verifyTxtSecret and disable document Upload box
        vm.backToVerifyTxtSecrete = function () {
            $("#verifyTxtSecret-box").show('slow');
            $("#verifyTxtSecret-box").removeClass("disable");
            $("#documentUpload-box").addClass("disable");
            $("#txt-verifypasscode").focus();
        }

        //on click of Cancel Order button Show cancelOrderModal
        vm.cancelOrder = function () {
            $('#cancelOrderModal').modal('show');
        }

        //on click of Ok button in cancelOrderModal call CancelOrder action method and change order status
        vm.cancelOrderCloseWindowTab = function () {
            $http.post("/CancelOrder", { useridentity: vm.model.userIdentity })
             .success(function (response, status) {
                 //vm.createToast("Text secrete is verified successfully.", "info");
             }).error(function (data, status) {
             });
            close();
        }
        vm.closeWindowTab = function () {
            //close(); //close current tab in browser
            var p1 = document.getElementById('siteUrl').value;
            window.open(p1, "_self");
        }

        //on click of send button show documentUploadModal
        vm.submitDocumentUpload = function () {
            $http.post("/SendCreditCardDocumentPushOver", { useridentity: vm.model.userIdentity })
            .success(function (response, status) {
                
            }).error(function (data, status) {
            });
            $('#documentUploadModal').modal('show');
        }
    }
})();
