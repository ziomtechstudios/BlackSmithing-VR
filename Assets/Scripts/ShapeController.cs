using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class ShapeController : MonoBehaviour
{
    ///<summary>
    /// Folowing code based on Mesh Deformer script created following blog in following link
    /// https://catlikecoding.com/unity/tutorials/mesh-deformation/
    ///</summary>

    #region "Private serialized fields"
    [SerializeField] private Mesh mbMesh; 
    [SerializeField] private MetalStruct mbStruct;
    [SerializeField] private float elongScale1, elongScale2, compScale;  
    [SerializeField] private bool isParallel;
    [SerializeField] private Rigidbody mbRB;
    #endregion
    #region private members
    private Vector3[] ogVerts, changedVerts, vertVelocities;
    #endregion
    #region "Private funcs"
    private bool CompDir(float elong1, float elong2, float comp1){
        elongScale1 = elong1;
        elongScale2 = elong2;
        compScale = comp1;
        return true;
    }
    #endregion
    void Start(){
        mbMesh = GetComponent<MeshFilter>().mesh;
        mbRB = GetComponent<Rigidbody>();
        ogVerts = mbMesh.vertices;
        changedVerts = new Vector3[ogVerts.Length];
        vertVelocities =  new Vector3[ogVerts.Length];
        foreach(Vector3 vert in ogVerts)
            changedVerts[System.Array.IndexOf(ogVerts, vert)] = ogVerts[System.Array.IndexOf(ogVerts, vert)];
    }
    //Changing shape of obj if deemed appropriate
    public void ChangeShape(float stress, RaycastHit hit){
        Debug.Log($"{transform.name} is changing shape.");
        ///<summary>
        /// Accessing vertices of mb prior to change in shape
        /// Copy stored init in array used to store final verts
        /// Change of position in each vert stored is verVelocity
        ///</summary>
        ///<summary>
        ///If any of the obj's axis is || to the normal of the hammer heads surface isParallel set to true +
        ///Appropriate axis in which the obj overall is elongated and compressed are identified
        ///Using calculated
        ///</summary>
        isParallel = (Vector3.Cross(transform.forward, hit.transform.forward) == Vector3.zero)?CompDir(transform.localScale.x, transform.localScale.y, transform.localScale.z):
        (Vector3.Cross(transform.right, hit.transform.forward) == Vector3.zero)?CompDir(transform.localScale.z, transform.localScale.y, transform.localScale.x):
        (Vector3.Cross(transform.up, hit.transform.forward) == Vector3.zero)?CompDir(transform.localScale.z, transform.localScale.x, transform.localScale.y):false;
        if(isParallel){
            // dl = stress * original length / Modulus of Elasticity
            //In future adding temp of obj to take into consideration
            elongScale1 += (stress*elongScale1)/mbStruct.youngsModulus;
            elongScale2 += (stress*elongScale2)/mbStruct.youngsModulus;
            compScale -= (stress*compScale)/mbStruct.youngsModulus;
            //Vector3 point = hit.point;
            //point =+ (hit.normal * stress);
        }

    }
    public void ApplySurfaceDeform(Vector3 point, float force){
        foreach(Vector3 changedVert in changedVerts)
            AddForceToVertex(System.Array.IndexOf(changedVerts,changedVert), point, force);
    }
    public void AddForceToVertex(int i, Vector3 point, float force){
        //Vector3 representing direction and distance off the deformation force
        Vector3 forceToVertex = changedVerts[i] - point;
        float forceMagnitude = force/(1.0f + forceToVertex.sqrMagnitude);
        
    }
}

