# Laboratorio # 4 📦

**Fecha:** 14/09/2026

## Contenido del Repositorio
En esta práctica de laboratorio se desarrolló un sistema de **Gestión de Inventario de Productos** en C# para entorno de escritorio mediante **Windows Forms**. La solución implementa las operaciones fundamentales del ciclo CRUD (Crear, Leer, Actualizar y Eliminar) vinculadas a una base de datos **MySQL**. Se puso especial énfasis en el procesamiento binario de archivos de imagen para su almacenamiento en campos tipo `LONGBLOB` utilizando flujos de memoria (`MemoryStream`), la prevención de ataques de inyección SQL mediante consultas parametrizadas, y el filtrado dinámico de datos en tiempo real sobre la interfaz gráfica.

## Tecnologías Utilizadas
* **Lenguaje / Framework:** C# (.NET Framework / Windows Forms)
* **Base de Datos / Conectores:** MySQL Server, `MySql.Data` (MySQL Connector/NET)
* **IDE / Herramientas:** Visual Studio, Git, GitHub

## Capturas de Pantalla y Problemas

### Interfaz Principal
![Interfaz Principal de Windows Forms](img/interfaz_principal.png)

### Programa 1: Formulario Principal de Productos (`Form1.cs`)
* **Descripción de la solución:**  
  Implementa la interfaz gráfica interactiva con el usuario. Administra los eventos del `DataGridView` para mostrar la lista actualizada de productos y la carga dinámica de imágenes al hacer clic en el `PictureBox` usando un `OpenFileDialog`. Incluye métodos de conversión binaria (`ImageToByteArray` y `ByteArrayToImage`) para transformar archivos visuales en arreglos de bytes (`byte[]`) requeridos por MySQL. Además, controla los eventos de botones para ejecutar las sentencias SQL parametrizadas (`INSERT`, `UPDATE`, `DELETE`) y el filtrado en tiempo real a través del evento `TextChanged` del buscador.

### Programa 2: Clase Helper de Conexión y Datos (`Conexion.cs`)
* **Descripción de la solución:**  
  Clase encargada de la persistencia y la conexión directa con la base de datos MySQL mediante `MySqlConnection`. Proporciona el método `GetProductos()` que utiliza `MySqlDataReader` para mapear los registros hacia una colección fuertemente tipada (`List`). Implementa algoritmos dinámicos como `InsertSeguro`, `Actualizar` y `Eliminar`, los cuales procesan la información mediante diccionarios clave-valor (`Dictionary`) para construir y ejecutar comandos de forma segura con parámetros SQL.

## Estructura de Carpetas o Directorios

```plaintext
Laboratorio4/
├── Form1.cs                    # Lógica de la interfaz gráfica y eventos CRUD
├── Form1.Designer.cs           # Diseño del formulario (DataGridView, PictureBox, etc.)
├── Form1.resx                  # Recursos locales de la vista
├── Conexion.cs                 # Helper de conexión a MySQL y consultas parametrizadas
├── Producto.cs                 # Clase modelo de la entidad Producto
├── Program.cs                  # Punto de entrada de la aplicación
├── Laboratorio4.csproj         # Configuración del proyecto y paquetes de NuGet
├── img/                        # Capturas de pantalla para la documentación
│   └── interfaz_principal.png
└── README.md                   # Documentación del laboratorio
