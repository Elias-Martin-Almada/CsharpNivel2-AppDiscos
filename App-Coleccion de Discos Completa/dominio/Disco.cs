using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dominio
{
    public class Disco // Creo mi clase base del objeto que voy a manipular.
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        [DisplayName("Fecha de Lanzamiento")]          // Agrego este Annotations para mostrar separadas las Frases en el DataGridView.
        public DateTime FechaLanzamiento { get; set; } // Agrego este Annotations para mostrar separadas las Frases en el DataGridView.
        [DisplayName("Cantidad de Canciones")]
        public int CantidadCanciones { get; set; }
        public string UrlImagen { get; set; }
        public Estilo Genero { get; set; }
        public TiposEdicion Edicion { get; set; }

    }
}
    
    


