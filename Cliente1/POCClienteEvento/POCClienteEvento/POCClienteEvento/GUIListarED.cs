using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;




namespace POCClienteEvento
{
    public partial class GUIListarED : Form
    {
        public GUIListarED()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
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
                    
                    var credentials = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes("admin:admin"));
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", credentials);

                    // URL del servidor
                    string url = "http://localhost:8091/eventos/";

                    // Petición GET al servidor
                    HttpResponseMessage response = await client.GetAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        
                        var json = await response.Content.ReadAsStringAsync();

                        
                        using (JsonDocument doc = JsonDocument.Parse(json))
                        {
                            var eventos = new List<object>(); // lista genérica para cargar a ls tabla

                            foreach (var item in doc.RootElement.EnumerateArray())
                            {
                                
                                string idEvento = item.GetProperty("idEvento").GetString();
                                string nombre = item.GetProperty("nombre").GetString();
                                string ciudad = item.GetProperty("ciudad").GetString();
                                int asistentes = item.GetProperty("asistentes").GetInt32();
                                string fecha = item.GetProperty("fecha").GetString();

                                
                                string tipoDeporte = item.TryGetProperty("tipoDeporte", out var deporteProp)
                                    ? deporteProp.GetString()
                                    : "";

                                string equiposTexto = "";

                                
                                if (item.TryGetProperty("equipos", out JsonElement equiposProp) &&
                                    equiposProp.ValueKind == JsonValueKind.Array &&
                                    equiposProp.GetArrayLength() > 0)
                                {
                                    List<string> nEquipos = new List<string>();

                                    foreach (var equipo in equiposProp.EnumerateArray())
                                    {
                                        string idEquipo = equipo.TryGetProperty("idEquipo", out var idProp)
                                            ? idProp.GetInt32().ToString()
                                            : "";
                                        string nombreEquipo = equipo.TryGetProperty("nombre", out var nombreProp)
                                            ? nombreProp.GetString()
                                            : "";

                                        // Combinar id + nombre
                                        nEquipos.Add($"[{idEquipo}] {nombreEquipo}");
                                    }

                                    equiposTexto = string.Join(Environment.NewLine, nEquipos);
                                }

                                // Configurar la columna Equipos para saltos de línea
                                if (dataGridView1.Columns.Contains("Equipos"))
                                {
                                    dataGridView1.Columns["Equipos"].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                                    dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
                                }



                                // Crear objeto para mostrar en tabla
                                eventos.Add(new
                                {
                                    IdEvento = idEvento,
                                    Nombre = nombre,
                                    Ciudad = ciudad,
                                    Asistentes = asistentes,
                                    Fecha = fecha,
                                    TipoDeporte = tipoDeporte,
                                    Equipos = equiposTexto
                                });
                            }

                            // Asignar la lista a la tabla
                            dataGridView1.AutoGenerateColumns = false  ;
                            dataGridView1.DataSource = eventos;


                        }
                    }
                    else
                    {
                        string errorMsg = await response.Content.ReadAsStringAsync();
                        MessageBox.Show($"Código: {response.StatusCode}\nError: {errorMsg}", "Error del servidor",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al listar eventos: " + ex.Message);
            }

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
