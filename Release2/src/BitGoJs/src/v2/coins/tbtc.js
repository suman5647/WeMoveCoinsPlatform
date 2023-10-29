const Btc = require('./btc');
const bitcoin = require('bitgo-utxo-lib');

class Tbtc extends Btc {

  constructor() {
    super();
    this.network = bitcoin.networks.testnet;
    this.network.bech32Prefix = 'tb';
  }

  getChain() {
    return 'tbtc';
  }

  getFullName() {
    return 'Testnet Bitcoin';
  }

}

module.exports = Tbtc;
