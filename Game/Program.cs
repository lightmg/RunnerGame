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

namespace Game
{
    public static class Program
    {
        private const int BetweenTicksDelay = 30;
        private static readonly GameForm gameForm = new GameForm();
        private static bool isDebugMode;
        private static GameRenderer gameRenderer;
        private static TexturesRepository texturesRepository;

        private static readonly Dictionary<PlayerState, GameObjectSize> playerSizesByStates =
            new Dictionary<PlayerState, GameObjectSize>
            {
                {PlayerState.OnGround, new GameObjectSize {Height = 100, Width = 40}},
                {PlayerState.Jumping, new GameObjectSize {Height = 100, Width = 40}},
                {PlayerState.Crouching, new GameObjectSize {Height = 40, Width = 100}}
            };

        private static readonly Dictionary<string, GameObjectSize> enemiesSizesByBehavior =
            new Dictionary<string, GameObjectSize>
            {
                {nameof(FlyingEnemyBehavior), new GameObjectSize {Height = 40, Width = 60}},
                {nameof(RunningEnemyBehavior), new GameObjectSize {Height = 40, Width = 40}},
                {nameof(StandingEnemyBehavior), new GameObjectSize {Height = 70, Width = 30}}
            };

        private static readonly KeyboardConfigurationController keyboardController =
            new KeyboardConfigurationController(Path.Combine(PathHelpers.RootPath, "keyboard_conf"));

        [STAThread]
        public static void Main(params string[] commandLineArgs)
        {
            isDebugMode = commandLineArgs.Contains("-debug");
            gameForm.Show();
            var gameFieldSize = new GameFieldSize(gameForm.GameFieldSize.Height, gameForm.GameFieldSize.Width);

            var renderersLoader = new ResourcesLoader(Path.Combine(PathHelpers.RootPath, "Resources"),
                playerSizesByStates,
                enemiesSizesByBehavior);
            texturesRepository = renderersLoader.LoadTextures();
            var renderersSet = renderersLoader.LoadRenderers();

            gameRenderer = new GameRenderer(
                new PointF(gameForm.GameFieldSize.Width - (float) gameFieldSize.Width, (float) gameFieldSize.Height),
                renderersSet);

            var game = new GameModel(
                new RandomBehaviorEnemyFactory(enemiesSizesByBehavior,
                    StandingEnemyBehavior.Creator(),
                    FlyingEnemyBehavior.Creator(1),
                    FlyingEnemyBehavior.Creator(2),
                    RunningEnemyBehavior.Creator(1),
                    RunningEnemyBehavior.Creator(1.5)),
                gameFieldSize, playerSizesByStates);

            gameForm.KeyDown += (_, args) => keyboardController.KeyPressed(args.KeyCode);
            gameForm.KeyUp += (_, args) => keyboardController.KeyReleased(args.KeyCode);

            RedrawEnvironment();
            using var timer = new Timer {Interval = BetweenTicksDelay};
            timer.Tick += (_, __) =>
            {
                game.Tick(keyboardController.ActiveAction);
                UpdateGameField(game);
            };

            timer.Start();
            Application.Run(gameForm);
        }

        private static void UpdateGameField(GameModel game)
        {
            gameForm.Draw(gameRenderer.Render(game, isDebugMode));
            gameForm.SetGameScores(game.State.Score);
            gameForm.SetGameState(game.State.PlayerAlive
                ? $"HP: {game.State.PlayerHealth}"
                : $"Press {keyboardController.GetKeyBindForAction(GameAction.Start)} to start");
        }

        private static void RedrawEnvironment()
        {
            var floorImage = new Bitmap(gameForm.FloorSize.Width, gameForm.FloorSize.Height)
                .FillWith(texturesRepository.Get("floor"));
            gameForm.SetFloorImage(floorImage);
            gameForm.SetBackgroundImage(texturesRepository.Get("background")
                .Resize(gameForm.GameFieldSize));
        }
    }
}