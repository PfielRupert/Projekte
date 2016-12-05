﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeutscheBankKreditrechner.web.Models
{
    public class KontaktDatenModel 
    {
        public string Strasse { get; set; }
        public string Hausnummer { get; set; }
        public string Stiege { get; set; }
        public string Etage { get; set; }
        public string Tuer { get; set; }
        public int ID_PLZ { get; set; }
        public string Mail { get; set; }
        public string TelefonNummer { get; set; }
    }
}