using MarvelAPI.Models;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Text;
using System.Security.Cryptography;

namespace MarvelAPI.Rest
{
    public class MarvelRest
    {
        const string BaseUrl = "https://gateway.marvel.com";
            
        public MarvelRest(){}

        public T Execute<T>(RestRequest request) where T : new()
        {
            RestClient client = new RestClient();
            client.BaseUrl = new System.Uri(BaseUrl);
            Thread.Sleep(500);
            var response = client.Execute<T>(request);

            try
            {
                if (response.ErrorException != null)
                {
                    const string message = "Erro ao enviar informações. Verifique os detalhes do envio para mais informações.";
                    var fipeException = new ApplicationException(message, response.ErrorException);
                    return response.Data;
                    throw fipeException;
                }
                return response.Data;
            }
            catch (Exception ex)
            {
                string innerExceptionMessage = ex.InnerException?.Message;
                Console.BackgroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("Error: {0} | InnerError: {1} | Type: {2}", ex.Message, innerExceptionMessage, ex.ToString());
                Console.BackgroundColor = ConsoleColor.Black;
                return new T();
            }
        }

        public List<Result> GetCharacters()
        {
            var request = new RestRequest();
            request.RequestFormat = DataFormat.Json;
            string publickey = "668f798d129fc2a07015b8d13103af67";
            string privatekey = "a492c5096c61d669c7dd6c488cccd358f26771d3";
            string ts = DateTime.Now.Ticks.ToString();
            string hash = GerarHash(ts, publickey, privatekey);
            request.Resource = $"/v1/public/characters?ts={ts}&apikey={publickey}&hash={hash}";
            request.RootElement = "GetCharacters";

            var listCharacters = Execute<Rootobject>(request);
            return listCharacters.data.results;
        }

        private string GerarHash(
           string ts, string publicKey, string privateKey)
        {
            byte[] bytes =
                Encoding.UTF8.GetBytes(ts + privateKey + publicKey);
            var gerador = MD5.Create();
            byte[] bytesHash = gerador.ComputeHash(bytes);
            return BitConverter.ToString(bytesHash)
                .ToLower().Replace("-", String.Empty);
        }

        public static DateTime UnixTimeStampToDateTime(int unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }

    }
}
