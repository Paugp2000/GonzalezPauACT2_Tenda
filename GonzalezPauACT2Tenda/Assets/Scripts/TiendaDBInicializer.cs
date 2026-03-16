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
    public string[] nombresFrutas = { "Manzana", "Naranja", "Piña", "Melocoton"};
    public int[] preciosFrutas = { 30, 25, 50, 65 };
    private bool existeInventario = false;

    public TextMeshProUGUI numManzanas;
    public TextMeshProUGUI numNaranjas;
    public TextMeshProUGUI numPiñas;
    public TextMeshProUGUI numMelocotones;

    public Inventario inventarioCargado;
    private void Start()
    {
        CargarDatabase();
        CargarInventario(LoginSQLController.idUsuarioIntroducido);
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
            dbConnection.CreateTable<Objeto>();
            
            for (int i = 0; i < nombresFrutas.Length; i++)
            {
                AddFruta(nombresFrutas[i], preciosFrutas[i], 0);
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
        Inventario inventario = new Inventario { IdUsuario = LoginSQLController.idUsuarioIntroducido, ObjetoList = returnListFrutas() };
        dbConnection.Insert(inventario);    
    }
    public List<Objeto> returnListFrutas()
    {
        List<Objeto> listaAAñadir = new List<Objeto>();
        var frutas = dbConnection.Table<Objeto>();
        foreach (Objeto objeto in frutas)
        {
            listaAAñadir.Add(objeto);
        }
        return listaAAñadir;
    }
    public void AddFruta(string nombreFruta, int precio, int cantidad)
    {
        Objeto objeto = new Objeto {NombreProducto = nombreFruta, PrecioProducto = precio, cantidadDeProducto = cantidad};
        dbConnection.Insert(objeto);
    }
    public void CargarInventario(int idInventario)
    {
        var invetarios = dbConnection.Table<Inventario>();
        foreach (var inventario in invetarios)
        {
            if (inventario.IdUsuario == idInventario)
            {

            }
        }
    }
}
