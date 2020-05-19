using SIS.MvcFramework;
using System;

namespace Panda.App
{
   public class Program
    {
       public static void Main(string[] args)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("EN-US");
            WebHost.Start(new Startup());
        }
    }
}
