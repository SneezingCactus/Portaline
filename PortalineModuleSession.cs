namespace Celeste.Mod.Portaline
{
  public class PortalineModuleSession : EverestModuleSession
  {
    public bool PortalGunEnabled { get; set; } = false;

    public bool oldEnabledConfig { get; set; } = false;
  }
}