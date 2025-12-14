using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.Networking;

namespace _ClashRoyal.Scripts.Network
{
    public static class NetworkExtensions
    {
        public const string MainURL = "http://localhost/Projects/ClashRoyale/";
        public const string AuthorizationURL = MainURL + "Project/authorization.php";
        
        public static async Task PostFormAsync(string url, Dictionary<string, string> formData,
            Action<string> onSuccess = null, Action<string> onFailure = null)
        {
            using var www = UnityWebRequest.Post(url, formData);
            
            try
            {
                var operation = www.SendWebRequest();
                while (!operation.isDone)
                    await Task.Yield();

                if (www.result == UnityWebRequest.Result.Success)
                    onSuccess?.Invoke(www.downloadHandler.text);
                else
                    onFailure?.Invoke(www.error);
            }
            catch (Exception ex)
            {
                onFailure?.Invoke(ex.Message);
            }
        }
    }
}