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
        private static GameRenderer renderer;

        private static readonly KeyboardConfigurationController keyboardController =
            new KeyboardConfigurationController(Path.Combine(PathHelpers.RootPath, "keyboard_conf"));

        [STAThread]
        public static void Main(params string[] commandLineArgs)
        {
            isDebugMode = commandLineArgs.Contains("-debug");

            var playerSizesByStates = new Dictionary<PlayerState, GameObjectSize>
            {
                {PlayerState.OnGround, new GameObjectSize {Height = 100, Width = 40}},
                {PlayerState.Jumping, new GameObjectSize {Height = 100, Width = 40}},
                {PlayerState.Crouching, new GameObjectSize {Height = 40, Width = 100}},
            };
            var enemySizesByBehavior = new Dictionary<Type, GameObjectSize>
            {
                {typeof(FlyingEnemyBehavior), new GameObjectSize {Height = 40, Width = 40}},
                {typeof(RunningEnemyBehavior), new GameObjectSize {Height = 40, Width = 40}}
            };
            var renderersLoader = new GameRendererSettingsLoader(Path.Combine(PathHelpers.RootPath, "Resources"),
                playerSizesByStates,
                enemySizesByBehavior);
            var renderersSet = renderersLoader.Load();
            renderersSet.DefaultRenderer =
                new DefaultGameObjectRenderer(DrawingHelpers.CreateSquare(20, 20, Color.Chartreuse));
            renderer = new GameRenderer(new PointF(0, mainForm.Size.Height), renderersSet);

            var game = new GameModel(
                new EnemyFactory(enemySizesByBehavior,
                    IEnemyBehavior.CreatorOf<FlyingEnemyBehavior>(),
                    RunningEnemyBehavior.Creator(0),
                    RunningEnemyBehavior.Creator(1),
                    RunningEnemyBehavior.Creator(1.5),
                    RunningEnemyBehavior.Creator(2)),
                new GameFieldSize(
                    mainForm.GameFieldSize.Height,
                    mainForm.GameFieldSize.Width), playerSizesByStates);

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