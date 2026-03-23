using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SQLite4Unity3d;

public class Objeto 
{
    [PrimaryKey] public int idObjeto { get; set; }
    public string NombreProducto { get; set; }
    public int PrecioProducto { get; set; }
}
