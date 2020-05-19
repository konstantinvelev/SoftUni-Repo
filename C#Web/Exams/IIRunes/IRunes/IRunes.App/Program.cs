namespace IRunes.App
{
    using System.Threading.Tasks;

    using SIS.MvcFramework;

    public static class Program
    {
        public static async Task Main()
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("EN-US");

            await WebHost.StartAsync(new Startup());
        }
    }
}
