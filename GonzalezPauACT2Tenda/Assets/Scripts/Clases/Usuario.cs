
using System.Collections;
using System.Collections.Generic;
using SQLite4Unity3d;
using UnityEngine;

public class Usuario
{

    [PrimaryKey, AutoIncrement] public int Id { get; set; }
    public string NombreUsuario { get; set; }
    public string Contraseña { get; set; }
    public float DineroDisponible { get; set; } 

    public override string ToString()
    {
        return string.Format("[Usuario: Id={0}, NombreUsuario={1}, Contraseña={2}, DineroDisponible={3})", Id, NombreUsuario, Contraseña, DineroDisponible);
    }
}