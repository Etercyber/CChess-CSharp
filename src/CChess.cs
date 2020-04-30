using System;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace Chess
{
    public delegate void UIThreadDelegate(GameChangedEnum Enum, object[] Arg);

    public enum GameChangedEnum
    {
        LIST_MOVE_CHANGED, ENGINE_LEVEL_CHANGED, ENGINE_PLAYER_MODE, LIST_MOVE_FLUSH, BOARD_FLUSH, GAME_OVER,
        ENGINE_READY, ENGINE_NOT_READY, ENGINE_THINK, ENGINE_DEBUG, ENGINE_DEBUG2, ENGINE_TURN
    }

    public class GameChangedEventArgs : EventArgs
    {
        public GameChangedEnum Enum;
        public object[] Arg;
    }

    public partial class CChess
    {
        public CChess()
        {
            Game_posSelected = new bool[256];
            Game_posMoved = new short[4];
            Game_posClicked = new short[4];
            ChessBlack_ImageList = new Image[]
            {
                Properties.Resources.BK,                Properties.Resources.BA,                Properties.Resources.BB,                Properties.Resources.BN,
                Properties.Resources.BR,                Properties.Resources.BC,                Properties.Resources.BP,                Properties.Resources.OO,
                Properties.Resources.BKS,                Properties.Resources.BAS,                Properties.Resources.BBS,                Properties.Resources.BNS,
                Properties.Resources.BRS,                Properties.Resources.BCS,                Properties.Resources.BPS,                Properties.Resources.OOS,
          };
            ChessRed_ImageList = new Image[]
            {
                Properties.Resources.RK,                Properties.Resources.RA,                Properties.Resources.RB,                Properties.Resources.RN,
                Properties.Resources.RR,                Properties.Resources.RC,                Properties.Resources.RP,                Properties.Resources.OO,
                Properties.Resources.RKS,                Properties.Resources.RAS,                Properties.Resources.RBS,                Properties.Resources.RNS,
                Properties.Resources.RRS,                Properties.Resources.RCS,                Properties.Resources.RPS,                Properties.Resources.OOS,
              };
            Game_pos = new CChessAPI.PositionStruct();
            Game_mvMove = new int[CChessAPI.MAX_MOVE];
            Game_szDraw = new string[] { "黑方长打作负", "双方不变作和。", "红方长打作负" };
            Engine_nDepth = 5;
            Engine_bRed = false;
            Engine_bBlack = true;
            Engine_nStatus = IDLE_UNLOAD;
        }

        public bool Init()
        {
            try
            {
                CChessAPI.CChessInit();
                CChessAPI.CChessPromotion(0);
                SetEngineLevel(-4);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void Quit()
        {
            CloseEngine();
        }

        public bool AssertRank(Point Rank)
        {
            if (Rank.X < 0 || Rank.Y < 0)
                return false;
            if (Rank.X > 8 || Rank.Y > 9)
                return false;
            return true;
        }

        private short GetCurrentPc(int id)
        {
            int ClickedPos = Game_posClicked[id];
            return ClickedPos >= 0 ? Game_pos.ucpcSquares[ClickedPos] : (short)0;
        }

        private bool CurrentSquareStatusAvailable()
        {
            return Game_posClicked[0] != -1 && Game_posClicked[1] != -1;
        }

        public void SquareClickDown(short sq)
        {
            short inc = 0;
            short pc = Game_pos.ucpcSquares[sq];

            if ((pc > 0 && pc < 32 && Game_pos.sdPlayer == 0) ||
                (pc >= 32 && Game_pos.sdPlayer == 1))
            {
                if (Game_posClicked[0] >= 0)
                    Game_posSelected[Game_posClicked[0]] = false;
            }
            else if (Game_posClicked[0] >= 0)
            {
                inc++;
            }
            Game_posClicked[inc] = sq;
            if (Game_posClicked[0] >= 0)
                Game_posSelected[Game_posClicked[0]] = true;
            if (Game_posClicked[1] >= 0)
                Game_posSelected[Game_posClicked[1]] = true;
            EventCallBack(GameChangedEnum.BOARD_FLUSH, null);
        }

        private void AddPc(short sq, short pc)
        {
            CChessAPI.CChessAddPiece(ref Game_pos, sq, pc);
        }

        public void SquareClickUp(short sq, short pc)
        {
                short pc1 = GetCurrentPc(0), pc2 = GetCurrentPc(1);

                if (CChessAPI.PlayerEquare(pc1, Game_pos.sdPlayer) ||
                    EngineWhileThink(Game_pos.sdPlayer) || pc1 == 0)
                {
                    if (!IsHotSelect(Game_posClicked[0]) && Game_posClicked[0] >= 0)
                    {
                        Game_posSelected[Game_posClicked[0]] = false;
                    }
                    if (!IsHotSelect(Game_posClicked[1]) && Game_posClicked[1] >= 0)
                    {
                        Game_posSelected[Game_posClicked[1]] = false;
                    }
                    ClearNewSquareStatus();
                }
                if (CurrentSquareStatusAvailable())
                {
                    int mv = CChessAPI.Move(Game_posClicked[0], Game_posClicked[1]);
                    if (CChessAPI.PlayerEquare(pc1, Engine_bRed ? 1 : 0))
                    {
                        if (!IsHotSelect(Game_posClicked[0]) && Game_posClicked[1] >= 0)
                        {
                            Game_posSelected[Game_posClicked[0]] = false;
                            Game_posSelected[Game_posClicked[1]] = false;
                        }
                    }
                    else if (!IsHotSelect(Game_posClicked[1]) && Game_posClicked[0] >= 0)
                    {
                        Game_posSelected[Game_posClicked[0]] = false;
                        Game_posSelected[Game_posClicked[1]] = false;
                    }
                    else if (IsHotSelect(Game_posClicked[1]))
                    {
                        Game_posSelected[Game_posClicked[0]] = false;
                    }
                    ReplaceSquareStatus();
                    ClearNewSquareStatus();
                    EventCallBack(GameChangedEnum.ENGINE_THINK, new object[] { mv });
                }
            EventCallBack(GameChangedEnum.BOARD_FLUSH, null);
        }

        private void ReplaceSquareStatus()
        {
            Game_posClicked[2] = Game_posClicked[1];
            Game_posClicked[3] = Game_posClicked[0];
        }

        private void ClearNewSquareStatus()
        {
            Game_posClicked[0] = Game_posClicked[1] = -1;
        }

        public Image GenerateChessImage(int piece, bool team, bool selected = false)
        {
            if (team)
                return ChessRed_ImageList[piece + (selected ? 8 : 0)];
            else
                return ChessBlack_ImageList[piece + (selected ? 8 : 0)];
        }

        public Point GetSquareFromBoard(Point location)
        {
            return new Point(
                location.X == 0 ? 0 : (int)((location.X - DrawPos.X) / (double)ChessSize.Width),
                location.Y == 0 ? 0 : (int)((location.Y - DrawPos.Y) / (double)ChessSize.Height));
        }

        public void SetControl(Control ctrl)
        {
            Size ClipScreen;
            ChessControl = ctrl;
            ClipScreen = ctrl.Size;
            ctrl.BackgroundImage = new Bitmap
                (ClipScreen.Width, ClipScreen.Height);
            if (ctrl.Size.Height < ctrl.Size.Width + ChessSize.Width)
                ClipScreen.Width = (int)(ClipScreen.Height / 10.0f * 9);
            else
                ClipScreen.Height = (int)(ClipScreen.Width / 9.0f * 10);
            ChessBoardSize = ClipScreen;
            ChessSize.Width = ClipScreen.Width / 9.0f - 0.2f;
            ChessSize.Height = ClipScreen.Height / 10.0f - 0.2f;
            DrawPos.X = (ctrl.Width - ChessBoardSize.Width) / 2;
            DrawPos.Y = (ctrl.Height - ChessBoardSize.Height) / 2;
        }

        private void DrawSquare(Graphics g, short sq, bool mate = false)
        {
            short x = (short)(sq % 16), y = (short)(sq / 16);
            short pc = Game_pos.ucpcSquares[sq], PictureID;
            PictureID = CChessAPI.PieceType(pc);
            Image ChessImage = GenerateChessImage(PictureID, pc < 32, Game_posSelected[sq]);
            g.DrawImage(ChessImage, (int)(DrawPos.X + (x - 3) * ChessSize.Width),
                (int)(DrawPos.Y + (y - 3) * ChessSize.Height), (int)ChessSize.Width, (int)ChessSize.Height);
        }

        public void BoardFlush()
        {
            Graphics GfxControl = ChessControl.CreateGraphics();
            Graphics GfxBuffer = Graphics.FromImage(ChessControl.BackgroundImage);

            GfxBuffer.SmoothingMode = SmoothingMode.HighQuality;
            GfxControl.SmoothingMode = SmoothingMode.None;
            GfxBuffer.CompositingQuality = CompositingQuality.HighQuality;
            GfxControl.CompositingQuality = CompositingQuality.Default;
            GfxBuffer.InterpolationMode = InterpolationMode.HighQualityBicubic;
            GfxControl.InterpolationMode = InterpolationMode.NearestNeighbor;

            GfxBuffer.DrawImage(Properties.Resources.WOOD,
                DrawPos.X, DrawPos.Y, ChessBoardSize.Width,
                ChessBoardSize.Height);

            for (short i = 3; i < 13; i++)
                for (short j = 3; j < 12; j++)
                    DrawSquare(GfxBuffer, (short)(i * 16 + j));

            GfxControl.DrawImage(ChessControl.BackgroundImage, new Point(0, 0));

            GfxBuffer.Dispose();
            GfxControl.Dispose();
        }
    }
}
