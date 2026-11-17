using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using FontAwesome.Sharp;

namespace P1
{
    public partial class FrmAperturaCaja : Form
    {
        private Label lblTitulo, lblInstruccion;
        private NumericUpDown numMontoInicial;
        private IconButton btnAbrir, btnCancelar;

        private readonly string connectionString =
            @"Server=DESKTOP-IQ00U68;Database=Libreria;Integrated Security=True;TrustServerCertificate=True;";

        public FrmAperturaCaja()
        {
            ConstruirInterfazManual();
        }

        private void ConstruirInterfazManual()
        {
            this.Text = "Apertura de Caja - Librería Morita";
            this.Size = new Size(500, 350);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.WhiteSmoke;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            lblTitulo = new Label
            {
                Text = "🔓 Apertura de Caja",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.FromArgb(39, 174, 96),
                AutoSize = true,
                Location = new Point(20, 20)
            };
            this.Controls.Add(lblTitulo);

            lblInstruccion = new Label
            {
                Text = $"Empleado: {Sesion.NombreUsuario}\n\n" +
                       "Ingresa el monto inicial con el que comienza el turno:",
                Font = new Font("Segoe UI", 10),
                Location = new Point(20, 70),
                Size = new Size(450, 60)
            };
            this.Controls.Add(lblInstruccion);

            Label lblMonto = new Label
            {
                Text = "Monto Inicial ($):",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Location = new Point(20, 150),
                AutoSize = true
            };
            this.Controls.Add(lblMonto);

            numMontoInicial = new NumericUpDown
            {
                Location = new Point(20, 180),
                Size = new Size(440, 35),
                Font = new Font("Segoe UI", 14),
                DecimalPlaces = 2,
                Maximum = 999999,
                Minimum = 0,
                TextAlign = HorizontalAlignment.Right
            };
            this.Controls.Add(numMontoInicial);

            btnAbrir = new IconButton
            {
                Text = "Abrir Caja",
                IconChar = IconChar.LockOpen,
                IconColor = Color.White,
                IconSize = 20,
                Location = new Point(20, 240),
                Size = new Size(215, 50),
                BackColor = Color.FromArgb(39, 174, 96),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                ImageAlign = ContentAlignment.MiddleLeft,
                TextAlign = ContentAlignment.MiddleRight,
                Font = new Font("Segoe UI", 11, FontStyle.Bold)
            };
            btnAbrir.FlatAppearance.BorderSize = 0;
            btnAbrir.Click += BtnAbrir_Click;
            this.Controls.Add(btnAbrir);

            btnCancelar = new IconButton
            {
                Text = "Cancelar",
                IconChar = IconChar.Times,
                IconColor = Color.White,
                IconSize = 18,
                Location = new Point(245, 240),
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

            this.AcceptButton = btnAbrir;
            this.CancelButton = btnCancelar;
        }

        private void BtnAbrir_Click(object sender, EventArgs e)
        {
            try
            {
                using var con = new SqlConnection(connectionString);
                con.Open();

                // Verificar que no haya otra caja abierta del mismo empleado
                var cmdCheck = new SqlCommand(@"
                    SELECT COUNT(*) FROM Caja 
                    WHERE idEmpleado = @idEmpleado AND estado = 'Abierta'", con);
                cmdCheck.Parameters.AddWithValue("@idEmpleado", Sesion.IdEmpleado);
                int abiertas = Convert.ToInt32(cmdCheck.ExecuteScalar());

                if (abiertas > 0)
                {
                    MessageBox.Show("Ya tienes una caja abierta. Ciérrala antes de abrir otra.",
                        "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Insertar nueva caja
                var cmd = new SqlCommand(@"
                    INSERT INTO Caja (idEmpleado, fecha_apertura, monto_inicial, estado)
                    VALUES (@idEmpleado, GETDATE(), @monto, 'Abierta')", con);

                cmd.Parameters.AddWithValue("@idEmpleado", Sesion.IdEmpleado);
                cmd.Parameters.AddWithValue("@monto", numMontoInicial.Value);
                cmd.ExecuteNonQuery();

                MessageBox.Show($"¡Caja abierta correctamente!\n\nMonto inicial: {numMontoInicial.Value:C2}",
                    "Apertura Exitosa", MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al abrir caja:\n" + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
