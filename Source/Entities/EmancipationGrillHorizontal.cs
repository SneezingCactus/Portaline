using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.Portaline {
  [CustomEntity("Portaline/EmancipationGrillHorizontal")]
  public class EmancipationGrillHorizontal : EmancipationGrill {
    public EmancipationGrillHorizontal(EntityData data, Vector2 offset) : base(data, offset) {
      horizontal = true;
    }
  }
}
