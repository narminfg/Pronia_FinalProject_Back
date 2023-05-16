using Pronia.Models;

namespace Pronia.ViewModels.HomeViewModels
{
    public class HomeVM
    {
        public IEnumerable<Slider>? Sliders { get; set; }
        public IEnumerable<Product> Featured { get; set; }
        public IEnumerable<Product> BestSeller { get; set; }
        public IEnumerable<Product> Latest { get; set; }
        public IEnumerable<Product> New { get; set; }
    }
}
