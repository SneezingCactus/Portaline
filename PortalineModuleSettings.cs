using Microsoft.Xna.Framework.Input;

namespace Celeste.Mod.Portaline {
  [SettingName("Portaline Settings")]
  public class PortalineModuleSettings : EverestModuleSettings {
    [SettingName("Portal Gun Override Enable")]
    [SettingSubText("By default, the portal gun is disabled upon spawning. If you set this to ON, the portal gun will always be enabled. \n\nNote: In addition to the keyboard and joystick settings below,\nyou can also shoot blue portals with the left mouse button,\nand orange portals with the right mouse button.")]
    public bool PortalGunOverrideEnable { get; set; } = false;

    [DefaultButtonBinding(Buttons.LeftShoulder, Keys.O)]
    public ButtonBinding ShootBluePortal { get; set; }

    [DefaultButtonBinding(Buttons.RightShoulder, Keys.P)]
    public ButtonBinding ShootOrangePortal { get; set; }

    [DefaultButtonBinding(Buttons.RightStick, Keys.Q)]
    public ButtonBinding RemovePortals { get; set; }
  }
}
