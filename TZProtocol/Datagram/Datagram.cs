using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datagram
{
    public class Datagram
    {
        public string ID;
        private string _NS,_OP, _OP_ID, _ST;
        public List<long> L = new List<long>();
        private int k;

        /* CZ > znacznik czasu
         * ID -> generowane pseudolosowo z adresu nadawcy
         * ST -> staus
         *
         * NS -> numer sekcji jeśli > 1
         *
         * OP -> operacja
         * LL -> liczba
         *
         * IO -> ID operacji (odsyla sserwer)
         *
         * 
         * 
         * OP, ST, NS, ID, ZC, LL, IO, ...
         * 
        */
        private static string separator = "\n";

        public string[] gen()
        {
            k += L.Count;

            if (k <= 2)
            {
                string bs = "ZC: " + DateTime.Now.Ticks + separator + "ID: " + ID;

                if (_ST != null) bs = "ST: " + _ST + separator + bs;
                if (_OP != null) bs = "OP: " + _OP + separator + bs;
                if (_OP_ID != null) bs += separator + "OI: " + _OP_ID;

                foreach (var item in L)
                {
                    bs += separator + "LL: " + item;
                }
                return new[] {bs};
            }

            var tmp = new string[k];
            int i = 0;

            if (_OP != null)
                tmp[i++] = "OP: " + _OP + separator + "NS: " + --k + separator + "ID: " + ID + separator + "ZC: " +
                           DateTime.Now.Ticks;
            if (_ST != null)
                tmp[i++] = "ST: " + _ST + separator + "NS: " + --k + separator + "ID: " + ID + separator + "ZC: " +
                           DateTime.Now.Ticks;
            if (_OP_ID != null)
                tmp[i++] = "NS: " + --k + separator + "ID: " + ID + separator + "ZC: " + DateTime.Now.Ticks +
                           separator + "IO: " + _OP_ID;
            foreach (var item in L)
                tmp[i++] = "NS: " + --k + separator + "ID: " + ID + separator + "ZC: " + DateTime.Now.Ticks +
                           separator + "LL: " + item;
            return tmp;
        }

        public static Dictionary<string, string> analyze(string s)
        {
            var tmp = s.Split(separator.ToCharArray());
            var map = new Dictionary<string, string>();
            foreach (var item in tmp)
            {
                var o = item.IndexOf(" ");
                var ss = item.Substring(0, o).Trim();
                ss = ss.Trim(':');
                var i = item.Substring(o).Trim();
                map.Add(ss,i);
            }
            return map;
        }

        public string OP
        {
            get => _OP;
            set
            {
                _OP = value;
                k++;
            }
        }

        public string ST
        {
            get => _ST;
            set
            {
                _ST = value;
                k++;
            }
        }

        public string OP_ID
        {
            get => OP_ID;
            set
            {
                OP_ID = value;
                k++;
            }
        }
    }
}
