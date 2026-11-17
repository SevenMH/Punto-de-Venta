using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using FontAwesome.Sharp;

namespace P1
{
    public partial class FrmCierreCaja : Form
    {
        private readonly int idCaja;

        private Label lblTitulo;
        private Label lblMontoInicial, lblVentasEfectivo, lblIngresos, lblEgresos, lblEsperado;
        private Label lblContado, lblDiferencia;
        private NumericUpDown numContado;
        private IconButton btnCerrar, btnCancelar;

        private readonly string connectionString =
            @"Server=DESKTOP-IQ00U68;Database=Libreria;Integrated Security=True;TrustServerCertificate=True;";

        private decimal efectivoEsperado = 0;

        public FrmCierreCaja(int idCaja)
        {
            this.idCaja = idCaja;
            ConstruirInterfazManual();
            this.Load += (s, e) => CargarResumen();
        }

        private void ConstruirInterfazManual()
        {
            this.Text = "Cierre de Caja - Librería Morita";
            this.Size = new Size(560, 650);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.WhiteSmoke;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            lblTitulo = new Label
            {
                Text = "🔒 Cierre de Caja",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.FromArgb(192, 57, 43),
                AutoSize = true,
                Location = new Point(20, 15)
            };
            this.Controls.Add(lblTitulo);

            // Panel resumen
            Panel panel = new Panel
            {
                Location = new Point(20, 65),
                Size = new Size(505, 260),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            int y = 15;
            panel.Controls.Add(new Label { Text = "Resumen del turno:", Location = new Point(15, y), AutoSize = true, Font = new Font("Segoe UI", 11, FontStyle.Bold) });
            y += 35;

            panel.Controls.Add(new Label { Text = "Monto inicial:", Location = new Point(15, y), AutoSize = true, Font = new Font("Segoe UI", 10) });
            lblMontoInicial = new Label { Text = "$0.00", Location = new Point(400, y), AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
            panel.Controls.Add(lblMontoInicial);
            y += 30;

            panel.Controls.Add(new Label { Text = "Ventas en efectivo:", Location = new Point(15, y), AutoSize = true, Font = new Font("Segoe UI", 10) });
            lblVentasEfectivo = new Label { Text = "$0.00", Location = new Point(400, y), AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = Color.FromArgb(39, 174, 96) };
            panel.Controls.Add(lblVentasEfectivo);
            y += 30;

            panel.Controls.Add(new Label { Text = "(+) Ingresos extra:", Location = new Point(15, y), AutoSize = true, Font = new Font("Segoe UI", 10) });
            lblIngresos = new Label { Text = "$0.00", Location = new Point(400, y), AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = Color.FromArgb(41, 128, 185) };
            panel.Controls.Add(lblIngresos);
            y += 30;

            panel.Controls.Add(new Label { Text = "(-) Egresos:", Location = new Point(15, y), AutoSize = true, Font = new Font("Segoe UI", 10) });
            lblEgresos = new Label { Text = "$0.00", Location = new Point(400, y), AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = Color.FromArgb(192, 57, 43) };
            panel.Controls.Add(lblEgresos);
            y += 35;

            panel.Controls.Add(new Label { Text = "EFECTIVO ESPERADO:", Location = new Point(15, y), AutoSize = true, Font = new Font("Segoe UI", 12, FontStyle.Bold) });
            lblEsperado = new Label { Text = "$0.00", Location = new Point(370, y), AutoSize = true, Font = new Font("Segoe UI", 14, FontStyle.Bold), ForeColor = Color.FromArgb(155, 89, 182) };
            panel.Controls.Add(lblEsperado);

            this.Controls.Add(panel);

            // Conteo real
            Label lblConteo = new Label
            {
                Text = "💵 Ingresa el efectivo CONTADO en caja:",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Location = new Point(20, 345),
                AutoSize = true
            };
            this.Controls.Add(lblConteo);

            numContado = new NumericUpDown
            {
                Location = new Point(20, 375),
                Size = new Size(505, 40),
                Font = new Font("Segoe UI", 14),
                DecimalPlaces = 2,
                Maximum = 999999,
                TextAlign = HorizontalAlignment.Right
            };
            numContado.ValueChanged += (s, e) => CalcularDiferencia();
            this.Controls.Add(numContado);

            // Diferencia
            Label lblDifTitulo = new Label
            {
                Text = "Diferencia:",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Location = new Point(20, 440),
                AutoSize = true
            };
            this.Controls.Add(lblDifTitulo);

            lblDiferencia = new Label
            {
                Text = "$0.00",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                Location = new Point(200, 432),
                AutoSize = true,
                ForeColor = Color.Gray
            };
            this.Controls.Add(lblDiferencia);

            lblContado = new Label
            {
                Text = "",
                Font = new Font("Segoe UI", 9, FontStyle.Italic),
                ForeColor = Color.Gray,
                Location = new Point(20, 490),
                Size = new Size(505, 40)
            };
            this.Controls.Add(lblContado);

            // Botones
            btnCerrar = new IconButton
            {
                Text = "Cerrar Caja",
                IconChar = IconChar.Lock,
                IconColor = Color.White,
                IconSize = 20,
                Location = new Point(20, 545),
                Size = new Size(250, 50),
                BackColor = Color.FromArgb(192, 57, 43),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                ImageAlign = ContentAlignment.MiddleLeft,
                TextAlign = ContentAlignment.MiddleRight,
                Font = new Font("Segoe UI", 12, FontStyle.Bold)
            };
            btnCerrar.FlatAppearance.BorderSize = 0;
            btnCerrar.Click += BtnCerrar_Click;
            this.Controls.Add(btnCerrar);

            btnCancelar = new IconButton
            {
                Text = "Cancelar",
                IconChar = IconChar.Times,
                IconColor = Color.White,
                IconSize = 18,
                Location = new Point(280, 545),
                Size = new Size(245, 50),
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
        }

        private void CargarResumen()
        {
            try
            {
                using var con = new SqlConnection(connectionString);
                con.Open();

                decimal montoInicial = ObtenerDecimal(con, "SELECT monto_inicial FROM Caja WHERE id_caja = @id", idCaja);
                decimal ventasEfectivo = ObtenerDecimal(con,
                    @"SELECT ISNULL(SUM(monto_total), 0) FROM Pedido 
                      WHERE id_caja = @id AND estado = 'Pagado' 
                        AND (metodo_pago = 'Efectivo' OR metodo_pago IS NULL)", idCaja);
                decimal ingresos = ObtenerDecimal(con,
                    "SELECT ISNULL(SUM(monto), 0) FROM MovimientoCaja WHERE id_caja = @id AND tipo = 'Ingreso'", idCaja);
                decimal egresos = ObtenerDecimal(con,
                    "SELECT ISNULL(SUM(monto), 0) FROM MovimientoCaja WHERE id_caja = @id AND tipo = 'Egreso'", idCaja);

                efectivoEsperado = montoInicial + ventasEfectivo + ingresos - egresos;

                lblMontoInicial.Text = montoInicial.ToString("C2");
                lblVentasEfectivo.Text = ventasEfectivo.ToString("C2");
                lblIngresos.Text = ingresos.ToString("C2");
                lblEgresos.Text = egresos.ToString("C2");
                lblEsperado.Text = efectivoEsperado.ToString("C2");

                numContado.Value = efectivoEsperado;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar resumen:\n" + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private decimal ObtenerDecimal(SqlConnection con, string query, int id)
        {
            var cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@id", id);
            var result = cmd.ExecuteScalar();
            return result != null && result != DBNull.Value ? Convert.ToDecimal(result) : 0;
        }

        private void CalcularDiferencia()
        {
            decimal diff = numContado.Value - efectivoEsperado;
            lblDiferencia.Text = diff.ToString("C2");

            if (diff == 0)
            {
                lblDiferencia.ForeColor = Color.FromArgb(39, 174, 96);
                lblContado.Text = "✅ Caja cuadrada perfectamente.";
                lblContado.ForeColor = Color.FromArgb(39, 174, 96);
            }
            else if (diff > 0)
            {
                lblDiferencia.ForeColor = Color.FromArgb(41, 128, 185);
                lblContado.Text = $"⚠️ Hay un SOBRANTE de {diff:C2} respecto al esperado.";
                lblContado.ForeColor = Color.FromArgb(41, 128, 185);
            }
            else
            {
                lblDiferencia.ForeColor = Color.FromArgb(192, 57, 43);
                lblContado.Text = $"⚠️ Hay un FALTANTE de {Math.Abs(diff):C2} respecto al esperado.";
                lblContado.ForeColor = Color.FromArgb(192, 57, 43);
            }
        }

        private void BtnCerrar_Click(object sender, EventArgs e)
        {
            decimal diff = numContado.Value - efectivoEsperado;

            string mensaje = $"¿Confirmar cierre de caja?\n\n" +
                             $"Efectivo esperado: {efectivoEsperado:C2}\n" +
                             $"Efectivo contado:  {numContado.Value:C2}\n" +
                             $"Diferencia:        {diff:C2}\n\n";

            if (diff != 0)
                mensaje += "⚠️ Hay una diferencia. ¿Continuar de todos modos?";

            var confirm = MessageBox.Show(mensaje, "Confirmar Cierre",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;

            try
            {
                using var con = new SqlConnection(connectionString);
                con.Open();

                var cmd = new SqlCommand(@"
                    UPDATE Caja SET 
                        fecha_cierre = GETDATE(),
                        monto_final = @montoFinal,
                        estado = 'Cerrada'
                    WHERE id_caja = @id", con);

                cmd.Parameters.AddWithValue("@id", idCaja);
                cmd.Parameters.AddWithValue("@montoFinal", numContado.Value);
                cmd.ExecuteNonQuery();

                MessageBox.Show("¡Caja cerrada correctamente!", "Cierre Exitoso",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cerrar caja:\n" + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
