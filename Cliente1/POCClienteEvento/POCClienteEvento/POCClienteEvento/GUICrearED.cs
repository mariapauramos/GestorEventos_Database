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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace POCClienteEvento
{
    public partial class GUICrearED : Form
    {
        public GUICrearED()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void buttonCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
         
        private async void buttonCrear_Click(object sender, EventArgs e)
        {

            // Crear objeto con los datos del evento
            var evento = new
            {
                idEvento = txtIdEvento.Text.Trim(),
                nombre = txtNombre.Text.Trim(),
                ciudad = txtCiudad.Text.Trim(),
                asistentes = int.TryParse(txtAsistentes.Text.Trim(), out int a) ? a : 0,
                fecha = dateTimePickerFecha.Value.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture),
                tipoDeporte = txtTipoDeporte.Text.Trim()
            };

            try
            {
                using (var client = new HttpClient())
                {
                    // Autenticación Basic Auth
                    var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes("admin:admin"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);

                    var json = JsonSerializer.Serialize(evento);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PostAsync("http://localhost:8091/eventos/", content);

                    if (response.IsSuccessStatusCode)
                    {
                        string jsonResponse = await response.Content.ReadAsStringAsync();

                        // Deserializar la respuesta del backend para obtener el idEvento generado
                        var eventoCreado = JsonSerializer.Deserialize<EventoD>(jsonResponse);

                        // Mostrar el ID generado en el TextBox readonly
                        txtIdEvento.Text = eventoCreado.idEvento;

                        MessageBox.Show("Evento creado", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // Limpiar los campos
                        txtNombre.Clear();
                        txtCiudad.Clear();
                        txtAsistentes.Clear();
                        txtTipoDeporte.Clear();
                        dateTimePickerFecha.Value = DateTime.Now;
                    }
                    else
                    {
                        string errorMsg = await response.Content.ReadAsStringAsync();
                        MessageBox.Show($"Código: {response.StatusCode}\n\nError:\n{errorMsg}", "Error del servidor", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocurrió un error: " + ex.Message);
            }
        }

            public class EventoD
        {
            public String idEvento { get; set; }
            public string nombre { get; set; }
            public string ciudad { get; set; }
            public int asistentes { get; set; }
            public string fecha { get; set; }
            public string tipoDeporte { get; set; }
        }
    }
}
       
    