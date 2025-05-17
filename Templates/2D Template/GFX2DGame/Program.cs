namespace GFX2DGame
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Game game = new Game();
            game.Initialize();
            game.Run();
        }
    }
}