using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace RestServiceGeoFit.Models
{
    public class Photo
    {
        [Key]
        [Required]
        public int PhotoId { get; set; }
    }
}