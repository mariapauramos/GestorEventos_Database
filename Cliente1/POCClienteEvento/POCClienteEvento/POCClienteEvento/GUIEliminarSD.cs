using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace POCClienteEvento
{
    public partial class GUIEliminarSD : Form
    {
        public GUIEliminarSD()
        {
            InitializeComponent();
        }

        private void buttonCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtFCosto_TextChanged(object sender, EventArgs e)
        {

        }

        private async void buttonBuscar_Click(object sender, EventArgs e)
        {
            try
            {
                string idTexto = txtIdSede.Text.Trim();
                if (string.IsNullOrEmpty(idTexto))
                {
                    MessageBox.Show("Por favor ingresa un ID de sede.");
                    return;
                }

                if (!int.TryParse(idTexto, out int idSede))
                {
                    MessageBox.Show("El ID de sede debe ser un número entero.");
                    return;
                }

                using (HttpClient client = new HttpClient())
                {
                    // Basic Auth
                    var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes("admin:admin"));
                    client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Basic", credentials);

                    string url = $"http://localhost:8092/sedes/{idSede}";

                    HttpResponseMessage response = await client.GetAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        string json = await response.Content.ReadAsStringAsync();

                        var sede = JsonSerializer.Deserialize<SedeBuscarDto>(
                            json,
                            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                        // Rellenar campos (asegúrate de que los nombres de TextBox existan)
                        txtNombre.Text = sede.nombre;
                        txtCapacidad.Text = sede.capacidad.ToString();
                        txtDireccion.Text = sede.direccion;
                        txtCosto.Text = sede.costoMantenimiento.ToString("0.00");

                        txtCubierta.Text = sede.cubierta ? "Sí" : "No";

                        txtFecha.Text = sede.fechaCreacion.ToString("yyyy-MM-dd HH:mm:ss");

                        txtEvento.Text = string.IsNullOrEmpty(sede.idEventoAsociado)
                                ? "Sin evento asociado"
                                : sede.idEventoAsociado;


                        // Bloquear edición
                        txtNombre.ReadOnly = true;
                        txtCapacidad.ReadOnly = true;
                        txtDireccion.ReadOnly = true;
                        txtCosto.ReadOnly = true;
                        txtCubierta.ReadOnly = true;
                        txtFecha.ReadOnly = true;
                        txtEvento.ReadOnly = true;
                    }
                    else
                    {
                        MessageBox.Show($"No se encontró la sede con el ID {idSede}");

                        // Limpiar si no existe
                        txtNombre.Clear();
                        txtCapacidad.Clear();
                        txtDireccion.Clear();
                        txtCosto.Clear();
                        txtCubierta.Clear();
                        txtFecha.Clear();
                        txtEvento.Clear();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al buscar sede: " + ex.Message);
            }

        }
        public class SedeBuscarDto
        {
            public int idSede { get; set; }
            public string nombre { get; set; }
            public int capacidad { get; set; }
            public string direccion { get; set; }
            public double costoMantenimiento { get; set; }
            public bool cubierta { get; set; }
            public DateTime fechaCreacion { get; set; }

            // Este campo viene si tu backend lo expone con @JsonProperty("nombreEvento")
            public string idEventoAsociado { get; set; }
        }

        private async void buttonEliminar_Click(object sender, EventArgs e)
        {
            // Validar que haya un ID de sede
            if (string.IsNullOrEmpty(txtIdSede.Text.Trim()))
            {
                MessageBox.Show("Debes buscar una sede primero.", "Advertencia",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(txtIdSede.Text.Trim(), out int idSede))
            {
                MessageBox.Show("El ID de sede no es válido.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Confirmar eliminación
            DialogResult confirmacion = MessageBox.Show(
                $"¿Estás seguro de que deseas eliminar la sede '{txtNombre.Text}' con ID {idSede}?\n\n" +
                "Esta acción no se puede deshacer.",
                "Confirmar Eliminación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirmacion != DialogResult.Yes)
            {
                return; // Usuario canceló
            }

            try
            {
                using (var client = new HttpClient())
                {
                    // Basic Auth
                    var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes("admin:admin"));
                    client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Basic", credentials);

                    string url = $"http://localhost:8092/sedes/{idSede}";

                    HttpResponseMessage response = await client.DeleteAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Sede eliminada correctamente.", "Éxito",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // Limpiar todos los campos después de eliminar
                        LimpiarFormulario();
                    }
                    else
                    {
                        string errorMsg = await response.Content.ReadAsStringAsync();
                        MessageBox.Show($"Error al eliminar sede:\n{errorMsg}", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al eliminar sede: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void LimpiarFormulario()
        {
            txtIdSede.Clear();
            txtNombre.Clear();
            txtCapacidad.Clear();
            txtDireccion.Clear();
            txtCosto.Clear();
            txtCubierta.Clear();
            txtFecha.Clear();
            txtEvento.Clear();

            // Desbloquear el campo de ID para una nueva búsqueda
            txtIdSede.ReadOnly = false;
            txtIdSede.Focus(); // Poner el cursor en el campo ID
        }
    }
}
