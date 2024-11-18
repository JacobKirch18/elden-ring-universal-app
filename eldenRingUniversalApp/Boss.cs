using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace eldenRingUniversalApp
{
    public class BossWrapper
    {
        public List<BossViewModel> Data { get; set; }
    }

    public class Boss
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Nickname { get; set; } = "Not given"; // Set default value
        public string Image { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public string[] Drops { get; set; }
        public string HealthPoints { get; set; }

        
    }
}
