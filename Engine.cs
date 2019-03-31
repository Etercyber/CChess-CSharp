using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace Chess
{
    public partial class CChess
    {
        public void SetEngineLevel(int depth)
        {
            lock (this)
            {
                Engine_nDepth = depth;
                EventCallBack(GameChangedEnum.ENGINE_LEVEL_CHANGED, new object[] { Level2Chin(Engine_nDepth), Engine_nDepth });
            }
        }

        public int GetEngineLevel()
        {
            lock (this)
            {
                return Engine_nDepth;
            }
        }

        public void SetEnginePlayer(bool red, bool black)
        {
            lock (this)
            {
                Engine_bRed = red;
                Engine_bBlack = black;
                //Game_bStart = red;
            }
            EventCallBack(GameChangedEnum.ENGINE_PLAYER_MODE,
                new object[] { Engine_bRed, Engine_bBlack });
            if ((Game_sdCurr == 0 && Engine_bRed) ||
                (Game_sdCurr == 1 && Engine_bBlack))
                EventCallBack(GameChangedEnum.ENGINE_TURN, null);
        }

        private void CloseEngine()
        {
            if (Engine_nStatus != IDLE_UNLOAD)
            {
                Engine_nStatus = IDLE_UNLOAD;
                // Break while in a death loop!
                Game_sdCurr = 2;
                // Raise exception!
                Engine_hThread.Abort();
                IdleEngine();
                // NOTE : This will automatically send a exit message
                CAlpha.UnLoadEngine();
                // Thread should quit!
                Engine_hThread.Join();
            }
        }

        private void OpenEngine()
        {
            CAlpha.UcciInputStruct UcciComm = new CAlpha.UcciInputStruct();

            // At this point, the internal message loop queue is set up
            CAlpha.LoadEngine("libevaluate.dll", "book.dat");
            Engine_nStatus = IDLE_READY;
            Engine_hThread = new Thread(EngineProc);
            // Our receive thread should born
            Engine_hThread.Start();

            // Setting the randomness of the combat method
            CAlpha.SendEngine(CAlpha.UcciInputEnum.UCCI_INPUT_ISREADY, UcciComm);

            // Trace out data
            UcciComm.S1.Option = CAlpha.UcciOptionEnum.UCCI_OPTION_DEBUG;
            UcciComm.S1.U1.bCheck = 1;
            CAlpha.SendEngine(CAlpha.UcciInputEnum.UCCI_INPUT_SETOPTION, UcciComm);

            // Random
            UcciComm.S1.Option = CAlpha.UcciOptionEnum.UCCI_OPTION_RANDOMNESS;
            UcciComm.S1.U1.Grade = CAlpha.UcciGradeEnum.UCCI_GRADE_LARGE;
            CAlpha.SendEngine(CAlpha.UcciInputEnum.UCCI_INPUT_SETOPTION, UcciComm);
        }

        public void EngineBusy(bool bBusy)
        {
            // Just lazy, heh
        }

        public void EngineLoaded(bool bLoaded)
        {
            // Just lazy, heh
        }

        private void AppendThink()
        {

        }

        public void IdleEngine()
        {
            CAlpha.UcciInputStruct UcciComm = new CAlpha.UcciInputStruct();
            CAlpha.SendEngine(CAlpha.UcciInputEnum.UCCI_INPUT_STOP, UcciComm);
        }

        public bool EngineWhileThink(int sdPlayer)
        {
            if (Engine_bRed && sdPlayer == 0)
                return true;
            else if (Engine_bBlack && sdPlayer == 1)
                return true;
            else return false;
        }

        public void EngineTurn()
        {
            string strFen = CChessAPI.CChessBoard2Fen(Game_pos);
            CAlpha.UcciInputStruct UcciComm = new CAlpha.UcciInputStruct();
            CAlpha.UcciInputEnum UcciEnum = CAlpha.UcciInputEnum.UCCI_INPUT_POSITION;

            UcciComm.S2.szFenStr = Marshal.StringToHGlobalAnsi(strFen);
            UcciComm.S2.nMoveNum = 0;
            CAlpha.SendEngine(UcciEnum, UcciComm);

            UcciEnum = CAlpha.UcciInputEnum.UCCI_INPUT_GO;
            UcciComm.S4.Go = CAlpha.UcciGoEnum.UCCI_GO_NODES;
            UcciComm.S4.bDraw = 0;
            UcciComm.S4.bPonder = 1;

            // There - set the think depth here
            UcciComm.S4.U2.nNodes = (int)Math.Pow(6, 2 - Engine_nDepth);

            CAlpha.SendEngine(UcciEnum, UcciComm);
        }

        private void EngineProc()
        {
            CAlpha.UcciOutputEnum UcciEnum = new CAlpha.UcciOutputEnum();
            CAlpha.UcciOutputStruct UcciOutput = new CAlpha.UcciOutputStruct();

            // Check the engine should quit
            while (Engine_nStatus != IDLE_UNLOAD)
            {
                CAlpha.ReceiveEngine(ref UcciEnum, ref UcciOutput);
                switch (UcciEnum)
                {
                    case CAlpha.UcciOutputEnum.UCCI_OUTPUT_READY:
                        EventCallBack(GameChangedEnum.ENGINE_READY, null);
                        break;
                    case CAlpha.UcciOutputEnum.UCCI_OUTPUT_POP_HASH:
                        break;
                    case CAlpha.UcciOutputEnum.UCCI_OUTPUT_PV_LINE:
                        EventCallBack(GameChangedEnum.ENGINE_DEBUG2,
                            new object[] { UcciOutput.S6.PopPvLine,
                                    UcciOutput.S6.U4 });
                        break;
                    case CAlpha.UcciOutputEnum.UCCI_OUTPUT_POS:
                        break;
                    case CAlpha.UcciOutputEnum.UCCI_OUTPUT_SEARCH:
                        switch (UcciOutput.S7.Search)
                        {
                            case CAlpha.UcciSearchEnum.UCCI_SEARCH_BEST_MOVE:
                                Engine_nStatus = IDLE_READY;
                                EventCallBack(GameChangedEnum.ENGINE_THINK,
                            new object[] { CChessAPI.CChessSearchMoveStr2Move(UcciOutput.S7.U5.dwSearchMoveStr) });
                                break;
                            case CAlpha.UcciSearchEnum.UCCI_SEARCH_DEPTH:
                                EventCallBack(GameChangedEnum.ENGINE_DEBUG,
                                    new object[] { CAlpha.UcciSearchEnum.UCCI_SEARCH_DEPTH,
                                    UcciOutput.S7.U5.nDepth.ToString() });
                                break;
                            case CAlpha.UcciSearchEnum.UCCI_SEARCH_SCORE:
                                EventCallBack(GameChangedEnum.ENGINE_DEBUG,
                                    new object[] { CAlpha.UcciSearchEnum.UCCI_SEARCH_SCORE,
                                    UcciOutput.S7.U5.wvl.ToString() });
                                break;
                            case CAlpha.UcciSearchEnum.UCCI_SEARCH_CURRMOVE:
                                int Move = CChessAPI.CChessSearchMoveStr2Move(UcciOutput.S7.U5.dwSearchMoveStr);
                                //Engine_nStatus = BUSY_THINK;
                                EventCallBack(GameChangedEnum.ENGINE_DEBUG,
                                    new object[] { CAlpha.UcciSearchEnum.UCCI_SEARCH_CURRMOVE,
                                    CChessAPI.CChessMove2Chin(Game_pos, Move) });
                                break;
                            case CAlpha.UcciSearchEnum.UCCI_SEARCH_NO_BEST_MOVE:
                                Game_bStart = false;
                                break;
                            case CAlpha.UcciSearchEnum.UCCI_SEARCH_PONDER:
                            case CAlpha.UcciSearchEnum.UCCI_SEARCH_PV:
                            case CAlpha.UcciSearchEnum.UCCI_SEARCH_RESIGN:
                            case CAlpha.UcciSearchEnum.UCCI_SEARCH_DRAW:
                            default:
                                break;
                        }
                        break;
                    case CAlpha.UcciOutputEnum.UCCI_OUTPUT_QUIT:
                        EventCallBack(GameChangedEnum.ENGINE_READY, null);
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
