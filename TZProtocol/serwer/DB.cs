using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace serwer
{
    public class DB
    {

        private List<string> ID = new List<string>();

        private Dictionary<string, Dictionary<string, string>> his =
            new Dictionary<string, Dictionary<string, string>>();

        public string newSession()
        {
            var rand = new Random();
            string id;
            do
            {
                id = rand.Next().ToString();

            } while (ID.Contains(id));

            ID.Add(id);
            his.Add(id,new Dictionary<string, string>());
            return id;
        }


    }
}