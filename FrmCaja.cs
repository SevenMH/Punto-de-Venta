using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using FontAwesome.Sharp;

namespace P1
{
    public partial class FrmCaja : Form
    {
        // Controles
        private Label lblTitulo, lblEstado, lblFechaApertura, lblMontoInicial;
        private Label lblVentasEfectivo, lblVentasTarjeta, lblIngresos, lblEgresos, lblEsperado;
        private Label lblHistorial;
        private Panel panelEstado, panelResumen;
        private IconButton btnAbrir, btnCerrarCaja, btnMovimiento, btnRefrescar, btnSalir;
        private DataGridView dgvHistorial;

        private readonly string connectionString =
            @"Server=DESKTOP-IQ00U68;Database=Libreria;Integrated Security=True;TrustServerCertificate=True;";

        private int? idCajaActual = null;

        public FrmCaja()
        {
            ConstruirInterfazManual();
            this.Load += (s, e) =>
            {
                CargarEstadoCaja();
                CargarHistorial();
            };
        }

        private void ConstruirInterfazManual()
        {
            this.Text = "Módulo de Caja - Librería Morita";
            this.Size = new Size(1100, 720);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.WhiteSmoke;
            this.MinimumSize = new Size(1000, 650);

            // HEADER
            Panel panelHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = Color.FromArgb(39, 174, 96)  // Verde
            };

            lblTitulo = new Label
            {
                Text = "💰 Módulo de Caja",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(20, 15),
                BackColor = Color.Transparent
            };
            panelHeader.Controls.Add(lblTitulo);

            btnSalir = new IconButton
            {
                Text = "Salir",
                IconChar = IconChar.SignOutAlt,
                IconColor = Color.White,
                IconSize = 16,
                Size = new Size(100, 35),
                BackColor = Color.FromArgb(192, 57, 43),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                ImageAlign = ContentAlignment.MiddleLeft,
                TextAlign = ContentAlignment.MiddleRight,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Location = new Point(panelHeader.Width - 120, 12)
            };
            btnSalir.FlatAppearance.BorderSize = 0;
            btnSalir.Click += (s, e) => this.Close();
            panelHeader.Controls.Add(btnSalir);

            panelHeader.Resize += (s, e) =>
            {
                btnSalir.Location = new Point(panelHeader.Width - 120, 12);
            };

            this.Controls.Add(panelHeader);

