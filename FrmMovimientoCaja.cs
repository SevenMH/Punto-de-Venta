using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using FontAwesome.Sharp;

namespace P1
{
    public partial class FrmMovimientoCaja : Form
    {
        private readonly int idCaja;

        private Label lblTitulo;
        private RadioButton rbIngreso, rbEgreso;
        private NumericUpDown numMonto;
        private TextBox txtDescripcion;
        private IconButton btnGuardar, btnCancelar;

        private readonly string connectionString =
            @"Server=DESKTOP-IQ00U68;Database=Libreria;Integrated Security=True;TrustServerCertificate=True;";

        public FrmMovimientoCaja(int idCaja)
        {
            this.idCaja = idCaja;
            ConstruirInterfazManual();
        }

        private void ConstruirInterfazManual()
        {
            this.Text = "Movimiento de Caja - Librería Morita";
            this.Size = new Size(500, 420);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.WhiteSmoke;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            lblTitulo = new Label
            {
                Text = "💰 Registrar Movimiento",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(41, 128, 185),
                AutoSize = true,
                Location = new Point(20, 20)
            };
            this.Controls.Add(lblTitulo);

            // Tipo
            Label lblTipo = new Label
            {
                Text = "Tipo de movimiento:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(20, 75),
                AutoSize = true
            };
            this.Controls.Add(lblTipo);

            rbIngreso = new RadioButton
            {
                Text = "Ingreso (entra dinero)",
                Location = new Point(20, 105),
                Size = new Size(200, 25),
                Font = new Font("Segoe UI", 10),
                Checked = true,
                ForeColor = Color.FromArgb(39, 174, 96)
            };
            this.Controls.Add(rbIngreso);

            rbEgreso = new RadioButton
            {
                Text = "Egreso (sale dinero)",
                Location = new Point(240, 105),
                Size = new Size(200, 25),
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(192, 57, 43)
            };
            this.Controls.Add(rbEgreso);

            // Monto
            Label lblMonto = new Label
            {
                Text = "Monto ($):",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(20, 155),
                AutoSize = true
            };
            this.Controls.Add(lblMonto);

            numMonto = new NumericUpDown
            {
                Location = new Point(20, 180),
                Size = new Size(440, 32),
                Font = new Font("Segoe UI", 12),
                DecimalPlaces = 2,
                Maximum = 999999,
                Minimum = 0.01m,
                TextAlign = HorizontalAlignment.Right
            };
            this.Controls.Add(numMonto);

            // Descripción
            Label lblDesc = new Label
            {
                Text = "Descripción:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(20, 225),
                AutoSize = true
            };
            this.Controls.Add(lblDesc);

            txtDescripcion = new TextBox
            {
                Location = new Point(20, 250),
                Size = new Size(440, 32),
                Font = new Font("Segoe UI", 10),
                MaxLength = 200,
                PlaceholderText = "Ej: Pago de servicio de luz, propina, etc."
            };
            this.Controls.Add(txtDescripcion);

            // Botones
            btnGuardar = new IconButton
            {
                Text = "Guardar",
                IconChar = IconChar.Save,
                IconColor = Color.White,
                IconSize = 18,
                Location = new Point(20, 310),
                Size = new Size(215, 50),
                BackColor = Color.FromArgb(39, 174, 96),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                ImageAlign = ContentAlignment.MiddleLeft,
                TextAlign = ContentAlignment.MiddleRight,
                Font = new Font("Segoe UI", 11, FontStyle.Bold)
            };
            btnGuardar.FlatAppearance.BorderSize = 0;
            btnGuardar.Click += BtnGuardar_Click;
            this.Controls.Add(btnGuardar);

            btnCancelar = new IconButton
            {
                Text = "Cancelar",
                IconChar = IconChar.Times,
                IconColor = Color.White,
                IconSize = 18,
                Location = new Point(245, 310),
                Size = new Size(215, 50),
                BackColor = Color.Gray,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                ImageAlign = ContentAlignment.MiddleLeft,
                TextAlign = ContentAlignment.MiddleRight,
                Font = new Font("Segoe UI", 11, FontStyle.Bold)
            };
            btnCancelar.FlatAppearance.BorderSize = 0;
            btnCancelar.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };
            this.Controls.Add(btnCancelar);

            this.AcceptButton = btnGuardar;
            this.CancelButton = btnCancelar;
        }

        private void BtnGuardar_Click(object sender, EventArgs e)
        {
            if (numMonto.Value <= 0)
            {
                MessageBox.Show("El monto debe ser mayor a 0.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtDescripcion.Text))
            {
                MessageBox.Show("La descripción es obligatoria.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtDescripcion.Focus();
                return;
            }

            try
            {
                using var con = new SqlConnection(connectionString);
                con.Open();

                var cmd = new SqlCommand(@"
                    INSERT INTO MovimientoCaja (id_caja, tipo, monto, descripcion, fecha_movimiento)
                    VALUES (@idCaja, @tipo, @monto, @descripcion, GETDATE())", con);

                cmd.Parameters.AddWithValue("@idCaja", idCaja);
                cmd.Parameters.AddWithValue("@tipo", rbIngreso.Checked ? "Ingreso" : "Egreso");
                cmd.Parameters.AddWithValue("@monto", numMonto.Value);
                cmd.Parameters.AddWithValue("@descripcion", txtDescripcion.Text.Trim());

                cmd.ExecuteNonQuery();

                MessageBox.Show("Movimiento registrado correctamente.", "Éxito",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar movimiento:\n" + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
