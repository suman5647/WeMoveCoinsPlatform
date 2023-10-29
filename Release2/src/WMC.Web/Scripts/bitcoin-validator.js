function validateBitcoinAddress(address) {
    var decoded_hex = base58_decode(address);
    //var decoded = str2ab(decoded_hex);
    var hexstr = hex2a(decoded_hex);
    var decoded = hexStringToByte(decoded_hex);

    //var decoded = decode(address);
    if (decoded.length != 25) return false;

    //var cksum = decoded.substr(decoded.length - 4);
    //var rest = decoded.substr(0, decoded.length - 4);

    //var good_cksum = hex2a(sha256_digest(hex2a(sha256_digest(rest)))).substr(0, 4);

    //if (cksum != good_cksum) return false;
    //return true;

    var length = decoded.length;
    var cksum = hexstr.slice(length - 4, length).toString('binary');
    var body = decoded.slice(0, length - 4);

    var good_cksum = sha256_digest(sha256_digest(body)).toString('binary').substr(0, 4);
    return (cksum === good_cksum ? decoded_hex.slice(0, 2) : null);
}

// prep position lookup table
var vals = '123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz';
var positions = {};
for (var i = 0 ; i < vals.length ; ++i) {
    positions[vals[i]] = i;
}

/// decode a base58 string payload into a hex representation
function decode(payload) {
    var base = 58;

    var length = payload.length;
    var num = int(0);
    var leading_zero = 0;
    var seen_other = false;
    for (var i = 0; i < length ; ++i) {
        var char = payload[i];
        var p = positions[char];

        // if we encounter an invalid character, decoding fails
        if (p === undefined) {
            throw new Error('invalid base58 string: ' + payload);
        }

        num = num.mul(base).add(p);

        if (char == '1' && !seen_other) {
            ++leading_zero;
        }
        else {
            seen_other = true;
        }
    }

    var hex = num.toString(16);

    // num.toString(16) does not have leading 0
    if (hex.length % 2 !== 0) {
        hex = '0' + hex;
    }

    // strings starting with only ones need to be adjusted
    // e.g. '1' should map to '00' and not '0000'
    if (leading_zero && !seen_other) {
        --leading_zero;
    }

    while (leading_zero-- > 0) {
        hex = '00' + hex;
    }

    return hex;
}

function base58_decode(string) {
    var table = '123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz';
    var table_rev = new Array();

    var i;
    for (i = 0; i < 58; i++) {
        table_rev[table[i]] = int2bigInt(i, 8, 0);
    }

    var l = string.length;
    var long_value = int2bigInt(0, 1, 0);

    var num_58 = int2bigInt(58, 8, 0);

    var c;
    for (i = 0; i < l; i++) {
        c = string[l - i - 1];
        long_value = add(long_value, mult(table_rev[c], pow(num_58, i)));
    }

    var hex = bigInt2str(long_value, 16);

    // num.toString(16) does not have leading 0
    if (hex.length % 2 !== 0) {
        hex = '0' + hex;
    }

    //var str = hex2a(hex);

    //var nPad;
    //for (nPad = 0; string[nPad] == table[0]; nPad++);

    //var output = str;
    //if (nPad > 0) output = repeat("\0", nPad) + str;

    return hex;
}

function hexStringToByte(str) {
    if (!str) {
        return new Uint8Array();
    }

    var a = [];
    for (var i = 0, len = str.length; i < len; i += 2) {
        a.push(parseInt(str.substr(i, 2), 16));
    }

    return new Uint8Array(a);
}
function byteToHexString(uint8arr) {
    if (!uint8arr) {
        return '';
    }
    var hexStr = '';    for (var i = 0; i < uint8arr.length; i++) {
        var hex = (uint8arr[i] & 0xff).toString(16);        hex = (hex.length === 1) ? '0' + hex : hex;        hexStr += hex;
    }    return hexStr.toUpperCase();
}
function hex2a(hex) {
    var str = '';
    for (var i = 0; i < hex.length; i += 2)
        str += String.fromCharCode(parseInt(hex.substr(i, 2), 16));
    return str;
}

function a2hex(str) {
    var aHex = "0123456789abcdef";
    var l = str.length;
    var nBuf;
    var strBuf;
    var strOut = "";
    for (var i = 0; i < l; i++) {
        nBuf = str.charCodeAt(i);
        strBuf = aHex[Math.floor(nBuf / 16)];
        strBuf += aHex[nBuf % 16];
        strOut += strBuf;
    }
    return strOut;
}
function ab2str(buf) {
    return String.fromCharCode.apply(null, new Uint16Array(buf));
}
function str2ab(str) {
    var buf = new ArrayBuffer(str.length * 2); // 2 bytes for each char
    var bufView = new Uint16Array(buf);
    for (var i = 0, strLen = str.length; i < strLen; i++) {
        bufView[i] = str.charCodeAt(i);
    }
    return buf;
}
function pow(big, exp) {
    if (exp == 0) return int2bigInt(1, 1, 0);
    var i;
    var newbig = big;
    for (i = 1; i < exp; i++) {
        newbig = mult(newbig, big);
    }

    return newbig;
}

function repeat(s, n) {
    var a = [];
    while (a.length < n) {
        a.push(s);
    }
    return a.join('');
}