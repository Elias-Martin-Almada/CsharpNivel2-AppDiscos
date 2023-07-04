using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;  // Agrego la libreria para poder usar los objetos de conexion. 
using dominio;                // Agrego la libreria para usar lo que esta en "dominio".

namespace negocio 
{
    // En esta clase voy a crear los metodos de Acceso a Datos: ¡ATENCION! Esto se realizo antes de tener la clase "AccesoDatos".
    public class DiscoNegocio 
    {
        // Creo un metodo/lista para leer registros de la base de datos, que returna una lista.
        public List<Disco> listar()
        {
            List<Disco> lista = new List<Disco>();
            SqlConnection conexion = new SqlConnection();// Objeto para conectarme.
            SqlCommand comando = new SqlCommand();       // Objeto para realizar acciones.
            SqlDataReader lector;                        // Objeto para guardar los datos obtenidos.


            // Lo primero es Manejo de Excepciones: Por si se rompe, muestre el error.
            try
            {
                // Configuro los objetos:   server al que conecto; DB al que conecto ; como me voy aconectar    ;
                conexion.ConnectionString = "server=.\\SQLEXPRESS; database=DISCOS_DB; integrated security=true";
                comando.CommandType = System.Data.CommandType.Text; // Al comando le digo de que "tipo" va a ser.                                                                                                                                                                       // D.Activo me trae solo los Discos Act.
                comando.CommandText = "select Titulo, FechaLanzamiento, CantidadCanciones, UrlImagenTapa, E.Descripcion as Estilo, T.Descripcion as Edicion, D.IdEstilo, D.IdTipoEdicion, D.Id from DISCOS D, ESTILOS E, TIPOSEDICION T where E.Id = IdEstilo AND IdTipoEdicion = T.Id And D.Activo = 1"; // Ahora le digo "Cual" va a ser el texto, la Consulta.
                comando.Connection = conexion; // Le digo al comando que ejecute la consulta en la conexion que estableci arriba.
                
                conexion.Open(); // Abro la conexion.
                lector = comando.ExecuteReader(); // Realizo la lectura y la guardo en lector.
                // Me creo un while para leer los datos de la tabla:
                while (lector.Read())
                {
                    // Creo un un Disco para cargarlo con los datos de ese registro:
                    Disco aux = new Disco();
                    aux.Id = (int)lector["Id"];
                    aux.Titulo = (string)lector["Titulo"];
                    aux.FechaLanzamiento = (DateTime)lector["FechaLanzamiento"];
                    aux.CantidadCanciones = (int)lector["CantidadCanciones"];
                    
                    // VALIDACION RECOMENDADA:
                    // Se utiliza para tomar decisiones basadas en si la columna "UrlImagen" tiene un valor nulo o no en la base de datos,
                    // si esta en "null" muestro la imagen predeterminada y no se rompe la App.
                    if (!(lector["UrlImagenTapa"] is DBNull)) // Si no(!) es DBNull entonces lee la url.
                    {
                        aux.UrlImagen = (string)lector["UrlImagenTapa"]; // Cargo la imagen.
                    }

                    aux.Genero = new Estilo(); // Creo un objeto nuevo de tipo Estilo.
                    aux.Genero.Id = (int)lector["IdEstilo"];           // Agrego el IdEstilo para precargar los Desplegables.
                    aux.Genero.Descripcion = (string)lector["Estilo"];
                    aux.Edicion = new TiposEdicion();
                    aux.Edicion.Id = (int)lector["IdTipoEdicion"];    // Agrego el IdTipoEdicion para precargar los Desplegables.
                    aux.Edicion.Descripcion = (string)lector["Edicion"];
                    // Finalmente Agrego ese Disco a la lista:
                    lista.Add(aux);                  
                }                
                conexion.Close(); // Cierro la conexion.

                return lista; // Retorna listar();
            }
            catch (Exception)
            {
                throw;
            }
        }
        // Si no tengo una Clase "Acceso datos", todo lo que esta en el metodo "listar()" voy a tener
        // que repetir en todos los metodos que realicen acciones sobre la DB, Ej: "agregar", "modificar".
        public void agregar(Disco nuevo) // Con esto agrego un pokemon nuevo.
        {
            AccesoDatos datos = new AccesoDatos(); // Con este objeto me puedo conectar a la DB.
            try
            {
                //datos.setearConsulta("insert DISCOS (Titulo, FechaLanzamiento, CantidadCanciones, UrlImagenTapa) values ('"+ nuevo.Titulo +"', '', 0, '') insert ESTILOS (Descripcion) values ('"+ nuevo.Genero +"') insert TIPOSEDICION (Descripcion) values ('"+ nuevo.Edicion +"') ");
                                                    // Agrego los nombres de las columnas que necesito.                                                                                                                                // Creo un nombre de "variable" para cargar los datos, son parametros.
                datos.setearConsulta("insert DISCOS (Titulo, FechaLanzamiento, CantidadCanciones, IdEstilo, IdTipoEdicion, UrlImagenTapa) values ('"+ nuevo.Titulo +"', '"+ nuevo.FechaLanzamiento.ToString("yyyy-MM-dd") + "', '"+ nuevo.CantidadCanciones +"', @idEstilo, @idTipoEdicion, @urlImagenTapa)");
                                   //( nombre  ,     valor       );
                datos.setearParametro("idEstilo", nuevo.Genero.Id);       // Agrego los valores al comando "setearParametro",                      // Opcion 1: Con values.
                datos.setearParametro("idTipoEdicion", nuevo.Edicion.Id); // Agrego tambien la "idTipoEdicion", todo esto usando el "nuevo" Disco. // Opcion 2: Con Parametros.
                datos.setearParametro("urlImagenTapa", nuevo.UrlImagen);  // Agrego la url de la imagen.
                datos.ejecutarAccion();                                   // Me voy a "AccesoDatos", para crear el metodo setearParametro.
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                datos.cerrarConexion();
            }
        }

