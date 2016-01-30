using Grabacr07.KanColleWrapper;
using Grabacr07.KanColleWrapper.Models;
using System.Collections.Generic;
using System.Linq;

namespace PresetViewerPlugin
{
    static class Extensions
    {
        public static double CalcViewRange(this Ship[] ships)
        {
            var logic = ViewRangeCalcLogic.Get(KanColleClient.Current.Settings.ViewRangeCalcType);
            return logic.Calc(ships);
        }

        public static int CalcMaxAirSuperiorityPotential(this Ship[] ships)
        {
            return ships.Sum(x => x.CalcMaxAirSuperiorityPotential());
        }

        public static int CalcMinAirSuperiorityPotential(this Ship[] ships)
        {
            return ships.Sum(x => x.CalcMinAirSuperiorityPotential());
        }
    }
}
