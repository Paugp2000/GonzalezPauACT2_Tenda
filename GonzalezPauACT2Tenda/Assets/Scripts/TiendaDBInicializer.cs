using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using UnityEngine;
using SQLite4Unity3d;
using UnityEngine.UIElements;
using TMPro;
using System.Data;
using Mono.Data.Sqlite;

public class TiendaDBInicializer : MonoBehaviour
{
    private string _dbPath;
    private string _streamingPath;
    private SQLiteConnection dbConnection;
    public string[] nombresFrutas = { "Manzana", "Naranja", "Piña", "Melocoton" };
    public int[] preciosFrutas = { 30, 25, 50, 65 };
    private bool existeInventario = false;
    public int frutaSeleccionadaActual;
    public int precioDeFrutaSeleccionada;
    public int cantidadDeFrutaSeleccionada;
    public int precioAPagarNum;
    public int idInventarioActual;

    public TextMeshProUGUI numManzanas;
    public TextMeshProUGUI numNaranjas;
    public TextMeshProUGUI numPiñas;
    public TextMeshProUGUI numMelocotones;
    public TextMeshProUGUI numDinero;
    public TextMeshProUGUI tipoDeProducto;
    public TextMeshProUGUI precioAPagar;

    public GameObject panelSeleccionCantidad;
    public GameObject panelConfirmacionCompra;
    public TMP_InputField cantidadDeseada;
   

    public Inventario inventarioCargado;
    public SQLiteCommand comandoCantidadManzanas;
    public SQLiteCommand comandoCantidadNaranjas;
    public SQLiteCommand comandoCantidadPiñas;
    public SQLiteCommand comandoCantidadMelocotones;
    public SQLiteCommand comandoDevolverDinero;
    public SQLiteCommand comandoExtraerPrecio;
    public SqliteTransaction transactionCompra;
   

