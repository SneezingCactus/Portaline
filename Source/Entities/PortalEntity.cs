using Monocle;
using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework.Graphics;
using System;
using MonoMod.Utils;
using static Celeste.Mod.Portaline.PortalineModule;

namespace Celeste.Mod.Portaline;

[CustomEntity("Portaline/PortalEntity")]
public class PortalEntity : Entity {
  public bool dead;
  public int orientation;
  public bool isOrangePortal;
  public Solid owner;

  private Vector2 oldOwnerPos;
  private float scale = 0;

  public Rectangle Hitbox {
    get {
      if (orientation < 2) {
        return new((int)Position.X - 2, (int)Position.Y - 4, 4, 8);
      } else {
        return new((int)Position.X - 4, (int)Position.Y - 2, 8, 4);
      }
    }
  }

  public PortalEntity(Vector2 position, int orientation, bool isOrangePortal, Solid owner) {
    Position = position;
    this.orientation = orientation;
    this.isOrangePortal = isOrangePortal;
    this.owner = owner;
    oldOwnerPos = owner.Position;
    (owner.Scene as Level).Add(this);
  }

  public Vector2 getTpPosition(Player player) {
    Vector2 finalPos = Position;

    switch (orientation) {
      case 0: finalPos.X += 8; break;
      case 1: finalPos.X -= 8; break;
      case 2: finalPos.Y += 10; break;
      case 3: finalPos.Y -= 10; break;
    }

    //finalPos.X += MathF.Round(player.Width / 2);
    finalPos.Y += MathF.Round(player.Height / 2);

    return finalPos;
  }

  public bool CollisionCheck(Player executer) {
    if (Scene == null) return false;

    Player player = Scene.CollideFirst<Player>(Hitbox);

    if (player != null) {
      PortalEntity opposingPortal = isOrangePortal ? Instance.bluePortal : Instance.orangePortal;

      // check if opposing portal exists
      if (opposingPortal == null) return false;

      // check if opposing portal is obstructed
      float oriRotation = 0;

      switch (opposingPortal.orientation) {
        case 0: oriRotation = 0; break;
        case 1: oriRotation = (float)Math.PI; break;
        case 2: oriRotation = (float)Math.PI * 0.5f; break;
        case 3: oriRotation = (float)Math.PI * 1.5f; break;
      }

      Rectangle frontRect = opposingPortal.RectFromVectors(new Vector2(2, 0).Rotate(oriRotation), new Vector2(3, 16).Rotate(oriRotation));

      if (Scene.CollideFirst<Solid>(frontRect) != null) return false;

      // same orientation portals
      if (orientation == opposingPortal.orientation) {
        if (orientation < 2) {
          player.Speed.X *= -1;
          player.DashDir.X *= -1;
          player.Facing = player.Facing == Facings.Left ? Facings.Right : Facings.Left;
        } else {
          player.Speed.Y *= -1;
          player.DashDir.Y *= -1;
        }
      }

      bool isPlayerInverted = GravityHelperImports.IsPlayerInverted?.Invoke() ?? false;

      // this portal has top-bottom orientation and the other portal has left-right orientation
      if (orientation > 1 && opposingPortal.orientation < 2) {
        if ((opposingPortal.orientation == 0 && orientation == 3) ||
            (opposingPortal.orientation == 1 && orientation == 2)) {
          player.Speed = player.Speed.Rotate((float)Math.PI / -2);
          player.DashDir = player.DashDir.Rotate((float)Math.PI / -2);
        } else {
          player.Speed = player.Speed.Rotate((float)Math.PI / 2);
          player.DashDir = player.DashDir.Rotate((float)Math.PI / 2);
        }
        player.Facing = player.Speed.X >= 0 ? Facings.Right : Facings.Left;

        if (isPlayerInverted) {
          player.Speed *= -1;
          player.DashDir *= -1;
        }
      }

      // this portal has left-right orientation and the other portal has top-bottom orientation
      if (orientation < 2 && opposingPortal.orientation > 1) {
        if ((opposingPortal.orientation == 2 && orientation == 0) ||
            (opposingPortal.orientation == 3 && orientation == 1)) {
          player.Speed = player.Speed.Rotate((float)Math.PI / -2);
          player.DashDir = player.DashDir.Rotate((float)Math.PI / -2);
        } else {
          player.Speed = player.Speed.Rotate((float)Math.PI / 2);
          player.DashDir = player.DashDir.Rotate((float)Math.PI / 2);
        }
        player.Facing = player.Speed.X >= 0 ? Facings.Right : Facings.Left;

        if (isPlayerInverted) {
          player.Speed *= -1;
          player.DashDir *= -1;
        }
      }

      // this is to prevent auto jump thing when entering a top portal that leads to another top portal
      DynamicData.For(player).Set("varJumpTimer", 0f);

      // change player position to portal
      player.Position = opposingPortal.getTpPosition(player);

      Audio.Play("event:/sneezingcactus/portal_travel");

      return player == executer;
    }

    return false;
  }

