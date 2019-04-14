namespace programmersdigest.Metamorphosis
{
    public sealed class App
    {
        public void Start(string modelFilename)
        {
            var loader = new Loader(modelFilename);
            loader.Init();
            loader.Run();
        }
    }
}
