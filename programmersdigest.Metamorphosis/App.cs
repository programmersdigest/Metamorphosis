using System;
using System.Reflection;

namespace programmersdigest.Metamorphosis
{
    public sealed class App
    {
        private readonly string _modelFilename;
        private readonly Func<ConstructorInfo[], object?[]?>? _resolveConstructorParametersCallback;

        public App(string modelFilename, Func<ConstructorInfo[], object?[]?>? resolveConstructorParametersCallback = null)
        {
            _modelFilename = modelFilename;
            _resolveConstructorParametersCallback = resolveConstructorParametersCallback;
        }

        public void Start()
        {
            var loader = new Loader(_modelFilename, _resolveConstructorParametersCallback);
            loader.Init();
            loader.Run();
        }
    }
}
