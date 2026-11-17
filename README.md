# Librería Morita — Sistema de Punto de Venta

## Descripción General

**Librería Morita** es un sistema de gestión de punto de venta (POS) desarrollado en **C# con Windows Forms** para una librería. Permite administrar ventas, compras, inventario, clientes, proveedores, caja y promociones, con control de acceso basado en roles y **generación de tickets en PDF**.

| Dato | Detalle |
|------|---------|
| **Versión** | 1.1 |
| **Plataforma** | Windows (WinForms) |
| **Lenguaje** | C# |
| **Base de Datos** | SQL Server (`Libreria`) |
| **Servidor** | `DESKTOP-IQ00U68` |
| **Autenticación BD** | Integrated Security |
| **Librería de iconos** | FontAwesome.Sharp |
| **Generación de PDFs** | QuestPDF |

---

## Arquitectura del Proyecto

El proyecto sigue un enfoque de **formularios autocontenidos** donde cada `Form` construye su interfaz manualmente mediante el método `ConstruirInterfazManual()`, evitando el diseñador visual. Las consultas SQL se realizan directamente con `Microsoft.Data.SqlClient`.

### Estructura de archivos identificados

```
P1/
├── Modelos/
│   ├── DetalleCompraItem.cs
│   └── DetalleVentaItem.cs
│
├── Formularios de Caja/
│   ├── FrmAperturaCaja.cs
│   ├── FrmCaja.cs
│   └── FrmCierreCaja.cs
│
├── Formularios de Clientes/
│   ├── FrmClientes.cs
│   └── FrmClienteEditar.cs
│
├── Formularios de Productos/
│   ├── FrmLibroEditar.cs
│   └── FrmAutorEditar.cs
│
├── Formularios de Compras/
│   └── FrmCompras.cs
│
├── Utilidades/
│   ├── Sesion.cs
│   ├── TicketGenerator.cs
│   └── FrmAcercaDe.cs
```

---

## Modelos de Datos

### `DetalleCompraItem`
Representa una línea del detalle de una compra a proveedor.

| Propiedad | Tipo | Descripción |
|-----------|------|-------------|
| `IdLibro` | `int` | Identificador del libro |
| `Titulo` | `string` | Título del libro |
| `Cantidad` | `int` | Unidades compradas |
| `PrecioCompra` | `decimal` | Costo unitario |
| `PrecioVenta` | `decimal` | Precio de venta sugerido |
| `Subtotal` | `decimal` | `Cantidad × PrecioCompra` (calculado) |

### `DetalleVentaItem`
Representa una línea del carrito de venta en el punto de venta.

| Propiedad | Tipo | Descripción |
|-----------|------|-------------|
| `IdLibro` | `int` | Identificador del libro |
| `IdCategoria` | `int` | Categoría (para promociones) |
| `Titulo` | `string` | Título del libro |
| `Cantidad` | `int` | Unidades vendidas |
| `PrecioUnitario` | `decimal` | Precio de venta unitario |
| `StockDisponible` | `int` | Stock validado al momento de la venta |
| `Subtotal` | `decimal` | `Cantidad × PrecioUnitario` (calculado) |

---

## Gestión de Sesión (`Sesion`)

Clase **estática** que mantiene el estado global de la sesión activa del usuario autenticado, accesible desde cualquier formulario del sistema sin necesidad de instanciarla.

```csharp
public static class Sesion
{
    public static int IdUsuario { get; set; }
    public static int IdEmpleado { get; set; }
    public static int IdRol { get; set; }
    public static string NombreUsuario { get; set; } = string.Empty;
    public static string NombreRol { get; set; } = string.Empty;
    public static bool CerrandoSesion { get; set; } = false;
}
```

| Propiedad | Tipo | Descripción |
|-----------|------|-------------|
| `IdUsuario` | `int` | Identificador del usuario logueado (usado en compras) |
| `IdEmpleado` | `int` | Identificador del empleado (usado en caja) |
| `IdRol` | `int` | Rol activo (1=Admin, 2=Cajero, 3=Almacenero) |
| `NombreUsuario` | `string` | Nombre mostrado en la UI (ej. Apertura de Caja) |
| `NombreRol` | `string` | Descripción del rol activo |
| `CerrandoSesion` | `bool` | Bandera para distinguir cierre de sesión de cierre de app |

**Usos identificados en el sistema:**
- `FrmAperturaCaja` → `Sesion.IdEmpleado` y `Sesion.NombreUsuario`
- `FrmCaja` → `Sesion.IdEmpleado`
- `FrmCompras` → `Sesion.IdUsuario`

