using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using TMPro;
using UnityEngine;
using SQLite4Unity3d;

using Mono.Data.Sqlite;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using System;

public class LoginSQLController : MonoBehaviour
{
    private string dbUri;
    private string _dbPath;
    private string _streamingPath;
    public static int idUsuario;
    private SQLiteConnection dbConnection;



    [SerializeField] TMP_InputField nombreUsuario;
    [SerializeField] TMP_InputField contraseña;
    [SerializeField] GameObject panelErrorUsuario;
    [SerializeField] GameObject panelErrorContraseña;
    [SerializeField] GameObject panelErrorConPequeña;
    [SerializeField] GameObject panelMostrarUsuarios;
    [SerializeField] TextMeshProUGUI textoMostrarUsuarios;
    [SerializeField] GameObject panelRegistrado;
    public static int idUsuarioIntroducido;
    public bool loginCorrecto = false;
    private float dineroInicial = 500;
    private void Start()
    {

        AssertFolder();
        loginCorrecto = false;
        StartDatabase();
        CreateDatabaseIfNecessary();
    }
    void OnDestroy()
    {
        if (dbConnection != null)
        {
            dbConnection.Close();
            dbConnection.Dispose();
            dbConnection = null;
        }
    }
    public void StartDatabase()
    {
        // 1. Definir rutas
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
    }

    void AssertFolder()
    {
        if (!Directory.Exists(CommonDB.dbFolderPath))
        {
            Directory.CreateDirectory(CommonDB.dbFolderPath);

        }
        File.Create(Application.streamingAssetsPath + "/MyDatabase.sqlite");
    }

    public void CreateDatabaseIfNecessary()
    {
        dbConnection.CreateTable<Usuario>();
    }
    public void PulsarBotonDelete()
    {
        if (string.IsNullOrEmpty(nombreUsuario.text))
        {
            panelErrorUsuario.SetActive(true);
            return;
        }
        foreach (Usuario usuario in dbConnection.Table<Usuario>())
        {
            if (nombreUsuario.text == usuario.NombreUsuario)
            {
                DeleteUsuario(usuario);
            }
        }
    }
    public void SaveUserNameAndPasswordIntoDatabase()
    {
        if (string.IsNullOrEmpty(nombreUsuario.text))
        {
            panelErrorUsuario.SetActive(true);
            return;
        }

        if (string.IsNullOrEmpty(contraseña.text))
        {
            panelErrorContraseña.SetActive(true);
            return;
        }

        if (contraseña.text.Length < 8)
        {
            panelErrorConPequeña.SetActive(true);
            return;
        }
        AddUsuario(nombreUsuario.text, contraseña.text, dineroInicial);
    }
    public void AddUsuario(string nombre, string contraseña, float dineroInicial)
    {
        Usuario usuario = new Usuario { NombreUsuario = nombre, Contraseña = contraseña, DineroDisponible = dineroInicial };
        dbConnection.Insert(usuario);
        Debug.Log("Usuario : " + usuario.NombreUsuario + " añadidio");
        panelRegistrado.SetActive(true);
    }

    public void DeleteUsuario(Usuario usuario)
    {
        dbConnection.Delete(usuario);
        Debug.Log("Usuario : " + usuario.NombreUsuario + " eliminado");
    }
    public void MostrarUsuarios()
    {
        List<string> listaDeUsuarios = new List<string>();
        var usuarios = dbConnection.Table<Usuario>();
        foreach (var usuario in usuarios)
        {
            panelMostrarUsuarios.SetActive(true);
            listaDeUsuarios.Add(usuario.NombreUsuario);

        }
        textoMostrarUsuarios.text = string.Join(" , ", listaDeUsuarios);
    }
    public void Login()
    {
        foreach (Usuario usuario in dbConnection.Table<Usuario>())
        {
            if (usuario.NombreUsuario == nombreUsuario.text && usuario.Contraseña == contraseña.text)
            {
                idUsuarioIntroducido = usuario.Id;
                SceneManager.LoadScene("Tienda");
                loginCorrecto = true;
            }
        }
        if (!loginCorrecto)
        {
            panelErrorContraseña.SetActive(true);
        }

    }
    public void DesactivarPanelError(GameObject panel) => panel.SetActive(false);
    public void Salir()
    {
        Application.Quit();
    }
}