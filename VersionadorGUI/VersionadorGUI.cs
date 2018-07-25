using System;

namespace VersionadorGUI
{
    public class VersionadorGUI
    {
        public static void Main()
        {
            VersionadorCore.VersionadorCore versionador = new VersionadorCore.VersionadorCore()
            {
                NomeProcedure = "",
                Diretorio = @""
            };
            versionador.AntesExecucaoEvent += (sender, e) =>
            {
                Console.WriteLine("Iniciando execução...");
            };
            versionador.AposExecucaoEvent += (sender, e) =>
            {
                if (e.HouveErro)
                {
                    Console.WriteLine("Erro ao executar: " + e.Erro.Message);
                }
                else
                {
                    Console.WriteLine("Execução finalizada com sucesso!");
                }
                Console.ReadKey();
            };
            versionador.Executar();
        }
    }
}
