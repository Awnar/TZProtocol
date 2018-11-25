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
        public string ID, ST;
        private string _NS,_OP, _OP_ID;
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
         * OI -> ID operacji (odsyla sserwer)
         * 
        */
        private static string separator = "\n";

        public string[] gen()
        {
            string bs = "CZ: " + DateTime.Now.Ticks + separator + "ID: " + ID + separator + "ST: " + ST;

            k += L.Count;

            if (k <= 2)
            {
                foreach (var item in L)
                {
                    bs += separator + "LL: " + item;
                }
                if (_OP != null) bs += separator + "OP: " + _OP;
                if (_OP_ID != null) bs += separator + "OI: " + _OP_ID;
                return new[] {bs};
            }

            var tmp = new string[k];
            int i = 0;
            
            foreach (var item in L)
                tmp[i++] = bs + separator + "NS: " + --k + separator + "LL: " + item;
            if (_OP != null) tmp[i++] = bs + separator + "NS: " + --k + separator + "OP: " + _OP;
            if (_OP_ID != null) tmp[i++] = bs + separator + "NS: " + --k + separator + "OI: " + _OP_ID;

            return tmp;
        }

        public Dictionary<string, string> analyze(string s)
        {
            var tmp = s.Split(separator.ToCharArray());
            var map = new Dictionary<string, string>();
            foreach (var item in tmp)
            {
                var o = item.IndexOf(" ");
                var ss = item.Substring(0, o-1);
                var i = item.Substring(o+1);
                map.Add(ss,i);
            }
            return map;
        }

        public string NS => _NS;

        public string OP
        {
            get => _OP;
            set
            {
                _OP = value;
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
