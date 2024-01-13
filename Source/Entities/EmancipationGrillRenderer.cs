using System.Collections.Generic;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.Portaline;

[Tracked]
[CustomEntity("Portaline/EmancipationGrillRenderer")]
public class EmancipationGrillRenderer : Entity {
  private List<EmancipationGrill> list = new List<EmancipationGrill>();

  private Color theColor = Color.DeepSkyBlue * 0.5f;

  public EmancipationGrillRenderer() {
    Tag = Tags.Global | Tags.TransitionUpdate;
    Depth = 0;
    Add(new CustomBloom(OnRenderBloom));
  }

  public void Track(EmancipationGrill grill) {
    list.Add(grill);
  }

  public void Untrack(EmancipationGrill grill) {
    list.Remove(grill);
  }

  private void OnRenderBloom() {
    foreach (EmancipationGrill grill in list) {
      if (!grill.Visible) continue;

      if (grill.horizontal) {
        Draw.Rect(grill.X, grill.Y + 1, grill.Width, grill.Height - 2, theColor);
      } else {
        Draw.Rect(grill.X + 1, grill.Y, grill.Width - 2, grill.Height, theColor);
      }
    }
  }

  public override void Render() {
    if (list.Count <= 0) return;
    foreach (EmancipationGrill grill in list) {
      if (!grill.Visible) continue;

      Draw.Rect(grill.Collider, theColor * 0.5f);
    }
  }
}
