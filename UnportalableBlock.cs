using Monocle;
using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;
using Celeste;
using System.Collections.Generic;

[Tracked]
[CustomEntity("Portaline/UnportalableBlock")]
public class UnportalableBlock : Solid {
  public TileGrid Tiles;

	public char tileType;

	public UnportalableBlock master;

	public Point GroupBoundsMin;

	public Point GroupBoundsMax;

	public List<UnportalableBlock> Group;

	public bool HasGroup { get; private set; }

	public bool MasterOfGroup { get; private set; }

  public UnportalableBlock(Vector2 position, float width, float height, char tileType) : base(position, width, height, safe: true)
	{
		/*
		int newSeed = Calc.Random.Next();
		Calc.PushRandom(newSeed);
		Add(Tiles = GFX.FGAutotiler.GenerateBox(tileType, (int)width / 8, (int)height / 8).TileGrid);
		Calc.PopRandom();
		Add(new LightOcclude());
		Add(new TileInterceptor(Tiles, highPriority: false));*/
		Add(new LightOcclude());
		SurfaceSoundIndex = SurfaceIndex.TileToIndex[tileType];
		this.tileType = tileType;
	}

  public UnportalableBlock(EntityData data, Vector2 offset) : this(data.Position + offset, data.Width, data.Height, data.Char("tileType", '3')) { }

	public override void Awake(Scene scene)
	{
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		base.Awake(scene);
		if (!HasGroup)
		{
			MasterOfGroup = true;
			Group = new List<UnportalableBlock>();
			GroupBoundsMin = new Point((int)base.X, (int)base.Y);
			GroupBoundsMax = new Point((int)base.Right, (int)base.Bottom);
			AddToGroupAndFindChildren(this);
			Rectangle val = new(GroupBoundsMin.X / 8, GroupBoundsMin.Y / 8, (GroupBoundsMax.X - GroupBoundsMin.X) / 8 + 1, (GroupBoundsMax.Y - GroupBoundsMin.Y) / 8 + 1);
			VirtualMap<char> virtualMap = new VirtualMap<char>(val.Width, val.Height, '0');
			foreach (UnportalableBlock item in Group)
			{
				int num = (int)(item.X / 8f) - val.X;
				int num2 = (int)(item.Y / 8f) - val.Y;
				int num3 = (int)(item.Width / 8f);
				int num4 = (int)(item.Height / 8f);
				for (int i = num; i < num + num3; i++)
				{
					for (int j = num2; j < num2 + num4; j++)
					{
						virtualMap[i, j] = tileType;
					}
				}
			}
			Tiles = GFX.FGAutotiler.GenerateMap(virtualMap, new Autotiler.Behaviour
			{
				EdgesExtend = false,
				EdgesIgnoreOutOfLevel = false,
				PaddingIgnoreOutOfLevel = false
			}).TileGrid;
			Tiles.Position = new Vector2((float)GroupBoundsMin.X - base.X, (float)GroupBoundsMin.Y - base.Y);
			Add(Tiles);
		}
		// TryToInitPosition();
	}

	private void AddToGroupAndFindChildren(UnportalableBlock from)
	{
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_0254: Unknown result type (might be due to invalid IL or missing references)
		if (from.X < GroupBoundsMin.X)
		{
			GroupBoundsMin.X = (int)from.X;
		}
		if (from.Y < GroupBoundsMin.Y)
		{
			GroupBoundsMin.Y = (int)from.Y;
		}
		if (from.Right > GroupBoundsMax.X)
		{
			GroupBoundsMax.X = (int)from.Right;
		}
		if (from.Bottom > GroupBoundsMax.Y)
		{
			GroupBoundsMax.Y = (int)from.Bottom;
		}
		from.HasGroup = true;
		Group.Add(from);
		if (from != this)
		{
			from.master = this;
		}
		foreach (UnportalableBlock entity in Scene.Tracker.GetEntities<UnportalableBlock>())
		{
			if (!entity.HasGroup && entity.tileType == tileType && (base.Scene.CollideCheck(new Rectangle((int)from.X - 1, (int)from.Y, (int)from.Width + 2, (int)from.Height), entity) || base.Scene.CollideCheck(new Rectangle((int)from.X, (int)from.Y - 1, (int)from.Width, (int)from.Height + 2), entity)))
			{
				AddToGroupAndFindChildren(entity);
			}
		}
	}
}