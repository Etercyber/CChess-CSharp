using System;
using System.Runtime.InteropServices;

namespace Chess
{
    public class CAlpha
    {
        public const int LEVEL_GRANDMASTER = -8;
        public const int LEVEL_MASTER = -7;
        public const int LEVEL_EXPERT = -6;
        public const int LEVEL_AMATEUR = -5;
        public const int LEVEL_BEGINNER = -4;
        public const int LEVEL_ROOKIE = -3;
        public const int LEVEL_NEWBIE = -2;
        public const int LEVEL_PEABRAIN = -1;
        public const int LEVEL_DEPTH_MIN = 1;
        public const int LEVEL_DEPTH_MAX = 99;
        public const int LEVEL_INFINITE = 100;

        // Keyword related options and UCCI instructions
        public enum UcciOptionEnum
        {
            UCCI_OPTION_UNKNOWN, UCCI_OPTION_BATCH, UCCI_OPTION_DEBUG, UCCI_OPTION_PONDER, UCCI_OPTION_ALWAYSCHECK, UCCI_OPTION_USEHASH, UCCI_OPTION_USEBOOK, UCCI_OPTION_USEEGTB,
            UCCI_OPTION_BOOKFILES, UCCI_OPTION_EGTBPATHS, UCCI_OPTION_EVALAPI, UCCI_OPTION_HASHSIZE, UCCI_OPTION_THREADS, UCCI_OPTION_PROMOTION,
            UCCI_OPTION_IDLE, UCCI_OPTION_PRUNING, UCCI_OPTION_KNOWLEDGE, UCCI_OPTION_RANDOMNESS, UCCI_OPTION_STYLE, UCCI_OPTION_NEWGAME
        }; // Option specified by "setoption"

        public enum UcciRepetEnum
        {
            UCCI_REPET_ALWAYSDRAW, UCCI_REPET_CHECKBAN, UCCI_REPET_ASIANRULE, UCCI_REPET_CHINESERULE
        }; // The setting of option "repetition"

        public enum UcciGradeEnum
        {
            UCCI_GRADE_NONE, UCCI_GRADE_SMALL, UCCI_GRADE_MEDIUM, UCCI_GRADE_LARGE
        }; // Option "idle" or "pruning", "knowledge" and "selectivity" set point

        public enum UcciStyleEnum
        {
            UCCI_STYLE_SOLID, UCCI_STYLE_NORMAL, UCCI_STYLE_RISKY
        }; // Option "style" of the set value

        public enum UcciGoEnum
        {
            UCCI_GO_DEPTH, UCCI_GO_NODES, UCCI_GO_TIME_MOVESTOGO, UCCI_GO_TIME_INCREMENT
        }; // The time modes specified by the "go" command are limited depth, limited nodes, time period and overtime

        public enum UcciInputEnum
        {
            UCCI_INPUT_UCCI, UCCI_INPUT_ISREADY, UCCI_INPUT_PONDERHIT, UCCI_INPUT_PONDERHIT_DRAW, UCCI_INPUT_STOP,
            UCCI_INPUT_SETOPTION, UCCI_INPUT_POSITION, UCCI_INPUT_BANMOVES, UCCI_INPUT_GO, UCCI_INPUT_PROBE, UCCI_INPUT_QUIT
        }; // UCCI instruction type

