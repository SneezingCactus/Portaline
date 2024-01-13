using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.Portaline;

// empty class used to identify entities that block portals like the emancipation grill
[Tracked(true)]
[CustomEntity("Portaline/PortalBlocker")]
public class PortalBlocker(Vector2 position) : Entity(position) {
}
