using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.Portaline;

[CustomEntity("Portaline/EmancipationGrillVertical")]
public class EmancipationGrillVertical : EmancipationGrill {
  public EmancipationGrillVertical(EntityData data, Vector2 offset) : base(data, offset) {
    horizontal = false;
  }
}
