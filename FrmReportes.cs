using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using FontAwesome.Sharp;
// Nota:No use using ClosedXML.Excel; porque lo referencie como (ClosedXML.Excel.XLWorkbook).

namespace P1
{
    public partial class FrmReportes : Form
    {
        // Controles
        private Label lblTitulo;
        private DateTimePicker dtpInicio, dtpFin;
        private IconButton btnExportar;
        private IconButton btnGenerar, btnSalir;

        // Tarjetas de resumen
        private Label lblTotalVentas, lblIngresosTotales, lblGanancia, lblTicketPromedio;

        // DataGridViews
        private DataGridView dgvTopLibros, dgvVentasPorDia;

        private readonly string connectionString =
            @"Server=DESKTOP-IQ00U68;Database=Libreria;Integrated Security=True;TrustServerCertificate=True;";

        public FrmReportes()
        {
            ConstruirInterfazManual();
            this.Load += (s, e) =>
            {
                // Por defecto: últimos 30 días
                dtpFin.Value = DateTime.Today;
                dtpInicio.Value = DateTime.Today.AddDays(-30);
                GenerarReporte();
            };
        }

        private void ConstruirInterfazManual()
        {
            this.Text = "Reportes - Librería Morita";
            this.Size = new Size(1250, 780);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.WhiteSmoke;
            this.MinimumSize = new Size(1150, 700);

            // ============ HEADER ============
            Panel panelHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = Color.FromArgb(155, 89, 182)  // Morado
            };

