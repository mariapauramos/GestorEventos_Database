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
    public partial class GUIListarParametroSD : Form
    {
        public GUIListarParametroSD()
        {
            InitializeComponent();
        }

        private void buttonCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private async void buttonListar_Click(object sender, EventArgs e)
        {
            try
            {
                // Validar capacidad mínima
                if (!int.TryParse(txtCapacidad.Text.Trim(), out int capacidadMin))
                {
                    MessageBox.Show("Por favor ingresa un número válido de capacidad mínima.");
                    return;
                }

                using (HttpClient client = new HttpClient())
                {
                    // Basic Auth
                    var credentials = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes("admin:admin"));
                    client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Basic", credentials);

                    // URL del microservicio
                    string url = $"http://localhost:8092/sedes/buscarPorCapacidad?capacidadMinima={capacidadMin}";

                    HttpResponseMessage response = await client.GetAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        string json = await response.Content.ReadAsStringAsync();
                        var sedesList = new List<object>();

                        using (JsonDocument doc = JsonDocument.Parse(json))
                        {
                            foreach (var item in doc.RootElement.EnumerateArray())
                            {
                                int idSede = item.GetProperty("idSede").GetInt32();
                                string nombre = item.GetProperty("nombre").GetString();
                                string direccion = item.GetProperty("direccion").GetString();
                                int capacidad = item.GetProperty("capacidad").GetInt32();
                                double costoMantenimiento = item.GetProperty("costoMantenimiento").GetDouble();
                                bool cubierta = item.GetProperty("cubierta").GetBoolean();

                                // FECHA — VIENE COMO STRING DESDE SPRING BOOT
                                string fechaCreacion = "";
                                if (item.TryGetProperty("fechaCreacion", out var fechaProp))
                                {
                                    string fechaStr = fechaProp.GetString();

                                    if (DateTime.TryParse(fechaStr, out var fechaParsed))
                                        fechaCreacion = fechaParsed.ToString("yyyy-MM-dd HH:mm");
                                    else
                                        fechaCreacion = fechaStr; // si no convierte, mostrar raw
                                }


                                // ID EVENTO → STRING
                                string idEventoAsociado =
                                    item.TryGetProperty("idEventoAsociado", out var evProp) &&
                                    evProp.ValueKind != JsonValueKind.Null
                                        ? evProp.GetString()
                                        : "N/A";

                              

                                // Agregar filas a la tabla
                                sedesList.Add(new
                                {
                                    IdSede = idSede,
                                    Nombre = nombre,
                                    Direccion = direccion,
                                    Capacidad = capacidad,
                                    Costo = costoMantenimiento,
                                    Cubierta = cubierta ? "Sí" : "No",
                                    FechaCreacion = fechaCreacion,
                                    EventoAsociado = idEventoAsociado

                                });
                            }
                        }

                        // Asignar al DataGridView
                        dataGridView1.AutoGenerateColumns = false;
                        dataGridView1.DataSource = sedesList;
                    }
                    else
                    {
                        string errorMsg = await response.Content.ReadAsStringAsync();
                        MessageBox.Show(
                            $"Código: {response.StatusCode}\nError: {errorMsg}",
                            "Error del servidor"
                        );
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al listar sedes: " + ex.Message);
            }
        }

    }
    public class SedeDto
    {
        public int idSede { get; set; }
        public string nombre { get; set; }
        public int capacidad { get; set; }
        public string direccion { get; set; }
        public double costoMantenimiento { get; set; }
        public bool cubierta { get; set; }
        public DateTime fechaCreacion { get; set; }

        // ID del evento asociado 
        public string idEventoAsociado { get; set; }


        
    }
}
