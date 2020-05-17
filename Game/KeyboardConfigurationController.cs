using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Game.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Game
{
    public class KeyboardConfigurationController
    {
        private readonly Lazy<Dictionary<Keys, GameAction>> defaultConfiguration;
        private readonly string configurationPath;

        private Dictionary<Keys, GameAction> KeyBindings { get; set; }
        public GameAction ActiveAction { get; private set; } = GameAction.Nothing;

        public KeyboardConfigurationController(string configurationPath)
        {
            this.configurationPath = configurationPath;

            defaultConfiguration = new Lazy<Dictionary<Keys, GameAction>>(() =>
                new Dictionary<Keys, GameAction>
                {
                    {Keys.W, GameAction.PlayerJump},
                    {Keys.S, GameAction.PlayerCrouch},
                    {Keys.E, GameAction.Start}
                });
            Read();
        }

        public void KeyPressed(Keys key)
        {
            if (KeyBindings.TryGetValue(key, out var value))
                ActiveAction |= value;
        }

        public void KeyReleased(Keys key)
        {
            if (KeyBindings.TryGetValue(key, out var value))
                ActiveAction ^= value;
        }

        public Keys GetKeyBindForAction(GameAction gameAction)
        {
            var requiredKeys = KeyBindings.Where(x => x.Value == gameAction)
                .Select(x => x.Key)
                .ToArray();
            return requiredKeys.Length != 0 
                ? requiredKeys[0]
                : Keys.None;
        }

        public void Save()
        {
            var serialized = JsonConvert.SerializeObject(KeyBindings, Formatting.Indented, new StringEnumConverter());
            using var streamWriter = new StreamWriter(OpenStream(true), Encoding.UTF8);
            streamWriter.Write(serialized);
        }

        public void Read()
        {
            if (File.Exists(configurationPath))
            {
                using var streamReader = new StreamReader(OpenStream(false), Encoding.UTF8);
                var rawText = streamReader.ReadToEnd();
                try
                {
                    KeyBindings = JsonConvert.DeserializeObject<Dictionary<Keys, GameAction>>(rawText);
                }
                catch
                {
                    Console.WriteLine($"Can't deserialize [{rawText}], using default bindings");
                    KeyBindings = null;
                }
            }

            if (KeyBindings != null && KeyBindings.Count != 0) return;
            KeyBindings = defaultConfiguration.Value;
            Save();
        }

        private FileStream OpenStream(bool forWriting)
        {
            return new FileStream(configurationPath, forWriting
                    ? FileMode.Create
                    : FileMode.Open,
                forWriting
                    ? FileAccess.Write
                    : FileAccess.Read);
        }
    }
}