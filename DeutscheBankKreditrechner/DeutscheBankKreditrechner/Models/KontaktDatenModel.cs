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
        [Required]
        [StringLength(20, ErrorMessage = "max. 20 Zeichen erlaubt")]
        [Display(Name = "Strasse")]
        public string Strasse { get; set; }
        [Required]
        [StringLength(10, ErrorMessage = "max. 10 Zeichen erlaubt")]
        [Display(Name = "Hausnummer")]
        public string Hausnummer { get; set; }

        [StringLength(20, ErrorMessage = "max. 20 Zeichen erlaubt")]
        [Display(Name = "Stiege")]
        public string Stiege { get; set; }

        [StringLength(20, ErrorMessage = "max. 20 Zeichen erlaubt")]
        [Display(Name = "Etage")]
        public string Etage { get; set; }

        [StringLength(20, ErrorMessage = "max. 20 Zeichen erlaubt")]
        [Display(Name = "Tür")]
        public string Tuer { get; set; }
        [Required]
        [Display(Name = "Wohnort")]
        public int ID_PLZ { get; set; }
        [EmailAddress]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Pflichtfeld")]
        [StringLength(30, ErrorMessage = "max. 30 Zeichen erlaubt")]
        [Display(Name = "Email")]
        public string Mail { get; set; }
       
        [Required(AllowEmptyStrings = false, ErrorMessage = "Pflichtfeld")]
        [StringLength(20, ErrorMessage = "max. 20 Zeichen erlaubt")]
        [RegularExpression("^(?!0+$)(\\+\\d{1,3}[- ]?)?(?!0+$)\\d{10,15}$", ErrorMessage = "Please enter valid phone no.")]
        [Display(Name = "Telefonnummer")]
        public string TelefonNummer { get; set; }
        [HiddenInput(DisplayValue = false)]
        [Required]
        public int ID_Kunde { get; set; }

        public List<PLZModel> AllePostleitZahlen { get; set; }

    }
}