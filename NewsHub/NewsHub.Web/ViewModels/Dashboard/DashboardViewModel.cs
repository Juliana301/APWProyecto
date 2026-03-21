using NewsHub.Web.ViewModels.Source;

namespace NewsHub.Web.ViewModels.Dashboard
{
    public class DashboardViewModel
    {
        public SourceViewModel NewSource { get; set; } = new();
        public List<SourceViewModel> Sources { get; set; } = new();
    }
}
