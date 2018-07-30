using System;
using System.Collections.Generic;

namespace VersionadorCore.Classes
{
    public class VersionadorException: Exception
    {
        public List<Exception> InnerExceptions { get; }

        #region Construtores
        /// <summary>
        /// Método construtor. Retorna uma nova instância de VersionadorException.
        /// </summary>
        /// <param name="message">Mensagem principal de erro.</param>
        public VersionadorException(string message) : base(message)
        {
            InnerExceptions = new List<Exception>();
        }

        /// <summary>
        /// Método construtor. Retorna uma nova instância de VersionadorException.
        /// </summary>
        /// <param name="message">Mensagem principal de erro.</param>
        /// <param name="innerException">Exception para ser adicionada à lista.</param>
        public VersionadorException(string message, Exception innerException) : base(message)
        {
            InnerExceptions = new List<Exception>();
            InnerExceptions.Add(innerException);
        }
        #endregion
    }
}
