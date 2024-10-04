using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HSBGHelper.Server.Models {
    public class GreaterTrinket {
        [Key]
        public int Id { get; set;}
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public string HtmlGuide { get; set; }

    }
}