using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;


public class FieldTypeHandler : NetworkBehaviour
{

    private FieldType localType;

    // Start is called before the first frame update
    void Start()
    {
        //localType = FieldType.White;
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void SetFieldTypeRpc(FieldType type)
    {
        localType = type;
        Debug.Log("Field type set to " + type);
    }

    public FieldType ReturnFieldType()
        { return localType; }

    public enum FieldType 
    {
        White,
        Red
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
