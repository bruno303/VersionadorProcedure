namespace VersionadorCore.Classes
{
    public class DatabaseClass
    {
        public string Servidor { get; set; }
        public string Database { get; set; }
        public string NomePasta { get; set; }
        public string Usuario { get; set; }
        public string Senha { get; set; }

        public string GetConnectionString()
        {
            return string.Format("Server = {0}; Database = {1}; User Id = {2};Password = {3};", Servidor, Database, Usuario, Senha);
        }
    }
}
