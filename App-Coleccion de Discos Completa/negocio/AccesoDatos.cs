using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient; // Agrego la libreria de SQL.

namespace negocio
{
    class AccesoDatos // Me creo una clase de datos para centralizar las conexiones.
    {
        private SqlConnection conexion; // Declaro los atributos que necesito.
        private SqlCommand comando;
        private SqlDataReader lector;
        public SqlDataReader Lector // Creo una propiedad para leer los datos del exterior.
        {
            get { return lector; }  // Retorna la lista de "while (datos.Lector.Read())", al SqlDataReader.
        }

        public AccesoDatos() // Me creo mi constructor.
        {                             // Aca le paso la cadena de conexion al contructor sobrecargado SqlConnection.
            conexion = new SqlConnection("server=.\\SQLEXPRESS; database=DISCOS_DB; integrated security=true");
            comando = new SqlCommand(); // Comando para hacer la consulta.
        }

        public void setearConsulta(string consulta) // Con este metodo hago la consulta.
        {
            comando.CommandType = System.Data.CommandType.Text; // Le digo que la consulta es de tipo texto.
            comando.CommandText = consulta;                     // Asigno al comando.
        }

        public void ejecutarLectura() // Este metodo realiza la lectura y lo guarda en el "lector".
        {
            comando.Connection = conexion;
            try
            {
                conexion.Open();
                lector = comando.ExecuteReader();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void ejecutarAccion()
        {
            comando.Connection = conexion;
            try
            {
                conexion.Open();
                comando.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void setearParametro(string nombre, object valor) // Creo el metodo para usarlo en DiscoNegocio,
        {                                                        // para agregar a la DB IdEstilo y IdTiposEdicion.
            comando.Parameters.AddWithValue(nombre, valor);      // Agrego los datos al comando .Parameters.
        }

        public void cerrarConexion() // Metodo para cerrar la coneccion.
        {
            if (lector != null) // Si realice una lectura:
            {
                lector.Close(); // Cierro el lector tambien.
            }
            conexion.Close();   // Cierro la conexion.
        }
    }
}