            // ============ PANEL ESTADO ============
            panelEstado = new Panel
            {
                Location = new Point(20, 80),
                Size = new Size(1050, 130),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            lblEstado = new Label
            {
                Text = "Estado:",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Location = new Point(20, 15),
                AutoSize = true
            };
            panelEstado.Controls.Add(lblEstado);

            lblFechaApertura = new Label
            {
                Text = "",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.Gray,
                Location = new Point(20, 50),
                AutoSize = true
            };
            panelEstado.Controls.Add(lblFechaApertura);

            lblMontoInicial = new Label
            {
                Text = "",
                Font = new Font("Segoe UI", 11),
                Location = new Point(20, 75),
                AutoSize = true
            };
            panelEstado.Controls.Add(lblMontoInicial);

            btnAbrir = new IconButton
            {
                Text = "Abrir Caja",
                IconChar = IconChar.LockOpen,
                IconColor = Color.White,
                IconSize = 20,
                Size = new Size(160, 50),
                Location = new Point(500, 40),
                BackColor = Color.FromArgb(39, 174, 96),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                ImageAlign = ContentAlignment.MiddleLeft,
                TextAlign = ContentAlignment.MiddleRight,
                Font = new Font("Segoe UI", 11, FontStyle.Bold)
            };
            btnAbrir.FlatAppearance.BorderSize = 0;
            btnAbrir.Click += BtnAbrir_Click;
            panelEstado.Controls.Add(btnAbrir);

            btnMovimiento = new IconButton
            {
                Text = "Movimiento",
                IconChar = IconChar.MoneyBillWave,
                IconColor = Color.White,
                IconSize = 18,
                Size = new Size(150, 50),
                Location = new Point(680, 40),
                BackColor = Color.FromArgb(41, 128, 185),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                ImageAlign = ContentAlignment.MiddleLeft,
                TextAlign = ContentAlignment.MiddleRight,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            btnMovimiento.FlatAppearance.BorderSize = 0;
            btnMovimiento.Click += BtnMovimiento_Click;
            panelEstado.Controls.Add(btnMovimiento);

            btnCerrarCaja = new IconButton
            {
                Text = "Cerrar Caja",
                IconChar = IconChar.Lock,
                IconColor = Color.White,
                IconSize = 18,
                Size = new Size(160, 50),
                Location = new Point(850, 40),
                BackColor = Color.FromArgb(192, 57, 43),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                ImageAlign = ContentAlignment.MiddleLeft,
                TextAlign = ContentAlignment.MiddleRight,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            btnCerrarCaja.FlatAppearance.BorderSize = 0;
            btnCerrarCaja.Click += BtnCerrarCaja_Click;
            panelEstado.Controls.Add(btnCerrarCaja);

            this.Controls.Add(panelEstado);

            // ============ PANEL RESUMEN ============
            panelResumen = new Panel
            {
                Location = new Point(20, 220),
                Size = new Size(1050, 130),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            Label lblResumenTitulo = new Label
            {
                Text = "📊 Resumen del Turno Actual",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(39, 174, 96),
                Location = new Point(20, 10),
                AutoSize = true
            };
            panelResumen.Controls.Add(lblResumenTitulo);

            // Ventas Efectivo
            panelResumen.Controls.Add(new Label
            {
                Text = "Ventas (Efectivo):",
                Location = new Point(20, 45),
                AutoSize = true,
                Font = new Font("Segoe UI", 10)
            });
            lblVentasEfectivo = new Label
            {
                Text = "$0.00",
                Location = new Point(20, 70),
                AutoSize = true,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(39, 174, 96)
            };
            panelResumen.Controls.Add(lblVentasEfectivo);

            // Ventas Tarjeta
            panelResumen.Controls.Add(new Label
            {
                Text = "Ventas (Tarjeta/Otro):",
                Location = new Point(220, 45),
                AutoSize = true,
                Font = new Font("Segoe UI", 10)
            });
            lblVentasTarjeta = new Label
            {
                Text = "$0.00",
                Location = new Point(220, 70),
                AutoSize = true,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94)
            };
            panelResumen.Controls.Add(lblVentasTarjeta);

            // Ingresos
            panelResumen.Controls.Add(new Label
            {
                Text = "Ingresos Extra:",
                Location = new Point(470, 45),
                AutoSize = true,
                Font = new Font("Segoe UI", 10)
            });
            lblIngresos = new Label
            {
                Text = "$0.00",
                Location = new Point(470, 70),
                AutoSize = true,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(41, 128, 185)
            };
            panelResumen.Controls.Add(lblIngresos);

            // Egresos
            panelResumen.Controls.Add(new Label
            {
                Text = "Egresos:",
                Location = new Point(670, 45),
                AutoSize = true,
                Font = new Font("Segoe UI", 10)
            });
            lblEgresos = new Label
            {
                Text = "$0.00",
                Location = new Point(670, 70),
                AutoSize = true,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(192, 57, 43)
            };
            panelResumen.Controls.Add(lblEgresos);

            // Efectivo esperado
            panelResumen.Controls.Add(new Label
            {
                Text = "Efectivo Esperado:",
                Location = new Point(850, 45),
                AutoSize = true,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            });
            lblEsperado = new Label
            {
                Text = "$0.00",
                Location = new Point(850, 70),
                AutoSize = true,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(155, 89, 182)
            };
            panelResumen.Controls.Add(lblEsperado);

            this.Controls.Add(panelResumen);

            // ============ HISTORIAL ============
            lblHistorial = new Label
            {
                Text = "📋 Historial de Cajas Cerradas",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(41, 128, 185),
                Location = new Point(20, 365),
                AutoSize = true
            };
            this.Controls.Add(lblHistorial);

            dgvHistorial = new DataGridView
            {
                Location = new Point(20, 400),
                Size = new Size(1050, 270),
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                RowHeadersVisible = false,
                Font = new Font("Segoe UI", 10),
                ColumnHeadersHeight = 32,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
            };
            dgvHistorial.Columns.Add("IdCaja", "ID");
            dgvHistorial.Columns.Add("Empleado", "Empleado");
            dgvHistorial.Columns.Add("FechaApertura", "Apertura");
            dgvHistorial.Columns.Add("FechaCierre", "Cierre");
            dgvHistorial.Columns.Add("MontoInicial", "M. Inicial");
            dgvHistorial.Columns.Add("MontoFinal", "M. Final");
            dgvHistorial.Columns.Add("Diferencia", "Diferencia");
            dgvHistorial.Columns["IdCaja"].Visible = false;
            dgvHistorial.Columns["MontoInicial"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvHistorial.Columns["MontoFinal"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvHistorial.Columns["Diferencia"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvHistorial.CellFormatting += DgvHistorial_CellFormatting;

            this.Controls.Add(dgvHistorial);
        }

        // ============ CARGAR ESTADO ============
        private void CargarEstadoCaja()
        {
            try
            {
                using var con = new SqlConnection(connectionString);
                con.Open();

                var cmd = new SqlCommand(@"
                    SELECT TOP 1 id_caja, fecha_apertura, monto_inicial
                    FROM Caja 
                    WHERE idEmpleado = @idEmpleado AND estado = 'Abierta'
                    ORDER BY fecha_apertura DESC", con);
                cmd.Parameters.AddWithValue("@idEmpleado", Sesion.IdEmpleado);

                using var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    idCajaActual = Convert.ToInt32(reader["id_caja"]);
                    DateTime apertura = Convert.ToDateTime(reader["fecha_apertura"]);
                    decimal montoInicial = Convert.ToDecimal(reader["monto_inicial"]);

                    lblEstado.Text = "Estado: 🟢 CAJA ABIERTA";
                    lblEstado.ForeColor = Color.FromArgb(39, 174, 96);
                    lblFechaApertura.Text = $"Apertura: {apertura:dd/MM/yyyy HH:mm}";
                    lblMontoInicial.Text = $"Monto inicial: {montoInicial:C2}";

                    btnAbrir.Enabled = false;
                    btnAbrir.BackColor = Color.Gray;
                    btnMovimiento.Enabled = true;
                    btnCerrarCaja.Enabled = true;

                    reader.Close();
                    CalcularResumen(con, idCajaActual.Value);
                }
                else
                {
                    idCajaActual = null;
                    lblEstado.Text = "Estado: 🔴 CAJA CERRADA";
                    lblEstado.ForeColor = Color.FromArgb(192, 57, 43);
                    lblFechaApertura.Text = "No hay caja abierta para este empleado.";
                    lblMontoInicial.Text = "";

                    btnAbrir.Enabled = true;
                    btnAbrir.BackColor = Color.FromArgb(39, 174, 96);
                    btnMovimiento.Enabled = false;
                    btnCerrarCaja.Enabled = false;

                    lblVentasEfectivo.Text = "$0.00";
                    lblVentasTarjeta.Text = "$0.00";
                    lblIngresos.Text = "$0.00";
                    lblEgresos.Text = "$0.00";
                    lblEsperado.Text = "$0.00";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar estado de caja:\n" + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ============ CALCULAR RESUMEN ============
        private void CalcularResumen(SqlConnection con, int idCaja)
        {
            // 1) Monto inicial
            var cmdInicial = new SqlCommand("SELECT monto_inicial FROM Caja WHERE id_caja = @id", con);
            cmdInicial.Parameters.AddWithValue("@id", idCaja);
            decimal montoInicial = Convert.ToDecimal(cmdInicial.ExecuteScalar());

            // 2) Ventas en efectivo
            var cmdVentasEfvo = new SqlCommand(@"
                SELECT ISNULL(SUM(monto_total), 0) FROM Pedido 
                WHERE id_caja = @id AND estado = 'Pagado' 
                  AND (metodo_pago = 'Efectivo' OR metodo_pago IS NULL)", con);
            cmdVentasEfvo.Parameters.AddWithValue("@id", idCaja);
            decimal ventasEfectivo = Convert.ToDecimal(cmdVentasEfvo.ExecuteScalar());

            // 3) Ventas tarjeta/otros
            var cmdVentasTarj = new SqlCommand(@"
                SELECT ISNULL(SUM(monto_total), 0) FROM Pedido 
                WHERE id_caja = @id AND estado = 'Pagado' 
                  AND metodo_pago IN ('Tarjeta', 'Transferencia', 'Yape/Plin')", con);
            cmdVentasTarj.Parameters.AddWithValue("@id", idCaja);
            decimal ventasTarjeta = Convert.ToDecimal(cmdVentasTarj.ExecuteScalar());

            // 4) Ingresos extra
            var cmdIngresos = new SqlCommand(@"
                SELECT ISNULL(SUM(monto), 0) FROM MovimientoCaja 
                WHERE id_caja = @id AND tipo = 'Ingreso'", con);
            cmdIngresos.Parameters.AddWithValue("@id", idCaja);
            decimal ingresos = Convert.ToDecimal(cmdIngresos.ExecuteScalar());

            // 5) Egresos
            var cmdEgresos = new SqlCommand(@"
                SELECT ISNULL(SUM(monto), 0) FROM MovimientoCaja 
                WHERE id_caja = @id AND tipo = 'Egreso'", con);
            cmdEgresos.Parameters.AddWithValue("@id", idCaja);
            decimal egresos = Convert.ToDecimal(cmdEgresos.ExecuteScalar());

            // 6) Cálculo final
            decimal esperado = montoInicial + ventasEfectivo + ingresos - egresos;

            // Mostrar
            lblVentasEfectivo.Text = ventasEfectivo.ToString("C2");
            lblVentasTarjeta.Text = ventasTarjeta.ToString("C2");
            lblIngresos.Text = ingresos.ToString("C2");
            lblEgresos.Text = egresos.ToString("C2");
            lblEsperado.Text = esperado.ToString("C2");
        }

        // ============ HISTORIAL ============
        private void CargarHistorial()
        {
            try
            {
                using var con = new SqlConnection(connectionString);
                con.Open();

                var cmd = new SqlCommand(@"
                    SELECT c.id_caja, e.nombreEmpleado, 
                           c.fecha_apertura, c.fecha_cierre, 
                           c.monto_inicial, c.monto_final,
                           (c.monto_final - c.monto_inicial) AS diferencia
                    FROM Caja c
                    INNER JOIN Empleado e ON c.idEmpleado = e.id_empleado
                    WHERE c.estado = 'Cerrada'
                    ORDER BY c.fecha_cierre DESC", con);

                using var reader = cmd.ExecuteReader();
                dgvHistorial.Rows.Clear();

                while (reader.Read())
                {
                    dgvHistorial.Rows.Add(
                        reader["id_caja"],
                        reader["nombreEmpleado"].ToString(),
                        Convert.ToDateTime(reader["fecha_apertura"]).ToString("dd/MM/yyyy HH:mm"),
                        reader["fecha_cierre"] != DBNull.Value
                            ? Convert.ToDateTime(reader["fecha_cierre"]).ToString("dd/MM/yyyy HH:mm") : "-",
                        Convert.ToDecimal(reader["monto_inicial"]).ToString("N2"),
                        reader["monto_final"] != DBNull.Value
                            ? Convert.ToDecimal(reader["monto_final"]).ToString("N2") : "-",
                        reader["diferencia"] != DBNull.Value
                            ? Convert.ToDecimal(reader["diferencia"]).ToString("N2") : "-"
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar historial:\n" + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DgvHistorial_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dgvHistorial.Columns[e.ColumnIndex].Name == "Diferencia" && e.Value != null)
            {
                if (decimal.TryParse(e.Value.ToString(), out decimal diff))
                {
                    if (diff == 0) e.CellStyle.ForeColor = Color.FromArgb(39, 174, 96);
                    else if (diff > 0) e.CellStyle.ForeColor = Color.FromArgb(41, 128, 185);
                    else e.CellStyle.ForeColor = Color.FromArgb(192, 57, 43);
                }
            }
        }

        // ============ ABRIR CAJA ============
        private void BtnAbrir_Click(object sender, EventArgs e)
        {
            using var frm = new FrmAperturaCaja();
            if (frm.ShowDialog() == DialogResult.OK)
            {
                CargarEstadoCaja();
                CargarHistorial();
            }
        }

        // ============ MOVIMIENTO ============
        private void BtnMovimiento_Click(object sender, EventArgs e)
        {
            if (idCajaActual == null) return;

            using var frm = new FrmMovimientoCaja(idCajaActual.Value);
            if (frm.ShowDialog() == DialogResult.OK)
            {
                using var con = new SqlConnection(connectionString);
                con.Open();
                CalcularResumen(con, idCajaActual.Value);
            }
        }

        // ============ CERRAR CAJA ============
        private void BtnCerrarCaja_Click(object sender, EventArgs e)
        {
            if (idCajaActual == null) return;

            using var frm = new FrmCierreCaja(idCajaActual.Value);
            if (frm.ShowDialog() == DialogResult.OK)
            {
                CargarEstadoCaja();
                CargarHistorial();
            }
        }
    }
}