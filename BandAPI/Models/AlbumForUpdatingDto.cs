using BandAPI.ValidationAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BandAPI.Models
{
    public class AlbumForUpdatingDto : AlbumManipulationDto
    {
        [Required(ErrorMessage = "You need to fill description")]
        public override string Description { get => base.Description; set => base.Description = value; }
    }
}
