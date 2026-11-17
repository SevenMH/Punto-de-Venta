using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P1
{ 
        public static class Sesion
        {
            public static int IdUsuario { get; set; }
            public static int IdEmpleado { get; set; }
            public static int IdRol { get; set; }
            public static string NombreUsuario { get; set; } = string.Empty;
            public static string NombreRol { get; set; } = string.Empty;

            public static bool CerrandoSesion { get; set; } = false; // Bandera de revision(Cerrar sesion)
    }
}
