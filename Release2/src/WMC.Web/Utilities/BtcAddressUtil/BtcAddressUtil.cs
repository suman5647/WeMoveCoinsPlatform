// Copyright 2012 Mike Caldwell (Casascius)
// This file is part of Bitcoin Address Utility.

// Bitcoin Address Utility is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.

// Bitcoin Address Utility is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.

// You should have received a copy of the GNU General Public License
// along with Bitcoin Address Utility.  If not, see http://www.gnu.org/licenses/.

using Org.BouncyCastle.Crypto.Digests;
using System;

namespace WMC.Web.Utilities
{
    public class BtcAddressUtil
    {
        public static byte[] Base58CheckToByteArray(string base58)
        {
            byte[] bb = Base58.ToByteArray(base58);
            if (bb == null || bb.Length < 4) return null;

            Sha256Digest bcsha256a = new Sha256Digest();
            bcsha256a.BlockUpdate(bb, 0, bb.Length - 4);

            byte[] checksum = new byte[32];  //sha256.ComputeHash(bb, 0, bb.Length - 4);
            bcsha256a.DoFinal(checksum, 0);
            bcsha256a.BlockUpdate(checksum, 0, 32);
            bcsha256a.DoFinal(checksum, 0);

            for (int i = 0; i < 4; i++)
                if (checksum[i] != bb[bb.Length - 4 + i]) return null;

            byte[] rv = new byte[bb.Length - 4];
            Array.Copy(bb, 0, rv, 0, bb.Length - 4);
            return rv;
        }
    }
}