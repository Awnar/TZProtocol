using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.InteropServices;

namespace serwer
{
    public class DB
    {
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

        private List<string> ID = new List<string>();

        public List<zzzz> his = new List<zzzz>();

        //private Dictionary<string, List<zzzz>> his =
        //    new Dictionary<string, List<zzzz>>();

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