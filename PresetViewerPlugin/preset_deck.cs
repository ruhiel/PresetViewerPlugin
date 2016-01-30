using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PresetViewerPlugin
{

    public class preset_deck
    {
        public int api_result { get; set; }
        public string api_result_msg { get; set; }
        public api_data api_data { get; set; }
    }

    public class api_data
    {
        public int api_max_num { get; set; }
        public Dictionary<string, api_deck> api_deck { get; set; }
    }

    public class api_deck
    {
        public int api_preset_no { get; set; }
        public string api_name { get; set; }
        public string api_name_id { get; set; }
        public int[] api_ship { get; set; }
    }
}
