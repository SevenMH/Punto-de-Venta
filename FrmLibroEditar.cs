using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using FontAwesome.Sharp;

namespace P1
{
    public partial class FrmLibroEditar : Form
    {
        private readonly int idLibro;
        private readonly string connectionString =
            @"Server=DESKTOP-IQ00U68;Database=Libreria;Integrated Security=True;TrustServerCertificate=True;";

        // Controles
        private Label lblTitulo;
        private TextBox txtTitulo, txtDescripcion;
        private DateTimePicker dtpFechaPublicacion;
        private ComboBox cmbCategoria, cmbAutor, cmbEditorial, cmbProveedor;
        private CheckBox chkEstado;
        private IconButton btnGuardar, btnCancelar;

        public FrmLibroEditar(int id)
        {
            idLibro = id;
            ConstruirInterfazManual();
            this.Load += (s, e) =>
            {
                CargarCombos();
                if (idLibro > 0)
                    CargarLibro();
                else
                    this.Text = "➕ Nuevo Producto - Librería Morita";
            };
        }

        private void ConstruirInterfazManual()
        {
            this.Text = "Editar Producto - Librería Morita";
            this.Size = new Size(600, 550); // Tamaño inicial(adaptable al maximizar) de la pestaña (ancho, alto)
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.WhiteSmoke;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            // TÍTULO
            lblTitulo = new Label
            {
                Text = "📖 Datos del Producto",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(41, 128, 185),
                AutoSize = true,
                Location = new Point(20, 15)
            };
            this.Controls.Add(lblTitulo);

            int y = 60;
            int xLabel = 20;
            int xInput = 20;
            int anchoInput = 540;

            // TÍTULO
            AddLabel("Título del libro: *", xLabel, y); y += 25;
            txtTitulo = AddTextBox(xInput, y, anchoInput); y += 45;

            // DESCRIPCIÓN
            AddLabel("Descripción:", xLabel, y); y += 25;
            txtDescripcion = AddTextBox(xInput, y, anchoInput); y += 45;

            // FECHA PUBLICACIÓN
            AddLabel("Fecha de publicación:", xLabel, y); y += 25;
            dtpFechaPublicacion = new DateTimePicker
            {
                Location = new Point(xInput, y),
                Size = new Size(anchoInput, 28),
                Font = new Font("Segoe UI", 10),
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Today
            };
            this.Controls.Add(dtpFechaPublicacion);
            y += 45;

            // CATEGORÍA y AUTOR (en dos columnas)
            AddLabel("Categoría: *", xLabel, y);
            AddLabel("Autor: *", 320, y);
            y += 25;

            cmbCategoria = new ComboBox
            {
                Location = new Point(xInput, y),
                Size = new Size(280, 28),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10)
            };
            this.Controls.Add(cmbCategoria);

            // minimizar el combo de 240 a 200 para hacerle espacio al botón
            cmbAutor = new ComboBox
            {
                Location = new Point(320, y),
                Size = new Size(200, 28),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10)
            };
            this.Controls.Add(cmbAutor);

            //  El botón usa la misma 'y' actual (no la incrementada)
            IconButton btnAgregarAutor = new IconButton
            {
                IconChar = IconChar.Plus,
                IconColor = Color.White,
                IconSize = 14,
                Location = new Point(525, y), // Se alinea perfectamente a la derecha del combo
                Size = new Size(35, 28),
                BackColor = Color.FromArgb(39, 174, 96), // Verde
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnAgregarAutor.FlatAppearance.BorderSize = 0;
            btnAgregarAutor.Click += BtnAgregarAutor_Click;
            this.Controls.Add(btnAgregarAutor);

            y += 45; // Aumentar Y AQUÍ, al final, para que el siguiente control baje

            // EDITORIAL y PROVEEDOR
            AddLabel("Editorial: *", xLabel, y);
            AddLabel("Proveedor: *", 320, y);
            y += 25;

            cmbEditorial = new ComboBox
            {
                Location = new Point(xInput, y),
                Size = new Size(280, 28),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10)
            };
            this.Controls.Add(cmbEditorial);

