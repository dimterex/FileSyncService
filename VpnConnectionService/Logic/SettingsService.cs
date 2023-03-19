using System;
using System.IO;
using System.Reflection;

using Core.Publisher._Interfaces_;

using Newtonsoft.Json;

using ServicesApi;
using ServicesApi.Common;
using ServicesApi.Configuration;

using VpnConnectionService._Interfaces_;
using VpnConnectionService.Models;

public class SettingsService : ISettingsService
{
    private const string VPN_NAME = "VPN_NAME";
    private const string WORKING_SSID = "WORKING_SSID";
    private const string RUN_AS_USERNAME = "RUN_AS_USERNAME";
    #region Properties

    public SettingsModel Settings { get; }

    #endregion

    #region Constructors

    public SettingsService(IPublisherService publisherService)
    {
        var response = publisherService.CallAsync(QueueConstants.CONFIGURATION_QUEUE, new GetCredentialsRequest());
                    
        if (response is not StatusResponse statusResponse)
            throw new NullReferenceException($"{response}");

        if (statusResponse.Status == Status.Error)
            throw new Exception(statusResponse.Message.ToString());
                    
        var credentials = JsonConvert.DeserializeObject<CredentialDto>(statusResponse.Message.ToString());
        
        Settings = new SettingsModel()
        {
            Username = $"{credentials.Domain}\\{credentials.Login}",
            Password = credentials.Password,
            VpnName = GetEnvironmentValue(VPN_NAME),
            RunAsUsername = bool.Parse(GetEnvironmentValue(RUN_AS_USERNAME)),
            WorkSsid = GetEnvironmentValue(WORKING_SSID),
        };
    }

    #endregion

    private string GetEnvironmentValue(string id)
    {
        var value = Environment.GetEnvironmentVariable(id);
        if (string.IsNullOrEmpty(value))
            throw new NullReferenceException($"Environment {id}");
        return value;
    }
}
