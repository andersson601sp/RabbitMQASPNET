using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace application.Infra
{
    public static class EnvConfig
    {
        public static string RabbitMQHost()
        {
            var root = Directory.GetCurrentDirectory();
            var dotenv = Path.Combine(root, ".env");
            Load(dotenv);

            return Environment.GetEnvironmentVariable("RabbitMQHOST");
        }

        public static void Load(string path)
        {
            var dir = path.Split("\\", StringSplitOptions.RemoveEmptyEntries);
            string _path = "";
            string _pathEnv = "";

            foreach (var item in dir)
            {
                _pathEnv = _pathEnv + item + "\\";

                if (File.Exists(_pathEnv + ("\\.env")))
                {
                    _path = _pathEnv + ("\\.env");
                    break;
                }

            }

            if (!File.Exists(_path))
                return;

            foreach (var line in File.ReadAllLines(_path))
            {
                var part = line.Split('=',StringSplitOptions.RemoveEmptyEntries);

                if (part.Length != 2)
                    continue;

                Environment.SetEnvironmentVariable(part[0], part[1]);
            }
        }
    }
}
