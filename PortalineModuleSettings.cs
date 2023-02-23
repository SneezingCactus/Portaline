using Microsoft.Xna.Framework.Input;

namespace Celeste.Mod.Portaline {
  [SettingName("Portaline Settings")]
  public class PortalineModuleSettings : EverestModuleSettings {
    [SettingName("Portal Gun Enabled")]
    [SettingSubText("If you disable it in-game, your portals will also disappear. \nNote: In addition to the keyboard and joystick settings below,\nyou can also shoot blue portals with the left mouse button,\nand orange portals with the right mouse button.")]
    public bool PortalGunEnabled { get; set; } = false;

    [DefaultButtonBinding(Buttons.LeftShoulder, Keys.O)]
    public ButtonBinding ShootBluePortal { get; set; }

    [DefaultButtonBinding(Buttons.RightShoulder, Keys.P)]
    public ButtonBinding ShootOrangePortal { get; set; }

    [DefaultButtonBinding(Buttons.RightStick, Keys.Q)]
    public ButtonBinding RemovePortals { get; set; }
  }
}
