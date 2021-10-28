using System;
using System.Linq;

namespace programmersdigest.Metamorphosis.Playground
{
    public sealed class Program
    {
        public static void Main()
        {
            var app = new App();
            app.Start("Model.json", ctors => ctors.First()
                                                  .GetParameters()
                                                  .Select(p => p.ParameterType == typeof(string)
                                                             ? "Injected Dependency :)"
                                                             : throw new InvalidOperationException())
                                                  .ToArray());
        }
    }
}
