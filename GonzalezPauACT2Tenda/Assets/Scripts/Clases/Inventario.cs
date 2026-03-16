using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SQLite4Unity3d;

public class Inventario 
{
    [PrimaryKey] public int IdInventario { get; set; }
    public int IdUsuario { get; set; }
    //public Objeto[] ObjetoList { get; set; }
    public Objeto objeto1;
    public Objeto objeto2;
    public Objeto objeto3;
    public Objeto objeto4;

}
