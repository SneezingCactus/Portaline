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
        private readonly bool enabled;

        public PortalineGiveGunTrigger(EntityData data, Vector2 offset) : base(data, offset)
        {
            enabled = data.Bool("portalgunenabled");
        }

        public override void OnEnter(Player player)
        {
            base.OnEnter(player);
            PortalineModule.Session.PortalGunEnabled = enabled;
        }
    }
}
