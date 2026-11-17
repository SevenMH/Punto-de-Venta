using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using FontAwesome.Sharp;

namespace P1
{
    public partial class FrmAutorEditar : Form
    {
        private TextBox txtNombre, txtApellido, txtNacionalidad;
        private DateTimePicker dtpFechaNacimiento;
        private IconButton btnGuardar, btnCancelar;

        // Propiedad para devolver el ID del autor recién creado
        public int IdAutorCreado { get; private set; } = 0;

        private readonly string connectionString =
            @"Server=DESKTOP-IQ00U68;Database=Libreria;Integrated Security=True;TrustServerCertificate=True;";

        public FrmAutorEditar()
        {
            ConstruirInterfazManual();
        }

        private void ConstruirInterfazManual()
        {
            this.Text = "➕ Nuevo Autor Rápido";
            this.Size = new Size(450, 450); //ancho, alto
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.WhiteSmoke;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            Label lblTitulo = new Label
            {
                Text = "✍️ Registrar Autor",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(155, 89, 182),
                Location = new Point(20, 15),
                AutoSize = true
            };
            this.Controls.Add(lblTitulo);

            int y = 60;
            int xInput = 20;
            int anchoInput = 390;

            // Nombre
            this.Controls.Add(new Label { Text = "Nombre: *", Location = new Point(xInput, y), AutoSize = true, Font = new Font("Segoe UI", 9, FontStyle.Bold) });
            y += 25;
            txtNombre = new TextBox { Location = new Point(xInput, y), Size = new Size(anchoInput, 28), Font = new Font("Segoe UI", 10) };
            this.Controls.Add(txtNombre);
            y += 40;

            // Apellido
            this.Controls.Add(new Label { Text = "Apellido: *", Location = new Point(xInput, y), AutoSize = true, Font = new Font("Segoe UI", 9, FontStyle.Bold) });
            y += 25;
            txtApellido = new TextBox { Location = new Point(xInput, y), Size = new Size(anchoInput, 28), Font = new Font("Segoe UI", 10) };
            this.Controls.Add(txtApellido);
            y += 40;

            // Nacionalidad
            this.Controls.Add(new Label { Text = "Nacionalidad:", Location = new Point(xInput, y), AutoSize = true, Font = new Font("Segoe UI", 9, FontStyle.Bold) });
            y += 25;
            txtNacionalidad = new TextBox { Location = new Point(xInput, y), Size = new Size(anchoInput, 28), Font = new Font("Segoe UI", 10) };
            this.Controls.Add(txtNacionalidad);
            y += 40;

            // Fecha Nacimiento
            this.Controls.Add(new Label { Text = "Fecha de Nacimiento: *", Location = new Point(xInput, y), AutoSize = true, Font = new Font("Segoe UI", 9, FontStyle.Bold) });
            y += 25;
            dtpFechaNacimiento = new DateTimePicker { Location = new Point(xInput, y), Size = new Size(anchoInput, 28), Font = new Font("Segoe UI", 10), Format = DateTimePickerFormat.Short, Value = DateTime.Today };
            this.Controls.Add(dtpFechaNacimiento);
            y += 45;

            // Botones
            btnGuardar = new IconButton
            {
                Text = "Guardar",
                IconChar = IconChar.Save,
                IconColor = Color.White,
                IconSize = 16,
                Location = new Point(140, y),
                Size = new Size(130, 40),
                BackColor = Color.FromArgb(39, 174, 96),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                ImageAlign = ContentAlignment.MiddleLeft,
                TextAlign = ContentAlignment.MiddleRight,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            btnGuardar.FlatAppearance.BorderSize = 0;
            btnGuardar.Click += BtnGuardar_Click;
            this.Controls.Add(btnGuardar);

            btnCancelar = new IconButton
            {
                Text = "Cancelar",
                IconChar = IconChar.Times,
                IconColor = Color.White,
                IconSize = 16,
                Location = new Point(280, y),
                Size = new Size(130, 40),
                BackColor = Color.Gray,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                ImageAlign = ContentAlignment.MiddleLeft,
                TextAlign = ContentAlignment.MiddleRight,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            btnCancelar.FlatAppearance.BorderSize = 0;
            btnCancelar.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };
            this.Controls.Add(btnCancelar);
        }

        private void BtnGuardar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text) || string.IsNullOrWhiteSpace(txtApellido.Text))
            {
                MessageBox.Show("El nombre y el apellido son obligatorios.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using var con = new SqlConnection(connectionString);
                con.Open();

                // Insertamos y obtenemos el ID generado
                var cmd = new SqlCommand(@"
                    INSERT INTO Autor (nombre, apellido, nacionalidad, fecha_nacimiento)
                    OUTPUT INSERTED.id_autor
                    VALUES (@nombre, @apellido, @nacionalidad, @fecha)", con);

                cmd.Parameters.AddWithValue("@nombre", txtNombre.Text.Trim());
                cmd.Parameters.AddWithValue("@apellido", txtApellido.Text.Trim());
                cmd.Parameters.AddWithValue("@nacionalidad", string.IsNullOrWhiteSpace(txtNacionalidad.Text) ? (object)DBNull.Value : txtNacionalidad.Text.Trim());
                cmd.Parameters.AddWithValue("@fecha", dtpFechaNacimiento.Value.Date);

                IdAutorCreado = (int)cmd.ExecuteScalar(); // Guardamos el ID

                MessageBox.Show("Autor registrado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar autor:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}