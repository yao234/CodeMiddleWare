using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NeuSoftMedicare.SQLServer.Hospital
{
    public sealed class NewFunctionCodeAttribute:Attribute
    {
        private string Code;
        public NewFunctionCodeAttribute(string Code)
        {
            this.Code = Code;
        }
        public string GetCode() => this.Code;
    }
}
