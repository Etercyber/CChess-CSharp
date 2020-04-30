using System;
using System.Runtime.InteropServices;

namespace Chess
{
    public partial class CChess
    {
        private void EventCallBack(GameChangedEnum e, object[] args)
        {
            if (OnGameChanged == null)
                return;
            OnGameChanged.Invoke(this,
                new GameChangedEventArgs()
                { Enum = e, Arg = args });
        }

        public string Level2Chin(int depth)
        {
            if (depth == -1)
                return "小白";
            else if (depth == -2)
                return "新手";
            else if (depth == -4)
                return "业余";
            else if (depth == -6)
                return "初级";
            else if (depth == -7)
                return "中级";
            else
                return "高级";
        }

        public void NewGame(bool bClear)
        {
            RestartGame(bClear);
            EventCallBack(GameChangedEnum.BOARD_FLUSH, null);
        }

        private void ClearBoard()
        {
            for (int i = 0; i < 256; i++)
                Game_posSelected[i] = false;
            for (int i = 0; i < 4; i++)
                Game_posMoved[i] = Game_posClicked[i] = -1;
        }

        private void RestartGame(bool bClear)
        {
            int nSize = Marshal.SizeOf(Game_pos);
            IntPtr ptrPos = Marshal.AllocHGlobal(nSize);
            string Game_szFen = CChessAPI.CCHESS_START_FEN + " - - 0 1";

            EventCallBack(GameChangedEnum.LIST_MOVE_FLUSH, null);

            CloseEngine();
            OpenEngine();
            ClearBoard();

            Game_sdCurr = 0;
            Game_nCurrMove = 0;
            Game_bStart = true;
            SetEnginePlayer(false, true);
            Game_nResult = RESULT_UNKNOWN;
            MoveCut();
            Marshal.StructureToPtr(Game_pos, ptrPos, false);

            if (bClear)
                CChessAPI.CChessClearBoard(ptrPos);
            else
                CChessAPI.CChessFen2Board(ptrPos, Game_szFen);

            Game_pos = (CChessAPI.PositionStruct)
                Marshal.PtrToStructure(ptrPos, typeof(CChessAPI.PositionStruct));

            if ((Game_sdCurr == 0 && Engine_bRed) ||
                (Game_sdCurr == 1 && Engine_bBlack))

            EventCallBack(GameChangedEnum.ENGINE_TURN, null);
            Marshal.FreeHGlobal(ptrPos);
        }

        public void GameOver(string szPrompt)
        {
            Game_bStart = false;
            EventCallBack(GameChangedEnum.GAME_OVER, new object[] { szPrompt });
            SetEnginePlayer(false, true);
        }
    }
}
