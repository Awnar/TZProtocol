using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.InteropServices;

namespace serwer
{
    public class DB
    {
        //struktura zachowująca informacje z pakietów 
        public struct zzzz
        {
            public List<int> L;
            public string OP;
            public string ID;
            public string ST;
            public int IO;
            public bool com;
            public bool OF;
        }

        //Lista identyfikatorów 
        private List<string> ID = new List<string>();

        //historia serwera
        public List<zzzz> his = new List<zzzz>();

        //przydzielanie nowej sesji
        public string newSession()
        {
            var rand = new Random();
            string id;
            do
            {
                id = rand.Next().ToString();

            } while (ID.Contains(id));

            ID.Add(id);
            return id;
        }
    }
}