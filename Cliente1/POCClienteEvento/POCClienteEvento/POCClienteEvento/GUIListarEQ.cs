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
    public partial class GUIListarEQ : Form
    {
        public GUIListarEQ()
        {
            InitializeComponent();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void buttonCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private async void buttonListar_Click(object sender, EventArgs e)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    // Basic Auth
                    var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes("admin:admin"));
                    client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Basic", credentials);

                    string url = "http://localhost:8091/equipos/";

                    HttpResponseMessage response = await client.GetAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        string json = await response.Content.ReadAsStringAsync();

                        var listaEquipos = new List<object>();

                        using (JsonDocument doc = JsonDocument.Parse(json))
                        {
                            foreach (var item in doc.RootElement.EnumerateArray())
                            {
                                int idEquipo = item.GetProperty("idEquipo").GetInt32();
                                string nombre = item.GetProperty("nombre").GetString();
                                string ciudadOrigen = item.GetProperty("ciudadOrigen").GetString();
                                int Jugadores = item.GetProperty("numeroJugadores").GetInt32();
                                double puntaje = item.GetProperty("puntaje").GetDouble();

                                // nombreEvento viene del @JsonProperty("nombreEvento")
                                string nombreEvento =
                                    item.TryGetProperty("nombreEvento", out var evProp) &&
                                    evProp.ValueKind != JsonValueKind.Null
                                        ? evProp.GetString()
                                        : "Sin evento";

                                // Objeto anónimo que se va a mostrar en la grilla
                                listaEquipos.Add(new
                                {
                                    IdEquipo = idEquipo,
                                    Nombre = nombre,
                                    CiudadO = ciudadOrigen,
                                    Jugadores = Jugadores,
                                    Puntaje = puntaje,
                                    Evento = nombreEvento
                                });
                            }
                        }

                        // Asegúrate de que las columnas del DataGridView tienen estos DataPropertyName:
                        // IdEquipo, Nombre, CiudadOrigen, NumeroJugadores, Puntaje, EventoDeportivo
                        dataGridView1.AutoGenerateColumns = false;
                        dataGridView1.DataSource = listaEquipos;
                    }
                    else
                    {
                        string errorMsg = await response.Content.ReadAsStringAsync();
                        MessageBox.Show($"Código: {response.StatusCode}\nError: {errorMsg}",
                            "Error del servidor", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al listar equipos: " + ex.Message);
            }
        }
    }
}
