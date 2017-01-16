using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameData
{
    public class PlayerData
    {
        public Guid PlayerID { get; set; }
        public string Name { get; set; }
        public string GamerTag { get; set; }
        public string Password { get; set; }
        public int XP { get; set; }
    }
}
