using System;
using System.Drawing;
using System.Windows.Forms;

namespace Chess
{
    public partial class MainFrm : Form
    {
        private int nDebugStep;
        private CChess ChessBoard;
        private UIThreadDelegate UIThreadInvoke;
        public MainFrm(CChess chess)
        {
            nDebugStep = 0;
            ChessBoard = chess;
            ChessBoard.OnGameChanged += new EventHandler
                <GameChangedEventArgs>(MainFrm_GameChanged);
            UIThreadInvoke = new UIThreadDelegate(MainFrm_GameChanged2);
            InitializeComponent();
        }

        private void MainFrm_GameChanged(object sender, GameChangedEventArgs e)
        {
            Invoke(UIThreadInvoke, new object[] { e.Enum, e.Arg });
        }

        private void menuItem12_Click(object sender, EventArgs e)
        {
            new AboutBox1().ShowDialog(this);
        }

        private void menuItem4_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void MainFrm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Hide();
            ChessBoard.Quit();
        }

        private void MainFrm_GameChanged2(GameChangedEnum Enum, object[] Arg)
        {
            switch (Enum)
            {
                case GameChangedEnum.LIST_MOVE_CHANGED:
                    listBox1.SelectedIndex = listBox1.Items.Add(Arg[0]);
                    break;
                case GameChangedEnum.ENGINE_TURN:
                    ChessBoard.EngineTurn();
                    break;
                case GameChangedEnum.LIST_MOVE_FLUSH:
                    listBox1.Items.Clear();
                    richTextBox1.Clear();
                    statusBar1.Panels[0].Text = "当前着法评分：0";
                    statusBar1.Panels[1].Text = "当前算法用时：0";
                    statusBar1.Panels[2].Text = "当前搜索深度：0";
                    statusBar1.Panels[3].Text = "当前搜索节点数：0";
                    break;
                case GameChangedEnum.BOARD_FLUSH:
                    ChessBoard.BoardFlush();
                    break;
                case GameChangedEnum.GAME_OVER:
                    string szArg = Arg[0].ToString();
                    if (szArg == "") return;
                    MessageBox.Show(this, szArg, "提示");
                    if (MessageBox.Show(this, "开始新游戏？", "游戏结束",
                        MessageBoxButtons.YesNo) == DialogResult.Yes)
                        ChessBoard.NewGame(false);
                    break;
                case GameChangedEnum.ENGINE_PLAYER_MODE:
                    label4.Text = ((bool)Arg[0] ? "电脑" : "人脑");
                    label3.Text = ((bool)Arg[1] ? "电脑" : "人脑");
                    menuItem10.Checked = (bool)Arg[0];
                    pictureBox2.Image = ((bool)Arg[1] ? Properties.Resources.BLARGE
                        : Properties.Resources.BK);
                    pictureBox3.Image = ((bool)Arg[0] ? Properties.Resources.RLARGE
                        : Properties.Resources.RK);
                    break;
                case GameChangedEnum.ENGINE_LEVEL_CHANGED:
                    label5.Text = "水平：" + Arg[0].ToString();
                    break;
                case GameChangedEnum.ENGINE_THINK:
                    ChessBoard.AddMove(Convert.ToInt32(Arg[0]));
                    break;
                case GameChangedEnum.ENGINE_DEBUG:
                    switch ((CAlpha.UcciSearchEnum)Arg[0])
                    {
                        case CAlpha.UcciSearchEnum.UCCI_SEARCH_DEPTH:
                            statusBar1.Panels[2].Text = "当前搜索深度：" + Arg[1].ToString();
                            if (!checkBox1.Checked)
                                break;
                            nDebugStep = 0;
                            richTextBox1.Clear();
                            break;
                        case CAlpha.UcciSearchEnum.UCCI_SEARCH_SCORE:
                            break;
                        case CAlpha.UcciSearchEnum.UCCI_SEARCH_CURRMOVE:
                            if (!checkBox1.Checked)
                                break;
                            if (nDebugStep > 10)
                                break;
                            richTextBox1.Text += Arg[1].ToString();
                            richTextBox1.Text += " ";
                            richTextBox1.Select(richTextBox1.TextLength, 0);
                            richTextBox1.ScrollToCaret();
                            nDebugStep++;
                            break;
                    }
                    break;
                case GameChangedEnum.ENGINE_DEBUG2:
                    switch ((CAlpha.UcciPopPvLineEnum)Arg[0])
                    {
                        case CAlpha.UcciPopPvLineEnum.UCCI_POP_PV_LINE_TIME:
                            statusBar1.Panels[1].Text = "当前算法用时：" + ((CAlpha.Union4)Arg[1]).llTIme.ToString();
                            break;
                        case CAlpha.UcciPopPvLineEnum.UCCI_POP_PV_LINE_NODES:
                            statusBar1.Panels[3].Text = "当前搜索节点数：" + ((CAlpha.Union4)Arg[1]).nNodes.ToString();
                            break;
                        case CAlpha.UcciPopPvLineEnum.UCCI_POP_PV_LINE_SCORE:
                            statusBar1.Panels[0].Text = "当前着法评分：" + ((CAlpha.Union4)Arg[1]).vl.ToString();
                            break;
                        default:
                            break;
                    }
                    break;
                default:
                    break;
            }
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            short sq = 0;
            Point pt = ChessBoard.GetSquareFromBoard(e.Location);

            if (!ChessBoard.AssertRank(pt))
                return;
            sq += 3 * 16;
            sq += (short)(pt.X + 3 + pt.Y * 16);

            ChessBoard.SquareClickDown(sq);
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            short sq = 0, pc = 0;
            Point pt = ChessBoard.GetSquareFromBoard(e.Location);

            if (!ChessBoard.AssertRank(pt))
                return;
            sq += 3 * 16;
            sq += (short)(pt.X + 3 + pt.Y * 16);

            ChessBoard.SquareClickUp(sq, pc);
        }

