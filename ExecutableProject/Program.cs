using System.Collections.Generic;
using System.Text;
using FileSystemProject;
using SdkProject;

namespace ExecutableProject
{
    using System;
    using System.Net;
    using System.Net.Sockets;

    class Program
    {
        /// <summary>
        /// TODO: Добавить DI.
        /// </summary>
        static void Main(string[] args)
        {
            var settingsManager = new SettingsManager();
            

            
            
         

            void SendToTelegram(string message)
            {
                telegramService.SendTextMessageAsync(message);
            }
            
            // TODO: Добавить CancellationToken
           

            ws.Stop();
        }
    }
}