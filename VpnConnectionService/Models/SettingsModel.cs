namespace VpnConnectionService.Models
{
    public class SettingsModel
    {
        #region Properties

        public string VpnName { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string WorkSsid { get; set; }
    
        public bool RunAsUsername { get; set; }
    
        #endregion
    }
}
