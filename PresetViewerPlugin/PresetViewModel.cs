using System;
using Grabacr07.KanColleWrapper;
using Grabacr07.KanColleWrapper.Models;
using Livet;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Runtime.Serialization.Json;
using System.IO;
using MetroTrilithon.Mvvm;
using System.Collections.Generic;

namespace PresetViewerPlugin
{
    public class PresetViewModel : ViewModel
    {
        private const int MaxPresetNum = 8;
        private object syncObject = new object();

        static string FilePath => "PresetViewPlugin.txt";

        DataContractJsonSerializer serializer;

        private string _SelectPreset;
        private string SelectPreset
        {
            get
            {
                if (_SelectPreset == null) _SelectPreset = "1";
                return _SelectPreset;
            }
            set
            {
                _SelectPreset = value;
                UpdatePresetFleet();
            }
        }

        private preset_deck _PresetDeck;

        private preset_deck PresetDeck
        {
            get
            {
                if (_PresetDeck == null)
                {

                    _PresetDeck = readFromFile();
                }
                return _PresetDeck;
            }
            set
            {
                _PresetDeck = value;
                
                UpdatePresetFleet();
            }
        }
        private string _DeckInfo;

        public string DeckInfo
        {
            get
            {
                return _DeckInfo;
            }
            set
            {
                _DeckInfo = value;
                this.RaisePropertyChanged();
            }
        }

        private string[] _DeckNames;

        public string[] DeckNames
        {
            get
            {
                return _DeckNames;
            }
            set
            {
                _DeckNames = value;
                this.RaisePropertyChanged();
            }
        }

        public PresetViewModel()
        {
            UpdatePresetDeckName();

            var settings = new DataContractJsonSerializerSettings();
            settings.UseSimpleDictionaryFormat = true;
            serializer = new DataContractJsonSerializer(typeof(preset_deck), settings);

            var proxy = KanColleClient.Current.Proxy;

            proxy.ApiSessionSource
                .Where(x => x.Request.PathAndQuery == "/kcsapi/api_get_member/preset_deck")
                .Subscribe(x => this.UpdatePresetDeck(x.Response.BodyAsString));

            KanColleClient.Current.Homeport.Organization
                .Subscribe(nameof(Organization.Ships), this.UpdatePresetFleet);

            PresetDeck = readFromFile();
        }

        #region _PresetFleet
        private Ship[] _PresetFleet;
        public Ship[] PresetFleet
        {
            get
            {
                return _PresetFleet;
            }
            set
            {
                _PresetFleet = value;
                this.RaisePropertyChanged();
            }
        }
        #endregion

        public void UpdatePresetDeck(String responseBody)
        {

            var bytes = Encoding.UTF8.GetBytes(responseBody.Replace("svdata=", ""));
            using (var stream = new MemoryStream(bytes))
            {
                PresetDeck = serializer.ReadObject(stream) as preset_deck;
            }
        }

        private void UpdatePresetFleet()
        {
            if (PresetDeck == null || !PresetDeck.api_data.api_deck.ContainsKey(SelectPreset)) return;

            try
            {
                lock (syncObject)
                {
                    using (var stream = new MemoryStream())
                    {
                        serializer.WriteObject(stream, PresetDeck);
                        File.WriteAllText(FilePath, Encoding.UTF8.GetString(stream.ToArray()), new UTF8Encoding());
                    }
                }
            }
            catch (Exception)
            {
                // 必須の機能でもないので握りつぶし
            }

            Ship[] presets = new Ship[] { };
            var deck = PresetDeck.api_data.api_deck[SelectPreset].api_ship
                .Select((shipid, index) => new { shipid, index }).ToList();

            var list =
            from ships in KanColleClient.Current.Homeport.Organization.Ships
            join decks in deck on ships.Value.Id equals decks.shipid
            orderby decks.index
            select ships.Value;

            PresetFleet = list.ToArray();

            DeckInfo = PresetDeck.api_data.api_deck[SelectPreset].api_name +
                " (索敵能力:" + PresetFleet.CalcViewRange().ToString("#.##") +
                "    制空戦力:" + PresetFleet.CalcMinAirSuperiorityPotential()
                + " - " + PresetFleet.CalcMaxAirSuperiorityPotential() + ")";

            UpdatePresetDeckName(PresetDeck.api_data.api_deck);
        }

        private void UpdatePresetDeckName(Dictionary<string, api_deck> dictionary = null)
        {
            string[] names = Enumerable.Repeat<string>("編成なし", MaxPresetNum).ToArray();

            if (dictionary != null)
            {
                Enumerable.Range(1, MaxPresetNum)
                    .Where(i => dictionary.ContainsKey(i.ToString()))
                    .ToList()
                    .ForEach(i => names[i - 1] = dictionary[i.ToString()].api_name);
            }

            DeckNames = names;
        }

        private preset_deck readFromFile()
        {
            try
            {
                using (var stream = File.OpenRead(FilePath))
                {
                    return serializer.ReadObject(stream) as preset_deck;
                }
            }
            catch (Exception)
            {
            }

            return null;
        }

        public void SetDeck(String number)
        {
            SelectPreset = number;
        }
    }
}
