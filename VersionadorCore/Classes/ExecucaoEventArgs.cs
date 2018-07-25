using System;

namespace VersionadorCore.Classes
{
    public class ExecucaoEventArgs : EventArgs
    {
        public bool HouveErro { get; set; }
        public Exception Erro { get; set; }
    }
}
