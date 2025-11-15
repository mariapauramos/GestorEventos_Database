using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace POCClienteEvento
{
    public partial class GUIEliminarEQ : Form
    {
        public GUIEliminarEQ()
        {
            InitializeComponent();
        }

        public class EquipoEliminarDto
        {
            public int idEquipo { get; set; }
            public string nombre { get; set; }
            public string ciudadOrigen { get; set; }
            public int numeroJugadores { get; set; }
            public double puntaje { get; set; }

            // viene del @JsonProperty("nombreEvento") en Java
            public string nombreEvento { get; set; }
        }

        private void GUIEliminarEQ_Load(object sender, EventArgs e)
        {

        }

        private async void buttonEliminar_Click(object sender, EventArgs e)
        {
            try
            {
                if (!int.TryParse(txtIdEquipo.Text.Trim(), out int idEquipo))
                {
                    MessageBox.Show("Primero ingresa un ID de equipo válido.");
                    return;
                }

                var confirm = MessageBox.Show(
                    $"¿Estás seguro de eliminar el equipo con ID {idEquipo}?",
                    "Confirmar eliminación",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (confirm == DialogResult.No)
                    return;

                using (HttpClient client = new HttpClient())
                {
                    var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes("admin:admin"));
                    client.DefaultRequestHeaders.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", credentials);

                    string url = $"http://localhost:8091/equipos/{idEquipo}";
                    HttpResponseMessage response = await client.DeleteAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        string resultado = await response.Content.ReadAsStringAsync();
                        MessageBox.Show(resultado, "Equipo eliminado",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // limpiar campos
                        txtIdEquipo.Clear();
                        txtNombre.Clear();
                        txtCiudadO.Clear();
                        txtJugadores.Clear();
                        txtPuntaje.Clear();
                        txtEvento.Clear();
                    }
                    else
                    {
                        string errorMsg = await response.Content.ReadAsStringAsync();
                        MessageBox.Show($"Error al eliminar: {errorMsg}",
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al eliminar equipo: " + ex.Message);
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void buttonCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private async void buttonBuscar_Click(object sender, EventArgs e)
        {
            try
            {
                if (!int.TryParse(txtIdEquipo.Text.Trim(), out int idEquipo))
                {
                    MessageBox.Show("Por favor ingresa un ID de equipo válido.");
                    return;
                }

                using (HttpClient client = new HttpClient())
                {
                    var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes("admin:admin"));
                    client.DefaultRequestHeaders.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", credentials);

                    string url = $"http://localhost:8091/equipos/{idEquipo}";
                    HttpResponseMessage response = await client.GetAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        string json = await response.Content.ReadAsStringAsync();

                        var equipo = JsonSerializer.Deserialize<EquipoEliminarDto>(
                            json,
                            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                        // llenar campos
                        txtNombre.Text = equipo.nombre;
                        txtCiudadO.Text = equipo.ciudadOrigen;
                        txtJugadores.Text = equipo.numeroJugadores.ToString();
                        txtPuntaje.Text = equipo.puntaje.ToString();
                        txtEvento.Text = string.IsNullOrEmpty(equipo.nombreEvento)
                                                ? "Sin evento asociado"
                                                : equipo.nombreEvento;

                        // solo lectura
                        txtNombre.ReadOnly = true;
                        txtCiudadO.ReadOnly = true;
                        txtJugadores.ReadOnly = true;
                        txtPuntaje.ReadOnly = true;
                        txtEvento.ReadOnly = true;
                    }
                    else
                    {
                        MessageBox.Show($"No se encontró el equipo con ID {idEquipo}");
                        // limpiar campos
                        txtNombre.Clear();
                        txtCiudadO.Clear();
                        txtJugadores.Clear();
                        txtPuntaje.Clear();
                        txtEvento.Clear();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al buscar equipo: " + ex.Message);
            }
        }
    }
}
