using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using UnityEngine;
using SQLite4Unity3d;
using UnityEngine.UIElements;
using TMPro;

public class TiendaDBInicializer : MonoBehaviour
{
    private string _dbPath;
    private string _streamingPath;
    private SQLiteConnection dbConnection;
    public string[] nombresFrutas = { "Manzana", "Naranja", "Piña", "Melocoton" };
    public int[] preciosFrutas = { 30, 25, 50, 65 };
    private bool existeInventario = false;

    public TextMeshProUGUI numManzanas;
    public TextMeshProUGUI numNaranjas;
    public TextMeshProUGUI numPiñas;
    public TextMeshProUGUI numMelocotones;

    public Inventario inventarioCargado;
    private SQLiteCommand comandoCantidadManzanas;
    private SQLiteCommand comandoCantidadNaranjas;
    private SQLiteCommand comandoCantidadPiñas;
    private SQLiteCommand comandoCantidadMelocotones;

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
                          "cantidad INTEGER, " +
                          "PRIMARY KEY (idInventario, idObjeto)," +
                          "FOREIGN KEY (idInventario) REFERENCES Inventario(IdInventario)," +
                          "FOREIGN KEY (idObjeto) REFERENCES Objeto(idObjeto));").ExecuteNonQuery();    
            dbConnection.CreateTable<Objeto>();

            for (int i = 0; i < nombresFrutas.Length; i++)
            {
                AddFruta(nombresFrutas[i], preciosFrutas[i]);
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
        Inventario inventario = new Inventario { IdUsuario = LoginSQLController.idUsuarioIntroducido};
        dbConnection.Insert(inventario);
    }
    public void AddFruta(string nombreFruta, int precio)
    {
        Objeto objeto = new Objeto { NombreProducto = nombreFruta, PrecioProducto = precio};
        dbConnection.Insert(objeto);
        dbConnection.CreateCommand("INSERT INTO InventarioObjeto (idInventario, idObjeto, cantidad) VALUES (" + LoginSQLController.idUsuarioIntroducido +
        ", "+ objeto.idObjeto + ", " + 0 + ")").ExecuteNonQuery();
    }
    public void CargarInventario(int idInventario)
    {
        var invetarios = dbConnection.Table<Inventario>();
        foreach (var inventario in invetarios)
        {
            if (inventario.IdUsuario == idInventario)
            {
                comandoCantidadManzanas.CommandText = ("SELECT cantidad ");
                
            }
        }
    }
}
