using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading;
using VersionadorCore.Classes;

namespace VersionadorCore
{
    public class VersionadorCore
    {
        public delegate void EventoExecucaoDelegate(object sender, ExecucaoEventArgs e);

        #region Propriedades
        public string Diretorio { get; set; }
        public string NomeProcedure { get; set; }
        public event EventoExecucaoDelegate AntesExecucaoEvent;
        public event EventoExecucaoDelegate AposExecucaoEvent;
        public List<DatabaseClass> Databases { get; }

        private bool houveErro = false;
        private VersionadorException erro = null;
        private Thread thread = null;
        #endregion

        #region Métodos
        /// <summary>
        /// Método construtor. Retorna uma nova instância de VersionadorCore.
        /// </summary>
        public VersionadorCore()
        {
            Databases = new List<DatabaseClass>();
        }

        /// <summary>
        /// Método para iniciar a execução do versionamento.
        /// </summary>
        public void Executar()
        {
            if (!EmExecucao())
            {
                thread = new Thread(ExecutarVersionamento);
                thread.Start();
            }
        }

        /// <summary>
        /// Verifica se há um versionamento em execução.
        /// </summary>
        /// <returns>True caso haja alguma execução em andamento. False caso contrário.</returns>
        private bool EmExecucao()
        {
            return thread?.ThreadState == ThreadState.Running;
        }

        /// <summary>
        /// Salva uma versão de backup e uma de alteração da procedure informada, no diretório selecionado.
        /// </summary>
        /// <returns>Retorna vazio caso dê certo. Caso contrário retorna a mensagem de erro.</returns>
        private void ExecutarVersionamento()
        {
            string erroValidacao = Validar();
            string textoCompleto = string.Empty;

            try
            {
                LimparVariaveis();
                OnAntesExecucaoEvento();
                if (string.IsNullOrEmpty(erroValidacao))
                {
                    foreach (DatabaseClass database in Databases)
                    {
                        string query = "SELECT C.TEXT " +
                                     "FROM SYS.SYSOBJECTS O WITH(NOLOCK) " +
                                     "INNER JOIN SYS.SYSCOMMENTS C ON O.ID = C.ID " +
                                     string.Format("WHERE O.NAME = '{0}'", NomeProcedure);
                        string textoProcedure = ConsultarTexto(database, query);
                        if (!string.IsNullOrEmpty(textoProcedure))
                        {
                            textoCompleto = MontarCabecalhoProcedure(database, query) + textoProcedure;
                            new ArquivoClass().Salvar(textoCompleto, Diretorio, database.NomePasta, NomeProcedure + "_BKP.sql");
                            new ArquivoClass().Salvar(textoCompleto, Diretorio, database.NomePasta, NomeProcedure + "_ALT.sql");
                        }
                        else
                        {
                            TratarErro(new Exception("Não há procedure de nome " + NomeProcedure + " na database " + database.Database.ToUpper() + "!!!"));
                        }
                    }
                    OnAposExecucaoEvento();
                }
                else
                {
                    TratarErro(new Exception(erroValidacao));
                    OnAposExecucaoEvento();
                }
            }
            catch (Exception ex)
            {
                TratarErro(ex);
                OnAposExecucaoEvento();
            }
        }

        /// <summary>
        /// Valida se há um nome de procedure e diretório informados.
        /// </summary>
        /// <returns>Retorna mensagem de erro, caso haja. S~e não retorna vazio.</returns>
        private string Validar()
        {
            string retorno = string.Empty;
            if (string.IsNullOrEmpty(Diretorio))
            {
                retorno = "É necessário definir o diretório!!";
            }
            else if (string.IsNullOrEmpty(NomeProcedure))
            {
                retorno = "É necessário definir o nome da procedure!!";
            }
            return retorno;
        }

        /// <summary>
        /// Retorna o texto do cabeçalho da procedure salva no Banco de Dados.
        /// </summary>
        /// <param name="database">DatabaseClass representando a database onde será consultada a procedure.</param>
        /// <param name="query">Query para execução da procedure no banco de dados.</param>
        /// <returns>Retorna o texto do cabeçalho da procedure salva no Banco de Dados.</returns>
        private string MontarCabecalhoProcedure(DatabaseClass database, string query)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("USE[" + database.Database + "]");
            builder.AppendLine("GO");
            builder.AppendLine("SET ANSI_NULLS ON");
            builder.AppendLine("GO");
            builder.AppendLine("SET QUOTED_IDENTIFIER ON");
            builder.AppendLine("GO");
            return builder.ToString();
        }

        /// <summary>
        /// Retorna o texto da procedure salvo no banco de dados.
        /// </summary>
        /// <param name="database">DatabaseClass representando a database onde será consultada a procedure.</param>
        /// <param name="query">Query para execução da procedure no banco de dados.</param>
        /// <returns>Retorna o texto da procedure salvo no banco de dados.</returns>
        private string ConsultarTexto(DatabaseClass database, string query)
        {
            string retorno = string.Empty;
            DataTable dados = new ConexaoClass(database.GetConnectionString()).RetornarDados(query);
            foreach (DataRow linha in dados.Rows)
            {
                retorno += linha[0].ToString();
            }
            return retorno;
        }

        /// <summary>
        /// Preenche variáveis internas com as informações do erro.
        /// </summary>
        /// <param name="erro">Exception com as informações do erro.</param>
        private void TratarErro(Exception erro)
        {
            if (!houveErro)
            {
                this.erro = new VersionadorException("Erro ao realizar a operação! Veja InnerExceptions para mais detalhes.");
                houveErro = true;
            }
            this.erro.InnerExceptions.Add(erro);
        }

        /// <summary>
        /// Limpa variáveis internas.
        /// </summary>
        private void LimparVariaveis()
        {
            houveErro = false;
            erro = null;
        }
        #endregion

        #region Eventos
        /// <summary>
        /// Chamada do evento AntesExecucaoEvent
        /// </summary>
        private void OnAntesExecucaoEvento()
        {
            AntesExecucaoEvent?.Invoke(this, new ExecucaoEventArgs() { HouveErro = houveErro, Erro = erro });
        }

        /// <summary>
        /// Chamada do evento AposExecucaoEvent
        /// </summary>
        private void OnAposExecucaoEvento()
        {
            AposExecucaoEvent?.Invoke(this, new ExecucaoEventArgs() { HouveErro = houveErro, Erro = erro });
        }
        #endregion
    }
}
