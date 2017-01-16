using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DeutscheBankKreditrechner.web.Models
{
    public class ArbeitgeberModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Pflichtfeld")]
        [StringLength(30, ErrorMessage = "max. 30 Zeichen erlaubt")]
        [Display(Name = "Firmen-Name")]
        public string FirmenName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Pflichtfeld")]
        [Display(Name = "Beschäftigungsart")]
        public int ID_BeschäftigungsArt { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Pflichtfeld")]
        [Display(Name = "Branche")]
        public int ID_Branche { get; set; }

        //[DataType(DataType.DateTime)]
        //[DisplayFormat(ApplyFormatInEditMode = true, ConvertEmptyStringToNull = true, DataFormatString = "MM.yyyy")]
        //[Display(Name = "beschäftigt seit")]
        //[Required(AllowEmptyStrings = false, ErrorMessage = "Pflichtfeld")]
        //public string BeschäftigtSeit { get; set; }

        [Required(ErrorMessage = "Bitte Geburtsdatum wählen.")]
        [ValidWorkDate]
        [DataType(DataType.Date)]
        [Display(Name = "beschäftigt seit")]
        [DisplayFormat(DataFormatString = "{0:yyy-MM-dd}", ApplyFormatInEditMode = true)]
        public string BeschäftigtSeit { get; set; }

        [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
        public sealed class ValidWorkDate : ValidationAttribute
        {
            protected override ValidationResult IsValid(object value, ValidationContext validationContext)
            {
                DateTime gebDat = Convert.ToDateTime(value);
                DateTime aktDatum = DateTime.Now;


                if (gebDat <= aktDatum)
                    return ValidationResult.Success;
                else
                    return new ValidationResult("Das gewählte Datum muss kleiner als das heutige Datum sein!");
            }
        }

        public List<BeschaeftigungsArtModel> AlleBeschaeftigungen { get; set; }
        public List<BrancheModel> AlleBranchen { get; set; }

        public int ID_Kunde { get; set; }
    }
}