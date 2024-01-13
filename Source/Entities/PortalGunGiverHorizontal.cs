using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.Portaline;

[CustomEntity("Portaline/PortalGunGiverHorizontal")]
public class PortalGunGiverHorizontal : PortalGunGiver {
  public PortalGunGiverHorizontal(EntityData data, Vector2 offset) : base(data, offset) {
    horizontal = true;
  }
}
