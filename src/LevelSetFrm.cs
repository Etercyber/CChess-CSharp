using System;
using System.Windows.Forms;

namespace Chess
{
    public partial class LevelSetFrm : Form
    {
        private CChess ChessBoard;
        private RadioButton[] RadioButtons;
        public LevelSetFrm(CChess chess)
        {
            ChessBoard = chess;
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            foreach (var item in RadioButtons)
            {
                if (item.Checked)
                {
                    ChessBoard.SetEngineLevel(Convert.ToInt32(item.Tag));
                    break;
                }
            }
        }

        private void LevelSetFrm_Load(object sender, EventArgs e)
        {
            int nDepth = ChessBoard.GetEngineLevel();
            RadioButtons = new RadioButton[] { 
                radioButton1, radioButton2, radioButton3,
                radioButton4, radioButton5, radioButton6 };

            foreach (var item in RadioButtons)
            {
                if (Convert.ToInt32(item.Tag) == nDepth)
                {
                    item.Checked = true;
                    break;
                }
            }
        }
    }
}
