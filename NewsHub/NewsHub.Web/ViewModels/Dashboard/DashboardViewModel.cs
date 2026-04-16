using NewsHub.Web.ViewModels.Source;

namespace NewsHub.Web.ViewModels.Dashboard
{
    public class DashboardViewModel
    {
        public SourceViewModel NewSource { get; set; } = new();
        public List<ListSourceViewViewModel> Sources { get; set; } = new();
        public EditSourceViewModel EditSource { get; set; } = new();
        public DeleteSourceViewModel DeleteSource { get; set; } = new();

        public int TotalSources { get { return Sources.Count; } set; }
    }
}
