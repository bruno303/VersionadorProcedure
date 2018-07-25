using System;
using System.Data;
using System.Data.SqlClient;

namespace VersionadorCore.Classes
{
    internal class ConexaoClass
    {
        /// <summary>
        /// ConnectionString a ser utilizada nas conexões.
        /// </summary>
        private string connectionString = string.Empty;

        #region Construtores
        /// <summary>
        /// Cria uma nova instância de ConexaoClass recebendo a string de conexão a ser utilizada.
        /// </summary>
        /// <param name="connectionString"></param>
        public ConexaoClass(string connectionString)
        {
            this.connectionString = connectionString;
        }

        /// <summary>
        /// Cria uma nova instância de ConexaoClass.
        /// </summary>
        public ConexaoClass() { }
        #endregion

        #region Métodos
        /// <summary>
        /// Retorna um único dado de uma consulta.
        /// </summary>
        /// <param name="campo">Nome do campo ser selecionado.</param>
        /// <param name="tabela">Nome da tabela onde está o campo.</param>
        /// <param name="where">Condição where para filtrar registros.</param>
        /// <returns>Retorna a primeira coluna, da primeira linha, em formato string.</returns>
        public string RetornarDadoUnico(string campo, string tabela, string where)
        {
            DataTable dados = new DataTable();
            string query = string.Format("SELECT {0} FROM {1} ", campo, tabela);

            if (!string.IsNullOrEmpty(where))
            {
                query += "WHERE " + where;
            }

            try
            {
                using (SqlConnection conexao = new SqlConnection(connectionString))
                {
                    conexao.Open();
                    using (SqlCommand comando = new SqlCommand(query, conexao))
                    {
                        comando.CommandType = CommandType.Text;
                        using (SqlDataAdapter adapter = new SqlDataAdapter(comando))
                        {
                            adapter.Fill(dados);
                            return dados.Rows[0][0].ToString();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception("Erro ao consultar dado único.", e);
            }
        }

        /// <summary>
        /// Retorna um único dado de uma consulta.
        /// </summary>
        /// <param name="query">Query a ser executada para retornar um dado.</param>
        /// <returns>Retorna a primeira coluna, da primeira linha, em formato string.</returns>
        public string RetornarDadoUnico(string query)
        {
            DataTable dados = new DataTable();

            try
            {
                using (SqlConnection conexao = new SqlConnection(connectionString))
                {
                    conexao.Open();
                    using (SqlCommand comando = new SqlCommand(query, conexao))
                    {
                        comando.CommandType = CommandType.Text;
                        using (SqlDataAdapter adapter = new SqlDataAdapter(comando))
                        {
                            adapter.Fill(dados);
                            return dados.Rows[0][0].ToString();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception("Erro ao consultar dado único.", e);
            }
        }

        /// <summary>
        /// Retorna todos os dados de uma consulta SQL.
        /// </summary>
        /// <param name="query">Query a ser executada</param>
        /// <returns>Retorna todos os dados retornados pela consulta em uma DataTable.</returns>
        public DataTable RetornarDados(string query)
        {
            DataTable dados = new DataTable();

            try
            {
                using (SqlConnection conexao = new SqlConnection(connectionString))
                {
                    conexao.Open();
                    using (SqlCommand comando = new SqlCommand(query, conexao))
                    {
                        comando.CommandType = CommandType.Text;
                        using (SqlDataAdapter adapter = new SqlDataAdapter(comando))
                        {
                            adapter.Fill(dados);
                            return dados;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception("Erro ao consultar dados.", e);
            }
        }
        #endregion
    }
}
