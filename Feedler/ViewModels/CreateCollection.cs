using System.ComponentModel.DataAnnotations;
using Feedler.Models;
using Newtonsoft.Json;

namespace Feedler.ViewModels
{
    public sealed class CreateCollection
    {
        /// <summary>
        /// Collection name.
        /// </summary>
        [JsonProperty]
        [Required, MinLength(3)]
        public string Name { get; set; }

        public Collection GetModel() => new Collection(Name);
    }
}