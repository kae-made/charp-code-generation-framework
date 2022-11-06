using Kae.Utility.Logging;
using Microsoft.Azure.Amqp.Framing;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kae.DomainModel.Csharp.Framework.ExternalEntities.AzureIoTHub
{
    public class AzureIoTHubImpl : AIHWrapper
    {
        protected static AzureIoTHubImpl instance;

        public override async Task<(string resultPayload, int status)> InvokeOperation(string name, string payload, string deviceId, string moduleName)
        {
            var method = new CloudToDeviceMethod(name);
            if (!string.IsNullOrEmpty(payload))
            {
                method.SetPayloadJson(payload);
            }
            CloudToDeviceMethodResult invocationResult = null;
            if (string.IsNullOrEmpty(moduleName))
            {
                invocationResult = await serviceClient.InvokeDeviceMethodAsync(deviceId, method);
            }
            else
            {
                invocationResult = await serviceClient.InvokeDeviceMethodAsync(deviceId, moduleName, method);
            }
            return (invocationResult.GetPayloadAsJson(), invocationResult.Status);
        }

        public override async Task SendCommand(string command, string deviceId, string moduleName = null)
        {
            var sendingMessage = new Message(System.Text.Encoding.UTF8.GetBytes(command));
            if (string.IsNullOrEmpty(moduleName))
            {
                await serviceClient.SendAsync(deviceId, sendingMessage);
            }
            else
            {
                await serviceClient.SendAsync(deviceId, moduleName, sendingMessage);
            }
        }

        public override async Task UpdateProperty(string name, object value, string deviceId, string moduleName = "")
        {
            Twin currentTwin = null;
            if (string.IsNullOrEmpty(moduleName))
            {
                currentTwin = await registryManager.GetTwinAsync(deviceId);
            }
            else
            {
                currentTwin = await registryManager.GetTwinAsync(deviceId, moduleName);
            }
            var updateTwin = new
            {
                properties = new
                {
                    desired = new
                    {
                        name = value
                    }

                }
            };
            string dtPatch = Newtonsoft.Json.JsonConvert.SerializeObject(updateTwin);
            if (string.IsNullOrEmpty(moduleName))
            {
                await registryManager.UpdateTwinAsync(deviceId, dtPatch, currentTwin.ETag);
            }
            else
            {
                await registryManager.UpdateTwinAsync(deviceId, moduleName, dtPatch, currentTwin.ETag);
            }
        }
    }
}
