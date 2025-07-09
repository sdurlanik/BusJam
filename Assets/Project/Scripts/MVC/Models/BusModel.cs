using System.Collections.Generic;
using Sdurlanik.BusJam.Models;
using Sdurlanik.BusJam.MVC.Views;

namespace Sdurlanik.BusJam.MVC.Models
{
    public class BusModel
    {
        public CharacterColor BusColor { get; }
        public int Capacity { get; }
        public IReadOnlyList<CharacterView> Passengers => _passengers;
        
        private readonly List<CharacterView> _passengers;

        public BusModel(CharacterColor busColor, int capacity)
        {
            BusColor = busColor;
            Capacity = capacity;
            _passengers = new List<CharacterView>(capacity);
        }

        public bool HasSpace() => _passengers.Count < Capacity;
        public bool IsColorMatch(CharacterColor color) => color == BusColor;
        
        public void AddPassenger(CharacterView passenger)
        {
            if (HasSpace())
            {
                _passengers.Add(passenger);
            }
        }
    }
}