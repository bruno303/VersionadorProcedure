using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Threading.Tasks;
using VersionadorCore.Classes;

namespace VersionadorGUI
{
    public class VersionadorGUI
    {
        const string FILE_NAME = "database.db";

        /// <summary>
        /// Início da execução.
        /// </summary>
        public static void Main()
        {
            Console.Write("Digite o nome da proc: ");
            string proc = Console.ReadLine();
            Console.WriteLine();
            Console.Write("Digite o diretório: ");
            string diretorio = Console.ReadLine();
            Console.WriteLine();
            VersionadorCore.VersionadorCore versionador = new VersionadorCore.VersionadorCore()
            {
                NomeProcedure = proc,
                Diretorio = diretorio
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
            Executar(versionador);
        }

        private static async void Executar(VersionadorCore.VersionadorCore core)
        {
            await AtribuirDatabases(core);
            core.Executar();
        }

        /// <summary>
        /// Adiciona as databases ao versionador.
        /// </summary>
        /// <param name="core">Instância do versionador que receberá as databases.</param>
        private static async Task AtribuirDatabases(VersionadorCore.VersionadorCore core)
        {
            var databases = await CarregarDatabases();
            core?.Databases.AddRange(databases);
        }

        private static async Task<IList<DatabaseClass>> CarregarDatabases()
        {
            IList<DatabaseClass> retorno = new List<DatabaseClass>();
            string user = string.Empty;
            string pass = string.Empty;

            using (var conexao = new SQLiteConnection($"Data Source={FILE_NAME};Version=3;"))
            {
                await conexao.OpenAsync();
                using (var command = new SQLiteCommand())
                {
                    command.Connection = conexao;
                    command.CommandType = System.Data.CommandType.Text;
                    command.CommandText = " SELECT USUARIO, SENHA FROM CONFIG; ";
                    using (var rs = await command.ExecuteReaderAsync())
                    {
                        if (rs.HasRows)
                        {
                            await rs.ReadAsync();
                            user = rs["USUARIO"].ToString();
                            pass = rs["SENHA"].ToString();
                        }
                    }

                    command.CommandText = " SELECT SERVIDOR, NOME_DATABASE, NOME_PASTA FROM BANCOS_DADOS; ";
                    using (var rs = await command.ExecuteReaderAsync())
                    {
                        if (rs.HasRows)
                        {
                            while (await rs.ReadAsync())
                            {
                                retorno.Add(new DatabaseClass()
                                {
                                    Servidor = rs["SERVIDOR"].ToString(),
                                    Database = rs["NOME_DATABASE"].ToString(),
                                    NomePasta = rs["NOME_PASTA"].ToString(),
                                    Usuario = user,
                                    Senha = pass
                                });
                            }
                        }
                    }
                }
            }

            return retorno;
        }
    }
}
