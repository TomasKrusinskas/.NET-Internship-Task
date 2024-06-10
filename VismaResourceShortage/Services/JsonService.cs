using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using VismaResourceShortage.Models;

namespace VismaResourceShortage.Services
{
    public class JsonService
    {
        private readonly string filePath;

        public JsonService(string filePath = "shortages.json")
        {
            this.filePath = filePath;
        }

        public List<Shortage> LoadShortages()
        {
            if (!File.Exists(filePath))
            {
                return new List<Shortage>();
            }

            var jsonData = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<List<Shortage>>(jsonData);
        }

        public void SaveShortages(List<Shortage> shortages)
        {
            var jsonData = JsonConvert.SerializeObject(shortages, Formatting.Indented);
            File.WriteAllText(filePath, jsonData);
        }
    }
}
