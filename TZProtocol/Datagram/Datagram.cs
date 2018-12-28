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
        private string _OP="default", _IO, _ST;
        public List<int> LL = new List<int>();
        public string[] inne;
        private int k = 2;

        /* ZC -> znacznik czasu
         * ID -> generowane pseudolosowo z adresu nadawcy
         * ST -> status
         *
         * NS -> numer sekcji jeśli > 1
         *
         * OP -> operacja
         * LL -> liczba
         *
         * IO -> ID operacji (odsyła serwer)
         *
         * 
         * 
         * OP, ST, NS, ID, ZC, LL, IO, ...
        */
        private static string separator = "\n";

        ///<summary>
        ///Generuje pakiety które mają zostać wysłane
        ///</summary>
        ///<return>Tablica stringów zawierająca treść pakietów</return>
        public string[] gen()
        {
            k += LL.Count;
            if (inne != null)
                k += inne.Length;

            var tmp = new string[k];
            int i = 0;

            if (_OP != null)
                tmp[i++] = "OP: " + _OP + separator + "NS: " + --k + separator + "ID: " + ID + separator + "ZC: " +
                           DateTime.Now.Ticks + separator;
            if (_ST != null)
                tmp[i++] = "ST: " + _ST + separator + "NS: " + --k + separator + "ID: " + ID + separator + "ZC: " +
                           DateTime.Now.Ticks + separator;
            if (_IO != null)
                tmp[i++] = "NS: " + --k + separator + "ID: " + ID + separator + "ZC: " + DateTime.Now.Ticks +
                           separator + "IO: " + _IO + separator;
            foreach (var item in LL)
                tmp[i++] = "NS: " + --k + separator + "ID: " + ID + separator + "ZC: " + DateTime.Now.Ticks +
                           separator + "LL: " + item + separator;
            if (inne != null)
                foreach (var item in inne)
                    tmp[i++] = "NS: " + --k + separator + "ID: " + ID + separator + "ZC: " + DateTime.Now.Ticks +
                               separator + item + separator;
            return tmp;
        }

        ///<summary>
        ///Parsuje treśc pakietu na słownik
        ///</summary>
        ///<param "s">String zawierający treść pakietu przychodzącego</param>
        ///<return>Słownik zawierający poszczególne pola z pakietu</return>
        public static Dictionary<string, string> analyze(string s)
        {
            var tmp = s.Split(separator.ToCharArray());
            var map = new Dictionary<string, string>();
            foreach (var item in tmp)
            {
                if(item=="")continue;
                var o = item.IndexOf(" ");
                var ss = item.Substring(0, o).Trim().ToUpper();
                ss = ss.Trim(':');
                var i = item.Substring(o).Trim().ToLower();
                map.Add(ss,i);
            }
            return map;
        }

        ///<summary>
        ///Dostęp do pola operacji
        ///</summary>
        public string OP
        {
            get => _OP;
            set => _OP = value;
        }

        ///<summary>
        ///Dostęp do pola statusu 
        ///</summary>
        public string ST
        {
            get => _ST;
            set => _ST = value;
        }

        ///<summary>
        ///Dostęp do pola ID operacji
        ///</summary>
        public string IO
        {
            get => _IO;
            set
            {
                _IO = value;
                ++k;
            }
        }
    }
}
