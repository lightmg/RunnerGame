using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Game.Helpers;
using Game.Models;
using Game.Models.Enemies;
using Game.Models.Enemies.Behavior;
using Game.Models.Player;
using Game.Rendering;
using Game.Rendering.Renderers;

namespace Game
{
    public static class Program
    {
        private const int BetweenTicksDelay = 30;
        private static readonly GameForm mainForm = new GameForm();
        private static bool isDebugMode;
        private static readonly GameRenderer renderer;

        private static readonly KeyboardConfigurationController keyboardController =
            new KeyboardConfigurationController(Path.Combine(PathHelpers.RootPath, "keyboard_conf"));

        static Program()
        {
            var renderersSet = GameRendererSettingsLoader.CreateDebugRenderersSet();
            renderersSet.DefaultRenderer =
                new DefaultGameObjectRenderer(DrawingHelpers.CreateSquare(20, 20, Color.Chartreuse));
            renderer = new GameRenderer(new PointF(0, mainForm.Size.Height - 20), renderersSet);
        }

        [STAThread]
        public static void Main(params string[] commandLineArgs)
        {
            isDebugMode = commandLineArgs.Contains("-debug");
            var game = new GameModel(
                new EnemyFactory(
                    new GameObjectSize {Width = 15, Height = 15},
                    IEnemyBehavior.CreatorOf<FlyingEnemyBehavior>(),
                    RunningEnemyBehavior.Creator(0),
                    RunningEnemyBehavior.Creator(1),
                    RunningEnemyBehavior.Creator(1.5),
                    RunningEnemyBehavior.Creator(2)),
                new GameFieldSize(
                    mainForm.GameFieldSize.Height,
                    mainForm.GameFieldSize.Width),
                new Dictionary<PlayerState, GameObjectSize>
                {
                    {PlayerState.OnGround, new GameObjectSize {Height = 100, Width = 100}},
                    {PlayerState.Crouching, new GameObjectSize {Height = 10, Width = 20}},
                    {PlayerState.Jumping, new GameObjectSize {Height = 23, Width = 20}}
                }
            );

            mainForm.KeyDown += (sender, args) => keyboardController.KeyPressed(args.KeyCode);
            mainForm.KeyUp += (sender, args) => keyboardController.KeyReleased(args.KeyCode);

            using var timer = new Timer {Interval = BetweenTicksDelay};
            timer.Tick += (sender, args) =>
            {
                game.Tick(keyboardController.ActiveAction);
                UpdateGameField(game);
            };

            timer.Start();
            Application.Run(mainForm);
            timer.Stop();
        }

        private static void UpdateGameField(GameModel game)
        {
            mainForm.Draw(renderer.Render(game, isDebugMode));
            mainForm.SetGameScores(game.State.Score);
            mainForm.SetGameState(game.State.PlayerAlive ? $"HP: {game.State.PlayerHealth}" : "Press E to start");
        }
    }
}