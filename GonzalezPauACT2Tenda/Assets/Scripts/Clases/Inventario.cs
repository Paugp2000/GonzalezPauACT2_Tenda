using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SQLite4Unity3d;

public class Inventario 
{
    [PrimaryKey] public int IdInventario { get; set; }
    public int IdUsuario { get; set; }  
    public List<Objeto> ObjetoList { get; set; } = new List<Objeto>();


}
