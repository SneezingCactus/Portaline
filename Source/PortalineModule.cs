using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Monocle;
using MonoMod.ModInterop;

namespace Celeste.Mod.Portaline;

[ModImportName("GravityHelper")]
public static class GravityHelperImports {
  public static Func<bool> IsPlayerInverted;
}

public class PortalineModule : EverestModule {
  public static PortalineModule Instance { get; private set; }

  public override Type SettingsType => typeof(PortalineModuleSettings);
  public static PortalineModuleSettings Settings => (PortalineModuleSettings)Instance._Settings;

  private static MouseState State => Mouse.GetState();

  private Texture2D aimTex;
  private Texture2D aimTexBlue;
  private Texture2D aimTexOrange;
  public MTexture emancipationGrillTex;
  public MTexture portalGunGiverEdgeTex;
  public MTexture portalGunGiverSymbolTex;
  public MTexture portalTex;
  public MTexture gunTex;

  public PortalEntity bluePortal;
  public PortalEntity orangePortal;
  public bool gunEnabledInLevel;

  private VirtualJoystick joystickAim;
  private Vector2 oldJoystickAim;
  private Vector2 MouseCursorPos => Vector2.Transform(new Vector2(State.X, State.Y), Matrix.Invert(Engine.ScreenMatrix));
  private Vector2 oldMouseCursorPos = Vector2.Zero;
  private Vector2 CursorPos = Vector2.Zero;
  private bool usingJoystickAim = false;

  public PortalineModule() {
    Instance = this;
  }

  [Obsolete]
  public override void LoadContent() {
    aimTex = GFX.Game["Portaline/AimIndicator"].Texture.Texture;
    aimTexBlue = GFX.Game["Portaline/AimIndicatorBlue"].Texture.Texture;
    aimTexOrange = GFX.Game["Portaline/AimIndicatorOrange"].Texture.Texture;
    emancipationGrillTex = GFX.Game["Portaline/EmancipationGrill"];
    portalGunGiverEdgeTex = GFX.Game["Portaline/PortalGunGiver/edge"];
    portalGunGiverSymbolTex = GFX.Game["Portaline/PortalGunGiver/symbol"];
    gunTex = GFX.Game["Portaline/Gun"];
    portalTex = GFX.Game["Portaline/Portal"];
  }

  public override void Load() {
    typeof(GravityHelperImports).ModInterop();

    On.Celeste.Player.Render += PlayerRender;
    On.Celeste.Player.Update += PlayerUpdate;
    On.Celeste.Player.OnCollideH += PlayerCollideH;
    On.Celeste.Player.OnCollideV += PlayerCollideV;
    On.Celeste.Level.Render += LevelRender;
    On.Celeste.Level.Update += LevelUpdate;
    On.Celeste.Level.LoadLevel += LevelBegin;
  }


  public override void Unload() {
    On.Celeste.Player.Render -= PlayerRender;
    On.Celeste.Player.Update -= PlayerUpdate;
    On.Celeste.Player.OnCollideH -= PlayerCollideH;
    On.Celeste.Player.OnCollideV -= PlayerCollideV;
    On.Celeste.Level.Render -= LevelRender;
    On.Celeste.Level.Update -= LevelUpdate;
    On.Celeste.Level.LoadLevel -= LevelBegin;
  }

  public override void OnInputInitialize() {
    base.OnInputInitialize();
    joystickAim = new VirtualJoystick(true, new VirtualJoystick.PadRightStick(Input.Gamepad, 0.1f));
  }

  public override void OnInputDeregister() {
    base.OnInputDeregister();
    joystickAim?.Deregister();
  }

  private void LevelBegin(On.Celeste.Level.orig_LoadLevel orig, Level self, Player.IntroTypes playerIntro, bool isFromLoader) {
    self.Add(new EmancipationGrillRenderer());
    gunEnabledInLevel = false;
    orig(self, playerIntro, isFromLoader);
  }

  private void LevelRender(On.Celeste.Level.orig_Render orig, Level self) {
    orig(self);

    if (!(Settings.PortalGunOverrideEnable || gunEnabledInLevel)) return;

    Draw.SpriteBatch.Begin(0, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Engine.ScreenMatrix);
    Draw.SpriteBatch.Draw(aimTex, CursorPos, null, Color.White, 0f, new Vector2(aimTex.Width / 2f, aimTex.Height / 2f), 4f, 0, 0f);
    if (bluePortal != null) Draw.SpriteBatch.Draw(aimTexBlue, CursorPos, null, Color.White, 0f, new Vector2(aimTex.Width / 2f, aimTex.Height / 2f), 4f, 0, 0f);
    if (orangePortal != null) Draw.SpriteBatch.Draw(aimTexOrange, CursorPos, null, Color.White, 0f, new Vector2(aimTex.Width / 2f, aimTex.Height / 2f), 4f, 0, 0f);
    Draw.SpriteBatch.End();
  }

  private void LevelUpdate(On.Celeste.Level.orig_Update orig, Level self) {
    orig(self);

    // portal gun enabled / in cutscene check
    if ((!(Settings.PortalGunOverrideEnable || gunEnabledInLevel) || self.InCutscene) && (bluePortal != null || orangePortal != null)) {
      Audio.Play("event:/sneezingcactus/portal_remove");
      bluePortal?.Kill();
      orangePortal?.Kill();
    }

    // portal existance check
    if (bluePortal != null && bluePortal.dead) {
      bluePortal = null;
    }
    if (orangePortal != null && orangePortal.dead) {
      orangePortal = null;
    }
  }

