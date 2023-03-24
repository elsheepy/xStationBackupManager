using Autofac;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using xStationBackupManager.Contracts;
using xStationBackupManager.Enums;

namespace xStationBackupManager.Assigner {
    internal class RomCollectionAlphabetAssigner : IRomCollectionAssigner {
        private readonly ILifetimeScope _scope;

        public RomGroup RomGroup => RomGroup.Alphabet;

        public RomCollectionAlphabetAssigner(ILifetimeScope scope) {
            _scope = scope;
        }

        public IRomCollection[] AssignRomsToCollection(IRom[] roms) {
            // Build base dictionary
            var dictionary = new Dictionary<string, IRomCollection>();
            // Collection for numbers
            var collection = _scope.Resolve<IRomCollection>();
            collection.Name = "#";
            dictionary["#"] = collection;
            // Add Alphabet
            for (char alphabet = 'A'; alphabet <= 'Z'; alphabet++) {
                var letter = alphabet.ToString();
                collection = _scope.Resolve<IRomCollection>();
                collection.Name = letter;
                dictionary[letter] = collection;
            }
            // Add collection for all other stuff
            collection = _scope.Resolve<IRomCollection>();
            collection.Name = "~";
            dictionary["~"] = collection;


            // Assign roms
            foreach (var rom in roms) {
                var letter = rom.Name[0].ToString();
                var isDigit = Regex.Match(letter, @"\d").Success;
                if (isDigit) letter = "#";
                if(!dictionary.TryGetValue(letter, out collection)) dictionary.TryGetValue("~", out collection);
                if (collection == null) continue;
                collection.Roms.Add(rom);
            }

            // Create final list
            var result = new List<IRomCollection>();
            foreach(var kvp in dictionary) {
                if(kvp.Value.Roms.Count > 0) {
                    result.Add(kvp.Value);
                }
            }

            return result.ToArray();
        }
    }
}
