using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Drawing;

namespace Chess
{
    public partial class CChess
    {
        // Global constant - the result of a chess game
        public const short RESULT_UNKNOWN = 0;
        public const short RESULT_REDWIN = 1;
        public const short RESULT_DRAW = 2;
        public const short RESULT_BLACKWIN = 3;

        // Global variable -- chess
        public bool Game_bStart;
        public short Game_nResult;
        public int Game_sdCurr;
        public int[] Game_mvMove;
        public string Game_szFen;
        private CChessAPI.PositionStruct Game_pos;
        private bool[] Game_posSelected;
        private short[] Game_posClicked;
        private short[] Game_posMoved;
        public int Game_nCurrMove;
        public int Game_nMaxMove;

        private Control ChessControl;
        private Point DrawPos;

        // Current board
        private SizeF ChessSize;
        private Size ChessBoardSize;
        private Image[] ChessBlack_ImageList;
        private Image[] ChessRed_ImageList;

        // Startup situation FEN
        public event EventHandler<GameChangedEventArgs> OnGameChanged;

        // Global constant - engine state
        public const short IDLE_UNLOAD = 0;
        public const short IDLE_READY = 1;
        public const short IDLE_REST = 2;
        public const short IDLE_PONDER = 3;
        public const short BUSY_WAIT = 4;
        public const short BUSY_ANALYZE = 5;
        public const short BUSY_THINK = 6;
        public const short BUSY_PONDER = 7;

        private int Engine_nStatus;
        private bool Engine_bRed;
        private bool Engine_bBlack;
        private int Engine_nDepth;
        private Thread Engine_hThread;
        private string[] Game_szDraw;
    }
}
