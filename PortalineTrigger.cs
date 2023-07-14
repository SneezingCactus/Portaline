using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.Portaline {
  [CustomEntity("Portaline/PortalineTrigger")]
  public class PortalineTrigger : Trigger {
    protected bool isEnable;

    public PortalineTrigger(EntityData data, Vector2 offset)
        : base(data, offset) {
      isEnable = data.Bool("is_enable", true);
    }

    public override void OnEnter(Player player) {
      base.OnEnter(player);
      PortalineModule.Session.PortalineEnabled = isEnable;
    }
  }
}