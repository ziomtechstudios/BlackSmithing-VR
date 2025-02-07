using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Maleability : MonoBehaviour{
    #region "Private Serialized Fields"
    [SerializeField] private Rigidbody objRB;
    [SerializeField] private float areaofImpact;
    [SerializeField] private Vector3 initVelocity;
    #endregion
    private RaycastHit hit;
    private LayerMask mbMask;
    // Start is called before the first frame update
    void Start(){
        objRB = transform.parent.gameObject.GetComponent<Rigidbody>();
        initVelocity = objRB.linearVelocity;
        //Deriving surface area of for impact(s)
        areaofImpact = Mathf.PI * Mathf.Pow(transform.parent.GetComponent<SphereCollider>().radius, 2.0f);
        initVelocity = objRB.linearVelocity;
        mbMask = LayerMask.GetMask("Metal Bars");
    }
    void FixedUpdate(){
        initVelocity = objRB.linearVelocity;
        //if(Physics.Raycast(transform.position, transform.forward, 0.5f, mbMask))
            //hit.transform.gameObject.GetComponent<ShapeController>().ChangeShape(DeriveStress(), hit);
    }
    /*void OnCollisionEnter(Collision other){
        if(other.gameObject.CompareTag("Metal Bars")){
            Debug.Log($"{transform.name} colliding with {other.transform.name}.");
            other.gameObject.GetComponent<ShapeController>().ChangeShape(DeriveStress(), transform.forward);
        }
    }*/
    public float DeriveStress(){
        //strain = external force of hammer/area of impact
        return (objRB.mass*((objRB.linearVelocity-initVelocity)/Time.deltaTime).magnitude)/areaofImpact;
    }
}