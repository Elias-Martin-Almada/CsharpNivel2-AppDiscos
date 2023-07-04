using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using dominio; // Agrego la libreria de "dominio" para poder configurar la ventana Agregar.
using negocio; // Agrego la libreria de "negocio" para usar los metodos de la DB.
using System.IO;     // Agrego este using para usar la clase File.Copy.
using System.Configuration; // Agrego el using de la Referencia, para guardar imagen Local.

namespace App_Discos
{
    public partial class frmAltaDisco : Form
    {
        private Disco disco = null; // Me creo un atributo privado en null, si toco agregar va a estar en null.
        private OpenFileDialog archivo = null; // Creo la ventana de dialogo en null.

        public frmAltaDisco()
        {
            InitializeComponent();
        }
        public frmAltaDisco(Disco disco) // Si toco modificar, al atributo "disco" le asigno el objeto Disco.
        {                                // Este constructor recibe por parametro un Disco.
            InitializeComponent();       // Cuando toco modificar voy a tener un Disco cargado en this.pokemon.
            this.disco = disco;
            Text = "Modificar Disco";
        }
        private void frmAltaDisco_Load(object sender, EventArgs e) // En el evento Load cargo los ComboBox.
        {
            EstiloNegocio estiloNegocio = new EstiloNegocio();              // Para cargarlo necesito un objeto de "EstiloNegocio".
            TiposEdicionNegocio edicionNegocio = new TiposEdicionNegocio(); // Para cargarlo necesito un objeto de "TiposEdicionNegocio".
            try
            {   
                cboGenero.DataSource = estiloNegocio.listar(); // DataSource acepta una fuente de datos para vincularla con el control.
                cboGenero.ValueMember = "Id";                  // Esto lo uso para precargar en los Desplegables el Estilo y 
                cboGenero.DisplayMember = "Descripcion";       // Edicion del Disco a modificar en la ventana Mofificar.
                cboTipoEdicion.DataSource = edicionNegocio.listar(); // Lo mismo para la Edicion, estos son los Desplegables.
                cboTipoEdicion.ValueMember = "Id";
                cboTipoEdicion.DisplayMember = "Descripcion";
                cboGenero.SelectedItem = null;                 // Iniciar Desplegables sin nada:
                cboTipoEdicion.SelectedItem = null;

                if (disco != null) // Si el "disco" es distinto de null, es un Modificar. Entonces Precargo los elementos.
                {
                    txtTitulo.Text = disco.Titulo;
                    txtFecha.Text = disco.FechaLanzamiento.ToString();
                    txtCantidad.Text = disco.CantidadCanciones.ToString();
                    txtUrlImagen.Text = disco.UrlImagen;
                    cargarImagen(disco.UrlImagen);
                    cboGenero.SelectedValue = disco.Genero.Id;       // Paso los datos a los Desplegables, para preseleccionar un valor.
                    cboTipoEdicion.SelectedValue = disco.Edicion.Id; // Paso los datos a los Desplegables.
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        
        private void cargarImagen(string imagen) // Uso el metodo que uso en frmDisco.
        {
            try
            {
                pbxDisco.Load(imagen); // Cargo al PictureBox.
            }
            catch (Exception ex)
            {
                pbxDisco.Load("https://efectocolibri.com/wp-content/uploads/2021/01/placeholder.png");
            }
        }
        
        private void txtUrlImagen_Leave(object sender, EventArgs e)
        {
            cargarImagen(txtUrlImagen.Text);
        }

        private void btnAgregarImagen_Click(object sender, EventArgs e) // Evento del Boton "+"
        {
            archivo = new OpenFileDialog();             // Objeto para poder levantar la imagen, abre una ventana de dialogo.
            archivo.Filter = "jpg|*.jpg;|png|*.png";    // A la ventana le digo que tipo de archivos me permite cargar.
            if (archivo.ShowDialog() == DialogResult.OK) // Pregunto si aprete Aceptar en la ventana.
            {
                txtUrlImagen.Text = archivo.FileName;   // Paso la ruta de la imagen en la caja de texto.
                cargarImagen(archivo.FileName);         // Esto me permite ver la imagen que quiero agregar.

                // Guardo la imagen:
                // Tengo que agregar el using System.IO, para usar la clase File.
                // Agrego referencia: Ensamblados/Configuracion/System.Configuration.
                // Agrego el using System.Configuration, para poder usar el: ConfigurationManager.

                //File.Copy(archivo.FileName, ConfigurationManager.AppSettings["images-folder"] + archivo.SafeFileName);
            }
        }
        
        private bool validarCarga() // Validacion para La ventana de Carga.
        {
            if (string.IsNullOrEmpty(txtTitulo.Text) || string.IsNullOrEmpty(txtFecha.Text)
                || cboGenero.SelectedIndex == -1 || cboTipoEdicion.SelectedIndex == -1) // Desplegables.
            {
                MessageBox.Show("Complete los campos vacíos * ", "", MessageBoxButtons.OK);
                return true;
            }
            if (!soloNumeros(txtCantidad.Text) || string.IsNullOrEmpty(txtCantidad.Text))
            {
                MessageBox.Show("Complete el campo 'Cantidad de Canciones' solo con números");
                return true;
            }
            else if (txtUrlImagen.Text == "")
            {
                MessageBox.Show("Puede Agregar una imagen");
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool soloNumeros(string cadena) // Funcion que valida solo numeros.
        {
            foreach (char caracter in cadena)
            {
                if (!(char.IsNumber(caracter))) // Pregunto si No es Numero.
                {
                    return false;
                }
            }
            return true;
        }
        
        private void btnAceptar_Click(object sender, EventArgs e) // Evento Boton Aceptar.
        {
            //Disco DiscoNuevo = new Disco();
            DiscoNegocio negocio = new DiscoNegocio();
            try
            {
                if (validarCarga()) // Verificar si el filtro es válido antes de continuar.
                {
                    return;          // Si tiene True sale Acá, sino sigue.
                }

                if (disco == null)        // Si aprete ACEPTAR en la ventana nueva, pregunto si disco esta en null?
                {
                    disco = new Disco(); // Si lo esta me creo un nuevo objeto Disco para cargarlo,
                }                         // y sino precargo los datos para modificar.
                
                disco.Titulo = txtTitulo.Text;          // Agrego los datos del usuario.                
                disco.FechaLanzamiento = DateTime.Parse(txtFecha.Text); // Agrego.
                disco.CantidadCanciones = int.Parse(txtCantidad.Text);  // Agrego.
                disco.UrlImagen = txtUrlImagen.Text; // Al momento de Aceptar cargo la imagen de la caja de texto al Objeto Disco.

                disco.Genero = (Estilo)cboGenero.SelectedItem;             // Agrego el Genero seleccionado convirtiéndolo en un objeto de tipo "Estilo".
                disco.Edicion = (TiposEdicion)cboTipoEdicion.SelectedItem; // Agrego el TipoEdicion conviertiendolo en un objeto de tipo "TiposEdicion".  

                if(disco.Id != 0) // Pregunto Si el Id es distinto de "0" quiere decir que estoy modificando.
                {
                    negocio.modificar(disco); // Uso el metodo "modificar" para mandarlo a la DB.
                    MessageBox.Show("Modificado exitosamente");
                }
                else             // Si el Id esta en "0" quiere decir que el Pokemon no esta en la DB.
                {
                    negocio.agregar(disco);  // Uso el metodo "agregar" para mandarlo a la DB.
                    MessageBox.Show("Agregado exitosamente");
                }

                //Guardo imagen si la levantó localmente:
                if (archivo != null && !(txtUrlImagen.Text.ToUpper().Contains("HTTP"))) // Pregunto si tengo que guardar la imagen localmente.
                    // Copia el archivo seleccionado por el usuario a la carpeta de imágenes especificada en la configuración de la aplicación disco-app.
                    File.Copy(archivo.FileName, ConfigurationManager.AppSettings["images-folder"] + archivo.SafeFileName);

                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        } 

        private void btnCancelar_Click(object sender, EventArgs e) // Boton cancelar.
        {
            this.Close();
        }



    }
}
