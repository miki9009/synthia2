using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Synthia
{
    class Questions
    {
        public static List<string> Load(string path)
        {
            var list = new List<string>();
            StreamReader sr = new StreamReader(path);
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                list.Add(line);
            }
            return list;
        }
    }
}