        private void MainFrm_Load(object sender, EventArgs e)
        {
            if (ChessBoard.Init())
            {
                ChessBoard.SetControl(pictureBox1);
                ChessBoard.NewGame(false);
                menuItem9_Click(sender, e);
            }
            else
            {
                Close();
                throw new Exception();
            }
        }

        private void pictureBox1_SizeChanged(object sender, EventArgs e)
        {
            if (ChessBoard.Game_bStart)
            {
                ChessBoard.SetControl(pictureBox1);
                ChessBoard.BoardFlush();
            }
        }

        private void menuItem2_Click(object sender, EventArgs e)
        {
            ChessBoard.NewGame(false);
        }

        private void menuItem14_Click(object sender, EventArgs e)
        {
            ChessBoard.MoveBack();
        }

        private void menuItem9_Click(object sender, EventArgs e)
        {
            menuItem9.Checked = !menuItem9.Checked;
            menuItem14.Enabled = (menuItem9.Checked != menuItem10.Checked) || (!menuItem9.Checked && !menuItem10.Checked);
            ChessBoard.SetEnginePlayer(menuItem10.Checked, menuItem9.Checked);
        }

        private void menuItem10_Click(object sender, EventArgs e)
        {
            menuItem10.Checked = !menuItem10.Checked;
            menuItem14.Enabled = (menuItem9.Checked != menuItem10.Checked) || (!menuItem9.Checked && !menuItem10.Checked);
            ChessBoard.SetEnginePlayer(menuItem10.Checked, menuItem9.Checked);
        }

        private void menuItem6_Click(object sender, EventArgs e)
        {
            new LevelSetFrm(ChessBoard).ShowDialog(this);
        }

        private void menuItem17_Click(object sender, EventArgs e)
        {
            ChessBoard.IdleEngine();
        }

        private void MainFrm_Activated(object sender, EventArgs e)
        {
            if (!ChessBoard.Game_bStart)
                return;
            ChessBoard.SetControl(pictureBox1);
            ChessBoard.BoardFlush();
        }
    }
}