        public void modificar(Disco disco) // La consulta del modificar es un Update.
        {
            AccesoDatos datos = new AccesoDatos();
            
            try
            {
                datos.setearConsulta("update DISCOS set Titulo = @titulo, FechaLanzamiento = @fecha, CantidadCanciones = @canciones, UrlImagenTapa = @urlImagen, IdEstilo = @idEstilo, IdTipoEdicion = @idEdicion where Id = @id");
                datos.setearParametro("@titulo", disco.Titulo);              // Actualizo todos los parametros.
                datos.setearParametro("@fecha", disco.FechaLanzamiento);
                datos.setearParametro("@canciones", disco.CantidadCanciones);
                datos.setearParametro("@urlImagen", disco.UrlImagen);
                datos.setearParametro("@idEstilo", disco.Genero.Id);
                datos.setearParametro("@idEdicion", disco.Edicion.Id);
                datos.setearParametro("@id", disco.Id);

                datos.ejecutarAccion();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                datos.cerrarConexion();
            }

        }

        public void eliminar(int id) // Metodo para eliminar que recibe el "id" de frmDiscos negocio.eliminar(seleccionado.Id)
        {
            try
            {
                AccesoDatos datos = new AccesoDatos();
                datos.setearConsulta("delete from DISCOS where Id = @id"); // Uso una Consulta: delete.
                datos.setearParametro("@id", id); // Paso los parametros.
                datos.ejecutarAccion();           // Ejecuto la accion en la DB.
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void eliminarLogico(int id) // Metodo para eliminar de la App, pero no de DB.
        {
            try
            {
                AccesoDatos datos = new AccesoDatos();
                datos.setearConsulta("update DISCOS set Activo = 0 Where id = @id"); // La consulta es un update para desactivar el Disco.
                datos.setearParametro("@id", id); // El metodo Recibe el Id para darlo de baja.
                datos.ejecutarAccion();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public object filtrar(string campo, string criterio, string filtro) // Genero el metodo Filtro Avanzado: 
        {
            List<Disco> lista = new List<Disco>();
            AccesoDatos datos = new AccesoDatos();
            try
            {// Copio la misma Consulta de listar() y la dejo "Abierta" con AND y (ESPACIO) para concatenar mas filtros. 
                string consulta = "select Titulo, FechaLanzamiento, CantidadCanciones, UrlImagenTapa, E.Descripcion as Estilo, T.Descripcion as Edicion, D.IdEstilo, D.IdTipoEdicion, D.Id from DISCOS D, ESTILOS E, TIPOSEDICION T where E.Id = IdEstilo AND IdTipoEdicion = T.Id And D.Activo = 1 AND ";

                if (campo == "Cantidad de Canciones") // Pregunto Si seleccionó un CantCanciones.
                {
                    switch (criterio) // y si lo es pregunto por Mayor, Menor o Igual
                    {
                        case "Mayor a":            // Filtro es lo que ingresa el usuario en la caja de Texto.
                            consulta += " CantidadCanciones > " + filtro;
                            break;
                        case "Menor a":
                            consulta += " CantidadCanciones < " + filtro;
                            break;
                        default:
                            consulta += " CantidadCanciones = " + filtro;
                            break;
                    }
                }
                else if (campo == "Titulo") // Sino si, es un Titulo:
                {
                    switch (criterio)
                    {// la palabra "like" me permite hacer busquedas por Strings:
                     // like 'Comienza%'
                     // like '%Termina'
                     // like '%Contiene%' 
                     // like '%': Esto me trae todos los Discos porque esta vacio.
                        case "Comienza con":
                            consulta += "Titulo like '" + filtro + "%' ";
                            break;
                        case "Termina con":
                            consulta += "Titulo like '%" + filtro + "'";
                            break;
                        default:
                            consulta += "Titulo like '%" + filtro + "%'";
                            break;
                    }
                }
                else // Sino es un Estilo:
                {
                    switch (criterio)
                    {
                        case "Comienza con":                  // Concateno lo que ingresa el usuario.
                            consulta += "E.Descripcion like '" + filtro + "%' ";
                            break;
                        case "Termina con":
                            consulta += "E.Descripcion like '%" + filtro + "'";
                            break;
                        default:
                            consulta += "E.Descripcion like '%" + filtro + "%'";
                            break;
                    }
                }

                datos.setearConsulta(consulta); // Ahora si paso la Consulta Filtrada con lo que se necesite.
                datos.ejecutarLectura();        // Ejecuto la Consulta.

                while (datos.Lector.Read())     // Uso el Read para Mostrar los Discos filtrados en la lista.
                {                               // Agrego el "datos.Lector..."
                                                // Arriba uso solo lector.Read() Porque tengo  SqlDataReader lector; antes no tenia la clase acceso a datos.
                    Disco aux = new Disco();
                    aux.Id = (int)datos.Lector["Id"];
                    aux.Titulo = (string)datos.Lector["Titulo"];
                    aux.FechaLanzamiento = (DateTime)datos.Lector["FechaLanzamiento"];
                    aux.CantidadCanciones = (int)datos.Lector["CantidadCanciones"];

                    if (!(datos.Lector["UrlImagenTapa"] is DBNull)) 
                    {
                        aux.UrlImagen = (string)datos.Lector["UrlImagenTapa"]; 
                    }

                    aux.Genero = new Estilo(); 
                    aux.Genero.Id = (int)datos.Lector["IdEstilo"];          
                    aux.Genero.Descripcion = (string)datos.Lector["Estilo"];
                    aux.Edicion = new TiposEdicion();
                    aux.Edicion.Id = (int)datos.Lector["IdTipoEdicion"];    
                    aux.Edicion.Descripcion = (string)datos.Lector["Edicion"];
                    
                    lista.Add(aux);  // Tengo la lista de los Discos Filtrados.
                }

                return lista;        // Llevo la lista de filtrados.
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
// Consulta 1) SQL: Datos Relacionados entre las tablas, me trae el "Estilo".
// select Titulo, FechaLanzamiento, CantidadCanciones, UrlImagenTapa, E.Descripcion as Estilo  
// from DISCOS D, ESTILOS E
// where E.Id = IdEstilo.

// Consulta 2) SQL: Me trae "Estilo" y tambien "Tipo de Edicion".
// select Titulo, FechaLanzamiento, CantidadCanciones, UrlImagenTapa, E.Descripcion as Estilo, T.Descripcion as Edicion  
// from DISCOS D, ESTILOS E, TIPOSEDICION T 
// where E.Id = IdEstilo AND IdTipoEdicion = T.Id.
