(function () {
    'use strict';
    //defining controller
    window.app.controller('vm.paymentController', paymentController)
        .directive('keyPressed', function () {
            return function (scope, element, attrs) {
                element.bind("keydown keypress", function (event) {
                    if (event.which === 13) {
                        scope.$apply(function () {
                            scope.$eval(attrs.myEnter);
                        });
                        scope.enterkeyPressed();
                        event.preventDefault();
                    }
                });
            }
        })
        .directive('opacity', function opacity($timeout) {
            return {
                link: function (scope, element, attrs) {
                    var value = attrs.opacity;
                    $timeout(function () {
                        element[0].style.opacity = value;
                    }, 500);
                }
            }
        })
        //create directive for bitcoinAddress
        .directive('buyAmount', function () {
            return {
                restrict: 'A',
                require: 'ngModel',
                link: function (scope, element, attributes, ngModel) {
                    ngModel.$parsers.unshift(function (modelValue) {
                        scope.validateBuyAmount(modelValue);
                        return modelValue;
                    });
                }
            };
        })
        //create directive for bitcoinAddress
        .directive('bitcoinAddress', function () {
            return {
                restrict: 'A',
                require: 'ngModel',
                link: function (scope, element, attributes, ngModel) {
                    ngModel.$parsers.unshift(function (modelValue) {
                        scope.validateBitcoinAddress(modelValue, true, false);
                        return modelValue;
                    });
                }
            };
        })
        //create directive for fullName
        .directive('fullName', function () {
            return {
                restrict: 'A',
                require: 'ngModel',
                link: function (scope, element, attributes, ngModel) {
                    ngModel.$parsers.unshift(function (modelValue) {
                        scope.validateFullName(modelValue);
                        return modelValue;
                    });
                }
            };
        })
        //create directive for couponCode
        .directive('couponCode', function () {
            return {
                restrict: 'A',
                require: 'ngModel',
                link: function (scope, element, attributes, ngModel) {
                    ngModel.$parsers.unshift(function (modelValue) {
                        scope.validateCoupon(modelValue);
                        return modelValue;
                    });
                }
            };
        })
        //create directive for SWIFT
        .directive('swift', function () {
            return {
                restrict: 'A',
                require: 'ngModel',
                link: function (scope, element, attributes, ngModel) {
                    ngModel.$parsers.unshift(function (modelValue) {
                        scope.validateSwift(modelValue);
                        return modelValue;
                    });
                }
            };
        })
        //create directive for Bank Value1 validation
        .directive('validbankval1', function () {
            return {
                restrict: 'A',
                require: 'ngModel',
                link: function (scope, element, attributes, ngModel) {
                    ngModel.$parsers.unshift(function (modelValue) {
                        scope.validateBankValue1(scope, modelValue);
                        return modelValue;
                    });
                }
            };
        })
        //create directive for Bank Value2 validation
        .directive('validbankval2', function () {
            return {
                restrict: 'A',
                require: 'ngModel',
                link: function (scope, element, attributes, ngModel) {
                    ngModel.$parsers.unshift(function (modelValue) {
                        scope.validateBankValue2(scope, modelValue);
                        return modelValue;
                    });
                }
            };
        })
        //create directive for IBAN
        .directive('iban', function () {
            return {
                restrict: 'A',
                require: 'ngModel',
                link: function (scope, element, attributes, ngModel) {
                    ngModel.$parsers.unshift(function (modelValue) {
                        scope.validateIBAN(modelValue);
                        return modelValue;
                    });
                }
            };
        })
        //create directive for Reg
        .directive('reg', function () {
            return {
                restrict: 'A',
                require: 'ngModel',
                link: function (scope, element, attributes, ngModel) {
                    ngModel.$parsers.unshift(function (modelValue) {
                        scope.validateReg(modelValue);
                        return modelValue;
                    });
                }
            };
        })
        //create directive for Account
        .directive('account', function () {
            return {
                restrict: 'A',
                require: 'ngModel',
                link: function (scope, element, attributes, ngModel) {
                    ngModel.$parsers.unshift(function (modelValue) {
                        scope.validateAccount(modelValue);
                        return modelValue;
                    });
                }
            };
        })
        //create directive for email
        .directive('email', function () {
            return {
                restrict: 'A',
                require: 'ngModel',
                link: function (scope, element, attributes, ngModel) {
                    ngModel.$parsers.unshift(function (modelValue) {
                        scope.validateEmail(modelValue);
                        return modelValue;
                    });
                }
            };
        })
        //create directive for mobile
        .directive('mobile', function () {
            return {
                restrict: 'A',
                require: 'ngModel',
                link: function (scope, element, attributes, ngModel) {
                    ngModel.$parsers.unshift(function (modelValue) {
                        scope.validateMobile(modelValue);
                        scope.orderSizeBoundaryLimit(modelValue);
                        return modelValue;
                    });
                }
            };
        })
        .directive('inputMask', function () {
            return {
                restrict: 'A',
                link: function (scope, el, attrs) {
                    $(el).inputmask(scope.$eval(attrs.inputMask));
                    $(el).on('change', function () {
                        scope.$eval(attrs.ngModel + "='" + el.val() + "'");
                        // or scope[attrs.ngModel] = el.val() if your expression doesn't contain dot.
                    });
                }
            };
        })
        //create directive for tc
        .directive('tc', function () {
            return {
                restrict: 'A',
                require: 'ngModel',
                link: function (scope, element, attributes, ngModel) {
                    ngModel.$parsers.unshift(function (modelValue) {
                        scope.validateTermsAndConditions(modelValue);
                        return modelValue;
                    });
                }
            };
            //create directive for droppableArea
        })
        .directive('droppableArea', function () {
            return {
                scope: {
                    drop: '&' // parent
                },
                link: function (scope, element) {
                    var el = element[0];
                    var fileType = el.id === "dragdrophandlerfilePhotoID" ? "PhotoID" : "ProofOfRecidency";
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
        })
        .run(function () {
            $('#progressbarContainer').show();
        });


    paymentController.$inject = ['$scope', '$window', '$http', '$interval', '$compile', 'model', 'ngToast'];

    function paymentController($scope, $window, $http, $interval, $compile, model, ngToast) {
        var selectedPaymentMethod;
        var userLang = navigator.language.substr(0, 2);
        var newPopoverTemplate = '<div class="popover bitcoin-address-help" role="popover">' +
            '<div class="arrow"></div>' +
            '<h3 class="popover-title"></h3>' +
            '<div class="popover-content bitcoin-address-help-popover-content"></div>' +
            '</div>';

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
        if (vm.model.receiptModel !== null && vm.model.receiptModel.type === 1) {
            if (vm.model.receiptModel.creditCardNumber) {
                updateDataLayerReceiptCreditCard(vm.model.receiptModel.orderNumber, vm.model.receiptModel.amount, vm.model.receiptModel.currency, vm.model.receiptModel.country, vm.model.receiptModel.couponCode, vm.model.receiptModel.type, vm.model.receiptModel.paymentMethod, vm.model.youReceiveBitcoins)
            }
        }
        if (vm.model.operationalStatus === "opensellclosebuy" || vm.model.operationalStatus === "closesellopenbuy") {
            $("#cbBuySellSlider").attr('disabled', 'disabled');
            $("#spBuySellSlider").attr('style', 'background-color: lightgray');
        }
        if (vm.model.operationalStatus === "bankonly") {
            setTimeout(function () {
                $("#cbbPaymentType option[value*='CreditCard']").prop('disabled', 'disabled');
            }, 1000);
        }
        setTimeout(function () {
            $("#forCryptoCurrency option[value*='BTC']").attr('style', "background-image:url(images/BTCLogo.png)");
        }, 1000);
        //vm.initialLoad = function () { //on change of payment method call paymentMethodChanged which returns OrderSizeBoundary
        //    debugger;
        //    $http.post("/index", {
        //        amount: getParameterByName('amount'),
        //        currency: getParameterByName('currency'),
        //        name: getParameterByName('name'),
        //        cryptoAddress: getParameterByName('cryptoAddress'),
        //        paymentMethod: getParameterByName('paymentMethod'),
        //        phoneCode: getParameterByName('phoneCode'),
        //        phoneNumber: getParameterByName('phoneNumber'),
        //        email: getParameterByName('email')
        //    }).success(function (response, status) {
        //        alert('success');
        //    }).error(function (data, status) {
        //        alert('error');
        //    });
        //}
        //vm.initialLoad();

        //function getParameterByName(name) {
        //    name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
        //    var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
        //        results = regex.exec(location.search);
        //    return results === null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
        //}

        //create Toast
        vm.createToast = function (text, type) {
            var options = {
                content: text,
                className: type + ' ngtoast-message',
            };
            ngToast.create(options);
        };
        registerToast(vm.createToast);
        //create cookie
        function createCookie(name, value, days) {
            var expires = "";
            if (days) {
                var date = new Date();
                date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
                expires = "; expires=" + date.toGMTString();
            }
            document.cookie = name + "=" + value + expires + "; path=/";
        }
        //read cookie
        function readCookie(name) {
            var nameEQ = name + "=";
            var ca = document.cookie.split(';');
            for (var i = 0; i < ca.length; i++) {
                var c = ca[i];
                while (c.charAt(0) === ' ') c = c.substring(1, c.length);
                if (c.indexOf(nameEQ) === 0) return c.substring(nameEQ.length, c.length);
            }
            return null;
        }
        function checkCookie(name) {
            return readCookie(name) !== null;
        }
        vm.paymentMethods = vm.model.paymentMethodDetail.paymentMethods;
        
        if (vm.model.buyPaymentMethods === null || vm.model.buyPaymentMethods === "") {
            vm.paymentMethod = vm.model.paymentMethodDetail;
        }
        else {
            vm.paymentMethod = vm.model.buyPaymentMethods;
        }

        vm.sellPaymentMethods = vm.model.sellPaymentMethodDetail.paymentMethods;
        if (vm.model.sellPaymentMethods === null || vm.model.sellPaymentMethods === "") {
            vm.sellPaymentMethod = vm.model.sellPaymentMethodDetail;
        }
        else {
            vm.sellPaymentMethod = vm.model.sellPaymentMethods;
        }

        // ForCurrency, ForSellCurrency, ForBuyCCCurrency;
        vm.sellBankCurrencyCode = vm.model.sellBankCurrency;
        vm.buyBankCurrencyCode = vm.model.forCurrency;
        vm.sellCCCurrencyCode = vm.buyCCCurrencyCode = vm.model.forBuyCCCurrency;
        vm.placeOrderButtonDisabled = true;
        vm.IBANButtonDisabled = true;
        vm.validBuyAmount = false;
        vm.validBitcoinAddress = vm.model.type === 2 ? true : false;
        vm.validFullName = false;
        vm.validEmail = false;
        vm.validMobile = false;
        vm.validReCaptcha = vm.model.reCaptchaStatus !== "Enabled";
        vm.minersFee = vm.model.minersFee;
        vm.cardFee = vm.model.cardFee;
        //$("#txtMobileNumber").inputmask({ mask: vm.model.mobileNumberFormat, greedy: false });
        vm.validTermsAndConditions = false;
        vm.recieveNewsletters = true;
        function ContainsInArray(array, item) {
            if (array.find(function (a) { return a === item; }) === item)
                return true;
            else
                return false;
        }

        function updateOrderDetail(amount) {
            vm.paymentMethodFee = $.global.format(vm.cardFee, "n2", vm.model.cultureCode);
            var fee = vm.cardFee;
            var commision = 0.0;
            vm.fixedFee = 0;
            var paymentMethod = vm.model.type === 1 ? vm.paymentMethod.name : vm.sellPaymentMethod.name;
            
            vm.paymentMethodCommision = vm.model.type === 2 ? vm.model.sellPaymentMethodDetail.bankCommission.commission : (paymentMethod === "CreditCard" ? vm.model.paymentMethodDetail.ccCommission.commission : vm.model.paymentMethodDetail.bankCommission.commission);

            var spreadFactor = vm.model.type === 2 ? (vm.model.sellPaymentMethodDetail.spread / 100) : (vm.model.paymentMethodDetail.spread / 100);
            if (vm.model.fxMarkUp === undefined || vm.model.fxMarkUp === null) vm.model.fxMarkUp = 0;
            var fxMarkUpFactor = (vm.model.fxMarkUp / 100);


            if (vm.model.type === 2) {
                vm.paymentMethodFee = $.global.format((vm.model.sellPaymentMethodDetail.bankCommission.fee), "n2", vm.model.cultureCode);
                fee = vm.cardFee / 100;
                commision = vm.model.sellPaymentMethodDetail.bankCommission.commission / 100;
                vm.paymentMethodCommision = vm.model.sellPaymentMethodDetail.bankCommission.commission;
                if (vm.model.sellPaymentMethods.name === "Bank") {
                    vm.fixedFee = vm.model.sellPaymentMethodDetail.bankCommission.fixedFee;
                    if (vm.fixedFee === 0) {
                        $("#fixedFee").css('display', 'none');
                    }
                }
            }

            if (amount === "" || amount <= 0) {
                if (vm.model.type === 1) {
                    vm.youPayBuyAmount = "0 " + vm.model.forCurrency;
                    vm.fee = "0 " + vm.model.forCurrency;
                    vm.commision = "0 " + vm.model.forCurrency;
                    vm.youReceiveBitcoins = "0 " + vm.model.forDigitalCurrency;
                } else {
                    vm.youPayBuyAmount = "0 " + vm.model.forDigitalCurrency;
                    vm.fee = "0 " + vm.model.forDigitalCurrency;
                    vm.commision = "0 " + vm.model.forDigitalCurrency;
                    vm.youReceiveBitcoins = "0 " + vm.model.forCurrency;
                }
            }
            else {
                if (vm.model.type === 1) {
                    fee = vm.cardFee / 100;
                    commision = vm.model.paymentMethodDetail.bankCommission.commission / 100;
                    if (paymentMethod === "CreditCard") {
                        commision = vm.model.paymentMethodDetail.ccCommission.commission / 100;
                        vm.fee = $.global.format(((amount * fee)), "n2", vm.model.cultureCode) + " " + vm.model.forCurrency;
                    }
                    else
                        vm.fee = $.global.format((amount * fee), "n2", vm.model.cultureCode) + " " + vm.model.forCurrency;
                }
                else {
                    //if (vm.forSellCurrency !== "BTC") {
                    if (!ContainsInArray(vm.model.digitalCurrencies, vm.forSellCurrency)) {
                        //amount = amount / (vm.model.btc2SellCurrencyNumeric * spreadFactor * fxMarkUpFactor);
                        amount = amount / (vm.model.btc2SellCurrencyNumeric * (1 - spreadFactor - fxMarkUpFactor));
                        vm.discount = (amount * commision) * (vm.discountPercentage / 100);
                    }

                    if (vm.sellPaymentMethod.name === "CreditCard")
                        vm.fee = $.global.format((amount * fee * vm.model.euroBtcRate * vm.model.euroCurrencyRate), "n2", vm.model.cultureCode) + " " + vm.model.forCurrency;
                }

                var rate = 0;
                if (vm.model.type === 1)
                    rate = parseFloat(vm.model.btc2LocalCurrencyNumeric) * (1 + spreadFactor + fxMarkUpFactor);
                else
                    rate = parseFloat(vm.model.btc2LocalCurrencyNumeric) * (1 - spreadFactor - fxMarkUpFactor);
                vm.minersFee = $.global.format(vm.model.minersFee * rate, "n2", vm.model.cultureCode) + " " + vm.model.forCurrency;
                if (vm.discountPercentage === undefined || vm.discountPercentage === 0) {
                    vm.commision = $.global.format((amount * commision), "n2", vm.model.cultureCode) + " " + vm.model.forCurrency;
                    vm.discount = 0;
                    vm.discountPercentage = 0;
                }
                else {
                    vm.discount = (amount * commision) * (vm.discountPercentage / 100);
                    vm.commision = $.global.format(((amount * commision) - vm.discount), "n2", vm.model.cultureCode) + " " + vm.model.forCurrency;
                }
                if (vm.model.type === 1) {
                    var buyAmount = 0;
                    if (vm.model.buyPaymentMethods.name === "CreditCard")
                        buyAmount = (amount - (amount * fee) - (amount * commision) - (vm.model.minersFee * rate)) / rate;
                    else
                        buyAmount = (amount - (amount * commision) - (vm.model.minersFee * rate)) / rate;
                    vm.youPayBuyAmount = $.global.format((amount * 1), "n2", vm.model.cultureCode) + " " + vm.model.forCurrency;

                    vm.youReceiveBitcoins = $.global.format(buyAmount, "n8", vm.model.cultureCode) + " " + vm.model.forDigitalCurrency;
                }
                else {
                    var sellAmount = 0;
                    if (vm.sellPaymentMethod.name === "CreditCard")
                        sellAmount = (amount - (amount * fee) - (amount * commision) - vm.discount);
                    else
                        sellAmount = (amount - (amount * commision) - vm.discount);
                    vm.fixedFeeLocalCurrency = $.global.format(vm.fixedFee * vm.model.euroCurrencyRate, "n2", vm.model.cultureCode) + " " + vm.model.forCurrency;
                    vm.youPayBuyAmount = $.global.format((amount * 1), "n8", vm.model.cultureCode) + " " + vm.model.forSellCurrency;

                    if (vm.discountPercentage === 0)
                        vm.commision = $.global.format((amount * commision) * rate, "n2", vm.model.cultureCode) + " " + vm.model.forCurrency;
                    else
                        vm.commision = $.global.format((amount * commision - vm.discount) * rate, "n2", vm.model.cultureCode) + " " + vm.model.forCurrency;

                    vm.youReceiveBitcoins = $.global.format((sellAmount * rate) - (vm.fixedFee * vm.model.euroCurrencyRate), "n2", vm.model.cultureCode) + " " + vm.model.forCurrency;
                    getSellMessage((sellAmount * rate) - (vm.fixedFee * vm.model.euroCurrencyRate), vm.model.forDigitalCurrency, vm.model.forCurrency);
                }
            }
        }
        function getSellMessage(amount, digitalCurrency, currency) {
            $.get("/GetSellMessageContent?amount=" + amount + "&digitalCurrency=" + digitalCurrency + "&sellCurrency=" + currency, function (response) {
                vm.sellMessage = response.message;
            });
        }
        vm.sellBankValidations;
        vm.orderInfoViewEnabled = true;
        vm.verifyBitcoinViewEnabled = false;
        vm.verifyPhoneNumberViewEnabled = false;
        vm.verifyIdentityViewEnabled = false;
        vm.ibanViewEnabled = false;
        //vm.paymentViewEnabled = false;
        $scope.enterkeyPressed = function () { //onclick of Enter button validate BuyAmount, FullName, Email, Mobile, BitcoinAddress
            if (vm.orderInfoViewEnabled) {
                $scope.validateBuyAmount(vm.model.buyAmount);
                $scope.validateFullName(vm.model.fullName);
                $scope.validateEmail(vm.model.eMail);
                $scope.validateMobile(vm.model.mobile);
                if (vm.model.couponCode !== "") $scope.validateCoupon(vm.model.couponCode);

                if (vm.model.type === 1) {
                    $scope.validateBitcoinAddress(vm.model.bitcoinAddress, true, true);
                }
            }
            if (vm.verifyBitcoinViewEnabled) {
                vm.confirmBitcoinAddress();
            }
            if (vm.verifyPhoneNumberViewEnabled) {
                vm.verifyPhoneNumber();
            }
            if (vm.verifyIdentityViewEnabled) {
                if (!$("#btn-submit").prop('disabled')) {
                    vm.submitIdentityVerification(true);
                }
            }
            if (vm.model.ViewName === 'IBAN') {
                $scope.validateIBAN(vm.IBAN, vm.Reg, vm.Account);
            }
        }



        function refreshPhoneCodes() {
            if (vm.model.receiptModel) {
                receiptModelCurrencyRates(vm.model.receiptModel);
            } else {
                var hasSelectedCurrency = false;
                var paymentMethodName = vm.model.type === 1 ? vm.model.buyPaymentMethods.name : vm.model.sellPaymentMethods.name;
                selectedPaymentMethod = vm.model.type === 1 ? vm.paymentMethod : vm.sellPaymentMethod;
                if (selectedPaymentMethod === null) {
                    paymentMethodName = vm.model.type === 1 ? vm.model.buyPaymentMethods.name : vm.model.sellPaymentMethods.name;
                }
                paymentMethodName = selectedPaymentMethod; 
                var paymentTypeId = paymentMethodName.name === "CreditCard" ? 4 : (vm.model.type === 1 ? 1 : 2);
                var selectedPhoneExist = false;
                vm.filterPhoneCodes = vm.model.phoneCodes.filter(obj => { if (vm.model.phoneCode && vm.model.phoneCode === obj.item3) selectedPhoneExist = true; return (obj.item5 & paymentTypeId) > 0; });
                var filterCurrencyList = vm.model.currencies.filter(obj => {
                    //if ((vm.model.type === 1 && !vm.model.forCurrency && vm.model.forCurrency === obj.item1) ||
                    //    (vm.model.type === 2 && !vm.forSellCurrency && vm.forSellCurrency === obj.item1)) {
                    //    hasSelectedCurrency = true;
                    //}
                    return (obj.item2 & paymentTypeId) > 0;//filter
                }).map(ele => ele.item1);

                vm.forSellCurrency = vm.model.forSellCurrency;
                vm.filterCurrencies = filterCurrencyList;

                if (vm.model.type === 2) {
                    vm.model.updatedSellCurrencies = filterCurrencyList.map((x) => x);;
                    vm.model.updatedSellCurrencies.push(vm.model.forDigitalCurrency);
                    switch (paymentMethodName) {
                        case 'CreditCard':
                            vm.model.forCurrency = vm.sellCCCurrencyCode;
                            break;
                        case 'Bank':
                        default:
                            vm.model.forCurrency = vm.sellBankCurrencyCode;
                            break;
                    }
                } else {
                    switch (paymentMethodName) {
                        case 'CreditCard':
                            vm.model.forCurrency = vm.buyCCCurrencyCode;
                            break;
                        case 'Bank':
                        default:
                            vm.model.forCurrency = vm.buyBankCurrencyCode;
                            break;
                    }
                }

                if (!selectedPhoneExist) {
                    vm.model.phoneCodeId = vm.filterPhoneCodes[0].item1;
                    vm.model.phoneCode = vm.model.phoneCodes.filter(function (item) { return item.item1 === vm.model.phoneCodeId; })[0].item3;
                    $.get('/getPhoneNumberStyle?phonecode=' + vm.model.phoneCode, function (response) {
                        vm.model.mobile = '';
                        vm.model.mobileNumberFormat = response.phoneNumberStyle;
                        vm.cardFee = response.cardFee;
                        $("#txtMobileNumber").inputmask({ mask: vm.model.mobileNumberFormat, greedy: false });
                        updateOrderDetail(vm.model.buyAmount);
                        $scope.$apply();
                    });
                }

                internalCurrencyChanged();

                // vm.model.forCurrency = vm.filterCurrencies[0];                    
                //$.get('/currencyChanged?currency=' + vm.model.forCurrency + '&forsellCurrency=' + vm.forSellCurrency + '&cryptoCurrency=' + vm.model.forDigitalCurrency + '&paymentMethod=' + paymentMethodName + '&type=' + vm.model.type, function (response) {
                //    vm.model.btc2LocalBuyTicker = response.buyTicker.data.formatedRate;
                //    vm.model.btc2LocalSellTicker = response.sellTicker.data.formatedRate;
                //    $scope.$apply();
                //});
            }
        }
        refreshPhoneCodes();
        function receiptModelCurrencyRates(receiptModel) {
            $.get('/currencyChanged?currency=' + receiptModel.currency + '&forsellCurrency=' + vm.forSellCurrency + '&cryptoCurrency=' + vm.model.forDigitalCurrency + '&paymentMethod=CreditCard' + '&type=' + vm.model.type, function (response) {
                vm.model.btc2LocalBuyTicker = response.buyTicker.data.formatedRate;
                vm.model.btc2LocalSellTicker = response.sellTicker.data.formatedRate;
                $scope.$apply();
            });
        }
        $scope.validateSwift = function (value) {
            $('#showVal1NullError').css('display', 'none');
            if (value === null || value === '') {
                $('#showVal1NullError').css('display', 'inherit');
                vm.validSwift = false;
            } else {
                vm.validSwift = true;
            }
            validateIBANForm();
        }
        $scope.validateBankValue1 = function (scope, value) {
            validateBankValue1Form(scope, value);
        }
        $scope.validateBankValue2 = function (scope, value) {
            validateBankValue2Form(scope, value);
        }
        $scope.validateIBAN = function (value) {
            $('#showVal2InvalidError').css('display', 'inherit');
            vm.validIBAN = IBAN.isValid(value);
            if (vm.validIBAN) {
                $('#showVal2InvalidError').css('display', 'none');
            }
            validateIBANForm();
        }
        $scope.validateReg = function (value) {
            $('#showVal1NullError').css('display', 'none');
            $('#showVal1InvalidError').css('display', 'none');
            $('#showVal1TooShortError').css('display', 'none');
            vm.validReg = false;
            if (value === null || value === '') {
                $('#showVal1NullError').css('display', 'inherit');
            } else if (value < 0 || value > 9999) {
                $('#showVal1InvalidError').css('display', 'inherit');
            } else if (value.length < 4) {
                $('#showVal1TooShortError').css('display', 'inherit');
            } else {
                vm.validReg = true;
            }
            validateIBANForm();
        }
        $scope.validateAccount = function (value) {
            $('#showVal2NullError').css('display', 'none');
            $('#showVal2InvalidError').css('display', 'none');
            $('#showVal2TooShortError').css('display', 'none');
            vm.validAccount = false;
            if (value === null || value === '') {
                $('#showVal2NullError').css('display', 'inherit');
            } else if (value < 0 || value > 9999999999) {
                $('#showVal2InvalidError').css('display', 'inherit');
            } else if (value.length < 10) {
                $('#showVal2TooShortError').css('display', 'inherit');
            } else {
                vm.validAccount = true;
            }
            validateIBANForm();
        }
        $scope.resetValidationErrors = function () {
            $('#showBuyAmountNullError').css('display', 'none');
            $('#showBuyAmountInvalidError').css('display', 'none');
            $('#showBuyAmountInvalidErrorBuy').css('display', 'none');
            $('#showBuyAmountInvalidErrorSell').css('display', 'none');
        }
        $scope.validateBuyAmount = function (amount) {
            vm.validBuyAmount = false;
            $scope.resetValidationErrors();
            if (!amount || amount === '') { //If amount is null or empty show error message
                $('#buyAmountDetail').css('opacity', '0');
                $('#buyAmountDetail').css('display', 'none');
                $('#showBuyAmountNullError').css('display', 'inherit');
            }
            else {  //If amount is greater than max value or less than min value show error message
                $('#buyAmountDetail').css('opacity', '1');
                $('#buyAmountDetail').css('display', 'inherit');
                $('#showBuyAmountNullError').css('display', 'none');
                if (vm.model.type === 1) {
                    if (amount && amount < vm.model.orderSizeBoundary.min || amount > vm.model.orderSizeBoundary.max) {
                        $('#showBuyAmountInvalidError').css('display', 'inherit');
                        $('#showBuyAmountInvalidErrorBuy').css('display', 'inherit');
                    }
                    else {
                        $('#showBuyAmountInvalidError').css('display', 'none');
                        $('#showBuyAmountInvalidErrorBuy').css('display', 'none');
                        vm.validBuyAmount = true;
                    }
                }
                else {
                    if (amount && amount < vm.model.orderSizeBoundary.min || amount > vm.model.orderSizeBoundary.max) {
                        //if (vm.forSellCurrency === 'BTC') {
                        if (ContainsInArray(vm.model.digitalCurrencies, vm.forSellCurrency)) {
                            vm.model.orderSizeBoundary.min = parseFloat(vm.model.orderSizeBoundary.min.toFixed(8));
                            vm.model.orderSizeBoundary.max = parseFloat(vm.model.orderSizeBoundary.max.toFixed(8));
                        }
                        else {
                            vm.model.orderSizeBoundary.min = Math.ceil(vm.model.orderSizeBoundary.min);
                            vm.model.orderSizeBoundary.max = Math.floor(vm.model.orderSizeBoundary.max);
                        }
                        $('#showBuyAmountInvalidError').css('display', 'inherit');
                        $('#showBuyAmountInvalidErrorSell').css('display', 'inherit');
                    }
                    else {
                        $('#showBuyAmountInvalidError').css('display', 'none');
                        $('#showBuyAmountInvalidErrorSell').css('display', 'none');
                        vm.validBuyAmount = true;
                    }
                }
            }
            updateOrderDetail(amount);
            validateForm();
        };
        $scope.orderSizeBoundaryLimit = function (phoneNumber) {
            var selectedPaymentMethod = vm.model.type === 1 ? vm.paymentMethod : vm.sellPaymentMethod;
            var paymentMethodName = selectedPaymentMethod.name; 
            
            $http.post("/GetUserSizeBoundarySize", {
                currency: vm.model.forCurrency,
                forsellCurrency: vm.forSellCurrency,
                paymentMethod: paymentMethodName,
                type: vm.model.type,
                phoneNumber: phoneNumber,
                phoneCode: vm.model.phoneCode
            }).success(function (response, status) {
                vm.model.orderSizeBoundary.minStr = response.orderSizeBoundary.minStr;
                vm.model.orderSizeBoundary.maxStr = response.orderSizeBoundary.maxStr;
                if (vm.model.type === 1) {
                    vm.model.orderSizeBoundary.min = Math.ceil(response.orderSizeBoundary.min);
                    vm.model.orderSizeBoundary.max = Math.floor(response.orderSizeBoundary.max);
                    vm.paymentMethod.commission = response.commission;
                } else {
                    if (ContainsInArray(vm.model.digitalCurrencies, vm.forSellCurrency)) {
                        vm.model.orderSizeBoundary.min = parseFloat(response.orderSizeBoundary.min.toFixed(8));
                        vm.model.orderSizeBoundary.max = parseFloat(response.orderSizeBoundary.max.toFixed(8));
                        vm.model.sellPaymentMethodDetail = response.sellPaymentMethodDetail;
                        vm.sellPaymentMethod.commission = response.sellPaymentMethodDetail.bankCommission.commission;
                    } else {
                        vm.model.orderSizeBoundary.min = Math.ceil(response.orderSizeBoundary.min);
                        vm.model.orderSizeBoundary.max = Math.floor(response.orderSizeBoundary.max);
                        vm.model.sellPaymentMethodDetail = response.sellPaymentMethodDetail;
                        vm.sellPaymentMethod.commission = response.sellPaymentMethodDetail.bankCommission.commission;
                    }
                }
                $scope.validateBuyAmount(vm.model.buyAmount);
                updateOrderDetail(vm.model.buyAmount);
                // $scope.$apply();
            });
        };
        //$scope.validateBitcoinAddress = function (value, doValidateForm, doSubmitForm) { //if Bitcoin Address is not null or empty then call ValidateBitcoinAddress action method
        //    vm.validBitcoinAddress = false;
        //    $("#loader").show();
        //    $('#orderForm').addClass("disable");
        //    $('#showBitcoinAddressNullError').css('display', 'none');
        //    $('#showBitcoinAddressInvalidError').css('display', 'none');
        //    if (value === null || value === '') { //if Bitcoin Address is empty or null show error message
        //        $("#loader").hide();
        //        $('#orderForm').removeClass("disable");
        //        $('#showBitcoinAddressNullError').css('display', 'inherit');
        //    } else {
        //        $.get('/ValidateBitcoinAddress?bitcoinAddress=' + value + '&forDigitalCurrency=' + vm.model.forDigitalCurrency, function (response) { //call ValidateBitcoinAddress action method with parameter bitcoinAddress
        //            if (response === 'Valid') {
        //                $("#loader").hide();
        //                $('#orderForm').removeClass("disable");
        //                $('#showBitcoinAddressInvalidError').css('display', 'none');
        //                vm.validBitcoinAddress = true; //if response is Valid set validBitcoinAddress to true else show bitcoin addres invalid error message
        //            }
        //            else {
        //                $("#loader").hide();
        //                $('#orderForm').removeClass("disable");
        //                $('#showBitcoinAddressInvalidError').css('display', 'inherit');
        //            }
        //            if (doValidateForm) {
        //                validateForm();
        //            }
        //            if (doSubmitForm) {
        //                if (!vm.placeOrderButtonDisabled)
        //                    vm.submitOrder(true);
        //            }
        //        });
        //    }
        //    validateForm();
        //}


        $scope.validateBitcoinAddress = function (value, doValidateForm, doSubmitForm) { //if Bitcoin Address is not null or empty then call ValidateBitcoinAddress action method
            vm.validBitcoinAddress = false;
            $("#loader").show();
            $('#orderForm').addClass("disable");
            $('#showBitcoinAddressNullError').css('display', 'none');
            $('#showBitcoinAddressInvalidError').css('display', 'none');
            if (value === null || value === '') { //if Bitcoin Address is empty or null show error message
                $("#loader").hide();
                $('#orderForm').removeClass("disable");
                $('#showBitcoinAddressNullError').css('display', 'inherit');
            } else {
                var valid = WAValidator.validate(value, vm.model.forDigitalCurrency, vm.model.isBitgoTest === true ? 'testnet' : undefined); //call ValidateBitcoinAddress action method with parameter bitcoinAddress
                if (valid) {
                    $("#loader").hide();
                    $('#orderForm').removeClass("disable");
                    $('#showBitcoinAddressInvalidError').css('display', 'none');
                    vm.validBitcoinAddress = true; //if response is Valid set validBitcoinAddress to true else show bitcoin addres invalid error message
                }
                else {
                    $("#loader").hide();
                    $('#orderForm').removeClass("disable");
                    $('#showBitcoinAddressInvalidError').css('display', 'inherit');
                }
                if (doValidateForm) {
                    validateForm();
                }
                if (doSubmitForm) {
                    if (!vm.placeOrderButtonDisabled)
                        vm.submitOrder(true);
                }
            }
            validateForm();
        };



        $scope.validateFullName = function (value) {
            vm.validFullName = false;
            $('#showFullNameNullError').css('display', 'none');
            $('#showFullNameInvalidError').css('display', 'none');
            $('#showFullNameTooLongError').css('display', 'none');
            if (value === null || value === '') { //if full name is empty or null show error message
                $('#showFullNameNullError').css('display', 'inherit');
            } else if (value.length > 50) { //if length of full name is greater than 50 show error message
                $('#showFullNameTooLongError').css('display', 'inherit');
            } else {
                if (validateFullName(value)) { // if full name doesn't contain last name show error message
                    $('#showFullNameInvalidError').css('display', 'none');
                    vm.validFullName = true;
                }
                else {
                    $('#showFullNameInvalidError').css('display', 'inherit');
                }
            }
            validateForm();
        };
        $scope.validateEmail = function (value) {
            vm.validEmail = false;
            $('#showEmailAddressNullError').css('display', 'none');
            $('#showEmailAddressInvalidError').css('display', 'none');
            $('#showEmailAddressTooLongError').css('display', 'none');

            if (value === null || value === '') { //if email address is empty or null show error message
                $('#showEmailAddressNullError').css('display', 'inherit');
            } else if (value.length > 50) { //if length of email address is greater than 50 show error message
                $('#showEmailAddressTooLongError').css('display', 'inherit');
            } else {
                if (validateEmailAddress(value)) { // check for email address syntax 
                    $('#showEmailAddressInvalidError').css('display', 'none');
                    vm.validEmail = true;
                }
                else {
                    $('#showEmailAddressInvalidError').css('display', 'inherit');
                }
            }
            validateForm();
        };
        $scope.validateMobile = function (value) {
            $('#showMobileNumberNullError').css('display', 'none');
            $('#showMobileNumberInvalidError').css('display', 'none');
            if (value === null || value === '') { //if mobile number is empty or null show error message
                vm.validMobile = false;
                $('#showMobileNumberNullError').css('display', 'inherit');
            } else {
                if (!Inputmask.isValid(value, { mask: vm.model.mobileNumberFormat })) { //if mobile number format is not valid show error message
                    vm.validMobile = false;
                    $('#showMobileNumberInvalidError').css('display', 'inherit');
                } else {
                    vm.validMobile = true;
                    $('#showMobileNumberNullError').css('display', 'none');
                }
            }
            validateForm();
        };
        $scope.validateCoupon = function (value) {
            if (value.length > 0) {
                $('#coupontext').text('Validating..');
                document.getElementById("couponicon").className = "fa fa-refresh fa-spin";
                $http.post("/ValidateCouponCode", {
                    couponCode: value,
                    lang: userLang
                }).success(function (data, status) {
                    vm.discountPercentage = data.discount;
                    vm.validCouponCode = data.validity;
                    vm.couponErrorMessage = data.errorMessage
                    if (true === vm.validCouponCode) {
                        $http({
                            url: "/GetMessage",
                            method: "GET",
                            params: {
                                key: "CouponAppliedSuccessfully",
                                language: (userLang === 'en') ? ' ' : userLang
                            }
                        }).success(function (response) {
                            vm.createToast(response, "info");
                        })
                        $('#coupontext').text(' Valid ');
                        $('#couponbtn').css('background-color', '#449d44');
                        document.getElementById("couponicon").className = "fa fa-check";
                    }
                    else {
                        if (vm.couponErrorMessage === "InvalidOrExpired") {
                            $http({
                                url: "/GetMessage",
                                method: "GET",
                                params: {
                                    key: vm.couponErrorMessage,
                                    language: (userLang === 'en') ? ' ' : userLang
                                }
                            }).success(function (response) {
                                vm.createToast(response, "danger");
                            })
                        }
                        if (vm.couponErrorMessage === "MaxTxnCountReached") {
                            $http({
                                url: "/GetMessage",
                                method: "GET",
                                params: {
                                    key: vm.couponErrorMessage,
                                    language: (userLang === 'en') ? ' ' : userLang
                                }
                            }).success(function (response) {
                                vm.createToast(response, "danger");
                            })
                        }
                        $('#coupontext').text(' Invalid ');
                        $('#couponbtn').css('background-color', '#d9534f');
                        document.getElementById("couponicon").className = "fa fa-times";
                    }
                    if (vm.model.buyAmount === "" || vm.buyAmount <= 0) {
                        // do nothing??
                    }
                    else {
                        updateOrderDetail(vm.model.buyAmount);
                    }
                    validateForm();
                }).error(function (data, status) {
                    vm.createToast(data.errors[0].item2, "danger");
                    dataLayer.push({
                        'ErrorMessage': data.errorMessage,
                        'event': 'error'
                    });
                });
            }
            else {
                document.getElementById("couponicon").className = "";
                $('#coupontext').text(' Validate ');
                $('#couponbtn').css('background-color', '#449d44');
                vm.discountPercentage = 0;
                if (vm.model.buyAmount === "" || vm.buyAmount <= 0) {
                    // do nothing??
                }
                else {
                    updateOrderDetail(vm.model.buyAmount);
                }
            }
        };
        $(document).ready(function () {
            BitCoinAddressAndPhoneNumberHelpPopup();
            //if (vm.model.couponCode !== "") $scope.validateCoupon(vm.model.couponCode); 
            //vm.phoneCodeChanged();
            fetchSellBankValidation(vm.model.forCurrency);
        });
        $scope.validateTermsAndConditions = function (value) { // validate Terms And Conditions
            if (value === null || value === true) {
                vm.validTermsAndConditions = true;
            } else {
                vm.validTermsAndConditions = false;
            }
            validateForm();
        };
        function validateFullName(name) { // validate Full name
            var words = $.trim(name).split(' ');
            if (words.length < 2)
                return false;
            var firstname = $.trim(name).split(' ')[0];
            var lastname = $.trim(name).split(' ')[1];

            if (firstname.length > 1 && lastname.length > 1)
                return true;
            else
                return false;
        }
        function validateEmailAddress(email) {
            var re = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
            return re.test(email);
        }
        //function validateEmailAddress(emailAddress) {
        //    var atpos = emailAddress.indexOf("@");
        //    var dotpos = emailAddress.lastIndexOf(".");
        //    if (atpos < 1 || dotpos < atpos + 2 || dotpos + 2 >= emailAddress.length)
        //        return false;
        //    else
        //        return true;
        //}
        function validateForm() {
            if (vm.validBuyAmount && (vm.model.type === 2 ? true : vm.validBitcoinAddress) && vm.validFullName && vm.validEmail && vm.validMobile && vm.validTermsAndConditions) //&& (vm.model.couponCode === "" ? true : vm.validCouponCode) 
                vm.placeOrderButtonDisabled = false;
            else
                vm.placeOrderButtonDisabled = true;
        }

        vm.isValidBankValue1Form = false;
        vm.isValidBankValue2Form = false;

        vm.bankValue1FormError = "";
        vm.bankValue2FormError = "";

        function validateBankValue1Form(scope, value) {
            vm.bankValue1FormError = validateBankValue(scope, vm.Value1LabelResourceName, value);
            vm.isValidBankValue1Form = (vm.bankValue1FormError === "");
            if (vm.bankValue1FormError === "") {
                $('#showVal1NullError').css('display', 'none');
                $('#showVal1TooShortError').css('display', 'none');
                $('#showVal1TooLongError').css('display', 'none');
                $('#showVal1InvalidError').css('display', 'none');
            } else {
                showErrorMessageBankValue1Form();
            }
            validateIBANForm();
        }
        function validateBankValue2Form(scope, value) {
            vm.bankValue2FormError = validateBankValue(scope, vm.Value2LabelResourceName, value);
            vm.isValidBankValue2Form = (vm.bankValue2FormError === "");
            if (vm.bankValue2FormError === "") {
                $('#showVal2NullError').css('display', 'none');
                $('#showVal2TooShortError').css('display', 'none');
                $('#showVal2TooLongError').css('display', 'none');
                $('#showVal2InvalidError').css('display', 'none');
            } else {
                showErrorMessageBankValue2Form();
            }
            validateIBANForm();
        }

        function showErrorMessageBankValue1Form() {
            $('#showVal1NullError').css('display', 'none');
            $('#showVal1TooShortError').css('display', 'none');
            $('#showVal1TooLongError').css('display', 'none');
            $('#showVal1InvalidError').css('display', 'none');
            if (vm.bankValue1FormError === "min") {
                $('#showVal1TooShortError').css('display', 'inherit');
            }
            else if (vm.bankValue1FormError === "max") {
                $('#showVal1TooLongError').css('display', 'inherit');
            }
            else if (vm.bankValue1FormError === "null") {
                $('#showVal1NullError').css('display', 'inherit');
            }
            else if (vm.bankValue1FormError === "invalid") {
                $('#showVal1InvalidError').css('display', 'inherit');
            }
            else {
                return;
            }
        }
        function showErrorMessageBankValue2Form() {
            $('#showVal2NullError').css('display', 'none');
            $('#showVal2TooShortError').css('display', 'none');
            $('#showVal2TooLongError').css('display', 'none');
            $('#showVal2InvalidError').css('display', 'none');
            if (vm.bankValue2FormError === "min") {
                $('#showVal2TooShortError').css('display', 'inherit');
            }
            else if (vm.bankValue2FormError === "max") {
                $('#showVal2TooLongError').css('display', 'inherit');
            }
            else if (vm.bankValue2FormError === "null") {
                $('#showVal2NullError').css('display', 'inherit');
            }
            else if (vm.bankValue2FormError === "invalid") {
                $('#showVal2InvalidError').css('display', 'inherit');
            }
            else {
                return;
            }
        }

        function fetchSellBankValidation(currencyCode) {
            $.get("/GetSellBankValidationSetting?currencyCode=" + currencyCode).done(function (response) {
                vm.sellBankValidations = response;
            });

        }

        function getSellBankValidation(labelResourceName) {
            for (var i = 0; i < vm.sellBankValidations.length; i++) {
                if (vm.sellBankValidations[i].labelName === labelResourceName) {
                    return vm.sellBankValidations[i];
                }
            }
        }

        function validateBankValue(scope, labelResourceName, value) {
            var bankValidation = getSellBankValidation(labelResourceName);
            var regNumeric = /^\d+$/;
            var regAplphaNumeric = /^[a-z0-9]+$/i;
            if (value === null || value === '') {
                return 'null';
            } else if (bankValidation.minLength !== 0 && value.length < bankValidation.minLength) {
                return 'min';
            } else if (bankValidation.maxLength !== 0 && value.length > bankValidation.maxLength) {
                return 'max';
            } else if (bankValidation.regex !== "" && !((new RegExp(bankValidation.regex, "i")).test(value))) {
                return 'invalid';
            } else if (bankValidation.type === "numeric" && !regNumeric.test(value)) {
                return 'invalid';
            } else if (bankValidation.type === "alphanumeric" && !regAplphaNumeric.test(value)) {
                return 'invalid';
            } else if (bankValidation.requiresSpecialValidation) {
                if (labelResourceName === 'Bank_IBAN') {
                    if (!(IBAN.isValid(value))) {
                        return 'invalid';
                    } else {
                        return "";
                    }
                } else {
                    return "";
                }
            } else {
                return "";
            }
        }

        function validateIBANForm() {
            $('#showIBANPageNullError').css('display', 'none');
            if ((vm.Value1LabelResourceName === "Bank_BICORSWIFT"
                && vm.Value2LabelResourceName === "Bank_IBAN") &&
                (vm.isValidBankValue1Form && vm.isValidBankValue2Form)) {
                vm.IBANButtonDisabled = false;
                if (vm.isValidBankValue1Form) {
                    $('#showVal2NullError').css('display', 'none');
                    $('#showVal2TooShortError').css('display', 'none');
                    $('#showVal2TooLongError').css('display', 'none');
                    $('#showVal2InvalidError').css('display', 'none');
                } else {
                    $('#showVal1NullError').css('display', 'none');
                    $('#showVal1TooShortError').css('display', 'none');
                    $('#showVal1TooLongError').css('display', 'none');
                    $('#showVal1InvalidError').css('display', 'none');
                }
            } else if (vm.isValidBankValue1Form && vm.isValidBankValue2Form) {
                vm.IBANButtonDisabled = false;
            }
            else {
                vm.IBANButtonDisabled = true;
            }
        }

        vm.acceptTAndC = function () {
            vm.acceptedTAndC = true;
            vm.validTermsAndConditions = true;
            validateForm();
        }
        vm.CheckIfCaptcha = function () { //change the function name
            vm.validReCaptcha = true;
            validateForm();
        }
        vm.CheckCaptcha = function () { //change the function name
            vm.validReCaptcha = false;
            validateForm();
        }
        vm.buySell = vm.model.type === 2;
        vm.buySellSelect = function () {
            vm.model.type = vm.buySell === true ? 2 : 1;
            if (vm.model.type === 2) {
                var paymentMethodName = vm.model.sellPaymentMethods.name;// vm.paymentMethod.name; 
                var paymentTypeId = paymentMethodName === "CreditCard" ? 4 : (vm.model.type === 1 ? 1 : 2);
                var filterCurrencyList = vm.model.currencies.filter(obj => {
                    return (obj.item2 & paymentTypeId) > 0;
                }).map(ele => ele.item1);
                vm.model.updatedSellCurrencies = filterCurrencyList.map((x) => x);;
                vm.model.updatedSellCurrencies.push(vm.model.forDigitalCurrency);
                switch (paymentMethodName) {
                    case 'CreditCard':
                        vm.model.forCurrency = vm.sellCCCurrencyCode;
                        break;
                    case 'Bank':
                    default:
                        vm.model.forCurrency = vm.sellBankCurrencyCode;
                        break;
                }
            }
            refreshPhoneCodes();
            //internalCurrencyChanged();
            vm.model.buyAmount = "";
            $('#showBuyAmountInvalidErrorBuy').css('display', 'none');
            $('#showBuyAmountInvalidErrorSell').css('display', 'none');
            $('#buyAmountDetail').css('display', 'none');
            BitCoinAddressAndPhoneNumberHelpPopup();
            updateOrderDetail(vm.model.buyAmount);
            //$scope.orderSizeBoundaryLimit(vm.model.mobile);
        }
        if (checkCookie("BitcoinAddress")) {
            if (vm.model.bitcoinAddress === null || vm.model.bitcoinAddress === "") {
                if (readCookie("BitcoinAddress") !== null) {
                    vm.model.bitcoinAddress = readCookie("BitcoinAddress");
                }
            }
        }
        if (checkCookie("FullName")) {
            if (vm.model.fullName === null || vm.model.fullName === "")
                vm.model.fullName = readCookie("FullName");
        }
        if (checkCookie("EMail")) {
            if (vm.model.eMail === null || vm.model.eMail === "")
                vm.model.eMail = readCookie("EMail");
        }
        if (checkCookie("PhoneCode")) {
            if (vm.model.phoneCode === null || vm.model.phoneCode === "")
                vm.model.phoneCode = readCookie("PhoneCode");
        }
        if (checkCookie("MobileNumber")) {
            if (vm.model.mobile === null || vm.model.mobile === "") {
                //vm.model.mobile = readCookie("MobileNumber");
                var mobileNumberFormat = (vm.model.mobileNumberFormat).replace(/9/g, '#');
                vm.model.mobile = readCookie("MobileNumber").replace(/ /g, '').mask(mobileNumberFormat);
            }
        }

        if (!(vm.model.buyAmount === null || vm.model.buyAmount === "")) //if buyAmount is not empty or null call validateBuyAmount function
            $scope.validateBuyAmount(vm.model.buyAmount);
        if (!(vm.model.bitcoinAddress === null || vm.model.bitcoinAddress === "")) //if bitcoinAddress is not empty or null call validateBitcoinAddress function
            $scope.validateBitcoinAddress(vm.model.bitcoinAddress, false, false);
        if (!(vm.model.fullName === null || vm.model.fullName === "")) //if fullName is not empty or null call validateFullName function
            $scope.validateFullName(vm.model.fullName);
        if (!(vm.model.eMail === null || vm.model.eMail === "")) //if eMail is not empty or null call validateEmail function
            $scope.validateEmail(vm.model.eMail);
        if (!(vm.model.mobile === null || vm.model.mobile === "")) {
            //if mobile is not empty or null call validateEmail function
            $scope.validateMobile(vm.model.mobile);
            $scope.orderSizeBoundaryLimit(vm.model.mobile);
        }

        function internalCurrencyChanged() {
            var selectedPaymentMethod = vm.model.type === 1 ? vm.paymentMethod : vm.sellPaymentMethod;
            var paymentMethodName = selectedPaymentMethod.name; //vm.model.type === 1 ? vm.model.buyPa
            $.get('/currencyChanged?currency=' + vm.model.forCurrency + '&forsellCurrency=' + vm.forSellCurrency + '&cryptoCurrency=' + vm.model.forDigitalCurrency + '&paymentMethod=' + paymentMethodName + '&type=' + vm.model.type, function (response) {
                vm.getBTC2LocalCurrency();
                vm.getBuySellBTC2LocalCurrency();
                vm.model.btc2LocalCurrency = response.latestBTCRate.data.formatedRate;
                vm.model.btc2LocalCurrencyNumeric = response.latestBTCRate.data.rate;
                vm.model.btc2SellCurrencyNumeric = response.latestSellRate.data.rate;
                vm.model.btc2LocalBuyTicker = response.buyTicker.data.formatedRate;
                vm.model.btc2LocalSellTicker = response.sellTicker.data.formatedRate;
                vm.model.btc2LocalCurrencyBuyNumeric = response.buyTicker.rate;
                vm.model.btc2LocalCurrencySellNumeric = response.sellTicker.rate;
                vm.model.euroCurrencyRate = response.euroCurrencyRate;
                vm.model.euroBtcRate = response.euroBtcRate;
                vm.model.cultureCode = response.cultureCode;
                vm.model.fxMarkUp = response.fxMarkUp;
                fetchSellBankValidation(vm.model.forCurrency);
                $scope.validateBuyAmount(vm.model.buyAmount);
                $scope.orderSizeBoundaryLimit(vm.model.mobile);
            });
        }

        vm.currencyChanged = function () { //on change of currency dropdown call paymentMethodChanged which returns OrderSizeBoundary
            $scope.resetValidationErrors();
            var paymentMethodName = vm.model.type === 1 ? vm.model.buyPaymentMethods.name : vm.model.sellPaymentMethods.name;

            if (vm.model.type === 2) {
                switch (paymentMethodName) {
                    case 'CreditCard':
                        vm.sellCCCurrencyCode = vm.model.forCurrency;
                        break;
                    case 'Bank':
                    default:
                        vm.sellBankCurrencyCode = vm.model.forCurrency;
                        break;
                }
            } else {
                switch (paymentMethodName) {
                    case 'CreditCard':
                        vm.buyCCCurrencyCode = vm.model.forCurrency;
                        break;
                    case 'Bank':
                    default:
                        vm.buyBankCurrencyCode = vm.model.forCurrency;
                        break;
                }
            }

            internalCurrencyChanged();

        }

        vm.paymentMethodChanged = function () { //on change of payment method call paymentMethodChanged which returns OrderSizeBoundary                  
            selectedPaymentMethod = vm.model.type === 1 ? vm.paymentMethod : vm.sellPaymentMethod;
            refreshPhoneCodes();
        }
        vm.KycRequirement = 'NONE';

        vm.showLoadingIndicator = function (show = false) {
            if (show === false) {
                $("#loader").hide();
                $('#orderForm').removeClass("disable");
                $('#id-box').removeClass("disable");

            }
            else {
                $("#loader").show();
                $('#orderForm').addClass("disable");
                $('#id-box').addClass("disable");
            }

        }
        //vm.recaptchaToken = "";
        var recaptchaKey = vm.model.reCaptchaPublicKey;
        recaptchaKey.replace("", '');
        vm.googlecaptchatoken = function () {
            grecaptcha.ready(function () {
                grecaptcha.execute(recaptchaKey, { action: 'orderform' }).then(function (token) {
                    document.getElementById("recaptcha").value = token;
                });
            });
        }

        vm.submitOrder = function (respondWithView) { //on click of Place Order button call verification method
            vm.showLoadingIndicator(true);
            if (vm.model.type === 1) {
                createCookie('BitcoinAddress', vm.model.bitcoinAddress);
            }
            else {
                vm.model.bitcoinAddress = "";
            }
            createCookie('FullName', vm.model.fullName);
            createCookie('EMail', vm.model.eMail);
            createCookie('MobileNumber', vm.model.mobile);
            createCookie('ForDigitalCurrency', vm.model.forDigitalCurrency);
            createCookie('PhoneCode', vm.model.phoneCode);
            var userLang = navigator.language.substr(0, 2);
            var language = (userLang === 'en') ? ' ' : userLang
            var recaptchaToken = recaptcha.defaultValue;
            var paymentMethodName = selectedPaymentMethod.name;
            // , dateOfBirth: vm.model.dateOfBirth
            $http.post("/Verification", {
                buyAmount: vm.model.buyAmount, purchaseCurrency: vm.model.forCurrency, forSellCurrency: vm.forSellCurrency, cryptoCurrency: vm.model.forDigitalCurrency, paymentMethod: paymentMethodName,
                btcAddress: vm.model.bitcoinAddress, fullName: vm.model.fullName, email: vm.model.eMail, phoneCodeId: vm.model.phoneCodeId, phoneNumber: vm.model.mobile,
                bccAddress: vm.model.bccAddress, partnerId: vm.model.partnerId, respondWithView: respondWithView, type: vm.model.type, recieveNewsletters: vm.recieveNewsletters, couponCode: vm.model.couponCode, reCaptcha: recaptchaToken,
                Reg: vm.model.reg, IBAN: vm.model.iban, SwiftCode: vm.model.swiftCode, AccountNumber: vm.model.accountNumber, lang: language
            }).success(function (response, status) {
                vm.showLoadingIndicator(false);
                if (respondWithView) {
                    vm.orderInfoViewEnabled = false;
                    $('#tabContent').html($compile(response)($scope));
                    if (vm.model.type === 1) {
                        vm.verifyBitcoinViewEnabled = true;
                        updateDataLayerAddressVerification()//GA: Function call to update data layer on Phone verification
                    }
                    else {
                        enableVerifyPhoneNumberView();
                    }
                    $("#orderInfoProgressBarItem").removeClass("active");
                    $("#verificationProgressBarItem").addClass("active");
                    $("#paymentProgressBarItem").removeClass("active");
                    $("#receiptProgressBarItem").removeClass("active");
                    $window.scroll(0, 0);
                }
                else {
                    vm.KycRequirement = response.kycRequirement;
                    if (vm.model.type === 1) {
                        vm.confirmBitcoinAddress();
                        updateDataLayerAddressVerification()//GA: Function call to update data layer on Phone verification
                    }
                    else
                        enableVerifyPhoneNumberView();
                }

            }).error(function (data, status) {
                vm.model.recaptcha = '';
                vm.googlecaptchatoken();
                vm.showLoadingIndicator(false);
                if (data.errors[0].item1 === "OrderAmountOutOfRange") { //error message for Order Amount Out Of Range
                    vm.createToast(data.errors[0].item2, "danger");
                    dataLayer.push({
                        'ErrorMessage': 'Order amount is not within range.',
                        'event': 'error'
                    });

                }
                else if (data.errors[0].item1 === "BitcoinAddressInvalid") { //error message for Bitcoin Address Invalid    
                    vm.createToast(data.errors[0].item2, "danger");
                    dataLayer.push({
                        'ErrorMessage': 'Invalid bitcoin address.',
                        'event': 'error'
                    });
                }
                else if (data.errors[0].item1 === "FullNameEmptyMessage") { //error message for Full Name Empty
                    vm.createToast(data.errors[0].item2, "danger");
                    dataLayer.push({
                        'ErrorMessage': 'Full name empty.',
                        'event': 'error'
                    });
                }

                else if (data.errors[0].item1 === "DayTransactionLimitExceeded") {
                    vm.createToast(data.errors[0].item2, "danger");
                    dataLayer.push({
                        'ErrorMessage': 'Number of transactions per day has exceeded.',
                        'event': 'error'
                    });
                }

                else if (data.errors[0].item1 === "PerTransactionAmountLimitExceeded") {
                    vm.createToast(data.errors[0].item2, "danger");
                    dataLayer.push({
                        'ErrorMessage': 'Transaction limit per order exceeded.',
                        'event': 'error'
                    });
                }

                else if (data.errors[0].item1 === "DayTransactionAmountLimitExceeded") {
                    vm.createToast(data.errors[0].item2, "danger");
                    dataLayer.push({
                        'ErrorMessage': 'Transaction limit per day has been exceeded.',
                        'event': 'error'
                    });
                }

                else if (data.errors[0].item1 === "MonthTransactionAmountLimitExceeded") {
                    vm.createToast(data.errors[0].item2, "danger");
                    dataLayer.push({
                        'ErrorMessage': 'Transaction limit per month has been exceeded.',
                        'event': 'error'
                    });
                }
                else if (data.errors[0].item1 === "FirstNameSurNameTooLongMessage") { //error message for Full Name Too Long
                    vm.createToast(data.errors[0].item2, "danger");
                    dataLayer.push({
                        'ErrorMessage': 'Name is too long.',
                        'event': 'error'
                    });
                }
                else if (data.errors[0].item1 === "EmailEmptyMessage") { //error message for empty email address
                    vm.createToast(data.errors[0].item2, "danger");
                    dataLayer.push({
                        'ErrorMessage': 'Email address is empty.',
                        'event': 'error'
                    });
                }
                else if (data.errors[0].item1 === "EmailInvalidMessage") { //error message for invalid email address
                    vm.createToast(data.errors[0].item2, "danger");
                    dataLayer.push({
                        'ErrorMessage': 'Email address is invalid..',
                        'event': 'error'
                    });
                }
                else if (data.errors[0].item1 === "EmailTooLongMessage") { //error message for email address when it is too long
                    vm.createToast(data.errors[0].item2, "danger");
                    dataLayer.push({
                        'ErrorMessage': 'Email address is too long.',
                        'event': 'error'
                    });
                }
                else if (data.errors[0].item1 === "GoogleRecaptchaExpried") { //error message for google recaptcha failure
                    vm.createToast(data.errors[0].item2, "danger");
                    dataLayer.push({
                        'ErrorMessage': 'EGoogle Recaptcha Expried.',
                        'event': 'error'
                    });
                }
                else if (data.errors[0].item1 === "CurrencyNotFound") { //error message for currency not found
                    vm.createToast(data.errors[0].item2, "danger");
                    dataLayer.push({
                        'ErrorMessage': 'Currency selected is not found',
                        'event': 'error'
                    });
                }
                else if (data.errors[0].item1 === "OrderAmountOnly999DKK") { //error message for Order Amount Out Of Range
                    vm.createToast(data.errors[0].item2, "danger");
                    dataLayer.push({
                        'ErrorMessage': 'Order amount exceed 999DKK.',
                        'event': 'error'
                    });
                }
                else if (data.errors[0].item1 === "MobileEmptyMessage") {
                    vm.createToast(data.errors[0].item2, "danger");
                    dataLayer.push({
                        'ErrorMessage': 'Mobile number is empty.',
                        'event': 'error'
                    });
                }
                else if (data.errors[0].item1 === "MobileInvalidMessage") {
                    vm.createToast(data.errors[0].item2, "danger");
                    dataLayer.push({
                        'ErrorMessage': 'Mobile number is invalid.',
                        'event': 'error'
                    });
                }
                else {
                    $http({
                        url: "/GetMessage",
                        method: "GET",
                        params: {
                            key: "UnableToProcessOrder",
                            language: (userLang === 'en') ? ' ' : userLang
                        }
                    }).success(function (response) {
                        vm.createToast(response, "danger");
                    })
                    dataLayer.push({
                        'ErrorMessage': 'Error in processing order.',
                        'event': 'error'
                    });
                }
            });
        };
        vm.showReceipt = function () {
            $("#orderInfoProgressBarItem").removeClass("active");
            $("#verificationProgressBarItem").removeClass("active");
            $("#paymentProgressBarItem").removeClass("active");
            $("#receiptProgressBarItem").addClass("active");
            // Called from QR code screen
            $http.post("/Receipt", { receiptModel: null }).success(function (response, status) {
                $('#tabContent').html($compile(response)($scope));
            })
            dataLayer.push({
                'virtualPage': '/' + userLang + '/funnel/ReceiptSell',
                'event': 'virtualPageview'
            });
        };

        vm.SaveIBAN = function () {
            vm.showLoadingIndicator(true);
            var postData = {
                Language: userLang,
                SwiftCode: '',
                IBAN: '',
                Reg: '',
                Account: ''
            };

            switch (vm.Value1LabelResourceName) {
                case "Bank_BICORSWIFT":
                    postData.SwiftCode = vm.Value1;
                    break;
                case "Bank_DKK_RegNumber":
                    postData.Reg = vm.Value1;
                    break;
                case "Bank_UKSortCode":
                    postData.Reg = vm.Value1;
                    break;
                case "Bank_BSBCode":
                default:
                    postData.Reg = vm.Value1;
                    break;
            }


            switch (vm.Value2LabelResourceName) {
                case "Bank_IBAN":
                    postData.IBAN = vm.Value2;
                    break;
                case "Bank_AccountNumber":
                default:
                    postData.Account = vm.Value2;
                    break;
            }
            $http.post("/SaveIBAN", postData).success(function () {
                $('#IbanInfo').hide('slow');
                $('#QRLoading').hide();
                $("#CrypPmntInfo").removeClass("disable");
                vm.model.CryptoCurrencyPaymentInfoPageLoaded = true;
                vm.model.ViewName = 'QRPage'
                updateDataLayerQRCode()//GA: Function call to update the datalayer when QRCode page is loaded
            })
                .error(function (data, status) {
                    vm.createToast(data.errors[0].item2, "danger");
                })
        };

        $scope.initPaymentFrame = function () {
            debugger;
            $('#paymentFrameLoading').hide();
        };

        $scope.CryptoCurrencyPaymentInfo = function () { ///refac check if change in this is needed
            $http.post("/CryptoCurrencyPaymentInfo").success(function (response, status) {
                $('#tabContent').html($compile(response)($scope));
                if (vm.model.ViewName === 'QRPage') {
                    vm.model.CryptoCurrencyPaymentInfoPageLoaded = true;
                }
                if (vm.model.ViewName === 'Receipt') {
                    vm.model.CryptoCurrencyPaymentInfoPageLoaded = false;
                    $("#paymentInstructionsProgressBarItem").removeClass("active");
                    $("#receiptSellProgressBarItem").addClass("active");
                    updateDataLayerReceiptSell(vm.model.orderNumber, vm.model.buyAmount, vm.buyBankCurrencyCode, vm.model.Country, vm.model.couponCode, vm.model.type, vm.model.paymentMethod.name)
                }
            }).error(function (data, status) {
            })
        };

        vm.backToOrder = function (onSkipOrderForm) { //on click of back to order button call OrderInfo action method
            $.get('/OrderInfo', function (response) {
                $('#tabContent').html($compile(response)($scope));
                if (onSkipOrderForm)
                    vm.model.skipOrderForm = false;

                vm.orderInfoViewEnabled = true;
                vm.verifyBitcoinViewEnabled = false;
                vm.validReCaptcha = vm.model.reCaptchaStatus !== "Enabled";
                vm.googlecaptchatoken();
                validateForm();
                $("#orderInfoProgressBarItem").addClass("active");
                $("#verificationProgressBarItem").removeClass("active");
                $("#paymentProgressBarItem").removeClass("active");
                $("#paymentInstructionsProgressBarItem").removeClass("active");
                $("#receiptProgressBarItem").removeClass("active");
                BitCoinAddressAndPhoneNumberHelpPopup();
            });
        };
        vm.confirmBitcoinAddress = function () { ///refac change in funciton name is needed
            if (vm.model.type === 1) {
                $("#address-box").addClass("disable");
                $("#loader").show();
            }
            $http.post("/SendVerificationCode", { lang: userLang })
                .success(function (response, status) {
                    //if (response.kycRequirement === "NONE") {
                    //    vm.submitIdentityVerification();
                    //}
                    //else {
                    updateDataLayerPhoneVerification() //GA:Function call to update data layer on Phone verification   
                    if (vm.model.type === 1) {
                        $("#address-box").removeClass("disable");
                        $("#loader").hide();
                        enableVerifyPhoneNumberView();
                    }
                    //}
                }).error(function (data, status) {
                    if (status === 429) {
                        $("#address-box").removeClass("disable");
                        $("#loader").hide();
                        vm.createToast(data.errors[0].item2, "warning");
                    }
                    dataLayer.push({
                        'ErrorMessage': 'Error in sending verification code.',
                        'event': 'error'
                    });
                });
        };
        function enableVerifyPhoneNumberView() {
            if (vm.model.type === 2) {
                vm.confirmBitcoinAddress(); ///refac move this condition and function out
            }
            vm.verifyBitcoinViewEnabled = false;
            vm.verifyPhoneNumberViewEnabled = true;
            $("#address-box").addClass("disable");
            $("#address-box").hide('slow');
            $("#confrm-box").removeClass("disable");
            $("#txt-verifypasscode").focus();
        }
        vm.verifyPhoneNumber = function () { //call function to verify phone number
            $("#confrm-box").addClass("disable");
            $("#loader").show();
            $http.post("/VerifyPhoneNumber", { verificationCode: vm.verificationCode })
                .success(function (response, status) {
                    var successMessage = "PhoneNumberVerifiedSuccessfully";
                    if (response.kycRequirement === "NONE") {
                        vm.submitIdentityVerification(false);
                        $http({
                            url: "/GetMessage",
                            method: "GET",
                            params: {
                                key: successMessage,
                                language: (userLang === 'en') ? ' ' : userLang
                            }
                        }).success(function (response) {
                            vm.createToast(response, "info");
                        })
                    }
                    else { //If kycRequirement is not NONE then enable Verify Identity View and show successfull verification method
                        $("#confrm-box").removeClass("disable");
                        $("#loader").hide();
                        enableVerifyIdentityView();
                        $http({
                            url: "/GetMessage",
                            method: "GET",
                            params: {
                                key: successMessage,
                                language: (userLang === 'en') ? ' ' : userLang
                            }
                        }).success(function (response) {
                            vm.createToast(response, "info");
                        })
                        updateDataLayerKYCPhotoIDVerification()//GA: Function call to update datalayer on KYC Photo ID verification
                        if (response.kycRequirement === "PhotoID&SelfieID") {
                            updateDataLayerKYCSelfieIDVerification(); //GA: Function call to update datalayer on KYC Selfie ID verification
                        }
                    }
                    vm.verificationCode = '';
                }).error(function (data, status) {
                    $("#confrm-box").removeClass("disable");
                    $("#loader").hide();
                    vm.verificationCode = '';
                    $http({
                        url: "/GetMessage",
                        method: "GET",
                        params: {
                            key: "VerificationCodeIncorrect",
                            language: (userLang === 'en') ? ' ' : userLang
                        }
                    }).success(function (response) {
                        vm.createToast(response, "danger");
                    })
                    dataLayer.push({
                        'ErrorMessage': 'Verification code is incorrect.',
                        'event': 'error'
                    });
                });
        };
        //TODO VALIDATE THIS FUNCITON
        vm.KYCRequirementCUMPaymentOption = function (withKYC) {
            vm.submitIdentityVerification(false);
        }
        function enableVerifyIdentityView() {
            vm.verifyIdentityViewEnabled = true;
            vm.verifyPhoneNumberViewEnabled = false;
            $("#confrm-box").addClass("disable");
            $("#id-box").removeClass("disable");
            $("#confrm-box").hide('slow');
            //document.getElementById('id-scan-button').click();
        }
        vm.backtoBitcoinAddressConfirmation = function () { //function to call back to Bitcoin Address Confirmation
            $http.post("/BitcoinAddressConfirmationView", { lang: userLang })
                .success(function (response, status) {
                    vm.verifyBitcoinViewEnabled = true;
                    vm.verifyPhoneNumberViewEnabled = false;
                    $("#address-box").show('slow');
                    $("#address-box").removeClass("disable");
                    $("#confrm-box").addClass("disable");
                }).error(function (data, status) {
                    vm.createToast(data.errors[0].item2, "danger");
                    dataLayer.push({
                        'ErrorMessage': 'Error in Wallet address confirmation view.',
                        'event': 'error'
                    });
                });
        };
        vm.resendVerificationCode = function () { //on click of resend, call Send Verification Code acttion method
            $("#confrm-box").addClass("disable");
            $("#loader").show();
            $http.post("/SendVerificationCode", { lang: userLang })
                .success(function (response, status) {
                    vm.verificationCode = '';
                    $("#confrm-box").removeClass("disable");
                    $("#loader").hide();
                    $http({
                        url: "/GetMessage",
                        method: "GET",
                        params: {
                            key: "VerificationCodeSent",
                            language: (userLang === 'en') ? ' ' : userLang
                        }
                    }).success(function (response) {
                        vm.createToast(response, "info");
                    })
                }).error(function (data, status) {
                    if (status === 429) {
                        $("#confrm-box").removeClass("disable");
                        $("#loader").hide();
                        vm.createToast(data.errors[0].item2, "warning");
                    }
                    dataLayer.push({
                        'ErrorMessage': 'Error in sending verification code.',
                        'event': 'error'
                    });
                });
        };
        vm.backtoPhoneNumberVerification = function () { // on click of Back button disable addres box and enable confirm box
            vm.verificationCode = '';
            vm.verifyPhoneNumberViewEnabled = true;
            vm.verifyIdentityViewEnabled = false;
            $("#loader").hide();
            $("#confrm-box").show('slow');
            $("#address-box").addClass("disable");
            $("#confrm-box").removeClass("disable");
            $("#id-box").addClass("disable");

            //$("#address-box").removeClass("disable");
            //$("#address-box").show('slow');
            //$("#id-box").addClass("disable");
        };

        vm.UpdateMinersFee = function () {
            $http.post("/GetMinersFee", {
                cryptoCurrency: vm.model.forDigitalCurrency
            }).success(function (data, status) {
                vm.model.minersFee = vm.minersFee = data.minersFee;
            });
        };

        vm.cryptoCurrencyChanged = function () {
            vm.model.updatedSellCurrencies.pop();
            vm.model.updatedSellCurrencies.push(vm.model.forDigitalCurrency);
            vm.forSellCurrency = vm.model.forDigitalCurrency;
            vm.getBTC2LocalCurrency();
            vm.getBuySellBTC2LocalCurrency();
            vm.model.bitcoinAddress = "";
            vm.model.buyAmount = "";
            vm.currencyChanged();
            vm.UpdateMinersFee();
            updateOrderDetail(vm.model.buyAmount)
        };

        window.addEventListener("submitKYC", function (evt) {
            vm.submitIdentityVerification(true);
        }, false);

        vm.submitIdentityVerification = function (withKYC) { //on click of Submit button call SubmitIdentityVerification action method
            vm.showLoadingIndicator(true);
            $http.post("/SubmitIdentityVerification", { withKYC: withKYC, lang: userLang })
                .success(function (verificationResponse, status) {
                    vm.showLoadingIndicator(false);
                    $('#tabContent').html($compile(verificationResponse)($scope));

                    $("#orderInfoProgressBarItem").removeClass("active");
                    $("#verificationProgressBarItem").removeClass("active");
                    var paymentMethodName = vm.model.type === 1 ? vm.model.buyPaymentMethods.name : vm.model.sellPaymentMethods.name;
                    if (paymentMethodName === "CreditCard") {
                        $("#paymentProgressBarItem").addClass("active");
                    }
                    else {
                        $("#paymentInstructionsProgressBarItem").addClass("active");
                        $("#receiptSellProgressBarItem").removeClass("active");
                    }
                    $("#receiptProgressBarItem").removeClass("active");
                }).error(function (data, status) {
                    vm.createToast(data.errors[0].item2, "danger");
                    dataLayer.push({
                        'ErrorMessage': data.errorMessage,
                        'event': 'error'
                    });
                });
        };
        vm.phoneCodeChanged = function () { //On changing phone number code get mobile number format            
            vm.model.phoneCode = vm.model.phoneCodes.filter(function (item) { return item.item1 === vm.model.phoneCodeId; })[0].item3;
            $.get('/getPhoneNumberStyle?phonecode=' + vm.model.phoneCode, function (response) {
                vm.model.mobile = '';
                vm.model.mobileNumberFormat = response.phoneNumberStyle;
                vm.cardFee = response.cardFee;
                $("#txtMobileNumber").inputmask({ mask: vm.model.mobileNumberFormat, greedy: false });
                //$scope.validateMobile(vm.model.mobile);
                updateOrderDetail(vm.model.buyAmount);
                //$scope.$apply();
            });
        }

        vm.getBTC2LocalCurrency = function () { //get btc rate for selected currency
            $.get('/LatestBTCRate2?currency=' + vm.model.forCurrency + '&cryptoCurrency=' + vm.model.forDigitalCurrency, function (response) {
                vm.model.btc2LocalCurrency = response.formatedRate;
                vm.model.btc2LocalCurrencyNumeric = response.rate;
            });
        };
        vm.getBuySellBTC2LocalCurrency = function () { //get btc rate for selected currency
            $.get('/LatestBTCBuySellRate2?currency=' + vm.model.forCurrency + '&type=1' + '&cryptoCurrency=' + vm.model.forDigitalCurrency, function (response) {
                vm.model.btc2LocalBuyTicker = response.formatedRate;
                vm.model.btc2LocalCurrencyBuyNumeric = response.rate;
            });
            $.get('/LatestBTCBuySellRate2?currency=' + vm.model.forCurrency + '&type=2' + '&cryptoCurrency=' + vm.model.forDigitalCurrency, function (response) {
                vm.model.btc2LocalSellTicker = response.formatedRate;
                vm.model.btc2LocalCurrencySellNumeric = response.rate;
            });
        };

        vm.checkBTCTransaction = function () { //on click of Submit button call  action method
            if (vm.model.CryptoCurrencyPaymentInfoPageLoaded) { ///remove this line
                $http.post("/CheckIfPaid")
                    .success(function (response, status) {
                        if (response === "True") {
                            vm.model.CryptoCurrencyPaymentInfoPageLoaded = false;
                            $scope.CryptoCurrencyPaymentInfo();
                        }
                    });
            }
        }
        vm.interval = function () {
            if (vm.model.CryptoCurrencyPaymentInfoPageLoaded) vm.checkBTCTransaction();
            if (vm.model.receiptModel) {
                receiptModelCurrencyRates(vm.model.receiptModel);
            } else {
                vm.getBTC2LocalCurrency();
                vm.getBuySellBTC2LocalCurrency();
            }
        }

        $interval(vm.interval, 10000);

        //Google Analytics: Function to update datalayer on address verification 
        function updateDataLayerAddressVerification() {
            dataLayer.push({
                'currency': vm.model.forCurrency,
                'paymentMethod': vm.model.type == 1 ? vm.model.buyPaymentMethods.name : vm.model.sellPaymentMethods.name,
                'country': vm.model.phoneCode,
                'virtualPage': '/' + userLang + '/funnel/verification-address',
                'event': 'virtualPageview'
            });
        }

        //Google Analytics: Function to update datalayer on phone number verification
        function updateDataLayerPhoneVerification() {
            dataLayer.push({
                'virtualPage': '/' + userLang + '/funnel/verification-number',
                'event': 'virtualPageview'
            });
        }

        //Google Analytics: Function to update datalayer on KYC Photo ID verification
        function updateDataLayerKYCPhotoIDVerification() {
            dataLayer.push({
                'virtualPage': '/' + userLang + '/funnel/KYCPhotoId',
                'event': 'virtualPageview'
            });
        }

        //Google Analytics: Function to update datalayer on KYC Selfie ID Verification
        function updateDataLayerKYCSelfieIDVerification() {
            dataLayer.push({
                'virtualPage': '/' + userLang + '/funnel/KYCSelfieId',
                'event': 'virtualPageview'
            });
        }

        //Google Analytics: Function to update datalayer on loading QRCode page
        function updateDataLayerQRCode() {
            dataLayer.push({
                'virtualPage': '/' + userLang + '/funnel/QRCode',
                'event': 'virtualPageview'
            });
        }

        //Google Analytics: Function to update datalayer on loading Receipt sell page
        function updateDataLayerReceiptSell(orderNumber, amount, currencyCode, country, coupon, type) {
            dataLayer.push({
                'virtualPage': '/' + userLang + '/funnel/sellReceipt',
                'event': 'transactionComplete',
                'ecommerce': {
                    'sell': {
                        'actionField': {
                            'Id': orderNumber,
                            'Amount': amount,
                            'Currency': currencyCode,
                            'Country': country,
                            'Coupon': coupon,
                            'Products': [{
                                'name': 'BTC',
                                'amount': amount,
                                'type': type
                            }]
                        }
                    }
                }
            });
            //(function (w, d, s, l, i) { w[l] = w[l] || []; w[l].push({ 'gtm.start': new Date().getTime(), event: 'gtm.js' }); var f = d.getElementsByTagName(s)[0], j = d.createElement(s), dl = l != 'dataLayer' ? '&l=' + l : ''; j.async = true; j.src = 'https://www.googletagmanager.com/gtm.js?id=' + i + dl; f.parentNode.insertBefore(j, f); })(window, document, 'script', 'dataLayer', googleTagManagerId);
        }

        //GA: Function to update datalayer when receipt for credit card transaction is loaded
        function updateDataLayerReceiptCreditCard(orderNumber, amount, currencyCode, country, coupon, type, paymentMethod, quantity) {
            dataLayer.push({
                'virtualPage': '/' + userLang + '/funnel/receipt',
                'event': 'transactionComplete',
                'ecommerce': {
                    'purchase': {
                        'actionField': {
                            'Id': orderNumber,
                            'Amount': amount,
                            'Currency': currencyCode,
                            'Country': country,
                            'Coupon': coupon,
                            'products': [{
                                'name': 'BTC',
                                'amount': quantity,
                                'type': type,
                                'payment method': paymentMethod
                            }]
                        }
                    }
                }
            });
        }
    }

    String.prototype.mask = function (m) {
        var m, l = (m = m.split("")).length, s = this.split(""), j = 0, h = "";
        for (var i = -1; ++i < l;)
            if (m[i] !== "#") {
                if (m[i] === "\\" && (h += m[++i])) continue;
                h += m[i];
                i + 1 === l && (s[j - 1] += h, h = "");
            }
            else {
                if (!s[j] && !(h = "")) break;
                (s[j] = h + s[j++]) && (h = "");
            }
        return s.join("") + h;
    };
})();