---

## Esquema de Base de Datos (inferido)

Las tablas utilizadas por los formularios son:

| Tabla | Campos principales |
|-------|-------------------|
| **Caja** | `id_caja`, `idEmpleado`, `fecha_apertura`, `fecha_cierre`, `monto_inicial`, `monto_final`, `estado` |
| **MovimientoCaja** | `id_caja`, `tipo` (Ingreso/Egreso), `monto` |
| **Pedido** | `id_pedido`, `id_caja`, `idCliente`, `nombre_cliente`, `estado`, `metodo_pago`, `monto_total`, `monto_pago`, `monto_cambio`, `fecha_pedido`, `numero_documento` |
| **DetallePedido** | `idPedido`, `idLibro`, `cantidad`, `precio_unitario`, `subtotal` |
| **Empleado** | `id_empleado`, `nombreEmpleado` |
| **Cliente** | `id_cliente`, `documento`, `nombreCliente`, `telefono`, `correo`, `direccion`, `estado`, `fecha_registro`, `fechaNacimiento` |
| **Autor** | `id_autor`, `nombre`, `apellido`, `nacionalidad`, `fecha_nacimiento` |
| **Libro** | `id_libro`, `titulo`, `precio_venta`, `estado`, `idCategoria`, `idAutor`, `idEditorial`, `idProveedor` |
| **Categoria** | `id_categoria`, `nombreCategoria` |
| **Editorial** | `id_editorial`, `nombre` |
| **Proveedor** | `id_proveedor`, `nombreProveedor`, `estado` |
| **Compra** | `id_compra`, `idUsuario`, `idProveedor`, `tipo_documento`, `numero_documento`, `monto_total`, `fecha_registro` |
| **DetalleCompra** | `idCompra`, `idLibro`, `precio_compra`, `precio_venta`, `cantidad`, `fecha_registro` |

---

## Módulos del Sistema

### 1. Módulo de Caja (`FrmCaja`)

Panel central para la gestión del turno de caja.

**Funcionalidades:**
- Visualización del estado actual (Abierta/Cerrada) del empleado logueado.
- Resumen en tiempo real del turno:
  - Monto inicial
  - Ventas en efectivo
  - Ventas con tarjeta/transferencia/Yape-Plin
  - Ingresos extra
  - Egresos
  - **Efectivo esperado** = `Monto Inicial + Ventas Efectivo + Ingresos − Egresos`
- Historial de cajas cerradas con diferencia (sobrante/faltante) resaltada por color.
- Botones de acción: **Abrir Caja**, **Movimiento**, **Cerrar Caja**.

**Estados visuales:**
- Caja abierta → botón "Abrir" deshabilitado.
- Caja cerrada → botones "Movimiento" y "Cerrar" deshabilitados.

---

### 2. Apertura de Caja (`FrmAperturaCaja`)

- Solicita el **monto inicial** del turno.
- Valida que el empleado no tenga ya una caja abierta (`estado = 'Abierta'`).
- Inserta un registro en la tabla `Caja` con `GETDATE()` como fecha de apertura.

---

### 3. Cierre de Caja (`FrmCierreCaja`)

- Muestra el resumen completo del turno (calculado en base a `Pedido` y `MovimientoCaja`).
- Solicita el **efectivo contado físicamente**.
- Calcula la **diferencia**:
  - Cuadrada (verde)
  - Sobrante (azul)
  - Faltante (rojo)
- Actualiza `Caja` con `fecha_cierre`, `monto_final` y `estado = 'Cerrada'`.

---

### 4. Gestión de Clientes (`FrmClientes`)

**Búsqueda y filtros:**
- Búsqueda por documento, nombre, correo o teléfono.
- Casilla "Mostrar inactivos" para incluir clientes desactivados.

**Operaciones:**
- **Nuevo / Editar**: abre `FrmClienteEditar`.
- **Eliminar**: lógica de protección:
  - El cliente **"Público General"** (documento `00000000`) no puede eliminarse ni desactivarse.
  - Si el cliente tiene ventas asociadas → solo se **desactiva** (`estado = 0`).
  - Si no tiene ventas → permite **eliminación permanente** o desactivación.
- Filas de clientes inactivos se muestran en gris cursiva.

---

### 5. Edición de Cliente (`FrmClienteEditar`)

**Campos:** Documento, Nombre, Fecha de nacimiento, Teléfono (10 dígitos), Correo, Dirección, Estado.

**Validaciones:**
- Documento y nombre obligatorios.
- Teléfono con exactamente 10 dígitos (solo numérico).
- Correo con formato `@` y `.`.
- Captura de `SqlException 2627` (violación UNIQUE) → mensaje "correo duplicado".