    private void Start()
    {
        CargarDatabase();

    }
    public void CargarDatabase()
    {
        _streamingPath = Path.Combine(Application.streamingAssetsPath, "MyDatabase.sqlite");
        _dbPath = Path.Combine(Application.persistentDataPath, "MyDatabase.sqlite");
        // 2. Verificar si el archivo ya existe en la carpeta de datos
        // Si ya existe, NO intentamos copiarlo (evita el error de "archivo en uso")
        if (File.Exists(_dbPath))
        {
            Debug.Log("Base de datos ya existe en persistentDataPath. Conectando...");
        }
        else
        {
            // 3. Solo copiar si no existe
            if (File.Exists(_streamingPath))
            {
                try
                {
                    File.Copy(_streamingPath, _dbPath);
                    Debug.Log("Base de datos copiada a persistentDataPath.");
                }
                catch (IOException ex)
                {
                    Debug.LogError($"Error al copiar archivo: {ex.Message}");
                    Debug.LogWarning("Intentando conectar sin copiar...");
                }
            }
            else
            {
                Debug.LogError("¡ERROR! No se encontró el archivo en StreamingAssets: " + _streamingPath);
                return;
            }
        }

        // 4. Abrir la conexión DESDE persistentDataPath
        try
        {
            dbConnection = new SQLiteConnection(_dbPath);
            Debug.Log("Conexión a SQLite establecida correctamente.");
    
        }
        catch (Exception ex)
        {
            Debug.LogError("Error al abrir SQLite: " + ex.Message);
        }

        CrearTablaInventario();
    }
    public void CrearTablaInventario()
    {
        dbConnection.CreateTable<Inventario>();
        var inventarios = dbConnection.Table<Inventario>();
        foreach (var row in inventarios)
        {
            if (row.IdInventario == LoginSQLController.idUsuarioIntroducido)
            {
                existeInventario = true;
            }
            else
            {

            }
        }
        if (!existeInventario)
        {
            dbConnection.CreateTable<Inventario>();
            dbConnection.CreateCommand("CREATE TABLE IF NOT EXISTS InventarioObjeto (" +
                          "idInventario INT," +
                          "idObjeto INT," +
                          "cantidad INT, " +
                          "PRIMARY KEY (idInventario, idObjeto)," +
                          "FOREIGN KEY (idInventario) REFERENCES Inventario(IdInventario)," +
                          "FOREIGN KEY (idObjeto) REFERENCES Objeto(idObjeto));").ExecuteNonQuery();    
            dbConnection.CreateTable<Objeto>();

            for (int i = 0; i < nombresFrutas.Length; i++)
            {
                AddFruta(i+1, nombresFrutas[i], preciosFrutas[i]);
            }
            AddInventario();
            CargarInventario(LoginSQLController.idUsuarioIntroducido);
        }
        else if (existeInventario)
        {
            CargarInventario(LoginSQLController.idUsuarioIntroducido);
        }

    }
    public void AddInventario()
    {
        Inventario inventario = new Inventario { IdInventario = LoginSQLController.idUsuarioIntroducido, IdUsuario = LoginSQLController.idUsuarioIntroducido};
        dbConnection.Insert(inventario);
    }
    public void AddFruta(int id, string nombreFruta, int precio)
    {
        Objeto objeto = new Objeto { idObjeto = id, NombreProducto = nombreFruta, PrecioProducto = precio};
        dbConnection.Insert(objeto);
        dbConnection.CreateCommand("INSERT INTO InventarioObjeto (idInventario, idObjeto, cantidad) VALUES (" + LoginSQLController.idUsuarioIntroducido +
        ", "+ objeto.idObjeto + ", " + 0 + ");").ExecuteNonQuery();
    }
    public void CargarInventario(int idInventario)
    {
        var invetarios = dbConnection.Table<Inventario>();
        comandoDevolverDinero = new SQLiteCommand(dbConnection);
        comandoDevolverDinero.CommandText = "SELECT DineroDisponible FROM Usuario WHERE (Usuario.Id = " + idInventario + ");";
        numDinero.text = comandoDevolverDinero.ExecuteScalar<int>().ToString();
        foreach (var inventario in invetarios)
        {
            if (inventario.IdUsuario == idInventario)
            {
                comandoCantidadManzanas = new SQLiteCommand(dbConnection);
                comandoCantidadManzanas.CommandText = "SELECT cantidad FROM InventarioObjeto, Objeto WHERE (InventarioObjeto.idObjeto = Objeto.idObjeto AND Objeto.idObjeto = 1 AND idInventario = " + idInventario+");";

                int numManzanasNum = comandoCantidadManzanas.ExecuteScalar<int>();
                numManzanas.text = numManzanasNum.ToString();

                comandoCantidadNaranjas = new SQLiteCommand(dbConnection);
                comandoCantidadNaranjas.CommandText = "SELECT cantidad FROM InventarioObjeto, Objeto WHERE (InventarioObjeto.idObjeto = Objeto.idObjeto AND Objeto.idObjeto = 2 AND idInventario = " +
                    idInventario+");";
                int numNaranjasNum = comandoCantidadNaranjas.ExecuteScalar<int>();
                numNaranjas.text = numNaranjasNum.ToString();

                comandoCantidadPiñas = new SQLiteCommand(dbConnection);
                comandoCantidadPiñas.CommandText = "SELECT cantidad FROM InventarioObjeto, Objeto WHERE (InventarioObjeto.idObjeto = Objeto.idObjeto AND Objeto.idObjeto = 3 AND idInventario = " +
                    idInventario + ");";
                int numPiñasNum = comandoCantidadPiñas.ExecuteScalar<int>();
                numPiñas.text = numPiñasNum.ToString();

                comandoCantidadMelocotones = new SQLiteCommand(dbConnection);   
                comandoCantidadMelocotones.CommandText = "SELECT cantidad FROM InventarioObjeto, Objeto WHERE (InventarioObjeto.idObjeto = Objeto.idObjeto AND Objeto.idObjeto = 4 AND idInventario = " +
                    idInventario + ");";
                int numMelocotonesNum = comandoCantidadMelocotones.ExecuteScalar<int>();
                numMelocotones.text = numMelocotonesNum.ToString();
            }
        }
        idInventarioActual = idInventario;
    }
    public void AccederAElegirCantidadProducto(int numProducto)
    {
        panelSeleccionCantidad.SetActive(true);
        if (numProducto == 1) 
        {
            tipoDeProducto.text = "Manzanas";

        }else if (numProducto == 2)
        {
            tipoDeProducto.text = "Naranjas";
        }
        else if (numProducto == 3)
        {
            tipoDeProducto.text = "Piñas";
        }
        else if (numProducto == 4)
        {
            tipoDeProducto.text = "Melocotones";
        }
        frutaSeleccionadaActual = numProducto;
    }
    public void SeguirConCompra()
    {
        
        
        if (string.IsNullOrEmpty(cantidadDeseada.text))
        {
            Debug.LogError("Error");
            cantidadDeFrutaSeleccionada = 0;   
        }
        else
        {
            cantidadDeFrutaSeleccionada = int.Parse(cantidadDeseada.text); 
        }
        panelSeleccionCantidad.SetActive(false);
        panelConfirmacionCompra.SetActive(true);
        comandoExtraerPrecio = new SQLiteCommand(dbConnection);
        comandoExtraerPrecio.CommandText = "SELECT PrecioProducto FROM Objeto WHERE idObjeto = " + frutaSeleccionadaActual + ";";
        precioDeFrutaSeleccionada = comandoExtraerPrecio.ExecuteScalar<int>();
        precioAPagarNum = precioDeFrutaSeleccionada * cantidadDeFrutaSeleccionada;
        precioAPagar.text = precioAPagarNum.ToString();

    }
    public void RealizarCompra()
    {
        try
        {
            //

            dbConnection.BeginTransaction(); 
             SQLiteCommand comandoAñadirCantidad = new SQLiteCommand(dbConnection);
            comandoAñadirCantidad.CommandText = "UPDATE InventarioObjeto SET cantidad = cantidad + " + cantidadDeFrutaSeleccionada +
               " WHERE  (InventarioObjeto.idObjeto = " + frutaSeleccionadaActual + " AND idInventario = " + idInventarioActual + ");";
            comandoAñadirCantidad.ExecuteNonQuery();

            dbConnection.SaveTransactionPoint();
            SQLiteCommand comandoPagarDinero = new SQLiteCommand(dbConnection); 
            comandoPagarDinero.CommandText = "UPDATE Usuario SET DineroDisponible = DineroDisponible - " + precioAPagarNum + " WHERE (Usuario.Id = "+ idInventarioActual + ");";
            comandoPagarDinero.ExecuteNonQuery();
            //transactionCompra.Commit();
            dbConnection.Commit();


        }
        catch (Exception ex) 
        {

            dbConnection.Rollback();
            //transactionCompra.Rollback();
            Debug.LogError("Transaccion Fallida");
        }
        finally
        {
            panelConfirmacionCompra.SetActive(false);
            comandoDevolverDinero = new SQLiteCommand(dbConnection);
            comandoDevolverDinero.CommandText = "SELECT DineroDisponible FROM Usuario WHERE (Usuario.Id = " + idInventarioActual + ");";
            numDinero.text = comandoDevolverDinero.ExecuteScalar<int>().ToString();
        }
    }
}
