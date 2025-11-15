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
    public partial class GUIListarParametroEQ : Form
    {
        public GUIListarParametroEQ()
        {
            InitializeComponent();
        }

        private void buttonCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void txtCiudad_TextChanged(object sender, EventArgs e)
        {

        }

        private async void buttonListar_Click(object sender, EventArgs e)
        {
            try
            {
                // Validación del parámetro
                if (!int.TryParse(txtJugadores.Text.Trim(), out int numeroJugadores))
                {
                    MessageBox.Show("Por favor ingresa un número válido.");
                    return;
                }

                using (HttpClient client = new HttpClient())
                {
                    // Basic Auth
                    var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes("admin:admin"));
                    client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Basic", credentials);

                    // Construir URL con el parámetro
                    string url = $"http://localhost:8091/equipos/filtro?numeroJugadores={numeroJugadores}";

                    HttpResponseMessage response = await client.GetAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        string json = await response.Content.ReadAsStringAsync();
                        var equiposList = new List<object>();

                        using (JsonDocument doc = JsonDocument.Parse(json))
                        {
                            foreach (var item in doc.RootElement.EnumerateArray())
                            {
                                int idEquipo = item.GetProperty("idEquipo").GetInt32();
                                string nombre = item.GetProperty("nombre").GetString();
                                string ciudadOrigen = item.GetProperty("ciudadOrigen").GetString();
                                int numJug = item.GetProperty("numeroJugadores").GetInt32();
                                double puntaje = item.GetProperty("puntaje").GetDouble();

                                // nombreEvento viene del @JsonProperty del backend
                                string nombreEvento =
                                    item.TryGetProperty("nombreEvento", out var evProp) &&
                                    evProp.ValueKind != JsonValueKind.Null
                                        ? evProp.GetString()
                                        : "Sin evento";

                                equiposList.Add(new
                                {
                                    IdEquipo = idEquipo,
                                    Nombre = nombre,
                                    CiudadO = ciudadOrigen,
                                    Jugadores = numJug,
                                    Puntaje = puntaje,
                                    Evento = nombreEvento
                                });
                            }
                        }

                        // Mostrar resultados
                        dataGridView1.AutoGenerateColumns = false;
                        dataGridView1.DataSource = equiposList;
                    }
                    else
                    {
                        string errorMsg = await response.Content.ReadAsStringAsync();
                        MessageBox.Show($"Código: {response.StatusCode}\nError: {errorMsg}",
                            "Error del servidor");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al listar equipos por número de jugadores: " + ex.Message);
            }
        }
    }
}
