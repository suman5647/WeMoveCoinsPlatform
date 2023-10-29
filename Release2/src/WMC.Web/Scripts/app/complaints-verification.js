(function () {
    'use strict';

    window.app.controller('vm.complaintVerificationController', complaintVerificationController)
        .directive('droppableArea', function () {
            return {
                scope: {
                    drop: '&' // parent
                },
                link: function (scope, element) {
                    var el = element[0];
                    var fileType = el.id == "dragdrophandlerfilePhotoID" ? "PhotoID" : "ProofOfRecidency";
                    //el.addEventListener('dragenter', function (e) {
                    //    e.stopPropagation();
                    //    e.preventDefault();
                    //    $(this).css('border', '2px solid #92AAB0');
                    //});
                    //el.addEventListener('dragleave', function (e) {
                    //    e.stopPropagation();
                    //    e.preventDefault();
                    //    $(this).css('border', '2px dashed #92AAB0');
                    //});
                    //el.addEventListener('dragover', function (e) {
                    //    e.stopPropagation();
                    //    e.preventDefault();
                    //});
                    //el.addEventListener('drop', function (e) {
                    //    $(this).css('border', '2px dashed #92AAB0');
                    //    e.preventDefault();
                    //    scope.handleMultipleFileUpload(e.dataTransfer.files, el, fileType);
                    //});
                }
            };
        });

    complaintVerificationController.$inject = ['$scope', '$window', '$http', '$interval', '$compile', 'model', 'ngToast'];

    function complaintVerificationController($scope, $window, $http, $interval, $compile, model, ngToast) {

        var newPopoverTemplate = '<div class="popover bitcoin-address-help" role="popover">' +
                                     '<div class="arrow"></div>' +
                                        '<h3 class="popover-title"></h3>' +
                                     '<div class="popover-content bitcoin-address-help-popover-content"></div>' +
                                  '</div>';

        BitCoinAddressAndPhoneNumberHelpPopup();
        function BitCoinAddressAndPhoneNumberHelpPopup() {
            $('#bitcoinAddressHelp').popover({
                //title: 'What is a bitcoin address?' + '<a class="close" href="#");">&times;</a>',
                content: function () {
                    return $('#bitcoinAddressHelpContent').html();
                },
                html: true,
                template: newPopoverTemplate,
                trigger: "focus"
            });
            $('#phoneNumberHelp').popover({
                //title: 'Why my mobile number?' + '<a class="close" href="#");">&times;</a>',
                content: function () {
                    return $('#phoneNumberHelpContent').html();
                },
                html: true,
                trigger: "focus"
            });
            $('#phoneNumberHelp').on('shown.bs.popover', function () {
                var id = $(this).data('bs.popover').$tip[0].id;
                // $('#' + id).attr('style', 'margin-top:-20px')
                $('#' + id).addClass('phone-help');
                $('.phone-help').find('.arrow').attr('style', 'top:60%');

            })
        }

        var vm = this;
        vm.model = model;

        vm.createToast = function (text, type) {
            var options = {
                content: text,
                className: type + ' ngtoast-message',
            };
            ngToast.create(options);
        };

        vm.enableDocumentUploadView = function () {
            $("#verifyTxtSecret-box").addClass("disable");
            $("#verifyTxtSecret-box").hide('slow');
            $("#documentUpload-box").removeClass("disable");
        }

        vm.verifyPhoneCode = function () {
            $http.post("/ComplaintsVerifyPhoneNumber", { userIdentity: vm.model.userIdentity, verificationCode: vm.verificationCode })
            .success(function (response, status) {
                //$('#verifyModal').find('#modalContent').html(response);
                //$('#verifyModal').modal('show');
                //vm.createToast("Text secrete is verified successfully.", "info");
                vm.enableDocumentUploadView();
            }).error(function (data, status) {
                if (data.errorMessage == 'Sorry, the entered code is incorrect.')
                    vm.createToast(data.errorMessage, "danger");
                else {
                    $('#verifyModal').find('#modalContent').html(data.errorMessage);
                    $('#verifyModal').modal('show');
                }
            });
        }

        vm.ResendVerifyPhoneCode = function () {
            $http.post("/ResendVerificationCode", { userIdentity: vm.model.userIdentity })
            .success(function (response, status) {
                vm.createToast("Verification code sent successfully.", "info");
            }).error(function (data, status) {
                vm.createToast("Error in sending verification code.", "info");
            });
        }

        vm.backToVerifyTxtSecrete = function () {
            $("#verifyTxtSecret-box").show('slow');
            $("#verifyTxtSecret-box").removeClass("disable");
            $("#documentUpload-box").addClass("disable");
            $("#txt-verifypasscode").focus();
        }

        vm.cancelOrder = function () {
            $('#cancelOrderModal').modal('show');
        }

        vm.cancelOrderCloseWindowTab = function () {
            $http.post("/CancelOrder", { useridentity: vm.model.userIdentity })
             .success(function (response, status) {
                 //vm.createToast("Text secrete is verified successfully.", "info");
             }).error(function (data, status) {
             });
            close();
        }
        vm.closeWindowTab = function () {
            close();
        }

        vm.submitDocumentUpload = function () {
            $('#documentUploadModal').modal('show');
        }
    }
})();