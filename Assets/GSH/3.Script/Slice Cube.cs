using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SliceCube : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.name == "Slice_Obj")
        {
            GameObject Piece_Obj_1 = Instantiate(this.gameObject, transform.position, Quaternion.identity);
            GameObject Piece_Obj_2 = Instantiate(this.gameObject, transform.position, Quaternion.identity);

            Piece_Obj_1.transform.position = new Vector3(Piece_Obj_1.transform.position.x, Piece_Obj_1.transform.position.y, Piece_Obj_1.transform.position.z + 1f);
            Piece_Obj_2.transform.position = new Vector3(Piece_Obj_2.transform.position.x, Piece_Obj_2.transform.position.y, Piece_Obj_2.transform.position.z - 1f); 

            Piece_Obj_1.transform.localScale = new Vector3(Piece_Obj_1.transform.localScale.x, Piece_Obj_1.transform.localScale.y, Piece_Obj_1.transform.localScale.z * 0.5f);
            Piece_Obj_2.transform.localScale = new Vector3(Piece_Obj_2.transform.localScale.x, Piece_Obj_2.transform.localScale.y, Piece_Obj_2.transform.localScale.z * 0.5f);

            Destroy(this.gameObject);
        }
    }
}
