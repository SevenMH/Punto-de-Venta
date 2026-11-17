using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using FontAwesome.Sharp;

namespace P1
{
    public partial class FrmProveedorEditar : Form
    {
        private readonly int idProveedor;
        private readonly string connectionString =
            @"Server=DESKTOP-IQ00U68;Database=Libreria;Integrated Security=True;TrustServerCertificate=True;";

        // Controles
        private Label lblTitulo;
        private TextBox txtNombre, txtTelefono, txtCorreo, txtDireccion;
        private CheckBox chkEstado;
        private IconButton btnGuardar, btnCancelar;

        public FrmProveedorEditar(int id)
        {
            idProveedor = id;
            ConstruirInterfazManual();
            this.Load += (s, e) =>
            {
                if (idProveedor > 0)
                    CargarProveedor();
                else
                    this.Text = "➕ Nuevo Proveedor - Librería Morita";
            };
        }

        private void ConstruirInterfazManual()
        {
            this.Text = "Editar Proveedor - Librería Morita";
            this.Size = new Size(550, 500);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.WhiteSmoke;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            // TÍTULO
            lblTitulo = new Label
            {
                Text = "🚚 Datos del Proveedor",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(155, 89, 182),
                AutoSize = true,
                Location = new Point(20, 15)
            };
            this.Controls.Add(lblTitulo);

            int y = 65;
            int xInput = 20;
            int anchoInput = 495;

            // NOMBRE
            AddLabel("Nombre del proveedor: *", xInput, y); y += 25;
            txtNombre = AddTextBox(xInput, y, anchoInput); y += 45;

            // TELÉFONO
            AddLabel("Teléfono: * (10 dígitos)", xInput, y); y += 25;
            txtTelefono = AddTextBox(xInput, y, anchoInput);
            txtTelefono.MaxLength = 10;
            txtTelefono.KeyPress += (s, e) =>
            {
                // Solo dígitos
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                    e.Handled = true;
            };
            y += 45;

            // CORREO
            AddLabel("Correo electrónico: *", xInput, y); y += 25;
            txtCorreo = AddTextBox(xInput, y, anchoInput); y += 45;

            // DIRECCIÓN
            AddLabel("Dirección: *", xInput, y); y += 25;
            txtDireccion = AddTextBox(xInput, y, anchoInput); y += 45;

            // ESTADO
            chkEstado = new CheckBox
            {
                Text = "Proveedor activo",
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
                Location = new Point(245, y),
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
                Location = new Point(380, y),
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

        // ============ CARGAR ============
        private void CargarProveedor()
        {
            try
            {
                using var con = new SqlConnection(connectionString);
                con.Open();

                var cmd = new SqlCommand(@"
                    SELECT nombreProveedor, telefono, correo, direccion, estado
                    FROM Proveedor WHERE id_proveedor = @id", con);
                cmd.Parameters.AddWithValue("@id", idProveedor);

                using var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    txtNombre.Text = reader["nombreProveedor"].ToString();
                    txtTelefono.Text = reader["telefono"].ToString();
                    txtCorreo.Text = reader["correo"].ToString();
                    txtDireccion.Text = reader["direccion"].ToString();
                    chkEstado.Checked = Convert.ToBoolean(reader["estado"]);

                    lblTitulo.Text = "✏️ Editar Proveedor";
                    this.Text = "Editar Proveedor - Librería Morita";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar proveedor:\n" + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ============ GUARDAR ============
        private void BtnGuardar_Click(object sender, EventArgs e)
        {
            // Validaciones
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                MessageBox.Show("El nombre es obligatorio.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNombre.Focus();
                return;
            }

            if (txtTelefono.Text.Length != 10)
            {
                MessageBox.Show("El teléfono debe tener exactamente 10 dígitos.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTelefono.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtCorreo.Text) || !txtCorreo.Text.Contains("@") || !txtCorreo.Text.Contains("."))
            {
                MessageBox.Show("El correo no tiene un formato válido.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCorreo.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtDireccion.Text))
            {
                MessageBox.Show("La dirección es obligatoria.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtDireccion.Focus();
                return;
            }

            try
            {
                using var con = new SqlConnection(connectionString);
                con.Open();

                string query;
                if (idProveedor > 0)
                {
                    query = @"
                        UPDATE Proveedor SET
                            nombreProveedor = @nombre,
                            telefono = @telefono,
                            correo = @correo,
                            direccion = @direccion,
                            estado = @estado
                        WHERE id_proveedor = @id";
                }
                else
                {
                    query = @"
                        INSERT INTO Proveedor 
                        (nombreProveedor, telefono, correo, direccion, estado, fecha_registro)
                        VALUES 
                        (@nombre, @telefono, @correo, @direccion, @estado, GETDATE())";
                }

                using var cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@nombre", txtNombre.Text.Trim());
                cmd.Parameters.AddWithValue("@telefono", txtTelefono.Text.Trim());
                cmd.Parameters.AddWithValue("@correo", txtCorreo.Text.Trim());
                cmd.Parameters.AddWithValue("@direccion", txtDireccion.Text.Trim());
                cmd.Parameters.AddWithValue("@estado", chkEstado.Checked ? 1 : 0);

                if (idProveedor > 0)
                    cmd.Parameters.AddWithValue("@id", idProveedor);

                cmd.ExecuteNonQuery();

                MessageBox.Show(
                    idProveedor > 0 ? "Proveedor actualizado correctamente." : "Proveedor creado correctamente.",
                    "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2627) // UNIQUE
                {
                    MessageBox.Show(
                        "Ya existe un proveedor con ese correo electrónico.",
                        "Correo duplicado",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtCorreo.Focus();
                }
                else
                {
                    MessageBox.Show("Error al guardar:\n" + ex.Message, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}