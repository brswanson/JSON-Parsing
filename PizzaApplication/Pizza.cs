using System.Collections.Generic;

namespace PizzaApplication
{
    internal class Pizza
    {
        public Pizza(List<string> toppings)
        {
            Toppings = toppings;
            Occurences = 1;
        }

        public List<string> Toppings { get; set; }
        public string ToppingsId => _ToppingsId ?? (_ToppingsId = string.Join(string.Empty, Toppings));
        private string _ToppingsId { get; set; }
        public int Occurences { get; set; }

        public override string ToString()
        {
            return string.Join(", ", Toppings);
        }
    }
}