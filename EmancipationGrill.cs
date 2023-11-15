using Monocle;
using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;
using Celeste;
using System.Collections.Generic;
using System.Xml.Serialization;
using IL.YYProject.XXHash;
using System;
using System.Xml.Schema;
using static Celeste.Mod.Portaline.PortalineModule;

[Tracked]
[CustomEntity("Portaline/EmancipationGrill")]
public class EmancipationGrill : Entity {
	struct GrillParticle {
		public Vector2 position;
		public int orientation;
	}

  public TileGrid Tiles;
	private List<GrillParticle> particles = new List<GrillParticle>();
	private float[] speeds = { 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 };

  public EmancipationGrill(Vector2 position, float width, float height, char tileType) : base(position)
	{
		Depth = -10;
		int newSeed = Calc.Random.Next();
		Calc.PushRandom(newSeed);
		//Add(Tiles = GFX.FGAutotiler.GenerateBox(tileType, (int)width / 8, (int)height / 8).TileGrid);
		Calc.PopRandom();
		// Add(new LightOcclude());
		//Add(new TileInterceptor(Tiles, highPriority: false));

		Collider = new Hitbox(width, height, 0, 0);

		for (int i = 0; i < Width * Height / 16f; i++) {
			particles.Add(new GrillParticle{
				position = new Vector2(Calc.Random.NextFloat(Width - 3f) + 1, Calc.Random.NextFloat(Height - 1f)),
				orientation = 1,
			});
		}
	}

  public EmancipationGrill(EntityData data, Vector2 offset) : this(data.Position + offset, data.Width, data.Height, data.Char("tileType", '3')) { }

	public override void Update()
	{
		base.Update();

		int num = speeds.Length;
		float height = Height;
		int i = 0;
		for (int count = particles.Count; i < count; i++)
		{
			GrillParticle particle = particles[i];

			Vector2 value = particle.position + Vector2.UnitY * speeds[i % num] * particle.orientation * Engine.DeltaTime;
			//value.Y %= height - 1f;
			if (value.Y >= height || value.Y <= 0) particle.orientation *= -1;
			particle.position = value;

			particles[i] = particle;
		}
	}

  public override void Render()
	{
		base.Render();
		Color color = Color.RoyalBlue * 0.5f;
		foreach (GrillParticle particle in particles) {
			Draw.Pixel.Draw(Position + particle.position, Vector2.Zero, color);
		}

		Instance.emancipationGrillTex.DrawJustified(
			Position,
			new Vector2(0, 0),
			Color.White,
			new Vector2(1/*Width / 64f*/, 1),
			0
		);

		Instance.emancipationGrillTex.DrawJustified(
			Position + new Vector2(Width, Height),
			new Vector2(0, 0),
			Color.White,
			new Vector2(1/*Width / 64f*/, 1),
			(float)Math.PI,
			0
		);
	}

  public override void Added(Scene scene)
	{
		base.Added(scene);
		scene.Tracker.GetEntity<EmancipationGrillRenderer>().Track(this);
	}

	public override void Removed(Scene scene)
	{
		base.Removed(scene);
		scene.Tracker.GetEntity<EmancipationGrillRenderer>().Untrack(this);
	}
}