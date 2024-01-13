using System;
using System.Collections.Generic;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using static Celeste.Mod.Portaline.PortalineModule;

namespace Celeste.Mod.Portaline {
  [Tracked(true)]
  [CustomEntity("Portaline/PortalGunGiver")]
  public class PortalGunGiver : PortalBlocker {
    struct GrillParticle {
      public Vector2 position;
      public int orientation;
    }

    public TileGrid Tiles;
    private List<GrillParticle> particles = new List<GrillParticle>();
    private float[] speeds = { 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 };
    private Color color;

    public bool enable;
    public bool horizontal;

    public PortalGunGiver(Vector2 position, float width, float height, bool enable) : base(position) {
      this.enable = enable;
      color = enable ? new Color(0x00, 0xff, 0x1b) : new Color(0xff, 0x51, 0x51);

      Depth = -10;
      int newSeed = Calc.Random.Next();
      Calc.PushRandom(newSeed);
      Calc.PopRandom();

      Collider = new Hitbox(width, height, 0, 0);
    }

    public PortalGunGiver(EntityData data, Vector2 offset) : this(data.Position + offset, data.Width, data.Height, data.Bool("enableGun", true)) { }

    public override void Awake(Scene scene) {
      base.Awake(scene);

      particles.Clear();

      if (horizontal) {
        for (int i = 0; i < Width * Height / 16f; i++) {
          particles.Add(new GrillParticle {
            position = new Vector2(Calc.Random.NextFloat(Width - 1f), Calc.Random.NextFloat(Height - 3f) + 1),
            orientation = 1,
          });
        }
      } else {
        for (int i = 0; i < Width * Height / 16f; i++) {
          particles.Add(new GrillParticle {
            position = new Vector2(Calc.Random.NextFloat(Width - 3f) + 1, Calc.Random.NextFloat(Height - 1f)),
            orientation = 1,
          });
        }
      }
    }

    public override void Update() {
      base.Update();

      int num = speeds.Length;
      int i = 0;
      if (horizontal) {
        for (int count = particles.Count; i < count; i++) {
          GrillParticle particle = particles[i];

          Vector2 value = particle.position + Vector2.UnitX * speeds[i % num] * particle.orientation * Engine.DeltaTime;
          if (value.X >= Width || value.X <= 0) particle.orientation *= -1;
          particle.position = value;

          particles[i] = particle;
        }
      } else {
        for (int count = particles.Count; i < count; i++) {
          GrillParticle particle = particles[i];

          Vector2 value = particle.position + Vector2.UnitY * speeds[i % num] * particle.orientation * Engine.DeltaTime;
          if (value.Y >= Height || value.Y <= 0) particle.orientation *= -1;
          particle.position = value;

          particles[i] = particle;
        }
      }
    }

    public override void Render() {
      base.Render();

      if (horizontal) {
        Draw.Rect(X, Y - 1, Width, 18, color * 0.2f);
      } else {
        Draw.Rect(X - 1, Y, 18, Height, color * 0.2f);
      }

      foreach (GrillParticle particle in particles) {
        Draw.Pixel.Draw(Position + particle.position, Vector2.Zero, color * 0.5f);
      }

      if (horizontal) {
        Instance.portalGunGiverEdgeTex.DrawJustified(
          Position - new Vector2(0, 2),
          new Vector2(1, 0),
          Color.White,
          new Vector2(1, 1),
          (float)Math.PI*1.5f
        );

        Instance.portalGunGiverEdgeTex.DrawJustified(
          Position + new Vector2(Width, Height + 2),
          new Vector2(1, 0),
          Color.White,
          new Vector2(1, 1),
          (float)Math.PI*0.5f,
          0
        );
      } else {
        Instance.portalGunGiverEdgeTex.DrawJustified(
          Position - new Vector2(2, 0),
          new Vector2(0, 0),
          Color.White,
          new Vector2(1, 1),
          0
        );

        Instance.portalGunGiverEdgeTex.DrawJustified(
          Position + new Vector2(Width + 2, Height),
          new Vector2(0, 0),
          Color.White,
          new Vector2(1, 1),
          (float)Math.PI,
          0
        );
      }

      Instance.portalGunGiverSymbolTex.DrawJustified(
        Position + new Vector2(Width / 2, Height / 2),
        new Vector2(0.5f, 0.5f),
        color,
        new Vector2(1, 1),
        0
      );
    }
  }
}
