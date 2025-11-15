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
    public partial class GUIBuscarEQ : Form
    {
        public GUIBuscarEQ()
        {
            InitializeComponent();
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private async void buttonBuscar_Click(object sender, EventArgs e)
        {
            try
            {
                string idTexto = txtIdEquipo.Text.Trim();
                if (string.IsNullOrEmpty(idTexto))
                {
                    MessageBox.Show("Por favor ingresa un ID de equipo.");
                    return;
                }

                if (!int.TryParse(idTexto, out int idEquipo))
                {
                    MessageBox.Show("El ID de equipo debe ser un número entero.");
                    return;
                }

                using (HttpClient client = new HttpClient())
                {
                    // Basic Auth
                    var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes("admin:admin"));
                    client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Basic", credentials);

                    string url = $"http://localhost:8091/equipos/{idEquipo}";

                    HttpResponseMessage response = await client.GetAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        string json = await response.Content.ReadAsStringAsync();

                        var equipo = JsonSerializer.Deserialize<EquipoBuscarDto>(
                            json,
                            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                        // Rellenar campos
                        txtNombre.Text = equipo.nombre;
                        txtCiudadO.Text = equipo.ciudadOrigen;
                        txtJugadores.Text = equipo.numeroJugadores.ToString();
                        txtPuntaje.Text = equipo.puntaje.ToString();

                        // nombreEvento viene del backend (getNombreEvento)
                        txtEvento.Text = string.IsNullOrEmpty(equipo.nombreEvento)
                            ? "Sin evento asociado"
                            : equipo.nombreEvento;

                        // Solo lectura
                        txtNombre.ReadOnly = true;
                        txtCiudadO.ReadOnly = true;
                        txtJugadores.ReadOnly = true;
                        txtPuntaje.ReadOnly = true;
                        txtEvento.ReadOnly = true;
                    }
                    else
                    {
                        MessageBox.Show($"No se encontró el equipo con el ID {idEquipo}");

                        // Limpiar si no existe
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

        public class EquipoBuscarDto
        {
            public int idEquipo { get; set; }
            public string nombre { get; set; }
            public string ciudadOrigen { get; set; }
            public int numeroJugadores { get; set; }
            public double puntaje { get; set; }

            // viene del @JsonProperty("nombreEvento") del backend
            public string nombreEvento { get; set; }
        }

        private void buttonCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void GUIBuscarEQ_Load(object sender, EventArgs e)
        {

        }
    }
}
