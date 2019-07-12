using MarvelAPI.Rest;
using System;

namespace MarvelAPI
{
    class Program
    {
        static void Main(string[] args)
        {
            MarvelRest marvel = new MarvelRest();
            marvel.GetCharacters();
        }
    }
}
