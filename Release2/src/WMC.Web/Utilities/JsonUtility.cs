using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace WMC.Web.Utilities
{
    public class JsonUtility
    {
        public static List<MyObject> readJSON(string path)
        {
            string jsonFromFile;
            using (var reader = new StreamReader(path))
            {
                jsonFromFile = reader.ReadToEnd();
            }

            var jsonValues = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonFromFile);
            List<MyObject> myObj = new List<MyObject>();
            foreach(var jsonObj in jsonValues)
            {
                Console.WriteLine(jsonObj.Key + ":" + jsonObj.Value);
                var obj = new MyObject(
                    jsonObj.Key,
                    jsonObj.Value
                );
                myObj.Add(obj);
            }
            return myObj;
        }
    }

    public class MyObject
    {
        public MyObject(string key, string value)
        {
            this.key = key;
            this.value = value;
        }

        public string key { get; set; }
        public string value { get; set; }
    }
}