**Protección especial:** el cliente "Público General" tiene campos críticos en solo lectura.

---

### 6. Registro de Compras (`FrmCompras`)

**Flujo:**
1. Seleccionar proveedor y datos del documento (tipo y número).
2. Agregar libros al detalle indicando: libro, cantidad, precio de compra y precio de venta.
3. Al seleccionar un libro, se autocompleta el `precio_venta` actual.
4. Guardar la compra dentro de una **transacción**:
   - Inserta cabecera en `Compra` (`OUTPUT INSERTED.id_compra`).
   - Inserta detalles en `DetalleCompra`.
   - (Nota: el comentario menciona actualización de stock y precio de venta, aunque la lógica no aparece en el fragmento visible.)

---

### 7. Edición de Producto (`FrmLibroEditar`)

**Campos:** Título, Descripción, Fecha de publicación, Categoría, Autor, Editorial, Proveedor, Estado.

**Características destacadas:**
- Botón verde **"+"** al lado del combo de autor → abre `FrmAutorEditar` para registrar un autor nuevo sin salir del formulario.
- Combos cargados dinámicamente desde la BD (solo proveedores activos).

---

### 8. Autor Rápido (`FrmAutorEditar`)

- Registro exprés de autores: Nombre, Apellido, Nacionalidad, Fecha de nacimiento.
- Devuelve el `IdAutorCreado` mediante `OUTPUT INSERTED.id_autor` para integrarlo al formulario de producto.

---

### 9. Generación de Tickets PDF (`TicketGenerator`)

Clase **estática** que genera el ticket de venta en formato PDF usando **QuestPDF**, con formato de impresora térmica de 80 mm.

**Método principal:**
```csharp
public static void GenerarTicket(int idPedido, string rutaDestino)
```

**Flujo de generación:**

1. **Carga de datos desde la BD** (dos consultas):
   - **Cabecera:** `nombre_cliente`, `monto_total`, `monto_pago`, `monto_cambio`, `fecha_pedido`, `numero_documento` desde `Pedido`.
   - **Detalle:** `titulo`, `cantidad`, `precio_unitario`, `subtotal` desde `DetallePedido` + `Libro`.

2. **Cálculo de impuestos** (el total ya incluye IVA):
   ```csharp
   decimal ivaPorcentaje = 0.16m;
   decimal subtotal = Math.Round(montoTotal / (1 + ivaPorcentaje), 2);
   decimal ivaMonto = montoTotal - subtotal;
   ```

3. **Renderizado del PDF con QuestPDF**:
   - **Tamaño:** 80 mm de ancho × 250 mm de alto (ticket térmico).
   - **Margen:** 5 mm.
   - **Tipografía:** Verdana 9 pt por defecto.

**Estructura del ticket:**

| Sección | Contenido |
|---------|-----------|
| **Header** | Nombre del negocio, subtítulo, teléfono, número de ticket, fecha, cliente |
| **Content** | Tabla de productos (cantidad, título, subtotal), subtotal, IVA (16%), **TOTAL**, pago y cambio |
| **Footer** | Mensaje de agradecimiento y nombre del negocio |

**Ejemplo de salida:**
```
═══════════════════════════
      LIBRERÍA MORITA
       Punto de Venta
      Tel: 555-123-4567
───────────────────────────
Ticket: F001-000123
Fecha: 15/03/2026 14:32
Cliente: Juan Pérez
───────────────────────────
Cant. Producto      Total
───────────────────────────
2x Cien Años...    $45.00
1x El Principito   $18.00
───────────────────────────
Subtotal:          $54.31
IVA (16%):          $8.69
TOTAL:             $63.00
Pago:              $70.00
Cambio:             $7.00
───────────────────────────

   ¡Gracias por su compra!
       Librería Morita
═══════════════════════════
```

**Ventajas del uso de QuestPDF:**
- Generación **fluida** con API de tipo `Document.Create() → container.Page()`.
- Salida a archivo directo: `.GeneratePdf(rutaDestino)`.
- Diseño responsivo con `RelativeItem()` y `AlignCenter()`.
- Sin dependencias de GDI+ o Crystal Reports.

---

### 10. Acerca De (`FrmAcercaDe`)

Formulario informativo con:
- **Guía rápida de uso** por módulo (POS, Promociones, Compras, Productos, Proveedores, Clientes, Caja, Permisos).
- **Créditos:** © 2026 Librería Morita.

---

## Promociones Automáticas

