using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Game
{
    public class GameForm : Form
    {
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

            gamePictureBox = new PictureBox
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent,
                BorderStyle = BorderStyle.None,
                Margin = Padding.Empty
            };

            floorPictureBox = new PictureBox
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent,
                BorderStyle = BorderStyle.None,
                Margin = Padding.Empty
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

            var table = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent,
                ForeColor = Color.Transparent,
                BorderStyle = BorderStyle.None,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None,
            };

            table.RowStyles.Clear();
            table.RowStyles.Add(new RowStyle(SizeType.Absolute, 25));
            table.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            table.RowStyles.Add(new RowStyle(SizeType.Absolute, 150));
            table.ColumnStyles.Clear();
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));
            table.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 3));
            table.Controls.Add(gameNameLabel, 0, 0);
            table.Controls.Add(scoresLabel, 1, 0);
            table.Controls.Add(gameStateLabel, 2, 0);
            table.Controls.Add(closeButtonPlaceholder, 3, 0);
            table.Controls.Add(gamePictureBox, 0, 1);
            table.SetColumnSpan(gamePictureBox, 4);
            table.Controls.Add(floorPictureBox, 0, 2);
            table.SetColumnSpan(floorPictureBox, 4);

            Controls.Add(table);

            Width = 800;
            Height = 600;
            MaximizeBox = false;
            FormBorderStyle = FormBorderStyle.None;

            base.BackColor = Color.DarkGray;
            Draw(Enumerable.Empty<ImageRenderInfo>());
        }

        public void Draw(IEnumerable<ImageRenderInfo> toDraw)
        {
            var canvas = new Bitmap(GameFieldSize.Width, GameFieldSize.Height);
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

        public void SetBackgroundImage(Image image)
        {
            gamePictureBox.BackgroundImage = image;
        }
    }
}