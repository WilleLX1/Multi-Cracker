namespace MultiCracker
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(true);

            // Create a single instance of your main form
            Form1 form1 = new Form1();

            // Run the application
            Application.Run(form1);
        }
    }
}