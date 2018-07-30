using System;
using VersionadorCore.Classes;

namespace VersionadorGUI
{
    public class VersionadorGUI
    {
        /// <summary>
        /// Início da execução.
        /// </summary>
        public static void Main()
        {
            VersionadorCore.VersionadorCore versionador = new VersionadorCore.VersionadorCore()
            {
                NomeProcedure = "PRC_MUITO_GRANDE",
                Diretorio = @"C:\Users\Bruno\Documents\teste"
            };
            versionador.AntesExecucaoEvent += (sender, e) =>
            {
                Console.WriteLine("Execução iniciada...");
            };
            versionador.AposExecucaoEvent += (sender, e) =>
            {
                if (e.HouveErro)
                {
                    Console.WriteLine("Execução finalizada! " + e.Erro.Message);
                    foreach (Exception exception in e.Erro.InnerExceptions)
                    {
                        Console.WriteLine(exception.Message);
                    }
                }
                else
                {
                    Console.WriteLine("Execução finalizada com sucesso!");
                }
                Console.ReadKey();
            };
            AtribuirDatabases(versionador);
            versionador.Executar();
        }

        /// <summary>
        /// Adiciona as databases ao versionador.
        /// </summary>
        /// <param name="core">Instância do versionador que receberá as databases.</param>
        private static void AtribuirDatabases(VersionadorCore.VersionadorCore core)
        {
            core?.Databases?.Add(new DatabaseClass() { Servidor = "", Database = "", NomePasta = "", Usuario = "", Senha = "" });
        }
    }
}
