using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.Portaline;

[CustomEntity("Portaline/PortalGunGiverVertical")]
public class PortalGunGiverVertical : PortalGunGiver {
  public PortalGunGiverVertical(EntityData data, Vector2 offset) : base(data, offset) {
    horizontal = false;
  }
}