  public bool PlacementCheck() {
    Scene scene = Engine.Scene;

    // portal overlap
    if ((isOrangePortal && Instance.bluePortal != null && (Position - Instance.bluePortal.Position).Length() < 16f) ||
        (!isOrangePortal && Instance.orangePortal != null && (Position - Instance.orangePortal.Position).Length() < 16f)) {
      Kill();
      return false;
    }

    float orientationAngle = 0;

    switch (orientation) {
      case 0: orientationAngle = 0; break;
      case 1: orientationAngle = (float)Math.PI; break;
      case 2: orientationAngle = (float)Math.PI * 0.5f; break;
      case 3: orientationAngle = (float)Math.PI * 1.5f; break;
    }

    for (int i = 0; i < 100; i++) {
      // obstruction (stuff in front of the portal)
      bool notObstructed = false;

      Rectangle frontLeft = RectFromVectors(new Vector2(2, -4).Rotate(orientationAngle), new Vector2(3, 8).Rotate(orientationAngle));
      Rectangle frontRight = RectFromVectors(new Vector2(2, 4).Rotate(orientationAngle), new Vector2(3, 8).Rotate(orientationAngle));

      Entity frontLeftBlocker = scene.CollideFirst<Solid>(frontLeft);
      Entity frontRightBlocker = scene.CollideFirst<Solid>(frontRight);

      frontLeftBlocker ??= scene.CollideFirst<PortalBlocker>(frontLeft);
      frontRightBlocker ??= scene.CollideFirst<PortalBlocker>(frontRight);

      if (frontLeftBlocker == null && frontRightBlocker == null) {
        // if the portal isn't obstructed at all
        notObstructed = true;
      } else if (frontLeftBlocker != null && frontRightBlocker != null) {
        // if the portal is completely obstructed
        Kill();
        return false;
      } else if (frontLeftBlocker != null) {
        // if the portal is obstructed from the left side
        Position += new Vector2(0, 1).Rotate(orientationAngle);
      } else {
        // if the portal is obstructed from the right side
        Position -= new Vector2(0, 1).Rotate(orientationAngle);
      }

      // off-surface (portal not being completely sticked to a surface)
      bool notOffSurface = false;

      Rectangle backLeft = RectFromVectors(new Vector2(-2, -8).Rotate(orientationAngle), new Vector2(3, 2).Rotate(orientationAngle));
      Rectangle backCenterLeft = RectFromVectors(new Vector2(-2, -4).Rotate(orientationAngle), new Vector2(3, 2).Rotate(orientationAngle));
      Rectangle backCenter = RectFromVectors(new Vector2(-2, 0).Rotate(orientationAngle), new Vector2(3, 2).Rotate(orientationAngle));
      Rectangle backCenterRight = RectFromVectors(new Vector2(-2, 4).Rotate(orientationAngle), new Vector2(3, 2).Rotate(orientationAngle));
      Rectangle backRight = RectFromVectors(new Vector2(-2, 8).Rotate(orientationAngle), new Vector2(3, 2).Rotate(orientationAngle));

      Solid backLeftSolid = scene.CollideFirst<Solid>(backLeft);
      Solid backCenterLeftSolid = scene.CollideFirst<Solid>(backCenterLeft);
      Solid backCenterSolid = scene.CollideFirst<Solid>(backCenter);
      Solid backCenterRightSolid = scene.CollideFirst<Solid>(backCenterRight);
      Solid backRightSolid = scene.CollideFirst<Solid>(backRight);

      if (backLeftSolid != null && backCenterLeftSolid != null && backCenterSolid != null && backCenterRightSolid != null && backRightSolid != null) {
        // if the portal is completely sticked to the surface
        notOffSurface = true;
      } else if (backLeftSolid == null && backRightSolid == null) {
        // if the portal is completely in the air (or placed on a single tile surface)
        Kill();
        return false;
      } else if (backLeftSolid == null) {
        // if the portal is sticked to the surface in the right side, but not the left
        Position += new Vector2(0, 1).Rotate(orientationAngle);
      } else {
        // if the portal is sticked to the surface in the left side, but not the right
        Position -= new Vector2(0, 1).Rotate(orientationAngle);
      }

      if (notObstructed && notOffSurface) {
        return true;
      }
    }

    // it shouldn't reach the end of the for loop if it was placed correctly, therefore Kill
    Kill();
    return false;
  }

  public Rectangle RectFromVectors(Vector2 position, Vector2 size) {
    return new(
      (int)Math.Round(Position.X + position.X - Math.Abs(size.X) / 2),
      (int)Math.Round(Position.Y + position.Y - Math.Abs(size.Y) / 2),
      (int)Math.Round(Math.Abs(size.X)),
      (int)Math.Round(Math.Abs(size.Y))
    );
  }

  public bool HighPriorityUpdate(Player executer) {
    return this?.CollisionCheck(executer) ?? false;
  }

  public override void Update() {
    base.Update();

    if (!dead) {
      if (owner != null) Position += owner.Position - oldOwnerPos;
      oldOwnerPos = owner.Position;
      CollisionCheck(null);
    }

    if (dead && Scene != null) {
      RemoveSelf();
    }
  }

  public override void Removed(Scene scene) {
    base.Removed(scene);
    Kill();
  }

  public override void SceneEnd(Scene scene) {
    base.SceneEnd(scene);
    Kill();
  }

  public override void Render() {
    base.Render();

    if (dead) return;

    Instance.portalTex.DrawCentered(
      Position,
      isOrangePortal ? Color.Orange : Color.Aqua,
      new Vector2(1, scale),
      orientation > 1 ? ((float)Math.PI) * 0.5f : 0,
      orientation == 1 || orientation == 3 ? SpriteEffects.FlipHorizontally : SpriteEffects.None
    );
    scale += (1 - scale) * 0.25f;
  }

  public void Kill() {
    dead = true;
    RemoveSelf();
  }
}
