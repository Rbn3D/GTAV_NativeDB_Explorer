using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NativeDb_Explorer.Model
{
    public class GTAVNative
    {
        public String Namespace { get; set; }
        public String FunctionName { get; set; }

        public String ParametersSignature { get; set; }
        //public List<FunctionParameter> Parameters { get; set; } = new List<FunctionParameter>();

        public String ReturnType { get; set; }
        public String Commentary { get; set; }

        public String Description { get; set; }

        public String Address { get {return getMemoryAddressFromCommentary(); } }

        private string getMemoryAddressFromCommentary()
        {
            if (Commentary == null || Commentary == String.Empty)
                return "";

            var sp = Commentary.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            return sp.Length >= 1 ? "0x" + sp[1] : "";
        }
    }
}
