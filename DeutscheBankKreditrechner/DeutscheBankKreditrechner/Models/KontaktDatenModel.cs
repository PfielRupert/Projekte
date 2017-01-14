using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DeutscheBankKreditrechner.web.Models
{
    public class KontaktDatenModel 
    {
        public string Strasse { get; set; }
        public string Hausnummer { get; set; }
        public string Stiege { get; set; }
        public string Etage { get; set; }
        public string Tuer { get; set; }
        [Display(Name = "Wohnort")]
        public int ID_PLZ { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Pflichtfeld")]
        public string Mail { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Pflichtfeld")]
        public string TelefonNummer { get; set; }
        [HiddenInput(DisplayValue = false)]
        [Required]
        public int ID_Kunde { get; set; }

        public List<PLZModel> AllePostleitZahlen { get; set; }

    }
}