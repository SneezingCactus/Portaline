using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.Portaline
{
    [CustomEntity("Portaline/PortalineGiveGunTrigger")]
    internal class PortalineGiveGunTrigger : Trigger
    {
        private readonly bool givenGun;

        public PortalineGiveGunTrigger(EntityData data, Vector2 offset) : base(data, offset)
        {
            givenGun = data.Bool("givenGun");
        }
    }
}
