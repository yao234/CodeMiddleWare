using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NeuSoftMedicare.SQLServer.Hospital
{
    public sealed class CheckRealElementAttribute:Attribute
    {
        private string Relname;
        public CheckRealElementAttribute() { }
        public CheckRealElementAttribute(string name)
        {
            this.Relname = name;
        }
        public string GetRelName() => this.Relname;
    }
}
