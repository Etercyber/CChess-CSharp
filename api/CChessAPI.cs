using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Chess
{
    class CChessAPI
    {
        private CChessAPI()
        {

        }
        public const int MAX_MOVE = 1023;
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct RollbackStruct
        {
            public uint dwZobristKey;
            public uint dwZobristLock0;
            public uint dwZobristLock1;
            public int vlWhiteValue;
            public int vlBlackValue;
            public uint mvs;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct PositionStruct
        {
            public int sdPlayer;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256, ArraySubType = UnmanagedType.U1)]
            public byte[] ucpcSquares;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 48, ArraySubType = UnmanagedType.U1)]
            public byte[] ucsqPieces;
            public uint dwZobristKey;
            public uint dwZobristLock0;
            public uint dwZobristLock1;
            public uint dwBitPiece;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.U2)]
            public ushort[] wBitRanks;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16, ArraySubType = UnmanagedType.U2)]
            public ushort[] wBitFiles;
            public int vlWhiteValue;
            public int vlBlackValue;
            public int nMoveNum;
            public int nDistance;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_MOVE, ArraySubType = UnmanagedType.Struct)]
            public RollbackStruct[] rbsList;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4095, ArraySubType = UnmanagedType.U1)]
            public byte[] ucRepHash;
        }

        [DllImport("libcchess.dll", EntryPoint = "_CChessVersion@0", CharSet = CharSet.Ansi)]
        public static extern string CChessVersion();
        [DllImport("libcchess.dll", EntryPoint = "_CChessInit@4")]
        public static extern void CChessInit(int bTraditional = 0);
        [DllImport("libcchess.dll", EntryPoint = "_CChessPromotion@4")]
        public static extern void CChessPromotion(int bPromotion = 0);
        [DllImport("libcchess.dll", EntryPoint = "_CChessAddPiece@16")]
        public static extern void _CChessAddPiece(IntPtr pos, int sq, int pc, int bDel = 0);
        [DllImport("libcchess.dll", EntryPoint = "_CChessCanPromote@8")]
        public static extern int CChessCanPromote(IntPtr pos, int sq);
        [DllImport("libcchess.dll", EntryPoint = "_CChessTryMove@12")]
        public static extern int CChessTryMove(IntPtr pos, IntPtr nStatus, int mv);
        [DllImport("libcchess.dll", EntryPoint = "_CChessUndoMove@4")]
        public static extern void CChessUndoMove(IntPtr pos);
        [DllImport("libcchess.dll", EntryPoint = "_CChessTryNull@4")]
        public static extern int CChessTryNull(IntPtr pos);
        [DllImport("libcchess.dll", EntryPoint = "_CChessUndoNull@4")]
        public static extern void CChessUndoNull(IntPtr pos);
        [DllImport("libcchess.dll", EntryPoint = "_CChessGenMoves@8")]
        public static extern int CChessGenMoves(IntPtr pos, IntPtr lpmv);
        [DllImport("libcchess.dll", EntryPoint = "_CChessSetIrrev@4")]
        public static extern void CChessSetIrrev(IntPtr pos);
        [DllImport("libcchess.dll", EntryPoint = "_CChessClearBoard@4")]
        public static extern void CChessClearBoard(IntPtr pos);
        [DllImport("libcchess.dll", EntryPoint = "_CChessCoord2Move@4")]
        public static extern int CChessCoord2Move(IntPtr szFen);
        [DllImport("libcchess.dll", EntryPoint = "_CChessBoardMirror@4")]
        public static extern void CChessBoardMirror(IntPtr pos);
        [DllImport("libcchess.dll", EntryPoint = "_CChessExchangeSide@4")]
        public static extern void CChessExchangeSide(IntPtr pos);
        [DllImport("libcchess.dll", EntryPoint = "_CChessFlipBoard@4")]
        public static extern void CChessFlipBoard(IntPtr pos);
        [DllImport("libcchess.dll", EntryPoint = "_CChessBoardText@8")]
        public static extern IntPtr CChessBoardText(IntPtr pos, int bAnsi = 0);
        [DllImport("libcchess.dll", EntryPoint = "_CChessBoard2Fen@4", CharSet = CharSet.Ansi)]
        public static extern IntPtr _CChessBoard2Fen(IntPtr pos);
        [DllImport("libcchess.dll", EntryPoint = "_CChessFen2Board@8", CharSet = CharSet.Ansi)]
        public static extern void CChessFen2Board(IntPtr pos, string szFen);
        [DllImport("libcchess.dll", EntryPoint = "_CChessFenMirror@4", CharSet = CharSet.Ansi)]
        public static extern IntPtr CChessFenMirror(string szFenSrc);
        [DllImport("libcchess.dll", EntryPoint = "_CChessFileMirror@4")]
        public static extern int CChessFileMirror(int dwFileStr);
        [DllImport("libcchess.dll", EntryPoint = "_CChessChin2File@8")]
        public static extern int CChessChin2File(long qwChinStr);
        [DllImport("libcchess.dll", EntryPoint = "_CChessFile2Chin@8")]
        public static extern ulong CChessFile2Chin(int dwFileStr, int sd);
        [DllImport("libcchess.dll", EntryPoint = "_CChessFile2Move@8")]
        public static extern int CChessFile2Move(int dwFileStr, IntPtr pos);
        [DllImport("libcchess.dll", EntryPoint = "_CChessMove2File@8")]
        public static extern int CChessMove2File(int mv, IntPtr pos);

        public const int MOVE_ILLEGAL = 256;
        public const int MOVE_INCHECK = 128;
        public const int MOVE_DRAW = 64;
        public const int MOVE_PERPETUAL_LOSS = 32;
        public const int MOVE_PERPETUAL_WIN = 16;
        public const int MOVE_PERPETUAL = 8;
        public const int MOVE_MATE = 4;
        public const int MOVE_CHECK = 2;
        public const int MOVE_CAPTURE = 1;

        public const int CCHESS_TRADITIONAL = 1;
        public const int CCHESS_DELETE_PIECE = 1;
        public const int CCHESS_ANSI_TEXT = 1;

        public const string CCHESS_START_FEN = "rnbakabnr/9/1c5c1/p1p1p1p1p/9/9/P1P1P1P1P/1C5C1/9/RNBAKABNR w";
        public static string[] CCHESS_TYPE_CHAR = new string[] { "K", "A", "B", "N", "R", "C", "P" };
        public static short[] CCHESS_TYPE_SWITCH = new short[] { 0, 1, 1, 2, 2, 3, 3, 4, 4, 5, 5, 6, 6, 6, 6, 6, 0, 1, 1, 2, 2, 3, 3, 4, 4, 5, 5, 6, 6, 6, 6, 6 };
        public static short[] EditRedPart = new short[] { 23, 21, 19, 17, 16, 25, 27 };
        public static short[] EditBlackPart = new short[] { 39, 37, 35, 33, 32, 41, 43 };

        public static short PieceType(short nArg)
        {
            short Ret = (short)(nArg - 15 - 1);

            if (Ret < 0 || Ret >= CCHESS_TYPE_SWITCH.Length)
                return 7;
            else
                return CCHESS_TYPE_SWITCH[Ret];
        }

        public static string PieceByte(short nArg)
        {
            return CCHESS_TYPE_CHAR[nArg + 1 - 1];
        }

        public static short Src(int mv)
        {
            return (short)(mv % 256);
        }

        public static short Dst(int mv)
        {
            return (short)(mv / 256);
        }

        public static int Move(short sqSrc, short sqDst)
        {
            return sqSrc + sqDst * 256;
        }

        public static int MoveMirror(int mv)
        {
            short sqSrc = (short)(Src(mv) / 16),
                sqDst = (short)(Dst(mv) / 16);
            sqSrc *= 16; sqDst *= 16;
            sqSrc += (short)(14 - Src(mv) % 16);
            sqDst += (short)(14 - Dst(mv) % 16);

            if (sqSrc >= 0 && sqSrc <= 255 &&
                sqDst >= 0 && sqDst <= 255)
                return Move(sqSrc, sqDst);
            else
                return 0;
        }

        public static byte Asc(string str)
        {
            return (byte)str[0];
        }

        public static string Chr(int code)
        {
            return ((char)(byte)code).ToString();
        }

        public static string Mid(string str, int start, int length)
        {
            return str.Substring(start, length);
        }

        public static int Coord2Move(string szCoord)
        {
            short sqSrc = 60, sqDst = 60;

            if (szCoord.Length >= 4)
            {
                sqSrc -= Asc(Mid(szCoord, 2, 1));
                sqSrc *= 16;
                sqSrc += Asc(Mid(szCoord, 1, 1));
                sqSrc -= 94;
                sqDst -= Asc(Mid(szCoord, 4, 1));
                sqDst *= 16;
                sqDst += Asc(Mid(szCoord, 3, 1));
                sqDst -= 94;

                if (sqSrc >= 0 && sqSrc <= 255 &&
                    sqDst >= 0 && sqDst <= 255)
                {
                    return Move(sqSrc, sqDst);
                }
            }

            return 0;
        }

        public static string Move2Coord(int mv)
        {
            return Chr(Src(mv) % 16 + 94) + Chr(60 - Src(mv) / 16) +
                Chr(Dst(mv) % 16 + 94) + Chr(60 - Dst(mv) / 16);
        }

        public static int Iccs2Move(string szIccs)
        {
            short sqSrc = 60, sqDst = 60;

            if (szIccs.Length >= 5)
            {
                sqSrc -= Asc(Mid(szIccs, 2, 1));
                sqSrc *= 16;
                sqSrc += Asc(Mid(szIccs, 1, 1));
                sqSrc -= 62;
                sqDst -= Asc(Mid(szIccs, 5, 1));
                sqDst *= 16;
                sqDst += Asc(Mid(szIccs, 4, 1));
                sqDst -= 62;

                if (sqSrc >= 0 && sqSrc <= 255 &&
                    sqDst >= 0 && sqDst <= 255)
                {
                    return Move(sqSrc, sqDst);
                }
            }
            return 0;
        }

        public static string Move2Iccs(int mv)
        {
            return Chr(Src(mv) % 16 + 62) + Chr(60 - Src(mv) / 16) +
                "-" + Chr(Dst(mv) % 16 + 62) + Chr(60 - Dst(mv) / 16);
        }

        public static int CvL(string sz)
        {
            int Dat = 0;
            IntPtr pInt = Marshal.AllocHGlobal(4);
            IntPtr pStr = Marshal.StringToHGlobalAnsi(sz);

            Win32API.RtlMoveMemory(pInt, pStr, 4);
            Dat = Marshal.ReadInt32(pInt);

            Marshal.FreeHGlobal(pInt);
            Marshal.FreeHGlobal(pStr);

            return Dat;
        }

        public static long CvC(string sz)
        {
            long Dat = 0;
            IntPtr pLong = Marshal.AllocHGlobal(8);
            IntPtr pStr = Marshal.StringToHGlobalAnsi(sz);

            Win32API.RtlMoveMemory(pLong, pStr, 4);
            Dat = Marshal.ReadInt64(pLong);

            Marshal.FreeHGlobal(pLong);
            Marshal.FreeHGlobal(pStr);

            return Dat;
        }

        public static bool PlayerEquare(short pc, int sdPlayer)
        {
            if (pc > 0 && pc < 32 && sdPlayer == 1)
                return true;
            else if (pc >= 32 && sdPlayer == 0)
                return true;
            else
                return false;
        }

        public static string CChessBoard2Fen(PositionStruct pos)
        {
            string nRet = "";
            int nSize = Marshal.SizeOf(pos);
            IntPtr ptrStr = IntPtr.Zero;
            IntPtr ptrPos = Marshal.AllocHGlobal(nSize);

            Marshal.StructureToPtr(pos, ptrPos, false);
            ptrStr = _CChessBoard2Fen(ptrPos);
            nRet = Marshal.PtrToStringAnsi(ptrStr);

            Marshal.FreeHGlobal(ptrPos);

            return nRet;
        }

        public static void CChessAddPiece(ref PositionStruct pos, int sq, int pc, int bDel = 0)
        {
            int nSize = Marshal.SizeOf(pos);
            IntPtr ptrPos = Marshal.AllocHGlobal(nSize);

            Marshal.StructureToPtr(pos, ptrPos, false);
            _CChessAddPiece(ptrPos, sq, pc, bDel);

            pos = (PositionStruct)Marshal.PtrToStructure
                (ptrPos, typeof (PositionStruct));

            Marshal.FreeHGlobal(ptrPos);
        }

        public static int CChessSearchMoveStr2Move(uint search)
        {
            int Move;
            byte[] ByteArray;
            IntPtr ptrVal = Marshal.AllocHGlobal(4);

            // Convert data to marshal type
            ByteArray = BitConverter.GetBytes(search);

            // Write the values of the uint type into memory by bytes
            for (int i = 0; i < ByteArray.Length; i++)
                Marshal.WriteByte(ptrVal, i, ByteArray[i]);
            Move = CChessCoord2Move(ptrVal);

            // Free any resource
            Marshal.FreeHGlobal(ptrVal);

            return Move;
        }


        public static string CChessMove2Chin(PositionStruct pos, int mv)
        {
            ulong Chin64;
            string ChinFen;
            byte[] ByteArray;

            // ANSI String
            IntPtr ptrPos = Marshal.AllocHGlobal(29024);
            Marshal.StructureToPtr(pos, ptrPos, false);

            Chin64 = CChessFile2Chin(CChessMove2File(mv, ptrPos), pos.sdPlayer);
            ByteArray = BitConverter.GetBytes(Chin64);

            // Spitter into 4 * 2 byte of string
            ChinFen = Encoding.Default.GetString(ByteArray);

            // Free any resource
            Marshal.FreeHGlobal(ptrPos);

            return ChinFen;
        }
    }
}
