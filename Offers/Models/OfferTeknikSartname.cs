﻿using Microsoft.AspNetCore.Mvc.ModelBinding;
using Models;
using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class OfferTeknikSartname
    {
        public int Id { get; set; }
        public int No { get; set; }
        [Required]
        public string EquipmentName { get; set; }
        [Required]
        public string Features { get; set; } // HTML text
        [Required]
        public string Birim { get; set; }
        [Required]
        public int Miktar { get; set; }
        [Required]
        public int OfferId { get; set; }

        [BindNever]
        public Offer Offer { get; set; }
    }
}