        [StructLayout(LayoutKind.Explicit)]
        public struct Union1
        {
            [FieldOffset(0)]
            public int nSpin;
            [FieldOffset(0)]
            public byte bCheck;
            [FieldOffset(0)]
            public UcciRepetEnum Repet;
            [FieldOffset(0)]
            public UcciGradeEnum Grade;
            [FieldOffset(0)]
            public UcciStyleEnum Style;
            [FieldOffset(0)]
            public IntPtr szOption;
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct Union2
        {
            [FieldOffset(0)]
            public int nDepth;
            [FieldOffset(0)]
            public int nNodes;
            [FieldOffset(0)]
            public int nTime;
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct Union3
        {
            [FieldOffset(0)]
            public int nMovesToGo;
            [FieldOffset(0)]
            public int nIncrement;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct Struct1
        {
            public UcciOptionEnum Option;
            [MarshalAs(UnmanagedType.Struct)]
            public Union1 U1;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct Struct2
        {
            public IntPtr szFenStr;
            public int nMoveNum;
            IntPtr lpdwMovesCoord;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct Struct3
        {
            public int nBanMoveNum;
            public IntPtr lpdwBanMovesCoord;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct Struct4
        {
            public UcciGoEnum Go;
            public byte bPonder;
            public byte bDraw;
            public Union2 U2;
            public Union3 U3;
        }

        [StructLayout(LayoutKind.Explicit, Size = 16)]
        public struct UcciInputStruct
        {
            /* There are only four types of UCCI instructions for which specific information can be obtained
             *
             * 1. The information passed by the "setoption" instruction is suitable for the "UCCI_INPUT_SETOPTION" instruction type
             *    The "setoption" directive is used to set the option, so the engine receives the information of "option type" and "option value".
             *    For example, "setoption batch on", the option type is "UCCI_OPTION_DEBUG", and the Value (value.bcheck) is "true".
             */
            [FieldOffset(0)]
            public Struct1 S1;

            /* 2. The information passed by "position" directive, suitable for "e_CommPosition" directive type
             *    The "position" directive is used to set up the situation, including the initial situation and the subsequent situation
             *    For example, the FEN string of position startpos moves h2e2 h9g8, the FEN string of "startpos" and the normal number (MoveNum) is 2
             */
            [FieldOffset(0)]
            public Struct2 S2;

            /* 3. The information passed by the "banmoves" directive is appropriate for the "e_CommBanMoves" directive type
             *    The "banmoves" instruction is used to set the forbidden method, and the data structure is similar to the following moves of the "position" instruction but without FEN string
             */
            [FieldOffset(0)]
            public Struct3 S3;

            /* 4. The information passed by the "go" directive is suitable for the "UCCI_INPUT_GO" directive type
             *    The "go" command makes the engine think (search) and sets the mode of thinking, be it fixed depth, time or overtime
             */
            [FieldOffset(0)]
            public Struct4 S4;
        };

        public enum UcciPopHashEnum
        {
            UCCI_POP_HASH_START, UCCI_POP_HASH_STOP, UCCI_POP_HASH_BEST_MOVE, UCCI_POP_HASH_LOWER_BOUND, UCCI_POP_HASH_UPPER_BOUND
        };

        public enum UcciPopPvLineEnum
        {
            UCCI_POP_PV_LINE_TIME, UCCI_POP_PV_LINE_NODES, UCCI_POP_PV_LINE_DEPTH, UCCI_POP_PV_LINE_SCORE, UCCI_POP_PV_LINE_PV
        };

        public enum UcciSearchEnum
        { // info currmove
            UCCI_SEARCH_NO_BEST_MOVE, UCCI_SEARCH_BEST_MOVE, UCCI_SEARCH_DEPTH, UCCI_SEARCH_SCORE, UCCI_SEARCH_PONDER,
            UCCI_SEARCH_PV, UCCI_SEARCH_RESIGN, UCCI_SEARCH_DRAW, UCCI_SEARCH_CURRMOVE
        };

        public enum UcciOutputEnum
        {
            UCCI_OUTPUT_READY, UCCI_OUTPUT_POP_HASH, UCCI_OUTPUT_PV_LINE, UCCI_OUTPUT_SEARCH, UCCI_OUTPUT_POS, UCCI_OUTPUT_QUIT
        };

        [StructLayout(LayoutKind.Explicit)]
        public struct Union4
        {
            [FieldOffset(0)]
            public long llTIme;
            [FieldOffset(0)]
            public int nNodes;
            [FieldOffset(0)]
            public int nPvDepth;
            [FieldOffset(0)]
            public int vl;
            [FieldOffset(0)]
            public uint dwPopPvLineMoveStr;
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct Union5
        {
            [FieldOffset(0)]
            public int nDepth;
            [FieldOffset(0)]
            public ushort wvl;
            [FieldOffset(0)]
            public uint dwSearchMoveStr;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct Struct5
        {
            public UcciPopHashEnum PopHash;
            // Depth->0
            public byte ucDepth; // Depth (upper boundary and lower boundary)
            public short svl;    // Score (upper boundary and lower boundary)
            public int dwPopHashMoveStr;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct Struct6
        {
            public UcciPopPvLineEnum PopPvLine;
            public Union4 U4;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct Struct7
        {
            public UcciSearchEnum Search;
            public Union5 U5;
        }

        [StructLayout(LayoutKind.Explicit, Size = 16)]
        public struct UcciOutputStruct
        {
            [FieldOffset(0)]
            public Struct5 S5;
            [FieldOffset(0)]
            public Struct6 S6;
            [FieldOffset(0)]
            public Struct7 S7;
        }

        public static void SendEngine(UcciInputEnum UcciEnum, UcciInputStruct UcciInput)
        {
            int[] nSize = new int[] { 4, Marshal.SizeOf(UcciInput) };
            IntPtr[] ptrObject = new IntPtr[] { Marshal.AllocHGlobal(nSize[0]), Marshal.AllocHGlobal(nSize[1]) };
            Marshal.WriteInt32(ptrObject[0], 0, (int)UcciEnum);
            Marshal.StructureToPtr(UcciInput, ptrObject[1], false);
            ALPHA7_Send(ptrObject[0], ptrObject[1]);
            Marshal.FreeHGlobal(ptrObject[0]);
            Marshal.FreeHGlobal(ptrObject[1]);
        }

        public static void ReceiveEngine(ref UcciOutputEnum UcciEnum, ref UcciOutputStruct UcciOutput)
        {
            int[] nSize = new int[] { 4, Marshal.SizeOf(UcciOutput) };
            IntPtr[] ptrObject = new IntPtr[] { Marshal.AllocHGlobal(nSize[0]), Marshal.AllocHGlobal(nSize[1]) };
            Marshal.StructureToPtr(UcciOutput, ptrObject[1], false);
            ALPHA7_Receive(ptrObject[0], ptrObject[1]);
            UcciEnum = (UcciOutputEnum)Marshal.ReadInt32(ptrObject[0]);
            UcciOutput = (UcciOutputStruct)Marshal.PtrToStructure(ptrObject[1], typeof(UcciOutputStruct));
            Marshal.FreeHGlobal(ptrObject[0]);
            Marshal.FreeHGlobal(ptrObject[1]);
        }

        public static void LoadEngine(string szLibEvalFile, string szBookFile)
        {
            ALPHA7_Init(szLibEvalFile, szBookFile);
        }

        public static void UnLoadEngine()
        {
            ALPHA7_Quit();
            GC.Collect();
        }

        [DllImport("alpha.dll", EntryPoint = "_ALPHA7_Init@8", CharSet = CharSet.Ansi)]
        private static extern void ALPHA7_Init(string szLibEvalFile, string szBookFile);
        [DllImport("alpha.dll", EntryPoint = "_ALPHA7_Quit@0")]
        private static extern void ALPHA7_Quit();
        [DllImport("alpha.dll", EntryPoint = "_ALPHA7_Send@8")]
        private static extern void ALPHA7_Send(IntPtr pUcciEnum, IntPtr pUcciInput);
        [DllImport("alpha.dll", EntryPoint = "_ALPHA7_Receive@8")]
        public static extern void ALPHA7_Receive(IntPtr pUcciEnum, IntPtr pUcciOutput);
    }
}