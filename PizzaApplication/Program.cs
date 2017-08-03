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

        public static void WriteOutPizzas(List<Pizza> pizzaList, int maxRank = 20)
        {
            var rankPad = pizzaList.Count.ToString().Length;
            var occurencePad = pizzaList.Max(c => c.Occurences).ToString().Length;

            for (var i = 0; i < Math.Min(maxRank, pizzaList.Count); i++)
            {
                var pizza = pizzaList[i];
                Console.WriteLine(
                    $"Rank[{i.ToString().PadLeft(rankPad, '0')}] Occurences[{pizza.Occurences.ToString().PadLeft(occurencePad, '0')}] Toppings: {pizza}");
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

        public static List<Pizza> GetPizzas(string filePath)
        {
            var pizzaList = new List<Pizza>();
            var jsonData = GetJson(filePath);

            foreach (var obj in jsonData)
            {
                var newPizza = GetPizza(obj);
                MergePizzas(pizzaList, newPizza);
            }

            return pizzaList.OrderByDescending(c => c.Occurences).ToList();
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

        public static Pizza GetPizza(dynamic jsonObj)
        {
            var toppings = new List<string>();

            foreach (var topping in jsonObj.toppings)
                toppings.Add(topping.ToString());

            return new Pizza(toppings);
        }

        public static void MergePizzas(List<Pizza> currentPizzas, Pizza newPizza)
        {
            var existingPizza = currentPizzas.FirstOrDefault(c => c.ToppingsId == newPizza.ToppingsId);

            if (existingPizza == null)
                currentPizzas.Add(newPizza);
            else
                existingPizza.Occurences++;
        }
    }
}