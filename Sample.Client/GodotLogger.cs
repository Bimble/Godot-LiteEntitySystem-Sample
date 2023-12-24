using Godot;
using LiteEntitySystem;

namespace Sample.Shared
{
    internal class GodotLogger : ILogger
    {
        public void Log(string log)
        {
            GD.Print(log);
        }

        public void LogError(string log)
        {
            GD.Print(log);
        }

        public void LogWarning(string log)
        {
            GD.Print(log);
        }
    }
}
