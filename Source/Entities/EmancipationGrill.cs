using Monocle;
using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;
using System.Collections.Generic;
using System;
using static Celeste.Mod.Portaline.PortalineModule;
using System.Linq;

namespace Celeste.Mod.Portaline;

[CustomEntity("Portaline/EmancipationGrill")]
public class EmancipationGrill : PortalBlocker {
  public struct GrillParticle {
    public Vector2 position;
    public int orientation;

    // don't let anyone accidentally modify the particle speeds by using an IReadOnlyList<float>
    public static readonly IReadOnlyList<float> speeds = Enumerable.Range(4, 10).Select(i => (float)i).ToList();
  }

  public TileGrid Tiles;
  private readonly List<GrillParticle> particles = [];

  public bool horizontal;
  public bool obstructable;

  public bool obstructed = false;
  public float barrierOpacity = 1;

  public EmancipationGrill(Vector2 position, float width, float height, bool obstructable) : base(position) {
    this.obstructable = obstructable;

    Depth = -10;
    int newSeed = Calc.Random.Next();
    Calc.PushRandom(newSeed);
    Calc.PopRandom();

    Collider = new Hitbox(width, height, 0, 0);
  }

  public EmancipationGrill(EntityData data, Vector2 offset) : this(data.Position + offset, data.Width, data.Height, data.Bool("obstructable", false)) { }

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

    int num = GrillParticle.speeds.Count;
    int i = 0;
    if (horizontal) {
      for (int count = particles.Count; i < count; i++) {
        GrillParticle particle = particles[i];

        Vector2 value = particle.position + Vector2.UnitX * GrillParticle.speeds[i % num] * particle.orientation * Engine.DeltaTime;
        if (value.X >= Width || value.X <= 0) particle.orientation *= -1;
        particle.position = value;

        particles[i] = particle;
      }
    } else {
      for (int count = particles.Count; i < count; i++) {
        GrillParticle particle = particles[i];

        Vector2 value = particle.position + Vector2.UnitY * GrillParticle.speeds[i % num] * particle.orientation * Engine.DeltaTime;
        if (value.Y >= Height || value.Y <= 0) particle.orientation *= -1;
        particle.position = value;

        particles[i] = particle;
      }
    }

    if (!obstructable) return;

    Rectangle obstructionRect;

    if (horizontal) {
      obstructionRect = new((int)Position.X, (int)Position.Y + 1, (int)Width, (int)Height - 2);
    } else {
      obstructionRect = new((int)Position.X + 1, (int)Position.Y, (int)Width - 2, (int)Height);
    }

    obstructed = Scene.CollideFirst<Solid>(obstructionRect) != null;
    Collidable = !obstructed;
  }

  public override void Render() {
    base.Render();

    MTexture edgeTexture;

    if (obstructed) {
      edgeTexture = Instance.emanciGrillEdgeInactiveTex;

      barrierOpacity -= barrierOpacity * 0.5f;
    } else {
      edgeTexture = Instance.emanciGrillEdgeActiveTex;

      barrierOpacity += (1 - barrierOpacity) * 0.2f;
    }

    Color color = Color.RoyalBlue * 0.5f;
    foreach (GrillParticle particle in particles) {
      Draw.Pixel.Draw(Position + particle.position, Vector2.Zero, color * barrierOpacity);
    }

    if (horizontal) {
      edgeTexture.DrawJustified(
        Position + new Vector2(0, Height),
        new Vector2(0, 0),
        Color.White,
        new Vector2(1, 1),
        (float)Math.PI*1.5f
      );

      edgeTexture.DrawJustified(
        Position + new Vector2(Width, 0),
        new Vector2(0, 0),
        Color.White,
        new Vector2(1, 1),
        (float)Math.PI*0.5f
      );
    } else {
      edgeTexture.DrawJustified(
        Position,
        new Vector2(0, 0),
        Color.White,
        new Vector2(1, 1),
        0
      );

      edgeTexture.DrawJustified(
        Position + new Vector2(Width, Height),
        new Vector2(0, 0),
        Color.White,
        new Vector2(1, 1),
        (float)Math.PI,
        0
      );
    }
  }

  public override void Added(Scene scene) {
    base.Added(scene);
    scene.Tracker.GetEntity<EmancipationGrillRenderer>().Track(this);
  }

  public override void Removed(Scene scene) {
    base.Removed(scene);
    scene.Tracker.GetEntity<EmancipationGrillRenderer>().Untrack(this);
  }
}
