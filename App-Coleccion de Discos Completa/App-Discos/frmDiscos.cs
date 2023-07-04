using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using dominio; // Agrego la libreria para usar lo que esta en "dominio".
using negocio; // Agrego la libreria para usar lo que esta en "negocio".

namespace App_Discos
{
    public partial class frmDiscos : Form
    {
        private List<Disco> listaDiscos;
        public frmDiscos()
        {
            InitializeComponent();
        }

        private void frmDiscos_Load(object sender, EventArgs e) // Metodo carga de Formulario.
        {
            cargar(); // Uso el metodo cargar() al Iniciar la App.
            cboCampo.Items.Add("Titulo"); // Cargo el desplegable Campo.
            cboCampo.Items.Add("Genero");
            cboCampo.Items.Add("Cantidad de Canciones");
        }

        // Metodo: Cambiar al seleccionar.
        private void dgvDiscos_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvDiscos.CurrentRow != null) // If para Error SelectedIndexChanged.   
            {                       //(Casteo) explicito para cambiar el objeto enlazado a Disco.
                Disco seleccionado = (Disco)dgvDiscos.CurrentRow.DataBoundItem; // Lo guardo en varible.
                cargarImagen(seleccionado.UrlImagen);                           // Al seleccionar otro lo cargo.
            }
        }

        // Me creo un metodo cargar() para poder volver a usarlo despues de el ingreso de un registro.
        private void cargar()
        {
            DiscoNegocio negocio = new DiscoNegocio();      // Creo un objeto negocio.
            // Manejo la excepción para la ventana Principal:
            try
            {
                listaDiscos = negocio.listar();                 // .listar() va a la DB y trae una lista de datos, lo guardo en la ListaDiscos,
                dgvDiscos.DataSource = listaDiscos;             // y asigno al DataSource, que lo modela en el DataGridView.
                ocultarColumnas();                              // Con esta linea oculto las columnas.              
                cargarImagen(listaDiscos[0].UrlImagen);         // Carga la imagen de la lista a partir del indice[0].          
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void ocultarColumnas() // Metodo para ocultar Columnas.
        {
            dgvDiscos.Columns["UrlImagen"].Visible = false;
            dgvDiscos.Columns["Id"].Visible = false;
        }

        // Me creo una Funcion "cargarImagen" para manejar Excepciones por si la Url da error,
        // de esta forma no se rompe la App.
        private void cargarImagen(string imagen)
        {
            try
            {
                pbxDisco.Load(imagen); // Cargo la imagen,
            }
            catch (Exception ex)       // Si no lo carga muestra imagen por defecto.
            {
                pbxDisco.Load("https://efectocolibri.com/wp-content/uploads/2021/01/placeholder.png");
            }
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            frmAltaDisco alta = new frmAltaDisco();
            alta.ShowDialog();
            cargar(); // Nuevamente uso el metodo cargar para actualizar el ultimo registro ingresado.
        }

        private void btnModificar_Click(object sender, EventArgs e) // Evento del boton Modificar.
        {
            Disco seleccionado;
            seleccionado = (Disco)dgvDiscos.CurrentRow.DataBoundItem; // Le paso los datos del Disco a la varible.

            frmAltaDisco modificar = new frmAltaDisco(seleccionado); // El constructor recibe un Disco.
            modificar.ShowDialog();
            cargar();
        }

        private void btnEliminarFisico_Click(object sender, EventArgs e) // Metodo Eliminar.
        {
            eliminar();      // Va por el else falso.
        }

        private void btnEliminarLogico_Click(object sender, EventArgs e)
        {
            eliminar(true);  // Va por el If verdadero.
        }

        private void eliminar(bool logico = false) // Me creo el metodo eliminar para usarlo en los dos tipos de "eliminar".
        {
            DiscoNegocio negocio = new DiscoNegocio();
            Disco seleccionado; // Creo una variable para elegir el Disco a eliminar.
            try
            {// DialogResult Guarda Si/NO   Ventana.          Texto del mensaje.           Titulo.           Botones Si / No.      Cartel Advertencia.
                DialogResult respuesta = MessageBox.Show("¿De verdad querés eliminarlo?", "Eliminando", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (respuesta == DialogResult.Yes) // Si la respuesta es Si, se elimina el registro.
                {
                    seleccionado = (Disco)dgvDiscos.CurrentRow.DataBoundItem; // Paso el Disco seleccionado a la variable.

                    if (logico) // Pregunto en que esta la variable logico, si esta en true, elimino de la App.
                    {
                        negocio.eliminarLogico(seleccionado.Id);
                    }
                    else        // Sino elimino de la DB.
                    {
                        negocio.eliminar(seleccionado.Id); // Elimino el Disco usando el Id.
                    }
                    cargar();   // Actualizo el la lista.
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void cboCampo_SelectedIndexChanged(object sender, EventArgs e)
        {
            string opcion = cboCampo.SelectedItem.ToString(); // Paso la opcion seleccionada a la variable.
            if (opcion == "Cantidad de Canciones")            // Pregunto cual fue la eleccion.
            {
                cboCriterio.Items.Clear();                    // Limpio el Desplegable.
                cboCriterio.Items.Add("Mayor a");             // Cargo las Opciones.
                cboCriterio.Items.Add("Menor a");
                cboCriterio.Items.Add("Igual a");
            }
            else
            {
                cboCriterio.Items.Clear();                    // Sino es CantCanciones es Titulo o Descripcion. 
                cboCriterio.Items.Add("Comienza con");
                cboCriterio.Items.Add("Termina con");
                cboCriterio.Items.Add("Contiene");
            }
        }

        private void txtFiltro_TextChanged(object sender, EventArgs e) // Filtro rapido TextChanged.
        {
            List<Disco> listaFiltrada;
            string filtro = txtFiltro.Text;

            if (filtro.Length >= 3) // Filtra a partir de 3 caracteres.
            {// El metodo FindAll me muestra todos lo objetos que coincida con la Busqueda,
             // Esta lista me filtra por Titulo y Genero. 
             // El .ToUpper pasa todo a mayuscula para comparar todo por igual.
             // El .Contains Filtra por partes de la palabra.
                listaFiltrada = listaDiscos.FindAll(x => x.Titulo.ToUpper().Contains(filtro.ToUpper()) || x.Genero.Descripcion.ToUpper().Contains(filtro.ToUpper()));
            }
            else
            {
                listaFiltrada = listaDiscos; // Al Buscar en vacio, muestro lista completa.
            }

            dgvDiscos.DataSource = null;          // Tengo borrar la lista para mostrar la Filtrada.
            dgvDiscos.DataSource = listaFiltrada; // Cargo lista Filtrada.
            ocultarColumnas(); // Llamo al metodo para ocultar las columnas en el filtrado.
        }

        private bool validarFiltro()       // Metodo para validar Desplegables: 
        {
            if (cboCampo.SelectedIndex < 0) // Pregunto si el Desplegable Campo esta cargado, el Campo es como una Coleccion, si esta en -1 esta vacio.
            {                              // Esto quiere decir que no esta seleccionado.
                MessageBox.Show("Por favor, seleccione el campo para filtrar.");
                return true;
            }
            if (cboCriterio.SelectedIndex < 0) // Pregunto si el Desplegable Campo esta cargado.
            {
                MessageBox.Show("Por favor, seleccione el criterio para filtrar.");
                return true;
            }
            if (cboCampo.SelectedItem.ToString() == "Cantidad de Canciones")     // Pregunto Si en el Campo seleccionaron Numero.
            {
                if (string.IsNullOrEmpty(txtFiltroAvanzado.Text)) // Pregunto si esta vacio.
                {
                    MessageBox.Show("Debes cargar el filtro para numéricos...");
                    return true;
                }
                if (!(soloNumeros(txtFiltroAvanzado.Text))) // Pregunto si NO son solos numeros.
                {
                    MessageBox.Show("Solo nros para filtrar por un campo numérico...");
                    return true;
                }

            }

            return false;
        }

        private bool soloNumeros(string cadena) // Funcion que valida solo numeros.
        {
            foreach (char caracter in cadena)
            {
                if (!(char.IsNumber(caracter))) // Pregunto si No es Numero.
                    return false;
            }
            return true;
        }

        private void btnFiltro_Click(object sender, EventArgs e) // Boton Buscar:
        {
            DiscoNegocio negocio = new DiscoNegocio();
            try
            {
                if (validarFiltro()) // Verificar si el filtro es válido antes de continuar.
                {
                    return;          // Si tiene True sale Acá, sino sigue.
                }

                string campo = cboCampo.SelectedItem.ToString(); // Capturo los datos seleccionados por los usuarios para hacer la consulta.
                string criterio = cboCriterio.SelectedItem.ToString();
                string filtro = txtFiltroAvanzado.Text;
                dgvDiscos.DataSource = negocio.filtrar(campo, criterio, filtro); // Filtrar me trae lista de Filtrados.

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
