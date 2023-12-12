using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MarvelWeb.Models;
using System.Text;
using Newtonsoft.Json;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using MarvelAPI.Models;
using System.Security.Cryptography;
using AutoMapper;
using RestSharp;
using MarvelAPI.Rest;

namespace MarvelWeb.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            var MarvelRest = new MarvelRest();
            var result = MarvelRest.GetCharacters();
            
            return View(result);
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

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }

        public IActionResult Busca()
        {
            MarvelRest rest = new MarvelRest();
            var request = new RestRequest();
            request.RequestFormat = DataFormat.Json;
            string publickey = "***";
            string privatekey = "****";
            string ts = DateTime.Now.Ticks.ToString();
            string hash = GerarHash(ts, publickey, privatekey);
            request.Resource = $"/v1/public/characters?ts={ts}&apikey={publickey}&hash={hash}";
            request.RootElement = "GetCharacters";

            var listCharacters = rest.Execute<Rootobject>(request);
            var characters = listCharacters.data.results;
            return View();
        }

        [HttpPost]
        public IActionResult Busca(string nome)
        {
            Personagem personagem;

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));

                string publickey = "***";
                string privatekey = "****";
                string ts = DateTime.Now.Ticks.ToString();
                string hash = GerarHash(ts, publickey, privatekey);

                HttpResponseMessage response = client.GetAsync(
                        $"https://gateway.marvel.com/v1/public/" +
                        $"characters?ts={ts}&apikey={publickey}&hash={hash}&" +
                        $"name={Uri.EscapeUriString($"{nome}")}").Result;

                response.EnsureSuccessStatusCode();
                string conteudo =
                    response.Content.ReadAsStringAsync().Result;

                dynamic resultado = JsonConvert.DeserializeObject(conteudo);

                personagem = new Personagem();
                personagem.Nome = resultado.data.results[0].name;
                personagem.Descricao = resultado.data.results[0].description;
                personagem.UrlImagem = resultado.data.results[0].thumbnail.path + "." +
                    resultado.data.results[0].thumbnail.extension;
                personagem.UrlWiki = resultado.data.results[0].urls[1].url;
                personagem.UrlWiki = resultado.data.results[0].urls[1].url;
            }

            return View(personagem);
        }
    }
}
