using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using FontAwesome.Sharp;

namespace P1
{
    public partial class FrmCompras : Form
    {
        // Controles
        private Label lblTitulo, lblProveedor, lblTipoDoc, lblNumDoc, lblTotal, lblInfo;
        private ComboBox cmbProveedor;
        private TextBox txtTipoDoc, txtNumDoc;
        private ComboBox cmbLibro;
        private NumericUpDown numCantidad, numPrecioCompra, numPrecioVenta;
        private IconButton btnAgregar, btnQuitar, btnGuardar, btnCancelar;
        private DataGridView dgvDetalle;

        // Datos
        private List<DetalleCompraItem> detalle = new List<DetalleCompraItem>();
        private readonly string connectionString =
            @"Server=DESKTOP-IQ00U68;Database=Libreria;Integrated Security=True;TrustServerCertificate=True;";

        public FrmCompras()
        {
            ConstruirInterfazManual();
            this.Load += (s, e) =>
            {
                CargarProveedores();
                CargarLibros();
            };
        }

        private void ConstruirInterfazManual()
        {
            // ============ FORMULARIO ============
            this.Text = "Registrar Compra - Librería Morita";
            this.Size = new Size(1000, 680);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.WhiteSmoke;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            // ============ TÍTULO ============
            lblTitulo = new Label
            {
                Text = "📦 Registrar Nueva Compra",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(41, 128, 185),
                AutoSize = true,
                Location = new Point(20, 15)
            };
            this.Controls.Add(lblTitulo);

            // ============ PROVEEDOR ============
            lblProveedor = new Label
            {
                Text = "Proveedor:",
                Location = new Point(20, 65),
                AutoSize = true,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            this.Controls.Add(lblProveedor);

            cmbProveedor = new ComboBox
            {
                Location = new Point(20, 88),
                Size = new Size(300, 28),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10)
            };
            this.Controls.Add(cmbProveedor);

            // ============ TIPO DOCUMENTO ============
            lblTipoDoc = new Label
            {
                Text = "Tipo Documento:",
                Location = new Point(340, 65),
                AutoSize = true,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            this.Controls.Add(lblTipoDoc);

            txtTipoDoc = new TextBox
            {
                Location = new Point(340, 88),
                Size = new Size(160, 28),
                Text = "Factura",
                Font = new Font("Segoe UI", 10)
            };
            this.Controls.Add(txtTipoDoc);

            // ============ NÚMERO DOCUMENTO ============
            lblNumDoc = new Label
            {
                Text = "N° Documento:",
                Location = new Point(520, 65),
                AutoSize = true,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            this.Controls.Add(lblNumDoc);

            txtNumDoc = new TextBox
            {
                Location = new Point(520, 88),
                Size = new Size(200, 28),
                PlaceholderText = "Ej: F001-000123",
                Font = new Font("Segoe UI", 10)
            };
            this.Controls.Add(txtNumDoc);

            // ============ PANEL AGREGAR LIBRO ============
            Panel panelAgregar = new Panel
            {
                Location = new Point(20, 135),
                Size = new Size(940, 110),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            Label lblAgregarTitulo = new Label
            {
                Text = "➕ Agregar Libro al Detalle",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(41, 128, 185),
                Location = new Point(10, 8),
                AutoSize = true
            };
            panelAgregar.Controls.Add(lblAgregarTitulo);

            // Libro
            panelAgregar.Controls.Add(new Label
            {
                Text = "Libro:",
                Location = new Point(10, 40),
                AutoSize = true,
                Font = new Font("Segoe UI", 10)
            });

            cmbLibro = new ComboBox
            {
                Location = new Point(10, 62),
                Size = new Size(340, 28),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10)
            };
            panelAgregar.Controls.Add(cmbLibro);

            // Cantidad
            panelAgregar.Controls.Add(new Label
            {
                Text = "Cant.:",
                Location = new Point(365, 40),
                AutoSize = true,
                Font = new Font("Segoe UI", 10)
            });

            numCantidad = new NumericUpDown
            {
                Location = new Point(365, 62),
                Size = new Size(70, 28),
                Minimum = 1,
                Maximum = 9999,
                Value = 1,
                Font = new Font("Segoe UI", 10)
            };
            panelAgregar.Controls.Add(numCantidad);

            // Precio Compra
            panelAgregar.Controls.Add(new Label
            {
                Text = "P. Compra:",
                Location = new Point(450, 40),
                AutoSize = true,
                Font = new Font("Segoe UI", 10)
            });

            numPrecioCompra = new NumericUpDown
            {
                Location = new Point(450, 62),
                Size = new Size(110, 28),
                DecimalPlaces = 2,
                Maximum = 999999,
                Font = new Font("Segoe UI", 10)
            };
            panelAgregar.Controls.Add(numPrecioCompra);

            // Precio Venta
            panelAgregar.Controls.Add(new Label
            {
                Text = "P. Venta:",
                Location = new Point(575, 40),
                AutoSize = true,
                Font = new Font("Segoe UI", 10)
            });

            numPrecioVenta = new NumericUpDown
            {
                Location = new Point(575, 62),
                Size = new Size(110, 28),
                DecimalPlaces = 2,
                Maximum = 999999,
                Font = new Font("Segoe UI", 10)
            };
            panelAgregar.Controls.Add(numPrecioVenta);

            // Botones
            btnAgregar = new IconButton
            {
                Text = "Agregar",
                IconChar = IconChar.Plus,
                IconColor = Color.White,
                IconSize = 16,
                Location = new Point(700, 55),
                Size = new Size(115, 40),
                BackColor = Color.FromArgb(39, 174, 96),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                ImageAlign = ContentAlignment.MiddleLeft,
                TextAlign = ContentAlignment.MiddleRight,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            btnAgregar.FlatAppearance.BorderSize = 0;
            btnAgregar.Click += BtnAgregar_Click;
            panelAgregar.Controls.Add(btnAgregar);

            btnQuitar = new IconButton
            {
                Text = "Quitar",
                IconChar = IconChar.TrashAlt,
                IconColor = Color.White,
                IconSize = 16,
                Location = new Point(825, 55),
                Size = new Size(105, 40),
                BackColor = Color.FromArgb(192, 57, 43),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                ImageAlign = ContentAlignment.MiddleLeft,
                TextAlign = ContentAlignment.MiddleRight,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            btnQuitar.FlatAppearance.BorderSize = 0;
            btnQuitar.Click += BtnQuitar_Click;
            panelAgregar.Controls.Add(btnQuitar);

            this.Controls.Add(panelAgregar);

            // ============ DATAGRIDVIEW ============
            dgvDetalle = new DataGridView
            {
                Location = new Point(20, 260),
                Size = new Size(940, 300),
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
                ColumnHeadersHeight = 32
            };
            dgvDetalle.Columns.Add("IdLibro", "ID");
            dgvDetalle.Columns.Add("Titulo", "Título");
            dgvDetalle.Columns.Add("Cantidad", "Cant.");
            dgvDetalle.Columns.Add("PrecioCompra", "P. Compra");
            dgvDetalle.Columns.Add("PrecioVenta", "P. Venta");
            dgvDetalle.Columns.Add("Subtotal", "Subtotal");
            dgvDetalle.Columns["IdLibro"].Visible = false;
            dgvDetalle.Columns["Cantidad"].Width = 60;
            dgvDetalle.Columns["PrecioCompra"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvDetalle.Columns["PrecioVenta"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvDetalle.Columns["Subtotal"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            this.Controls.Add(dgvDetalle);

            // ============ TOTAL ============
            lblTotal = new Label
            {
                Text = "Total: $0.00",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(192, 57, 43),
                AutoSize = true,
                Location = new Point(20, 575)
            };
            this.Controls.Add(lblTotal);

            lblInfo = new Label
            {
                Text = "Los precios de venta se actualizarán automáticamente en el catálogo.",
                Font = new Font("Segoe UI", 9, FontStyle.Italic),
                ForeColor = Color.Gray,
                AutoSize = true,
                Location = new Point(20, 610)
            };
            this.Controls.Add(lblInfo);

            // ============ BOTONES ============
            btnGuardar = new IconButton
            {
                Text = "Guardar Compra",
                IconChar = IconChar.Save,
                IconColor = Color.White,
                IconSize = 20,
                Location = new Point(640, 575),
                Size = new Size(200, 45),
                BackColor = Color.FromArgb(41, 128, 185),
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
                Location = new Point(850, 575),
                Size = new Size(110, 45),
                BackColor = Color.Gray,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                ImageAlign = ContentAlignment.MiddleLeft,
                TextAlign = ContentAlignment.MiddleRight,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            btnCancelar.FlatAppearance.BorderSize = 0;
            btnCancelar.Click += (s, e) => this.Close();
            this.Controls.Add(btnCancelar);
        }

        // ============ CARGAR DATOS ============
        private void CargarProveedores()
        {
            try
            {
                using var con = new SqlConnection(connectionString);
                con.Open();
                var cmd = new SqlCommand(
                    "SELECT id_proveedor, nombreProveedor FROM Proveedor WHERE estado = 1 ORDER BY nombreProveedor",
                    con);
                var dt = new System.Data.DataTable();
                new SqlDataAdapter(cmd).Fill(dt);
                cmbProveedor.DataSource = dt;
                cmbProveedor.DisplayMember = "nombreProveedor";
                cmbProveedor.ValueMember = "id_proveedor";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar proveedores: " + ex.Message);
            }
        }

        private void CargarLibros()
        {
            try
            {
                using var con = new SqlConnection(connectionString);
                con.Open();
                var cmd = new SqlCommand(
                    "SELECT id_libro, titulo, precio_venta FROM Libro WHERE estado = 1 ORDER BY titulo",
                    con);
                var dt = new System.Data.DataTable();
                new SqlDataAdapter(cmd).Fill(dt);
                cmbLibro.DataSource = dt;
                cmbLibro.DisplayMember = "titulo";
                cmbLibro.ValueMember = "id_libro";

                // Cuando se selecciona un libro, autocompletar precio venta
                cmbLibro.SelectedIndexChanged += (s, e) =>
                {
                    if (cmbLibro.SelectedItem is System.Data.DataRowView row)
                    {
                        numPrecioVenta.Value = Convert.ToDecimal(row["precio_venta"]);
                    }
                };
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar libros: " + ex.Message);
            }
        }

        // ============ AGREGAR / QUITAR ============
        private void BtnAgregar_Click(object sender, EventArgs e)
        {
            if (cmbLibro.SelectedValue == null)
            {
                MessageBox.Show("Selecciona un libro.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (numPrecioCompra.Value <= 0)
            {
                MessageBox.Show("El precio de compra debe ser mayor a 0.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (numPrecioVenta.Value <= 0)
            {
                MessageBox.Show("El precio de venta debe ser mayor a 0.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int idLibro = Convert.ToInt32(cmbLibro.SelectedValue);
            string titulo = cmbLibro.Text;
            int cantidad = (int)numCantidad.Value;
            decimal precioCompra = numPrecioCompra.Value;
            decimal precioVenta = numPrecioVenta.Value;

            var existente = detalle.FirstOrDefault(d => d.IdLibro == idLibro);

            if (existente != null)
            {
                existente.Cantidad += cantidad;
                existente.PrecioCompra = precioCompra;
                existente.PrecioVenta = precioVenta;
            }
            else
            {
                detalle.Add(new DetalleCompraItem
                {
                    IdLibro = idLibro,
                    Titulo = titulo,
                    Cantidad = cantidad,
                    PrecioCompra = precioCompra,
                    PrecioVenta = precioVenta
                });
            }

            RefrescarGrid();

            // Reset
            numCantidad.Value = 1;
            numPrecioCompra.Value = 0;
        }

        private void BtnQuitar_Click(object sender, EventArgs e)
        {
            if (dgvDetalle.CurrentRow == null) return;
            int index = dgvDetalle.CurrentRow.Index;
            detalle.RemoveAt(index);
            RefrescarGrid();
        }

        private void RefrescarGrid()
        {
            dgvDetalle.Rows.Clear();
            foreach (var item in detalle)
            {
                dgvDetalle.Rows.Add(
                    item.IdLibro,
                    item.Titulo,
                    item.Cantidad,
                    item.PrecioCompra.ToString("N2"),
                    item.PrecioVenta.ToString("N2"),
                    item.Subtotal.ToString("N2")
                );
            }
            lblTotal.Text = $"Total: ${detalle.Sum(d => d.Subtotal):N2}";
        }

        // ============ GUARDAR COMPRA ============
        private void BtnGuardar_Click(object sender, EventArgs e)
        {
            if (cmbProveedor.SelectedValue == null)
            {
                MessageBox.Show("Selecciona un proveedor.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (detalle.Count == 0)
            {
                MessageBox.Show("Agrega al menos un libro a la compra.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var confirm = MessageBox.Show(
                $"¿Confirmar compra?\n\nProveedor: {cmbProveedor.Text}\nTotal: ${detalle.Sum(d => d.Subtotal):N2}",
                "Confirmar Compra",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes) return;

            try
            {
                using var con = new SqlConnection(connectionString);
                con.Open();
                using var trans = con.BeginTransaction();

                try
                {
                    // 1) Insertar cabecera de Compra
                    var cmdCompra = new SqlCommand(@"
                        INSERT INTO Compra (idUsuario, idProveedor, tipo_documento, numero_documento, monto_total, fecha_registro)
                        OUTPUT INSERTED.id_compra
                        VALUES (@idUsuario, @idProveedor, @tipo, @numero, @total, GETDATE())", con, trans);

                    cmdCompra.Parameters.AddWithValue("@idUsuario", Sesion.IdUsuario);
                    cmdCompra.Parameters.AddWithValue("@idProveedor", cmbProveedor.SelectedValue);
                    cmdCompra.Parameters.AddWithValue("@tipo", txtTipoDoc.Text.Trim());
                    cmdCompra.Parameters.AddWithValue("@numero",
                        string.IsNullOrWhiteSpace(txtNumDoc.Text) ? "S/N" : txtNumDoc.Text.Trim());
                    cmdCompra.Parameters.AddWithValue("@total", detalle.Sum(d => d.Subtotal));

                    int idCompra = (int)cmdCompra.ExecuteScalar();

                    // 2) Insertar detalles y actualizar stock + precio_venta
                    foreach (var item in detalle)
                    {
                        var cmdDetalle = new SqlCommand(@"
                            INSERT INTO DetalleCompra (idCompra, idLibro, precio_compra, precio_venta, cantidad, fecha_registro)
                            VALUES (@idCompra, @idLibro, @pc, @pv, @cant, GETDATE())", con, trans);

                        cmdDetalle.Parameters.AddWithValue("@idCompra", idCompra);
                        cmdDetalle.Parameters.AddWithValue("@idLibro", item.IdLibro);
                        cmdDetalle.Parameters.AddWithValue("@pc", item.PrecioCompra);
                        cmdDetalle.Parameters.AddWithValue("@pv", item.PrecioVenta);
                        cmdDetalle.Parameters.AddWithValue("@cant", item.Cantidad);
                        cmdDetalle.ExecuteNonQuery();
                    }

                    trans.Commit();

                    MessageBox.Show(
                        $"¡Compra registrada con éxito!\n\nCompra #{idCompra}\nTotal: ${detalle.Sum(d => d.Subtotal):N2}\nStock actualizado.",
                        "Compra Exitosa",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Limpiar formulario
                    detalle.Clear();
                    RefrescarGrid();
                    txtNumDoc.Clear();
                }
                catch
                {
                    trans.Rollback();
                    throw;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar la compra:\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}