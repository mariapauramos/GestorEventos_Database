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
    public partial class GUIActualizarED : Form
    {
        public GUIActualizarED()
        {
            InitializeComponent();
        }

        private void buttonCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private async void buttonBuscar_Click(object sender, EventArgs e)
        {
            try
            {
                
                string idEvento = txtIdEvento.Text.Trim();
                if (string.IsNullOrEmpty(idEvento))
                {
                    MessageBox.Show("Por favor ingresa un IdEvento.");
                    return;
                }

                using (HttpClient client = new HttpClient())
                {
                    // Autenticación
                    var credentials = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes("admin:admin"));
                    client.DefaultRequestHeaders.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", credentials);

                    // URL con el ID
                    string url = $"http://localhost:8091/eventos/{idEvento}";

                    // Request GET
                    HttpResponseMessage response = await client.GetAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        string json = await response.Content.ReadAsStringAsync();

                        using (JsonDocument doc = JsonDocument.Parse(json))
                        {
                            var item = doc.RootElement;

                            
                            string nombre = item.GetProperty("nombre").GetString();
                            string ciudad = item.GetProperty("ciudad").GetString();
                            int asistentes = item.GetProperty("asistentes").GetInt32();
                            string fecha = item.GetProperty("fecha").GetString();
                            string tipoDeporte = item.TryGetProperty("tipoDeporte", out var deporteProp)
                                ? deporteProp.GetString()
                                : "";

                            
                            txtNombre.Text = nombre;
                            txtCiudad.Text = ciudad;
                            txtAsistentes.Text = asistentes.ToString();
                            dateTimePicker1.Value = DateTime.Parse(item.GetProperty("fecha").GetString());
                            txtTipoDeporte.Text = tipoDeporte;


                            // Limpiar lista
                            listaEquipos.Items.Clear();

                            // Cargar equipos
                            if (item.TryGetProperty("equipos", out JsonElement equiposProp) &&
                                equiposProp.ValueKind == JsonValueKind.Array)
                            {
                                foreach (var equipoJson in equiposProp.EnumerateArray())
                                {
                                    if (equipoJson.TryGetProperty("nombre", out var nombreEquipoProp))
                                    {
                                        listaEquipos.Items.Add(nombreEquipoProp.GetString());
                                    }
                                }
                            }
                        }

                        txtNombre.ReadOnly = true;
                        txtCiudad.ReadOnly = true;
                        txtAsistentes.ReadOnly = true;
                        txtTipoDeporte.ReadOnly = true;
                        dateTimePicker1.Enabled = false;
                        listaEquipos.Enabled = false;
                        listaEquipos.SelectionMode = SelectionMode.None;
                    }
                    else
                    {
                        MessageBox.Show($"No se encontró el evento con el Id {idEvento}");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al buscar evento: " + ex.Message);
            }

        }

        private void buttonEditar_Click(object sender, EventArgs e)
        {
            txtNombre.ReadOnly = false;
            txtCiudad.ReadOnly = false;
            txtAsistentes.ReadOnly = false;
            dateTimePicker1.Enabled = true;
            txtTipoDeporte.ReadOnly = false;
            listaEquipos.Enabled = false;
            

        }

        private async void buttonGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                string idEvento = txtIdEvento.Text.Trim();
                if (string.IsNullOrEmpty(idEvento))
                {
                    MessageBox.Show("Debes buscar un evento primero.");
                    return;
                }

                using (HttpClient client = new HttpClient())
                {
                    var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes("admin:admin"));
                    client.DefaultRequestHeaders.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", credentials);

                    string url = $"http://localhost:8091/eventos/{idEvento}";

                    // Capturar lista de equipos desde listbox
                    List<string> equiposActualizados = new List<string>();

                    foreach (var item in listaEquipos.Items)
                    {
                        equiposActualizados.Add(item.ToString());
                    }

                    
                    var eventoActualizado = new
                    {
                        idEvento = idEvento,  
                        nombre = txtNombre.Text.Trim(),
                        ciudad = txtCiudad.Text.Trim(),
                        asistentes = int.Parse(txtAsistentes.Text.Trim()),
                        fecha = dateTimePicker1.Value.ToString("yyyy-MM-dd"),
                        tipoDeporte = txtTipoDeporte.Text.Trim(),
                  
                    };

                    string json = JsonSerializer.Serialize(eventoActualizado);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PutAsync(url, content);

                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Evento actualizado");
                    }
                    else
                    {
                        string errorMsg = await response.Content.ReadAsStringAsync();
                        MessageBox.Show($"Error al actualizar: {errorMsg}");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar: " + ex.Message);
            }

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void listaEquipos_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
