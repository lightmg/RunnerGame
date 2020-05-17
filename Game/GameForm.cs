using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Game
{
    public class GameForm : Form
    {
        // public readonly PictureBox PictureBox;
        private readonly Label scoresLabel;
        private readonly Label gameStateLabel;
        private readonly PictureBox gamePictureBox;
        private readonly PictureBox floorPictureBox;

        public Size GameFieldSize => gamePictureBox.Size;
        public Size FloorSize => floorPictureBox.Size;

        public GameForm()
        {
            scoresLabel = new Label
            {
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Text = "Score: ",
                BackColor = Color.Transparent,
                BorderStyle = BorderStyle.None,
                FlatStyle = FlatStyle.Flat
            };

            gameStateLabel = new Label
            {
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Text = "Paused",
                BackColor = Color.Transparent,
                BorderStyle = BorderStyle.None,
                FlatStyle = FlatStyle.Flat
            };

            var closeButtonPlaceholder = new Label
            {
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Red,
                ForeColor = Color.Black,
                Font = new Font(DefaultFont, FontStyle.Bold),
                BorderStyle = BorderStyle.FixedSingle,

                Text = "X"
            };
            closeButtonPlaceholder.MouseClick += (sender, args) => Close();

            var gameNameLabel = new Label
            {
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Text = "Just Another Runner by @lightmg",
                BackColor = Color.Transparent,
                BorderStyle = BorderStyle.None,
                FlatStyle = FlatStyle.Flat
            };
            
            var topTable = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                BackColor = Color.Transparent,
                ForeColor = Color.Transparent,
                Height = 20
            };

            topTable.RowStyles.Clear();
            topTable.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            topTable.ColumnStyles.Clear();
            topTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));
            topTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));
            topTable.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            topTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 3));

            topTable.Controls.Add(gameNameLabel, 0, 0);
            topTable.Controls.Add(scoresLabel, 1, 0);
            topTable.Controls.Add(gameStateLabel, 2, 0);
            topTable.Controls.Add(closeButtonPlaceholder, 3, 0);

            Controls.Add(topTable);

            floorPictureBox = new PictureBox
            {
                Dock = DockStyle.Bottom,
                Height = 100,
            };
            Controls.Add(floorPictureBox);

            gamePictureBox = new PictureBox
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Black,
                Margin = new Padding{Bottom = 100}
            };
            Controls.Add(gamePictureBox);

            Width = 800;
            Height = 600;
            MaximizeBox = false;
            FormBorderStyle = FormBorderStyle.None;

            base.BackColor = Color.DarkGray;
            Draw(Enumerable.Empty<ImageRenderInfo>());
        }

        public void Draw(IEnumerable<ImageRenderInfo> toDraw)
        {
            var canvas = new Bitmap(Width, Height);
            using var graphics = Graphics.FromImage(canvas);
            foreach (var imageInfo in toDraw)
                graphics.DrawImage(imageInfo.Image, imageInfo.Point);
            gamePictureBox.Image = canvas;
            gamePictureBox.Refresh();
        }

        public void SetGameState(string state)
        {
            gameStateLabel.Text = state;
        }

        public void SetGameScores(ulong score)
        {
            scoresLabel.Text = $"Score: {score.ToString()}";
        }

        public void SetFloorImage(Image image)
        {
            floorPictureBox.Image = (Image) image.Clone();
        }
    }
}