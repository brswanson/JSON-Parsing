using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Newtonsoft.Json;

namespace PizzaApplication
{
    internal class Program
    {
        // Source of the input file
        private const string PizzaUrl = "https://raw.githubusercontent.com/brswanson/JSON-Parsing-Example/master/PizzaApplication/Input/pizzas.json";
        private const int MaxRank = 20;

        /// <summary>
        ///     Downloads a JSON file of pizza toppings. Ranks topping combinations by frequency.
        /// </summary>
        private static void Main()
        {
            var tempSourceLocation = DownloadInputFileToTemp(PizzaUrl);
            var pizzaList = GetPizzas(tempSourceLocation);

            WriteOutPizzas(pizzaList);

            if (File.Exists(tempSourceLocation)) File.Delete(tempSourceLocation);

            Console.WriteLine("Press the Enter key to exit ...");
            Console.ReadLine();
        }

        public static void WriteOutPizzas(KeyValuePair<string, int>[] pizzaArray, int maxRank = MaxRank)
        {
            var pizzasOrderedByCount = pizzaArray.OrderByDescending(p => p.Value).ToArray();

            for (var i = 0; i < Math.Min(maxRank, pizzaArray.Length); i++)
            {
                var pizza = pizzasOrderedByCount[i];
                Console.WriteLine($"Rank[{i + 1}] Occurences[{pizza.Value}] Toppings: {pizza.Key}");
            }
        }

        public static string DownloadInputFileToTemp(string webUri)
        {
            var tempSourceLocation = Path.GetTempFileName();

            using (var client = new WebClient())
            {
                client.DownloadFile(webUri, tempSourceLocation);
            }

            return tempSourceLocation;
        }

        public static KeyValuePair<string, int>[] GetPizzas(string filePath)
        {
            var pizzaDict = new Dictionary<string, int>();
            var jsonData = GetJson(filePath);

            foreach (var obj in jsonData)
            {
                var pizzaKey = GetPizzaToppings(obj);

                pizzaDict.TryGetValue(pizzaKey, out int currentCount);
                pizzaDict[pizzaKey] = currentCount + 1;
            }

            return pizzaDict.ToArray();
        }

        public static dynamic GetJson(string filePath)
        {
            using (var r = new StreamReader(filePath))
            {
                var json = r.ReadToEnd();
                dynamic jsonArray = JsonConvert.DeserializeObject(json);

                return jsonArray;
            }
        }

        public static string GetPizzaToppings(dynamic jsonObj)
        {
            // HashSet prevents duplicate toppings
            var toppings = new HashSet<string>();

            foreach (var topping in jsonObj.toppings)
            {
                toppings.Add(topping.ToString());
            }

            return string.Join(", ", toppings.OrderBy(p => p.ToString()).ToList());
        }
    }
}