            lblTitulo = new Label
            {
                Text = "📊 Reportes de Ventas",
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

            // ============ FILTRO DE FECHAS ============
            Panel panelFiltro = new Panel
            {
                Location = new Point(20, 80),
                Size = new Size(1200, 70),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            // Icono calendario
            var picCalendario = new IconPictureBox
            {
                IconChar = IconChar.CalendarAlt,
                IconColor = Color.FromArgb(155, 89, 182),
                IconSize = 28,
                Location = new Point(15, 20),
                Size = new Size(28, 28),
                BackColor = Color.Transparent
            };
            panelFiltro.Controls.Add(picCalendario);

            // Fecha Inicio
            panelFiltro.Controls.Add(new Label
            {
                Text = "Desde:",
                Location = new Point(60, 12),
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            });

            dtpInicio = new DateTimePicker
            {
                Location = new Point(60, 32),
                Size = new Size(160, 28),
                Font = new Font("Segoe UI", 10),
                Format = DateTimePickerFormat.Short
            };
            panelFiltro.Controls.Add(dtpInicio);

            // Fecha Fin
            panelFiltro.Controls.Add(new Label
            {
                Text = "Hasta:",
                Location = new Point(240, 12),
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            });

            dtpFin = new DateTimePicker
            {
                Location = new Point(240, 32),
                Size = new Size(160, 28),
                Font = new Font("Segoe UI", 10),
                Format = DateTimePickerFormat.Short
            };
            panelFiltro.Controls.Add(dtpFin);

            // Botón Generar
            btnGenerar = new IconButton
            {
                Text = "Generar Reporte",
                IconChar = IconChar.ChartLine,
                IconColor = Color.White,
                IconSize = 18,
                Location = new Point(430, 20),
                Size = new Size(180, 40),
                BackColor = Color.FromArgb(155, 89, 182),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                ImageAlign = ContentAlignment.MiddleLeft,
                TextAlign = ContentAlignment.MiddleRight,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnGenerar.FlatAppearance.BorderSize = 0;
            btnGenerar.Click += BtnGenerar_Click;
            panelFiltro.Controls.Add(btnGenerar);

            // Botones rápidos
            var btnHoy = CrearBotonRapido("Hoy", 620, 20); // ("Hoy", izq/dere, arriba/abajo) mas alto mas der o mas arriba
            btnHoy.Click += (s, e) =>
            {
                dtpInicio.Value = DateTime.Today;
                dtpFin.Value = DateTime.Today;
                GenerarReporte();
            };
            panelFiltro.Controls.Add(btnHoy);

            var btnSemana = CrearBotonRapido("Esta semana", 735, 20);
            btnSemana.Click += (s, e) =>
            {
                dtpInicio.Value = DateTime.Today.AddDays(-7);
                dtpFin.Value = DateTime.Today;
                GenerarReporte();
            };
            panelFiltro.Controls.Add(btnSemana);

            var btnMes = CrearBotonRapido("Este mes", 850, 20);
            btnMes.Click += (s, e) =>
            {
                dtpInicio.Value = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                dtpFin.Value = DateTime.Today;
                GenerarReporte();
            };
            panelFiltro.Controls.Add(btnMes);

            btnExportar = new IconButton
            {
                Text = "Exportar a Excel",
                IconChar = IconChar.FileExcel,
                IconColor = Color.White,
                IconSize = 18,
                Location = new Point(970, 20),
                Size = new Size(210, 40),
                BackColor = Color.FromArgb(39, 174, 96),   // Verde
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                ImageAlign = ContentAlignment.MiddleLeft,
                TextAlign = ContentAlignment.MiddleRight,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnExportar.FlatAppearance.BorderSize = 0;
            btnExportar.Click += BtnExportar_Click;
            panelFiltro.Controls.Add(btnExportar);

            this.Controls.Add(panelFiltro);

            // ============ TARJETAS DE RESUMEN ============
            int yTarjetas = 165;
            int anchoTarjeta = 285; //290
            int espacio = 20;

            // Tarjeta 1
            Panel tarjeta1 = CrearTarjeta("N° de Ventas", "0", Color.FromArgb(41, 128, 185), IconChar.ShoppingCart, 20, yTarjetas, anchoTarjeta, out lblTotalVentas);
            this.Controls.Add(tarjeta1);

            // Tarjeta 2
            Panel tarjeta2 = CrearTarjeta("Ingresos Totales", "$0.00", Color.FromArgb(39, 174, 96), IconChar.MoneyBillWave, 20 + anchoTarjeta + espacio, yTarjetas, anchoTarjeta, out lblIngresosTotales);
            this.Controls.Add(tarjeta2);

            // Tarjeta 3
            Panel tarjeta3 = CrearTarjeta("Ganancia Estimada", "$0.00", Color.FromArgb(230, 126, 34), IconChar.ArrowUp, 20 + (anchoTarjeta + espacio) * 2, yTarjetas, anchoTarjeta, out lblGanancia);
            this.Controls.Add(tarjeta3);

            // Tarjeta 4
            Panel tarjeta4 = CrearTarjeta("Ticket Promedio", "$0.00", Color.FromArgb(155, 89, 182), IconChar.Receipt, 20 + (anchoTarjeta + espacio) * 3, yTarjetas, anchoTarjeta, out lblTicketPromedio);
            this.Controls.Add(tarjeta4);

            // ============ TOP LIBROS ============
            Label lblTopTitulo = new Label
            {
                Text = "🏆 Top 10 Libros Más Vendidos",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(155, 89, 182),
                Location = new Point(20, 280),
                AutoSize = true
            };
            this.Controls.Add(lblTopTitulo);

            dgvTopLibros = new DataGridView
            {
                Location = new Point(20, 310),
                Size = new Size(590, 400),
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
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom
            };
            dgvTopLibros.Columns.Add("Pos", "#");
            dgvTopLibros.Columns.Add("Titulo", "Libro");
            dgvTopLibros.Columns.Add("Vendidos", "Vendidos");
            dgvTopLibros.Columns.Add("Ingresos", "Ingresos");
            dgvTopLibros.Columns["Pos"].Width = 40;
            dgvTopLibros.Columns["Pos"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvTopLibros.Columns["Vendidos"].Width = 80;
            dgvTopLibros.Columns["Vendidos"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvTopLibros.Columns["Ingresos"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            this.Controls.Add(dgvTopLibros);

            // ============ VENTAS POR ORDEN ELEGIDO (DÍA, SEMANA Y MES) ============
            Label lblDiaTitulo = new Label
            {
                Text = "📅 Ventas ordenadas",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(41, 128, 185),
                Location = new Point(630, 280), 
                AutoSize = true,
                Anchor = AnchorStyles.Top | AnchorStyles.Right // calculará su distancia desde el borde derecho de la ventana
            };
            this.Controls.Add(lblDiaTitulo);

            dgvVentasPorDia = new DataGridView
            {
                Location = new Point(630, 310),
                Size = new Size(590, 400),
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
                Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom
            };
            dgvVentasPorDia.Columns.Add("Fecha", "Fecha");
            dgvVentasPorDia.Columns.Add("NumVentas", "# Ventas");
            dgvVentasPorDia.Columns.Add("Total", "Total");
            dgvVentasPorDia.Columns["NumVentas"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvVentasPorDia.Columns["Total"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            this.Controls.Add(dgvVentasPorDia);
        }

        // ============ TARJETA DE RESUMEN ============
        private Panel CrearTarjeta(string titulo, string valorInicial, Color color, IconChar icono, int x, int y, int ancho, out Label lblValor)
        {
            Panel tarjeta = new Panel
            {
                Location = new Point(x, y),
                Size = new Size(ancho, 100),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            // Barra de color
            Panel barraColor = new Panel { Dock = DockStyle.Left, Width = 8, BackColor = color };
            tarjeta.Controls.Add(barraColor);

            // Icono
            var picIcono = new IconPictureBox
            {
                IconChar = icono,
                IconColor = color,
                IconSize = 36,
                Location = new Point(ancho - 55, 30),
                Size = new Size(36, 36),
                BackColor = Color.Transparent
            };
            tarjeta.Controls.Add(picIcono);

            // Título
            var lblTitulo = new Label
            {
                Text = titulo,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.Gray,
                Location = new Point(20, 15),
                AutoSize = true
            };
            tarjeta.Controls.Add(lblTitulo);

            // Valor (Usamos el parámetro 'out' aquí en lugar de una variable local)
            lblValor = new Label
            {
                Text = valorInicial,
                Name = "lblValor",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = color,
                Location = new Point(18, 40),
                AutoSize = true
            };
            tarjeta.Controls.Add(lblValor);

            return tarjeta;
        }

        // ============ BOTÓN RÁPIDO ============
        private IconButton CrearBotonRapido(string texto, int x, int y)
        {
            var btn = new IconButton
            {
                Text = texto,
                IconChar = IconChar.Clock,
                IconColor = Color.White,
                IconSize = 14,
                Location = new Point(x, y),
                Size = new Size(100, 40),
                BackColor = Color.FromArgb(52, 73, 94),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                ImageAlign = ContentAlignment.MiddleLeft,
                TextAlign = ContentAlignment.MiddleRight,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            return btn;
        }

        // ============ GENERAR REPORTE ============
        private void BtnGenerar_Click(object sender, EventArgs e)
        {
            GenerarReporte();
        }

        private void GenerarReporte()
        {
            if (dtpInicio.Value > dtpFin.Value)
            {
                MessageBox.Show("La fecha de inicio no puede ser mayor que la de fin.",
                    "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using var con = new SqlConnection(connectionString);
                con.Open();

                DateTime inicio = dtpInicio.Value.Date;
                DateTime fin = dtpFin.Value.Date.AddDays(1);   // incluir todo el día fin

                // ============ RESUMEN ============
                CargarResumen(con, inicio, fin);

                // ============ TOP LIBROS ============
                CargarTopLibros(con, inicio, fin);

                // ============ VENTAS POR DÍA ============
                CargarVentasPorDia(con, inicio, fin);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al generar el reporte:\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ============ EXPORTAR A EXCEL ============
        private void BtnExportar_Click(object sender, EventArgs e)
        {
            if (dtpInicio.Value > dtpFin.Value)
            {
                MessageBox.Show("La fecha de inicio no puede ser mayor que la de fin.",
                    "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Preguntar dónde guardar
            using var sfd = new SaveFileDialog
            {
                Filter = "Archivo Excel (*.xlsx)|*.xlsx",
                FileName = $"Reporte_Ventas_{dtpInicio.Value:yyyyMMdd}_{dtpFin.Value:yyyyMMdd}.xlsx",
                Title = "Guardar reporte como..."
            };

            if (sfd.ShowDialog() != DialogResult.OK) return;

            try
            {
                ExportarExcel(sfd.FileName);

                var confirm = MessageBox.Show(
                    $"✅ Reporte exportado exitosamente a:\n\n{sfd.FileName}\n\n¿Deseas abrir el archivo ahora?",
                    "Exportación Exitosa",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Information);

                if (confirm == DialogResult.Yes)
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = sfd.FileName,
                        UseShellExecute = true
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al exportar a Excel:\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ============ GENERAR EXCEL ============
        private void ExportarExcel(string ruta)
        {
            using var con = new SqlConnection(connectionString);
            con.Open();

            DateTime inicio = dtpInicio.Value.Date;
            DateTime fin = dtpFin.Value.Date.AddDays(1);

            // Usar ClosedXML
            using var workbook = new ClosedXML.Excel.XLWorkbook();
            var ws = workbook.Worksheets.Add("Reporte de Ventas");

            // ============ ESTILOS DE COLOR ============
            var colorMorado = ClosedXML.Excel.XLColor.FromHtml("#9B59B6");
            var colorAzul = ClosedXML.Excel.XLColor.FromHtml("#2980B9");
            var colorVerde = ClosedXML.Excel.XLColor.FromHtml("#27AE60");
            var colorNaranja = ClosedXML.Excel.XLColor.FromHtml("#E67E22");
            var colorGris = ClosedXML.Excel.XLColor.FromHtml("#ECF0F1");

            // ============ TÍTULO ============
            ws.Cell("A1").Value = "LIBRERÍA MORITA";
            ws.Range("A1:F1").Merge();
            ws.Cell("A1").Style.Font.FontSize = 20;
            ws.Cell("A1").Style.Font.Bold = true;
            ws.Cell("A1").Style.Font.FontColor = ClosedXML.Excel.XLColor.White;
            ws.Cell("A1").Style.Fill.BackgroundColor = colorMorado;
            ws.Cell("A1").Style.Alignment.Horizontal = ClosedXML.Excel.XLAlignmentHorizontalValues.Center;
            ws.Cell("A1").Style.Alignment.Vertical = ClosedXML.Excel.XLAlignmentVerticalValues.Center;
            ws.Row(1).Height = 35;

            ws.Cell("A2").Value = "REPORTE DE VENTAS";
            ws.Range("A2:F2").Merge();
            ws.Cell("A2").Style.Font.FontSize = 14;
            ws.Cell("A2").Style.Font.Bold = true;
            ws.Cell("A2").Style.Font.FontColor = colorMorado;
            ws.Cell("A2").Style.Alignment.Horizontal = ClosedXML.Excel.XLAlignmentHorizontalValues.Center;
            ws.Row(2).Height = 25;

            ws.Cell("A3").Value = $"Período: {inicio:dd/MM/yyyy} al {dtpFin.Value:dd/MM/yyyy}";
            ws.Range("A3:F3").Merge();
            ws.Cell("A3").Style.Font.FontSize = 11;
            ws.Cell("A3").Style.Font.Italic = true;
            ws.Cell("A3").Style.Alignment.Horizontal = ClosedXML.Excel.XLAlignmentHorizontalValues.Center;

            ws.Cell("A4").Value = $"Generado: {DateTime.Now:dd/MM/yyyy HH:mm}";
            ws.Range("A4:F4").Merge();
            ws.Cell("A4").Style.Font.FontSize = 9;
            ws.Cell("A4").Style.Font.Italic = true;
            ws.Cell("A4").Style.Font.FontColor = ClosedXML.Excel.XLColor.Gray;
            ws.Cell("A4").Style.Alignment.Horizontal = ClosedXML.Excel.XLAlignmentHorizontalValues.Center;

            // ============ RESUMEN (KPIs) ============
            ws.Cell("A6").Value = "RESUMEN GENERAL";
            ws.Range("A6:F6").Merge();
            ws.Cell("A6").Style.Font.FontSize = 12;
            ws.Cell("A6").Style.Font.Bold = true;
            ws.Cell("A6").Style.Fill.BackgroundColor = colorGris;

            ws.Cell("A7").Value = "Métrica";
            ws.Cell("B7").Value = "Valor";
            ws.Range("A7:B7").Style.Font.Bold = true;
            ws.Range("A7:B7").Style.Fill.BackgroundColor = colorAzul;
            ws.Range("A7:B7").Style.Font.FontColor = ClosedXML.Excel.XLColor.White;

            ws.Cell("A8").Value = "N° de Ventas";
            ws.Cell("B8").Value = lblTotalVentas.Text;

            ws.Cell("A9").Value = "Ingresos Totales";
            ws.Cell("B9").Value = lblIngresosTotales.Text;

            ws.Cell("A10").Value = "Ganancia Estimada";
            ws.Cell("B10").Value = lblGanancia.Text;

            ws.Cell("A11").Value = "Ticket Promedio";
            ws.Cell("B11").Value = lblTicketPromedio.Text;

            ws.Range("A8:A11").Style.Font.Bold = true;
            ws.Range("B8:B11").Style.Alignment.Horizontal = ClosedXML.Excel.XLAlignmentHorizontalValues.Right;

            // ============ TOP LIBROS ============
            int filaTop = 13;
            ws.Cell($"A{filaTop}").Value = "TOP 10 LIBROS MÁS VENDIDOS";
            ws.Range($"A{filaTop}:F{filaTop}").Merge();
            ws.Cell($"A{filaTop}").Style.Font.FontSize = 12;
            ws.Cell($"A{filaTop}").Style.Font.Bold = true;
            ws.Cell($"A{filaTop}").Style.Fill.BackgroundColor = colorGris;

            filaTop++;
            ws.Cell($"A{filaTop}").Value = "#";
            ws.Cell($"B{filaTop}").Value = "Libro";
            ws.Cell($"C{filaTop}").Value = "Vendidos";
            ws.Cell($"D{filaTop}").Value = "Ingresos";
            ws.Range($"A{filaTop}:D{filaTop}").Style.Font.Bold = true;
            ws.Range($"A{filaTop}:D{filaTop}").Style.Fill.BackgroundColor = colorVerde;
            ws.Range($"A{filaTop}:D{filaTop}").Style.Font.FontColor = ClosedXML.Excel.XLColor.White;

            // Llenar TOP libros desde el query
            var cmdTop = new SqlCommand(@"
        SELECT TOP 10
            l.titulo,
            SUM(dp.cantidad) AS total_vendidos,
            SUM(dp.cantidad * dp.precio_unitario - dp.descuento_aplicado) AS ingresos
        FROM DetallePedido dp
        INNER JOIN Pedido p ON dp.idPedido = p.id_pedido
        INNER JOIN Libro l ON dp.idLibro = l.id_libro
        WHERE p.estado = 'Pagado'
          AND p.fecha_pedido >= @inicio
          AND p.fecha_pedido < @fin
        GROUP BY l.titulo
        ORDER BY total_vendidos DESC", con);
            cmdTop.Parameters.AddWithValue("@inicio", inicio);
            cmdTop.Parameters.AddWithValue("@fin", fin);

            using (var reader = cmdTop.ExecuteReader())
            {
                int pos = 1;
                filaTop++;
                while (reader.Read())
                {
                    ws.Cell($"A{filaTop}").Value = pos;
                    ws.Cell($"B{filaTop}").Value = reader["titulo"].ToString();
                    ws.Cell($"C{filaTop}").Value = Convert.ToInt32(reader["total_vendidos"]);
                    ws.Cell($"D{filaTop}").Value = Convert.ToDecimal(reader["ingresos"]);
                    ws.Cell($"D{filaTop}").Style.NumberFormat.Format = "$ #,##0.00";
                    ws.Cell($"C{filaTop}").Style.Alignment.Horizontal = ClosedXML.Excel.XLAlignmentHorizontalValues.Center;
                    ws.Cell($"A{filaTop}").Style.Alignment.Horizontal = ClosedXML.Excel.XLAlignmentHorizontalValues.Center;
                    filaTop++;
                    pos++;
                }

                if (pos == 1)
                {
                    ws.Cell($"B{filaTop}").Value = "Sin datos en este período";
                    ws.Range($"A{filaTop}:D{filaTop}").Merge();
                    filaTop++;
                }
            }

            // ============ VENTAS POR DÍA ============
            int filaDia = filaTop + 2;
            ws.Cell($"A{filaDia}").Value = "VENTAS POR DÍA";
            ws.Range($"A{filaDia}:F{filaDia}").Merge();
            ws.Cell($"A{filaDia}").Style.Font.FontSize = 12;
            ws.Cell($"A{filaDia}").Style.Font.Bold = true;
            ws.Cell($"A{filaDia}").Style.Fill.BackgroundColor = colorGris;

            filaDia++;
            ws.Cell($"A{filaDia}").Value = "Fecha";
            ws.Cell($"B{filaDia}").Value = "N° Ventas";
            ws.Cell($"C{filaDia}").Value = "Total";
            ws.Range($"A{filaDia}:C{filaDia}").Style.Font.Bold = true;
            ws.Range($"A{filaDia}:C{filaDia}").Style.Fill.BackgroundColor = colorNaranja;
            ws.Range($"A{filaDia}:C{filaDia}").Style.Font.FontColor = ClosedXML.Excel.XLColor.White;

            var cmdDia = new SqlCommand(@"
        SELECT 
            CAST(fecha_pedido AS DATE) AS dia,
            COUNT(*) AS num_ventas,
            SUM(monto_total) AS total
        FROM Pedido
        WHERE estado = 'Pagado'
          AND fecha_pedido >= @inicio
          AND fecha_pedido < @fin
        GROUP BY CAST(fecha_pedido AS DATE)
        ORDER BY dia DESC", con);
            cmdDia.Parameters.AddWithValue("@inicio", inicio);
            cmdDia.Parameters.AddWithValue("@fin", fin);

            using (var reader = cmdDia.ExecuteReader())
            {
                filaDia++;
                int filas = 0;
                while (reader.Read())
                {
                    DateTime dia = Convert.ToDateTime(reader["dia"]);
                    ws.Cell($"A{filaDia}").Value = dia.ToString("dd/MM/yyyy");
                    ws.Cell($"B{filaDia}").Value = Convert.ToInt32(reader["num_ventas"]);
                    ws.Cell($"C{filaDia}").Value = Convert.ToDecimal(reader["total"]);
                    ws.Cell($"C{filaDia}").Style.NumberFormat.Format = "$ #,##0.00";
                    ws.Cell($"B{filaDia}").Style.Alignment.Horizontal = ClosedXML.Excel.XLAlignmentHorizontalValues.Center;
                    filaDia++;
                    filas++;
                }

                if (filas == 0)
                {
                    ws.Cell($"A{filaDia}").Value = "Sin datos en este período";
                    ws.Range($"A{filaDia}:C{filaDia}").Merge();
                }
            }

            // ============ AJUSTES FINALES ============
            ws.Columns("A").Width = 8;
            ws.Columns("B").Width = 35;
            ws.Columns("C").Width = 15;
            ws.Columns("D").Width = 18;
            ws.Columns("E").Width = 15;
            ws.Columns("F").Width = 15;

            // Aplicar bordes a todas las celdas con datos
            var lastRow = ws.LastRowUsed().RowNumber();
            var tableRange = ws.Range($"A7:D{lastRow}");
            tableRange.Style.Border.OutsideBorder = ClosedXML.Excel.XLBorderStyleValues.Thin;
            tableRange.Style.Border.InsideBorder = ClosedXML.Excel.XLBorderStyleValues.Thin;

            // Guardar
            workbook.SaveAs(ruta);
        }

        private void CargarResumen(SqlConnection con, DateTime inicio, DateTime fin)
        {
            // Total de ventas + Ingresos + Ticket promedio
            var cmd = new SqlCommand(@"
        SELECT 
            COUNT(*) AS total_ventas,
            ISNULL(SUM(monto_total), 0) AS ingresos,
            ISNULL(AVG(monto_total), 0) AS promedio
        FROM Pedido
        WHERE estado = 'Pagado'
          AND fecha_pedido >= @inicio
          AND fecha_pedido < @fin", con);
            cmd.Parameters.AddWithValue("@inicio", inicio);
            cmd.Parameters.AddWithValue("@fin", fin);

            using (var reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    int totalVentas = Convert.ToInt32(reader["total_ventas"]);
                    decimal ingresos = Convert.ToDecimal(reader["ingresos"]);
                    decimal promedio = Convert.ToDecimal(reader["promedio"]);

                    lblTotalVentas.Text = totalVentas.ToString("N0");
                    lblIngresosTotales.Text = ingresos.ToString("C2");
                    lblTicketPromedio.Text = promedio.ToString("C2");
                }
            } // El reader se cierra automáticamente aquí

            // Ganancia: ingresos - costo de los libros vendidos
            var cmdGanancia = new SqlCommand(@"
        SELECT ISNULL(SUM(
                 (dp.precio_unitario - ISNULL(l.precio_compra, 0)) * dp.cantidad 
                 - ISNULL(dp.descuento_aplicado, 0)
               ), 0) AS ganancia
        FROM DetallePedido dp
        INNER JOIN Pedido p ON dp.idPedido = p.id_pedido
        INNER JOIN Libro l ON dp.idLibro = l.id_libro
        WHERE p.estado = 'Pagado'
          AND p.fecha_pedido >= @inicio
          AND p.fecha_pedido < @fin", con);

            cmdGanancia.Parameters.AddWithValue("@inicio", inicio);
            cmdGanancia.Parameters.AddWithValue("@fin", fin);

            // Resultado
            decimal ganancia = Convert.ToDecimal(cmdGanancia.ExecuteScalar());
            lblGanancia.Text = ganancia.ToString("C2");
        }

        private void CargarTopLibros(SqlConnection con, DateTime inicio, DateTime fin)
        {
          var cmd = new SqlCommand(@"
          SELECT TOP 10
        l.titulo,
        SUM(dp.cantidad) AS total_vendidos,
        SUM(dp.cantidad * dp.precio_unitario - ISNULL(dp.descuento_aplicado, 0)) AS ingresos
        FROM DetallePedido dp
        INNER JOIN Pedido p ON dp.idPedido = p.id_pedido
        INNER JOIN Libro l ON dp.idLibro = l.id_libro
        WHERE p.estado = 'Pagado'
        AND p.fecha_pedido >= @inicio
        AND p.fecha_pedido < @fin
        GROUP BY l.titulo
        ORDER BY total_vendidos DESC", con);

            cmd.Parameters.AddWithValue("@inicio", inicio);
            cmd.Parameters.AddWithValue("@fin", fin);

            using var reader = cmd.ExecuteReader();
            dgvTopLibros.Rows.Clear();
            int pos = 1;

            while (reader.Read())
            {
                dgvTopLibros.Rows.Add(
                    pos.ToString(),
                    reader["titulo"].ToString(),
                    reader["total_vendidos"].ToString(),
                    Convert.ToDecimal(reader["ingresos"]).ToString("C2")
                );
                pos++;
            }

            if (dgvTopLibros.Rows.Count == 0)
            {
                dgvTopLibros.Rows.Add("-", "No hay ventas en este período", "-", "-");
            }
        }

        private void CargarVentasPorDia(SqlConnection con, DateTime inicio, DateTime fin)
        {
            var cmd = new SqlCommand(@"
                SELECT 
                    CAST(fecha_pedido AS DATE) AS dia,
                    COUNT(*) AS num_ventas,
                    SUM(monto_total) AS total
                FROM Pedido
                WHERE estado = 'Pagado'
                  AND fecha_pedido >= @inicio
                  AND fecha_pedido < @fin
                GROUP BY CAST(fecha_pedido AS DATE)
                ORDER BY dia DESC", con);
            cmd.Parameters.AddWithValue("@inicio", inicio);
            cmd.Parameters.AddWithValue("@fin", fin);

            using var reader = cmd.ExecuteReader();
            dgvVentasPorDia.Rows.Clear();

            while (reader.Read())
            {
                DateTime dia = Convert.ToDateTime(reader["dia"]);
                dgvVentasPorDia.Rows.Add(
                    dia.ToString("dd/MM/yyyy"),
                    reader["num_ventas"].ToString(),
                    Convert.ToDecimal(reader["total"]).ToString("C2")
                );
            }

            if (dgvVentasPorDia.Rows.Count == 0)
            {
                dgvVentasPorDia.Rows.Add("Sin datos", "-", "-");
            }
        }
    }
}