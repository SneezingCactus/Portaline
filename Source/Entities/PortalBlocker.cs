using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

// empty class used to identify entities that block portals like the emancipation grill
[Tracked(true)]
[CustomEntity("Portaline/PortalBlocker")]
public class PortalBlocker : Entity {
  public PortalBlocker(Vector2 position) : base(position) { }
}
