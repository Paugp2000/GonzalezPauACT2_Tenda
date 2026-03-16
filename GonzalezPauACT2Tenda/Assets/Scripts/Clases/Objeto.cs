using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SQLite4Unity3d;

public class Objeto 
{
    [PrimaryKey, AutoIncrement] public int IdInventario { get; set; }
    public string NombreProducto { get; set; }
    public int PrecioProducto { get; set; }
    public int cantidadDeProducto { get; set; }
}
