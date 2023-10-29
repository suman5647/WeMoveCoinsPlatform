const common = require('../common');
const _ = require('lodash');
const Promise = require('bluebird');
const co = Promise.coroutine;

const Keychains = function(bitgo, baseCoin) {
  this.bitgo = bitgo;
  this.baseCoin = baseCoin;
};

Keychains.prototype.get = function(params, callback) {
  params = params || {};
  common.validateParams(params, [], ['xpub', 'ethAddress'], callback);

  if (!params.id) {
    throw new Error('id must be defined');
  }

  const id = params.id;
  return this.bitgo.get(this.baseCoin.url('/key/' + encodeURIComponent(id)))
  .result()
  .nodeify(callback);
};

/**
 * list the users keychains
 * @param params.limit - Max number of results in a single call.
 * @param params.prevId - Continue iterating (provided by nextBatchPrevId in the previous list)
 * @param callback
 * @returns {*}
 */
Keychains.prototype.list = function(params, callback) {
  return co(function *() {
    params = params || {};
    common.validateParams(params, [], [], callback);

    const queryObject = {};

    if (!_.isUndefined(params.limit)) {
      if (!_.isNumber(params.limit)) {
        throw new Error('invalid limit argument, expecting number');
      }
      queryObject.limit = params.limit;
    }
    if (!_.isUndefined(params.prevId)) {
      if (!_.isString(params.prevId)) {
        throw new Error('invalid prevId argument, expecting string');
      }
      queryObject.prevId = params.prevId;
    }

    return this.bitgo.get(this.baseCoin.url('/key')).query(queryObject).result();
  }).call(this).asCallback(callback);
};

/**
 * iterates through all keys associated with the user, decrypts them with the old password and encrypts them with the
 * new password
 * @param params.oldPassword - The old password used for encrypting the key
 * @param params.newPassword - The new password to be used for encrypting the key
 * @param callback
 * @returns changedKeys Object - e.g.:
 *  {
 *    xpub1: encryptedPrv,
 *    ...
 *  }
 */
Keychains.prototype.updatePassword = function(params, callback) {
  return co(function *() {
    common.validateParams(params, ['oldPassword', 'newPassword'], [], callback);
    const changedKeys = {};
    let prevId;
    let keysLeft = true;
    while (keysLeft) {
      const result = yield this.list({ limit: 500, prevId });
      for (const key of result.keys) {
        const oldEncryptedPrv = key.encryptedPrv;
        if (_.isUndefined(oldEncryptedPrv)) {
          continue;
        }
        try {
          const decryptedPrv = this.bitgo.decrypt({ input: oldEncryptedPrv, password: params.oldPassword });
          const newEncryptedPrv = this.bitgo.encrypt({ input: decryptedPrv, password: params.newPassword });
          changedKeys[key.pub] = newEncryptedPrv;
        } catch (e) {
          // catching an error here means that the password was incorrect and hence there is nothing to change
        }
      }
      if (result.nextBatchPrevId) {
        prevId = result.nextBatchPrevId;
      } else {
        keysLeft = false;
      }
    }
    return changedKeys;
  }).call(this).asCallback(callback);
};

Keychains.prototype.create = function(params) {
  params = params || {};
  common.validateParams(params, [], []);

  return this.baseCoin.generateKeyPair(params.seed);
};

Keychains.prototype.add = function(params, callback) {
  params = params || {};
  common.validateParams(params, [], ['pub', 'encryptedPrv', 'type', 'source', 'originalPasscodeEncryptionCode', 'enterprise', 'derivedFromParentWithSeed'], callback);

  return this.bitgo.post(this.baseCoin.url('/key'))
  .send({
    pub: params.pub,
    encryptedPrv: params.encryptedPrv,
    type: params.type,
    source: params.source,
    provider: params.provider,
    originalPasscodeEncryptionCode: params.originalPasscodeEncryptionCode,
    enterprise: params.enterprise,
    derivedFromParentWithSeed: params.derivedFromParentWithSeed
  })
  .result()
  .nodeify(callback);
};

Keychains.prototype.createBitGo = function(params, callback) {
  params = params || {};
  common.validateParams(params, [], [], callback);
  params.source = 'bitgo';

  this.baseCoin.preCreateBitGo(params);

  return this.add(params, callback);
};

Keychains.prototype.createBackup = function(params, callback) {
  return co(function *() {
    params = params || {};
    common.validateParams(params, [], ['provider'], callback);
    params.source = 'backup';

    if (!params.provider) {
      // if the provider is undefined, we generate a local key and add the source details
      const key = this.create();
      _.extend(params, key);
    }

    const serverResponse = yield this.add(params, callback);
    return _.extend({}, serverResponse, _.pick(params, ['prv', 'encryptedPrv', 'provider', 'source']));
  }).call(this).asCallback(callback);
};


module.exports = Keychains;
