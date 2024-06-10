using Newtonsoft.Json;
using VismaResourceShortage.Models;

namespace VismaResourceShortage.Services
{
    public class JsonService
    {
        private readonly string filePath = "shortages.json";

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