  private void PlayerRender(On.Celeste.Player.orig_Render orig, Player self) {
    orig(self);

    if (!(Settings.PortalGunOverrideEnable || gunEnabledInLevel)) return;

    Vector2 gunVector = ToCursor(self, CursorPos);

    SpriteEffects flip = SpriteEffects.None;
    float gunRotation = Math.Min(Math.Max(ToRotation(gunVector), -1.2f), 1.2f);

    if (self.Facing == Facings.Left) {
      flip = SpriteEffects.FlipVertically;

      if (gunVector.Y > 0) {
        gunRotation = Math.Max(ToRotation(gunVector), (float)Math.PI - 1.2f);
      } else {
        gunRotation = Math.Min(ToRotation(gunVector), -(float)Math.PI + 1.2f);
      }
    }

    gunTex.DrawJustified(
      self.Center,
      new Vector2(0.3f, 0.5f),
      Color.White,
      1f,
      gunRotation,
      flip
    );
  }

  private void PlayerUpdate(On.Celeste.Player.orig_Update orig, Player self) {
    orig(self);

    foreach (PortalGunGiver entity in self.Scene.Tracker.GetEntities<PortalGunGiver>()) {
      if (self.CollideCheck(entity)) {
        if (entity.enable && !Instance.gunEnabledInLevel) {
          Audio.Play("event:/sneezingcactus/portalgun_activation");
        }
        Instance.gunEnabledInLevel = entity.enable;
      }
    }

    if (!(Settings.PortalGunOverrideEnable || gunEnabledInLevel)) return;

    // cursor pos update
    if (joystickAim.Value.LengthSquared() > 0.04f) {
      usingJoystickAim = true;
    } else if (MouseCursorPos != oldMouseCursorPos) {
      usingJoystickAim = false;
    }

    if (usingJoystickAim && self.Scene != null) {
      CursorPos = (PlayerPosScreenSpace(self) + oldJoystickAim * 70f) * 6f;
      if (joystickAim.Value.LengthSquared() > 0.04f) {
        oldJoystickAim = joystickAim.Value;
      }
    } else {
      CursorPos = MouseCursorPos;
    }

    oldMouseCursorPos = MouseCursorPos;

    if (self.Scene == null || self.Scene.TimeActive <= 0f || (TalkComponent.PlayerOver != null && Input.Talk.Pressed)) {
      return;
    }

    foreach (PortalBlocker entity in self.Scene.Tracker.GetEntities<PortalBlocker>()) {
      if (self.CollideCheck(entity)) {
        if (bluePortal != null || orangePortal != null) {
          Audio.Play("event:/sneezingcactus/portal_remove");
          bluePortal?.Kill();
          orangePortal?.Kill();
          bluePortal = null;
          orangePortal = null;
        }
        return;
      }
    }

    if (Settings.RemovePortals.Pressed) {
      Audio.Play("event:/sneezingcactus/portal_remove");
      bluePortal?.Kill();
      orangePortal?.Kill();
      bluePortal = null;
      orangePortal = null;
    }
    if (Settings.ShootBluePortal.Pressed || MInput.Mouse.PressedLeftButton) {
      self.Facing = (Facings)Math.Sign(ToCursor(self, CursorPos).X);
      if (self.Facing == 0) self.Facing = Facings.Right;
      Audio.Play("event:/sneezingcactus/portal_shoot_blue");
      new PortalBullet(self.Center, ToCursor(self, CursorPos) * 15f, false, self);
    }
    if (Settings.ShootOrangePortal.Pressed || MInput.Mouse.PressedRightButton) {
      self.Facing = (Facings)Math.Sign(ToCursor(self, CursorPos).X);
      if (self.Facing == 0) self.Facing = Facings.Right;
      Audio.Play("event:/sneezingcactus/portal_shoot_orange");
      new PortalBullet(self.Center, ToCursor(self, CursorPos) * 15f, true, self);
    }
  }

  private void PlayerCollideH(On.Celeste.Player.orig_OnCollideH orig, Player self, CollisionData data) {
    if (!(Settings.PortalGunOverrideEnable || gunEnabledInLevel)) {
      orig(self, data);
      return;
    }

    bool changedMe = false;

    changedMe = (Instance.bluePortal?.HighPriorityUpdate(self) ?? false) || changedMe;
    changedMe = (Instance.orangePortal?.HighPriorityUpdate(self) ?? false) || changedMe;

    if (changedMe) return;

    orig(self, data);
  }

  private void PlayerCollideV(On.Celeste.Player.orig_OnCollideV orig, Player self, CollisionData data) {
    if (!(Settings.PortalGunOverrideEnable || gunEnabledInLevel)) {
      orig(self, data);
      return;
    }

    bool changedMe = false;

    changedMe = (Instance.bluePortal?.HighPriorityUpdate(self) ?? false) || changedMe;
    changedMe = (Instance.orangePortal?.HighPriorityUpdate(self) ?? false) || changedMe;

    if (changedMe) return;

    orig(self, data);
  }

  private static float ToRotation(Vector2 vector) {
    return (float)Math.Atan2(vector.Y, vector.X);
  }

  private static Vector2 ToCursor(Actor player, Vector2 cursorPos) {
    return Vector2.Normalize(cursorPos / 6f - PlayerPosScreenSpace(player));
  }

  private static Vector2 PlayerPosScreenSpace(Actor self) {
    return self.Center - (self.Scene as Level).Camera.Position;
  }
}