            cmbProveedor = new ComboBox
            {
                Location = new Point(320, y),
                Size = new Size(240, 28),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10)
            };
            this.Controls.Add(cmbProveedor);
            y += 45;

            // ESTADO
            chkEstado = new CheckBox
            {
                Text = "Producto activo",
                Location = new Point(xInput, y),
                AutoSize = true,
                Font = new Font("Segoe UI", 10),
                Checked = true
            };
            this.Controls.Add(chkEstado);
            y += 45;

            // BOTONES
            btnGuardar = new IconButton
            {
                Text = "Guardar",
                IconChar = IconChar.Save,
                IconColor = Color.White,
                IconSize = 18,
                Location = new Point(300, y),
                Size = new Size(125, 45),
                BackColor = Color.FromArgb(39, 174, 96),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                ImageAlign = ContentAlignment.MiddleLeft,
                TextAlign = ContentAlignment.MiddleRight,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
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
                Location = new Point(435, y),
                Size = new Size(125, 45),
                BackColor = Color.Gray,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                ImageAlign = ContentAlignment.MiddleLeft,
                TextAlign = ContentAlignment.MiddleRight,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            btnCancelar.FlatAppearance.BorderSize = 0;
            btnCancelar.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };
            this.Controls.Add(btnCancelar);

            this.AcceptButton = btnGuardar;
            this.CancelButton = btnCancelar;
        }

        private void AddLabel(string texto, int x, int y)
        {
            var lbl = new Label
            {
                Text = texto,
                Location = new Point(x, y),
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            this.Controls.Add(lbl);
        }

        private TextBox AddTextBox(int x, int y, int ancho)
        {
            var txt = new TextBox
            {
                Location = new Point(x, y),
                Size = new Size(ancho, 28),
                Font = new Font("Segoe UI", 10)
            };
            this.Controls.Add(txt);
            return txt;
        }

        // ============ CARGAR COMBOS ============
        private void CargarCombos()
        {
            try
            {
                using var con = new SqlConnection(connectionString);
                con.Open();

                cmbCategoria.DataSource = ObtenerTabla(con,
                    "SELECT id_categoria, nombreCategoria FROM Categoria ORDER BY nombreCategoria");
                cmbCategoria.DisplayMember = "nombreCategoria";
                cmbCategoria.ValueMember = "id_categoria";

                cmbAutor.DataSource = ObtenerTabla(con,
                    "SELECT id_autor, nombre + ' ' + apellido AS nombreCompleto FROM Autor ORDER BY apellido");
                cmbAutor.DisplayMember = "nombreCompleto";
                cmbAutor.ValueMember = "id_autor";

                cmbEditorial.DataSource = ObtenerTabla(con,
                    "SELECT id_editorial, nombre FROM Editorial ORDER BY nombre");
                cmbEditorial.DisplayMember = "nombre";
                cmbEditorial.ValueMember = "id_editorial";

                cmbProveedor.DataSource = ObtenerTabla(con,
                    "SELECT id_proveedor, nombreProveedor FROM Proveedor WHERE estado = 1 ORDER BY nombreProveedor");
                cmbProveedor.DisplayMember = "nombreProveedor";
                cmbProveedor.ValueMember = "id_proveedor";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar combos:\n" + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ============ CREACIÓN RÁPIDA DE AUTOR ============
        private void BtnAgregarAutor_Click(object sender, EventArgs e)
        {
            using var frm = new FrmAutorEditar();

            // Si el usuario guardó el autor, actualizamos el combo
            if (frm.ShowDialog() == DialogResult.OK)
            {
                // Volvemos a cargar todos los combos para que aparezca el nuevo
                CargarCombos();

                // Seleccionamos automáticamente el autor que se acaba de crear
                cmbAutor.SelectedValue = frm.IdAutorCreado;
            }
        }

        private DataTable ObtenerTabla(SqlConnection con, string query)
        {
            var dt = new DataTable();
            using var cmd = new SqlCommand(query, con);
            using var da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            return dt;
        }

        // ============ CARGAR LIBRO EXISTENTE ============
        private void CargarLibro()
        {
            try
            {
                using var con = new SqlConnection(connectionString);
                con.Open();

                var cmd = new SqlCommand(@"
                    SELECT titulo, descripcion, fecha_publicacion, 
                           ISNULL(precio_compra, 0) AS precio_compra, 
                           precio_venta, stock, 
                           idCategoria, idAutor, idEditorial, idProveedor, estado
                    FROM Libro WHERE id_libro = @id", con);
                cmd.Parameters.AddWithValue("@id", idLibro);

                using var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    txtTitulo.Text = reader["titulo"].ToString();
                    txtDescripcion.Text = reader["descripcion"]?.ToString() ?? "";

                    if (reader["fecha_publicacion"] != DBNull.Value)
                        dtpFechaPublicacion.Value = Convert.ToDateTime(reader["fecha_publicacion"]);

                    cmbCategoria.SelectedValue = reader["idCategoria"];
                    cmbAutor.SelectedValue = reader["idAutor"];
                    cmbEditorial.SelectedValue = reader["idEditorial"];
                    cmbProveedor.SelectedValue = reader["idProveedor"];
                    chkEstado.Checked = Convert.ToBoolean(reader["estado"]);

                    lblTitulo.Text = "✏️ Editar Producto";
                    this.Text = "Editar Producto - Librería Morita";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar libro:\n" + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ============ GUARDAR ============
        private void BtnGuardar_Click(object sender, EventArgs e)
        {
            // Validaciones de título y combos
            if (string.IsNullOrWhiteSpace(txtTitulo.Text))
            {
                MessageBox.Show("El título es obligatorio.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTitulo.Focus();
                return;
            }

            if (cmbCategoria.SelectedValue == null || cmbAutor.SelectedValue == null ||
                cmbEditorial.SelectedValue == null || cmbProveedor.SelectedValue == null)
            {
                MessageBox.Show("Todos los combos son obligatorios.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using var con = new SqlConnection(connectionString);
                con.Open();

                string query;
                if (idLibro > 0)
                { 
                    query = @"
                        UPDATE Libro SET
                            titulo = @titulo,
                            descripcion = @descripcion,
                            fecha_publicacion = @fechaPub,
                            idCategoria = @idCat,
                            idAutor = @idAut,
                            idEditorial = @idEdi,
                            idProveedor = @idProv,
                            estado = @estado
                        WHERE id_libro = @id";
                }
                else
                {
                    // En el INSERT inicializamos precios y stock en 0
                    query = @"
                        INSERT INTO Libro 
                        (titulo, descripcion, fecha_publicacion, precio_compra, precio_venta, stock,
                         idCategoria, idAutor, idEditorial, idProveedor, estado, fecha_registro)
                        VALUES 
                        (@titulo, @descripcion, @fechaPub, 0, 0, 0,
                         @idCat, @idAut, @idEdi, @idProv, @estado, GETDATE())";
                }

                using var cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@titulo", txtTitulo.Text.Trim());
                cmd.Parameters.AddWithValue("@descripcion",
                    string.IsNullOrWhiteSpace(txtDescripcion.Text) ? (object)DBNull.Value : txtDescripcion.Text.Trim());
                cmd.Parameters.AddWithValue("@fechaPub", dtpFechaPublicacion.Value);
                cmd.Parameters.AddWithValue("@idCat", cmbCategoria.SelectedValue);
                cmd.Parameters.AddWithValue("@idAut", cmbAutor.SelectedValue);
                cmd.Parameters.AddWithValue("@idEdi", cmbEditorial.SelectedValue);
                cmd.Parameters.AddWithValue("@idProv", cmbProveedor.SelectedValue);
                cmd.Parameters.AddWithValue("@estado", chkEstado.Checked ? 1 : 0);

                if (idLibro > 0)
                    cmd.Parameters.AddWithValue("@id", idLibro);

                cmd.ExecuteNonQuery();

                MessageBox.Show(
                    idLibro > 0 ? "Producto actualizado correctamente." : "Producto creado correctamente.",
                    "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}