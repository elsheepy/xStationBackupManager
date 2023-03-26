using Autofac;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using xStationBackupManager.Contracts;
using xStationBackupManager.Enums;
using xStationBackupManager.Json;

namespace xStationBackupManager.Manager {
    public class OptionsManager : IOptionsManager {
        private readonly string _optionsPath = @"Options.json";
        private ILifetimeScope _scope;
        private Dictionary<Options, IOption> _options = new Dictionary<Options, IOption>();

        public OptionsManager(ILifetimeScope scope) {
            _scope = scope;

            if(File.Exists(_optionsPath)) {
                try {
                    var jsonTxt = File.ReadAllText(_optionsPath);
                    var json = JsonConvert.DeserializeObject<OptionsJson>(jsonTxt);
                    if (json == null) return;

                    foreach (var kvp in json.Options) {
                        if (!Enum.TryParse<Options>(kvp.Key, out var option)) continue;

                        var optionObj = _scope.Resolve<IOption>();
                        optionObj.Option = option;
                        optionObj.SetValue(kvp.Value);
                        _options.Add(option, optionObj);
                    }
                } catch(Exception ex) {
                    // Dann halt nicht
                }
            }
        }

        IOption IOptionsManager.GetOption(Options option) {
            if(_options.TryGetValue(option, out var optionObj)) return optionObj;

            var newOption = _scope.Resolve<IOption>();
            newOption.Option = option;
            _options.Add(option, newOption);
            return newOption;
        }

        public void Save() {
            try {
                var json = new OptionsJson();
                foreach (var kvp in _options) {
                    json.Options.Add(kvp.Key.ToString(), kvp.Value.GetValue());
                }

                var jsonTxt = JsonConvert.SerializeObject(json);
                File.WriteAllText(_optionsPath, jsonTxt);
            } catch (Exception ex) {
                MessageBox.Show($"{Resources.Localization.Resources.CanNotSave}\r\n\r\n{ex.Message}");
            }
        }

    }
}
