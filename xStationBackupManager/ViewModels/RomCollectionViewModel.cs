using System.Collections.Generic;
using xStationBackupManager.Contracts;

namespace xStationBackupManager.ViewModels {
    public class RomCollectionViewModel : RomEntryViewModel {
        public override string Name { get; }

        public IRomCollection RomCollection;
        public List<RomEntryViewModel> Entrys { get; } = new List<RomEntryViewModel>();

        public RomCollectionViewModel(IRomCollection romCollection) {
            Name = romCollection.Name;
            RomCollection = romCollection;
            foreach (var collection in RomCollection.Collections) {
                Entrys.Add(new RomCollectionViewModel(collection));
            }
            foreach (var rom in RomCollection.Roms) {
                Entrys.Add(new RomViewModel(rom));
            }
        }
    }
}
