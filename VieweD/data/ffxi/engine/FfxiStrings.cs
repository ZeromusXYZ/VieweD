using VieweD.engine.common;

namespace VieweD.data.ffxi.engine
{
    public static class FfxiStrings
    {
        /// <summary>
        /// Special String encoding used by FFXI
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="encoded6BitKey"></param>
        /// <returns></returns>
        public static string GetPackedString16AtPos(BasePacketData packetData, int pos, char[] encoded6BitKey)
        {
            var res = "";
            // Hex: B8 81 68 24  72 14 4F 10  54 0C 8F 00  00 00 00 00
            // Bits:
            // 101110 00
            // 1000 0001
            // 01 101000
            // 001001 00
            // 0111 0010
            // 00 010100
            // 010011 11
            // 0001 0000
            // 01 010100
            // 000011 00
            // 1000 1111
            // 00 000000

            // PackedString: TheNightsWatch (with no spaces)
            // PackedNum: 2E 08 05 ...
            // 101110  T
            // 001000  h
            // 000101  e
            //

            // A_  6F F0    011011 11-1111 0000  =>  1B 3F 00  =>  A
            // B_  73 F0    011100 11-1111 0000  =>  1C 3F 00  =>  B
            // F_  83 F0    100000 11-1111 0000  =>  20 3F 00  =>  F

            // EncodeLSStr : Array [0..63] of Char = (
            // #0 ,'a','b','c','d','e','f','g',  'h','i','j','k','l','m','n','o', // $00
            // 'p','q','r','s','t','u','v','w',  'x','y','z','A','B','C','D','E', // $10
            // 'F','G','H','I','J','K','L','M',  'N','O','P','Q','R','S','T','U', // $20
            // 'V','W','X','Y','Z',' ',' ',' ',  ' ',' ',' ',' ',' ',' ',' ', #0  // $30
            //  0   1   2   3   4   5   6   7     8   9   A   B   C   D   E   F
            // );
            var offset = 0;
            while ((offset / 8) < 15)
            {
                byte encodedChar = 0;
                byte bitMask = 0b00100000;
                for (var bit = 0; bit < 6; bit++)
                {
                    bool isSet = packetData.GetBitAtPos(pos + ((offset + bit) / 8), 7 - ((offset + bit) % 8));
                    if (isSet)
                        encodedChar += bitMask;
                    bitMask >>= 1;
                }
                // GetBitsAtPos(pos + (Offset / 8), (Offset % 8), 6);
                if ((encodedChar >= encoded6BitKey.Length) || (encodedChar <= 0))
                    break;
                var c = encoded6BitKey[encodedChar];
                if (c == 0)
                    break;
                res += c;
                offset += 6;
            }
            return res;
        }

        public static char[] ItemEncoding = new char[0x40] {
            //   0    1    2    3    4    5    6    7    8    9    A    B    C    D    E    F
            '\0', '0', '1', '2', '3', '4', '5', '6', '7', '9', '8', 'A', 'B', 'C', 'D', 'E', // 0x00
            'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', // 0x10
            'V', 'W', 'X', 'Y', 'Z', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', // 0x20
            'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '\0' // 0x30
        };
        public static char[] LinkShellEncoding = new char[0x40] {
            //   0    1    2    3    4    5    6    7    8    9    A    B    C    D    E    F
            '\0', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', // 0x00
            'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', 'A', 'B', 'C', 'D', 'E', // 0x10
            'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', // 0x20
            'V', 'W', 'X', 'Y', 'Z', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '\0' // 0x30
        };
    }
}
