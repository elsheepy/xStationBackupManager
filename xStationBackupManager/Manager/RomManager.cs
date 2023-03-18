using Autofac;
using System.Collections.Generic;
using System.IO;
using System.Windows.Documents;
using xStationBackupManager.Contracts;

namespace xStationBackupManager.Manager {
    internal class RomManager : IRomManager {
        private readonly ILifetimeScope _scope;

        public RomManager(ILifetimeScope scope) {
            _scope = scope;
        }

        public IRom[] GetRoms(string path) {
            if (!Directory.Exists(path)) return new IRom[0];

            var result = new List<IRom>();
            foreach (var file in Directory.GetFiles(path)) {
                if (!file.Contains(".7z")) continue;
                var rom = _scope.Resolve<IRom>();
                var fileInfo = new FileInfo(file);
                rom.Name = fileInfo.Name.Replace(fileInfo.Extension, string.Empty);
                rom.Path = file;
                result.Add(rom);
            }
            return result.ToArray();
        }
    }
}
