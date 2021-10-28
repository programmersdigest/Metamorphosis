using System;
using System.Reflection;

namespace programmersdigest.Metamorphosis
{
    public sealed class App
    {
        public void Start(string modelFilename, Func<ConstructorInfo[], object[]> resolveConstructorParametersCallback = null)
        {
            var loader = new Loader(modelFilename, resolveConstructorParametersCallback);
            loader.Init();
            loader.Run();
        }
    }
}
