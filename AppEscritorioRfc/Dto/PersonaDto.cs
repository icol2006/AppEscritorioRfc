using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAppRfc.Dto
{
    public class PersonaDto
    {
        public String Nombre { get; set; }
        public String Apellido1 { get; set; }
        public String Apellido2 { get; set; }
        public string Fecha { get; set; }
        public string Curp { get; set; }
        public string Rtc { get; set; }
    }
}