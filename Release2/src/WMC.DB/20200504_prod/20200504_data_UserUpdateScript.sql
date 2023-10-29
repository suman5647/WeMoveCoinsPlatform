-- get app settings from server
DECLARE @TransactionLimitsDetails as nvarchar(max);
DECLARE @PaymentMethodDetails as nvarchar(max);
DECLARE @SellPaymentMethodDetails as nvarchar(max);
SET @TransactionLimitsDetails = N'{"DayTransactionLimit":4,"PerTransactionAmountLimit":500,"DayTransactionAmountLimit":500,"MonthTransactionAmountLimit":1000}';
-- N'{"DayTransactionLimit":4,"PerTransactionAmountLimit":3400,"DayTransactionAmountLimit":3400,"MonthTransactionAmountLimit":7500}'
SET @SellPaymentMethodDetails = N'[{"SiteId":0,"Spread":2,"Methods":[{"Name":"Bank","Fee":0,"Commission":0,"FixedFee":0,"OrderSizeBoundary":{"Min":13.36,"Max":3342.25}}]}]';
--N'[{"SiteId":0,"Spread":1,"Methods":[{"Name":"Bank","Fee":0,"Commission":2.9,"FixedFee":0,"OrderSizeBoundary":{"Min":13.36,"Max":3342.25}}]}]';
SET @PaymentMethodDetails  = N'[
{"SiteId":1,"Spread":4,"Methods":[{"Name":"Bank","Fee":0,"Commission":0,"OrderSizeBoundary":{"Min":13.36,"Max":3342.25}}, {"Name":"CreditCard","Fee":0,"Commission":9.9,"OrderSizeBoundary":{"Min":40,"Max":500}}]},
{"SiteId":2,"Spread":4,"Methods":[{"Name":"Bank","Fee":0,"Commission":0,"OrderSizeBoundary":{"Min":13.36,"Max":3342.25}}, {"Name":"CreditCard","Fee":0,"Commission":9.9,"OrderSizeBoundary":{"Min":40,"Max":500}}]},
{"SiteId":3,"Spread":4,"Methods":[{"Name":"Bank","Fee":0,"Commission":0,"OrderSizeBoundary":{"Min":13.36,"Max":3342.25}}]},
{"SiteId":4,"Spread":4,"Methods":[{"Name":"Bank","Fee":0,"Commission":0,"OrderSizeBoundary":{"Min":13.36,"Max":3342.25}}, {"Name":"CreditCard","Fee":0,"Commission":9.9,"OrderSizeBoundary":{"Min":40,"Max":500}}]}]';
--N'[
--{"SiteId":1,"Spread":1,"Methods":[{"Name":"Bank","Fee":0,"Commission":2.9,"OrderSizeBoundary":{"Min":13.36,"Max":3342.25}}]},
--{"SiteId":2,"Spread":1,"Methods":[{"Name":"Bank","Fee":0,"Commission":2.9,"OrderSizeBoundary":{"Min":13.36,"Max":3342.25}}]},
--{"SiteId":3,"Spread":1,"Methods":[{"Name":"Bank","Fee":0,"Commission":2.9,"OrderSizeBoundary":{"Min":13.36,"Max":3342.25}}]},
--{"SiteId":4,"Spread":1,"Methods":[{"Name":"Bank","Fee":0,"Commission":2.9,"OrderSizeBoundary":{"Min":13.36,"Max":3342.25}}]}]';

update [User]
Set TransactionLimitsDetails = @TransactionLimitsDetails
WHERE TransactionLimitsDetails IS NULL;

update [User]
Set SellPaymentMethodDetails = @SellPaymentMethodDetails
WHERE SellPaymentMethodDetails IS NULL;

update [User]
Set PaymentMethodDetails =  @PaymentMethodDetails
WHERE PaymentMethodDetails IS NULL;

