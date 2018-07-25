using System.IO;

namespace VersionadorCore.Classes
{
    internal class ArquivoClass
    {
        /// <summary>
        /// Salvar um texto em arquivo.
        /// </summary>
        /// <param name="texto">Texto a ser salvo em arquivo.</param>
        /// <param name="diretorio">Diretorio raiz do arquivo a ser criado/substituído.</param>
        /// <param name="pasta">Pasta que deve existir ou será criada dentro do diretório.</param>
        /// <param name="fileName">Nome do arquivo a ser criado/substituído.</param>
        public void Salvar(string texto, string diretorio, string pasta, string fileName)
        {
            string fullFileName = string.Empty;

            if (!diretorio.EndsWith("\\"))
            {
                diretorio += "\\";
            }
            diretorio += pasta;
            if (!Directory.Exists(diretorio))
            {
                Directory.CreateDirectory(diretorio);
            }

            fullFileName = diretorio + "\\" + fileName;

            using (StreamWriter writer = new StreamWriter(fullFileName))
            {
                writer.WriteLine(texto);
            }
        }
    }
}
