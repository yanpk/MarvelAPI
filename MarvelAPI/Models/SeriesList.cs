using System;
using System.Collections.Generic;
using System.Text;

namespace MarvelAPI.Models
{
    public class SeriesList
    {
        public int available { get; set; }
        public int returned { get; set; }
        public string collectionURI { get; set; }
        public List<SeriesSummary> items { get; set; }
    }
}