Documentadas en la guía rápida del sistema:

| Promoción | Descuento | Condición |
|-----------|-----------|-----------|
| **Aniversario** | 20% | Solo el `09/11/2026` |
| **Cumpleaños** | 10% | Cliente registrado y es su día |
| **Categorías** | 5% | Categorías seleccionadas |

> Los descuentos se **acumulan** y se muestran antes del cobro.

---

## Roles y Permisos

| Rol | Acceso |
|-----|--------|
| **Administrador** | Acceso total al sistema |
| **Cajero** | Punto de Venta y Caja |
| **Almacenero** | Productos, Proveedores y Compras |

La sesión activa se gestiona mediante la clase estática `Sesion` (`Sesion.IdEmpleado`, `Sesion.IdUsuario`, `Sesion.NombreUsuario`, `Sesion.NombreRol`).

---

## Convenciones de UI

- **Colores corporativos:**
  - Azul (`41, 128, 185`) → Módulos de productos / compras
  - Verde (`39, 174, 96`) → Caja abierta / acciones de guardado
  - Naranja (`230, 126, 34`) → Clientes
  - Rojo (`192, 57, 43`) → Eliminar / cerrar / egresos
  - Púrpura (`155, 89, 182`) → Efectivo esperado
- **Fuente:** Segoe UI (con estilos Bold en títulos y botones).
- **Iconografía:** `FontAwesome.Sharp` (`IconChar.BookOpen`, `IconChar.Save`, `IconChar.LockOpen`, etc.).
- **Layout:** Paneles con `BorderStyle.FixedSingle`, headers `Dock = Top`, `DataGridView` con `AutoSizeColumnsMode = Fill`.

---

## Patrones y prácticas observadas

| Práctica | Implementación |
|----------|----------------|
| **Construcción manual de UI** | Todos los formularios usan `ConstruirInterfazManual()` en el constructor |
| **Estado global de sesión** | Clase estática `Sesion` con propiedades compartidas |
| **Consultas parametrizadas** | Uso de `cmd.Parameters.AddWithValue()` para evitar inyección SQL |
| **Transacciones** | `con.BeginTransaction()` + `trans.Commit()/Rollback()` en compras |
| **Manejo de excepciones** | Bloques `try/catch` con `MessageBox` informativos |
| **Recursos liberados** | Uso de `using var` para `SqlConnection`, `SqlCommand`, `SqlDataReader` |
| **Validación defensiva** | Verificación de estados antes de operaciones sensibles (caja, cliente protegido) |
| **Generación de documentos** | QuestPDF para tickets PDF sin depender de visores externos |

---

## Observaciones y oportunidades de mejora

1. **Cadena de conexión hardcodeada** en cada archivo (`Sesion.cs` es la excepción, no la incluye) → debería centralizarse en `App.config` o una clase `DbConfig`.
2. **Lógica de negocio en la capa de UI** → considerar extraer a una capa de servicios/repositorios.
3. **Falta la lógica de actualización de stock** al registrar compras (el comentario existe, pero no la implementación visible).
4. **FrmLibroEditar está incompleto** (el archivo se corta en `// =`).
5. **Sin validación de stock negativo** visible en el fragmento del POS.
6. **No se observa `FrmPuntoVenta`** ni `FrmMovimientoCaja` en el conjunto entregado, aunque son referenciados por `FrmCaja`.
7. **TicketGenerator**: el tamaño del ticket es fijo (80×250 mm). Tickets con muchos productos podrían cortarse; se sugiere calcular la altura dinámicamente según el número de ítems.
8. **TicketGenerator**: la tasa de IVA (16%) está hardcodeada → debería configurarse o leerse de la BD.

---

## Resumen Ejecutivo

**Librería Morita** es un POS completo para librerías que integra:

Control de caja con apertura, movimientos y cierre con conciliación.  
Gestión de clientes con protección del cliente genérico y borrado lógico.  
Registro de compras a proveedores con actualización de precios.  
Catálogo de productos con alta rápida de autores.  
Sistema de promociones automáticas (aniversario, cumpleaños, categoría).  
Control de acceso por roles (Administrador, Cajero, Almacenero).  
**Generación de tickets en PDF** con formato térmico de 80 mm vía QuestPDF.  
**Gestión centralizada de sesión** mediante la clase estática `Sesion`.

El sistema está diseñado para operar en un entorno Windows con SQL Server local, priorizando la **trazabilidad** (historial de cajas, protección de clientes con ventas), la **usabilidad** (interfaces con iconografía clara y guía integrada) y la **profesionalización de la salida** (tickets PDF imprimibles).
