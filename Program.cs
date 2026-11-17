namespace P1
{
    internal static class Program
    {
        ///  Método main punto de entrada a la aplicación
        [STAThread]
        static void Main()
        { 
            ApplicationConfiguration.Initialize();
            QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;
            QuestPDF.Settings.UseSystemFonts = true; //Habilitar uso de fuentes del sistema operativo
            Application.Run(new Login());
        }
    }
}