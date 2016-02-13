using Grabacr07.KanColleViewer.Composition;
using System.ComponentModel.Composition;

namespace PresetViewerPlugin
{
    [Export(typeof(IPlugin))]
    [Export(typeof(ITool))]
    [ExportMetadata("Guid", "53F7734F-5CB8-47DB-9B7D-604A742FB1D6")]
    [ExportMetadata("Title", "PresetViewer")]
    [ExportMetadata("Description", "プリセット一覧を表示します。")]
    [ExportMetadata("Version", "1.0.3")]
    [ExportMetadata("Author", "@ruhiel_murrue")]
    class PresetViewerPlugin : ITool, IPlugin
    {
        public string Name => "PresetViewer";

        public object View => new UserControl1 { DataContext = new PresetViewModel() };

        public void Initialize() { }
    }
}
