using NBitcoin;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BitGoSharp
{
    class TransactionBuilder
    {
        Wallet walletInstance;
        BitGoClient bitGoClientInstance;
        public TransactionBuilder(Wallet wallet, BitGoClient bitGoClient)
        {
            walletInstance = wallet;
            bitGoClientInstance = bitGoClient;
        }

        internal dynamic CreateTransaction(dynamic param)
        {
            var minConfirms = param.minConfirms ?? 0;
            var validate = param.validate ?? true;
            var recipients = param.recipients;
            var fee = (decimal?)null;
            var extraChangeAmounts = new List<dynamic>();

            // Flag indicating whether this class will compute the fee
            var shouldComputeBestFee = true;

            dynamic travelInfos = null;

            // Sanity check the arguments passed in
            //...

            dynamic feeSingleKeySourceAddress = null;
            var feeSingleKeyInputAmount = 0;
            dynamic feeParamsDefined = null;

            var totalOutputAmount = 0;
            foreach (var recipient in recipients)
            {
                totalOutputAmount += recipient.Value;
            }
            //var bitgoFeeInfo = param.bitgoFee;
            var totalAmount = totalOutputAmount; // + (fee || 0);
                                                 // The sum of the input values for this transaction.

            var inputAmount = 0;
            var feeSingleKeyUnspentsUsed = new List<dynamic>();
            var changeOutputs = new List<dynamic>();

            //The transaction.
            var transaction = new Transaction();
            //transaction.AddInput(new TxIn(new OutPoint(uint256.Parse(""), 3)));
            dynamic instant = null;// param.instant
            dynamic bitgoFeeInfo = null;
            GetBitGoFee(totalOutputAmount, instant, totalAmount, bitgoFeeInfo);
            GetBitGoFeeAddress(bitgoFeeInfo);
            dynamic feeTxConfirmTarget = null;
            dynamic maxFeeRate = BitGoClient.Constants.maxFeeRate;
            var feeRate = GetDynamicFeeEstimate(new
            {
                feeTxConfirmTarget,
                maxFeeRate
            });
            dynamic minUnspentSize = null;
            bool forceChangeAtEnd = true;
            dynamic targetWalletUnspents = null;
            var enforceMinConfirmsForChange = true;
            dynamic getUnspentsResult = GetUnspents(new
            {
                totalAmount,
                minUnspentSize,
                instant,
                targetWalletUnspents,
                minConfirms,
                enforceMinConfirmsForChange
            });

            var feeSingleKeyUnspents = GetUnspentsForSingleKey(new
            {
                feeSingleKeySourceAddress,
                instant,
                totalAmount
            });

            // Approximate the fee based on number of inputs & outputs
            var estimatedSize = 0M;
            CollectInputs(new
            {
                getUnspentsResult.unspents,
                shouldComputeBestFee,
                feeRate,
                recipients,
                bitgoFeeInfo,
                extraChangeAmounts,
                totalOutputAmount,
                feeSingleKeySourceAddress,
                feeSingleKeyUnspents
            }, transaction, ref inputAmount, ref totalAmount, ref fee, ref feeSingleKeyInputAmount, ref feeSingleKeyUnspentsUsed, ref estimatedSize);
            CollectOutputs(new
            {
                transaction,
                recipients,
                bitgoFeeInfo,
                extraChangeAmounts,
                forceChangeAtEnd,
                feeSingleKeySourceAddress
            }, inputAmount, totalAmount, fee, ref changeOutputs);
            var serialized = Serialize(new
            {
                getUnspentsResult.unspents,
                transaction,
                fee,
                feeSingleKeyUnspentsUsed,
                changeOutputs,
                feeRate,
                bitgoFeeInfo,
                estimatedSize,
                instant,
                travelInfos
            });
            dynamic feeSingleKeyWIF = null;
            return new
            {
                serialized.transactionHex,
                serialized.unspents,
                serialized.fee,
                serialized.changeAddresses,
                serialized.walletId,
                serialized.walletKeychains,
                serialized.feeRate,
                feeSingleKeyWIF,
                serialized.instant,
                serialized.bitgoFee,
                serialized.estimatedSize,
                serialized.travelInfos,
            };
        }

        internal void GetBitGoFee(decimal amount, dynamic instant, int totalAmount, dynamic bitgoFeeInfo)
        {
            if (bitgoFeeInfo == null)
            {
                var response = walletInstance.GetBitGoFee(amount, instant);
                dynamic result = SimpleJson.DeserializeObject(response.Content);
                if (result != null && result.fee > 0)
                    bitgoFeeInfo = new { amount = result.fee };
            }
            if (bitgoFeeInfo != null && bitgoFeeInfo.amount > 0)
                totalAmount += bitgoFeeInfo.amount;
        }

        internal void GetBitGoFeeAddress(dynamic bitgoFeeInfo)
        {
            // If we don't have bitgoFeeInfo, or address is already set, don't get a new one
            if (!(bitgoFeeInfo == null || bitgoFeeInfo.address != null))
            {
                var response = bitGoClientInstance.GetBitGoFeeAddress();
                dynamic result = SimpleJson.DeserializeObject(response.Content);
                bitgoFeeInfo.address = result.address;
            }
        }

        internal dynamic GetDynamicFeeEstimate(dynamic param)
        {
            //feeTxConfirmTarget = undifined
            //maxFeeRate = 
            //if (params.feeTxConfirmTarget || !feeParamsDefined) {
            var response = bitGoClientInstance.EstimateFee(param.feeTxConfirmTarget, param.maxFeeRate.ToString(), "12");
            dynamic result = SimpleJson.DeserializeObject(response.Content);
            var estimatedFeeRate = result.feePerKb;
            if (estimatedFeeRate < BitGoClient.Constants.minFeeRate) //incorrect constant value minFeeRate
                return BitGoClient.Constants.minFeeRate;
            else if (estimatedFeeRate > param.maxFeeRate)
                return param.maxFeeRate;
            else
                return estimatedFeeRate;
            //}
        }

        internal dynamic GetUnspents(dynamic param)
        {
            var target = (decimal)(param.totalAmount + 0.01e8);
            var minSize = 0; //param.minUnspentSize ??
            var response = walletInstance.UnspentsPaged(param.instant, minSize, target, param.targetWalletUnspents);
            dynamic result = SimpleJson.DeserializeObject(response.Content);
            var unspents = new List<dynamic>(result.unspents).Where(unspent =>
            {
                var confirms = unspent.confirmations ?? 0;
                if (!param.enforceMinConfirmsForChange && unspent.isChange)
                    return true;
                return confirms >= param.minConfirms;
            }).ToList();
            //foreach (var unspent in result.unspents)
            //{
            //    var confirms = unspent.confirmations ?? 0;
            //    if (!param.enforceMinConfirmsForChange && unspent.isChange)
            //        return true;
            //}
            //result.unspents[0].confirmations
            //param.unspend = result.unspents[0];
            // For backwards compatibility, respect the old splitChangeSize=0 parameter
            //if (!param.noSplitChange && param.splitChangeSize !== 0) {
            //    param.extraChangeAmounts = result.extraChangeAmounts || [];
            //}
            return new { unspents = unspents, result.extraChangeAmounts };
        }

        internal dynamic GetUnspentsForSingleKey(dynamic param)
        {
            if (param.feeSingleKeySourceAddress != null)
            {
                var feeTarget = 0.01e8;
                if (param.instant != null)
                    feeTarget += param.totalAmount * 0.001;
                var response = bitGoClientInstance.GetUnspentsForSingleKey(param.feeSingleKeySourceAddress, param.feeTarget);
                dynamic result = SimpleJson.DeserializeObject(response.Content);
                //if (response.body.total <= 0)
                //    throw new Exception("No unspents available in single key fee source");
                return new List<dynamic>(result.unspents);
            }
            return null;
        }

        internal dynamic CollectInputs(dynamic param, Transaction transaction, ref int inputAmount, ref int totalAmount, ref decimal? fee, ref int feeSingleKeyInputAmount, ref List<dynamic> feeSingleKeyUnspentsUsed, ref decimal estimatedSize)
        {
            inputAmount = 0;
            foreach (var unspent in param.unspents)
            {
                inputAmount += unspent.value;
                transaction.AddInput(new TxIn(new OutPoint(uint256.Parse(unspent.tx_hash), (int)unspent.tx_output_n), new Script(StringToByteArray(unspent.script))));
                //return (inputAmount < (feeSingleKeySourceAddress ? totalOutputAmount : totalAmount));
            }
            // if paying fees from an external single key wallet, add the inputs
            if (param.feeSingleKeySourceAddress != null)
            {
                // collect the amount used in the fee inputs so we can get change later
                feeSingleKeyInputAmount = 0;
                feeSingleKeyUnspentsUsed = new List<dynamic>();
                foreach (var unspent in param.feeSingleKeyUnspents)
                {
                    feeSingleKeyInputAmount += unspent.value;
                    inputAmount += unspent.value;
                    transaction.AddInput(new TxIn(new OutPoint(uint256.Parse(unspent.tx_hash), (int)unspent.tx_output_n)));
                    feeSingleKeyUnspentsUsed.Add(unspent);
                }
            }
            if (param.shouldComputeBestFee)
            {
                var approximateFee = calculateApproximateFee(new { param.feeRate, param.recipients, param.bitgoFeeInfo, transaction, param.extraChangeAmounts, param.feeSingleKeySourceAddress }, ref estimatedSize);
                var shouldRecurse = !fee.HasValue || approximateFee > fee;
                fee = approximateFee;
                // Recompute totalAmount from scratch
                totalAmount = (int)(fee + param.totalOutputAmount);
                if (param.bitgoFeeInfo != null)
                    totalAmount += param.bitgoFeeInfo.amount;
                if (shouldRecurse)
                {
                    // if fee changed, re-collect inputs
                    inputAmount = 0;
                    transaction = new Transaction();
                    return CollectInputs(new { param.unspents, param.shouldComputeBestFee, param.feeRate, param.recipients, param.bitgoFeeInfo, param.extraChangeAmounts, param.totalOutputAmount, param.feeSingleKeySourceAddress, param.feeSingleKeyUnspents }, transaction, ref inputAmount, ref totalAmount, ref fee, ref feeSingleKeyInputAmount, ref feeSingleKeyUnspentsUsed, ref estimatedSize);
                }
            }

            var totalFee = fee + (param.bitgoFeeInfo != null ? param.bitgoFeeInfo.amount : 0);
            if (param.feeSingleKeySourceAddress != null)
                if (totalFee > new List<dynamic>(param.feeSingleKeyUnspents).Sum(q => q.value))
                    throw new Exception("Insufficient fee amount available in single key fee source");
            if (inputAmount < (param.feeSingleKeySourceAddress != null ? param.totalOutputAmount : totalAmount))
                throw new Exception("Insufficient funds");
            return "";
        }

        internal void CollectOutputs(dynamic param, int inputAmount, decimal totalAmount, decimal? fee, ref List<dynamic> changeOutputs)
        {
            var estimatedTxSize = estimateTxSizeBytes(new { param.recipients, param.bitgoFeeInfo, param.transaction, param.extraChangeAmounts, param.feeSingleKeySourceAddress });
            if (estimatedTxSize >= 90000)
                throw new Exception("transaction too large: estimated size " + estimatedTxSize + " bytes");
            var outputs = new List<dynamic>();
            foreach (dynamic recipient in param.recipients)
            {
                var script = new List<byte>();

                if (recipient.Key != null)
                    script.AddRange(BitcoinAddress.Create(recipient.Key, BitGoClient.Constants.network).ScriptPubKey.ToBytes());
                else if (recipient.script != null)
                    script.AddRange(recipient.script);
                else
                    throw new Exception("neither recipient address nor script was provided");

                // validate travelInfo if it exists
                dynamic travelInfo = null;
                //if (!_.isEmpty(recipient.travelInfo))
                //{
                //    travelInfo = recipient.travelInfo;
                //    // Better to avoid trouble now, before tx is created
                //    bitgo.travelRule().validateTravelInfo(travelInfo);
                //}
                outputs.Add(new { script = script.ToArray(), amount = recipient.Value, travelInfo });
            }

            var result = getChangeOutputs(new { changeAmount = inputAmount - totalAmount, param.feeSingleKeySourceAddress, fee, param.bitgoFeeInfo, param.extraChangeAmounts });

            changeOutputs = result;
            var extraOutputs = changeOutputs.Select(q => new { q.address, q.amount, script = new BitcoinScriptAddress(q.address, BitGoClient.Constants.network).ScriptPubKey.ToBytes() }).ToList(); // copy the array
            //      if (bitgoFeeInfo && bitgoFeeInfo.amount > 0)
            //      {
            //          extraOutputs.push(bitgoFeeInfo);
            //      }
            foreach (var output in extraOutputs)
            {
                var outputIndex = param.forceChangeAtEnd ? outputs.Count : new Random().Next(0, outputs.Count);
                outputs.Insert(outputIndex, output);
            }
            //      extraOutputs.forEach(function(output) {
            //          output.script = bitcoin.address.toOutputScript(output.address, bitcoin.getNetwork());

            //          // decide where to put the outputs - default is to randomize unless forced to end
            //          var outputIndex = params.forceChangeAtEnd? outputs.length: _.random(0, outputs.length);
            //          outputs.splice(outputIndex, 0, output);
            //      });

            // Add all outputs to the transaction
            outputs.ForEach(output =>
            {
                ((Transaction)param.transaction).AddOutput(new Money((long)output.amount), new Script(output.script));
                //param.transaction.AddOutput(output.script, output.amount);
            });

            //      travelInfos = _(outputs).map(function(output, index) {
            //          var result = output.travelInfo;
            //          if (!result)
            //          {
            //              return undefined;
            //          }
            //          result.outputIndex = index;
            //          return result;
            //      })
            //.filter()
            //.value();
        }

        private decimal calculateApproximateFee(dynamic param, ref decimal estimatedSize)
        {
            var feeRateToUse = param.feeRate ?? BitGoClient.Constants.fallbackFeeRate;
            estimatedSize = estimateTxSizeBytes(new { param.recipients, param.bitgoFeeInfo, param.transaction, param.extraChangeAmounts, param.feeSingleKeySourceAddress });
            return Math.Ceiling(estimatedSize * feeRateToUse / 1000);
        }

        List<dynamic> getChangeOutputs(dynamic param)
        {
            if (param.changeAmount < 0)
                throw new Exception("negative change amount");
            var result = new List<dynamic>();
            if (param.feeSingleKeySourceAddress != null)
            {
                var feeSingleKeyWalletChangeAmount = param.feeSingleKeyInputAmount - (param.fee + (param.bitgoFeeInfo ? param.bitgoFeeInfo.amount : 0));
                if (feeSingleKeyWalletChangeAmount >= BitGoClient.Constants.minOutputSize)
                {
                    result.Add(new { address = param.feeSingleKeySourceAddress, amount = feeSingleKeyWalletChangeAmount });
                    param.changeAmount = param.changeAmount - feeSingleKeyWalletChangeAmount;
                }
            }

            if (param.changeAmount < BitGoClient.Constants.minOutputSize)
            {
                // Give it to the miners
                return result;
            }

            if (walletInstance.walletInstance.type == "safe") //Error
            {
                dynamic response = walletInstance.Addresses();
                //.then(function(response) {
                result.Add(new { address = response.address, amount = param.changeAmount });
                //    return result;
                //});
            }

            var extraChangeTotal = new List<dynamic>(param.extraChangeAmounts).Sum(q => q);
            // Sanity check
            if (extraChangeTotal > param.changeAmount)
            {
                param.extraChangeAmounts = new List<dynamic>();
                extraChangeTotal = 0;
            }

            // copy and add remaining change amount
            var allChangeAmounts = new List<dynamic>(param.extraChangeAmounts).Take(0).ToList();
            allChangeAmounts.Add(param.changeAmount - extraChangeTotal);

            dynamic changeAddress = null;
            var newResults = addChangeOutputs(new { walletInstance, allChangeAmounts, changeAddress = changeAddress });
            result.AddRange(newResults);
            return result;
        }

        List<dynamic> addChangeOutputs(dynamic param)
        {
            var result = new List<dynamic>();
            foreach (var changeAmount in param.allChangeAmounts)
                if (param.changeAddress != null)
                    // If user passed a change address, use it for all outputs
                    result.Add(new { address = param.changeAddress, amount = changeAmount });
                else
                    // Otherwise create a new address per output, for privacy
                    result.Add(new { address = walletInstance.CreateAddress(1, true), amount = changeAmount });
            return result;
        }

        decimal estimateTxSizeBytes(dynamic param)
        {
            var nExtraChange = param.extraChangeAmounts.Count;
            var sizePerP2SHInput = 295;
            var sizePerP2PKHInput = 160;
            var sizePerOutput = 34;

            // Add 1 output for change, possibly 1 output for instant fee, and 1 output for the single key change if needed.
            // If we do change splitting, we will add more fee later.

            // Incorrect bitgoFeeInfo value. Should be null?
            var nOutputs = (param.recipients.Count + 1 + nExtraChange + (param.bitgoFeeInfo != null ? 1 : 0) + (param.feeSingleKeySourceAddress != null ? 1 : 0));
            var nP2SHInputs = param.transaction.Inputs.Count + (param.feeSingleKeySourceAddress != null ? 1 : 0);
            var nP2PKHInputs = (param.feeSingleKeySourceAddress != null ? 1 : 0);

            return sizePerP2SHInput * nP2SHInputs + sizePerP2PKHInput * nP2PKHInputs + sizePerOutput * nOutputs;
        }

        byte[] StringToByteArray(String hex)
        {
            int NumberChars = hex.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }

        internal dynamic Serialize(dynamic param)
        {
            // only need to return the unspents that were used and just the chainPath, redeemScript, and instant flag
            var pickedUnspents = new List<dynamic>(param.unspents).Select(q => new { q.tx_hash, q.tx_output_n, q.value, q.chainPath, q.redeemScript, q.instant }).ToList();
            var prunedUnspents = new List<dynamic>(pickedUnspents).GetRange(0, ((NBitcoin.Transaction)param.transaction).Inputs.Count - new List<dynamic>(param.feeSingleKeyUnspentsUsed).Count); //var prunedUnspents = _.slice(pickedUnspents, 0, transaction.tx.ins.length - feeSingleKeyUnspentsUsed.length);
            foreach (dynamic feeUnspent in param.feeSingleKeyUnspentsUsed)
                prunedUnspents.Add(new { chainPath = false, redeemScript = false }); // mark as false to signify a non-multisig address

            var result = new
            {
                transactionHex = ((Transaction)param.transaction).ToHex(),
                unspents = prunedUnspents,
                fee = param.fee,
                changeAddresses = new List<dynamic>(param.changeOutputs).Select(q => new { q.address, q.amount }).ToList(), //q.path, 
                walletId = walletInstance.walletInstance.id,
                walletKeychains = walletInstance.walletInstance.@private.keychains,
                feeRate = param.feeRate,
                instant = param.instant,
                bitgoFee = param.bitgoFeeInfo,
                estimatedSize = param.estimatedSize,
                travelInfos = param.travelInfos,
                instantFee = new List<dynamic>()
            };

            //// Add for backwards compatibility
            //if (result.instant && param.bitgoFeeInfo)
            //    result.instantFee = new List<dynamic>(param.bitgoFeeInfo).Select(q => new { q.amount, q.address }).ToList();

            return result;
        }

        internal dynamic SignTransaction(dynamic param)
        {
            var keychain = param.keychain; // duplicate so as to not mutate below

            //var validate = param.validate ?? true;
            //dynamic privKey = null;
            //if (typeof(param.transactionHex) != 'string')
            //    throw new Exception("expecting the transaction hex as a string");
            //if (!Array.isArray(param.unspents))
            //    throw new Exception("expecting the unspents array");
            //if (typeof(validate) != 'boolean')
            //    throw new Exception("expecting validate to be a boolean");
            //if (typeof(keychain) != 'object' || typeof(keychain.xprv) != 'string')
            //{
            //    if (typeof(param.signingKey) === 'string')
            //    {
            //        privKey = bitcoin.ECPair.fromWIF(param.signingKey, bitcoin.getNetwork());
            //        keychain = undefined;
            //    }
            //    else
            //        throw new Exception("expecting the keychain object with xprv");
            //}

            //var feeSingleKey;
            //if (param.feeSingleKeyWIF)
            //    feeSingleKey = bitcoin.ECPair.fromWIF(param.feeSingleKeyWIF, bitcoin.getNetwork());

            var transaction = new Transaction(param.transactionHex);
            //var transaction = bitcoin.Transaction.fromHex(param.transactionHex);
            if (transaction.Inputs.Count != param.unspents.Count)
                throw new Exception("length of unspents array should equal to the number of transaction inputs");

            var rootExtKey = ExtKey.Parse(keychain.xprv);

            for (var index = 0; index < transaction.Inputs.Count; ++index)
            {
                //if (param.unspents[index].redeemScript === false)
                //{
                //    // this is the input from a single key fee address
                //    if (!feeSingleKey)
                //        throw new Exception("single key address used in input but feeSingleKeyWIF not provided");

                //    txb = bitcoin.TransactionBuilder.fromTransaction(transaction, bitcoin.getNetwork());
                //    txb.sign(index, feeSingleKey);
                //    transaction = txb.buildIncomplete();
                //    continue;
                //}

                if (rootExtKey != null)
                {
                    var subPath = keychain.walletSubPath ?? "/0/0";
                    var path = keychain.path + subPath + param.unspents[index].chainPath;
                    var extKey = rootExtKey.Derive(new KeyPath(path));
                    BitcoinSecret secret = extKey.PrivateKey.GetBitcoinSecret(BitGoClient.Constants.network);

                    List<Coin> coins = new List<Coin>();
                    foreach (var unspent in param.unspents)
                    {
                        var redeemScript = new Script(StringToByteArray(unspent.redeemScript));
                        coins.Add(new ScriptCoin(new OutPoint(uint256.Parse(unspent.tx_hash),
                            (uint)unspent.tx_output_n), new TxOut(new Money((decimal)unspent.value, MoneyUnit.BTC),
                            redeemScript.Hash.ScriptPubKey), redeemScript));
                    }
                    transaction = new NBitcoin.TransactionBuilder().
                                        AddCoins(coins).
                                        AddKeys(secret.PrivateKey).
                                        SignTransaction(transaction, SigHash.All);
                }

                // subscript is the part of the output script after the OP_CODESEPARATOR.
                // Since we are only ever signing p2sh outputs, which do not have
                // OP_CODESEPARATORS, it is always the output script.

                // In order to sign with bitcoinjs-lib, we must use its transaction
                // builder, confusingly named the same exact thing as our transaction
                // builder, but with inequivalent behavior.
                //txb = bitcoin.TransactionBuilder.fromTransaction(transaction, bitcoin.getNetwork());
                //var txb = new NBitcoin.TransactionBuilder().ContinueToBuild(transaction);
                //try
                //{
                //    txb.SignTransaction(index, privKey, subscript, bitcoin.Transaction.SIGHASH_ALL);
                //}
                //catch (Exception)
                //{
                //    return Q.reject('Failed to sign input #' + index);
                //}

                //// The signatures are validated server side and on the bitcoin network, so
                //// the signature validation is optional and can be disabled by setting:
                //if (validate)
                //    if (exports.verifyInputSignatures(transaction, index, subscript) !== -1)
                //        throw new Exception("number of signatures is invalid - something went wrong when signing");
            }

            return transaction.ToHex();
        }

        internal void VerifyInputSignatures(dynamic transaction, dynamic inputIndex, dynamic pubScript, dynamic ignoreKeyIndices)
        {
            if (inputIndex < 0 || inputIndex >= transaction.ins.length)
                throw new Exception("illegal index");
/*
            ignoreKeyIndices = ignoreKeyIndices || [];
            var sigScript = transaction.ins[inputIndex].script;
            var sigsNeeded = 1;
            var sigs = [];
            var pubKeys = [];
            var decompiledSigScript = bitcoin.script.decompile(sigScript);

            // Check the script type to determine number of signatures, the pub keys, and the script to hash.
            switch (bitcoin.script.classifyInput(sigScript, true))
            {
                case 'scripthash':
                    // Replace the pubScript with the P2SH Script.
                    pubScript = decompiledSigScript[decompiledSigScript.length - 1];
                    var decompiledPubScript = bitcoin.script.decompile(pubScript);
                    sigsNeeded = decompiledPubScript[0] - bitcoin.opcodes.OP_1 + 1;
                    for (var index = 1; index < decompiledSigScript.length - 1; ++index)
                    {
                        sigs.push(decompiledSigScript[index]);
                    }
                    for (index = 1; index < decompiledPubScript.length - 2; ++index)
                    {
                        // we minus 1 because the key indexes start from the second chunk (first chunk is used for total keys)
                        if (_.includes(ignoreKeyIndices, index - 1))
                        {
                            // ignore this public key (do not treat it as valid for a signature)
                            continue;
                        }
                        pubKeys.push(decompiledPubScript[index]);
                    }
                    break;
                case 'pubkeyhash':
                    sigsNeeded = 1;
                    sigs.push(decompiledSigScript[0]);
                    pubKeys.push(decompiledSigScript[1]);
                    break;
                default:
                    return 0;
            }

            var numVerifiedSignatures = 0;
            for (var sigIndex = 0; sigIndex < sigs.length; ++sigIndex)
            {
                // If this is an OP_0, then its been left as a placeholder for a future sig.
                if (sigs[sigIndex] == bitcoin.opcodes.OP_0)
                {
                    continue;
                }

                var hashType = sigs[sigIndex][sigs[sigIndex].length - 1];
                sigs[sigIndex] = sigs[sigIndex].slice(0, sigs[sigIndex].length - 1); // pop hash type from end
                var signatureHash = transaction.hashForSignature(inputIndex, pubScript, hashType);

                var validSig = false;

                // Enumerate the possible public keys
                for (var pubKeyIndex = 0; pubKeyIndex < pubKeys.length; ++pubKeyIndex)
                {
                    var pubKey = bitcoin.ECPair.fromPublicKeyBuffer(pubKeys[pubKeyIndex]);
                    var signature = bitcoin.ECSignature.fromDER(sigs[sigIndex]);
                    validSig = pubKey.verify(signatureHash, signature);
                    if (validSig)
                    {
                        pubKeys.splice(pubKeyIndex, 1);  // remove the pubkey so we can't match 2 sigs against the same pubkey
                        break;
                    }
                }
                if (!validSig)
                    throw new Exception("invalid signature for index " + inputIndex);
                numVerifiedSignatures++;
            }

            if (numVerifiedSignatures < sigsNeeded)
                numVerifiedSignatures = -numVerifiedSignatures;
            return numVerifiedSignatures;
            */
        }
    }
}