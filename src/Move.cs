using System;
using System.Runtime.InteropServices;

namespace Chess
{
    public partial class CChess
    {
        public void MoveCut()
        {
            Game_nMaxMove = Game_nCurrMove;
        }

        public bool MoveInRange()
        {
            if (Game_nMaxMove < CChessAPI.MAX_MOVE - 1)
                return true;
            GameOver("超过最大回合数限制");
            return false;
        }

        public void MoveStatus(int sta)
        {
            if ((sta & CChessAPI.MOVE_MATE) == 0)
            {
                if ((sta & CChessAPI.MOVE_DRAW) != 0)
                {
                    Game_nResult = RESULT_DRAW;
                    GameOver("自然作和");
                }
                else if ((sta & CChessAPI.MOVE_PERPETUAL) != 0)
                {
                    Game_nResult = RESULT_DRAW;

                    if ((sta & CChessAPI.MOVE_PERPETUAL_WIN) != 0)
                        Game_nResult = (short)(Game_nResult + 1 - Game_sdCurr * 2);
                    else if ((sta & CChessAPI.MOVE_PERPETUAL_LOSS) != 0)
                        Game_nResult = (short)(Game_nResult - 1 - Game_sdCurr * 2);

                    if (Game_nResult == 2)
                        GameOver(Game_szDraw[Game_nResult - 1]);
                }
            }
            else
            {
                Game_nResult = Game_sdCurr == 1 ?
                    RESULT_REDWIN : RESULT_BLACKWIN;
                GameOver(Game_sdCurr == 1 ? "红胜" : "黑胜");
            }
        }

        public bool AddMove(int mv, bool bEngine = false)
        {
            int nMoveStatus = 0;
            IntPtr ptrPos = Marshal.AllocHGlobal(29024);
            IntPtr ptrMoveStatus = Marshal.AllocHGlobal(4);

            Marshal.StructureToPtr(Game_pos, ptrPos, false);
            Marshal.StructureToPtr(nMoveStatus, ptrMoveStatus, false);

            if (CChessAPI.CChessTryMove(ptrPos, ptrMoveStatus, mv) == 0)
                return false;
            CChessAPI.CChessUndoMove(ptrPos);

            if (Game_nCurrMove < Game_nMaxMove)
                MoveCut();
            if (!MoveInRange())
                return false;

            Game_nMaxMove++;
            Game_mvMove[Game_nMaxMove] = mv;
            nMoveStatus = (int)Marshal.PtrToStructure(
                ptrMoveStatus, typeof(int));

            EventCallBack(GameChangedEnum.LIST_MOVE_CHANGED,
                new object[] { CChessAPI.CChessMove2Chin(Game_pos, mv) });
            MoveForward();

            if (Game_sdCurr == 0 && Engine_bRed)
                EventCallBack(GameChangedEnum.ENGINE_TURN, null);
            else if (Game_sdCurr == 1 && Engine_bBlack)
                EventCallBack(GameChangedEnum.ENGINE_TURN, null);

            MoveHotSelect(mv);
            MoveStatus(nMoveStatus);
            EventCallBack(GameChangedEnum.BOARD_FLUSH, null);
            Win32API.PlaySound("sound.wav", IntPtr.Zero, 0x20000);
            Marshal.FreeHGlobal(ptrPos);
            Marshal.FreeHGlobal(ptrMoveStatus);

            return true;
        }

        private bool IsHotSelect(short sq)
        {
            if (sq < 0)
                return false;
            else if (Game_posMoved[0] == sq)
                return true;
            else if (Game_posMoved[1] == sq)
                return true;
            else
                return false;
        }

        private void MoveHotSelect(int mv)
        {
            if (Game_posMoved[0] != -1 && Game_posMoved[1] != -1)
            {
                Game_posSelected[Game_posMoved[1]] = false;
                Game_posSelected[Game_posMoved[0]] = false;
            }
            Game_posMoved[0] = CChessAPI.Src(mv);
            Game_posMoved[1] = CChessAPI.Dst(mv);
            Game_posSelected[Game_posMoved[1]] = true;
            Game_posSelected[Game_posMoved[0]] = true;
        }

        public void MoveBack()
        {
            int nSize = Marshal.SizeOf(Game_pos);
            IntPtr ptrPos = Marshal.AllocHGlobal(nSize);

            Marshal.StructureToPtr(Game_pos, ptrPos, false);

            if (Game_nMaxMove <= 0)
                goto end;

            CChessAPI.CChessUndoMove(ptrPos);
            CChessAPI.CChessUndoMove(ptrPos);
            Game_pos = (CChessAPI.PositionStruct)Marshal.PtrToStructure(
                ptrPos, typeof(CChessAPI.PositionStruct));
            Marshal.FreeHGlobal(ptrPos);

            Game_nCurrMove -= 2;
            Game_posSelected[Game_posMoved[1]] = false;
            Game_posSelected[Game_posMoved[0]] = false;

        end:
            EventCallBack(GameChangedEnum.BOARD_FLUSH, null);
        }

        public void MoveForward(short step = 0)
        {
            int sqSrc, sqDst, mv;
            int nMoveStatus = 0;
            int nSize = Marshal.SizeOf(Game_pos);
            IntPtr ptrPos = Marshal.AllocHGlobal(nSize);

            nSize = Marshal.SizeOf(nMoveStatus);
            IntPtr ptrMoveStatus = Marshal.AllocHGlobal(nSize);
            Marshal.StructureToPtr(Game_pos, ptrPos, false);

            Game_nCurrMove++;
            Game_sdCurr = 1 - Game_sdCurr;
            mv = Game_mvMove[Game_nCurrMove];
            sqSrc = CChessAPI.Src(mv);
            sqDst = CChessAPI.Dst(mv);

            CChessAPI.CChessTryMove(ptrPos, ptrMoveStatus, mv);
            Game_pos = (CChessAPI.PositionStruct)
                Marshal.PtrToStructure(ptrPos, typeof(CChessAPI.PositionStruct));

            Marshal.FreeHGlobal(ptrPos);
            Marshal.FreeHGlobal(ptrMoveStatus);
            EventCallBack(GameChangedEnum.BOARD_FLUSH, null);
        }
    }
}
